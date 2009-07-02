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
    /// Controller class for Product_Category_Map
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class Product_Category_MapController
    {
        // Preload our schema..
        Product_Category_Map thisSchemaLoad = new Product_Category_Map();
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
        public Product_Category_MapCollection FetchAll()
        {
            Product_Category_MapCollection coll = new Product_Category_MapCollection();
            Query qry = new Query(Product_Category_Map.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public Product_Category_MapCollection FetchByID(object CategoryID)
        {
            Product_Category_MapCollection coll = new Product_Category_MapCollection().Where("CategoryID", CategoryID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public Product_Category_MapCollection FetchByQuery(Query qry)
        {
            Product_Category_MapCollection coll = new Product_Category_MapCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object CategoryID)
        {
            return (Product_Category_Map.Delete(CategoryID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object CategoryID)
        {
            return (Product_Category_Map.Destroy(CategoryID) == 1);
        }
        
        
        
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(int CategoryID,int ProductID)
        {
            Query qry = new Query(Product_Category_Map.Schema);
            qry.QueryType = QueryType.Delete;
            qry.AddWhere("CategoryID", CategoryID).AND("ProductID", ProductID);
            qry.Execute();
            return (true);
        }        
       
    	
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(int CategoryID,int ProductID)
	    {
		    Product_Category_Map item = new Product_Category_Map();
		    
            item.CategoryID = CategoryID;
            
            item.ProductID = ProductID;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(int CategoryID,int ProductID)
	    {
		    Product_Category_Map item = new Product_Category_Map();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.CategoryID = CategoryID;
				
			item.ProductID = ProductID;
				
	        item.Save(UserName);
	    }
    }
}
