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
    /// Controller class for Customers
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class CustomerController
    {
        // Preload our schema..
        Customer thisSchemaLoad = new Customer();
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
        public CustomerCollection FetchAll()
        {
            CustomerCollection coll = new CustomerCollection();
            Query qry = new Query(Customer.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public CustomerCollection FetchByID(object CustomerID)
        {
            CustomerCollection coll = new CustomerCollection().Where("CustomerID", CustomerID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public CustomerCollection FetchByQuery(Query qry)
        {
            CustomerCollection coll = new CustomerCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object CustomerID)
        {
            return (Customer.Delete(CustomerID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object CustomerID)
        {
            return (Customer.Destroy(CustomerID) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(string CustomerID,string CompanyName,string ContactName,string ContactTitle,string Address,string City,string Region,string PostalCode,string Country,string Phone,string Fax)
	    {
		    Customer item = new Customer();
		    
            item.CustomerID = CustomerID;
            
            item.CompanyName = CompanyName;
            
            item.ContactName = ContactName;
            
            item.ContactTitle = ContactTitle;
            
            item.Address = Address;
            
            item.City = City;
            
            item.Region = Region;
            
            item.PostalCode = PostalCode;
            
            item.Country = Country;
            
            item.Phone = Phone;
            
            item.Fax = Fax;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(string CustomerID,string CompanyName,string ContactName,string ContactTitle,string Address,string City,string Region,string PostalCode,string Country,string Phone,string Fax)
	    {
		    Customer item = new Customer();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.CustomerID = CustomerID;
				
			item.CompanyName = CompanyName;
				
			item.ContactName = ContactName;
				
			item.ContactTitle = ContactTitle;
				
			item.Address = Address;
				
			item.City = City;
				
			item.Region = Region;
				
			item.PostalCode = PostalCode;
				
			item.Country = Country;
				
			item.Phone = Phone;
				
			item.Fax = Fax;
				
	        item.Save(UserName);
	    }
    }
}
