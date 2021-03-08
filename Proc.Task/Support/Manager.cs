///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020-2021 Jose E. Gonzalez (nxoffice2021@gmail.com) - All Rights Reserved
/// 
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
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

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;

namespace Proc.Task
{
    public class ManagerClass : ExtManagerClass<CommandClass>
    {
        #region Constructor
        /// <summary>
        /// 
        /// Constructor
        /// 
        /// </summary>
        /// <param name="env">The current environment</param>
        public ManagerClass(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The parameter table
        /// 
        /// </summary>
        public NamedListClass<DescriptionClass> Parameters { get; private set; } = new NamedListClass<DescriptionClass>();
        #endregion

        #region Methods
        /// <summary>
        /// 
        /// Calls a task
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="ds"></param>
        /// <param name="task"></param>
        /// <param name="passed"></param>
        /// <param name="store"></param>
        public TaskContextClass Exec(string user, AO.DatasetClass ds, string task, string passed, StoreClass store, AO.ObjectClass obj)
        {
            //
            AO.Definitions.ElsaClass c_Task = null;

            // Is it a built-in?
            if (this.Exists(task))
            {
                // Make a dynamic task
                c_Task = new AO.Definitions.ElsaClass();

                //
                string sID = "".GUID();

                //
                JObject c_Def = new JObject();
                c_Def.Set("id", sID);
                c_Def.Set("type", task);

                JArray c_Conn = new JArray();

                // Make a step
                AO.Definitions.ElsaActivityClass c_Step = new AO.Definitions.ElsaActivityClass(c_Task, c_Def, c_Conn);

                // Add the step
                c_Task.Steps.Add(sID, c_Step);
            }
            else
            {
                // Get the task
                c_Task = ds.Task(task);
            }

            // Make the instance
            using (InstanceClass c_Instance = new InstanceClass(this, ds, c_Task))
            {
                // Make the context
                TaskContextClass c_Ctx = new TaskContextClass(this, c_Instance, user, store, obj);

                // Make the arguments
                using (ArgsClass c_Args = new ArgsClass(c_Ctx, passed, 0))
                {
                    // Run
                    c_Instance.Exec(c_Ctx, c_Args);

                    // Clean up
                    c_Task.Dispose();

                    // Exit
                    return c_Ctx;
                }
            }
        }

        /// <summary>
        /// 
        /// Returns the process code for a given name
        /// 
        /// </summary>
        /// <param name="name">The function name</param>
        /// <returns>The function code (if any)</returns>
        public CommandClass GetCommand(string name)
        {
            return (CommandClass)this.Get(name);
        }

        /// <summary>
        /// 
        /// Generate MD file
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void GenerateMD(string path)
        {
            // Get the list
            List<string> c_Fns = new List<string>(this.Names);
            // Sort
            c_Fns.Sort();

            // List of categories
            List<string> c_Cat = new List<string>();
            // :oop thru
            foreach (string sFN in c_Fns)
            {
                CommandClass c_FN = this.GetCommand(sFN);

                DescriptionClass c_Desc = c_FN.Description;
                if (c_Desc != null)
                {
                    // Add if new
                    string sCat = c_Desc.Category.Name;
                    if (!c_Cat.Contains(sCat)) c_Cat.Add(sCat);
                }
            }
            // Sort
            c_Cat.Sort();

            // Buffer
            StringBuilder c_Final = new StringBuilder();

            // Loop thru
            foreach (string sCat in c_Cat)
            {
                //
                StringBuilder c_Buffer = new StringBuilder();

                c_Final.AppendLine("### " + sCat);

                // :oop thru
                foreach (string sFN in c_Fns)
                {
                    CommandClass c_FN = this.GetCommand(sFN);

                    DescriptionClass c_Desc = c_FN.Description;
                    if (c_Desc != null)
                    {
                        if (sCat.IsSameValue(c_Desc.Category.Name))
                        {
                            string sLine = "|" + c_FN.Command + "|" + c_Desc.Description + "|";
                            // If none. ue empty
                            if (c_Desc.Parameters.Count == 0)
                            {
                                c_Buffer.AppendLine(sLine + " |");
                            }
                            else
                            {
                                bool bShowLine = true;

                                foreach (string sParam in c_Desc.Parameters.Keys)
                                {
                                    ParamDefinitionClass c_P = c_Desc.Parameters[sParam];
                                    //
                                    string sP = sParam + "|" + c_P.Description + "|" + (c_P.Type == ParamDefinitionClass.Types.Required ? c_P.Type.ToString() : "") + "|";

                                    if (bShowLine)
                                    {
                                        c_Buffer.AppendLine(sLine + sP);
                                        bShowLine = false;
                                    }
                                    else
                                    {
                                        c_Buffer.AppendLine("| | |" + sP);
                                    }

                                }
                            }
                        }
                    }
                    else
                    {
                        c_Buffer.AppendLine("|" + c_FN.Command + "|MISSING DESCRIPTION|");
                    }
                }

                // Make header
                string sHeader = "|Command|Description|Parameter|Use| |";
                string sDelim = "|-|-|-|-|-|";

                // Make
                string sText = sHeader + "\n" + sDelim + "\n" + c_Buffer.ToString();
                // Add
                c_Final.AppendLine(sText);
            }

            // Read template
            string sTemplate = (path + ".template").ReadFile();
            // Replace
            sTemplate = sTemplate.Replace("{{fns}}", c_Final.ToString());
            // Write
            path.WriteFile(sTemplate);
        }

        /// <summary>
        /// 
        /// Generate elsa typescript
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void GenerateElsa(string path)
        {
            // Buffers
            StringBuilder c_Fns = new StringBuilder();
            StringBuilder c_Calls = new StringBuilder();

            bool bFirst = true;

            // Loop thru
            foreach (string sCmd in this.Names)
            {
                // Get
                CommandClass c_Cmd = this.GetCommand(sCmd);
                // Generate
                Tuple<string, string> c_Elsa = c_Cmd.GenerateElsa();
                if (c_Elsa != null)
                {
                    // Valid?
                    if (c_Elsa.Item1.HasValue())
                    {
                        // Save name
                        if (!bFirst) c_Fns.AppendLine(",");
                        bFirst = false;
                        c_Fns.Append("this." + c_Elsa.Item1 + "()");

                        // And body
                        c_Calls.AppendLine(c_Elsa.Item2);
                    }
                }
            }

            // Get the directory
            string sTemplate = (path + ".template").ReadFile();
            // Change
            sTemplate = sTemplate.Replace("{{items}}", c_Fns.ToString());
            sTemplate = sTemplate.Replace("{{definitions}}", c_Calls.ToString());
            sTemplate = sTemplate.Replace("{{ts}}", DateTime.Now.ToString());

            // Write out
            path.WriteFile(sTemplate);

        }
        #endregion
    }
}