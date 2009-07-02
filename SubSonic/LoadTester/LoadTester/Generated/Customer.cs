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
	/// Strongly-typed collection for the Customer class.
	/// </summary>
    [Serializable]
	public partial class CustomerCollection : ActiveList<Customer, CustomerCollection>
	{	   
		public CustomerCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>CustomerCollection</returns>
		public CustomerCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Customer o = this[i];
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
	/// This is an ActiveRecord class which wraps the Customers table.
	/// </summary>
	[Serializable]
	public partial class Customer : ActiveRecord<Customer>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Customer()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Customer(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Customer(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Customer(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Customers", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
				colvarCustomerID.ColumnName = "CustomerID";
				colvarCustomerID.DataType = DbType.String;
				colvarCustomerID.MaxLength = 5;
				colvarCustomerID.AutoIncrement = false;
				colvarCustomerID.IsNullable = false;
				colvarCustomerID.IsPrimaryKey = true;
				colvarCustomerID.IsForeignKey = false;
				colvarCustomerID.IsReadOnly = false;
				colvarCustomerID.DefaultSetting = @"";
				colvarCustomerID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCustomerID);
				
				TableSchema.TableColumn colvarCompanyName = new TableSchema.TableColumn(schema);
				colvarCompanyName.ColumnName = "CompanyName";
				colvarCompanyName.DataType = DbType.String;
				colvarCompanyName.MaxLength = 40;
				colvarCompanyName.AutoIncrement = false;
				colvarCompanyName.IsNullable = false;
				colvarCompanyName.IsPrimaryKey = false;
				colvarCompanyName.IsForeignKey = false;
				colvarCompanyName.IsReadOnly = false;
				colvarCompanyName.DefaultSetting = @"";
				colvarCompanyName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCompanyName);
				
				TableSchema.TableColumn colvarContactName = new TableSchema.TableColumn(schema);
				colvarContactName.ColumnName = "ContactName";
				colvarContactName.DataType = DbType.String;
				colvarContactName.MaxLength = 30;
				colvarContactName.AutoIncrement = false;
				colvarContactName.IsNullable = true;
				colvarContactName.IsPrimaryKey = false;
				colvarContactName.IsForeignKey = false;
				colvarContactName.IsReadOnly = false;
				colvarContactName.DefaultSetting = @"";
				colvarContactName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarContactName);
				
				TableSchema.TableColumn colvarContactTitle = new TableSchema.TableColumn(schema);
				colvarContactTitle.ColumnName = "ContactTitle";
				colvarContactTitle.DataType = DbType.String;
				colvarContactTitle.MaxLength = 30;
				colvarContactTitle.AutoIncrement = false;
				colvarContactTitle.IsNullable = true;
				colvarContactTitle.IsPrimaryKey = false;
				colvarContactTitle.IsForeignKey = false;
				colvarContactTitle.IsReadOnly = false;
				colvarContactTitle.DefaultSetting = @"";
				colvarContactTitle.ForeignKeyTableName = "";
				schema.Columns.Add(colvarContactTitle);
				
				TableSchema.TableColumn colvarAddress = new TableSchema.TableColumn(schema);
				colvarAddress.ColumnName = "Address";
				colvarAddress.DataType = DbType.String;
				colvarAddress.MaxLength = 60;
				colvarAddress.AutoIncrement = false;
				colvarAddress.IsNullable = true;
				colvarAddress.IsPrimaryKey = false;
				colvarAddress.IsForeignKey = false;
				colvarAddress.IsReadOnly = false;
				colvarAddress.DefaultSetting = @"";
				colvarAddress.ForeignKeyTableName = "";
				schema.Columns.Add(colvarAddress);
				
				TableSchema.TableColumn colvarCity = new TableSchema.TableColumn(schema);
				colvarCity.ColumnName = "City";
				colvarCity.DataType = DbType.String;
				colvarCity.MaxLength = 15;
				colvarCity.AutoIncrement = false;
				colvarCity.IsNullable = true;
				colvarCity.IsPrimaryKey = false;
				colvarCity.IsForeignKey = false;
				colvarCity.IsReadOnly = false;
				colvarCity.DefaultSetting = @"";
				colvarCity.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCity);
				
				TableSchema.TableColumn colvarRegion = new TableSchema.TableColumn(schema);
				colvarRegion.ColumnName = "Region";
				colvarRegion.DataType = DbType.String;
				colvarRegion.MaxLength = 15;
				colvarRegion.AutoIncrement = false;
				colvarRegion.IsNullable = true;
				colvarRegion.IsPrimaryKey = false;
				colvarRegion.IsForeignKey = false;
				colvarRegion.IsReadOnly = false;
				colvarRegion.DefaultSetting = @"";
				colvarRegion.ForeignKeyTableName = "";
				schema.Columns.Add(colvarRegion);
				
				TableSchema.TableColumn colvarPostalCode = new TableSchema.TableColumn(schema);
				colvarPostalCode.ColumnName = "PostalCode";
				colvarPostalCode.DataType = DbType.String;
				colvarPostalCode.MaxLength = 10;
				colvarPostalCode.AutoIncrement = false;
				colvarPostalCode.IsNullable = true;
				colvarPostalCode.IsPrimaryKey = false;
				colvarPostalCode.IsForeignKey = false;
				colvarPostalCode.IsReadOnly = false;
				colvarPostalCode.DefaultSetting = @"";
				colvarPostalCode.ForeignKeyTableName = "";
				schema.Columns.Add(colvarPostalCode);
				
				TableSchema.TableColumn colvarCountry = new TableSchema.TableColumn(schema);
				colvarCountry.ColumnName = "Country";
				colvarCountry.DataType = DbType.String;
				colvarCountry.MaxLength = 15;
				colvarCountry.AutoIncrement = false;
				colvarCountry.IsNullable = true;
				colvarCountry.IsPrimaryKey = false;
				colvarCountry.IsForeignKey = false;
				colvarCountry.IsReadOnly = false;
				colvarCountry.DefaultSetting = @"";
				colvarCountry.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCountry);
				
				TableSchema.TableColumn colvarPhone = new TableSchema.TableColumn(schema);
				colvarPhone.ColumnName = "Phone";
				colvarPhone.DataType = DbType.String;
				colvarPhone.MaxLength = 24;
				colvarPhone.AutoIncrement = false;
				colvarPhone.IsNullable = true;
				colvarPhone.IsPrimaryKey = false;
				colvarPhone.IsForeignKey = false;
				colvarPhone.IsReadOnly = false;
				colvarPhone.DefaultSetting = @"";
				colvarPhone.ForeignKeyTableName = "";
				schema.Columns.Add(colvarPhone);
				
				TableSchema.TableColumn colvarFax = new TableSchema.TableColumn(schema);
				colvarFax.ColumnName = "Fax";
				colvarFax.DataType = DbType.String;
				colvarFax.MaxLength = 24;
				colvarFax.AutoIncrement = false;
				colvarFax.IsNullable = true;
				colvarFax.IsPrimaryKey = false;
				colvarFax.IsForeignKey = false;
				colvarFax.IsReadOnly = false;
				colvarFax.DefaultSetting = @"";
				colvarFax.ForeignKeyTableName = "";
				schema.Columns.Add(colvarFax);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("Customers",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("CustomerID")]
		[Bindable(true)]
		public string CustomerID 
		{
			get { return GetColumnValue<string>(Columns.CustomerID); }
			set { SetColumnValue(Columns.CustomerID, value); }
		}
		  
		[XmlAttribute("CompanyName")]
		[Bindable(true)]
		public string CompanyName 
		{
			get { return GetColumnValue<string>(Columns.CompanyName); }
			set { SetColumnValue(Columns.CompanyName, value); }
		}
		  
		[XmlAttribute("ContactName")]
		[Bindable(true)]
		public string ContactName 
		{
			get { return GetColumnValue<string>(Columns.ContactName); }
			set { SetColumnValue(Columns.ContactName, value); }
		}
		  
		[XmlAttribute("ContactTitle")]
		[Bindable(true)]
		public string ContactTitle 
		{
			get { return GetColumnValue<string>(Columns.ContactTitle); }
			set { SetColumnValue(Columns.ContactTitle, value); }
		}
		  
		[XmlAttribute("Address")]
		[Bindable(true)]
		public string Address 
		{
			get { return GetColumnValue<string>(Columns.Address); }
			set { SetColumnValue(Columns.Address, value); }
		}
		  
		[XmlAttribute("City")]
		[Bindable(true)]
		public string City 
		{
			get { return GetColumnValue<string>(Columns.City); }
			set { SetColumnValue(Columns.City, value); }
		}
		  
		[XmlAttribute("Region")]
		[Bindable(true)]
		public string Region 
		{
			get { return GetColumnValue<string>(Columns.Region); }
			set { SetColumnValue(Columns.Region, value); }
		}
		  
		[XmlAttribute("PostalCode")]
		[Bindable(true)]
		public string PostalCode 
		{
			get { return GetColumnValue<string>(Columns.PostalCode); }
			set { SetColumnValue(Columns.PostalCode, value); }
		}
		  
		[XmlAttribute("Country")]
		[Bindable(true)]
		public string Country 
		{
			get { return GetColumnValue<string>(Columns.Country); }
			set { SetColumnValue(Columns.Country, value); }
		}
		  
		[XmlAttribute("Phone")]
		[Bindable(true)]
		public string Phone 
		{
			get { return GetColumnValue<string>(Columns.Phone); }
			set { SetColumnValue(Columns.Phone, value); }
		}
		  
		[XmlAttribute("Fax")]
		[Bindable(true)]
		public string Fax 
		{
			get { return GetColumnValue<string>(Columns.Fax); }
			set { SetColumnValue(Columns.Fax, value); }
		}
		
		#endregion
		
		
		#region PrimaryKey Methods		
		
        protected override void SetPrimaryKey(object oValue)
        {
            base.SetPrimaryKey(oValue);
            
            SetPKValues();
        }
        
		
		public Northwind.CustomerCustomerDemoCollection CustomerCustomerDemoRecords()
		{
			return new Northwind.CustomerCustomerDemoCollection().Where(CustomerCustomerDemo.Columns.CustomerID, CustomerID).Load();
		}
		public Northwind.OrderCollection Orders()
		{
			return new Northwind.OrderCollection().Where(Order.Columns.CustomerID, CustomerID).Load();
		}
		#endregion
		
			
		
		//no foreign key tables defined (0)
		
		
		
		#region Many To Many Helpers
		
		 
		public Northwind.CustomerDemographicCollection GetCustomerDemographicCollection() { return Customer.GetCustomerDemographicCollection(this.CustomerID); }
		public static Northwind.CustomerDemographicCollection GetCustomerDemographicCollection(string varCustomerID)
		{
		    SubSonic.QueryCommand cmd = new SubSonic.QueryCommand("SELECT * FROM [dbo].[CustomerDemographics] INNER JOIN [CustomerCustomerDemo] ON [CustomerDemographics].[CustomerTypeID] = [CustomerCustomerDemo].[CustomerTypeID] WHERE [CustomerCustomerDemo].[CustomerID] = @CustomerID", Customer.Schema.Provider.Name);
			cmd.AddParameter("@CustomerID", varCustomerID, DbType.String);
			IDataReader rdr = SubSonic.DataService.GetReader(cmd);
			CustomerDemographicCollection coll = new CustomerDemographicCollection();
			coll.LoadAndCloseReader(rdr);
			return coll;
		}
		
		public static void SaveCustomerDemographicMap(string varCustomerID, CustomerDemographicCollection items)
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [CustomerCustomerDemo] WHERE [CustomerCustomerDemo].[CustomerID] = @CustomerID", Customer.Schema.Provider.Name);
			cmdDel.AddParameter("@CustomerID", varCustomerID, DbType.String);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (CustomerDemographic item in items)
			{
				CustomerCustomerDemo varCustomerCustomerDemo = new CustomerCustomerDemo();
				varCustomerCustomerDemo.SetColumnValue("CustomerID", varCustomerID);
				varCustomerCustomerDemo.SetColumnValue("CustomerTypeID", item.GetPrimaryKeyValue());
				varCustomerCustomerDemo.Save();
			}
		}
		public static void SaveCustomerDemographicMap(string varCustomerID, System.Web.UI.WebControls.ListItemCollection itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [CustomerCustomerDemo] WHERE [CustomerCustomerDemo].[CustomerID] = @CustomerID", Customer.Schema.Provider.Name);
			cmdDel.AddParameter("@CustomerID", varCustomerID, DbType.String);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (System.Web.UI.WebControls.ListItem l in itemList) 
			{
				if (l.Selected) 
				{
					CustomerCustomerDemo varCustomerCustomerDemo = new CustomerCustomerDemo();
					varCustomerCustomerDemo.SetColumnValue("CustomerID", varCustomerID);
					varCustomerCustomerDemo.SetColumnValue("CustomerTypeID", l.Value);
					varCustomerCustomerDemo.Save();
				}
			}
		}
		public static void SaveCustomerDemographicMap(string varCustomerID , string[] itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [CustomerCustomerDemo] WHERE [CustomerCustomerDemo].[CustomerID] = @CustomerID", Customer.Schema.Provider.Name);
			cmdDel.AddParameter("@CustomerID", varCustomerID, DbType.String);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (string item in itemList) 
			{
				CustomerCustomerDemo varCustomerCustomerDemo = new CustomerCustomerDemo();
				varCustomerCustomerDemo.SetColumnValue("CustomerID", varCustomerID);
				varCustomerCustomerDemo.SetColumnValue("CustomerTypeID", item);
				varCustomerCustomerDemo.Save();
			}
		}
		
		public static void DeleteCustomerDemographicMap(string varCustomerID) 
		{
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [CustomerCustomerDemo] WHERE [CustomerCustomerDemo].[CustomerID] = @CustomerID", Customer.Schema.Provider.Name);
			cmdDel.AddParameter("@CustomerID", varCustomerID, DbType.String);
			DataService.ExecuteQuery(cmdDel);
		}
		
		#endregion
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varCustomerID,string varCompanyName,string varContactName,string varContactTitle,string varAddress,string varCity,string varRegion,string varPostalCode,string varCountry,string varPhone,string varFax)
		{
			Customer item = new Customer();
			
			item.CustomerID = varCustomerID;
			
			item.CompanyName = varCompanyName;
			
			item.ContactName = varContactName;
			
			item.ContactTitle = varContactTitle;
			
			item.Address = varAddress;
			
			item.City = varCity;
			
			item.Region = varRegion;
			
			item.PostalCode = varPostalCode;
			
			item.Country = varCountry;
			
			item.Phone = varPhone;
			
			item.Fax = varFax;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(string varCustomerID,string varCompanyName,string varContactName,string varContactTitle,string varAddress,string varCity,string varRegion,string varPostalCode,string varCountry,string varPhone,string varFax)
		{
			Customer item = new Customer();
			
				item.CustomerID = varCustomerID;
			
				item.CompanyName = varCompanyName;
			
				item.ContactName = varContactName;
			
				item.ContactTitle = varContactTitle;
			
				item.Address = varAddress;
			
				item.City = varCity;
			
				item.Region = varRegion;
			
				item.PostalCode = varPostalCode;
			
				item.Country = varCountry;
			
				item.Phone = varPhone;
			
				item.Fax = varFax;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn CustomerIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn CompanyNameColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn ContactNameColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn ContactTitleColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn AddressColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn CityColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn RegionColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn PostalCodeColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn CountryColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn PhoneColumn
        {
            get { return Schema.Columns[9]; }
        }
        
        
        
        public static TableSchema.TableColumn FaxColumn
        {
            get { return Schema.Columns[10]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string CustomerID = @"CustomerID";
			 public static string CompanyName = @"CompanyName";
			 public static string ContactName = @"ContactName";
			 public static string ContactTitle = @"ContactTitle";
			 public static string Address = @"Address";
			 public static string City = @"City";
			 public static string Region = @"Region";
			 public static string PostalCode = @"PostalCode";
			 public static string Country = @"Country";
			 public static string Phone = @"Phone";
			 public static string Fax = @"Fax";
						
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
