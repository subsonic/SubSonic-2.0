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
 //Generated on <%=DateTime.Now.ToString() %> by <%=Environment.UserName %>
<% }  %>

using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SubSonic;
using SubSonic.Utilities;
using <%=provider.GeneratedNamespace%>;

namespace <%=provider.GeneratedNamespace%>
{

	public partial class <%=generatedClassName%> : System.Web.UI.Page 
	{
		private bool isAdd = false;
		private const string SORT_DIRECTION = "SORT_DIRECTION";
        private const string ORDER_BY = "ORDER_BY";

    
		protected void Page_Load(object sender, EventArgs e) 
		{
			if (Request.QueryString["id"] != null)
			{
				string id = Utility.GetParameter("id");
				if (!String.IsNullOrEmpty(id) && id != "0")
				{
					if (!Page.IsPostBack)
					{
						LoadEditor(id);
					}
				} 
				else 
				{
					//it's an add, show the editor
					isAdd = true;
					ToggleEditor(true);
					LoadDrops();
					btnDelete.Visible = false;
				}
			} 
			else 
			{
				ToggleEditor(false);
				if(!Page.IsPostBack)
				{
					BindGrid(String.Empty);
				}
			}
		}

		/// <summary>
		/// Loads the editor with data
		/// </summary>
		/// <param name="id"></param>
		void LoadEditor(string id) 
		{
			ToggleEditor(true);
			LoadDrops();
			if (!String.IsNullOrEmpty(id) && id != "0")
			{
				lblID.Text = id.ToString();
	            
				//pull the record
				<%=className%> item = new <%=className%>(id);

				//bind the page 
					<%
						foreach (TableSchema.TableColumn col in tbl.Columns)
						{
							if (!col.IsPrimaryKey)
							{
								bool toString = true;
								string controlAssignment = null;
								bool isNullableType = (col.IsNullable && Utility.IsNullableDbType(col.DataType));

								if (col.IsForeignKey)
									controlAssignment = ControlValueProperty.DROP_DOWN_LIST;
								else if (Utility.MatchesOne(col.ColumnName, ReservedColumnName.CREATED_ON, ReservedColumnName.MODIFIED_ON))
									controlAssignment = ControlValueProperty.LABEL;
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
						if(item.<%=propName%>.HasValue)
						{
						<%
						if (toString)
						{%>
							<%=controlID%>.<%=controlAssignment%> = item.<%=propName%>.Value.ToString();
						<%
						}
						else
						{%>
							<%=controlID%>.<%=controlAssignment%> = item.<%=propName%>.Value;
						<%
						}%>
						}
					<%
						}

						else
						{
							if (toString)
							{%>
							<%=controlID%>.<%=controlAssignment%> = item.<%=propName%>.ToString();
						<%
						}
						else
						{%>
							<%=controlID%>.<%=controlAssignment%> = item.<%=propName%>;
						<%
						}
					}
				}
			}
		}
%>			
  
				//set the delete confirmation
				btnDelete.Attributes.Add("onclick", "return CheckDelete();");
			}
		}

		/// <summary>
		/// Loads the DropDownLists
		/// </summary>
		void LoadDrops() 
		{
			//load the listboxes
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
								Query qry<%= controlName %> = <%= FKTable.ClassName %>.CreateQuery(); 
								qry<%= controlName %>.OrderBy = OrderBy.Asc("<%= FKTable.Columns[1].ColumnName %>");
								Utility.LoadDropDown(<%= controlName %>, qry<%= controlName %>.ExecuteReader(), true);
			<% 
								if(col.IsNullable)
								{
			%>						<%= controlName %>.Items.Insert(0, new ListItem("(Not Specified)", String.Empty));							
			<%
								}
							}
						}
					}
				} 
			 %>
		}
	    
		/// <summary>
		/// Shows/Hides the Grid and Editor panels
		/// </summary>
		/// <param name="showIt"></param>
		void ToggleEditor(bool showIt) 
		{
			pnlEdit.Visible = showIt;
			pnlGrid.Visible = !showIt;
		}

		protected void btnAdd_Click(object sender, EventArgs e) 
		{
			LoadEditor("0");
		}
	    
		protected void btnDelete_Click(object sender, EventArgs e) 
		{
			<%= className %>.Delete(Utility.GetParameter("id"));

			//redirect
			Response.Redirect(Request.CurrentExecutionFilePath);
		}
		protected void btnSave_Click(object sender, EventArgs e) 
		{
			string id = Utility.GetParameter("id");
			//bool haveError = false;
			try 
			{
				BindAndSave(id);
				lblResult.Text = "[<]span style=\"font-weight:bold; color:#22bb22\"[>]<%= className %> saved.[<]/span[>]";
			}
			 catch (Exception x) 
			 {
				//haveError = true;
				lblResult.Text = "[<]span style=\"font-weight:bold; color:#990000\"[>]<%= className %> not saved:[<]/span[>] " + x.Message;
			}

			//if(!haveError)
			//  Response.Redirect(Request.CurrentExecutionFilePath);
		}

		/// <summary>
		/// Binds and saves the data
		/// </summary>
		/// <param name="id"></param>
		void BindAndSave(string id) 
		{
	        
			<%= className %> item;
			if (!String.IsNullOrEmpty(id) && id != "0") 
			{
				//it's an edit
				item = new <%= className %>(id);
			}
			else 
			{
				//add
				item = new <%= className %>();
			}  
	      
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
						object val<%= controlID %> = Utility.GetDefaultControlValue(<%= className %>.Schema.GetColumn("<%= col.ColumnName %>"), <%= controlID %>, isAdd, false);
						<%

						if (col.IsNullable)
						{
						%>
						if(val<%= controlID %> == null)
						{
							item.<%= propName %> = null;
						}
						else
						{
						<%
						}

						if (col.DataType != DbType.Guid)
						{
						%>
							item.<%= propName %> = Convert.To<%= converterType %>(val<%= controlID %>);
						<%
						}
						else
						{
						%>
							item.<%= propName %> = new <%= converterType %>(val<%= controlID %>.ToString());
						<%
						}

						if (col.IsNullable)
						{
						%>
						}
						<%
						}
				}
			}
	%>    
			//bind it

			item.Save(User.Identity.Name);
		}

		/// <summary>
		/// Binds the GridView
		/// </summary>
        private void BindGrid(string orderBy)
        {
		    TableSchema.Table tblSchema = DataService.GetTableSchema("<%= tbl.Name %>", "<%= tbl.Provider.Name %>");
            if (tblSchema.PrimaryKey != null)
            {
                Query query = new Query(tblSchema);

                string sortColumn = null;
                if (!String.IsNullOrEmpty(orderBy))
                {
                    sortColumn = orderBy;
                }
                else if (ViewState[ORDER_BY] != null)
                {
                    sortColumn = (string)ViewState[ORDER_BY];
                }

                int colIndex = -1;

                if (!String.IsNullOrEmpty(sortColumn))
                {
                    ViewState.Add(ORDER_BY, sortColumn);
                    TableSchema.TableColumn col = tblSchema.GetColumn(sortColumn);
                    if (col == null)
                    {
                        for (int i = 0; i < tblSchema.Columns.Count; i++)
                        {
                            TableSchema.TableColumn fkCol = tblSchema.Columns[i];
                            if (fkCol.IsForeignKey && !String.IsNullOrEmpty(fkCol.ForeignKeyTableName))
                            {
                                TableSchema.Table fkTbl = DataService.GetSchema(fkCol.ForeignKeyTableName, tblSchema.Provider.Name, TableType.Table);
                                if (fkTbl != null)
                                {
                                    col = fkTbl.Columns[1];
                                    colIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    if (col != null && col.MaxLength < 2048)
                    {
                        if (ViewState[SORT_DIRECTION] == null || ((string)ViewState[SORT_DIRECTION]) == SqlFragment.ASC)
                        {
                            if (colIndex > -1)
                            {
                                query.OrderBy = OrderBy.Asc(col, SqlFragment.JOIN_PREFIX + colIndex);
                            }
                            else
                            {
                                query.OrderBy = OrderBy.Asc(col);
                            }
                            ViewState[SORT_DIRECTION] = SqlFragment.ASC;
                        }
                        else
                        {
                            if (colIndex > -1)
                            {
                                query.OrderBy = OrderBy.Desc(col, SqlFragment.JOIN_PREFIX + colIndex);
                            }
                            else
                            {
                                query.OrderBy = OrderBy.Desc(col);
                            }
                            ViewState[SORT_DIRECTION] = SqlFragment.DESC;
                        }
                    }
                }


                DataTable dt = query.ExecuteJoinedDataSet().Tables[0];
                GridView1.DataSource = dt;

                for (int i = 1; i < tblSchema.Columns.Count; i++)
                {
                    BoundField field = (BoundField)GridView1.Columns[i];
                    field.DataField = dt.Columns[i].ColumnName;
                    field.SortExpression = dt.Columns[i].ColumnName;
                    field.HtmlEncode = false;
                    if (tblSchema.Columns[i].IsForeignKey)
                    {
                        TableSchema.Table schema;
                        if (tblSchema.Columns[i].ForeignKeyTableName == null)
                        {
                            schema = DataService.GetForeignKeyTable(tblSchema.Columns[i], tblSchema);
                        }
                        else
                        {
                            schema = DataService.GetSchema(tblSchema.Columns[i].ForeignKeyTableName, tblSchema.Provider.Name, TableType.Table);
                        }
                        if (schema != null)
                        {
                            field.HeaderText = schema.DisplayName;
                        }
                    }
                    else
                    {
                        field.HeaderText = tblSchema.Columns[i].DisplayName;
                    }
                }

                GridView1.DataBind();
            }
        }

		protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
		{
			GridView1.PageIndex = e.NewPageIndex;
			BindGrid(String.Empty);
		}
	    
		protected void GridView1_DataBound(Object sender, EventArgs e)
		{
			GridViewRow gvrPager = GridView1.BottomPagerRow;
			if (gvrPager == null)
			{
				return;
			}
			// get your controls from the gridview
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlPages");
			Label lblPageCount = (Label)gvrPager.Cells[0].FindControl("lblPageCount");

			if (ddlPages != null)
			{
				// populate pager
				for (int i = 0; i < GridView1.PageCount; i++) 
				{
					int intPageNumber = i + 1;
					ListItem lstItem = new ListItem(intPageNumber.ToString());
					if (i == GridView1.PageIndex)
					{
						lstItem.Selected = true;
					}
					ddlPages.Items.Add(lstItem);
				}
			}

			int itemCount = 0;

			// populate page count
			if (lblPageCount != null) 
			{

				//pull the datasource
				DataSet ds = GridView1.DataSource as DataSet;
				if (ds != null)
				{
					itemCount = ds.Tables[0].Rows.Count;
				}
				string pageCount = "[<]b[>]" + GridView1.PageCount.ToString() + "[<]/b[>] (" + itemCount.ToString() + " Items)";
				lblPageCount.Text = pageCount;

			}

			Button btnPrev = (Button)gvrPager.Cells[0].FindControl("btnPrev");
			Button btnNext = (Button)gvrPager.Cells[0].FindControl("btnNext");
			Button btnFirst = (Button)gvrPager.Cells[0].FindControl("btnFirst");
			Button btnLast = (Button)gvrPager.Cells[0].FindControl("btnLast");

			//now figure out what page we're on
			if (GridView1.PageIndex == 0)
			{
				btnPrev.Enabled = false;
				btnFirst.Enabled = false;
			}
			else if (GridView1.PageIndex + 1 == GridView1.PageCount)
			{
				btnLast.Enabled = false;
				btnNext.Enabled = false;
			}
			else 
			{
				btnLast.Enabled = true;
				btnNext.Enabled = true;
				btnPrev.Enabled = true;
				btnFirst.Enabled = true;
			}
		}
	    
		protected void ddlPages_SelectedIndexChanged(Object sender, EventArgs e) 
		{
			GridViewRow gvrPager = GridView1.BottomPagerRow;
			DropDownList ddlPages = (DropDownList)gvrPager.Cells[0].FindControl("ddlPages");

			GridView1.PageIndex = ddlPages.SelectedIndex;

			// a method to populate your grid
			BindGrid(String.Empty);
		}


        protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
        {
            string columnName = e.SortExpression;
            //rebind the grid
            if (ViewState[SORT_DIRECTION] == null || ((string)ViewState[SORT_DIRECTION]) == SqlFragment.ASC)
            {
                ViewState[SORT_DIRECTION] = SqlFragment.DESC;
            }
            else
            {
                ViewState[SORT_DIRECTION] = SqlFragment.ASC;
            }
            BindGrid(columnName);
        }
	    
		string GetSortDirection(string sortBy) 
		{
			string sortDir = " ASC";
			if (ViewState["sortBy"] != null)
			{
				string sortedBy = ViewState["sortBy"].ToString();

				if (sortedBy == sortBy) 
				{
					//the direction should be desc
					sortDir = " DESC";

					//reset the sorter to null
					ViewState["sortBy"] = null;
				}
				else
				{
					//this is the first sort for this row
					//put it to the ViewState
					ViewState["sortBy"] = sortBy;
				}
			}
			else
			{
				//it's null, so this is the first sort
				ViewState["sortBy"] = sortBy;
			}
			return sortDir;
		}
	}
}
