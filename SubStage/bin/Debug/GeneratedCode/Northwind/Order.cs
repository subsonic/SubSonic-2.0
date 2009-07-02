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
	/// Strongly-typed collection for the Order class.
	/// </summary>
    [Serializable]
	public partial class OrderCollection : ActiveList<Order, OrderCollection>
	{	   
		public OrderCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>OrderCollection</returns>
		public OrderCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Order o = this[i];
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
	/// This is an ActiveRecord class which wraps the Orders table.
	/// </summary>
	[Serializable]
	public partial class Order : ActiveRecord<Order>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Order()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Order(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Order(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Order(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Orders", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = true;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				colvarOrderID.DefaultSetting = @"";
				colvarOrderID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarOrderID);
				
				TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
				colvarCustomerID.ColumnName = "CustomerID";
				colvarCustomerID.DataType = DbType.String;
				colvarCustomerID.MaxLength = 5;
				colvarCustomerID.AutoIncrement = false;
				colvarCustomerID.IsNullable = true;
				colvarCustomerID.IsPrimaryKey = false;
				colvarCustomerID.IsForeignKey = true;
				colvarCustomerID.IsReadOnly = false;
				colvarCustomerID.DefaultSetting = @"";
				
					colvarCustomerID.ForeignKeyTableName = "Customers";
				schema.Columns.Add(colvarCustomerID);
				
				TableSchema.TableColumn colvarEmployeeID = new TableSchema.TableColumn(schema);
				colvarEmployeeID.ColumnName = "EmployeeID";
				colvarEmployeeID.DataType = DbType.Int32;
				colvarEmployeeID.MaxLength = 0;
				colvarEmployeeID.AutoIncrement = false;
				colvarEmployeeID.IsNullable = true;
				colvarEmployeeID.IsPrimaryKey = false;
				colvarEmployeeID.IsForeignKey = true;
				colvarEmployeeID.IsReadOnly = false;
				colvarEmployeeID.DefaultSetting = @"";
				
					colvarEmployeeID.ForeignKeyTableName = "Employees";
				schema.Columns.Add(colvarEmployeeID);
				
				TableSchema.TableColumn colvarOrderDate = new TableSchema.TableColumn(schema);
				colvarOrderDate.ColumnName = "OrderDate";
				colvarOrderDate.DataType = DbType.DateTime;
				colvarOrderDate.MaxLength = 0;
				colvarOrderDate.AutoIncrement = false;
				colvarOrderDate.IsNullable = true;
				colvarOrderDate.IsPrimaryKey = false;
				colvarOrderDate.IsForeignKey = false;
				colvarOrderDate.IsReadOnly = false;
				colvarOrderDate.DefaultSetting = @"";
				colvarOrderDate.ForeignKeyTableName = "";
				schema.Columns.Add(colvarOrderDate);
				
				TableSchema.TableColumn colvarRequiredDate = new TableSchema.TableColumn(schema);
				colvarRequiredDate.ColumnName = "RequiredDate";
				colvarRequiredDate.DataType = DbType.DateTime;
				colvarRequiredDate.MaxLength = 0;
				colvarRequiredDate.AutoIncrement = false;
				colvarRequiredDate.IsNullable = true;
				colvarRequiredDate.IsPrimaryKey = false;
				colvarRequiredDate.IsForeignKey = false;
				colvarRequiredDate.IsReadOnly = false;
				colvarRequiredDate.DefaultSetting = @"";
				colvarRequiredDate.ForeignKeyTableName = "";
				schema.Columns.Add(colvarRequiredDate);
				
				TableSchema.TableColumn colvarShippedDate = new TableSchema.TableColumn(schema);
				colvarShippedDate.ColumnName = "ShippedDate";
				colvarShippedDate.DataType = DbType.DateTime;
				colvarShippedDate.MaxLength = 0;
				colvarShippedDate.AutoIncrement = false;
				colvarShippedDate.IsNullable = true;
				colvarShippedDate.IsPrimaryKey = false;
				colvarShippedDate.IsForeignKey = false;
				colvarShippedDate.IsReadOnly = false;
				colvarShippedDate.DefaultSetting = @"";
				colvarShippedDate.ForeignKeyTableName = "";
				schema.Columns.Add(colvarShippedDate);
				
				TableSchema.TableColumn colvarShipVia = new TableSchema.TableColumn(schema);
				colvarShipVia.ColumnName = "ShipVia";
				colvarShipVia.DataType = DbType.Int32;
				colvarShipVia.MaxLength = 0;
				colvarShipVia.AutoIncrement = false;
				colvarShipVia.IsNullable = true;
				colvarShipVia.IsPrimaryKey = false;
				colvarShipVia.IsForeignKey = true;
				colvarShipVia.IsReadOnly = false;
				colvarShipVia.DefaultSetting = @"";
				
					colvarShipVia.ForeignKeyTableName = "Shippers";
				schema.Columns.Add(colvarShipVia);
				
				TableSchema.TableColumn colvarFreight = new TableSchema.TableColumn(schema);
				colvarFreight.ColumnName = "Freight";
				colvarFreight.DataType = DbType.Currency;
				colvarFreight.MaxLength = 0;
				colvarFreight.AutoIncrement = false;
				colvarFreight.IsNullable = true;
				colvarFreight.IsPrimaryKey = false;
				colvarFreight.IsForeignKey = false;
				colvarFreight.IsReadOnly = false;
				
						colvarFreight.DefaultSetting = @"((0))";
				colvarFreight.ForeignKeyTableName = "";
				schema.Columns.Add(colvarFreight);
				
				TableSchema.TableColumn colvarShipName = new TableSchema.TableColumn(schema);
				colvarShipName.ColumnName = "ShipName";
				colvarShipName.DataType = DbType.String;
				colvarShipName.MaxLength = 40;
				colvarShipName.AutoIncrement = false;
				colvarShipName.IsNullable = true;
				colvarShipName.IsPrimaryKey = false;
				colvarShipName.IsForeignKey = false;
				colvarShipName.IsReadOnly = false;
				colvarShipName.DefaultSetting = @"";
				colvarShipName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarShipName);
				
				TableSchema.TableColumn colvarShipAddress = new TableSchema.TableColumn(schema);
				colvarShipAddress.ColumnName = "ShipAddress";
				colvarShipAddress.DataType = DbType.String;
				colvarShipAddress.MaxLength = 60;
				colvarShipAddress.AutoIncrement = false;
				colvarShipAddress.IsNullable = true;
				colvarShipAddress.IsPrimaryKey = false;
				colvarShipAddress.IsForeignKey = false;
				colvarShipAddress.IsReadOnly = false;
				colvarShipAddress.DefaultSetting = @"";
				colvarShipAddress.ForeignKeyTableName = "";
				schema.Columns.Add(colvarShipAddress);
				
				TableSchema.TableColumn colvarShipCity = new TableSchema.TableColumn(schema);
				colvarShipCity.ColumnName = "ShipCity";
				colvarShipCity.DataType = DbType.String;
				colvarShipCity.MaxLength = 15;
				colvarShipCity.AutoIncrement = false;
				colvarShipCity.IsNullable = true;
				colvarShipCity.IsPrimaryKey = false;
				colvarShipCity.IsForeignKey = false;
				colvarShipCity.IsReadOnly = false;
				colvarShipCity.DefaultSetting = @"";
				colvarShipCity.ForeignKeyTableName = "";
				schema.Columns.Add(colvarShipCity);
				
				TableSchema.TableColumn colvarShipRegion = new TableSchema.TableColumn(schema);
				colvarShipRegion.ColumnName = "ShipRegion";
				colvarShipRegion.DataType = DbType.String;
				colvarShipRegion.MaxLength = 15;
				colvarShipRegion.AutoIncrement = false;
				colvarShipRegion.IsNullable = true;
				colvarShipRegion.IsPrimaryKey = false;
				colvarShipRegion.IsForeignKey = false;
				colvarShipRegion.IsReadOnly = false;
				colvarShipRegion.DefaultSetting = @"";
				colvarShipRegion.ForeignKeyTableName = "";
				schema.Columns.Add(colvarShipRegion);
				
				TableSchema.TableColumn colvarShipPostalCode = new TableSchema.TableColumn(schema);
				colvarShipPostalCode.ColumnName = "ShipPostalCode";
				colvarShipPostalCode.DataType = DbType.String;
				colvarShipPostalCode.MaxLength = 10;
				colvarShipPostalCode.AutoIncrement = false;
				colvarShipPostalCode.IsNullable = true;
				colvarShipPostalCode.IsPrimaryKey = false;
				colvarShipPostalCode.IsForeignKey = false;
				colvarShipPostalCode.IsReadOnly = false;
				colvarShipPostalCode.DefaultSetting = @"";
				colvarShipPostalCode.ForeignKeyTableName = "";
				schema.Columns.Add(colvarShipPostalCode);
				
				TableSchema.TableColumn colvarShipCountry = new TableSchema.TableColumn(schema);
				colvarShipCountry.ColumnName = "ShipCountry";
				colvarShipCountry.DataType = DbType.String;
				colvarShipCountry.MaxLength = 15;
				colvarShipCountry.AutoIncrement = false;
				colvarShipCountry.IsNullable = true;
				colvarShipCountry.IsPrimaryKey = false;
				colvarShipCountry.IsForeignKey = false;
				colvarShipCountry.IsReadOnly = false;
				colvarShipCountry.DefaultSetting = @"";
				colvarShipCountry.ForeignKeyTableName = "";
				schema.Columns.Add(colvarShipCountry);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("Orders",schema);
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
		  
		[XmlAttribute("CustomerID")]
		[Bindable(true)]
		public string CustomerID 
		{
			get { return GetColumnValue<string>(Columns.CustomerID); }
			set { SetColumnValue(Columns.CustomerID, value); }
		}
		  
		[XmlAttribute("EmployeeID")]
		[Bindable(true)]
		public int? EmployeeID 
		{
			get { return GetColumnValue<int?>(Columns.EmployeeID); }
			set { SetColumnValue(Columns.EmployeeID, value); }
		}
		  
		[XmlAttribute("OrderDate")]
		[Bindable(true)]
		public DateTime? OrderDate 
		{
			get { return GetColumnValue<DateTime?>(Columns.OrderDate); }
			set { SetColumnValue(Columns.OrderDate, value); }
		}
		  
		[XmlAttribute("RequiredDate")]
		[Bindable(true)]
		public DateTime? RequiredDate 
		{
			get { return GetColumnValue<DateTime?>(Columns.RequiredDate); }
			set { SetColumnValue(Columns.RequiredDate, value); }
		}
		  
		[XmlAttribute("ShippedDate")]
		[Bindable(true)]
		public DateTime? ShippedDate 
		{
			get { return GetColumnValue<DateTime?>(Columns.ShippedDate); }
			set { SetColumnValue(Columns.ShippedDate, value); }
		}
		  
		[XmlAttribute("ShipVia")]
		[Bindable(true)]
		public int? ShipVia 
		{
			get { return GetColumnValue<int?>(Columns.ShipVia); }
			set { SetColumnValue(Columns.ShipVia, value); }
		}
		  
		[XmlAttribute("Freight")]
		[Bindable(true)]
		public decimal? Freight 
		{
			get { return GetColumnValue<decimal?>(Columns.Freight); }
			set { SetColumnValue(Columns.Freight, value); }
		}
		  
		[XmlAttribute("ShipName")]
		[Bindable(true)]
		public string ShipName 
		{
			get { return GetColumnValue<string>(Columns.ShipName); }
			set { SetColumnValue(Columns.ShipName, value); }
		}
		  
		[XmlAttribute("ShipAddress")]
		[Bindable(true)]
		public string ShipAddress 
		{
			get { return GetColumnValue<string>(Columns.ShipAddress); }
			set { SetColumnValue(Columns.ShipAddress, value); }
		}
		  
		[XmlAttribute("ShipCity")]
		[Bindable(true)]
		public string ShipCity 
		{
			get { return GetColumnValue<string>(Columns.ShipCity); }
			set { SetColumnValue(Columns.ShipCity, value); }
		}
		  
		[XmlAttribute("ShipRegion")]
		[Bindable(true)]
		public string ShipRegion 
		{
			get { return GetColumnValue<string>(Columns.ShipRegion); }
			set { SetColumnValue(Columns.ShipRegion, value); }
		}
		  
		[XmlAttribute("ShipPostalCode")]
		[Bindable(true)]
		public string ShipPostalCode 
		{
			get { return GetColumnValue<string>(Columns.ShipPostalCode); }
			set { SetColumnValue(Columns.ShipPostalCode, value); }
		}
		  
		[XmlAttribute("ShipCountry")]
		[Bindable(true)]
		public string ShipCountry 
		{
			get { return GetColumnValue<string>(Columns.ShipCountry); }
			set { SetColumnValue(Columns.ShipCountry, value); }
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
			return new Northwind.Order_DetailCollection().Where(Order_Detail.Columns.OrderID, OrderID).Load();
		}
		#endregion
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a Customer ActiveRecord object related to this Order
		/// 
		/// </summary>
		public Northwind.Customer Customer
		{
			get { return Northwind.Customer.FetchByID(this.CustomerID); }
			set { SetColumnValue("CustomerID", value.CustomerID); }
		}
		
		
		/// <summary>
		/// Returns a Employee ActiveRecord object related to this Order
		/// 
		/// </summary>
		public Northwind.Employee Employee
		{
			get { return Northwind.Employee.FetchByID(this.EmployeeID); }
			set { SetColumnValue("EmployeeID", value.EmployeeID); }
		}
		
		
		/// <summary>
		/// Returns a Shipper ActiveRecord object related to this Order
		/// 
		/// </summary>
		public Northwind.Shipper Shipper
		{
			get { return Northwind.Shipper.FetchByID(this.ShipVia); }
			set { SetColumnValue("ShipVia", value.ShipperID); }
		}
		
		
		#endregion
		
		
		
		#region Many To Many Helpers
		
		 
		public Northwind.ProductCollection GetProductCollection() { return Order.GetProductCollection(this.OrderID); }
		public static Northwind.ProductCollection GetProductCollection(int varOrderID)
		{
		    SubSonic.QueryCommand cmd = new SubSonic.QueryCommand("SELECT * FROM [dbo].[Products] INNER JOIN [Order Details] ON [Products].[ProductID] = [Order Details].[ProductID] WHERE [Order Details].[OrderID] = @OrderID", Order.Schema.Provider.Name);
			cmd.AddParameter("@OrderID", varOrderID, DbType.Int32);
			IDataReader rdr = SubSonic.DataService.GetReader(cmd);
			ProductCollection coll = new ProductCollection();
			coll.LoadAndCloseReader(rdr);
			return coll;
		}
		
		public static void SaveProductMap(int varOrderID, ProductCollection items)
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [Order Details] WHERE [Order Details].[OrderID] = @OrderID", Order.Schema.Provider.Name);
			cmdDel.AddParameter("@OrderID", varOrderID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (Product item in items)
			{
				Order_Detail varOrder_Detail = new Order_Detail();
				varOrder_Detail.SetColumnValue("OrderID", varOrderID);
				varOrder_Detail.SetColumnValue("ProductID", item.GetPrimaryKeyValue());
				varOrder_Detail.Save();
			}
		}
		public static void SaveProductMap(int varOrderID, System.Web.UI.WebControls.ListItemCollection itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [Order Details] WHERE [Order Details].[OrderID] = @OrderID", Order.Schema.Provider.Name);
			cmdDel.AddParameter("@OrderID", varOrderID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (System.Web.UI.WebControls.ListItem l in itemList) 
			{
				if (l.Selected) 
				{
					Order_Detail varOrder_Detail = new Order_Detail();
					varOrder_Detail.SetColumnValue("OrderID", varOrderID);
					varOrder_Detail.SetColumnValue("ProductID", l.Value);
					varOrder_Detail.Save();
				}
			}
		}
		public static void SaveProductMap(int varOrderID , int[] itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [Order Details] WHERE [Order Details].[OrderID] = @OrderID", Order.Schema.Provider.Name);
			cmdDel.AddParameter("@OrderID", varOrderID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (int item in itemList) 
			{
				Order_Detail varOrder_Detail = new Order_Detail();
				varOrder_Detail.SetColumnValue("OrderID", varOrderID);
				varOrder_Detail.SetColumnValue("ProductID", item);
				varOrder_Detail.Save();
			}
		}
		
		public static void DeleteProductMap(int varOrderID) 
		{
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [Order Details] WHERE [Order Details].[OrderID] = @OrderID", Order.Schema.Provider.Name);
			cmdDel.AddParameter("@OrderID", varOrderID, DbType.Int32);
			DataService.ExecuteQuery(cmdDel);
		}
		
		#endregion
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varCustomerID,int? varEmployeeID,DateTime? varOrderDate,DateTime? varRequiredDate,DateTime? varShippedDate,int? varShipVia,decimal? varFreight,string varShipName,string varShipAddress,string varShipCity,string varShipRegion,string varShipPostalCode,string varShipCountry)
		{
			Order item = new Order();
			
			item.CustomerID = varCustomerID;
			
			item.EmployeeID = varEmployeeID;
			
			item.OrderDate = varOrderDate;
			
			item.RequiredDate = varRequiredDate;
			
			item.ShippedDate = varShippedDate;
			
			item.ShipVia = varShipVia;
			
			item.Freight = varFreight;
			
			item.ShipName = varShipName;
			
			item.ShipAddress = varShipAddress;
			
			item.ShipCity = varShipCity;
			
			item.ShipRegion = varShipRegion;
			
			item.ShipPostalCode = varShipPostalCode;
			
			item.ShipCountry = varShipCountry;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varOrderID,string varCustomerID,int? varEmployeeID,DateTime? varOrderDate,DateTime? varRequiredDate,DateTime? varShippedDate,int? varShipVia,decimal? varFreight,string varShipName,string varShipAddress,string varShipCity,string varShipRegion,string varShipPostalCode,string varShipCountry)
		{
			Order item = new Order();
			
				item.OrderID = varOrderID;
			
				item.CustomerID = varCustomerID;
			
				item.EmployeeID = varEmployeeID;
			
				item.OrderDate = varOrderDate;
			
				item.RequiredDate = varRequiredDate;
			
				item.ShippedDate = varShippedDate;
			
				item.ShipVia = varShipVia;
			
				item.Freight = varFreight;
			
				item.ShipName = varShipName;
			
				item.ShipAddress = varShipAddress;
			
				item.ShipCity = varShipCity;
			
				item.ShipRegion = varShipRegion;
			
				item.ShipPostalCode = varShipPostalCode;
			
				item.ShipCountry = varShipCountry;
			
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
        
        
        
        public static TableSchema.TableColumn CustomerIDColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn EmployeeIDColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn OrderDateColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn RequiredDateColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn ShippedDateColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn ShipViaColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn FreightColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn ShipNameColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn ShipAddressColumn
        {
            get { return Schema.Columns[9]; }
        }
        
        
        
        public static TableSchema.TableColumn ShipCityColumn
        {
            get { return Schema.Columns[10]; }
        }
        
        
        
        public static TableSchema.TableColumn ShipRegionColumn
        {
            get { return Schema.Columns[11]; }
        }
        
        
        
        public static TableSchema.TableColumn ShipPostalCodeColumn
        {
            get { return Schema.Columns[12]; }
        }
        
        
        
        public static TableSchema.TableColumn ShipCountryColumn
        {
            get { return Schema.Columns[13]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string OrderID = @"OrderID";
			 public static string CustomerID = @"CustomerID";
			 public static string EmployeeID = @"EmployeeID";
			 public static string OrderDate = @"OrderDate";
			 public static string RequiredDate = @"RequiredDate";
			 public static string ShippedDate = @"ShippedDate";
			 public static string ShipVia = @"ShipVia";
			 public static string Freight = @"Freight";
			 public static string ShipName = @"ShipName";
			 public static string ShipAddress = @"ShipAddress";
			 public static string ShipCity = @"ShipCity";
			 public static string ShipRegion = @"ShipRegion";
			 public static string ShipPostalCode = @"ShipPostalCode";
			 public static string ShipCountry = @"ShipCountry";
						
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
