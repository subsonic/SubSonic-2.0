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
	/// Strongly-typed collection for the Product class.
	/// </summary>
    [Serializable]
	public partial class ProductCollection : ActiveList<Product, ProductCollection>
	{	   
		public ProductCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>ProductCollection</returns>
		public ProductCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Product o = this[i];
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
	/// This is an ActiveRecord class which wraps the Products table.
	/// </summary>
	[Serializable]
	public partial class Product : ActiveRecord<Product>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Product()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Product(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Product(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Product(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Products", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarProductID = new TableSchema.TableColumn(schema);
				colvarProductID.ColumnName = "ProductID";
				colvarProductID.DataType = DbType.Int32;
				colvarProductID.MaxLength = 0;
				colvarProductID.AutoIncrement = true;
				colvarProductID.IsNullable = false;
				colvarProductID.IsPrimaryKey = true;
				colvarProductID.IsForeignKey = false;
				colvarProductID.IsReadOnly = false;
				colvarProductID.DefaultSetting = @"";
				colvarProductID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarProductID);
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = false;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				colvarProductName.DefaultSetting = @"";
				colvarProductName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarProductName);
				
				TableSchema.TableColumn colvarSupplierID = new TableSchema.TableColumn(schema);
				colvarSupplierID.ColumnName = "SupplierID";
				colvarSupplierID.DataType = DbType.Int32;
				colvarSupplierID.MaxLength = 0;
				colvarSupplierID.AutoIncrement = false;
				colvarSupplierID.IsNullable = true;
				colvarSupplierID.IsPrimaryKey = false;
				colvarSupplierID.IsForeignKey = true;
				colvarSupplierID.IsReadOnly = false;
				colvarSupplierID.DefaultSetting = @"";
				
					colvarSupplierID.ForeignKeyTableName = "Suppliers";
				schema.Columns.Add(colvarSupplierID);
				
				TableSchema.TableColumn colvarCategoryID = new TableSchema.TableColumn(schema);
				colvarCategoryID.ColumnName = "CategoryID";
				colvarCategoryID.DataType = DbType.Int32;
				colvarCategoryID.MaxLength = 0;
				colvarCategoryID.AutoIncrement = false;
				colvarCategoryID.IsNullable = true;
				colvarCategoryID.IsPrimaryKey = false;
				colvarCategoryID.IsForeignKey = true;
				colvarCategoryID.IsReadOnly = false;
				colvarCategoryID.DefaultSetting = @"";
				
					colvarCategoryID.ForeignKeyTableName = "Categories";
				schema.Columns.Add(colvarCategoryID);
				
				TableSchema.TableColumn colvarQuantityPerUnit = new TableSchema.TableColumn(schema);
				colvarQuantityPerUnit.ColumnName = "QuantityPerUnit";
				colvarQuantityPerUnit.DataType = DbType.String;
				colvarQuantityPerUnit.MaxLength = 20;
				colvarQuantityPerUnit.AutoIncrement = false;
				colvarQuantityPerUnit.IsNullable = true;
				colvarQuantityPerUnit.IsPrimaryKey = false;
				colvarQuantityPerUnit.IsForeignKey = false;
				colvarQuantityPerUnit.IsReadOnly = false;
				colvarQuantityPerUnit.DefaultSetting = @"";
				colvarQuantityPerUnit.ForeignKeyTableName = "";
				schema.Columns.Add(colvarQuantityPerUnit);
				
				TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
				colvarUnitPrice.ColumnName = "UnitPrice";
				colvarUnitPrice.DataType = DbType.Currency;
				colvarUnitPrice.MaxLength = 0;
				colvarUnitPrice.AutoIncrement = false;
				colvarUnitPrice.IsNullable = true;
				colvarUnitPrice.IsPrimaryKey = false;
				colvarUnitPrice.IsForeignKey = false;
				colvarUnitPrice.IsReadOnly = false;
				
						colvarUnitPrice.DefaultSetting = @"((0))";
				colvarUnitPrice.ForeignKeyTableName = "";
				schema.Columns.Add(colvarUnitPrice);
				
				TableSchema.TableColumn colvarUnitsInStock = new TableSchema.TableColumn(schema);
				colvarUnitsInStock.ColumnName = "UnitsInStock";
				colvarUnitsInStock.DataType = DbType.Int16;
				colvarUnitsInStock.MaxLength = 0;
				colvarUnitsInStock.AutoIncrement = false;
				colvarUnitsInStock.IsNullable = true;
				colvarUnitsInStock.IsPrimaryKey = false;
				colvarUnitsInStock.IsForeignKey = false;
				colvarUnitsInStock.IsReadOnly = false;
				
						colvarUnitsInStock.DefaultSetting = @"((0))";
				colvarUnitsInStock.ForeignKeyTableName = "";
				schema.Columns.Add(colvarUnitsInStock);
				
				TableSchema.TableColumn colvarUnitsOnOrder = new TableSchema.TableColumn(schema);
				colvarUnitsOnOrder.ColumnName = "UnitsOnOrder";
				colvarUnitsOnOrder.DataType = DbType.Int16;
				colvarUnitsOnOrder.MaxLength = 0;
				colvarUnitsOnOrder.AutoIncrement = false;
				colvarUnitsOnOrder.IsNullable = true;
				colvarUnitsOnOrder.IsPrimaryKey = false;
				colvarUnitsOnOrder.IsForeignKey = false;
				colvarUnitsOnOrder.IsReadOnly = false;
				
						colvarUnitsOnOrder.DefaultSetting = @"((0))";
				colvarUnitsOnOrder.ForeignKeyTableName = "";
				schema.Columns.Add(colvarUnitsOnOrder);
				
				TableSchema.TableColumn colvarReorderLevel = new TableSchema.TableColumn(schema);
				colvarReorderLevel.ColumnName = "ReorderLevel";
				colvarReorderLevel.DataType = DbType.Int16;
				colvarReorderLevel.MaxLength = 0;
				colvarReorderLevel.AutoIncrement = false;
				colvarReorderLevel.IsNullable = true;
				colvarReorderLevel.IsPrimaryKey = false;
				colvarReorderLevel.IsForeignKey = false;
				colvarReorderLevel.IsReadOnly = false;
				
						colvarReorderLevel.DefaultSetting = @"((0))";
				colvarReorderLevel.ForeignKeyTableName = "";
				schema.Columns.Add(colvarReorderLevel);
				
				TableSchema.TableColumn colvarDiscontinued = new TableSchema.TableColumn(schema);
				colvarDiscontinued.ColumnName = "Discontinued";
				colvarDiscontinued.DataType = DbType.Boolean;
				colvarDiscontinued.MaxLength = 0;
				colvarDiscontinued.AutoIncrement = false;
				colvarDiscontinued.IsNullable = false;
				colvarDiscontinued.IsPrimaryKey = false;
				colvarDiscontinued.IsForeignKey = false;
				colvarDiscontinued.IsReadOnly = false;
				
						colvarDiscontinued.DefaultSetting = @"((0))";
				colvarDiscontinued.ForeignKeyTableName = "";
				schema.Columns.Add(colvarDiscontinued);
				
				TableSchema.TableColumn colvarAttributeXML = new TableSchema.TableColumn(schema);
				colvarAttributeXML.ColumnName = "AttributeXML";
				colvarAttributeXML.DataType = DbType.AnsiString;
				colvarAttributeXML.MaxLength = -1;
				colvarAttributeXML.AutoIncrement = false;
				colvarAttributeXML.IsNullable = true;
				colvarAttributeXML.IsPrimaryKey = false;
				colvarAttributeXML.IsForeignKey = false;
				colvarAttributeXML.IsReadOnly = false;
				colvarAttributeXML.DefaultSetting = @"";
				colvarAttributeXML.ForeignKeyTableName = "";
				schema.Columns.Add(colvarAttributeXML);
				
				TableSchema.TableColumn colvarDateCreated = new TableSchema.TableColumn(schema);
				colvarDateCreated.ColumnName = "DateCreated";
				colvarDateCreated.DataType = DbType.DateTime;
				colvarDateCreated.MaxLength = 0;
				colvarDateCreated.AutoIncrement = false;
				colvarDateCreated.IsNullable = true;
				colvarDateCreated.IsPrimaryKey = false;
				colvarDateCreated.IsForeignKey = false;
				colvarDateCreated.IsReadOnly = false;
				
						colvarDateCreated.DefaultSetting = @"(getdate())";
				colvarDateCreated.ForeignKeyTableName = "";
				schema.Columns.Add(colvarDateCreated);
				
				TableSchema.TableColumn colvarProductGUID = new TableSchema.TableColumn(schema);
				colvarProductGUID.ColumnName = "ProductGUID";
				colvarProductGUID.DataType = DbType.Guid;
				colvarProductGUID.MaxLength = 0;
				colvarProductGUID.AutoIncrement = false;
				colvarProductGUID.IsNullable = true;
				colvarProductGUID.IsPrimaryKey = false;
				colvarProductGUID.IsForeignKey = false;
				colvarProductGUID.IsReadOnly = false;
				
						colvarProductGUID.DefaultSetting = @"(newid())";
				colvarProductGUID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarProductGUID);
				
				TableSchema.TableColumn colvarCreatedOn = new TableSchema.TableColumn(schema);
				colvarCreatedOn.ColumnName = "CreatedOn";
				colvarCreatedOn.DataType = DbType.DateTime;
				colvarCreatedOn.MaxLength = 0;
				colvarCreatedOn.AutoIncrement = false;
				colvarCreatedOn.IsNullable = false;
				colvarCreatedOn.IsPrimaryKey = false;
				colvarCreatedOn.IsForeignKey = false;
				colvarCreatedOn.IsReadOnly = false;
				
						colvarCreatedOn.DefaultSetting = @"(getdate())";
				colvarCreatedOn.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCreatedOn);
				
				TableSchema.TableColumn colvarCreatedBy = new TableSchema.TableColumn(schema);
				colvarCreatedBy.ColumnName = "CreatedBy";
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
				colvarModifiedOn.ColumnName = "ModifiedOn";
				colvarModifiedOn.DataType = DbType.DateTime;
				colvarModifiedOn.MaxLength = 0;
				colvarModifiedOn.AutoIncrement = false;
				colvarModifiedOn.IsNullable = false;
				colvarModifiedOn.IsPrimaryKey = false;
				colvarModifiedOn.IsForeignKey = false;
				colvarModifiedOn.IsReadOnly = false;
				
						colvarModifiedOn.DefaultSetting = @"(getdate())";
				colvarModifiedOn.ForeignKeyTableName = "";
				schema.Columns.Add(colvarModifiedOn);
				
				TableSchema.TableColumn colvarModifiedBy = new TableSchema.TableColumn(schema);
				colvarModifiedBy.ColumnName = "ModifiedBy";
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
				
				TableSchema.TableColumn colvarDeleted = new TableSchema.TableColumn(schema);
				colvarDeleted.ColumnName = "Deleted";
				colvarDeleted.DataType = DbType.Boolean;
				colvarDeleted.MaxLength = 0;
				colvarDeleted.AutoIncrement = false;
				colvarDeleted.IsNullable = false;
				colvarDeleted.IsPrimaryKey = false;
				colvarDeleted.IsForeignKey = false;
				colvarDeleted.IsReadOnly = false;
				
						colvarDeleted.DefaultSetting = @"((0))";
				colvarDeleted.ForeignKeyTableName = "";
				schema.Columns.Add(colvarDeleted);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("Products",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("ProductID")]
		[Bindable(true)]
		public int ProductID 
		{
			get { return GetColumnValue<int>(Columns.ProductID); }
			set { SetColumnValue(Columns.ProductID, value); }
		}
		  
		[XmlAttribute("ProductName")]
		[Bindable(true)]
		public string ProductName 
		{
			get { return GetColumnValue<string>(Columns.ProductName); }
			set { SetColumnValue(Columns.ProductName, value); }
		}
		  
		[XmlAttribute("SupplierID")]
		[Bindable(true)]
		public int? SupplierID 
		{
			get { return GetColumnValue<int?>(Columns.SupplierID); }
			set { SetColumnValue(Columns.SupplierID, value); }
		}
		  
		[XmlAttribute("CategoryID")]
		[Bindable(true)]
		public int? CategoryID 
		{
			get { return GetColumnValue<int?>(Columns.CategoryID); }
			set { SetColumnValue(Columns.CategoryID, value); }
		}
		  
		[XmlAttribute("QuantityPerUnit")]
		[Bindable(true)]
		public string QuantityPerUnit 
		{
			get { return GetColumnValue<string>(Columns.QuantityPerUnit); }
			set { SetColumnValue(Columns.QuantityPerUnit, value); }
		}
		  
		[XmlAttribute("UnitPrice")]
		[Bindable(true)]
		public decimal? UnitPrice 
		{
			get { return GetColumnValue<decimal?>(Columns.UnitPrice); }
			set { SetColumnValue(Columns.UnitPrice, value); }
		}
		  
		[XmlAttribute("UnitsInStock")]
		[Bindable(true)]
		public short? UnitsInStock 
		{
			get { return GetColumnValue<short?>(Columns.UnitsInStock); }
			set { SetColumnValue(Columns.UnitsInStock, value); }
		}
		  
		[XmlAttribute("UnitsOnOrder")]
		[Bindable(true)]
		public short? UnitsOnOrder 
		{
			get { return GetColumnValue<short?>(Columns.UnitsOnOrder); }
			set { SetColumnValue(Columns.UnitsOnOrder, value); }
		}
		  
		[XmlAttribute("ReorderLevel")]
		[Bindable(true)]
		public short? ReorderLevel 
		{
			get { return GetColumnValue<short?>(Columns.ReorderLevel); }
			set { SetColumnValue(Columns.ReorderLevel, value); }
		}
		  
		[XmlAttribute("Discontinued")]
		[Bindable(true)]
		public bool Discontinued 
		{
			get { return GetColumnValue<bool>(Columns.Discontinued); }
			set { SetColumnValue(Columns.Discontinued, value); }
		}
		  
		[XmlAttribute("AttributeXML")]
		[Bindable(true)]
		public string AttributeXML 
		{
			get { return GetColumnValue<string>(Columns.AttributeXML); }
			set { SetColumnValue(Columns.AttributeXML, value); }
		}
		  
		[XmlAttribute("DateCreated")]
		[Bindable(true)]
		public DateTime? DateCreated 
		{
			get { return GetColumnValue<DateTime?>(Columns.DateCreated); }
			set { SetColumnValue(Columns.DateCreated, value); }
		}
		  
		[XmlAttribute("ProductGUID")]
		[Bindable(true)]
		public Guid? ProductGUID 
		{
			get { return GetColumnValue<Guid?>(Columns.ProductGUID); }
			set { SetColumnValue(Columns.ProductGUID, value); }
		}
		  
		[XmlAttribute("CreatedOn")]
		[Bindable(true)]
		public DateTime CreatedOn 
		{
			get { return GetColumnValue<DateTime>(Columns.CreatedOn); }
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
		public DateTime ModifiedOn 
		{
			get { return GetColumnValue<DateTime>(Columns.ModifiedOn); }
			set { SetColumnValue(Columns.ModifiedOn, value); }
		}
		  
		[XmlAttribute("ModifiedBy")]
		[Bindable(true)]
		public string ModifiedBy 
		{
			get { return GetColumnValue<string>(Columns.ModifiedBy); }
			set { SetColumnValue(Columns.ModifiedBy, value); }
		}
		  
		[XmlAttribute("Deleted")]
		[Bindable(true)]
		public bool Deleted 
		{
			get { return GetColumnValue<bool>(Columns.Deleted); }
			set { SetColumnValue(Columns.Deleted, value); }
		}
		
		#endregion
		
		
		#region PrimaryKey Methods		
		
        protected override void SetPrimaryKey(object oValue)
        {
            base.SetPrimaryKey(oValue);
            
            SetPKValues();
        }
        
		
		public Northwind.Order_DetailCollection Order_Details()
		{
			return new Northwind.Order_DetailCollection().Where(Order_Detail.Columns.ProductID, ProductID).Load();
		}
		public Northwind.Product_Category_MapCollection Product_Category_MapRecords()
		{
			return new Northwind.Product_Category_MapCollection().Where(Product_Category_Map.Columns.ProductID, ProductID).Load();
		}
		#endregion
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a Category ActiveRecord object related to this Product
		/// 
		/// </summary>
		public Northwind.Category Category
		{
			get { return Northwind.Category.FetchByID(this.CategoryID); }
			set { SetColumnValue("CategoryID", value.CategoryID); }
		}
		
		
		/// <summary>
		/// Returns a Supplier ActiveRecord object related to this Product
		/// 
		/// </summary>
		public Northwind.Supplier Supplier
		{
			get { return Northwind.Supplier.FetchByID(this.SupplierID); }
			set { SetColumnValue("SupplierID", value.SupplierID); }
		}
		
		
		#endregion
		
		
		
		#region Many To Many Helpers
		
		 
		public Northwind.OrderCollection GetOrderCollection() { return Product.GetOrderCollection(this.ProductID); }
		public static Northwind.OrderCollection GetOrderCollection(int varProductID)
		{
		    SubSonic.QueryCommand cmd = new SubSonic.QueryCommand("SELECT * FROM [dbo].[Orders] INNER JOIN [Order Details] ON [Orders].[OrderID] = [Order Details].[OrderID] WHERE [Order Details].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmd.AddParameter("@ProductID", varProductID, DbType.Int32);
			IDataReader rdr = SubSonic.DataService.GetReader(cmd);
			OrderCollection coll = new OrderCollection();
			coll.LoadAndCloseReader(rdr);
			return coll;
		}
		
		public static void SaveOrderMap(int varProductID, OrderCollection items)
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [Order Details] WHERE [Order Details].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmdDel.AddParameter("@ProductID", varProductID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (Order item in items)
			{
				Order_Detail varOrder_Detail = new Order_Detail();
				varOrder_Detail.SetColumnValue("ProductID", varProductID);
				varOrder_Detail.SetColumnValue("OrderID", item.GetPrimaryKeyValue());
				varOrder_Detail.Save();
			}
		}
		public static void SaveOrderMap(int varProductID, System.Web.UI.WebControls.ListItemCollection itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [Order Details] WHERE [Order Details].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmdDel.AddParameter("@ProductID", varProductID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (System.Web.UI.WebControls.ListItem l in itemList) 
			{
				if (l.Selected) 
				{
					Order_Detail varOrder_Detail = new Order_Detail();
					varOrder_Detail.SetColumnValue("ProductID", varProductID);
					varOrder_Detail.SetColumnValue("OrderID", l.Value);
					varOrder_Detail.Save();
				}
			}
		}
		public static void SaveOrderMap(int varProductID , int[] itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [Order Details] WHERE [Order Details].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmdDel.AddParameter("@ProductID", varProductID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (int item in itemList) 
			{
				Order_Detail varOrder_Detail = new Order_Detail();
				varOrder_Detail.SetColumnValue("ProductID", varProductID);
				varOrder_Detail.SetColumnValue("OrderID", item);
				varOrder_Detail.Save();
			}
		}
		
		public static void DeleteOrderMap(int varProductID) 
		{
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [Order Details] WHERE [Order Details].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmdDel.AddParameter("@ProductID", varProductID, DbType.Int32);
			DataService.ExecuteQuery(cmdDel);
		}
		
		 
		public Northwind.CategoryCollection GetCategoryCollection() { return Product.GetCategoryCollection(this.ProductID); }
		public static Northwind.CategoryCollection GetCategoryCollection(int varProductID)
		{
		    SubSonic.QueryCommand cmd = new SubSonic.QueryCommand("SELECT * FROM [dbo].[Categories] INNER JOIN [Product_Category_Map] ON [Categories].[CategoryID] = [Product_Category_Map].[CategoryID] WHERE [Product_Category_Map].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmd.AddParameter("@ProductID", varProductID, DbType.Int32);
			IDataReader rdr = SubSonic.DataService.GetReader(cmd);
			CategoryCollection coll = new CategoryCollection();
			coll.LoadAndCloseReader(rdr);
			return coll;
		}
		
		public static void SaveCategoryMap(int varProductID, CategoryCollection items)
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [Product_Category_Map] WHERE [Product_Category_Map].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmdDel.AddParameter("@ProductID", varProductID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (Category item in items)
			{
				Product_Category_Map varProduct_Category_Map = new Product_Category_Map();
				varProduct_Category_Map.SetColumnValue("ProductID", varProductID);
				varProduct_Category_Map.SetColumnValue("CategoryID", item.GetPrimaryKeyValue());
				varProduct_Category_Map.Save();
			}
		}
		public static void SaveCategoryMap(int varProductID, System.Web.UI.WebControls.ListItemCollection itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [Product_Category_Map] WHERE [Product_Category_Map].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmdDel.AddParameter("@ProductID", varProductID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (System.Web.UI.WebControls.ListItem l in itemList) 
			{
				if (l.Selected) 
				{
					Product_Category_Map varProduct_Category_Map = new Product_Category_Map();
					varProduct_Category_Map.SetColumnValue("ProductID", varProductID);
					varProduct_Category_Map.SetColumnValue("CategoryID", l.Value);
					varProduct_Category_Map.Save();
				}
			}
		}
		public static void SaveCategoryMap(int varProductID , int[] itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [Product_Category_Map] WHERE [Product_Category_Map].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmdDel.AddParameter("@ProductID", varProductID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (int item in itemList) 
			{
				Product_Category_Map varProduct_Category_Map = new Product_Category_Map();
				varProduct_Category_Map.SetColumnValue("ProductID", varProductID);
				varProduct_Category_Map.SetColumnValue("CategoryID", item);
				varProduct_Category_Map.Save();
			}
		}
		
		public static void DeleteCategoryMap(int varProductID) 
		{
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [Product_Category_Map] WHERE [Product_Category_Map].[ProductID] = @ProductID", Product.Schema.Provider.Name);
			cmdDel.AddParameter("@ProductID", varProductID, DbType.Int32);
			DataService.ExecuteQuery(cmdDel);
		}
		
		#endregion
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varProductName,int? varSupplierID,int? varCategoryID,string varQuantityPerUnit,decimal? varUnitPrice,short? varUnitsInStock,short? varUnitsOnOrder,short? varReorderLevel,bool varDiscontinued,string varAttributeXML,DateTime? varDateCreated,Guid? varProductGUID,DateTime varCreatedOn,string varCreatedBy,DateTime varModifiedOn,string varModifiedBy,bool varDeleted)
		{
			Product item = new Product();
			
			item.ProductName = varProductName;
			
			item.SupplierID = varSupplierID;
			
			item.CategoryID = varCategoryID;
			
			item.QuantityPerUnit = varQuantityPerUnit;
			
			item.UnitPrice = varUnitPrice;
			
			item.UnitsInStock = varUnitsInStock;
			
			item.UnitsOnOrder = varUnitsOnOrder;
			
			item.ReorderLevel = varReorderLevel;
			
			item.Discontinued = varDiscontinued;
			
			item.AttributeXML = varAttributeXML;
			
			item.DateCreated = varDateCreated;
			
			item.ProductGUID = varProductGUID;
			
			item.CreatedOn = varCreatedOn;
			
			item.CreatedBy = varCreatedBy;
			
			item.ModifiedOn = varModifiedOn;
			
			item.ModifiedBy = varModifiedBy;
			
			item.Deleted = varDeleted;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varProductID,string varProductName,int? varSupplierID,int? varCategoryID,string varQuantityPerUnit,decimal? varUnitPrice,short? varUnitsInStock,short? varUnitsOnOrder,short? varReorderLevel,bool varDiscontinued,string varAttributeXML,DateTime? varDateCreated,Guid? varProductGUID,DateTime varCreatedOn,string varCreatedBy,DateTime varModifiedOn,string varModifiedBy,bool varDeleted)
		{
			Product item = new Product();
			
				item.ProductID = varProductID;
			
				item.ProductName = varProductName;
			
				item.SupplierID = varSupplierID;
			
				item.CategoryID = varCategoryID;
			
				item.QuantityPerUnit = varQuantityPerUnit;
			
				item.UnitPrice = varUnitPrice;
			
				item.UnitsInStock = varUnitsInStock;
			
				item.UnitsOnOrder = varUnitsOnOrder;
			
				item.ReorderLevel = varReorderLevel;
			
				item.Discontinued = varDiscontinued;
			
				item.AttributeXML = varAttributeXML;
			
				item.DateCreated = varDateCreated;
			
				item.ProductGUID = varProductGUID;
			
				item.CreatedOn = varCreatedOn;
			
				item.CreatedBy = varCreatedBy;
			
				item.ModifiedOn = varModifiedOn;
			
				item.ModifiedBy = varModifiedBy;
			
				item.Deleted = varDeleted;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn ProductIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn ProductNameColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn SupplierIDColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn CategoryIDColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn QuantityPerUnitColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn UnitPriceColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn UnitsInStockColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn UnitsOnOrderColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn ReorderLevelColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn DiscontinuedColumn
        {
            get { return Schema.Columns[9]; }
        }
        
        
        
        public static TableSchema.TableColumn AttributeXMLColumn
        {
            get { return Schema.Columns[10]; }
        }
        
        
        
        public static TableSchema.TableColumn DateCreatedColumn
        {
            get { return Schema.Columns[11]; }
        }
        
        
        
        public static TableSchema.TableColumn ProductGUIDColumn
        {
            get { return Schema.Columns[12]; }
        }
        
        
        
        public static TableSchema.TableColumn CreatedOnColumn
        {
            get { return Schema.Columns[13]; }
        }
        
        
        
        public static TableSchema.TableColumn CreatedByColumn
        {
            get { return Schema.Columns[14]; }
        }
        
        
        
        public static TableSchema.TableColumn ModifiedOnColumn
        {
            get { return Schema.Columns[15]; }
        }
        
        
        
        public static TableSchema.TableColumn ModifiedByColumn
        {
            get { return Schema.Columns[16]; }
        }
        
        
        
        public static TableSchema.TableColumn DeletedColumn
        {
            get { return Schema.Columns[17]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string ProductID = @"ProductID";
			 public static string ProductName = @"ProductName";
			 public static string SupplierID = @"SupplierID";
			 public static string CategoryID = @"CategoryID";
			 public static string QuantityPerUnit = @"QuantityPerUnit";
			 public static string UnitPrice = @"UnitPrice";
			 public static string UnitsInStock = @"UnitsInStock";
			 public static string UnitsOnOrder = @"UnitsOnOrder";
			 public static string ReorderLevel = @"ReorderLevel";
			 public static string Discontinued = @"Discontinued";
			 public static string AttributeXML = @"AttributeXML";
			 public static string DateCreated = @"DateCreated";
			 public static string ProductGUID = @"ProductGUID";
			 public static string CreatedOn = @"CreatedOn";
			 public static string CreatedBy = @"CreatedBy";
			 public static string ModifiedOn = @"ModifiedOn";
			 public static string ModifiedBy = @"ModifiedBy";
			 public static string Deleted = @"Deleted";
						
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
