<%@ Page Language="C#" %>
<%@ Import namespace="System.Data"%>
<%@ Import namespace="SubSonic.Utilities"%>
<%@ Import Namespace="SubSonic" %>
<% 
	const string masterPage = "#MASTERPAGE#";
	const string tableName = "#TABLENAME#";
	const string className = "#CLASSNAME#";
	const string generatedClassName = "#PAGEFILE#";
	const string language = "#LANGUAGE#";
	const string fileExtension = "#LANGEXTENSION#";
	const string providerName = "#PROVIDER#";
	DataProvider provider = DataService.GetInstance(providerName);
	TableSchema.Table tbl = DataService.GetSchema(tableName, provider.Name, TableType.Table);

%>

<%
	if (String.IsNullOrEmpty(masterPage))
	{
%>
[<]%@ Page Language="C#" Title="<%=className%> Scaffold" CodeFile="<%=generatedClassName%>.aspx<%=fileExtension%>" Inherits="<%=provider.GeneratedNamespace%>.<%=generatedClassName%>" %[>]
[<]!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"[>]
[<]html xmlns="http://www.w3.org/1999/xhtml" [>]
[<]head id="Head1" runat="server"[>]
[<]title[>]<%=className%>[<]/title[>]
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
	[<]form id="elForm" runat="server"[>]
	
	<%
	}
	else
	{
%>
[<]%@ Page Language="<%=language%>" Title="<%=className%> Scaffold" CodeFile="<%=generatedClassName%>.aspx<%=fileExtension%>" Inherits="<%=provider.GeneratedNamespace%>.<%=generatedClassName%>" MasterPageFile="<%= masterPage %>" Theme="default" %[>]
[<]asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="server"[>]
<%
	}
%>
	
	[<]h2[>]<%= className %>[<]/h2[>]
	[<]asp:Panel id="pnlGrid" runat="server"[>]
    [<]asp:GridView 
    ID="GridView1"
    runat="server" 
    AllowPaging="True" 
    AllowSorting="True"
    AutoGenerateColumns="False" 
    OnDataBound="GridView1_DataBound" 
    OnSorting="GridView1_Sorting"
	OnPageIndexChanging="GridView1_PageIndexChanging"
    DataKeyNames="<%= tbl.PrimaryKey.ColumnName %>" 
    PageSize="50"
    [>]
        [<]Columns[>]
	            [<]asp:HyperLinkField Text="Edit" DataNavigateUrlFields="<%= tbl.PrimaryKey.ColumnName %>" DataNavigateUrlFormatString="<%= generatedClassName %>.aspx?id={0}" /[>]
	            <% 
					foreach (TableSchema.TableColumn col in tbl.Columns)
					{
					    if(!col.IsPrimaryKey)
					    {
%>
	            [<]asp:BoundField DataField="<%=col.ColumnName%>" HeaderText="<%=col.DisplayName%>" SortExpression="<%=col.ColumnName%>"[>][<]/asp:BoundField[>]
	            <%
					    }
					}
%>
        [<]/Columns[>]
        [<]EmptyDataTemplate[>]
            No <%= tbl.ClassNamePlural %> 
        [<]/EmptyDataTemplate[>]
        [<]PagerTemplate[>]
        
            [<]div style="border-top:1px solid #666666"[>]
            [<]br /[>]
           [<]asp:Button ID="btnFirst" runat="server" CssClass="scaffoldButton" Text="[<][<] First" CommandArgument="First" CommandName="Page"/[>]
            [<]asp:Button ID="btnPrev" runat="server" CssClass="scaffoldButton" Text="[<] Previous" CommandArgument="Prev" CommandName="Page"/[>]

                Page
                [<]asp:DropDownList ID="ddlPages" runat="server" CssClass="scaffoldEditItem" AutoPostBack="True" OnSelectedIndexChanged="ddlPages_SelectedIndexChanged"[>]
                [<]/asp:DropDownList[>] of [<]asp:Label ID="lblPageCount" runat="server"[>][<]/asp:Label[>]

            [<]asp:Button ID="btnNext" runat="server" CssClass="scaffoldButton" Text="Next [>]" CommandArgument="Next" CommandName="Page"/[>]
            [<]asp:Button ID="btnLast" runat="server" CssClass="scaffoldButton" Text="Last [>][>]" CommandArgument="Last" CommandName="Page"/[>]
        [<]/PagerTemplate[>]
    [<]/asp:GridView[>]
	[<]a href="<%= generatedClassName %>.aspx?id=0"[>]Add New...[<]/a[>]
	[<]/asp:Panel[>]
	
	[<]asp:panel id="pnlEdit" Runat="server"[>]
	[<]asp:Label ID="lblResult" runat="server"[>][<]/asp:Label[>]	
	
	[<]table class="scaffoldEditTable" cellpadding="5" cellspacing="0" Width="600px"[>]
			[<]tr[>]
			[<]td class="scaffoldEditItemCaption"[>]<%= tbl.PrimaryKey.DisplayName %>[<]/td[>]
			[<]td class="scaffoldEditItem"[>][<]asp:Label id="lblID" runat="server" /[>][<]/td[>]
		[<]/tr[>]
	<%
		foreach (TableSchema.TableColumn col in tbl.Columns)
		{
		    if(!col.IsPrimaryKey)
		    {
%>
		[<]tr[>]
			[<]td class="scaffoldEditItemCaption"[>]<%=col.DisplayName%>[<]/td[>]
			<%
		        string controlID = "ctrl" + col.PropertyName;
		        string displayPrefix = String.Empty;
		        string controlType;
		        string controlProperties = String.Empty;

		        bool isTextBox = false;

		        if(col.IsForeignKey)
		        {
		            controlType = "DropDownList";
		        }
		        else
		        {
		            switch(col.DataType)
		            {
		                case DbType.Binary:
		                    controlType = String.Empty;
		                    break;
		                case DbType.Guid:
		                case DbType.AnsiString:
		                case DbType.String:
		                case DbType.StringFixedLength:
		                case DbType.Xml:
		                case DbType.Object:
		                case DbType.AnsiStringFixedLength:
		                    if(Utility.MatchesOne(col.ColumnName, ReservedColumnName.CREATED_BY, ReservedColumnName.MODIFIED_BY))
		                    {
		                        controlType = "Label";
		                    }
		                    else
		                    {
		                        isTextBox = true;
		                        controlType = "TextBox";
		                        if(Utility.GetEffectiveMaxLength(col) > 250)
		                        {
		                            controlProperties += "TextMode=\"MultiLine\" ";
		                            controlProperties += "Height=\"100px\" ";
		                            controlProperties += "Width=\"500px\" ";
		                        }
		                    }
		                    break;
		                case DbType.Date:
		                case DbType.Time:
		                case DbType.DateTime:
		                    if(Utility.MatchesOne(col.ColumnName, ReservedColumnName.MODIFIED_ON, ReservedColumnName.CREATED_ON))
		                    {
		                        controlType = "Label";
		                    }
		                    else
		                    {
		                        controlType = "CalendarControl";
		                    }
		                    break;
		                case DbType.Int16:
		                case DbType.Int32:
		                case DbType.UInt16:
		                case DbType.Int64:
		                case DbType.UInt32:
		                case DbType.UInt64:
		                case DbType.VarNumeric:
		                case DbType.Single:
		                case DbType.Currency:
		                case DbType.Decimal:
		                case DbType.Double:
                        case DbType.Byte:
		                    isTextBox = true;
		                    controlType = "TextBox";
		                    controlProperties += "Width=\"50px\" ";
		                    if(col.DataType == DbType.Currency)
		                    {
		                        displayPrefix = "$";
		                    }
		                    break;
		                case DbType.Boolean:
		                    bool isChecked = false;
		                    if(Utility.IsMatch(col.ColumnName, ReservedColumnName.IS_ACTIVE))
		                    {
		                        isChecked = true;
		                    }
		                    controlType = "CheckBox";
		                    controlProperties += "Checked=\"" + isChecked + "\" ";
		                    break;
		                default:
		                    isTextBox = true;
		                    controlType = "TextBox";
		                    break;
		            }
		        }
				int maxLength = Utility.GetEffectiveMaxLength(col);
				if (isTextBox && maxLength > 0)
		        {
					controlProperties += "MaxLength=\"" + maxLength + "\" ";
		        }


		        string controlMarkup = String.Empty;
		        if(!String.IsNullOrEmpty(controlType))
		        {
					string wrapper = "asp";
					if (controlType == "CalendarControl")
						wrapper = "subsonic";
					controlMarkup = displayPrefix + "<" + wrapper + ":" + controlType + " ID=\"" + controlID + "\" runat=\"server\" " + controlProperties + "></" + wrapper + ":" + controlType + ">";
		        }
%>
		[<]td class="scaffoldEditItem"[>]<%=controlMarkup%>[<]/td[>]
		[<]/tr[>]
		<%
		    }
		}
%>

		[<]tr[>]
			[<]td colspan="2" align="left"[>]
			[<]asp:Button id="btnSave" CssClass="scaffoldButton" runat="server" Text="Save" OnClick="btnSave_Click"[>][<]/asp:Button[>]&nbsp;
			[<]input type="button" onclick="location.href='<%= generatedClassName %>.aspx'" class="scaffoldButton" value="Return" /[>]
			[<]asp:Button id="btnDelete" CssClass="scaffoldButton" runat="server" CausesValidation="False" Text="Delete" OnClick="btnDelete_Click"[>][<]/asp:Button[>][<]/td[>]
		[<]/tr[>]
	[<]/table[>]
	[<]/asp:panel[>]

<%if(String.IsNullOrEmpty(masterPage))
{
%>
[<]/form[>]
[<]/body[>]
[<]/html[>]
<%
}
else 
{
%>
[<]/asp:Content[>]
<%
}
%>