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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SubSonic
{
    /// <summary>
    /// Summary for the TurboTemplateCollection class
    /// </summary>
    [Serializable]
    public class TurboTemplateCollection : List<TurboTemplate> {}

    /// <summary>
    /// Summary for the TurboTemplate class
    /// </summary>
    [Serializable]
    public class TurboTemplate
    {
        private const string FOOTER =
            @"

                    StreamReader sr = new StreamReader(mStream); 
			        writer.Flush();
			        mStream.Position = 0;
			        return sr.ReadToEnd();
		        }
	        }";

        private const string HEADER =
            @"using System;
            using System.Text.RegularExpressions;
            using System.Collections;
            using System.IO;
            using System.Text;
            using SubSonic;
            using System.Data;
            using System.Configuration;
            using SubSonic.Utilities;
            
	        public class Parser#TEMPLATENUMBER#
	        {
		        public static string Render()
		        {
			        MemoryStream mStream = new MemoryStream();
			        StreamWriter writer = new StreamWriter(mStream, System.Text.Encoding.UTF8);
                    writer.AutoFlush = false;

                ";

        private const string WRITER_CLOSE = ");";
        private const string WRITER_OPEN = "\t\t\twriter.Write(";

        private static readonly Regex regexAssembly = new Regex("(?i)<%@\\s*Assembly.*?%>", RegexOptions.Compiled);
        private static readonly Regex regexCleanCalls = new Regex(@"<%=.*?%>", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex regexCleanCodeTags = new Regex(@"<%[^=|@].*?%>", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex regexCodeTemplate = new Regex("(?i)<%@\\s*CodeTemplate.*?%>", RegexOptions.Compiled);
        private static readonly Regex regexEmptyBrackets = new Regex(@"<%%>", RegexOptions.Singleline | RegexOptions.Compiled);
        private static readonly Regex regexImport = new Regex("(?i)<%@\\s*Import.*?%>", RegexOptions.Compiled);
        private static readonly Regex regexPage = new Regex("(?i)<%@\\s*Page.*?%>", RegexOptions.Compiled);
        private static readonly Regex regexProperty = new Regex("(?i)<%@\\s*Property.*?%>", RegexOptions.Compiled);
        private static readonly Regex regexWriterBegin = new Regex("<%=", RegexOptions.Compiled);
        private static readonly Regex regexWriterEnd = new Regex("%>", RegexOptions.Compiled);

        private static ICodeLanguage compileLanguage;

        private static StringCollection references;
        private readonly string customUsingBlock = String.Empty;

        private readonly ICodeLanguage renderLanguage = new CSharpCodeLanguage();
        private bool addUsingBlock = true;
        private string entryPoint = "Render";
        private string finalCode;
        private string generatedRenderType = "Parser";
        private string outputPath;
        private string templateName = String.Empty;
        private string templateText;

        /// <summary>
        /// Initializes a new instance of the <see cref="TurboTemplate"/> class.
        /// </summary>
        /// <param name="templateInputText">The template input text.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        public TurboTemplate(string templateInputText, ICodeLanguage language, DataProvider provider)
        {
            renderLanguage = language;
            templateText = CleanTemplate(String.Concat(templateInputText, "<%%>"), ref renderLanguage);
            if(provider.AdditionalNamespaces.Length > 0)
                customUsingBlock = renderLanguage.GetUsingStatements(provider.AdditionalNamespaces);
        }

        /// <summary>
        /// Gets the references.
        /// </summary>
        /// <value>The references.</value>
        public StringCollection References
        {
            get
            {
                if(references == null)
                {
                    references = new StringCollection
                                     {
                                         "System.dll",
                                         "System.Data.dll",
                                         "System.Drawing.dll",
                                         "System.Xml.dll",
                                         "System.Windows.Forms.dll",
                                         "System.Configuration.dll",
                                         typeof(TurboTemplate).Assembly.Location
                                     };
                }

                return references;
            }
        }

        /// <summary>
        /// Gets the render language.
        /// </summary>
        /// <value>The render language.</value>
        public ICodeLanguage RenderLanguage
        {
            get { return renderLanguage; }
        }

        /// <summary>
        /// Gets the compile language.
        /// </summary>
        /// <value>The compile language.</value>
        public ICodeLanguage CompileLanguage
        {
            get
            {
                if(compileLanguage == null)
                    compileLanguage = new CSharpCodeLanguage();

                return compileLanguage;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [add using block].
        /// </summary>
        /// <value><c>true</c> if [add using block]; otherwise, <c>false</c>.</value>
        public bool AddUsingBlock
        {
            get { return addUsingBlock; }
            set { addUsingBlock = value; }
        }

        /// <summary>
        /// Gets the custom using block.
        /// </summary>
        /// <value>The custom using block.</value>
        public string CustomUsingBlock
        {
            get { return customUsingBlock; }
        }

        /// <summary>
        /// Gets or sets the entry point.
        /// </summary>
        /// <value>The entry point.</value>
        public string EntryPoint
        {
            get { return entryPoint; }
            set { entryPoint = value; }
        }

        /// <summary>
        /// Gets or sets the type of the generated render.
        /// </summary>
        /// <value>The type of the generated render.</value>
        public string GeneratedRenderType
        {
            get { return generatedRenderType; }
            set { generatedRenderType = value; }
        }

        /// <summary>
        /// Gets or sets the template text.
        /// </summary>
        /// <value>The template text.</value>
        public string TemplateText
        {
            get { return templateText; }
            set { templateText = value; }
        }

        /// <summary>
        /// Gets or sets the name of the template.
        /// </summary>
        /// <value>The name of the template.</value>
        public string TemplateName
        {
            get { return templateName; }
            set { templateName = value; }
        }

        /// <summary>
        /// Gets or sets the final code.
        /// </summary>
        /// <value>The final code.</value>
        public string FinalCode
        {
            get { return finalCode; }
            set { finalCode = value; }
        }

        /// <summary>
        /// Gets or sets the output path.
        /// </summary>
        /// <value>The output path.</value>
        public string OutputPath
        {
            get { return outputPath; }
            set { outputPath = value; }
        }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns></returns>
        public string Render()
        {
            if(String.IsNullOrEmpty(TemplateText))
                return String.Empty;

            TurboCompiler engine = new TurboCompiler();
            engine.AddTemplate(this);
            engine.Run();
            return engine.Templates[0].FinalCode;
        }

        /// <summary>
        /// Cleans the template.
        /// </summary>
        /// <param name="templateInputText">The template input text.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        private static string CleanTemplate(string templateInputText, ref ICodeLanguage language)
        {
            //// Modify this part if you want to read the <%@ tags also you can implement your own tags here.

            templateInputText = regexProperty.Replace(templateInputText, String.Empty);
            templateInputText = regexAssembly.Replace(templateInputText, String.Empty);
            templateInputText = regexImport.Replace(templateInputText, String.Empty);
            templateInputText = regexCodeTemplate.Replace(templateInputText, String.Empty);
            templateInputText = ParseTemplate(templateInputText);
            templateInputText = regexCleanCalls.Replace(templateInputText, new MatchEvaluator(CleanCalls));
            templateInputText = regexEmptyBrackets.Replace(templateInputText, String.Empty);
            templateInputText = regexCleanCodeTags.Replace(templateInputText, new MatchEvaluator(CleanCodeTags));

            // strip the directive
            templateInputText = regexPage.Replace(templateInputText, String.Empty);

            StringBuilder sb = new StringBuilder(HEADER);
            sb.Append(templateInputText);
            sb.Append(FOOTER);
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Cleans the calls.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static string CleanCalls(Match m)
        {
            string x = m.ToString();

            x = regexWriterBegin.Replace(x, WRITER_OPEN);
            x = regexWriterEnd.Replace(x, WRITER_CLOSE);
            return x;
        }

        /// <summary>
        /// Cleans the code tags.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static string CleanCodeTags(Match m)
        {
            string x = m.ToString();
            x = x.Substring(2, x.Length - 4);
            x = String.Concat("\t\t\t", x);
            return x;
        }

        /// <summary>
        /// Parses the template.
        /// </summary>
        /// <param name="templateInputText">The template input text.</param>
        /// <returns></returns>
        private static string ParseTemplate(string templateInputText)
        {
            if(String.IsNullOrEmpty(templateInputText))
                return String.Empty;

            MemoryStream mStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(mStream, Encoding.UTF8);

            int lnLast = 0;
            int lnAt = templateInputText.IndexOf("<%", 0);
            if(lnAt == -1)
                return templateInputText;

            while(lnAt > -1)
            {
                if(lnAt > -1)
                    writer.Write(String.Concat("\t\t\twriter.Write(@\"", templateInputText.Substring(lnLast, lnAt - lnLast).Replace("\"", "\"\""), "\" );"));

                int lnAt2 = templateInputText.IndexOf("%>", lnAt);
                if(lnAt2 < 0)
                    break;

                writer.Write(templateInputText.Substring(lnAt, lnAt2 - lnAt + 2));

                lnLast = lnAt2 + 2;
                lnAt = templateInputText.IndexOf("<%", lnLast);
                if(lnAt < 0)
                    writer.Write(String.Concat("\t\t\twriter.Write(@\"", templateInputText.Substring(lnLast, templateInputText.Length - lnLast).Replace("\"", "\"\""), "\" );"));
            }

            writer.Flush();
            mStream.Position = 0;
            StreamReader sr = new StreamReader(mStream);
            string returndata = sr.ReadToEnd();
            sr.Close();
            mStream.Close();
            writer.Close();
            return returndata;
        }

        /// <summary>
        /// Loads the text from manifest.
        /// </summary>
        /// <param name="templateFileName">Name of the template file.</param>
        /// <returns></returns>
        public static string LoadTextFromManifest(string templateFileName)
        {
            string templateText = null;
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream stream = asm.GetManifestResourceStream(String.Format("SubSonic.CodeGeneration.Templates.{0}", templateFileName));

            if(stream != null)
            {
                StreamReader sReader = new StreamReader(stream);
                templateText = sReader.ReadToEnd();
                sReader.Close();
            }

            return templateText;
        }
    }
}