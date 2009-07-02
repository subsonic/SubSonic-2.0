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
    /// Strongly-typed collection for the Summary_Of_Sales_By_Year class.
    /// </summary>
    [Serializable]
    public partial class Summary_Of_Sales_By_YearCollection : ReadOnlyList<Summary_Of_Sales_By_Year, Summary_Of_Sales_By_YearCollection>
    {        
        public Summary_Of_Sales_By_YearCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Summary of Sales by Year view.
    /// </summary>
    [Serializable]
    public partial class Summary_Of_Sales_By_Year : ReadOnlyRecord<Summary_Of_Sales_By_Year>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("Summary of Sales by Year", TableType.View, DataService.GetInstance("Northwind"));
                schema.Columns = new TableSchema.TableColumnCollection();
                schema.SchemaName = @"dbo";
                //columns
                
                TableSchema.TableColumn colvarShippedDate = new TableSchema.TableColumn(schema);
                colvarShippedDate.ColumnName = "ShippedDate";
                colvarShippedDate.DataType = DbType.DateTime;
                colvarShippedDate.MaxLength = 0;
                colvarShippedDate.AutoIncrement = false;
                colvarShippedDate.IsNullable = true;
                colvarShippedDate.IsPrimaryKey = false;
                colvarShippedDate.IsForeignKey = false;
                colvarShippedDate.IsReadOnly = false;
                
                schema.Columns.Add(colvarShippedDate);
                
                TableSchema.TableColumn colvarOrderID = new TableSchema.TableColumn(schema);
                colvarOrderID.ColumnName = "OrderID";
                colvarOrderID.DataType = DbType.Int32;
                colvarOrderID.MaxLength = 0;
                colvarOrderID.AutoIncrement = false;
                colvarOrderID.IsNullable = false;
                colvarOrderID.IsPrimaryKey = false;
                colvarOrderID.IsForeignKey = false;
                colvarOrderID.IsReadOnly = false;
                
                schema.Columns.Add(colvarOrderID);
                
                TableSchema.TableColumn colvarSubtotal = new TableSchema.TableColumn(schema);
                colvarSubtotal.ColumnName = "Subtotal";
                colvarSubtotal.DataType = DbType.Currency;
                colvarSubtotal.MaxLength = 0;
                colvarSubtotal.AutoIncrement = false;
                colvarSubtotal.IsNullable = true;
                colvarSubtotal.IsPrimaryKey = false;
                colvarSubtotal.IsForeignKey = false;
                colvarSubtotal.IsReadOnly = false;
                
                schema.Columns.Add(colvarSubtotal);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Summary of Sales by Year",schema);
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
	    public Summary_Of_Sales_By_Year()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public Summary_Of_Sales_By_Year(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public Summary_Of_Sales_By_Year(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public Summary_Of_Sales_By_Year(string columnName, object columnValue)
        {
            SetSQLProps();
            LoadByParam(columnName,columnValue);
        }
        
	    #endregion
	    
	    #region Props
	    
          
        [XmlAttribute("ShippedDate")]
        [Bindable(true)]
        public DateTime? ShippedDate 
	    {
		    get
		    {
			    return GetColumnValue<DateTime?>("ShippedDate");
		    }
            set 
		    {
			    SetColumnValue("ShippedDate", value);
            }
        }
	      
        [XmlAttribute("OrderID")]
        [Bindable(true)]
        public int OrderID 
	    {
		    get
		    {
			    return GetColumnValue<int>("OrderID");
		    }
            set 
		    {
			    SetColumnValue("OrderID", value);
            }
        }
	      
        [XmlAttribute("Subtotal")]
        [Bindable(true)]
        public decimal? Subtotal 
	    {
		    get
		    {
			    return GetColumnValue<decimal?>("Subtotal");
		    }
            set 
		    {
			    SetColumnValue("Subtotal", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string ShippedDate = @"ShippedDate";
            
            public static string OrderID = @"OrderID";
            
            public static string Subtotal = @"Subtotal";
            
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
