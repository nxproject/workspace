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

namespace Common.TaskWF
{
    public class CategoriesClass
    {
        #region Constructor
        public CategoriesClass(string cat, string icon)
        {
            //
            this.Name = cat;
            this.Icon = icon;
        }
        #endregion

        #region Properties
        public string Name { get; private set; }
        public string Icon { get; private set; }
        #endregion

        #region Task Constants
        public static CategoriesClass Array { get { return new CategoriesClass("Array", "layer-group"); } }
        public static CategoriesClass Comm { get { return new CategoriesClass("Comm", "tty"); } }
        public static CategoriesClass DingDong { get { return new CategoriesClass("DingDong", "bell"); } }
        public static CategoriesClass Docs { get { return new CategoriesClass("Document", "file"); } }
        public static CategoriesClass DocList { get { return new CategoriesClass("Document List", "folder"); } }
        public static CategoriesClass External { get { return new CategoriesClass("External", "concierge-bell"); } }
        public static CategoriesClass Flow { get { return new CategoriesClass("Flow", "code-branch"); } }
        public static CategoriesClass FTP { get { return new CategoriesClass("FTP", "server"); } }
        public static CategoriesClass HTTP { get { return new CategoriesClass("HTTP", "desktop"); } }
        public static CategoriesClass Obj { get { return new CategoriesClass("Object", "vector-square"); } }
        public static CategoriesClass ObjList { get { return new CategoriesClass("Object List", "object-group"); } }
        public static CategoriesClass Ops { get { return new CategoriesClass("Ops", "plus-square"); } }
        public static CategoriesClass PDF { get { return new CategoriesClass("PDF", "file-pdf"); } }
        public static CategoriesClass Query { get { return new CategoriesClass("Query", "question"); } }
        public static CategoriesClass Store { get { return new CategoriesClass("Store", "database"); } }
        public static CategoriesClass Text { get { return new CategoriesClass("Text", "paragraph"); } }
        public static CategoriesClass Trace { get { return new CategoriesClass("Trace", "search"); } }
        public static CategoriesClass USPS { get { return new CategoriesClass("USPS", "envelope"); } }
        public static CategoriesClass Words { get { return new CategoriesClass("Word", "keyboard"); } }
        public static CategoriesClass Workflow { get { return new CategoriesClass("Workflow", "network-wired"); } }
        public static CategoriesClass TimeTrack { get { return new CategoriesClass("Time Track", "clock"); } }
        public static CategoriesClass Address { get { return new CategoriesClass("Address", "map-marker-alt"); } }
        #endregion

        #region Workflow COnstants
        public static CategoriesClass Action { get { return new CategoriesClass("Action", "dot-circle"); } }
        #endregion
    }
}
