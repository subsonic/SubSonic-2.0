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
    /// Strongly-typed collection for the Orders_Qry class.
    /// </summary>
    [Serializable]
    public partial class Orders_QryCollection : ReadOnlyList<Orders_Qry, Orders_QryCollection>
    {        
        public Orders_QryCollection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Orders Qry view.
    /// </summary>
    [Serializable]
    public partial class Orders_Qry : ReadOnlyRecord<Orders_Qry>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("Orders Qry", TableType.View, DataService.GetInstance("Northwind"));
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
                
                TableSchema.TableColumn colvarCustomerID = new TableSchema.TableColumn(schema);
                colvarCustomerID.ColumnName = "CustomerID";
                colvarCustomerID.DataType = DbType.String;
                colvarCustomerID.MaxLength = 5;
                colvarCustomerID.AutoIncrement = false;
                colvarCustomerID.IsNullable = true;
                colvarCustomerID.IsPrimaryKey = false;
                colvarCustomerID.IsForeignKey = false;
                colvarCustomerID.IsReadOnly = false;
                
                schema.Columns.Add(colvarCustomerID);
                
                TableSchema.TableColumn colvarEmployeeID = new TableSchema.TableColumn(schema);
                colvarEmployeeID.ColumnName = "EmployeeID";
                colvarEmployeeID.DataType = DbType.Int32;
                colvarEmployeeID.MaxLength = 0;
                colvarEmployeeID.AutoIncrement = false;
                colvarEmployeeID.IsNullable = true;
                colvarEmployeeID.IsPrimaryKey = false;
                colvarEmployeeID.IsForeignKey = false;
                colvarEmployeeID.IsReadOnly = false;
                
                schema.Columns.Add(colvarEmployeeID);
                
                TableSchema.TableColumn colvarOrderDate = new TableSchema.TableColumn(schema);
                colvarOrderDate.ColumnName = "OrderDate";
                colvarOrderDate.DataType = DbType.DateTime;
                colvarOrderDate.MaxLength = 0;
                colvarOrderDate.AutoIncrement = false;
                colvarOrderDate.IsNullable = true;
                colvarOrderDate.IsPrimaryKey = false;
                colvarOrderDate.IsForeignKey = false;
                colvarOrderDate.IsReadOnly = false;
                
                schema.Columns.Add(colvarOrderDate);
                
                TableSchema.TableColumn colvarRequiredDate = new TableSchema.TableColumn(schema);
                colvarRequiredDate.ColumnName = "RequiredDate";
                colvarRequiredDate.DataType = DbType.DateTime;
                colvarRequiredDate.MaxLength = 0;
                colvarRequiredDate.AutoIncrement = false;
                colvarRequiredDate.IsNullable = true;
                colvarRequiredDate.IsPrimaryKey = false;
                colvarRequiredDate.IsForeignKey = false;
                colvarRequiredDate.IsReadOnly = false;
                
                schema.Columns.Add(colvarRequiredDate);
                
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
                
                TableSchema.TableColumn colvarShipVia = new TableSchema.TableColumn(schema);
                colvarShipVia.ColumnName = "ShipVia";
                colvarShipVia.DataType = DbType.Int32;
                colvarShipVia.MaxLength = 0;
                colvarShipVia.AutoIncrement = false;
                colvarShipVia.IsNullable = true;
                colvarShipVia.IsPrimaryKey = false;
                colvarShipVia.IsForeignKey = false;
                colvarShipVia.IsReadOnly = false;
                
                schema.Columns.Add(colvarShipVia);
                
                TableSchema.TableColumn colvarFreight = new TableSchema.TableColumn(schema);
                colvarFreight.ColumnName = "Freight";
                colvarFreight.DataType = DbType.Currency;
                colvarFreight.MaxLength = 0;
                colvarFreight.AutoIncrement = false;
                colvarFreight.IsNullable = true;
                colvarFreight.IsPrimaryKey = false;
                colvarFreight.IsForeignKey = false;
                colvarFreight.IsReadOnly = false;
                
                schema.Columns.Add(colvarFreight);
                
                TableSchema.TableColumn colvarShipName = new TableSchema.TableColumn(schema);
                colvarShipName.ColumnName = "ShipName";
                colvarShipName.DataType = DbType.String;
                colvarShipName.MaxLength = 40;
                colvarShipName.AutoIncrement = false;
                colvarShipName.IsNullable = true;
                colvarShipName.IsPrimaryKey = false;
                colvarShipName.IsForeignKey = false;
                colvarShipName.IsReadOnly = false;
                
                schema.Columns.Add(colvarShipName);
                
                TableSchema.TableColumn colvarShipAddress = new TableSchema.TableColumn(schema);
                colvarShipAddress.ColumnName = "ShipAddress";
                colvarShipAddress.DataType = DbType.String;
                colvarShipAddress.MaxLength = 60;
                colvarShipAddress.AutoIncrement = false;
                colvarShipAddress.IsNullable = true;
                colvarShipAddress.IsPrimaryKey = false;
                colvarShipAddress.IsForeignKey = false;
                colvarShipAddress.IsReadOnly = false;
                
                schema.Columns.Add(colvarShipAddress);
                
                TableSchema.TableColumn colvarShipCity = new TableSchema.TableColumn(schema);
                colvarShipCity.ColumnName = "ShipCity";
                colvarShipCity.DataType = DbType.String;
                colvarShipCity.MaxLength = 15;
                colvarShipCity.AutoIncrement = false;
                colvarShipCity.IsNullable = true;
                colvarShipCity.IsPrimaryKey = false;
                colvarShipCity.IsForeignKey = false;
                colvarShipCity.IsReadOnly = false;
                
                schema.Columns.Add(colvarShipCity);
                
                TableSchema.TableColumn colvarShipRegion = new TableSchema.TableColumn(schema);
                colvarShipRegion.ColumnName = "ShipRegion";
                colvarShipRegion.DataType = DbType.String;
                colvarShipRegion.MaxLength = 15;
                colvarShipRegion.AutoIncrement = false;
                colvarShipRegion.IsNullable = true;
                colvarShipRegion.IsPrimaryKey = false;
                colvarShipRegion.IsForeignKey = false;
                colvarShipRegion.IsReadOnly = false;
                
                schema.Columns.Add(colvarShipRegion);
                
                TableSchema.TableColumn colvarShipPostalCode = new TableSchema.TableColumn(schema);
                colvarShipPostalCode.ColumnName = "ShipPostalCode";
                colvarShipPostalCode.DataType = DbType.String;
                colvarShipPostalCode.MaxLength = 10;
                colvarShipPostalCode.AutoIncrement = false;
                colvarShipPostalCode.IsNullable = true;
                colvarShipPostalCode.IsPrimaryKey = false;
                colvarShipPostalCode.IsForeignKey = false;
                colvarShipPostalCode.IsReadOnly = false;
                
                schema.Columns.Add(colvarShipPostalCode);
                
                TableSchema.TableColumn colvarShipCountry = new TableSchema.TableColumn(schema);
                colvarShipCountry.ColumnName = "ShipCountry";
                colvarShipCountry.DataType = DbType.String;
                colvarShipCountry.MaxLength = 15;
                colvarShipCountry.AutoIncrement = false;
                colvarShipCountry.IsNullable = true;
                colvarShipCountry.IsPrimaryKey = false;
                colvarShipCountry.IsForeignKey = false;
                colvarShipCountry.IsReadOnly = false;
                
                schema.Columns.Add(colvarShipCountry);
                
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
                
                TableSchema.TableColumn colvarAddress = new TableSchema.TableColumn(schema);
                colvarAddress.ColumnName = "Address";
                colvarAddress.DataType = DbType.String;
                colvarAddress.MaxLength = 60;
                colvarAddress.AutoIncrement = false;
                colvarAddress.IsNullable = true;
                colvarAddress.IsPrimaryKey = false;
                colvarAddress.IsForeignKey = false;
                colvarAddress.IsReadOnly = false;
                
                schema.Columns.Add(colvarAddress);
                
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
                
                TableSchema.TableColumn colvarRegion = new TableSchema.TableColumn(schema);
                colvarRegion.ColumnName = "Region";
                colvarRegion.DataType = DbType.String;
                colvarRegion.MaxLength = 15;
                colvarRegion.AutoIncrement = false;
                colvarRegion.IsNullable = true;
                colvarRegion.IsPrimaryKey = false;
                colvarRegion.IsForeignKey = false;
                colvarRegion.IsReadOnly = false;
                
                schema.Columns.Add(colvarRegion);
                
                TableSchema.TableColumn colvarPostalCode = new TableSchema.TableColumn(schema);
                colvarPostalCode.ColumnName = "PostalCode";
                colvarPostalCode.DataType = DbType.String;
                colvarPostalCode.MaxLength = 10;
                colvarPostalCode.AutoIncrement = false;
                colvarPostalCode.IsNullable = true;
                colvarPostalCode.IsPrimaryKey = false;
                colvarPostalCode.IsForeignKey = false;
                colvarPostalCode.IsReadOnly = false;
                
                schema.Columns.Add(colvarPostalCode);
                
                TableSchema.TableColumn colvarCountry = new TableSchema.TableColumn(schema);
                colvarCountry.ColumnName = "Country";
                colvarCountry.DataType = DbType.String;
                colvarCountry.MaxLength = 15;
                colvarCountry.AutoIncrement = false;
                colvarCountry.IsNullable = true;
                colvarCountry.IsPrimaryKey = false;
                colvarCountry.IsForeignKey = false;
                colvarCountry.IsReadOnly = false;
                
                schema.Columns.Add(colvarCountry);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Orders Qry",schema);
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
	    public Orders_Qry()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public Orders_Qry(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public Orders_Qry(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public Orders_Qry(string columnName, object columnValue)
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
	      
        [XmlAttribute("CustomerID")]
        [Bindable(true)]
        public string CustomerID 
	    {
		    get
		    {
			    return GetColumnValue<string>("CustomerID");
		    }
            set 
		    {
			    SetColumnValue("CustomerID", value);
            }
        }
	      
        [XmlAttribute("EmployeeID")]
        [Bindable(true)]
        public int? EmployeeID 
	    {
		    get
		    {
			    return GetColumnValue<int?>("EmployeeID");
		    }
            set 
		    {
			    SetColumnValue("EmployeeID", value);
            }
        }
	      
        [XmlAttribute("OrderDate")]
        [Bindable(true)]
        public DateTime? OrderDate 
	    {
		    get
		    {
			    return GetColumnValue<DateTime?>("OrderDate");
		    }
            set 
		    {
			    SetColumnValue("OrderDate", value);
            }
        }
	      
        [XmlAttribute("RequiredDate")]
        [Bindable(true)]
        public DateTime? RequiredDate 
	    {
		    get
		    {
			    return GetColumnValue<DateTime?>("RequiredDate");
		    }
            set 
		    {
			    SetColumnValue("RequiredDate", value);
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
	      
        [XmlAttribute("ShipVia")]
        [Bindable(true)]
        public int? ShipVia 
	    {
		    get
		    {
			    return GetColumnValue<int?>("ShipVia");
		    }
            set 
		    {
			    SetColumnValue("ShipVia", value);
            }
        }
	      
        [XmlAttribute("Freight")]
        [Bindable(true)]
        public decimal? Freight 
	    {
		    get
		    {
			    return GetColumnValue<decimal?>("Freight");
		    }
            set 
		    {
			    SetColumnValue("Freight", value);
            }
        }
	      
        [XmlAttribute("ShipName")]
        [Bindable(true)]
        public string ShipName 
	    {
		    get
		    {
			    return GetColumnValue<string>("ShipName");
		    }
            set 
		    {
			    SetColumnValue("ShipName", value);
            }
        }
	      
        [XmlAttribute("ShipAddress")]
        [Bindable(true)]
        public string ShipAddress 
	    {
		    get
		    {
			    return GetColumnValue<string>("ShipAddress");
		    }
            set 
		    {
			    SetColumnValue("ShipAddress", value);
            }
        }
	      
        [XmlAttribute("ShipCity")]
        [Bindable(true)]
        public string ShipCity 
	    {
		    get
		    {
			    return GetColumnValue<string>("ShipCity");
		    }
            set 
		    {
			    SetColumnValue("ShipCity", value);
            }
        }
	      
        [XmlAttribute("ShipRegion")]
        [Bindable(true)]
        public string ShipRegion 
	    {
		    get
		    {
			    return GetColumnValue<string>("ShipRegion");
		    }
            set 
		    {
			    SetColumnValue("ShipRegion", value);
            }
        }
	      
        [XmlAttribute("ShipPostalCode")]
        [Bindable(true)]
        public string ShipPostalCode 
	    {
		    get
		    {
			    return GetColumnValue<string>("ShipPostalCode");
		    }
            set 
		    {
			    SetColumnValue("ShipPostalCode", value);
            }
        }
	      
        [XmlAttribute("ShipCountry")]
        [Bindable(true)]
        public string ShipCountry 
	    {
		    get
		    {
			    return GetColumnValue<string>("ShipCountry");
		    }
            set 
		    {
			    SetColumnValue("ShipCountry", value);
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
	      
        [XmlAttribute("Address")]
        [Bindable(true)]
        public string Address 
	    {
		    get
		    {
			    return GetColumnValue<string>("Address");
		    }
            set 
		    {
			    SetColumnValue("Address", value);
            }
        }
	      
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
	      
        [XmlAttribute("Region")]
        [Bindable(true)]
        public string Region 
	    {
		    get
		    {
			    return GetColumnValue<string>("Region");
		    }
            set 
		    {
			    SetColumnValue("Region", value);
            }
        }
	      
        [XmlAttribute("PostalCode")]
        [Bindable(true)]
        public string PostalCode 
	    {
		    get
		    {
			    return GetColumnValue<string>("PostalCode");
		    }
            set 
		    {
			    SetColumnValue("PostalCode", value);
            }
        }
	      
        [XmlAttribute("Country")]
        [Bindable(true)]
        public string Country 
	    {
		    get
		    {
			    return GetColumnValue<string>("Country");
		    }
            set 
		    {
			    SetColumnValue("Country", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string OrderID = @"OrderID";
            
            public static string CustomerID = @"CustomerID";
            
            public static string EmployeeID = @"EmployeeID";
            
            public static string OrderDate = @"OrderDate";
            
            public static string RequiredDate = @"RequiredDate";
            
            public static string ShippedDate = @"ShippedDate";
            
            public static string ShipVia = @"ShipVia";
            
            public static string Freight = @"Freight";
            
            public static string ShipName = @"ShipName";
            
            public static string ShipAddress = @"ShipAddress";
            
            public static string ShipCity = @"ShipCity";
            
            public static string ShipRegion = @"ShipRegion";
            
            public static string ShipPostalCode = @"ShipPostalCode";
            
            public static string ShipCountry = @"ShipCountry";
            
            public static string CompanyName = @"CompanyName";
            
            public static string Address = @"Address";
            
            public static string City = @"City";
            
            public static string Region = @"Region";
            
            public static string PostalCode = @"PostalCode";
            
            public static string Country = @"Country";
            
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
