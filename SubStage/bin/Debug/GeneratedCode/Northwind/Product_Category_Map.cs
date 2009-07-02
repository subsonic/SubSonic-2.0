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
	/// Strongly-typed collection for the Product_Category_Map class.
	/// </summary>
    [Serializable]
	public partial class Product_Category_MapCollection : ActiveList<Product_Category_Map, Product_Category_MapCollection>
	{	   
		public Product_Category_MapCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>Product_Category_MapCollection</returns>
		public Product_Category_MapCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Product_Category_Map o = this[i];
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
	/// This is an ActiveRecord class which wraps the Product_Category_Map table.
	/// </summary>
	[Serializable]
	public partial class Product_Category_Map : ActiveRecord<Product_Category_Map>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Product_Category_Map()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Product_Category_Map(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Product_Category_Map(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Product_Category_Map(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Product_Category_Map", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarCategoryID = new TableSchema.TableColumn(schema);
				colvarCategoryID.ColumnName = "CategoryID";
				colvarCategoryID.DataType = DbType.Int32;
				colvarCategoryID.MaxLength = 0;
				colvarCategoryID.AutoIncrement = false;
				colvarCategoryID.IsNullable = false;
				colvarCategoryID.IsPrimaryKey = true;
				colvarCategoryID.IsForeignKey = true;
				colvarCategoryID.IsReadOnly = false;
				colvarCategoryID.DefaultSetting = @"";
				
					colvarCategoryID.ForeignKeyTableName = "Categories";
				schema.Columns.Add(colvarCategoryID);
				
				TableSchema.TableColumn colvarProductID = new TableSchema.TableColumn(schema);
				colvarProductID.ColumnName = "ProductID";
				colvarProductID.DataType = DbType.Int32;
				colvarProductID.MaxLength = 0;
				colvarProductID.AutoIncrement = false;
				colvarProductID.IsNullable = false;
				colvarProductID.IsPrimaryKey = true;
				colvarProductID.IsForeignKey = true;
				colvarProductID.IsReadOnly = false;
				colvarProductID.DefaultSetting = @"";
				
					colvarProductID.ForeignKeyTableName = "Products";
				schema.Columns.Add(colvarProductID);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("Product_Category_Map",schema);
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
		  
		[XmlAttribute("ProductID")]
		[Bindable(true)]
		public int ProductID 
		{
			get { return GetColumnValue<int>(Columns.ProductID); }
			set { SetColumnValue(Columns.ProductID, value); }
		}
		
		#endregion
		
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a Category ActiveRecord object related to this Product_Category_Map
		/// 
		/// </summary>
		public Northwind.Category Category
		{
			get { return Northwind.Category.FetchByID(this.CategoryID); }
			set { SetColumnValue("CategoryID", value.CategoryID); }
		}
		
		
		/// <summary>
		/// Returns a Product ActiveRecord object related to this Product_Category_Map
		/// 
		/// </summary>
		public Northwind.Product Product
		{
			get { return Northwind.Product.FetchByID(this.ProductID); }
			set { SetColumnValue("ProductID", value.ProductID); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(int varCategoryID,int varProductID)
		{
			Product_Category_Map item = new Product_Category_Map();
			
			item.CategoryID = varCategoryID;
			
			item.ProductID = varProductID;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varCategoryID,int varProductID)
		{
			Product_Category_Map item = new Product_Category_Map();
			
				item.CategoryID = varCategoryID;
			
				item.ProductID = varProductID;
			
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
        
        
        
        public static TableSchema.TableColumn ProductIDColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string CategoryID = @"CategoryID";
			 public static string ProductID = @"ProductID";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}
