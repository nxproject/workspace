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
/// Install-Package IronPython -Version 2.7.11
/// 

using System;
using System.Collections.Generic;

using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

using NX.Shared;
using NX.Engine;

namespace Proc.Task
{
    public class PythonClass : IDisposable
    {
        #region Constants
        #endregion

        #region Constructor
        public PythonClass()
        {
            pyEngine = IronPython.Hosting.Python.CreateEngine();
        }
        #endregion

        #region Properties
        private ScriptEngine pyEngine { get; set; }
        //private ScriptRuntime pyRuntime { get; set; }
        private ScriptScope pyScope { get; set; }
        #endregion

        #region IDisposable
        public void Dispose()
        { }
        #endregion

        #region Methods
        public bool Execute(object env, object values, string code)
        {
            bool bAns = false;

            try
            {
                pyScope = pyEngine.CreateScope();
                pyScope.SetVariable("env", env);
                pyScope.SetVariable("args", values);

                ScriptSource source = pyEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                CompiledCode compiled = source.Compile();
                // Executes in the scope of Python
                compiled.Execute(pyScope);

                bAns = true;
            }
            catch { }

            return bAns;
        }
        #endregion
    }
}
