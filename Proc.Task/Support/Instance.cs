//--------------------------------------------------------------------------------
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
using System.Text;
using System.Collections.Generic;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;

namespace Proc.Task
{
    public class InstanceClass : ChildOfClass<ManagerClass>
    {
        #region Constructor
        public InstanceClass(ManagerClass mgr, AO.DatasetClass ds, AO.Definitions.ElsaClass task)
            : base(mgr)
        {
            // Save
            this.Elsa = task;
            this.Dataset = ds;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The task being executed
        /// 
        /// </summary>
        public AO.Definitions.ElsaClass Elsa { get; private set; }

        /// <summary>
        /// 
        /// Label to go to if error
        /// 
        /// </summary>
        public AO.Definitions.ElsaActivityClass OnError { get; set; }

        /// <summary>
        ///  The dataset
        ///  
        /// </summary>
        public AO.DatasetClass Dataset { get; private set; }
        #endregion

        #region Managers
        private AO.ManagerClass IAOManager { get; set; }
        public AO.ManagerClass AOManager
        {
            get
            {
                if (this.IAOManager == null)
                {
                    this.IAOManager = this.Parent.Parent.Globals.Get<AO.ManagerClass>();
                }

                return this.IAOManager;
            }
        }

        private ManagerClass IChoreManager { get; set; }
        public ManagerClass ChoreManager
        {
            get
            {
                if (this.IChoreManager == null)
                {
                    this.IChoreManager = this.Parent.Parent.Globals.Get<ManagerClass>();
                }

                return this.IChoreManager;
            }
        }
        #endregion

        #region Methods
        public ReturnClass Exec(TaskContextClass ctx, ArgsClass args)
        {
            ReturnClass eAns = ReturnClass.Done;

            // Build at list
            foreach (AO.Definitions.ElsaActivityClass c_Step in this.Elsa.Steps.Values)
            {
                // Called?
                if (!c_Step.IsCalled)
                {
                    // Run
                    eAns = this.Exec(ctx, args, c_Step.ID, args.Depth);
                }
            }

            return eAns;
        }

        /// <summary>
        ///  
        /// Executes a task to completion
        ///  
        /// </summary>
        /// <param name="ctx">The context</param>
        /// <param name="args">The parameters</param>
        /// <returns>Return code</returns>
        public ReturnClass Exec(TaskContextClass ctx, ArgsClass args, string at, int depth, string contto = null)
        {
            ReturnClass eAns = ReturnClass.Done;

            // Do until done
            while (at.HasValue())
            {
                // Get step
                AO.Definitions.ElsaActivityClass c_Step = this.Elsa[at];
                // If none, there is an error
                if (c_Step == null)
                {
                    eAns = ReturnClass.Failure("Missing step {0}".FormatString(at));
                    break;
                }
                else
                {
                    // And get the executable
                    CommandClass c_Cmd = this.Parent.Get(c_Step.Type);
                    // If none, treat as noop
                    if (c_Cmd != null)
                    {
                        // Make the arguments
                        ArgsClass c_Args = new ArgsClass(ctx, this.Elsa, at, depth);
                        // Validate
                        eAns = this.CheckValidity(c_Cmd, ctx, c_Args);
                        if (eAns == null)
                        {
                            // Protect
                            try
                            {
                                // Execute
                                eAns = c_Cmd.ExecStep(ctx, c_Args);
                            }
                            catch (Exception e)
                            {
                                eAns = ReturnClass.Failure(e);
                                this.Parent.Parent.LogException("TASK STEP {0} FAULTED".FormatString(c_Step.Type), e);
                            }

                            // Assure
                            if (eAns == null) eAns = ReturnClass.Failure("Invalid command");

                            // Trace
                            if (this.Tracing)
                            {
                                //
                                this.TraceAdd(c_Step, c_Args, eAns);
                            }
                        }

                        // Handle result
                        if (eAns.Outcome.IsSameValue("End"))
                        {
                            // And stop loop
                            at = null;
                        }
                        else if (eAns.Outcome.HasValue())
                        {
                            // Is it continue?
                            if (eAns.Outcome.IsSameValue("Continue"))
                            {
                                at = contto;
                            }

                            // Do we have a route?
                            at = c_Step.Outcomes[eAns.Outcome];

                            // Handle globals
                            if (!at.HasValue())
                            {
                                string sSub = null;

                                switch (eAns.Outcome)
                                {
                                    case "OnError":
                                        sSub = ctx.OnError;
                                        break;
                                }

                                if (sSub.HasValue())
                                {
                                    // CAll sub
                                    this.Exec(ctx, args, sSub, depth + 1, c_Step.Outcomes["Done"]);
                                }
                            }
                        }
                        else
                        {
                            at = null;
                        }
                    }
                }
            }

            return eAns;
        }

        /// <summary>
        /// 
        /// Checks to see if args are there
        /// 
        /// </summary>
        /// <param name="args">The arguments/param>
        /// <returns>Tue if all values have a value</returns>
        public ReturnClass CheckValidity(CommandClass step, Context ctx, ArgsClass args)
        {

            // Assume all is OK
            ReturnClass eAns = null;

            // Make room
            List<string> c_Missing = new List<string>();

            // Get the definition
            DescriptionClass c_Desc = step.Description;

            // Loop thru
            foreach (string sKey in c_Desc.Parameters.Keys)
            {
                //
                ParamDefinitionClass c_Def = c_Desc.Parameters[sKey];
                // Required?
                if (c_Def.Type == ParamDefinitionClass.Types.Required)
                {
                    // Passed
                    string sValue = args.GetRaw(sKey);
                    // Missing?
                    if (!sValue.HasValue())
                    {
                        // Add to missing
                        c_Missing.Add(sKey);
                    }
                }
            }

            // Any?
            bool bAns = c_Missing.Count == 0;

            // If so
            if (!bAns)
            {
                //
                eAns = ReturnClass.Failure("Missing {0}".FormatString(c_Missing.Join(", ")));
            }

            return eAns;
        }
        #endregion

        #region Tracing
        /// <summary>
        /// 
        /// The buffer
        /// 
        /// </summary>
        private StringBuilder TraceBuffer { get; set; }

        /// <summary>
        /// 
        /// Are we tracing?
        /// 
        /// </summary>
        public bool Tracing { get { return this.TraceBuffer != null; } }

        /// <summary>
        /// 
        /// Adds a line to the trace
        /// 
        /// </summary>
        /// <param name="line">The command being executed</param>
        /// <param name="args">The args passed</param>
        public void TraceAdd(AO.Definitions.ElsaActivityClass line, ArgsClass args, ReturnClass rc)
        {
            this.TraceAdd(line.Type, args, rc.Outcome + ":" + rc.Message);
        }

        public void TraceAdd(string cmd, ArgsClass args, string extra = "")
        {
            this.TraceAdd("{0} :: {1} {2}".FormatString(cmd, args.ToString(), extra), args.Depth);
        }

        public void TraceAdd(string msg, int depth)
        {
            // Tracing?
            if (this.TraceBuffer != null)
            {
                int iCount = depth - 1;
                if (iCount > 0) iCount--;
                string sSpacer = "".RPad(4 * iCount, " ");
                this.TraceBuffer.AppendLine().Append(sSpacer).Append("[{0}]".FormatString(msg));
            }
        }

        public void TraceClear()
        {
            // Tracing?
            if (this.TraceBuffer != null)
            {
                this.TraceBuffer = new StringBuilder();
            }
        }

        /// <summary>
        /// 
        /// Starts the trace
        /// 
        /// </summary>
        public void StartTracing()
        {
            if (this.TraceBuffer == null)
            {
                this.TraceBuffer = new StringBuilder();
            }
        }

        /// <summary>
        /// 
        /// Ends the trace
        /// 
        /// </summary>
        public void StopTracing()
        {
            if (this.TraceBuffer != null)
            {
                this.TraceBuffer = null;
            }
        }
        #endregion
    }

    public class Names
    {
        public const string Passed = "passed";
        public const string Original = "params";
        public const string Local = "l";
        public const string Obj = "obj";
        public const string Portal = "portal";

        public const string Sys = "sys";
        public const string Changes = "changes";
        public const string User = "user";
    }
}