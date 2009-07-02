<%@ Page Language="C#" %>
<%@ Import namespace="System.Data"%>
<%@ Import namespace="SubSonic.Utilities"%>
<%@ Import Namespace="SubSonic" %>
<%
    //The data we need
    const string providerName = "#PROVIDER#";
    const string tableName = "#TABLENAME#";
    const string generatedClassName = "#PAGEFILE#";
    TableSchema.Table tbl = DataService.GetSchema(tableName, providerName, TableType.Table);
    DataProvider provider = DataService.Providers[providerName];

    //The main vars we need
    string className = tbl.ClassName;
  const bool showGenerationInfo = false;
  
%>

<% if(showGenerationInfo)
   { %>
 'Generated on <%=DateTime.Now.ToString() %> by <%=Environment.UserName %>
<% }  %>
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports SubSonic
Imports SubSonic.Utilities
Imports <%=provider.GeneratedNamespace%>

Namespace <%=provider.GeneratedNamespace%>

	Public Partial Class <%=generatedClassName%> 
	Inherits System.Web.UI.Page 
	
		Private isAdd As Boolean = False
		Private Const SORT_DIRECTION As String = "SORT_DIRECTION"
		Private Const ORDER_BY As String = "ORDER_BY"
    
		Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
			If Request.QueryString("id") <> Nothing Then
				Dim id As String = Utility.GetParameter("id")
				If Not String.IsNullOrEmpty(id) AndAlso id <> "0" Then
					If Not Page.IsPostBack Then
						LoadEditor(id)
					End If
				Else
					'it's an add, show the editor
					isAdd = True
					ToggleEditor(True)
					LoadDrops()
					btnDelete.Visible = False
				End If
			Else
				ToggleEditor(False)
				If Not Page.IsPostBack Then
					BindGrid(String.Empty)
				End If
			End If
		End Sub


		''' <summary>
		''' Loads the editor with data
		''' </summary>
		''' <param name="id"></param>
		Sub LoadEditor(ByVal id As String)
			ToggleEditor(True)
			LoadDrops()
			If Not [String].IsNullOrEmpty(id) AndAlso id <> "0" Then
				lblID.Text = id.ToString()
	            
				'pull the record
				Dim item As <%=className%>  = New <%=className%>(id)

				'bind the page 
					<%
						foreach (TableSchema.TableColumn col in tbl.Columns)
						{
							if (!col.IsPrimaryKey)
							{
								bool toString = true;
								string controlAssignment = null;
								bool isNullableType = (col.IsNullable && Utility.IsNullableDbType(col.DataType));

								if (col.IsForeignKey)
								{
									controlAssignment = ControlValueProperty.DROP_DOWN_LIST;
								}
								else if(Utility.MatchesOne(col.ColumnName, ReservedColumnName.CREATED_ON, ReservedColumnName.MODIFIED_ON))
								{
									controlAssignment = ControlValueProperty.LABEL;
								}
								else
								{
									switch (col.DataType)
									{
										case DbType.Binary:
											break;
										case DbType.DateTime:
											toString = false;
											controlAssignment = ControlValueProperty.CALENDAR;
											break;
										case DbType.Boolean:
											toString = false;
											controlAssignment = ControlValueProperty.CHECK_BOX;
											break;
										case DbType.Currency:
										case DbType.VarNumeric:
											controlAssignment = ControlValueProperty.TEXT_BOX;
											break;
										case DbType.Int16:
										case DbType.Int32:
										case DbType.UInt16:
										case DbType.Int64:
										case DbType.UInt32:
										case DbType.UInt64:
										case DbType.Single:
										case DbType.Decimal:
										case DbType.Double:
                                        case DbType.Byte:
											controlAssignment = ControlValueProperty.TEXT_BOX;
											break;
										case DbType.AnsiString:
										case DbType.AnsiStringFixedLength:	
										case DbType.String:
											toString = false;
											controlAssignment = ControlValueProperty.TEXT_BOX;
											break;
										default:
											controlAssignment = ControlValueProperty.TEXT_BOX;
											break;
									}
								}
								string propName = col.PropertyName;
								string controlID = "ctrl" + propName;

								if (!String.IsNullOrEmpty(controlAssignment))
								{
									if (isNullableType)
									{%>
						If item.<%=propName%>.HasValue Then
						<%
						if (toString)
						{%>
							<%=controlID%>.<%=controlAssignment%> = item.<%=propName%>.Value.ToString()
						<%
						}
						else
						{%>
							<%=controlID%>.<%=controlAssignment%> = item.<%=propName%>.Value
						<%
						}%>
						End If
					<%
						}

						else
						{
							if (toString)
							{%>
							<%=controlID%>.<%=controlAssignment%> = item.<%=propName%>.ToString()
						<%
						}
						else
						{%>
							<%=controlID%>.<%=controlAssignment%> = item.<%=propName%>
						<%
						}
					}
				}
			}
		}
%>			
  
				'set the delete confirmation
				btnDelete.Attributes.Add("onclick", "return CheckDelete();")
			End If
		End Sub

		' <summary>
		' Loads the DropDownLists
		' </summary>
		Private Sub LoadDrops() 
			'load the listboxes
			<%
				foreach (TableSchema.TableColumn col in tbl.Columns)
				{
					string controlName = "ctrl" + col.PropertyName;

					if (!col.IsPrimaryKey)
					{
						if(col.IsForeignKey)
						{
							TableSchema.Table FKTable = DataService.GetForeignKeyTable(col, tbl);
							if(FKTable != null)
							{
			%>
								Dim qry<%= controlName %> As Query = <%= FKTable.ClassName %>.CreateQuery() 
								qry<%= controlName %>.OrderBy = OrderBy.Asc("<%= FKTable.Columns[1].ColumnName %>")
								Utility.LoadDropDown(<%= controlName %>, qry<%= controlName %>.ExecuteReader(), True)
			<% 
								if(col.IsNullable)
								{
			%>						<%= controlName %>.Items.Insert(0, new ListItem("(Not Specified)", String.Empty))							
			<%
								}
							}
						}
					}
				} 
			 %>
		End Sub
	    
		''' <summary>
		''' Shows/Hides the Grid and Editor panels
		''' </summary>
		''' <param name="showIt"></param>
		Sub ToggleEditor(ByVal showIt As Boolean)
			pnlEdit.Visible = showIt
			pnlGrid.Visible = Not showIt
		End Sub

		Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As EventArgs)
			LoadEditor("0")
		End Sub
		
		Protected Sub btnDelete_Click(ByVal sender As Object, ByVal e As EventArgs)
			<%= className %>.Delete(Utility.GetParameter("id"))

			'redirect
			Response.Redirect(Request.CurrentExecutionFilePath)
		End Sub
		
		Protected Sub btnSave_Click(ByVal sender As Object, ByVal e As EventArgs)
			Dim id As String = Utility.GetParameter("id")
			'bool haveError = false;
			Try
				BindAndSave(id)
				lblResult.Text = "[<]span style=""font-weight:bold; color:#22bb22""[>]<%= className %> saved.[<]/span[>]"
			Catch x As Exception
				'haveError = true;
				lblResult.Text = "[<]span style=""font-weight:bold; color:#990000""[>]<%= className %> not saved:[<]/span[>] " + x.Message
			End Try

			'if(!haveError)
			'  Response.Redirect(Request.CurrentExecutionFilePath);
		End Sub


		'<summary>
		'Binds and saves the data
		'</summary>
		'<param name="id"></param>
		Private Sub BindAndSave(ByVal id As String) 
		
			Dim item As <%= className %>
			If Not String.IsNullOrEmpty(id) AndAlso id <> "0" Then 
				'it's an edit
				item = New <%= className %>(id)
			Else
				'add
				item = New <%= className %>()
			End If  
	      
			<%
			    
			
				foreach (TableSchema.TableColumn col in tbl.Columns)
				{
					if (!col.IsPrimaryKey && col.DataType != DbType.Binary)
					{
						string controlID = "ctrl" + col.PropertyName;
						string propName = col.PropertyName;
						string converterType;

						switch (col.DataType)
						{
							case DbType.Currency:
							case DbType.VarNumeric:
								converterType = "Decimal";
								break;
							case DbType.AnsiString:
							case DbType.AnsiStringFixedLength:
								converterType = "String";
								break;
							default:
								converterType = col.DataType.ToString();
								break;
						}
						
						%>
						Dim val<%= controlID %> As Object = Utility.GetDefaultControlValue(<%= className %>.Schema.GetColumn("<%= col.ColumnName %>"), <%= controlID %>, isAdd, False)
						<%

						if (col.IsNullable)
						{
						%>
						If val<%= controlID %> Is Nothing Then
							item.<%= propName %> = Nothing
						Else
						<%
						}

						if (col.DataType != DbType.Guid)
						{
						%>
							item.<%= propName %> = Convert.To<%= converterType %>(val<%= controlID %>)
						<%
						}
						else
						{
						%>
							item.<%= propName %> = New <%= converterType %>(val<%= controlID %>.ToString())
						<%
						}

						if (col.IsNullable)
						{
						%>
						End If
						<%
						}
				}
			}
	%>    
			'bind it

			item.Save(User.Identity.Name)
		End Sub

		''' <summary>
		''' Binds the GridView
		''' </summary>
		Private Sub BindGrid(ByVal orderBy As String)

			Dim tblSchema As TableSchema.Table = DataService.GetTableSchema("<%= tbl.Name %>", "<%= tbl.Provider.Name %>")
			If Not tblSchema.PrimaryKey Is Nothing Then

				Dim query As New Query(tblSchema)

				Dim sortColumn As String = Nothing
				If Not [String].IsNullOrEmpty(orderBy) Then
					sortColumn = orderBy
		ElseIf ViewState(ORDER_BY) Is Nothing Then
					sortColumn = DirectCast(ViewState(ORDER_BY), String)
				End If

				Dim colIndex As Integer = -1

				If Not [String].IsNullOrEmpty(sortColumn) Then
					ViewState.Add(ORDER_BY, sortColumn)
					Dim col As TableSchema.TableColumn = tblSchema.GetColumn(sortColumn)
					If col Is Nothing Then
						Dim iCount As Integer = 0
						While iCount < tblSchema.Columns.Count
							Dim fkCol As TableSchema.TableColumn = tblSchema.Columns(iCount)
							If fkCol.IsForeignKey AndAlso Not [String].IsNullOrEmpty(fkCol.ForeignKeyTableName) Then
								Dim fkTbl As TableSchema.Table = DataService.GetSchema(fkCol.ForeignKeyTableName, tblSchema.Provider.Name, TableType.Table)
								If Not fkTbl is Nothing Then
									col = fkTbl.Columns(1)
									colIndex = iCount
									Exit While
								End If
							End If
							iCount = iCount + 1
						End While
					End If
					If Not col Is Nothing AndAlso col.MaxLength < 2048 Then
						If ViewState(SORT_DIRECTION) Is Nothing OrElse (DirectCast(ViewState(SORT_DIRECTION), String)) = SqlFragment.ASC Then
							If colIndex > -1 Then
								query.OrderBy = SubSonic.OrderBy.Asc(col, String.Concat(SqlFragment.JOIN_PREFIX, colIndex))
							Else
								query.OrderBy = SubSonic.OrderBy.Asc(col)
							End If
							ViewState(SORT_DIRECTION) = SqlFragment.ASC
						Else
							If colIndex > -1 Then
								query.OrderBy = SubSonic.OrderBy.Desc(col, String.Concat(SqlFragment.JOIN_PREFIX, colIndex))
							Else
								query.OrderBy = SubSonic.OrderBy.Desc(col)
							End If
							ViewState(SORT_DIRECTION) = SqlFragment.DESC
						End If
					End If
				End If


				Dim dt As DataTable = query.ExecuteJoinedDataSet().Tables(0)
				GridView1.DataSource = dt

				Dim i As Integer = 1
				While i < tblSchema.Columns.Count
					Dim field As BoundField = DirectCast(GridView1.Columns(i), BoundField)
					field.DataField = dt.Columns(i).ColumnName
					field.SortExpression = dt.Columns(i).ColumnName
					field.HtmlEncode = False
					If tblSchema.Columns(i).IsForeignKey Then
						Dim schema As TableSchema.Table
						If tblSchema.Columns(i).ForeignKeyTableName Is Nothing Then
							schema = DataService.GetForeignKeyTable(tblSchema.Columns(i), tblSchema)
						Else
							schema = DataService.GetSchema(tblSchema.Columns(i).ForeignKeyTableName, tblSchema.Provider.Name, TableType.Table)
						End If
						If Not schema Is Nothing Then
							field.HeaderText = schema.DisplayName
						End If
					Else
						field.HeaderText = tblSchema.Columns(i).DisplayName
					End If
					i = i + 1
				End While

				GridView1.DataBind()
			End If
		End Sub


		Protected Sub GridView1_PageIndexChanging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
			GridView1.PageIndex = e.NewPageIndex
			BindGrid(String.Empty)
		End Sub

		Protected Sub GridView1_DataBound(ByVal sender As Object, ByVal e As EventArgs)
			Dim gvrPager As GridViewRow = GridView1.BottomPagerRow
			If gvrPager Is Nothing Then
				Return
			End If
			' get your controls from the gridview
			Dim ddlPages As DropDownList = DirectCast(gvrPager.Cells(0).FindControl("ddlPages"), DropDownList)
			Dim lblPageCount As Label = DirectCast(gvrPager.Cells(0).FindControl("lblPageCount"), Label)

			If Not ddlPages Is Nothing Then
				' populate pager
				Dim i As Integer = 0
				While i < GridView1.PageCount
					Dim intPageNumber As Integer = i + 1
					Dim lstItem As New ListItem(intPageNumber.ToString())
					If i = GridView1.PageIndex Then
						lstItem.Selected = True
					End If
					ddlPages.Items.Add(lstItem)
					i = i + 1
				End While
			End If

			Dim itemCount As Integer = 0

			' populate page count
			If Not lblPageCount Is Nothing Then

				'pull the datasource
				Dim ds As DataSet = TryCast(GridView1.DataSource, DataSet)
				If Not ds Is Nothing Then
					itemCount = ds.Tables(0).Rows.Count
				End If
				Dim pageCount As String = "[<]b[>]" + GridView1.PageCount.ToString() + "[<]/b[>] (" + itemCount.ToString() + " Items)"

				lblPageCount.Text = pageCount
			End If

			Dim btnPrev As Button = DirectCast(gvrPager.Cells(0).FindControl("btnPrev"), Button)
			Dim btnNext As Button = DirectCast(gvrPager.Cells(0).FindControl("btnNext"), Button)
			Dim btnFirst As Button = DirectCast(gvrPager.Cells(0).FindControl("btnFirst"), Button)
			Dim btnLast As Button = DirectCast(gvrPager.Cells(0).FindControl("btnLast"), Button)

			'now figure out what page we're on
			If GridView1.PageIndex = 0 Then
				btnPrev.Enabled = False
				btnFirst.Enabled = False
		ElseIf GridView1.PageIndex + 1 = GridView1.PageCount Then
				btnLast.Enabled = False
				btnNext.Enabled = False
			Else
				btnLast.Enabled = True
				btnNext.Enabled = True
				btnPrev.Enabled = True
				btnFirst.Enabled = True
			End If
		End Sub

		Protected Sub ddlPages_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
			Dim gvrPager As GridViewRow = GridView1.BottomPagerRow
			Dim ddlPages As DropDownList = DirectCast(gvrPager.Cells(0).FindControl("ddlPages"), DropDownList)

			GridView1.PageIndex = ddlPages.SelectedIndex

			' a method to populate your grid
			BindGrid(String.Empty)
		End Sub


		Protected Sub GridView1_Sorting(ByVal sender As Object, ByVal e As GridViewSortEventArgs)
			Dim columnName As String = e.SortExpression
			'rebind the grid
			If ViewState(SORT_DIRECTION) Is Nothing OrElse ViewState(SORT_DIRECTION).ToString() = SqlFragment.ASC Then
				ViewState(SORT_DIRECTION) = SqlFragment.DESC
			Else
				ViewState(SORT_DIRECTION) = SqlFragment.ASC
			End If
			BindGrid(columnName)
		End Sub

		Function GetSortDirection(ByVal sortBy As String) As String
			Dim sortDir As String = " ASC"
			If ViewState("sortBy") Is Nothing Then
				Dim sortedBy As String = ViewState("sortBy").ToString()

				If sortedBy = sortBy Then
					'the direction should be desc
					sortDir = " DESC"

					'reset the sorter to null
					ViewState("sortBy") = Nothing
				Else
					'this is the first sort for this row
					'put it to the ViewState
					ViewState("sortBy") = sortBy
				End If
			Else
				'it's null, so this is the first sort
				ViewState("sortBy") = sortBy
			End If
			Return sortDir
		End Function
	End Class
End Namespace
