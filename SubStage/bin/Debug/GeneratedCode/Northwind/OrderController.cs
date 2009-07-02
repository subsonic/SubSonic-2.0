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
    /// Controller class for Orders
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class OrderController
    {
        // Preload our schema..
        Order thisSchemaLoad = new Order();
        private string userName = String.Empty;
        protected string UserName
        {
            get
            {
				if (userName.Length == 0) 
				{
    				if (System.Web.HttpContext.Current != null)
    				{
						userName=System.Web.HttpContext.Current.User.Identity.Name;
					}
					else
					{
						userName=System.Threading.Thread.CurrentPrincipal.Identity.Name;
					}
				}
				return userName;
            }
        }
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public OrderCollection FetchAll()
        {
            OrderCollection coll = new OrderCollection();
            Query qry = new Query(Order.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public OrderCollection FetchByID(object OrderID)
        {
            OrderCollection coll = new OrderCollection().Where("OrderID", OrderID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public OrderCollection FetchByQuery(Query qry)
        {
            OrderCollection coll = new OrderCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object OrderID)
        {
            return (Order.Delete(OrderID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object OrderID)
        {
            return (Order.Destroy(OrderID) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(string CustomerID,int? EmployeeID,DateTime? OrderDate,DateTime? RequiredDate,DateTime? ShippedDate,int? ShipVia,decimal? Freight,string ShipName,string ShipAddress,string ShipCity,string ShipRegion,string ShipPostalCode,string ShipCountry)
	    {
		    Order item = new Order();
		    
            item.CustomerID = CustomerID;
            
            item.EmployeeID = EmployeeID;
            
            item.OrderDate = OrderDate;
            
            item.RequiredDate = RequiredDate;
            
            item.ShippedDate = ShippedDate;
            
            item.ShipVia = ShipVia;
            
            item.Freight = Freight;
            
            item.ShipName = ShipName;
            
            item.ShipAddress = ShipAddress;
            
            item.ShipCity = ShipCity;
            
            item.ShipRegion = ShipRegion;
            
            item.ShipPostalCode = ShipPostalCode;
            
            item.ShipCountry = ShipCountry;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(int OrderID,string CustomerID,int? EmployeeID,DateTime? OrderDate,DateTime? RequiredDate,DateTime? ShippedDate,int? ShipVia,decimal? Freight,string ShipName,string ShipAddress,string ShipCity,string ShipRegion,string ShipPostalCode,string ShipCountry)
	    {
		    Order item = new Order();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.OrderID = OrderID;
				
			item.CustomerID = CustomerID;
				
			item.EmployeeID = EmployeeID;
				
			item.OrderDate = OrderDate;
				
			item.RequiredDate = RequiredDate;
				
			item.ShippedDate = ShippedDate;
				
			item.ShipVia = ShipVia;
				
			item.Freight = Freight;
				
			item.ShipName = ShipName;
				
			item.ShipAddress = ShipAddress;
				
			item.ShipCity = ShipCity;
				
			item.ShipRegion = ShipRegion;
				
			item.ShipPostalCode = ShipPostalCode;
				
			item.ShipCountry = ShipCountry;
				
	        item.Save(UserName);
	    }
    }
}
