<%@ Page Language="C#" %>
<%@ Import namespace="SubSonic.Utilities"%>
<%@ Import Namespace="SubSonic" %>
<% 
	const string masterPage = "#MASTERPAGE#";
	const string tableName = "#TABLENAME#";
	const string className = "#CLASSNAME#";
	const string language = "#LANGUAGE#";

%>
<%
	if (!String.IsNullOrEmpty(masterPage))
	{
%>

[<]%@ Page Language="<%= language %>" Title="<%= className %> Scaffold" MasterPageFile="<%= masterPage %>" Theme="default" %[>]
[<]%@ Register Assembly="SubSonic" Namespace="SubSonic" TagPrefix="cc1" %[>]
[<]asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="server"[>]
	[<]div[>]
		[<]cc1:Scaffold ID="Scaffold1" runat="server" TableName="<%= tableName %>" GridViewSkinID="scaffold" EditTableItemCaptionCellCssClass="scaffoldEditLabel" ButtonCssClass="scaffoldButton" TextBoxCssClass="scaffoldTextBox"[>]
		[<]/cc1:Scaffold[>]
	[<]/div[>]
[<]/asp:Content[>]

<%
	}
	else
	{
%>

[<]%@ Page Language="<%= language %>" Title="<%=className%> Scaffold" %[>]
[<]%@ Register Assembly="SubSonic" Namespace="SubSonic" TagPrefix="cc1" %[>]
[<]!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"[>]
[<]html xmlns="http://www.w3.org/1999/xhtml" [>]
[<]head id="Head1" runat="server"[>]
[<]script language="javascript" type="text/javascript"[>]

	function CheckDelete()
	{ 
		  return confirm('Delete this record? This action cannot be undone...'); 
	}

	function imposeMaxLength(e, Object, MaxLen, rowIndex)
	{
		var keyCode = e.keyCode;
		var counter = document.getElementById('counter' + rowIndex);
		var charText = Object.value;
		var charCount = charText.length;
		var charRemain = MaxLen - charCount;
		counter.style.visibility = 'visible';
		if(keyCode == 8 || keyCode == 46)
		{
			if(charCount == MaxLen)
			{
				charRemain = 1;
			}
			else if(charCount == 0)
			{
				charRemain = MaxLen;
			}
			counter.innerHTML = charRemain;
			return true;
		}
		else
		{
			if(charRemain [>] 0)
			{
				counter.innerHTML = charRemain;
				return true;
			}
			else
			{
				Object.value = charText.substring(0, MaxLen);
				counter.innerHTML = '0';
				return false;
			}
		}
	}
[<]/script[>]
[<]/head[>]
[<]body[>]
	[<]form id="form1" runat="server"[>]    
		[<]div[>]
			[<]cc1:Scaffold ID="Scaffold1" runat="server" TableName="<%= tableName %>" GridViewSkinID="scaffold" EditTableItemCaptionCellCssClass="scaffoldEditLabel" ButtonCssClass="scaffoldButton" TextBoxCssClass="scaffoldTextBox"[>]
			[<]/cc1:Scaffold[>]
		[<]/div[>]
    [<]/form[>]
[<]/body[>]
[<]/html[>]
<%	    
	}
%> 

