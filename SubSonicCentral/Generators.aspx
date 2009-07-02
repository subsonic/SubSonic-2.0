<%@ Page Language="C#" MasterPageFile="~/res/MasterPage.master" AutoEventWireup="true"
	CodeFile="Generators.aspx.cs" Inherits="Generators" Theme="Default" Title="SubSonic Generators"
	ValidateRequest="false" %>

<%@ Register Src="inc/ClassGenerator.ascx" TagName="ClassGenerator" TagPrefix="uc1" %>
<%@ Register Src="inc/ScaffoldGenerator.ascx" TagName="ScaffoldGenerator" TagPrefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="cphFull" runat="Server">
	<div id="divWarning" runat="server" visible="false">
		<p>
			The template directory is missing or incorrect.<br />
			Please make sure that you have specified a valid to the templates in the <span style="color: #ff0000">
				templateDirectory</span> parameter in web.config.
		</p>
	</div>
	<uc1:ClassGenerator ID="ClassGenerator1" runat="server" />
	<uc2:ScaffoldGenerator ID="ScaffoldGenerator1" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphLeft" runat="Server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="cphRight" runat="Server">
</asp:Content>
