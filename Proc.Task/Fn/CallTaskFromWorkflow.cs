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
using Proc.Docs.Files;
using Proc.AO;

namespace Proc.Task
{
    /// <summary>
    /// 
    /// Calls a task 
    /// 
    /// </summary>
    public class CallTaskFromWorkflow : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            StoreClass c_Ans = null;

            // Get the manager
            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

            //
           string sTask = store["task"].AsKeyword();
            StoreClass c_Passed= store.GetAsStore("passed");
            if (c_Passed == null) c_Passed = new StoreClass();
            string sRet = store["return"];

            // Validate
            if (sTask.HasValue())
            {
                // Get the database manager
                AO.ManagerClass c_DBMgr = call.Env.Globals.Get<AO.ManagerClass>();

                // Make group
                using (AO.Extended.GroupClass c_Group = AO.Extended.GroupClass.FromGroupData(c_DBMgr.DefaultDatabase, store))
                {
                    // Make the object volatile
                    using (AO.ObjectClass c_Obj = c_Group.Object)
                    {
                        //
                        c_Obj.Volatile();

                        // Run
                        TaskContextClass c_Ctx = c_Mgr.Exec(call.UserInfo.Name, c_Group.UUID.Dataset, sTask, null, c_Passed, c_Obj);

                        //
                        if (sRet.HasValue())
                        {
                            c_Ans = c_Ctx.Stores[sRet];
                        }
                    }
                }
            }

            // Assure
            if (c_Ans == null) c_Ans = new StoreClass();

            return c_Ans;
        }
    }
}