/*
 * SubSonic - http://subsonicproject.com
 * 
 * The contents of this file are subject to the Mozilla Public
 * License Version 1.1 (the "License"); you may not use this file
 * except in compliance with the License. You may obtain a copy of
 * the License at http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an 
 * "AS IS" basis, WITHOUT WARRANTY OF ANY KIND, either express or
 * implied. See the License for the specific language governing
 * rights and limitations under the License.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Web.Configuration;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the DataProviderCollection class
    /// </summary>
    public class DataProviderCollection : ProviderCollection
    {
        private static readonly object _lockProvider = new object();

        /// <summary>
        /// Gets the <see cref="SubSonic.DataProvider"/> with the specified name.
        /// </summary>
        /// <value></value>
        public new DataProvider this[string name]
        {
            get { return (DataProvider)base[name]; }
        }

        /// <summary>
        /// Adds a provider to the collection.
        /// </summary>
        /// <param name="provider">The provider to be added.</param>
        /// <exception cref="T:System.NotSupportedException">The collection is read-only.</exception>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="provider"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">The <see cref="P:System.Configuration.Provider.ProviderBase.Name"/> of <paramref name="provider"/> is null.- or -The length of the <see cref="P:System.Configuration.Provider.ProviderBase.Name"/> of <paramref name="provider"/> is less than 1.</exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
        /// </PermissionSet>
        public override void Add(ProviderBase provider)
        {
            if(provider == null)
                throw new ArgumentNullException("provider");

            if(!(provider is DataProvider))
                throw new ArgumentException("Invalid provider type", "provider");

            if(base[provider.Name] == null)
            {
                lock(_lockProvider)
                {
                    if(base[provider.Name] == null)
                        base.Add(provider);
                }
            }
        }
    }

    /// <summary>
    /// Summary for the DataProvider class
    /// </summary>
    public abstract class DataProvider : ProviderBase
    {
        #region props

        private readonly object _lockSchemaCollection = new object();
        private string _databaseVersion;
        private string _defaultConnectionString;
        private string additionalNamespaces = String.Empty;
        private string appendWith = String.Empty;
        private string connectionStringName;
        private string excludeProcedureList = String.Empty;
		private string excludeTableList = String.Empty;
		private string enumIncludeList = String.Empty;
		private string enumExcludeList = String.Empty;
		private bool enumShowDebugInfo = false;
		private bool extractClassNameFromSPName;
        private bool fixDatabaseObjectCasing = true;
        private bool fixPluralClassNames = true;
        private string generatedNamespace = "SubSonic.Generated";
        private bool generateLazyLoads;
        private bool generateNullableProperties = true;
        private bool generateODSControllers = true;
        private bool generateRelatedTablesAsProperties;
        private string includeProcedureList = "*";
        private string includeTableList = "*";
        private string manyToManySuffix = "_Map";
        private string regexDictionaryReplace = String.Empty;
        private bool regexIgnoreCase;
        private string regexMatchExpression = String.Empty;
        private string regexReplaceExpression = String.Empty;
        private string relatedTableLoadPrefix = String.Empty;
        private bool removeUnderscores = true;
        internal SortedList<string, TableSchema.Table> schemaCollection = new SortedList<string, TableSchema.Table>();
        private bool setPropertyDefaultsFromDatabase;
        private string spBaseClass = "StoredProcedure";
        private string spClassName = ClassName.STORED_PROCEDURES;
        private string spStartsWith = String.Empty;
        private string stripColumnText = String.Empty;
        private string stripParamText = String.Empty;
        private string stripSPText = String.Empty;
        private string stripTableText = String.Empty;
        private string stripViewText = String.Empty;
        private string tableBaseClass = "ActiveRecord";
        private bool useExtendedProperties;
        private bool useSPs = true;
        private bool useUtc;
        private string viewBaseClass = "ReadOnlyRecord";
        private string viewStartsWith = String.Empty;
        protected bool dbRequiresBracketedJoins = false; 
        protected bool dbAllowsMultipleStatement = true;
        protected bool dbAllowsSpOutputParam = true;
        protected bool dbSupportsITransactionLocal = true;
        protected bool dbSupportsInlineComments = true;

        /// <summary>
        /// Gets the type of the named provider.
        /// </summary>
        /// <value>The type of the named provider.</value>
        public abstract string NamedProviderType { get; }

        /// <summary>
        /// Gets or sets the table base class.
        /// </summary>
        /// <value>The table base class.</value>
        public string TableBaseClass
        {
            get { return tableBaseClass; }
            set { tableBaseClass = value; }
        }

        /// <summary>
        /// Gets or sets the view base class.
        /// </summary>
        /// <value>The view base class.</value>
        public string ViewBaseClass
        {
            get { return viewBaseClass; }
            set { viewBaseClass = value; }
        }

        /// <summary>
        /// Gets or sets the stored procedure base class.
        /// </summary>
        /// <value>The stored procedure base class.</value>
        public string StoredProcedureBaseClass
        {
            get { return spBaseClass; }
            set { spBaseClass = value; }
        }

        /// <summary>
        /// Gets or sets the view names.
        /// </summary>
        /// <value>The view names.</value>
        protected string[] ViewNames { get; set; }

        /// <summary>
        /// Gets or sets the table names.
        /// </summary>
        /// <value>The table names.</value>
        protected string[] TableNames { get; set; }

        /// <summary>
        /// Gets or sets the default connection string.
        /// </summary>
        /// <value>The default connection string.</value>
        public string DefaultConnectionString
        {
            get
            {
                if(String.IsNullOrEmpty(_defaultConnectionString) && !String.IsNullOrEmpty(connectionStringName))
                {
                    try
                    {
                        //_defaultConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                        _defaultConnectionString = WebConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                    }
                    catch
                    {
                        //swallow the exception, since the connection string will be set later possibly
                        _defaultConnectionString = "NOT SET";
                    }
                }
                return _defaultConnectionString;
            }
            set { _defaultConnectionString = value; }
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <value>The database version.</value>
        public string DatabaseVersion
        {
            get
            {
                if(String.IsNullOrEmpty(_databaseVersion))
                    _databaseVersion = GetDatabaseVersion(Name);

                return _databaseVersion;
            }
        }

        /// <summary>
        /// A comma-separated list of additional namespaces that will be included in the "using" section of generated classes
        /// </summary>
        /// <value>The list of namespaces, i.e. My.Namespace1,My.Namespace2</value>
        public string[] AdditionalNamespaces
        {
            get
            {
                string[] result = new string[0];

                if(!String.IsNullOrEmpty(additionalNamespaces))
                    result = additionalNamespaces.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the default value to append to SubSonic generated objects when the default transformations
        /// would result in a keyword conflicts.
        /// </summary>
        /// <value>The string that will be appended</value>
        public string AppendWith
        {
            get { return appendWith; }
            set { appendWith = value; }
        }

        /// <summary>
        /// Gets or sets the name of the connection string.
        /// </summary>
        /// <value>The name of the connection string.</value>
        public string ConnectionStringName
        {
            get { return connectionStringName; }
            set { connectionStringName = value; }
        }

        /// <summary>
        /// Gets the exclude procedures.
        /// </summary>
        /// <value>The exclude procedures.</value>
        public string[] ExcludeProcedures
        {
            get
            {
                string[] result = new string[0];
                if(!String.IsNullOrEmpty(excludeProcedureList))
                    result = excludeProcedureList.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                return result;
            }
        }

        /// <summary>
        /// Gets the exclude tables.
        /// </summary>
        /// <value>The exclude tables.</value>
        public string[] ExcludeTables
        {
            get
            {
                string[] result = new string[0];
                if(!String.IsNullOrEmpty(excludeTableList))
                    result = excludeTableList.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                return result;
            }
        }

       /// <summary>
        /// Gets the enum include list.
        /// </summary>
        /// <value>The enum include tables.</value>
		public string[] EnumIncludeList
        {
            get
            {
                string[] result = new string[0];
				if (!String.IsNullOrEmpty(enumIncludeList))
					result = enumIncludeList.Split(new[] { ',' }, StringSplitOptions.None);

                return result;
            }
        }

       /// <summary>
		/// Gets the enum exclude list.
        /// </summary>
        /// <value>The enum exclude tables.</value>
		public string[] EnumExcludeList
        {
            get
            {
                string[] result = new string[0];
				if (!String.IsNullOrEmpty(enumExcludeList))
					result = enumExcludeList.Split(new[] { ',' }, StringSplitOptions.None);

                return result;
            }
        }

		public bool EnumShowDebugInfo {
			get {
				return enumShowDebugInfo;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [extract class name from SP name].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [extract class name from SP name]; otherwise, <c>false</c>.
        /// </value>
        public bool ExtractClassNameFromSPName
        {
            get { return extractClassNameFromSPName; }
            set { extractClassNameFromSPName = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [fix plural class names].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [fix plural class names]; otherwise, <c>false</c>.
        /// </value>
        public bool FixPluralClassNames
        {
            get { return fixPluralClassNames; }
            set { fixPluralClassNames = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [fix database object casing].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [fix database object casing]; otherwise, <c>false</c>.
        /// </value>
        public bool FixDatabaseObjectCasing
        {
            get { return fixDatabaseObjectCasing; }
            set { fixDatabaseObjectCasing = value; }
        }

        /// <summary>
        /// Gets or sets the generated namespace.
        /// </summary>
        /// <value>The generated namespace.</value>
        public string GeneratedNamespace
        {
            get { return generatedNamespace; }
            set { generatedNamespace = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [generate lazy loads].
        /// </summary>
        /// <value><c>true</c> if [generate lazy loads]; otherwise, <c>false</c>.</value>
        public bool GenerateLazyLoads
        {
            get { return generateLazyLoads; }
            set { generateLazyLoads = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [generate nullable properties].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [generate nullable properties]; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateNullableProperties
        {
            get { return generateNullableProperties; }
            set { generateNullableProperties = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [generate ODS controllers].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [generate ODS controllers]; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateODSControllers
        {
            get { return generateODSControllers; }
            set { generateODSControllers = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [generate related tables as properties].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [generate related tables as properties]; otherwise, <c>false</c>.
        /// </value>
        public bool GenerateRelatedTablesAsProperties
        {
            get { return generateRelatedTablesAsProperties; }
            set { generateRelatedTablesAsProperties = value; }
        }

        /// <summary>
        /// Gets the include procedures.
        /// </summary>
        /// <value>The include procedures.</value>
        public string[] IncludeProcedures
        {
            get
            {
                string[] result = new string[0];
                if(!String.IsNullOrEmpty(includeProcedureList))
                    result = includeProcedureList.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                return result;
            }
        }

        /// <summary>
        /// Gets the include tables.
        /// </summary>
        /// <value>The include tables.</value>
        public string[] IncludeTables
        {
            get
            {
                string[] result = new string[0];
                if(!String.IsNullOrEmpty(includeTableList))
                    result = includeTableList.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

                return result;
            }
        }

        /// <summary>
        /// Gets or sets the many to many suffix.
        /// </summary>
        /// <value>The many to many suffix.</value>
        public string ManyToManySuffix
        {
            get { return manyToManySuffix; }
            set { manyToManySuffix = value; }
        }

        /// <summary>
        /// Gets or sets the regex dictionary replace.
        /// </summary>
        /// <value>The regex dictionary replace.</value>
        public string RegexDictionaryReplace
        {
            get { return regexDictionaryReplace; }
            set { regexDictionaryReplace = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [regex ignore case].
        /// </summary>
        /// <value><c>true</c> if [regex ignore case]; otherwise, <c>false</c>.</value>
        public bool RegexIgnoreCase
        {
            get { return regexIgnoreCase; }
            set { regexIgnoreCase = value; }
        }

        /// <summary>
        /// Gets or sets the regex match expression.
        /// </summary>
        /// <value>The regex match expression.</value>
        public string RegexMatchExpression
        {
            get { return regexMatchExpression; }
            set { regexMatchExpression = value; }
        }

        /// <summary>
        /// Gets or sets the regex replace expression.
        /// </summary>
        /// <value>The regex replace expression.</value>
        public string RegexReplaceExpression
        {
            get { return regexReplaceExpression; }
            set { regexReplaceExpression = value; }
        }

        /// <summary>
        /// Gets or sets the related table load prefix.
        /// </summary>
        /// <value>The related table load prefix.</value>
        public string RelatedTableLoadPrefix
        {
            get { return relatedTableLoadPrefix; }
            set { relatedTableLoadPrefix = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [remove underscores].
        /// </summary>
        /// <value><c>true</c> if [remove underscores]; otherwise, <c>false</c>.</value>
        public bool RemoveUnderscores
        {
            get { return removeUnderscores; }
            set { removeUnderscores = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set property defaults from database].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [set property defaults from database]; otherwise, <c>false</c>.
        /// </value>
        public bool SetPropertyDefaultsFromDatabase
        {
            get { return setPropertyDefaultsFromDatabase; }
            set { setPropertyDefaultsFromDatabase = value; }
        }

        /// <summary>
        /// Gets or sets the name of the SP class.
        /// </summary>
        /// <value>The name of the SP class.</value>
        public string SPClassName
        {
            get { return spClassName; }
            set { spClassName = value; }
        }

        /// <summary>
        /// Gets or sets the SP starts with.
        /// </summary>
        /// <value>The SP starts with.</value>
        public string SPStartsWith
        {
            get { return spStartsWith; }
            set { spStartsWith = value; }
        }

        /// <summary>
        /// Gets or sets the strip column text.
        /// </summary>
        /// <value>The strip column text.</value>
        public string StripColumnText
        {
            get { return stripColumnText; }
            set { stripColumnText = value; }
        }

        /// <summary>
        /// Gets or sets the strip param text.
        /// </summary>
        /// <value>The strip param text.</value>
        public string StripParamText
        {
            get { return stripParamText; }
            set { stripParamText = value; }
        }

        /// <summary>
        /// Gets or sets the strip SP text.
        /// </summary>
        /// <value>The strip SP text.</value>
        public string StripSPText
        {
            get { return stripSPText; }
            set { stripSPText = value; }
        }

        /// <summary>
        /// Gets or sets the strip view text.
        /// </summary>
        /// <value>The strip view text.</value>
        public string StripViewText
        {
            get { return stripViewText; }
            set { stripViewText = value; }
        }

        /// <summary>
        /// Gets or sets the strip table text.
        /// </summary>
        /// <value>The strip table text.</value>
        public string StripTableText
        {
            get { return stripTableText; }
            set { stripTableText = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use extended properties].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use extended properties]; otherwise, <c>false</c>.
        /// </value>
        public bool UseExtendedProperties
        {
            get { return useExtendedProperties; }
            set { useExtendedProperties = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use S ps].
        /// </summary>
        /// <value><c>true</c> if [use S ps]; otherwise, <c>false</c>.</value>
        public bool UseSPs
        {
            get { return useSPs; }
            set { useSPs = value; }
        }

        /// <summary>
        /// Gets or sets the view starts with.
        /// </summary>
        /// <value>The view starts with.</value>
        public string ViewStartsWith
        {
            get { return viewStartsWith; }
            set { viewStartsWith = value; }
        }

        /// <summary>
        /// Sets the default connection string.
        /// </summary>
        /// <param name="defaultConnectionString">The default connection string.</param>
        public void SetDefaultConnectionString(string defaultConnectionString)
        {
            _defaultConnectionString = defaultConnectionString;
        }

        /// <summary>
        /// Adds the schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="schema">The schema.</param>
        public void AddSchema(string tableName, TableSchema.Table schema)
        {
            if(!schemaCollection.ContainsKey(tableName))
            {
                lock(_lockSchemaCollection)
                {
                    if(!schemaCollection.ContainsKey(tableName))
                        schemaCollection.Add(tableName, schema);
                }
            }
        }

        /// <summary>
        /// Flag indicating if the RDBMS requires each join to be individually bracketed.
        /// </summary>
        public bool DatabaseRequiresBracketedJoins
        {
            get { return dbRequiresBracketedJoins; }
		}
		
        /// Flag indicating if the RDBMS supports multi-statement batching.
        /// </summary>
        /// <value>The database version.</value>
        public bool DatabaseAllowsMultipleStatement
        {
            get { return dbAllowsMultipleStatement; }
        }

        /// <summary>
        /// Flag indicating if the RDBMS supports output parameters for stored procedures.
        /// </summary>
        public bool DatabaseAllowsSpOutputParameters
        {
            get { return dbAllowsSpOutputParam; }
        }

        /// <summary>
        /// Flag indicating if the RDBMS requires each join to be individually bracketed.
        /// </summary>
        public bool DatabaseSupportsInlineComments
        {
            get { return dbSupportsInlineComments; }
        }

        /// <summary>
        /// Flag indicating if the RDBMS supports the ITransactionLocal interface.
         /// </summary>
        public bool DatabaseSupportsITransactionLocal
        {
            get { return dbSupportsITransactionLocal; }
        }

        #endregion


        [ThreadStatic]
        private static DbConnection __sharedConnection;

        /// <summary>
        /// Gets or sets the current shared connection.
        /// </summary>
        /// <value>The current shared connection.</value>
        public DbConnection CurrentSharedConnection
        {
            get { return __sharedConnection; }

            protected set
            {
                if(value == null)
                {
                    __sharedConnection.Dispose();
                    __sharedConnection = null;
                }
                else
                {
                    __sharedConnection = value;
                    __sharedConnection.Disposed += __sharedConnection_Disposed;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether [current connection string is default].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [current connection string is default]; otherwise, <c>false</c>.
        /// </value>
        public bool CurrentConnectionStringIsDefault
        {
            get
            {
                if(CurrentSharedConnection != null)
                {
                    if(CurrentSharedConnection.ConnectionString != DefaultConnectionString)
                        return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [use regex replace].
        /// </summary>
        /// <value><c>true</c> if [use regex replace]; otherwise, <c>false</c>.</value>
        public bool UseRegexReplace
        {
            get { return !String.IsNullOrEmpty(RegexMatchExpression) || !String.IsNullOrEmpty(RegexDictionaryReplace); }
        }

        /// <summary>
        /// Gets a value indicating whether [use UTC timestamps].
        /// </summary>
        /// <value><c>true</c> if [use UTC timestamps]; otherwise, <c>false</c>.</value>
        public bool UseUtc
        {
            get { return useUtc; }
            set { useUtc = value; }
        }

        /// <summary>
        /// Gets a DateTime object that is set to the current date and time on this computer,
        /// expressed either as the Coordinated Universal Time (UTC) when <see cref="UseUtc"/> = true,
        /// or the local time when <see cref="UseUtc"/> = false.
        /// </summary>
        /// <value><c>DateTime.UtcNow</c> if <see cref="UseUtc"/> = true; otherwise, <c>DateTime.Now</c>.</value>
        public DateTime Now
        {
            get { return UseUtc ? DateTime.UtcNow : DateTime.Now; }
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        protected abstract string GetDatabaseVersion(string providerName);

        /// <summary>
        /// Returns a SQL Wildcard character
        /// </summary>
        /// <returns>System.String</returns>
        public virtual string GetWildCard()
        {
            return "%";
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public abstract IDataReader GetReader(QueryCommand cmd);

        /// <summary>
        /// Gets the single record reader.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public abstract IDataReader GetSingleRecordReader(QueryCommand cmd);

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
		public virtual DataSet GetDataSet(QueryCommand cmd)
        {
            return GetDataSet<DataSet>(cmd);
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public abstract T GetDataSet<T>(QueryCommand qry) where T : DataSet, new();

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public abstract object ExecuteScalar(QueryCommand cmd);

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <returns></returns>
        public abstract int ExecuteQuery(QueryCommand cmd);

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <returns></returns>
        public abstract TableSchema.Table GetTableSchema(string tableName, TableType tableType);

        /// <summary>
        /// Gets the SP list.
        /// </summary>
        /// <returns></returns>
        public abstract string[] GetSPList();

        /// <summary>
        /// Gets the SP list.
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetSPList(bool includeSchema) {
            return GetSPList();
        }

        /// <summary>
        /// Gets the table name list.
        /// </summary>
        /// <returns></returns>
        public abstract string[] GetTableNameList();

        /// <summary>
        /// Gets the view name list.
        /// </summary>
        /// <returns></returns>
        public abstract string[] GetViewNameList();

        /// <summary>
        /// Gets the SP params.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        public abstract IDataReader GetSPParams(string spName);

        /// <summary>
        /// Gets the foreign key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public abstract string[] GetForeignKeyTables(string tableName);

        /// <summary>
        /// Gets the table name by primary key.
        /// </summary>
        /// <param name="pkName">Name of the pk.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public abstract string GetTableNameByPrimaryKey(string pkName, string providerName);

        /// <summary>
        /// Gets the primary key table names.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public abstract ArrayList GetPrimaryKeyTableNames(string tableName);

        /// <summary>
        /// Gets the primary key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public abstract TableSchema.Table[] GetPrimaryKeyTables(string tableName);

        /// <summary>
        /// Gets the name of the foreign key table.
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <returns></returns>
        public abstract string GetForeignKeyTableName(string fkColumnName);

        /// <summary>
        /// Gets the name of the foreign key table.
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public abstract string GetForeignKeyTableName(string fkColumnName, string tableName);

        /// <summary>
        /// Gets the foreign key table.
        /// </summary>
        /// <param name="fkColumn">The fk column.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public TableSchema.Table GetForeignKeyTable(TableSchema.TableColumn fkColumn, TableSchema.Table table)
        {
            string fkName = GetForeignKeyTableName(fkColumn.ColumnName, table.Name);
            if(!String.IsNullOrEmpty(fkName))
                return DataService.GetSchema(fkName, Name, table.TableType);

            return null;
        }

        /// <summary>
        /// Gets the type of the db.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns></returns>
        public abstract DbType GetDbType(string dataType);

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public abstract IDbCommand GetCommand(QueryCommand qry);

        /// <summary>
        /// Gets the db command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public virtual DbCommand GetDbCommand(QueryCommand qry)
        {
            AutomaticConnectionScope conn = new AutomaticConnectionScope(this);

            DbCommand cmd = conn.Connection.CreateCommand();
            cmd.CommandText = qry.CommandSql;
            cmd.CommandType = qry.CommandType;

            foreach(QueryParameter par in qry.Parameters)
            {
                DbParameter newParam = cmd.CreateParameter();
                newParam.Direction = par.Mode;
                newParam.ParameterName = par.ParameterName;
                newParam.Value = par.ParameterValue;
                newParam.Size = par.Size;
                newParam.DbType = par.DataType;
                cmd.Parameters.Add(newParam);
            }

            return cmd;
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns></returns>
        public abstract DbConnection CreateConnection();

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public abstract DbConnection CreateConnection(string connectionString);

        /// <summary>
        /// Handles the Disposed event of the __sharedConnection control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void __sharedConnection_Disposed(object sender, EventArgs e)
        {
            __sharedConnection = null;
        }

        /// <summary>
        /// Initializes the shared connection.
        /// </summary>
        /// <returns></returns>
        internal DbConnection InitializeSharedConnection()
        {
            if(CurrentSharedConnection == null)
                CurrentSharedConnection = CreateConnection();

            return CurrentSharedConnection;
        }

        /// <summary>
        /// Initializes the shared connection.
        /// </summary>
        /// <param name="sharedConnectionString">The shared connection string.</param>
        /// <returns></returns>
        internal DbConnection InitializeSharedConnection(string sharedConnectionString)
        {
            if(CurrentSharedConnection == null)
                CurrentSharedConnection = CreateConnection(sharedConnectionString);

            return CurrentSharedConnection;
        }

        /// <summary>
        /// Resets the shared connection.
        /// </summary>
        internal void ResetSharedConnection()
        {
            CurrentSharedConnection = null;
        }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public virtual int GetRecordCount(Query qry)
        {
            string select = String.Concat(SqlFragment.SELECT, "COUNT(1) FROM ");
            if(qry.IsDistinct && !String.IsNullOrEmpty(qry.SelectList))
            {
                string[] distinct = qry.SelectList.Split(new[] {','});
                select = String.Concat(SqlFragment.SELECT, "COUNT(DISTINCT ", distinct[0], ") FROM ");
            }

            QueryCommand qc = BuildSelectCommand(String.Concat(select, qry.Schema.QualifiedName, BuildWhere(qry)), qry);
            qc.CommandType = CommandType.Text;

            object obj = ExecuteScalar(qc);
            int returnVal = -1;

            if(obj != null)
            {
                string strValue = obj.ToString();
                int.TryParse(strValue, out returnVal);
            }
            return returnVal;
        }

        /// <summary>
        /// Builds the select command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public QueryCommand BuildSelectCommand(Query qry)
        {
            //get the SQL
            string sql = GetSql(qry);
            return BuildSelectCommand(sql, qry);
        }

        /// <summary>
        /// Adds the where parameters.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="qry">The qry.</param>
        public static void AddWhereParameters(QueryCommand cmd, Query qry)
        {
            foreach(Where where in qry.wheres)
            {
                if(where.ParameterValue != null && where.ParameterValue != DBNull.Value)
                    cmd.AddParameter(where.ParameterName, where.ParameterValue, where.DbType);
            }

            foreach(BetweenAnd between in qry.betweens)
            {
                cmd.AddParameter(between.StartParameterName, between.StartDate, DbType.DateTime);
                cmd.AddParameter(between.EndParameterName, between.EndDate, DbType.DateTime);
            }
            if(qry.inList != null)
            {
                if(qry.inList.Length > 0)
                {
                    int inCount = 1;
                    foreach(object inItem in qry.inList)
                    {
                        cmd.AddParameter(String.Concat("in", inCount), inItem, DbType.AnsiString);
                        inCount++;
                    }
                }
            }
            if(qry.notInList != null)
            {
                if(qry.notInList.Length > 0)
                {
                    int notInCount = 1;
                    foreach(object notInItem in qry.notInList)
                    {
                        cmd.AddParameter(String.Concat("notIn", notInCount), notInItem, DbType.AnsiString);
                        notInCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Builds the select command.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public static QueryCommand BuildSelectCommand(string sql, Query qry)
        {
            QueryCommand cmd = new QueryCommand(sql, qry.ProviderName);
            AddWhereParameters(cmd, qry);

            return cmd;
        }

        /// <summary>
        /// Gets the update SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public string GetUpdateSql(Query qry)
        {
            return GetUpdateSql(qry, qry.Schema.Columns);
        }

        /// <summary>
        /// Gets the update SQL.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="dirtyColumns">The dirty columns.</param>
        /// <returns></returns>
        public string GetUpdateSql(Query query, TableSchema.TableColumnCollection dirtyColumns)
        {
            //split the TablNames and loop out the SQL
            TableSchema.Table table = query.Schema;
            StringBuilder updateSQL = new StringBuilder(SqlFragment.UPDATE);
            updateSQL.Append(table.QualifiedName);
            updateSQL.Append(SqlFragment.SET);

            StringBuilder cols = new StringBuilder();
            //int loopCount = 1;
            bool foundModifiedBy = false;
            bool foundModifiedOn = false;
            bool isFirstColumn = true;
            foreach(TableSchema.TableColumn col in dirtyColumns)
            {
                string colName = col.ColumnName;

                //don't want to change the created bits
                if(Utility.IsWritableColumn(col))
                {
                    if(!isFirstColumn)
                        cols.Append(", ");

                    isFirstColumn = false;

                    if(col.IsReservedColumn)
                    {
                        if(Utility.IsAuditField(col.ColumnName))
                        {
                            if (Utility.IsMatch(col.ColumnName, ReservedColumnName.MODIFIED_ON))
                                foundModifiedOn = true;
                            else if (Utility.IsMatch(col.ColumnName, ReservedColumnName.MODIFIED_BY))
                                foundModifiedBy = true;
                        }
                    }
                    cols.Append(Utility.MakeParameterAssignment(colName, colName, query.Provider));
                }
            }

            //if there are ModifiedOn and ModifiedBy, add them in as well
            if(table.Columns.Contains(ReservedColumnName.MODIFIED_ON) && !foundModifiedOn)
            {
                if(!isFirstColumn)
                    cols.Append(", ");

                cols.Append(Utility.MakeParameterAssignment(ReservedColumnName.MODIFIED_ON, ReservedColumnName.MODIFIED_ON, query.Provider));
                isFirstColumn = false;
            }

            if(table.Columns.Contains(ReservedColumnName.MODIFIED_BY) && !foundModifiedBy)
            {
                cols.Append(isFirstColumn ? String.Empty : ", ");
                cols.Append(Utility.MakeParameterAssignment(ReservedColumnName.MODIFIED_BY, ReservedColumnName.MODIFIED_BY, query.Provider));
            }

            //string returnSql;
            updateSQL.Append(cols);

            if(query.wheres.Count == 0)
            {
                // Thanks Jason!
                TableSchema.TableColumn[] keys = table.PrimaryKeys;
                updateSQL.Append(SqlFragment.WHERE);
                for(int i = 0; i < keys.Length; i++)
                {
                    updateSQL.Append(Utility.MakeParameterAssignment(keys[i].ColumnName, keys[i].ColumnName, query.Provider));
                    if(i + 1 != keys.Length)
                        updateSQL.Append(SqlFragment.AND);
                }

                return AdjustUpdateSql(query, table, updateSQL.ToString());
            }
            return updateSQL.Append(BuildWhere(query)).ToString();
        }

        /// <summary>
        /// Adjusts the update SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <param name="table">The table.</param>
        /// <param name="updateSql">The update SQL.</param>
        /// <returns></returns>
        protected virtual string AdjustUpdateSql(Query qry, TableSchema.Table table, string updateSql)
        {
            return String.Concat(updateSql, "; ", SqlFragment.SELECT, 
				FormatParameterNameForSQL(table.PrimaryKey.ParameterName), 
                SqlFragment.AS, "id");
        }

        /// <summary>
        /// Builds the update command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public QueryCommand BuildUpdateCommand(Query qry)
        {
            TableSchema.Table table = qry.Schema;
            if(qry.UpdateSettings == null)
                throw new InvalidOperationException("No update settings have been set. Use Query.AddUpdateSetting to add some in");

            StringBuilder sql = new StringBuilder(SqlFragment.UPDATE);
            sql.Append(qry.Provider.FormatIdentifier(table.Name));
            QueryCommand cmd = new QueryCommand(sql.ToString(), qry.ProviderName);

            //append the update statements
            IDictionaryEnumerator looper = qry.UpdateSettings.GetEnumerator();
            string setClause = SqlFragment.SET;
            bool isFirstColumn = true;
            while(looper.MoveNext())
            {
                TableSchema.TableColumn column = table.GetColumn(looper.Key.ToString());
                if(column != null)
                {
                    if(!isFirstColumn)
                        sql.Append(",");
                    else
                    {
                        isFirstColumn = false;
                        sql.Append(setClause);
                        setClause = String.Empty;
                    }
                    sql.Append(Utility.MakeParameterAssignment(looper.Key.ToString(), looper.Key.ToString(), qry.Provider));
                    cmd.AddParameter(looper.Key.ToString(), looper.Value, column.DataType);
                }
                else
                    throw new Exception("There is no column in " + table.Name + " called " + looper.Key);
            }

            sql.Append(BuildWhere(qry));
            AddWhereParameters(cmd, qry);
            cmd.CommandSql = sql.ToString();
            return cmd;
        }

        /// <summary>
        /// Builds the delete command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public QueryCommand BuildDeleteCommand(Query qry)
        {
            StringBuilder sql = new StringBuilder(SqlFragment.DELETE_FROM);
            sql.Append(qry.Schema.QualifiedName);
            QueryCommand cmd = new QueryCommand(sql.ToString(), qry.ProviderName);
            if(qry.UpdateSettings != null)
            {
                IDictionaryEnumerator looper = qry.UpdateSettings.GetEnumerator();
                while(looper.MoveNext())
                {
                    string key = looper.Key.ToString();
                    TableSchema.TableColumn column = qry.Schema.GetColumn(key);
                    if(column != null)
                        cmd.AddParameter(key, looper.Value, column.DataType);
                }
            }
            sql.Append(BuildWhere(qry));
            AddWhereParameters(cmd, qry);
            cmd.CommandSql = sql.ToString();
            return cmd;
        }

        /// <summary>
        /// Gets the delete SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public static string GetDeleteSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;
            StringBuilder sql = new StringBuilder(SqlFragment.DELETE_FROM);
            sql.Append(table.QualifiedName);
            if(qry.wheres.Count == 0)
            {
                // Thanks Jason!
                TableSchema.TableColumn[] keys = table.PrimaryKeys;
                for(int i = 0; i < keys.Length; i++)
                {
                    sql.Append(SqlFragment.WHERE);
                    sql.Append(Utility.MakeParameterAssignment(keys[i].ColumnName, keys[i].ColumnName, qry.Provider));
                    if(i + 1 != keys.Length)

                        sql.Append(SqlFragment.AND);
                }
            }
            else
                sql.Append(BuildWhere(qry));
            return sql.ToString();
        }

        /// <summary>
        /// Builds the where.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public static string BuildWhere(Query qry)
        {
            StringBuilder where = new StringBuilder();
            string whereOperator = SqlFragment.WHERE;
            bool isFirstPass = true;
            List<Where> removedWheres = new List<Where>();
            foreach(Where wWhere in qry.wheres)
            {
                if(wWhere.Comparison != Comparison.In && wWhere.Comparison != Comparison.NotIn)
                {
                    whereOperator = isFirstPass ? SqlFragment.WHERE : String.Concat(" ", Enum.GetName(typeof(Where.WhereCondition), wWhere.Condition), " ");
                    where.Append(whereOperator);

                    where.Append(qry.Provider.QualifyColumnName(qry.Schema.SchemaName, wWhere.TableName, wWhere.ColumnName));
                    where.Append(Where.GetComparisonOperator(wWhere.Comparison));

                    if (wWhere.ParameterValue != DBNull.Value && wWhere.ParameterValue != null)
                        where.Append(qry.Provider.FormatParameterNameForSQL(wWhere.ParameterName));
                    else
                        where.Append(" NULL");

                    isFirstPass = false;
                }
                else
                {
                    if(wWhere.Comparison == Comparison.In)
                        qry.inColumn = wWhere.ColumnName;
                    else
                        qry.notInColumn = wWhere.ColumnName;

                    object[] values;
                    IEnumerable enumerable = wWhere.ParameterValue as IEnumerable;

                    if(enumerable == null)
                        values = new[] {wWhere.ParameterValue};
                    else
                    {
                        List<object> objects = new List<object>();
                        IEnumerator enumer = enumerable.GetEnumerator();
                        while(enumer.MoveNext())
                            objects.Add(enumer.Current);
                        values = objects.ToArray();
                    }

                    if(wWhere.Comparison == Comparison.In)
                    {
                        qry.inList = values;
                        qry.inColumn = wWhere.ColumnName;
                    }
                    else
                    {
                        qry.notInList = values;
                        qry.notInColumn = wWhere.ColumnName;
                    }
                    removedWheres.Add(wWhere);
                }
            }

            foreach(Where removedWhere in removedWheres)
                qry.wheres.Remove(removedWhere);
            //isFirstPass = true;
            foreach(BetweenAnd between in qry.betweens)
            {
                if(qry.wheres.Count == 0 && isFirstPass)
                    whereOperator = SqlFragment.WHERE;
                else
                    whereOperator = isFirstPass ? SqlFragment.WHERE : String.Concat(" ", Enum.GetName(typeof(Where.WhereCondition), between.Condition), " ");

                where.Append(whereOperator);
                where.Append(qry.Provider.QualifyColumnName("", between.TableName, between.ColumnName));
                where.Append(SqlFragment.BETWEEN);
                where.Append(qry.Provider.FormatParameterNameForSQL(between.StartParameterName));
                where.Append(SqlFragment.AND);
                where.Append(qry.Provider.FormatParameterNameForSQL(between.EndParameterName));
                isFirstPass = false;
            }

            for(int i = qry.wheres.Count - 1; i >= 0; i--)
            {
                if(qry.wheres[i].ParameterValue == DBNull.Value)
                    qry.wheres.RemoveAt(i);
            }

            if(qry.inList != null && qry.inList.Length > 0)
            {
                if(isFirstPass)
                    where.Append(whereOperator);
                else
                    where.Append(SqlFragment.AND);

                where.Append(qry.Provider.FormatIdentifier(qry.inColumn));
                where.Append(SqlFragment.IN);
                where.Append("(");
                bool isFirst = true;

                for(int i = 1; i <= qry.inList.Length; i++)
                {
                    if(!isFirst)
                        where.Append(", ");
                    isFirst = false;

                    where.Append(qry.Provider.FormatParameterNameForSQL(String.Concat("in", i)));
                }
                where.Append(")");
            }

            if(qry.notInList != null && qry.notInList.Length > 0)
            {
                if(isFirstPass)
                    where.Append(whereOperator);
                else
                    where.Append(SqlFragment.AND);

                where.Append(qry.Provider.FormatIdentifier(qry.notInColumn));
                where.Append(SqlFragment.NOT_IN);
                where.Append("(");
                bool isFirst = true;

                for(int i = 1; i <= qry.notInList.Length; i++)
                {
                    if(!isFirst)
                        where.Append(", ");
                    isFirst = false;

                    where.Append(qry.Provider.FormatParameterNameForSQL(String.Concat("notIn", i)));
                }
                where.Append(")");
            }

            if(!String.IsNullOrEmpty(qry.LogicalDeleteColumn))
            {
                string fragment = (qry.wheres.Count > 0 || qry.betweens.Count > 0) ? SqlFragment.AND : SqlFragment.WHERE;
                where.Append(fragment);
                where.Append("(");
                where.Append(qry.LogicalDeleteColumn);
                where.Append(" IS NULL OR ");
                where.Append(qry.LogicalDeleteColumn);
                where.Append(" = 0)");
            }
            return where.ToString();
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="parameter">The parameter.</param>
        public abstract void SetParameter(IDataReader dataReader, StoredProcedure.Parameter parameter);

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        /// <returns></returns>
        public abstract string GetParameterPrefix();

        /// <summary>
        /// Format the Parameter for inclusion in SQL (eg. @param_name for MSSQL, [param_name] for MSAccess)
        /// Only use this in the final marshalling for SQL insertion, NOT in initial formatting of the parameter name.
        /// </summary>
        public virtual string FormatParameterNameForSQL(string parameterName) {
            string prefix = GetParameterPrefix();
            if (!parameterName.StartsWith(prefix))
                parameterName = prefix + parameterName.Replace(" ", String.Empty);
            return parameterName;
        }

        /// <summary>
        /// Format the Parameter name to ensure it is valid
        /// </summary>
        public virtual string PreformatParameterName(string parameterName) {
            return parameterName.Replace(" ", String.Empty);
        }

        /// <summary>
        /// Qualify table name according to RDBMS format (eg. '[owner].[table]')
        /// </summary>
        public virtual string QualifyTableName(string schemaName, string tableName) {
            return (string.IsNullOrEmpty(schemaName) ? "" : FormatIdentifier(schemaName) + ".") + FormatIdentifier(tableName);
        }

        /// <summary>
        /// Qualify column name according to RDBMS format (eg. '[owner].[table].[column]')
        /// </summary>
        public virtual string QualifyColumnName(string schemaName, string tableName, string columnName) {
            return (string.IsNullOrEmpty(schemaName) ? "" : FormatIdentifier(schemaName) + ".")
                + (string.IsNullOrEmpty(tableName) ? "" : FormatIdentifier(tableName) + ".")
                + FormatIdentifier(columnName);
        }
        
        /// <summary>
        /// Delimits the name of the db.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public virtual string FormatIdentifier(string columnName)
        {
            return columnName;
        }

        /// <summary>
        /// Filter out elements of the SQL for unit testing
        /// </summary>
        public virtual string FilterTestSQL(string sqlString) {
            return sqlString;
        }

        /// <summary>
        /// Executes the transaction.
        /// </summary>
        /// <param name="commands">The commands.</param>
        public abstract void ExecuteTransaction(QueryCommandCollection commands);

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public string GetSql(Query qry)
        {
            string result = String.Empty;
            switch(qry.QueryType)
            {
                case QueryType.Select:
                    result = GetSelectSql(qry);
                    break;
                case QueryType.Update:
                    result = GetUpdateSql(qry);
                    break;
                case QueryType.Insert:
                    result = GetInsertSql(qry);
                    break;
                case QueryType.Delete:
                    result = GetDeleteSql(qry);
                    break;
            }
            return result;
        }

        /// <summary>
        /// Gets the insert SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public abstract string GetInsertSql(Query qry);

        /// <summary>
        /// Gets the select SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public abstract string GetSelectSql(Query qry);

        /// <summary>
        /// Builds the command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public QueryCommand BuildCommand(Query qry)
        {
            QueryCommand cmd;
            switch(qry.QueryType)
            {
                case QueryType.Select:
                    cmd = BuildSelectCommand(qry);
                    break;
                case QueryType.Update:
                    cmd = BuildUpdateCommand(qry);
                    break;
                case QueryType.Insert:
                    cmd = null;
                    break;
                case QueryType.Delete:
                    cmd = BuildDeleteCommand(qry);
                    break;
                default:
                    cmd = null;
                    break;
            }
            return cmd;
        }

        /// <summary>
        /// Scripts the data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public string ScriptData(string tableName)
        {
            return ScriptData(tableName, DataService.Provider.Name);
        }

        /// <summary>
        /// Scripts the data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public abstract string ScriptData(string tableName, string providerName);

        //public abstract string ScriptSchema();

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="name">The friendly name of the provider.</param>
        /// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
        /// <exception cref="T:System.ArgumentNullException">The name of the provider is null.</exception>
        /// <exception cref="T:System.ArgumentException">The name of the provider has a length of zero.</exception>
        /// <exception cref="T:System.InvalidOperationException">An attempt is made to call <see cref="M:System.Configuration.Provider.ProviderBase.Initialize(System.String,System.Collections.Specialized.NameValueCollection)"/> on a provider after the provider has already been initialized.</exception>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);
            connectionStringName = config[ConfigurationPropertyName.CONNECTION_STRING_NAME];

            additionalNamespaces = Utility.StripWhitespace(additionalNamespaces);
            excludeProcedureList = Utility.StripWhitespace(excludeProcedureList);
            excludeTableList = Utility.StripWhitespace(excludeTableList);
            includeProcedureList = Utility.StripWhitespace(includeProcedureList);
            includeTableList = Utility.StripWhitespace(includeTableList);

            ApplyConfig(config, ref appendWith, ConfigurationPropertyName.APPEND_WITH);
            ApplyConfig(config, ref additionalNamespaces, ConfigurationPropertyName.ADDITIONAL_NAMESPACES);
            ApplyConfig(config, ref connectionStringName, ConfigurationPropertyName.CONNECTION_STRING_NAME);
            ApplyConfig(config, ref excludeProcedureList, ConfigurationPropertyName.EXCLUDE_PROCEDURE_LIST);
            ApplyConfig(config, ref excludeTableList, ConfigurationPropertyName.EXCLUDE_TABLE_LIST);
			ApplyConfig(config, ref enumIncludeList, ConfigurationPropertyName.ENUM_INCLUDE_LIST);
			ApplyConfig(config, ref enumExcludeList, ConfigurationPropertyName.ENUM_EXCLUDE_LIST);
			ApplyConfig(config, ref enumShowDebugInfo, ConfigurationPropertyName.ENUM_SHOW_DEBUG_INFO);
			ApplyConfig(config, ref extractClassNameFromSPName, ConfigurationPropertyName.EXTRACT_CLASS_NAME_FROM_SP_NAME);
            ApplyConfig(config, ref fixDatabaseObjectCasing, ConfigurationPropertyName.FIX_DATABASE_OBJECT_CASING);
            ApplyConfig(config, ref fixPluralClassNames, ConfigurationPropertyName.FIX_PLURAL_CLASS_NAMES);
            ApplyConfig(config, ref generateLazyLoads, ConfigurationPropertyName.GENERATE_LAZY_LOADS);
            ApplyConfig(config, ref generateNullableProperties, ConfigurationPropertyName.GENERATE_NULLABLE_PROPERTIES);
            ApplyConfig(config, ref generateODSControllers, ConfigurationPropertyName.GENERATE_ODS_CONTROLLERS);
            ApplyConfig(config, ref generateRelatedTablesAsProperties, ConfigurationPropertyName.GENERATE_RELATED_TABLES_AS_PROPERTIES);
            ApplyConfig(config, ref generatedNamespace, ConfigurationPropertyName.GENERATED_NAMESPACE);
            ApplyConfig(config, ref includeProcedureList, ConfigurationPropertyName.INCLUDE_PROCEDURE_LIST);
            ApplyConfig(config, ref includeTableList, ConfigurationPropertyName.INCLUDE_TABLE_LIST);
            ApplyConfig(config, ref manyToManySuffix, ConfigurationPropertyName.MANY_TO_MANY_SUFFIX);
            ApplyConfig(config, ref regexDictionaryReplace, ConfigurationPropertyName.REGEX_DICTIONARY_REPLACE);
            ApplyConfig(config, ref regexIgnoreCase, ConfigurationPropertyName.REGEX_IGNORE_CASE);
            ApplyConfig(config, ref regexMatchExpression, ConfigurationPropertyName.REGEX_MATCH_EXPRESSION);
            ApplyConfig(config, ref regexReplaceExpression, ConfigurationPropertyName.REGEX_REPLACE_EXPRESSION);
            ApplyConfig(config, ref relatedTableLoadPrefix, ConfigurationPropertyName.RELATED_TABLE_LOAD_PREFIX);
            ApplyConfig(config, ref removeUnderscores, ConfigurationPropertyName.REMOVE_UNDERSCORES);
            ApplyConfig(config, ref setPropertyDefaultsFromDatabase, ConfigurationPropertyName.SET_PROPERTY_DEFAULTS_FROM_DATABASE);
            ApplyConfig(config, ref spClassName, ConfigurationPropertyName.STORED_PROCEDURE_CLASS_NAME);
            ApplyConfig(config, ref spStartsWith, ConfigurationPropertyName.SP_STARTS_WITH);
            ApplyConfig(config, ref stripColumnText, ConfigurationPropertyName.STRIP_COLUMN_TEXT);
            ApplyConfig(config, ref stripParamText, ConfigurationPropertyName.STRIP_PARAM_TEXT);
            ApplyConfig(config, ref stripSPText, ConfigurationPropertyName.STRIP_STORED_PROCEDURE_TEXT);
            ApplyConfig(config, ref stripTableText, ConfigurationPropertyName.STRIP_TABLE_TEXT);
            ApplyConfig(config, ref stripViewText, ConfigurationPropertyName.STRIP_VIEW_TEXT);
            ApplyConfig(config, ref useExtendedProperties, ConfigurationPropertyName.USE_EXTENDED_PROPERTIES);
            ApplyConfig(config, ref useSPs, ConfigurationPropertyName.USE_STORED_PROCEDURES);
            ApplyConfig(config, ref useUtc, ConfigurationPropertyName.USE_UTC_TIMES);
            ApplyConfig(config, ref viewStartsWith, ConfigurationPropertyName.VIEW_STARTS_WITH);
            ApplyConfig(config, ref tableBaseClass, ConfigurationPropertyName.TABLE_BASE_CLASS);
            ApplyConfig(config, ref viewBaseClass, ConfigurationPropertyName.VIEW_BASE_CLASS);
            ApplyConfig(config, ref spBaseClass, ConfigurationPropertyName.STORED_PROCEDURE_BASE_CLASS);
        }

        /// <summary>
        /// Applies the config.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="configName">Name of the config.</param>
        private static void ApplyConfig(NameValueCollection config, ref string parameterValue, string configName)
        {
            if(config[configName] != null)
                parameterValue = config[configName];
        }

        /// <summary>
        /// Applies the config.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="parameterValue">if set to <c>true</c> [parameter value].</param>
        /// <param name="configName">Name of the config.</param>
        private static void ApplyConfig(NameValueCollection config, ref bool parameterValue, string configName)
        {
            if(config[configName] != null)
                parameterValue = Convert.ToBoolean(config[configName]);
        }

        /// <summary>
        /// DataSet helper class used to get the names of the tables in the DataSet.
        /// </summary>
        /// <param name="ds">The DataSet to get the names for.</param>
        /// <returns>The names of the tables in the DataSet.</returns>
        protected static string[] GetTableNames(DataSet ds)
        {
            int tblCount = ds.Tables.Count;
            string[] tableNames = new string[tblCount];

            for(int iTable = 0; iTable < tblCount; iTable++)
                tableNames[iTable] = ds.Tables[iTable].TableName;

            return tableNames;
        }

        /// <summary>
        /// DataSet helper class used to add table mappings to a DataAdapter.
        /// </summary>
        /// <remarks>
        /// TableMappings keep the table names during a Fill operation.
        /// </remarks>
        /// <param name="da">The DataAdapter to add the mappings to.</param>
        /// <param name="ds">The DataSet containing the tables to map.</param>
        protected static void AddTableMappings(DataAdapter da, DataSet ds)
        {
            const string rootName = "Table";
            string[] tableNames = GetTableNames(ds);

            for(int i = 0; i < tableNames.Length; i++)
            {
                string newName = (i == 0) ? rootName : rootName + i;
                da.TableMappings.Add(newName, tableNames[i]);
            }
        }


        #region SQL Generation Support

        /// <summary>
        /// Gets the SQL generator.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns></returns>
        public virtual ISqlGenerator GetSqlGenerator(SqlQuery sqlQuery)
        {
            return new ANSISqlGenerator(sqlQuery);
        }

        #endregion


        #region Migration Support

        /// <summary>
        /// Force-reloads a provider's schema
        /// </summary>
        public abstract void ReloadSchema();

        #endregion
    }
}