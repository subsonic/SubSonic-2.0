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
    /// Strongly-typed collection for the AlphabeticalListOfProduct class.
    /// </summary>
    [Serializable]
    public partial class AlphabeticalListOfProductCollection : ReadOnlyList<AlphabeticalListOfProduct, AlphabeticalListOfProductCollection>
    {        
        public AlphabeticalListOfProductCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Alphabetical list of products view.
    /// </summary>
    [Serializable]
    public partial class AlphabeticalListOfProduct : ReadOnlyRecord<AlphabeticalListOfProduct>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("Alphabetical list of products", TableType.View, DataService.GetInstance("Northwind"));
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
                
                TableSchema.TableColumn colvarSupplierID = new TableSchema.TableColumn(schema);
                colvarSupplierID.ColumnName = "SupplierID";
                colvarSupplierID.DataType = DbType.Int32;
                colvarSupplierID.MaxLength = 0;
                colvarSupplierID.AutoIncrement = false;
                colvarSupplierID.IsNullable = true;
                colvarSupplierID.IsPrimaryKey = false;
                colvarSupplierID.IsForeignKey = false;
                colvarSupplierID.IsReadOnly = false;
                
                schema.Columns.Add(colvarSupplierID);
                
                TableSchema.TableColumn colvarCategoryID = new TableSchema.TableColumn(schema);
                colvarCategoryID.ColumnName = "CategoryID";
                colvarCategoryID.DataType = DbType.Int32;
                colvarCategoryID.MaxLength = 0;
                colvarCategoryID.AutoIncrement = false;
                colvarCategoryID.IsNullable = true;
                colvarCategoryID.IsPrimaryKey = false;
                colvarCategoryID.IsForeignKey = false;
                colvarCategoryID.IsReadOnly = false;
                
                schema.Columns.Add(colvarCategoryID);
                
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
                
                TableSchema.TableColumn colvarUnitPrice = new TableSchema.TableColumn(schema);
                colvarUnitPrice.ColumnName = "UnitPrice";
                colvarUnitPrice.DataType = DbType.Currency;
                colvarUnitPrice.MaxLength = 0;
                colvarUnitPrice.AutoIncrement = false;
                colvarUnitPrice.IsNullable = true;
                colvarUnitPrice.IsPrimaryKey = false;
                colvarUnitPrice.IsForeignKey = false;
                colvarUnitPrice.IsReadOnly = false;
                
                schema.Columns.Add(colvarUnitPrice);
                
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
                
                TableSchema.TableColumn colvarUnitsOnOrder = new TableSchema.TableColumn(schema);
                colvarUnitsOnOrder.ColumnName = "UnitsOnOrder";
                colvarUnitsOnOrder.DataType = DbType.Int16;
                colvarUnitsOnOrder.MaxLength = 0;
                colvarUnitsOnOrder.AutoIncrement = false;
                colvarUnitsOnOrder.IsNullable = true;
                colvarUnitsOnOrder.IsPrimaryKey = false;
                colvarUnitsOnOrder.IsForeignKey = false;
                colvarUnitsOnOrder.IsReadOnly = false;
                
                schema.Columns.Add(colvarUnitsOnOrder);
                
                TableSchema.TableColumn colvarReorderLevel = new TableSchema.TableColumn(schema);
                colvarReorderLevel.ColumnName = "ReorderLevel";
                colvarReorderLevel.DataType = DbType.Int16;
                colvarReorderLevel.MaxLength = 0;
                colvarReorderLevel.AutoIncrement = false;
                colvarReorderLevel.IsNullable = true;
                colvarReorderLevel.IsPrimaryKey = false;
                colvarReorderLevel.IsForeignKey = false;
                colvarReorderLevel.IsReadOnly = false;
                
                schema.Columns.Add(colvarReorderLevel);
                
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
                
                TableSchema.TableColumn colvarAttributeXML = new TableSchema.TableColumn(schema);
                colvarAttributeXML.ColumnName = "AttributeXML";
                colvarAttributeXML.DataType = DbType.AnsiString;
                colvarAttributeXML.MaxLength = -1;
                colvarAttributeXML.AutoIncrement = false;
                colvarAttributeXML.IsNullable = true;
                colvarAttributeXML.IsPrimaryKey = false;
                colvarAttributeXML.IsForeignKey = false;
                colvarAttributeXML.IsReadOnly = false;
                
                schema.Columns.Add(colvarAttributeXML);
                
                TableSchema.TableColumn colvarDateCreated = new TableSchema.TableColumn(schema);
                colvarDateCreated.ColumnName = "DateCreated";
                colvarDateCreated.DataType = DbType.DateTime;
                colvarDateCreated.MaxLength = 0;
                colvarDateCreated.AutoIncrement = false;
                colvarDateCreated.IsNullable = true;
                colvarDateCreated.IsPrimaryKey = false;
                colvarDateCreated.IsForeignKey = false;
                colvarDateCreated.IsReadOnly = false;
                
                schema.Columns.Add(colvarDateCreated);
                
                TableSchema.TableColumn colvarProductGUID = new TableSchema.TableColumn(schema);
                colvarProductGUID.ColumnName = "ProductGUID";
                colvarProductGUID.DataType = DbType.Guid;
                colvarProductGUID.MaxLength = 0;
                colvarProductGUID.AutoIncrement = false;
                colvarProductGUID.IsNullable = true;
                colvarProductGUID.IsPrimaryKey = false;
                colvarProductGUID.IsForeignKey = false;
                colvarProductGUID.IsReadOnly = false;
                
                schema.Columns.Add(colvarProductGUID);
                
                TableSchema.TableColumn colvarCreatedOn = new TableSchema.TableColumn(schema);
                colvarCreatedOn.ColumnName = "CreatedOn";
                colvarCreatedOn.DataType = DbType.DateTime;
                colvarCreatedOn.MaxLength = 0;
                colvarCreatedOn.AutoIncrement = false;
                colvarCreatedOn.IsNullable = false;
                colvarCreatedOn.IsPrimaryKey = false;
                colvarCreatedOn.IsForeignKey = false;
                colvarCreatedOn.IsReadOnly = false;
                
                schema.Columns.Add(colvarCreatedOn);
                
                TableSchema.TableColumn colvarCreatedBy = new TableSchema.TableColumn(schema);
                colvarCreatedBy.ColumnName = "CreatedBy";
                colvarCreatedBy.DataType = DbType.String;
                colvarCreatedBy.MaxLength = 50;
                colvarCreatedBy.AutoIncrement = false;
                colvarCreatedBy.IsNullable = true;
                colvarCreatedBy.IsPrimaryKey = false;
                colvarCreatedBy.IsForeignKey = false;
                colvarCreatedBy.IsReadOnly = false;
                
                schema.Columns.Add(colvarCreatedBy);
                
                TableSchema.TableColumn colvarModifiedOn = new TableSchema.TableColumn(schema);
                colvarModifiedOn.ColumnName = "ModifiedOn";
                colvarModifiedOn.DataType = DbType.DateTime;
                colvarModifiedOn.MaxLength = 0;
                colvarModifiedOn.AutoIncrement = false;
                colvarModifiedOn.IsNullable = false;
                colvarModifiedOn.IsPrimaryKey = false;
                colvarModifiedOn.IsForeignKey = false;
                colvarModifiedOn.IsReadOnly = false;
                
                schema.Columns.Add(colvarModifiedOn);
                
                TableSchema.TableColumn colvarModifiedBy = new TableSchema.TableColumn(schema);
                colvarModifiedBy.ColumnName = "ModifiedBy";
                colvarModifiedBy.DataType = DbType.String;
                colvarModifiedBy.MaxLength = 50;
                colvarModifiedBy.AutoIncrement = false;
                colvarModifiedBy.IsNullable = true;
                colvarModifiedBy.IsPrimaryKey = false;
                colvarModifiedBy.IsForeignKey = false;
                colvarModifiedBy.IsReadOnly = false;
                
                schema.Columns.Add(colvarModifiedBy);
                
                TableSchema.TableColumn colvarDeleted = new TableSchema.TableColumn(schema);
                colvarDeleted.ColumnName = "Deleted";
                colvarDeleted.DataType = DbType.Boolean;
                colvarDeleted.MaxLength = 0;
                colvarDeleted.AutoIncrement = false;
                colvarDeleted.IsNullable = false;
                colvarDeleted.IsPrimaryKey = false;
                colvarDeleted.IsForeignKey = false;
                colvarDeleted.IsReadOnly = false;
                
                schema.Columns.Add(colvarDeleted);
                
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
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Alphabetical list of products",schema);
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
	    public AlphabeticalListOfProduct()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public AlphabeticalListOfProduct(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public AlphabeticalListOfProduct(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public AlphabeticalListOfProduct(string columnName, object columnValue)
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
	      
        [XmlAttribute("SupplierID")]
        [Bindable(true)]
        public int? SupplierID 
	    {
		    get
		    {
			    return GetColumnValue<int?>("SupplierID");
		    }
            set 
		    {
			    SetColumnValue("SupplierID", value);
            }
        }
	      
        [XmlAttribute("CategoryID")]
        [Bindable(true)]
        public int? CategoryID 
	    {
		    get
		    {
			    return GetColumnValue<int?>("CategoryID");
		    }
            set 
		    {
			    SetColumnValue("CategoryID", value);
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
	      
        [XmlAttribute("UnitPrice")]
        [Bindable(true)]
        public decimal? UnitPrice 
	    {
		    get
		    {
			    return GetColumnValue<decimal?>("UnitPrice");
		    }
            set 
		    {
			    SetColumnValue("UnitPrice", value);
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
	      
        [XmlAttribute("UnitsOnOrder")]
        [Bindable(true)]
        public short? UnitsOnOrder 
	    {
		    get
		    {
			    return GetColumnValue<short?>("UnitsOnOrder");
		    }
            set 
		    {
			    SetColumnValue("UnitsOnOrder", value);
            }
        }
	      
        [XmlAttribute("ReorderLevel")]
        [Bindable(true)]
        public short? ReorderLevel 
	    {
		    get
		    {
			    return GetColumnValue<short?>("ReorderLevel");
		    }
            set 
		    {
			    SetColumnValue("ReorderLevel", value);
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
	      
        [XmlAttribute("AttributeXML")]
        [Bindable(true)]
        public string AttributeXML 
	    {
		    get
		    {
			    return GetColumnValue<string>("AttributeXML");
		    }
            set 
		    {
			    SetColumnValue("AttributeXML", value);
            }
        }
	      
        [XmlAttribute("DateCreated")]
        [Bindable(true)]
        public DateTime? DateCreated 
	    {
		    get
		    {
			    return GetColumnValue<DateTime?>("DateCreated");
		    }
            set 
		    {
			    SetColumnValue("DateCreated", value);
            }
        }
	      
        [XmlAttribute("ProductGUID")]
        [Bindable(true)]
        public Guid? ProductGUID 
	    {
		    get
		    {
			    return GetColumnValue<Guid?>("ProductGUID");
		    }
            set 
		    {
			    SetColumnValue("ProductGUID", value);
            }
        }
	      
        [XmlAttribute("CreatedOn")]
        [Bindable(true)]
        public DateTime CreatedOn 
	    {
		    get
		    {
			    return GetColumnValue<DateTime>("CreatedOn");
		    }
            set 
		    {
			    SetColumnValue("CreatedOn", value);
            }
        }
	      
        [XmlAttribute("CreatedBy")]
        [Bindable(true)]
        public string CreatedBy 
	    {
		    get
		    {
			    return GetColumnValue<string>("CreatedBy");
		    }
            set 
		    {
			    SetColumnValue("CreatedBy", value);
            }
        }
	      
        [XmlAttribute("ModifiedOn")]
        [Bindable(true)]
        public DateTime ModifiedOn 
	    {
		    get
		    {
			    return GetColumnValue<DateTime>("ModifiedOn");
		    }
            set 
		    {
			    SetColumnValue("ModifiedOn", value);
            }
        }
	      
        [XmlAttribute("ModifiedBy")]
        [Bindable(true)]
        public string ModifiedBy 
	    {
		    get
		    {
			    return GetColumnValue<string>("ModifiedBy");
		    }
            set 
		    {
			    SetColumnValue("ModifiedBy", value);
            }
        }
	      
        [XmlAttribute("Deleted")]
        [Bindable(true)]
        public bool Deleted 
	    {
		    get
		    {
			    return GetColumnValue<bool>("Deleted");
		    }
            set 
		    {
			    SetColumnValue("Deleted", value);
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
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string ProductID = @"ProductID";
            
            public static string ProductName = @"ProductName";
            
            public static string SupplierID = @"SupplierID";
            
            public static string CategoryID = @"CategoryID";
            
            public static string QuantityPerUnit = @"QuantityPerUnit";
            
            public static string UnitPrice = @"UnitPrice";
            
            public static string UnitsInStock = @"UnitsInStock";
            
            public static string UnitsOnOrder = @"UnitsOnOrder";
            
            public static string ReorderLevel = @"ReorderLevel";
            
            public static string Discontinued = @"Discontinued";
            
            public static string AttributeXML = @"AttributeXML";
            
            public static string DateCreated = @"DateCreated";
            
            public static string ProductGUID = @"ProductGUID";
            
            public static string CreatedOn = @"CreatedOn";
            
            public static string CreatedBy = @"CreatedBy";
            
            public static string ModifiedOn = @"ModifiedOn";
            
            public static string ModifiedBy = @"ModifiedBy";
            
            public static string Deleted = @"Deleted";
            
            public static string CategoryName = @"CategoryName";
            
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
