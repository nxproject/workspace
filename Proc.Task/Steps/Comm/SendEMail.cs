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

using System.Collections.Generic;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;
using Proc.Communication;

namespace Proc.Task
{
    public class SendEMail : CommandClass
    {
        #region Constants
        private const string ArgTo = "to";
        private const string ArgMsg = "msg";
        private const string ArgSubj = "subj";
        private const string ArgList = "list";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public SendEMail()
        { }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();

                c_P.Add(ArgTo, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "E-Mail address to send the email"));
                c_P.Add(ArgSubj, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The subject of the message to be sent"));
                c_P.Add(ArgMsg, new ParamDefinitionClass(ParamDefinitionClass.Types.Required,  "The message to be sent"));
                c_P.Add(ArgList, new ParamDefinitionClass(ParamDefinitionClass.Types.Optional, "List of documents to attach", ""));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.Comm, "Sends an EMail", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "send.email"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // 
            string sTo = args.Get(ArgTo).IfEmpty(ctx.User.Name);
            string sSubj = args.Get(ArgSubj);
            string sMsg = args.Get(ArgMsg);
            string sList = args.Get(ArgList);;

            //
            using (eMessageClass c_Msg = new eMessageClass(ctx))
            {
                c_Msg.Subject = sSubj;
                c_Msg.Message = sMsg;
                c_Msg.To.Parse(sTo, eAddressClass.AddressTypes.EMail);
                c_Msg.Attachments.Add(ctx.DocumentLists[sList].Paths(ctx));

                c_Msg.Send();
            }

            return eAns;
        }
        #endregion
    }
}
