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

using System.Collections.Generic;
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Shared;

using Proc.AO;

namespace Proc.Office
{
    /// <summary>
    /// 
    /// Gets the start menu
    /// 
    /// </summary>
    public class GetStartMenu : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Set the return
            StoreClass c_Ans = new StoreClass();

            // Get the manager
            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

            // Get the passed
            string sName = store["name"];
            string sAllowed = store["allowed"];
            string sLocation = store["location"];

            // Do we need to reload?
            if (store["reload"].FromDBBoolean())
            {
                // Get from database
                sAllowed = c_Mgr.DefaultDatabase[DatabaseClass.DatasetUser][sName]["allowed"];
            }

            // Get a list of datasets
            List<string> c_DSS = c_Mgr.DefaultDatabase.UserDatasets;
            // If super user
            bool bAdmin = sAllowed.IsSameValue("*");

            // And room for the tools
            List<MenuEntryClass> c_AllEntries = new List<MenuEntryClass>();

            // Map
            string sQXDir = "".WorkingDirectory().CombinePath("ui.qx");
            // Get tools
            List<string> c_Files = sQXDir.CombinePath("tools").GetTreeInPath();
            List<string> c_GTools = new List<string>();

            List<string> c_Icons = sQXDir.CombinePath("icons").GetFilesNamesOnlyInPath();
            c_Icons.Sort();
            for (int i = 0; i < c_Icons.Count; i++) c_Icons[i] = c_Icons[i].Replace(".png", "");
            c_Ans.Set("icons", c_Icons.ToJArray());

            List<string> c_Docs = sQXDir.CombinePath("docs").GetFilesNamesOnlyInPath();
            c_Docs.Sort();
            for (int i = c_Docs.Count; i > 0; i--)
            {
                if (c_Docs[i - 1].IsSameValue("Backups"))
                {
                    c_Docs.RemoveAt(i - 1);
                }
            }
            for (int i = 0; i < c_Docs.Count; i++) c_Docs[i] = c_Docs[i].Replace(".odt", "");
            c_Ans.Set("docs", c_Docs.ToJArray());

            // Make the dataset privileges
            JObject c_Privs = new JObject();
            JArray c_Selectors = new JArray();
            List<string> c_GSelectors = new List<string>();
            JObject c_Groups = new JObject();

            // Get the allowed
            if (!sAllowed.HasValue() || bAdmin)
            {
                // Loop
                foreach (string sDS in c_DSS)
                {
                    // Validate
                    if (bAdmin || !sDS.StartsWith("_"))
                    {
                        // Get
                        Proc.AO.Definitions.DatasetClass c_Wkg = c_Mgr.DefaultDatabase[sDS].Definition;
                        if (c_Wkg.IsValid)
                        {
                            // Privileges
                            string sPrivileges = null;

                            // If admin, do all
                            if (bAdmin)
                            {
                                sPrivileges = "*";
                            }
                            else
                            {
                                sPrivileges = c_Wkg.Privileges;
                            }

                            // Make menu entry
                            MenuEntryClass c_ME = MenuEntryClass.FromDataset(c_Wkg, sPrivileges, "default");
                            // Valid?
                            if (c_ME != null)
                            {
                                //
                                bool bHidden = this.ComputeHidden(c_Wkg, c_ME, sLocation);

                                // Add to menu
                                if (!bHidden) c_AllEntries.Add(c_ME);

                                // Make entry
                                JObject c_Entry = new JObject();
                                c_Entry.Set("ds", sDS);
                                c_Entry.Set("caption", c_Wkg.Caption);
                                c_Entry.Set("privileges", sPrivileges);
                                c_Entry.Set("view", c_ME.View);
                                c_Entry.Set("icon", c_ME.Icon);

                                // Loop thru
                                foreach (string sFile in c_Files)
                                {
                                    // Make the menu entry
                                    MenuEntryClass c_MEa = MenuEntryClass.FromTool(sFile);
                                    // If real, add
                                    if (c_MEa != null && c_MEa.Dataset.IsSameValue(sDS))
                                    {
                                        if (bAdmin || !c_MEa.StartPrivilege.IsSameValue("*"))
                                        {
                                            if (!bHidden) c_AllEntries.Add(c_MEa);

                                            if (c_ME.Tools == null) c_ME.Tools = new List<string>();
                                            c_ME.Tools.Add(c_MEa.Name);
                                        }
                                    }
                                }

                                c_Privs.Set(sDS, c_Entry);
                                JArray c_G = new JArray();
                                c_G.Add("*");
                                c_Groups.Set(sDS, c_G);
                            }
                        }
                    }
                }

                //
                JArray c_SIO = new JArray();
                c_SIO.Add(Proc.SIO.ManagerClass.InternalCode);
                c_SIO.Add(Proc.SIO.ManagerClass.AccountCode);
                c_Ans.Set("sio", c_SIO);

                // Admin has all selectors
                if (bAdmin) c_GSelectors.Add("ALL");
            }
            else
            {
                // Expand
                string sExpanded = call.FN("Office.ExpandAllowed", new StoreClass("allowed", sAllowed))["value"];
                // And parse
                using (ItemsClass c_Items = Proc.AO.Definitions.UserPrivilegesClass.Parse(sExpanded))
                {
                    // Loop thru
                    foreach (ItemClass c_Item in c_Items)
                    {
                        //
                        string sDS = c_Item.Key;
                        string sView = null;
                        List<string> c_Tools = new List<string>();

                        JArray c_G = new JArray();

                        // Loop thru
                        foreach (ItemOptionClass c_Option in c_Item.Options)
                        {
                            // According to type
                            switch (c_Option.Option)
                            {
                                case "@":
                                    sView = c_Option.Value;
                                    break;

                                case "$":
                                    if (c_Option.Value.HasValue())
                                    {
                                        if (!sDS.HasValue())
                                        {
                                            c_GTools.Add(c_Option.Value);
                                        }
                                        else
                                        {
                                            c_Tools.Add(c_Option.Value);
                                        }
                                    }
                                    break;

                                case "%":
                                    c_G.Add(c_Option.Value);
                                    break;

                                case "?":
                                    if (c_Option.Value.HasValue())
                                    {
                                        c_GSelectors.Add(c_Option.Value);
                                    }
                                    else
                                    {
                                        c_Selectors.Add(c_Option.Value);
                                    }
                                    break;
                            }
                        }

                        // Handle global
                        if (sDS.HasValue())
                        {
                            // Get
                            Proc.AO.Definitions.DatasetClass c_Wkg = c_Mgr.DefaultDatabase[sDS].Definition;
                            if (c_Wkg.IsValid)
                            {
                                // Make menu entry
                                MenuEntryClass c_ME = MenuEntryClass.FromDataset(c_Wkg, c_Item.Value, sView);
                                // Valid?
                                if (c_ME != null)
                                {
                                    // Compute hidden
                                    bool bHidden = this.ComputeHidden(c_Wkg, c_ME, sLocation);

                                    // Loop thru
                                    foreach (string sFile in c_Files)
                                    {
                                        // Make the menu entry
                                        MenuEntryClass c_MEa = MenuEntryClass.FromTool(sFile);
                                        // Privileges
                                        if (c_MEa != null && c_MEa.StartPrivilege.HasValue())
                                        {
                                            if (c_GSelectors.IndexOf(c_MEa.StartPrivilege) == -1)
                                            {
                                                // Not allowed
                                                c_MEa = null;
                                            }
                                        }
                                        // If real, add
                                        if (c_MEa != null && c_MEa.Dataset.IsSameValue(sDS) && c_Tools.IndexOf(c_ME.Name) != -1)
                                        {
                                            if (c_ME.Tools == null) c_ME.Tools = new List<string>();
                                            c_ME.Tools.Add(c_MEa.Name);
                                        }
                                    }

                                    // Add tools
                                    c_ME.Tools = c_Tools;

                                    // Add to menu
                                    if (!bHidden) c_AllEntries.Add(c_ME);

                                    // Make entry
                                    JObject c_Entry = new JObject();
                                    c_Entry.Set("ds", sDS);
                                    c_Entry.Set("caption", c_Wkg.Caption);
                                    c_Entry.Set("privileges", c_Item.Value);
                                    c_Entry.Set("view", sView);
                                    c_Entry.Set("tools", c_Tools.ToJArray());
                                    c_Entry.Set("icon", c_ME.Icon);

                                    c_Privs.Set(sDS, c_Entry);
                                    c_Groups.Set(sDS, c_G);
                                }
                            }
                        }
                    }

                    //
                    JArray c_SIO = new JArray();
                    if (c_GSelectors.IndexOf("ACCT") == -1)
                    {
                        c_SIO.Add(Proc.SIO.ManagerClass.InternalCode);
                        c_SIO.Add(Proc.SIO.ManagerClass.AccountCode);
                    }
                    else
                    {
                        c_SIO.Add(Proc.SIO.ManagerClass.AccountCode);
                    }
                    c_Ans.Set("sio", c_SIO);
                }
            }
            // Add
            c_Ans.Set("datasets", c_Privs);

            c_Ans.Set("groups", c_Groups);
            c_Ans.Set("selectors", c_GSelectors);

            // Make the commands
            JArray c_Cmds = new JArray();
            // Get the tags
            List<TagClass> c_Tags = null;
            if (c_GSelectors.Contains("MGR"))
            {
                c_Tags = c_Mgr.DefaultDatabase.Tagged.ActiveTags(null, "pin");
            }
            else
            {
                c_Tags = c_Mgr.DefaultDatabase.Tagged.ActiveTags(sName, "pin");
            }
            // Loop thru
            foreach (TagClass c_Tag in c_Tags)
            {
                // 
                try
                {
                    c_Cmds.Add(c_Tag.AsJObject);
                }
                catch { }
            }
            c_Ans.Set("commands", c_Cmds);

            // Add system tools
            // Loop thru
            foreach (string sFile in c_Files)
            {
                // Make the menu entry
                MenuEntryClass c_MEa = MenuEntryClass.FromTool(sFile);
                // If real, add
                if (c_MEa != null && !c_MEa.Dataset.HasValue() &&
                    (bAdmin || c_MEa.IsSystem || c_GTools.Contains(c_MEa.Name)))
                {
                    // Privileges
                    if (c_MEa.StartPrivilege.HasValue() && !bAdmin)
                    {
                        if (c_GSelectors.IndexOf(c_MEa.StartPrivilege) == -1)
                        {
                            // Not allowed
                            c_MEa = null;
                        }
                    }
                    if (c_MEa != null) c_AllEntries.Add(c_MEa);
                }
            }

            // Add user config
            MenuEntryClass c_Extra = new MenuEntryClass();
            c_Extra.Name = "UserSettings";
            c_Extra.Caption = sName; // + "@" + call.Env.Hive.Name;
            c_Extra.Icon = "user";
            c_Extra.StartPriority = ".";
            c_Extra.StartGroup = "";
            c_Extra.StartIndex = 0;
            c_Extra.IsTool = true;
            c_AllEntries.Add(c_Extra);

            c_Extra = new MenuEntryClass();
            c_Extra.Caption = "-";
            c_Extra.StartPriority = ".";
            c_Extra.StartGroup = "";
            c_Extra.StartIndex = 1;
            c_Extra.IsTool = true;
            c_AllEntries.Add(c_Extra);

            // Fill in group indexes
            MenuEntryClass.ComputeGroupIndex(c_AllEntries);
            // Sort
            c_AllEntries.Sort(new MenuEntryComparer());

            // Build the root entries
            c_Ans.Set("menu", this.BuildEntries(call.Env, c_AllEntries, c_Privs, null));

            return c_Ans;
        }

        #region Methods
        private JArray BuildEntries(EnvironmentClass env, List<MenuEntryClass> list, JObject privs, string ds)
        {
            // Assume none
            JArray c_Ans = new JArray();

            // Set the group
            string sGroup = "";
            // Room for sub entries
            JArray c_Sub = new JArray();
            JObject c_GroupButton = null;
            // Priority
            string sPrio = null;

            // Loop thru
            foreach (MenuEntryClass c_ME in list)
            {
                // Flag
                bool bOK = false;

                // Root?
                if (!ds.HasValue())
                {
                    bOK = !c_ME.IsTool || (c_ME.IsTool && !c_ME.Dataset.HasValue());
                }
                else
                {
                    bOK = ds.IsSameValue(c_ME.Dataset) &&
                            c_ME.IsTool &&
                            (c_ME.Tools == null || c_ME.Tools.Contains(c_ME.Name));
                }

                // 
                if (bOK)
                {
                    // Priority changed?
                    if (sPrio.HasValue() && !sPrio.IsSameValue(c_ME.StartPriority))
                    {
                        // Add separator
                        c_Ans.Add("-");
                    }
                    // Save
                    sPrio = c_ME.StartPriority;

                    // Assume no children
                    JArray c_Items = null;

                    // Get the group
                    string sGroupMe = c_ME.StartGroup.IfEmpty();
                    // None?
                    if (!sGroupMe.HasValue())
                    {
                        // Add entry
                        this.AddMenu(c_Ans, c_ME, c_Items);
                    }
                    else
                    {
                        // New group?
                        if (!sGroup.IsSameValue(sGroupMe) || c_GroupButton == null)
                        {
                            // Save
                            sGroup = sGroupMe;

                            // Make room
                            c_Sub = new JArray();

                            // Make
                            c_GroupButton = new JObject();
                            c_GroupButton.Set("label", sGroup);
                            c_GroupButton.Set("items", c_Sub);

                            // Add
                            c_Ans.Add(c_GroupButton);
                        }

                        // Add button
                        this.AddMenu(c_Sub, c_ME, c_Items);
                        c_GroupButton.Set("items", c_Sub);
                    }
                }
            }

            return c_Ans;
        }

        private void AddMenu(JArray list, MenuEntryClass entry, JArray items)
        {
            // 
            JObject c_Btn = new JObject();

            // By type
            if (entry.IsTool)
            {
                c_Btn.Set("tool", entry.Name);
                c_Btn.Set("label", entry.Caption.IfEmpty(entry.Name));
                c_Btn.Set("icon", entry.Icon.IfEmpty());
            }
            else
            {
                if (!entry.Privileges.Contains("h"))
                {
                    c_Btn.Set("ds", entry.Name);
                    c_Btn.Set("label", entry.Caption.IfEmpty(entry.Name));
                    c_Btn.Set("fns", entry.Privileges);
                    c_Btn.Set("icon", entry.Icon.IfEmpty());
                    c_Btn.Set("view", entry.View);

                    // Items?
                    if (items != null && items.Count > 0)
                    {
                        // Add separator
                        items.Insert(0, "_");
                        //
                        c_Btn.Set("tools", items);
                    }
                }
                else
                {
                    c_Btn = null;
                }
            }

            if (c_Btn != null) list.Add(c_Btn);
        }

        private bool ComputeHidden(AO.Definitions.DatasetClass ds, MenuEntryClass me, string location)
        {
            //
            bool bAns = false;

            // Check to see "s" privileges
            if (me.Privileges.Contains("s"))
            {
                bAns = false;
            }
            // Now "e"
            else if (me.Privileges.Contains("e"))
            {
                bAns = true;
            }
            else
            {
                // Use the start index
                bAns = ds.StartIndex.IsSameValue("hidden");
                // Visible?
                if (!bAns)
                {
                    switch (ds.AtStart)
                    {
                        case AO.Definitions.DatasetClass.AtStartOptions.Yes:
                            bAns = true;
                            break;

                        case AO.Definitions.DatasetClass.AtStartOptions.AtMobile:
                        case AO.Definitions.DatasetClass.AtStartOptions.AtMobileViewOnly:
                            bAns = !location.IsSameValue("m");
                            break;

                        case AO.Definitions.DatasetClass.AtStartOptions.AtWebtop:
                        case AO.Definitions.DatasetClass.AtStartOptions.AtWebtopViewOnly:
                            bAns = !location.IsSameValue("w");
                            break;
                    }
                }
            }

            return bAns;
        }
        #endregion
    }

    public class MenuEntryComparer : IComparer<MenuEntryClass>
    {
        #region Methods
        public int Compare(MenuEntryClass v1, MenuEntryClass v2)
        {
            // Compare priority
            int iAns = v1.StartPriority.CompareTo(v2.StartPriority);
            if (iAns == 0)
            {
                // Compare group index
                iAns = v1.StartGroupIndex.CompareTo(v2.StartGroupIndex);
                // If same move to gorup
                if (iAns == 0)
                {
                    iAns = v1.StartGroup.CompareTo(v2.StartGroup);
                    // If same, index
                    if (iAns == 0)
                    {
                        iAns = v1.StartIndex.CompareTo(v2.StartIndex);
                        // If same, caption
                        if (iAns == 0)
                        {
                            iAns = v1.Caption.CompareTo(v2.Caption);
                        }
                    }
                }
            }

            return iAns;
        }
        #endregion
    }

    public class MenuEntryClass
    {
        #region Properties
        public string Name { get; set; }
        public string Caption { get; set; }
        public string Icon { get; set; }
        public string Dataset { get; set; }
        public string Privileges { get; set; }
        public string View { get; set; }

        private string IStartPriority { get; set; }
        public string StartPriority
        {
            get
            {
                // Get
                string sAns = this.IStartPriority;
                // If none, use default
                if (!sAns.HasValue()) sAns = this.IsTool ? "L" : "H";

                return sAns;
            }
            set { this.IStartPriority = value; }
        }

        public int StartIndex { get; set; }
        public string StartGroup { get; set; }
        public int StartGroupIndex { get; set; }
        public string StartPrivilege { get; set; }

        public bool IsTool { get; set; }
        public List<string> Tools { get; set; }

        public bool IsSystem { get; set; }
        #endregion

        #region Statics
        public static MenuEntryClass FromDataset(AO.Definitions.DatasetClass ds, string privileges, string view)
        {
            //
            MenuEntryClass c_Ans = new MenuEntryClass();

            // Fill
            c_Ans.Name = ds.Parent.Name;
            c_Ans.Caption = ds.Caption;
            c_Ans.Icon = ds.Icon;
            c_Ans.Dataset = ds.Name;
            c_Ans.View = view.IfEmpty("default");
            c_Ans.StartGroup = ds.StartGroup;
            c_Ans.StartPriority = ds.StartPriority;

            c_Ans.StartIndex = ds.StartIndex.ToInteger(0);
            c_Ans.Privileges = privileges;

            return c_Ans;
        }

        public static MenuEntryClass FromTool(string path)
        {
            //
            MenuEntryClass c_Ans = new MenuEntryClass();

            // Read
            string sContent = path.ReadFile();
            // Fill
            c_Ans.Name = path.GetFileNameOnlyFromPath();
            c_Ans.Caption = FetchFromTool(sContent, "caption");
            c_Ans.Icon = FetchFromTool(sContent, "icon");
            c_Ans.Dataset = FetchFromTool(sContent, "ds");
            c_Ans.StartPrivilege = FetchFromTool(sContent, "startprivilege");

            if (FetchFromTool(sContent, "startskipds").IsSameValue("y"))
            {
                c_Ans.Dataset = "";
            }

            c_Ans.StartGroup = FetchFromTool(sContent, "startgroup");
            c_Ans.StartPriority = FetchFromTool(sContent, "startpriority");

            string sWkg = FetchFromTool(sContent, "startindex");
            if (sWkg.IsSameValue("hidden"))
            {
                c_Ans = null;
            }
            else
            {
                c_Ans.StartIndex = sWkg.ToInteger(0);
                c_Ans.IsTool = true;
                c_Ans.IsSystem = c_Ans.StartIndex >= 9000;
            }

            return c_Ans;
        }

        private static string FetchFromTool(string content, string key)
        {
            //
            string sAns = null;

            // Get icon
            Match c_Match = Regex.Match(content, key + @"\x3A\s\x27(?<value>[^'\x27]+)\x27");
            // Parse
            if (c_Match.Success) sAns = c_Match.Groups["value"].Value;

            return sAns;
        }

        public static void ComputeGroupIndex(List<MenuEntryClass> menu)
        {
            // Make dictionary
            NamedListClass<int> c_Index = new NamedListClass<int>();

            // Loop thru
            foreach (MenuEntryClass c_Entry in menu)
            {
                // Assure
                c_Entry.StartGroup = c_Entry.StartGroup.IfEmpty();

                // Do we have an index?
                if (c_Index.ContainsKey(c_Entry.StartGroup))
                {
                    // Compare
                    if (c_Entry.StartIndex < c_Index[c_Entry.StartGroup])
                    {
                        // Replace
                        c_Index[c_Entry.StartGroup] = c_Entry.StartIndex;
                    }
                }
                else
                {
                    // Set
                    c_Index[c_Entry.StartGroup] = c_Entry.StartIndex;
                }
            }

            // Loop thru
            foreach (MenuEntryClass c_Entry in menu)
            {
                // Set
                c_Entry.StartGroupIndex = c_Index[c_Entry.StartGroup];
            }
        }
        #endregion
    }
}