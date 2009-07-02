<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ConfigBuilder.ascx.cs"
  Inherits="ConfigBuilder" %>
<h2>
  Configuration Builder</h2>
<p>
  The configuration builder is designed to help you create new SubSonic configurations
  without manually adjusting the property of your config file. Fill in the fields
  below as appropriate, and then click the button to create the new configuration.
</p>
<asp:Panel ID="pnlForm" runat="server">
  <div class="Box">
    <div class="BoxTitle">
      General Settings
    </div>
    <div class="BoxContent">
      <div class="FormCaption">
        Name</div>
      <asp:TextBox ID="tbxName" runat="server" Width="150px" MaxLength="75">MyProvider</asp:TextBox>
      <div class="FormCaption">
        Provider Type</div>
      <asp:DropDownList ID="ddlProviderType" runat="Server">
        <asp:ListItem Text="Microsoft SQL Server" Value="SqlDataProvider" />
        <asp:ListItem Text="MySQL" Value="MySqlDataProvider" />
        <asp:ListItem Text="Oracle" Value="OracleDataProvider" />
        <asp:ListItem Text="Enterprise Library" Value="ELib3DataProvider" />
      </asp:DropDownList>
      <!--div class="FormCaption">
			Connection String</div-->
      <asp:TextBox ID="tbxConnectionString" runat="server" Width="500px" MaxLength="255"
        Visible="false">Data Source=(local); Database=Northwind; Integrated Security=true;</asp:TextBox>
      <div class="FormCaption">
        Template Directory</div>
      <asp:TextBox ID="tbxTemplateDirectory" runat="server" Width="500px" MaxLength="255"></asp:TextBox>
      <div class="FormCaption">
        Use Extended Properties</div>
      <asp:RadioButtonList ID="rblUseExtendedProperties" runat="server">
        <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false" Selected="true"></asp:ListItem>
      </asp:RadioButtonList>
            <div class="FormCaption">
        Enable Tracing</div>
      <asp:RadioButtonList ID="rblEnableTrace" runat="server">
        <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false" Selected="true"></asp:ListItem>
      </asp:RadioButtonList>
                  <div class="FormCaption">
        Fix Capitalization of database objects</div>
      <asp:RadioButtonList ID="rblFixDatabaseObjectCasing" runat="server">
        <asp:ListItem Text="Yes" Value="true" Selected="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false"></asp:ListItem>
      </asp:RadioButtonList>
                        <div class="FormCaption">
        Use UTC date formats for managed time fields</div>
      <asp:RadioButtonList ID="rblUseUtcDates" runat="server">
        <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false" Selected="true"></asp:ListItem>
      </asp:RadioButtonList>
            <div class="FormCaption">
        Additional Namespaces to include when generating code</div>
      <asp:TextBox ID="tbxAdditionalNamespaces" runat="server" Width="500px" MaxLength="255"></asp:TextBox>
    </div>
  </div>
  <br />
  <div class="Box">
    <div class="BoxTitle">
      Name Transformation
    </div>
    <div class="BoxContent">
      <div class="FormCaption">
        Namespace for Generated Code</div>
      <asp:TextBox ID="tbxGeneratedNamespace" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Make Class Names Singular</div>
      <asp:RadioButtonList ID="rblFixPluralClassNames" runat="server">
        <asp:ListItem Text="Yes" Value="true" Selected="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false"></asp:ListItem>
      </asp:RadioButtonList>
      <div class="FormCaption">
        Text to append to reserved Words</div>
      <asp:TextBox ID="tbxReservedWordSuffix" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Prefix related table retrieval methods with</div>
      <asp:TextBox ID="tbxRelatedTableLoadPrefix" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Text to strip from Columns</div>
      <asp:TextBox ID="tbxColumnStrip" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Remove underscores when generating</div>
      <asp:RadioButtonList ID="rblRemoveUnderscores" runat="server">
        <asp:ListItem Text="Yes" Value="true" Selected="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false"></asp:ListItem>
      </asp:RadioButtonList>
    </div>
  </div>
  <br />
  <div class="Box">
    <div class="BoxTitle">
      Tables
    </div>
    <div class="BoxContent">
      <div class="FormCaption">
        Include the following tables</div>
      <asp:TextBox ID="tbxTableInclude" runat="server" Width="300px" MaxLength="75">*</asp:TextBox>
      <div class="FormCaption">
        Exclude the following tables</div>
      <asp:TextBox ID="tbxTableExclude" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Text to strip from Tables</div>
      <asp:TextBox ID="tbxTableStrip" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
            <div class="FormCaption">
        Table Base Class</div>
      <asp:TextBox ID="tbxTableBaseClass" runat="server" Width="300px" MaxLength="75">ActiveRecord</asp:TextBox>
      <div class="FormCaption">
        Generate Object Data Source (ODS) Controllers for tables.</div>
      <asp:RadioButtonList ID="rblGenerateODSControllers" runat="server">
        <asp:ListItem Text="Yes" Value="true" Selected="True"></asp:ListItem>
        <asp:ListItem Text="No" Value="false"></asp:ListItem>
      </asp:RadioButtonList>
      <div class="FormCaption">
        Generate related table names as properties, not methods</div>
      <asp:RadioButtonList ID="rblRelatedTablesAsProperties" runat="server">
        <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false" Selected="true"></asp:ListItem>
      </asp:RadioButtonList>
      <div class="FormCaption">
        Use lazy loading on related tables</div>
      <asp:RadioButtonList ID="rblUseLazyLoads" runat="server">
        <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false" Selected="true"></asp:ListItem>
      </asp:RadioButtonList>
      <div class="FormCaption">
        Initialize object properties with database defaults</div>
      <asp:RadioButtonList ID="rblSetPropertyDefaultsFromDatabase" runat="server">
        <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false" Selected="true"></asp:ListItem>
      </asp:RadioButtonList>
      <div class="FormCaption">
        Use nullable types for nullable properties</div>
      <asp:RadioButtonList ID="rblGenerateNullableProperties" runat="server">
        <asp:ListItem Text="Yes" Value="true" Selected="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false"></asp:ListItem>
      </asp:RadioButtonList>
    </div>
  </div>
  <br />
  <div class="Box">
    <div class="BoxTitle">
      Views
    </div>
    <div class="BoxContent">
      <div class="FormCaption">
        Only process Views that start with</div>
      <asp:TextBox ID="tbxViewPrefixFilter" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Text to strip from Views</div>
      <asp:TextBox ID="tbxViewStrip" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        View Base Class</div>
      <asp:TextBox ID="tbxViewBaseClass" runat="server" Width="300px" MaxLength="75">ReadOnlyRecord</asp:TextBox>
    </div>
  </div>
  <br />
  <div class="Box">
    <div class="BoxTitle">
      Stored Procedures
    </div>
    <div class="BoxContent">
      <div class="FormCaption">
        Use Stored Procedures</div>
      <asp:RadioButtonList ID="rblUseStoredProcedures" runat="server">
        <asp:ListItem Text="Yes" Value="true" Selected="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false"></asp:ListItem>
      </asp:RadioButtonList>
      <div class="FormCaption">
        Include the following Stored Procedures</div>
      <asp:TextBox ID="tbxSPInclude" runat="server" Width="300px" MaxLength="75">*</asp:TextBox>
      <div class="FormCaption">
        Exclude the following Stored Procedures</div>
      <asp:TextBox ID="tbxSPExclude" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Attempt to extract class name from stored procedure name</div>
      <asp:RadioButtonList ID="rblExtractSPClassName" runat="server">
        <asp:ListItem Text="Yes" Value="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false" Selected="true"></asp:ListItem>
      </asp:RadioButtonList>
      <div class="FormCaption">
        Only process stored procedures that begin with</div>
      <asp:TextBox ID="tbxStoredProcedurePrefixFilter" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Stored Procedure Output Class Name</div>
      <asp:TextBox ID="tbxStoredProcedureClassName" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Text to Strip from Stored Procedures</div>
      <asp:TextBox ID="tbxStoredProcedureStrip" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Text to Strip from Stored Procedure Parameters</div>
      <asp:TextBox ID="tbxStoredProcedureParameterStrip" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
            <div class="FormCaption">
        Stored Procedure Base Class</div>
      <asp:TextBox ID="tbxSPBaseClass" runat="server" Width="300px" MaxLength="75">StoredProcedure</asp:TextBox>
    </div>
  </div>
  <br />
  <div class="Box">
    <div class="BoxTitle">
      Regular Expressions
    </div>
    <div class="BoxContent">
      <div class="FormCaption">
        Ignore case when applying regular expressions</div>
      <asp:RadioButtonList ID="rblRegexIgnoreCase" runat="server">
        <asp:ListItem Text="Yes" Value="true" Selected="true"></asp:ListItem>
        <asp:ListItem Text="No" Value="false"></asp:ListItem>
      </asp:RadioButtonList>
      <div class="FormCaption">
        Process text that matches the expression:</div>
      <asp:TextBox ID="tbxRegexMatch" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Replace matched text with the expression:</div>
      <asp:TextBox ID="tbxRegexReplace" runat="server" Width="300px" MaxLength="75"></asp:TextBox>
      <div class="FormCaption">
        Process the following key/value pairs</div>
      <asp:TextBox ID="tbxRegexDictionaryReplace" runat="server" Width="300px" MaxLength="75">foo,bar;bar,foo</asp:TextBox>
    </div>
  </div>
  <br />
  <div class="Box">
    <div class="BoxTitle">
    </div>
    <div class="BoxContent">
      <div style="padding-left: 100px;">
        <asp:Button ID="btnGenerate" runat="server" Text="Generate!" OnClick="btnGenerate_Click" />
      </div>
    </div>
  </div>
</asp:Panel>
<asp:Panel ID="pnlOutput" runat="server" Visible="false">
  <div class="Box">
    <div class="BoxTitle">
      Configuration Output
    </div>
    <div class="BoxContent">
      <asp:TextBox ID="tbxOutput" runat="server" Rows="6" TextMode="MultiLine" Width="500px"></asp:TextBox></div>
  </div>
</asp:Panel>
