

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


namespace NorthwindTT
{

    
	/// <summary>
	/// Strongly-typed collection for the CustomerDemographic class.
	/// </summary>
    [Serializable]
	public partial class CustomerDemographicCollection : ActiveList<CustomerDemographic, CustomerDemographicCollection>
	{	   
		public CustomerDemographicCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the CustomerDemographic table.
	/// </summary>
	[Serializable]
    public class CustomerDemographic : ActiveRecord<CustomerDemographic>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("CustomerDemographics", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCustomerDesc = new TableSchema.TableColumn(schema);
				colvarCustomerDesc.ColumnName = "CustomerDesc";
				colvarCustomerDesc.DataType = DbType.String;
				colvarCustomerDesc.MaxLength = 1073741823;
				colvarCustomerDesc.AutoIncrement = true;
				colvarCustomerDesc.IsNullable = true;
				colvarCustomerDesc.IsPrimaryKey = false;
				colvarCustomerDesc.IsForeignKey = false;
				colvarCustomerDesc.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerDesc);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCustomerTypeID = new TableSchema.TableColumn(schema);
				colvarCustomerTypeID.ColumnName = "CustomerTypeID";
				colvarCustomerTypeID.DataType = DbType.String;
				colvarCustomerTypeID.MaxLength = 10;
				colvarCustomerTypeID.AutoIncrement = true;
				colvarCustomerTypeID.IsNullable = false;
				colvarCustomerTypeID.IsPrimaryKey = false;
				colvarCustomerTypeID.IsForeignKey = false;
				colvarCustomerTypeID.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerTypeID);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Region class.
	/// </summary>
    [Serializable]
	public partial class RegionCollection : ActiveList<Region, RegionCollection>
	{	   
		public RegionCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Region table.
	/// </summary>
	[Serializable]
    public class Region : ActiveRecord<Region>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Region", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarRegionDescription = new TableSchema.TableColumn(schema);
				colvarRegionDescription.ColumnName = "RegionDescription";
				colvarRegionDescription.DataType = DbType.String;
				colvarRegionDescription.MaxLength = 50;
				colvarRegionDescription.AutoIncrement = true;
				colvarRegionDescription.IsNullable = false;
				colvarRegionDescription.IsPrimaryKey = false;
				colvarRegionDescription.IsForeignKey = false;
				colvarRegionDescription.IsReadOnly = false;
				schema.Columns.Add(colvarRegionDescription);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRegionID = new TableSchema.TableColumn(schema);
				colvarRegionID.ColumnName = "RegionID";
				colvarRegionID.DataType = DbType.Int32;
				colvarRegionID.MaxLength = 0;
				colvarRegionID.AutoIncrement = true;
				colvarRegionID.IsNullable = false;
				colvarRegionID.IsPrimaryKey = false;
				colvarRegionID.IsForeignKey = false;
				colvarRegionID.IsReadOnly = false;
				schema.Columns.Add(colvarRegionID);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Employee class.
	/// </summary>
    [Serializable]
	public partial class EmployeeCollection : ActiveList<Employee, EmployeeCollection>
	{	   
		public EmployeeCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Employee table.
	/// </summary>
	[Serializable]
    public class Employee : ActiveRecord<Employee>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Employees", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarAddress = new TableSchema.TableColumn(schema);
				colvarAddress.ColumnName = "Address";
				colvarAddress.DataType = DbType.String;
				colvarAddress.MaxLength = 60;
				colvarAddress.AutoIncrement = true;
				colvarAddress.IsNullable = true;
				colvarAddress.IsPrimaryKey = false;
				colvarAddress.IsForeignKey = false;
				colvarAddress.IsReadOnly = false;
				schema.Columns.Add(colvarAddress);

				
				
				
				
				
				
				TableSchema.TableColumn colvarBirthDate = new TableSchema.TableColumn(schema);
				colvarBirthDate.ColumnName = "BirthDate";
				colvarBirthDate.DataType = DbType.DateTime;
				colvarBirthDate.MaxLength = 0;
				colvarBirthDate.AutoIncrement = true;
				colvarBirthDate.IsNullable = true;
				colvarBirthDate.IsPrimaryKey = false;
				colvarBirthDate.IsForeignKey = false;
				colvarBirthDate.IsReadOnly = false;
				schema.Columns.Add(colvarBirthDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCity = new TableSchema.TableColumn(schema);
				colvarCity.ColumnName = "City";
				colvarCity.DataType = DbType.String;
				colvarCity.MaxLength = 15;
				colvarCity.AutoIncrement = true;
				colvarCity.IsNullable = true;
				colvarCity.IsPrimaryKey = false;
				colvarCity.IsForeignKey = false;
				colvarCity.IsReadOnly = false;
				schema.Columns.Add(colvarCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCountry = new TableSchema.TableColumn(schema);
				colvarCountry.ColumnName = "Country";
				colvarCountry.DataType = DbType.String;
				colvarCountry.MaxLength = 15;
				colvarCountry.AutoIncrement = true;
				colvarCountry.IsNullable = true;
				colvarCountry.IsPrimaryKey = false;
				colvarCountry.IsForeignKey = false;
				colvarCountry.IsReadOnly = false;
				schema.Columns.Add(colvarCountry);

				
				
				
				
				
				
				TableSchema.TableColumn colvarDeleted = new TableSchema.TableColumn(schema);
				colvarDeleted.ColumnName = "Deleted";
				colvarDeleted.DataType = DbType.Boolean;
				colvarDeleted.MaxLength = 0;
				colvarDeleted.AutoIncrement = true;
				colvarDeleted.IsNullable = false;
				colvarDeleted.IsPrimaryKey = false;
				colvarDeleted.IsForeignKey = false;
				colvarDeleted.IsReadOnly = false;
				schema.Columns.Add(colvarDeleted);

				
				
				
				
				
				
				TableSchema.TableColumn colvarEmployeeID = new TableSchema.TableColumn(schema);
				colvarEmployeeID.ColumnName = "EmployeeID";
				colvarEmployeeID.DataType = DbType.Int32;
				colvarEmployeeID.MaxLength = 0;
				colvarEmployeeID.AutoIncrement = true;
				colvarEmployeeID.IsNullable = false;
				colvarEmployeeID.IsPrimaryKey = false;
				colvarEmployeeID.IsForeignKey = false;
				colvarEmployeeID.IsReadOnly = false;
				schema.Columns.Add(colvarEmployeeID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarExtension = new TableSchema.TableColumn(schema);
				colvarExtension.ColumnName = "Extension";
				colvarExtension.DataType = DbType.String;
				colvarExtension.MaxLength = 4;
				colvarExtension.AutoIncrement = true;
				colvarExtension.IsNullable = true;
				colvarExtension.IsPrimaryKey = false;
				colvarExtension.IsForeignKey = false;
				colvarExtension.IsReadOnly = false;
				schema.Columns.Add(colvarExtension);

				
				
				
				
				
				
				TableSchema.TableColumn colvarFirstName = new TableSchema.TableColumn(schema);
				colvarFirstName.ColumnName = "FirstName";
				colvarFirstName.DataType = DbType.String;
				colvarFirstName.MaxLength = 10;
				colvarFirstName.AutoIncrement = true;
				colvarFirstName.IsNullable = false;
				colvarFirstName.IsPrimaryKey = false;
				colvarFirstName.IsForeignKey = false;
				colvarFirstName.IsReadOnly = false;
				schema.Columns.Add(colvarFirstName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarHireDate = new TableSchema.TableColumn(schema);
				colvarHireDate.ColumnName = "HireDate";
				colvarHireDate.DataType = DbType.DateTime;
				colvarHireDate.MaxLength = 0;
				colvarHireDate.AutoIncrement = true;
				colvarHireDate.IsNullable = true;
				colvarHireDate.IsPrimaryKey = false;
				colvarHireDate.IsForeignKey = false;
				colvarHireDate.IsReadOnly = false;
				schema.Columns.Add(colvarHireDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarHomePhone = new TableSchema.TableColumn(schema);
				colvarHomePhone.ColumnName = "HomePhone";
				colvarHomePhone.DataType = DbType.String;
				colvarHomePhone.MaxLength = 24;
				colvarHomePhone.AutoIncrement = true;
				colvarHomePhone.IsNullable = true;
				colvarHomePhone.IsPrimaryKey = false;
				colvarHomePhone.IsForeignKey = false;
				colvarHomePhone.IsReadOnly = false;
				schema.Columns.Add(colvarHomePhone);

				
				
				
				
				
				
				TableSchema.TableColumn colvarLastName = new TableSchema.TableColumn(schema);
				colvarLastName.ColumnName = "LastName";
				colvarLastName.DataType = DbType.String;
				colvarLastName.MaxLength = 20;
				colvarLastName.AutoIncrement = true;
				colvarLastName.IsNullable = false;
				colvarLastName.IsPrimaryKey = false;
				colvarLastName.IsForeignKey = false;
				colvarLastName.IsReadOnly = false;
				schema.Columns.Add(colvarLastName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarNotes = new TableSchema.TableColumn(schema);
				colvarNotes.ColumnName = "Notes";
				colvarNotes.DataType = DbType.String;
				colvarNotes.MaxLength = 1073741823;
				colvarNotes.AutoIncrement = true;
				colvarNotes.IsNullable = true;
				colvarNotes.IsPrimaryKey = false;
				colvarNotes.IsForeignKey = false;
				colvarNotes.IsReadOnly = false;
				schema.Columns.Add(colvarNotes);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPhoto = new TableSchema.TableColumn(schema);
				colvarPhoto.ColumnName = "Photo";
				colvarPhoto.DataType = DbType.Binary;
				colvarPhoto.MaxLength = 2147483647;
				colvarPhoto.AutoIncrement = true;
				colvarPhoto.IsNullable = true;
				colvarPhoto.IsPrimaryKey = false;
				colvarPhoto.IsForeignKey = false;
				colvarPhoto.IsReadOnly = false;
				schema.Columns.Add(colvarPhoto);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPhotoPath = new TableSchema.TableColumn(schema);
				colvarPhotoPath.ColumnName = "PhotoPath";
				colvarPhotoPath.DataType = DbType.String;
				colvarPhotoPath.MaxLength = 255;
				colvarPhotoPath.AutoIncrement = true;
				colvarPhotoPath.IsNullable = true;
				colvarPhotoPath.IsPrimaryKey = false;
				colvarPhotoPath.IsForeignKey = false;
				colvarPhotoPath.IsReadOnly = false;
				schema.Columns.Add(colvarPhotoPath);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPostalCode = new TableSchema.TableColumn(schema);
				colvarPostalCode.ColumnName = "PostalCode";
				colvarPostalCode.DataType = DbType.String;
				colvarPostalCode.MaxLength = 10;
				colvarPostalCode.AutoIncrement = true;
				colvarPostalCode.IsNullable = true;
				colvarPostalCode.IsPrimaryKey = false;
				colvarPostalCode.IsForeignKey = false;
				colvarPostalCode.IsReadOnly = false;
				schema.Columns.Add(colvarPostalCode);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRegion = new TableSchema.TableColumn(schema);
				colvarRegion.ColumnName = "Region";
				colvarRegion.DataType = DbType.String;
				colvarRegion.MaxLength = 15;
				colvarRegion.AutoIncrement = true;
				colvarRegion.IsNullable = true;
				colvarRegion.IsPrimaryKey = false;
				colvarRegion.IsForeignKey = false;
				colvarRegion.IsReadOnly = false;
				schema.Columns.Add(colvarRegion);

				
				
				
				
				
				
				TableSchema.TableColumn colvarReportsTo = new TableSchema.TableColumn(schema);
				colvarReportsTo.ColumnName = "ReportsTo";
				colvarReportsTo.DataType = DbType.Int32;
				colvarReportsTo.MaxLength = 0;
				colvarReportsTo.AutoIncrement = true;
				colvarReportsTo.IsNullable = true;
				colvarReportsTo.IsPrimaryKey = false;
				colvarReportsTo.IsForeignKey = false;
				colvarReportsTo.IsReadOnly = false;
				schema.Columns.Add(colvarReportsTo);

				
				
				
				
				
				
				TableSchema.TableColumn colvarTitle = new TableSchema.TableColumn(schema);
				colvarTitle.ColumnName = "Title";
				colvarTitle.DataType = DbType.String;
				colvarTitle.MaxLength = 30;
				colvarTitle.AutoIncrement = true;
				colvarTitle.IsNullable = true;
				colvarTitle.IsPrimaryKey = false;
				colvarTitle.IsForeignKey = false;
				colvarTitle.IsReadOnly = false;
				schema.Columns.Add(colvarTitle);

				
				
				
				
				
				
				TableSchema.TableColumn colvarTitleOfCourtesy = new TableSchema.TableColumn(schema);
				colvarTitleOfCourtesy.ColumnName = "TitleOfCourtesy";
				colvarTitleOfCourtesy.DataType = DbType.String;
				colvarTitleOfCourtesy.MaxLength = 25;
				colvarTitleOfCourtesy.AutoIncrement = true;
				colvarTitleOfCourtesy.IsNullable = true;
				colvarTitleOfCourtesy.IsPrimaryKey = false;
				colvarTitleOfCourtesy.IsForeignKey = false;
				colvarTitleOfCourtesy.IsReadOnly = false;
				schema.Columns.Add(colvarTitleOfCourtesy);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Category class.
	/// </summary>
    [Serializable]
	public partial class CategoryCollection : ActiveList<Category, CategoryCollection>
	{	   
		public CategoryCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Category table.
	/// </summary>
	[Serializable]
    public class Category : ActiveRecord<Category>, IActiveRecord{
    
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
				colvarCategoryID.IsPrimaryKey = false;
				colvarCategoryID.IsForeignKey = false;
				colvarCategoryID.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCategoryName = new TableSchema.TableColumn(schema);
				colvarCategoryName.ColumnName = "CategoryName";
				colvarCategoryName.DataType = DbType.String;
				colvarCategoryName.MaxLength = 15;
				colvarCategoryName.AutoIncrement = true;
				colvarCategoryName.IsNullable = false;
				colvarCategoryName.IsPrimaryKey = false;
				colvarCategoryName.IsForeignKey = false;
				colvarCategoryName.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarDescription = new TableSchema.TableColumn(schema);
				colvarDescription.ColumnName = "Description";
				colvarDescription.DataType = DbType.String;
				colvarDescription.MaxLength = 1073741823;
				colvarDescription.AutoIncrement = true;
				colvarDescription.IsNullable = true;
				colvarDescription.IsPrimaryKey = false;
				colvarDescription.IsForeignKey = false;
				colvarDescription.IsReadOnly = false;
				schema.Columns.Add(colvarDescription);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPicture = new TableSchema.TableColumn(schema);
				colvarPicture.ColumnName = "Picture";
				colvarPicture.DataType = DbType.Binary;
				colvarPicture.MaxLength = 2147483647;
				colvarPicture.AutoIncrement = true;
				colvarPicture.IsNullable = true;
				colvarPicture.IsPrimaryKey = false;
				colvarPicture.IsForeignKey = false;
				colvarPicture.IsReadOnly = false;
				schema.Columns.Add(colvarPicture);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Customer class.
	/// </summary>
    [Serializable]
	public partial class CustomerCollection : ActiveList<Customer, CustomerCollection>
	{	   
		public CustomerCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Customer table.
	/// </summary>
	[Serializable]
    public class Customer : ActiveRecord<Customer>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Customers", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarAddress = new TableSchema.TableColumn(schema);
				colvarAddress.ColumnName = "Address";
				colvarAddress.DataType = DbType.String;
				colvarAddress.MaxLength = 60;
				colvarAddress.AutoIncrement = true;
				colvarAddress.IsNullable = true;
				colvarAddress.IsPrimaryKey = false;
				colvarAddress.IsForeignKey = false;
				colvarAddress.IsReadOnly = false;
				schema.Columns.Add(colvarAddress);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCity = new TableSchema.TableColumn(schema);
				colvarCity.ColumnName = "City";
				colvarCity.DataType = DbType.String;
				colvarCity.MaxLength = 15;
				colvarCity.AutoIncrement = true;
				colvarCity.IsNullable = true;
				colvarCity.IsPrimaryKey = false;
				colvarCity.IsForeignKey = false;
				colvarCity.IsReadOnly = false;
				schema.Columns.Add(colvarCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCompanyName = new TableSchema.TableColumn(schema);
				colvarCompanyName.ColumnName = "CompanyName";
				colvarCompanyName.DataType = DbType.String;
				colvarCompanyName.MaxLength = 40;
				colvarCompanyName.AutoIncrement = true;
				colvarCompanyName.IsNullable = false;
				colvarCompanyName.IsPrimaryKey = false;
				colvarCompanyName.IsForeignKey = false;
				colvarCompanyName.IsReadOnly = false;
				schema.Columns.Add(colvarCompanyName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarContactName = new TableSchema.TableColumn(schema);
				colvarContactName.ColumnName = "ContactName";
				colvarContactName.DataType = DbType.String;
				colvarContactName.MaxLength = 30;
				colvarContactName.AutoIncrement = true;
				colvarContactName.IsNullable = true;
				colvarContactName.IsPrimaryKey = false;
				colvarContactName.IsForeignKey = false;
				colvarContactName.IsReadOnly = false;
				schema.Columns.Add(colvarContactName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarContactTitle = new TableSchema.TableColumn(schema);
				colvarContactTitle.ColumnName = "ContactTitle";
				colvarContactTitle.DataType = DbType.String;
				colvarContactTitle.MaxLength = 30;
				colvarContactTitle.AutoIncrement = true;
				colvarContactTitle.IsNullable = true;
				colvarContactTitle.IsPrimaryKey = false;
				colvarContactTitle.IsForeignKey = false;
				colvarContactTitle.IsReadOnly = false;
				schema.Columns.Add(colvarContactTitle);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCountry = new TableSchema.TableColumn(schema);
				colvarCountry.ColumnName = "Country";
				colvarCountry.DataType = DbType.String;
				colvarCountry.MaxLength = 15;
				colvarCountry.AutoIncrement = true;
				colvarCountry.IsNullable = true;
				colvarCountry.IsPrimaryKey = false;
				colvarCountry.IsForeignKey = false;
				colvarCountry.IsReadOnly = false;
				schema.Columns.Add(colvarCountry);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
				colvarCustomerID.ColumnName = "CustomerID";
				colvarCustomerID.DataType = DbType.String;
				colvarCustomerID.MaxLength = 5;
				colvarCustomerID.AutoIncrement = true;
				colvarCustomerID.IsNullable = false;
				colvarCustomerID.IsPrimaryKey = false;
				colvarCustomerID.IsForeignKey = false;
				colvarCustomerID.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarFax = new TableSchema.TableColumn(schema);
				colvarFax.ColumnName = "Fax";
				colvarFax.DataType = DbType.String;
				colvarFax.MaxLength = 24;
				colvarFax.AutoIncrement = true;
				colvarFax.IsNullable = true;
				colvarFax.IsPrimaryKey = false;
				colvarFax.IsForeignKey = false;
				colvarFax.IsReadOnly = false;
				schema.Columns.Add(colvarFax);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPhone = new TableSchema.TableColumn(schema);
				colvarPhone.ColumnName = "Phone";
				colvarPhone.DataType = DbType.String;
				colvarPhone.MaxLength = 24;
				colvarPhone.AutoIncrement = true;
				colvarPhone.IsNullable = true;
				colvarPhone.IsPrimaryKey = false;
				colvarPhone.IsForeignKey = false;
				colvarPhone.IsReadOnly = false;
				schema.Columns.Add(colvarPhone);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPostalCode = new TableSchema.TableColumn(schema);
				colvarPostalCode.ColumnName = "PostalCode";
				colvarPostalCode.DataType = DbType.String;
				colvarPostalCode.MaxLength = 10;
				colvarPostalCode.AutoIncrement = true;
				colvarPostalCode.IsNullable = true;
				colvarPostalCode.IsPrimaryKey = false;
				colvarPostalCode.IsForeignKey = false;
				colvarPostalCode.IsReadOnly = false;
				schema.Columns.Add(colvarPostalCode);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRegion = new TableSchema.TableColumn(schema);
				colvarRegion.ColumnName = "Region";
				colvarRegion.DataType = DbType.String;
				colvarRegion.MaxLength = 15;
				colvarRegion.AutoIncrement = true;
				colvarRegion.IsNullable = true;
				colvarRegion.IsPrimaryKey = false;
				colvarRegion.IsForeignKey = false;
				colvarRegion.IsReadOnly = false;
				schema.Columns.Add(colvarRegion);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Shipper class.
	/// </summary>
    [Serializable]
	public partial class ShipperCollection : ActiveList<Shipper, ShipperCollection>
	{	   
		public ShipperCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Shipper table.
	/// </summary>
	[Serializable]
    public class Shipper : ActiveRecord<Shipper>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Shippers", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCompanyName = new TableSchema.TableColumn(schema);
				colvarCompanyName.ColumnName = "CompanyName";
				colvarCompanyName.DataType = DbType.String;
				colvarCompanyName.MaxLength = 40;
				colvarCompanyName.AutoIncrement = true;
				colvarCompanyName.IsNullable = false;
				colvarCompanyName.IsPrimaryKey = false;
				colvarCompanyName.IsForeignKey = false;
				colvarCompanyName.IsReadOnly = false;
				schema.Columns.Add(colvarCompanyName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPhone = new TableSchema.TableColumn(schema);
				colvarPhone.ColumnName = "Phone";
				colvarPhone.DataType = DbType.String;
				colvarPhone.MaxLength = 24;
				colvarPhone.AutoIncrement = true;
				colvarPhone.IsNullable = true;
				colvarPhone.IsPrimaryKey = false;
				colvarPhone.IsForeignKey = false;
				colvarPhone.IsReadOnly = false;
				schema.Columns.Add(colvarPhone);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipperID = new TableSchema.TableColumn(schema);
				colvarShipperID.ColumnName = "ShipperID";
				colvarShipperID.DataType = DbType.Int32;
				colvarShipperID.MaxLength = 0;
				colvarShipperID.AutoIncrement = true;
				colvarShipperID.IsNullable = false;
				colvarShipperID.IsPrimaryKey = false;
				colvarShipperID.IsForeignKey = false;
				colvarShipperID.IsReadOnly = false;
				schema.Columns.Add(colvarShipperID);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Supplier class.
	/// </summary>
    [Serializable]
	public partial class SupplierCollection : ActiveList<Supplier, SupplierCollection>
	{	   
		public SupplierCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Supplier table.
	/// </summary>
	[Serializable]
    public class Supplier : ActiveRecord<Supplier>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Suppliers", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarAddress = new TableSchema.TableColumn(schema);
				colvarAddress.ColumnName = "Address";
				colvarAddress.DataType = DbType.String;
				colvarAddress.MaxLength = 60;
				colvarAddress.AutoIncrement = true;
				colvarAddress.IsNullable = true;
				colvarAddress.IsPrimaryKey = false;
				colvarAddress.IsForeignKey = false;
				colvarAddress.IsReadOnly = false;
				schema.Columns.Add(colvarAddress);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCity = new TableSchema.TableColumn(schema);
				colvarCity.ColumnName = "City";
				colvarCity.DataType = DbType.String;
				colvarCity.MaxLength = 15;
				colvarCity.AutoIncrement = true;
				colvarCity.IsNullable = true;
				colvarCity.IsPrimaryKey = false;
				colvarCity.IsForeignKey = false;
				colvarCity.IsReadOnly = false;
				schema.Columns.Add(colvarCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCompanyName = new TableSchema.TableColumn(schema);
				colvarCompanyName.ColumnName = "CompanyName";
				colvarCompanyName.DataType = DbType.String;
				colvarCompanyName.MaxLength = 40;
				colvarCompanyName.AutoIncrement = true;
				colvarCompanyName.IsNullable = false;
				colvarCompanyName.IsPrimaryKey = false;
				colvarCompanyName.IsForeignKey = false;
				colvarCompanyName.IsReadOnly = false;
				schema.Columns.Add(colvarCompanyName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarContactName = new TableSchema.TableColumn(schema);
				colvarContactName.ColumnName = "ContactName";
				colvarContactName.DataType = DbType.String;
				colvarContactName.MaxLength = 30;
				colvarContactName.AutoIncrement = true;
				colvarContactName.IsNullable = true;
				colvarContactName.IsPrimaryKey = false;
				colvarContactName.IsForeignKey = false;
				colvarContactName.IsReadOnly = false;
				schema.Columns.Add(colvarContactName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarContactTitle = new TableSchema.TableColumn(schema);
				colvarContactTitle.ColumnName = "ContactTitle";
				colvarContactTitle.DataType = DbType.String;
				colvarContactTitle.MaxLength = 30;
				colvarContactTitle.AutoIncrement = true;
				colvarContactTitle.IsNullable = true;
				colvarContactTitle.IsPrimaryKey = false;
				colvarContactTitle.IsForeignKey = false;
				colvarContactTitle.IsReadOnly = false;
				schema.Columns.Add(colvarContactTitle);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCountry = new TableSchema.TableColumn(schema);
				colvarCountry.ColumnName = "Country";
				colvarCountry.DataType = DbType.String;
				colvarCountry.MaxLength = 15;
				colvarCountry.AutoIncrement = true;
				colvarCountry.IsNullable = true;
				colvarCountry.IsPrimaryKey = false;
				colvarCountry.IsForeignKey = false;
				colvarCountry.IsReadOnly = false;
				schema.Columns.Add(colvarCountry);

				
				
				
				
				
				
				TableSchema.TableColumn colvarFax = new TableSchema.TableColumn(schema);
				colvarFax.ColumnName = "Fax";
				colvarFax.DataType = DbType.String;
				colvarFax.MaxLength = 24;
				colvarFax.AutoIncrement = true;
				colvarFax.IsNullable = true;
				colvarFax.IsPrimaryKey = false;
				colvarFax.IsForeignKey = false;
				colvarFax.IsReadOnly = false;
				schema.Columns.Add(colvarFax);

				
				
				
				
				
				
				TableSchema.TableColumn colvarHomePage = new TableSchema.TableColumn(schema);
				colvarHomePage.ColumnName = "HomePage";
				colvarHomePage.DataType = DbType.String;
				colvarHomePage.MaxLength = 1073741823;
				colvarHomePage.AutoIncrement = true;
				colvarHomePage.IsNullable = true;
				colvarHomePage.IsPrimaryKey = false;
				colvarHomePage.IsForeignKey = false;
				colvarHomePage.IsReadOnly = false;
				schema.Columns.Add(colvarHomePage);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPhone = new TableSchema.TableColumn(schema);
				colvarPhone.ColumnName = "Phone";
				colvarPhone.DataType = DbType.String;
				colvarPhone.MaxLength = 24;
				colvarPhone.AutoIncrement = true;
				colvarPhone.IsNullable = true;
				colvarPhone.IsPrimaryKey = false;
				colvarPhone.IsForeignKey = false;
				colvarPhone.IsReadOnly = false;
				schema.Columns.Add(colvarPhone);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPostalCode = new TableSchema.TableColumn(schema);
				colvarPostalCode.ColumnName = "PostalCode";
				colvarPostalCode.DataType = DbType.String;
				colvarPostalCode.MaxLength = 10;
				colvarPostalCode.AutoIncrement = true;
				colvarPostalCode.IsNullable = true;
				colvarPostalCode.IsPrimaryKey = false;
				colvarPostalCode.IsForeignKey = false;
				colvarPostalCode.IsReadOnly = false;
				schema.Columns.Add(colvarPostalCode);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRegion = new TableSchema.TableColumn(schema);
				colvarRegion.ColumnName = "Region";
				colvarRegion.DataType = DbType.String;
				colvarRegion.MaxLength = 15;
				colvarRegion.AutoIncrement = true;
				colvarRegion.IsNullable = true;
				colvarRegion.IsPrimaryKey = false;
				colvarRegion.IsForeignKey = false;
				colvarRegion.IsReadOnly = false;
				schema.Columns.Add(colvarRegion);

				
				
				
				
				
				
				TableSchema.TableColumn colvarSupplierID = new TableSchema.TableColumn(schema);
				colvarSupplierID.ColumnName = "SupplierID";
				colvarSupplierID.DataType = DbType.Int32;
				colvarSupplierID.MaxLength = 0;
				colvarSupplierID.AutoIncrement = true;
				colvarSupplierID.IsNullable = false;
				colvarSupplierID.IsPrimaryKey = false;
				colvarSupplierID.IsForeignKey = false;
				colvarSupplierID.IsReadOnly = false;
				schema.Columns.Add(colvarSupplierID);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the EmployeeTerritory class.
	/// </summary>
    [Serializable]
	public partial class EmployeeTerritoryCollection : ActiveList<EmployeeTerritory, EmployeeTerritoryCollection>
	{	   
		public EmployeeTerritoryCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the EmployeeTerritory table.
	/// </summary>
	[Serializable]
    public class EmployeeTerritory : ActiveRecord<EmployeeTerritory>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("EmployeeTerritories", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarEmployeeID = new TableSchema.TableColumn(schema);
				colvarEmployeeID.ColumnName = "EmployeeID";
				colvarEmployeeID.DataType = DbType.Int32;
				colvarEmployeeID.MaxLength = 0;
				colvarEmployeeID.AutoIncrement = true;
				colvarEmployeeID.IsNullable = false;
				colvarEmployeeID.IsPrimaryKey = false;
				colvarEmployeeID.IsForeignKey = false;
				colvarEmployeeID.IsReadOnly = false;
				schema.Columns.Add(colvarEmployeeID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarTerritoryID = new TableSchema.TableColumn(schema);
				colvarTerritoryID.ColumnName = "TerritoryID";
				colvarTerritoryID.DataType = DbType.String;
				colvarTerritoryID.MaxLength = 20;
				colvarTerritoryID.AutoIncrement = true;
				colvarTerritoryID.IsNullable = false;
				colvarTerritoryID.IsPrimaryKey = false;
				colvarTerritoryID.IsForeignKey = false;
				colvarTerritoryID.IsReadOnly = false;
				schema.Columns.Add(colvarTerritoryID);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Order_Detail class.
	/// </summary>
    [Serializable]
	public partial class Order_DetailCollection : ActiveList<Order_Detail, Order_DetailCollection>
	{	   
		public Order_DetailCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Order_Detail table.
	/// </summary>
	[Serializable]
    public class Order_Detail : ActiveRecord<Order_Detail>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Order Details", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarDiscount = new TableSchema.TableColumn(schema);
				colvarDiscount.ColumnName = "Discount";
				colvarDiscount.DataType = DbType.Single;
				colvarDiscount.MaxLength = 0;
				colvarDiscount.AutoIncrement = true;
				colvarDiscount.IsNullable = false;
				colvarDiscount.IsPrimaryKey = false;
				colvarDiscount.IsForeignKey = false;
				colvarDiscount.IsReadOnly = false;
				schema.Columns.Add(colvarDiscount);

				
				
				
				
				
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = false;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				schema.Columns.Add(colvarOrderID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductID = new TableSchema.TableColumn(schema);
				colvarProductID.ColumnName = "ProductID";
				colvarProductID.DataType = DbType.Int32;
				colvarProductID.MaxLength = 0;
				colvarProductID.AutoIncrement = true;
				colvarProductID.IsNullable = false;
				colvarProductID.IsPrimaryKey = false;
				colvarProductID.IsForeignKey = false;
				colvarProductID.IsReadOnly = false;
				schema.Columns.Add(colvarProductID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarQuantity = new TableSchema.TableColumn(schema);
				colvarQuantity.ColumnName = "Quantity";
				colvarQuantity.DataType = DbType.Int16;
				colvarQuantity.MaxLength = 0;
				colvarQuantity.AutoIncrement = true;
				colvarQuantity.IsNullable = false;
				colvarQuantity.IsPrimaryKey = false;
				colvarQuantity.IsForeignKey = false;
				colvarQuantity.IsReadOnly = false;
				schema.Columns.Add(colvarQuantity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
				colvarUnitPrice.ColumnName = "UnitPrice";
				colvarUnitPrice.DataType = DbType.Currency;
				colvarUnitPrice.MaxLength = 0;
				colvarUnitPrice.AutoIncrement = true;
				colvarUnitPrice.IsNullable = false;
				colvarUnitPrice.IsPrimaryKey = false;
				colvarUnitPrice.IsForeignKey = false;
				colvarUnitPrice.IsReadOnly = false;
				schema.Columns.Add(colvarUnitPrice);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the CustomerCustomerDemo class.
	/// </summary>
    [Serializable]
	public partial class CustomerCustomerDemoCollection : ActiveList<CustomerCustomerDemo, CustomerCustomerDemoCollection>
	{	   
		public CustomerCustomerDemoCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the CustomerCustomerDemo table.
	/// </summary>
	[Serializable]
    public class CustomerCustomerDemo : ActiveRecord<CustomerCustomerDemo>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("CustomerCustomerDemo", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
				colvarCustomerID.ColumnName = "CustomerID";
				colvarCustomerID.DataType = DbType.String;
				colvarCustomerID.MaxLength = 5;
				colvarCustomerID.AutoIncrement = true;
				colvarCustomerID.IsNullable = false;
				colvarCustomerID.IsPrimaryKey = false;
				colvarCustomerID.IsForeignKey = false;
				colvarCustomerID.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCustomerTypeID = new TableSchema.TableColumn(schema);
				colvarCustomerTypeID.ColumnName = "CustomerTypeID";
				colvarCustomerTypeID.DataType = DbType.String;
				colvarCustomerTypeID.MaxLength = 10;
				colvarCustomerTypeID.AutoIncrement = true;
				colvarCustomerTypeID.IsNullable = false;
				colvarCustomerTypeID.IsPrimaryKey = false;
				colvarCustomerTypeID.IsForeignKey = false;
				colvarCustomerTypeID.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerTypeID);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Territory class.
	/// </summary>
    [Serializable]
	public partial class TerritoryCollection : ActiveList<Territory, TerritoryCollection>
	{	   
		public TerritoryCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Territory table.
	/// </summary>
	[Serializable]
    public class Territory : ActiveRecord<Territory>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Territories", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarRegionID = new TableSchema.TableColumn(schema);
				colvarRegionID.ColumnName = "RegionID";
				colvarRegionID.DataType = DbType.Int32;
				colvarRegionID.MaxLength = 0;
				colvarRegionID.AutoIncrement = true;
				colvarRegionID.IsNullable = false;
				colvarRegionID.IsPrimaryKey = false;
				colvarRegionID.IsForeignKey = false;
				colvarRegionID.IsReadOnly = false;
				schema.Columns.Add(colvarRegionID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarTerritoryDescription = new TableSchema.TableColumn(schema);
				colvarTerritoryDescription.ColumnName = "TerritoryDescription";
				colvarTerritoryDescription.DataType = DbType.String;
				colvarTerritoryDescription.MaxLength = 50;
				colvarTerritoryDescription.AutoIncrement = true;
				colvarTerritoryDescription.IsNullable = false;
				colvarTerritoryDescription.IsPrimaryKey = false;
				colvarTerritoryDescription.IsForeignKey = false;
				colvarTerritoryDescription.IsReadOnly = false;
				schema.Columns.Add(colvarTerritoryDescription);

				
				
				
				
				
				
				TableSchema.TableColumn colvarTerritoryID = new TableSchema.TableColumn(schema);
				colvarTerritoryID.ColumnName = "TerritoryID";
				colvarTerritoryID.DataType = DbType.String;
				colvarTerritoryID.MaxLength = 20;
				colvarTerritoryID.AutoIncrement = true;
				colvarTerritoryID.IsNullable = false;
				colvarTerritoryID.IsPrimaryKey = false;
				colvarTerritoryID.IsForeignKey = false;
				colvarTerritoryID.IsReadOnly = false;
				schema.Columns.Add(colvarTerritoryID);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Order class.
	/// </summary>
    [Serializable]
	public partial class OrderCollection : ActiveList<Order, OrderCollection>
	{	   
		public OrderCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Order table.
	/// </summary>
	[Serializable]
    public class Order : ActiveRecord<Order>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Orders", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
				colvarCustomerID.ColumnName = "CustomerID";
				colvarCustomerID.DataType = DbType.String;
				colvarCustomerID.MaxLength = 5;
				colvarCustomerID.AutoIncrement = true;
				colvarCustomerID.IsNullable = true;
				colvarCustomerID.IsPrimaryKey = false;
				colvarCustomerID.IsForeignKey = false;
				colvarCustomerID.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarEmployeeID = new TableSchema.TableColumn(schema);
				colvarEmployeeID.ColumnName = "EmployeeID";
				colvarEmployeeID.DataType = DbType.Int32;
				colvarEmployeeID.MaxLength = 0;
				colvarEmployeeID.AutoIncrement = true;
				colvarEmployeeID.IsNullable = true;
				colvarEmployeeID.IsPrimaryKey = false;
				colvarEmployeeID.IsForeignKey = false;
				colvarEmployeeID.IsReadOnly = false;
				schema.Columns.Add(colvarEmployeeID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarFreight = new TableSchema.TableColumn(schema);
				colvarFreight.ColumnName = "Freight";
				colvarFreight.DataType = DbType.Currency;
				colvarFreight.MaxLength = 0;
				colvarFreight.AutoIncrement = true;
				colvarFreight.IsNullable = true;
				colvarFreight.IsPrimaryKey = false;
				colvarFreight.IsForeignKey = false;
				colvarFreight.IsReadOnly = false;
				schema.Columns.Add(colvarFreight);

				
				
				
				
				
				
				TableSchema.TableColumn colvarOrderDate = new TableSchema.TableColumn(schema);
				colvarOrderDate.ColumnName = "OrderDate";
				colvarOrderDate.DataType = DbType.DateTime;
				colvarOrderDate.MaxLength = 0;
				colvarOrderDate.AutoIncrement = true;
				colvarOrderDate.IsNullable = true;
				colvarOrderDate.IsPrimaryKey = false;
				colvarOrderDate.IsForeignKey = false;
				colvarOrderDate.IsReadOnly = false;
				schema.Columns.Add(colvarOrderDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = false;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				schema.Columns.Add(colvarOrderID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRequiredDate = new TableSchema.TableColumn(schema);
				colvarRequiredDate.ColumnName = "RequiredDate";
				colvarRequiredDate.DataType = DbType.DateTime;
				colvarRequiredDate.MaxLength = 0;
				colvarRequiredDate.AutoIncrement = true;
				colvarRequiredDate.IsNullable = true;
				colvarRequiredDate.IsPrimaryKey = false;
				colvarRequiredDate.IsForeignKey = false;
				colvarRequiredDate.IsReadOnly = false;
				schema.Columns.Add(colvarRequiredDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipAddress = new TableSchema.TableColumn(schema);
				colvarShipAddress.ColumnName = "ShipAddress";
				colvarShipAddress.DataType = DbType.String;
				colvarShipAddress.MaxLength = 60;
				colvarShipAddress.AutoIncrement = true;
				colvarShipAddress.IsNullable = true;
				colvarShipAddress.IsPrimaryKey = false;
				colvarShipAddress.IsForeignKey = false;
				colvarShipAddress.IsReadOnly = false;
				schema.Columns.Add(colvarShipAddress);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipCity = new TableSchema.TableColumn(schema);
				colvarShipCity.ColumnName = "ShipCity";
				colvarShipCity.DataType = DbType.String;
				colvarShipCity.MaxLength = 15;
				colvarShipCity.AutoIncrement = true;
				colvarShipCity.IsNullable = true;
				colvarShipCity.IsPrimaryKey = false;
				colvarShipCity.IsForeignKey = false;
				colvarShipCity.IsReadOnly = false;
				schema.Columns.Add(colvarShipCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipCountry = new TableSchema.TableColumn(schema);
				colvarShipCountry.ColumnName = "ShipCountry";
				colvarShipCountry.DataType = DbType.String;
				colvarShipCountry.MaxLength = 15;
				colvarShipCountry.AutoIncrement = true;
				colvarShipCountry.IsNullable = true;
				colvarShipCountry.IsPrimaryKey = false;
				colvarShipCountry.IsForeignKey = false;
				colvarShipCountry.IsReadOnly = false;
				schema.Columns.Add(colvarShipCountry);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipName = new TableSchema.TableColumn(schema);
				colvarShipName.ColumnName = "ShipName";
				colvarShipName.DataType = DbType.String;
				colvarShipName.MaxLength = 40;
				colvarShipName.AutoIncrement = true;
				colvarShipName.IsNullable = true;
				colvarShipName.IsPrimaryKey = false;
				colvarShipName.IsForeignKey = false;
				colvarShipName.IsReadOnly = false;
				schema.Columns.Add(colvarShipName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShippedDate = new TableSchema.TableColumn(schema);
				colvarShippedDate.ColumnName = "ShippedDate";
				colvarShippedDate.DataType = DbType.DateTime;
				colvarShippedDate.MaxLength = 0;
				colvarShippedDate.AutoIncrement = true;
				colvarShippedDate.IsNullable = true;
				colvarShippedDate.IsPrimaryKey = false;
				colvarShippedDate.IsForeignKey = false;
				colvarShippedDate.IsReadOnly = false;
				schema.Columns.Add(colvarShippedDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipPostalCode = new TableSchema.TableColumn(schema);
				colvarShipPostalCode.ColumnName = "ShipPostalCode";
				colvarShipPostalCode.DataType = DbType.String;
				colvarShipPostalCode.MaxLength = 10;
				colvarShipPostalCode.AutoIncrement = true;
				colvarShipPostalCode.IsNullable = true;
				colvarShipPostalCode.IsPrimaryKey = false;
				colvarShipPostalCode.IsForeignKey = false;
				colvarShipPostalCode.IsReadOnly = false;
				schema.Columns.Add(colvarShipPostalCode);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipRegion = new TableSchema.TableColumn(schema);
				colvarShipRegion.ColumnName = "ShipRegion";
				colvarShipRegion.DataType = DbType.String;
				colvarShipRegion.MaxLength = 15;
				colvarShipRegion.AutoIncrement = true;
				colvarShipRegion.IsNullable = true;
				colvarShipRegion.IsPrimaryKey = false;
				colvarShipRegion.IsForeignKey = false;
				colvarShipRegion.IsReadOnly = false;
				schema.Columns.Add(colvarShipRegion);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipVia = new TableSchema.TableColumn(schema);
				colvarShipVia.ColumnName = "ShipVia";
				colvarShipVia.DataType = DbType.Int32;
				colvarShipVia.MaxLength = 0;
				colvarShipVia.AutoIncrement = true;
				colvarShipVia.IsNullable = true;
				colvarShipVia.IsPrimaryKey = false;
				colvarShipVia.IsForeignKey = false;
				colvarShipVia.IsReadOnly = false;
				schema.Columns.Add(colvarShipVia);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Product class.
	/// </summary>
    [Serializable]
	public partial class ProductCollection : ActiveList<Product, ProductCollection>
	{	   
		public ProductCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Product table.
	/// </summary>
	[Serializable]
    public class Product : ActiveRecord<Product>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Products", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCategoryID = new TableSchema.TableColumn(schema);
				colvarCategoryID.ColumnName = "CategoryID";
				colvarCategoryID.DataType = DbType.Int32;
				colvarCategoryID.MaxLength = 0;
				colvarCategoryID.AutoIncrement = true;
				colvarCategoryID.IsNullable = true;
				colvarCategoryID.IsPrimaryKey = false;
				colvarCategoryID.IsForeignKey = false;
				colvarCategoryID.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarDiscontinued = new TableSchema.TableColumn(schema);
				colvarDiscontinued.ColumnName = "Discontinued";
				colvarDiscontinued.DataType = DbType.Boolean;
				colvarDiscontinued.MaxLength = 0;
				colvarDiscontinued.AutoIncrement = true;
				colvarDiscontinued.IsNullable = false;
				colvarDiscontinued.IsPrimaryKey = false;
				colvarDiscontinued.IsForeignKey = false;
				colvarDiscontinued.IsReadOnly = false;
				schema.Columns.Add(colvarDiscontinued);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductID = new TableSchema.TableColumn(schema);
				colvarProductID.ColumnName = "ProductID";
				colvarProductID.DataType = DbType.Int32;
				colvarProductID.MaxLength = 0;
				colvarProductID.AutoIncrement = true;
				colvarProductID.IsNullable = false;
				colvarProductID.IsPrimaryKey = false;
				colvarProductID.IsForeignKey = false;
				colvarProductID.IsReadOnly = false;
				schema.Columns.Add(colvarProductID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = true;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				schema.Columns.Add(colvarProductName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarQuantityPerUnit = new TableSchema.TableColumn(schema);
				colvarQuantityPerUnit.ColumnName = "QuantityPerUnit";
				colvarQuantityPerUnit.DataType = DbType.String;
				colvarQuantityPerUnit.MaxLength = 20;
				colvarQuantityPerUnit.AutoIncrement = true;
				colvarQuantityPerUnit.IsNullable = true;
				colvarQuantityPerUnit.IsPrimaryKey = false;
				colvarQuantityPerUnit.IsForeignKey = false;
				colvarQuantityPerUnit.IsReadOnly = false;
				schema.Columns.Add(colvarQuantityPerUnit);

				
				
				
				
				
				
				TableSchema.TableColumn colvarReorderLevel = new TableSchema.TableColumn(schema);
				colvarReorderLevel.ColumnName = "ReorderLevel";
				colvarReorderLevel.DataType = DbType.Int16;
				colvarReorderLevel.MaxLength = 0;
				colvarReorderLevel.AutoIncrement = true;
				colvarReorderLevel.IsNullable = true;
				colvarReorderLevel.IsPrimaryKey = false;
				colvarReorderLevel.IsForeignKey = false;
				colvarReorderLevel.IsReadOnly = false;
				schema.Columns.Add(colvarReorderLevel);

				
				
				
				
				
				
				TableSchema.TableColumn colvarSupplierID = new TableSchema.TableColumn(schema);
				colvarSupplierID.ColumnName = "SupplierID";
				colvarSupplierID.DataType = DbType.Int32;
				colvarSupplierID.MaxLength = 0;
				colvarSupplierID.AutoIncrement = true;
				colvarSupplierID.IsNullable = true;
				colvarSupplierID.IsPrimaryKey = false;
				colvarSupplierID.IsForeignKey = false;
				colvarSupplierID.IsReadOnly = false;
				schema.Columns.Add(colvarSupplierID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
				colvarUnitPrice.ColumnName = "UnitPrice";
				colvarUnitPrice.DataType = DbType.Currency;
				colvarUnitPrice.MaxLength = 0;
				colvarUnitPrice.AutoIncrement = true;
				colvarUnitPrice.IsNullable = true;
				colvarUnitPrice.IsPrimaryKey = false;
				colvarUnitPrice.IsForeignKey = false;
				colvarUnitPrice.IsReadOnly = false;
				schema.Columns.Add(colvarUnitPrice);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitsInStock = new TableSchema.TableColumn(schema);
				colvarUnitsInStock.ColumnName = "UnitsInStock";
				colvarUnitsInStock.DataType = DbType.Int16;
				colvarUnitsInStock.MaxLength = 0;
				colvarUnitsInStock.AutoIncrement = true;
				colvarUnitsInStock.IsNullable = true;
				colvarUnitsInStock.IsPrimaryKey = false;
				colvarUnitsInStock.IsForeignKey = false;
				colvarUnitsInStock.IsReadOnly = false;
				schema.Columns.Add(colvarUnitsInStock);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitsOnOrder = new TableSchema.TableColumn(schema);
				colvarUnitsOnOrder.ColumnName = "UnitsOnOrder";
				colvarUnitsOnOrder.DataType = DbType.Int16;
				colvarUnitsOnOrder.MaxLength = 0;
				colvarUnitsOnOrder.AutoIncrement = true;
				colvarUnitsOnOrder.IsNullable = true;
				colvarUnitsOnOrder.IsPrimaryKey = false;
				colvarUnitsOnOrder.IsForeignKey = false;
				colvarUnitsOnOrder.IsReadOnly = false;
				schema.Columns.Add(colvarUnitsOnOrder);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Orders_Qry class.
	/// </summary>
    [Serializable]
	public partial class Orders_QryCollection : ActiveList<Orders_Qry, Orders_QryCollection>
	{	   
		public Orders_QryCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Orders_Qry table.
	/// </summary>
	[Serializable]
    public class Orders_Qry : ActiveRecord<Orders_Qry>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Orders Qry", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarAddress = new TableSchema.TableColumn(schema);
				colvarAddress.ColumnName = "Address";
				colvarAddress.DataType = DbType.String;
				colvarAddress.MaxLength = 60;
				colvarAddress.AutoIncrement = true;
				colvarAddress.IsNullable = true;
				colvarAddress.IsPrimaryKey = false;
				colvarAddress.IsForeignKey = false;
				colvarAddress.IsReadOnly = false;
				schema.Columns.Add(colvarAddress);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCity = new TableSchema.TableColumn(schema);
				colvarCity.ColumnName = "City";
				colvarCity.DataType = DbType.String;
				colvarCity.MaxLength = 15;
				colvarCity.AutoIncrement = true;
				colvarCity.IsNullable = true;
				colvarCity.IsPrimaryKey = false;
				colvarCity.IsForeignKey = false;
				colvarCity.IsReadOnly = false;
				schema.Columns.Add(colvarCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCompanyName = new TableSchema.TableColumn(schema);
				colvarCompanyName.ColumnName = "CompanyName";
				colvarCompanyName.DataType = DbType.String;
				colvarCompanyName.MaxLength = 40;
				colvarCompanyName.AutoIncrement = true;
				colvarCompanyName.IsNullable = false;
				colvarCompanyName.IsPrimaryKey = false;
				colvarCompanyName.IsForeignKey = false;
				colvarCompanyName.IsReadOnly = false;
				schema.Columns.Add(colvarCompanyName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCountry = new TableSchema.TableColumn(schema);
				colvarCountry.ColumnName = "Country";
				colvarCountry.DataType = DbType.String;
				colvarCountry.MaxLength = 15;
				colvarCountry.AutoIncrement = true;
				colvarCountry.IsNullable = true;
				colvarCountry.IsPrimaryKey = false;
				colvarCountry.IsForeignKey = false;
				colvarCountry.IsReadOnly = false;
				schema.Columns.Add(colvarCountry);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
				colvarCustomerID.ColumnName = "CustomerID";
				colvarCustomerID.DataType = DbType.String;
				colvarCustomerID.MaxLength = 5;
				colvarCustomerID.AutoIncrement = true;
				colvarCustomerID.IsNullable = true;
				colvarCustomerID.IsPrimaryKey = false;
				colvarCustomerID.IsForeignKey = false;
				colvarCustomerID.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarEmployeeID = new TableSchema.TableColumn(schema);
				colvarEmployeeID.ColumnName = "EmployeeID";
				colvarEmployeeID.DataType = DbType.Int32;
				colvarEmployeeID.MaxLength = 0;
				colvarEmployeeID.AutoIncrement = true;
				colvarEmployeeID.IsNullable = true;
				colvarEmployeeID.IsPrimaryKey = false;
				colvarEmployeeID.IsForeignKey = false;
				colvarEmployeeID.IsReadOnly = false;
				schema.Columns.Add(colvarEmployeeID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarFreight = new TableSchema.TableColumn(schema);
				colvarFreight.ColumnName = "Freight";
				colvarFreight.DataType = DbType.Currency;
				colvarFreight.MaxLength = 0;
				colvarFreight.AutoIncrement = true;
				colvarFreight.IsNullable = true;
				colvarFreight.IsPrimaryKey = false;
				colvarFreight.IsForeignKey = false;
				colvarFreight.IsReadOnly = false;
				schema.Columns.Add(colvarFreight);

				
				
				
				
				
				
				TableSchema.TableColumn colvarOrderDate = new TableSchema.TableColumn(schema);
				colvarOrderDate.ColumnName = "OrderDate";
				colvarOrderDate.DataType = DbType.DateTime;
				colvarOrderDate.MaxLength = 0;
				colvarOrderDate.AutoIncrement = true;
				colvarOrderDate.IsNullable = true;
				colvarOrderDate.IsPrimaryKey = false;
				colvarOrderDate.IsForeignKey = false;
				colvarOrderDate.IsReadOnly = false;
				schema.Columns.Add(colvarOrderDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = false;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				schema.Columns.Add(colvarOrderID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPostalCode = new TableSchema.TableColumn(schema);
				colvarPostalCode.ColumnName = "PostalCode";
				colvarPostalCode.DataType = DbType.String;
				colvarPostalCode.MaxLength = 10;
				colvarPostalCode.AutoIncrement = true;
				colvarPostalCode.IsNullable = true;
				colvarPostalCode.IsPrimaryKey = false;
				colvarPostalCode.IsForeignKey = false;
				colvarPostalCode.IsReadOnly = false;
				schema.Columns.Add(colvarPostalCode);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRegion = new TableSchema.TableColumn(schema);
				colvarRegion.ColumnName = "Region";
				colvarRegion.DataType = DbType.String;
				colvarRegion.MaxLength = 15;
				colvarRegion.AutoIncrement = true;
				colvarRegion.IsNullable = true;
				colvarRegion.IsPrimaryKey = false;
				colvarRegion.IsForeignKey = false;
				colvarRegion.IsReadOnly = false;
				schema.Columns.Add(colvarRegion);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRequiredDate = new TableSchema.TableColumn(schema);
				colvarRequiredDate.ColumnName = "RequiredDate";
				colvarRequiredDate.DataType = DbType.DateTime;
				colvarRequiredDate.MaxLength = 0;
				colvarRequiredDate.AutoIncrement = true;
				colvarRequiredDate.IsNullable = true;
				colvarRequiredDate.IsPrimaryKey = false;
				colvarRequiredDate.IsForeignKey = false;
				colvarRequiredDate.IsReadOnly = false;
				schema.Columns.Add(colvarRequiredDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipAddress = new TableSchema.TableColumn(schema);
				colvarShipAddress.ColumnName = "ShipAddress";
				colvarShipAddress.DataType = DbType.String;
				colvarShipAddress.MaxLength = 60;
				colvarShipAddress.AutoIncrement = true;
				colvarShipAddress.IsNullable = true;
				colvarShipAddress.IsPrimaryKey = false;
				colvarShipAddress.IsForeignKey = false;
				colvarShipAddress.IsReadOnly = false;
				schema.Columns.Add(colvarShipAddress);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipCity = new TableSchema.TableColumn(schema);
				colvarShipCity.ColumnName = "ShipCity";
				colvarShipCity.DataType = DbType.String;
				colvarShipCity.MaxLength = 15;
				colvarShipCity.AutoIncrement = true;
				colvarShipCity.IsNullable = true;
				colvarShipCity.IsPrimaryKey = false;
				colvarShipCity.IsForeignKey = false;
				colvarShipCity.IsReadOnly = false;
				schema.Columns.Add(colvarShipCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipCountry = new TableSchema.TableColumn(schema);
				colvarShipCountry.ColumnName = "ShipCountry";
				colvarShipCountry.DataType = DbType.String;
				colvarShipCountry.MaxLength = 15;
				colvarShipCountry.AutoIncrement = true;
				colvarShipCountry.IsNullable = true;
				colvarShipCountry.IsPrimaryKey = false;
				colvarShipCountry.IsForeignKey = false;
				colvarShipCountry.IsReadOnly = false;
				schema.Columns.Add(colvarShipCountry);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipName = new TableSchema.TableColumn(schema);
				colvarShipName.ColumnName = "ShipName";
				colvarShipName.DataType = DbType.String;
				colvarShipName.MaxLength = 40;
				colvarShipName.AutoIncrement = true;
				colvarShipName.IsNullable = true;
				colvarShipName.IsPrimaryKey = false;
				colvarShipName.IsForeignKey = false;
				colvarShipName.IsReadOnly = false;
				schema.Columns.Add(colvarShipName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShippedDate = new TableSchema.TableColumn(schema);
				colvarShippedDate.ColumnName = "ShippedDate";
				colvarShippedDate.DataType = DbType.DateTime;
				colvarShippedDate.MaxLength = 0;
				colvarShippedDate.AutoIncrement = true;
				colvarShippedDate.IsNullable = true;
				colvarShippedDate.IsPrimaryKey = false;
				colvarShippedDate.IsForeignKey = false;
				colvarShippedDate.IsReadOnly = false;
				schema.Columns.Add(colvarShippedDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipPostalCode = new TableSchema.TableColumn(schema);
				colvarShipPostalCode.ColumnName = "ShipPostalCode";
				colvarShipPostalCode.DataType = DbType.String;
				colvarShipPostalCode.MaxLength = 10;
				colvarShipPostalCode.AutoIncrement = true;
				colvarShipPostalCode.IsNullable = true;
				colvarShipPostalCode.IsPrimaryKey = false;
				colvarShipPostalCode.IsForeignKey = false;
				colvarShipPostalCode.IsReadOnly = false;
				schema.Columns.Add(colvarShipPostalCode);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipRegion = new TableSchema.TableColumn(schema);
				colvarShipRegion.ColumnName = "ShipRegion";
				colvarShipRegion.DataType = DbType.String;
				colvarShipRegion.MaxLength = 15;
				colvarShipRegion.AutoIncrement = true;
				colvarShipRegion.IsNullable = true;
				colvarShipRegion.IsPrimaryKey = false;
				colvarShipRegion.IsForeignKey = false;
				colvarShipRegion.IsReadOnly = false;
				schema.Columns.Add(colvarShipRegion);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipVia = new TableSchema.TableColumn(schema);
				colvarShipVia.ColumnName = "ShipVia";
				colvarShipVia.DataType = DbType.Int32;
				colvarShipVia.MaxLength = 0;
				colvarShipVia.AutoIncrement = true;
				colvarShipVia.IsNullable = true;
				colvarShipVia.IsPrimaryKey = false;
				colvarShipVia.IsForeignKey = false;
				colvarShipVia.IsReadOnly = false;
				schema.Columns.Add(colvarShipVia);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Quarterly_Order class.
	/// </summary>
    [Serializable]
	public partial class Quarterly_OrderCollection : ActiveList<Quarterly_Order, Quarterly_OrderCollection>
	{	   
		public Quarterly_OrderCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Quarterly_Order table.
	/// </summary>
	[Serializable]
    public class Quarterly_Order : ActiveRecord<Quarterly_Order>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Quarterly Orders", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCity = new TableSchema.TableColumn(schema);
				colvarCity.ColumnName = "City";
				colvarCity.DataType = DbType.String;
				colvarCity.MaxLength = 15;
				colvarCity.AutoIncrement = true;
				colvarCity.IsNullable = true;
				colvarCity.IsPrimaryKey = false;
				colvarCity.IsForeignKey = false;
				colvarCity.IsReadOnly = false;
				schema.Columns.Add(colvarCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCompanyName = new TableSchema.TableColumn(schema);
				colvarCompanyName.ColumnName = "CompanyName";
				colvarCompanyName.DataType = DbType.String;
				colvarCompanyName.MaxLength = 40;
				colvarCompanyName.AutoIncrement = true;
				colvarCompanyName.IsNullable = true;
				colvarCompanyName.IsPrimaryKey = false;
				colvarCompanyName.IsForeignKey = false;
				colvarCompanyName.IsReadOnly = false;
				schema.Columns.Add(colvarCompanyName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCountry = new TableSchema.TableColumn(schema);
				colvarCountry.ColumnName = "Country";
				colvarCountry.DataType = DbType.String;
				colvarCountry.MaxLength = 15;
				colvarCountry.AutoIncrement = true;
				colvarCountry.IsNullable = true;
				colvarCountry.IsPrimaryKey = false;
				colvarCountry.IsForeignKey = false;
				colvarCountry.IsReadOnly = false;
				schema.Columns.Add(colvarCountry);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
				colvarCustomerID.ColumnName = "CustomerID";
				colvarCustomerID.DataType = DbType.String;
				colvarCustomerID.MaxLength = 5;
				colvarCustomerID.AutoIncrement = true;
				colvarCustomerID.IsNullable = true;
				colvarCustomerID.IsPrimaryKey = false;
				colvarCustomerID.IsForeignKey = false;
				colvarCustomerID.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerID);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Invoice class.
	/// </summary>
    [Serializable]
	public partial class InvoiceCollection : ActiveList<Invoice, InvoiceCollection>
	{	   
		public InvoiceCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Invoice table.
	/// </summary>
	[Serializable]
    public class Invoice : ActiveRecord<Invoice>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Invoices", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarAddress = new TableSchema.TableColumn(schema);
				colvarAddress.ColumnName = "Address";
				colvarAddress.DataType = DbType.String;
				colvarAddress.MaxLength = 60;
				colvarAddress.AutoIncrement = true;
				colvarAddress.IsNullable = true;
				colvarAddress.IsPrimaryKey = false;
				colvarAddress.IsForeignKey = false;
				colvarAddress.IsReadOnly = false;
				schema.Columns.Add(colvarAddress);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCity = new TableSchema.TableColumn(schema);
				colvarCity.ColumnName = "City";
				colvarCity.DataType = DbType.String;
				colvarCity.MaxLength = 15;
				colvarCity.AutoIncrement = true;
				colvarCity.IsNullable = true;
				colvarCity.IsPrimaryKey = false;
				colvarCity.IsForeignKey = false;
				colvarCity.IsReadOnly = false;
				schema.Columns.Add(colvarCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCountry = new TableSchema.TableColumn(schema);
				colvarCountry.ColumnName = "Country";
				colvarCountry.DataType = DbType.String;
				colvarCountry.MaxLength = 15;
				colvarCountry.AutoIncrement = true;
				colvarCountry.IsNullable = true;
				colvarCountry.IsPrimaryKey = false;
				colvarCountry.IsForeignKey = false;
				colvarCountry.IsReadOnly = false;
				schema.Columns.Add(colvarCountry);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
				colvarCustomerID.ColumnName = "CustomerID";
				colvarCustomerID.DataType = DbType.String;
				colvarCustomerID.MaxLength = 5;
				colvarCustomerID.AutoIncrement = true;
				colvarCustomerID.IsNullable = true;
				colvarCustomerID.IsPrimaryKey = false;
				colvarCustomerID.IsForeignKey = false;
				colvarCustomerID.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCustomerName = new TableSchema.TableColumn(schema);
				colvarCustomerName.ColumnName = "CustomerName";
				colvarCustomerName.DataType = DbType.String;
				colvarCustomerName.MaxLength = 40;
				colvarCustomerName.AutoIncrement = true;
				colvarCustomerName.IsNullable = false;
				colvarCustomerName.IsPrimaryKey = false;
				colvarCustomerName.IsForeignKey = false;
				colvarCustomerName.IsReadOnly = false;
				schema.Columns.Add(colvarCustomerName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarDiscount = new TableSchema.TableColumn(schema);
				colvarDiscount.ColumnName = "Discount";
				colvarDiscount.DataType = DbType.Single;
				colvarDiscount.MaxLength = 0;
				colvarDiscount.AutoIncrement = true;
				colvarDiscount.IsNullable = false;
				colvarDiscount.IsPrimaryKey = false;
				colvarDiscount.IsForeignKey = false;
				colvarDiscount.IsReadOnly = false;
				schema.Columns.Add(colvarDiscount);

				
				
				
				
				
				
				TableSchema.TableColumn colvarExtendedPrice = new TableSchema.TableColumn(schema);
				colvarExtendedPrice.ColumnName = "ExtendedPrice";
				colvarExtendedPrice.DataType = DbType.Currency;
				colvarExtendedPrice.MaxLength = 0;
				colvarExtendedPrice.AutoIncrement = true;
				colvarExtendedPrice.IsNullable = true;
				colvarExtendedPrice.IsPrimaryKey = false;
				colvarExtendedPrice.IsForeignKey = false;
				colvarExtendedPrice.IsReadOnly = false;
				schema.Columns.Add(colvarExtendedPrice);

				
				
				
				
				
				
				TableSchema.TableColumn colvarFreight = new TableSchema.TableColumn(schema);
				colvarFreight.ColumnName = "Freight";
				colvarFreight.DataType = DbType.Currency;
				colvarFreight.MaxLength = 0;
				colvarFreight.AutoIncrement = true;
				colvarFreight.IsNullable = true;
				colvarFreight.IsPrimaryKey = false;
				colvarFreight.IsForeignKey = false;
				colvarFreight.IsReadOnly = false;
				schema.Columns.Add(colvarFreight);

				
				
				
				
				
				
				TableSchema.TableColumn colvarOrderDate = new TableSchema.TableColumn(schema);
				colvarOrderDate.ColumnName = "OrderDate";
				colvarOrderDate.DataType = DbType.DateTime;
				colvarOrderDate.MaxLength = 0;
				colvarOrderDate.AutoIncrement = true;
				colvarOrderDate.IsNullable = true;
				colvarOrderDate.IsPrimaryKey = false;
				colvarOrderDate.IsForeignKey = false;
				colvarOrderDate.IsReadOnly = false;
				schema.Columns.Add(colvarOrderDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = false;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				schema.Columns.Add(colvarOrderID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarPostalCode = new TableSchema.TableColumn(schema);
				colvarPostalCode.ColumnName = "PostalCode";
				colvarPostalCode.DataType = DbType.String;
				colvarPostalCode.MaxLength = 10;
				colvarPostalCode.AutoIncrement = true;
				colvarPostalCode.IsNullable = true;
				colvarPostalCode.IsPrimaryKey = false;
				colvarPostalCode.IsForeignKey = false;
				colvarPostalCode.IsReadOnly = false;
				schema.Columns.Add(colvarPostalCode);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductID = new TableSchema.TableColumn(schema);
				colvarProductID.ColumnName = "ProductID";
				colvarProductID.DataType = DbType.Int32;
				colvarProductID.MaxLength = 0;
				colvarProductID.AutoIncrement = true;
				colvarProductID.IsNullable = false;
				colvarProductID.IsPrimaryKey = false;
				colvarProductID.IsForeignKey = false;
				colvarProductID.IsReadOnly = false;
				schema.Columns.Add(colvarProductID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = true;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				schema.Columns.Add(colvarProductName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarQuantity = new TableSchema.TableColumn(schema);
				colvarQuantity.ColumnName = "Quantity";
				colvarQuantity.DataType = DbType.Int16;
				colvarQuantity.MaxLength = 0;
				colvarQuantity.AutoIncrement = true;
				colvarQuantity.IsNullable = false;
				colvarQuantity.IsPrimaryKey = false;
				colvarQuantity.IsForeignKey = false;
				colvarQuantity.IsReadOnly = false;
				schema.Columns.Add(colvarQuantity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRegion = new TableSchema.TableColumn(schema);
				colvarRegion.ColumnName = "Region";
				colvarRegion.DataType = DbType.String;
				colvarRegion.MaxLength = 15;
				colvarRegion.AutoIncrement = true;
				colvarRegion.IsNullable = true;
				colvarRegion.IsPrimaryKey = false;
				colvarRegion.IsForeignKey = false;
				colvarRegion.IsReadOnly = false;
				schema.Columns.Add(colvarRegion);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRequiredDate = new TableSchema.TableColumn(schema);
				colvarRequiredDate.ColumnName = "RequiredDate";
				colvarRequiredDate.DataType = DbType.DateTime;
				colvarRequiredDate.MaxLength = 0;
				colvarRequiredDate.AutoIncrement = true;
				colvarRequiredDate.IsNullable = true;
				colvarRequiredDate.IsPrimaryKey = false;
				colvarRequiredDate.IsForeignKey = false;
				colvarRequiredDate.IsReadOnly = false;
				schema.Columns.Add(colvarRequiredDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarSalesperson = new TableSchema.TableColumn(schema);
				colvarSalesperson.ColumnName = "Salesperson";
				colvarSalesperson.DataType = DbType.String;
				colvarSalesperson.MaxLength = 31;
				colvarSalesperson.AutoIncrement = true;
				colvarSalesperson.IsNullable = false;
				colvarSalesperson.IsPrimaryKey = false;
				colvarSalesperson.IsForeignKey = false;
				colvarSalesperson.IsReadOnly = false;
				schema.Columns.Add(colvarSalesperson);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipAddress = new TableSchema.TableColumn(schema);
				colvarShipAddress.ColumnName = "ShipAddress";
				colvarShipAddress.DataType = DbType.String;
				colvarShipAddress.MaxLength = 60;
				colvarShipAddress.AutoIncrement = true;
				colvarShipAddress.IsNullable = true;
				colvarShipAddress.IsPrimaryKey = false;
				colvarShipAddress.IsForeignKey = false;
				colvarShipAddress.IsReadOnly = false;
				schema.Columns.Add(colvarShipAddress);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipCity = new TableSchema.TableColumn(schema);
				colvarShipCity.ColumnName = "ShipCity";
				colvarShipCity.DataType = DbType.String;
				colvarShipCity.MaxLength = 15;
				colvarShipCity.AutoIncrement = true;
				colvarShipCity.IsNullable = true;
				colvarShipCity.IsPrimaryKey = false;
				colvarShipCity.IsForeignKey = false;
				colvarShipCity.IsReadOnly = false;
				schema.Columns.Add(colvarShipCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipCountry = new TableSchema.TableColumn(schema);
				colvarShipCountry.ColumnName = "ShipCountry";
				colvarShipCountry.DataType = DbType.String;
				colvarShipCountry.MaxLength = 15;
				colvarShipCountry.AutoIncrement = true;
				colvarShipCountry.IsNullable = true;
				colvarShipCountry.IsPrimaryKey = false;
				colvarShipCountry.IsForeignKey = false;
				colvarShipCountry.IsReadOnly = false;
				schema.Columns.Add(colvarShipCountry);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipName = new TableSchema.TableColumn(schema);
				colvarShipName.ColumnName = "ShipName";
				colvarShipName.DataType = DbType.String;
				colvarShipName.MaxLength = 40;
				colvarShipName.AutoIncrement = true;
				colvarShipName.IsNullable = true;
				colvarShipName.IsPrimaryKey = false;
				colvarShipName.IsForeignKey = false;
				colvarShipName.IsReadOnly = false;
				schema.Columns.Add(colvarShipName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShippedDate = new TableSchema.TableColumn(schema);
				colvarShippedDate.ColumnName = "ShippedDate";
				colvarShippedDate.DataType = DbType.DateTime;
				colvarShippedDate.MaxLength = 0;
				colvarShippedDate.AutoIncrement = true;
				colvarShippedDate.IsNullable = true;
				colvarShippedDate.IsPrimaryKey = false;
				colvarShippedDate.IsForeignKey = false;
				colvarShippedDate.IsReadOnly = false;
				schema.Columns.Add(colvarShippedDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipperName = new TableSchema.TableColumn(schema);
				colvarShipperName.ColumnName = "ShipperName";
				colvarShipperName.DataType = DbType.String;
				colvarShipperName.MaxLength = 40;
				colvarShipperName.AutoIncrement = true;
				colvarShipperName.IsNullable = false;
				colvarShipperName.IsPrimaryKey = false;
				colvarShipperName.IsForeignKey = false;
				colvarShipperName.IsReadOnly = false;
				schema.Columns.Add(colvarShipperName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipPostalCode = new TableSchema.TableColumn(schema);
				colvarShipPostalCode.ColumnName = "ShipPostalCode";
				colvarShipPostalCode.DataType = DbType.String;
				colvarShipPostalCode.MaxLength = 10;
				colvarShipPostalCode.AutoIncrement = true;
				colvarShipPostalCode.IsNullable = true;
				colvarShipPostalCode.IsPrimaryKey = false;
				colvarShipPostalCode.IsForeignKey = false;
				colvarShipPostalCode.IsReadOnly = false;
				schema.Columns.Add(colvarShipPostalCode);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShipRegion = new TableSchema.TableColumn(schema);
				colvarShipRegion.ColumnName = "ShipRegion";
				colvarShipRegion.DataType = DbType.String;
				colvarShipRegion.MaxLength = 15;
				colvarShipRegion.AutoIncrement = true;
				colvarShipRegion.IsNullable = true;
				colvarShipRegion.IsPrimaryKey = false;
				colvarShipRegion.IsForeignKey = false;
				colvarShipRegion.IsReadOnly = false;
				schema.Columns.Add(colvarShipRegion);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
				colvarUnitPrice.ColumnName = "UnitPrice";
				colvarUnitPrice.DataType = DbType.Currency;
				colvarUnitPrice.MaxLength = 0;
				colvarUnitPrice.AutoIncrement = true;
				colvarUnitPrice.IsNullable = false;
				colvarUnitPrice.IsPrimaryKey = false;
				colvarUnitPrice.IsForeignKey = false;
				colvarUnitPrice.IsReadOnly = false;
				schema.Columns.Add(colvarUnitPrice);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Product_Sales_for_1997 class.
	/// </summary>
    [Serializable]
	public partial class Product_Sales_for_1997Collection : ActiveList<Product_Sales_for_1997, Product_Sales_for_1997Collection>
	{	   
		public Product_Sales_for_1997Collection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Product_Sales_for_1997 table.
	/// </summary>
	[Serializable]
    public class Product_Sales_for_1997 : ActiveRecord<Product_Sales_for_1997>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Product Sales for 1997", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCategoryName = new TableSchema.TableColumn(schema);
				colvarCategoryName.ColumnName = "CategoryName";
				colvarCategoryName.DataType = DbType.String;
				colvarCategoryName.MaxLength = 15;
				colvarCategoryName.AutoIncrement = true;
				colvarCategoryName.IsNullable = false;
				colvarCategoryName.IsPrimaryKey = false;
				colvarCategoryName.IsForeignKey = false;
				colvarCategoryName.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = true;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				schema.Columns.Add(colvarProductName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductSales = new TableSchema.TableColumn(schema);
				colvarProductSales.ColumnName = "ProductSales";
				colvarProductSales.DataType = DbType.Currency;
				colvarProductSales.MaxLength = 0;
				colvarProductSales.AutoIncrement = true;
				colvarProductSales.IsNullable = true;
				colvarProductSales.IsPrimaryKey = false;
				colvarProductSales.IsForeignKey = false;
				colvarProductSales.IsReadOnly = false;
				schema.Columns.Add(colvarProductSales);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Order_Details_Extended class.
	/// </summary>
    [Serializable]
	public partial class Order_Details_ExtendedCollection : ActiveList<Order_Details_Extended, Order_Details_ExtendedCollection>
	{	   
		public Order_Details_ExtendedCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Order_Details_Extended table.
	/// </summary>
	[Serializable]
    public class Order_Details_Extended : ActiveRecord<Order_Details_Extended>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Order Details Extended", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarDiscount = new TableSchema.TableColumn(schema);
				colvarDiscount.ColumnName = "Discount";
				colvarDiscount.DataType = DbType.Single;
				colvarDiscount.MaxLength = 0;
				colvarDiscount.AutoIncrement = true;
				colvarDiscount.IsNullable = false;
				colvarDiscount.IsPrimaryKey = false;
				colvarDiscount.IsForeignKey = false;
				colvarDiscount.IsReadOnly = false;
				schema.Columns.Add(colvarDiscount);

				
				
				
				
				
				
				TableSchema.TableColumn colvarExtendedPrice = new TableSchema.TableColumn(schema);
				colvarExtendedPrice.ColumnName = "ExtendedPrice";
				colvarExtendedPrice.DataType = DbType.Currency;
				colvarExtendedPrice.MaxLength = 0;
				colvarExtendedPrice.AutoIncrement = true;
				colvarExtendedPrice.IsNullable = true;
				colvarExtendedPrice.IsPrimaryKey = false;
				colvarExtendedPrice.IsForeignKey = false;
				colvarExtendedPrice.IsReadOnly = false;
				schema.Columns.Add(colvarExtendedPrice);

				
				
				
				
				
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = false;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				schema.Columns.Add(colvarOrderID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductID = new TableSchema.TableColumn(schema);
				colvarProductID.ColumnName = "ProductID";
				colvarProductID.DataType = DbType.Int32;
				colvarProductID.MaxLength = 0;
				colvarProductID.AutoIncrement = true;
				colvarProductID.IsNullable = false;
				colvarProductID.IsPrimaryKey = false;
				colvarProductID.IsForeignKey = false;
				colvarProductID.IsReadOnly = false;
				schema.Columns.Add(colvarProductID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = true;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				schema.Columns.Add(colvarProductName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarQuantity = new TableSchema.TableColumn(schema);
				colvarQuantity.ColumnName = "Quantity";
				colvarQuantity.DataType = DbType.Int16;
				colvarQuantity.MaxLength = 0;
				colvarQuantity.AutoIncrement = true;
				colvarQuantity.IsNullable = false;
				colvarQuantity.IsPrimaryKey = false;
				colvarQuantity.IsForeignKey = false;
				colvarQuantity.IsReadOnly = false;
				schema.Columns.Add(colvarQuantity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
				colvarUnitPrice.ColumnName = "UnitPrice";
				colvarUnitPrice.DataType = DbType.Currency;
				colvarUnitPrice.MaxLength = 0;
				colvarUnitPrice.AutoIncrement = true;
				colvarUnitPrice.IsNullable = false;
				colvarUnitPrice.IsPrimaryKey = false;
				colvarUnitPrice.IsForeignKey = false;
				colvarUnitPrice.IsReadOnly = false;
				schema.Columns.Add(colvarUnitPrice);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Products_Above_Average_Price class.
	/// </summary>
    [Serializable]
	public partial class Products_Above_Average_PriceCollection : ActiveList<Products_Above_Average_Price, Products_Above_Average_PriceCollection>
	{	   
		public Products_Above_Average_PriceCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Products_Above_Average_Price table.
	/// </summary>
	[Serializable]
    public class Products_Above_Average_Price : ActiveRecord<Products_Above_Average_Price>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Products Above Average Price", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = true;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				schema.Columns.Add(colvarProductName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
				colvarUnitPrice.ColumnName = "UnitPrice";
				colvarUnitPrice.DataType = DbType.Currency;
				colvarUnitPrice.MaxLength = 0;
				colvarUnitPrice.AutoIncrement = true;
				colvarUnitPrice.IsNullable = true;
				colvarUnitPrice.IsPrimaryKey = false;
				colvarUnitPrice.IsForeignKey = false;
				colvarUnitPrice.IsReadOnly = false;
				schema.Columns.Add(colvarUnitPrice);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Products_by_Category class.
	/// </summary>
    [Serializable]
	public partial class Products_by_CategoryCollection : ActiveList<Products_by_Category, Products_by_CategoryCollection>
	{	   
		public Products_by_CategoryCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Products_by_Category table.
	/// </summary>
	[Serializable]
    public class Products_by_Category : ActiveRecord<Products_by_Category>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Products by Category", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCategoryName = new TableSchema.TableColumn(schema);
				colvarCategoryName.ColumnName = "CategoryName";
				colvarCategoryName.DataType = DbType.String;
				colvarCategoryName.MaxLength = 15;
				colvarCategoryName.AutoIncrement = true;
				colvarCategoryName.IsNullable = false;
				colvarCategoryName.IsPrimaryKey = false;
				colvarCategoryName.IsForeignKey = false;
				colvarCategoryName.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarDiscontinued = new TableSchema.TableColumn(schema);
				colvarDiscontinued.ColumnName = "Discontinued";
				colvarDiscontinued.DataType = DbType.Boolean;
				colvarDiscontinued.MaxLength = 0;
				colvarDiscontinued.AutoIncrement = true;
				colvarDiscontinued.IsNullable = false;
				colvarDiscontinued.IsPrimaryKey = false;
				colvarDiscontinued.IsForeignKey = false;
				colvarDiscontinued.IsReadOnly = false;
				schema.Columns.Add(colvarDiscontinued);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = true;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				schema.Columns.Add(colvarProductName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarQuantityPerUnit = new TableSchema.TableColumn(schema);
				colvarQuantityPerUnit.ColumnName = "QuantityPerUnit";
				colvarQuantityPerUnit.DataType = DbType.String;
				colvarQuantityPerUnit.MaxLength = 20;
				colvarQuantityPerUnit.AutoIncrement = true;
				colvarQuantityPerUnit.IsNullable = true;
				colvarQuantityPerUnit.IsPrimaryKey = false;
				colvarQuantityPerUnit.IsForeignKey = false;
				colvarQuantityPerUnit.IsReadOnly = false;
				schema.Columns.Add(colvarQuantityPerUnit);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitsInStock = new TableSchema.TableColumn(schema);
				colvarUnitsInStock.ColumnName = "UnitsInStock";
				colvarUnitsInStock.DataType = DbType.Int16;
				colvarUnitsInStock.MaxLength = 0;
				colvarUnitsInStock.AutoIncrement = true;
				colvarUnitsInStock.IsNullable = true;
				colvarUnitsInStock.IsPrimaryKey = false;
				colvarUnitsInStock.IsForeignKey = false;
				colvarUnitsInStock.IsReadOnly = false;
				schema.Columns.Add(colvarUnitsInStock);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Alphabetical_list_of_product class.
	/// </summary>
    [Serializable]
	public partial class Alphabetical_list_of_productCollection : ActiveList<Alphabetical_list_of_product, Alphabetical_list_of_productCollection>
	{	   
		public Alphabetical_list_of_productCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Alphabetical_list_of_product table.
	/// </summary>
	[Serializable]
    public class Alphabetical_list_of_product : ActiveRecord<Alphabetical_list_of_product>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Alphabetical list of products", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarAttributeXML = new TableSchema.TableColumn(schema);
				colvarAttributeXML.ColumnName = "AttributeXML";
				colvarAttributeXML.DataType = DbType.AnsiString;
				colvarAttributeXML.MaxLength = -1;
				colvarAttributeXML.AutoIncrement = true;
				colvarAttributeXML.IsNullable = true;
				colvarAttributeXML.IsPrimaryKey = false;
				colvarAttributeXML.IsForeignKey = false;
				colvarAttributeXML.IsReadOnly = false;
				schema.Columns.Add(colvarAttributeXML);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCategoryID = new TableSchema.TableColumn(schema);
				colvarCategoryID.ColumnName = "CategoryID";
				colvarCategoryID.DataType = DbType.Int32;
				colvarCategoryID.MaxLength = 0;
				colvarCategoryID.AutoIncrement = true;
				colvarCategoryID.IsNullable = true;
				colvarCategoryID.IsPrimaryKey = false;
				colvarCategoryID.IsForeignKey = false;
				colvarCategoryID.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCategoryName = new TableSchema.TableColumn(schema);
				colvarCategoryName.ColumnName = "CategoryName";
				colvarCategoryName.DataType = DbType.String;
				colvarCategoryName.MaxLength = 15;
				colvarCategoryName.AutoIncrement = true;
				colvarCategoryName.IsNullable = false;
				colvarCategoryName.IsPrimaryKey = false;
				colvarCategoryName.IsForeignKey = false;
				colvarCategoryName.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCreatedBy = new TableSchema.TableColumn(schema);
				colvarCreatedBy.ColumnName = "CreatedBy";
				colvarCreatedBy.DataType = DbType.String;
				colvarCreatedBy.MaxLength = 50;
				colvarCreatedBy.AutoIncrement = true;
				colvarCreatedBy.IsNullable = true;
				colvarCreatedBy.IsPrimaryKey = false;
				colvarCreatedBy.IsForeignKey = false;
				colvarCreatedBy.IsReadOnly = false;
				schema.Columns.Add(colvarCreatedBy);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCreatedOn = new TableSchema.TableColumn(schema);
				colvarCreatedOn.ColumnName = "CreatedOn";
				colvarCreatedOn.DataType = DbType.DateTime;
				colvarCreatedOn.MaxLength = 0;
				colvarCreatedOn.AutoIncrement = true;
				colvarCreatedOn.IsNullable = false;
				colvarCreatedOn.IsPrimaryKey = false;
				colvarCreatedOn.IsForeignKey = false;
				colvarCreatedOn.IsReadOnly = false;
				schema.Columns.Add(colvarCreatedOn);

				
				
				
				
				
				
				TableSchema.TableColumn colvarDateCreated = new TableSchema.TableColumn(schema);
				colvarDateCreated.ColumnName = "DateCreated";
				colvarDateCreated.DataType = DbType.DateTime;
				colvarDateCreated.MaxLength = 0;
				colvarDateCreated.AutoIncrement = true;
				colvarDateCreated.IsNullable = true;
				colvarDateCreated.IsPrimaryKey = false;
				colvarDateCreated.IsForeignKey = false;
				colvarDateCreated.IsReadOnly = false;
				schema.Columns.Add(colvarDateCreated);

				
				
				
				
				
				
				TableSchema.TableColumn colvarDeleted = new TableSchema.TableColumn(schema);
				colvarDeleted.ColumnName = "Deleted";
				colvarDeleted.DataType = DbType.Boolean;
				colvarDeleted.MaxLength = 0;
				colvarDeleted.AutoIncrement = true;
				colvarDeleted.IsNullable = false;
				colvarDeleted.IsPrimaryKey = false;
				colvarDeleted.IsForeignKey = false;
				colvarDeleted.IsReadOnly = false;
				schema.Columns.Add(colvarDeleted);

				
				
				
				
				
				
				TableSchema.TableColumn colvarDiscontinued = new TableSchema.TableColumn(schema);
				colvarDiscontinued.ColumnName = "Discontinued";
				colvarDiscontinued.DataType = DbType.Boolean;
				colvarDiscontinued.MaxLength = 0;
				colvarDiscontinued.AutoIncrement = true;
				colvarDiscontinued.IsNullable = false;
				colvarDiscontinued.IsPrimaryKey = false;
				colvarDiscontinued.IsForeignKey = false;
				colvarDiscontinued.IsReadOnly = false;
				schema.Columns.Add(colvarDiscontinued);

				
				
				
				
				
				
				TableSchema.TableColumn colvarModifiedBy = new TableSchema.TableColumn(schema);
				colvarModifiedBy.ColumnName = "ModifiedBy";
				colvarModifiedBy.DataType = DbType.String;
				colvarModifiedBy.MaxLength = 50;
				colvarModifiedBy.AutoIncrement = true;
				colvarModifiedBy.IsNullable = true;
				colvarModifiedBy.IsPrimaryKey = false;
				colvarModifiedBy.IsForeignKey = false;
				colvarModifiedBy.IsReadOnly = false;
				schema.Columns.Add(colvarModifiedBy);

				
				
				
				
				
				
				TableSchema.TableColumn colvarModifiedOn = new TableSchema.TableColumn(schema);
				colvarModifiedOn.ColumnName = "ModifiedOn";
				colvarModifiedOn.DataType = DbType.DateTime;
				colvarModifiedOn.MaxLength = 0;
				colvarModifiedOn.AutoIncrement = true;
				colvarModifiedOn.IsNullable = false;
				colvarModifiedOn.IsPrimaryKey = false;
				colvarModifiedOn.IsForeignKey = false;
				colvarModifiedOn.IsReadOnly = false;
				schema.Columns.Add(colvarModifiedOn);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductGUID = new TableSchema.TableColumn(schema);
				colvarProductGUID.ColumnName = "ProductGUID";
				colvarProductGUID.DataType = DbType.Guid;
				colvarProductGUID.MaxLength = 0;
				colvarProductGUID.AutoIncrement = true;
				colvarProductGUID.IsNullable = true;
				colvarProductGUID.IsPrimaryKey = false;
				colvarProductGUID.IsForeignKey = false;
				colvarProductGUID.IsReadOnly = false;
				schema.Columns.Add(colvarProductGUID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductID = new TableSchema.TableColumn(schema);
				colvarProductID.ColumnName = "ProductID";
				colvarProductID.DataType = DbType.Int32;
				colvarProductID.MaxLength = 0;
				colvarProductID.AutoIncrement = true;
				colvarProductID.IsNullable = false;
				colvarProductID.IsPrimaryKey = false;
				colvarProductID.IsForeignKey = false;
				colvarProductID.IsReadOnly = false;
				schema.Columns.Add(colvarProductID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = true;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				schema.Columns.Add(colvarProductName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarQuantityPerUnit = new TableSchema.TableColumn(schema);
				colvarQuantityPerUnit.ColumnName = "QuantityPerUnit";
				colvarQuantityPerUnit.DataType = DbType.String;
				colvarQuantityPerUnit.MaxLength = 20;
				colvarQuantityPerUnit.AutoIncrement = true;
				colvarQuantityPerUnit.IsNullable = true;
				colvarQuantityPerUnit.IsPrimaryKey = false;
				colvarQuantityPerUnit.IsForeignKey = false;
				colvarQuantityPerUnit.IsReadOnly = false;
				schema.Columns.Add(colvarQuantityPerUnit);

				
				
				
				
				
				
				TableSchema.TableColumn colvarReorderLevel = new TableSchema.TableColumn(schema);
				colvarReorderLevel.ColumnName = "ReorderLevel";
				colvarReorderLevel.DataType = DbType.Int16;
				colvarReorderLevel.MaxLength = 0;
				colvarReorderLevel.AutoIncrement = true;
				colvarReorderLevel.IsNullable = true;
				colvarReorderLevel.IsPrimaryKey = false;
				colvarReorderLevel.IsForeignKey = false;
				colvarReorderLevel.IsReadOnly = false;
				schema.Columns.Add(colvarReorderLevel);

				
				
				
				
				
				
				TableSchema.TableColumn colvarSupplierID = new TableSchema.TableColumn(schema);
				colvarSupplierID.ColumnName = "SupplierID";
				colvarSupplierID.DataType = DbType.Int32;
				colvarSupplierID.MaxLength = 0;
				colvarSupplierID.AutoIncrement = true;
				colvarSupplierID.IsNullable = true;
				colvarSupplierID.IsPrimaryKey = false;
				colvarSupplierID.IsForeignKey = false;
				colvarSupplierID.IsReadOnly = false;
				schema.Columns.Add(colvarSupplierID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
				colvarUnitPrice.ColumnName = "UnitPrice";
				colvarUnitPrice.DataType = DbType.Currency;
				colvarUnitPrice.MaxLength = 0;
				colvarUnitPrice.AutoIncrement = true;
				colvarUnitPrice.IsNullable = true;
				colvarUnitPrice.IsPrimaryKey = false;
				colvarUnitPrice.IsForeignKey = false;
				colvarUnitPrice.IsReadOnly = false;
				schema.Columns.Add(colvarUnitPrice);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitsInStock = new TableSchema.TableColumn(schema);
				colvarUnitsInStock.ColumnName = "UnitsInStock";
				colvarUnitsInStock.DataType = DbType.Int16;
				colvarUnitsInStock.MaxLength = 0;
				colvarUnitsInStock.AutoIncrement = true;
				colvarUnitsInStock.IsNullable = true;
				colvarUnitsInStock.IsPrimaryKey = false;
				colvarUnitsInStock.IsForeignKey = false;
				colvarUnitsInStock.IsReadOnly = false;
				schema.Columns.Add(colvarUnitsInStock);

				
				
				
				
				
				
				TableSchema.TableColumn colvarUnitsOnOrder = new TableSchema.TableColumn(schema);
				colvarUnitsOnOrder.ColumnName = "UnitsOnOrder";
				colvarUnitsOnOrder.DataType = DbType.Int16;
				colvarUnitsOnOrder.MaxLength = 0;
				colvarUnitsOnOrder.AutoIncrement = true;
				colvarUnitsOnOrder.IsNullable = true;
				colvarUnitsOnOrder.IsPrimaryKey = false;
				colvarUnitsOnOrder.IsForeignKey = false;
				colvarUnitsOnOrder.IsReadOnly = false;
				schema.Columns.Add(colvarUnitsOnOrder);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Current_Product_List class.
	/// </summary>
    [Serializable]
	public partial class Current_Product_ListCollection : ActiveList<Current_Product_List, Current_Product_ListCollection>
	{	   
		public Current_Product_ListCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Current_Product_List table.
	/// </summary>
	[Serializable]
    public class Current_Product_List : ActiveRecord<Current_Product_List>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Current Product List", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarProductID = new TableSchema.TableColumn(schema);
				colvarProductID.ColumnName = "ProductID";
				colvarProductID.DataType = DbType.Int32;
				colvarProductID.MaxLength = 0;
				colvarProductID.AutoIncrement = true;
				colvarProductID.IsNullable = false;
				colvarProductID.IsPrimaryKey = false;
				colvarProductID.IsForeignKey = false;
				colvarProductID.IsReadOnly = false;
				schema.Columns.Add(colvarProductID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = true;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				schema.Columns.Add(colvarProductName);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Order_Subtotal class.
	/// </summary>
    [Serializable]
	public partial class Order_SubtotalCollection : ActiveList<Order_Subtotal, Order_SubtotalCollection>
	{	   
		public Order_SubtotalCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Order_Subtotal table.
	/// </summary>
	[Serializable]
    public class Order_Subtotal : ActiveRecord<Order_Subtotal>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Order Subtotals", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = false;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				schema.Columns.Add(colvarOrderID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarSubtotal = new TableSchema.TableColumn(schema);
				colvarSubtotal.ColumnName = "Subtotal";
				colvarSubtotal.DataType = DbType.Currency;
				colvarSubtotal.MaxLength = 0;
				colvarSubtotal.AutoIncrement = true;
				colvarSubtotal.IsNullable = true;
				colvarSubtotal.IsPrimaryKey = false;
				colvarSubtotal.IsForeignKey = false;
				colvarSubtotal.IsReadOnly = false;
				schema.Columns.Add(colvarSubtotal);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Customer_and_Suppliers_by_City class.
	/// </summary>
    [Serializable]
	public partial class Customer_and_Suppliers_by_CityCollection : ActiveList<Customer_and_Suppliers_by_City, Customer_and_Suppliers_by_CityCollection>
	{	   
		public Customer_and_Suppliers_by_CityCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Customer_and_Suppliers_by_City table.
	/// </summary>
	[Serializable]
    public class Customer_and_Suppliers_by_City : ActiveRecord<Customer_and_Suppliers_by_City>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Customer and Suppliers by City", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCity = new TableSchema.TableColumn(schema);
				colvarCity.ColumnName = "City";
				colvarCity.DataType = DbType.String;
				colvarCity.MaxLength = 15;
				colvarCity.AutoIncrement = true;
				colvarCity.IsNullable = true;
				colvarCity.IsPrimaryKey = false;
				colvarCity.IsForeignKey = false;
				colvarCity.IsReadOnly = false;
				schema.Columns.Add(colvarCity);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCompanyName = new TableSchema.TableColumn(schema);
				colvarCompanyName.ColumnName = "CompanyName";
				colvarCompanyName.DataType = DbType.String;
				colvarCompanyName.MaxLength = 40;
				colvarCompanyName.AutoIncrement = true;
				colvarCompanyName.IsNullable = false;
				colvarCompanyName.IsPrimaryKey = false;
				colvarCompanyName.IsForeignKey = false;
				colvarCompanyName.IsReadOnly = false;
				schema.Columns.Add(colvarCompanyName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarContactName = new TableSchema.TableColumn(schema);
				colvarContactName.ColumnName = "ContactName";
				colvarContactName.DataType = DbType.String;
				colvarContactName.MaxLength = 30;
				colvarContactName.AutoIncrement = true;
				colvarContactName.IsNullable = true;
				colvarContactName.IsPrimaryKey = false;
				colvarContactName.IsForeignKey = false;
				colvarContactName.IsReadOnly = false;
				schema.Columns.Add(colvarContactName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarRelationship = new TableSchema.TableColumn(schema);
				colvarRelationship.ColumnName = "Relationship";
				colvarRelationship.DataType = DbType.AnsiString;
				colvarRelationship.MaxLength = 9;
				colvarRelationship.AutoIncrement = true;
				colvarRelationship.IsNullable = false;
				colvarRelationship.IsPrimaryKey = false;
				colvarRelationship.IsForeignKey = false;
				colvarRelationship.IsReadOnly = false;
				schema.Columns.Add(colvarRelationship);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Category_Sales_for_1997 class.
	/// </summary>
    [Serializable]
	public partial class Category_Sales_for_1997Collection : ActiveList<Category_Sales_for_1997, Category_Sales_for_1997Collection>
	{	   
		public Category_Sales_for_1997Collection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Category_Sales_for_1997 table.
	/// </summary>
	[Serializable]
    public class Category_Sales_for_1997 : ActiveRecord<Category_Sales_for_1997>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Category Sales for 1997", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCategoryName = new TableSchema.TableColumn(schema);
				colvarCategoryName.ColumnName = "CategoryName";
				colvarCategoryName.DataType = DbType.String;
				colvarCategoryName.MaxLength = 15;
				colvarCategoryName.AutoIncrement = true;
				colvarCategoryName.IsNullable = false;
				colvarCategoryName.IsPrimaryKey = false;
				colvarCategoryName.IsForeignKey = false;
				colvarCategoryName.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCategorySales = new TableSchema.TableColumn(schema);
				colvarCategorySales.ColumnName = "CategorySales";
				colvarCategorySales.DataType = DbType.Currency;
				colvarCategorySales.MaxLength = 0;
				colvarCategorySales.AutoIncrement = true;
				colvarCategorySales.IsNullable = true;
				colvarCategorySales.IsPrimaryKey = false;
				colvarCategorySales.IsForeignKey = false;
				colvarCategorySales.IsReadOnly = false;
				schema.Columns.Add(colvarCategorySales);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Sales_by_Category class.
	/// </summary>
    [Serializable]
	public partial class Sales_by_CategoryCollection : ActiveList<Sales_by_Category, Sales_by_CategoryCollection>
	{	   
		public Sales_by_CategoryCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Sales_by_Category table.
	/// </summary>
	[Serializable]
    public class Sales_by_Category : ActiveRecord<Sales_by_Category>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Sales by Category", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCategoryID = new TableSchema.TableColumn(schema);
				colvarCategoryID.ColumnName = "CategoryID";
				colvarCategoryID.DataType = DbType.Int32;
				colvarCategoryID.MaxLength = 0;
				colvarCategoryID.AutoIncrement = true;
				colvarCategoryID.IsNullable = false;
				colvarCategoryID.IsPrimaryKey = false;
				colvarCategoryID.IsForeignKey = false;
				colvarCategoryID.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarCategoryName = new TableSchema.TableColumn(schema);
				colvarCategoryName.ColumnName = "CategoryName";
				colvarCategoryName.DataType = DbType.String;
				colvarCategoryName.MaxLength = 15;
				colvarCategoryName.AutoIncrement = true;
				colvarCategoryName.IsNullable = false;
				colvarCategoryName.IsPrimaryKey = false;
				colvarCategoryName.IsForeignKey = false;
				colvarCategoryName.IsReadOnly = false;
				schema.Columns.Add(colvarCategoryName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
				colvarProductName.ColumnName = "ProductName";
				colvarProductName.DataType = DbType.String;
				colvarProductName.MaxLength = 40;
				colvarProductName.AutoIncrement = true;
				colvarProductName.IsNullable = false;
				colvarProductName.IsPrimaryKey = false;
				colvarProductName.IsForeignKey = false;
				colvarProductName.IsReadOnly = false;
				schema.Columns.Add(colvarProductName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarProductSales = new TableSchema.TableColumn(schema);
				colvarProductSales.ColumnName = "ProductSales";
				colvarProductSales.DataType = DbType.Currency;
				colvarProductSales.MaxLength = 0;
				colvarProductSales.AutoIncrement = true;
				colvarProductSales.IsNullable = true;
				colvarProductSales.IsPrimaryKey = false;
				colvarProductSales.IsForeignKey = false;
				colvarProductSales.IsReadOnly = false;
				schema.Columns.Add(colvarProductSales);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Summary_of_Sales_by_Quarter class.
	/// </summary>
    [Serializable]
	public partial class Summary_of_Sales_by_QuarterCollection : ActiveList<Summary_of_Sales_by_Quarter, Summary_of_Sales_by_QuarterCollection>
	{	   
		public Summary_of_Sales_by_QuarterCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Summary_of_Sales_by_Quarter table.
	/// </summary>
	[Serializable]
    public class Summary_of_Sales_by_Quarter : ActiveRecord<Summary_of_Sales_by_Quarter>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Summary of Sales by Quarter", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = false;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				schema.Columns.Add(colvarOrderID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShippedDate = new TableSchema.TableColumn(schema);
				colvarShippedDate.ColumnName = "ShippedDate";
				colvarShippedDate.DataType = DbType.DateTime;
				colvarShippedDate.MaxLength = 0;
				colvarShippedDate.AutoIncrement = true;
				colvarShippedDate.IsNullable = true;
				colvarShippedDate.IsPrimaryKey = false;
				colvarShippedDate.IsForeignKey = false;
				colvarShippedDate.IsReadOnly = false;
				schema.Columns.Add(colvarShippedDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarSubtotal = new TableSchema.TableColumn(schema);
				colvarSubtotal.ColumnName = "Subtotal";
				colvarSubtotal.DataType = DbType.Currency;
				colvarSubtotal.MaxLength = 0;
				colvarSubtotal.AutoIncrement = true;
				colvarSubtotal.IsNullable = true;
				colvarSubtotal.IsPrimaryKey = false;
				colvarSubtotal.IsForeignKey = false;
				colvarSubtotal.IsReadOnly = false;
				schema.Columns.Add(colvarSubtotal);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Summary_of_Sales_by_Year class.
	/// </summary>
    [Serializable]
	public partial class Summary_of_Sales_by_YearCollection : ActiveList<Summary_of_Sales_by_Year, Summary_of_Sales_by_YearCollection>
	{	   
		public Summary_of_Sales_by_YearCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Summary_of_Sales_by_Year table.
	/// </summary>
	[Serializable]
    public class Summary_of_Sales_by_Year : ActiveRecord<Summary_of_Sales_by_Year>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Summary of Sales by Year", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = false;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				schema.Columns.Add(colvarOrderID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShippedDate = new TableSchema.TableColumn(schema);
				colvarShippedDate.ColumnName = "ShippedDate";
				colvarShippedDate.DataType = DbType.DateTime;
				colvarShippedDate.MaxLength = 0;
				colvarShippedDate.AutoIncrement = true;
				colvarShippedDate.IsNullable = true;
				colvarShippedDate.IsPrimaryKey = false;
				colvarShippedDate.IsForeignKey = false;
				colvarShippedDate.IsReadOnly = false;
				schema.Columns.Add(colvarShippedDate);

				
				
				
				
				
				
				TableSchema.TableColumn colvarSubtotal = new TableSchema.TableColumn(schema);
				colvarSubtotal.ColumnName = "Subtotal";
				colvarSubtotal.DataType = DbType.Currency;
				colvarSubtotal.MaxLength = 0;
				colvarSubtotal.AutoIncrement = true;
				colvarSubtotal.IsNullable = true;
				colvarSubtotal.IsPrimaryKey = false;
				colvarSubtotal.IsForeignKey = false;
				colvarSubtotal.IsReadOnly = false;
				schema.Columns.Add(colvarSubtotal);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the Sales_Totals_by_Amount class.
	/// </summary>
    [Serializable]
	public partial class Sales_Totals_by_AmountCollection : ActiveList<Sales_Totals_by_Amount, Sales_Totals_by_AmountCollection>
	{	   
		public Sales_Totals_by_AmountCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the Sales_Totals_by_Amount table.
	/// </summary>
	[Serializable]
    public class Sales_Totals_by_Amount : ActiveRecord<Sales_Totals_by_Amount>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("Sales Totals by Amount", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarCompanyName = new TableSchema.TableColumn(schema);
				colvarCompanyName.ColumnName = "CompanyName";
				colvarCompanyName.DataType = DbType.String;
				colvarCompanyName.MaxLength = 40;
				colvarCompanyName.AutoIncrement = true;
				colvarCompanyName.IsNullable = false;
				colvarCompanyName.IsPrimaryKey = false;
				colvarCompanyName.IsForeignKey = false;
				colvarCompanyName.IsReadOnly = false;
				schema.Columns.Add(colvarCompanyName);

				
				
				
				
				
				
				TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
				colvarOrderID.ColumnName = "OrderID";
				colvarOrderID.DataType = DbType.Int32;
				colvarOrderID.MaxLength = 0;
				colvarOrderID.AutoIncrement = true;
				colvarOrderID.IsNullable = false;
				colvarOrderID.IsPrimaryKey = false;
				colvarOrderID.IsForeignKey = false;
				colvarOrderID.IsReadOnly = false;
				schema.Columns.Add(colvarOrderID);

				
				
				
				
				
				
				TableSchema.TableColumn colvarSaleAmount = new TableSchema.TableColumn(schema);
				colvarSaleAmount.ColumnName = "SaleAmount";
				colvarSaleAmount.DataType = DbType.Currency;
				colvarSaleAmount.MaxLength = 0;
				colvarSaleAmount.AutoIncrement = true;
				colvarSaleAmount.IsNullable = true;
				colvarSaleAmount.IsPrimaryKey = false;
				colvarSaleAmount.IsForeignKey = false;
				colvarSaleAmount.IsReadOnly = false;
				schema.Columns.Add(colvarSaleAmount);

				
				
				
				
				
				
				TableSchema.TableColumn colvarShippedDate = new TableSchema.TableColumn(schema);
				colvarShippedDate.ColumnName = "ShippedDate";
				colvarShippedDate.DataType = DbType.DateTime;
				colvarShippedDate.MaxLength = 0;
				colvarShippedDate.AutoIncrement = true;
				colvarShippedDate.IsNullable = true;
				colvarShippedDate.IsPrimaryKey = false;
				colvarShippedDate.IsForeignKey = false;
				colvarShippedDate.IsReadOnly = false;
				schema.Columns.Add(colvarShippedDate);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the sysdiagram class.
	/// </summary>
    [Serializable]
	public partial class sysdiagramCollection : ActiveList<sysdiagram, sysdiagramCollection>
	{	   
		public sysdiagramCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the sysdiagram table.
	/// </summary>
	[Serializable]
    public class sysdiagram : ActiveRecord<sysdiagram>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("sysdiagrams", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvardefinition = new TableSchema.TableColumn(schema);
				colvardefinition.ColumnName = "definition";
				colvardefinition.DataType = DbType.Binary;
				colvardefinition.MaxLength = -1;
				colvardefinition.AutoIncrement = true;
				colvardefinition.IsNullable = true;
				colvardefinition.IsPrimaryKey = false;
				colvardefinition.IsForeignKey = false;
				colvardefinition.IsReadOnly = false;
				schema.Columns.Add(colvardefinition);

				
				
				
				
				
				
				TableSchema.TableColumn colvardiagram_id = new TableSchema.TableColumn(schema);
				colvardiagram_id.ColumnName = "diagram_id";
				colvardiagram_id.DataType = DbType.Int32;
				colvardiagram_id.MaxLength = 0;
				colvardiagram_id.AutoIncrement = true;
				colvardiagram_id.IsNullable = false;
				colvardiagram_id.IsPrimaryKey = false;
				colvardiagram_id.IsForeignKey = false;
				colvardiagram_id.IsReadOnly = false;
				schema.Columns.Add(colvardiagram_id);

				
				
				
				
				
				
				TableSchema.TableColumn colvarname = new TableSchema.TableColumn(schema);
				colvarname.ColumnName = "name";
				colvarname.DataType = DbType.String;
				colvarname.MaxLength = 128;
				colvarname.AutoIncrement = true;
				colvarname.IsNullable = false;
				colvarname.IsPrimaryKey = false;
				colvarname.IsForeignKey = false;
				colvarname.IsReadOnly = false;
				schema.Columns.Add(colvarname);

				
				
				
				
				
				
				TableSchema.TableColumn colvarprincipal_id = new TableSchema.TableColumn(schema);
				colvarprincipal_id.ColumnName = "principal_id";
				colvarprincipal_id.DataType = DbType.Int32;
				colvarprincipal_id.MaxLength = 0;
				colvarprincipal_id.AutoIncrement = true;
				colvarprincipal_id.IsNullable = false;
				colvarprincipal_id.IsPrimaryKey = false;
				colvarprincipal_id.IsForeignKey = false;
				colvarprincipal_id.IsReadOnly = false;
				schema.Columns.Add(colvarprincipal_id);

				
				
				
				
				
				
				TableSchema.TableColumn colvarversion = new TableSchema.TableColumn(schema);
				colvarversion.ColumnName = "version";
				colvarversion.DataType = DbType.Int32;
				colvarversion.MaxLength = 0;
				colvarversion.AutoIncrement = true;
				colvarversion.IsNullable = true;
				colvarversion.IsPrimaryKey = false;
				colvarversion.IsForeignKey = false;
				colvarversion.IsReadOnly = false;
				schema.Columns.Add(colvarversion);

				
				
				
            }
        }
    
    
    }


    
	/// <summary>
	/// Strongly-typed collection for the TestTable class.
	/// </summary>
    [Serializable]
	public partial class TestTableCollection : ActiveList<TestTable, TestTableCollection>
	{	   
		public TestTableCollection() {}
	}
    
    /// <summary>
	/// This is an ActiveRecord class which wraps the TestTable table.
	/// </summary>
	[Serializable]
    public class TestTable : ActiveRecord<TestTable>, IActiveRecord{
    
        private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("TestTable", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				
				//columns
				
				
				
				TableSchema.TableColumn colvarBuffTop = new TableSchema.TableColumn(schema);
				colvarBuffTop.ColumnName = "BuffTop";
				colvarBuffTop.DataType = DbType.String;
				colvarBuffTop.MaxLength = 50;
				colvarBuffTop.AutoIncrement = true;
				colvarBuffTop.IsNullable = false;
				colvarBuffTop.IsPrimaryKey = false;
				colvarBuffTop.IsForeignKey = false;
				colvarBuffTop.IsReadOnly = false;
				schema.Columns.Add(colvarBuffTop);

				
				
				
				
				
				
				TableSchema.TableColumn colvarThingID = new TableSchema.TableColumn(schema);
				colvarThingID.ColumnName = "ThingID";
				colvarThingID.DataType = DbType.Guid;
				colvarThingID.MaxLength = 0;
				colvarThingID.AutoIncrement = true;
				colvarThingID.IsNullable = false;
				colvarThingID.IsPrimaryKey = false;
				colvarThingID.IsForeignKey = false;
				colvarThingID.IsReadOnly = false;
				schema.Columns.Add(colvarThingID);

				
				
				
            }
        }
    
    
    }


}


