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
    /// Strongly-typed collection for the Products_By_Category class.
    /// </summary>
    [Serializable]
    public partial class Products_By_CategoryCollection : ReadOnlyList<Products_By_Category, Products_By_CategoryCollection>
    {        
        public Products_By_CategoryCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Products by Category view.
    /// </summary>
    [Serializable]
    public partial class Products_By_Category : ReadOnlyRecord<Products_By_Category>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("Products by Category", TableType.View, DataService.GetInstance("Northwind"));
                schema.Columns = new TableSchema.TableColumnCollection();
                schema.SchemaName = @"dbo";
                //columns
                
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
                
                TableSchema.TableColumn colvarQuantityPerUnit = new TableSchema.TableColumn(schema);
                colvarQuantityPerUnit.ColumnName = "QuantityPerUnit";
                colvarQuantityPerUnit.DataType = DbType.String;
                colvarQuantityPerUnit.MaxLength = 20;
                colvarQuantityPerUnit.AutoIncrement = false;
                colvarQuantityPerUnit.IsNullable = true;
                colvarQuantityPerUnit.IsPrimaryKey = false;
                colvarQuantityPerUnit.IsForeignKey = false;
                colvarQuantityPerUnit.IsReadOnly = false;
                
                schema.Columns.Add(colvarQuantityPerUnit);
                
                TableSchema.TableColumn colvarUnitsInStock = new TableSchema.TableColumn(schema);
                colvarUnitsInStock.ColumnName = "UnitsInStock";
                colvarUnitsInStock.DataType = DbType.Int16;
                colvarUnitsInStock.MaxLength = 0;
                colvarUnitsInStock.AutoIncrement = false;
                colvarUnitsInStock.IsNullable = true;
                colvarUnitsInStock.IsPrimaryKey = false;
                colvarUnitsInStock.IsForeignKey = false;
                colvarUnitsInStock.IsReadOnly = false;
                
                schema.Columns.Add(colvarUnitsInStock);
                
                TableSchema.TableColumn colvarDiscontinued = new TableSchema.TableColumn(schema);
                colvarDiscontinued.ColumnName = "Discontinued";
                colvarDiscontinued.DataType = DbType.Boolean;
                colvarDiscontinued.MaxLength = 0;
                colvarDiscontinued.AutoIncrement = false;
                colvarDiscontinued.IsNullable = false;
                colvarDiscontinued.IsPrimaryKey = false;
                colvarDiscontinued.IsForeignKey = false;
                colvarDiscontinued.IsReadOnly = false;
                
                schema.Columns.Add(colvarDiscontinued);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Products by Category",schema);
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
	    public Products_By_Category()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public Products_By_Category(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public Products_By_Category(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public Products_By_Category(string columnName, object columnValue)
        {
            SetSQLProps();
            LoadByParam(columnName,columnValue);
        }
        
	    #endregion
	    
	    #region Props
	    
          
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
	      
        [XmlAttribute("QuantityPerUnit")]
        [Bindable(true)]
        public string QuantityPerUnit 
	    {
		    get
		    {
			    return GetColumnValue<string>("QuantityPerUnit");
		    }
            set 
		    {
			    SetColumnValue("QuantityPerUnit", value);
            }
        }
	      
        [XmlAttribute("UnitsInStock")]
        [Bindable(true)]
        public short? UnitsInStock 
	    {
		    get
		    {
			    return GetColumnValue<short?>("UnitsInStock");
		    }
            set 
		    {
			    SetColumnValue("UnitsInStock", value);
            }
        }
	      
        [XmlAttribute("Discontinued")]
        [Bindable(true)]
        public bool Discontinued 
	    {
		    get
		    {
			    return GetColumnValue<bool>("Discontinued");
		    }
            set 
		    {
			    SetColumnValue("Discontinued", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string CategoryName = @"CategoryName";
            
            public static string ProductName = @"ProductName";
            
            public static string QuantityPerUnit = @"QuantityPerUnit";
            
            public static string UnitsInStock = @"UnitsInStock";
            
            public static string Discontinued = @"Discontinued";
            
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
