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
	/// Strongly-typed collection for the Supplier class.
	/// </summary>
    [Serializable]
	public partial class SupplierCollection : ActiveList<Supplier, SupplierCollection>
	{	   
		public SupplierCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>SupplierCollection</returns>
		public SupplierCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Supplier o = this[i];
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
	/// This is an ActiveRecord class which wraps the Suppliers table.
	/// </summary>
	[Serializable]
	public partial class Supplier : ActiveRecord<Supplier>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Supplier()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Supplier(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Supplier(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Supplier(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Suppliers", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarSupplierID = new TableSchema.TableColumn(schema);
				colvarSupplierID.ColumnName = "SupplierID";
				colvarSupplierID.DataType = DbType.Int32;
				colvarSupplierID.MaxLength = 0;
				colvarSupplierID.AutoIncrement = true;
				colvarSupplierID.IsNullable = false;
				colvarSupplierID.IsPrimaryKey = true;
				colvarSupplierID.IsForeignKey = false;
				colvarSupplierID.IsReadOnly = false;
				colvarSupplierID.DefaultSetting = @"";
				colvarSupplierID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarSupplierID);
				
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
				
				TableSchema.TableColumn colvarHomePage = new TableSchema.TableColumn(schema);
				colvarHomePage.ColumnName = "HomePage";
				colvarHomePage.DataType = DbType.String;
				colvarHomePage.MaxLength = 1073741823;
				colvarHomePage.AutoIncrement = false;
				colvarHomePage.IsNullable = true;
				colvarHomePage.IsPrimaryKey = false;
				colvarHomePage.IsForeignKey = false;
				colvarHomePage.IsReadOnly = false;
				colvarHomePage.DefaultSetting = @"";
				colvarHomePage.ForeignKeyTableName = "";
				schema.Columns.Add(colvarHomePage);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("Suppliers",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("SupplierID")]
		[Bindable(true)]
		public int SupplierID 
		{
			get { return GetColumnValue<int>(Columns.SupplierID); }
			set { SetColumnValue(Columns.SupplierID, value); }
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
		  
		[XmlAttribute("HomePage")]
		[Bindable(true)]
		public string HomePage 
		{
			get { return GetColumnValue<string>(Columns.HomePage); }
			set { SetColumnValue(Columns.HomePage, value); }
		}
		
		#endregion
		
		
		#region PrimaryKey Methods		
		
        protected override void SetPrimaryKey(object oValue)
        {
            base.SetPrimaryKey(oValue);
            
            SetPKValues();
        }
        
		
		public Northwind.ProductCollection Products()
		{
			return new Northwind.ProductCollection().Where(Product.Columns.SupplierID, SupplierID).Load();
		}
		#endregion
		
			
		
		//no foreign key tables defined (0)
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varCompanyName,string varContactName,string varContactTitle,string varAddress,string varCity,string varRegion,string varPostalCode,string varCountry,string varPhone,string varFax,string varHomePage)
		{
			Supplier item = new Supplier();
			
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
			
			item.HomePage = varHomePage;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varSupplierID,string varCompanyName,string varContactName,string varContactTitle,string varAddress,string varCity,string varRegion,string varPostalCode,string varCountry,string varPhone,string varFax,string varHomePage)
		{
			Supplier item = new Supplier();
			
				item.SupplierID = varSupplierID;
			
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
			
				item.HomePage = varHomePage;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn SupplierIDColumn
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
        
        
        
        public static TableSchema.TableColumn HomePageColumn
        {
            get { return Schema.Columns[11]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string SupplierID = @"SupplierID";
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
			 public static string HomePage = @"HomePage";
						
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
