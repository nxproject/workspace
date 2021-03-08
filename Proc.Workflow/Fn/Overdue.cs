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

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Engine;
using NX.Engine.Files;
using NX.Shared;
using Proc.AO;

namespace Proc.Workflow
{
    /// <summary>
    /// 
    /// Owrkflow activity is overdue
    /// 
    /// </summary>
    public class Overdue : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            StoreClass c_Ans = new StoreClass();

            // Get the manager
            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

            //
            string sAt = store[AO.Extended.WorkflowClass.FieldWFIfDone];

            // Validate
            if (sAt.HasValue())
            {
                // Get the database manager
                AO.ManagerClass c_DBMgr = call.Env.Globals.Get<AO.ManagerClass>();

                // Make group
                using (AO.Extended.GroupClass c_Group = AO.Extended.GroupClass.FromGroupData(c_DBMgr.DefaultDatabase, store))
                {
                    // Get object
                    using (AO.ObjectClass c_Obj = c_Group.Object)
                    {
                        // Set volatile
                        c_Obj.Volatile();
                        // Get the outcome field
                        string sOutcomeFld = c_Obj[AO.Extended.WorkflowClass.FieldWFOutcome];
                        // Get the outcome value
                        if (!c_Obj[sOutcomeFld].HasValue())
                        {
                            // Set
                            c_Obj[sOutcomeFld] = "Overdue";
                            // Save
                            c_Obj.Save();
                        }
                    }
                }
            }

            return c_Ans;
        }
    }
}