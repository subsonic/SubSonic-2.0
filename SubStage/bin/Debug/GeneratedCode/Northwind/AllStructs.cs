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
        
		public static string Order_Detail = @"Order Details";
        
		public static string Order = @"Orders";
        
		public static string Product_Category_Map = @"Product_Category_Map";
        
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
        
		public static TableSchema.Table Order_Detail{
            get { return DataService.GetSchema("Order Details","Northwind"); }
		}
        
		public static TableSchema.Table Order{
            get { return DataService.GetSchema("Orders","Northwind"); }
		}
        
		public static TableSchema.Table Product_Category_Map{
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
		
		public static string Alphabetical_List_Of_Product = @"Alphabetical list of products";
        
		public static string Category_Sales_For_1997 = @"Category Sales for 1997";
        
		public static string Current_Product_List = @"Current Product List";
        
		public static string Customer_And_Suppliers_By_City = @"Customer and Suppliers by City";
        
		public static string Invoice = @"Invoices";
        
		public static string Order_Details_Extended = @"Order Details Extended";
        
		public static string Order_Subtotal = @"Order Subtotals";
        
		public static string Orders_Qry = @"Orders Qry";
        
		public static string Product_Sales_For_1997 = @"Product Sales for 1997";
        
		public static string Products_Above_Average_Price = @"Products Above Average Price";
        
		public static string Products_By_Category = @"Products by Category";
        
		public static string Quarterly_Order = @"Quarterly Orders";
        
		public static string Sales_By_Category = @"Sales by Category";
        
		public static string Sales_Totals_By_Amount = @"Sales Totals by Amount";
        
		public static string Summary_Of_Sales_By_Quarter = @"Summary of Sales by Quarter";
        
		public static string Summary_Of_Sales_By_Year = @"Summary of Sales by Year";
        
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