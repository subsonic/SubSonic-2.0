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
	/// Strongly-typed collection for the CustomerDemographic class.
	/// </summary>
    [Serializable]
	public partial class CustomerDemographicCollection : ActiveList<CustomerDemographic, CustomerDemographicCollection>
	{	   
		public CustomerDemographicCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>CustomerDemographicCollection</returns>
		public CustomerDemographicCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                CustomerDemographic o = this[i];
                foreach (SubSonic.Where w in this.wheres)
                {
                    bool remove = false;
                    System.Reflection.PropertyInfo pi = o.GetType().GetProperty(w.ColumnName);
                    if (pi.CanRead)
                    {
                        object val = pi.GetValue(o, null);
                        switch (w.Comparison)
                        {
                            case SubSonic.Comparison.Equals:
                                if (!val.Equals(w.ParameterValue))
                                {
                                    remove = true;
                                }
                                break;
                        }
                    }
                    if (remove)
                    {
                        this.Remove(o);
                        break;
                    }
                }
            }
            return this;
        }
		
		
	}
	/// <summary>
	/// This is an ActiveRecord class which wraps the CustomerDemographics table.
	/// </summary>
	[Serializable]
	public partial class CustomerDemographic : ActiveRecord<CustomerDemographic>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public CustomerDemographic()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public CustomerDemographic(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public CustomerDemographic(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public CustomerDemographic(string columnName, object columnValue)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByParam(columnName,columnValue);
		}
		
		protected static void SetSQLProps() { GetTableSchema(); }
		
		#endregion
		
		#region Schema and Query Accessor	
		public static Query CreateQuery() { return new Query(Schema); }
		public static TableSchema.Table Schema
		{
			get
			{
				if (BaseSchema == null)
					SetSQLProps();
				return BaseSchema;
			}
		}
		
		private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("CustomerDemographics", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarCustomerTypeID = new TableSchema.TableColumn(schema);
				colvarCustomerTypeID.ColumnName = "CustomerTypeID";
				colvarCustomerTypeID.DataType = DbType.String;
				colvarCustomerTypeID.MaxLength = 10;
				colvarCustomerTypeID.AutoIncrement = false;
				colvarCustomerTypeID.IsNullable = false;
				colvarCustomerTypeID.IsPrimaryKey = true;
				colvarCustomerTypeID.IsForeignKey = false;
				colvarCustomerTypeID.IsReadOnly = false;
				colvarCustomerTypeID.DefaultSetting = @"";
				colvarCustomerTypeID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCustomerTypeID);
				
				TableSchema.TableColumn colvarCustomerDesc = new TableSchema.TableColumn(schema);
				colvarCustomerDesc.ColumnName = "CustomerDesc";
				colvarCustomerDesc.DataType = DbType.String;
				colvarCustomerDesc.MaxLength = 1073741823;
				colvarCustomerDesc.AutoIncrement = false;
				colvarCustomerDesc.IsNullable = true;
				colvarCustomerDesc.IsPrimaryKey = false;
				colvarCustomerDesc.IsForeignKey = false;
				colvarCustomerDesc.IsReadOnly = false;
				colvarCustomerDesc.DefaultSetting = @"";
				colvarCustomerDesc.ForeignKeyTableName = "";
				schema.Columns.Add(colvarCustomerDesc);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("CustomerDemographics",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("CustomerTypeID")]
		[Bindable(true)]
		public string CustomerTypeID 
		{
			get { return GetColumnValue<string>(Columns.CustomerTypeID); }
			set { SetColumnValue(Columns.CustomerTypeID, value); }
		}
		  
		[XmlAttribute("CustomerDesc")]
		[Bindable(true)]
		public string CustomerDesc 
		{
			get { return GetColumnValue<string>(Columns.CustomerDesc); }
			set { SetColumnValue(Columns.CustomerDesc, value); }
		}
		
		#endregion
		
		
		#region PrimaryKey Methods		
		
        protected override void SetPrimaryKey(object oValue)
        {
            base.SetPrimaryKey(oValue);
            
            SetPKValues();
        }
        
		
		public Northwind.CustomerCustomerDemoCollection CustomerCustomerDemoRecords()
		{
			return new Northwind.CustomerCustomerDemoCollection().Where(CustomerCustomerDemo.Columns.CustomerTypeID, CustomerTypeID).Load();
		}
		#endregion
		
			
		
		//no foreign key tables defined (0)
		
		
		
		#region Many To Many Helpers
		
		 
		public Northwind.CustomerCollection GetCustomerCollection() { return CustomerDemographic.GetCustomerCollection(this.CustomerTypeID); }
		public static Northwind.CustomerCollection GetCustomerCollection(string varCustomerTypeID)
		{
		    SubSonic.QueryCommand cmd = new SubSonic.QueryCommand("SELECT * FROM [dbo].[Customers] INNER JOIN [CustomerCustomerDemo] ON [Customers].[CustomerID] = [CustomerCustomerDemo].[CustomerID] WHERE [CustomerCustomerDemo].[CustomerTypeID] = @CustomerTypeID", CustomerDemographic.Schema.Provider.Name);
			cmd.AddParameter("@CustomerTypeID", varCustomerTypeID, DbType.String);
			IDataReader rdr = SubSonic.DataService.GetReader(cmd);
			CustomerCollection coll = new CustomerCollection();
			coll.LoadAndCloseReader(rdr);
			return coll;
		}
		
		public static void SaveCustomerMap(string varCustomerTypeID, CustomerCollection items)
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [CustomerCustomerDemo] WHERE [CustomerCustomerDemo].[CustomerTypeID] = @CustomerTypeID", CustomerDemographic.Schema.Provider.Name);
			cmdDel.AddParameter("@CustomerTypeID", varCustomerTypeID, DbType.String);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (Customer item in items)
			{
				CustomerCustomerDemo varCustomerCustomerDemo = new CustomerCustomerDemo();
				varCustomerCustomerDemo.SetColumnValue("CustomerTypeID", varCustomerTypeID);
				varCustomerCustomerDemo.SetColumnValue("CustomerID", item.GetPrimaryKeyValue());
				varCustomerCustomerDemo.Save();
			}
		}
		public static void SaveCustomerMap(string varCustomerTypeID, System.Web.UI.WebControls.ListItemCollection itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [CustomerCustomerDemo] WHERE [CustomerCustomerDemo].[CustomerTypeID] = @CustomerTypeID", CustomerDemographic.Schema.Provider.Name);
			cmdDel.AddParameter("@CustomerTypeID", varCustomerTypeID, DbType.String);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (System.Web.UI.WebControls.ListItem l in itemList) 
			{
				if (l.Selected) 
				{
					CustomerCustomerDemo varCustomerCustomerDemo = new CustomerCustomerDemo();
					varCustomerCustomerDemo.SetColumnValue("CustomerTypeID", varCustomerTypeID);
					varCustomerCustomerDemo.SetColumnValue("CustomerID", l.Value);
					varCustomerCustomerDemo.Save();
				}
			}
		}
		public static void SaveCustomerMap(string varCustomerTypeID , string[] itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [CustomerCustomerDemo] WHERE [CustomerCustomerDemo].[CustomerTypeID] = @CustomerTypeID", CustomerDemographic.Schema.Provider.Name);
			cmdDel.AddParameter("@CustomerTypeID", varCustomerTypeID, DbType.String);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (string item in itemList) 
			{
				CustomerCustomerDemo varCustomerCustomerDemo = new CustomerCustomerDemo();
				varCustomerCustomerDemo.SetColumnValue("CustomerTypeID", varCustomerTypeID);
				varCustomerCustomerDemo.SetColumnValue("CustomerID", item);
				varCustomerCustomerDemo.Save();
			}
		}
		
		public static void DeleteCustomerMap(string varCustomerTypeID) 
		{
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [CustomerCustomerDemo] WHERE [CustomerCustomerDemo].[CustomerTypeID] = @CustomerTypeID", CustomerDemographic.Schema.Provider.Name);
			cmdDel.AddParameter("@CustomerTypeID", varCustomerTypeID, DbType.String);
			DataService.ExecuteQuery(cmdDel);
		}
		
		#endregion
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varCustomerTypeID,string varCustomerDesc)
		{
			CustomerDemographic item = new CustomerDemographic();
			
			item.CustomerTypeID = varCustomerTypeID;
			
			item.CustomerDesc = varCustomerDesc;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(string varCustomerTypeID,string varCustomerDesc)
		{
			CustomerDemographic item = new CustomerDemographic();
			
				item.CustomerTypeID = varCustomerTypeID;
			
				item.CustomerDesc = varCustomerDesc;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn CustomerTypeIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn CustomerDescColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string CustomerTypeID = @"CustomerTypeID";
			 public static string CustomerDesc = @"CustomerDesc";
						
		}
		#endregion
		
		#region Update PK Collections
		
        public void SetPKValues()
        {
}
        #endregion
    
        #region Deep Save
		
        public void DeepSave()
        {
            Save();
            
}
        #endregion
	}
}
