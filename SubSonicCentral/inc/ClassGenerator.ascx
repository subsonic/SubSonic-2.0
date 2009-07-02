<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ClassGenerator.ascx.cs" Inherits="ClassGenerator" %>
    <h3>Batch Class Generator</h3>
    You can generate files straight to your hard drive using this utility. Just check the tables below, and specify where you want the files dropped (make sure you give ASPNET write permissions to this file, or run this in debug mode). All of your class files will be dropped there (overwriting existing ones).
    <br />
    <table cellpadding="5" cellspacing="5">
        <colgroup>
            <col style="font-weight: bold; white-space: nowrap; text-align: right;" />
            <col />
        </colgroup>
        <tr>
            <td>Output Directory</td>
            <td><asp:TextBox ID="txtOut" runat="server" Width="424px" BorderStyle="Solid" BorderWidth="1px">C:\ActionPack\BatchOutput</asp:TextBox></td>
        </tr>
        <tr>
            <td>Language</td>
            <td>
                <asp:RadioButtonList ID="languageSelect" runat="server" BorderStyle="None" RepeatDirection="Horizontal" RepeatLayout="Flow" />
            </td>
        </tr>
        <tr>
            <td>Select the Provider:</td>
            <td><asp:DropDownList ID="ddlProviders" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlProvider_SelectedIndexChanged" /></td>
        </tr>
        <tr>
            <td style="white-space: nowrap" align="right"><strong>Selected Tables</strong><br />
                (<a href="javascript:toggleAll(true, '<%= chkTables.ClientID %>');">All</a> | <a href="javascript:toggleAll(false, '<%= chkTables.ClientID %>');">None</a>) </td>
            <td><asp:CheckBoxList ID="chkTables" runat="server" RepeatColumns="3" /></td>
        </tr>
        <tr>
            <td>
                Select the Views<br />
                (these are Read Only objects)<br />
                (<a href="javascript:toggleAll(true, '<%= chkViews.ClientID %>');">All</a> | <a href="javascript:toggleAll(false, '<%= chkViews.ClientID %>');">None</a>)
            </td>
            <td><asp:CheckBoxList ID="chkViews" runat="server" RepeatColumns="3" /></td>
        </tr>
                <tr>
            <td>Generate ODS Controllers?</td>
            <td><asp:CheckBox ID="chkODS" runat="server" Checked="true" /></td>
        </tr>
        <tr>
            <td>Create a Stored Procedure Wrapper?</td>
            <td><asp:CheckBox ID="chkSP" runat="server" Checked="true" /></td>
        </tr>
        <tr>
            <td></td>
            <td align="right"><asp:Button ID="btnGo" runat="server" Text="Generate" OnClick="btnGo_Click" BorderStyle="Solid" BorderWidth="1px" /></td>
        </tr>
    </table>
    <br />
    <asp:Label ID="lblResult" runat="server" Style="font-weight: bold; color: Blue;"></asp:Label>
    <br />