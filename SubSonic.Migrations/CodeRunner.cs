using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SubSonic
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
            string[] source = new string[1];
            source[0] = sourceCode;
            CompilerParameters compileParams = new CompilerParameters();
            compileParams.GenerateInMemory = true;

            //Gonna create an assembly on the fly
            //so we need all the standard DLL's referenced from the GAC
            compileParams.ReferencedAssemblies.Add("System.Configuration.dll");
            compileParams.ReferencedAssemblies.Add("System.Web.dll");
            compileParams.ReferencedAssemblies.Add("System.Data.dll");
            compileParams.ReferencedAssemblies.Add("System.dll");
            compileParams.ReferencedAssemblies.Add("System.Xml.dll");

            //add a dash of COM interop
            compileParams.ReferencedAssemblies.Add("mscorlib.dll");

            //have to make sure SubSonic is added in.  Since this assembly is
            //generated in memory it needs to be told exactly where the SubSonic.dll is
            //because SubSonic.dll doesn't live in the Framework folder, so lets grab
            //the same SubSonic.dll that is already loaded and use that one.  Not
            //sure how much of a hack this is but it works for me.            
            //
            //this is a little sketch because if the SubSonic.dll hasn't been
            //loaded yet then this will fail.
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly subsonicAssembly = Array.Find(
                assemblies,
                delegate(Assembly a)
                    {
                        AssemblyName an = a.GetName();
                        return an.Name == "SubSonic";
                    }
                );

            if(subsonicAssembly == null)
                throw new Exception("Unable to location the SubSonic.dll.  Make sure it's referenced in your project.");

            Uri subsonicUri = new Uri(subsonicAssembly.CodeBase);
            compileParams.ReferencedAssemblies.Add(subsonicUri.LocalPath);

            //compile the assembly
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

            string typeName = GetTypeName(sourceCode, lang);
            //instance up the class
            object instance = results.CompiledAssembly.CreateInstance(typeName);

            //if the instance is null, it means we haven't parsed the namespace/classname properly
            if(instance == null)
                throw new InvalidOperationException("SubSonic was not able to parse the namespace/class name of your Migration class properly - cannot find it: " + typeName);

            Type instanceType = instance.GetType();

            //grab the method we're looking for
            MethodInfo method = instanceType.GetMethod(methodName);
            method.Invoke(instance, parameters);
        }

        /// <summary>
        /// Gets the name of the type.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="lang">The lang.</param>
        /// <returns></returns>
        private static string GetTypeName(string source, ICodeLanguage lang)
        {
            string classReplacement;
            string namespaceReplacement;
            Regex classRegex;
            Regex namespaceRegex;

            if (lang is VBCodeLanguage)
            {
                classReplacement = "${Class}";
                namespaceReplacement = "${Namespace}";
                classRegex = new Regex(@"Class (?<Class>\w*)");
                namespaceRegex = new Regex(@"Namespace (?<Namespace>[a-zA-Z0-9.-[{]]*)");
            }
            else
            {
                classReplacement = "${class}";
                namespaceReplacement = "${namespace}";
                classRegex = new Regex(@"class (?<class>\w*)");
                //many thanks to rballonline!!!
                namespaceRegex = new Regex(@"namespace (?<namespace>[a-zA-Z0-9.-[{]]*)");
            }

            string result = String.Empty;
            const string resultFormat = "{0}.{1}";

            Match namespaceMatch = namespaceRegex.Match(source);
            Match classMatch = classRegex.Match(source);

            if(classMatch.Success && namespaceMatch.Success)
                result = string.Format(resultFormat, namespaceMatch.Result(namespaceReplacement), classMatch.Result(classReplacement));
            return result;
        }
    }
}