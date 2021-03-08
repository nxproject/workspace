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

using System.Text;

using Newtonsoft.Json.Linq;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class SendFax : CommandClass
    {
        #region Constants
        private const string ArgTo = "to";
        private const string ArgMsg = "msg";
        private const string ArgSubj = "subj";
        private const string ArgList = "list";
        private const string ArgCO = "co";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public SendFax(EnvironmentClass env)
            : base(env)
        { }
        #endregion

        #region Properties
        #endregion

        #region Methods
        private string GenerateFooter(bool usesign, AO.Definitions.UserClass assoc, string footer)
        {
            StringBuilder c_Buffer = new StringBuilder();

            if (usesign)
            {
                if (assoc.Displayable.HasValue()) c_Buffer.AppendLine(assoc.Displayable);
                if (assoc.Title.HasValue()) c_Buffer.AppendLine(assoc.Title);
                if (assoc.EMailName.HasValue()) c_Buffer.AppendLine("E-Mail: " + assoc.EMailName);
                if (assoc.TwilioPhone.HasValue()) c_Buffer.AppendLine("Phone: " + assoc.TwilioPhone);
            }

            if (footer.HasValue())
            {
                if (c_Buffer.Length > 0) c_Buffer.AppendLine("");
                c_Buffer.AppendLine(footer);
            }

            return c_Buffer.ToString();
        }
        #endregion

        #region IStep
        public override DescriptionClass Description
        {
            get
            {
                NamedListClass<ParameterDefinitionClass> c_P = new NamedListClass<ParameterDefinitionClass>();

                c_P.Add(ArgTo, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "Phone # to send the fax to"));
                c_P.Add(ArgCO, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "Name in c/o"));
                c_P.Add(ArgSubj, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The subject of the message to be sent"));
                c_P.Add(ArgMsg, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Required, "The message to be sent"));
                c_P.Add(ArgList, new ParameterDefinitionClass(ParameterDefinitionClass.Types.Optional, "List of documents to attach"));

                return new DescriptionClass(CategoriesClass.Comm, "Sends a Fax", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "send.fax"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Get from system store
            StoreClass c_Sys = ctx.Stores[Names.Sys];

            string sTo = args.GetDefined(ArgTo).IfEmpty(ctx.User.Name);
            string sCO = args.GetDefined(ArgCO);
            string sSubj = args.GetDefined(ArgSubj);
            string sMsg = args.GetDefined(ArgMsg);
            string sList = args.GetDefined(ArgList);

            //Validate
            if (this.CheckValidity(ctx,
                                        ArgTo, sTo,
                                        ArgMsg, sMsg))
            {
                using (eMessageClass c_Msg = new eMessageClass(ctx))
                {
                    c_Msg.Subject = sSubj;
                    c_Msg.Message = sMsg;
                    c_Msg.To.Parse(sTo, eAddressClass.Types.Fax);
                    c_Msg.Attachments.Add(ctx.DocumentLists[sList].Reachable);

                    c_Msg.Send();

                    eAns = ReturnClass.OK;
                }
            }

            return eAns;
        }
        #endregion
    }
}