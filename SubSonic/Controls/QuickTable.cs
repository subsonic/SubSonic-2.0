/*
 * SubSonic - http://subsonicproject.com
 * 
 * The contents of this file are subject to the Mozilla Public
 * License Version 1.1 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a copy of
 * the License at http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or
 * implied. See the License for the specific language governing
 * rights and limitations under the License.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SubSonic.Sugar;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the QuickTable class
    /// </summary>
    [DefaultProperty("TableName")]
    [ToolboxData("<{0}:QuickTable runat=server></{0}:QuickTable>")]
    public class QuickTable : WebControl, INamingContainer
    {
        #region props

        private const string headerLinkStyle = "text-decoration:none;color:black;font-size:12pt;font-family:arial;";
        private const string pagerButtonStyle = "border:1px solid #cccccc;background-color:whitesmoke;font-family:helvetica;font-size:10pt";
        private const string pagerStyle = "alignment:center; font-size:10pt;font-family:arial;border-top:1px solid #666666;margin-top:5px";
        private const string tableAlternatingStyle = "padding:3px;background-color:whitesmoke;font-family:arial;font-size:10pt;";
        private const string tableCellStyle = "padding:3px;background-color:white;font-family:arial;font-size:10pt;";
        private const string tableHeaderStyle = "font-weight:bold; text-align:center;font-family:arial;font-size:12pt;border-bottom:1px solid #666666;";
        private readonly string tableStyle = String.Empty;
        private string buttonFirstText = " << ";
        private string buttonLastText = " >> ";
        private string buttonNextText = " > ";
        private string buttonPreviousText = " < ";
        private string columnList = String.Empty;
        private string headerLinkCSSClass = String.Empty;
        private string linkOnColumn = String.Empty;
        private string linkToPage = String.Empty;
        private int pageIndex = 1;
        private string pagerButtonCSS = String.Empty;
        private string pagerCSS = String.Empty;
        private int pageSize;
        private string providerName = String.Empty;
        private bool showSort = true;
        private string tableAlternatingCSSClass = String.Empty;
        private string tableCellCSSClass = String.Empty;
        private string tableCSSClass = String.Empty;
        private string tableHeaderCSSClass = String.Empty;
        private string tableName = String.Empty;
        private int totalPages;
        private int totalRecords;
        private List<Where> whereCollection = new List<Where>();
        private string whereExpression = String.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether [show sort].
        /// </summary>
        /// <value><c>true</c> if [show sort]; otherwise, <c>false</c>.</value>
        public bool ShowSort
        {
            get { return showSort; }
            set { showSort = value; }
        }

        /// <summary>
        /// Gets or sets the column list.
        /// </summary>
        /// <value>The column list.</value>
        public string ColumnList
        {
            get { return columnList; }
            set { columnList = value; }
        }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public string ProviderName
        {
            get
            {
                if(String.IsNullOrEmpty(providerName))
                    providerName = DataService.Provider.Name;
                return providerName;
            }
            set { providerName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }

        /// <summary>
        /// Gets or sets the link to page.
        /// </summary>
        /// <value>The link to page.</value>
        public string LinkToPage
        {
            get { return linkToPage; }
            set { linkToPage = value; }
        }

        /// <summary>
        /// Gets or sets the link on column.
        /// </summary>
        /// <value>The link on column.</value>
        public string LinkOnColumn
        {
            get { return linkOnColumn; }
            set { linkOnColumn = value; }
        }

        /// <summary>
        /// Gets or sets the where collection.
        /// </summary>
        /// <value>The where collection.</value>
        public List<Where> WhereCollection
        {
            get { return whereCollection; }
            set { whereCollection = value; }
        }

        /// <summary>
        /// Gets or sets the where expression.
        /// </summary>
        /// <value>The where expression.</value>
        public string WhereExpression
        {
            get { return whereExpression; }
            set { whereExpression = value; }
        }

        /// <summary>
        /// Gets or sets the header link CSS class.
        /// </summary>
        /// <value>The header link CSS class.</value>
        public string HeaderLinkCSSClass
        {
            get { return headerLinkCSSClass; }
            set { headerLinkCSSClass = value; }
        }

        /// <summary>
        /// Gets or sets the table CSS class.
        /// </summary>
        /// <value>The table CSS class.</value>
        public string TableCSSClass
        {
            get { return tableCSSClass; }
            set { tableCSSClass = value; }
        }

        /// <summary>
        /// Gets or sets the table header CSS class.
        /// </summary>
        /// <value>The table header CSS class.</value>
        public string TableHeaderCSSClass
        {
            get { return tableHeaderCSSClass; }
            set { tableHeaderCSSClass = value; }
        }

        /// <summary>
        /// Gets or sets the table cell CSS class.
        /// </summary>
        /// <value>The table cell CSS class.</value>
        public string TableCellCSSClass
        {
            get { return tableCellCSSClass; }
            set { tableCellCSSClass = value; }
        }

        /// <summary>
        /// Gets or sets the table alternating CSS class.
        /// </summary>
        /// <value>The table alternating CSS class.</value>
        public string TableAlternatingCSSClass
        {
            get { return tableAlternatingCSSClass; }
            set { tableAlternatingCSSClass = value; }
        }

        /// <summary>
        /// Gets or sets the pager CSS.
        /// </summary>
        /// <value>The pager CSS.</value>
        public string PagerCSS
        {
            get { return pagerCSS; }
            set { pagerCSS = value; }
        }

        /// <summary>
        /// Gets or sets the page button CSS class.
        /// </summary>
        /// <value>The page button CSS class.</value>
        public string PageButtonCSSClass
        {
            get { return pagerButtonCSS; }
            set { pagerButtonCSS = value; }
        }

        /// <summary>
        /// Gets or sets the total records.
        /// </summary>
        /// <value>The total records.</value>
        public int TotalRecords
        {
            get { return totalRecords; }
            set { totalRecords = value; }
        }

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages
        {
            get { return totalPages; }
            set { totalPages = value; }
        }

        /// <summary>
        /// Gets or sets the button first text.
        /// </summary>
        /// <value>The button first text.</value>
        public string ButtonFirstText
        {
            get { return buttonFirstText; }
            set { buttonFirstText = value; }
        }

        /// <summary>
        /// Gets or sets the button previous text.
        /// </summary>
        /// <value>The button previous text.</value>
        public string ButtonPreviousText
        {
            get { return buttonPreviousText; }
            set { buttonPreviousText = value; }
        }

        /// <summary>
        /// Gets or sets the button next text.
        /// </summary>
        /// <value>The button next text.</value>
        public string ButtonNextText
        {
            get { return buttonNextText; }
            set { buttonNextText = value; }
        }

        /// <summary>
        /// Gets or sets the button last text.
        /// </summary>
        /// <value>The button last text.</value>
        public string ButtonLastText
        {
            get { return buttonLastText; }
            set { buttonLastText = value; }
        }

        #endregion


        //The "wrapper" table that holds the data
        //and the pager
        private const string CENTER = "center";
        private const string CLASS = "class";
        private const string CURRENT_PAGE = "currentPage";
        private const string LEFT = "left";
        private const string RIGHT = "right";
        private const string SORT_BY = "sortBy";
        private const string STYLE = "style";
        private const string TOTAL_RECORDS = "totalRecords";
        private readonly Button btnFirst = new Button();
        private readonly Button btnLast = new Button();
        private readonly Button btnNext = new Button();
        private readonly Button btnPrev = new Button();
        private readonly ArrayList colList = new ArrayList();
        private readonly DropDownList ddlPages = new DropDownList();
        private readonly Label lblRecordCount = new Label();
        private readonly Literal litPagerLabel = new Literal();
        private readonly HtmlTable tbl = new HtmlTable();
        private readonly HtmlTable tblPage = new HtmlTable();
        private readonly HtmlTable tblWrap = new HtmlTable();
        private readonly HtmlTableCell tdBottom = new HtmlTableCell();
        private readonly HtmlTableCell tdTop = new HtmlTableCell();
        private readonly HtmlTableRow trBottom = new HtmlTableRow();
        private readonly HtmlTableRow trTop = new HtmlTableRow();
        private DataTable dataSource;
        private TableSchema.Table schema;

        private string sortBy;

        private string sortDirection = "ASC";
        private HtmlTableCell td;
        private HtmlTableRow tr;

        /// <summary>
        /// Gets or sets the sort by.
        /// </summary>
        /// <value>The sort by.</value>
        public string SortBy
        {
            get { return sortBy; }
            set { sortBy = value; }
        }

        /// <summary>
        /// Gets or sets the sort direction.
        /// </summary>
        /// <value>The sort direction.</value>
        public string SortDirection
        {
            get { return sortDirection; }
            set { sortDirection = value; }
        }

        /// <summary>
        /// Adds the header text.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="tdHeaderCell">The td header cell.</param>
        /// <param name="overrideText">The override text.</param>
        private void AddHeaderText(TableSchema.TableColumn tableColumn, HtmlTableCell tdHeaderCell, string overrideText)
        {
            if(showSort)
            {
                LinkButton btn = new LinkButton
                                     {
                                         ID = "btn" + tableColumn.ColumnName,
                                         CommandArgument = tableColumn.ColumnName,
                                         Text = String.IsNullOrEmpty(overrideText) ? tableColumn.DisplayName : overrideText
                                     };

                if(!String.IsNullOrEmpty(headerLinkCSSClass))
                    btn.CssClass = headerLinkCSSClass;
                else
                    btn.Attributes.Add(STYLE, headerLinkStyle);

                tdHeaderCell.Controls.Add(btn);

                if(tableColumn.IsNumeric)
                    tdHeaderCell.Align = RIGHT;
                else if(tableColumn.IsDateTime)
                    tdHeaderCell.Align = CENTER;

                btn.Click += Sort_Click;
            }
            else
                tdHeaderCell.InnerHtml = String.IsNullOrEmpty(overrideText) ? overrideText : tableColumn.DisplayName;
        }

        /// <summary>
        /// Ensures the totals.
        /// </summary>
        /// <param name="qry">The qry.</param>
        private void EnsureTotals(SqlQuery qry)
        {
            //if there's paging, we have to run an initial query to figure out 
            //how many records we have
            //we should only do this once...
            if(pageSize > 0)
            {
                if(ViewState[TOTAL_RECORDS] == null)
                {
                    //set it
                    int originalPageSize = qry.PageSize;
                    qry.PageSize = 0;
                    TotalRecords = qry.GetRecordCount();
                    qry.PageSize = originalPageSize;
                    ViewState[TOTAL_RECORDS] = TotalRecords;
                }
                else
                    TotalRecords = (int)ViewState[TOTAL_RECORDS];
                //the pages are the records/pageSize+1
                //totalPages = (totalPages % pageSize == 0) ? totalRecords / pageSize : totalRecords / pageSize + 1;
                totalPages = (totalRecords % pageSize == 0) ? totalRecords / pageSize : totalRecords / pageSize + 1;
                lblRecordCount.Text = " of " + totalPages + " (" + totalRecords + " total) ";

                if(ddlPages.Items.Count == 0)
                {
                    //set up the dropDown
                    for(int i = 1; i <= totalPages; i++)
                        ddlPages.Items.Add(new ListItem(i.ToString(), i.ToString()));
                }
            }
        }

        /// <summary>
        /// Loads the grid.
        /// </summary>
        private void LoadGrid()
        {
            if(String.IsNullOrEmpty(tableName))
                throw new ArgumentException("No tableName property set - please be sure to set the name of the table or view you'd like to see", tableName);

            DecideSortDirection();

            //load the data
            DataProvider provider = DataService.GetInstance(ProviderName);
            SqlQuery q = new Select(provider).From(tableName);

            //set the select list
            StringBuilder selectList = new StringBuilder("*");
            if(!String.IsNullOrEmpty(columnList))
            {
                selectList = new StringBuilder();
                for(int i = 0; i < colList.Count; i++)
                {
                    selectList.Append(colList[i].ToString().Trim());
                    if(i + 1 < colList.Count)
                        selectList.Append(",");
                }
            }

            q.SelectColumnList = selectList.ToString().Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            //sorting
            if(!String.IsNullOrEmpty(sortBy))
            {
                TableSchema.TableColumn col = provider.GetTableSchema(tableName, TableType.Table).GetColumn(sortBy);
                if(col != null && col.MaxLength < 2048 && col.DataType != DbType.Binary && col.DataType != DbType.Byte)
                {
                    if(String.IsNullOrEmpty(sortDirection) || sortDirection.Trim() == SqlFragment.ASC.Trim())
                        q.OrderAsc(sortBy);
                    else
                        q.OrderDesc(sortBy);
                }
            }

            //paging
            if(pageSize > 0)
            {
                q.Paged(pageIndex, pageSize);
                ddlPages.SelectedValue = pageIndex.ToString();
            }

            //honor logical deletes
            q.CheckLogicalDelete();

            //where
            if(!String.IsNullOrEmpty(whereExpression))
                q.WhereExpression(whereExpression);

            SqlQueryBridge.AddLegacyWhereCollection(q, whereCollection);

            DataSet ds = q.ExecuteDataSet();

            if(ds.Tables.Count > 0)
                dataSource = ds.Tables[0];
            else
                throw new Exception("Bad query - no data returned. Did you set the correct provider?");

            EnsureTotals(q);
            //set the buttons
            SetPagingButtonState();
            //create a table
            BuildRows();
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            tbl.CellPadding = 3;
            tbl.CellSpacing = 0;
            tbl.Width = "100%";
            tblPage.Width = "100%";

            //add three rows to this table
            //i know this might seem sort of strange
            //but given that we're dealing with eventing of buttons
            //.NET is a bit clumsy about it - these controls
            //need to exist in the control bag in order for the 
            //event to be recognize. So we add them up front

            trTop.Cells.Add(tdTop);
            tblWrap.Rows.Add(trTop);
            trBottom.Cells.Add(tdBottom);
            tblWrap.Rows.Add(trBottom);

            tdTop.Controls.Add(tbl);
            tdBottom.Controls.Add(tblPage);
            Controls.Add(tblWrap);

            //set CSS
            if(!String.IsNullOrEmpty(tableCSSClass))
                tbl.Attributes.Add(CLASS, tableCSSClass);
            else
                tbl.Attributes.Add(STYLE, tableStyle);

            if(!String.IsNullOrEmpty(pagerButtonCSS))
            {
                btnFirst.Attributes.Add(CLASS, pagerButtonCSS);
                btnPrev.Attributes.Add(CLASS, pagerButtonCSS);
                btnNext.Attributes.Add(CLASS, pagerButtonCSS);
                btnLast.Attributes.Add(CLASS, pagerButtonCSS);
                ddlPages.Attributes.Add(CLASS, pagerButtonCSS);
            }
            else
            {
                btnFirst.Attributes.Add(STYLE, pagerButtonStyle);
                btnPrev.Attributes.Add(STYLE, pagerButtonStyle);
                btnNext.Attributes.Add(STYLE, pagerButtonStyle);
                btnLast.Attributes.Add(STYLE, pagerButtonStyle);
                ddlPages.Attributes.Add(STYLE, pagerButtonStyle);
            }

            //have to load up the pager buttons to the control set so we recognize them on 
            //postback

            //load the schema
            schema = DataService.GetSchema(tableName, ProviderName, TableType.Table) ?? DataService.GetSchema(tableName, ProviderName, TableType.View);
            if(schema == null)
                throw new Exception("Can't find a table names " + tableName + ". Did you set the correct providerName?");

            //load the headers
            BuildHeader();

            BuildPager();

            //if(!Page.IsPostBack)
            //{
            LoadGrid();

            trBottom.Visible = !(pageSize >= totalRecords);
            //}
        }


        #region Sorting

        /// <summary>
        /// Handles the Click event of the Sort control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Sort_Click(object sender, EventArgs e)
        {
            //the sort event. use a view to sort the table data
            LinkButton btn = (LinkButton)sender;
            SortBy = btn.CommandArgument;
            LoadGrid();
        }

        /// <summary>
        /// Decides the sort direction.
        /// </summary>
        private void DecideSortDirection()
        {
            //string sortDir = "ASC";
            if(ViewState[SORT_BY] != null)
            {
                string selectedSort = ViewState[SORT_BY].ToString();

                if(String.IsNullOrEmpty(SortBy))
                {
                    //sort wasn't clicked - a postback
                    //has reset the property to null
                    //so set the sort to what's in the viewState
                    SortBy = selectedSort;
                }
                else
                {
                    if(selectedSort == SortBy)
                    {
                        //the direction should be desc
                        SortDirection = SqlFragment.DESC;

                        //reset the sorter to null
                        ViewState[SORT_BY] = null;
                    }
                    else
                    {
                        //this is the first sort for this row
                        //put it to the ViewState
                        ViewState[SORT_BY] = SortBy;
                    }
                }
            }
            else
            {
                //if this is the first sort, store it in the ViewState
                ViewState[SORT_BY] = SortBy;
            }
        }

        #endregion


        #region Table Builders

        /// <summary>
        /// Builds the header.
        /// </summary>
        private void BuildHeader()
        {
            tr = new HtmlTableRow();

            string[] customCols = columnList.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            if(customCols.Length != 0)
            {
                foreach(string s in customCols)
                {
                    td = new HtmlTableCell();
                    if(!String.IsNullOrEmpty(tableHeaderCSSClass))
                        td.Attributes.Add(CLASS, tableHeaderCSSClass);
                    else
                        td.Attributes.Add(STYLE, tableHeaderStyle);

                    if(s.Contains(":"))
                    {
                        //it's a cast in the form of "productID:ID"
                        string[] castedList = s.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);

                        try
                        {
                            TableSchema.TableColumn col = schema.GetColumn(castedList[0].Trim()) ?? schema.GetColumn(castedList[1].Trim());

                            if(col == null)
                                throw new Exception("Can't find a column for this table named " + castedList[0] + " or " + castedList[1]);

                            colList.Add(col.ColumnName.ToLower());
                            AddHeaderText(col, td, castedList[1]);
                        }
                        catch
                        {
                            throw new Exception("Invalid Custom Columns. If you want to pass in a custom colum, it should be in the form 'columnName:Replacement Name'");
                        }
                    }
                    else
                    {
                        TableSchema.TableColumn col = schema.GetColumn(s.Trim());
                        if(col == null)
                            throw new Exception("Can't find a column for this table named " + s);

                        colList.Add(col.ColumnName.ToLower());
                        AddHeaderText(col, td, String.Empty);
                    }
                    tr.Cells.Add(td);
                }
            }
            else
            {
                //loop the schema
                foreach(TableSchema.TableColumn col in schema.Columns)
                {
                    td = new HtmlTableCell();
                    td.Attributes.Add(STYLE, tableHeaderStyle);

                    AddHeaderText(col, td, String.Empty);

                    tr.Cells.Add(td);
                    colList.Add(col.ColumnName.ToLower());
                }
            }
            tbl.Rows.Add(tr);
        }

        /// <summary>
        /// Builds the rows.
        /// </summary>
        private void BuildRows()
        {
            //pull the data
            //bool isEven = false;
            for(int i = tbl.Rows.Count - 1; i > 0; i--)
                tbl.Rows.RemoveAt(i);
            //tbl.Rows.Clear();
            string cellAttribute = String.IsNullOrEmpty(tableCellCSSClass) ? STYLE : CLASS;
            string cellAttributeValue = String.IsNullOrEmpty(tableCellCSSClass) ? tableCellStyle : tableCellCSSClass;
            string cellAttributeAltValue = String.IsNullOrEmpty(tableCellCSSClass) ? tableAlternatingStyle : tableAlternatingCSSClass;
            int rowCount = dataSource.Rows.Count;

            for(int r = 0; r < rowCount; r++)
            {
                DataRow dr = dataSource.Rows[r];
                tr = new HtmlTableRow();
                if(Numbers.IsEven(r))
                    tr.Attributes.Add(cellAttribute, cellAttributeValue);
                else
                    tr.Attributes.Add(cellAttribute, cellAttributeAltValue);

                int colCounter = 0;
                for(int i = 0; i < colList.Count; i++)
                {
                    string colName = colList[i].ToString();
                    TableSchema.TableColumn schemaColumn = schema.GetColumn(colName);

                    td = new HtmlTableCell();
                    if(schemaColumn.IsDateTime)
                    {
                        DateTime dt;
                        td.InnerHtml = DateTime.TryParse(dr[colName].ToString(), out dt) ? dt.ToShortDateString() : dr[colName].ToString();
                        td.Align = CENTER;
                    }
                    else if(schemaColumn.DataType == DbType.Currency)
                    {
                        decimal dCurr;
                        decimal.TryParse(dr[colName].ToString(), out dCurr);
                        td.InnerHtml = dCurr.ToString("c");
                        td.Align = RIGHT;
                    }
                    else if(schemaColumn.IsNumeric)
                    {
                        td.InnerHtml = dr[colName].ToString();
                        td.Align = RIGHT;
                    }
                    else
                        td.InnerHtml = dr[colName].ToString();

                    //check the linkTo
                    if(!String.IsNullOrEmpty(linkToPage))
                    {
                        string link;
                        if(schema.PrimaryKey != null && linkToPage.Contains("{0}"))
                            link = "<a href=\"" + linkToPage.Replace("{0}", dr[schema.PrimaryKey.ColumnName].ToString()) + "\">" + td.InnerHtml + "</a>";
                        else if(schema.PrimaryKey != null)
                            link = "<a href=\"" + linkToPage + "?id=" + dr[schema.PrimaryKey.ColumnName] + "\">" + td.InnerHtml + "</a>";
                        else if(!String.IsNullOrEmpty(linkOnColumn) && linkToPage.Contains("{0}"))
                            link = "<a href=\"" + linkToPage.Replace("{0}", dr[linkOnColumn].ToString()) + "\">" + td.InnerHtml + "</a>";
                        else if(!String.IsNullOrEmpty(linkOnColumn))
                            link = "<a href=\"" + linkToPage + "?id=" + dr[linkOnColumn] + "\">" + td.InnerHtml + "</a>";
                        else
                            link = "<a href=\"" + linkToPage + "\">" + td.InnerHtml + "</a>";

                        if((!String.IsNullOrEmpty(linkOnColumn) && Utility.IsMatch(linkOnColumn, colName)) || colCounter == 0)
                            td.InnerHtml = link;
                    }

                    tr.Cells.Add(td);
                    colCounter++;
                }
                tbl.Rows.Add(tr);
            }
        }

        /// <summary>
        /// Builds the pager.
        /// </summary>
        private void BuildPager()
        {
            if(pageSize > 0)
            {
                HtmlTableRow trPage = new HtmlTableRow();
                tblPage.Rows.Add(trPage);

                HtmlTableCell tdPage = new HtmlTableCell();
                trPage.Cells.Add(tdPage);

                litPagerLabel.Text = " Page ";

                //add the pager buttons
                tdPage.Controls.Add(btnFirst);
                tdPage.Controls.Add(btnPrev);
                tdPage.Controls.Add(litPagerLabel);
                tdPage.Controls.Add(ddlPages);
                tdPage.Controls.Add(lblRecordCount);
                tdPage.Controls.Add(btnNext);
                tdPage.Controls.Add(btnLast);

                //set the text
                btnFirst.Text = buttonFirstText;
                btnLast.Text = buttonLastText;
                btnPrev.Text = buttonPreviousText;
                btnNext.Text = buttonNextText;

                //always the same
                btnFirst.CommandArgument = "1";

                //handle the page event
                btnFirst.Click += Paging_Click;
                btnLast.Click += Paging_Click;
                btnPrev.Click += Paging_Click;
                btnNext.Click += Paging_Click;

                ddlPages.SelectedIndexChanged += ddlPages_SelectedIndexChanged;
                ddlPages.AutoPostBack = true;

                tdPage.ColSpan = colList.Count;
                tdPage.Align = CENTER;
                if(!String.IsNullOrEmpty(pagerCSS))
                    tdPage.Attributes.Add(CLASS, pagerCSS);
                else
                    tdPage.Attributes.Add(STYLE, pagerStyle);

                //add to the pager table
                tblPage.Rows.Add(trPage);
            }
        }

        #endregion


        #region Paging

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlPages control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void ddlPages_SelectedIndexChanged(object sender, EventArgs e)
        {
            pageIndex = int.Parse(ddlPages.SelectedValue);
            //put it to the ViewState so we know what
            //page we're on if sorting is clicked
            ViewState[CURRENT_PAGE] = pageIndex;

            LoadGrid();
        }

        /// <summary>
        /// Handles the Click event of the Paging control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void Paging_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int pIndex;
            int.TryParse(btn.CommandArgument, out pIndex);
            PageIndex = pIndex;

            //put it to the ViewState so we know what
            //page we're on if sorting is clicked
            ViewState[CURRENT_PAGE] = pIndex;

            LoadGrid();
        }

        /// <summary>
        /// Sets the state of the paging button.
        /// </summary>
        private void SetPagingButtonState()
        {
            if(ViewState[CURRENT_PAGE] != null)
                pageIndex = (int)ViewState[CURRENT_PAGE];
            else
            {
                pageIndex = 1;
                ViewState[CURRENT_PAGE] = 1;
            }

            if(totalPages > 0 && totalRecords > 0)
            {
                //this is always the same
                if(pageIndex == 0)
                    pageIndex = 1;

                int nextPage = pageIndex + 1;
                int prevPage = pageIndex - 1;

                //command args
                btnNext.CommandArgument = nextPage.ToString();
                btnPrev.CommandArgument = prevPage.ToString();
                btnLast.CommandArgument = totalPages.ToString();

                //use
                btnPrev.Enabled = true;
                btnNext.Enabled = true;
                btnLast.Enabled = true;
                btnFirst.Enabled = true;

                if(pageIndex == totalPages)
                {
                    btnNext.Enabled = false;
                    btnLast.Enabled = false;
                }
                else if(pageIndex == 1)
                {
                    btnPrev.Enabled = false;
                    btnFirst.Enabled = false;
                }
            }
        }

        #endregion
    }
}