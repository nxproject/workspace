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
/// Install-Package MongoDb.Driver -Version 2.11.0
/// Install-Package MongoDb.Bson -Version 2.11.0
/// 

using System;
using System.Collections.Generic;

using MongoDB.Driver;
using MongoDB.Bson;

using NX.Shared;

namespace Proc.AO
{
    public class AccessClass : ChildOfClass<ManagerClass>
    {
        #region Constructor
        public AccessClass(ManagerClass db, string id)
            : base(db)
        {
            //
            this.ID = id;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The access ID
        /// 
        /// </summary>
        public string ID { get; private set; }
        #endregion

        #region Methods
        public void UpdateContactIn(string source, string type, string campaign)
        {
            // Query access
            using (AO.QueryClass c_Qry = new AO.QueryClass(this.Parent.DefaultDatabase[AO.DatabaseClass.DatasetBillAccess].DataCollection))
            {
                // As email
                c_Qry.Add("name", AO.QueryElementClass.QueryOps.Eq, this.ID);
                // Any?
                List<AO.ObjectClass> c_Poss = c_Qry.FindObjects();
                // Loop thru
                foreach (AO.ObjectClass c_PO in c_Poss)
                {
                    // Update last contact out
                    c_PO["lastctcin"] = DateTime.Now.ToDBDate();
                    c_PO["lastctcinsource"] = source;
                    c_PO["lastctcinvia"] = type;
                    c_PO["lastctcincmp"] = campaign;

                    c_PO.Save();
                }
            }
        }

        public void UpdateContactOut(string source, string type, string campaign)
        {
            // Query access
            using (AO.QueryClass c_Qry = new AO.QueryClass(this.Parent.DefaultDatabase[AO.DatabaseClass.DatasetBillAccess].DataCollection))
            {
                // As email
                c_Qry.Add("name", AO.QueryElementClass.QueryOps.Eq, this.ID);
                // Any?
                List<AO.ObjectClass> c_Poss = c_Qry.FindObjects();
                // Loop thru
                foreach (AO.ObjectClass c_PO in c_Poss)
                {
                    // Update last contact out
                    c_PO["lastctcout"] = DateTime.Now.ToDBDate();
                    c_PO["lastctcoutsource"] = source;
                    c_PO["lastctcoutvia"] = type;
                    c_PO["lastctcoutcmp"] = campaign;
                    c_PO.Save();
                }
            }
        }
        #endregion
    }
}