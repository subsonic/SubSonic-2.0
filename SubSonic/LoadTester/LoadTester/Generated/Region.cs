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
	/// Strongly-typed collection for the Region class.
	/// </summary>
    [Serializable]
	public partial class RegionCollection : ActiveList<Region, RegionCollection>
	{	   
		public RegionCollection() {}
        
        /// <summary>
		/// Filters an existing collection based on the set criteria. This is an in-memory filter
		/// Thanks to developingchris for this!
        /// </summary>
        /// <returns>RegionCollection</returns>
		public RegionCollection Filter()
        {
            for (int i = this.Count - 1; i > -1; i--)
            {
                Region o = this[i];
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
	/// This is an ActiveRecord class which wraps the Region table.
	/// </summary>
	[Serializable]
	public partial class Region : ActiveRecord<Region>, IActiveRecord
	{
		#region .ctors and Default Settings
		
		public Region()
		{
		  SetSQLProps();
		  InitSetDefaults();
		  MarkNew();
		}
		
		private void InitSetDefaults() { SetDefaults(); }
		
		public Region(bool useDatabaseDefaults)
		{
			SetSQLProps();
			if(useDatabaseDefaults)
				ForceDefaults();
			MarkNew();
		}
        
		public Region(object keyID)
		{
			SetSQLProps();
			InitSetDefaults();
			LoadByKey(keyID);
		}
		 
		public Region(string columnName, object columnValue)
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
				TableSchema.Table schema = new TableSchema.Table("Region", TableType.Table, DataService.GetInstance("Northwind"));
				schema.Columns = new TableSchema.TableColumnCollection();
				schema.SchemaName = @"dbo";
				//columns
				
				TableSchema.TableColumn colvarRegionID = new TableSchema.TableColumn(schema);
				colvarRegionID.ColumnName = "RegionID";
				colvarRegionID.DataType = DbType.Int32;
				colvarRegionID.MaxLength = 0;
				colvarRegionID.AutoIncrement = true;
				colvarRegionID.IsNullable = false;
				colvarRegionID.IsPrimaryKey = true;
				colvarRegionID.IsForeignKey = false;
				colvarRegionID.IsReadOnly = false;
				colvarRegionID.DefaultSetting = @"";
				colvarRegionID.ForeignKeyTableName = "";
				schema.Columns.Add(colvarRegionID);
				
				TableSchema.TableColumn colvarRegionDescription = new TableSchema.TableColumn(schema);
				colvarRegionDescription.ColumnName = "RegionDescription";
				colvarRegionDescription.DataType = DbType.String;
				colvarRegionDescription.MaxLength = 50;
				colvarRegionDescription.AutoIncrement = false;
				colvarRegionDescription.IsNullable = false;
				colvarRegionDescription.IsPrimaryKey = false;
				colvarRegionDescription.IsForeignKey = false;
				colvarRegionDescription.IsReadOnly = false;
				colvarRegionDescription.DefaultSetting = @"";
				colvarRegionDescription.ForeignKeyTableName = "";
				schema.Columns.Add(colvarRegionDescription);
				
				BaseSchema = schema;
				//add this schema to the provider
				//so we can query it later
				DataService.Providers["Northwind"].AddSchema("Region",schema);
			}
		}
		#endregion
		
		#region Props
		  
		[XmlAttribute("RegionID")]
		[Bindable(true)]
		public int RegionID 
		{
			get { return GetColumnValue<int>(Columns.RegionID); }
			set { SetColumnValue(Columns.RegionID, value); }
		}
		  
		[XmlAttribute("RegionDescription")]
		[Bindable(true)]
		public string RegionDescription 
		{
			get { return GetColumnValue<string>(Columns.RegionDescription); }
			set { SetColumnValue(Columns.RegionDescription, value); }
		}
		
		#endregion
		
		
		#region PrimaryKey Methods		
		
        protected override void SetPrimaryKey(object oValue)
        {
            base.SetPrimaryKey(oValue);
            
            SetPKValues();
        }
        
		
		public Northwind.TerritoryCollection Territories()
		{
			return new Northwind.TerritoryCollection().Where(Territory.Columns.RegionID, RegionID).Load();
		}
		#endregion
		
			
		
		//no foreign key tables defined (0)
		
		
		
		//no ManyToMany tables defined (0)
		
        
        
		#region ObjectDataSource support
		
		
		/// <summary>
		/// Inserts a record, can be used with the Object Data Source
		/// </summary>
		public static void Insert(string varRegionDescription)
		{
			Region item = new Region();
			
			item.RegionDescription = varRegionDescription;
			
		
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		
		/// <summary>
		/// Updates a record, can be used with the Object Data Source
		/// </summary>
		public static void Update(int varRegionID,string varRegionDescription)
		{
			Region item = new Region();
			
				item.RegionID = varRegionID;
			
				item.RegionDescription = varRegionDescription;
			
			item.IsNew = false;
			if (System.Web.HttpContext.Current != null)
				item.Save(System.Web.HttpContext.Current.User.Identity.Name);
			else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name);
		}
		#endregion
        
        
        
        #region Typed Columns
        
        
        public static TableSchema.TableColumn RegionIDColumn
        {
            get { return Schema.Columns[0]; }
        }
        
        
        
        public static TableSchema.TableColumn RegionDescriptionColumn
        {
            get { return Schema.Columns[1]; }
        }
        
        
        
        #endregion
		#region Columns Struct
		public struct Columns
		{
			 public static string RegionID = @"RegionID";
			 public static string RegionDescription = @"RegionDescription";
						
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
