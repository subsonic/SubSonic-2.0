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
    /// Controller class for CustomerDemographics
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class CustomerDemographicController
    {
        // Preload our schema..
        CustomerDemographic thisSchemaLoad = new CustomerDemographic();
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
        public CustomerDemographicCollection FetchAll()
        {
            CustomerDemographicCollection coll = new CustomerDemographicCollection();
            Query qry = new Query(CustomerDemographic.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CustomerDemographicCollection FetchByID(object CustomerTypeID)
        {
            CustomerDemographicCollection coll = new CustomerDemographicCollection().Where("CustomerTypeID", CustomerTypeID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public CustomerDemographicCollection FetchByQuery(Query qry)
        {
            CustomerDemographicCollection coll = new CustomerDemographicCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object CustomerTypeID)
        {
            return (CustomerDemographic.Delete(CustomerTypeID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object CustomerTypeID)
        {
            return (CustomerDemographic.Destroy(CustomerTypeID) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(string CustomerTypeID,string CustomerDesc)
	    {
		    CustomerDemographic item = new CustomerDemographic();
		    
            item.CustomerTypeID = CustomerTypeID;
            
            item.CustomerDesc = CustomerDesc;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(string CustomerTypeID,string CustomerDesc)
	    {
		    CustomerDemographic item = new CustomerDemographic();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.CustomerTypeID = CustomerTypeID;
				
			item.CustomerDesc = CustomerDesc;
				
	        item.Save(UserName);
	    }
    }
}
