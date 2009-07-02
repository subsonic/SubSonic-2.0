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
	#region Tables Struct
	public partial struct Tables
	{
		
		public static string Category = @"Categories";
        
		public static string CustomerCustomerDemo = @"CustomerCustomerDemo";
        
		public static string CustomerDemographic = @"CustomerDemographics";
        
		public static string Customer = @"Customers";
        
		public static string Employee = @"Employees";
        
		public static string EmployeeTerritory = @"EmployeeTerritories";
        
		public static string OrderDetail = @"Order Details";
        
		public static string Order = @"Orders";
        
		public static string ProductCategoryMap = @"Product_Category_Map";
        
		public static string Product = @"Products";
        
		public static string Region = @"Region";
        
		public static string Shipper = @"Shippers";
        
		public static string SubSonicSchemaInfo = @"SubSonicSchemaInfo";
        
		public static string Supplier = @"Suppliers";
        
		public static string Territory = @"Territories";
        
		public static string TextEntry = @"TextEntry";
        
	}
	#endregion
    #region Schemas
    public partial class Schemas {
		
		public static TableSchema.Table Category{
            get { return DataService.GetSchema("Categories","Northwind"); }
		}
        
		public static TableSchema.Table CustomerCustomerDemo{
            get { return DataService.GetSchema("CustomerCustomerDemo","Northwind"); }
		}
        
		public static TableSchema.Table CustomerDemographic{
            get { return DataService.GetSchema("CustomerDemographics","Northwind"); }
		}
        
		public static TableSchema.Table Customer{
            get { return DataService.GetSchema("Customers","Northwind"); }
		}
        
		public static TableSchema.Table Employee{
            get { return DataService.GetSchema("Employees","Northwind"); }
		}
        
		public static TableSchema.Table EmployeeTerritory{
            get { return DataService.GetSchema("EmployeeTerritories","Northwind"); }
		}
        
		public static TableSchema.Table OrderDetail{
            get { return DataService.GetSchema("Order Details","Northwind"); }
		}
        
		public static TableSchema.Table Order{
            get { return DataService.GetSchema("Orders","Northwind"); }
		}
        
		public static TableSchema.Table ProductCategoryMap{
            get { return DataService.GetSchema("Product_Category_Map","Northwind"); }
		}
        
		public static TableSchema.Table Product{
            get { return DataService.GetSchema("Products","Northwind"); }
		}
        
		public static TableSchema.Table Region{
            get { return DataService.GetSchema("Region","Northwind"); }
		}
        
		public static TableSchema.Table Shipper{
            get { return DataService.GetSchema("Shippers","Northwind"); }
		}
        
		public static TableSchema.Table SubSonicSchemaInfo{
            get { return DataService.GetSchema("SubSonicSchemaInfo","Northwind"); }
		}
        
		public static TableSchema.Table Supplier{
            get { return DataService.GetSchema("Suppliers","Northwind"); }
		}
        
		public static TableSchema.Table Territory{
            get { return DataService.GetSchema("Territories","Northwind"); }
		}
        
		public static TableSchema.Table TextEntry{
            get { return DataService.GetSchema("TextEntry","Northwind"); }
		}
        
	
    }
    #endregion
    #region View Struct
    public partial struct Views 
    {
		
		public static string AlphabeticalListOfProduct = @"Alphabetical list of products";
        
		public static string CategorySalesFor1997 = @"Category Sales for 1997";
        
		public static string CurrentProductList = @"Current Product List";
        
		public static string CustomerAndSuppliersByCity = @"Customer and Suppliers by City";
        
		public static string Invoice = @"Invoices";
        
		public static string OrderDetailsExtended = @"Order Details Extended";
        
		public static string OrderSubtotal = @"Order Subtotals";
        
		public static string OrdersQry = @"Orders Qry";
        
		public static string ProductSalesFor1997 = @"Product Sales for 1997";
        
		public static string ProductsAboveAveragePrice = @"Products Above Average Price";
        
		public static string ProductsByCategory = @"Products by Category";
        
		public static string QuarterlyOrder = @"Quarterly Orders";
        
		public static string SalesByCategory = @"Sales by Category";
        
		public static string SalesTotalsByAmount = @"Sales Totals by Amount";
        
		public static string SummaryOfSalesByQuarter = @"Summary of Sales by Quarter";
        
		public static string SummaryOfSalesByYear = @"Summary of Sales by Year";
        
    }
    #endregion
    
    #region Query Factories
	public static partial class DB
	{
        public static DataProvider _provider = DataService.Providers["Northwind"];
        static ISubSonicRepository _repository;
        public static ISubSonicRepository Repository {
            get {
                if (_repository == null)
                    return new SubSonicRepository(_provider);
                return _repository; 
            }
            set { _repository = value; }
        }
	
        public static Select SelectAllColumnsFrom<T>() where T : RecordBase<T>, new()
	    {
            return Repository.SelectAllColumnsFrom<T>();
            
	    }
	    public static Select Select()
	    {
            return Repository.Select();
	    }
	    
		public static Select Select(params string[] columns)
		{
            return Repository.Select(columns);
        }
	    
		public static Select Select(params Aggregate[] aggregates)
		{
            return Repository.Select(aggregates);
        }
   
	    public static Update Update<T>() where T : RecordBase<T>, new()
	    {
            return Repository.Update<T>();
	    }
     
	    
	    public static Insert Insert()
	    {
            return Repository.Insert();
	    }
	    
	    public static Delete Delete()
	    {
            
            return Repository.Delete();
	    }
	    
	    public static InlineQuery Query()
	    {
            
            return Repository.Query();
	    }
	    	    
	    
	}
    #endregion
    
}
#region Databases
public partial struct Databases 
{
	
	public static string Northwind = @"Northwind";
    
}
#endregion