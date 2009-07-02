using System; 
using System.Text; 
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration; 
using System.Xml; 
using System.Xml.Serialization;
using SubSonic; 
using SubSonic.Utilities;
namespace Northwind
{
	/// <summary>
	/// Strongly-typed collection for the TextEntry class.
	/// </summary>
    [Serializable]
	public partial class TextEntryCollection : ActiveList<TextEntry, TextEntryCollection>
	{	   
		public TextEntryCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>TextEntryCollection</returns>
		public TextEntryCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                TextEntry o = this[i];
                foreach (SubSonic.Where w in this.wheres)
                {
                    bool remove = false;
                    System.Reflection.PropertyInfo pi = o.GetType().GetProperty(w.ColumnName);
                    if (pi.CanRead)
                    {
                        object val = pi.GetValue(o, null);
                        switch (w.Comparison)
                        {
                            case SubSonic.Comparison.Equals:
                                if (!val.Equals(w.ParameterValue))
                                {
                                    remove = true;
                                }
                                break;
                        }
                    }
                    if (remove)
                    {
                        this.Remove(o);
                        break;
                    }
                }
            }
            return this;
        }
		
		
	}
	/// <summary>
	/// This is an ActiveRecord class which wraps the TextEntry table.
	/// </summary>
	[Serializable]
	public partial class TextEntry : ActiveRecord<TextEntry>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public TextEntry()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public TextEntry(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public TextEntry(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public TextEntry(string columnName, object columnValue)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByParam(columnName,columnValue);
		}
		
		protected static void SetSQLProps() { GetTableSchema(); }
		
		#endregion
		
		#region Schema and Query Accessor	
		public static Query CreateQuery() { return new Query(Schema); }
		public static TableSchema.Table Schema
		{
			get
			{
				if (BaseSchema == null)
					SetSQLProps();
				return BaseSchema;
			}
		}
		
		private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("TextEntry", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarContentID = new TableSchema.TableColumn(schema);
				colvarContentID.ColumnName = "contentID";
				colvarContentID.DataType = DbType.Int32;
				colvarContentID.MaxLength = 0;
				colvarContentID.AutoIncrement = true;
				colvarContentID.IsNullable = false;
				colvarContentID.IsPrimaryKey = true;
				colvarContentID.IsForeignKey = false;
				colvarContentID.IsReadOnly = false;
				colvarContentID.DefaultSetting = @"";
				colvarContentID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarContentID);
				
				TableSchema.TableColumn colvarContentGUID = new TableSchema.TableColumn(schema);
				colvarContentGUID.ColumnName = "contentGUID";
				colvarContentGUID.DataType = DbType.Guid;
				colvarContentGUID.MaxLength = 0;
				colvarContentGUID.AutoIncrement = false;
				colvarContentGUID.IsNullable = false;
				colvarContentGUID.IsPrimaryKey = false;
				colvarContentGUID.IsForeignKey = false;
				colvarContentGUID.IsReadOnly = false;
				
						colvarContentGUID.DefaultSetting = @"(newid())";
				colvarContentGUID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarContentGUID);
				
				TableSchema.TableColumn colvarTitle = new TableSchema.TableColumn(schema);
				colvarTitle.ColumnName = "title";
				colvarTitle.DataType = DbType.String;
				colvarTitle.MaxLength = 500;
				colvarTitle.AutoIncrement = false;
				colvarTitle.IsNullable = true;
				colvarTitle.IsPrimaryKey = false;
				colvarTitle.IsForeignKey = false;
				colvarTitle.IsReadOnly = false;
				colvarTitle.DefaultSetting = @"";
				colvarTitle.ForeignKeyTableName = "";
				schema.Columns.Add(colvarTitle);
				
				TableSchema.TableColumn colvarContentName = new TableSchema.TableColumn(schema);
				colvarContentName.ColumnName = "contentName";
				colvarContentName.DataType = DbType.String;
				colvarContentName.MaxLength = 50;
				colvarContentName.AutoIncrement = false;
				colvarContentName.IsNullable = false;
				colvarContentName.IsPrimaryKey = false;
				colvarContentName.IsForeignKey = false;
				colvarContentName.IsReadOnly = false;
				colvarContentName.DefaultSetting = @"";
				colvarContentName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarContentName);
				
				TableSchema.TableColumn colvarContent = new TableSchema.TableColumn(schema);
				colvarContent.ColumnName = "content";
				colvarContent.DataType = DbType.String;
				colvarContent.MaxLength = 3000;
				colvarContent.AutoIncrement = false;
				colvarContent.IsNullable = true;
				colvarContent.IsPrimaryKey = false;
				colvarContent.IsForeignKey = false;
				colvarContent.IsReadOnly = false;
				colvarContent.DefaultSetting = @"";
				colvarContent.ForeignKeyTableName = "";
				schema.Columns.Add(colvarContent);
				
				TableSchema.TableColumn colvarIconPath = new TableSchema.TableColumn(schema);
				colvarIconPath.ColumnName = "iconPath";
				colvarIconPath.DataType = DbType.String;
				colvarIconPath.MaxLength = 250;
				colvarIconPath.AutoIncrement = false;
				colvarIconPath.IsNullable = true;
				colvarIconPath.IsPrimaryKey = false;
				colvarIconPath.IsForeignKey = false;
				colvarIconPath.IsReadOnly = false;
				colvarIconPath.DefaultSetting = @"";
				colvarIconPath.ForeignKeyTableName = "";
				schema.Columns.Add(colvarIconPath);
				
				TableSchema.TableColumn colvarDateExpires = new TableSchema.TableColumn(schema);
				colvarDateExpires.ColumnName = "dateExpires";
				colvarDateExpires.DataType = DbType.DateTime;
				colvarDateExpires.MaxLength = 0;
				colvarDateExpires.AutoIncrement = false;
				colvarDateExpires.IsNullable = true;
				colvarDateExpires.IsPrimaryKey = false;
				colvarDateExpires.IsForeignKey = false;
				colvarDateExpires.IsReadOnly = false;
				colvarDateExpires.DefaultSetting = @"";
				colvarDateExpires.ForeignKeyTableName = "";
				schema.Columns.Add(colvarDateExpires);
				
				TableSchema.TableColumn colvarLastEditedBy = new TableSchema.TableColumn(schema);
				colvarLastEditedBy.ColumnName = "lastEditedBy";
				colvarLastEditedBy.DataType = DbType.String;
				colvarLastEditedBy.MaxLength = 100;
				colvarLastEditedBy.AutoIncrement = false;
				colvarLastEditedBy.IsNullable = true;
				colvarLastEditedBy.IsPrimaryKey = false;
				colvarLastEditedBy.IsForeignKey = false;
				colvarLastEditedBy.IsReadOnly = false;
				colvarLastEditedBy.DefaultSetting = @"";
				colvarLastEditedBy.ForeignKeyTableName = "";
				schema.Columns.Add(colvarLastEditedBy);
				
				TableSchema.TableColumn colvarExternalLink = new TableSchema.TableColumn(schema);
				colvarExternalLink.ColumnName = "externalLink";
				colvarExternalLink.DataType = DbType.String;
				colvarExternalLink.MaxLength = 250;
				colvarExternalLink.AutoIncrement = false;
				colvarExternalLink.IsNullable = true;
				colvarExternalLink.IsPrimaryKey = false;
				colvarExternalLink.IsForeignKey = false;
				colvarExternalLink.IsReadOnly = false;
				colvarExternalLink.DefaultSetting = @"";
				colvarExternalLink.ForeignKeyTableName = "";
				schema.Columns.Add(colvarExternalLink);
				
				TableSchema.TableColumn colvarStatus = new TableSchema.TableColumn(schema);
				colvarStatus.ColumnName = "status";
				colvarStatus.DataType = DbType.String;
				colvarStatus.MaxLength = 50;
				colvarStatus.AutoIncrement = false;
				colvarStatus.IsNullable = true;
				colvarStatus.IsPrimaryKey = false;
				colvarStatus.IsForeignKey = false;
				colvarStatus.IsReadOnly = false;
				colvarStatus.DefaultSetting = @"";
				colvarStatus.ForeignKeyTableName = "";
				schema.Columns.Add(colvarStatus);
				
				TableSchema.TableColumn colvarListOrder = new TableSchema.TableColumn(schema);
				colvarListOrder.ColumnName = "listOrder";
				colvarListOrder.DataType = DbType.Int32;
				colvarListOrder.MaxLength = 0;
				colvarListOrder.AutoIncrement = false;
				colvarListOrder.IsNullable = false;
				colvarListOrder.IsPrimaryKey = false;
				colvarListOrder.IsForeignKey = false;
				colvarListOrder.IsReadOnly = false;
				
						colvarListOrder.DefaultSetting = @"((1))";
				colvarListOrder.ForeignKeyTableName = "";
				schema.Columns.Add(colvarListOrder);
				
				TableSchema.TableColumn colvarCallOut = new TableSchema.TableColumn(schema);
				colvarCallOut.ColumnName = "callOut";
				colvarCallOut.DataType = DbType.String;
				colvarCallOut.MaxLength = 250;
				colvarCallOut.AutoIncrement = false;
				colvarCallOut.IsNullable = true;
				colvarCallOut.IsPrimaryKey = false;
				colvarCallOut.IsForeignKey = false;
				colvarCallOut.IsReadOnly = false;
				colvarCallOut.DefaultSetting = @"";
				colvarCallOut.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCallOut);
				
				TableSchema.TableColumn colvarCreatedOn = new TableSchema.TableColumn(schema);
				colvarCreatedOn.ColumnName = "createdOn";
				colvarCreatedOn.DataType = DbType.DateTime;
				colvarCreatedOn.MaxLength = 0;
				colvarCreatedOn.AutoIncrement = false;
				colvarCreatedOn.IsNullable = true;
				colvarCreatedOn.IsPrimaryKey = false;
				colvarCreatedOn.IsForeignKey = false;
				colvarCreatedOn.IsReadOnly = false;
				
						colvarCreatedOn.DefaultSetting = @"(getdate())";
				colvarCreatedOn.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCreatedOn);
				
				TableSchema.TableColumn colvarCreatedBy = new TableSchema.TableColumn(schema);
				colvarCreatedBy.ColumnName = "createdBy";
				colvarCreatedBy.DataType = DbType.String;
				colvarCreatedBy.MaxLength = 50;
				colvarCreatedBy.AutoIncrement = false;
				colvarCreatedBy.IsNullable = true;
				colvarCreatedBy.IsPrimaryKey = false;
				colvarCreatedBy.IsForeignKey = false;
				colvarCreatedBy.IsReadOnly = false;
				colvarCreatedBy.DefaultSetting = @"";
				colvarCreatedBy.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCreatedBy);
				
				TableSchema.TableColumn colvarModifiedOn = new TableSchema.TableColumn(schema);
				colvarModifiedOn.ColumnName = "modifiedOn";
				colvarModifiedOn.DataType = DbType.DateTime;
				colvarModifiedOn.MaxLength = 0;
				colvarModifiedOn.AutoIncrement = false;
				colvarModifiedOn.IsNullable = true;
				colvarModifiedOn.IsPrimaryKey = false;
				colvarModifiedOn.IsForeignKey = false;
				colvarModifiedOn.IsReadOnly = false;
				
						colvarModifiedOn.DefaultSetting = @"(getdate())";
				colvarModifiedOn.ForeignKeyTableName = "";
				schema.Columns.Add(colvarModifiedOn);
				
				TableSchema.TableColumn colvarModifiedBy = new TableSchema.TableColumn(schema);
				colvarModifiedBy.ColumnName = "modifiedBy";
				colvarModifiedBy.DataType = DbType.String;
				colvarModifiedBy.MaxLength = 50;
				colvarModifiedBy.AutoIncrement = false;
				colvarModifiedBy.IsNullable = true;
				colvarModifiedBy.IsPrimaryKey = false;
				colvarModifiedBy.IsForeignKey = false;
				colvarModifiedBy.IsReadOnly = false;
				colvarModifiedBy.DefaultSetting = @"";
				colvarModifiedBy.ForeignKeyTableName = "";
				schema.Columns.Add(colvarModifiedBy);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("TextEntry",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("ContentID")]
		[Bindable(true)]
		public int ContentID 
		{
			get { return GetColumnValue<int>(Columns.ContentID); }
			set { SetColumnValue(Columns.ContentID, value); }
		}
		  
		[XmlAttribute("ContentGUID")]
		[Bindable(true)]
		public Guid ContentGUID 
		{
			get { return GetColumnValue<Guid>(Columns.ContentGUID); }
			set { SetColumnValue(Columns.ContentGUID, value); }
		}
		  
		[XmlAttribute("Title")]
		[Bindable(true)]
		public string Title 
		{
			get { return GetColumnValue<string>(Columns.Title); }
			set { SetColumnValue(Columns.Title, value); }
		}
		  
		[XmlAttribute("ContentName")]
		[Bindable(true)]
		public string ContentName 
		{
			get { return GetColumnValue<string>(Columns.ContentName); }
			set { SetColumnValue(Columns.ContentName, value); }
		}
		  
		[XmlAttribute("Content")]
		[Bindable(true)]
		public string Content 
		{
			get { return GetColumnValue<string>(Columns.Content); }
			set { SetColumnValue(Columns.Content, value); }
		}
		  
		[XmlAttribute("IconPath")]
		[Bindable(true)]
		public string IconPath 
		{
			get { return GetColumnValue<string>(Columns.IconPath); }
			set { SetColumnValue(Columns.IconPath, value); }
		}
		  
		[XmlAttribute("DateExpires")]
		[Bindable(true)]
		public DateTime? DateExpires 
		{
			get { return GetColumnValue<DateTime?>(Columns.DateExpires); }
			set { SetColumnValue(Columns.DateExpires, value); }
		}
		  
		[XmlAttribute("LastEditedBy")]
		[Bindable(true)]
		public string LastEditedBy 
		{
			get { return GetColumnValue<string>(Columns.LastEditedBy); }
			set { SetColumnValue(Columns.LastEditedBy, value); }
		}
		  
		[XmlAttribute("ExternalLink")]
		[Bindable(true)]
		public string ExternalLink 
		{
			get { return GetColumnValue<string>(Columns.ExternalLink); }
			set { SetColumnValue(Columns.ExternalLink, value); }
		}
		  
		[XmlAttribute("Status")]
		[Bindable(true)]
		public string Status 
		{
			get { return GetColumnValue<string>(Columns.Status); }
			set { SetColumnValue(Columns.Status, value); }
		}
		  
		[XmlAttribute("ListOrder")]
		[Bindable(true)]
		public int ListOrder 
		{
			get { return GetColumnValue<int>(Columns.ListOrder); }
			set { SetColumnValue(Columns.ListOrder, value); }
		}
		  
		[XmlAttribute("CallOut")]
		[Bindable(true)]
		public string CallOut 
		{
			get { return GetColumnValue<string>(Columns.CallOut); }
			set { SetColumnValue(Columns.CallOut, value); }
		}
		  
		[XmlAttribute("CreatedOn")]
		[Bindable(true)]
		public DateTime? CreatedOn 
		{
			get { return GetColumnValue<DateTime?>(Columns.CreatedOn); }
			set { SetColumnValue(Columns.CreatedOn, value); }
		}
		  
		[XmlAttribute("CreatedBy")]
		[Bindable(true)]
		public string CreatedBy 
		{
			get { return GetColumnValue<string>(Columns.CreatedBy); }
			set { SetColumnValue(Columns.CreatedBy, value); }
		}
		  
		[XmlAttribute("ModifiedOn")]
		[Bindable(true)]
		public DateTime? ModifiedOn 
		{
			get { return GetColumnValue<DateTime?>(Columns.ModifiedOn); }
			set { SetColumnValue(Columns.ModifiedOn, value); }
		}
		  
		[XmlAttribute("ModifiedBy")]
		[Bindable(true)]
		public string ModifiedBy 
		{
			get { return GetColumnValue<string>(Columns.ModifiedBy); }
			set { SetColumnValue(Columns.ModifiedBy, value); }
		}
		
		#endregion
		
		
			
		
		//no foreign key tables defined (0)
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(Guid varContentGUID,string varTitle,string varContentName,string varContent,string varIconPath,DateTime? varDateExpires,string varLastEditedBy,string varExternalLink,string varStatus,int varListOrder,string varCallOut,DateTime? varCreatedOn,string varCreatedBy,DateTime? varModifiedOn,string varModifiedBy)
		{
			TextEntry item = new TextEntry();
			
			item.ContentGUID = varContentGUID;
			
			item.Title = varTitle;
			
			item.ContentName = varContentName;
			
			item.Content = varContent;
			
			item.IconPath = varIconPath;
			
			item.DateExpires = varDateExpires;
			
			item.LastEditedBy = varLastEditedBy;
			
			item.ExternalLink = varExternalLink;
			
			item.Status = varStatus;
			
			item.ListOrder = varListOrder;
			
			item.CallOut = varCallOut;
			
			item.CreatedOn = varCreatedOn;
			
			item.CreatedBy = varCreatedBy;
			
			item.ModifiedOn = varModifiedOn;
			
			item.ModifiedBy = varModifiedBy;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varContentID,Guid varContentGUID,string varTitle,string varContentName,string varContent,string varIconPath,DateTime? varDateExpires,string varLastEditedBy,string varExternalLink,string varStatus,int varListOrder,string varCallOut,DateTime? varCreatedOn,string varCreatedBy,DateTime? varModifiedOn,string varModifiedBy)
		{
			TextEntry item = new TextEntry();
			
				item.ContentID = varContentID;
			
				item.ContentGUID = varContentGUID;
			
				item.Title = varTitle;
			
				item.ContentName = varContentName;
			
				item.Content = varContent;
			
				item.IconPath = varIconPath;
			
				item.DateExpires = varDateExpires;
			
				item.LastEditedBy = varLastEditedBy;
			
				item.ExternalLink = varExternalLink;
			
				item.Status = varStatus;
			
				item.ListOrder = varListOrder;
			
				item.CallOut = varCallOut;
			
				item.CreatedOn = varCreatedOn;
			
				item.CreatedBy = varCreatedBy;
			
				item.ModifiedOn = varModifiedOn;
			
				item.ModifiedBy = varModifiedBy;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn ContentIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn ContentGUIDColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn TitleColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn ContentNameColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn ContentColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn IconPathColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn DateExpiresColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn LastEditedByColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn ExternalLinkColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn StatusColumn
        {
            get { return Schema.Columns[9]; }
        }
        
        
        
        public static TableSchema.TableColumn ListOrderColumn
        {
            get { return Schema.Columns[10]; }
        }
        
        
        
        public static TableSchema.TableColumn CallOutColumn
        {
            get { return Schema.Columns[11]; }
        }
        
        
        
        public static TableSchema.TableColumn CreatedOnColumn
        {
            get { return Schema.Columns[12]; }
        }
        
        
        
        public static TableSchema.TableColumn CreatedByColumn
        {
            get { return Schema.Columns[13]; }
        }
        
        
        
        public static TableSchema.TableColumn ModifiedOnColumn
        {
            get { return Schema.Columns[14]; }
        }
        
        
        
        public static TableSchema.TableColumn ModifiedByColumn
        {
            get { return Schema.Columns[15]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string ContentID = @"contentID";
			 public static string ContentGUID = @"contentGUID";
			 public static string Title = @"title";
			 public static string ContentName = @"contentName";
			 public static string Content = @"content";
			 public static string IconPath = @"iconPath";
			 public static string DateExpires = @"dateExpires";
			 public static string LastEditedBy = @"lastEditedBy";
			 public static string ExternalLink = @"externalLink";
			 public static string Status = @"status";
			 public static string ListOrder = @"listOrder";
			 public static string CallOut = @"callOut";
			 public static string CreatedOn = @"createdOn";
			 public static string CreatedBy = @"createdBy";
			 public static string ModifiedOn = @"modifiedOn";
			 public static string ModifiedBy = @"modifiedBy";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}
