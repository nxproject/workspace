///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

using Proc.AO;
using Proc.Docs;

namespace Proc.Chore
{
    public class UIFDataset : CommandClass
    {
        #region Constants
        private const string ArgDS = "ds";
        private const string ArgObj = "obj";
        private const string ArgFlds = "flds";
        private const string ArgExcl = "excl";
        private const string ArgStore = "store";
        private const string ArgChores = "chores";
        private const string ArgChore = "chore";
        private const string ArgTitle = "title";
        private const string ArgTitleType = "ttype";
        private const string ArgOnComp = "oncompletion";
        private const string ArgRegen = "regen";

        //private const string ArgIXX = "if";

        //private const string FormName = "input";
        #endregion

        #region Constructor
        public UIFDataset(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Enums
        public enum PanelTypes
        {
            Normal,
            Repeating,
            Side
        }
        #endregion

        #region Properties
        private string DS { get; set; }
        private AO.UUIDClass UUID { get; set; }
        private JObject Info { get; set; }
        private AO.ObjectClass Obj { get; set; }
        #endregion

        #region Methods
        private void Setup(SandboxClass env, ArgsClass arguments)
        {
            string sStore = args.GetDefined(ArgStore);

            this.DS = args.GetDefined(ArgDS);
            string sObj = args.GetDefined(ArgObj);

            if (!this.DS.HasValue()) sObj = env.Objects.Default;

            // Get the store
            this.Info = env.Stores[sStore];

            // Assume no UUID
            //AO.UUIDClass this.UUID = null;
            //AO.ObjectClass this.Obj = null;

            // Do we have a form name that is an UUID?
            string sFormName = this.Info.UIFormName();
            //env.Env.LogInfo("FORM: {0}".FormatString(sFormName));

            if (AO.UUIDClass.IsValid(sFormName))
            {
                this.UUID = new AO.UUIDClass(sFormName);
            }

            // If no UUID
            if (this.UUID == null && sObj.HasValue())
            {
                // Object?
                this.Obj = env.Objects[sObj];
                if (this.Obj != null)
                {
                    this.UUID = this.Obj.UUID;
                }
            }

            // Set the DS
            if (this.UUID != null)
            {
                this.DS = this.UUID.Dataset;
            }

            if (this.DS.HasValue())
            {
                // Do we have an object?
                if (this.Obj == null)
                {
                    // Old?
                    if (this.UUID != null)
                    {
                        this.Obj = env.Associate.FetchObject(this.UUID, AO.ObjectClass.Types.Data);
                    }
                    else
                    {
                        // New
                        this.Obj = env.Associate.CreateObject(this.DS, AO.ObjectClass.Types.Data);
                        this.UUID = this.Obj.UUID;
                    }
                }
            }
        }

        private static void MakePanel(SandboxClass env,
                                    UIFormClass form,
                                    List<AO.FieldDefinitionClass> flds,
                                    AO.ObjectClass obj,
                                    AO.FieldDefinitionClass panel,
                                    PanelTypes type,
                                    bool forcero)
        {
            // Get the value
            JObject c_Value = obj[panel.Name].ToJObject();
            if (c_Value == null) c_Value = new JObject();
            // List of potential tabs
            List<string> c_Tabs = c_Value.AssureJArray("tabs").ToList();
            if (c_Tabs.Count == 0)
            {
                c_Tabs = panel.SupportFields.SplitSpaces(true);
                if (c_Tabs.Count == 0)
                {
                    // Add the fields
                    foreach (AO.FieldDefinitionClass c_Fld in flds)
                    {
                        // Only the valid ones
                        if (c_Fld != null && c_Fld.Panel.IsSameValue(panel.Name) && !c_Fld.Mods.IsSameValue("hidden"))
                        {
                            string sTab = c_Fld.Tab;
                            if (c_Tabs.IndexOf(sTab) == -1)
                            {
                                c_Tabs.Add(sTab);
                            }
                        }
                    }
                }
            }

            //string sSel = c_Value.Get("value");
            //if (sSel.HasValue())
            //{
            //    for (int i = c_Tabs.Count - 1; i >= 0; i--)
            //    {
            //        if (c_Tabs[i].IsSameValue(sSel))
            //        {
            //            c_Tabs.RemoveAt(i);
            //            break;
            //        }
            //    }

            //    if (c_Tabs.Count == 0)
            //    {
            //        c_Tabs.Add(sSel);
            //    }
            //    else
            //    {
            //        c_Tabs.Insert(0, sSel);
            //    }
            //}

            // Build
            foreach (string sTab in c_Tabs)
            {
                UIFDataset.MakeSection(env,
                                    form,
                                    flds,
                                    obj,
                                    panel.Name,
                                    sTab,
                                    sTab,
                                    forcero);
            }
        }

        private static void MakeSection(SandboxClass env,
                                    UIFormClass form,
                                    List<AO.FieldDefinitionClass> flds,
                                    AO.ObjectClass obj,
                                    string panel,
                                    string tab,
                                    string label,
                                    bool forcero)
        {

            // Add the fields
            foreach (AO.FieldDefinitionClass c_Fld in flds)
            {
                // Only the valid ones
                if (c_Fld != null && c_Fld.Panel.IsSameValue(panel) && c_Fld.Tab.IsSameValue(tab) && !c_Fld.Mods.IsSameValue("hidden"))
                {
                    // Assume invalid
                    HTMLFragmentClass c_Entry = null;

                    // Obtained
                    string sLabel = c_Fld.Label;
                    string sName = c_Fld.Name;
                    string sValue = obj[c_Fld.Name];
                    string sPlaceH = c_Fld.Info;
                    bool bRO = c_Fld.Mods.IsSameValue("ro");
                    if (forcero) bRO = true;

                    switch (c_Fld.Type)
                    {
                        case AO.FieldDefinitionClass.Types.ACCPHONE:
                        case AO.FieldDefinitionClass.Types.PHONE:
                        case AO.FieldDefinitionClass.Types.WIREDPHONE:
                        case AO.FieldDefinitionClass.Types.FAX:
                        case AO.FieldDefinitionClass.Types.ACCFAX:
                            c_Entry = "".UIPhoneField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.COMBO:
                        case AO.FieldDefinitionClass.Types.MULTICOMBO:
                            c_Entry = "".UIChoiceCField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null, c_Fld.Choices.SplitSpaces(true).ToArray());
                            break;

                        case AO.FieldDefinitionClass.Types.TEXT:
                        case AO.FieldDefinitionClass.Types.SEARCHTEXT:
                            c_Entry = "".UITextField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null, 5);
                            break;

                        case AO.FieldDefinitionClass.Types.PASSWORD:
                            c_Entry = "".UIPasswordField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.CHECKBOX:
                            c_Entry = "".UICheckBoxField(sLabel, sName, obj[c_Fld.Name].FromDBBoolean(), (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.EMAIL:
                        case AO.FieldDefinitionClass.Types.ACCEMAIL:
                            c_Entry = "".UIEMailField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.DATE:
                            c_Entry = "".UIDateField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.DATETIME:
                            c_Entry = "".UIDateTimeField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.INT:
                        case AO.FieldDefinitionClass.Types.POSITIVEINT:
                        case AO.FieldDefinitionClass.Types.FLOAT:
                        case AO.FieldDefinitionClass.Types.CURRENCY:
                            c_Entry = "".UINumberField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.ZIP:
                            c_Entry = "".UIZIPField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.SSN:
                            c_Entry = "".UISSNField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.TIME:
                            c_Entry = "".UITimeField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.STATE:
                            c_Entry = "".UIStateField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.STATENAME:
                            c_Entry = "".UIStateNameField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.STATEABBREV:
                            c_Entry = "".UIStateAbbrevField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.LABEL:
                            c_Entry = c_Fld.Label.UIFText("info");
                            break;

                        case AO.FieldDefinitionClass.Types.RO:
                            c_Entry = c_Fld.Label.UIFText("error");
                            break;

                        case AO.FieldDefinitionClass.Types.UPLOAD:
                            c_Entry = "".UIFileField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.USER:
                            c_Entry = "".UIUsersField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null, env.Associate);
                            break;

                        case AO.FieldDefinitionClass.Types.NAME:
                        case AO.FieldDefinitionClass.Types.ADDRESS:
                        case AO.FieldDefinitionClass.Types.CITY:
                        case AO.FieldDefinitionClass.Types.AUTOCAPS:
                            c_Entry = "".UIAutoCapsField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.CAPS:
                            c_Entry = "".UICapsField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.EAMS:
                            c_Entry = "".UIEAMSField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.EAMSTEXT:
                            c_Entry = "".UIEAMSTextField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null, 5);
                            break;

                        case AO.FieldDefinitionClass.Types.CREDITCARD:
                            c_Entry = "".UICCField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.CREDITCARDEXP:
                            c_Entry = "".UICCExpField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        // Lookups are simple string fields
                        //case AO.FieldDefinitionClass.Types.LU:
                        //    break;

                        case AO.FieldDefinitionClass.Types.LINK:
                        case AO.FieldDefinitionClass.Types.OPTLINK:
                            bRO = true;
                            if (sValue.HasValue())
                            {
                                if(AO.UUIDClass.IsValid(sValue))
                                {
                                    sValue = env.Associate.FetchPlaceholder(new AO.UUIDClass(sValue));
                                }
                                else
                                {
                                    sValue = "";
                                }
                            }
                            c_Entry = "".UIStringField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;

                        case AO.FieldDefinitionClass.Types.IMAGE:
                        case AO.FieldDefinitionClass.Types.DOCUMENT:
                        case AO.FieldDefinitionClass.Types.SIGNATURE:
                        case AO.FieldDefinitionClass.Types.APP_DOCUMENTS:
                        case AO.FieldDefinitionClass.Types.APP_DOCUMENTSSHOW:
                        case AO.FieldDefinitionClass.Types.DURATION:
                        case AO.FieldDefinitionClass.Types.PRINTER:
                        case AO.FieldDefinitionClass.Types.MEMO:
                        case AO.FieldDefinitionClass.Types.BUTTON:
                        case AO.FieldDefinitionClass.Types.BILLING:
                            break;

                        case AO.FieldDefinitionClass.Types.PANEL:
                            UIFDataset.MakePanel(env, form, flds, obj, c_Fld, PanelTypes.Normal, forcero);
                            break;

                        case AO.FieldDefinitionClass.Types.RPANEL:
                            UIFDataset.MakePanel(env, form, flds, obj, c_Fld, PanelTypes.Repeating, forcero);
                            break;

                        case AO.FieldDefinitionClass.Types.SPANEL:
                            UIFDataset.MakePanel(env, form, flds, obj, c_Fld, PanelTypes.Side, forcero);
                            break;


                        default:
                            c_Entry = "".UIStringField(sLabel, sName, sValue, bRO, sPlaceH, (JSFragmentClass)null);
                            break;
                    }

                    if (c_Entry != null)
                    {
                        if (label.HasValue())
                        {
                            form.Body.Append(label.UIText("label"));
                            label = null;
                        }

                        form.Body.Append(c_Entry);
                    }
                }
            }
        }
        #endregion

        #region Code Line
        public override string Description { get { return("Dataset form"); } }

        public override string ParamDescription
        {
            get
            {
                return this.BuildParams(

            CommandClass.Required,ArgTitle, "The title in the menu",
            CommandClass.Optional, ArgTitleType, "Title type (Main/H1/H2/H3/H4/None)",
            CommandClass.Optional, ArgStore, "Form data store",
            CommandClass.Optional, ArgDS, "The dataset",
            CommandClass.Optional, ArgObj, "The object",
            CommandClass.Optional, ArgFlds, "Fields to display",
            CommandClass.Optional, ArgExcl, "Fields to exclude",
            CommandClass.Optional, ArgChores, "If true (non zero) allow chores, otherwise disallow", "#1#",
            CommandClass.Optional, "*", "List of field/value pairs to save (will not be displayed)",
            CommandClass.Optional, ArgChore, "Chore to run upon completion",
            CommandClass.Optional, ArgOnComp, "Label to go to upon completion",
            CommandClass.Optional, ArgRegen, "Regenerate form upon completion", "#1");

            }
        }

        public override string Command
        {
            get { return "ui.frame.dataset"; }
        }

        public override ReturnClass ExecStep(ChoreContext ctx, InstanceClass instance, ArgsClass args)
        {
            //
            ReturnClass eAns = base.ExecStep(ctx, instance, args);

            this.Setup(env, arguments);

            // Validate
            if (this.DS.HasValue())
            {
                // 
                bool bGen = false;

                // Is it a return?
                if (this.Info.UIIsReturnDS(this.Obj.UUID))
                {
                    //env.Env.LogInfo("RETURN: {0}".FormatString(this.UUID.ToString()));

                    if (this.Info != null)
                    {
                        // Do each
                        foreach (string sField in this.Info.Keys())
                        {
                            this.Obj[sField] = this.Info.Get(sField);
                        }
                    }

                    // Passed
                    List<string> c_Set = arguments.GetList();
                    for (int i = 0; i < c_Set.Count; i += 2)
                    {
                        this.Obj[c_Set[i]] = c_Set[i + 1];
                    }

                    bool bChores = args.GetDefinedAsBool(ArgChores, true);
                    if (!bChores) this.Obj.Type = AO.ObjectClass.Types.SkipChores;
                    bool bNew = this.Obj.IsNew;

                    if (this.Obj.Put())
                    {
                        // If new, do billing
                        if(bNew)
                        {
                            ChargeClass c_Charge = ChargeClass.DataExchangeReceive(env.Associate.Caller, "via portal", this.Obj.UUID.Dataset);
                            if (!ChargeClass.IsDataExchangeReceiveDefaultCode( c_Charge))
                            {
                                env.Associate.Caller.PortalCallBill(env.Associate.UserID, c_Charge);
                            }
                        }

                        eAns = ReturnClass.OK;

                        string sChore = args.GetDefined(ArgChore);
                        if (sChore.HasValue())
                        {
                            env.UI.Clear(this.Info);
                            using (ChoreNameClass c_ChoreRef = new ChoreNameClass(sChore))
                            {
                                env.ExecStep(c_ChoreRef, arguments);
                            }
                        }

                        string sGoTo = args.GetDefined(ArgOnComp);
                        if (sGoTo.HasValue())
                        {
                            eAns = ReturnClass.GoTo(sGoTo);
                        }
                    }

                    bGen = args.GetDefinedAsBool(ArgRegen, true);
                    if (bGen)
                    {
                        env.UI.Clear(this.Info);

                        this.DS = null;
                        this.UUID = null;
                        this.Obj = null;
                        this.Info = null;

                        this.Setup(env, arguments);
                    }
                }
                else
                {
                    bGen = true;
                }

                if (bGen)
                {
                    //env.Env.LogInfo("GENERATE: {0}".FormatString(this.UUID.ToString()));

                    // Get the dataset definition
                    AO.DatasetClass c_DS = env.Associate.Datasets[this.UUID.Dataset];
                    // Real?
                    if (c_DS != null && c_DS.Layout.Fields.Count > 0)
                    {
                        // Passed
                        string sTitle = args.GetDefined(ArgTitle);
                        List<string> c_Flds = args.GetDefined(ArgFlds).SplitSpaces(true);
                        List<string> c_EFlds = args.GetDefined(ArgExcl).SplitSpaces(true);

                        string sTType = args.GetDefined(ArgTitleType);
                        UICtxClass.TitleTypes eType = UICtxClass.TitleTypes.H2;
                        if (sTType.HasValue())
                        {
                            try
                            {
                                eType = (UICtxClass.TitleTypes)Enum.Parse(typeof(UICtxClass.TitleTypes), sTType, true);
                            }
                            catch { }
                        }
                        // The list of fields
                        List<AO.FieldDefinitionClass> c_Fields = c_DS.Layout.Fields;

                        // Do we have a list of fields
                        if (c_Flds != null && c_Flds.Count > 0)
                        {
                            List<AO.FieldDefinitionClass> c_Chose = new List<AO.FieldDefinitionClass>();
                            while (c_Chose.Count < c_Flds.Count) c_Chose.Add(null);

                            foreach (AO.FieldDefinitionClass c_Fld in c_Fields)
                            {
                                if (c_EFlds.IndexOf(c_Fld.Name) == -1)
                                {
                                    int iPos = c_Flds.IndexOf(c_Fld.Name);
                                    if (iPos != -1)
                                    {
                                        c_Chose[iPos] = c_Fld;
                                    }
                                    c_Fields = c_Chose;
                                }
                            }
                        }

                        // Exclude
                        if (c_EFlds != null && c_EFlds.Count > 0)
                        {
                            for (int i = c_Fields.Count - 1; i >= 0; i--)
                            {
                                if (c_EFlds.IndexOf(c_Fields[i].Name) != -1)
                                {
                                    c_Fields.RemoveAt(i);
                                }
                            }
                        }

                        // Map the UI
                        UIWorkspaceClass c_WS = env.UI;
                        //UIUserClass c_Assoc = c_WS.State.User;

                        // The one  container
                        UICtxClass c_Ctx = new UICtxClass("ds", sTitle.IfEmpty(c_DS.Description), eType);
                        // Make  the form
                        UIFormClass c_Form = "".UIForm(this.UUID.ToString(), (this.Obj.IsNew ? "Add" : "Update"), "", null, "Reset", env.UI.State.User);

                        // Now for the base fields
                        UIFDataset.MakeSection(env, c_Form, c_Fields, this.Obj, "", "", "", false);

                        // Add
                        c_Ctx.Contents.Add(c_Form);
                        c_WS.Add(c_Ctx);
                    }
                }

                eAns = ReturnClass.OK;
            }

            return eAns;
        }
        #endregion
    }
}