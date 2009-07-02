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
    /// Controller class for TextEntry
    /// </summary>
    [System.ComponentModel.DataObject]
    public partial class TextEntryController
    {
        // Preload our schema..
        TextEntry thisSchemaLoad = new TextEntry();
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
        public TextEntryCollection FetchAll()
        {
            TextEntryCollection coll = new TextEntryCollection();
            Query qry = new Query(TextEntry.Schema);
            coll.LoadAndCloseReader(qry.ExecuteReader());
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public TextEntryCollection FetchByID(object ContentID)
        {
            TextEntryCollection coll = new TextEntryCollection().Where("contentID", ContentID).Load();
            return coll;
        }
		
		[DataObjectMethod(DataObjectMethodType.Select, false)]
        public TextEntryCollection FetchByQuery(Query qry)
        {
            TextEntryCollection coll = new TextEntryCollection();
            coll.LoadAndCloseReader(qry.ExecuteReader()); 
            return coll;
        }
        [DataObjectMethod(DataObjectMethodType.Delete, true)]
        public bool Delete(object ContentID)
        {
            return (TextEntry.Delete(ContentID) == 1);
        }
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public bool Destroy(object ContentID)
        {
            return (TextEntry.Destroy(ContentID) == 1);
        }
        
        
    	
	    /// <summary>
	    /// Inserts a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Insert, true)]
	    public void Insert(Guid ContentGUID,string Title,string ContentName,string Content,string IconPath,DateTime? DateExpires,string LastEditedBy,string ExternalLink,string Status,int ListOrder,string CallOut,DateTime? CreatedOn,string CreatedBy,DateTime? ModifiedOn,string ModifiedBy)
	    {
		    TextEntry item = new TextEntry();
		    
            item.ContentGUID = ContentGUID;
            
            item.Title = Title;
            
            item.ContentName = ContentName;
            
            item.Content = Content;
            
            item.IconPath = IconPath;
            
            item.DateExpires = DateExpires;
            
            item.LastEditedBy = LastEditedBy;
            
            item.ExternalLink = ExternalLink;
            
            item.Status = Status;
            
            item.ListOrder = ListOrder;
            
            item.CallOut = CallOut;
            
            item.CreatedOn = CreatedOn;
            
            item.CreatedBy = CreatedBy;
            
            item.ModifiedOn = ModifiedOn;
            
            item.ModifiedBy = ModifiedBy;
            
	    
		    item.Save(UserName);
	    }
    	
	    /// <summary>
	    /// Updates a record, can be used with the Object Data Source
	    /// </summary>
        [DataObjectMethod(DataObjectMethodType.Update, true)]
	    public void Update(int ContentID,Guid ContentGUID,string Title,string ContentName,string Content,string IconPath,DateTime? DateExpires,string LastEditedBy,string ExternalLink,string Status,int ListOrder,string CallOut,DateTime? CreatedOn,string CreatedBy,DateTime? ModifiedOn,string ModifiedBy)
	    {
		    TextEntry item = new TextEntry();
	        item.MarkOld();
	        item.IsLoaded = true;
		    
			item.ContentID = ContentID;
				
			item.ContentGUID = ContentGUID;
				
			item.Title = Title;
				
			item.ContentName = ContentName;
				
			item.Content = Content;
				
			item.IconPath = IconPath;
				
			item.DateExpires = DateExpires;
				
			item.LastEditedBy = LastEditedBy;
				
			item.ExternalLink = ExternalLink;
				
			item.Status = Status;
				
			item.ListOrder = ListOrder;
				
			item.CallOut = CallOut;
				
			item.CreatedOn = CreatedOn;
				
			item.CreatedBy = CreatedBy;
				
			item.ModifiedOn = ModifiedOn;
				
			item.ModifiedBy = ModifiedBy;
				
	        item.Save(UserName);
	    }
    }
}
