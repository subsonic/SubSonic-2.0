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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using SubSonic.Utilities;

namespace SubSonic
{
    public class Scaffold : Control, INamingContainer
    {
        #region ScaffoldMode enum

        public enum ScaffoldMode
        {
            List,
            Add,
            Edit
        }

        #endregion


        #region ScaffoldTypeEnum enum

        public enum ScaffoldTypeEnum
        {
            Normal,
            Auto
        }

        #endregion


        private const string NO_PRIMARY_KEY = "The specified table does not contain a primary key, a requirement for use of the scaffolds, and much of SubSonic.";
        private const string NO_TABLE_NAME = "No <strong>TableName</strong> was specified. Please set this property, or set <strong>ScaffoldType</strong> to 'Auto'.";

        private const string NO_TABLE_SCHEMA =
            "No table was located with the <strong>TableName</strong> specified. Please change the property, or set <strong>ScaffoldType</strong> to 'Auto'.";

        protected const string ORDER_BY = "ORDER_BY";
        protected const string PK_ID = "pkID";
        protected const string PRIMARY_KEY_VALUE = "PRIMARY_KEY_VALUE";
        protected const string SCAFFOLD_MODE = "SCAFFOLD_MODE";
        protected const string SCAFFOLD_TYPE = "SCAFFOLD_TYPE";
        protected const string SORT_DIRECTION = "SORT_DIRECTION";
        protected readonly Button btnAdd = new Button();
        protected readonly Button btnCancel = new Button();
        protected readonly Button btnDelete = new Button();
        protected readonly Button btnSave = new Button();
        private readonly DropDownList ddlProviders = new DropDownList();
        private readonly DropDownList ddlTables = new DropDownList();
        protected readonly GridView grid = new GridView();
        protected readonly Label lblSorter = new Label();
        protected readonly List<string> listHiddenEditorColumns = new List<string>();
        protected readonly List<string> listHiddenGridColumns = new List<string>();
        protected readonly List<string> listReadOnlyColumns = new List<string>();
        protected readonly Panel surroundingPanel = new Panel();
        private bool _autoGenerateM2M = true;
        private string _buttonCssClass = ScaffoldCss.BUTTON;
        private string _checkBoxCssClass = ScaffoldCss.CHECK_BOX;
        private string _dropDownCssClass = ScaffoldCss.DROP_DOWN;
        private string _editTableCssClass = ScaffoldCss.EDIT_TABLE;
        private string _editTableItemCaptionCellCssClass = ScaffoldCss.EDIT_ITEM_CAPTION;
        private string _editTableItemCssClass = ScaffoldCss.EDIT_ITEM;
        private string _editTableLabelCssClass = ScaffoldCss.EDIT_TABLE_LABEL;

        private string _manyToManyMap;
        private bool _showScaffoldCaption = true;
        private string _textBoxCssClass = ScaffoldCss.TEXT_BOX;
        private List<Where> _whereCollection;
        private string _whereExpression = String.Empty;
        protected TableSchema.Table activeTableSchema;
        private string deleteConfirm = "Delete this record? This action cannot be undone...";
        private string tableName = String.Empty;

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        public string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the primary key value.
        /// </summary>
        /// <value>The primary key value.</value>
        [Browsable(false)]
        public string PrimaryKeyValue
        {
            get { return (string)ViewState[PRIMARY_KEY_VALUE]; }
            protected set { ViewState[PRIMARY_KEY_VALUE] = value; }
        }

        /// <summary>
        /// Gets primary key value.
        /// </summary>
        [Browsable(false)]
        public string PrimaryKeyControlValue
        {
            get
            {
                if(TableSchema.PrimaryKey.AutoIncrement || (TableSchema.PrimaryKey.DataType == DbType.Guid))
                    return ((Label)FindControl(PK_ID + TableSchema.PrimaryKey.ColumnName)).Text;
                return ((TextBox)FindControl(PK_ID + TableSchema.PrimaryKey.ColumnName)).Text;
            }
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        /// <summary>
        /// Gets the grid view.
        /// </summary>
        /// <value>The grid view.</value>
        public GridView GridView
        {
            get { return grid; }
        }

        /// <summary>
        /// Gets or sets the delete confirm.
        /// </summary>
        /// <value>The delete confirm.</value>
        [Bindable(true)]
        [Category("Behavior")]
        [DefaultValue("Delete this record? This action cannot be undone...")]
        public string DeleteConfirm
        {
            get { return deleteConfirm; }
            set { deleteConfirm = value; }
        }

        /// <summary>
        /// Whether to show the list view on save.
        /// </summary>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue(true)]
        public bool ReturnOnSave { get; set; }

        /// <summary>
        /// Sets the hidden editor columns.
        /// </summary>
        /// <value>The hidden editor columns.</value>
        [Bindable(true)]
        [Category("Data")]
        [Description("A comma delimited list of column names which are not displayed in the Editor.")]
        [DefaultValue("")]
        public string HiddenEditorColumns
        {
            set
            {
                listHiddenEditorColumns.Clear();
                foreach(string columnName in Utility.Split(value))
                    listHiddenEditorColumns.Add(columnName.ToLower());
            }
        }

        /// <summary>
        /// Sets the hidden grid columns.
        /// </summary>
        /// <value>The hidden grid columns.</value>
        [Bindable(true)]
        [Category("Data")]
        [Description("A comma delimited list of column names which will not be displayed in the GridView.")]
        [DefaultValue("")]
        public string HiddenGridColumns
        {
            set
            {
                listHiddenGridColumns.Clear();
                foreach(string columnName in Utility.Split(value))
                    listHiddenGridColumns.Add(columnName.ToLower());
            }
        }

        /// <summary>
        /// Sets the read only columns.
        /// </summary>
        /// <value>The read only columns.</value>
        [Bindable(true)]
        [Category("Data")]
        [Description("A comma delimited list of column names which are read only.")]
        [DefaultValue("")]
        public string ReadOnlyColumns
        {
            set
            {
                listReadOnlyColumns.Clear();
                foreach(string columnName in Utility.Split(value))
                    listReadOnlyColumns.Add(columnName.ToLower());
            }
        }

        /// <summary>
        /// Gets or sets the where expression.
        /// </summary>
        /// <value>The where expression.</value>
        [Bindable(true)]
        [Category("Data")]
        [Description("An expression which allows filtering the rows which are displayed.")]
        [DefaultValue("")]
        public string WhereExpression
        {
            get { return _whereExpression; }
            set { _whereExpression = value; }
        }

        /// <summary>
        /// Gets or sets the where collection.
        /// </summary>
        /// <value>The where collection.</value>
        [Browsable(false)]
        public List<Where> WhereCollection
        {
            get { return _whereCollection; }
            set { _whereCollection = value; }
        }

        /// <summary>
        /// Gets or sets the many to many map.
        /// </summary>
        /// <value>The many to many map.</value>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("")]
        public string ManyToManyMap
        {
            get { return _manyToManyMap; }
            set { _manyToManyMap = value.Trim().Replace(" ", String.Empty); }
        }

        private TableSchema.ManyToManyRelationshipCollection ManyToManyCollection
        {
            get
            {
                TableSchema.ManyToManyRelationshipCollection m2mCollection = TableSchema.ManyToManys;
                if(!String.IsNullOrEmpty(ManyToManyMap))
                {
                    string[] m2mTables = ManyToManyMap.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                    for(int i = m2mCollection.Count - 1; i >= 0; i--)
                    {
                        bool foundMatch = false;
                        foreach(string m2m in m2mTables)
                        {
                            if(Utility.IsMatch(m2m, m2mCollection[i].MapTableName))
                            {
                                foundMatch = true;
                                break;
                            }
                        }
                        if(!foundMatch)
                            m2mCollection.Remove(m2mCollection[i]);
                    }
                }
                else if(!AutoGenerateManyToMany)
                    m2mCollection.Clear();
                return m2mCollection;
            }
        }

        /// <summary>
        /// Whether or not the scaffold should detect and automatically generate Many to Many maps for tables automatically.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if it should automatically generate Many to Many maps; otherwise, <c>false</c>.
        /// </value>
        [Bindable(true)]
        [Category("Data")]
        [DefaultValue("True")]
        public bool AutoGenerateManyToMany
        {
            get { return _autoGenerateM2M; }
            set { _autoGenerateM2M = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can create.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can create; otherwise, <c>false</c>.
        /// </value>
        public bool CanCreate
        {
            get { return listReadOnlyColumns.Count == 0; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance can delete.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance can delete; otherwise, <c>false</c>.
        /// </value>
        public bool CanDelete
        {
            get { return listReadOnlyColumns.Count == 0; }
        }

        /// <summary>
        /// Gets or sets the CSS class.
        /// </summary>
        /// <value>The CSS class.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Sets the CSS class for the div surrounding the scaffold.")]
        [DefaultValue(ScaffoldCss.WRAPPER)]
        public string CssClass
        {
            get { return surroundingPanel.CssClass; }
            set { surroundingPanel.CssClass = value; }
        }

        /// <summary>
        /// Gets or sets the grid view skin ID.
        /// </summary>
        /// <value>The grid view skin ID.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Sets the skin for GridView.")]
        [DefaultValue("scaffold")]
        public string GridViewSkinID
        {
            get { return grid.SkinID; }
            set { grid.SkinID = value; }
        }

        /// <summary>
        /// Gets or sets the edit table CSS class.
        /// </summary>
        /// <value>The edit table CSS class.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Sets the CSS class used by the HTML table in the edit form.")]
        [DefaultValue(ScaffoldCss.EDIT_TABLE)]
        public string EditTableCssClass
        {
            get { return _editTableCssClass; }
            set { _editTableCssClass = value; }
        }

        /// <summary>
        /// Gets or sets the edit table item CSS class.
        /// </summary>
        /// <value>The edit table item CSS class.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Sets the CSS class used by editable form field elements in the edit form.")]
        [DefaultValue(ScaffoldCss.EDIT_ITEM)]
        public string EditTableItemCssClass
        {
            get { return _editTableItemCssClass; }
            set { _editTableItemCssClass = value; }
        }

        /// <summary>
        /// Gets or sets the edit table label CSS class.
        /// </summary>
        /// <value>The edit table label CSS class.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Sets the CSS class used by the table label in the edit form.")]
        [DefaultValue(ScaffoldCss.EDIT_TABLE_LABEL)]
        public string EditTableLabelCssClass
        {
            get { return _editTableLabelCssClass; }
            set { _editTableLabelCssClass = value; }
        }

        /// <summary>
        /// Gets or sets the button CSS class.
        /// </summary>
        /// <value>The button CSS class.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Sets the CSS class used by all buttons.")]
        [DefaultValue(ScaffoldCss.BUTTON)]
        public string ButtonCssClass
        {
            get { return _buttonCssClass; }
            set { _buttonCssClass = value; }
        }

        /// <summary>
        /// Gets or Sets the CSS class used by all TextBox elements in the edit form.
        /// </summary>
        /// <value>The text box CSS class.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Gets or Sets the CSS class used by all TextBox elements in the edit form.")]
        [DefaultValue(ScaffoldCss.TEXT_BOX)]
        public string TextBoxCssClass
        {
            get { return _textBoxCssClass; }
            set { _textBoxCssClass = value; }
        }

        /// <summary>
        /// Gets or Sets the CSS class used by all CheckBox elements in the edit form.
        /// </summary>
        /// <value>The text box CSS class.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Gets or Sets the CSS class used by all CheckBox elements in the edit form.")]
        [DefaultValue(ScaffoldCss.CHECK_BOX)]
        public string CheckBoxCssClass
        {
            get { return _checkBoxCssClass; }
            set { _checkBoxCssClass = value; }
        }

        /// <summary>
        /// Gets or Sets the CSS class used by all DropDown elements in the edit form.
        /// </summary>
        /// <value>The text box CSS class.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Gets or Sets the CSS class used by all DropDown elements in the edit form.")]
        [DefaultValue(ScaffoldCss.TEXT_BOX)]
        public string DropDownCssClass
        {
            get { return _dropDownCssClass; }
            set { _dropDownCssClass = value; }
        }

        /// <summary>
        /// Gets or sets the edit table item caption cell CSS class.
        /// </summary>
        /// <value>The edit table item caption cell CSS class.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Sets the CSS class used by the table cell surrounding edit item captions in the edit form.")]
        [DefaultValue(ScaffoldCss.EDIT_ITEM_CAPTION)]
        public string EditTableItemCaptionCellCssClass
        {
            get { return _editTableItemCaptionCellCssClass; }
            set { _editTableItemCaptionCellCssClass = value; }
        }

        /// <summary>
        /// Whether or not the Scaffold should emit it's default embedded style sheet at runtime.
        /// </summary>
        /// <value><c>true</c> if [use embedded styles]; otherwise, <c>false</c>.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("Whether or not the Scaffold should emit it's default embedded style sheet at runtime.")]
        [DefaultValue(false)]
        public bool UseEmbeddedStyles { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [show scaffold caption].
        /// </summary>
        /// <value><c>true</c> if [show scaffold caption]; otherwise, <c>false</c>.</value>
        [Bindable(true)]
        [Category("Display")]
        [Description("If true, a scaffold caption will be shown")]
        public bool ShowScaffoldCaption
        {
            get { return _showScaffoldCaption; }
            set { _showScaffoldCaption = value; }
        }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public ScaffoldMode Mode
        {
            get
            {
                if(ViewState[SCAFFOLD_MODE] == null)
                    ViewState[SCAFFOLD_MODE] = ScaffoldMode.List;
                return (ScaffoldMode)ViewState[SCAFFOLD_MODE];
            }
            protected set { ViewState[SCAFFOLD_MODE] = value; }
        }

        /// <summary>
        /// Gets or sets the type of the scaffold.
        /// </summary>
        /// <value>The type of the scaffold.</value>
        public ScaffoldTypeEnum ScaffoldType
        {
            get
            {
                if(ViewState[SCAFFOLD_TYPE] == null)
                    ViewState[SCAFFOLD_TYPE] = ScaffoldTypeEnum.Normal;
                return (ScaffoldTypeEnum)ViewState[SCAFFOLD_TYPE];
            }
            set { ViewState[SCAFFOLD_TYPE] = value; }
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <value>The table schema.</value>
        protected TableSchema.Table TableSchema
        {
            get { return activeTableSchema; }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            WebUIHelper.EmitClientScripts(Page);
            if(UseEmbeddedStyles)
            {
                grid.CssClass = ScaffoldCss.GRID;
                grid.CellPadding = 3;
                grid.GridLines = GridLines.Both;
                grid.BorderColor = Color.FromArgb(220, 220, 220);
                grid.FooterStyle.BackColor = Color.FromArgb(204, 204, 204);
                grid.RowStyle.BackColor = Color.FromArgb(238, 238, 238);
                grid.AlternatingRowStyle.BackColor = Color.FromArgb(255, 255, 255);
                grid.PagerStyle.BackColor = Color.FromArgb(153, 153, 153);
                grid.HeaderStyle.BackColor = Color.FromArgb(220, 220, 220);
                grid.HeaderStyle.Font.Bold = true;
                const string includeTemplate = "<link rel='stylesheet' text='text/css' href='{0}' />";
                string includeLocation = Page.ClientScript.GetWebResourceUrl(GetType(), "SubSonic.Controls.Resources.Scaffold.css");
                Page.Header.Controls.Add(new LiteralControl(String.Format(includeTemplate, includeLocation)));
            }
            base.OnPreRender(e);
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            surroundingPanel.ID = "pnlSurround";
            Controls.Add(surroundingPanel);
            surroundingPanel.Controls.Clear();

            Label lblTitle = new Label {ID = "lblTitle"};
            surroundingPanel.Controls.Add(lblTitle);
            lblTitle.Visible = true;

            if(String.IsNullOrEmpty(TableName) && ScaffoldType == ScaffoldTypeEnum.Normal)
            {
                lblTitle.Text = NO_TABLE_NAME;
                return;
            }

            if(ScaffoldType == ScaffoldTypeEnum.Normal)
                activeTableSchema = DataService.GetSchema(TableName, ProviderName, TableType.Table);

            if(ScaffoldType == ScaffoldTypeEnum.Normal && (activeTableSchema == null || activeTableSchema.PrimaryKey == null))
            {
                lblTitle.Text = activeTableSchema == null ? NO_TABLE_SCHEMA : NO_PRIMARY_KEY;
                return;
            }

            lblTitle.Visible = ShowScaffoldCaption;
            //add in the button row
            Panel pnlButtons = new Panel {ID = "pnlButtons"};
            surroundingPanel.Controls.Add(pnlButtons);

            btnAdd.ID = "btnAdd";
            pnlButtons.Controls.Add(btnAdd);
            btnSave.ID = "btnSave";
            pnlButtons.Controls.Add(btnSave);
            btnCancel.ID = "btnCancel";
            pnlButtons.Controls.Add(btnCancel);
            btnDelete.ID = "btnDelete";
            pnlButtons.Controls.Add(btnDelete);

            foreach(Button button in pnlButtons.Controls)
                ApplyCssClass(button, ButtonCssClass);

            btnDelete.OnClientClick = "return CheckDelete();";

            btnSave.Text = "Save";
            btnDelete.Text = "Delete";
            btnCancel.Text = "Return";
            btnAdd.Text = "Add";

            btnAdd.Click += btnAdd_Click;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            btnDelete.Click += btnDelete_Click;

            btnDelete.Visible = CanDelete;

            bool isAdd = (Mode == ScaffoldMode.Add);
            bool isEdit = (Mode == ScaffoldMode.Edit);
            bool isGrid = (Mode == ScaffoldMode.List);

            if(ScaffoldType == ScaffoldTypeEnum.Auto)
            {
                Panel pnlNavigator = new Panel {ID = "pnlNavigator"};
                surroundingPanel.Controls.Add(pnlNavigator);

                pnlNavigator.Controls.Clear();

                if(DataService.Providers != null)
                {
                    pnlNavigator.Attributes.Add("style", "float:left");
                    pnlNavigator.Controls.Add(new LiteralControl("<div style=\"margin-top:10px\">Provider</div><div>"));
                    ddlProviders.ID = "ddlProviders";
                    pnlNavigator.Controls.Add(ddlProviders);
                    ApplyCssClass(ddlProviders, DropDownCssClass);
                    PopulateProviderList();
                    ddlProviders.AutoPostBack = true;
                    ddlProviders.SelectedIndexChanged += ddlProviders_SelectedIndexChanged;

                    ProviderName = ddlProviders.SelectedValue;

                    pnlNavigator.Controls.Add(new LiteralControl("</div><div style=\"margin-top:10px\">Table</div><div>"));

                    ddlTables.ID = "ddlTables";
                    pnlNavigator.Controls.Add(ddlTables);
                    ApplyCssClass(ddlTables, DropDownCssClass);
                    PopulateTableList();
                    ddlTables.AutoPostBack = true;
                    ddlTables.SelectedIndexChanged += ddlTables_SelectedIndexChanged;

                    pnlNavigator.Controls.Add(new LiteralControl("</div>"));

                    //if (TableSchema == null && ddlTables.SelectedIndex > -1)
                    activeTableSchema = DataService.GetSchema(ddlTables.SelectedValue, ProviderName);
                }

                pnlNavigator.Visible = isGrid;
            }

            Panel pnlEditor = new Panel {ID = "pnlEditor"};
            surroundingPanel.Controls.Add(pnlEditor);

            CreateEditor(pnlEditor, isEdit);

            Panel pnlGrid = new Panel {ID = "pnlGrid"};
            surroundingPanel.Controls.Add(pnlGrid);

            grid.ID = "grid";
            pnlGrid.Controls.Add(grid);

            grid.Sorting += grid_Sorting;
            grid.RowEditing += grid_RowEditing;
            grid.AllowSorting = true;

            btnAdd.Visible = false;

            if(isGrid)
            {
                btnAdd.Visible = CanCreate;
                pnlEditor.Visible = false;
                pnlGrid.Visible = true;
                btnSave.Visible = false;
                btnCancel.Visible = false;
                btnDelete.Visible = false;
                BindGrid(String.Empty);
            }

            if(isEdit)
            {
                pnlEditor.Visible = true;
                pnlGrid.Visible = false;
                BindEditor(TableSchema, PrimaryKeyValue);
                btnSave.Visible = true;
                btnCancel.Visible = true;
                btnDelete.Visible = CanDelete;
            }

            if(isAdd)
            {
                pnlEditor.Visible = true;
                pnlGrid.Visible = false;
                btnSave.Visible = true;
                btnCancel.Visible = true;
                btnDelete.Visible = false;
                lblTitle.Visible = !ShowScaffoldCaption;
            }

            lblTitle.Text = String.Format("<h2>{0} Admin</h2>", TableSchema.DisplayName);

            Label lblMessage = new Label {ID = "lblMessage"};
            surroundingPanel.Controls.Add(lblMessage);

            lblSorter.Text = String.Empty;
            surroundingPanel.Controls.Add(lblSorter);

            ViewState[SCAFFOLD_MODE] = Mode;
            if(Mode != ScaffoldMode.Edit)
                PrimaryKeyValue = String.Empty;
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlTables control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ddlTables_SelectedIndexChanged(object sender, EventArgs e)
        {
            CreateChildControls();
            //throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlProviders control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ddlProviders_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlTables.Items.Clear();
            CreateChildControls();
            //throw new Exception("The method or operation is not implemented.");
        }

        private void PopulateProviderList()
        {
            if(ddlProviders.Items.Count == 0)
            {
                foreach(DataProvider p in DataService.Providers)
                    ddlProviders.Items.Add(p.Name);
            }
        }

        private void PopulateTableList()
        {
            if(ddlTables.Items.Count == 0)
            {
                if(!String.IsNullOrEmpty(ProviderName))
                {
                    //ddlTables.Items.Clear();
                    TableSchema.Table[] tables = DataService.GetTables(ProviderName);
                    foreach(TableSchema.Table table in tables)
                    {
                        if(CodeService.ShouldGenerate(table))
                        {
                            if(table.PrimaryKey != null && table.PrimaryKeys.Length > 0)
                            {
                                ListItem li = new ListItem(table.ClassName, table.Name);
                                ddlTables.Items.Add(li);
                            }
                        }
                    }
                }
            }
        }

        private static object TransformBooleanAndDateValues(object oVal, DbType dataType)
        {
            if(oVal != DBNull.Value && oVal != null)
            {
                if(dataType == DbType.Boolean)
                {
                    if(Utility.IsMatch(oVal.ToString(), Boolean.FalseString))
                        oVal = 0;
                    else if(Utility.IsMatch(oVal.ToString(), Boolean.TrueString))
                        oVal = 1;
                }
                else if(dataType == DbType.DateTime)
                    oVal = Convert.ToDateTime(oVal);
            }
            return oVal;
        }

        /// <summary>
        /// Builds the update SQL.
        /// </summary>
        /// <returns></returns>
        protected void UpdateRecord(string primaryKeyValue)
        {
            Update qryUpdate = new Update(TableSchema);
            foreach(TableSchema.TableColumn col in TableSchema.Columns)
            {
                if(col.DataType != DbType.Binary && Utility.IsWritableColumn(col))
                {
                    Control ctrl = FindControl(col.IsPrimaryKey ? PK_ID + col.ColumnName : col.ColumnName);
                    if(ctrl != null)
                    {
                        object oVal = Utility.GetDefaultControlValue(col, ctrl, false, true);
                        oVal = TransformBooleanAndDateValues(oVal, col.DataType);
                        qryUpdate.Set(col).EqualTo(oVal);
                    }
                }
            }
            qryUpdate.Where(TableSchema.PrimaryKey).IsEqualTo(primaryKeyValue);
            qryUpdate.Execute();
        }

        /// <summary>
        /// Builds the insert SQL.
        /// </summary>
        /// <returns></returns>
        protected void InsertRecord()
        {
            Insert qryInsert = new Insert(TableSchema, false);
            foreach(TableSchema.TableColumn col in TableSchema.Columns)
            {
                if(!col.AutoIncrement && !col.IsReadOnly)
                {
                    Control ctrl = FindControl(col.IsPrimaryKey ? PK_ID + col.ColumnName : col.ColumnName);
                    if(ctrl == null && col.IsPrimaryKey)
                        ctrl = FindControl(PK_ID);

                    if(ctrl != null)
                    {
                        object oVal = Utility.GetDefaultControlValue(col, ctrl, true, true);
                        bool insertValue = true;
                        if(col.DataType == DbType.Guid)
                        {
                            if(!Utility.IsMatch(col.DefaultSetting, SqlSchemaVariable.DEFAULT))
                            {
                                bool isEmptyGuid = Utility.IsMatch(oVal.ToString(), Guid.Empty.ToString());

                                if(col.IsNullable && isEmptyGuid)
                                    oVal = null;
                                else if(col.IsPrimaryKey && isEmptyGuid)
                                    oVal = Guid.NewGuid();
                            }
                            else
                            {
                                oVal = null;
                                insertValue = false;
                            }
                        }
                        else
                            oVal = TransformBooleanAndDateValues(oVal, col.DataType);

                        if(oVal == null)
                            oVal = DBNull.Value;

                        if(insertValue)
                            qryInsert.Value(col, oVal);
                    }
                }

                //if (col.DataType != DbType.Binary && col.DataType != DbType.Byte && Utility.IsWritableColumn(col))
                //{
                //    if (!col.AutoIncrement && !(col.IsPrimaryKey && col.DataType == DbType.Guid))
                //    {
                //        Control ctrl = FindControl(col.ColumnName);
                //        if (ctrl != null)
                //        {
                //            object oVal = Utility.GetDefaultControlValue(col, ctrl, true, true);
                //            qryInsert.Value(col, oVal);
                //        }
                //    }
                //}
            }
            qryInsert.Execute();
        }

        /// <summary>
        /// Deletes the record.
        /// </summary>
        protected void DeleteRecord()
        {
            if(!CanDelete)
                throw new SecurityException(String.Format("This row can not be deleted as it has {0} read-only fields", listReadOnlyColumns.Count));

            SqlQuery qryDelete = new Delete().From(TableSchema).Where(TableSchema.PrimaryKey).IsEqualTo(PrimaryKeyControlValue);
            qryDelete.Execute();
        }

        /// <summary>
        /// Adds the row.
        /// </summary>
        /// <param name="htmlTable">The HTML table.</param>
        /// <param name="cellValue1">The cell value1.</param>
        /// <param name="control">The control.</param>
        protected void AddRow(HtmlTable htmlTable, string cellValue1, Control control)
        {
            HtmlTableRow tr = new HtmlTableRow();
            htmlTable.Rows.Add(tr);

            HtmlTableCell td = new HtmlTableCell();
            tr.Cells.Add(td);

            //label
            ApplyCssClass(td, EditTableItemCaptionCellCssClass);
            td.InnerHtml = String.Format("<span style=\"font-weight:bold\">{0}</span>", cellValue1);

            //control
            HtmlTableCell td2 = new HtmlTableCell();
            tr.Cells.Add(td2);
            ApplyCssClass(td2, EditTableItemCssClass);
            td2.Controls.Add(control);
        }

        /// <summary>
        /// Used to apply CSS class values to WebControls. Ensures that no empty classes are applied;
        /// </summary>
        /// <param name="control"></param>
        /// <param name="cssClass"></param>
        protected static void ApplyCssClass(WebControl control, string cssClass)
        {
            if(!String.IsNullOrEmpty(cssClass))
                control.CssClass = cssClass;
        }

        /// <summary>
        /// Used to apply class attribute to HtmlControls. Ensures that no empty classes are applied;
        /// </summary>
        /// <param name="control"></param>
        /// <param name="cssClass"></param>
        protected static void ApplyCssClass(HtmlControl control, string cssClass)
        {
            if(!String.IsNullOrEmpty(cssClass))
                control.Attributes.Add("class", cssClass);
        }

        /// <summary>
        /// Adds the row.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="text">The text.</param>
        /// <param name="colspan">The colspan.</param>
        protected void AddRow(HtmlTable table, string text, int colspan)
        {
            HtmlTableRow tr = new HtmlTableRow();
            table.Rows.Add(tr);

            HtmlTableCell td = new HtmlTableCell();
            tr.Cells.Add(td);
            ApplyCssClass(td, EditTableLabelCssClass);

            if(colspan > 0)
                td.ColSpan = colspan;
            td.InnerHtml = text;
        }

        /// <summary>
        /// Shows the message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected void ShowMessage(string message)
        {
            Label lblMessage = (Label)FindControl("lblMessage");
            if(lblMessage != null)
                lblMessage.Text = String.Format("{0}<br/><span style=\"font-style:italic\">{1}</span>", message, TableSchema.Provider.Now);
        }

        /// <summary>
        /// Gets the edit control.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        protected Control GetEditControl(TableSchema.TableColumn tableColumn)
        {
            WebControl cOut = null;
            string colName = tableColumn.ColumnName.ToLower();
            //use special care with the Primary Key
            if(tableColumn.IsPrimaryKey && FindControl(PK_ID + tableColumn.ColumnName) == null)
            {
                //don't want to edit an auto-increment
                if(tableColumn.AutoIncrement || tableColumn.DataType == DbType.Guid)
                    cOut = new Label();
                else
                    cOut = new TextBox();

                cOut.ID = PK_ID + tableColumn.ColumnName;
            }
            else
            {
                bool buildForeignKey = false;
                if(tableColumn.IsForeignKey)
                {
                    DropDownList ddl = new DropDownList();
                    ApplyCssClass(ddl, DropDownCssClass);
                    string fkTableName = tableColumn.ForeignKeyTableName;

                    if(!String.IsNullOrEmpty(fkTableName))
                    {
                        buildForeignKey = true;
                        TableSchema.Table tbl = DataService.GetSchema(fkTableName, ProviderName, TableType.Table);
                        TableSchema.TableColumn displayCol = Utility.GetDisplayTableColumn(tbl);

                        bool isSortable = Utility.GetEffectiveMaxLength(displayCol) < 250;

                        if(tableColumn.IsNullable)
                        {
                            ListItem liNull = new ListItem("(Not Specified)", String.Empty);
                            ddl.Items.Add(liNull);
                        }

                        SqlQuery qry = new Select(tbl.Provider, tbl.Columns[0].ColumnName, displayCol.ColumnName).From(tbl);

                        if(isSortable)
                            qry.OrderAsc(displayCol.ColumnName);

                        using(IDataReader rdr = qry.ExecuteReader())
                        {
                            while(rdr.Read())
                            {
                                ListItem item = new ListItem(rdr[1].ToString(), rdr[0].ToString());
                                ddl.Items.Add(item);
                            }
                            rdr.Close();
                        }
                        cOut = ddl;
                    }
                }
                if(!buildForeignKey)
                {
                    switch(tableColumn.DataType)
                    {
                        case DbType.Guid:
                        case DbType.AnsiString:
                        case DbType.String:
                        case DbType.StringFixedLength:
                        case DbType.Xml:
                        case DbType.Object:
                        case DbType.AnsiStringFixedLength:
                            if(Utility.MatchesOne(colName, ReservedColumnName.CREATED_BY, ReservedColumnName.MODIFIED_BY))
                                cOut = new Label();
                            else
                            {
                                TextBox t = new TextBox();
                                if(Utility.GetEffectiveMaxLength(tableColumn) > 250)
                                {
                                    t.TextMode = TextBoxMode.MultiLine;
                                    t.Columns = 60;
                                    t.Rows = 4;
                                }
                                else
                                {
                                    t.Width = Unit.Pixel(250);
                                    if(colName.EndsWith("guid", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        t.Text = Guid.NewGuid().ToString();
                                        t.Enabled = false;
                                    }
                                }
                                cOut = t;
                            }
                            break;

                        case DbType.Binary:
                            //do nothing
                            break;
                        case DbType.Boolean:
                            CheckBox chk = new CheckBox();
                            ApplyCssClass(chk, CheckBoxCssClass);
                            if(Utility.IsMatch(colName, ReservedColumnName.IS_ACTIVE))
                                chk.Checked = true;

                            if(Utility.MatchesOne(colName, ReservedColumnName.DELETED, ReservedColumnName.IS_DELETED))
                                chk.Checked = false;

                            cOut = chk;
                            break;

                        case DbType.Date:
                        case DbType.Time:
                        case DbType.DateTime:
                            if(Utility.MatchesOne(colName, ReservedColumnName.MODIFIED_ON, ReservedColumnName.CREATED_ON))
                                cOut = new Label();
                            else
                                cOut = new CalendarControl();
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
                            TextBox tt = new TextBox {Width = Unit.Pixel(50)};
                            //if (!this.isNew)
                            //tt.Text = this.GetColumnValue(col.ColumnName).ToString();
                            cOut = tt;
                            break;
                        default:
                            cOut = new TextBox();
                            break;
                    }
                }

                if(cOut != null)
                    cOut.ID = tableColumn.ColumnName;
            }
            if(cOut is TextBox)
            {
                TextBox tbx = (TextBox)cOut;
                ApplyCssClass(tbx, TextBoxCssClass);
                if(cOut.GetType() == typeof(TextBox)) //Not Redundant! CalendarControl is TextBox == true; myCalendarControl.GetType() == typeof(TextBox) == false!
                {
                    int maxLength = Utility.GetEffectiveMaxLength(tableColumn);
                    if(maxLength > 0)
                        tbx.MaxLength = maxLength;
                }
            }

            if(cOut != null && listReadOnlyColumns.Contains(colName))
                cOut.Enabled = false;

            return cOut;
        }

        /// <summary>
        /// Binds the editor.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <param name="keyId">The key id.</param>
        protected void BindEditor(TableSchema.Table tableSchema, string keyId)
        {
            //get all the data for this row
            SqlQuery qry = new Select(tableSchema.Provider).From(tableSchema).Where(tableSchema.PrimaryKey).IsEqualTo(keyId);
            //qry.AddWhere(tbl.PrimaryKey.ColumnName, keyID);

            using(IDataReader rdr = qry.ExecuteReader())
            {
                if(rdr.Read())
                {
                    foreach(TableSchema.TableColumn col in tableSchema.Columns)
                    {
                        if(col.IsPrimaryKey)
                        {
                            Control ctrl = FindControl(PK_ID + col.ColumnName);
                            if(ctrl != null)
                            {
                                string colValue = rdr[col.ColumnName].ToString();
                                Type ctrlType = ctrl.GetType();
                                if(ctrlType == typeof(Label))
                                    ((Label)ctrl).Text = colValue;
                                else if(ctrlType == typeof(DropDownList))
                                    ((DropDownList)ctrl).SelectedValue = colValue;
                                else if(ctrlType == typeof(TextBox))
                                    ((TextBox)ctrl).Text = colValue;
                            }
                        }
                        else
                        {
                            Control ctrl = FindControl(col.ColumnName);
                            if(ctrl != null)
                            {
                                Type ctrlType = ctrl.GetType();
                                if(ctrlType == typeof(TextBox))
                                {
                                    TextBox tbx = ((TextBox)ctrl);
                                    tbx.Text = rdr[col.ColumnName].ToString();
                                }
                                else if(ctrlType == typeof(CheckBox))
                                {
                                    if(!col.IsNullable || (col.IsNullable && rdr[col.ColumnName] != DBNull.Value))
                                        ((CheckBox)ctrl).Checked = Convert.ToBoolean(rdr[col.ColumnName]);
                                }
                                else if(ctrlType == typeof(DropDownList))
                                    ((DropDownList)ctrl).SelectedValue = rdr[col.ColumnName].ToString();
                                else if(ctrlType == typeof(CalendarControl))
                                {
                                    DateTime dt;
                                    if(DateTime.TryParse(rdr[col.ColumnName].ToString(), out dt))
                                    {
                                        CalendarControl cal = (CalendarControl)ctrl;
                                        cal.SelectedDate = dt.Date;
                                    }
                                }
                                else if(ctrlType == typeof(Label))
                                    ((Label)ctrl).Text = rdr[col.ColumnName].ToString();
                            }
                        }
                    }
                }
                rdr.Close();
            }
        }

        /// <summary>
        /// Binds the grid.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        protected void BindGrid(string orderBy)
        {
            if(TableSchema != null && TableSchema.PrimaryKey != null)
            {
                SqlQuery query = new Select(TableSchema.Provider).From(TableSchema);

                if(!String.IsNullOrEmpty(_whereExpression))
                    query.WhereExpression(_whereExpression);

                if(_whereCollection != null)
                    SqlQueryBridge.AddLegacyWhereCollection(query, _whereCollection);

                string sortColumn = null;
                if(!String.IsNullOrEmpty(orderBy))
                    sortColumn = orderBy;
                else if(ViewState[ORDER_BY] != null)
                    sortColumn = (string)ViewState[ORDER_BY];

                int colIndex = -1;

                if(!String.IsNullOrEmpty(sortColumn))
                {
                    ViewState.Add(ORDER_BY, sortColumn);
                    TableSchema.TableColumn col = TableSchema.GetColumn(sortColumn);
                    if(col == null)
                    {
                        for(int i = 0; i < TableSchema.Columns.Count; i++)
                        {
                            TableSchema.TableColumn fkCol = TableSchema.Columns[i];
                            if(fkCol.IsForeignKey && !String.IsNullOrEmpty(fkCol.ForeignKeyTableName))
                            {
                                TableSchema.Table fkTbl = DataService.GetSchema(fkCol.ForeignKeyTableName, ProviderName, TableType.Table);
                                if(fkTbl != null)
                                {
                                    col = Utility.GetDisplayTableColumn(fkTbl);
                                    colIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                    if(col != null && col.MaxLength < 2048)
                    {
                        string sortAlias = colIndex > -1 ? SqlFragment.JOIN_PREFIX + colIndex : col.ColumnName;
                        if(ViewState[SORT_DIRECTION] == null || ((string)ViewState[SORT_DIRECTION]) == SqlFragment.ASC)
                        {
                            query.OrderAsc(sortAlias);
                            //query.OrderBy = colIndex > -1 ? OrderBy.Asc(col, SqlFragment.JOIN_PREFIX + colIndex) : OrderBy.Asc(col);
                            ViewState[SORT_DIRECTION] = SqlFragment.ASC;
                        }
                        else
                        {
                            query.OrderDesc(sortAlias);
                            //query.OrderBy = colIndex > -1 ? OrderBy.Desc(col, SqlFragment.JOIN_PREFIX + colIndex) : OrderBy.Desc(col);
                            ViewState[SORT_DIRECTION] = SqlFragment.DESC;
                        }
                    }
                }

                DataTable dt = query.ExecuteJoinedDataSet().Tables[0];
                grid.DataSource = dt;
                grid.AutoGenerateColumns = false;
                grid.Columns.Clear();

                int columnOffset = 0;
                string dataKey = TableSchema.PrimaryKey.ColumnName;
                if(Utility.IsMappingTable(TableSchema) && dt.Columns.Count > TableSchema.Columns.Count)
                {
                    columnOffset = 1;
                    dataKey = dt.Columns[0].ColumnName;
                }

                grid.DataKeyNames = new[] {dataKey};

                CommandField link = new CommandField
                                        {
                                            ShowEditButton = true, 
                                            EditText = "Edit"
                                        };

                grid.Columns.Insert(0, link);

                for(int i = 0; i < TableSchema.Columns.Count; i++)
                {
                    int dtColIndex = i + columnOffset;

                    BoundField field = new BoundField
                                           {
                                               DataField = dt.Columns[dtColIndex].ColumnName, 
                                               SortExpression = dt.Columns[dtColIndex].ColumnName, HtmlEncode = false
                                           };
                    //field.SortExpression = Utility.QualifyColumnName(schema.Name, dt.Columns[i].ColumnName, schema.Provider);
                    TableSchema.TableColumn col = TableSchema.Columns[i];
                    if(col.IsForeignKey)
                    {
                        TableSchema.Table fkSchema = col.ForeignKeyTableName == null
                                                         ? DataService.GetForeignKeyTable(col, TableSchema)
                                                         : DataService.GetSchema(col.ForeignKeyTableName, ProviderName, TableType.Table);

                        if(fkSchema != null)
                            field.HeaderText = fkSchema.DisplayName;
                    }
                    else
                        field.HeaderText = col.DisplayName;

                    if(!Utility.IsAuditField(dt.Columns[dtColIndex].ColumnName) && !listHiddenGridColumns.Contains(dt.Columns[dtColIndex].ColumnName.ToLower()))
                        grid.Columns.Add(field);
                }

                grid.DataBind();
            }
        }

        /// <summary>
        /// Saves the editor.
        /// </summary>
        private void SaveEditor()
        {
            if(Mode == ScaffoldMode.Edit)
                UpdateRecord(PrimaryKeyControlValue);
            else
                InsertRecord();

            SaveManyToMany();

            if(ReturnOnSave)
                BuildWithModeChange(ScaffoldMode.List);
        }

        /// <summary>
        /// Saves the many to many.
        /// </summary>
        private void SaveManyToMany()
        {
            foreach(TableSchema.ManyToManyRelationship m2m in ManyToManyCollection)
            {
                TableSchema.Table mapTable = DataService.GetSchema(m2m.MapTableName, ProviderName);

                CheckBoxList chk = (CheckBoxList)FindControl(mapTable.ClassName);
                if(chk != null)
                {
                    ListItemCollection listItems = new ListItemCollection();
                    foreach(ListItem item in chk.Items)
                    {
                        if(item.Selected)
                            listItems.Add(item);
                    }
                    //using(TransactionScope ts = new TransactionScope())
                    //{
                    //    using(SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    //    {
                    SqlQuery qryDelete = new Delete().From(mapTable).Where(m2m.MapTableLocalTableKeyColumn).IsEqualTo(PrimaryKeyControlValue);
                    qryDelete.Execute();

                    foreach(ListItem selectedItem in listItems)
                    {
                        Insert qryInsert = new Insert(mapTable, false);
                        qryInsert.Value(mapTable.GetColumn(m2m.MapTableLocalTableKeyColumn), PrimaryKeyControlValue);
                        qryInsert.Value(mapTable.GetColumn(m2m.MapTableForeignTableKeyColumn), selectedItem.Value);
                        qryInsert.Execute();
                    }
                    //    }
                    //    ts.Complete();
                    //}
                }
            }
        }

        /// <summary>
        /// Creates the editor.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <param name="isEdit">if set to <c>true</c> [is edit].</param>
        protected void CreateEditor(Control destination, bool isEdit)
        {
            //if this is a many to many, we need to construct it differently
            HtmlTable tbl = new HtmlTable();
            if(Utility.IsMappingTable(TableSchema))
            {
                tbl = CreateManyMapper();
                destination.Controls.Add(tbl);
            }
            else
            {
                destination.Controls.Add(tbl);
                tbl.Width = "600px";

                if(ShowScaffoldCaption)
                    AddRow(tbl, String.Format("<h2>{0} Editor</h2>", TableSchema.DisplayName), 2);

                foreach(TableSchema.TableColumn col in TableSchema.Columns)
                {
                    Control ctrl = GetEditControl(col);
                    if(ctrl != null)
                    {
                        string label = col.DisplayName;
                        AddRow(tbl, label, ctrl);
                        if(listHiddenEditorColumns.Contains(col.ColumnName.ToLower()))
                            ctrl.Parent.Parent.Visible = false;
                        if(ctrl.GetType() == typeof(TextBox))
                        {
                            TextBox tbx = (TextBox)ctrl;
                            if(tbx.TextMode == TextBoxMode.MultiLine)
                            {
                                int maxLength = Utility.GetEffectiveMaxLength(col);
                                string remainingLength = (maxLength - tbx.Text.Length).ToString();
                                //string maxLength = efftectiveMaxLength.ToString();

                                string jsAttribute = String.Format("return imposeMaxLength(event, this, {0}, {1});", maxLength, tbl.Rows.Count);
                                tbx.Attributes.Add("onkeyup", jsAttribute);
                                tbx.Attributes.Add("onChange", jsAttribute);
                                LiteralControl lc =
                                    new LiteralControl(
                                        String.Format(
                                            "<div style='padding: 2px;'><div style='float:left'>Characters Remaining:&nbsp;</div><div id=\"counter{0}\" style=\"visibility:hidden\">{1}</div></div>",
                                            tbl.Rows.Count, remainingLength));
                                tbx.Parent.Controls.Add(lc);
                            }
                        }
                    }
                }
                if(isEdit)
                    AddManyToMany(tbl);
            }
        }

        /// <summary>
        /// Special builder for many to many relational tables.
        /// </summary>
        /// <returns></returns>
        private HtmlTable CreateManyMapper()
        {
            HtmlTable tbl = new HtmlTable {Width = "600px"};

            if(ShowScaffoldCaption)
                AddRow(tbl, String.Format("<h2>{0} Map</h2>", TableSchema.DisplayName), 2);

            foreach(TableSchema.TableColumn col in TableSchema.Columns)
            {
                //by convention, each key in the map table should be a foreignkey
                //if not, it's not good
                if(col.IsPrimaryKey)
                {
                    string fkTable = col.ForeignKeyTableName;
                    SqlQuery qry = new Select(col.Table.Provider).From(fkTable);
                    //Query qry = new Query(DataService.GetSchema(fkTable, ProviderName, TableType.Table));
                    DropDownList ddl = new DropDownList {ID = col.ColumnName};
                    AddRow(tbl, fkTable, ddl);

                    using(IDataReader rdr = qry.ExecuteReader())
                    {
                        while(rdr.Read())
                            ddl.Items.Add(new ListItem(rdr[1].ToString(), rdr[0].ToString()));
                        rdr.Close();
                    }
                }
                else
                {
                    Control ctrl = GetEditControl(col);
                    AddRow(tbl, Utility.ParseCamelToProper(col.ColumnName), ctrl);
                }
            }
            return tbl;
        }

        /// <summary>
        /// Adds the many to many.
        /// </summary>
        /// <param name="htmlTable">The HTML table.</param>
        private void AddManyToMany(HtmlTable htmlTable)
        {
            foreach(TableSchema.ManyToManyRelationship m2m in ManyToManyCollection)
            {
                TableSchema.Table mapTable = DataService.GetSchema(m2m.MapTableName, ProviderName);
                TableSchema.Table foreignTable = DataService.GetSchema(m2m.ForeignTableName, ProviderName);

                CheckBoxList chk = new CheckBoxList {ID = mapTable.ClassName};
                AddRow(htmlTable, mapTable.DisplayName, chk);
                chk.Items.Clear();
                chk.RepeatColumns = 2;

                bool isSortable = Utility.GetEffectiveMaxLength(foreignTable.Columns[1]) < 256;

                SqlQuery query = new Select(foreignTable.Provider).From(foreignTable);

                if(isSortable)
                    query.OrderAsc(foreignTable.Columns[1].ColumnName);

                using(IDataReader rdrAllMappings = query.ExecuteReader())
                {
                    while(rdrAllMappings.Read())
                        chk.Items.Add(new ListItem(rdrAllMappings[1].ToString(), rdrAllMappings[0].ToString().ToLower()));
                    rdrAllMappings.Close();
                }

                List<string> activeIds = new List<string>();
                SqlQuery queryMappings = new Select(mapTable.Provider).From(mapTable).Where(mapTable.GetColumn(m2m.MapTableLocalTableKeyColumn)).IsEqualTo(PrimaryKeyValue);
                using(IDataReader rdrActiveMappings = queryMappings.ExecuteReader())
                {
                    while(rdrActiveMappings.Read())
                        activeIds.Add(rdrActiveMappings[m2m.MapTableForeignTableKeyColumn].ToString().ToLower());
                    rdrActiveMappings.Close();
                }
                foreach(string id in activeIds)
                {
                    ListItem li = chk.Items.FindByValue(id);
                    if(li != null)
                        li.Selected = true;
                }
            }
        }

        private void BuildWithModeChange(ScaffoldMode mode)
        {
            Mode = mode;
            CreateChildControls();
        }


        #region Event Handlers

        /// <summary>
        /// Handles the Sorting event of the grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewSortEventArgs"/> instance containing the event data.</param>
        protected void grid_Sorting(object sender, GridViewSortEventArgs e)
        {
            string columnName = e.SortExpression;
            //rebind the grid
            if(ViewState[SORT_DIRECTION] == null || ((string)ViewState[SORT_DIRECTION]) == SqlFragment.ASC)
                ViewState[SORT_DIRECTION] = SqlFragment.DESC;
            else
                ViewState[SORT_DIRECTION] = SqlFragment.ASC;
            Mode = ScaffoldMode.List;
            BindGrid(columnName);
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            BuildWithModeChange(ScaffoldMode.List);
        }

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveEditor();
                BuildWithModeChange(ScaffoldMode.List);
            }
            catch(DbException x)
            {
                ShowMessage("<span style=\"color:#990000;font-weight:bold\">" + x.Message + "</span>");
            }
        }

        /// <summary>
        /// Handles the Click event of the btnDelete control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteRecord();
            BuildWithModeChange(ScaffoldMode.List);
        }

        /// <summary>
        /// Handles the Click event of the btnAdd control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            BuildWithModeChange(ScaffoldMode.Add);
        }

        /// <summary>
        /// Handles the RowEditing event of the grid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.GridViewEditEventArgs"/> instance containing the event data.</param>
        protected void grid_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if(grid.DataKeys != null && grid.DataKeys.Count > e.NewEditIndex)
            {
                DataKey key = grid.DataKeys[e.NewEditIndex];
                if(key != null)
                {
                    PrimaryKeyValue = key.Value.ToString();
                    BuildWithModeChange(ScaffoldMode.Edit);
                }
            }
        }

        #endregion
    }
}