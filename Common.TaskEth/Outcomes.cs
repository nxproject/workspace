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

using System;
using System.Collections.Generic;
using System.Text;

using NX.Shared;
using NX.Engine;

namespace Common.TaskETH
{
    public class OutcomesClass
    {
        #region Constructor
        public OutcomesClass(params string[] exits)
        {
            //
            this.Outcomes = new List<string>(exits);
        }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// the list of outcomes
        /// 
        /// </summary>
        public List<string> Outcomes { get; private set; }

        /// <summary>
        /// 
        /// The dynamic exit (if set, Outcoms list is not used)
        /// 
        /// </summary>
        public string DynamicOutcome { get; set; }
        #endregion

        #region Statics
        /// <summary>
        /// 
        /// The default 
        /// 
        /// </summary>
        /// <returns></returns>
        public static OutcomesClass TaskDefault()
        {
            return new OutcomesClass("Done", "OnError");
        }

        /// <summary>
        /// 
        /// The default plus extras 
        /// 
        /// </summary>
        /// <returns></returns>
        public static OutcomesClass TaskDefaultPlus(params string[] exits)
        {
            OutcomesClass c_Ans = OutcomesClass.TaskDefault();
            c_Ans.Outcomes.AddRange(exits);

            return c_Ans;
        }

        /// <summary>
        /// 
        /// The default 
        /// 
        /// </summary>
        /// <returns></returns>
        public static OutcomesClass WorkflowDefault()
        {
            return new OutcomesClass("Done");
        }

        /// <summary>
        /// 
        /// The default plus extras 
        /// 
        /// </summary>
        /// <returns></returns>
        public static OutcomesClass WorkflowDefaultPlus(params string[] exits)
        {
            OutcomesClass c_Ans = OutcomesClass.WorkflowDefault();
            c_Ans.Outcomes.AddRange(exits);

            return c_Ans;
        }

        /// <summary>
        /// 
        /// The default 
        /// 
        /// </summary>
        /// <returns></returns>
        public static OutcomesClass WorkflowDoneOnly()
        {
            return new OutcomesClass("Done");
        }

        /// <summary>
        /// 
        /// The only extras 
        /// </summary>
        /// <returns></returns>
        public static OutcomesClass Only(params string[] exits)
        {
            return new OutcomesClass(exits);
        }

        /// <summary>
        /// 
        /// The dynamic
        /// </summary>
        /// <returns></returns>
        public static OutcomesClass Dynamic(string exits)
        {
            OutcomesClass c_Ans = new OutcomesClass();
            c_Ans.DynamicOutcome = exits;

            return c_Ans;
        }

        /// <summary>
        /// 
        /// No exits
        /// 
        /// </summary>
        /// <returns></returns>
        public static OutcomesClass None()
        {
            return new OutcomesClass();
        }
        #endregion
    }
}
