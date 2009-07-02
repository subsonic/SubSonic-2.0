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
	/// Strongly-typed collection for the Territory class.
	/// </summary>
    [Serializable]
	public partial class TerritoryCollection : ActiveList<Territory, TerritoryCollection>
	{	   
		public TerritoryCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>TerritoryCollection</returns>
		public TerritoryCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Territory o = this[i];
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
	/// This is an ActiveRecord class which wraps the Territories table.
	/// </summary>
	[Serializable]
	public partial class Territory : ActiveRecord<Territory>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Territory()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Territory(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Territory(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Territory(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Territories", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarTerritoryID = new TableSchema.TableColumn(schema);
				colvarTerritoryID.ColumnName = "TerritoryID";
				colvarTerritoryID.DataType = DbType.String;
				colvarTerritoryID.MaxLength = 20;
				colvarTerritoryID.AutoIncrement = false;
				colvarTerritoryID.IsNullable = false;
				colvarTerritoryID.IsPrimaryKey = true;
				colvarTerritoryID.IsForeignKey = false;
				colvarTerritoryID.IsReadOnly = false;
				colvarTerritoryID.DefaultSetting = @"";
				colvarTerritoryID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarTerritoryID);
				
				TableSchema.TableColumn colvarTerritoryDescription = new TableSchema.TableColumn(schema);
				colvarTerritoryDescription.ColumnName = "TerritoryDescription";
				colvarTerritoryDescription.DataType = DbType.String;
				colvarTerritoryDescription.MaxLength = 50;
				colvarTerritoryDescription.AutoIncrement = false;
				colvarTerritoryDescription.IsNullable = false;
				colvarTerritoryDescription.IsPrimaryKey = false;
				colvarTerritoryDescription.IsForeignKey = false;
				colvarTerritoryDescription.IsReadOnly = false;
				colvarTerritoryDescription.DefaultSetting = @"";
				colvarTerritoryDescription.ForeignKeyTableName = "";
				schema.Columns.Add(colvarTerritoryDescription);
				
				TableSchema.TableColumn colvarRegionID = new TableSchema.TableColumn(schema);
				colvarRegionID.ColumnName = "RegionID";
				colvarRegionID.DataType = DbType.Int32;
				colvarRegionID.MaxLength = 0;
				colvarRegionID.AutoIncrement = false;
				colvarRegionID.IsNullable = false;
				colvarRegionID.IsPrimaryKey = false;
				colvarRegionID.IsForeignKey = true;
				colvarRegionID.IsReadOnly = false;
				colvarRegionID.DefaultSetting = @"";
				
					colvarRegionID.ForeignKeyTableName = "Region";
				schema.Columns.Add(colvarRegionID);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("Territories",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("TerritoryID")]
		[Bindable(true)]
		public string TerritoryID 
		{
			get { return GetColumnValue<string>(Columns.TerritoryID); }
			set { SetColumnValue(Columns.TerritoryID, value); }
		}
		  
		[XmlAttribute("TerritoryDescription")]
		[Bindable(true)]
		public string TerritoryDescription 
		{
			get { return GetColumnValue<string>(Columns.TerritoryDescription); }
			set { SetColumnValue(Columns.TerritoryDescription, value); }
		}
		  
		[XmlAttribute("RegionID")]
		[Bindable(true)]
		public int RegionID 
		{
			get { return GetColumnValue<int>(Columns.RegionID); }
			set { SetColumnValue(Columns.RegionID, value); }
		}
		
		#endregion
		
		
		#region PrimaryKey Methods		
		
        protected override void SetPrimaryKey(object oValue)
        {
            base.SetPrimaryKey(oValue);
            
            SetPKValues();
        }
        
		
		public Northwind.EmployeeTerritoryCollection EmployeeTerritories()
		{
			return new Northwind.EmployeeTerritoryCollection().Where(EmployeeTerritory.Columns.TerritoryID, TerritoryID).Load();
		}
		#endregion
		
			
		
		#region ForeignKey Properties
		
		/// <summary>
		/// Returns a Region ActiveRecord object related to this Territory
		/// 
		/// </summary>
		public Northwind.Region Region
		{
			get { return Northwind.Region.FetchByID(this.RegionID); }
			set { SetColumnValue("RegionID", value.RegionID); }
		}
		
		
		#endregion
		
		
		
		#region Many To Many Helpers
		
		 
		public Northwind.EmployeeCollection GetEmployeeCollection() { return Territory.GetEmployeeCollection(this.TerritoryID); }
		public static Northwind.EmployeeCollection GetEmployeeCollection(string varTerritoryID)
		{
		    SubSonic.QueryCommand cmd = new SubSonic.QueryCommand("SELECT * FROM [dbo].[Employees] INNER JOIN [EmployeeTerritories] ON [Employees].[EmployeeID] = [EmployeeTerritories].[EmployeeID] WHERE [EmployeeTerritories].[TerritoryID] = @TerritoryID", Territory.Schema.Provider.Name);
			cmd.AddParameter("@TerritoryID", varTerritoryID, DbType.String);
			IDataReader rdr = SubSonic.DataService.GetReader(cmd);
			EmployeeCollection coll = new EmployeeCollection();
			coll.LoadAndCloseReader(rdr);
			return coll;
		}
		
		public static void SaveEmployeeMap(string varTerritoryID, EmployeeCollection items)
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [EmployeeTerritories] WHERE [EmployeeTerritories].[TerritoryID] = @TerritoryID", Territory.Schema.Provider.Name);
			cmdDel.AddParameter("@TerritoryID", varTerritoryID, DbType.String);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (Employee item in items)
			{
				EmployeeTerritory varEmployeeTerritory = new EmployeeTerritory();
				varEmployeeTerritory.SetColumnValue("TerritoryID", varTerritoryID);
				varEmployeeTerritory.SetColumnValue("EmployeeID", item.GetPrimaryKeyValue());
				varEmployeeTerritory.Save();
			}
		}
		public static void SaveEmployeeMap(string varTerritoryID, System.Web.UI.WebControls.ListItemCollection itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [EmployeeTerritories] WHERE [EmployeeTerritories].[TerritoryID] = @TerritoryID", Territory.Schema.Provider.Name);
			cmdDel.AddParameter("@TerritoryID", varTerritoryID, DbType.String);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (System.Web.UI.WebControls.ListItem l in itemList) 
			{
				if (l.Selected) 
				{
					EmployeeTerritory varEmployeeTerritory = new EmployeeTerritory();
					varEmployeeTerritory.SetColumnValue("TerritoryID", varTerritoryID);
					varEmployeeTerritory.SetColumnValue("EmployeeID", l.Value);
					varEmployeeTerritory.Save();
				}
			}
		}
		public static void SaveEmployeeMap(string varTerritoryID , int[] itemList) 
		{
			QueryCommandCollection coll = new SubSonic.QueryCommandCollection();
			//delete out the existing
			 QueryCommand cmdDel = new QueryCommand("DELETE FROM [EmployeeTerritories] WHERE [EmployeeTerritories].[TerritoryID] = @TerritoryID", Territory.Schema.Provider.Name);
			cmdDel.AddParameter("@TerritoryID", varTerritoryID, DbType.String);
			coll.Add(cmdDel);
			DataService.ExecuteTransaction(coll);
			foreach (int item in itemList) 
			{
				EmployeeTerritory varEmployeeTerritory = new EmployeeTerritory();
				varEmployeeTerritory.SetColumnValue("TerritoryID", varTerritoryID);
				varEmployeeTerritory.SetColumnValue("EmployeeID", item);
				varEmployeeTerritory.Save();
			}
		}
		
		public static void DeleteEmployeeMap(string varTerritoryID) 
		{
			QueryCommand cmdDel = new QueryCommand("DELETE FROM [EmployeeTerritories] WHERE [EmployeeTerritories].[TerritoryID] = @TerritoryID", Territory.Schema.Provider.Name);
			cmdDel.AddParameter("@TerritoryID", varTerritoryID, DbType.String);
			DataService.ExecuteQuery(cmdDel);
		}
		
		#endregion
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varTerritoryID,string varTerritoryDescription,int varRegionID)
		{
			Territory item = new Territory();
			
			item.TerritoryID = varTerritoryID;
			
			item.TerritoryDescription = varTerritoryDescription;
			
			item.RegionID = varRegionID;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(string varTerritoryID,string varTerritoryDescription,int varRegionID)
		{
			Territory item = new Territory();
			
				item.TerritoryID = varTerritoryID;
			
				item.TerritoryDescription = varTerritoryDescription;
			
				item.RegionID = varRegionID;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn TerritoryIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn TerritoryDescriptionColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        public static TableSchema.TableColumn RegionIDColumn
        {
            get { return Schema.Columns[2]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string TerritoryID = @"TerritoryID";
			 public static string TerritoryDescription = @"TerritoryDescription";
			 public static string RegionID = @"RegionID";
						
		}
		#endregion
		
		#region Update PK Collections
		
        public void SetPKValues()
        {
}
        #endregion
    
        #region Deep Save
		
        public void DeepSave()
        {
            Save();
            
}
        #endregion
	}
}
