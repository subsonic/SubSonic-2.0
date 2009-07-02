<%@ Page Language="C#" MasterPageFile="~/res/MasterPage.master" AutoEventWireup="true" CodeFile="Configuration.aspx.cs" Inherits="Configuration" Theme="Default" Title="SubSonic Configuration Builder" %>
<%@ Register Src="inc/ConfigBuilder.ascx" TagName="ConfigBuilder" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphFull" Runat="Server">
<uc1:ConfigBuilder ID="ConfigBuilder1" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphLeft" Runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphRight" Runat="Server">
</asp:Content>