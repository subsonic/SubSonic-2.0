using System;
using System.Collections.Specialized;
using System.Collections;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SubSonic;
using SubSonic.Utilities;

public partial class ScaffoldGenerator : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (txtOut.Text == String.Empty)
        {
            txtOut.Text = Server.MapPath("~/Dev");
        }
        if (!Page.IsPostBack)
        {
            foreach (DataProvider p in DataService.Providers)
            {
                ddlProvider.Items.Add(p.Name);
            }
            foreach (ICodeLanguage language in CodeLanguageFactory.AllCodeLanguages)
                languageSelect.Items.Add(new ListItem(language.Identifier, language.ShortName));
            BuildTableList();
        }
    }

    private void BuildTableList()
    {
        chkTables.Items.Clear();
        string[] tableList = DataService.GetTableNames(ddlProvider.SelectedValue);
        foreach (string s in tableList)
        {
            if(CodeService.ShouldGenerate(s, ddlProvider.SelectedValue))
            {
                TableSchema.Table tbl = DataService.GetSchema(s, ddlProvider.SelectedValue, TableType.Table);
                string chkName = s;
                bool isChecked = true;
                if(tbl.PrimaryKey == null)
                {
                    chkName += " (No PK!)";
                    isChecked = false;
                }
                ListItem item = new ListItem(chkName, s);
                item.Selected = isChecked;
                item.Enabled = isChecked;
                chkTables.Items.Add(item);
            }
        }
    }

    protected void btnGo_Click(object sender, EventArgs e)
    {
        DataProvider provider = DataService.GetInstance(ddlProvider.SelectedValue);
        if (provider != null)
        {
            try
            {
                StringBuilder sbIndex = new StringBuilder();
                ArrayList fileNames = new ArrayList();
                bool outputCode = radOutputType.SelectedIndex == 0;

                ICodeLanguage language = CodeLanguageFactory.GetByShortName(languageSelect.SelectedValue);
                string masterPageText = masterPageName.Text.Trim();

                foreach (ListItem item in chkTables.Items)
                {
                    TurboCompiler turboCompiler = new TurboCompiler();
                    if(item.Selected)
                    {
                        string tableFileName = item.Value.Replace(SpecialString.SPACE, String.Empty);
                        string tableName = item.Value;
                        string className = DataService.GetSchema(tableName, provider.Name, TableType.Table).ClassName;
                        string fileNameNoExtension = tbxPrefix.Text.Trim() + FormatTableName(tableFileName) + tbxSuffix.Text.Trim();
                        string fileName = fileNameNoExtension + FileExtension.DOT_ASPX;
                        string filePath = txtOut.Text + "\\" + fileName;
                        fileNames.Add(filePath);

                        NameValueCollection nVal = new NameValueCollection();
                        nVal.Add(TemplateVariable.LANGUAGE, language.Identifier);
                        nVal.Add(TemplateVariable.CLASS_NAME, className);
                        nVal.Add(TemplateVariable.TABLE_NAME, tableName);
                        nVal.Add(TemplateVariable.MASTER_PAGE, masterPageText);

                        if(outputCode)
                        {
                            nVal.Add(TemplateVariable.LANGUAGE_EXTENSION, language.FileExtension);
                            nVal.Add(TemplateVariable.PROVIDER, provider.Name);
                            nVal.Add(TemplateVariable.PAGE_FILE, fileNameNoExtension);
                            
                            TurboTemplate scaffoldCodeBehind = CodeService.BuildTemplate(CodeService.TemplateType.GeneratedScaffoldCodeBehind, nVal, language, provider);
                            scaffoldCodeBehind.AddUsingBlock = false;
                            scaffoldCodeBehind.OutputPath = filePath.Replace(FileExtension.DOT_ASPX, (FileExtension.DOT_ASPX + language.FileExtension));
                            turboCompiler.AddTemplate(scaffoldCodeBehind);

                            TurboTemplate scaffoldMarkup = CodeService.BuildTemplate(CodeService.TemplateType.GeneratedScaffoldMarkup, nVal, language, provider);
                            scaffoldMarkup.AddUsingBlock = false;
                            scaffoldMarkup.OutputPath = filePath;
                            turboCompiler.AddTemplate(scaffoldMarkup);
                        }
                        else
                        {
                            TurboTemplate dynamicScaffold = CodeService.BuildTemplate(CodeService.TemplateType.DynamicScaffold, nVal, language, provider);
                            dynamicScaffold.AddUsingBlock = false;
                            dynamicScaffold.OutputPath = filePath;
                            turboCompiler.AddTemplate(dynamicScaffold);
                        }
                        sbIndex.AppendLine("<a href=\"" + fileName + "\">" + FormatTableName(tableName) + "</a><br/>");
                    }
                    if(turboCompiler.Templates.Count > 0)
                    {
                        turboCompiler.Run();
                        foreach(TurboTemplate template in turboCompiler.Templates)
                        {
                            Utility.WriteTrace("Writing " + template.TemplateName + " as " + template.OutputPath.Substring(template.OutputPath.LastIndexOf("\\") + 1));
                            SubSonic.Sugar.Files.CreateToFile(template.OutputPath, template.FinalCode);
                        }
                    }
                }

                if (chkIndexPage.Checked && tbxIndexName.Text != String.Empty)
                {
                    string before = "<html><head><title>SubSonic Scaffold Index Page</title></head><body>";
                    string after = "</body></html>";
                    WriteToFile(txtOut.Text + "\\" + tbxIndexName.Text, before + sbIndex + after);
                }

                lblResult.Text = "Finished";
            }
            catch (Exception x)
            {
                lblResult.Text = "Error: " + x.Message;
            }
        }
    }

    public void WriteToFile(string AbsoluteFilePath, string fileText)
    {
        using (StreamWriter sw = new StreamWriter(AbsoluteFilePath, false))
        {
            sw.Write(fileText);
            sw.Close();
        }
    }

    public string FormatTableName(string tableName)
    {
        if (Utility.IsMatch(rblCapitalization.SelectedValue, "upper"))
            return tableName.ToUpper();

        if (Utility.IsMatch(rblCapitalization.SelectedValue, "lower"))
            return tableName.ToLower();

        return tableName;
    }

    public string GetProperName(string sIn)
    {
        string propertyName = sIn;
        string leftOne = propertyName.Substring(0, 1).ToUpper();
        propertyName = propertyName.Substring(1, propertyName.Length - 1);
        propertyName = leftOne + propertyName;

        //if (propertyName.EndsWith("TypeCode")) propertyName = propertyName.Substring(0, propertyName.Length - 4);

        return propertyName;
    }

    protected void ddlProvider_SelectedIndexChanged(object sender, EventArgs e)
    {
        BuildTableList();
    }
	
}
