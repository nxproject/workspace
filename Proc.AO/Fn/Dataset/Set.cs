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
using Newtonsoft.Json.Linq;

using MongoDB.Driver;
using MongoDB.Bson;

using NX.Engine;
using NX.Shared;

namespace Proc.AO
{
    /// <summary>
    /// 
    /// Sets an object
    /// 
    /// </summary>
    public class DatasetSet : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            // Get the params
            JObject c_Data = store.GetAsJObject("data");
            string sDS = c_Data.Get("_ds").AsDatasetName();

            // Valid?
            if (sDS.HasValue())
            {
                // Get the manager
                ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];

                // Get the current definition
                Definitions.DatasetClass c_Def = c_DS.Definition;

                // Get the current placeholder
                string sCPH = c_Def.Placeholder;
                string sCCSubj = c_Def.CalendarSubject;

                // Save
                c_Def.LoadFrom(c_Data);

                // Save
                c_Def.Save();

                // Get the new placeholder
                string sPH = c_Def.Placeholder;
                // Get the new calendar subject
                string sCSubj = c_Def.CalendarSubject;
                // Changed?
                if (!sPH.IsSameValue(sCPH) || !sCSubj.IsSameValue(sCCSubj))
                {
                    // Rebuild placeholder!
                    SafeThreadManagerClass.StartThread("".GUID(), new System.Threading.ParameterizedThreadStart(Rebuild), sDS, call.Env);
                }

                // Do we have a workflow?
                if (c_Def.WorkflowAllow && c_Def.WorkflowDataset.HasValue())
                {
                    // Get the dataset
                    Definitions.DatasetClass c_WFDS = c_Mgr.DefaultDatabase[c_Def.WorkflowDataset].Definition;
                    // Flag
                    c_WFDS.IsWorkflow = true;
                    // Caption
                    if (c_WFDS.Caption.IsSameValue(c_Def.WorkflowDataset))
                    {
                        c_WFDS.Caption = "Activity";
                        c_WFDS.StartIndex = c_Def.StartIndex;
                        c_WFDS.StartGroup = c_Def.StartGroup;
                    }

                    // Assure
                    if (!c_Def.WorkflowDescription.HasValue()) c_Def.WorkflowDescription = "desc";
                    if (!c_Def.WorkflowStartedOn.HasValue()) c_Def.WorkflowStartedOn = "starton";
                    if (!c_Def.WorkflowExpectedOn.HasValue()) c_Def.WorkflowExpectedOn = "expectedon";
                    if (!c_Def.WorkflowEndedOn.HasValue()) c_Def.WorkflowEndedOn = "endon";
                    if (!c_Def.WorkflowMessage.HasValue()) c_Def.WorkflowMessage = "note";
                    if (!c_Def.WorkflowAssignedTo.HasValue()) c_Def.WorkflowAssignedTo = "assgto";
                    if (!c_Def.WorkflowDoneBy.HasValue()) c_Def.WorkflowDoneBy = "doneby";
                    if (!c_Def.WorkflowOutcome.HasValue()) c_Def.WorkflowOutcome = "outcome";

                    // Make
                    Definitions.DatasetFieldClass c_Fld = c_WFDS[c_Def.WorkflowDescription];
                    c_Fld.Type = Definitions.DatasetFieldClass.FieldTypes.String;
                    c_Fld.Label = "Description";

                    c_Fld = c_WFDS[c_Def.WorkflowStartedOn];
                    c_Fld.Type = Definitions.DatasetFieldClass.FieldTypes.Date;
                    c_Fld.Label = "Start On";

                    c_Fld = c_WFDS[c_Def.WorkflowExpectedOn];
                    c_Fld.Type = Definitions.DatasetFieldClass.FieldTypes.Date;
                    c_Fld.Label = "Expected On";

                    c_Fld = c_WFDS[c_Def.WorkflowEndedOn];
                    c_Fld.Type = Definitions.DatasetFieldClass.FieldTypes.Date;
                    c_Fld.Label = "Ended On";

                    c_Fld = c_WFDS[c_Def.WorkflowMessage];
                    c_Fld.Type = Definitions.DatasetFieldClass.FieldTypes.TextArea;
                    c_Fld.Label = "Note";

                    c_Fld = c_WFDS[c_Def.WorkflowAssignedTo];
                    c_Fld.Type = Definitions.DatasetFieldClass.FieldTypes.Group;
                    c_Fld.Label = "Assg. To";

                    c_Fld = c_WFDS[c_Def.WorkflowDoneBy];
                    c_Fld.Type = Definitions.DatasetFieldClass.FieldTypes.Users;
                    c_Fld.Label = "Done By";

                    c_Fld = c_WFDS[c_Def.WorkflowOutcome];
                    c_Fld.Type = Definitions.DatasetFieldClass.FieldTypes.ComboBox;
                    c_Fld.Label = "Outcome";
                    c_Fld.Choices = "Done Fail";

                    // Adjust placeholder
                    if (!c_WFDS.Placeholder.HasValue())
                    {
                        // Start fresh
                        string sPHW = "";

                        sPHW += "#dateonlysortable([{0}],[*sys:timezone])# ".FormatString(c_Def.WorkflowStartedOn);
                        c_WFDS.SortOrder = "desc";

                        sPHW += "[{0}] ".FormatString(c_Def.WorkflowDescription);

                        sPHW += " ' // ' [{0}] ".FormatString(c_Def.WorkflowAssignedTo);

                        sPHW += " ' - ' #ifte([{0}]='','Active',[{1}])# ".FormatString(c_Def.WorkflowEndedOn, c_Def.WorkflowOutcome);

                        //
                        c_WFDS.Placeholder = sPHW;
                    }

                    // Picks
                    Definitions.PickListClass c_Pick = c_WFDS.Parent.PickList("assgme");
                    c_Pick.Label = "By/For me";
                    c_Pick.AllAny = "Any";
                    c_Pick.Field1 = c_Def.WorkflowAssignedTo;
                    c_Pick.Op1 = "Eq";
                    c_Pick.Value1 = "[*user:name]";
                    c_Pick.Field2 = c_Def.WorkflowDoneBy;
                    c_Pick.Op2 = "Eq";
                    c_Pick.Value2 = "[*user:name]";
                    c_Pick.Save();

                    c_Pick = c_WFDS.Parent.PickList("open");
                    c_Pick.Label = "Active";
                    c_Pick.AllAny = "All";
                    c_Pick.Field1 = c_Def.WorkflowOutcome;
                    c_Pick.Op1 = "Nin";
                    c_Pick.Value1 = "Done Fail Other";
                    c_Pick.Save();

                    c_Pick = c_WFDS.Parent.PickList("done");
                    c_Pick.Label = "Done";
                    c_Pick.AllAny = "All";
                    c_Pick.Field1 = c_Def.WorkflowOutcome;
                    c_Pick.Op1 = "Eq";
                    c_Pick.Value1 = "Done";
                    c_Pick.Save();

                    c_Pick = c_WFDS.Parent.PickList("fail");
                    c_Pick.Label = "Failed";
                    c_Pick.AllAny = "All";
                    c_Pick.Field1 = c_Def.WorkflowOutcome;
                    c_Pick.Op1 = "Eq";
                    c_Pick.Value1 = "Fail";
                    c_Pick.Save();

                    c_Pick = c_WFDS.Parent.PickList("overdue");
                    c_Pick.Label = "Overdue";
                    c_Pick.AllAny = "All";
                    c_Pick.Field1 = c_Def.WorkflowOutcome;
                    c_Pick.Op1 = "Eq";
                    c_Pick.Value1 = "Overdue";
                    c_Pick.Save();

                    // Calendar
                    c_WFDS.CalendarAllow = true;
                    c_WFDS.CalendarSubject = c_Def.WorkflowDescription.AsObjField();
                    c_WFDS.CalendarStart = c_Def.WorkflowStartedOn.AsObjField();
                    c_WFDS.CalendarBy = " ".Join(c_Def.WorkflowAssignedTo, c_Def.WorkflowOutcome);

                    // Analyze
                    c_WFDS.AnalyzerAllow = true;
                    c_WFDS.AnalyzerBy = " ".Join(c_Def.WorkflowStartedOn, c_Def.WorkflowAssignedTo, c_Def.WorkflowDoneBy, c_Def.WorkflowOutcome);

                    // Save
                    c_WFDS.Save();

                    // Get the default view
                    Definitions.ViewClass c_WFView = c_WFDS.Parent.View("default");
                    // Make it
                    c_WFView.UseFields(c_Def.WorkflowStartedOn,
                                        c_Def.WorkflowAssignedTo,
                                        c_Def.WorkflowEndedOn,
                                        c_Def.WorkflowDoneBy,
                                        c_Def.WorkflowOutcome,
                                        c_Def.WorkflowDescription,
                                        c_Def.WorkflowMessage);

                    // Make message bigger
                    c_WFView[c_Def.WorkflowMessage].Height = "5";

                    //
                    c_WFView.Save();

                    // Make sure that the activity dataset is a child
                    List<string> c_Child = c_Def.ChildDSs.SplitSpaces();
                    // There?
                    if (!c_Child.Contains(c_Def.WorkflowDataset))
                    {
                        // Add
                        c_Child.Add(c_Def.WorkflowDataset);
                        // Put back
                        c_Def.ChildDSs = c_Child.Join(" ");
                    }

                    // And save
                    c_Def.Save();
                }
            }

            return store;
        }

        #region Methods
        private void Rebuild(object status)
        {
            //
            SafeThreadStatusClass c_Status = status as SafeThreadStatusClass;

            //
            if (c_Status.IsActive)
            {
                // Get the passed values
                string sDS = (string)c_Status.Values[0];
                EnvironmentClass c_Env = c_Status.Values[1] as EnvironmentClass;

                // Get the manager
                ManagerClass c_Mgr = c_Env.Globals.Get<ManagerClass>();

                // Get the user dataset
                DatasetClass c_DS = c_Mgr.DefaultDatabase[sDS];
                // And the data
                CollectionClass c_Data = c_DS.DataCollection;

                // Loop thru
                var filter = Builders<BsonDocument>.Filter.Empty;
                foreach (BsonDocument c_Doc in c_Data.Documents.Find(filter).ToListAsync().Result)
                {
                    // Get
                    using (ObjectClass c_Obj = c_DS[c_Doc.GetValue("_id").AsString])
                    {
                        c_Obj.Save();
                    }
                }
            }

            // End
            c_Status.End();
        }
        #endregion
    }
}