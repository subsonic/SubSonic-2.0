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
	/// Strongly-typed collection for the Order_Detail class.
	/// </summary>
    [Serializable]
	public partial class Order_DetailCollection : ActiveList<Order_Detail, Order_DetailCollection>
	{	   
		public Order_DetailCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>Order_DetailCollection</returns>
		public Order_DetailCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Order_Detail o = this[i];
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
	/// This is an ActiveRecord class which wraps the Order Details table.
	/// </summary>
	[Serializable]
	public partial class Order_Detail : ActiveRecord<Order_Detail>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Order_Detail()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Order_Detail(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Order_Detail(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Order_Detail(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Order Details", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = false;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = true;
				colvarOrderID.IsForeignKey = true;
				colvarOrderID.IsReadOnly = false;
				colvarOrderID.DefaultSetting = @"";
				
					colvarOrderID.ForeignKeyTableName = "Orders";
				schema.Columns.Add(colvarOrderID);
				
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
				
				TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
				colvarUnitPrice.ColumnName = "UnitPrice";
				colvarUnitPrice.DataType = DbType.Currency;
				colvarUnitPrice.MaxLength = 0;
				colvarUnitPrice.AutoIncrement = false;
				colvarUnitPrice.IsNullable = false;
				colvarUnitPrice.IsPrimaryKey = false;
				colvarUnitPrice.IsForeignKey = false;
				colvarUnitPrice.IsReadOnly = false;
				
						colvarUnitPrice.DefaultSetting = @"((0))";
				colvarUnitPrice.ForeignKeyTableName = "";
				schema.Columns.Add(colvarUnitPrice);
				
				TableSchema.TableColumn colvarQuantity = new TableSchema.TableColumn(schema);
				colvarQuantity.ColumnName = "Quantity";
				colvarQuantity.DataType = DbType.Int16;
				colvarQuantity.MaxLength = 0;
				colvarQuantity.AutoIncrement = false;
				colvarQuantity.IsNullable = false;
				colvarQuantity.IsPrimaryKey = false;
				colvarQuantity.IsForeignKey = false;
				colvarQuantity.IsReadOnly = false;
				
						colvarQuantity.DefaultSetting = @"((1))";
				colvarQuantity.ForeignKeyTableName = "";
				schema.Columns.Add(colvarQuantity);
				
				TableSchema.TableColumn colvarDiscount = new TableSchema.TableColumn(schema);
				colvarDiscount.ColumnName = "Discount";
				colvarDiscount.DataType = DbType.Single;
				colvarDiscount.MaxLength = 0;
				colvarDiscount.AutoIncrement = false;
				colvarDiscount.IsNullable = false;
				colvarDiscount.IsPrimaryKey = false;
				colvarDiscount.IsForeignKey = false;
				colvarDiscount.IsReadOnly = false;
				
						colvarDiscount.DefaultSetting = @"((0))";
				colvarDiscount.ForeignKeyTableName = "";
				schema.Columns.Add(colvarDiscount);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("Order Details",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("OrderID")]
		[Bindable(true)]
		public int OrderID 
		{
			get { return GetColumnValue<int>(Columns.OrderID); }
			set { SetColumnValue(Columns.OrderID, value); }
		}
		  
		[XmlAttribute("ProductID")]
		[Bindable(true)]
		public int ProductID 
		{
			get { return GetColumnValue<int>(Columns.ProductID); }
			set { SetColumnValue(Columns.ProductID, value); }
		}
		  
		[XmlAttribute("UnitPrice")]
		[Bindable(true)]
		public decimal UnitPrice 
		{
			get { return GetColumnValue<decimal>(Columns.UnitPrice); }
			set { SetColumnValue(Columns.UnitPrice, value); }
		}
		  
		[XmlAttribute("Quantity")]
		[Bindable(true)]
		public short Quantity 
		{
			get { return GetColumnValue<short>(Columns.Quantity); }
			set { SetColumnValue(Columns.Quantity, value); }
		}
		  
		[XmlAttribute("Discount")]
		[Bindable(true)]
		public float Discount 
		{
			get { return GetColumnValue<float>(Columns.Discount); }
			set { SetColumnValue(Columns.Discount, value); }
		}
		
		#endregion
		
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a Order ActiveRecord object related to this Order_Detail
		/// 
		/// </summary>
		public Northwind.Order Order
		{
			get { return Northwind.Order.FetchByID(this.OrderID); }
			set { SetColumnValue("OrderID", value.OrderID); }
		}
		
		
		/// <summary>
		/// Returns a Product ActiveRecord object related to this Order_Detail
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
		public static void Insert(int varOrderID,int varProductID,decimal varUnitPrice,short varQuantity,float varDiscount)
		{
			Order_Detail item = new Order_Detail();
			
			item.OrderID = varOrderID;
			
			item.ProductID = varProductID;
			
			item.UnitPrice = varUnitPrice;
			
			item.Quantity = varQuantity;
			
			item.Discount = varDiscount;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varOrderID,int varProductID,decimal varUnitPrice,short varQuantity,float varDiscount)
		{
			Order_Detail item = new Order_Detail();
			
				item.OrderID = varOrderID;
			
				item.ProductID = varProductID;
			
				item.UnitPrice = varUnitPrice;
			
				item.Quantity = varQuantity;
			
				item.Discount = varDiscount;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn OrderIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn ProductIDColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn UnitPriceColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn QuantityColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn DiscountColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string OrderID = @"OrderID";
			 public static string ProductID = @"ProductID";
			 public static string UnitPrice = @"UnitPrice";
			 public static string Quantity = @"Quantity";
			 public static string Discount = @"Discount";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}
