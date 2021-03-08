﻿///--------------------------------------------------------------------------------
/// 
/// Copyright (C) 2020 Jose E. Gonzalez (jegbhe@gmail.com) - All Rights Reserved
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

/// Packet Manager Requirements
/// 
/// Install-Package Newtonsoft.Json -Version 12.0.3
/// 

using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;
using Cronos;

using NX.Shared;
using NX.Engine;

namespace Proc.Cron
{
    public class ExpressionClass : ChildOfClass<CronEntryClass>
    {
        #region Constants
        private const string DefaultExpression = "* * * * *";
        #endregion

        #region Constructor
        public ExpressionClass(CronEntryClass entry)
            : base(entry)
        {
            this.Pattern = DefaultExpression;
        }
        #endregion

        #region Properties
        public string Pattern
        {
            get { return this.Parent.Pattern; }
            set { this.Parent.Pattern = value; }
        }

        public DateTime StartOn
        {
            get { return this.Parent.StartOn; }
            set { this.Parent.StartOn = value; }
        }

        public bool Enabled
        {
            get { return this.Parent.Enabled; }
            set { this.Parent.Enabled = value; }
        }

        public DateTime Next
        {
            get { return this.Parent.NextOn; }
            set { this.Parent.NextOn = value; }
        }

        public List<string> NextList
        {
            get { return this.Parent.ScheduledOn.SplitSpaces(); }
            set { this.Parent.ScheduledOn = value.Join(" "); }
        }
        #endregion

        #region Methods
        public void Compute(TimeZoneInfo tz, int count)
        {
            CronExpression expression = CronExpression.Parse(this.Pattern);

            JArray c_TBD = new JArray();

            DateTime c_Start = this.StartOn.ToUniversalTime();
            if (c_Start < DateTime.Now) c_Start = DateTime.Now.ToUniversalTime();
            if (count < 1) count = 1;

            DateTime? c_Next = expression.GetNextOccurrence(c_Start, tz);
            while (c_Next != null && c_TBD.Count < count)
            {
                c_Start = (DateTime)c_Next;
                c_TBD.Add(c_Start.ToUniversalTime().ToDBDate());
                c_Start = c_Start.AddSeconds(1);
                c_Next = expression.GetNextOccurrence(c_Start, tz);
            }

            this.NextList = c_TBD.ToList();
            if (c_TBD.Count == 0)
            {
                this.Next = DateTime.MaxValue;
            }
            else
            {
                this.Next = this.NextList[0].FromDBDate();
            }
        }
        #endregion
    }
}