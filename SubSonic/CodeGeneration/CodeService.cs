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
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using SubSonic.Sugar;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the CodeService class
    /// </summary>
    public static class CodeService
    {
        #region Helpers


        #region ReplacementVariable enum

        /// <summary>
        /// 
        /// </summary>
        public enum ReplacementVariable
        {
            /// <summary>
            /// 
            /// </summary>
            Table,
            /// <summary>
            /// 
            /// </summary>
            Provider,
            /// <summary>
            /// 
            /// </summary>
            View,
            /// <summary>
            /// 
            /// </summary>
            StoredProcedure
        }

        #endregion


        #region TemplateSet enum

        /// <summary>
        /// 
        /// </summary>
        public enum TemplateSet
        {
            /// <summary>
            /// 
            /// </summary>
            Default,
            /// <summary>
            /// 
            /// </summary>
            MVC
        }

        #endregion


        #region TemplateType enum

        /// <summary>
        /// 
        /// </summary>
        public enum TemplateType
        {
            Enum, 
			/// <summary>
            /// 
            /// </summary>
            Class,
            /// <summary>
            /// 
            /// </summary>
            ODSController,
            /// <summary>
            /// 
            /// </summary>
            ReadOnly,
            /// <summary>
            /// 
            /// </summary>
            SP,
            /// <summary>
            /// 
            /// </summary>
            Structs,
            /// <summary>
            /// 
            /// </summary>
            DynamicScaffold,
            /// <summary>
            /// 
            /// </summary>
            GeneratedScaffoldCodeBehind,
            /// <summary>
            /// 
            /// </summary>
            GeneratedScaffoldMarkup
        }

        #endregion


        private static string templateDirectory = String.Empty;

        /// <summary>
        /// Gets or sets the template directory.
        /// </summary>
        /// <value>The template directory.</value>
        public static string TemplateDirectory
        {
            get { return templateDirectory; }
            set { templateDirectory = value; }
        }

        /// <summary>
        /// Summary for the Replacement class
        /// </summary>
        public class Replacement
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Replacement"/> class.
            /// </summary>
            /// <param name="variable">The variable.</param>
            /// <param name="replace">The replace.</param>
            public Replacement(ReplacementVariable variable, string replace)
            {
                Variable = variable;
                ReplaceWith = replace;
            }

            /// <summary>
            /// Gets or sets the variable.
            /// </summary>
            /// <value>The variable.</value>
            public ReplacementVariable Variable { get; set; }

            /// <summary>
            /// Gets or sets the replace with.
            /// </summary>
            /// <value>The replace with.</value>
            public string ReplaceWith { get; set; }
        }

        #endregion


        /// <summary>
        /// Not currently used, but will be the basis for user defined templates. Please don't remove!
        /// </summary>
        /// <param name="templateFile">The template file.</param>
        /// <param name="values">The values.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string RunTemplate(string templateFile, NameValueCollection values, ICodeLanguage language, DataProvider provider)
        {
            string templatePath = Path.Combine(TemplateDirectory, templateFile);
            string templateText = Files.GetFileText(templatePath);

            for(int i = 0; i < values.Count; i++)
                templateText = templateText.Replace(values.GetKey(i), values.Get(i));

            TurboTemplate t = new TurboTemplate(templateText, language, provider);
            return t.Render();
        }

        /// <summary>
        /// Builds the template.
        /// </summary>
        /// <param name="templateType">Type of the template.</param>
        /// <param name="values">The values.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static TurboTemplate BuildTemplate(TemplateType templateType, NameValueCollection values, ICodeLanguage language, DataProvider provider)
        {
            string templateText = GetTemplateText(templateType, language);

            for(int i = 0; i < values.Count; i++)
                templateText = templateText.Replace(values.GetKey(i), values.Get(i));

            TurboTemplate t = new TurboTemplate(templateText, language, provider);
            return t;
        }

        /// <summary>
        /// Runs the template.
        /// </summary>
        /// <param name="templateType">Type of the template.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string RunTemplate(TemplateType templateType, IEnumerable<Replacement> settings, ICodeLanguage language, DataProvider provider)
        {
            Utility.WriteTrace("Getting Template");
            string templateText = GetTemplateText(templateType, language);

            Utility.WriteTrace("Replacing values in template");
            foreach(Replacement var in settings)
            {
                string replaceHolder = String.Concat("#", Enum.GetName(typeof(ReplacementVariable), var.Variable).ToUpper(new CultureInfo("en")), "#");
                templateText = Utility.FastReplace(templateText, replaceHolder, var.ReplaceWith, StringComparison.InvariantCultureIgnoreCase);
            }

            TurboTemplate t = new TurboTemplate(templateText, language, provider);
            Utility.WriteTrace("Rendering template");
            string output = t.Render();
            Utility.WriteTrace("Finished :)");
            return output;
        }

        /// <summary>
        /// Evaluates the passed object name against current provider parameters to determine whether or not it should be generated.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <param name="includeList">The include list.</param>
        /// <param name="excludeList">The exclude list.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static bool ShouldGenerate(string objectName, string[] includeList, string[] excludeList, DataProvider provider)
        {
            // TODO: provider isn't being used?  Seems like that may be a bad thing in a multi-provider environment
            bool result = true;
            bool generateAll = false;

            // first, check to see if the includeList says to include all tables
            // this is a default
            if(includeList.Length == 1)
            {
                if(includeList[0] == "*")
                    generateAll = true;
            }

            // if we need to generate all tables, then we need to check the excludeList
            if(generateAll)
            {
                foreach(string s in excludeList)
                {
                    if(Utility.IsRegexMatch(objectName, s.Trim()))
                    {
                        result = false;
                        break;
                    }
                }
            }
            else
            {
                // IncludeList TRUMPs excludeList in case of confusion
                // what this means is that if there is an includeList,
                // be definition there's an excludeList of all tables not included
                // yep, confusing.

                // this means that tables were specifically requested in the includeList
                // need to make them prove themselves
                result = false;

                foreach(string s in includeList)
                {
                    if(Utility.IsRegexMatch(objectName, s.Trim()))
                    {
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Evaluates the passed object name against current provider parameters to determine whether or not it should be generated.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <returns></returns>
        public static bool ShouldGenerate(TableSchema.Table tableSchema)
        {
            return ShouldGenerate(tableSchema.TableName, tableSchema.Provider.Name);
        }

        /// <summary>
        /// Evaluates the passed object name against current provider parameters to determine whether or not it should be generated.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static bool ShouldGenerate(string tableName, string providerName)
        {
            DataProvider provider = DataService.Providers[providerName];
            if(provider == null)
                throw new ArgumentException("There is no provider with the name " + providerName, "providerName");

            return ShouldGenerate(tableName, provider.IncludeTables, provider.ExcludeTables, provider);
        }

        /// <summary>
        /// Builds the ODS template.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static TurboTemplate BuildODSTemplate(string tableName, ICodeLanguage language, DataProvider provider)
        {
            DataService.LoadProviders();

            if(ShouldGenerate(tableName, provider.Name) && provider.GenerateODSControllers)
            {
                List<Replacement> list = new List<Replacement>
                                             {
                                                 new Replacement(ReplacementVariable.Table, tableName),
                                                 new Replacement(ReplacementVariable.Provider, provider.Name)
                                             };

                return PrepareTemplate(String.Concat("ODS Controller - ", provider.Name, ": ", tableName), TemplateType.ODSController, list, language, provider);
            }

            Utility.WriteTrace(String.Format("{0} is excluded from generation", tableName));
            return null;
        }

        /// <summary>
        /// Builds the class template.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static TurboTemplate BuildClassTemplate(string tableName, ICodeLanguage language, DataProvider provider)
        {
            DataService.LoadProviders();

            if(ShouldGenerate(tableName, provider.Name))
            {
                List<Replacement> list = new List<Replacement>
                                             {
                                                 new Replacement(ReplacementVariable.Table, tableName),
                                                 new Replacement(ReplacementVariable.Provider, provider.Name)
                                             };

                return PrepareTemplate(String.Concat("Class - ", provider.Name, ": ", tableName), TemplateType.Class, list, language, provider);
            }

            Utility.WriteTrace(String.Format("Class {0} is excluded from generation", tableName));
            return null;
        }

        /// <summary>
        /// Builds the view template.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static TurboTemplate BuildViewTemplate(string tableName, ICodeLanguage language, DataProvider provider)
        {
            DataService.LoadProviders();

            if(ShouldGenerate(tableName, provider.Name))
            {
                List<Replacement> list = new List<Replacement>
                                             {
                                                 new Replacement(ReplacementVariable.View, tableName),
                                                 new Replacement(ReplacementVariable.Provider, provider.Name)
                                             };

                return PrepareTemplate(String.Concat("View - ", provider.Name, ": ", tableName), TemplateType.ReadOnly, list, language, provider);
            }

            Utility.WriteTrace(String.Format("View {0} is excluded from generation", tableName));
            return null;
        }

        /// <summary>
        /// Builds the SP template.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static TurboTemplate BuildSPTemplate(ICodeLanguage language, DataProvider provider)
        {
            DataService.LoadProviders();
            List<Replacement> list = new List<Replacement>
                                         {
                                             new Replacement(ReplacementVariable.Provider, provider.Name)
                                         };
            return PrepareTemplate("Stored Procedure Class - " + provider.Name, TemplateType.SP, list, language, provider);
        }

		/// <summary>
		/// Builds the Enum template.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <param name="provider">The provider.</param>
		/// <returns></returns>
		public static TurboTemplate BuildEnumTemplate(ICodeLanguage language, DataProvider provider) {
			DataService.LoadProviders();
			List<Replacement> list = new List<Replacement>
                                         {
                                             new Replacement(ReplacementVariable.Provider, provider.Name)
                                         };
			return PrepareTemplate("Enums - " + provider.Name, TemplateType.Enum, list, language, provider);
		}

		/// <summary>
        /// Builds the structs template.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static TurboTemplate BuildStructsTemplate(ICodeLanguage language, DataProvider provider)
        {
            DataService.LoadProviders();
            List<Replacement> list = new List<Replacement>();
            return PrepareTemplate("Structs Class", TemplateType.Structs, list, language, provider);
        }

        /// <summary>
        /// Prepares the template.
        /// </summary>
        /// <param name="templateName">Name of the template.</param>
        /// <param name="templateType">Type of the template.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        private static TurboTemplate PrepareTemplate(string templateName, TemplateType templateType, IEnumerable<Replacement> settings, ICodeLanguage language,
                                                     DataProvider provider)
        {
            Utility.WriteTrace(String.Format("Getting template for {0}", templateName));
            string templateText = GetTemplateText(templateType, language);

			//if (templateType== TemplateType.Enum) System.IO.File.AppendAllText(@"C:\temp\enumlog.txt", templateName + "\r\n");

            // set the provider and tablename
            Utility.WriteTrace(String.Format("Preparing template for {0}", templateName));

            foreach(Replacement var in settings)
            {
                string replaceHolder = String.Concat("#", Enum.GetName(typeof(ReplacementVariable), var.Variable).ToUpper(new CultureInfo("en")), "#");
                templateText = Utility.FastReplace(templateText, replaceHolder, var.ReplaceWith, StringComparison.InvariantCultureIgnoreCase);
            }

            TurboTemplate t = new TurboTemplate(templateText, language, provider)
                                  {
                                      TemplateName = templateName
                                  };

			//if (templateType == TemplateType.Enum) System.IO.File.AppendAllText(@"C:\temp\enumlog.txt", t.TemplateText + "\r\n");

			return t;
        }

        /// <summary>
        /// Runs the ODS.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string RunODS(string tableName, ICodeLanguage language, DataProvider provider)
        {
            TurboTemplate tt = BuildODSTemplate(tableName, language, provider);
            return tt.Render();
        }

        /// <summary>
        /// Runs the read only.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string RunReadOnly(string viewName, ICodeLanguage language, DataProvider provider)
        {
            TurboTemplate tt = BuildViewTemplate(viewName, language, provider);
            return tt.Render();
        }

        /// <summary>
        /// Runs the class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string RunClass(string tableName, ICodeLanguage language, DataProvider provider)
        {
            TurboTemplate tt = BuildClassTemplate(tableName, language, provider);
            return tt.Render();
        }

		/// <summary>
		/// Runs the S ps.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <param name="provider">The provider.</param>
		/// <returns></returns>
		public static string RunSPs(ICodeLanguage language, DataProvider provider) {
			TurboTemplate tt = BuildSPTemplate(language, provider);
			return tt.Render();
		}

		/// <summary>
		/// Runs the Enums.
		/// </summary>
		/// <param name="language">The language.</param>
		/// <param name="provider">The provider.</param>
		/// <returns></returns>
		public static string RunEnums(ICodeLanguage language, DataProvider provider) {
			TurboTemplate tt = BuildEnumTemplate(language, provider);
			return tt.Render();
		}

		/// <summary>
        /// Runs the structs.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string RunStructs(ICodeLanguage language, DataProvider provider)
        {
            TurboTemplate tt = BuildStructsTemplate(language, provider);
            return tt.Render();
        }

        /// <summary>
        /// Builds the compil unit.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <returns></returns>
        public static CodeCompileUnit BuildCompilUnit(string code)
        {
            return new CodeSnippetCompileUnit(code);
        }

        /// <summary>
        /// Gets the template text.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        private static string GetTemplateText(TemplateType t, ICodeLanguage language)
        {
            string template;
            string templateText = null;

            switch(t)
            {
                case TemplateType.Class:
                    template = TemplateName.CLASS;
                    break;
                case TemplateType.ReadOnly:
                    template = TemplateName.VIEW;
                    break;
				case TemplateType.SP:
					template = TemplateName.STORED_PROCEDURE;
					break;
				case TemplateType.Enum:
					template = TemplateName.ENUM;
					break;
				case TemplateType.Structs:
                    template = TemplateName.STRUCTS;
                    break;
                case TemplateType.ODSController:
                    template = TemplateName.ODS_CONTROLLER;
                    break;
                case TemplateType.DynamicScaffold:
                    template = TemplateName.DYNAMIC_SCAFFOLD;
                    break;
                case TemplateType.GeneratedScaffoldCodeBehind:
                    template = TemplateName.GENERATED_SCAFFOLD_CODE_BEHIND;
                    break;
                case TemplateType.GeneratedScaffoldMarkup:
                    template = TemplateName.GENERATED_SCAFFOLD_MARKUP;
                    break;
                default:
                    template = TemplateName.CLASS;
                    break;
            }

            template = String.Concat(language.TemplatePrefix, template, FileExtension.DOT_ASPX);

            // decide where to pull the text from
            if(!String.IsNullOrEmpty(templateDirectory))
            {
                Utility.WriteTrace(String.Concat("Looking for template ", template, " in ", templateDirectory));

                // make sure the template exists
                string templatePath = Path.Combine(templateDirectory, template);

                if(File.Exists(templatePath))
                    templateText = Files.GetFileText(templatePath);
                else
                    Utility.WriteTrace(String.Concat("Template ", template, " NOT FOUND in directory ", templateDirectory, "; using embedded resource template instead..."));
            }

            if(String.IsNullOrEmpty(templateText))
            {
                Utility.WriteTrace(String.Format("Loading template from resource: {0}", template));
                templateText = TurboTemplate.LoadTextFromManifest(template);
            }

            if(String.IsNullOrEmpty(templateText))
                throw new Exception(String.Format("The template \"{0}\" is empty or cannot be found.", template));

            return templateText;
        }
    }
}