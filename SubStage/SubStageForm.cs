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
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using ComponentFactory.Krypton.Toolkit;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using PropertyGridEx;
using SubSonic.Sugar;
using SubSonic.Utilities;
using Configuration=System.Configuration.Configuration;
using CPN = SubSonic.ConfigurationPropertyName;
using DM = SubSonic.SubStage.MasterStore;
using Server=Wilco.WebServer.Server;

namespace SubSonic.SubStage
{
    public partial class SubStageForm : KryptonForm
    {
        private const string CODE_GENERATION = "Code Generation";
        private const string COLUMN_IMAGE = "textfield.png";
        private const string COLUMN_PK_IMAGE = "textfield_key.png";
        private const string CONNECTION_STRING_FOLDER_IMAGE = "folder_link.png";
        private const string CONNECTION_STRING_IMAGE = "link.png";
        private const string DRIVE_IMAGE = "drive.png";
        private const string FILE_URL_PREFIX = @"file:\\\";
        private const string FOLDER_IMAGE = "folder.png";
        private const string GLOBAL = "Global";
        private const string PROJECT_IMAGE = "package.png";
        private const string PROVIDER_IMAGE = "database.png";
        private const string REGULAR_EXPRESSIONS = "Regular Expressions";
        private const string SP_FOLDER_IMAGE = "folder.png";
        private const string SP_IMAGE = "database_gear.png";
        private const string SP_PARAM_IMAGE = "script_gear.png";
        private const string STORED_PROCEDURES = "Stored Procedures";
        private const string SUBSTAGE_CONFIGURATION_IMAGE = "brick.png";
        private const string TABLE_EXCLUDED_IMAGE = "table_delete.png";
        private const string TABLE_FOLDER_IMAGE = "folder.png";
        private const string TABLE_IMAGE = "table.png";
        private const string TABLE_NO_PK_IMAGE = "table_error.png";
        private const string TABLES = "Tables";
        private const string TEMPLATE_CONNECTION_STRINGS = "[#CONNECTION_STRINGS#]";
        private const string TEMPLATE_DEFAULT_PROVIDER = "[#DEFAULT_PROVIDER#]";
        private const string TEMPLATE_ENABLE_TRACE = "[#ENABLE_TRACE#]";
        private const string TEMPLATE_PROVIDERS = "[#PROVIDERS#]";
        private const string TEMPLATE_SCAFFOLD_PROPERTIES = "[#CUSTOM_PROPERTIES#]";
        private const string TEMPLATE_TEMPLATE_DIRECTORY = "[#TEMPLATE_DIRECTORY#]";
        private const string VIEW_FOLDER_IMAGE = "folder.png";
        private const string VIEW_IMAGE = "database_table.png";
        private const string VIEWS = "Views";
        private static Thread serverThread;
        private Server activeServer;
        private MasterStore.SubStageConfigurationRow config;
        private string configTemplate;
        private string embeddedWebRootPath;
        private string executionDirectory;
        private bool navigatingFromFileTree;
        private string scaffoldFilePath;
        private string scaffoldFileRedirectorPath;
        private string scaffoldInfoFilePath;
        private string scaffoldInfoUrlPath;
        private string scaffoldTemplate;
        private string scaffoldUrlPath;
        private string webBinPath;
        private string webConfigPath;

        public SubStageForm(string[] args)
        {
            InitializeComponent();

            LoadConfiguration();

            ClearWebBinDirectory();
            CopyToWebBinDirectory();

            pGrid.ToolStrip.Items[2].Visible = false;
            pGrid.ToolStrip.Items[3].Visible = false;
            BuildTree();

            activeServer = CreateServer("localhost", "/", embeddedWebRootPath, config.WebServerPort);

            if(args != null && args.Length == 1)
                ImportProject(args[0]);

            if(treeFileSystem.SelectedNode != null)
                NavigateFileBrowser(FILE_URL_PREFIX + treeFileSystem.SelectedNode.Tag);
            else
                NavigateFileBrowser(FILE_URL_PREFIX + "C:\\");
        }

        private static ModelManager MM
        {
            get { return ModelManager.Instance; }
        }

        private StageNode SelectedNode
        {
            get
            {
                if(treeView1.SelectedNode != null)
                    return (StageNode)treeView1.SelectedNode;
                return null;
            }
        }


        #region Events

        private void miFileExit_Click(object sender, EventArgs e)
        {
            if(activeServer != null)
            {
                //serverThread.Interrupt();
                activeServer.Stop();
            }
            Close();
        }

        private void miHelpAbout_Click(object sender, EventArgs e)
        {
            About aboutForm = new About();
            aboutForm.ShowDialog();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SelectNode((StageNode)e.Node);
        }

        private void propertyGridEx1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            StageNode nodeTag = (StageNode)pGrid.Tag;
            if(nodeTag.IsProjectNode)
            {
                SaveProjectProperties(e.ChangedItem);
                BuildConfig(nodeTag.Project);
            }
            else if(nodeTag.IsProviderNode)
            {
                SaveProviderProperties(e.ChangedItem);
                BuildConfig(nodeTag.Project);
            }
            else if(nodeTag.IsConnectionStringNode)
                SaveConnectionStringProperties(e.ChangedItem);
            else if(nodeTag.NodeType == StageNodeType.SubStageConfiguration)
                SaveConfigurationProperties(e.ChangedItem);
        }

        private void btnNewProject_Click(object sender, EventArgs e)
        {
            AddProject();
        }

        private void cmTree_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem == cmiTreeDeleteProject && SelectedNode.IsProjectNode)
            {
                DeleteProject(SelectedNode.Project.ProjectId);
                SelectedNode.Remove();
            }
            else if(e.ClickedItem == cmiTreeDeleteProvider && SelectedNode.IsProviderNode)
            {
                DeleteProvider(SelectedNode.Provider.ProviderId);
                SelectedNode.Remove();
            }
            else if(e.ClickedItem == cmiTreeDeleteConnectionString && SelectedNode.IsConnectionStringNode)
            {
                DeleteConnectionString(SelectedNode.ConnectionString.ConnectionStringId);
                SelectedNode.Remove();
            }
            else if(e.ClickedItem == cmiTreeAddProvider && SelectedNode.IsProjectNode)
            {
                AddProvider(SelectedNode.Project.ProjectId);
                //BuildTree();
            }
            else if(e.ClickedItem == cmiTreeAddConnectionString && SelectedNode.IsConnectionStringFolderNode)
                AddConnectionString();
            else if(e.ClickedItem == cmiGenerateObjectEnabled && SelectedNode.IsTableNode || SelectedNode.IsViewNode)
                EnableDisableTablesViews();
        }

        /// <summary>
        /// Enables or disable a table or a view.
        /// </summary>
        private void EnableDisableTablesViews()
        {
            MasterStore.ProvidersRow provider = SelectedNode.Provider;
            List<string> genGroups = new List<string>(provider.ExcludeTableList.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries));
            if(SelectedNode.NodeType == StageNodeType.Table || SelectedNode.NodeType == StageNodeType.View)
                genGroups.Add(string.Format("^{0}$", SelectedNode.DatabaseName));
            else if(SelectedNode.NodeType == StageNodeType.TableExcluded || SelectedNode.NodeType == StageNodeType.ViewExcluded)
            {
                foreach(string s in provider.ExcludeTableList.Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries))
                {
                    if(Utility.IsRegexMatch(SelectedNode.DatabaseName, s.Trim()))
                    {
                        genGroups.Remove(s);
                        //break;//There might be a few stuff that are blocking it from being generated so dont break out.
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < genGroups.Count; i++)
            {
                if(i < genGroups.Count - 1)
                    sb.Append(genGroups[i] + ",");
                else
                    sb.Append(genGroups[i]);
            }

            provider.ExcludeTableList = sb.ToString();
            MM.Save();

            //Lets reload the tree and expand the selected node
            int stagenode = SelectedNode.Index; //save the old index
            bool isOnTables = ((StageNode)SelectedNode.Parent).NodeType == StageNodeType.TableFolder;
            PopulateProviderData((StageNode)SelectedNode.Parent.Parent);
            if(isOnTables)
            {
                //SelectedNode.NextVisibleNode.Expand();
                SelectedNode.NextVisibleNode.Nodes[stagenode].EnsureVisible();
            }
            else
            {
                //SelectedNode.NextVisibleNode.NextVisibleNode.Expand();
                SelectedNode.NextVisibleNode.NextVisibleNode.Nodes[stagenode].EnsureVisible();
            }
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Right)
            {
                StageNode thisNode = (StageNode)treeView1.GetNodeAt(e.X, e.Y);
                if(thisNode != null)
                {
                    treeView1.SelectedNode = thisNode;
                    ToggleContextMenuItems(thisNode);
                    cmTree.Show(treeView1, e.X, e.Y);
                }
            }
        }

        private void btnNewProvider_Click(object sender, EventArgs e)
        {
            if(SelectedNode != null)
            {
                if(SelectedNode.IsProjectNode)
                    AddProvider(SelectedNode.RowId);
            }
        }

        private void btnAddConnectionString_Click(object sender, EventArgs e)
        {
            if(SelectedNode != null)
            {
                if(SelectedNode.IsConnectionStringFolderNode)
                    AddConnectionString();
            }
        }

        private void btnInvokeProviders_Click(object sender, EventArgs e)
        {
            if(SelectedNode != null)
            {
                if(SelectedNode.IsProjectNode || SelectedNode.IsProviderNode)
                    PopulateProviderData(SelectedNode);
            }
        }

        private void ImportProject(string importConfigPath)
        {
            if(File.Exists(importConfigPath))
            {
                FileInfo fi = new FileInfo(importConfigPath);
                DirectoryInfo di = fi.Directory;
                if(di != null)
                {
                    string originalDirectory = di.FullName;
                    string projectName = di.Name;

                    ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                    fileMap.ExeConfigFilename = importConfigPath;
                    Configuration subConfig = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
                    SubSonicSection section = (SubSonicSection)subConfig.GetSection(ConfigurationSectionName.SUB_SONIC_SERVICE);

                    if(section != null)
                    {
                        MasterStore.ProjectsRow newProject = MM.Projects.NewProjectsRow();
                        newProject.Name = projectName;
                        newProject.OriginalLocation = originalDirectory;
                        newProject.EnableTrace = Convert.ToBoolean(!String.IsNullOrEmpty(section.EnableTrace) ? section.EnableTrace : MM.Projects.EnableTraceColumn.DefaultValue);
                        newProject.TemplateDirectory =
                            Convert.ToString(!String.IsNullOrEmpty(section.TemplateDirectory) ? section.TemplateDirectory : MM.Projects.TemplateDirectoryColumn.DefaultValue);
                        MM.Projects.AddProjectsRow(newProject);

                        ConnectionStringSettingsCollection connStrings = new ConnectionStringSettingsCollection();
                        if(subConfig.ConnectionStrings != null)
                            connStrings = subConfig.ConnectionStrings.ConnectionStrings;

                        List<int> insertedConnStringIds = new List<int>();
                        //Loop through connection strings and remove any that aren't referenced by the provider.
                        foreach(ConnectionStringSettings connSetting in connStrings)
                        {
                            bool foundMatch = false;
                            foreach(ProviderSettings providerSettings in section.Providers)
                            {
                                if(providerSettings.Parameters.Get(ConfigurationPropertyName.CONNECTION_STRING_NAME) != null)
                                {
                                    if(providerSettings.Parameters.Get(ConfigurationPropertyName.CONNECTION_STRING_NAME) == connSetting.Name)
                                    {
                                        foundMatch = true;
                                        break;
                                    }
                                }
                            }
                            if(!foundMatch)
                                connStrings.Remove(connSetting);
                            else
                            {
                                int referencedId = -1;
                                foreach(MasterStore.ConnectionStringsRow existingConnection in MM.ConnectionStrings)
                                {
                                    if(existingConnection.Name == connSetting.Name && existingConnection.ConnectionString == connSetting.ConnectionString)
                                    {
                                        referencedId = existingConnection.ConnectionStringId;
                                        break;
                                    }
                                }
                                if(referencedId < 0)
                                {
                                    MasterStore.ConnectionStringsRow newConnection = (MasterStore.ConnectionStringsRow)MM.ConnectionStrings.NewRow();
                                    newConnection.Name = connSetting.Name;
                                    newConnection.ConnectionString = connSetting.ConnectionString;
                                    MM.ConnectionStrings.AddConnectionStringsRow(newConnection);
                                    referencedId = newConnection.ConnectionStringId;
                                }
                                insertedConnStringIds.Add(referencedId);
                            }
                        }

                        foreach(ProviderSettings pset in section.Providers)
                        {
                            MasterStore.ProvidersRow newProvider = (MasterStore.ProvidersRow)MM.Providers.NewRow();
                            newProvider.Name = pset.Name;
                            newProvider.ProjectId = newProject.ProjectId;

                            string connectionName = GetConfig(pset, ConfigurationPropertyName.CONNECTION_STRING_NAME, null).ToString();

                            MasterStore.ConnectionStringsRow[] conns =
                                (MasterStore.ConnectionStringsRow[])MM.ConnectionStrings.Select(MM.ConnectionStrings.NameColumn + " = '" + connectionName + "'");

                            int activeConnectionId = -1;
                            if(conns.Length > 1)
                            {
                                foreach(MasterStore.ConnectionStringsRow cRow in MM.ConnectionStrings)
                                {
                                    if(insertedConnStringIds.Contains(cRow.ConnectionStringId))
                                    {
                                        activeConnectionId = cRow.ConnectionStringId;
                                        break;
                                    }
                                }
                            }
                            else if(conns.Length == 1)
                                activeConnectionId = conns[0].ConnectionStringId;
                            else
                                ShowStatus("Invalid Configuration File... Import Aborted.");

                            newProvider.ConnectionStringId = activeConnectionId;
                            string providerType = section.Providers[0].Type.Split(new char[] {'.', ','})[1].Trim();

                            bool foundProvider = false;
                            foreach(MasterStore.ProviderTypesRow providerTypeRow in MM.ProviderTypes)
                            {
                                if(providerTypeRow.InternalName == providerType)
                                {
                                    newProvider.ProviderTypeId = providerTypeRow.ProviderTypeId;
                                    foundProvider = true;
                                    break;
                                }
                            }

                            if(!foundProvider)
                                newProvider.ProviderTypeId = MM.ProviderTypes[0].ProviderTypeId;

                            newProvider.AppendWith = GetConfig(pset, ConfigurationPropertyName.APPEND_WITH, MM.Providers.AppendWithColumn).ToString();
                            newProvider.AdditionalNamespaces = GetConfig(pset, ConfigurationPropertyName.ADDITIONAL_NAMESPACES, MM.Providers.AdditionalNamespacesColumn).ToString();
                            newProvider.ExcludeProcedureList = GetConfig(pset, ConfigurationPropertyName.EXCLUDE_PROCEDURE_LIST, MM.Providers.ExcludeProcedureListColumn).ToString();
                            newProvider.ExcludeTableList = GetConfig(pset, ConfigurationPropertyName.EXCLUDE_TABLE_LIST, MM.Providers.ExcludeTableListColumn).ToString();
                            newProvider.ExtractClassNameFromSPName =
                                Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.EXTRACT_CLASS_NAME_FROM_SP_NAME, MM.Providers.ExtractClassNameFromSPNameColumn));
                            newProvider.FixDatabaseObjectCasing =
                                Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.FIX_DATABASE_OBJECT_CASING, MM.Providers.FixDatabaseObjectCasingColumn));
                            newProvider.FixPluralClassNames =
                                Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.FIX_PLURAL_CLASS_NAMES, MM.Providers.FixPluralClassNamesColumn));
                            newProvider.GeneratedNamespace = GetConfig(pset, ConfigurationPropertyName.GENERATED_NAMESPACE, MM.Providers.GeneratedNamespaceColumn).ToString();
                            newProvider.GenerateLazyLoads = Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.GENERATE_LAZY_LOADS, MM.Providers.GenerateLazyLoadsColumn));
                            newProvider.GenerateNullableProperties =
                                Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.GENERATE_NULLABLE_PROPERTIES, MM.Providers.GenerateNullablePropertiesColumn));
                            newProvider.GenerateODSControllers =
                                Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.GENERATE_ODS_CONTROLLERS, MM.Providers.GenerateODSControllersColumn));
                            newProvider.GenerateRelatedTablesAsProperties =
                                Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.GENERATE_RELATED_TABLES_AS_PROPERTIES,
                                    MM.Providers.GenerateRelatedTablesAsPropertiesColumn));
                            newProvider.IncludeProcedureList = GetConfig(pset, ConfigurationPropertyName.INCLUDE_PROCEDURE_LIST, MM.Providers.IncludeProcedureListColumn).ToString();
                            newProvider.IncludeTableList = GetConfig(pset, ConfigurationPropertyName.INCLUDE_TABLE_LIST, MM.Providers.IncludeTableListColumn).ToString();
                            newProvider.RegexDictionaryReplace =
                                GetConfig(pset, ConfigurationPropertyName.REGEX_DICTIONARY_REPLACE, MM.Providers.RegexDictionaryReplaceColumn).ToString();
                            newProvider.RegexIgnoreCase = Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.REGEX_IGNORE_CASE, MM.Providers.RegexIgnoreCaseColumn));
                            newProvider.RegexMatchExpression = GetConfig(pset, ConfigurationPropertyName.REGEX_MATCH_EXPRESSION, MM.Providers.RegexMatchExpressionColumn).ToString();
                            newProvider.RegexReplaceExpression =
                                GetConfig(pset, ConfigurationPropertyName.REGEX_REPLACE_EXPRESSION, MM.Providers.RegexReplaceExpressionColumn).ToString();
                            newProvider.RelatedTableLoadPrefix =
                                GetConfig(pset, ConfigurationPropertyName.RELATED_TABLE_LOAD_PREFIX, MM.Providers.RelatedTableLoadPrefixColumn).ToString();
                            newProvider.RemoveUnderscores = Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.REMOVE_UNDERSCORES, MM.Providers.RemoveUnderscoresColumn));
                            newProvider.SetPropertyDefaultsFromDatabase =
                                Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.SET_PROPERTY_DEFAULTS_FROM_DATABASE, MM.Providers.SetPropertyDefaultsFromDatabaseColumn));
                            newProvider.SPStartsWith = GetConfig(pset, ConfigurationPropertyName.SP_STARTS_WITH, MM.Providers.SPStartsWithColumn).ToString();
                            newProvider.SPClassName = GetConfig(pset, ConfigurationPropertyName.STORED_PROCEDURE_CLASS_NAME, MM.Providers.SPClassNameColumn).ToString();
                            newProvider.StoredProcedureBaseClass =
                                GetConfig(pset, ConfigurationPropertyName.STORED_PROCEDURE_BASE_CLASS, MM.Providers.StoredProcedureBaseClassColumn).ToString();
                            newProvider.StripColumnText = GetConfig(pset, ConfigurationPropertyName.STRIP_COLUMN_TEXT, MM.Providers.StripColumnTextColumn).ToString();
                            newProvider.StripParamText = GetConfig(pset, ConfigurationPropertyName.STRIP_PARAM_TEXT, MM.Providers.StripParamTextColumn).ToString();
                            newProvider.StripSPText = GetConfig(pset, ConfigurationPropertyName.STRIP_STORED_PROCEDURE_TEXT, MM.Providers.StripSPTextColumn).ToString();
                            newProvider.StripTableText = GetConfig(pset, ConfigurationPropertyName.STRIP_TABLE_TEXT, MM.Providers.StripTableTextColumn).ToString();
                            newProvider.StripViewText = GetConfig(pset, ConfigurationPropertyName.STRIP_VIEW_TEXT, MM.Providers.StripViewTextColumn).ToString();
                            newProvider.TableBaseClass = GetConfig(pset, ConfigurationPropertyName.TABLE_BASE_CLASS, MM.Providers.TableBaseClassColumn).ToString();
                            newProvider.UseExtendedProperties =
                                Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.USE_EXTENDED_PROPERTIES, MM.Providers.UseExtendedPropertiesColumn));
                            newProvider.UseSPs = Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.USE_STORED_PROCEDURES, MM.Providers.UseSPsColumn));
                            newProvider.UseUTC = Convert.ToBoolean(GetConfig(pset, ConfigurationPropertyName.USE_UTC_TIMES, MM.Providers.UseUTCColumn));
                            newProvider.ViewBaseClass = GetConfig(pset, ConfigurationPropertyName.VIEW_BASE_CLASS, MM.Providers.ViewBaseClassColumn).ToString();
                            newProvider.ViewStartsWith = GetConfig(pset, ConfigurationPropertyName.VIEW_STARTS_WITH, MM.Providers.ViewStartsWithColumn).ToString();

                            MM.Providers.AddProvidersRow(newProvider);

                            if(!String.IsNullOrEmpty(section.DefaultProvider))
                            {
                                if(Utility.IsMatch(section.DefaultProvider, newProvider.Name))
                                    newProject.DefaultProvider = newProvider.ProviderId;
                            }
                        }
                        MM.Save();
                        BuildTree();
                    }
                }
            }
        }

        private void miFileImportProject_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdOpen = new OpenFileDialog();
            fdOpen.Multiselect = false;
            fdOpen.Filter = "Config Files|*.config";
            fdOpen.Title = "Import Existing web.config or app.config File";
            //LoadMasterFile(null);
            bool openedFile = fdOpen.ShowDialog(this) == DialogResult.OK;
            if(openedFile)
                ImportProject(fdOpen.FileName);
        }

        private void pGrid_ToolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem == pgbTestConnection)
            {
                if(SelectedNode.IsProviderNode)
                {
                    try
                    {
                        InstantiateProvider(SelectedNode.Provider);
                        DataService.GetTableNames(SelectedNode.Provider.Name);
                        ShowStatus("Connection Successful.");
                    }
                    catch(Exception ex)
                    {
                        ShowStatus("Connection Failed: " + ex.Message);
                    }
                }
            }
        }

        private void btnSplitGenerateCode_Click(object sender, EventArgs e)
        {
            if(((ToolStripSplitButton)sender).ButtonPressed)
            {
                if(SelectedNode != null)
                {
                    if(SelectedNode.IsProjectNode || SelectedNode.IsProviderNode)
                        GenerateCode(SelectedNode);
                }
            }
        }

        private void miUseGeneratedNames_Click(object sender, EventArgs e)
        {
            miUseDatabaseNames.Checked = !miUseGeneratedNames.Checked;
            ToggleNodeNames();
        }

        private void miUseDatabaseNames_Click(object sender, EventArgs e)
        {
            miUseGeneratedNames.Checked = !miUseDatabaseNames.Checked;
            ToggleNodeNames();
        }

        private void ToggleNodeNames()
        {
            //Using BeginUpdate() & EndUpdate() makes a HUGE difference in performance...
            //Usign them, this operation dropped from 4000ms to 2ms!

            treeView1.BeginUpdate();

            foreach(StageNode node in treeView1.Nodes)
                ToggleNodeNames(node);

            treeView1.EndUpdate();
        }

        private void ToggleNodeNames(StageNode node)
        {
            node.Text = miUseDatabaseNames.Checked ? node.DatabaseName : node.SubSonicName;

            foreach(StageNode subNode in node.Nodes)
                ToggleNodeNames(subNode);
        }

        private void btnCopyConfig_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(tbxConfigOutput.Text);
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            tbxLog.Text = String.Empty;
        }

        private void tbxLog_TextChanged(object sender, EventArgs e)
        {
            tbxLog.ScrollToCaret();
        }

        private void btnSplitGenerateCode_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if(e.ClickedItem == miGenerateVB)
            {
                if(!miGenerateVB.Checked)
                {
                    miGenerateVB.Checked = !miGenerateVB.Checked;
                    miGenerateCSharp.Checked = !miGenerateVB.Checked;
                }
            }
            else if(e.ClickedItem == miGenerateCSharp)
            {
                if(!miGenerateCSharp.Checked)
                {
                    miGenerateCSharp.Checked = !miGenerateCSharp.Checked;
                    miGenerateVB.Checked = !miGenerateCSharp.Checked;
                }
            }

            string currentCodeGenerationLanguage = config.CodeGenerationLanguage;

            if (miGenerateVB.Checked)
                config.CodeGenerationLanguage = "VB";
            else
                config.CodeGenerationLanguage = "C#";

            if (!Utility.IsMatch(config.CodeGenerationLanguage, currentCodeGenerationLanguage))
                MM.Save();
        }

        #endregion


        private void LoadConfiguration()
        {
            config = MM.SubStageConfiguration.FindBySubStageConfigurationId(0);
            if(config == null)
            {
                config = MM.SubStageConfiguration.NewSubStageConfigurationRow();
                config.SubStageConfigurationId = 0;
                MM.SubStageConfiguration.AddSubStageConfigurationRow(config);
                MM.Save();
            }

            if(config.WebServerPort < 1 || config.WebServerPort > 65535)
                config.WebServerPort = Convert.ToInt32(MM.SubStageConfiguration.WebServerPortColumn.DefaultValue);

            executionDirectory = AppDomain.CurrentDomain.BaseDirectory;

            if(String.IsNullOrEmpty(config.CodeGenerationPath))
                config.CodeGenerationPath = executionDirectory + "GeneratedCode\\";
            if(!Directory.Exists(config.CodeGenerationPath))
                Directory.CreateDirectory(config.CodeGenerationPath);

            if(String.IsNullOrEmpty(config.CodeGenerationLanguage) || config.CodeGenerationLanguage.ToUpper().Contains("C"))
            {
                config.CodeGenerationLanguage = "C#";
                miGenerateCSharp.Checked = true;
                miGenerateVB.Checked = false;
            }
            else
            {
                config.CodeGenerationLanguage = "VB";
                miGenerateCSharp.Checked = false;
                miGenerateVB.Checked = true;
            }

            embeddedWebRootPath = executionDirectory + "root\\";
            if(!Directory.Exists(embeddedWebRootPath))
                Directory.CreateDirectory(embeddedWebRootPath);

            webBinPath = embeddedWebRootPath + "bin\\";
            if(!Directory.Exists(webBinPath))
                Directory.CreateDirectory(webBinPath);

            scaffoldFileRedirectorPath = embeddedWebRootPath + "Default.htm";
            if(!File.Exists(scaffoldFileRedirectorPath))
                File.Copy(executionDirectory + "\\Default.htm", scaffoldFileRedirectorPath);

            scaffoldInfoFilePath = embeddedWebRootPath + "Info.htm";
            if(!File.Exists(scaffoldInfoFilePath))
                File.Copy(executionDirectory + "\\Info.htm", scaffoldInfoFilePath);

            scaffoldFilePath = embeddedWebRootPath + "Scaffold.aspx";
            scaffoldUrlPath = "http://localhost:" + config.WebServerPort + "/Default.htm";
            scaffoldInfoUrlPath = "http://localhost:" + config.WebServerPort + "/Info.htm";

            configTemplate = File.ReadAllText(executionDirectory + "ConfigTemplate.txt");
            scaffoldTemplate = File.ReadAllText(executionDirectory + "ScaffoldTemplate.txt");

            webConfigPath = embeddedWebRootPath + "web.config";
        }

        private void BuildDirectoryTree(string selectedPath)
        {
            string[] pathArray = null;
            if(!String.IsNullOrEmpty(selectedPath))
                pathArray = ParseDirectoryPath(selectedPath);
            treeFileSystem.BeginUpdate();
            treeFileSystem.SuspendLayout();
            treeFileSystem.Nodes.Clear();
            string[] drives = Directory.GetLogicalDrives();
            foreach(string drive in drives)
            {
                DriveInfo driveInfo = new DriveInfo(drive);
                if(driveInfo.DriveType != DriveType.CDRom && driveInfo.IsReady)
                {
                    TreeNode node = new TreeNode(drive);
                    node.Tag = drive;
                    node.ImageKey = DRIVE_IMAGE;
                    node.SelectedImageKey = DRIVE_IMAGE;
                    treeFileSystem.Nodes.Add(node);
                    GetSubNodes(node, pathArray);
                }
            }
            treeFileSystem.ResumeLayout();
            treeFileSystem.EndUpdate();
        }

        private void GetSubNodes(TreeNode node, string[] pathArray)
        {
            node.Nodes.Clear();
            string path = node.Tag.ToString();
            string[] subDirs = new string[0];
            const FileAttributes ignoreAttributes = FileAttributes.Hidden | FileAttributes.System | FileAttributes.Offline;
            try
            {
                subDirs = Directory.GetDirectories(path);
            }
            catch {}

            foreach(string subDir in subDirs)
            {
                DirectoryInfo di = new DirectoryInfo(subDir);
                if((di.Attributes & ignoreAttributes) == 0)
                {
                    TreeNode subNode = new TreeNode(di.Name);
                    subNode.Tag = di.FullName;
                    subNode.ImageKey = FOLDER_IMAGE;
                    subNode.SelectedImageKey = FOLDER_IMAGE;
                    node.Nodes.Add(subNode);

                    if(pathArray != null && subNode.Level < pathArray.Length)
                    {
                        if(Utility.IsMatch(di.Name, pathArray[subNode.Level]))
                        {
                            if(pathArray.Length == subNode.Level + 1)
                            {
                                treeFileSystem.AfterSelect -= treeFileSystem_AfterSelect;
                                treeFileSystem.BeforeExpand -= treeFileSystem_BeforeExpand;
                                subNode.EnsureVisible();
                                treeFileSystem.SelectedNode = subNode;
                                treeFileSystem.BeforeExpand += treeFileSystem_BeforeExpand;
                                treeFileSystem.AfterSelect += treeFileSystem_AfterSelect;
                            }
                            else
                                GetSubNodes(subNode, pathArray);
                        }
                    }
                }
            }
        }

        private static string[] ParseDirectoryPath(string path)
        {
            path = path.Replace("/", "\\");
            string[] parts = path.Split(new char[] {'\\'}, StringSplitOptions.RemoveEmptyEntries);
            if(parts.Length > 0)
                parts[0] = parts[0] + "\\";
            return parts;
        }

        private Server CreateServer(string prefix, string vpath, string path, int port)
        {
            string[] defaultDocuments = {"default.aspx", "default.htm", "default.html", "index.htm", "index.html"};
            string[] restrictedDirs = {"/bin", "/app_browsers", "/app_code", "/app_data", "/app_localresources", "/app_globalresources", "/app_webreferences"};

            Server server = new Server(prefix, port, vpath, path, defaultDocuments, restrictedDirs);

            serverThread = new Thread(serverThread_Start);
            serverThread.Start(server);

            return server;
        }

        private void serverThread_Start(object data)
        {
            activeServer = (Server)data;
            try
            {
                activeServer.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
        }

        private void ClearWebBinDirectory()
        {
            DirectoryInfo di = new DirectoryInfo(webBinPath);
            FileSystemInfo[] files = di.GetFileSystemInfos();

            foreach(FileSystemInfo file in files)
                file.Delete();
        }

        private void CopyToWebBinDirectory()
        {
            string fileName = Assembly.GetExecutingAssembly().Location;
            if (!String.IsNullOrEmpty(fileName))
            {
                string executable = Path.GetFileName(fileName);
                string newLoc = webBinPath + executable;
                File.Copy(fileName, newLoc, true);
                while(!File.Exists(newLoc))
                {
                    //wait...
                }

                DirectoryInfo di = new DirectoryInfo(executionDirectory);
                FileSystemInfo[] files = di.GetFileSystemInfos("*.dll");

                foreach(FileSystemInfo file in files)
                {
                    File.Copy(file.FullName, webBinPath + file.Name, true);
                    while(!File.Exists(webBinPath + file.Name))
                    {
                        //wait...
                    }
                }
            }
        }

        private static StageNode MakeNode(string text, StageNodeType nodeType, int nodeId, DataRow row)
        {
            StageNode node = new StageNode(text, row);
            node.NodeType = nodeType;
            node.ImageKey = GetImageKey(nodeType);
            node.SelectedImageKey = GetImageKey(nodeType);
            node.RowId = nodeId;
            node.DatabaseName = text;
            node.SubSonicName = text;
            return node;
        }

        private static StageNode MakeNode(string text, string databaseName, string subSonicName, StageNodeType nodeType, string nodeKey, DataRow row)
        {
            StageNode node = new StageNode(text, row);
            node.NodeType = nodeType;
            node.ImageKey = GetImageKey(nodeType);
            node.SelectedImageKey = GetImageKey(nodeType);
            node.ItemKey = nodeKey;
            node.DatabaseName = databaseName;
            node.SubSonicName = subSonicName;
            return node;
        }

        private static string GetImageKey(StageNodeType nodeType)
        {
            switch(nodeType)
            {
                case StageNodeType.ColumnNode:
                    return COLUMN_IMAGE;
                case StageNodeType.ColumnPrimaryKeyNode:
                    return COLUMN_PK_IMAGE;
                case StageNodeType.Project:
                    return PROJECT_IMAGE;
                case StageNodeType.Provider:
                    return PROVIDER_IMAGE;
                case StageNodeType.ConnectionStringFolder:
                    return CONNECTION_STRING_FOLDER_IMAGE;
                case StageNodeType.ConnectionString:
                    return CONNECTION_STRING_IMAGE;
                case StageNodeType.TableFolder:
                    return TABLE_FOLDER_IMAGE;
                case StageNodeType.ViewFolder:
                    return VIEW_FOLDER_IMAGE;
                case StageNodeType.StoredProcedureFolder:
                    return SP_FOLDER_IMAGE;
                case StageNodeType.SubStageConfiguration:
                    return SUBSTAGE_CONFIGURATION_IMAGE;
                case StageNodeType.Table:
                    return TABLE_IMAGE;
                case StageNodeType.TableWithoutPrimaryKey:
                    return TABLE_NO_PK_IMAGE;
                case StageNodeType.TableExcluded:
                case StageNodeType.ViewExcluded:
                case StageNodeType.StoredProcedureExcluded:
                    return TABLE_EXCLUDED_IMAGE;
                case StageNodeType.View:
                    return VIEW_IMAGE;
                case StageNodeType.StoredProcedure:
                    return SP_IMAGE;
                case StageNodeType.StoredProcedureParameter:
                    return SP_PARAM_IMAGE;
                default:
                    return PROVIDER_IMAGE;
            }
        }

        private void BuildTree()
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();

            MasterStore.ProjectsRow[] projects = (MasterStore.ProjectsRow[])MM.Projects.Select(String.Empty, MM.Projects.NameColumn + " ASC");
            foreach(MasterStore.ProjectsRow project in projects)
            {
                StageNode rootNode = MakeNode(project.Name, StageNodeType.Project, project.ProjectId, project);
                treeView1.Nodes.Add(rootNode);
                foreach(MasterStore.ProvidersRow provider in project.Providers())
                {
                    StageNode providerNode = MakeNode(provider.Name, StageNodeType.Provider, provider.ProviderId, provider);
                    rootNode.Nodes.Add(providerNode);
                }
            }

            const string connStringText = "Connection Strings";
            StageNode connectionStringFolderNode = MakeNode(connStringText, connStringText, connStringText, StageNodeType.ConnectionStringFolder, null, null);
            treeView1.Nodes.Add(connectionStringFolderNode);

            MasterStore.ConnectionStringsRow[] connStrings =
                (MasterStore.ConnectionStringsRow[])MM.ConnectionStrings.Select(String.Empty, MM.ConnectionStrings.NameColumn.ColumnName + " ASC");
            foreach(MasterStore.ConnectionStringsRow connectionString in connStrings)
            {
                StageNode connectionStringNode = MakeNode(connectionString.Name, StageNodeType.ConnectionString, connectionString.ConnectionStringId, connectionString);
                connectionStringFolderNode.Nodes.Add(connectionStringNode);
            }

            const string configText = "SubStage Configuration";
            StageNode configurationNode = MakeNode(configText, StageNodeType.SubStageConfiguration, config.SubStageConfigurationId, config);
            treeView1.Nodes.Add(configurationNode);

            treeView1.EndUpdate();
            //MM.Projects
        }

        private static CustomProperty CreateCustomProperty(DataRow row, DataColumn dc, bool readOnly, bool visible, string category)
        {
            CustomProperty cp = new CustomProperty();
            cp.Name = dc.ColumnName;
            cp.Tag = dc.ColumnName + "|" + row[0];
            cp.IsReadOnly = readOnly;
            cp.Visible = visible;
            cp.Value = row[dc];
            cp.Description = dc.Caption;
            cp.Category = category;
            cp.DefaultValue = dc.DefaultValue;
            return cp;
        }

        private static CustomProperty CreateCustomProperty(string propertyName, string propertyValue, string description)
        {
            CustomProperty cp = new CustomProperty();
            cp.Name = propertyName;
            cp.Value = propertyValue ?? String.Empty;
            cp.IsReadOnly = true;
            cp.IsBrowsable = false;
            cp.Visible = true;
            cp.Description = description;
            return cp;
        }

        private void TogglePropertyGridButtons(StageNode node)
        {
            pgbTestConnection.Visible = node.NodeType == StageNodeType.Provider;
        }

        private void ToggleContextMenuItems(StageNode node)
        {
            cmiTreeDeleteProject.Enabled = node.IsProjectNode;
            cmiTreeAddProvider.Enabled = node.IsProjectNode;
            cmiTreeDeleteProvider.Enabled = node.IsProviderNode;
            cmiTreeAddConnectionString.Enabled = node.IsConnectionStringFolderNode;
            cmiTreeDeleteConnectionString.Enabled = node.IsConnectionStringNode;
            if(node.IsTableNode || node.IsViewNode)
            {
                cmiGenerateObjectEnabled.Visible = true;
                //cmiGenerateObjectEnabled.Enabled = true;
                SetGenerateButtonText(node);
            }
            else
            {
                cmiGenerateObjectEnabled.Visible = false;
                //cmiGenerateObjectEnabled.Enabled = false;
            }
        }

        /// <summary>
        /// Sets the generate button text.
        /// </summary>
        /// <param name="node">The node.</param>
        private void SetGenerateButtonText(StageNode node)
        {
            switch(node.NodeType)
            {
                case StageNodeType.StoredProcedure: //Nothing for now - where not going to get to here - TODO: make this work
                case StageNodeType.StoredProcedureExcluded: //where not going to get to here (never shows up)
                    break;
                case StageNodeType.Table:
                case StageNodeType.TableWithoutPrimaryKey:
                    cmiGenerateObjectEnabled.Text = "Disable Generate Table";
                    break;
                case StageNodeType.TableExcluded:
                    cmiGenerateObjectEnabled.Text = "Enable Generate Table";
                    break;
                case StageNodeType.View:
                    cmiGenerateObjectEnabled.Text = "Disable Generate View";
                    break;
                case StageNodeType.ViewExcluded:
                    cmiGenerateObjectEnabled.Text = "Enable Generate View";
                    break;
                default:
                    cmiGenerateObjectEnabled.Text = "Generate Object?";
                    break;
            }
        }

        private void ToggleToolbarItems(StageNode node)
        {
            btnNewProvider.Enabled = node.IsProjectNode;
            btnAddConnectionString.Enabled = node.IsConnectionStringFolderNode;
            btnInvokeProviders.Enabled = (node.IsProjectNode || node.IsProviderNode);
            btnSplitGenerateCode.Enabled = (node.IsProjectNode || node.IsProviderNode);
        }

        private static void AddConfigProperty(StringBuilder sb, string propertyName, DataRow row, DataColumn dc, bool buildIfDefault)
        {
            if(row[dc] != null & !String.IsNullOrEmpty(row[dc].ToString()))
            {
                string propValue = row[dc].ToString();
                string compareValue = dc.DefaultValue.ToString();
                if(dc.DataType == typeof(bool))
                {
                    propValue = propValue.ToLower();
                    compareValue = compareValue.ToLower();
                }

                if(propValue == compareValue && !buildIfDefault)
                    return;

                if(sb.Length > 0)
                    sb.Append(SpecialString.SPACE);

                sb.Append(propertyName);
                sb.Append("=\"");
                sb.Append(propValue);
                sb.Append("\"");
            }
        }

        private static void AddConfigProperty(StringBuilder sb, string property, string value)
        {
            if(!String.IsNullOrEmpty(value))
            {
                if(sb.Length > 0)
                    sb.Append(SpecialString.SPACE);
                sb.Append(property);
                sb.Append("=\"");
                sb.Append(value);
                sb.Append("\"");
            }
        }

        private static bool IsDefaultValue(DataRow row, DataColumn dc)
        {
            if(row[dc] != null & !String.IsNullOrEmpty(row[dc].ToString()))
            {
                string propValue = row[dc].ToString();
                string compareValue = dc.DefaultValue.ToString();
                if(dc.DataType == typeof(bool))
                {
                    propValue = propValue.ToLower();
                    compareValue = compareValue.ToLower();
                }

                return propValue == compareValue;
            }
            return false;
        }

        private void BuildConfig(MasterStore.ProjectsRow project)
        {
            //bool buildIfDefault = false;
            if(project != null)
            {
                string newConfig = configTemplate;

                StringBuilder sbConnections = new StringBuilder();
                List<int> addedConnections = new List<int>();
                foreach(MasterStore.ProvidersRow provider in project.Providers())
                {
                    if(!provider.IsConnectionStringIdNull() && !addedConnections.Contains(provider.ConnectionStringId))
                    {
                        sbConnections.AppendLine("\t\t<add name=\"" + provider.ConnectionString.Name + "\" connectionString=\"" + provider.ConnectionString.ConnectionString +
                                                 "\"/>");
                        addedConnections.Add(provider.ConnectionStringId);
                    }
                }
                newConfig = newConfig.Replace(TEMPLATE_CONNECTION_STRINGS, sbConnections.ToString());

                if(!project.IsDefaultProviderNull())
                    newConfig = newConfig.Replace(TEMPLATE_DEFAULT_PROVIDER, project.DefaultProviderEntry.Name);
                newConfig = newConfig.Replace(TEMPLATE_TEMPLATE_DIRECTORY, project.TemplateDirectory);
                newConfig = newConfig.Replace(TEMPLATE_ENABLE_TRACE, project.EnableTrace.ToString().ToLower());

                StringBuilder sbService = new StringBuilder();
                foreach(MasterStore.ProvidersRow provider in project.Providers())
                    sbService.Append(GetProviderConfigText(provider));

                newConfig = newConfig.Replace(TEMPLATE_PROVIDERS, sbService.ToString());
                tbxConfigOutput.Text = newConfig;

                ActivateConfig();
                InstantiateProvider(project);
            }
        }

        private void ActivateConfig()
        {
            if(File.Exists(webConfigPath))
                File.Delete(webConfigPath);
            while(File.Exists(webConfigPath))
            {
                //wait...
            }

            File.WriteAllText(webConfigPath, tbxConfigOutput.Text);
            while(!File.Exists(webConfigPath))
            {
                //wait...
            }
        }

        private void PopulateProviderData(StageNode node)
        {
            MasterStore.ProjectsRow projectRow = node.Project;
            InstantiateProvider(projectRow);
            treeView1.BeginUpdate();
            if(node.IsProjectNode)
            {
                foreach(MasterStore.ProvidersRow provider in projectRow.Providers())
                    PopulateProviderNodeData(provider);
            }
            else if(node.IsProviderNode)
            {
                MasterStore.ProvidersRow provider = node.Provider;
                if(provider != null)
                    PopulateProviderNodeData(provider);
            }
            treeView1.EndUpdate();
        }

        private void GenerateCode(StageNode node)
        {
            MasterStore.ProjectsRow projectRow = node.Project;
            InstantiateProvider(projectRow);
            tabDetail.SelectedTab = tabDetailLog;
            tabDetailLog.Focus();
            string navigateTo = GenerateProviderCode(node);
            tabDetail.SelectedTab = tabDetailFileBrowser;
            tabDetailFileBrowser.Focus();
            BuildDirectoryTree(navigateTo);
            NavigateFileBrowser();
        }

        private void PopulateProviderNodeData(MasterStore.ProvidersRow provider)
        {
            StageNode findNode = FindNodeByRow(provider);
            if(findNode != null)
            {
                findNode.Nodes.Clear();

                if(DataService.Providers[provider.Name] != null)
                {
                    DataService.Providers[provider.Name].ReloadSchema();
                    TableSchema.Table[] tables = DataService.GetTables(provider.Name);
                    if(tables.Length > 0)
                    {
                        StageNode tableFolderNode = MakeNode(TABLES, TABLES, TABLES, StageNodeType.TableFolder, null, provider);
                        findNode.Nodes.Add(tableFolderNode);

                        foreach(TableSchema.Table table in tables)
                        {
                            string displayName = miUseGeneratedNames.Checked ? table.ClassName : table.Name;
                            StageNodeType nodeType = table.HasPrimaryKey ? StageNodeType.Table : StageNodeType.TableWithoutPrimaryKey;
                            nodeType = CodeService.ShouldGenerate(table.Name, DataService.Providers[provider.Name].Name) ? nodeType : StageNodeType.TableExcluded;
                            StageNode tableNode = MakeNode(displayName, table.Name, table.ClassName, nodeType, table.Name, provider);
                            if(!table.HasPrimaryKey)
                            {
                                tableNode.ForeColor = Color.Red;
                                tableNode.ToolTipText = "There is no Primary Key on this table.";
                            }
                            tableFolderNode.Nodes.Add(tableNode);

                            TableSchema.TableColumnCollection columns = table.Columns;
                            foreach(TableSchema.TableColumn column in columns)
                            {
                                string displayColumnName = miUseGeneratedNames.Checked ? column.PropertyName : column.ColumnName;
                                StageNodeType columnNodeType = column.IsPrimaryKey ? StageNodeType.ColumnPrimaryKeyNode : StageNodeType.ColumnNode;
                                StageNode columnNode = MakeNode(displayColumnName, column.ColumnName, column.PropertyName, columnNodeType, column.ColumnName, provider);
                                tableNode.Nodes.Add(columnNode);
                            }
                        }
                    }

                    TableSchema.Table[] views = DataService.GetViews(provider.Name);
                    if(views.Length > 0)
                    {
                        StageNode viewFolderNode = MakeNode(VIEWS, VIEWS, VIEWS, StageNodeType.ViewFolder, null, provider);
                        findNode.Nodes.Add(viewFolderNode);
                        foreach(TableSchema.Table view in views)
                        {
                            StageNodeType nodeType = StageNodeType.View;
                            nodeType = CodeService.ShouldGenerate(view.Name, DataService.Providers[provider.Name].Name) ? nodeType : StageNodeType.ViewExcluded;
                            string displayName = miUseGeneratedNames.Checked ? view.ClassName : view.Name;
                            StageNode viewNode = MakeNode(displayName, view.Name, view.ClassName, nodeType, view.Name, provider);
                            viewFolderNode.Nodes.Add(viewNode);

                            TableSchema.TableColumnCollection columns = view.Columns;
                            foreach(TableSchema.TableColumn column in columns)
                            {
                                string displayColumnName = miUseGeneratedNames.Checked ? column.PropertyName : column.ColumnName;
                                StageNodeType columnNodeType = column.IsPrimaryKey ? StageNodeType.ColumnPrimaryKeyNode : StageNodeType.ColumnNode;
                                StageNode columnNode = MakeNode(displayColumnName, column.ColumnName, column.PropertyName, columnNodeType, column.ColumnName, provider);
                                viewNode.Nodes.Add(columnNode);
                            }
                        }
                    }

                    List<StoredProcedure> storedProcedures = DataService.GetSPSchemaCollection(provider.Name);
                    if(storedProcedures.Count > 0)
                    {
                        StageNode spFolderNode = MakeNode(STORED_PROCEDURES, STORED_PROCEDURES, STORED_PROCEDURES, StageNodeType.StoredProcedureFolder, null, provider);
                        findNode.Nodes.Add(spFolderNode);
                        foreach(StoredProcedure storedProcedure in storedProcedures)
                        {
                            string displayName = miUseGeneratedNames.Checked ? storedProcedure.DisplayName : storedProcedure.Name;
                            DataProvider thisProvider = DataService.Providers[provider.Name];
                            StageNodeType nodeType = CodeService.ShouldGenerate(storedProcedure.Name, thisProvider.IncludeProcedures, thisProvider.ExcludeProcedures, thisProvider)
                                                         ? StageNodeType.StoredProcedure
                                                         : StageNodeType.StoredProcedureExcluded;
                            StageNode spNode = MakeNode(displayName, storedProcedure.Name, storedProcedure.DisplayName, nodeType, storedProcedure.Name, provider);
                            spFolderNode.Nodes.Add(spNode);

                            StoredProcedure.ParameterCollection parameters = storedProcedure.Parameters;
                            foreach(StoredProcedure.Parameter parameter in parameters)
                            {
                                string paramDisplayName = miUseGeneratedNames.Checked ? parameter.DisplayName : parameter.Name;
                                StageNode spParamNode = MakeNode(paramDisplayName, parameter.Name, parameter.DisplayName, StageNodeType.StoredProcedureParameter, parameter.Name,
                                    provider);
                                spNode.Nodes.Add(spParamNode);
                            }
                        }
                    }
                }
            }
        }

        private static void AddToProviderConfig(NameValueCollection config, string key, DataRow row, DataColumn dc)
        {
            if(row != null)
            {
                if(!IsDefaultValue(row, dc))
                {
                    string propValue = row[dc].ToString();
                    if(dc.DataType == typeof(bool))
                        propValue = propValue.ToLower();
                    config.Add(key, propValue);
                }
            }
        }

        private void ShowStatus(string statusText)
        {
            tbxLog.AppendText(String.Concat(String.Format("{0:T}", DateTime.Now), " - ", statusText, "\r\n"));
            tsStatus.Text = statusText;
            Application.DoEvents();
        }

        private static void InitializeProviderCollection()
        {
            DataService.Provider = new SqlDataProvider();
            DataService.Providers = new DataProviderCollection();
        }

        private static StageNode GetCurrentProviderNode(StageNode selectedNode, ProviderBase provider)
        {
            if(selectedNode.IsProjectNode && selectedNode.Nodes.Count > 0)
            {
                foreach(StageNode childNode in selectedNode.Nodes)
                {
                    if(childNode.Provider != null)
                    {
                        if(Utility.IsMatch(childNode.Provider.Name, provider.Name))
                            return childNode;
                    }
                }
            }
            return selectedNode;
        }

        private string GetNodeGenerationPath(StageNode node)
        {
            const string backSlash = "\\";
            string outDir = config.CodeGenerationPath;
            if(node.Provider != null)
            {
                if(!String.IsNullOrEmpty(node.Provider.CodeGenerationPath))
                    outDir = node.Provider.CodeGenerationPath;
                else if(!String.IsNullOrEmpty(node.Project.CodeGenerationPath))
                    outDir = node.Project.CodeGenerationPath;

                
                if(!outDir.EndsWith(backSlash))
                outDir = String.Concat(outDir, backSlash);
                
                if (node.Provider.OrganizeCodeByProvider)
                    outDir = String.Concat(outDir, node.Provider.Name, backSlash);
                
            }
            if (!outDir.EndsWith(backSlash))
                outDir = String.Concat(outDir, backSlash);
            return outDir;
        }

        private string GenerateProviderCode(StageNode selectedNode)
        {
            TurboCompiler turboCompiler = new TurboCompiler();

            ICodeLanguage language;
            if(miGenerateVB.Checked)
                language = new VBCodeLanguage();
            else
                language = new CSharpCodeLanguage();

            //string outDir = codeDestination;

            DataProviderCollection generationProviders = new DataProviderCollection();
            string navigatePath = config.CodeGenerationPath;
            if (selectedNode.IsProjectNode)
            {
                generationProviders = DataService.Providers;
                if (selectedNode.Nodes.Count > 1)
                    navigatePath = GetNodeGenerationPath(selectedNode);
            }
            else if (selectedNode.IsProviderNode)
            {
                generationProviders.Add(DataService.Providers[selectedNode.Provider.Name]);
                navigatePath = GetNodeGenerationPath(selectedNode);
            }

            foreach(DataProvider provider in generationProviders)
            {
                StageNode currentNode = GetCurrentProviderNode(selectedNode, provider);
                if(currentNode != null)
                {
                    string providerDir = GetNodeGenerationPath(currentNode);
                    if(!Directory.Exists(providerDir))
                        Directory.CreateDirectory(providerDir);

                    string[] tables = DataService.GetTableNames(provider.Name);

                    foreach(string tbl in tables)
                    {
                        if(CodeService.ShouldGenerate(tbl, provider.Name))
                        {
                            ShowStatus(String.Format("Generating {0}...", tbl));
                            string className = DataService.GetSchema(tbl, provider.Name, TableType.Table).ClassName;
                            TurboTemplate tt = CodeService.BuildClassTemplate(tbl, language, provider);
                            tt.OutputPath = Path.Combine(providerDir, className + language.FileExtension);
                            turboCompiler.AddTemplate(tt);

                            if(provider.GenerateODSControllers && provider.TableBaseClass != "RepositoryRecord")
                            {
                                TurboTemplate ttODS = CodeService.BuildODSTemplate(tbl, language, provider);
                                ttODS.OutputPath = Path.Combine(providerDir, className + "Controller" + language.FileExtension);
                                turboCompiler.AddTemplate(ttODS);
                            }
                        }
                    }

                    string[] views = DataService.GetViewNames(provider.Name);
                    foreach(string tbl in views)
                    {
                        if(CodeService.ShouldGenerate(tbl, provider.Name))
                        {
                            ShowStatus(String.Format("Generating {0}...", tbl));
                            string className = DataService.GetSchema(tbl, provider.Name, TableType.View).ClassName;
                            TurboTemplate tt = CodeService.BuildViewTemplate(tbl, language, provider);
                            tt.OutputPath = Path.Combine(providerDir, className + language.FileExtension);
                            turboCompiler.AddTemplate(tt);
                        }
                    }

                    if(provider.UseSPs)
                    {
                        ShowStatus("Generating Stored Procedures...");
                        string outPath = Path.Combine(providerDir, provider.SPClassName + language.FileExtension);
                        TurboTemplate tt = CodeService.BuildSPTemplate(language, provider);
                        tt.OutputPath = outPath;
                        turboCompiler.AddTemplate(tt);
                    }

                    ShowStatus("Generating Structs...");
                    string structPath = Path.Combine(providerDir, "AllStructs" + language.FileExtension);
                    TurboTemplate ttStructs = CodeService.BuildStructsTemplate(language, DataService.Provider);
                    ttStructs.OutputPath = structPath;
                    turboCompiler.AddTemplate(ttStructs);

                    if(miScriptSchemas.Checked && provider.NamedProviderType == DataProviderTypeName.SQL_SERVER)
                    {
                        ShowStatus("Scripting Schema...");
                        string schema = ScriptSchema(provider.DefaultConnectionString);
                        string outSchemaFileName =
                            string.Format("{0}_{1}_{2}_{3}_{4}_Schema.sql", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Environment.UserName, provider.Name);
                        Files.CreateToFile(Path.Combine(providerDir, outSchemaFileName), schema);
                    }

                    if(miScriptData.Checked)
                    {
                        string outFileName = string.Format("{0}_Data_{1}_{2}_{3}.sql", provider.Name, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                        string outPath = Path.Combine(providerDir, outFileName);

                        using(StreamWriter sw = File.CreateText(outPath))
                        {
                            foreach(string tbl in tables)
                            {
                                if(CodeService.ShouldGenerate(tbl, provider.Name))
                                {
                                    Utility.WriteTrace(String.Format("Scripting Table: {0}", tbl));
                                    string dataScript = DataService.ScriptData(tbl, provider.Name);
                                    sw.Write(dataScript);
                                    sw.Write(Environment.NewLine);
                                }
                            }
                        }
                    }
                }
            }

            turboCompiler.Run();
            foreach(TurboTemplate template in turboCompiler.Templates)
            {
                ShowStatus(String.Concat("Writing ", template.TemplateName, " as ", template.OutputPath.Substring(template.OutputPath.LastIndexOf("\\") + 1)));
                Files.CreateToFile(template.OutputPath, template.FinalCode);
            }

            ShowStatus("Finished.");
            Application.DoEvents();
            return navigatePath;
        }

        /// <summary>
        /// Scripts the schema.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static string ScriptSchema(string connectionString)
        {
            StringBuilder result = new StringBuilder();

            SqlConnection conn = new SqlConnection(connectionString);
            SqlConnectionStringBuilder cString = new SqlConnectionStringBuilder(connectionString);
            ServerConnection sconn = new ServerConnection(conn);
            Microsoft.SqlServer.Management.Smo.Server server = new Microsoft.SqlServer.Management.Smo.Server(sconn);
            Database db = server.Databases[cString.InitialCatalog];
            Transfer trans = new Transfer(db);

            //set the objects to copy
            trans.CopyAllTables = true;
            trans.CopyAllDefaults = true;
            trans.CopyAllUserDefinedFunctions = true;
            trans.CopyAllStoredProcedures = true;
            trans.CopyAllViews = true;
            trans.CopyData = true;
            trans.CopySchema = true;
            trans.DropDestinationObjectsFirst = true;
            trans.UseDestinationTransaction = true;

            trans.Options.AnsiFile = true;
            trans.Options.ClusteredIndexes = true;
            trans.Options.DriAll = true;
            trans.Options.IncludeHeaders = true;
            trans.Options.IncludeIfNotExists = true;
            trans.Options.SchemaQualify = true;

            StringCollection script = trans.ScriptTransfer();

            foreach(string s in script)
                result.AppendLine(s);

            result.AppendLine();
            result.AppendLine();

            return result.ToString();
        }

        private static void InstantiateProvider(DataRow configSource)
        {
            bool isProjectSource = configSource is MasterStore.ProjectsRow;
            //bool isProviderSource = configSource is DM.ProvidersRow;

            MasterStore.ProjectsRow activeProject;
            MasterStore.ProvidersRow[] activeProviders;
            if(isProjectSource)
            {
                activeProject = (MasterStore.ProjectsRow)configSource;
                activeProviders = activeProject.Providers();
            }
            else
            {
                activeProject = ((MasterStore.ProvidersRow)configSource).Project;
                activeProviders = new MasterStore.ProvidersRow[] {(MasterStore.ProvidersRow)configSource};
            }

            SubSonicSection section = new SubSonicSection();

            if(activeProject.DefaultProviderEntry != null)
                section.DefaultProvider = activeProject.DefaultProviderEntry.Name;
            else if(activeProject.Providers().Length > 0)
            {
                activeProject.DefaultProvider = activeProject.Providers()[0].ProviderId;
                MM.Save();
            }
            else
            {
                activeProject.SetDefaultProviderNull();
                MM.Save();
                return;
            }

            section.TemplateDirectory = activeProject.TemplateDirectory;

            CodeService.TemplateDirectory = section.TemplateDirectory;

            InitializeProviderCollection();
            foreach(MasterStore.ProvidersRow providerEntry in activeProviders)
            {
                NameValueCollection config = new NameValueCollection();

                AddToProviderConfig(config, ConfigurationPropertyName.CONNECTION_STRING_NAME, providerEntry.ConnectionString, MM.ConnectionStrings.NameColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.APPEND_WITH, providerEntry, MM.Providers.AppendWithColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.ADDITIONAL_NAMESPACES, providerEntry, MM.Providers.AdditionalNamespacesColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.EXCLUDE_PROCEDURE_LIST, providerEntry, MM.Providers.ExcludeProcedureListColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.EXCLUDE_TABLE_LIST, providerEntry, MM.Providers.ExcludeTableListColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.EXTRACT_CLASS_NAME_FROM_SP_NAME, providerEntry, MM.Providers.ExtractClassNameFromSPNameColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.FIX_DATABASE_OBJECT_CASING, providerEntry, MM.Providers.FixDatabaseObjectCasingColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.FIX_PLURAL_CLASS_NAMES, providerEntry, MM.Providers.FixPluralClassNamesColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.GENERATED_NAMESPACE, providerEntry, MM.Providers.GeneratedNamespaceColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.GENERATE_LAZY_LOADS, providerEntry, MM.Providers.GenerateLazyLoadsColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.GENERATE_NULLABLE_PROPERTIES, providerEntry, MM.Providers.GenerateNullablePropertiesColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.GENERATE_ODS_CONTROLLERS, providerEntry, MM.Providers.GenerateODSControllersColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.GENERATE_RELATED_TABLES_AS_PROPERTIES, providerEntry, MM.Providers.GenerateRelatedTablesAsPropertiesColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.INCLUDE_PROCEDURE_LIST, providerEntry, MM.Providers.IncludeProcedureListColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.INCLUDE_TABLE_LIST, providerEntry, MM.Providers.IncludeTableListColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.REGEX_DICTIONARY_REPLACE, providerEntry, MM.Providers.RegexDictionaryReplaceColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.REGEX_IGNORE_CASE, providerEntry, MM.Providers.RegexIgnoreCaseColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.REGEX_MATCH_EXPRESSION, providerEntry, MM.Providers.RegexMatchExpressionColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.REGEX_REPLACE_EXPRESSION, providerEntry, MM.Providers.RegexReplaceExpressionColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.RELATED_TABLE_LOAD_PREFIX, providerEntry, MM.Providers.RelatedTableLoadPrefixColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.REMOVE_UNDERSCORES, providerEntry, MM.Providers.RemoveUnderscoresColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.SET_PROPERTY_DEFAULTS_FROM_DATABASE, providerEntry, MM.Providers.SetPropertyDefaultsFromDatabaseColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.SP_STARTS_WITH, providerEntry, MM.Providers.SPStartsWithColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.STORED_PROCEDURE_BASE_CLASS, providerEntry, MM.Providers.StoredProcedureBaseClassColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.STORED_PROCEDURE_CLASS_NAME, providerEntry, MM.Providers.SPClassNameColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.STRIP_COLUMN_TEXT, providerEntry, MM.Providers.StripColumnTextColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.STRIP_PARAM_TEXT, providerEntry, MM.Providers.StripParamTextColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.STRIP_STORED_PROCEDURE_TEXT, providerEntry, MM.Providers.StripSPTextColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.STRIP_TABLE_TEXT, providerEntry, MM.Providers.StripTableTextColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.STRIP_VIEW_TEXT, providerEntry, MM.Providers.StripViewTextColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.TABLE_BASE_CLASS, providerEntry, MM.Providers.TableBaseClassColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.USE_EXTENDED_PROPERTIES, providerEntry, MM.Providers.UseExtendedPropertiesColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.USE_STORED_PROCEDURES, providerEntry, MM.Providers.UseSPsColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.USE_UTC_TIMES, providerEntry, MM.Providers.UseUTCColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.VIEW_BASE_CLASS, providerEntry, MM.Providers.ViewBaseClassColumn);
                AddToProviderConfig(config, ConfigurationPropertyName.VIEW_STARTS_WITH, providerEntry, MM.Providers.ViewStartsWithColumn);

                if(!providerEntry.IsConnectionStringIdNull())
                {
                    string connString = providerEntry.ConnectionString.ConnectionString;
                    string location = providerEntry.Project.OriginalLocation;
                    location = location.EndsWith(@"\") ? location : location + @"\";
                    location += @"App_Data\";

                    bool loadProvider = true;
                    string modConn = Regex.Replace(connString, @"\|DataDirectory\|", location, RegexOptions.IgnoreCase);
                    if(!Utility.IsMatch(modConn, connString))
                    {
                        Match dbFileName = Regex.Match(connString, @"(?<=\|DataDirectory\|).*\.mdf", RegexOptions.IgnoreCase);
                        if(dbFileName != null)
                        {
                            string fileLocation = location + dbFileName.Value;
                            if(!File.Exists(fileLocation))
                                loadProvider = false;
                        }
                    }

                    if(loadProvider)
                    {
                        Assembly asm = Assembly.Load("SubSonic");
                        Type type = asm.GetType("SubSonic." + providerEntry.ProviderType.InternalName);
                        if(type != null)
                        {
                            DataProvider provider = (DataProvider)Activator.CreateInstance(type, false);
                            provider.Initialize(providerEntry.Name, config);
                            provider.DefaultConnectionString = modConn;
                            DataService.Providers.Add(provider);
                        }
                    }
                }
            }
            Application.DoEvents();
        }

        private static string GetProviderConfigText(MasterStore.ProvidersRow provider)
        {
            const bool buildIfDefault = false;
            StringBuilder sbService = new StringBuilder();

            sbService.Append("\t\t\t<add name=\"" + provider.Name + "\" type=\"SubSonic." + provider.ProviderType.InternalName + ", SubSonic\"");
            if(!provider.IsConnectionStringIdNull())
                AddConfigProperty(sbService, ConfigurationPropertyName.CONNECTION_STRING_NAME, provider.ConnectionString.Name);
            AddConfigProperty(sbService, ConfigurationPropertyName.APPEND_WITH, provider, MM.Providers.AppendWithColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.ADDITIONAL_NAMESPACES, provider, MM.Providers.AdditionalNamespacesColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.EXCLUDE_PROCEDURE_LIST, provider, MM.Providers.ExcludeProcedureListColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.EXCLUDE_TABLE_LIST, provider, MM.Providers.ExcludeTableListColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.EXTRACT_CLASS_NAME_FROM_SP_NAME, provider, MM.Providers.ExtractClassNameFromSPNameColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.FIX_DATABASE_OBJECT_CASING, provider, MM.Providers.FixDatabaseObjectCasingColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.FIX_PLURAL_CLASS_NAMES, provider, MM.Providers.FixPluralClassNamesColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.GENERATED_NAMESPACE, provider, MM.Providers.GeneratedNamespaceColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.GENERATE_LAZY_LOADS, provider, MM.Providers.GenerateLazyLoadsColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.GENERATE_NULLABLE_PROPERTIES, provider, MM.Providers.GenerateNullablePropertiesColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.GENERATE_ODS_CONTROLLERS, provider, MM.Providers.GenerateODSControllersColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.GENERATE_RELATED_TABLES_AS_PROPERTIES, provider, MM.Providers.GenerateRelatedTablesAsPropertiesColumn,
                buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.INCLUDE_PROCEDURE_LIST, provider, MM.Providers.IncludeProcedureListColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.INCLUDE_TABLE_LIST, provider, MM.Providers.IncludeTableListColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.REGEX_DICTIONARY_REPLACE, provider, MM.Providers.RegexDictionaryReplaceColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.REGEX_IGNORE_CASE, provider, MM.Providers.RegexIgnoreCaseColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.REGEX_MATCH_EXPRESSION, provider, MM.Providers.RegexMatchExpressionColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.REGEX_REPLACE_EXPRESSION, provider, MM.Providers.RegexReplaceExpressionColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.RELATED_TABLE_LOAD_PREFIX, provider, MM.Providers.RelatedTableLoadPrefixColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.REMOVE_UNDERSCORES, provider, MM.Providers.RemoveUnderscoresColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.SET_PROPERTY_DEFAULTS_FROM_DATABASE, provider, MM.Providers.SetPropertyDefaultsFromDatabaseColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.SP_STARTS_WITH, provider, MM.Providers.SPStartsWithColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.STORED_PROCEDURE_CLASS_NAME, provider, MM.Providers.StoredProcedureBaseClassColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.STORED_PROCEDURE_CLASS_NAME, provider, MM.Providers.SPClassNameColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_COLUMN_TEXT, provider, MM.Providers.StripColumnTextColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_PARAM_TEXT, provider, MM.Providers.StripParamTextColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_STORED_PROCEDURE_TEXT, provider, MM.Providers.StripSPTextColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_TABLE_TEXT, provider, MM.Providers.StripTableTextColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_VIEW_TEXT, provider, MM.Providers.StripViewTextColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.TABLE_BASE_CLASS, provider, MM.Providers.TableBaseClassColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.USE_EXTENDED_PROPERTIES, provider, MM.Providers.UseExtendedPropertiesColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.USE_STORED_PROCEDURES, provider, MM.Providers.UseSPsColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.USE_UTC_TIMES, provider, MM.Providers.UseUTCColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.VIEW_BASE_CLASS, provider, MM.Providers.ViewBaseClassColumn, buildIfDefault);
            AddConfigProperty(sbService, ConfigurationPropertyName.VIEW_STARTS_WITH, provider, MM.Providers.ViewStartsWithColumn, buildIfDefault);
            sbService.AppendLine("/>");

            return sbService.ToString();
        }

        private static object GetConfig(ProviderSettings settings, string key, DataColumn dc)
        {
            if(settings.Parameters[key] != null)
                return settings.Parameters[key];
            return dc != null ? dc.DefaultValue : String.Empty;
        }

        private void LoadScaffold(StageNode node)
        {
            if(node != null && tabMaster.SelectedTab == tabMasterScaffolds && node.Project != null)
            {
                string outputText = null;
                if(node.IsTableNode)
                {
                    outputText = scaffoldTemplate.Replace(TEMPLATE_SCAFFOLD_PROPERTIES,
                        "ScaffoldType=\"Normal\" TableName=\"" + node.DatabaseName + "\" ProviderName=\"" + node.Provider.Name + "\"");
                }
                else if(node.IsProjectNode)
                    outputText = scaffoldTemplate.Replace(TEMPLATE_SCAFFOLD_PROPERTIES, "ScaffoldType=\"Auto\"");

                if(!String.IsNullOrEmpty(outputText))
                {
                    if(File.Exists(scaffoldFilePath))
                        File.Delete(scaffoldFilePath);
                    File.WriteAllText(scaffoldFilePath, outputText);
                    while(!File.Exists(scaffoldFilePath))
                    {
                        //wait
                    }
                    webScaffolds.Navigate(scaffoldUrlPath + "?nocache=" + Guid.NewGuid());
                }
                else
                    webScaffolds.Navigate(scaffoldInfoUrlPath);
            }
        }

        private void tabMaster_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadScaffold((StageNode)treeView1.SelectedNode);
        }

        private void SubStageForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Some times the app doesn't really close it just closes the form so lets make sure it closes.
            Application.Exit();
        }

        private void NavigateFileBrowser(string path)
        {
            if(fileBrowser.AllowNavigation)
            {
                fileBrowser.AllowNavigation = false;
                fileBrowser.SuspendLayout();
                fileBrowser.Navigate(path);
                fileBrowser.ResumeLayout();
                fileBrowser.AllowNavigation = true;
            }
        }

        private void NavigateFileBrowser()
        {
            if(treeFileSystem.SelectedNode != null)
                NavigateFileBrowser(FILE_URL_PREFIX + treeFileSystem.SelectedNode.Tag);
        }

        private void treeFileSystem_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            navigatingFromFileTree = true;
            treeFileSystem.BeginUpdate();
            foreach(TreeNode subNode in e.Node.Nodes)
                GetSubNodes(subNode, null);
            treeFileSystem.EndUpdate();
        }

        private void treeFileSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            navigatingFromFileTree = true;
            treeFileSystem.BeginUpdate();
            GetSubNodes(e.Node, null);
            foreach(TreeNode subNode in e.Node.Nodes)
                GetSubNodes(subNode, null);
            treeFileSystem.EndUpdate();
            NavigateFileBrowser(FILE_URL_PREFIX + e.Node.Tag);
        }

        private void fileBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if(!navigatingFromFileTree)
                BuildDirectoryTree(e.Url.AbsolutePath.Replace(FILE_URL_PREFIX, String.Empty));
            navigatingFromFileTree = false;
        }

        private void fileBrowser_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            //BuildDirectoryTree(e.Url.AbsolutePath.Replace(FILE_URL_PREFIX, String.Empty));
        }


        #region Node Operations

        private StageNode FindNodeByRow(DataRow row)
        {
            return FindNodeByRow(row, treeView1.Nodes);
        }

        private static StageNode FindNodeByRow(DataRow row, TreeNodeCollection searchNodes)
        {
            StageNode foundNode = null;
            foreach(StageNode node in searchNodes)
            {
                foundNode = IsNodeMatch(row, node) ? node : FindNodeByRow(row, node.Nodes);

                if(foundNode != null)
                    break;
            }
            return foundNode;
        }

        private static bool IsNodeMatch(DataRow row, StageNode node)
        {
            if(node.IsProjectNode && row is MasterStore.ProjectsRow && ((MasterStore.ProjectsRow)row).ProjectId == node.Project.ProjectId)
                return true;
            if(node.IsProviderNode && row is MasterStore.ProvidersRow && ((MasterStore.ProvidersRow)row).ProviderId == node.Provider.ProviderId)
                return true;
            if(node.IsConnectionStringNode && row is MasterStore.ConnectionStringsRow &&
               ((MasterStore.ConnectionStringsRow)row).ConnectionStringId == node.ConnectionString.ConnectionStringId)
                return true;

            return false;
        }

        private void UpdateNodeName(DataRow row, string newName)
        {
            StageNode findNode = FindNodeByRow(row);
            if(findNode != null)
                findNode.Text = newName;
        }

        private void SelectNode(StageNode node)
        {
            //ShowStatus("Ready.");
            if(node.IsProjectNode)
            {
                LoadProjectProperties(node);
                BuildConfig(node.Project);
                string path = GetNodeGenerationPath(node);
                BuildDirectoryTree(path);
                NavigateFileBrowser(path);
            }
            else if(node.IsProviderNode)
            {
                LoadProviderProperties(node);
                BuildConfig(node.Project);
                string path = GetNodeGenerationPath(node);
                BuildDirectoryTree(path);
                NavigateFileBrowser(path);
            }
            else if(node.IsConnectionStringNode)
                LoadConnectionStringProperties(node);
            else if(node.IsTableNode || node.IsViewNode)
                LoadTableProperties(node);
            else if(node.IsColumnNode)
                LoadColumnProperties(node);
            else if(node.NodeType == StageNodeType.StoredProcedure)
                LoadStoredProcedureProperties(node);
            else if(node.NodeType == StageNodeType.StoredProcedureParameter)
                LoadStoredProcedureParameterProperties(node);
            else if(node.NodeType == StageNodeType.SubStageConfiguration)
                LoadConfigurationProperties(node);

            ToggleToolbarItems(node);
            TogglePropertyGridButtons(node);

            LoadScaffold(node);
        }

        #endregion


        #region Property Loaders

        private static string GetTagString(object tag)
        {
            if(tag != null)
            {
                string tagValue = tag.ToString();
                return tagValue.Split(new char[] {'|'})[0];
            }
            return String.Empty;
        }

        private void LoadConnectionStringProperties(StageNode node)
        {
            if(node.ConnectionString != null)
            {
                pGrid.Tag = node;
                pGrid.ShowCustomProperties = true;
                pGrid.Items.Clear();
                const string category = "Connection String Properties";
                pGrid.Items.Add(CreateCustomProperty(node.ConnectionString, MM.ConnectionStrings.NameColumn, false, true, category));
                pGrid.Items.Add(CreateCustomProperty(node.ConnectionString, MM.ConnectionStrings.ConnectionStringColumn, false, true, category));

                pGrid.Refresh();
            }
        }

        private void LoadConfigurationProperties(StageNode node)
        {
            pGrid.Tag = node;
            pGrid.ShowCustomProperties = true;
            pGrid.Items.Clear();
            
            pGrid.Items.Add(CreateCustomProperty(config, MM.SubStageConfiguration.CodeGenerationLanguageColumn, false, true, CODE_GENERATION));

            CustomProperty cpCodeGenerationPath = CreateCustomProperty(config, MM.SubStageConfiguration.CodeGenerationPathColumn, false, true, CODE_GENERATION);
            cpCodeGenerationPath.CustomEditor = new FolderNameEditor();
            pGrid.Items.Add(cpCodeGenerationPath);

            pGrid.Items.Add(CreateCustomProperty(config, MM.SubStageConfiguration.OrganizeCodeByProviderColumn, false, true, CODE_GENERATION));

            const string category = "Embedded Web Server";

            CustomProperty cpWebServerRootPath = CreateCustomProperty(config, MM.SubStageConfiguration.WebServerRootPathColumn, false, true, category);
            cpWebServerRootPath.CustomEditor = new FolderNameEditor();
            pGrid.Items.Add(cpWebServerRootPath);

            pGrid.Items.Add(CreateCustomProperty(config, MM.SubStageConfiguration.WebServerPortColumn, false, true, category));
            pGrid.Refresh();
        }

        private void LoadProjectProperties(StageNode node)
        {
            if(node.Project != null)
            {
                pGrid.Tag = node;
                pGrid.ShowCustomProperties = true;
                pGrid.Items.Clear();
                const string category = "Project Properties";
                pGrid.Items.Add(CreateCustomProperty(node.Project, MM.Projects.NameColumn, false, true, category));
                pGrid.Items.Add(CreateCustomProperty(node.Project, MM.Projects.OriginalLocationColumn, true, true, category));

                pGrid.Items.Add(CreateCustomProperty(node.Project, MM.Projects.EnableTraceColumn, false, true, category));

                if(node.Project.Providers().Length > 0)
                {
                    if(node.Project.IsDefaultProviderNull())
                    {
                        node.Project.DefaultProvider = node.Project.Providers()[0].ProviderId;
                        MM.Save();
                    }
                    string displayValue = node.Project.DefaultProviderEntry.Name;
                    pGrid.Items.Add(MM.Projects.DefaultProviderColumn.ColumnName, displayValue, false, category, MM.Projects.DefaultProviderColumn.Caption,
                        true);
                    int currentIndex = pGrid.Items.Count - 1;
                    pGrid.Items[currentIndex].Tag = MM.Projects.DefaultProviderColumn.ColumnName + "|" + node.Project.ProjectId;
                    pGrid.Items[currentIndex].ValueMember = MM.Providers.ProviderIdColumn.ColumnName;
                    pGrid.Items[currentIndex].DisplayMember = MM.Providers.NameColumn.ColumnName;
                    pGrid.Items[currentIndex].Datasource = node.Project.Providers();
                    pGrid.Items[currentIndex].SelectedValue = node.Project.DefaultProvider;
                }
                else
                {
                    pGrid.Items.Add(MM.Projects.DefaultProviderColumn.ColumnName, "No Providers Available", true, category,
                        MM.Projects.DefaultProviderColumn.Caption, true);
                }

                CustomProperty cpCodeGenerationPath = CreateCustomProperty(node.Project, MM.Projects.CodeGenerationPathColumn, false, true, CODE_GENERATION);
                cpCodeGenerationPath.CustomEditor = new FolderNameEditor();
                pGrid.Items.Add(cpCodeGenerationPath);

                pGrid.Items.Add(CreateCustomProperty(node.Project, MM.Projects.OrganizeCodeByProviderColumn, false, true, CODE_GENERATION));

                CustomProperty cpTemplateDirectory = CreateCustomProperty(node.Project, MM.Projects.TemplateDirectoryColumn, false, true, CODE_GENERATION);
                cpTemplateDirectory.CustomEditor = new FolderNameEditor();
                pGrid.Items.Add(cpTemplateDirectory);

                pGrid.Refresh();
            }
        }

        private void LoadTableProperties(StageNode node)
        {
            if(node.Provider != null)
            {
                TableType tableType = node.IsTableNode ? TableType.Table : TableType.View;
                InstantiateProvider(node.Provider);
                TableSchema.Table table = DataService.GetInstance(node.Provider.Name).GetTableSchema(node.ItemKey, tableType);

                if(table != null)
                {
                    pGrid.Tag = node;
                    pGrid.ShowCustomProperties = true;
                    pGrid.Items.Clear();

                    pGrid.Items.Add(CreateCustomProperty("Class Name", table.ClassName, "The name of the generated class."));
                    pGrid.Items.Add(CreateCustomProperty("Database Name", table.Name, "The name of the underlying database object."));
                    pGrid.Items.Add(CreateCustomProperty("Has Primary Key", BoolToYesNo(table.HasPrimaryKey), "Whether or not the table has at least one primary key."));
                    pGrid.Items.Add(CreateCustomProperty("Has Foreign Keys", BoolToYesNo(table.HasForeignKeys), "Whether or not the table has one or more foreign keys."));
                    pGrid.Refresh();
                }
            }
        }

        private void LoadColumnProperties(StageNode node)
        {
            if(node.Provider != null)
            {
                TableType tableType = node.IsTableNode ? TableType.Table : TableType.View;
                InstantiateProvider(node.Provider);

                TableSchema.Table table = DataService.GetInstance(node.Provider.Name).GetTableSchema(((StageNode)node.Parent).ItemKey, tableType);
                if(table != null)
                {
                    TableSchema.TableColumn column = table.GetColumn(node.ItemKey);
                    if(column != null)
                    {
                        pGrid.Tag = node;
                        pGrid.ShowCustomProperties = true;
                        pGrid.Items.Clear();

                        pGrid.Items.Add(CreateCustomProperty("Property Name", column.PropertyName, "The name of the generated property."));
                        pGrid.Items.Add(CreateCustomProperty("Display Name", column.DisplayName, "The formatted display name."));
                        pGrid.Items.Add(CreateCustomProperty("Database Name", column.ColumnName, "The name of the underlying database object."));
                        pGrid.Items.Add(CreateCustomProperty("Data Type", column.DataType.ToString(), "The data type of the property."));
                        pGrid.Items.Add(CreateCustomProperty("System Type", column.GetPropertyType().ToString(), "The system type of the property."));
                        pGrid.Items.Add(CreateCustomProperty("Default Setting", column.DefaultSetting, "The default setting of the property."));
                        pGrid.Items.Add(CreateCustomProperty("Max Length", column.MaxLength.ToString(), "The maximum length of this property value."));

                        pGrid.Items.Add(CreateCustomProperty("Numeric", BoolToYesNo(column.IsNumeric), "Whether or not this property is numeric."));
                        pGrid.Items.Add(CreateCustomProperty("Date/Time", BoolToYesNo(column.IsDateTime), "Whether or not this property is date/time."));
                        pGrid.Items.Add(CreateCustomProperty("Nullable", BoolToYesNo(column.IsNullable), "Whether or not this property is nullable."));
                        pGrid.Items.Add(CreateCustomProperty("Read Only", BoolToYesNo(column.IsReadOnly), "Whether or not this property is read only."));
                        pGrid.Items.Add(
                            CreateCustomProperty("Managed by SubSonic", BoolToYesNo(column.IsReservedColumn), "Whether or not SubSonic manages the value of this property."));

                        pGrid.Refresh();
                    }
                }
            }
        }

        private void LoadStoredProcedureProperties(StageNode node)
        {
            if(node.Provider != null)
            {
                InstantiateProvider(node.Provider);

                StoredProcedure storedProcedure = null;
                List<StoredProcedure> storedProcedures = DataService.GetSPSchemaCollection(node.Provider.Name);

                foreach(StoredProcedure sp in storedProcedures)
                {
                    if(sp.Name == node.ItemKey)
                    {
                        storedProcedure = sp;
                        break;
                    }
                }

                if(storedProcedure != null)
                {
                    pGrid.Tag = node;
                    pGrid.ShowCustomProperties = true;
                    pGrid.Items.Clear();

                    pGrid.Items.Add(CreateCustomProperty("Display Name", storedProcedure.DisplayName, "The formatted display name."));
                    pGrid.Items.Add(CreateCustomProperty("Database Name", storedProcedure.Name, "The name of the underlying database object."));
                    pGrid.Items.Add(CreateCustomProperty("Schema Name", storedProcedure.SchemaName, "The name of the schema this stored procedure belongs to."));

                    pGrid.Refresh();
                }
            }
        }

        private void LoadStoredProcedureParameterProperties(StageNode node)
        {
            if(node.Provider != null)
            {
                InstantiateProvider(node.Provider);

                StoredProcedure storedProcedure = null;
                List<StoredProcedure> storedProcedures = DataService.GetSPSchemaCollection(node.Provider.Name);

                foreach(StoredProcedure sp in storedProcedures)
                {
                    if(sp.Name == ((StageNode)node.Parent).ItemKey)
                    {
                        storedProcedure = sp;
                        break;
                    }
                }

                if(storedProcedure != null)
                {
                    StoredProcedure.Parameter parameter = null;
                    foreach(StoredProcedure.Parameter p in storedProcedure.Parameters)
                    {
                        if(p.Name == node.ItemKey)
                        {
                            parameter = p;
                            break;
                        }
                    }

                    if(parameter != null)
                    {
                        pGrid.Tag = node;
                        pGrid.ShowCustomProperties = true;
                        pGrid.Items.Clear();

                        pGrid.Items.Add(CreateCustomProperty("Display Name", parameter.DisplayName, "The formatted display name."));
                        pGrid.Items.Add(CreateCustomProperty("Database Name", parameter.Name, "The name of the underlying database object."));
                        pGrid.Items.Add(CreateCustomProperty("Parameter Mode", parameter.Mode.ToString(), "The mode that this parameter operates in."));
                        pGrid.Items.Add(CreateCustomProperty("Database Type", parameter.DBType.ToString(), "The database type of the parameter."));

                        pGrid.Refresh();
                    }
                }
            }
        }

        private static string BoolToYesNo(bool booleanValue)
        {
            return booleanValue ? "Yes" : "No";
        }

        private void LoadProviderProperties(StageNode node)
        {
            if(node.Provider != null)
            {
                pGrid.Tag = node;
                pGrid.ShowCustomProperties = true;
                pGrid.Items.Clear();
                const string category = "General";
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.NameColumn, false, true, category));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.GeneratedNamespaceColumn, false, true, category));

                if(MM.ConnectionStrings.Count > 0)
                {
                    if(node.Provider.IsConnectionStringIdNull())
                    {
                        node.Provider.ConnectionStringId = MM.ConnectionStrings[0].ConnectionStringId;
                        MM.Save();
                    }
                    string displayValue = node.Provider.ConnectionString.Name;
                    pGrid.Items.Add(MM.Providers.ConnectionStringIdColumn.ColumnName, displayValue, false, category, "Connection String", true);
                    int currentIndex = pGrid.Items.Count - 1;
                    pGrid.Items[currentIndex].Tag = MM.Providers.ConnectionStringIdColumn.ColumnName + "|" + node.Provider.ProviderId;
                    pGrid.Items[currentIndex].ValueMember = MM.ConnectionStrings.ConnectionStringIdColumn.ColumnName;
                    pGrid.Items[currentIndex].DisplayMember = MM.ConnectionStrings.NameColumn.ColumnName;
                    pGrid.Items[currentIndex].Datasource = MM.ConnectionStrings;
                    pGrid.Items[currentIndex].SelectedValue = node.Provider.ConnectionStringId;
                }
                else
                {
                    pGrid.Items.Add(MM.Providers.ConnectionStringIdColumn.ColumnName, "No Connection Strings Available", true, category,
                        MM.Providers.ConnectionStringIdColumn.Caption, true);
                }

                if(MM.ProviderTypes.Count > 0)
                {
                    if(node.Provider.IsProviderTypeIdNull())
                    {
                        node.Provider.ProviderTypeId = MM.ProviderTypes[0].ProviderTypeId;
                        MM.Save();
                    }
                    string displayValue = node.Provider.ProviderType.DisplayName;
                    pGrid.Items.Add(MM.Providers.ProviderTypeIdColumn.ColumnName, displayValue, false, category, MM.Providers.ProviderTypeIdColumn.Caption, true);
                    int currentIndex = pGrid.Items.Count - 1;
                    pGrid.Items[currentIndex].Tag = MM.Providers.ProviderTypeIdColumn.ColumnName + "|" + node.Provider.ProviderId;
                    pGrid.Items[currentIndex].ValueMember = MM.ProviderTypes.ProviderTypeIdColumn.ColumnName;
                    pGrid.Items[currentIndex].DisplayMember = MM.ProviderTypes.DisplayNameColumn.ColumnName;
                    pGrid.Items[currentIndex].Datasource = MM.ProviderTypes;
                    pGrid.Items[currentIndex].SelectedValue = node.Provider.ProviderType;
                }

                CustomProperty cpCodeGenerationPath = CreateCustomProperty(node.Provider, MM.Providers.CodeGenerationPathColumn, false, true, CODE_GENERATION);
                cpCodeGenerationPath.CustomEditor = new FolderNameEditor();
                pGrid.Items.Add(cpCodeGenerationPath);
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.OrganizeCodeByProviderColumn, false, true, CODE_GENERATION));

                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.RemoveUnderscoresColumn, false, true, GLOBAL));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.AppendWithColumn, false, true, GLOBAL));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.AdditionalNamespacesColumn, false, true, GLOBAL));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.FixDatabaseObjectCasingColumn, false, true, GLOBAL));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.FixPluralClassNamesColumn, false, true, GLOBAL));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.GenerateNullablePropertiesColumn, false, true, GLOBAL));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.GenerateODSControllersColumn, false, true, GLOBAL));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.SetPropertyDefaultsFromDatabaseColumn, false, true, GLOBAL));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.UseExtendedPropertiesColumn, false, true, GLOBAL));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.UseUTCColumn, false, true, GLOBAL));

                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.RelatedTableLoadPrefixColumn, false, true, TABLES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.GenerateRelatedTablesAsPropertiesColumn, false, true, TABLES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.GenerateLazyLoadsColumn, false, true, TABLES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.ManyToManySuffixColumn, false, true, TABLES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.StripTableTextColumn, false, true, TABLES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.StripColumnTextColumn, false, true, TABLES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.IncludeTableListColumn, false, true, TABLES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.ExcludeTableListColumn, false, true, TABLES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.TableBaseClassColumn, false, true, TABLES));

                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.ViewBaseClassColumn, false, true, VIEWS));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.ViewStartsWithColumn, false, true, VIEWS));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.StripViewTextColumn, false, true, VIEWS));

                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.UseSPsColumn, false, true, STORED_PROCEDURES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.SPClassNameColumn, false, true, STORED_PROCEDURES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.SPStartsWithColumn, false, true, STORED_PROCEDURES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.ExtractClassNameFromSPNameColumn, false, true, STORED_PROCEDURES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.StripSPTextColumn, false, true, STORED_PROCEDURES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.StripParamTextColumn, false, true, STORED_PROCEDURES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.IncludeProcedureListColumn, false, true, STORED_PROCEDURES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.ExcludeProcedureListColumn, false, true, STORED_PROCEDURES));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.StoredProcedureBaseClassColumn, false, true, STORED_PROCEDURES));

                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.RegexIgnoreCaseColumn, false, true, REGULAR_EXPRESSIONS));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.RegexDictionaryReplaceColumn, false, true, REGULAR_EXPRESSIONS));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.RegexMatchExpressionColumn, false, true, REGULAR_EXPRESSIONS));
                pGrid.Items.Add(CreateCustomProperty(node.Provider, MM.Providers.RegexReplaceExpressionColumn, false, true, REGULAR_EXPRESSIONS));

                pGrid.Refresh();
            }
        }

        #endregion


        #region Add

        private void AddProject()
        {
            MasterStore.ProjectsRow newProject = (MasterStore.ProjectsRow)MM.Projects.NewRow();
            MM.Projects.AddProjectsRow(newProject);
            MM.Save();
            BuildTree();
            treeView1.SelectedNode = FindNodeByRow(newProject);
        }

        private void AddProvider(int projectId)
        {
            MasterStore.ProvidersRow newProvider = (MasterStore.ProvidersRow)MM.Providers.NewRow();
            MM.Providers.AddProvidersRow(newProvider);
            newProvider.ProjectId = projectId;
            MM.Save();
            BuildTree();
            treeView1.SelectedNode = FindNodeByRow(newProvider);
        }

        private void AddConnectionString()
        {
            MasterStore.ConnectionStringsRow newConnectionString = (MasterStore.ConnectionStringsRow)MM.ConnectionStrings.NewRow();
            MM.ConnectionStrings.AddConnectionStringsRow(newConnectionString);
            MM.Save();
            BuildTree();
            treeView1.SelectedNode = FindNodeByRow(newConnectionString);
        }

        #endregion


        #region Save

        private void SaveConnectionStringProperties(GridItem property)
        {
            MasterStore.ConnectionStringsRow connectionString = ((StageNode)pGrid.Tag).ConnectionString;
            if(connectionString != null)
            {
                foreach(CustomProperty cp in pGrid.Items)
                {
                    string tagType = GetTagString(cp.Tag);
                    if(tagType == property.Label)
                    {
                        connectionString[tagType] = cp.SelectedValue ?? property.Value;
                        MM.Save();
                        if(tagType == MM.ConnectionStrings.NameColumn.ColumnName)
                            UpdateNodeName(connectionString, connectionString.Name);
                        break;
                    }
                }
            }
        }

        private void SaveConfigurationProperties(GridItem property)
        {
            foreach(CustomProperty cp in pGrid.Items)
            {
                string tagType = GetTagString(cp.Tag);
                if(tagType == property.Label)
                {
                    config[tagType] = cp.SelectedValue ?? property.Value;
                    MM.Save();
                    break;
                }
            }
        }

        private void SaveProjectProperties(GridItem property)
        {
            MasterStore.ProjectsRow project = ((StageNode)pGrid.Tag).Project;
            if(project != null)
            {
                foreach(CustomProperty cp in pGrid.Items)
                {
                    string tagType = GetTagString(cp.Tag);
                    if(tagType == property.Label)
                    {
                        project[tagType] = cp.SelectedValue ?? property.Value;
                        MM.Save();
                        if(tagType == MM.Projects.NameColumn.ColumnName)
                            UpdateNodeName(project, project.Name);
                        break;
                    }
                }
            }
        }

        private void SaveProviderProperties(GridItem property)
        {
            //DM.ProvidersRow provider = MM.Providers.FindByProviderId(GetTagId(pGrid.Tag));

            MasterStore.ProvidersRow provider = ((StageNode)pGrid.Tag).Provider;
            if(provider != null)
            {
                foreach(CustomProperty cp in pGrid.Items)
                {
                    string tagType = GetTagString(cp.Tag);
                    if(tagType == property.Label)
                    {
                        provider[tagType] = cp.SelectedValue ?? property.Value;
                        MM.Save();
                        if(tagType == MM.Providers.NameColumn.ColumnName)
                            UpdateNodeName(provider, provider.Name);
                        break;
                    }
                }
            }
        }

        #endregion


        #region Delete

        private static void DeleteProject(int projectId)
        {
            MasterStore.ProjectsRow row = MM.Projects.FindByProjectId(projectId);
            if(row != null)
            {
                row.Delete();
                MM.Save();
            }
        }

        private static void DeleteProvider(int providerId)
        {
            MasterStore.ProvidersRow row = MM.Providers.FindByProviderId(providerId);
            if(row != null)
            {
                foreach(MasterStore.ProjectsRow projectRow in row.DefaultForProject())
                    projectRow.SetDefaultProviderNull();
                row.Delete();
                MM.Save();
            }
        }

        private static void DeleteConnectionString(int connectionStringId)
        {
            MasterStore.ConnectionStringsRow row = MM.ConnectionStrings.FindByConnectionStringId(connectionStringId);
            if(row != null)
            {
                foreach(MasterStore.ProvidersRow providerRow in row.Providers())
                    providerRow.SetConnectionStringIdNull();
                row.Delete();
                MM.Save();
            }
        }

        #endregion
    }
}