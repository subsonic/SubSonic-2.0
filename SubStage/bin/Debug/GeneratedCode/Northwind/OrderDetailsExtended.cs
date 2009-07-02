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
    /// Strongly-typed collection for the OrderDetailsExtended class.
    /// </summary>
    [Serializable]
    public partial class OrderDetailsExtendedCollection : ReadOnlyList<OrderDetailsExtended, OrderDetailsExtendedCollection>
    {        
        public OrderDetailsExtendedCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Order Details Extended view.
    /// </summary>
    [Serializable]
    public partial class OrderDetailsExtended : ReadOnlyRecord<OrderDetailsExtended>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("Order Details Extended", TableType.View, DataService.GetInstance("Northwind"));
                schema.Columns = new TableSchema.TableColumnCollection();
                schema.SchemaName = @"dbo";
                //columns
                
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
                
                TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
                colvarUnitPrice.ColumnName = "UnitPrice";
                colvarUnitPrice.DataType = DbType.Currency;
                colvarUnitPrice.MaxLength = 0;
                colvarUnitPrice.AutoIncrement = false;
                colvarUnitPrice.IsNullable = false;
                colvarUnitPrice.IsPrimaryKey = false;
                colvarUnitPrice.IsForeignKey = false;
                colvarUnitPrice.IsReadOnly = false;
                
                schema.Columns.Add(colvarUnitPrice);
                
                TableSchema.TableColumn colvarQuantity = new TableSchema.TableColumn(schema);
                colvarQuantity.ColumnName = "Quantity";
                colvarQuantity.DataType = DbType.Int16;
                colvarQuantity.MaxLength = 0;
                colvarQuantity.AutoIncrement = false;
                colvarQuantity.IsNullable = false;
                colvarQuantity.IsPrimaryKey = false;
                colvarQuantity.IsForeignKey = false;
                colvarQuantity.IsReadOnly = false;
                
                schema.Columns.Add(colvarQuantity);
                
                TableSchema.TableColumn colvarDiscount = new TableSchema.TableColumn(schema);
                colvarDiscount.ColumnName = "Discount";
                colvarDiscount.DataType = DbType.Single;
                colvarDiscount.MaxLength = 0;
                colvarDiscount.AutoIncrement = false;
                colvarDiscount.IsNullable = false;
                colvarDiscount.IsPrimaryKey = false;
                colvarDiscount.IsForeignKey = false;
                colvarDiscount.IsReadOnly = false;
                
                schema.Columns.Add(colvarDiscount);
                
                TableSchema.TableColumn colvarExtendedPrice = new TableSchema.TableColumn(schema);
                colvarExtendedPrice.ColumnName = "ExtendedPrice";
                colvarExtendedPrice.DataType = DbType.Currency;
                colvarExtendedPrice.MaxLength = 0;
                colvarExtendedPrice.AutoIncrement = false;
                colvarExtendedPrice.IsNullable = true;
                colvarExtendedPrice.IsPrimaryKey = false;
                colvarExtendedPrice.IsForeignKey = false;
                colvarExtendedPrice.IsReadOnly = false;
                
                schema.Columns.Add(colvarExtendedPrice);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Order Details Extended",schema);
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
	    public OrderDetailsExtended()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public OrderDetailsExtended(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public OrderDetailsExtended(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public OrderDetailsExtended(string columnName, object columnValue)
        {
            SetSQLProps();
            LoadByParam(columnName,columnValue);
        }
        
	    #endregion
	    
	    #region Props
	    
          
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
	      
        [XmlAttribute("UnitPrice")]
        [Bindable(true)]
        public decimal UnitPrice 
	    {
		    get
		    {
			    return GetColumnValue<decimal>("UnitPrice");
		    }
            set 
		    {
			    SetColumnValue("UnitPrice", value);
            }
        }
	      
        [XmlAttribute("Quantity")]
        [Bindable(true)]
        public short Quantity 
	    {
		    get
		    {
			    return GetColumnValue<short>("Quantity");
		    }
            set 
		    {
			    SetColumnValue("Quantity", value);
            }
        }
	      
        [XmlAttribute("Discount")]
        [Bindable(true)]
        public float Discount 
	    {
		    get
		    {
			    return GetColumnValue<float>("Discount");
		    }
            set 
		    {
			    SetColumnValue("Discount", value);
            }
        }
	      
        [XmlAttribute("ExtendedPrice")]
        [Bindable(true)]
        public decimal? ExtendedPrice 
	    {
		    get
		    {
			    return GetColumnValue<decimal?>("ExtendedPrice");
		    }
            set 
		    {
			    SetColumnValue("ExtendedPrice", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string OrderID = @"OrderID";
            
            public static string ProductID = @"ProductID";
            
            public static string ProductName = @"ProductName";
            
            public static string UnitPrice = @"UnitPrice";
            
            public static string Quantity = @"Quantity";
            
            public static string Discount = @"Discount";
            
            public static string ExtendedPrice = @"ExtendedPrice";
            
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
