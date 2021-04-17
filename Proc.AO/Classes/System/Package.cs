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
/// Install-Package MongoDb.Bson -Version 2.11.0
/// 

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

using MongoDB.Driver;
using MongoDB.Bson;

using System.IO;
using System.IO.Compression;


using NX.Shared;
using NX.Engine;

namespace Proc.AO
{
    public class PackageClass : ChildOfClass<DatabaseClass>
    {
        #region Constructor
        public PackageClass(DatabaseClass db, NX.Engine.Files.DocumentClass doc)
            : base(db)
        {
            //
            this.Document = doc;
            // 
            this.Document.Location.GetDirectoryFromPath().AssurePath();
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The path of the package
        /// 
        /// </summary>
        public NX.Engine.Files.DocumentClass Document { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Dumps multiple datasets into the package
        /// 
        /// </summary>
        /// <param name="dss">If + is part of the name, the data will also be packaged</param>
        public void DumpFromSystem(List<string> dss)
        {
            //
            NX.Engine.Files.ManagerClass c_DocMgr = this.Parent.Parent.Parent.Globals.Get<NX.Engine.Files.ManagerClass>();

            using (FileStream c_ZStream = new FileStream(this.Document.Location, FileMode.Create))
            {
                using (ZipArchive c_Pkg = new ZipArchive(c_ZStream, ZipArchiveMode.Update))
                {
                    // Loop thru
                    foreach (string sDSRaw in dss)
                    {
                        // Settings
                        if (sDSRaw.StartsWith("^"))
                        {
                            // All?
                            if (sDSRaw.IsSameValue("^^"))
                            {
                                // Make list
                                List<string> c_TBD = "acctenabled acctdefallowed billenabled ttenabled teleenabled quorumenabled iotenabled helproot".SplitSpaces();
                                // Loop thru
                                foreach(string sKey in c_TBD)
                                {
                                    // Get the value
                                    string sValue = this.Parent.SiteInfo[sKey];

                                    // Add
                                    this.AddEntry(c_Pkg, "^" + sKey, sValue);
                                }
                            }
                            else
                            {
                                // Split
                                string sValue = "";
                                string sKey = sDSRaw;
                                int iPos = sKey.IndexOf("-");
                                if (iPos != -1)
                                {
                                    sValue = sKey.Substring(iPos + 1);
                                    sKey = sKey.Substring(0, iPos);
                                }
                                // Add
                                this.AddEntry(c_Pkg, sKey, sValue);
                            }
                        }
                        else
                        {
                            // Parse
                            bool bIncludeData = sDSRaw.Contains("+");
                            string sDS = sDSRaw.Replace("+", "");

                            // Map the dataset
                            DatasetClass c_DS = this.Parent[sDS];

                            // Do
                            this.CopyCollection(c_Pkg, c_DS.SettingsCollection);

                            // Make the templates folder
                            using (NX.Engine.Files.FolderClass c_Templates = new NX.Engine.Files.FolderClass(c_DocMgr, "/ao/{0}/templates".FormatString(sDS)))
                            {
                                // Add
                                this.AddDocuments(c_Pkg, c_Templates);
                            }

                            //
                            if (bIncludeData)
                            {
                                // Copy the data
                                this.CopyCollection(c_Pkg, c_DS.DataCollection, c_DocMgr);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// Copies a collection into a package
        /// 
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="coll"></param>
        private void CopyCollection(ZipArchive pkg, CollectionClass coll, NX.Engine.Files.ManagerClass docmgr = null)
        {
            // Open a query
            using (QueryClass c_Qry = new QueryClass(coll))
            {
                // Loop thru
                foreach (BsonDocument c_Obj in c_Qry.Find(sort:"_id"))
                {
                    //
                    string sTarget = UUIDClass.MakeString(coll.Parent.Name, c_Obj.GetField("_id"));

                    // Add
                    this.AddEntry(pkg, sTarget, c_Obj.ToJObject().ToSimpleString());

                    // Include docs?
                    if (docmgr != null)
                    {
                        // Gte path
                        using (NX.Engine.Files.FolderClass c_Folder = new NX.Engine.Files.FolderClass(docmgr, "/ao/{0}/{1}".FormatString(coll.Parent.Name, c_Obj.GetField("_id"))))
                        {
                            // Do
                            this.AddDocuments(pkg, c_Folder);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="folder"></param>
        private void AddDocuments(ZipArchive pkg, NX.Engine.Files.FolderClass folder)
        {
            // Get all the files
            List<NX.Engine.Files.DocumentClass> c_Files = folder.Files;
            // Loop thru
            {
                foreach (NX.Engine.Files.DocumentClass c_Doc in c_Files)
                {
                    // Make the ID
                    string sID = "!" + c_Doc.Path.MD5HashString();
                    // Make the payload
                    JObject c_Info = new JObject();
                    c_Info.Set("path", c_Doc.Path);
                    c_Info.Set("content", c_Doc.ValueAsBytes.ToBase64());
                    // Save
                    this.AddEntry(pkg, sID, c_Info.ToSimpleString());
                }
            }

            // Get directories
            List<NX.Engine.Files.FolderClass> c_Folders = folder.Folders;
            // Loop thru
            foreach (NX.Engine.Files.FolderClass c_Folder in c_Folders)
            {
                // Do
                this.AddDocuments(pkg, c_Folder);
            }
        }

        /// <summary>
        /// 
        /// Adds an entry in the package
        /// 
        /// </summary>
        /// <param name="pkg"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        private void AddEntry(ZipArchive pkg, string name, string value)
        {
            ZipArchiveEntry c_Entry = pkg.CreateEntry(name);

            using (MemoryStream c_Source = new MemoryStream(value.ToBytes()))
            {
                using (Stream c_Target = c_Entry.Open())
                {
                    c_Source.CopyTo(c_Target);
                }
            }
        }

        /// <summary>
        /// 
        /// Loads a package into the system
        /// 
        /// </summary>
        public void LoadIntoSystem()
        {
            //
            NX.Engine.Files.ManagerClass c_DocMgr = this.Parent.Parent.Parent.Globals.Get<NX.Engine.Files.ManagerClass>();

            using (ZipArchive c_Pkg = ZipFile.OpenRead(this.Document.Location))
            {
                // Disable messaging
                SIO.ManagerClass c_SIO = this.Parent.Parent.Parent.Globals.Get<SIO.ManagerClass>();
                // Turn off messaging
                c_SIO.Enabled = false;

                // Starting dataset
                string sCDS = null;
                // No changes to settings
                bool bSettChanged = false;

                // List of changed datasets
                List<string> c_Changed = new List<string>();

                foreach (ZipArchiveEntry c_Entry in c_Pkg.Entries)
                {
                    // Get the name
                    string sUUID = c_Entry.FullName;

                    // Handle settings
                    if (sUUID.StartsWith("^"))
                    {
                        // Get the value
                        string sValue = this.GetEntry(c_Entry);
                        // Save
                        this.Parent.SiteInfo.Set(sUUID.Substring(1), sValue);
                        bSettChanged = true;
                    }
                    else if (sUUID.StartsWith("!"))
                    {
                        // TBD
                        // Get the value
                        string sValue = this.GetEntry(c_Entry);
                        // Parse
                        JObject c_Info = sValue.ToJObject();
                        // Valid?
                        if (c_Info != null && c_Info.Get("path").HasValue())
                        {
                            // Save
                            using (NX.Engine.Files.DocumentClass c_Doc = new NX.Engine.Files.DocumentClass(c_DocMgr, c_Info.Get("path")))
                            {
                                c_Doc.Folder.AssurePath();
                                c_Doc.ValueAsBytes = c_Info.Get("content").FromBase64Bytes();
                            }
                        }
                    }
                    else
                    {
                        if (UUIDClass.IsValid(sUUID))
                        {
                            using (UUIDClass c_UUID = new UUIDClass(this.Parent, sUUID))
                            {
                                // Did the dataset change?
                                if (!sCDS.IsSameValue(c_UUID.Dataset.Name))
                                {
                                    // Clear cache
                                    if (sCDS.HasValue()) this.DatasetChange(sCDS, c_Changed);
                                    // Save
                                    sCDS = c_UUID.Dataset.Name;
                                }

                                // Copy
                                using (Stream c_Source = c_Entry.Open())
                                {
                                    using (MemoryStream c_Target = new MemoryStream())
                                    {
                                        c_Source.CopyTo(c_Target);

                                        // Get body
                                        string sDoc = c_Target.ToArray().FromBytes();
                                        // Convert to JSON
                                        JObject c_Orig = sDoc.ToJObject();
                                        // 
                                        if(c_Orig.Get("_id").IsSameValue("#def"))
                                        {
                                            // Get the fields
                                            JObject c_Fields = c_Orig.GetJObject("fields");
                                            // Remove empty field
                                            c_Fields.Remove("");
                                            // Set
                                            c_Orig.Set("fields", c_Fields);
                                            // And back
                                            sDoc = c_Orig.ToSimpleString();
                                        }

                                        // Is it a setting?
                                        if (c_UUID.ID.StartsWith("#"))
                                        {
                                            c_UUID.Dataset.SettingsCollection.AddDirect(sDoc);
                                        }
                                        else
                                        {
                                            c_UUID.Dataset.DataCollection.AddDirect(sDoc);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Clear cache
                //if (sCDS.HasValue()) this.DatasetChange(sCDS, c_Changed);

                // Turn on messaging
                c_SIO.Enabled = true;

                this.Parent.RebuildCache();

                //// Loop thru
                //foreach (string sDS in c_Changed)
                //{
                //    // Map
                //    DatasetClass c_DS = this.Parent[sDS];
                //    // Reload
                //    c_DS.Definition.Reload();
                //    // Tell world
                //    this.Parent.Parent.SignalChange(c_DS);
                //}

                // Save settings
                if (bSettChanged)
                {
                    this.Parent.SiteInfo.Save();
                }

                // Housekeeping
                this.Parent.SiteInfo.Reload();
            }
        }

        /// <summary>
        /// 
        /// Get the contents of an entry
        /// 
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        private string GetEntry(ZipArchiveEntry entry)
        {
            // Copy
            using (Stream c_Source = entry.Open())
            {
                using (MemoryStream c_Target = new MemoryStream())
                {
                    c_Source.CopyTo(c_Target);

                    // Get body
                    return c_Target.ToArray().FromBytes();
                }
            }
        }

        /// <summary>
        /// 
        /// Handles updating the dataset
        /// 
        /// </summary>
        /// <param name="ds"></param>
        private void DatasetChange(string ds, List<string> changed)
        {
            if (ds.HasValue())
            {
                if (!changed.Contains(ds)) changed.Add(ds);
            }
        }
        #endregion
    }
}