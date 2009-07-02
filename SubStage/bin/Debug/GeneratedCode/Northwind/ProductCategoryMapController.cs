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
    public partial class ProductCategoryMapController
    {
        // Preload our schema..
        ProductCategoryMap thisSchemaLoad = new ProductCategoryMap();
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
        public ProductCategoryMapCollection FetchAll()
        {
            ProductCategoryMapCollection coll = new ProductCategoryMapCollection();
            Query qry = new Query(ProductCategoryMap.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public ProductCategoryMapCollection FetchByID(object CategoryID)
        {
            ProductCategoryMapCollection coll = new ProductCategoryMapCollection().Where("CategoryID", CategoryID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public ProductCategoryMapCollection FetchByQuery(Query qry)
        {
            ProductCategoryMapCollection coll = new ProductCategoryMapCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object CategoryID)
        {
            return (ProductCategoryMap.Delete(CategoryID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object CategoryID)
        {
            return (ProductCategoryMap.Destroy(CategoryID) == 1);
        }
        
        
        
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(int CategoryID,int ProductID)
        {
            Query qry = new Query(ProductCategoryMap.Schema);
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
		    ProductCategoryMap item = new ProductCategoryMap();
		    
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
		    ProductCategoryMap item = new ProductCategoryMap();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.CategoryID = CategoryID;
				
			item.ProductID = ProductID;
				
	        item.Save(UserName);
	    }
    }
}
