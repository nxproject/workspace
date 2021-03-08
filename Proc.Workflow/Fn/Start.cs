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
    /// Calls a workflow 
    /// 
    /// </summary>
    public class Start : FNClass
    {
        public override StoreClass Do(HTTPCallClass call, StoreClass store)
        {
            StoreClass c_Ans = new StoreClass();

            // Get the manager
            ManagerClass c_Mgr = call.Env.Globals.Get<ManagerClass>();

            //
            string sDS = store["ds"];
            string sID = store["id"];
            string sWF = store["wf"].AsKeyword();
            string sInstance = store["instance"].AsKeyword();
            StoreClass c_Store = store.GetAsStore("params");
            if (c_Store == null) c_Store = new StoreClass();

            // Validate
            if (sDS.HasValue() && sID.HasValue() && sWF.HasValue())
            {
                // Get the database manager
                AO.ManagerClass c_DBMgr = call.Env.Globals.Get<AO.ManagerClass>();

                // Map dataset
                AO.DatasetClass c_DS = c_DBMgr.DefaultDatabase[sDS];

                //
                using (AO.UUIDClass c_UUID = new AO.UUIDClass(c_DBMgr.DefaultDatabase, sDS, sID, ""))

                // Make group
                using (AO.Extended.GroupClass c_Group = new AO.Extended.GroupClass (c_DBMgr.DefaultDatabase, AO.Extended.GroupClass.Types.Workflow, c_UUID, sWF, sInstance))
                {
                    // Get object
                    using (AO.ObjectClass c_Obj = c_DS[sID])
                    {
                        // Set volatile
                        c_Obj.Volatile();
                        // Run
                        c_Mgr.Exec(c_Obj, c_Group, c_Store, call.UserInfo.Name);

                        c_Ans["ok"] = "y";
                    }
                }
            }

            return c_Ans;
        }
    }
}