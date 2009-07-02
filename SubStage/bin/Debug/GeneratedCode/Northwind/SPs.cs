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
namespace Northwind{
    public partial class SPs{
        
        /// <summary>
        /// Creates an object wrapper for the CustOrderHist Procedure
        /// </summary>
        public static StoredProcedure CustOrderHist(string CustomerID)
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("CustOrderHist", DataService.GetInstance("Northwind"), "dbo");
        	
            sp.Command.AddParameter("@CustomerID", CustomerID, DbType.String, null, null);
        	
            return sp;
        }
        
        /// <summary>
        /// Creates an object wrapper for the CustOrdersDetail Procedure
        /// </summary>
        public static StoredProcedure CustOrdersDetail(int? OrderID)
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("CustOrdersDetail", DataService.GetInstance("Northwind"), "dbo");
        	
            sp.Command.AddParameter("@OrderID", OrderID, DbType.Int32, 0, 10);
        	
            return sp;
        }
        
        /// <summary>
        /// Creates an object wrapper for the CustOrdersOrders Procedure
        /// </summary>
        public static StoredProcedure CustOrdersOrders(string CustomerID)
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("CustOrdersOrders", DataService.GetInstance("Northwind"), "dbo");
        	
            sp.Command.AddParameter("@CustomerID", CustomerID, DbType.String, null, null);
        	
            return sp;
        }
        
        /// <summary>
        /// Creates an object wrapper for the Employee Sales by Country Procedure
        /// </summary>
        public static StoredProcedure Employee_Sales_By_Country(DateTime? Beginning_Date, DateTime? Ending_Date)
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Employee Sales by Country", DataService.GetInstance("Northwind"), "dbo");
        	
            sp.Command.AddParameter("@Beginning_Date", Beginning_Date, DbType.DateTime, null, null);
        	
            sp.Command.AddParameter("@Ending_Date", Ending_Date, DbType.DateTime, null, null);
        	
            return sp;
        }
        
        /// <summary>
        /// Creates an object wrapper for the Peaches Procedure
        /// </summary>
        public static StoredProcedure Peaches(string tablename, string mapSuffix)
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Peaches", DataService.GetInstance("Northwind"), "dbo");
        	
            sp.Command.AddParameter("@tablename", tablename, DbType.String, null, null);
        	
            sp.Command.AddParameter("@mapSuffix", mapSuffix, DbType.String, null, null);
        	
            return sp;
        }
        
        /// <summary>
        /// Creates an object wrapper for the Sales by Year Procedure
        /// </summary>
        public static StoredProcedure Sales_By_Year(DateTime? Beginning_Date, DateTime? Ending_Date)
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Sales by Year", DataService.GetInstance("Northwind"), "dbo");
        	
            sp.Command.AddParameter("@Beginning_Date", Beginning_Date, DbType.DateTime, null, null);
        	
            sp.Command.AddParameter("@Ending_Date", Ending_Date, DbType.DateTime, null, null);
        	
            return sp;
        }
        
        /// <summary>
        /// Creates an object wrapper for the SalesByCategory Procedure
        /// </summary>
        public static StoredProcedure SalesByCategory(string CategoryName, string OrdYear)
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("SalesByCategory", DataService.GetInstance("Northwind"), "dbo");
        	
            sp.Command.AddParameter("@CategoryName", CategoryName, DbType.String, null, null);
        	
            sp.Command.AddParameter("@OrdYear", OrdYear, DbType.String, null, null);
        	
            return sp;
        }
        
        /// <summary>
        /// Creates an object wrapper for the SubSonicTest Procedure
        /// </summary>
        public static StoredProcedure SubSonicTest(DateTime? param)
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("SubSonicTest", DataService.GetInstance("Northwind"), "dbo");
        	
            sp.Command.AddOutputParameter("@param", DbType.DateTime, null, null);
            
            return sp;
        }
        
        /// <summary>
        /// Creates an object wrapper for the SubSonicTestNW Procedure
        /// </summary>
        public static StoredProcedure SubSonicTestNW(DateTime? param)
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("SubSonicTestNW", DataService.GetInstance("Northwind"), "dbo");
        	
            sp.Command.AddOutputParameter("@param", DbType.DateTime, null, null);
            
            return sp;
        }
        
        /// <summary>
        /// Creates an object wrapper for the Ten Most Expensive Products Procedure
        /// </summary>
        public static StoredProcedure Ten_Most_Expensive_Products()
        {
            SubSonic.StoredProcedure sp = new SubSonic.StoredProcedure("Ten Most Expensive Products", DataService.GetInstance("Northwind"), "");
        	
            return sp;
        }
        
    }
    
}
