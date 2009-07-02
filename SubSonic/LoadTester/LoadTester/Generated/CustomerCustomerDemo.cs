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
	/// Strongly-typed collection for the CustomerCustomerDemo class.
	/// </summary>
    [Serializable]
	public partial class CustomerCustomerDemoCollection : ActiveList<CustomerCustomerDemo, CustomerCustomerDemoCollection>
	{	   
		public CustomerCustomerDemoCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>CustomerCustomerDemoCollection</returns>
		public CustomerCustomerDemoCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                CustomerCustomerDemo o = this[i];
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
	/// This is an ActiveRecord class which wraps the CustomerCustomerDemo table.
	/// </summary>
	[Serializable]
	public partial class CustomerCustomerDemo : ActiveRecord<CustomerCustomerDemo>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public CustomerCustomerDemo()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public CustomerCustomerDemo(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public CustomerCustomerDemo(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public CustomerCustomerDemo(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("CustomerCustomerDemo", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
				colvarCustomerID.ColumnName = "CustomerID";
				colvarCustomerID.DataType = DbType.String;
				colvarCustomerID.MaxLength = 5;
				colvarCustomerID.AutoIncrement = false;
				colvarCustomerID.IsNullable = false;
				colvarCustomerID.IsPrimaryKey = true;
				colvarCustomerID.IsForeignKey = true;
				colvarCustomerID.IsReadOnly = false;
				colvarCustomerID.DefaultSetting = @"";
				
					colvarCustomerID.ForeignKeyTableName = "Customers";
				schema.Columns.Add(colvarCustomerID);
				
				TableSchema.TableColumn colvarCustomerTypeID = new TableSchema.TableColumn(schema);
				colvarCustomerTypeID.ColumnName = "CustomerTypeID";
				colvarCustomerTypeID.DataType = DbType.String;
				colvarCustomerTypeID.MaxLength = 10;
				colvarCustomerTypeID.AutoIncrement = false;
				colvarCustomerTypeID.IsNullable = false;
				colvarCustomerTypeID.IsPrimaryKey = true;
				colvarCustomerTypeID.IsForeignKey = true;
				colvarCustomerTypeID.IsReadOnly = false;
				colvarCustomerTypeID.DefaultSetting = @"";
				
					colvarCustomerTypeID.ForeignKeyTableName = "CustomerDemographics";
				schema.Columns.Add(colvarCustomerTypeID);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("CustomerCustomerDemo",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("CustomerID")]
		[Bindable(true)]
		public string CustomerID 
		{
			get { return GetColumnValue<string>(Columns.CustomerID); }
			set { SetColumnValue(Columns.CustomerID, value); }
		}
		  
		[XmlAttribute("CustomerTypeID")]
		[Bindable(true)]
		public string CustomerTypeID 
		{
			get { return GetColumnValue<string>(Columns.CustomerTypeID); }
			set { SetColumnValue(Columns.CustomerTypeID, value); }
		}
		
		#endregion
		
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a CustomerDemographic ActiveRecord object related to this CustomerCustomerDemo
		/// 
		/// </summary>
		public Northwind.CustomerDemographic CustomerDemographic
		{
			get { return Northwind.CustomerDemographic.FetchByID(this.CustomerTypeID); }
			set { SetColumnValue("CustomerTypeID", value.CustomerTypeID); }
		}
		
		
		/// <summary>
		/// Returns a Customer ActiveRecord object related to this CustomerCustomerDemo
		/// 
		/// </summary>
		public Northwind.Customer Customer
		{
			get { return Northwind.Customer.FetchByID(this.CustomerID); }
			set { SetColumnValue("CustomerID", value.CustomerID); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varCustomerID,string varCustomerTypeID)
		{
			CustomerCustomerDemo item = new CustomerCustomerDemo();
			
			item.CustomerID = varCustomerID;
			
			item.CustomerTypeID = varCustomerTypeID;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(string varCustomerID,string varCustomerTypeID)
		{
			CustomerCustomerDemo item = new CustomerCustomerDemo();
			
				item.CustomerID = varCustomerID;
			
				item.CustomerTypeID = varCustomerTypeID;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn CustomerIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn CustomerTypeIDColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string CustomerID = @"CustomerID";
			 public static string CustomerTypeID = @"CustomerTypeID";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}
