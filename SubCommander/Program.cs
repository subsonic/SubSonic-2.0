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
using System.Configuration;
using System.Configuration.Provider;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web.Configuration;
using SubSonic.Migrations;
using SubSonic.Sugar;
using SubSonic.Utilities;

namespace SubSonic.SubCommander
{
    /// <summary>
    /// 
    /// </summary>
    internal class Program
    {
        private delegate void ActionDelegate();
        private static readonly TurboCompiler turboCompiler = new TurboCompiler();
        private static string[] ArgList = new string[0];
        private static Arguments arguments;
        private static ICodeLanguage language = new CSharpCodeLanguage();

        /// <summary>
        /// Main
        /// </summary>
        /// <param name="args">The args.</param>
        private static void Main(string[] args)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            arguments = new Arguments(Environment.CommandLine);
            ArgList = args;
            //the first args gonna tell us what to do
            string command = String.Empty;
            if(args.Length > 0)
                command = args[0];

            // Set DataDirectory macro for compatibility with ASP.NET's App_Data folder
            AppDomain.CurrentDomain.SetData("DataDirectory",
                Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "App_Data");

            try
            {
                Dictionary<string, ActionDelegate> commandsToActions = new Dictionary<string, ActionDelegate>();
                commandsToActions["scriptdata"] = ScriptData;
                commandsToActions["migrate"] = Migrate;
                commandsToActions["scriptschema"] = ScriptSchema;
                commandsToActions["version"] = VersionDB;
                commandsToActions["generatetables"] = GenerateTables;
                commandsToActions["generateods"] = GenerateODSControllers;
                commandsToActions["generateviews"] = GenerateViews;
				commandsToActions["generatesps"] = GenerateSPs;
				commandsToActions["generateenums"] = GenerateEnums;
				commandsToActions["generate"] = GenerateAll;
                commandsToActions["generateall"] = GenerateAll;
                commandsToActions["generateeditor"] = GenerateEditor;
                commandsToActions["editor"] = GenerateEditor;
                commandsToActions["help"] = delegate { ThrowHelp(true); };
                commandsToActions["dir"] = delegate
                                               {
                                                   Console.WriteLine(Directory.GetCurrentDirectory());
                                                   Console.WriteLine("");
                                                   Console.WriteLine("Press any key to exit...");
                                                   Console.Read();
                                               };
                
                //find appropriate command
                string key = command.ToLower();
                if(commandsToActions.ContainsKey(key))
                {
                    //execute it!
                    ActionDelegate action = commandsToActions[key];
                    action();
                }
                else
                {
                    Console.WriteLine("ERROR: No Command Specified");
                    ThrowHelp(true);
                }
               
                if(turboCompiler.Templates.Count > 0)
                {
                    Console.WriteLine("Running Compiler...");
                    turboCompiler.Run();
                    Console.WriteLine("Writing Files...");
                    foreach(TurboTemplate template in turboCompiler.Templates)
                    {
                        Utility.WriteTrace(String.Concat("Writing ", template.TemplateName, " as ", template.OutputPath.Substring(template.OutputPath.LastIndexOf("\\") + 1)));
                        Files.CreateToFile(template.OutputPath, template.FinalCode);
                    }
                    Console.WriteLine("Done!");
                }
            }
            catch(Exception x)
            {
                Console.WriteLine("ERROR: Trying to execute {0}{1}Error Message: {2}", command, Environment.NewLine, x);
            }
            timer.Stop();
            Console.WriteLine("Execution Time: {0}ms", timer.ElapsedMilliseconds);
        }

        /// <summary>
        /// Throws the help.
        /// </summary>
        /// <param name="verbose">if set to <c>true</c> [verbose].</param>
        private static void ThrowHelp(bool verbose)
        {
            WriteVersionInformation("sonic.exe v{0} - Command Line Interface to SubSonic v{1}");
            Console.WriteLine("Usage:   sonic command [options]");
            Console.WriteLine("Sample:  sonic generate /server localhost /db northwind /out GeneratedFiles");
            Console.WriteLine("Help:    sonic help");
            Console.WriteLine("TIP:    SubSonic will read your App.Config or Web.Config - just select the project ");
            Console.WriteLine("and run your command.");
            Console.WriteLine(String.Empty);
            if(!verbose)
                return;

            Console.WriteLine("******************** Commands *********************************");
            Console.WriteLine("version:        Scripts out the schema/data of your db to file");
            Console.WriteLine("scriptdata:     Scripts the data to file for your database");
            Console.WriteLine("scriptschema:   Scripts your Database schema to file");
            Console.WriteLine("generate:       Generates output code for tables, views, and SPs");
            Console.WriteLine("generatetables: Generates output code for your tables");
            Console.WriteLine("generateODS:    Generates and ObjectDataSource controller for each table");
            Console.WriteLine("generateviews:  Generates output code for your views");
			Console.WriteLine("generatesps:    Generates output code for your SPs");
			Console.WriteLine("generateenums:  Generates output code for your Enums");
			Console.WriteLine("editor:         Creates an Editor for a particular table");
            Console.WriteLine("migrate:        Migrate the database using migrations in \\Migrations folder");
            Console.WriteLine(String.Empty);
            Console.WriteLine("******************** Argument List ****************************");
            Console.WriteLine("####### Required For all commands (these can be read from config files)");
            Console.WriteLine("if you don't have a Web or App.config, these need to be set");
            Console.WriteLine("/override       SubCommander won't try to find a config - instead it will use what you pass in");
            Console.WriteLine("/server -       the database server - ALWAYS REQUIRED");
            Console.WriteLine("/db -           the database to use");
            Console.WriteLine(String.Empty);
            Console.WriteLine("####### Other Commands (some may be required for specific commands)");
            Console.WriteLine("/userid -       the User ID for your database (blank = use SSPI)");
            Console.WriteLine("/password -     the password for your DB (blank = use SSPI)");
            Console.WriteLine("/out -          the output directory for generated items. (default = current)");

            List<string> languages = new List<string>();
            foreach(ICodeLanguage codeLanguage in CodeLanguageFactory.AllCodeLanguages)
                languages.Add(codeLanguage.ShortName);

            Console.WriteLine(
                String.Format("/lang -         generated code language: {0} (default = {1})",
                    String.Join(", ", languages.ToArray()), CodeLanguageFactory.DefaultCodeLanguage.ShortName));
            Console.WriteLine("/provider -     the name of the provider to use");
            Console.WriteLine("/includeTableList -    used for generating classes. A comma-delimited list that ");
            Console.WriteLine("                defines which tables should be used to generate classes");
            Console.WriteLine("/config -       the path your App/Web.Config - used to instance SubSonic ");
            Console.WriteLine("/excludeTableList -the opposite of tablelist. These tables will NOT be ");
            Console.WriteLine("                     used to generate classes");
            Console.WriteLine("/version -           Version to migrate database to");
            Console.WriteLine("/migrationDirectory - Directory containing migration files.  Defaults to \\Migrations");
            Console.WriteLine("/groupOutput - Allows you to group your generated code into subfolders.  Options are schema, type, schemaAndType.");
            Console.WriteLine("               'schema' refers to SQL database schema (i.e. dbo) and 'type' refers to Models/Controllers/Views");
            Console.WriteLine("/migrationDirectory - Directory containing migration files.  Defaults to \\Migrations");


            Console.WriteLine(String.Empty);
            Console.WriteLine("******** Arguments Matching SubSonic web.config Settings ********");
            Console.WriteLine("Just add a '/' in front");
            Console.WriteLine("/generatedNamespace -  the namespace to use for generated code");
            Console.WriteLine("/templateDirectory -   the directory containing template overrides");
            Console.WriteLine("/fixDatabaseObjectCasing - fix the capitilization of object generated from database? true/false (default is true)");
            Console.WriteLine("/fixPluralClassNames - reset all plural table names to singular? true/false");
            Console.WriteLine("/useSPs -              whether to generate SP wrapper (true/false)");
            Console.WriteLine("/spClassName -         default is 'StoredProcedures' - this will override that");
            Console.WriteLine("/stripTableText -      replace table text with this command");
            Console.WriteLine("/stripColumnText -     replace column text with this command");
            Console.WriteLine("/stripParamText -      replace SP param text with this command");
            Console.WriteLine("/appendWith -          when you have reserved words in your table columns");
            Console.WriteLine("                       we need to append it with something. Our default is 'X'.");
            Console.WriteLine("                       You can change that here.");
            Console.WriteLine("/spStartsWith -        use SPs that start with this");
            Console.WriteLine("/viewStartsWith -      use Views that start with this");
            Console.WriteLine("/relatedTableLoadPrefix - prefix related table loaders");
            Console.WriteLine("/removeUnderscores -   whether to remove underscores from generated object");
            Console.WriteLine("                       names (true/false) default is false");
            Console.WriteLine("/templateDirectory     The path to your custom templates. This is a directory reference");
            Console.WriteLine("/regexMatchExpression");
            Console.WriteLine("/regexReplaceExpression");
            Console.WriteLine("/regexIgnoreCase");
            Console.WriteLine("/regexDictionaryReplace");
            Console.WriteLine("/generateLazyLoads");
            Console.WriteLine("/generateRelatedTablesAsProperties");
            Console.WriteLine("/extractClassNameFromSPName");
            Console.WriteLine("/includeProcedureList");
            Console.WriteLine("/excludeProcedureList");
            Console.WriteLine("/useExtendedProperties");
            Console.WriteLine("/useUtc");
            Console.WriteLine("/additionalNamespaces");
        }

        /// <summary>
        /// Writes the version information.
        /// </summary>
        /// <param name="formatString">The format string.</param>
        private static void WriteVersionInformation(string formatString)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string sonicVersion = asm.GetName().Version.ToString();
            string subSonicVersion = "---";

            AssemblyName[] asmNames = asm.GetReferencedAssemblies();
            foreach(AssemblyName nm in asmNames)
            {
                if(nm.Name == "SubSonic")
                {
                    subSonicVersion = nm.Version.ToString();
                    break;
                }
            }

            Console.WriteLine(formatString, sonicVersion, subSonicVersion);
        }


        #region Migrations

        /// <summary>
        /// Migrates the specified migration directory.
        /// </summary>
        public static void Migrate()
        {
            SetProvider();

            string providerName = GetArg("provider");
            if(String.IsNullOrEmpty(providerName))
                providerName = DataService.Provider.Name;

            string migrationDirectory = GetArg("migrationDirectory");
            if(String.IsNullOrEmpty(migrationDirectory))
                migrationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Migrations");

            string sVersion = GetArg("version");
            int? toVersion = null;
            if(!String.IsNullOrEmpty(sVersion))
            {
                int version;
                if(int.TryParse(sVersion, out version))
                    toVersion = version;
            }

            Migrator.Migrate(providerName, migrationDirectory, toVersion);
        }

        #endregion


        #region Provider Startup

        /// <summary>
        /// Gets the config path.
        /// </summary>
        /// <returns></returns>
        private static string GetConfigPath()
        {
            // Try cmd line arg first.
            string configArg = GetArg("config");

            string configPath = GetConfigInDir(configArg);
            if(configPath != null)
                return configPath;

            string thisDir = Directory.GetCurrentDirectory();

            configPath = GetConfigInDir(Path.Combine(thisDir, configArg));
            if(configPath != null)
                return configPath;

            return GetConfigInDir(thisDir);
        }

        /// <summary>
        /// Tries to find the config file in the specified directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns></returns>
        private static string GetConfigInDir(string directory)
        {
            if(File.Exists(directory))
                return directory;

            string webConfigPath = Path.Combine(directory, "Web.config");
            if(File.Exists(webConfigPath))
                return webConfigPath;

            string appConfigPath = Path.Combine(directory, "App.config");
            if(File.Exists(appConfigPath))
                return appConfigPath;

            return null;
        }

        /// <summary>
        /// Sets the provider.
        /// </summary>
        private static void SetProvider()
        {
            string overrideFlag = GetArg("override");
            bool configSet = false;
            if(String.IsNullOrEmpty(overrideFlag))
            {
                string configPath = GetConfigPath();
                if(File.Exists(configPath))
                {
                    configSet = true;
                    Console.WriteLine("Setting ConfigPath: '{0}'", configPath);
                    SetProvider(configPath);
                }
            }

            if(!configSet)
                SetProviderManually();
        }

        /// <summary>
        /// Sets the provider manually.
        /// </summary>
        private static void SetProviderManually()
        {
            string traceFlag = GetArg(ConfigurationPropertyName.ENABLE_TRACE);

            if(!String.IsNullOrEmpty(traceFlag))
                DataService.EnableTrace = Convert.ToBoolean(traceFlag);

            Utility.WriteTrace("Setting config manually - need AT LEAST a /server and /db");

            //clear the providers and reset
            DataService.Provider = new SqlDataProvider();
            DataService.Providers = new DataProviderCollection();

            //instance a section - we'll set this manually for the DataService
            SubSonicSection section = new SubSonicSection();
            section.TemplateDirectory = GetArg(ConfigurationPropertyName.TEMPLATE_DIRECTORY);
            CodeService.TemplateDirectory = section.TemplateDirectory;

            string providerName = GetArg(ConfigurationPropertyName.PROVIDER_TO_USE);
            if(string.IsNullOrEmpty(providerName))
                providerName = "Default";
            section.DefaultProvider = providerName;

            section.DefaultProvider = "Default";

            CodeService.TemplateDirectory = section.TemplateDirectory;

            //set the properties
            DataProvider provider = DataService.Provider;
            NameValueCollection config = new NameValueCollection();

            //need to add this for now
            config.Add("connectionStringName", "Default");

            if(!string.IsNullOrEmpty(GetArg(ConfigurationPropertyName.TEMPLATE_DIRECTORY)))
                config.Add(ConfigurationPropertyName.TEMPLATE_DIRECTORY, GetArg(ConfigurationPropertyName.TEMPLATE_DIRECTORY));

            //setup the config
            SetConfig(config, ConfigurationPropertyName.APPEND_WITH);
            SetConfig(config, ConfigurationPropertyName.ADDITIONAL_NAMESPACES);
            SetConfig(config, ConfigurationPropertyName.EXCLUDE_PROCEDURE_LIST);
            SetConfig(config, ConfigurationPropertyName.EXCLUDE_TABLE_LIST);
            SetConfig(config, ConfigurationPropertyName.EXTRACT_CLASS_NAME_FROM_SP_NAME);
            SetConfig(config, ConfigurationPropertyName.FIX_DATABASE_OBJECT_CASING);
            SetConfig(config, ConfigurationPropertyName.FIX_PLURAL_CLASS_NAMES);
            SetConfig(config, ConfigurationPropertyName.GENERATED_NAMESPACE);
            SetConfig(config, ConfigurationPropertyName.GENERATE_LAZY_LOADS);
            SetConfig(config, ConfigurationPropertyName.GENERATE_NULLABLE_PROPERTIES);
            SetConfig(config, ConfigurationPropertyName.GENERATE_ODS_CONTROLLERS);
            SetConfig(config, ConfigurationPropertyName.GENERATE_RELATED_TABLES_AS_PROPERTIES);
            SetConfig(config, ConfigurationPropertyName.INCLUDE_PROCEDURE_LIST);
            SetConfig(config, ConfigurationPropertyName.INCLUDE_TABLE_LIST);
            SetConfig(config, ConfigurationPropertyName.REGEX_DICTIONARY_REPLACE);
            SetConfig(config, ConfigurationPropertyName.REGEX_IGNORE_CASE);
            SetConfig(config, ConfigurationPropertyName.REGEX_MATCH_EXPRESSION);
            SetConfig(config, ConfigurationPropertyName.REGEX_REPLACE_EXPRESSION);
            SetConfig(config, ConfigurationPropertyName.RELATED_TABLE_LOAD_PREFIX);
            SetConfig(config, ConfigurationPropertyName.REMOVE_UNDERSCORES);
            SetConfig(config, ConfigurationPropertyName.SET_PROPERTY_DEFAULTS_FROM_DATABASE);
            SetConfig(config, ConfigurationPropertyName.SP_STARTS_WITH);
            SetConfig(config, ConfigurationPropertyName.STORED_PROCEDURE_CLASS_NAME);
            SetConfig(config, ConfigurationPropertyName.STRIP_COLUMN_TEXT);
            SetConfig(config, ConfigurationPropertyName.STRIP_PARAM_TEXT);
            SetConfig(config, ConfigurationPropertyName.STRIP_STORED_PROCEDURE_TEXT);
            SetConfig(config, ConfigurationPropertyName.STRIP_TABLE_TEXT);
            SetConfig(config, ConfigurationPropertyName.STRIP_VIEW_TEXT);
            SetConfig(config, ConfigurationPropertyName.USE_EXTENDED_PROPERTIES);
            SetConfig(config, ConfigurationPropertyName.USE_STORED_PROCEDURES);
            SetConfig(config, ConfigurationPropertyName.USE_UTC_TIMES);
            SetConfig(config, ConfigurationPropertyName.VIEW_STARTS_WITH);
            SetConfig(config, ConfigurationPropertyName.GROUP_OUTPUT);

            //initialize the provider
            Utility.WriteTrace("Initializing the provider with the passed in configuration >>> hold on to your hats...");
            provider.Initialize(providerName, config);

            //first, make sure there's a connection
            Utility.WriteTrace("Overriding the connection string...");
            provider.DefaultConnectionString = GetConnnectionString();
            Utility.WriteTrace("Set connection string to " + provider.DefaultConnectionString);

            DataService.Providers.Add(provider);
        }

        /// <summary>
        /// Sets the config.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="key">The key.</param>
        private static void SetConfig(NameValueCollection config, string key)
        {
            string setting = GetArg(key);
            if(!String.IsNullOrEmpty(setting))
            {
                Utility.WriteTrace("Setting " + key + " to " + setting);
                config.Add(key, setting);
            }
        }

        /// <summary>
        /// Sets the provider.
        /// </summary>
        /// <param name="appConfigPath">The app config path.</param>
        private static void SetProvider(string appConfigPath)
        {
            //clear the providers and reset
            DataService.Provider = new SqlDataProvider();
            DataService.Providers = new DataProviderCollection();

            //if present, get the connection strings and the SubSonic config
            if(File.Exists(appConfigPath))
            {
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                Console.WriteLine("Building configuration from " + Path.Combine(Directory.GetCurrentDirectory(), appConfigPath));
                fileMap.ExeConfigFilename = appConfigPath;

                // Open another config file 
                Configuration subConfig = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

                try
                {
                    SubSonicSection section = (SubSonicSection)subConfig.GetSection(ConfigurationSectionName.SUB_SONIC_SERVICE);

                    if(section != null)
                    {
                        DataService.ConfigSection = section;
                        string argTemplateDirectory = GetArg(ConfigurationPropertyName.TEMPLATE_DIRECTORY);
                        string activeTemplateDirectory = String.IsNullOrEmpty(argTemplateDirectory) ? section.TemplateDirectory : argTemplateDirectory;

                        string argTraceFlag = GetArg(ConfigurationPropertyName.ENABLE_TRACE);
                        string activeTraceFlag = String.IsNullOrEmpty(argTraceFlag) ? section.EnableTrace : argTraceFlag;

                        if(!String.IsNullOrEmpty(activeTraceFlag))
                            DataService.EnableTrace = Convert.ToBoolean(activeTraceFlag);

                        if(!String.IsNullOrEmpty(activeTemplateDirectory))
                        {
                            Console.WriteLine("Overriding default templates with those from " + section.TemplateDirectory);
                            CodeService.TemplateDirectory = activeTemplateDirectory;
                        }

                        //initialize
                        //need to pull out the default connection string
                        //since this application doesn't have a config file, the target one does
                        //so reconciling connection string won't work
                        string connectionStringName = section.Providers[0].Parameters["connectionStringName"];
                        if(connectionStringName == null)
                            throw new ConfigurationErrorsException("The Parameter 'connectionStringName' was not specified");

                        ConnectionStringSettings connSettings = subConfig.ConnectionStrings.ConnectionStrings[connectionStringName];
                        if(connSettings == null)
                            throw new ConfigurationErrorsException(string.Format("ConnectionStrings section missing connection string with the name '{0}'", connectionStringName));

                        string connString = subConfig.ConnectionStrings.ConnectionStrings[connectionStringName].ConnectionString;
                        //DataService.ConnectionString = connString;

                        ProvidersHelper.InstantiateProviders(section.Providers, DataService.Providers, typeof(DataProvider));

                        //this is a tad backwards, but it's what needs to happen since our application
                        //is configuring another application's providers
                        //go back and reset the provider's connection strings

                        //int counter = 0;
                        foreach(DataProvider provider in DataService.Providers)
                        {
                            Console.WriteLine("Adding connection to " + provider.Name);

                            provider.SetDefaultConnectionString(subConfig.ConnectionStrings.ConnectionStrings[provider.ConnectionStringName].ConnectionString);
                            //provider.ConnectionString = subConfig.ConnectionStrings.ConnectionStrings[provider.ConnectionStringName].ConnectionString;
                        }

                        //reset the default provider
                        string providerName = GetArg("provider");
                        if(providerName != String.Empty)
                        {
                            try
                            {
                                DataService.Provider = DataService.Providers[providerName];
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine("ERROR: There is no provider with the name '{0}'. Exception: {1}", providerName, e);
                            }
                        }
                        else
                            DataService.Provider = DataService.Providers[section.DefaultProvider];
                    }
                }
                catch(ConfigurationErrorsException x)
                {
                    //let the user know the config was problematic...
                    Console.WriteLine(
                        "Can't set the configuration for the providers. There is an error with your config setup (did you remember to configure SubSonic in your config file?). '{0}'",
                        x.Message);
                }
            }
            else
                throw new Exception("There's no config file present at " + appConfigPath);
        }

        /// <summary>
        /// Gets the output directory.
        /// </summary>
        /// <returns></returns>
        private static string GetOutputDirectory()
        {
            string result;

            //this can be an absolute reference, or a partial name of a directory
            //like "App_Code/Generated"

            //see if this path is absolute
            string thisOutput = GetArg("out");

            //if there's a drive specified, then it's absolute
            result = thisOutput.Contains(":") ? thisOutput : Path.Combine(Directory.GetCurrentDirectory(), thisOutput);            

            //now, if the output directory doesn't exist, create it
            if(!Directory.Exists(result))
                Directory.CreateDirectory(result);

            return result;
        }

        #endregion


        #region Utility

        /// <summary>
        /// Outputs the file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileText">The file text.</param>
        private static void OutputFile(string filePath, string fileText)
        {
            using(StreamWriter sw = File.CreateText(filePath))
                sw.Write(fileText);
        }

        /// <summary>
        /// Gets the arg.
        /// </summary>
        /// <param name="argSwitch">The arg switch.</param>
        /// <returns></returns>
        private static string GetArg(string argSwitch)
        {
            return arguments[argSwitch] ?? String.Empty;
        }

        /// <summary>
        /// Gets the connnection string.
        /// </summary>
        /// <returns></returns>
        private static string GetConnnectionString()
        {
            bool haveError = false;

            string server = GetArg("server");
            string db = GetArg("db");

            if(server == String.Empty)
            {
                Console.WriteLine("No server name was passed in - please specify using /s MyServerName");
                haveError = true;
            }
            if(db == String.Empty)
            {
                Console.WriteLine("No Database name was passed in - please specify using /db MyDatabaseName");
                haveError = true;
            }

            //optional
            //string tableList = GetArg("tablelist"); //Not being used.
            string userID = GetArg("userid");
            string pasword = GetArg("password");
            string sConn = String.Empty;
            if(!haveError)
            {
                sConn = "Server=" + server + ";Database=" + db + ";";
                if(userID == String.Empty)
                    sConn += "Integrated Security=SSPI;";
                else
                    sConn += "User ID=" + userID + ";Password=" + pasword;
            }
            else
                ThrowHelp(false);

            return sConn;
        }

        #endregion


        #region Scripters

        /// <summary>
        /// Versions the DB.
        /// </summary>
        private static void VersionDB()
        {
            ScriptSchema();
            ScriptData();
        }

        /// <summary>
        /// Scripts the data.
        /// </summary>
        private static void ScriptData()
        {
            SetProvider();
            //for this to work, we need a Servername, DB, and output. Optional elements are table list and user id/password
            //string[] tables = DataService.GetTableNames(SubSonicConfig.ProviderName);
            foreach(DataProvider provider in DataService.Providers)
            {
                //string[] tables = DataService.GetTableNames(provider.Name);
                string[] tables = DataService.GetOrderedTableNames(provider.Name);

                string outDir = GetOutputDirectory();
                if(outDir == String.Empty)
                    outDir = Directory.GetCurrentDirectory();

                Utility.WriteTrace("Scripting Data");
                Utility.WriteTrace("#####################################");

                string outFileName = string.Format("{0}_Data_{1}_{2}_{3}.sql", provider.Name, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                string outPath = Path.Combine(outDir, outFileName);

                using(StreamWriter sw = File.CreateText(outPath))
                {
                    foreach(string tbl in tables)
                    {
                        if(IsInList(tbl) || CodeService.ShouldGenerate(tbl, provider.Name))
                        {
                            Utility.WriteTrace(String.Format("Scripting Table: {0}", tbl));
                            string dataScript = DBScripter.ScriptData(tbl, provider.Name);
                            sw.Write(dataScript);
                            sw.Write(Environment.NewLine);
                        }
                    }
                }

                Utility.WriteTrace("Finished!");
            }
        }

        /// <summary>
        /// Scripts the schema.
        /// </summary>
        private static void ScriptSchema()
        {
            SetProvider();

            foreach(DataProvider provider in DataService.Providers)
            {
                string sConn = provider.DefaultConnectionString; //GetConnnectionString();

                if(sConn != String.Empty)
                {
                    Utility.WriteTrace("Scripting Schema:" + provider.Name);
                    Utility.WriteTrace("#####################################");
                    //string db = GetArg("db");
                    string outDir = GetOutputDirectory();

                    string schema = DBScripter.ScriptSchema(sConn);
                    string outFileName =
                        string.Format("{0}_{1}_{2}_{3}_{4}_Schema.sql", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Environment.UserName, provider.Name);
                    string outPath = Path.Combine(outDir, outFileName);

                    OutputFile(outPath, schema);
                    Console.WriteLine("Finished!");
                }
            }
        }

        #endregion


        #region Generators

        /// <summary>
        /// Generates the editor.
        /// </summary>
        private static void GenerateEditor()
        {
            string table = GetArg("table");
            string outDir = GetOutputDirectory();
            GenerateEditor(table, outDir);
        }

        /// <summary>
        /// Generates the editor.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="outDir">The out dir.</param>
        private static void GenerateEditor(string table, string outDir)
        {
            bool haveError = false;
            WriteVersionInformation(String.Empty);

            if(table == String.Empty)
            {
                Console.WriteLine("No table name was entered; please specify the name of the table using /table");
                haveError = true;
            }

            if(!haveError)
            {
                Console.WriteLine("Generating editor for " + table);

                if(DataService.Provider == null)
                    SetProvider();

                language = CodeLanguageFactory.GetByShortName(GetArg("lang"));

                TableSchema.Table tableSchema = DataService.GetSchema(table, DataService.Provider.Name, TableType.Table);
                string pageName = tableSchema.ClassName + "Editor.aspx";
                string codePageName = pageName + language.FileExtension;
                string pageFile = Path.Combine(outDir, pageName);
                string codeFile = Path.Combine(outDir, codePageName);

                //generate up the editor
                try
                {
                    //string page = ScaffoldCodeGenerator.GeneratePage(DataService.Provider.Name, table, pageName, "", langType);
                    //string code = ScaffoldCodeGenerator.GenerateCode(pageName, DataService.Provider.Name, table, langType);
                    //string page = "";
                    //string code = "";
                    //Clipboard.SetData(System.Windows.Forms.DataFormats.StringFormat, page);
                    //OutputFile(pageFile, page);
                    //OutputFile(codeFile, code);
                    //Console.WriteLine("Copied to clipboard" + pageName);
                }
                catch
                {
                    Console.WriteLine("ERROR: Can't generate editor for " + table + ".");
                    Console.WriteLine("Please check the table name and that the you specified the right provider (you can set the provider by using /provider)");
                }
            }
            else
                ThrowHelp(false);
        }

        /// <summary>
        /// Generates all.
        /// </summary>
        private static void GenerateAll()
        {
            GenerateTables();
            GenerateODSControllers();
            GenerateViews();
			GenerateSPs();
			GenerateEnums();
			GenerateStructs();
        }

        /// <summary>
        /// Determines whether the specified table name is excluded.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        /// 	<c>true</c> if the specified table name is excluded; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsExcluded(string tableName)
        {
            bool bOut = false;
            string excludeList = GetArg("excludelist");

            if(excludeList != String.Empty)
            {
                string[] tables = excludeList.Split(',');
                foreach(string tbl in tables)
                {
                    if(tbl.ToLower() == tableName.ToLower())
                    {
                        bOut = true;
                        break;
                    }
                }
            }
            return bOut;
        }

        /// <summary>
        /// Determines whether [is in list] [the specified table name].
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>
        /// 	<c>true</c> if [is in list] [the specified table name]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsInList(string tableName)
        {
            string tableList = GetArg("tablelist");
            bool bOut = false;

            if(tableList.Trim() == "*")
                bOut = true;
            else
            {
                if(tableList != String.Empty)
                {
                    string[] tables = tableList.Split(',');
                    foreach(string tbl in tables)
                    {
                        if(tbl.ToLower() == tableName.ToLower())
                        {
                            bOut = true;
                            break;
                        }
                    }
                }
                else
                {
                    //it's not set, default it to true
                    bOut = true;
                }
            }

            return bOut;
        }

        /// <summary>
        /// Gets the out sub dir.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        private static string GetOutSubDir(ProviderBase provider)
        {
            string outDir = GetOutputDirectory();
            if(outDir == String.Empty)
                outDir = Directory.GetCurrentDirectory();

            if(DataService.Providers.Count > 1)
            {
                //this is set by the routines
                outDir = Path.Combine(outDir, provider.Name);
            }
            if(!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);

            return outDir;
        }

        /// <summary>
        /// Generates the tables.
        /// </summary>
        private static void GenerateTables()
        {
            //if (DataService.Provider == null)
            SetProvider();

            language = CodeLanguageFactory.GetByShortName(GetArg("lang"));
            //string usings = ParseUtility.GetUsings(language);

            if(DataService.Providers.Count == 0)
                Console.WriteLine("There's a problem with the providers - none were loaded and no exceptions where thrown.");
            else
            {
                //loop the providers, and if there's more than one, output to their own folder
                //for tidiness
                foreach(DataProvider provider in DataService.Providers)
                {
                    //get the table list
                    string[] tables = DataService.GetTableNames(provider.Name);

                    int tableCount = 0;

                    //adjust based on IsExcluded
                    foreach(string tbl in tables)
                    {
                        if(IsInList(tbl) && !IsExcluded(tbl) && CodeService.ShouldGenerate(tbl, provider.Name))
                            tableCount++;
                    }

                    string message = "Generating classes for " + provider.Name + " (" + tables.Length + " total)";
                    if(tableCount > 200)
                    {
                        message +=
                            " that's a serious amount of tables to generate. But we can handle it. You just will need to be patient and go get some coffee while we do this thang...";
                    }
                    else if(tableCount > 100)
                        message += " that's a lot of tables. This could take a few minutes...";
                    else if(tableCount > 50)
                        message += " - moderate amount of tables... this could take 30 seconds or so...";

                    Console.WriteLine(message);
                    string baseDir = GetOutSubDir(provider);
                                        
                    foreach(string tbl in tables)
                    {
                        if(IsInList(tbl) && !IsExcluded(tbl) && CodeService.ShouldGenerate(tbl, provider.Name))
                        {
                            TableSchema.Table tableSchema = DataService.GetSchema(tbl, provider.Name, TableType.Table);
                            string className = tableSchema.ClassName;
                            TurboTemplate tt = CodeService.BuildClassTemplate(tbl, language, provider);

                            string additionalPath = GetAdditionalPath(tableSchema, GenerationType.Models);
                            string outDir = Path.Combine(baseDir, additionalPath);
                            EnsureDirectoryExists(outDir);

                            tt.OutputPath = Path.Combine(outDir, className + language.FileExtension);                            

                            turboCompiler.AddTemplate(tt);
                        }
                    }
                }
                Console.WriteLine("Finished");
            }
        }

        private static string GetAdditionalPath(TableSchema.Table tableSchema, string generationType)
        {
            string groupOutputValue = GetArg("groupOutput");
            if (!string.IsNullOrEmpty(groupOutputValue))
            {
                return GetOutputGroupingPath(tableSchema, groupOutputValue, generationType);                
            }

            return string.Empty;
        }

        private static string GetOutputGroupingPath(TableSchema.Table tableSchema, string groupOutputValue, string typeOfGeneration)
        {
            switch(groupOutputValue.ToLower())
            {
                case "schema":
                    return tableSchema.SchemaName;

                case "type":
                    return typeOfGeneration;

                case "schemaandtype":
                    return tableSchema.SchemaName + "\\" + typeOfGeneration;

                default:
                    throw new ArgumentOutOfRangeException("Valid values for /groupOutput are [schema,type,schemaAndType]");
            }
        }

        /// <summary>
        /// Generates the ODS controllers.
        /// </summary>
        private static void GenerateODSControllers()
        {
            SetProvider();

            language = CodeLanguageFactory.GetByShortName(GetArg("lang"));

            if(DataService.Providers.Count == 0)
                Console.WriteLine("There's a problem with the providers - none were loaded and no exceptions where thrown.");
            else
            {
                //loop the providers, and if there's more than one, output to their own folder
                //for tidiness
                foreach(DataProvider provider in DataService.Providers)
                {
                    if(provider.TableBaseClass == "ActiveRecord")
                    {
                        //get the table list
                        string[] tables = DataService.GetTableNames(provider.Name);
                        string message = "Generating ODS Controllers for " + provider.Name + " (" + tables.Length + " total)";

                        if(tables.Length > 200)
                        {
                            message +=
                                " that's a serious amount of tables to generate. But we can handle it. You just will need to be patient and go get some coffee while we do this thang...";
                        }
                        else if(tables.Length > 100)
                            message += " that's a lot of tables. This could take a few minutes...";
                        else if(tables.Length > 50)
                            message += " - moderate amount of tables... this could take 30 seconds or so...";

                        Console.WriteLine(message);
                        string baseDir = GetOutSubDir(provider);
                        
                        if(!Directory.Exists(baseDir))
                            Directory.CreateDirectory(baseDir);
                   
                        foreach(string tbl in tables)
                        {
                            if(IsInList(tbl) && !IsExcluded(tbl) && CodeService.ShouldGenerate(tbl, provider.Name))
                            {
                                TableSchema.Table tableSchema = DataService.GetSchema(tbl, provider.Name, TableType.Table);
                                string className = tableSchema.ClassName;
                                TurboTemplate tt = CodeService.BuildODSTemplate(tbl, language, provider);

                                string additionalPath = GetAdditionalPath(tableSchema, GenerationType.Controllers);
                                string outDir = Path.Combine(baseDir, additionalPath);
                                EnsureDirectoryExists(outDir);

                                if(tt != null)
                                {
                                    tt.OutputPath = Path.Combine(outDir, className + "Controller" + language.FileExtension);
                                    turboCompiler.AddTemplate(tt);
                                }
                            }
                        }
                    }
                }
                Console.WriteLine("Finished");
            }
        }

        private static void EnsureDirectoryExists(string outDir)
        {
            if (!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
        }

        /// <summary>
        /// Generates the views.
        /// </summary>
        private static void GenerateViews()
        {
            SetProvider();

            language = CodeLanguageFactory.GetByShortName(GetArg("lang"));

            //loop the providers, and if there's more than one, output to their own folder
            //for tidiness
            foreach(DataProvider provider in DataService.Providers)
            {
                //get the view list
                string[] views = DataService.GetViewNames(provider.Name);
                string baseDir = GetOutSubDir(provider);
                
                foreach(string tbl in views)
                {
                    if(IsInList(tbl) && !IsExcluded(tbl) && CodeService.ShouldGenerate(tbl, provider.Name))
                    {
                        TableSchema.Table tableSchema = DataService.GetSchema(tbl, provider.Name, TableType.View);
                        string className = tableSchema.ClassName;
                        TurboTemplate tt = CodeService.BuildViewTemplate(tbl, language, provider);

                        string additionalPath = GetAdditionalPath(tableSchema, GenerationType.Views);
                        string outDir = Path.Combine(baseDir, additionalPath);
                        EnsureDirectoryExists(outDir);

                        tt.OutputPath = Path.Combine(outDir, className + language.FileExtension);
                        turboCompiler.AddTemplate(tt);
                    }
                }
            }
        }

        /// <summary>
        /// Generates the S ps.
        /// </summary>
        private static void GenerateSPs()
        {
            SetProvider();

            language = CodeLanguageFactory.GetByShortName(GetArg("lang"));

            //loop the providers, and if there's more than one, output to their own folder
            //for tidiness
            foreach(DataProvider provider in DataService.Providers)
            {
                string outDir = GetOutSubDir(provider);
                if(outDir == String.Empty)
                    outDir = Directory.GetCurrentDirectory();

                if(!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);
                string outPath = Path.Combine(outDir, "StoredProcedures" + language.FileExtension);
                Console.WriteLine("Generating SPs to " + outPath);

                TurboTemplate tt = CodeService.BuildSPTemplate(language, provider);
                tt.OutputPath = outPath;
                turboCompiler.AddTemplate(tt);
            }

            Console.WriteLine("Finished");
        }

        /// <summary>
        /// Generates the S ps.
        /// </summary>
		private static void GenerateEnums()
        {
            SetProvider();

            language = CodeLanguageFactory.GetByShortName(GetArg("lang"));

            //loop the providers, and if there's more than one, output to their own folder
            //for tidiness
            foreach(DataProvider provider in DataService.Providers)
            {
                string outDir = GetOutSubDir(provider);
                if(outDir == String.Empty)
                    outDir = Directory.GetCurrentDirectory();

                if(!Directory.Exists(outDir))
                    Directory.CreateDirectory(outDir);
                string outPath = Path.Combine(outDir, "Enums" + language.FileExtension);
                Console.WriteLine("Generating Enums to " + outPath);

				TurboTemplate tt = CodeService.BuildEnumTemplate(language, provider);
                tt.OutputPath = outPath;
                turboCompiler.AddTemplate(tt);
            }

            Console.WriteLine("Finished");
        }

        /// <summary>
        /// Generates the structs.
        /// </summary>
        private static void GenerateStructs()
        {
            SetProvider();

            language = CodeLanguageFactory.GetByShortName(GetArg("lang"));

            string outDir = GetOutputDirectory();
            if(outDir == String.Empty)
                outDir = Directory.GetCurrentDirectory();
            
            if(!Directory.Exists(outDir))
                Directory.CreateDirectory(outDir);
            
            string outPath = Path.Combine(outDir, "AllStructs" + language.FileExtension);
            Console.WriteLine("Generating Structs to " + outPath);
            TurboTemplate tt = CodeService.BuildStructsTemplate(language, DataService.Provider);
            tt.OutputPath = outPath;
            turboCompiler.AddTemplate(tt);

            Console.WriteLine("Finished");
        }

        private static class GenerationType
        {
            public const string Models = "Models";
            public const string Controllers = "Controllers";
            public const string Views = "Views";            
        }

        #endregion
    }
}