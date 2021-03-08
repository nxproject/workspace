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
using NX.Engine.Files;
using Proc.AO;
using Proc.Comm;

namespace Proc.Task
{
    public class SelfSMSAnswer : CommandClass
    {
        #region Constants
        private const string ArgMsg = "msg";
        private const string ArgList = "doclist";
        private const string ArgTo = "to";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public SelfSMSAnswer(EnvironmentClass env)
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

                c_P.Add(ArgMsg, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The message to be sent"));
                c_P.Add(ArgTo, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "Number to send SMS to"));
                c_P.Add(ArgList, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "List of documents to attach"));

                return new DescriptionClass(CategoriesClass.Comm, "Sends SMS message to current user", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "selfsms.answer"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // 
            string sMsg = args.GetDefined(ArgMsg);
            string sList = args.GetDefined(ArgList);
            string sTo = args.GetDefined(ArgTo).IfEmpty(ctx.User.TwilioPhone);

            //Validate
            if (this.CheckValidity(ctx,
                                    ArgTo, sTo,
                                    ArgMsg, sMsg))
            {
                try
                {
                    List<string> c_Attachments = null;

                    if (sList.HasValue())
                    {
                        c_Attachments = ctx.DocumentLists[sList];
                    }

                    using (eMessageClass c_Msg = new eMessageClass(ctx))
                    {
                        c_Msg.Subject = "";
                        c_Msg.Message = sMsg;
                        c_Msg.To.Parse(sTo, eAddressClass.Types.SMS);
                        c_Msg.Attachments.Attach(sList);

                        c_Msg.Send();

                        eAns = ReturnClass.OK;
                    }

                    using (Proc.Comm.InterfaceClass c_Itx = new InterfaceClass(ctx))
                    {
                        if (c_Itx.Send(sTo, InterfaceClass.Preferences.SMS, "", sMsg, c_Attachments))
                        {
                            eAns = ReturnClass.OK;
                        }
                    }
                }
                catch { }
            }

            return eAns;
        }
        #endregion
    }
}