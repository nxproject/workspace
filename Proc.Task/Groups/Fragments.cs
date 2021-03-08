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
using System.Text;

using Newtonsoft.Json.Linq;
using HtmlAgilityPack;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class HTMLTextClass : IDisposable
    {
        #region Constants
        //private const string KeyParaStart = "<div>";
        //private const string KeyParaEnd = "</div>";
        //private const string KeyParaSplit = "<br>";
        #endregion

        #region Constructor
        public HTMLTextClass()
            : this(null)
        { }

        public HTMLTextClass(Action<string> cb)
        {
            //
            this.Callback = cb;
        }

        public HTMLTextClass(string value, Action<string> cb)
            : this(cb)
        {
            //
            this.Parse(value);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        private HtmlDocument IDocument { get; set; }
        private HtmlDocument Document
        {
            get
            {
                if (this.IDocument == null) this.IDocument = new HtmlDocument();

                return this.IDocument;
            }
        }

        private Action<string> Callback { get; set; }
        public HtmlNode RootNode { get { return this.Document.DocumentNode; } }
        public List<string> DefaultQualifiers { get; set; }

        public int RowCount { get { return this.RootNode.ChildNodes.Count; } }
        #endregion

        #region Methods
        public HtmlNode NewNode()
        {
            return HtmlNode.CreateNode("<p></p>");
        }

        public override string ToString()
        {
            return this.RootNode.OuterHtml;
            //string sAns = null;

            //try
            //{
            //    using (MemoryStream c_Stream = new MemoryStream())
            //    {
            //        this.Document.Save(c_Stream);
            //        sAns = c_Stream.ToArray().FromBytes();
            //    }
            //}
            //catch { }

            //return sAns;
        }

        private string Encode(string value)
        {
            return this.Encode(value, null);
        }

        private string Encode(string value, List<string> qual)
        {
            string sAns = System.Net.WebUtility.HtmlEncode(value);

            if (qual != null)
            {
                for (int iLoop = qual.Count - 1; iLoop >= 0; iLoop--)
                {
                    string sVerb = qual[iLoop];
                    string sExtra = "";
                    int iPos = sVerb.IndexOf(" ");
                    if (iPos != -1)
                    {
                        sExtra = sVerb.Substring(iPos).Trim();
                        if (sExtra.HasValue()) sExtra = " " + sExtra;
                        sVerb = sVerb.Substring(0, iPos);
                    }

                    sAns = "<" + sVerb + sExtra + ">" + sAns + "</" + sVerb + ">";
                }
            }

            return sAns;
        }

        private void UpdateCB()
        {
            if (this.Callback != null)
            {
                this.Callback(this.ToString());
            }
        }

        public void Parse(string value)
        {
            this.IDocument = null;

            try
            {
                if (value.HasValue())
                {
                    this.Document.LoadHtml(value);

                    this.UpdateCB();
                }
            }
            catch { }
        }

        public void AppendWord(string value, int row, List<string> qual)
        {
            if (row >= 0 && row < this.RowCount)
            {
                HtmlNode c_Node = this.RootNode.ChildNodes[row];
                c_Node.InnerHtml += this.Encode(" " + value, qual);
            }
        }

        public void Append(string value, List<string> qual)
        {
            try
            {
                if (this.DefaultQualifiers != null && this.DefaultQualifiers.Count > 0)
                {
                    List<string> c_Wkg = new List<string>();
                    c_Wkg.AddRange(this.DefaultQualifiers);
                    if (qual != null && qual.Count > 0) c_Wkg.AddRange(qual);
                    qual = c_Wkg;
                }

                this.RootNode.AppendChild(HtmlNode.CreateNode("<p>{0}</p>".FormatString(this.Encode(value, qual))));

                this.UpdateCB();
            }
            catch { }
        }

        public void Append(HTMLTextClass value)
        {
            foreach (HtmlNode c_Node in value.RootNode.ChildNodes)
            {
                this.RootNode.AppendChild(c_Node);
            }
        }

        public void AppendHTML(string value)
        {
            try
            {
                this.RootNode.AppendChild(HtmlNode.CreateNode(value));

                this.UpdateCB();
            }
            catch { }
        }

        public int InsertParagraph(int row)
        {
            while (row < 0)
            {
                if (this.RowCount == 0)
                {
                    this.Append("", new List<string>());
                }
                else
                {
                    this.RootNode.InsertBefore(this.NewNode(), this.RootNode.FirstChild);
                }
                row++;
            }

            if (row == 0)
            {
                if (this.RootNode.ChildNodes.Count == 0)
                {
                    this.RootNode.AppendChild(this.NewNode());
                }
                else
                {
                    this.RootNode.InsertBefore(this.NewNode(), this.RootNode.FirstChild);
                }
            }
            else if (row <= this.RowCount)
            {
                this.RootNode.InsertBefore(this.NewNode(), this.RootNode.ChildNodes[row]);
            }
            else
            {
                while (row > (this.RowCount - 1))
                {
                    this.RootNode.InsertAfter(this.NewNode(), this.RootNode.LastChild);
                }
            }

            return row;
        }
        #endregion
    }

    public class HTMLFragmentClass : IDisposable
    {
        #region Constructor
        public HTMLFragmentClass(string html)
        {
            //
            this.Node = HtmlNode.CreateNode(html);
        }

        public HTMLFragmentClass(string verb, string value, JSFragmentClass js, params string[] attr)
        {
            //
            string sWkg = "";
            string sType = verb;

            for (int i = 0; i < attr.Length; i += 2)
            {
                if (attr[i].IsSameValue("type")) sType = attr[i + 1];

                if (!attr[i].IsSameValue("id") || attr[i + 1].HasValue())
                {
                    sWkg += " {0}=\"{1}\"".FormatString(attr[i], attr[i + 1]);
                }
            }
            if (attr.Length % 2 == 1)
            {
                sWkg += " {0}".FormatString(attr[attr.Length - 1]);
            }

            if (js != null)
            {
                string sAction = "onblur";
                switch (sType)
                {
                    case "select":
                        sAction = "onchange";
                        break;

                    case "radio":
                        sAction = "onclick";
                        break;
                }

                JSFragmentsClass c_Frag = new JSFragmentsClass(js);
                sWkg += " {0}={1}".FormatString(sAction, c_Frag.ToString());
            }

            this.Node = HtmlNode.CreateNode("<{0}{1}>{2}</{0}>".FormatString(verb, sWkg, value.IfEmpty()));
        }

        public HTMLFragmentClass(string verb, string value, JSFragmentsClass js, params string[] attr)
        {
            //
            string sWkg = "";
            string sType = verb;

            for (int i = 0; i < attr.Length; i += 2)
            {
                if (attr[i].IsSameValue("type")) sType = attr[i + 1];

                if (!attr[i].IsSameValue("id") || attr[i + 1].HasValue())
                {
                    sWkg += " {0}=\"{1}\"".FormatString(attr[i], attr[i + 1]);
                }
            }
            if (attr.Length % 2 == 1)
            {
                sWkg += " {0}".FormatString(attr[attr.Length - 1]);
            }

            if (js != null)
            {
                string sAction = "onblur";
                switch (sType)
                {
                    case "select":
                        sAction = "onchange";
                        break;

                    case "radio":
                        sAction = "onclick";
                        break;
                }

                sWkg += " {0}={1}".FormatString(sAction, js.ToString());
            }

            this.Node = HtmlNode.CreateNode("<{0}{1}>{2}</{0}>".FormatString(verb, sWkg, value.IfEmpty()));
        }
        #endregion

        #region IDisposable
        public virtual void Dispose()
        { }
        #endregion

        #region Properties
        public HtmlNode Node { get; internal set; }
        public string Text
        {
            get { return this.Node.InnerText; }
        }

        private bool IsListWrapper { get; set; }

        public virtual string HTML
        {
            get { return this.Node.OuterHtml; }
        }

        private HtmlNode Bottom { get; set; }
        #endregion

        #region Methods
        public HTMLFragmentClass IsList()
        {
            this.IsListWrapper = true;

            return this;
        }

        public virtual HTMLFragmentClass Append(string text)
        {
            this.Node.InnerHtml += text.HTMLEncode();

            return this;
        }

        public virtual HTMLFragmentClass AppendBreak(string text)
        {
            this.Node.InnerHtml += text.HTMLEncode() + "<br/>";

            return this;
        }

        public virtual HTMLFragmentClass Append(HTMLFragmentClass child)
        {
            return this.Append(child, "");
        }

        public virtual HTMLFragmentClass Append(HTMLFragmentClass child, string pos)
        {
            if (child != null)
            {
                HtmlNode c_New = null;

                if (this.IsListWrapper)
                {
                    HTMLFragmentClass c_LI = HTMLFragmentClass.Create("li", "", (JSFragmentClass)null);
                    c_LI.Append(child, "");

                    c_New = c_LI.Node;
                }
                else
                {
                    c_New = child.Node;
                }

                if (c_New != null)
                {
                    if (pos.IsSameValue("b"))
                    {
                        if (this.Bottom == null)
                        {
                            this.Bottom = c_New;
                        }
                        this.Node.AppendChild(c_New);
                    }
                    else if (pos.IsNum())
                    {
                        int iPos = pos.ToInteger(0);
                        if (iPos < 0)
                        {

                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        if (this.Bottom != null)
                        {
                            this.Node.InsertBefore(c_New, this.Bottom);
                        }
                        else
                        {
                            this.Node.AppendChild(c_New);
                        }
                    }
                }
            }

            return this;
        }

        public override string ToString()
        {
            return this.Node.OuterHtml;
        }
        #endregion

        #region Statics
        public static HTMLFragmentClass Create(string verb,
                                               string value,
                                               JSFragmentClass js,
                                               params string[] attr)
        {
            JSFragmentsClass c_JS = null;
            if (js != null)
            {
                c_JS = new JSFragmentsClass(js);
            }

            return Create(verb, value, c_JS, attr);
        }

        public static HTMLFragmentClass Create(string verb,
                                                string value,
                                                JSFragmentsClass js,
                                                params string[] attr)
        {
            return new HTMLFragmentClass(verb, value.IfEmpty(), js, attr);
        }

        public static HTMLFragmentClass Parse(string html)
        {
            return new HTMLFragmentClass(html);
        }
        #endregion
    }

    public class JSFragmentClass : IDisposable
    {
        #region Constructor
        public JSFragmentClass(string fn, bool isscript = false)
        {
            if (isscript)
            {
                this.Script(fn);
            }
            else
            {
                this.JS = fn + "(this);";
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public string JS { get; private set; }
        #endregion

        #region Methods
        public void Script(string value)
        {
            this.JS = "(function(me){" + value + "})(this);";
        }
        #endregion
    }

    public class JSFragmentsClass : IDisposable
    {
        #region Constructor
        public JSFragmentsClass(params JSFragmentClass[] script)
        { }

        public JSFragmentsClass(string script, JSFragmentClass frag)
        {
            if (script.HasValue()) this.Fragments.Add(new JSFragmentClass(script));

            if (frag != null) this.Fragments.Add(frag);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        public List<JSFragmentClass> Fragments { get; private set; } = new List<JSFragmentClass>();
        #endregion

        #region Methods
        public override string ToString()
        {
            string sAns = "";

            foreach (JSFragmentClass c_Frag in this.Fragments)
            {
                if (c_Frag != null) sAns += c_Frag.JS;
            }

            return sAns;
        }
        #endregion
    }
    public class ASCIITextClass : IDisposable
    {
        #region Constants
        private const string KeyPara = "\n";
        #endregion

        #region Constructor
        public ASCIITextClass()
        {
            this.Parse("");
        }

        public ASCIITextClass(Action<string> cb)
        {
            this.Parse("");
            this.Callback = cb;
        }

        public ASCIITextClass(string value, Action<string> cb)
        {
            //
            this.Parse(value);
            this.Callback = cb;
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        private List<string> Contents { get; set; }
        private Action<string> Callback { get; set; }

        public int Rows { get { return this.Contents.Count; } }
        public int LastRow { get { return this.Rows - 1; } }
        #endregion

        #region Methods
        public override string ToString()
        {
            StringBuilder c_Ans = new StringBuilder();

            foreach (string sLine in this.Contents) c_Ans.Append(sLine + KeyPara);

            return c_Ans.ToString();
        }

        private void UpdateCB()
        {
            if (this.Callback != null)
            {
                this.Callback(this.ToString());
            }
        }

        public void Parse(string value)
        {
            this.Contents = new List<string>(value.IfEmpty().Replace("\r", "").Replace(KeyPara, "\t").Split('\t'));
        }

        private int AssureRow(int row)
        {
            if (row < 0)
            {
                this.InsertParagraph(0);
                row = 0;
            }
            else
            {
                while (row >= this.Contents.Count)
                {
                    this.AppendParagraph();
                }
            }

            return row;
        }

        public int AppendParagraph()
        {
            this.Contents.Add("");
            this.UpdateCB();

            return this.LastRow;
        }

        public int InsertParagraph(int pos)
        {
            if (pos < 0) pos = 0;
            if (pos >= this.Contents.Count)
            {
                this.AppendParagraph();
                pos = this.LastRow;
            }
            else
            {
                this.Contents.Insert(pos, "");
                this.UpdateCB();
            }

            return pos;
        }

        public string Append(string value, int row)
        {
            row = this.AssureRow(row);

            this.Contents[row] += value;

            return this.Contents[row];
        }
        #endregion
    }
}