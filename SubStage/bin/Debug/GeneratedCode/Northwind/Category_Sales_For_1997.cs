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
    /// Strongly-typed collection for the Category_Sales_For_1997 class.
    /// </summary>
    [Serializable]
    public partial class Category_Sales_For_1997Collection : ReadOnlyList<Category_Sales_For_1997, Category_Sales_For_1997Collection>
    {        
        public Category_Sales_For_1997Collection() {}
    }
    /// <summary>
    /// This is  Read-only wrapper class for the Category Sales for 1997 view.
    /// </summary>
    [Serializable]
    public partial class Category_Sales_For_1997 : ReadOnlyRecord<Category_Sales_For_1997>, IReadOnlyRecord
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
                TableSchema.Table schema = new TableSchema.Table("Category Sales for 1997", TableType.View, DataService.GetInstance("Northwind"));
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
                
                TableSchema.TableColumn colvarCategorySales = new TableSchema.TableColumn(schema);
                colvarCategorySales.ColumnName = "CategorySales";
                colvarCategorySales.DataType = DbType.Currency;
                colvarCategorySales.MaxLength = 0;
                colvarCategorySales.AutoIncrement = false;
                colvarCategorySales.IsNullable = true;
                colvarCategorySales.IsPrimaryKey = false;
                colvarCategorySales.IsForeignKey = false;
                colvarCategorySales.IsReadOnly = false;
                
                schema.Columns.Add(colvarCategorySales);
                
                
                BaseSchema = schema;
                //add this schema to the provider
                //so we can query it later
                DataService.Providers["Northwind"].AddSchema("Category Sales for 1997",schema);
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
	    public Category_Sales_For_1997()
	    {
            SetSQLProps();
            SetDefaults();
            MarkNew();
        }
        public Category_Sales_For_1997(bool useDatabaseDefaults)
	    {
		    SetSQLProps();
		    if(useDatabaseDefaults)
		    {
				ForceDefaults();
			}
			MarkNew();
	    }
	    
	    public Category_Sales_For_1997(object keyID)
	    {
		    SetSQLProps();
		    LoadByKey(keyID);
	    }
    	 
	    public Category_Sales_For_1997(string columnName, object columnValue)
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
	      
        [XmlAttribute("CategorySales")]
        [Bindable(true)]
        public decimal? CategorySales 
	    {
		    get
		    {
			    return GetColumnValue<decimal?>("CategorySales");
		    }
            set 
		    {
			    SetColumnValue("CategorySales", value);
            }
        }
	    
	    #endregion
    
	    #region Columns Struct
	    public struct Columns
	    {
		    
		    
            public static string CategoryName = @"CategoryName";
            
            public static string CategorySales = @"CategorySales";
            
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
