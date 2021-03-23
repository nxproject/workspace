using System;
using System.Collections.Generic;
using System.Text;

namespace Proc.Communication
{
    public class eActionClass : IDisposable
    {
        #region Constructor
        public eActionClass(string caption, string task)
        {
            //
            this.Caption = caption;
            // TBD Create URL
        }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Properties
        /// <summary>
        /// 
        /// The display caption of the action
        /// 
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// 
        /// The callback URL
        /// 
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 
        /// Color of the button
        /// 
        /// </summary>
        public string Color { get; set; }
        #endregion
    }
}