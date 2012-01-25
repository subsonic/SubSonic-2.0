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
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Web.Configuration;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the DataService class
    /// </summary>
    public static class DataService
    {
        /// <summary>
        /// 
        /// </summary>
        public static SubSonicSection ConfigSectionSettings;


        #region Provider-specific bits

        private static readonly object _lock = new object();
        private static DataProviderCollection _providers;
        private static DataProvider defaultProvider;
        private static bool enableTrace;
        private static SubSonicSection section;

        /// <summary>
        /// Gets or sets a value indicating whether [enable trace].
        /// </summary>
        /// <value><c>true</c> if [enable trace]; otherwise, <c>false</c>.</value>
        public static bool EnableTrace
        {
            get { return enableTrace; }
            set { enableTrace = value; }
        }

        /// <summary>
        /// Gets the provider count.
        /// </summary>
        /// <value>The provider count.</value>
        public static int ProviderCount
        {
            get
            {
                if(_providers != null)
                    return _providers.Count;
                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the config section.
        /// </summary>
        /// <value>The config section.</value>
        public static SubSonicSection ConfigSection
        {
            get { return section; }
            set { section = value; }
        }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public static DataProvider Provider
        {
            get
            {
                if(defaultProvider == null)
                    LoadProviders();

                return defaultProvider;
            }
            set { defaultProvider = value; }
        }

        /// <summary>
        /// Gets or sets the providers.
        /// </summary>
        /// <value>The providers.</value>
        public static DataProviderCollection Providers
        {
            get
            {
                if(_providers == null)
                    LoadProviders();

                return _providers;
            }
            set { _providers = value; }
        }

        /// <summary>
        /// Gets the namespace.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string GetNamespace(string providerName)
        {
            return Providers[providerName].GeneratedNamespace;
        }

        /// <summary>
        /// Gets the provider names.
        /// </summary>
        /// <returns></returns>
        public static string[] GetProviderNames()
        {
            if(Providers != null)
            {
                int providerCount = Providers.Count;
                string[] providerNames = new string[providerCount];

                int i = 0;
                foreach(DataProvider provider in Providers)
                {
                    providerNames[i] = provider.Name;
                    i++;
                }
                return providerNames;
            }
            return new string[] {};
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static DataProvider GetInstance(string providerName)
        {
            //ensure load
            LoadProviders();

            //ensure it's instanced
            if(String.IsNullOrEmpty(providerName) || String.IsNullOrEmpty(providerName.Trim()))
                return defaultProvider;

            DataProvider provider = _providers[providerName];
            if(provider != null)
                return provider;

            throw new ArgumentException("No provider is defined with the name " + providerName, "providerName");
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        private static DataProvider GetInstance()
        {
            return GetInstance(null);
        }

        /// <summary>
        /// Loads the providers.
        /// </summary>
        public static void LoadProviders()
        {
            // Avoid claiming lock if providers are already loaded
            if(defaultProvider == null)
            {
                lock(_lock)
                {
                    // Do this again to make sure DefaultProvider is still null
                    if(defaultProvider == null)
                    {
                        //we allow for passing in a configuration section
                        //check to see if one's been passed in
                        if(section == null)
                        {
                            section = ConfigSectionSettings ?? (SubSonicSection)ConfigurationManager.GetSection(ConfigurationSectionName.SUB_SONIC_SERVICE);

                            //if it's still null, throw an exception
                            if(section == null)
                                throw new ConfigurationErrorsException("Can't find the SubSonicService section of the application config file");
                        }
                        //set the builder's template directory
                        CodeService.TemplateDirectory = section.TemplateDirectory;

                        enableTrace = Convert.ToBoolean(section.EnableTrace);
                        // Load registered providers and point DefaultProvider
                        // to the default provider
                        _providers = new DataProviderCollection();
                        ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(DataProvider));

                        defaultProvider = _providers[section.DefaultProvider];

                        if(defaultProvider == null && _providers.Count > 0)
                        {
                            IEnumerator enumer = _providers.GetEnumerator();
                            enumer.MoveNext();
                            defaultProvider = (DataProvider)enumer.Current;
                            if(defaultProvider == null)
                                throw new ProviderException("No providers could be located in the SubSonicService section of the application config file.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Resets the databases.
        /// </summary>
        public static void ResetDatabases()
        {
            /*
            //add in the providers, and their databases
            Databases = new DBCollection();
            if(_providers != null)
            {
                foreach(DataProvider prov in _providers)
                {
                    DB db = new DB(prov.Name);
                    db.ConnectionString = prov.ConnectionString;
                    db.ProviderName = prov.Name;
                    db.Provider = prov;
                    Databases.Add(db);
                }
            }
             * */
        }

        /// <summary>
        /// Adds the provider.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public static void AddProvider(DataProvider provider)
        {
            if(_providers == null)
                _providers = new DataProviderCollection();
            _providers.Add(provider);
        }

        #endregion


        #region SQL Generation Support

        /// <summary>
        /// Gets the generator.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static ISqlGenerator GetGenerator(string providerName)
        {
            DataProvider provider = GetInstance(providerName);
            return GetGenerator(provider);
        }

        /// <summary>
        /// Gets a SQL Generator specific to provider specified by providerName.
        /// </summary>
        /// <param name="provider">A provider instance.</param>
        /// <returns>An ISQLGenerator instance.</returns>
        public static ISqlGenerator GetGenerator(DataProvider provider)
        {
            return GetGenerator(provider, null);
        }

        /// <summary>
        /// Gets a SQL Generator specific to provider specified by providerName.
        /// </summary>
        /// <param name="provider">A provider instance.</param>
        /// <param name="sqlQuery">A SqlQuery instance.</param>
        /// <returns>An ISQLGenerator instance.</returns>
        public static ISqlGenerator GetGenerator(DataProvider provider, SqlQuery sqlQuery)
        {
            ISqlGenerator generator;
            switch(provider.NamedProviderType)
            {
                case DataProviderTypeName.SQL_SERVER:
                    if(Utility.IsSql2005(provider))
                        generator = new Sql2005Generator(sqlQuery);
                    else if(Utility.IsSql2008(provider))
                        generator = new Sql2008Generator(sqlQuery);
                    else
                        generator = new Sql2000Generator(sqlQuery);
                    break;
                case DataProviderTypeName.MY_SQL:
                    generator = new MySqlGenerator(sqlQuery);
                    break;
                case DataProviderTypeName.ORACLE:
                    generator = new OracleGenerator(sqlQuery);
                    break;
                case DataProviderTypeName.SQLITE:
                    generator = new SqlLiteGenerator(sqlQuery);
                    break;
                case DataProviderTypeName.SQL_CE:
                    generator = new SqlCEGenerator(sqlQuery);
                    break;
				case DataProviderTypeName.MSACCESS:
					generator = new MSJetGenerator(sqlQuery);
					break;
                default:
                    generator = new ANSISqlGenerator(sqlQuery);
                    break;
            }
            return generator;
        }

        #endregion


        #region Scripting

        /// <summary>
        /// Scripts the table data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public static string ScriptTableData(string tableName)
        {
            return GetInstance().ScriptData(tableName);
        }

        /// <summary>
        /// Scripts the table data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string ScriptTableData(string tableName, string providerName)
        {
            return GetInstance(providerName).ScriptData(tableName);
        }

        /// <summary>
        /// Scripts the data.
        /// </summary>
        /// <returns></returns>
        public static string ScriptData()
        {
            return ScriptData(String.Empty);
        }

        /// <summary>
        /// Scripts the data.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string ScriptData(string providerName)
        {
            string[] tables = GetTableNames(providerName);

            StringBuilder sb = new StringBuilder();

            foreach(string table in tables)
            {
                sb.Append(GetInstance(providerName).ScriptData(table, providerName));
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Scripts the data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string ScriptData(string tableName, string providerName)
        {
            return GetInstance(providerName).ScriptData(tableName, providerName);
        }

        #endregion


        #region Database Interaction

        /// <summary>
        /// Gets the SP schema collection.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static List<StoredProcedure> GetSPSchemaCollection(string providerName)
        {
            List<StoredProcedure> _sps = new List<StoredProcedure>();
            string[] sps = GetSPList(providerName, true);

            DataProvider provider = Providers[providerName];
            {
                Utility.WriteTrace(String.Format("Generating Stored Procedure collection for {0}", providerName));
                int generatedSprocs = 0;
                foreach(string s in sps)
                {
                    string spName = s;
                    string spSchemaName = "";
                    int i = s.IndexOf('.');
                    if (i >= 0) {
                        spName = s.Substring(i + 1);
                        spSchemaName = s.Substring(0, i);
                    }
                    if (CodeService.ShouldGenerate(spName, provider.IncludeProcedures, provider.ExcludeProcedures, provider))
                    {

                        //declare the sp
                        StoredProcedure sp = new StoredProcedure(spName, provider, spSchemaName);

                        //get the params
                        using (IDataReader rdr = GetSPParams(spName, providerName))
                        {
                            while(rdr.Read())
                            {
                                StoredProcedure.Parameter par = new StoredProcedure.Parameter();

                                provider.SetParameter(rdr, par);
                                par.QueryParameter = provider.FormatParameterNameForSQL(par.Name);
                                par.DisplayName = Utility.GetParameterName(par.Name, provider);
                                sp.Parameters.Add(par);
                            }
                            rdr.Close();
                        }
                        _sps.Add(sp);
                        generatedSprocs++;
                    }
                }
                Utility.WriteTrace(String.Format("Finished! {0} of {1} procedures generated.", generatedSprocs, sps.GetLength(0)));
            }
            return _sps;
        }

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public static string GetSql(Query qry)
        {
            return GetInstance(qry.ProviderName).GetSql(qry);
        }

        /// <summary>
        /// Builds the command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public static QueryCommand BuildCommand(Query qry)
        {
            QueryCommand cmd = GetInstance(qry.ProviderName).BuildCommand(qry);
            cmd.ProviderName = qry.ProviderName;
            cmd.Provider = qry.Provider;
            cmd.CommandTimeout = qry.CommandTimeout;
            return cmd;
        }

        /// <summary>
        /// Executes a transaction of the passed-in commands. 
        /// </summary>
        /// <param name="commands">The commands.</param>
        public static void ExecuteTransaction(QueryCommandCollection commands)
        {
            if(commands == null || commands.Count == 0)
                return;
            commands[0].Provider.ExecuteTransaction(commands);
            //GetInstance().ExecuteTransaction(commands);
        }

        /// <summary>
        /// Executes a transaction of the passed-in commands. 
        /// </summary>
        /// <param name="commands">The commands.</param>
        /// <param name="providerName">Name of the provider.</param>
        public static void ExecuteTransaction(QueryCommandCollection commands, string providerName)
        {
            GetInstance(providerName).ExecuteTransaction(commands);
        }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public static int GetRecordCount(Query qry)
        {
            return GetInstance(qry.ProviderName).GetRecordCount(qry);
        }

        /// <summary>
        /// Returns an IDataReader using the passed-in command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>IDataReader</returns>
        public static IDataReader GetReader(QueryCommand cmd)
        {
            return GetInstance(cmd.ProviderName).GetReader(cmd);
        }

        /// <summary>
        /// Returns a single row optimized IDataReader using the passed-in command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns>IDataReader</returns>
        public static IDataReader GetSingleRecordReader(QueryCommand cmd)
        {
            return GetInstance(cmd.ProviderName).GetSingleRecordReader(cmd);
        }

        /// <summary>
        /// Returns a DataSet based on the passed-in command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(QueryCommand cmd)
        {
            return GetInstance(cmd.ProviderName).GetDataSet(cmd);
        }

        /// <summary>
        /// Returns a DataSet based on the passed-in command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static T GetDataSet<T>(QueryCommand cmd) where T : DataSet, new()
        {
            return GetInstance(cmd.ProviderName).GetDataSet<T>(cmd);
        }

        /// <summary>
        /// Returns a scalar object based on the passed-in command
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public static object ExecuteScalar(QueryCommand cmd)
        {
            return GetInstance(cmd.ProviderName).ExecuteScalar(cmd);
        }

        /// <summary>
        /// Executes a pass-through query on the DB
        /// </summary>
        /// <param name="cmd"></param>
        public static int ExecuteQuery(QueryCommand cmd)
        {
            return GetInstance(cmd.ProviderName).ExecuteQuery(cmd);
        }

        #endregion


        #region Schema

        /// <summary>
        /// Clears the schema cache.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        public static void ClearSchemaCache(string providerName)
        {
            DataProvider provider = GetInstance(providerName);
            provider.schemaCollection.Clear();
            provider.ReloadSchema();
        }

        //public static TableSchema.Table GetSchema(string tableName, string providerName)
        //{
        //    string[] views = GetViewNames(providerName);
        //    foreach (string view in views)
        //    {
        //        if (Utilities.Utility.IsMatch(view, tableName))
        //        {
        //            return GetSchema(tableName, providerName, TableType.View);
        //        }
        //    }
        //    return GetSchema(tableName, providerName, TableType.Table);
        //}

        /// <summary>
        /// Reverse-compat overload; this defaults to Tables. if you need to specify a view, use the 
        /// other method call.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static TableSchema.Table GetSchema(string tableName, string providerName)
        {
            return GetSchema(tableName, providerName, TableType.Table);
        }

        /// <summary>
        /// Tables the exists.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public static bool TableExists(string providerName, string tableName)
        {
            string[] tables = GetTableNames(providerName);
            bool result = false;
            foreach(string tbl in tables)
            {
                if(Utility.IsMatch(tbl, tableName))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <returns></returns>
        public static TableSchema.Table GetSchema(string tableName, string providerName, TableType tableType)
        {
            TableSchema.Table result;

            //the schema could be held in memory
            //if it is, use that; else go to the DB

            DataProvider provider = GetInstance(providerName);

            if(provider.schemaCollection.ContainsKey(tableName))
                result = provider.schemaCollection[tableName];
            else
            {
                //look it up
                //when done, add it to the collection
                result = provider.GetTableSchema(tableName, tableType);
                if(result != null)
                    provider.AddSchema(tableName, result);
            }
            return result;
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static TableSchema.Table GetTableSchema(string tableName, string providerName)
        {
            return GetSchema(tableName, providerName, TableType.Table) ?? GetSchema(tableName, providerName, TableType.View);
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <returns></returns>
        public static TableSchema.Table GetTableSchema(string tableName, string providerName, TableType tableType)
        {
            return GetSchema(tableName, providerName, tableType);
        }

        /// <summary>
        /// Gets the table names.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string[] GetTableNames(string providerName)
        {
            return GetInstance(providerName).GetTableNameList();
        }

        /// <summary>
        /// Gets the ordered table names.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string[] GetOrderedTableNames(string providerName)
        {
            List<string> addedTables = new List<string>();
            TableSchema.Table[] tbls = GetTables(providerName);
            for(int i = 0; i < tbls.Length; i++)
            {
                int fkCount = tbls[i].ForeignKeys.Count;
                if(fkCount == 0)
                    addedTables.Add(tbls[i].TableName);
                else
                {
                    List<string> fkTables = new List<string>();
                    foreach(TableSchema.TableColumn col in tbls[i].Columns)
                    {
                        if(col.IsForeignKey)
                            fkTables.Add(col.ForeignKeyTableName);
                    }

                    bool allDependenciesAdded = true;
                    foreach(string fkTableName in fkTables)
                    {
                        if(!addedTables.Contains(fkTableName) && !Utility.IsMatch(fkTableName, tbls[i].TableName))
                        {
                            allDependenciesAdded = false;
                            break;
                        }
                    }

                    if(allDependenciesAdded && !addedTables.Contains(tbls[i].TableName))
                        addedTables.Add(tbls[i].TableName);
                }
            }

            for(int i = 0; i < tbls.Length; i++)
            {
                if(tbls[i].PrimaryKeys.Length < 2 && !addedTables.Contains(tbls[i].TableName))
                    addedTables.Add(tbls[i].TableName);
            }

            for(int i = 0; i < tbls.Length; i++)
            {
                if(tbls[i].PrimaryKeys.Length > 1 && !addedTables.Contains(tbls[i].TableName))
                    addedTables.Add(tbls[i].TableName);
            }

            return addedTables.ToArray();
        }

        /// <summary>
        /// Gets the tables.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static TableSchema.Table[] GetTables(string providerName)
        {
            string[] tableNames = GetTableNames(providerName);
            TableSchema.Table[] tables = new TableSchema.Table[tableNames.Length];

            for(int i = 0; i < tables.Length; i++)
                tables[i] = GetSchema(tableNames[i], providerName, TableType.Table);

            return tables;
        }

        /// <summary>
        /// Gets the foreign key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string[] GetForeignKeyTables(string tableName, string providerName)
        {
            return GetInstance().GetForeignKeyTables(tableName);
        }

        //public static string[] GetManyToMany(string providerName, string tableName)
        //{
        //    return GetInstance(providerName).GetManyToManyTables(tableName);
        //}

        /// <summary>
        /// Gets the view names.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string[] GetViewNames(string providerName)
        {
            return GetInstance(providerName).GetViewNameList();
        }

        /// <summary>
        /// Gets the views.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static TableSchema.Table[] GetViews(string providerName)
        {
            string[] viewNames = GetViewNames(providerName);
            TableSchema.Table[] views = new TableSchema.Table[viewNames.Length];

            for(int i = 0; i < views.Length; i++)
                views[i] = GetSchema(viewNames[i], providerName, TableType.View);

            return views;
        }

        /// <summary>
        /// Gets the SP list.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string[] GetSPList(string providerName)
        {
            return GetInstance(providerName).GetSPList();
        }

        /// <summary>
        /// Gets the SP name and schema list.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string[] GetSPList(string providerName, bool includeSchema)
        {
            return GetInstance(providerName).GetSPList(includeSchema);
        }

        /// <summary>
        /// Gets the primary key table names.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static ArrayList GetPrimaryKeyTableNames(string tableName, string providerName)
        {
            return GetInstance(providerName).GetPrimaryKeyTableNames(tableName);
        }

        /// <summary>
        /// Gets the primary key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static TableSchema.Table[] GetPrimaryKeyTables(string tableName, string providerName)
        {
            return GetInstance(providerName).GetPrimaryKeyTables(tableName);
        }

        /// <summary>
        /// Gets the name of the foreign key table.
        /// </summary>
        /// <param name="fkColumn">The fk column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string GetForeignKeyTableName(string fkColumn, string tableName, string providerName)
        {
            return GetInstance(providerName).GetForeignKeyTableName(fkColumn, tableName);
        }

        /// <summary>
        /// Gets the foreign key table.
        /// </summary>
        /// <param name="fkColumn">The fk column.</param>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public static TableSchema.Table GetForeignKeyTable(TableSchema.TableColumn fkColumn, TableSchema.Table table)
        {
            return GetInstance(table.Provider.Name).GetForeignKeyTable(fkColumn, table);
        }

        /// <summary>
        /// Gets the SP params.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static IDataReader GetSPParams(string spName, string providerName)
        {
            return GetInstance(providerName).GetSPParams(spName);
        }

        /// <summary>
        /// Gets the type of the db.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static DbType GetDbType(string dataType, string providerName)
        {
            return GetInstance(providerName).GetDbType(dataType);
        }

        /// <summary>
        /// Gets the type of the client.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string GetClientType(string providerName)
        {
            return GetInstance(providerName).GetType().Name;
        }

        #endregion


        #region DBCommand Helpers

        /// <summary>
        /// Gets the I db command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        internal static IDbCommand GetIDbCommand(QueryCommand qry)
        {
            return GetInstance(qry.ProviderName).GetCommand(qry);
        }

        /// <summary>
        /// Gets the db command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        internal static DbCommand GetDbCommand(QueryCommand qry)
        {
            return GetInstance(qry.ProviderName).GetDbCommand(qry);
        }

        #endregion
    }
}