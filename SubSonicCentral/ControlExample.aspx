<%@ Page Language="C#" MasterPageFile="~/res/MasterPage.master" AutoEventWireup="true"
  CodeFile="ControlExample.aspx.cs" Inherits="ControlExample" Title="Control Examples" %>

<asp:Content ID="Content1" ContentPlaceHolderID="cphFull" runat="Server">
  <h1>
    Control Examples</h1>
  SubSonic has four controls that you can use to make your life a bit easier.
  <h3>
    DropDown</h3>
  The first is our DropDown control - all you need to do is add a TableName to the
  declaration and you have a nice DropDown that loads itself. Note that we don't query
  "SELECT *" - we only grab the two columns that we need for the selection:
  <br />
  <b>
    <br />
    Categories</b>
  <br />
  <subsonic:DropDown TableName="Categories" runat="server" ID="elDrop">
  </subsonic:DropDown>
  <h3>
    QuickTable</h3>
  Did you ever just need to show some data on a page? Did you ever wish there was
  a simple way to do this without using the GridView/DataGrid? Those controls are
  very nice, but setting them up with paging and sorting can take some time and extra
  code. Instead you can use our QuickTable:
  <subsonic:QuickTable ID="pTable" runat="server" TableName="Products" ProviderName="Northwind"
    PageSize="20" ColumnList="ProductID:ID,ProductName:Name,UnitPrice:Price, CategoryID:Category" />
  --ManyManyList Coming Soon---
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphLeft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphRight" runat="Server">
</asp:Content>
