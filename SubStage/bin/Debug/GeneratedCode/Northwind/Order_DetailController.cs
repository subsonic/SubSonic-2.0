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
    /// Controller class for Order Details
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class Order_DetailController
    {
        // Preload our schema..
        Order_Detail thisSchemaLoad = new Order_Detail();
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
        public Order_DetailCollection FetchAll()
        {
            Order_DetailCollection coll = new Order_DetailCollection();
            Query qry = new Query(Order_Detail.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public Order_DetailCollection FetchByID(object OrderID)
        {
            Order_DetailCollection coll = new Order_DetailCollection().Where("OrderID", OrderID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public Order_DetailCollection FetchByQuery(Query qry)
        {
            Order_DetailCollection coll = new Order_DetailCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object OrderID)
        {
            return (Order_Detail.Delete(OrderID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object OrderID)
        {
            return (Order_Detail.Destroy(OrderID) == 1);
        }
        
        
        
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(int OrderID,int ProductID)
        {
            Query qry = new Query(Order_Detail.Schema);
            qry.QueryType = QueryType.Delete;
            qry.AddWhere("OrderID", OrderID).AND("ProductID", ProductID);
            qry.Execute();
            return (true);
        }        
       
    	
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(int OrderID,int ProductID,decimal UnitPrice,short Quantity,float Discount)
	    {
		    Order_Detail item = new Order_Detail();
		    
            item.OrderID = OrderID;
            
            item.ProductID = ProductID;
            
            item.UnitPrice = UnitPrice;
            
            item.Quantity = Quantity;
            
            item.Discount = Discount;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(int OrderID,int ProductID,decimal UnitPrice,short Quantity,float Discount)
	    {
		    Order_Detail item = new Order_Detail();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.OrderID = OrderID;
				
			item.ProductID = ProductID;
				
			item.UnitPrice = UnitPrice;
				
			item.Quantity = Quantity;
				
			item.Discount = Discount;
				
	        item.Save(UserName);
	    }
    }
}
