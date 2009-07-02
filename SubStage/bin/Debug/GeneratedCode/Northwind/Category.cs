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
	/// Strongly-typed collection for the Category class.
	/// </summary>
    [Serializable]
	public partial class CategoryCollection : ActiveList<Category, CategoryCollection>
	{	   
		public CategoryCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>CategoryCollection</returns>
		public CategoryCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Category o = this[i];
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
	/// This is an ActiveRecord class which wraps the Categories table.
	/// </summary>
	[Serializable]
	public partial class Category : ActiveRecord<Category>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Category()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Category(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Category(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Category(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Categories", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarCategoryID = new TableSchema.TableColumn(schema);
				colvarCategoryID.ColumnName = "CategoryID";
				colvarCategoryID.DataType = DbType.Int32;
				colvarCategoryID.MaxLength = 0;
				colvarCategoryID.AutoIncrement = true;
				colvarCategoryID.IsNullable = false;
				colvarCategoryID.IsPrimaryKey = true;
				colvarCategoryID.IsForeignKey = false;
				colvarCategoryID.IsReadOnly = false;
				colvarCategoryID.DefaultSetting = @"";
				colvarCategoryID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCategoryID);
				
				TableSchema.TableColumn colvarCategoryName = new TableSchema.TableColumn(schema);
				colvarCategoryName.ColumnName = "CategoryName";
				colvarCategoryName.DataType = DbType.String;
				colvarCategoryName.MaxLength = 15;
				colvarCategoryName.AutoIncrement = false;
				colvarCategoryName.IsNullable = false;
				colvarCategoryName.IsPrimaryKey = false;
				colvarCategoryName.IsForeignKey = false;
				colvarCategoryName.IsReadOnly = false;
				colvarCategoryName.DefaultSetting = @"";
				colvarCategoryName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCategoryName);
				
				TableSchema.TableColumn colvarDescription = new TableSchema.TableColumn(schema);
				colvarDescription.ColumnName = "Description";
				colvarDescription.DataType = DbType.String;
				colvarDescription.MaxLength = 1073741823;
				colvarDescription.AutoIncrement = false;
				colvarDescription.IsNullable = true;
				colvarDescription.IsPrimaryKey = false;
				colvarDescription.IsForeignKey = false;
				colvarDescription.IsReadOnly = false;
				colvarDescription.DefaultSetting = @"";
				colvarDescription.ForeignKeyTableName = "";
				schema.Columns.Add(colvarDescription);
				
				TableSchema.TableColumn colvarPicture = new TableSchema.TableColumn(schema);
				colvarPicture.ColumnName = "Picture";
				colvarPicture.DataType = DbType.Binary;
				colvarPicture.MaxLength = 2147483647;
				colvarPicture.AutoIncrement = false;
				colvarPicture.IsNullable = true;
				colvarPicture.IsPrimaryKey = false;
				colvarPicture.IsForeignKey = false;
				colvarPicture.IsReadOnly = false;
				colvarPicture.DefaultSetting = @"";
				colvarPicture.ForeignKeyTableName = "";
				schema.Columns.Add(colvarPicture);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("Categories",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("CategoryID")]
		[Bindable(true)]
		public int CategoryID 
		{
			get { return GetColumnValue<int>(Columns.CategoryID); }
			set { SetColumnValue(Columns.CategoryID, value); }
		}
		  
		[XmlAttribute("CategoryName")]
		[Bindable(true)]
		public string CategoryName 
		{
			get { return GetColumnValue<string>(Columns.CategoryName); }
			set { SetColumnValue(Columns.CategoryName, value); }
		}
		  
		[XmlAttribute("Description")]
		[Bindable(true)]
		public string Description 
		{
			get { return GetColumnValue<string>(Columns.Description); }
			set { SetColumnValue(Columns.Description, value); }
		}
		  
		[XmlAttribute("Picture")]
		[Bindable(true)]
		public byte[] Picture 
		{
			get { return GetColumnValue<byte[]>(Columns.Picture); }
			set { SetColumnValue(Columns.Picture, value); }
		}
		
		#endregion
		
		
		#region PrimaryKey Methods		
		
        protected override void SetPrimaryKey(object oValue)
        {
            base.SetPrimaryKey(oValue);
            
            SetPKValues();
        }
        
		
		public Northwind.Product_Category_MapCollection Product_Category_MapRecords()
		{
			return new Northwind.Product_Category_MapCollection().Where(Product_Category_Map.Columns.CategoryID, CategoryID).Load();
		}
		public Northwind.ProductCollection Products()
		{
			return new Northwind.ProductCollection().Where(Product.Columns.CategoryID, CategoryID).Load();
		}
		#endregion
		
			
		
		//no foreign key tables defined (0)
		
		
		
		#region Many To Many Helpers
		
		 
		public Northwind.ProductCollection GetProductCollection() { return Category.GetProductCollection(this.CategoryID); }
		public static Northwind.ProductCollection GetProductCollection(int varCategoryID)
		{
		    SubSonic.QueryCommand cmd = new SubSonic.QueryCommand("SELECT * FROM [dbo].[Products] INNER JOIN [Product_Category_Map] ON [Products].[ProductID] = [Product_Category_Map].[ProductID] WHERE [Product_Category_Map].[CategoryID] = @CategoryID", Category.Schema.Provider.Name);
			cmd.AddParameter("@CategoryID", varCategoryID, DbType.Int32);
			IDataReader rdr = SubSonic.DataService.GetReader(cmd);
			ProductCollection coll = new ProductCollection();
			coll.LoadAndCloseReader(rdr);
			return coll;
		}
		
		public static void SaveProductMap(int varCategoryID, ProductCollection items)
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [Product_Category_Map] WHERE [Product_Category_Map].[CategoryID] = @CategoryID", Category.Schema.Provider.Name);
			cmdDel.AddParameter("@CategoryID", varCategoryID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (Product item in items)
			{
				Product_Category_Map varProduct_Category_Map = new Product_Category_Map();
				varProduct_Category_Map.SetColumnValue("CategoryID", varCategoryID);
				varProduct_Category_Map.SetColumnValue("ProductID", item.GetPrimaryKeyValue());
				varProduct_Category_Map.Save();
			}
		}
		public static void SaveProductMap(int varCategoryID, System.Web.UI.WebControls.ListItemCollection itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [Product_Category_Map] WHERE [Product_Category_Map].[CategoryID] = @CategoryID", Category.Schema.Provider.Name);
			cmdDel.AddParameter("@CategoryID", varCategoryID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (System.Web.UI.WebControls.ListItem l in itemList) 
			{
				if (l.Selected) 
				{
					Product_Category_Map varProduct_Category_Map = new Product_Category_Map();
					varProduct_Category_Map.SetColumnValue("CategoryID", varCategoryID);
					varProduct_Category_Map.SetColumnValue("ProductID", l.Value);
					varProduct_Category_Map.Save();
				}
			}
		}
		public static void SaveProductMap(int varCategoryID , int[] itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [Product_Category_Map] WHERE [Product_Category_Map].[CategoryID] = @CategoryID", Category.Schema.Provider.Name);
			cmdDel.AddParameter("@CategoryID", varCategoryID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (int item in itemList) 
			{
				Product_Category_Map varProduct_Category_Map = new Product_Category_Map();
				varProduct_Category_Map.SetColumnValue("CategoryID", varCategoryID);
				varProduct_Category_Map.SetColumnValue("ProductID", item);
				varProduct_Category_Map.Save();
			}
		}
		
		public static void DeleteProductMap(int varCategoryID) 
		{
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [Product_Category_Map] WHERE [Product_Category_Map].[CategoryID] = @CategoryID", Category.Schema.Provider.Name);
			cmdDel.AddParameter("@CategoryID", varCategoryID, DbType.Int32);
			DataService.ExecuteQuery(cmdDel);
		}
		
		#endregion
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varCategoryName,string varDescription,byte[] varPicture)
		{
			Category item = new Category();
			
			item.CategoryName = varCategoryName;
			
			item.Description = varDescription;
			
			item.Picture = varPicture;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varCategoryID,string varCategoryName,string varDescription,byte[] varPicture)
		{
			Category item = new Category();
			
				item.CategoryID = varCategoryID;
			
				item.CategoryName = varCategoryName;
			
				item.Description = varDescription;
			
				item.Picture = varPicture;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn CategoryIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn CategoryNameColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn DescriptionColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn PictureColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string CategoryID = @"CategoryID";
			 public static string CategoryName = @"CategoryName";
			 public static string Description = @"Description";
			 public static string Picture = @"Picture";
						
		}
		#endregion
		
		#region Update PK Collections
		
        public void SetPKValues()
        {
}
        #endregion
    
        #region Deep Save
		
        public void DeepSave()
        {
            Save();
            
}
        #endregion
	}
}
