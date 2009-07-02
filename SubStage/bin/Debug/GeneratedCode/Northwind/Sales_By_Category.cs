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
    /// Strongly-typed collection for the Sales_By_Category class.
    /// </summary>
    [Serializable]
    public partial class Sales_By_CategoryCollection : ReadOnlyList<Sales_By_Category, Sales_By_CategoryCollection>
    {        
        public Sales_By_CategoryCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Sales by Category view.
    /// </summary>
    [Serializable]
    public partial class Sales_By_Category : ReadOnlyRecord<Sales_By_Category>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("Sales by Category", TableType.View, DataService.GetInstance("Northwind"));
                schema.Columns = new TableSchema.TableColumnCollection();
                schema.SchemaName = @"dbo";
                //columns
                
                TableSchema.TableColumn colvarCategoryID = new TableSchema.TableColumn(schema);
                colvarCategoryID.ColumnName = "CategoryID";
                colvarCategoryID.DataType = DbType.Int32;
                colvarCategoryID.MaxLength = 0;
                colvarCategoryID.AutoIncrement = false;
                colvarCategoryID.IsNullable = false;
                colvarCategoryID.IsPrimaryKey = false;
                colvarCategoryID.IsForeignKey = false;
                colvarCategoryID.IsReadOnly = false;
                
                schema.Columns.Add(colvarCategoryID);
                
                TableSchema.TableColumn colvarCategoryName = new TableSchema.TableColumn(schema);
                colvarCategoryName.ColumnName = "CategoryName";
                colvarCategoryName.DataType = DbType.String;
                colvarCategoryName.MaxLength = 15;
                colvarCategoryName.AutoIncrement = false;
                colvarCategoryName.IsNullable = false;
                colvarCategoryName.IsPrimaryKey = false;
                colvarCategoryName.IsForeignKey = false;
                colvarCategoryName.IsReadOnly = false;
                
                schema.Columns.Add(colvarCategoryName);
                
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
                
                TableSchema.TableColumn colvarProductSales = new TableSchema.TableColumn(schema);
                colvarProductSales.ColumnName = "ProductSales";
                colvarProductSales.DataType = DbType.Currency;
                colvarProductSales.MaxLength = 0;
                colvarProductSales.AutoIncrement = false;
                colvarProductSales.IsNullable = true;
                colvarProductSales.IsPrimaryKey = false;
                colvarProductSales.IsForeignKey = false;
                colvarProductSales.IsReadOnly = false;
                
                schema.Columns.Add(colvarProductSales);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Sales by Category",schema);
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
	    public Sales_By_Category()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public Sales_By_Category(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public Sales_By_Category(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public Sales_By_Category(string columnName, object columnValue)
        {
            SetSQLProps();
            LoadByParam(columnName,columnValue);
        }
        
	    #endregion
	    
	    #region Props
	    
          
        [XmlAttribute("CategoryID")]
        [Bindable(true)]
        public int CategoryID 
	    {
		    get
		    {
			    return GetColumnValue<int>("CategoryID");
		    }
            set 
		    {
			    SetColumnValue("CategoryID", value);
            }
        }
	      
        [XmlAttribute("CategoryName")]
        [Bindable(true)]
        public string CategoryName 
	    {
		    get
		    {
			    return GetColumnValue<string>("CategoryName");
		    }
            set 
		    {
			    SetColumnValue("CategoryName", value);
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
	      
        [XmlAttribute("ProductSales")]
        [Bindable(true)]
        public decimal? ProductSales 
	    {
		    get
		    {
			    return GetColumnValue<decimal?>("ProductSales");
		    }
            set 
		    {
			    SetColumnValue("ProductSales", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string CategoryID = @"CategoryID";
            
            public static string CategoryName = @"CategoryName";
            
            public static string ProductName = @"ProductName";
            
            public static string ProductSales = @"ProductSales";
            
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
