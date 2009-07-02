using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using SubSonic;
using SubSonic.Utilities;

public partial class ClassGenerator : System.Web.UI.UserControl
{
    private const string PROVIDER_NAME = "PROVIDER_NAME";
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            InitializeControls();
            if (ddlProviders.Items.Count > 0)
            {
                Session[PROVIDER_NAME] = ddlProviders.Items[0].Value;
            }
        }
    }

    protected void ddlProvider_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session[PROVIDER_NAME] = ddlProviders.SelectedValue;
        InitializeControls();
    }

    void InitializeControls()
    {
        string providerName = (string)Session[PROVIDER_NAME];
        if (DataService.Providers == null)
        {
            DataService.LoadProviders();
        }
        if (DataService.Providers != null)
        {
            ddlProviders.Items.Clear();
            foreach (DataProvider p in DataService.Providers)
            {
                ddlProviders.Items.Add(p.Name);
            }
        }

        foreach (ICodeLanguage language in CodeLanguageFactory.AllCodeLanguages)
            languageSelect.Items.Add(new ListItem(language.Identifier, language.ShortName));

        SetTables(providerName);
        SetViews(providerName);
    }

    private void SetTables(string providerName)
    {
        string[] tableList;
        if (!String.IsNullOrEmpty(providerName))
        {
            ddlProviders.SelectedValue = providerName;
            tableList = DataService.GetTableNames(providerName);
        }
        else
        {
            tableList = DataService.GetTableNames(DataService.Provider.Name);
        }
        BuildList(chkTables, tableList);
    }

    private void SetViews(string providerName)
    {
        string[] viewList;
        if (!String.IsNullOrEmpty(providerName))
        {
            ddlProviders.SelectedValue = providerName;
            viewList = DataService.GetViewNames(providerName);
        }
        else
        {
            viewList = DataService.GetViewNames(DataService.Provider.Name);
        }
        BuildList(chkViews, viewList);
    }

    private void BuildList(ListControl cbl, IEnumerable<string> tableList)
    {
        cbl.Items.Clear();
        foreach (string s in tableList)
        {
            if (CodeService.ShouldGenerate(s, ddlProviders.SelectedValue))
            {
                ListItem item = new ListItem(s, s);
                cbl.Items.Add(item);
            }
        }
    }

    protected void btnGo_Click(object sender, EventArgs e)
    {
        string sOutPath = txtOut.Text;

        if (!Directory.Exists(sOutPath))
        {
            Directory.CreateDirectory(sOutPath);
        }
        try
        {
            ICodeLanguage language = CodeLanguageFactory.GetByShortName(languageSelect.SelectedValue);

            string providerName = (string)Session[PROVIDER_NAME];
            if (!String.IsNullOrEmpty(providerName))
            {
                DataProvider provider = DataService.GetInstance(providerName);
                if (provider != null)
                {
                    TurboCompiler turboCompiler = new TurboCompiler();
                    StringBuilder sbTableList = new StringBuilder();
                    foreach (ListItem item in chkTables.Items)
                    {
                        if (item.Selected)
                        {
                            sbTableList.AppendLine(item.Value);
                        }
                    }

                    foreach (ListItem item in chkTables.Items)
                    {
                        if (item.Selected)
                        {

                            TableSchema.Table t = DataService.GetTableSchema(item.Value, provider.Name, TableType.Table);
                            if (t.ForeignKeys.Count > 0)
                            {
                                //provider.GetManyToManyTables(t, evalTables);
                            }
                            string className = DataService.GetTableSchema(item.Value, providerName, TableType.Table).ClassName;
                            string filePath = Path.Combine(sOutPath, className + language.FileExtension);
                            TurboTemplate classTemplate = CodeService.BuildClassTemplate(item.Value, language, provider);
                            classTemplate.OutputPath = filePath;
                            turboCompiler.AddTemplate(classTemplate);
                            //SubSonic.Sugar.Files.CreateToFile(filePath, usings + CodeService.RunClass(item.Value, providerName, language));

                            if(chkODS.Checked)
                            {
                                filePath = Path.Combine(sOutPath, className + "Controller" + language.FileExtension);
                                TurboTemplate odsTemplate = CodeService.BuildODSTemplate(item.Value, language, provider);
                                odsTemplate.OutputPath = filePath;
                                turboCompiler.AddTemplate(odsTemplate);
                                //SubSonic.Sugar.Files.CreateToFile(filePath, usings + CodeService.RunODS(item.Value, providerName, language));
                            }
                        }
                    }

                    //output the Views
                    foreach (ListItem item in chkViews.Items)
                    {
                        if (item.Selected)
                        {
                            string className = DataService.GetTableSchema(item.Value, providerName, TableType.View).ClassName;
                            string filePath = Path.Combine(sOutPath, className + language.FileExtension);
                            TurboTemplate viewTemplate = CodeService.BuildViewTemplate(item.Value, language, provider);
                            viewTemplate.OutputPath = filePath;
                            turboCompiler.AddTemplate(viewTemplate);
                            //SubSonic.Sugar.Files.CreateToFile(filePath, usings + CodeService.RunReadOnly(item.Value, providerName, language));
                        }
                    }
                    //output the SPs
                    if (chkSP.Checked)
                    {
                        string filePath = Path.Combine(sOutPath, "StoredProcedures" + language.FileExtension);
                        TurboTemplate spTemplate = CodeService.BuildSPTemplate(language, provider);
                        spTemplate.OutputPath = filePath;
                        turboCompiler.AddTemplate(spTemplate);
                        //SubSonic.Sugar.Files.CreateToFile(filePath, usings + CodeService.RunSPs(providerName, language));
                    }

                    //structs
                    string structPath = Path.Combine(sOutPath, "AllStructs" + language.FileExtension);
                    TurboTemplate structsTemplate = CodeService.BuildStructsTemplate(language, provider);
                    structsTemplate.OutputPath = structPath;
                    turboCompiler.AddTemplate(structsTemplate);
                    //SubSonic.Sugar.Files.CreateToFile(structPath, usings + CodeService.RunStructs(language));

                    if (turboCompiler.Templates.Count > 0)
                    {
                        turboCompiler.Run();
                        foreach (TurboTemplate template in turboCompiler.Templates)
                        {
                            Utility.WriteTrace("Writing " + template.TemplateName + " as " + template.OutputPath.Substring(template.OutputPath.LastIndexOf("\\") + 1));
                            SubSonic.Sugar.Files.CreateToFile(template.OutputPath, template.FinalCode);
                        }
                    }
                    lblResult.Text = "View your files: <a href='file://" + sOutPath + "'>" + sOutPath + "</a>";
                }
            }
        }
        catch (Exception x)
        {
            lblResult.Text = "Error: " + x.Message;
        }
    }
}
