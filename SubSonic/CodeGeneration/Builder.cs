using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.IO;
using System.CodeDom.Compiler;
using System.CodeDom;
using Microsoft.CSharp;
using Microsoft.VisualBasic;
using System.Web.Compilation;
using System.Text.RegularExpressions;

namespace SubSonic.CodeGenerator {
    public class Builder {
        private static string templateDirectory=string.Empty;

        public static string TemplateDirectory {
            get { return templateDirectory; }
            set { templateDirectory = value; }
        }
	
        enum TemplateType{
            Class,
            ReadOnly,
            SP,
            Structs
        }

        public static string RunTemplate(string templateFile, NameValueCollection values)
        {
            string result;
            string templatePath = Path.Combine(SubSonicConfig.TemplateDirectory, templateFile);
            string templateText = SubSonic.Utilities.Utility.GetFileText(templatePath);

            for (int i = 0; i < values.Count; i++)
            {
                templateText = templateText.Replace(values.GetKey(i), values.Get(i));
            }

            Template t = new Template(templateText);
            result = t.Render();

            //must run AFTER Template.Render()
            result = result.Replace("[<]", "<");
            result = result.Replace("[>]", ">");

            //the generator has an issue with adding extra lines. Trim them out
            Regex reg = new Regex(@"\s*\r\s*\r");
            result = reg.Replace(result, string.Empty);

            //now, for readability, add a space after the end of the method/class
            result = result.Replace("}", "}\r\n");
            result = result.Replace("namespace", "\r\nnamespace");
            result = result.Replace("public class", "\r\npublic class");

            if (result == string.Empty)
            {
                throw new Exception(t.Error);
            }
            return result;
        }

        public static string RunTemplate(string templatePath, string outPutFilePath, string providerName, string tableName, string SP){
            string result = string.Empty;
            string templateText = SubSonic.Utilities.Utility.GetFileText(templatePath);
            
            //run replacements
            templateText = templateText.Replace("#TABLE#", tableName);
            templateText = templateText.Replace("#PROVIDER#", providerName);
            templateText = templateText.Replace("#STOREDPROCEDURE#", SP);
          
            
            Template t = new Template(templateText);
            result=t.Render();

            //the generator has an issue with adding extra lines. Trim them out
            Regex reg = new Regex(@"\s*\r\s*\r");
            result = reg.Replace(result, string.Empty);

            //now, for readability, add a space after the end of the method/class
            result = result.Replace("}", "}\r\n");
            result = result.Replace("namespace", "\r\nnamespace");
            result = result.Replace("public class", "\r\npublic class");

            if (result == string.Empty)
                throw new Exception(t.Error);
            return result;

        }

        static string GetTemplateText(TemplateType t, LanguageType lang) {
            string template = string.Empty;
            string templateText = string.Empty;

            switch (t) {
                case TemplateType.Class:
                    template = "ClassTemplate";
                    break;
                case TemplateType.ReadOnly:
                    template = "ViewTemplate";
                    break;
                case TemplateType.SP:
                    template = "SPTemplate";
                    break;
                case TemplateType.Structs:
                    template = "StructsTemplate";
                    break;
                default:
                    template = "ClassTemplate";
                    break;
            }

            if (lang == LanguageType.VB)
                template += "VB";
            template += ".aspx";

            string templatePath = "Resources";

            //decide where to pull the text from
            if (!String.IsNullOrEmpty(templateDirectory)) {
                
                //make sure the template exists
                templatePath = Path.Combine(templateDirectory, template);

                if (File.Exists(templatePath)) {

                    //pull the text from there
                    templateText=SubSonic.Utilities.Utility.GetFileText(templatePath);
                
                } else {
                    //empty out the templateDirectory
                    templateText ="Resources";
                }
            }

            if (templateText == string.Empty) {
                templateText = Properties.Resources.ResourceManager.GetString(template);
            }


            if (templateText == string.Empty)
                throw new Exception("Can't find the template " + template);

            return templateText;
        }

        public static string RunClass(string tableName, string providerName, LanguageType lang) {

            string templateText = GetTemplateText(TemplateType.Class, lang);
            
            //set the provider and tablename
            templateText = templateText.Replace("#TABLE#", tableName);
            templateText = templateText.Replace("#PROVIDER#", providerName);

            string result = string.Empty;
            Template t = new Template(templateText);
            
            result = t.Render();

            result = SrubOutput(result);

            if (result == string.Empty)
                throw new Exception(t.Error);
            return result;

        }
        static string SrubOutput(string sIn) {
            string result = sIn;
            //the generator has an issue with adding extra lines. Trim them out
            Regex reg = new Regex(@"\s*\r\s*\r");
            result = reg.Replace(result, string.Empty);

            //now, for readability, add a space after the end of the method/class
            result = result.Replace("}", "}\r\n");
            result = result.Replace("namespace", "\r\nnamespace");
            result = result.Replace("public class", "\r\npublic class");
            return result;

        }
        public static string RunReadOnly(string viewName, string providerName, LanguageType lang) {

            string templateText = GetTemplateText(TemplateType.ReadOnly, lang);

            //set the provider and tablename
            templateText = templateText.Replace("#VIEW#", viewName);
            templateText = templateText.Replace("#PROVIDER#", providerName);

            string result = string.Empty;
            Template t = new Template(templateText);
            result = t.Render();

            result = SrubOutput(result);


            if (result == string.Empty)
                throw new Exception(t.Error);
            return result;

        }
        public static string RunSPs(string providerName, LanguageType lang) {

            string templateText = GetTemplateText(TemplateType.SP, lang);

            //set the provider and tablename
            templateText = templateText.Replace("#PROVIDER#", providerName);

            string result = string.Empty;
            Template t = new Template(templateText);
            result = t.Render();

            result = SrubOutput(result);


            if (result == string.Empty)
                throw new Exception(t.Error);
            return result;

        }
        public static string RunStructs(LanguageType lang) {

            string templateText = GetTemplateText(TemplateType.Structs, lang);

            string result = string.Empty;
            Template t = new Template(templateText);
            result = t.Render();

            result = SrubOutput(result);


            if (result == string.Empty)
                throw new Exception(t.Error);
            return result;

        }
    
    }
}
