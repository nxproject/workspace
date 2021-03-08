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
using System.Text.RegularExpressions;

using Newtonsoft.Json.Linq;
using HandlebarsDotNet;

using NX.Shared;
using NX.Engine;

namespace Proc.Office
{
    public static class ExtensionsClass
    {
        #region Handlebars
        /// <summary>
        /// 
        /// Has the handlebars extension been initioalized?
        /// 
        /// </summary>
        private static bool IsHandlebarsInit { get; set; }

        /// <summary>
        /// 
        /// Initialize handlebars extensions
        /// 
        /// </summary>
        private static void HandlebarsInit()
        {
            // Only once
            if (!IsHandlebarsInit)
            {
                // Flag
                IsHandlebarsInit = true;

                // If
                HandlebarsDotNet.Handlebars.RegisterHelper("iff", (output, options, context, arguments) =>
                {
                    if (arguments.Length != 3)
                    {
                        throw new HandlebarsException("{{#iff}} helper must have exactly three arguments");
                    }

                    // Params
                    var field = arguments.At<string>(0);
                    var op = arguments.At<string>(1);
                    var value = arguments.At<string>(2);

                    // Get the field value
                    var fieldvalue = context.GetValue<string>(field);

                    // Assume failure
                    bool bCmp = false;

                    // Now according to op
                    switch (op)
                    {
                        case "=":
                            bCmp = value.IsSameValue(fieldvalue);
                            break;

                        case "==":
                            bCmp = value.IsExactSameValue(fieldvalue);
                            break;

                        case "!=":
                            bCmp = !value.IsSameValue(fieldvalue);
                            break;

                        case "!==":
                            bCmp = !value.IsExactSameValue(fieldvalue);
                            break;

                        case ">":
                            bCmp = fieldvalue.CompareTo(value) > 0;
                            break;

                        case ">=":
                            bCmp = fieldvalue.CompareTo(value) >= 0;
                            break;

                        case "<":
                            bCmp = fieldvalue.CompareTo(value) < 0;
                            break;

                        case "<=":
                            bCmp = fieldvalue.CompareTo(value) <= 0;
                            break;
                    }

                    // Do
                    if (bCmp)
                    {
                        options.Template(output, context);
                    }
                    else
                    {
                        options.Inverse(output, context);
                    }
                });

                // Is
                HandlebarsDotNet.Handlebars.RegisterHelper("is", (output, options, context, arguments) =>
                {
                    if (arguments.Length != 2)
                    {
                        throw new HandlebarsException("{{#is}} helper must have exactly two arguments");
                    }

                    // Params
                    var field = arguments.At<string>(0);
                    var value = arguments.At<string>(1);

                    // Get the field value
                    var fieldvalue = context.GetValue<string>(field);

                    // Check
                    bool bCmp = value.IsSameValue(fieldvalue);

                    // Do
                    if (bCmp)
                    {
                        options.Template(output, context);
                    }
                    else
                    {
                        options.Inverse(output, context);
                    }
                });

                // Is Not
                HandlebarsDotNet.Handlebars.RegisterHelper("isnt", (output, options, context, arguments) =>
                {
                    if (arguments.Length != 2)
                    {
                        throw new HandlebarsException("{{#isnt}} helper must have exactly two arguments");
                    }

                    // Params
                    var field = arguments.At<string>(0);
                    var value = arguments.At<string>(1);

                    // Get the field value
                    var fieldvalue = context.GetValue<string>(field);

                    // Check
                    bool bCmp = !value.IsSameValue(fieldvalue);

                    // Do
                    if (bCmp)
                    {
                        options.Template(output, context);
                    }
                    else
                    {
                        options.Inverse(output, context);
                    }
                });
            }
        }

        /// <summary>
        /// 
        /// Processes a handlerbars text using the given values
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Handlebars(this string text, JObject values)
        {
            // Init
            HandlebarsInit();

            // Change the markers
            text = text.IfEmpty().Replace("[[", "{{").Replace("]]", "}}");
            // Compile
            var c_Template = HandlebarsDotNet.Handlebars.Compile(text);
            // Process
            text = c_Template(values.ToDictionary());

            return text;
        }
        #endregion
    }
}