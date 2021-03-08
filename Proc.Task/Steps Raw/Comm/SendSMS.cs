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

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class SendSMS : CommandClass
    {
        #region Constants
        private const string ArgTo = "to";
        private const string ArgMsg = "msg";
        private const string ArgList = "list";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public SendSMS(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParameterDefinitionClass> c_P = new NamedListClass<ParameterDefinitionClass>();

                c_P.Add(ArgTo, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "Phone number to send SMS message to (self if empty)"));
                c_P.Add(ArgMsg, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The message to be sent"));
                c_P.Add(ArgList, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "List of documents to attach"));

                return new DescriptionClass(CategoriesClass.Comm, "Sends a SMS message", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "send.sms"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            string sTo = args.GetDefined(ArgTo);
            string sMsg = args.GetDefined(ArgMsg);
            string sList = args.GetDefined(ArgList);

            //Validate
            if (this.CheckValidity(ctx,
                                                ArgTo, sTo,
                                                ArgMsg, sMsg))
            {
                using (eMessageClass c_Msg = new eMessageClass(env))
                {
                    c_Msg.Message = sMsg;
                    c_Msg.To.Parse(sTo);
                    c_Msg.Attachments.Add(ctx.GetDocuments(sList), false);

                    c_Msg.Send();

                    eAns = ReturnClass.OK;
                }
            }

            return eAns;
        }
        #endregion
    }
}