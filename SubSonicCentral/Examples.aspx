<%@ Page Language="C#" MasterPageFile="~/res/MasterPage.master" AutoEventWireup="true"
  CodeFile="Examples.aspx.cs" Inherits="Examples" Theme="Default" Title="SubSonic Examples" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphFull" runat="Server">
  <table>
    <tr>
      <td style="white-space: nowrap" valign="top">
        <div class="Box">
          <div class="BoxTitle">
            <asp:Label ID="Label1" runat="server">Examples</asp:Label>
          </div>
          <div class="BoxContent">
            <asp:LinkButton ID="lnkAll" runat="server" OnClick="lnkAll_Click">Show All Data</asp:LinkButton><br />
            <asp:LinkButton ID="LinkButton2" runat="server" OnClick="LinkButton2_Click">Use a Collection</asp:LinkButton><br />
            <asp:LinkButton ID="lnkTop20" runat="server" OnClick="lnkTop20_Click">Show Top 20</asp:LinkButton><br />
            <asp:LinkButton ID="lnkTopPrice" runat="server" OnClick="lnkTopPrice_Click">Show Top 10 by Price</asp:LinkButton><br />
            <asp:LinkButton ID="LinkButton1" runat="server" OnClick="LinkButton1_Click">Use a View</asp:LinkButton><br />
            <asp:LinkButton ID="LinkButton3" runat="server" OnClick="LinkButton3_Click">Use a Stored Procedure</asp:LinkButton><br />
            <asp:LinkButton ID="lnkBatchUpdate" runat="server" OnClick="lnkBatchUpdate_Click">Batch Update</asp:LinkButton><br />
            <asp:LinkButton ID="lbtInspect" runat="server" OnClick="lbtInspect_Click">Use Query.Inspect()</asp:LinkButton><br />
            <asp:LinkButton ID="LinkButton4" runat="server" OnClick="LinkButton4_Click">Use Product.Query()</asp:LinkButton><br />
            <asp:LinkButton ID="LinkButton5" runat="server" OnClick="LinkButton5_Click">Use IN()</asp:LinkButton><br />
            <asp:LinkButton ID="LinkButton6" runat="server" OnClick="LinkButton6_Click">Use OR()</asp:LinkButton><br />
            <asp:LinkButton ID="LinkButton7" runat="server" OnClick="LinkButton7_Click">Use a Complex OR()</asp:LinkButton><br />
          </div>
        </div>
      </td>
      <td style="width: 100%" valign="top">
        <div class="Box">
          <div class="BoxTitle">
            <asp:Label ID="lblExampleName" runat="server">Click a link to see SubSonic in Action!</asp:Label>
          </div>
          <div class="BoxContent">
            <pre><asp:Label ID="lblCode" runat="server"></asp:Label></pre>
          </div>
          <div class="BoxContent">
            <div style="overflow:auto">
              <asp:GridView ID="GridView1" runat="server" SkinID="scaffold">
              </asp:GridView>
              <asp:Literal ID="litInspect" runat="server"></asp:Literal>
              <asp:Panel ID="pnlSubSonicControls" Visible="false" runat="server">
                <p>
                  DropDown bound to Products table</p>
                <subsonic:DropDown ID="ddlProducts" TableName="Products" runat="server" />
                <hr />
                <p>
                  ManyManyList showing Territories selected for Employee ID 1</p>
                <subsonic:ManyManyList ID="manyEmployeesTerritories" PrimaryTableName="Employees"
                  PrimaryKeyValue="1" ForeignTableName="Territories" MapTableName="EmployeeTerritories"
                  ProviderName="Northwind" runat="server" />
              </asp:Panel>
              <br />
            </div>
          </div>
        </div>
      </td>
    </tr>
  </table>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphLeft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphRight" runat="Server">
</asp:Content>
