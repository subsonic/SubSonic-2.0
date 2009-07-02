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
    /// Controller class for Territories
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class TerritoryController
    {
        // Preload our schema..
        Territory thisSchemaLoad = new Territory();
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
        public TerritoryCollection FetchAll()
        {
            TerritoryCollection coll = new TerritoryCollection();
            Query qry = new Query(Territory.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public TerritoryCollection FetchByID(object TerritoryID)
        {
            TerritoryCollection coll = new TerritoryCollection().Where("TerritoryID", TerritoryID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public TerritoryCollection FetchByQuery(Query qry)
        {
            TerritoryCollection coll = new TerritoryCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object TerritoryID)
        {
            return (Territory.Delete(TerritoryID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object TerritoryID)
        {
            return (Territory.Destroy(TerritoryID) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(string TerritoryID,string TerritoryDescription,int RegionID)
	    {
		    Territory item = new Territory();
		    
            item.TerritoryID = TerritoryID;
            
            item.TerritoryDescription = TerritoryDescription;
            
            item.RegionID = RegionID;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(string TerritoryID,string TerritoryDescription,int RegionID)
	    {
		    Territory item = new Territory();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.TerritoryID = TerritoryID;
				
			item.TerritoryDescription = TerritoryDescription;
				
			item.RegionID = RegionID;
				
	        item.Save(UserName);
	    }
    }
}
