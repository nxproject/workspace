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
/// Install-Package TweetSharp -Version 2.3.1
/// 

using System;
using System.Collections.Generic;

using TweetSharp;

using NX.Shared;
using NX.Engine;
using Common.TaskWF;

namespace Proc.Task
{
    public class TweetClass : CommandClass
    {
        #region Constants
        private const string KeyKey = "consumerKey";
        // MultiModal =  KhGmurVwlu87BoaGoJSETm9oB
        private const string KeySecret = "consumerSecret";
        // MultiModal = CocEoO1TzN5s9SveErDu7iEX3Mmn9mc85zfv0VRm8g0LVdyR15
        private const string KeyAccess = "accessToken";
        private const string KeyAccessSecret = "accessTokenSecret";

        private const string ArgMsg = "msg";

        //private const string ArgIXX = "if";
        #endregion

        #region Constructor
        public TweetClass()
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
                NamedListClass<ParamDefinitionClass> c_P = new NamedListClass<ParamDefinitionClass>();


                c_P.Add(KeyKey, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "Your consumer key from Twitter"));
                c_P.Add(KeySecret, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "Your secret key from Twitter"));
                c_P.Add(KeyAccess, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "Your access token from Twitter"));
                c_P.Add(KeyAccessSecret, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "Your access token secret from Twitter"));

                c_P.Add(ArgMsg, new ParamDefinitionClass(ParamDefinitionClass.Types.Required, "The message to be tweeted"));

                this.AddSystem(c_P);

                return new DescriptionClass(CategoriesClass.External, "Sends a tweet", c_P);
            }
        }
        #endregion

        #region Code Line
        public override string Command
        {
            get { return "send.tweet"; }
        }

        public override ReturnClass ExecStep(TaskContextClass ctx, ArgsClass args)
        {
            //
            ReturnClass eAns = ReturnClass.Done;

            // Do
            TwitterService c_Twitter;
            try
            {
                // Create service
                c_Twitter = new TwitterService();
                // Get site info
                AO.SiteInfoClass c_Info = ctx.SiteInfo;
                // Login
                c_Twitter.AuthenticateWith(c_Info.TwitterConsumerKey,
                    c_Info.TwitterSecretKey,
                    c_Info.TwitterAccessToken,
                    c_Info.TwitterAccessTokenSecret);
            }
            catch (Exception e)
            {
                eAns = ReturnClass.Failure("Twitter authentication: {0}", e);
                c_Twitter = null;
            }

            if (c_Twitter != null)
            {
               try
                {
                    var tweet = c_Twitter.SendTweet(new SendTweetOptions { Status = args.Get(ArgMsg) });
                }
                catch (Exception e)
                {
                    eAns = ReturnClass.Failure("During send: {0}", e);
                }
            }

            return eAns;
        }
        #endregion
    }
}