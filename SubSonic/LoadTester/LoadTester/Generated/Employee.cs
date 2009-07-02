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
	/// Strongly-typed collection for the Employee class.
	/// </summary>
    [Serializable]
	public partial class EmployeeCollection : ActiveList<Employee, EmployeeCollection>
	{	   
		public EmployeeCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>EmployeeCollection</returns>
		public EmployeeCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Employee o = this[i];
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
	/// This is an ActiveRecord class which wraps the Employees table.
	/// </summary>
	[Serializable]
	public partial class Employee : ActiveRecord<Employee>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Employee()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Employee(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Employee(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Employee(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Employees", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarEmployeeID = new TableSchema.TableColumn(schema);
				colvarEmployeeID.ColumnName = "EmployeeID";
				colvarEmployeeID.DataType = DbType.Int32;
				colvarEmployeeID.MaxLength = 0;
				colvarEmployeeID.AutoIncrement = true;
				colvarEmployeeID.IsNullable = false;
				colvarEmployeeID.IsPrimaryKey = true;
				colvarEmployeeID.IsForeignKey = false;
				colvarEmployeeID.IsReadOnly = false;
				colvarEmployeeID.DefaultSetting = @"";
				colvarEmployeeID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarEmployeeID);
				
				TableSchema.TableColumn colvarLastName = new TableSchema.TableColumn(schema);
				colvarLastName.ColumnName = "LastName";
				colvarLastName.DataType = DbType.String;
				colvarLastName.MaxLength = 20;
				colvarLastName.AutoIncrement = false;
				colvarLastName.IsNullable = false;
				colvarLastName.IsPrimaryKey = false;
				colvarLastName.IsForeignKey = false;
				colvarLastName.IsReadOnly = false;
				colvarLastName.DefaultSetting = @"";
				colvarLastName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarLastName);
				
				TableSchema.TableColumn colvarFirstName = new TableSchema.TableColumn(schema);
				colvarFirstName.ColumnName = "FirstName";
				colvarFirstName.DataType = DbType.String;
				colvarFirstName.MaxLength = 10;
				colvarFirstName.AutoIncrement = false;
				colvarFirstName.IsNullable = false;
				colvarFirstName.IsPrimaryKey = false;
				colvarFirstName.IsForeignKey = false;
				colvarFirstName.IsReadOnly = false;
				colvarFirstName.DefaultSetting = @"";
				colvarFirstName.ForeignKeyTableName = "";
				schema.Columns.Add(colvarFirstName);
				
				TableSchema.TableColumn colvarTitle = new TableSchema.TableColumn(schema);
				colvarTitle.ColumnName = "Title";
				colvarTitle.DataType = DbType.String;
				colvarTitle.MaxLength = 30;
				colvarTitle.AutoIncrement = false;
				colvarTitle.IsNullable = true;
				colvarTitle.IsPrimaryKey = false;
				colvarTitle.IsForeignKey = false;
				colvarTitle.IsReadOnly = false;
				colvarTitle.DefaultSetting = @"";
				colvarTitle.ForeignKeyTableName = "";
				schema.Columns.Add(colvarTitle);
				
				TableSchema.TableColumn colvarTitleOfCourtesy = new TableSchema.TableColumn(schema);
				colvarTitleOfCourtesy.ColumnName = "TitleOfCourtesy";
				colvarTitleOfCourtesy.DataType = DbType.String;
				colvarTitleOfCourtesy.MaxLength = 25;
				colvarTitleOfCourtesy.AutoIncrement = false;
				colvarTitleOfCourtesy.IsNullable = true;
				colvarTitleOfCourtesy.IsPrimaryKey = false;
				colvarTitleOfCourtesy.IsForeignKey = false;
				colvarTitleOfCourtesy.IsReadOnly = false;
				colvarTitleOfCourtesy.DefaultSetting = @"";
				colvarTitleOfCourtesy.ForeignKeyTableName = "";
				schema.Columns.Add(colvarTitleOfCourtesy);
				
				TableSchema.TableColumn colvarBirthDate = new TableSchema.TableColumn(schema);
				colvarBirthDate.ColumnName = "BirthDate";
				colvarBirthDate.DataType = DbType.DateTime;
				colvarBirthDate.MaxLength = 0;
				colvarBirthDate.AutoIncrement = false;
				colvarBirthDate.IsNullable = true;
				colvarBirthDate.IsPrimaryKey = false;
				colvarBirthDate.IsForeignKey = false;
				colvarBirthDate.IsReadOnly = false;
				colvarBirthDate.DefaultSetting = @"";
				colvarBirthDate.ForeignKeyTableName = "";
				schema.Columns.Add(colvarBirthDate);
				
				TableSchema.TableColumn colvarHireDate = new TableSchema.TableColumn(schema);
				colvarHireDate.ColumnName = "HireDate";
				colvarHireDate.DataType = DbType.DateTime;
				colvarHireDate.MaxLength = 0;
				colvarHireDate.AutoIncrement = false;
				colvarHireDate.IsNullable = true;
				colvarHireDate.IsPrimaryKey = false;
				colvarHireDate.IsForeignKey = false;
				colvarHireDate.IsReadOnly = false;
				colvarHireDate.DefaultSetting = @"";
				colvarHireDate.ForeignKeyTableName = "";
				schema.Columns.Add(colvarHireDate);
				
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
				
				TableSchema.TableColumn colvarHomePhone = new TableSchema.TableColumn(schema);
				colvarHomePhone.ColumnName = "HomePhone";
				colvarHomePhone.DataType = DbType.String;
				colvarHomePhone.MaxLength = 24;
				colvarHomePhone.AutoIncrement = false;
				colvarHomePhone.IsNullable = true;
				colvarHomePhone.IsPrimaryKey = false;
				colvarHomePhone.IsForeignKey = false;
				colvarHomePhone.IsReadOnly = false;
				colvarHomePhone.DefaultSetting = @"";
				colvarHomePhone.ForeignKeyTableName = "";
				schema.Columns.Add(colvarHomePhone);
				
				TableSchema.TableColumn colvarExtension = new TableSchema.TableColumn(schema);
				colvarExtension.ColumnName = "Extension";
				colvarExtension.DataType = DbType.String;
				colvarExtension.MaxLength = 4;
				colvarExtension.AutoIncrement = false;
				colvarExtension.IsNullable = true;
				colvarExtension.IsPrimaryKey = false;
				colvarExtension.IsForeignKey = false;
				colvarExtension.IsReadOnly = false;
				colvarExtension.DefaultSetting = @"";
				colvarExtension.ForeignKeyTableName = "";
				schema.Columns.Add(colvarExtension);
				
				TableSchema.TableColumn colvarPhoto = new TableSchema.TableColumn(schema);
				colvarPhoto.ColumnName = "Photo";
				colvarPhoto.DataType = DbType.Binary;
				colvarPhoto.MaxLength = 2147483647;
				colvarPhoto.AutoIncrement = false;
				colvarPhoto.IsNullable = true;
				colvarPhoto.IsPrimaryKey = false;
				colvarPhoto.IsForeignKey = false;
				colvarPhoto.IsReadOnly = false;
				colvarPhoto.DefaultSetting = @"";
				colvarPhoto.ForeignKeyTableName = "";
				schema.Columns.Add(colvarPhoto);
				
				TableSchema.TableColumn colvarNotes = new TableSchema.TableColumn(schema);
				colvarNotes.ColumnName = "Notes";
				colvarNotes.DataType = DbType.String;
				colvarNotes.MaxLength = 1073741823;
				colvarNotes.AutoIncrement = false;
				colvarNotes.IsNullable = true;
				colvarNotes.IsPrimaryKey = false;
				colvarNotes.IsForeignKey = false;
				colvarNotes.IsReadOnly = false;
				colvarNotes.DefaultSetting = @"";
				colvarNotes.ForeignKeyTableName = "";
				schema.Columns.Add(colvarNotes);
				
				TableSchema.TableColumn colvarReportsTo = new TableSchema.TableColumn(schema);
				colvarReportsTo.ColumnName = "ReportsTo";
				colvarReportsTo.DataType = DbType.Int32;
				colvarReportsTo.MaxLength = 0;
				colvarReportsTo.AutoIncrement = false;
				colvarReportsTo.IsNullable = true;
				colvarReportsTo.IsPrimaryKey = false;
				colvarReportsTo.IsForeignKey = true;
				colvarReportsTo.IsReadOnly = false;
				colvarReportsTo.DefaultSetting = @"";
				
					colvarReportsTo.ForeignKeyTableName = "Employees";
				schema.Columns.Add(colvarReportsTo);
				
				TableSchema.TableColumn colvarPhotoPath = new TableSchema.TableColumn(schema);
				colvarPhotoPath.ColumnName = "PhotoPath";
				colvarPhotoPath.DataType = DbType.String;
				colvarPhotoPath.MaxLength = 255;
				colvarPhotoPath.AutoIncrement = false;
				colvarPhotoPath.IsNullable = true;
				colvarPhotoPath.IsPrimaryKey = false;
				colvarPhotoPath.IsForeignKey = false;
				colvarPhotoPath.IsReadOnly = false;
				colvarPhotoPath.DefaultSetting = @"";
				colvarPhotoPath.ForeignKeyTableName = "";
				schema.Columns.Add(colvarPhotoPath);
				
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
				DataService.Providers["Northwind"].AddSchema("Employees",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("EmployeeID")]
		[Bindable(true)]
		public int EmployeeID 
		{
			get { return GetColumnValue<int>(Columns.EmployeeID); }
			set { SetColumnValue(Columns.EmployeeID, value); }
		}
		  
		[XmlAttribute("LastName")]
		[Bindable(true)]
		public string LastName 
		{
			get { return GetColumnValue<string>(Columns.LastName); }
			set { SetColumnValue(Columns.LastName, value); }
		}
		  
		[XmlAttribute("FirstName")]
		[Bindable(true)]
		public string FirstName 
		{
			get { return GetColumnValue<string>(Columns.FirstName); }
			set { SetColumnValue(Columns.FirstName, value); }
		}
		  
		[XmlAttribute("Title")]
		[Bindable(true)]
		public string Title 
		{
			get { return GetColumnValue<string>(Columns.Title); }
			set { SetColumnValue(Columns.Title, value); }
		}
		  
		[XmlAttribute("TitleOfCourtesy")]
		[Bindable(true)]
		public string TitleOfCourtesy 
		{
			get { return GetColumnValue<string>(Columns.TitleOfCourtesy); }
			set { SetColumnValue(Columns.TitleOfCourtesy, value); }
		}
		  
		[XmlAttribute("BirthDate")]
		[Bindable(true)]
		public DateTime? BirthDate 
		{
			get { return GetColumnValue<DateTime?>(Columns.BirthDate); }
			set { SetColumnValue(Columns.BirthDate, value); }
		}
		  
		[XmlAttribute("HireDate")]
		[Bindable(true)]
		public DateTime? HireDate 
		{
			get { return GetColumnValue<DateTime?>(Columns.HireDate); }
			set { SetColumnValue(Columns.HireDate, value); }
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
		  
		[XmlAttribute("HomePhone")]
		[Bindable(true)]
		public string HomePhone 
		{
			get { return GetColumnValue<string>(Columns.HomePhone); }
			set { SetColumnValue(Columns.HomePhone, value); }
		}
		  
		[XmlAttribute("Extension")]
		[Bindable(true)]
		public string Extension 
		{
			get { return GetColumnValue<string>(Columns.Extension); }
			set { SetColumnValue(Columns.Extension, value); }
		}
		  
		[XmlAttribute("Photo")]
		[Bindable(true)]
		public byte[] Photo 
		{
			get { return GetColumnValue<byte[]>(Columns.Photo); }
			set { SetColumnValue(Columns.Photo, value); }
		}
		  
		[XmlAttribute("Notes")]
		[Bindable(true)]
		public string Notes 
		{
			get { return GetColumnValue<string>(Columns.Notes); }
			set { SetColumnValue(Columns.Notes, value); }
		}
		  
		[XmlAttribute("ReportsTo")]
		[Bindable(true)]
		public int? ReportsTo 
		{
			get { return GetColumnValue<int?>(Columns.ReportsTo); }
			set { SetColumnValue(Columns.ReportsTo, value); }
		}
		  
		[XmlAttribute("PhotoPath")]
		[Bindable(true)]
		public string PhotoPath 
		{
			get { return GetColumnValue<string>(Columns.PhotoPath); }
			set { SetColumnValue(Columns.PhotoPath, value); }
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
        
		
		public Northwind.EmployeeCollection ChildEmployees()
		{
			return new Northwind.EmployeeCollection().Where(Employee.Columns.ReportsTo, EmployeeID).Load();
		}
		public Northwind.EmployeeTerritoryCollection EmployeeTerritories()
		{
			return new Northwind.EmployeeTerritoryCollection().Where(EmployeeTerritory.Columns.EmployeeID, EmployeeID).Load();
		}
		public Northwind.OrderCollection Orders()
		{
			return new Northwind.OrderCollection().Where(Order.Columns.EmployeeID, EmployeeID).Load();
		}
		#endregion
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a Employee ActiveRecord object related to this Employee
		/// 
		/// </summary>
		public Northwind.Employee ParentEmployee
		{
			get { return Northwind.Employee.FetchByID(this.ReportsTo); }
			set { SetColumnValue("ReportsTo", value.EmployeeID); }
		}
		
		
		#endregion
		
		
		
		#region Many To Many Helpers
		
		 
		public Northwind.TerritoryCollection GetTerritoryCollection() { return Employee.GetTerritoryCollection(this.EmployeeID); }
		public static Northwind.TerritoryCollection GetTerritoryCollection(int varEmployeeID)
		{
		    SubSonic.QueryCommand cmd = new SubSonic.QueryCommand("SELECT * FROM [dbo].[Territories] INNER JOIN [EmployeeTerritories] ON [Territories].[TerritoryID] = [EmployeeTerritories].[TerritoryID] WHERE [EmployeeTerritories].[EmployeeID] = @EmployeeID", Employee.Schema.Provider.Name);
			cmd.AddParameter("@EmployeeID", varEmployeeID, DbType.Int32);
			IDataReader rdr = SubSonic.DataService.GetReader(cmd);
			TerritoryCollection coll = new TerritoryCollection();
			coll.LoadAndCloseReader(rdr);
			return coll;
		}
		
		public static void SaveTerritoryMap(int varEmployeeID, TerritoryCollection items)
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [EmployeeTerritories] WHERE [EmployeeTerritories].[EmployeeID] = @EmployeeID", Employee.Schema.Provider.Name);
			cmdDel.AddParameter("@EmployeeID", varEmployeeID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (Territory item in items)
			{
				EmployeeTerritory varEmployeeTerritory = new EmployeeTerritory();
				varEmployeeTerritory.SetColumnValue("EmployeeID", varEmployeeID);
				varEmployeeTerritory.SetColumnValue("TerritoryID", item.GetPrimaryKeyValue());
				varEmployeeTerritory.Save();
			}
		}
		public static void SaveTerritoryMap(int varEmployeeID, System.Web.UI.WebControls.ListItemCollection itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [EmployeeTerritories] WHERE [EmployeeTerritories].[EmployeeID] = @EmployeeID", Employee.Schema.Provider.Name);
			cmdDel.AddParameter("@EmployeeID", varEmployeeID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (System.Web.UI.WebControls.ListItem l in itemList) 
			{
				if (l.Selected) 
				{
					EmployeeTerritory varEmployeeTerritory = new EmployeeTerritory();
					varEmployeeTerritory.SetColumnValue("EmployeeID", varEmployeeID);
					varEmployeeTerritory.SetColumnValue("TerritoryID", l.Value);
					varEmployeeTerritory.Save();
				}
			}
		}
		public static void SaveTerritoryMap(int varEmployeeID , string[] itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [EmployeeTerritories] WHERE [EmployeeTerritories].[EmployeeID] = @EmployeeID", Employee.Schema.Provider.Name);
			cmdDel.AddParameter("@EmployeeID", varEmployeeID, DbType.Int32);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (string item in itemList) 
			{
				EmployeeTerritory varEmployeeTerritory = new EmployeeTerritory();
				varEmployeeTerritory.SetColumnValue("EmployeeID", varEmployeeID);
				varEmployeeTerritory.SetColumnValue("TerritoryID", item);
				varEmployeeTerritory.Save();
			}
		}
		
		public static void DeleteTerritoryMap(int varEmployeeID) 
		{
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [EmployeeTerritories] WHERE [EmployeeTerritories].[EmployeeID] = @EmployeeID", Employee.Schema.Provider.Name);
			cmdDel.AddParameter("@EmployeeID", varEmployeeID, DbType.Int32);
			DataService.ExecuteQuery(cmdDel);
		}
		
		#endregion
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varLastName,string varFirstName,string varTitle,string varTitleOfCourtesy,DateTime? varBirthDate,DateTime? varHireDate,string varAddress,string varCity,string varRegion,string varPostalCode,string varCountry,string varHomePhone,string varExtension,byte[] varPhoto,string varNotes,int? varReportsTo,string varPhotoPath,bool varDeleted)
		{
			Employee item = new Employee();
			
			item.LastName = varLastName;
			
			item.FirstName = varFirstName;
			
			item.Title = varTitle;
			
			item.TitleOfCourtesy = varTitleOfCourtesy;
			
			item.BirthDate = varBirthDate;
			
			item.HireDate = varHireDate;
			
			item.Address = varAddress;
			
			item.City = varCity;
			
			item.Region = varRegion;
			
			item.PostalCode = varPostalCode;
			
			item.Country = varCountry;
			
			item.HomePhone = varHomePhone;
			
			item.Extension = varExtension;
			
			item.Photo = varPhoto;
			
			item.Notes = varNotes;
			
			item.ReportsTo = varReportsTo;
			
			item.PhotoPath = varPhotoPath;
			
			item.Deleted = varDeleted;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varEmployeeID,string varLastName,string varFirstName,string varTitle,string varTitleOfCourtesy,DateTime? varBirthDate,DateTime? varHireDate,string varAddress,string varCity,string varRegion,string varPostalCode,string varCountry,string varHomePhone,string varExtension,byte[] varPhoto,string varNotes,int? varReportsTo,string varPhotoPath,bool varDeleted)
		{
			Employee item = new Employee();
			
				item.EmployeeID = varEmployeeID;
			
				item.LastName = varLastName;
			
				item.FirstName = varFirstName;
			
				item.Title = varTitle;
			
				item.TitleOfCourtesy = varTitleOfCourtesy;
			
				item.BirthDate = varBirthDate;
			
				item.HireDate = varHireDate;
			
				item.Address = varAddress;
			
				item.City = varCity;
			
				item.Region = varRegion;
			
				item.PostalCode = varPostalCode;
			
				item.Country = varCountry;
			
				item.HomePhone = varHomePhone;
			
				item.Extension = varExtension;
			
				item.Photo = varPhoto;
			
				item.Notes = varNotes;
			
				item.ReportsTo = varReportsTo;
			
				item.PhotoPath = varPhotoPath;
			
				item.Deleted = varDeleted;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn EmployeeIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn LastNameColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn FirstNameColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        public static TableSchema.TableColumn TitleColumn
        {
            get { return Schema.Columns[3]; }
        }
        
        
        
        public static TableSchema.TableColumn TitleOfCourtesyColumn
        {
            get { return Schema.Columns[4]; }
        }
        
        
        
        public static TableSchema.TableColumn BirthDateColumn
        {
            get { return Schema.Columns[5]; }
        }
        
        
        
        public static TableSchema.TableColumn HireDateColumn
        {
            get { return Schema.Columns[6]; }
        }
        
        
        
        public static TableSchema.TableColumn AddressColumn
        {
            get { return Schema.Columns[7]; }
        }
        
        
        
        public static TableSchema.TableColumn CityColumn
        {
            get { return Schema.Columns[8]; }
        }
        
        
        
        public static TableSchema.TableColumn RegionColumn
        {
            get { return Schema.Columns[9]; }
        }
        
        
        
        public static TableSchema.TableColumn PostalCodeColumn
        {
            get { return Schema.Columns[10]; }
        }
        
        
        
        public static TableSchema.TableColumn CountryColumn
        {
            get { return Schema.Columns[11]; }
        }
        
        
        
        public static TableSchema.TableColumn HomePhoneColumn
        {
            get { return Schema.Columns[12]; }
        }
        
        
        
        public static TableSchema.TableColumn ExtensionColumn
        {
            get { return Schema.Columns[13]; }
        }
        
        
        
        public static TableSchema.TableColumn PhotoColumn
        {
            get { return Schema.Columns[14]; }
        }
        
        
        
        public static TableSchema.TableColumn NotesColumn
        {
            get { return Schema.Columns[15]; }
        }
        
        
        
        public static TableSchema.TableColumn ReportsToColumn
        {
            get { return Schema.Columns[16]; }
        }
        
        
        
        public static TableSchema.TableColumn PhotoPathColumn
        {
            get { return Schema.Columns[17]; }
        }
        
        
        
        public static TableSchema.TableColumn DeletedColumn
        {
            get { return Schema.Columns[18]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string EmployeeID = @"EmployeeID";
			 public static string LastName = @"LastName";
			 public static string FirstName = @"FirstName";
			 public static string Title = @"Title";
			 public static string TitleOfCourtesy = @"TitleOfCourtesy";
			 public static string BirthDate = @"BirthDate";
			 public static string HireDate = @"HireDate";
			 public static string Address = @"Address";
			 public static string City = @"City";
			 public static string Region = @"Region";
			 public static string PostalCode = @"PostalCode";
			 public static string Country = @"Country";
			 public static string HomePhone = @"HomePhone";
			 public static string Extension = @"Extension";
			 public static string Photo = @"Photo";
			 public static string Notes = @"Notes";
			 public static string ReportsTo = @"ReportsTo";
			 public static string PhotoPath = @"PhotoPath";
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
