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
/// Install-Package MongoDb.Bson -Version 2.11.0
/// 

using System.Collections.Generic;

using MongoDB.Bson;

using NX.Shared;
using NX.Engine;
using Proc.AO.BuiltIn;

namespace Proc.AO.Extended
{
    public class WorkflowClass : CronEntryClass
    {
        #region Constants
        public const string FieldWFActivity = "_wfisactivity";
        public const string FieldWFDesc = "_wfdesc";
        public const string FieldWFStartOn = "_wfstart";
        public const string FieldWFExpectedOn = "_wfexpected";
        public const string FieldWFEndedOn = "_wfend";
        public const string FieldWFMessage = "_wfmsg";
        public const string FieldWFAssignedTo = "_wfassg";
        public const string FieldWFBy = "_wfby";
        public const string FieldWFOutcome = "_wfout";
        public const string FieldWFShadow = "_wfshadow";

        public const string FieldWFFlowName = "_wfflow";
        public const string FieldWFInstance = "_wfinstance";

        public const string FieldWFActivityUUIID = "_wfactuuid";
        public const string FieldWFType = "_wftype";
        public const string FieldWFUUIID = "_wfuuid";
        //public const string FieldWFOverdueOn = "_overdueon";
        public const string FieldWFAt = "_wfat";
        public const string FieldWFIfDone = "_wfgotodone";
        public const string FieldWFIfFail = "_wfgotofail";
        public const string FieldWFIfOverdue = "_wfgotooverdue";

        #endregion

        #region Constructor
        public WorkflowClass(ObjectClass obj)
            : base(obj)
        { }
        #endregion
    }
}