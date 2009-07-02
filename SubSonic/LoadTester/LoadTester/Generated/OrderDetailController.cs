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
    public partial class OrderDetailController
    {
        // Preload our schema..
        OrderDetail thisSchemaLoad = new OrderDetail();
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
        public OrderDetailCollection FetchAll()
        {
            OrderDetailCollection coll = new OrderDetailCollection();
            Query qry = new Query(OrderDetail.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public OrderDetailCollection FetchByID(object OrderID)
        {
            OrderDetailCollection coll = new OrderDetailCollection().Where("OrderID", OrderID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public OrderDetailCollection FetchByQuery(Query qry)
        {
            OrderDetailCollection coll = new OrderDetailCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object OrderID)
        {
            return (OrderDetail.Delete(OrderID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object OrderID)
        {
            return (OrderDetail.Destroy(OrderID) == 1);
        }
        
        
        
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(int OrderID,int ProductID)
        {
            Query qry = new Query(OrderDetail.Schema);
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
		    OrderDetail item = new OrderDetail();
		    
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
		    OrderDetail item = new OrderDetail();
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
