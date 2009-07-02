using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SubSonic;

public partial class ConfigBuilder : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private static void AddConfigProperty(StringBuilder sb, string property, string value)
    {
        if(!String.IsNullOrEmpty(value))
        {
            if(sb.Length > 0)
            {
                sb.Append(SpecialString.SPACE);
            }
            sb.Append(property);
            sb.Append("=\"");
            sb.Append(value);
            sb.Append("\"");
        }
    }

    private void BuildConfig()
    {
        StringBuilder sbService = new StringBuilder("<SubSonicService");
        AddConfigProperty(sbService, ConfigurationPropertyName.DEFAULT_PROVIDER, tbxName.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.TEMPLATE_DIRECTORY, tbxTemplateDirectory.Text);
        sbService.AppendLine(">");
        sbService.AppendLine("     <providers>");
        sbService.AppendLine("     <clear/>"); //http://weblogs.asp.net/scottgu/archive/2006/11/20/common-gotcha-don-t-forget-to-clear-when-adding-providers.aspx
        sbService.Append("          <add name=\"" + tbxName.Text + "\" type=\"SubSonic." + ddlProviderType.SelectedValue + ", SubSonic\"");
        AddConfigProperty(sbService, ConfigurationPropertyName.APPEND_WITH, tbxReservedWordSuffix.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.ADDITIONAL_NAMESPACES, tbxAdditionalNamespaces.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.ENABLE_TRACE, rblEnableTrace.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.EXCLUDE_PROCEDURE_LIST, tbxSPExclude.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.EXCLUDE_TABLE_LIST, tbxTableExclude.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.EXTRACT_CLASS_NAME_FROM_SP_NAME, rblExtractSPClassName.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.FIX_DATABASE_OBJECT_CASING, rblFixDatabaseObjectCasing.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.FIX_PLURAL_CLASS_NAMES, rblFixPluralClassNames.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.GENERATED_NAMESPACE, tbxGeneratedNamespace.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.GENERATE_LAZY_LOADS, rblUseLazyLoads.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.GENERATE_NULLABLE_PROPERTIES, rblGenerateNullableProperties.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.GENERATE_ODS_CONTROLLERS, rblGenerateODSControllers.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.GENERATE_RELATED_TABLES_AS_PROPERTIES, rblRelatedTablesAsProperties.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.INCLUDE_PROCEDURE_LIST, tbxSPInclude.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.INCLUDE_TABLE_LIST, tbxTableInclude.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.REGEX_DICTIONARY_REPLACE, tbxRegexDictionaryReplace.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.REGEX_IGNORE_CASE, rblRegexIgnoreCase.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.REGEX_MATCH_EXPRESSION, tbxRegexMatch.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.REGEX_REPLACE_EXPRESSION, tbxRegexReplace.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.RELATED_TABLE_LOAD_PREFIX, tbxRelatedTableLoadPrefix.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.REMOVE_UNDERSCORES, rblRemoveUnderscores.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.SET_PROPERTY_DEFAULTS_FROM_DATABASE, rblSetPropertyDefaultsFromDatabase.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.SP_STARTS_WITH, tbxStoredProcedurePrefixFilter.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.STORED_PROCEDURE_CLASS_NAME, tbxStoredProcedureClassName.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_COLUMN_TEXT, tbxColumnStrip.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_PARAM_TEXT, tbxStoredProcedureParameterStrip.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_STORED_PROCEDURE_TEXT, tbxStoredProcedureStrip.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_TABLE_TEXT, tbxTableStrip.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.STRIP_VIEW_TEXT, tbxViewStrip.Text);
        AddConfigProperty(sbService, ConfigurationPropertyName.USE_EXTENDED_PROPERTIES, rblUseExtendedProperties.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.USE_STORED_PROCEDURES, rblUseStoredProcedures.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.USE_UTC_TIMES, rblUseUtcDates.SelectedValue);
        AddConfigProperty(sbService, ConfigurationPropertyName.VIEW_STARTS_WITH, tbxViewPrefixFilter.Text);

        sbService.AppendLine("/>");
        sbService.AppendLine("     </providers>");
        sbService.AppendLine("</SubSonicService>");

        tbxOutput.Text = sbService.ToString();

    }
    protected void btnGenerate_Click(object sender, EventArgs e)
    {
        pnlForm.Visible = false;
        pnlOutput.Visible = true;
        BuildConfig();
    }
}
