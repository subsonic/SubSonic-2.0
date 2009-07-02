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
namespace Northwind
{
	/// <summary>
	/// Strongly-typed collection for the EmployeeTerritory class.
	/// </summary>
    [Serializable]
	public partial class EmployeeTerritoryCollection : ActiveList<EmployeeTerritory, EmployeeTerritoryCollection>
	{	   
		public EmployeeTerritoryCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>EmployeeTerritoryCollection</returns>
		public EmployeeTerritoryCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                EmployeeTerritory o = this[i];
                foreach (SubSonic.Where w in this.wheres)
                {
                    bool remove = false;
                    System.Reflection.PropertyInfo pi = o.GetType().GetProperty(w.ColumnName);
                    if (pi.CanRead)
                    {
                        object val = pi.GetValue(o, null);
                        switch (w.Comparison)
                        {
                            case SubSonic.Comparison.Equals:
                                if (!val.Equals(w.ParameterValue))
                                {
                                    remove = true;
                                }
                                break;
                        }
                    }
                    if (remove)
                    {
                        this.Remove(o);
                        break;
                    }
                }
            }
            return this;
        }
		
		
	}
	/// <summary>
	/// This is an ActiveRecord class which wraps the EmployeeTerritories table.
	/// </summary>
	[Serializable]
	public partial class EmployeeTerritory : ActiveRecord<EmployeeTerritory>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public EmployeeTerritory()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public EmployeeTerritory(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public EmployeeTerritory(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public EmployeeTerritory(string columnName, object columnValue)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByParam(columnName,columnValue);
		}
		
		protected static void SetSQLProps() { GetTableSchema(); }
		
		#endregion
		
		#region Schema and Query Accessor	
		public static Query CreateQuery() { return new Query(Schema); }
		public static TableSchema.Table Schema
		{
			get
			{
				if (BaseSchema == null)
					SetSQLProps();
				return BaseSchema;
			}
		}
		
		private static void GetTableSchema() 
		{
			if(!IsSchemaInitialized)
			{
				//Schema declaration
				TableSchema.Table schema = new TableSchema.Table("EmployeeTerritories", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarEmployeeID = new TableSchema.TableColumn(schema);
				colvarEmployeeID.ColumnName = "EmployeeID";
				colvarEmployeeID.DataType = DbType.Int32;
				colvarEmployeeID.MaxLength = 0;
				colvarEmployeeID.AutoIncrement = false;
				colvarEmployeeID.IsNullable = false;
				colvarEmployeeID.IsPrimaryKey = true;
				colvarEmployeeID.IsForeignKey = true;
				colvarEmployeeID.IsReadOnly = false;
				colvarEmployeeID.DefaultSetting = @"";
				
					colvarEmployeeID.ForeignKeyTableName = "Employees";
				schema.Columns.Add(colvarEmployeeID);
				
				TableSchema.TableColumn colvarTerritoryID = new TableSchema.TableColumn(schema);
				colvarTerritoryID.ColumnName = "TerritoryID";
				colvarTerritoryID.DataType = DbType.String;
				colvarTerritoryID.MaxLength = 20;
				colvarTerritoryID.AutoIncrement = false;
				colvarTerritoryID.IsNullable = false;
				colvarTerritoryID.IsPrimaryKey = true;
				colvarTerritoryID.IsForeignKey = true;
				colvarTerritoryID.IsReadOnly = false;
				colvarTerritoryID.DefaultSetting = @"";
				
					colvarTerritoryID.ForeignKeyTableName = "Territories";
				schema.Columns.Add(colvarTerritoryID);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("EmployeeTerritories",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("EmployeeID")]
		[Bindable(true)]
		public int EmployeeID 
		{
			get { return GetColumnValue<int>(Columns.EmployeeID); }
			set { SetColumnValue(Columns.EmployeeID, value); }
		}
		  
		[XmlAttribute("TerritoryID")]
		[Bindable(true)]
		public string TerritoryID 
		{
			get { return GetColumnValue<string>(Columns.TerritoryID); }
			set { SetColumnValue(Columns.TerritoryID, value); }
		}
		
		#endregion
		
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a Employee ActiveRecord object related to this EmployeeTerritory
		/// 
		/// </summary>
		public Northwind.Employee Employee
		{
			get { return Northwind.Employee.FetchByID(this.EmployeeID); }
			set { SetColumnValue("EmployeeID", value.EmployeeID); }
		}
		
		
		/// <summary>
		/// Returns a Territory ActiveRecord object related to this EmployeeTerritory
		/// 
		/// </summary>
		public Northwind.Territory Territory
		{
			get { return Northwind.Territory.FetchByID(this.TerritoryID); }
			set { SetColumnValue("TerritoryID", value.TerritoryID); }
		}
		
		
		#endregion
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(int varEmployeeID,string varTerritoryID)
		{
			EmployeeTerritory item = new EmployeeTerritory();
			
			item.EmployeeID = varEmployeeID;
			
			item.TerritoryID = varTerritoryID;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varEmployeeID,string varTerritoryID)
		{
			EmployeeTerritory item = new EmployeeTerritory();
			
				item.EmployeeID = varEmployeeID;
			
				item.TerritoryID = varTerritoryID;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn EmployeeIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn TerritoryIDColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string EmployeeID = @"EmployeeID";
			 public static string TerritoryID = @"TerritoryID";
						
		}
		#endregion
		
		#region Update PK Collections
		
        #endregion
    
        #region Deep Save
		
        #endregion
	}
}
