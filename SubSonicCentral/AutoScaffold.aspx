<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AutoScaffold.aspx.cs" Inherits="AutoScaffold" ValidateRequest="false" EnableEventValidation="false" Theme="Default" MasterPageFile="~/res/MasterPage.master" Title="SubSonic AutoScaffold" %>
<asp:Content ID="Content2" ContentPlaceHolderID="cphLeft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphRight" Runat="Server">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="cphFull" Runat="Server">
<subsonic:Scaffold id="scaffold" runat="server" ScaffoldType="Auto" UseEmbeddedStyles="true"></subsonic:Scaffold>
</asp:Content>

