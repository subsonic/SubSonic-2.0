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
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the TurboCompiler class
    /// </summary>
    public class TurboCompiler
    {
        private static readonly Regex regLineFix = new Regex(@"[\r\n]+", RegexOptions.Compiled);
        private static readonly Regex regNamespace = new Regex(@"^\bnamespace\b", RegexOptions.Compiled);

        private readonly TurboTemplateCollection templates = new TurboTemplateCollection();
        private CompilerParameters codeCompilerParameters;

        private CodeDomProvider codeProvider;
        internal StringBuilder errMsg = new StringBuilder();
        internal ICodeLanguage Language = new CSharpCodeLanguage();
        internal StringCollection References = new StringCollection();

        /// <summary>
        /// Gets the templates.
        /// </summary>
        /// <value>The templates.</value>
        public TurboTemplateCollection Templates
        {
            get { return templates; }
        }

        /// <summary>
        /// Gets the code provider.
        /// </summary>
        /// <value>The code provider.</value>
        private CodeDomProvider CodeProvider
        {
            get
            {
                if(codeProvider == null)
                    codeProvider = Language.CreateCodeProvider();
                return codeProvider;
            }
        }

        /// <summary>
        /// Gets the code compiler parameters.
        /// </summary>
        /// <value>The code compiler parameters.</value>
        private CompilerParameters CodeCompilerParameters
        {
            get
            {
                if(codeCompilerParameters == null)
                {
                    codeCompilerParameters = new CompilerParameters
                                                 {
                                                     CompilerOptions = "/target:library",
                                                     GenerateExecutable = false,
                                                     GenerateInMemory = true,
                                                     IncludeDebugInformation = false
                                                 };
                    codeCompilerParameters.ReferencedAssemblies.Add("mscorlib.dll");
                    foreach(string s in References)
                        codeCompilerParameters.ReferencedAssemblies.Add(s);
                }

                return codeCompilerParameters;
            }
        }

        /// <summary>
        /// Adds the template.
        /// </summary>
        /// <param name="template">The template.</param>
        public void AddTemplate(TurboTemplate template)
        {
            if(template != null)
            {
                if(templates.Count == 0)
                {
                    References = template.References;
                    Language = template.CompileLanguage;
                }

                template.EntryPoint = "Render";
                template.GeneratedRenderType = String.Concat("Parser", Templates.Count);
                template.TemplateText =
                    Utility.FastReplace(template.TemplateText, "#TEMPLATENUMBER#", Templates.Count.ToString(), StringComparison.InvariantCultureIgnoreCase);
                templates.Add(template);
            }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            templates.Clear();
        }

        /// <summary>
        /// Runs the and execute.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        /// <param name="methodName">Name of the method.</param>
        public void RunAndExecute(string sourceCode, string methodName)
        {
            // TODO: And what again is methodName doing? It's not used anywhere...
            // Utility.WriteTrace("Compiling migration code...");
            // string[] source = new string[1];
            // source[0] = sourceCode;
            // CompilerResults results = CodeProvider.CompileAssemblyFromSource(CodeCompilerParameters, source);
            // Utility.WriteTrace("Done!");
            // MethodInfo method=results.CompiledAssembly.GetType().GetMethod(methodName);
            // method.Invoke();
        }

        /// <summary>
        /// Runs the and execute.
        /// </summary>
        /// <param name="sourceCode">The source code.</param>
        public void RunAndExecute(string sourceCode)
        {
            RunAndExecute(sourceCode, "Main");
        }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            int templateCount = Templates.Count;
            if(templateCount > 0)
            {
                ClearErrorMessages();

                string[] templateArray = new string[templateCount];
                for(int i = 0; i < templateCount; i++)
                    templateArray[i] = Templates[i].TemplateText;
                Utility.WriteTrace("Compiling assembly...");
                CompilerResults results = CodeProvider.CompileAssemblyFromSource(CodeCompilerParameters, templateArray);
                Utility.WriteTrace("Done!");

                if(results.Errors.Count > 0 || results.CompiledAssembly == null)
                {
                    if(results.Errors.Count > 0)
                    {
                        foreach(CompilerError error in results.Errors)
                            LogErrorMessages("Compile Error: " + error.ErrorText);
                    }
                    if(results.CompiledAssembly == null)
                    {
                        const string errorMessage = "Error generating template code: This usually indicates an error in template itself, such as use of reserved words. Detail: ";
                        Utility.WriteTrace(errorMessage + errMsg);
                        string sMessage = errorMessage + Environment.NewLine + errMsg;
                        throw new Exception(sMessage);
                    }
                    return;
                }

                Utility.WriteTrace("Extracting code from assembly and scrubbing output...");
                CallEntry(results.CompiledAssembly);
                Utility.WriteTrace("Done!");
            }
        }

        /// <summary>
        /// Scrubs the output.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        private static string ScrubOutput(string result, ICodeLanguage language)
        {
            if(!String.IsNullOrEmpty(result))
            {
                // the generator has an issue with adding extra lines. Trim them out
                result = regLineFix.Replace(result, "\r\n");

                // now, for readability, add a space after the end of the method/class
                if(language is CSharpCodeLanguage)
                {
                    result = regNamespace.Replace(result, "\r\nnamespace");
                    result = Utility.FastReplace(result, "public class ", "\r\npublic class ", StringComparison.InvariantCulture);
                }
                else
                {
                    // trailing space needed to address class names that begin with "Class"
                    result = Utility.FastReplace(result, "Public Class ", "\r\nPublic Class ", StringComparison.InvariantCulture);
                }

                result = Utility.FastReplace(result, "[<]", "<", StringComparison.InvariantCultureIgnoreCase);
                result = Utility.FastReplace(result, "[>]", ">", StringComparison.InvariantCultureIgnoreCase);

                // This is should be executed last. While this value will ultimately be removed, it can be inserted in a template to keep an earlier operation from executing.
                // For example: <System.ComponentModel.DataObject()> Public Class MyController would normally wrap to a second line due upstream processing, which would
                // result in VB code that won't compile. However, <System.ComponentModel.DataObject()> Public [MONKEY_WRENCH]Class MyController, would not.

                // Nice Eric... :P
                // result = Utility.FastReplace(result, "[MONKEY_WRENCH]", String.Empty, StringComparison.InvariantCultureIgnoreCase);
                // Not currently used, but please leave in case we need it in the future!
            }
            return result;
        }

        /// <summary>
        /// Calls the entry.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        private void CallEntry(Assembly assembly)
        {
            int templateCount = Templates.Count;
            Module mod = assembly.GetModules(false)[0];
            for(int i = 0; i < templateCount; i++)
            {
                Type type = mod.GetType(Templates[i].GeneratedRenderType);
                if(type != null)
                {
                    MethodInfo mi = type.GetMethod(Templates[i].EntryPoint, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    if(mi != null)
                    {
                        StringBuilder returnText = new StringBuilder();

                        string resultMessage = String.Format("Successfully generated {0}", Templates[i].TemplateName);
                        if(Templates[i].AddUsingBlock)
                        {
                            Utility.WriteTrace("Adding Custom Using Blocks");
                            returnText.Append(Templates[i].RenderLanguage.DefaultUsingStatements);
                            returnText.Append(Templates[i].CustomUsingBlock);
                        }
                        try
                        {
                            returnText.Append((string)mi.Invoke(null, null));
                        }
                        catch(Exception ex)
                        {
                            resultMessage = "An Exception occured in template " + Templates[i].TemplateName + ": " + ex.Message;
                        }

                        Templates[i].FinalCode = ScrubOutput(returnText.ToString(), Templates[i].RenderLanguage);
                        Utility.WriteTrace(resultMessage);
                    }
                }
            }
        }

        /// <summary>
        /// Logs the error messages.
        /// </summary>
        /// <param name="customMessage">The custom message.</param>
        internal void LogErrorMessages(string customMessage)
        {
            LogErrorMessages(customMessage, null);
        }

        /// <summary>
        /// Logs the error messages.
        /// </summary>
        /// <param name="customMessage">The custom message.</param>
        /// <param name="ex">The ex.</param>
        internal void LogErrorMessages(string customMessage, Exception ex)
        {
            // put the error message into builder
            errMsg.Append("\r\n").Append(customMessage).Append(Environment.NewLine);

            // get all the exceptions and add their error messages
            while(ex != null)
            {
                errMsg.Append("\t").Append(ex.Message).Append(Environment.NewLine);
                ex = ex.InnerException;
            }
        }

        /// <summary>
        /// Clears the error messages.
        /// </summary>
        internal void ClearErrorMessages()
        {
            errMsg.Remove(0, errMsg.Length);
        }
    }
}