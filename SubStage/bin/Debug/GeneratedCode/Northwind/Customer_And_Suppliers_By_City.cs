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
namespace Northwind{
    /// <summary>
    /// Strongly-typed collection for the Customer_And_Suppliers_By_City class.
    /// </summary>
    [Serializable]
    public partial class Customer_And_Suppliers_By_CityCollection : ReadOnlyList<Customer_And_Suppliers_By_City, Customer_And_Suppliers_By_CityCollection>
    {        
        public Customer_And_Suppliers_By_CityCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Customer and Suppliers by City view.
    /// </summary>
    [Serializable]
    public partial class Customer_And_Suppliers_By_City : ReadOnlyRecord<Customer_And_Suppliers_By_City>, IReadOnlyRecord
    {
    
	    #region Default Settings
	    protected static void SetSQLProps() 
	    {
		    GetTableSchema();
	    }
	    #endregion
        #region Schema Accessor
	    public static TableSchema.Table Schema
        {
            get
            {
                if (BaseSchema == null)
                {
                    SetSQLProps();
                }
                return BaseSchema;
            }
        }
    	
        private static void GetTableSchema() 
        {
            if(!IsSchemaInitialized)
            {
                //Schema declaration
                TableSchema.Table schema = new TableSchema.Table("Customer and Suppliers by City", TableType.View, DataService.GetInstance("Northwind"));
                schema.Columns = new TableSchema.TableColumnCollection();
                schema.SchemaName = @"dbo";
                //columns
                
                TableSchema.TableColumn colvarCity = new TableSchema.TableColumn(schema);
                colvarCity.ColumnName = "City";
                colvarCity.DataType = DbType.String;
                colvarCity.MaxLength = 15;
                colvarCity.AutoIncrement = false;
                colvarCity.IsNullable = true;
                colvarCity.IsPrimaryKey = false;
                colvarCity.IsForeignKey = false;
                colvarCity.IsReadOnly = false;
                
                schema.Columns.Add(colvarCity);
                
                TableSchema.TableColumn colvarCompanyName = new TableSchema.TableColumn(schema);
                colvarCompanyName.ColumnName = "CompanyName";
                colvarCompanyName.DataType = DbType.String;
                colvarCompanyName.MaxLength = 40;
                colvarCompanyName.AutoIncrement = false;
                colvarCompanyName.IsNullable = false;
                colvarCompanyName.IsPrimaryKey = false;
                colvarCompanyName.IsForeignKey = false;
                colvarCompanyName.IsReadOnly = false;
                
                schema.Columns.Add(colvarCompanyName);
                
                TableSchema.TableColumn colvarContactName = new TableSchema.TableColumn(schema);
                colvarContactName.ColumnName = "ContactName";
                colvarContactName.DataType = DbType.String;
                colvarContactName.MaxLength = 30;
                colvarContactName.AutoIncrement = false;
                colvarContactName.IsNullable = true;
                colvarContactName.IsPrimaryKey = false;
                colvarContactName.IsForeignKey = false;
                colvarContactName.IsReadOnly = false;
                
                schema.Columns.Add(colvarContactName);
                
                TableSchema.TableColumn colvarRelationship = new TableSchema.TableColumn(schema);
                colvarRelationship.ColumnName = "Relationship";
                colvarRelationship.DataType = DbType.AnsiString;
                colvarRelationship.MaxLength = 9;
                colvarRelationship.AutoIncrement = false;
                colvarRelationship.IsNullable = false;
                colvarRelationship.IsPrimaryKey = false;
                colvarRelationship.IsForeignKey = false;
                colvarRelationship.IsReadOnly = false;
                
                schema.Columns.Add(colvarRelationship);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Customer and Suppliers by City",schema);
            }
        }
        #endregion
        
        #region Query Accessor
	    public static Query CreateQuery()
	    {
		    return new Query(Schema);
	    }
	    #endregion
	    
	    #region .ctors
	    public Customer_And_Suppliers_By_City()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public Customer_And_Suppliers_By_City(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public Customer_And_Suppliers_By_City(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public Customer_And_Suppliers_By_City(string columnName, object columnValue)
        {
            SetSQLProps();
            LoadByParam(columnName,columnValue);
        }
        
	    #endregion
	    
	    #region Props
	    
          
        [XmlAttribute("City")]
        [Bindable(true)]
        public string City 
	    {
		    get
		    {
			    return GetColumnValue<string>("City");
		    }
            set 
		    {
			    SetColumnValue("City", value);
            }
        }
	      
        [XmlAttribute("CompanyName")]
        [Bindable(true)]
        public string CompanyName 
	    {
		    get
		    {
			    return GetColumnValue<string>("CompanyName");
		    }
            set 
		    {
			    SetColumnValue("CompanyName", value);
            }
        }
	      
        [XmlAttribute("ContactName")]
        [Bindable(true)]
        public string ContactName 
	    {
		    get
		    {
			    return GetColumnValue<string>("ContactName");
		    }
            set 
		    {
			    SetColumnValue("ContactName", value);
            }
        }
	      
        [XmlAttribute("Relationship")]
        [Bindable(true)]
        public string Relationship 
	    {
		    get
		    {
			    return GetColumnValue<string>("Relationship");
		    }
            set 
		    {
			    SetColumnValue("Relationship", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string City = @"City";
            
            public static string CompanyName = @"CompanyName";
            
            public static string ContactName = @"ContactName";
            
            public static string Relationship = @"Relationship";
            
	    }
	    #endregion
	    
	    
	    #region IAbstractRecord Members
        public new CT GetColumnValue<CT>(string columnName) {
            return base.GetColumnValue<CT>(columnName);
        }
        public object GetColumnValue(string columnName) {
            return base.GetColumnValue<object>(columnName);
        }
        #endregion
	    
    }
}
