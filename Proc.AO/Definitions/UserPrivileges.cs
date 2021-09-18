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

using System;
using System.Collections.Generic;
using System.Text;

using NX.Shared;
using NX.Engine;

namespace Proc.AO.Definitions
{
    /// <summary>
    /// 
    /// Allowed entry format:
    /// 
    /// <ds>:<privileges>@<view>%<group$<tool>#<allowedname>
    /// 
    /// Missing ds: Defines the defaults
    /// 
    /// * ds: Defines all datasets not defined
    /// 
    /// </summary>
    public class UserPrivilegesClass : IDisposable
    {
        #region Constructor
        public UserPrivilegesClass(string value, AO.DatasetClass allowedds, List<string> dss)
        {
            //
            this.AllowedDataset = allowedds;
            this.DatasetsAvailable = dss;

            // Do we have a value?
            if (!value.HasValue())
            {
                // default to all datasets
                value = dss.Join(" ");
            }
            // And the starting allowed
            this.Append(value);

            this.IsManager = value.IsSameValue("*");
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The comamnd stack
        /// 
        /// </summary>
        private ItemsClass Stack { get; set; } = new ItemsClass();

        /// <summary>
        /// 
        /// Is the user a manager?
        /// 
        /// </summary>
        public bool IsManager { get; set; }

        /// <summary>
        /// 
        /// The datasets defined
        /// 
        /// </summary>
        private NamedListClass<string> Buffer { get; set; } = new NamedListClass<string>();

        /// <summary>
        /// 
        /// The allowed dataset
        /// 
        /// </summary>
        private AO.DatasetClass AllowedDataset { get; set; }

        /// <summary>
        /// 
        /// The datasets availble
        /// 
        /// </summary>
        private List<string> DatasetsAvailable { get; set; }

        /// <summary>
        /// 
        /// The current privileges
        /// 
        /// </summary>
        private string DefaultPrivileges { get; set; } = "av";

        /// <summary>
        /// 
        /// The current view
        /// 
        /// </summary>
        private string DefaultView { get; set; }

        /// <summary>
        /// 
        /// List of groups allowed 
        /// 
        /// </summary>
        private List<string> DefaultGroups { get; set; }

        /// <summary>
        /// 
        /// List of tools allowed 
        /// 
        /// </summary>
        private List<string> DefaultTools { get; set; }

        /// <summary>
        /// 
        /// List of selectors allowed 
        /// 
        /// </summary>
        private List<string> DefaultSelectors { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Prepends a allowed string into the stack
        /// 
        /// </summary>
        /// <param name="value"></param>
        public void Append(string value)
        {
            // Must have value
            if (value.HasValue())
            {
                ItemsClass c_Cmds = UserPrivilegesClass.Parse(value);

                // Add current
                c_Cmds.Push(this.Stack);

                // And make the new stack
                this.Stack = c_Cmds;
            }
        }

        /// <summary>
        /// 
        /// Processes stack
        /// 
        /// </summary>
        /// <returns></returns>
        public string Process(List<string> dss)
        {
            //
            string sAns = "";

            // Get next item
            ItemClass c_Item = this.Stack.Pop();

            // Loop thru
            while (c_Item != null)
            {
                // Global?
                if (!c_Item.Key.HasValue())
                {
                    // Do we have privileges
                    if (c_Item.Value.HasValue())
                    {
                        // Save
                        this.DefaultPrivileges = c_Item.Value;
                    }

                    // Loop thru
                    foreach (ItemOptionClass c_Option in c_Item.Options)
                    {
                        // According to option
                        switch (c_Option.Option)
                        {
                            case "#":
                                // Must have a value
                                if (c_Option.Value.HasValue())
                                {
                                    // Get the allowed
                                    ObjectClass c_Obj = this.AllowedDataset[c_Option.Value];
                                    // Add
                                    this.Append(c_Obj["value"]);
                                }
                                break;

                            case "@":
                                this.DefaultView = c_Option.Value;
                                break;

                            case "$":
                                if (this.DefaultTools == null || !c_Option.Value.HasValue()) this.DefaultTools = new List<string>();
                                this.DefaultTools.Add(c_Option.Value);
                                break;

                            case "%":
                                if (this.DefaultGroups == null || !c_Option.Value.HasValue()) this.DefaultGroups = new List<string>();
                                this.DefaultGroups.Add(c_Option.Value);
                                break;

                            case "?":
                                if (this.DefaultSelectors == null || !c_Option.Value.HasValue()) this.DefaultSelectors = new List<string>();
                                this.DefaultSelectors.Add(c_Option.Value);

                                // Loop thru all datasets
                                foreach (string sWDS in this.DatasetsAvailable)
                                {
                                    // Get
                                    Definitions.DatasetClass c_Def = this.AllowedDataset.Parent[sWDS].Definition;
                                    if (c_Def.Selector.IsSameValue(c_Option.Value))
                                    {
                                        this.Stack.Push(new ItemClass(sWDS + ":zzz"));
                                    }
                                }
                                break;

                        }
                    }
                }
                else
                {
                    // The key is the dataset
                    string sDS = c_Item.Key;
                    // Special value
                    if (!c_Item.Value.IsSameValue("zzz") || !this.Buffer.Contains(sDS))
                    {//
                        string sPriv = c_Item.Value;
                        string sView = null;
                        List<string> c_Groups = null;
                        List<string> c_Tools = null;

                        // Get the options
                        List<ItemOptionClass> c_Options = new List<ItemOptionClass>(c_Item.Options);
                        // Loop thru
                        while (c_Options.Count > 0)
                        {
                            // Get
                            ItemOptionClass c_Option = c_Options[0];
                            c_Options.RemoveAt(0);

                            // According to option
                            switch (c_Option.Option)
                            {
                                case "#":
                                    // Must have a value
                                    if (c_Option.Value.HasValue())
                                    {
                                        // Get the allowed
                                        ObjectClass c_Obj = this.AllowedDataset[c_Option.Value];
                                        // Get the allowed
                                        string sCAllowed = c_Obj["value"];
                                        // Must have something
                                        if (sCAllowed.HasValue())
                                        {
                                            // Parse
                                            using (ItemsClass c_ChildItems = UserPrivilegesClass.Parse(sCAllowed))
                                            {
                                                // Look for the same key
                                                foreach (ItemClass c_ChildItem in c_ChildItems)
                                                {
                                                    // Is it this dataset?
                                                    if (c_Item.Key.IsSameValue(c_ChildItem.Key))
                                                    {
                                                        // Replace
                                                        if (c_ChildItem.Value.HasValue()) sPriv = c_ChildItem.Value;
                                                        // Get new options
                                                        List<ItemOptionClass> c_NewOptions = new List<ItemOptionClass>(c_ChildItem.Options);
                                                        // Add unprocessed
                                                        c_NewOptions.AddRange(c_Options);
                                                        // Replace
                                                        c_Options = c_NewOptions;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case "@":
                                    // Set the view
                                    sView = c_Option.Value;
                                    break;

                                case "%":
                                    if (c_Groups == null) c_Groups = new List<string>();
                                    c_Groups.Add(c_Option.Value);
                                    break;

                                case "$":
                                    if (c_Tools == null) c_Tools = new List<string>();
                                    c_Tools.Add(c_Option.Value);
                                    break;

                            }
                        }

                        // Handle manager
                        if (this.IsManager || sPriv.Contains("*")) sPriv = "avdcfr";

                        // Build the new item
                        string sEntry = sDS + ":" + sPriv.IfEmpty(this.DefaultPrivileges) +
                                            "@" + sView.IfEmpty(this.DefaultView).IfEmpty("default");

                        // Groups?
                        if (c_Groups == null) c_Groups = this.DefaultGroups;
                        if (c_Groups != null)
                        {
                            sEntry += "%" + c_Groups.Join("%");
                        }

                        // Tools?
                        if (c_Tools != null)
                        {
                            sEntry += "$" + c_Tools.Join("$");
                        }

                        // Global?
                        if (sDS.IsSameValue("*"))
                        {
                            // Loop thru
                            foreach (string sDSx in dss)
                            {
                                // Any?
                                if (!this.Buffer.ContainsKey(sDSx))
                                {
                                    if (!sDSx.StartsWith("_") || this.IsManager) this.Buffer[sDSx] = sDSx + sEntry.Substring(1);
                                }
                            }
                        }
                        else
                        {
                            // Store
                            this.Buffer[sDS] = sEntry;
                        }
                    }
                }

                // Get next item
                c_Item = this.Stack.Pop();
            }

            // Build result 
            List<string> c_Values = new List<string>();

            // Cleanup selectors
            int iPAcct = this.DefaultSelectors.IndexOf("ACCT");
            int iPUser = this.DefaultSelectors.IndexOf("USER");
            while (iPAcct != -1 && iPUser != -1)
            {
                if (iPAcct < iPUser)
                {
                    this.DefaultSelectors.RemoveAt(iPAcct);
                }
                else
                {
                    this.DefaultSelectors.RemoveAt(iPUser);
                }
                iPAcct = this.DefaultSelectors.IndexOf("ACCT");
                iPUser = this.DefaultSelectors.IndexOf("USER");
            }


            // Add selectors
            if (this.DefaultSelectors != null)
            {
                foreach (string sSel in this.DefaultSelectors)
                {
                    c_Values.Add("?" + sSel);
                }
            }

            // Add tools
            if (this.DefaultTools != null)
            {
                foreach (string sTool in this.DefaultTools)
                {
                    c_Values.Add("$" + sTool);
                }
            }

            // The datasets
            foreach (string sDS in this.Buffer.Keys)
            {
                // Get
                Definitions.DatasetClass c_Def = this.AllowedDataset.Parent[sDS].Definition;
                if (!c_Def.Selector.HasValue() || this.DefaultSelectors == null || this.DefaultSelectors.Contains(c_Def.Selector))
                {
                    c_Values.Add(this.Buffer[sDS]);
                }
            }

            //
            sAns = c_Values.Join(" ");

            return sAns;
        }
        #endregion

        #region Statics
        /// <summary>
        /// 
        /// Parses an allowed string
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ItemsClass Parse(string value)
        {
            // Parse
            return new ItemsClass(value, new ItemDefinitionClass()
            {
                ValueIsPriority = false,
                ItemDelimiter = " ",
                KeyValueDelimiter = ":",
                OptionDelimiters = new List<string>() { "@", "#", "$", "%", "?" }
            });
        }
        #endregion
    }
}