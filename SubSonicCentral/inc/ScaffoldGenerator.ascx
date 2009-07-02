<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ScaffoldGenerator.ascx.cs" Inherits="ScaffoldGenerator" %>
    <h3>
        Batch Scaffold Generator</h3>
    To generate scaffold files:<br />
    <br />
    <ol>
        <li>Create a folder in your web site at the location specified in the <strong>Output
            Directory</strong> field. If this field is blank, files will be placed in the "Dev"
            folder directly below the root of the site</li>
        <li>Select the output language for the pages.</li>
        <li>If you wish to use a master page, specify the path and name in the <strong>Master
            Page Reference</strong> field. Example: <em>~/default.master</em></li>
        <li>Select what type of case transformation will be applied to the table name when the
            output file is generated.</li>
        <li>To change how the file is named, you can enter a prefix and/or suffix that will
            be added to the table name to create the output filename. The Table Case Transformation
            value is applied only to the table portion of the name, not the prefix or suffix.</li>
        <li>Select the tables that you wish to generate Scaffold pages for. For each table selected,
            a Scaffold page will be created in the output directory.</li>
        <li>To add the generated pages to your solution, select the output folder and click
            the "Show All Files" button in the solution command bar. Select the pages and then
            click "Add To Project".</li>
    </ol>
    <table cellpadding="5" cellspacing="5">
        <tr>
            <td align="right" style="font-weight: bold; white-space: nowrap">
                Provider</td>
            <td>
                <asp:DropDownList ID="ddlProvider" runat="server" OnSelectedIndexChanged="ddlProvider_SelectedIndexChanged" AutoPostBack="True">
                </asp:DropDownList></td>
        </tr>
        <tr>
            <td style="white-space: nowrap; font-weight: bold" align="right">
                Output Directory</td>
            <td>
                <asp:TextBox ID="txtOut" runat="server" Width="400px" BorderStyle="Solid" BorderWidth="1px"></asp:TextBox></td>
        </tr>
        <tr>
            <td align="right" style="font-weight: bold; white-space: nowrap">
                Output Type</td>
            <td>
                <asp:RadioButtonList ID="radOutputType" runat="server" BorderStyle="None" RepeatDirection="Horizontal"
                    RepeatLayout="Flow">
                    <asp:ListItem Selected="True" Text="Page And Code" Value="Code"></asp:ListItem>
                    <asp:ListItem Text="Scaffold Control" Value="Scaffold"></asp:ListItem>
                </asp:RadioButtonList></td>
        </tr>
        <tr>
            <td style="white-space: nowrap; font-weight: bold" align="right">
                Language</td>
            <td>
                <asp:RadioButtonList ID="languageSelect" runat="server" BorderStyle="None" RepeatDirection="Horizontal"
                    RepeatLayout="Flow" />
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap; font-weight: bold" align="right">
                Master Page
            </td>
            <td>
                <asp:TextBox ID="masterPageName" runat="server" BorderStyle="Solid" BorderWidth="1px"
                    Width="250px"></asp:TextBox></td>
        </tr>
        <tr>
            <td style="white-space: nowrap; font-weight: bold" align="right">
                Table Case Transformation
            </td>
            <td>
                <asp:RadioButtonList ID="rblCapitalization" runat="server" BorderStyle="None" RepeatDirection="Horizontal"
                    RepeatLayout="Flow">
                    <asp:ListItem Text="None" Value="none" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="UPPER" Value="upper"></asp:ListItem>
                    <asp:ListItem Text="lower" Value="lower"></asp:ListItem>
                </asp:RadioButtonList></td>
        </tr>
        <tr>
            <td style="white-space: nowrap; font-weight: bold" align="right">
                &nbsp;File Name Prefix/Suffix
            </td>
            <td style="white-space: nowrap">
                <asp:TextBox ID="tbxPrefix" runat="server" BorderStyle="Solid" BorderWidth="1px"
                    Width="125px"></asp:TextBox>
                &lt;TableName&gt;
                <asp:TextBox ID="tbxSuffix" runat="server" BorderStyle="Solid" BorderWidth="1px"
                    Width="125px">_scaffold</asp:TextBox>
                .aspx
            </td>
        </tr>
        <tr>
              <td style="white-space: nowrap; font-weight: bold" align="right">
                Generate Index Page
            </td>
            <td valign="top">
                <asp:CheckBox ID="chkIndexPage" runat="server">
                </asp:CheckBox>&nbsp;&nbsp;&nbsp;
                <span style="font-weight:bold">Name:</span>&nbsp;<asp:TextBox ID="tbxIndexName" runat="server" BorderStyle="Solid" BorderWidth="1px" Width="125px">default.htm</asp:TextBox>
            </td>
        </tr>
        <tr>
            <td style="white-space: nowrap" align="right">
                <strong>Selected Tables</strong><br />
                (<a href="javascript:toggleAll(true, '<%= chkTables.ClientID %>');">All</a> | <a href="javascript:toggleAll(false, '<%= chkTables.ClientID %>');">
                    None</a>)
                <br />
            </td>
            <td>
                <asp:CheckBoxList ID="chkTables" runat="server" RepeatColumns="3">
                </asp:CheckBoxList></td>
        </tr>
        <tr>
            <td>
            </td>
            <td align="right">
                <asp:Button ID="btnGo" runat="server" Text="Generate" OnClick="btnGo_Click" BorderStyle="Solid"
                    BorderWidth="1px" /></td>
        </tr>
    </table>
    <br />
    <span style="font-weight: bold; color: Blue">
        <asp:Label ID="lblResult" runat="server"></asp:Label></span>
    <br />
    <p>
        <em>Note: To generate files, the ASPNET worker process must have write access to the
            site. </em>
    </p>