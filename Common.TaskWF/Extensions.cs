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
    public interface ITaskWF
    {
        /// <summary>
        /// 
        /// The command
        /// 
        /// </summary>
        string Command { get; }

        /// <summary>
        /// 
        /// The description
        /// 
        /// </summary>
        DescriptionClass Description { get; }
    }

    public static class Extensions
    {
        /// <summary>
        /// 
        /// Generate Elsa code
        /// 
        /// </summary>
        /// <returns></returns>
        public static Tuple<string, string> GenerateElsa(this ITaskWF value)
        {
            //
            string sName = null;
            string sCall = null;

            try
            {
                // Name is simple
                sName = value.Command.AlphaNumOnly();

                // The call itself
                sCall = " private " + sName + " = (): ActivityDefinition => ({";

                sCall += " type: '{0}', ".FormatString(value.Command);
                sCall += " displayName: '{0},', ".FormatString(value.Command);
                sCall += " description: '{0},', ".FormatString(value.Description.Description.Replace("'", ""));

                if (value.Description.RuntimeDescription.HasValue())
                {
                    sCall += " runtimeDescription: '{0}', ".FormatString(value.Description.RuntimeDescription.Replace("'", ""));
                }

                sCall += " category: '{0}', ".FormatString(value.Description.Category.Name);
                sCall += " icon: 'fas fa-{0}', ".FormatString(value.Description.Category.Icon);

                // Outcomes
                string sOutcomes = "";
                if (value.Description.Outcomes != null)
                {
                    // Use the dynamic
                    sOutcomes = value.Description.Outcomes.DynamicOutcome.IfEmpty();
                    if (!sOutcomes.HasValue())
                    {
                        foreach (string sOut in value.Description.Outcomes.Outcomes)
                        {
                            if (sOutcomes.Length > 0) sOutcomes += ",";
                            sOutcomes += "OutcomeNames." + sOut;
                        }

                        sOutcomes = "[" + sOutcomes + "]";
                    }
                    else
                    {
                        sOutcomes = "'" + sOutcomes + "'";
                    }
                }

                if (sOutcomes.HasValue()) sCall += " outcomes: " + sOutcomes + ", ";

                // Parameters
                string sProps = "[";
                if (value.Description.Parameters != null)
                {
                    // Loop thru
                    foreach (string sP in value.Description.Parameters.Keys)
                    {
                        // Get
                        ParamDefinitionClass c_P = value.Description.Parameters[sP];
                        //
                        if (sProps.Length != 1) sProps += ", ";
                        sProps += "{";
                        sProps += " name: '{0}',".FormatString(sP);
                        sProps += " type: '{0}',".FormatString(c_P.ElsaType);
                        sProps += " label: '{0}',".FormatString(WesternNameClass.CapFirstWord(sP));

                        if (c_P.Choices != null)
                        {
                            sProps += " options: { items: " + c_P.Choices.ToJArray().ToSimpleString() + " },";
                        }

                        sProps += " hint: '{0}'".FormatString(c_P.Description);
                        sProps += "}";

                    }
                }

                sCall += "  properties: " + sProps + "]});\n";
            }
            catch 
            {
                sCall = "ERRPOR";
            }

            return new Tuple<string, string>(sName, sCall);
        }
    }
}
