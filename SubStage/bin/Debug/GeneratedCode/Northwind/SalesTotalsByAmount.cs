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
    /// Strongly-typed collection for the SalesTotalsByAmount class.
    /// </summary>
    [Serializable]
    public partial class SalesTotalsByAmountCollection : ReadOnlyList<SalesTotalsByAmount, SalesTotalsByAmountCollection>
    {        
        public SalesTotalsByAmountCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Sales Totals by Amount view.
    /// </summary>
    [Serializable]
    public partial class SalesTotalsByAmount : ReadOnlyRecord<SalesTotalsByAmount>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("Sales Totals by Amount", TableType.View, DataService.GetInstance("Northwind"));
                schema.Columns = new TableSchema.TableColumnCollection();
                schema.SchemaName = @"dbo";
                //columns
                
                TableSchema.TableColumn colvarSaleAmount = new TableSchema.TableColumn(schema);
                colvarSaleAmount.ColumnName = "SaleAmount";
                colvarSaleAmount.DataType = DbType.Currency;
                colvarSaleAmount.MaxLength = 0;
                colvarSaleAmount.AutoIncrement = false;
                colvarSaleAmount.IsNullable = true;
                colvarSaleAmount.IsPrimaryKey = false;
                colvarSaleAmount.IsForeignKey = false;
                colvarSaleAmount.IsReadOnly = false;
                
                schema.Columns.Add(colvarSaleAmount);
                
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
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Sales Totals by Amount",schema);
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
	    public SalesTotalsByAmount()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public SalesTotalsByAmount(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public SalesTotalsByAmount(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public SalesTotalsByAmount(string columnName, object columnValue)
        {
            SetSQLProps();
            LoadByParam(columnName,columnValue);
        }
        
	    #endregion
	    
	    #region Props
	    
          
        [XmlAttribute("SaleAmount")]
        [Bindable(true)]
        public decimal? SaleAmount 
	    {
		    get
		    {
			    return GetColumnValue<decimal?>("SaleAmount");
		    }
            set 
		    {
			    SetColumnValue("SaleAmount", value);
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
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string SaleAmount = @"SaleAmount";
            
            public static string OrderID = @"OrderID";
            
            public static string CompanyName = @"CompanyName";
            
            public static string ShippedDate = @"ShippedDate";
            
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
