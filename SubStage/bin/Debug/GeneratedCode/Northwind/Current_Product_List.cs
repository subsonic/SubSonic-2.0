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
    /// Strongly-typed collection for the Current_Product_List class.
    /// </summary>
    [Serializable]
    public partial class Current_Product_ListCollection : ReadOnlyList<Current_Product_List, Current_Product_ListCollection>
    {        
        public Current_Product_ListCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Current Product List view.
    /// </summary>
    [Serializable]
    public partial class Current_Product_List : ReadOnlyRecord<Current_Product_List>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("Current Product List", TableType.View, DataService.GetInstance("Northwind"));
                schema.Columns = new TableSchema.TableColumnCollection();
                schema.SchemaName = @"dbo";
                //columns
                
                TableSchema.TableColumn colvarProductID = new TableSchema.TableColumn(schema);
                colvarProductID.ColumnName = "ProductID";
                colvarProductID.DataType = DbType.Int32;
                colvarProductID.MaxLength = 0;
                colvarProductID.AutoIncrement = false;
                colvarProductID.IsNullable = false;
                colvarProductID.IsPrimaryKey = false;
                colvarProductID.IsForeignKey = false;
                colvarProductID.IsReadOnly = false;
                
                schema.Columns.Add(colvarProductID);
                
                TableSchema.TableColumn colvarProductName = new TableSchema.TableColumn(schema);
                colvarProductName.ColumnName = "ProductName";
                colvarProductName.DataType = DbType.String;
                colvarProductName.MaxLength = 40;
                colvarProductName.AutoIncrement = false;
                colvarProductName.IsNullable = false;
                colvarProductName.IsPrimaryKey = false;
                colvarProductName.IsForeignKey = false;
                colvarProductName.IsReadOnly = false;
                
                schema.Columns.Add(colvarProductName);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Current Product List",schema);
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
	    public Current_Product_List()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public Current_Product_List(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public Current_Product_List(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public Current_Product_List(string columnName, object columnValue)
        {
            SetSQLProps();
            LoadByParam(columnName,columnValue);
        }
        
	    #endregion
	    
	    #region Props
	    
          
        [XmlAttribute("ProductID")]
        [Bindable(true)]
        public int ProductID 
	    {
		    get
		    {
			    return GetColumnValue<int>("ProductID");
		    }
            set 
		    {
			    SetColumnValue("ProductID", value);
            }
        }
	      
        [XmlAttribute("ProductName")]
        [Bindable(true)]
        public string ProductName 
	    {
		    get
		    {
			    return GetColumnValue<string>("ProductName");
		    }
            set 
		    {
			    SetColumnValue("ProductName", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string ProductID = @"ProductID";
            
            public static string ProductName = @"ProductName";
            
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
