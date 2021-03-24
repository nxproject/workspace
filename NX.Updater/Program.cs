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

using NX.Engine;
using NX.Shared;
using NX.Engine.Hive;

namespace NXUpdater
{
    class Program
    {
        #region Entry point
        static void Main(string[] args)
        {
            // Create working environment
            EnvironmentClass c_Env = new EnvironmentClass(args);

            // Make the processor name
            DockerIFNameClass c_Name = DockerIFNameClass.Make(c_Env, HiveClass.ProcessorDNAName);

            // Flag no change
            bool bRefresh = false;
            // And the count
            int iCount = 0;

            // Get the field
            foreach (FieldClass c_Field in c_Env.Hive.Fields.Values)
            {
                // Gte client
                DockerIFClass c_Client = c_Field.DockerIF;
                // Any?
                if (c_Client != null)
                {
                    // Get the creation date
                    long lCreOn = c_Client.GetImageCreatedDate(c_Name);
                    // Move to Docjer hub
                    c_Name.Repo = "docker.io";
                    // Get the date
                    long lDCreon = c_Client.GetImageCreatedDate(c_Name, true);
                    // New one?
                    if (lDCreon > lCreOn)
                    {
                        // Flag
                        bRefresh = true;
                        // Pull
                        c_Client.PullImage(c_Name, delegate (bool done)
                        {
                            iCount++;
                        });
                    }
                }
            }

            // Do we refresh?
            if (bRefresh)
            {
                // No ore than an hour
                DateTime c_By = DateTime.Now.AddHours(1);

                // Wait until all images are pulled
                while (iCount < c_Env.Hive.Fields.Count && c_By >= DateTime.Now)
                {
                    // Wait
                    1.MinutesAsTimeSpan().Sleep();
                }

                // All done?
                if (iCount == c_Env.Hive.Fields.Count)
                {
                    // Kill processor
                    c_Env.Hive.KillDNA(HiveClass.ProcessorDNAName);
                    // And restart one
                    c_Env.Hive.MakeBee();
                }
            }

            c_Env.LogInfo("Update completed");
        }
        #endregion
    }
}
