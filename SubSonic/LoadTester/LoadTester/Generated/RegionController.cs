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
    /// Controller class for Region
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class RegionController
    {
        // Preload our schema..
        Region thisSchemaLoad = new Region();
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
        public RegionCollection FetchAll()
        {
            RegionCollection coll = new RegionCollection();
            Query qry = new Query(Region.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public RegionCollection FetchByID(object RegionID)
        {
            RegionCollection coll = new RegionCollection().Where("RegionID", RegionID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public RegionCollection FetchByQuery(Query qry)
        {
            RegionCollection coll = new RegionCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object RegionID)
        {
            return (Region.Delete(RegionID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object RegionID)
        {
            return (Region.Destroy(RegionID) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(string RegionDescription)
	    {
		    Region item = new Region();
		    
            item.RegionDescription = RegionDescription;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(int RegionID,string RegionDescription)
	    {
		    Region item = new Region();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.RegionID = RegionID;
				
			item.RegionDescription = RegionDescription;
				
	        item.Save(UserName);
	    }
    }
}
