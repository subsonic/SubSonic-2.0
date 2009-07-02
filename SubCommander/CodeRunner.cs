/*
 * SubSonic - http://subsonicproject.com
 * 
 * The contents of this file are subject to the Mozilla Public
 * License Version 1.1 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a copy of
 * the License at http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or
 * implied. See the License for the specific language governing
 * rights and limitations under the License.
*/

using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;

namespace SubSonic.SubCommander
{
    /// <summary>
    /// Runs and executes code for use with our scripted bits - like Migrations
    /// </summary>
    public class CodeRunner
    {
        /// <summary>
        /// Executes the passed-in code
        /// </summary>
        /// <param name="lang">ICodeLanguage</param>
        /// <param name="sourceCode">the code</param>
        /// <param name="methodName">the entry point method</param>
        /// <param name="parameters">the params</param>
        public static void RunAndExecute(ICodeLanguage lang, string sourceCode, string methodName, object[] parameters)
        {
            Console.WriteLine("Compiling source code...");
            string[] source = new string[1];
            source[0] = sourceCode;
            CompilerParameters compileParams = new CompilerParameters();

            //Gonna create an assembly on the fly
            //so we need all the standard DLL's referenced from the GAC
            compileParams.ReferencedAssemblies.Add("System.Configuration.dll");
            compileParams.ReferencedAssemblies.Add("System.Web.dll");
            compileParams.ReferencedAssemblies.Add("System.Data.dll");
            compileParams.ReferencedAssemblies.Add("System.dll");
            compileParams.ReferencedAssemblies.Add("System.Xml.dll");

            //add a dash of COM interop
            compileParams.ReferencedAssemblies.Add("mscorlib.dll");

            //have to make sure SubSonic is added in  
            compileParams.ReferencedAssemblies.Add("SubSonic.dll");

            CompilerResults results = lang.CreateCodeProvider().CompileAssemblyFromSource(compileParams, source);

            if(results.Errors.Count > 0 || results.CompiledAssembly == null)
            {
                if(results.Errors.Count > 0)
                {
                    StringBuilder sbError = new StringBuilder();
                    foreach(CompilerError error in results.Errors)
                        sbError.AppendLine(error.ErrorText);

                    //fails
                    throw new Exception("Compile errors: \r\n" + sbError);
                }
                if(results.CompiledAssembly == null)
                    throw new Exception("Compiler errors: the code won't compile");
                return;
            }

            Console.WriteLine("Done!");
            Console.WriteLine("Executing " + methodName);

            const string stubTypeName = "SubSonic.MigrationRunner";
            //instance up the class
            object instance = results.CompiledAssembly.CreateInstance(stubTypeName);
            Type instanceType = instance.GetType();

            //grab the method we're looking for
            MethodInfo method = instanceType.GetMethod(methodName);

            method.Invoke(instance, parameters);
        }
    }
}