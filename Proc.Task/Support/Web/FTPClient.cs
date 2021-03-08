///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com) - All Rights Reserved
/// 
/// This work is covered by GPL v3 as defined in https://www.gnu.org/licenses/gpl-3.0.en.html
/// 
/// The above copyright notice and this permission notice shall be included in all
/// copies or substantial portions of the Software.
/// 
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
/// SOFTWARE.
/// 
///--------------------------------------------------------------------------------

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Linq;

using Renci.SshNet;
using Renci.SshNet.Sftp;
using FluentFTP;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class FTPClientClass : IDisposable
    {
        #region Constructor
        public FTPClientClass()
        { }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            this.Close();

            if (this.FTPServer != null)
            {
                this.FTPServer.Dispose();
                this.FTPServer = null;
            }

            if (this.SFTPServer != null)
            {
                this.SFTPServer.Dispose();
                this.SFTPServer = null;
            }
        }
        #endregion

        #region Properties
        private SftpClient SFTPServer { get; set; }
        private FluentFTP.FtpClient FTPServer { get; set; }

        public string CurrentDirectory { get; internal set; } = "/";
        public string LastError { get; set; }

        public string ServerURL { get; internal set; }
        public string ServerUserName { get; internal set; }
        #endregion

        #region Methods
        public override string ToString()
        {
            string sAns = "";

            if (this.SFTPServer != null) sAns += "SFTP: ";
            if (this.FTPServer != null) sAns += "FTP: ";

            if (!sAns.HasValue()) sAns += "NO SERVER: ";

            sAns += "Server: {0}".FormatString(this.ServerURL);
            sAns += ", Name: {0}".FormatString(this.ServerUserName);

            return sAns;
        }

        public bool Open(TaskContextClass ctx,
                            string url,
                            int port,
                            List<AO.SSHKeyClass> certs,
                            string name,
                            string pwd,
                            bool issftp)
        {
            bool bAns = false;

            this.Close();

            this.LastError = "";
            this.ServerURL = url;
            this.ServerUserName = name;

            try
            {
                if (!issftp)
                {
                    this.FTPServer = FtpClient.Connect(new Uri(url));
                    bAns = true;
                }
                else
                {
                    List<AuthenticationMethod> c_Mthds = new List<AuthenticationMethod>();

                    //string sCerts = "";
                    if (certs != null && certs.Count > 0)
                    {
                        //sCerts = "with certs ";
                        foreach (AO.SSHKeyClass c_Key in certs)
                        {
                            AO.PGPKeysClass c_Cert = c_Key.AsPGPKey;

                            PrivateKeyFile privateKeyFile = null;
                            if (c_Cert.Password.HasValue())
                            {
                                privateKeyFile = new PrivateKeyFile(c_Cert.PrivateKey.AsStream, c_Cert.Password);
                            }
                            else
                            {
                                privateKeyFile = new PrivateKeyFile(c_Cert.PrivateKey.AsStream);
                            }

                            // Add the cert
                            PrivateKeyAuthenticationMethod privateKeyAuthenticationMethod = new PrivateKeyAuthenticationMethod(name, privateKeyFile);
                            c_Mthds.Add(privateKeyAuthenticationMethod);
                        }
                    }

                    // Password is failsafe
                    c_Mthds.Add(new PasswordAuthenticationMethod(name, pwd));

                    //env.LogInfo("Attempting to log in to {2}:{3} {4} + {0}:{1}".FormatString(name, pwd, url, port, sCerts));

                    ConnectionInfo c_Info = new ConnectionInfo(url, port, name, c_Mthds.ToArray());
                    this.SFTPServer = new SftpClient(c_Info);
                    this.SFTPServer.Connect();

                    bAns = true;
                }
            }
            catch (Exception e)
            {
                this.LastError = e.GetAllExceptions();
            }

            if (!bAns)
            {
                this.Close();
            }

            ctx.Parent.LogInfo((issftp ? "SFTP" : "FTP") + " Connection being attempted");
            ctx.Parent.LogInfo("Server: {0}".FormatString(url));
            ctx.Parent.LogInfo("Port: {0}".FormatString(port));
            ctx.Parent.LogInfo("User Name: {0}".FormatString(name));

            string sMsg = "Successful";
            if (!bAns)
            {
                sMsg += "Failed - " + this.LastError;
            }

            ctx.Parent.LogInfo("Connection: {0}".FormatString(sMsg));

            this.CurrentDirectory = "/";

            return bAns;
        }

        public void Close()
        {
            try
            {
                if (this.FTPServer != null) this.FTPServer.Disconnect();
                if (this.SFTPServer != null) this.SFTPServer.Disconnect();
            }
            catch { }
            finally
            {
                if (this.FTPServer != null)
                {
                    this.FTPServer.Dispose();
                    this.FTPServer = null;
                }

                if (this.SFTPServer != null)
                {
                    this.SFTPServer.Dispose();
                    this.SFTPServer = null;
                }
            }
        }

        private bool CreateDirectory()
        {
            bool bAns = true;

            try
            {
                if (this.FTPServer != null)
                {
                    this.FTPServer.CreateDirectory(this.CurrentDirectory);
                }
                else if (this.SFTPServer != null)
                {
                    this.SFTPServer.CreateDirectory(this.CurrentDirectory);
                }

                bAns = true;
            }
            catch { }

            return bAns;
        }

        public bool MoveTo(string dir = null)
        {
            if (!dir.HasValue() || dir.IsSameValue("."))
            { }
            else if (dir.StartsWith("/"))
            {
                this.CurrentDirectory = dir;
            }
            else
            {
                this.CurrentDirectory += (!this.CurrentDirectory.EndsWith("/") ? "/" : "") + dir;
            }

            return true;
        }

        public bool MoveRoot()
        {
            return this.MoveTo("/");
        }

        public bool Delete(string serverpath)
        {
            bool bAns = false;

            List<string> c_Parsed = this.ParseDirectory(serverpath);

            this.MoveTo(c_Parsed[0]);

            try
            {
                if (this.FTPServer != null)
                {
                    this.FTPServer.DeleteFile(c_Parsed[1]);
                }
                else if (this.SFTPServer != null)
                {
                    this.SFTPServer.DeleteFile(c_Parsed[0].CombinePath(c_Parsed[1]));
                }

                bAns = true;
            }
            catch { }

            this.MoveRoot();

            return bAns;
        }

        public bool Upload(string serverpath, string localpath)
        {
            bool bAns = false;

            string sAt = this.CurrentDirectory;

            List<string> c_Parsed = this.ParseDirectory(serverpath);

            this.MoveTo(c_Parsed[0]);

            try
            {
                this.CreateDirectory();

                if (this.FTPServer != null)
                {
                    bAns = this.FTPServer.UploadFile(localpath, c_Parsed[1]) == FtpStatus.Success;
                }
                else if (this.SFTPServer != null)
                {
                    using (MemoryStream c_Stream = new MemoryStream(localpath.ReadFileAsBytes()))
                    {
                        this.SFTPServer.UploadFile(c_Stream, c_Parsed[0].CombinePath(c_Parsed[1]));
                    }

                    bAns = true;
                }
            }
            catch { }

            this.MoveTo(sAt);

            return bAns;
        }

        public bool Upload(string serverpath, byte[] data)
        {
            bool bAns = false;

            string sAt = this.CurrentDirectory;

            List<string> c_Parsed = this.ParseDirectory(serverpath);

            this.MoveTo(c_Parsed[0]);

            try
            {
                this.CreateDirectory();

                if (this.FTPServer != null)
                {
                    bAns = this.FTPServer.Upload(data, c_Parsed[1]) == FtpStatus.Success;
                }
                else if (this.SFTPServer != null)
                {
                    this.SFTPServer.WriteAllBytes(c_Parsed[0].CombinePath(c_Parsed[1]), data);

                    bAns = true;
                }
            }
            catch { }

            this.MoveTo(sAt);

            return bAns;
        }

        public bool Download(string serverpath, string localpath)
        {
            bool bAns = false;

            string sAt = this.CurrentDirectory;

            List<string> c_Parsed = this.ParseDirectory(serverpath);

            this.MoveTo(c_Parsed[0]);

            try
            {
                if (this.FTPServer != null)
                {
                    bAns = this.FTPServer.DownloadFile(c_Parsed[1], localpath) == FtpStatus.Success;
                }
                else if (this.SFTPServer != null)
                {
                    using (MemoryStream c_Stream = new MemoryStream())
                    {
                        this.SFTPServer.DownloadFile(c_Parsed[0].CombinePath(c_Parsed[1]), c_Stream);
                        localpath.WriteFileAsBytes(c_Stream.ToArray());
                    }

                    bAns = true;
                }
            }
            catch { }

            this.MoveTo(sAt);

            return bAns;
        }

        private List<string> ParseDirectory(string path, bool isdir = false)
        {
            List<string> c_Ans = new List<string>();

            path = path.IfEmpty();

            if (isdir)
            {
                c_Ans.Add(this.CurrentDirectory.CombinePath(path));
                c_Ans.Add("");
            }
            else
            {
                int iPos = path.LastIndexOf("/");
                if (iPos != -1)
                {
                    c_Ans.Add(this.CurrentDirectory.CombinePath(path.Substring(0, iPos)));
                    c_Ans.Add(path.Substring(iPos + 1));
                }
                else
                {
                    c_Ans.Add(this.CurrentDirectory);
                    c_Ans.Add(path);
                }
            }

            return c_Ans;
        }

        public List<string> Files(string serverpath)
        {
            List<string> c_Ans = new List<string>();

            string sAt = this.CurrentDirectory;

            List<string> c_Parsed = this.ParseDirectory(serverpath, true);

            this.MoveTo(c_Parsed[0]);

            try
            {
                if (this.FTPServer != null)
                {
                    FtpListItem[] c_Items = this.FTPServer.GetListing(this.CurrentDirectory);

                    for (int i = 0; i < c_Items.Length; i++)
                    {
                        FtpListItem c_Item = c_Items[i];

                        if (c_Item.Type == FtpFileSystemObjectType.File && !c_Item.Name.StartsWith("."))
                        {
                            c_Ans.Add(this.CurrentDirectory.CombinePath(c_Item.Name));
                        }
                    }
                }
                else if (this.SFTPServer != null)
                {
                    IEnumerable<SftpFile> c_Items = this.SFTPServer.ListDirectory(serverpath);
                    foreach (SftpFile c_File in c_Items)
                    {
                        if (!c_File.Name.StartsWith("."))
                        {
                            c_Ans.Add(serverpath.CombinePath(c_File.Name));
                        }
                    }
                }
            }
            catch { }

            this.MoveTo(sAt);

            return c_Ans;
        }
        #endregion
    }
}
