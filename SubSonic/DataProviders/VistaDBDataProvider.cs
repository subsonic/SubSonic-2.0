using System;
using System.Collections;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Data.Common;
using SubSonic.Utilities;
using VistaDB.Provider;
using VistaDB.DDA;
using VistaDB.Extra;
using VistaDB;
using System.Diagnostics;
using System.Collections.Generic;

namespace SubSonic
{
    /// <summary>
    /// VistaDB Data Provider
    /// </summary>
    public class VistaDBDataProvider: DataProvider
    {
        //private DataTable _types = null;
        private DataTable _columns = null;
        private DataTable _foreignkeys;
        
        // VistaDB FAQ:

        // Q: Queries with table name in the form of [dbname].[tablename] give invalid table name error.
        // A: That's right. We don't support multiple schemas in single database. The only allowed db name space is [dbo].

        // Q: How can you tell if an identity is auto increment or not.
        // A: You can tell if an identity is auto increment based upon whether it has a seed and a step expression. 

        // Q: System tables not the same as sql server, so many sql server schema queries don't work.
        // A: We do not have plans to add all of the system tables from SQL Server. 
        // They do not make sense in most contexts. If you have specific needs ask and we may 
        // be able to provide something. Remember this is an in process database, SQL Server needs most 
        // of those because of the server context. 

        // Q: SCOPE_IDENTITY() not supported in VistaDB. You have to use @@IDENTITY.
        // A: Correct. Our identity is unique per connection (once again because we are in proc). 

        // Q: I want to implement paging with VistaDB. Is a pseudo column like SQL 2005's ROW_NUMBER available?
        // A: DDA part allows you to read RowId for the records. That's the analog of what you are asking for. 
        // These numbers are persistent in table. They are not re-used in row delete operations. 
        // Only packdatabase operation is followed by re-orderding and re-assigning new values to them.


        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return CreateConnection(DefaultConnectionString);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="newConnectionString">The new connection string.</param>
        /// <returns></returns>
        public override DbConnection CreateConnection(string newConnectionString)
        {
            VistaDBConnection retVal = new VistaDBConnection(newConnectionString);
            retVal.Open();
            return retVal;
        }

        /// <summary>
        /// Adds the params.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="qry">The qry.</param>
        private static void AddParams(VistaDBCommand cmd, QueryCommand qry)
        {
            if (qry.Parameters != null)
            {
                foreach (QueryParameter param in qry.Parameters)
                {
                    VistaDBParameter sqlParam = new VistaDBParameter(param.ParameterName, Utility.GetSqlDBType(param.DataType));
                    sqlParam.Direction = param.Mode;

                    //output parameters need to define a size
                    //our default is 50
                    if (sqlParam.Direction == ParameterDirection.Output || sqlParam.Direction == ParameterDirection.InputOutput)
                    {
                        sqlParam.Size = param.Size;
                    }

                    //fix for NULLs as parameter values
                    if (param.ParameterValue == null || Utility.IsMatch(param.ParameterValue.ToString(), "null"))
                    {
                        sqlParam.Value = DBNull.Value;
                    }
                    else if (param.DataType == DbType.Guid)
                    {
                        string paramValue = param.ParameterValue.ToString();
                        if (!String.IsNullOrEmpty(paramValue))
                        {
                            if (!Utility.IsMatch(paramValue, SqlSchemaVariable.DEFAULT))
                            {
                                sqlParam.Value = new Guid(param.ParameterValue.ToString());
                            }
                        }
                        else
                        {
                            sqlParam.Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        sqlParam.Value = param.ParameterValue;
                    }

                    cmd.Parameters.Add(sqlParam);

                }
            }
        }

        /// <summary>
        /// Checkouts the output params.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="qry">The qry.</param>
        private static void CheckoutOutputParams(VistaDBCommand cmd, QueryCommand qry)
        {
            if (qry.CommandType == CommandType.StoredProcedure && qry.HasOutputParams())
            {
                //loop the params, getting the values and setting them for the return
                foreach (QueryParameter param in qry.Parameters)
                {
                    if (param.Mode == ParameterDirection.InputOutput || param.Mode == ParameterDirection.Output || param.Mode == ParameterDirection.ReturnValue)
                    {
                        object oVal = cmd.Parameters[param.ParameterName].Value;
                    	param.ParameterValue = oVal;
                        qry.OutputValues.Add(oVal);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="rdr">The RDR.</param>
        /// <param name="par">The par.</param>
        public override void SetParameter(IDataReader rdr, StoredProcedure.Parameter par)
        {
            par.DBType = GetDbType(rdr[SqlSchemaVariable.DATA_TYPE].ToString());
            string sMode = rdr[SqlSchemaVariable.MODE].ToString();
            if (sMode == SqlSchemaVariable.MODE_INOUT)
                par.Mode = ParameterDirection.InputOutput;
            par.Name = rdr[SqlSchemaVariable.NAME].ToString();
        }

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        /// <returns></returns>
        public override string GetParameterPrefix()
        {
            return SqlSchemaVariable.PARAMETER_PREFIX;
        }

        /// <summary>
        /// Delimits the name of the db.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public override string DelimitDbName(string columnName)
        {
            if(!String.IsNullOrEmpty(columnName) && !columnName.StartsWith("[") && !columnName.EndsWith("]"))
            {
                return "[" + columnName + "]";
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDataReader GetReader(QueryCommand qry)
        {
            AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this);
            VistaDBCommand cmd = new VistaDBCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;
            AddParams(cmd, qry);

            cmd.Connection = (VistaDBConnection)automaticConnectionScope.Connection;
            //let this bubble up
            IDataReader rdr;

            //Thanks jcoenen!
            try
            {
                // if it is a shared connection, we shouldn't be telling the reader to close it when it is done
                if (automaticConnectionScope.IsUsingSharedConnection)
                    rdr = cmd.ExecuteReader();
                else
                    rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (SqlException)
            {
                // AutoConnectionScope will figure out what to do with the connection
                automaticConnectionScope.Dispose();
                //rethrow retaining stack trace.
                throw;
            }
            CheckoutOutputParams(cmd, qry);

            return rdr;
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override T GetDataSet<T>(QueryCommand qry)
        {
            T ds = new T();
            if (qry.CommandType == CommandType.Text)
            {
                qry.CommandSql = qry.CommandSql;
            }
            VistaDBCommand cmd = new VistaDBCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;
            VistaDBDataAdapter da = new VistaDBDataAdapter(cmd);

            AddTableMappings(da, ds);
            using (AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
            {
                cmd.Connection = (VistaDBConnection)conn.Connection;
                AddParams(cmd, qry);
                da.Fill(ds);

                CheckoutOutputParams(cmd, qry);

                return ds;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        public override object ExecuteScalar(QueryCommand qry)
        {
            //using (VistaDBConnection conn = new VistaDBConnection(connectionString))
            using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
            {
                VistaDBCommand cmd = new VistaDBCommand(qry.CommandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout;
                AddParams(cmd, qry);
                cmd.Connection = (VistaDBConnection)automaticConnectionScope.Connection;
                object result = cmd.ExecuteScalar();
                CheckoutOutputParams(cmd, qry);
                return result;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        public override int ExecuteQuery(QueryCommand qry)
        {
            using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
            {
                VistaDBCommand cmd = new VistaDBCommand(qry.CommandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout;

                AddParams(cmd, qry);
                cmd.Connection = (VistaDBConnection)automaticConnectionScope.Connection;
                int result = cmd.ExecuteNonQuery();
                CheckoutOutputParams(cmd, qry);
                return result;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        private bool IsPrimaryKey(string columnName, IVistaDBTableSchema schema)
        {
            foreach(IVistaDBIndexInformation info in schema.Indexes)
            {
                string[] keys = info.KeyExpression.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                ArrayList s = new ArrayList(keys);
                if(s.Contains(columnName))
                {
                    if(info.Primary)
                        return true;
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        private bool IsForeignKey(string columnName, IVistaDBTableSchema schema)
        {
            foreach(IVistaDBRelationshipInformation info in schema.ForeignKeys)
            {
                string[] keys = info.ForeignKey.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                ArrayList s = new ArrayList(keys);
                if(s.Contains(columnName))
                {
                    return true;
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        private bool IsAutoIncrement(string columnName, IVistaDBTableSchema schema)
        {
            foreach(IVistaDBIdentityInformation info in schema.Identities)
            {
                if(info.ColumnName == columnName)
                {
                    // TODO: assume it's autoincrement?
                    return true;
                }
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        private string GetColumnDefault(string columnName, IVistaDBTableSchema schema)
        {
            foreach(IVistaDBDefaultValueInformation info in schema.Defaults)
            {
                if(info.ColumnName == columnName)
                {
                    return info.Expression;
                }
            }
            return string.Empty;
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Load and cache all foreign keys
        /// </summary>
        /// <returns></returns>
        private DataTable AllForeignKeys
        {
            get
            {
                using(VistaDBConnection con = (VistaDBConnection)CreateConnection())
                    _foreignkeys = con.GetSchema("FOREIGNKEYS");

                return _foreignkeys;
            }

        }

        //-----------------------------------------------------------------------------------------------------------------------//
        private DataTable AllColumns
        {
            get
            {
                using(VistaDBConnection con = (VistaDBConnection)CreateConnection())
                    _columns = con.GetSchema("COLUMNS");

                return _columns;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        private DataTable GetViewColumns(string viewName)
        {
            DataTable table = new DataTable();
            using(VistaDBConnection conn = (VistaDBConnection)CreateConnection())
            {
                VistaDBCommand cmd = new VistaDBCommand("SELECT * FROM GetViewColumns('" + viewName + "');");
                cmd.Connection = conn;
                cmd.CommandType = CommandType.Text;
                VistaDBDataAdapter da = new VistaDBDataAdapter(cmd);
                da.Fill(table);
                return table;
            }
        }

        /*
        Table: Columns   ColumnName: TABLE_CATALOG
        Table: Columns   ColumnName: TABLE_SCHEMA
        Table: Columns   ColumnName: TABLE_NAME
        Table: Columns   ColumnName: COLUMN_NAME
        Table: Columns   ColumnName: COLUMN_GUID
        Table: Columns   ColumnName: COLUMN_PROPID
        Table: Columns   ColumnName: ORDINAL_POSITION
        Table: Columns   ColumnName: COLUMN_HASDEFAULT
        Table: Columns   ColumnName: COLUMN_DEFAULT
        Table: Columns   ColumnName: IS_NULLABLE
        Table: Columns   ColumnName: DATA_TYPE
        Table: Columns   ColumnName: TYPE_GUID
        Table: Columns   ColumnName: CHARACTER_MAXIMUM_LENGTH
        Table: Columns   ColumnName: CHARACTER_OCTET_LENGTH
        Table: Columns   ColumnName: NUMERIC_PRECISION
        Table: Columns   ColumnName: NUMERIC_SCALE
        Table: Columns   ColumnName: DATETIME_PRECISION
        Table: Columns   ColumnName: CHARACTER_SET_CATALOG
        Table: Columns   ColumnName: CHARACTER_SET_SCHEMA
        Table: Columns   ColumnName: CHARACTER_SET_NAME
        Table: Columns   ColumnName: COLLATION_CATALOG
        Table: Columns   ColumnName: COLLATION_SCHEMA
        Table: Columns   ColumnName: COLLATION_NAME
        Table: Columns   ColumnName: DOMAIN_CATALOG
        Table: Columns   ColumnName: DOMAIN_NAME
        Table: Columns   ColumnName: DESCRIPTION
        Table: Columns   ColumnName: PRIMARY_KEY
        Table: Columns   ColumnName: COLUMN_CAPTION
        Table: Columns   ColumnName: COLUMN_ENCRYPTED
        Table: Columns   ColumnName: COLUMN_PACKED

        Table: ForeignKeys   ColumnName: CONSTRAINT_CATALOG
        Table: ForeignKeys   ColumnName: CONSTRAINT_SCHEMA
        Table: ForeignKeys   ColumnName: CONSTRAINT_NAME
        Table: ForeignKeys   ColumnName: TABLE_CATALOG
        Table: ForeignKeys   ColumnName: TABLE_SCHEMA
        Table: ForeignKeys   ColumnName: TABLE_NAME
        Table: ForeignKeys   ColumnName: CONSTRAINT_TYPE
        Table: ForeignKeys   ColumnName: IS_DEFERRABLE
        Table: ForeignKeys   ColumnName: INITIALLY_DEFERRED
        Table: ForeignKeys   ColumnName: FKEY_TO_CATALOG
        Table: ForeignKeys   ColumnName: FKEY_TO_SCHEMA
        Table: ForeignKeys   ColumnName: FKEY_TO_TABLE
        */

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <returns></returns>
        public override TableSchema.Table GetTableSchema(string tableName, TableType tableType)
        {
            Debug.WriteLine("GetTableSchema: tableName = " + tableName);


            TableSchema.Table table = new TableSchema.Table(tableName, tableType, this);
            table.Name = tableName;
            table.Columns = new TableSchema.TableColumnCollection();
            table.ForeignKeys = new TableSchema.ForeignKeyTableCollection();
            table.SchemaName = string.Empty;

            
            // This will get called on views as well as tables.
 
            // Attempt to create schema for views.
            
            ArrayList viewNames = new ArrayList(GetViewNameList());
            
            if(viewNames.Contains(tableName))
            {
                // TODO: Check view for IS_UPDATABLE and IS_CORRECT

                DataTable viewColumnTable = GetViewColumns(tableName);

                //Add all the columns
                foreach(DataRow row in viewColumnTable.Rows)
                {
                    TableSchema.TableColumn column = new TableSchema.TableColumn(table);

                    column.ColumnName = row["COLUMN_NAME"].ToString();
                    column.IsPrimaryKey = Convert.ToBoolean(row["IS_KEY"]);
                    // TODO: foreign keys?
                    column.IsForeignKey = false;

                    string nativeDataType = row["DATA_TYPE_NAME"].ToString().ToLower();
                    column.DataType = GetDbType(nativeDataType);

                    if(column.DataType == DbType.String)
                        column.MaxLength = Convert.ToInt32(row["COLUMN_SIZE"]);

                    column.AutoIncrement = false;

                    if(row["IDENTITY_STEP"] != DBNull.Value)
                        column.AutoIncrement = Convert.ToInt32(row["IDENTITY_STEP"]) > 0 && Convert.ToBoolean(row["IS_KEY"]);

                    column.IsNullable = Convert.ToBoolean(row["ALLOW_NULL"]);
                    column.IsReadOnly = false;
                    column.DefaultSetting = row["DEFAULT_VALUE"].ToString();

                    table.Columns.Add(column);
                }
                return table;

            }
            else
            {
                // TODO: Cludge for now. Connection string only has "Data Source=File.vdb3".
                string filename = DefaultConnectionString.Substring(DefaultConnectionString.IndexOf("=") + 1);
                IVistaDBDatabase db = VistaDBEngine.Connections.OpenDDA().OpenDatabase(filename, VistaDBDatabaseOpenMode.NonexclusiveReadWrite, null);
                IVistaDBTableSchema schema = db.TableSchema(tableName);
                
                IVistaDBRelationshipList foreignKeyList = schema.ForeignKeys;
                foreach (IVistaDBRelationshipInformation info in foreignKeyList)
                {
                    //Debug.WriteLine(
                    //    "  Name: " + info.Name +
                    //    "  ForeignKey: " + info.ForeignKey +
                    //    "  ForeignTable: " + info.ForeignTable +
                    //    "  PrimaryTable: " + info.PrimaryTable +
                    //    "  DeleteIntegrity: " + info.DeleteIntegrity.ToString() +
                    //    "  UpdateIntegrity: " + info.UpdateIntegrity.ToString());

                    // TODO: fix this for compound foreign keys.
                    TableSchema.ForeignKeyTable fk = new TableSchema.ForeignKeyTable(this);
                    string foreignKey = info.ForeignKey;
                    if (foreignKey.Contains(";"))
                    {
                        foreignKey = foreignKey.Substring(0, foreignKey.IndexOf(";"));
                    }

                    fk.ColumnName = foreignKey;
                    fk.TableName = info.PrimaryTable;
                    fk.TableType = TableType.Table;
                    table.ForeignKeys.Add(fk);
                }

                //Add all the columns
                for (int n = 0; n < schema.ColumnCount; n++)
                {
                    TableSchema.TableColumn column = new TableSchema.TableColumn(table);

                    column.ColumnName = schema[n].Name;
                    column.IsNullable = schema[n].AllowNull;
                    column.IsReadOnly = schema[n].ReadOnly;
                    column.IsPrimaryKey = IsPrimaryKey(column.ColumnName, schema);
                    column.IsForeignKey = IsForeignKey(column.ColumnName, schema);
                    column.AutoIncrement = IsAutoIncrement(column.ColumnName, schema);

                    column.DataType = GetDbType(schema[n].Type.ToString().ToLower());

                    // System will know the size for non-strings?
                    if (column.DataType == DbType.String)
                        column.MaxLength = schema[n].MaxLength;

                    column.DefaultSetting = GetColumnDefault(column.ColumnName, schema);

                    table.Columns.Add(column);
                }

                return table;
            }

        } // GetTableSchema

        /// <summary>
        /// Gets the SP params.
        /// Not currently supported for VistaDB. No stored procedures.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        public override IDataReader GetSPParams(string spName)
        {
            return null;
        }

        /// <summary>
        /// Gets the SP list.
        /// Not currently supported for VistaDB. No stored procedures.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSPList()
        {
            return new string[] {};
        }

        /*
            Query to list the database schema (meta information):

            SELECT * FROM [DATABASE SCHEMA];
            SELECT * FROM GetViews();
            SELECT * FROM GetViewColumns('NewView1');
        
            VistDB TypeID Values:

            Tables = 1
            Table Indexes = 2
            Table Columns = 3
            Table Constraint = 4
            DefaultValue = 5
            Table Identity = 6
            Relationship = 7
            Trigger = 8
            Database Description = 9
            View = 10
            CLR Procs = 11
            Assembly containing CLR Procs = 12
        */

        /// <summary>
        /// Gets the view name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetViewNameList()
        {
            // TODO: support views.
            //ViewNames = new string[] {};
            //return ViewNames;

            if(ViewNames == null || !CurrentConnectionStringIsDefault)
            {
                // Get the views in VistaDB:
                QueryCommand cmd = new QueryCommand("SELECT * FROM GetViews()", Name);
                StringBuilder sList = new StringBuilder();
                using(IDataReader rdr = GetReader(cmd))
                {
                    while(rdr.Read())
                    {
                        string viewName = rdr["VIEW_NAME"].ToString();

                        if(String.IsNullOrEmpty(ViewStartsWith) || viewName.StartsWith(ViewStartsWith))
                        {
                            sList.Append(viewName);
                            sList.Append("|");
                        }
                    }
                    rdr.Close();
                }
                string strList = sList.ToString();
                string[] tempViewNames = strList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                Array.Sort(tempViewNames);
                if(CurrentConnectionStringIsDefault)
                {
                    ViewNames = tempViewNames;
                }
                else
                {
                    return tempViewNames;
                }
            }
            return ViewNames;
        }

        /// <summary>
        /// Gets the table name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetTableNameList()
        {
            if (TableNames == null || !CurrentConnectionStringIsDefault)
            {
                IVistaDBDatabase db;
                VistaDBDataTable table;

                string filename = DefaultConnectionString.Substring(DefaultConnectionString.IndexOf("=")+1);

                db = VistaDBEngine.Connections.OpenDDA().OpenDatabase(filename, VistaDBDatabaseOpenMode.NonexclusiveReadWrite, null);
                ArrayList tables = db.EnumTables();

                tables.Sort();
                List<string> names = new List<string>();

                foreach(string tableName in tables)
                {
                    IVistaDBTableSchema s = db.TableSchema(tableName);
                    Debug.WriteLine(tableName + ": s.ColumnCount = " + s.ColumnCount);

                    names.Add(tableName);
                }
                
                if(CurrentConnectionStringIsDefault)
                {
                    TableNames = names.ToArray();
                }
                else
                {
                    return names.ToArray();
                }
            }
            return TableNames;
        }

        /// <summary>
        /// Gets the primary key table names.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override ArrayList GetPrimaryKeyTableNames(string tableName)
        {
            return new ArrayList();
        }

        /// <summary>
        /// Gets the primary key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override TableSchema.Table[] GetPrimaryKeyTables(string tableName)
        {
            return null;
        }

        /// <summary>
        /// Gets the name of the foreign key table.
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName, string tableName)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the name of the foreign key table.
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName)
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets the foreign key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override string[] GetForeignKeyTables(string tableName)
        {
            return new string[] { "" };
            //throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the table name by primary key.
        /// </summary>
        /// <param name="pkName">Name of the pk.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public override string GetTableNameByPrimaryKey(string pkName, string providerName)
        {
            // TODO: Look in to the use of this method and program if possible.
            return string.Empty;
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        protected override string GetDatabaseVersion(string providerName)
        {
            QueryCommand cmd = new QueryCommand("SELECT @@version", providerName);
            object oResult = DataService.ExecuteScalar(cmd);
            if(oResult != null)
                return oResult.ToString();
            return "Unknown";
        }

        /// <summary>
        /// Gets the type of the db.
        /// </summary>
        /// <param name="sqlType">Type of the SQL.</param>
        /// <returns></returns>
        public override DbType GetDbType(string sqlType)
        {
            switch (sqlType)
            {
                case "varchar":
                    return DbType.String;
                case "nvarchar":
                    return DbType.String;
                case "int":
                    return DbType.Int32;
                case "uniqueidentifier":
                    return DbType.Guid;
                case "datetime":
                    return DbType.DateTime;
                case "bigint":
                    return DbType.Int64;
                case "binary":
                    return DbType.Binary;
                case "bit":
                    return DbType.Boolean;
                case "char":
                    return DbType.AnsiStringFixedLength;
                case "decimal":
                    return DbType.Decimal;
                case "float":
                    return DbType.Double;
                case "image":
                    return DbType.Binary;
                case "money":
                    return DbType.Currency;
                case "nchar":
                    return DbType.String;
                case "ntext":
                    return DbType.String;
                case "numeric":
                    return DbType.Decimal;
                case "real":
                    return DbType.Decimal;
                case "smalldatetime":
                    return DbType.DateTime;
                case "smallint":
                    return DbType.Int16;
                case "smallmoney":
                    return DbType.Currency;
                case "sql_variant":
                    return DbType.String;
                case "sysname":
                    return DbType.String;
                case "text":
                    return DbType.String;
                case "timestamp":
                    return DbType.Binary;
                case "tinyint":
                    return DbType.Byte;
                case "varbinary":
                    return DbType.Binary;
                default:
                    return DbType.String;
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        public override IDbCommand GetCommand(QueryCommand qry)
        {
            VistaDBCommand cmd = new VistaDBCommand(qry.CommandSql);
            AddParams(cmd, qry);
            return cmd;
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Executes a transaction using the passed-commands
        /// </summary>
        /// <param name="commands"></param>
        public override void ExecuteTransaction(QueryCommandCollection commands)
        {
            //make sure we have at least one
            if (commands.Count > 0)
            {
                VistaDBCommand cmd = null;

                //a using statement will make sure we close off the connection

                using (AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
                {
                    
                    //open up the connection and start the transaction
                    if (conn.Connection.State == ConnectionState.Closed)
                        conn.Connection.Open();

                    VistaDBTransaction trans = (VistaDBTransaction)conn.Connection.BeginTransaction();

                    foreach (QueryCommand qry in commands)
                    {
                        if (qry.CommandType == CommandType.Text)
                        {
                            //qry.CommandSql = "/* ExecuteTransaction() */ " + qry.CommandSql;
                            qry.CommandSql = qry.CommandSql;
                        }
                        cmd = new VistaDBCommand(qry.CommandSql, (VistaDBConnection)conn.Connection); //, trans);
                        cmd.CommandType = qry.CommandType;
                        cmd.Transaction = trans;

                        AddParams(cmd, qry);

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (SqlException x)
                        {
                            //if there's an error, roll everything back
                            trans.Rollback();

                            //clean up
                            conn.Connection.Close();
                            cmd.Dispose();

                            //throw the error retaining the stack.
                            throw new Exception(x.Message);
                        }
                    }
                    //if we get to this point, we're good to go
                    trans.Commit();

                    //close off the connection
                    conn.Connection.Close();
                    if (cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            else
            {
                throw new Exception("No commands present");
            }
        }

        #region SQL Builders

        //-----------------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Helper method to fix the TOP string of a given query.
        /// </summary>
        /// <param name="qry">Query to build the TOP string from.</param>
        /// <returns></returns>
        private string GetTop(Query qry)
        {
            string top = string.Empty;

            // Paging doesn't seem to be supported in VistaDB.

            if(qry.PageIndex == -1)
            {
                // By default VistaDB will return 100% of the results
                // there is no need to apply a limit so we will
                // return an empty string.
                if(qry.Top == "100 PERCENT" || String.IsNullOrEmpty(qry.Top))
                    return top;

                // If the Top property of the query contains either
                // a % character or the word percent we need to do
                // some extra work
                if(qry.Top.Contains("%") || qry.Top.ToLower().Contains("percent"))
                {
                    // strip everything but the numeric portion of
                    // the top property.
                    top = qry.Top.ToLower().Replace("%", string.Empty).Replace("percent", string.Empty).Trim();

                    // we will try/catch just incase something fails
                    // fails a conversion.  This gives us an easy out
                    try
                    {
                        // Convert the percetage to a decimal
                        decimal percentTop = Convert.ToDecimal(top) / 100;

                        // Get the total count of records to
                        // be returned.
                        int count = GetRecordCount(qry);

                        // Using the new decimal and the amount
                        // of records to be returned calculate
                        // what percentage of the records are
                        // to be returned
                        top = Convert.ToString((int)(count * percentTop));
                    }
                    catch
                    {
                        // If something fails in the try lets
                        // just return an empty string and
                        // move on.
                        top = string.Empty;
                    }
                }
                else
                {
                    // TODO: paging?
                    top = qry.Top;
                }
            }
            else
            {
                // TODO: paging?
                top = qry.Top;
            }

            return top;
        }

        /// <summary>
        /// Creates a SELECT statement based on the Query object settings
        /// this is only used with the SQL constructors below
        /// it's not used in the command builders above, which need to set the parameters
        /// right at the time of the command build
        /// </summary>
        /// <returns></returns>
        public override string GetSelectSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;
            string distinct = qry.IsDistinct ? SqlFragment.DISTINCT : String.Empty;

            //different rules for how to do TOP
            //string select = "/* GetSelectSql(" + table.Name + ") */ " + SqlFragment.SELECT + 
            //   distinct + SqlFragment.TOP + qry.Top + " ";
            //string select = SqlFragment.SELECT + distinct + SqlFragment.TOP + qry.Top + " ";
            
            // TODO: Fix this for VistaDB. There is no PERCENT?
            string top = GetTop(qry);
            string select;
            if(top.Length == 0)
                select = SqlFragment.SELECT + distinct + " ";
            else
                select = SqlFragment.SELECT + distinct + SqlFragment.TOP + top + " ";

            StringBuilder order = new StringBuilder();
            StringBuilder query = new StringBuilder();
            string columns;

            //append on the selectList, which is a property that can be set
            //and is "*" by default

            if (qry.SelectList != null && qry.SelectList.Trim().Length >= 2)
            {
                columns = qry.SelectList;
            }
            else
            {
                columns = GetQualifiedSelect(table);
            }

            string where = BuildWhere(qry);

            //Finally, do the orderby

            if (qry.OrderByCollection.Count > 0)
            {
                order.Append(SqlFragment.ORDER_BY);
                for(int j = 0; j < qry.OrderByCollection.Count; j++)
                {
                    string orderString = qry.OrderByCollection[j].OrderString;
                    if (!String.IsNullOrEmpty(orderString))
                    {
                        order.Append(orderString);
                        if (j + 1 != qry.OrderByCollection.Count)
                        {
                            order.Append(", ");
                        }
                    }
                }
            }
            else
            {
                if (table.PrimaryKey != null)
                {
                    order.Append(SqlFragment.ORDER_BY + OrderBy.Asc(table.PrimaryKey.ColumnName).OrderString);
                }
            }

            if (qry.PageIndex < 0)
            {
                query.Append(select);
                query.Append(columns);
                query.Append(SqlFragment.FROM);
                //query.Append(Utility.QualifyColumnName(table.SchemaName, table.Name, qry.Provider));
                query.Append(Utility.QualifyColumnName("DBO", table.Name, qry.Provider));
                query.Append(where);
                query.Append(order);
                query.Append(";");
            }
            else
            {
                if (table.PrimaryKey != null)
                {
                    query.Append(string.Format(
                        PAGING_SQL, 
                        table.PrimaryKey.ColumnName,
                        Utility.QualifyColumnName(table.SchemaName, table.Name, qry.Provider),
                        columns, 
                        where, 
                        order, 
                        qry.PageIndex, 
                        qry.PageSize, 
                        Utility.GetSqlDBType(table.PrimaryKey.DataType)));
                    query.Append(";");
                }
                else
                {
                    //pretend it's a view
                    query.Append(string.Format(
                        PAGING_VIEW_SQL,
                        Utility.QualifyColumnName(table.SchemaName, table.Name, qry.Provider),
                        where, 
                        order, 
                        qry.PageIndex, 
                        qry.PageSize));
                    query.Append(";");

                }
            }

            return query.ToString();
        }

        #region Paging sql

        //thanks Jon G!
        private const string PAGING_VIEW_SQL = @"					
					DECLARE @Page int
					DECLARE @PageSize int

					SET @Page = {3}
					SET @PageSize = {4}

					SET NOCOUNT ON
                    
                    SELECT * INTO #temp FROM {0} WHERE 1 = 0
                    ALTER TABLE #temp ADD _indexID int PRIMARY KEY IDENTITY(1,1)
                    
                    INSERT INTO #temp SELECT * FROM {0} {1} {2}

                    SELECT * FROM #temp
                    WHERE _indexID BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)
					
                    --clean up
                    DROP TABLE #temp
                    ";

        private const string PAGING_SQL = @"					
					DECLARE @Page int
					DECLARE @PageSize int

					SET @Page = {5}
					SET @PageSize = {6}

					SET NOCOUNT ON

					-- create a temp table to hold order ids
					DECLARE @TempTable TABLE (IndexId int identity, _keyID {7})

					-- insert the table ids and row numbers into the memory table
					INSERT INTO @TempTable
					(
					  _keyID
					)
					SELECT 
						{0}
					FROM
						{1}
					{3}
					{4}

					-- select only those rows belonging to the proper page
					SELECT 
					{2}
					FROM {1}
					INNER JOIN @TempTable t ON {1}.{0} = t._keyID
					WHERE t.IndexId BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)
                    {4}";

        #endregion

        /// <summary>
        /// Returns a qualified list of columns ([Table].[Column])
        /// </summary>
        /// <returns></returns>
        protected static string GetQualifiedSelect(TableSchema.Table table)
        {
            StringBuilder sb = new StringBuilder();
            foreach (TableSchema.TableColumn tc in table.Columns)
            {
                // VistaDB doesn't support database names.
                //sb.AppendFormat(", [{0}].[{1}].[{2}]", table.SchemaName, table.Name, tc.ColumnName);
                sb.AppendFormat(", [{0}].[{1}]", table.Name, tc.ColumnName);
            }

            string result = sb.ToString();
            if (result.Length > 1)
                result = sb.ToString().Substring(1);

            return result;
        }

        /// <summary>
        /// Loops the TableColums[] array for the object, creating a SQL string
        /// for use as an INSERT statement
        /// </summary>
        /// <returns></returns>
        public override string GetInsertSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;

            //split the TablNames and loop out the SQL
            //string insertSQL = "/* GetInsertSql(" + table.Name + ") */ " + SqlFragment.INSERT_INTO + Utility.QualifyColumnName(table.SchemaName, table.Name, qry.Provider);
            string insertSQL = SqlFragment.INSERT_INTO + Utility.QualifyColumnName(table.SchemaName, table.Name, qry.Provider);
            //string client = DataService.GetClientType();

            string cols = String.Empty;
            string pars = String.Empty;

            //returns Guid from VS2005 only!
            bool primaryKeyIsGuid = false;
            string primaryKeyName = "";

            bool isFirstColumn = true;
            //if table columns are null toss an exception
            foreach (TableSchema.TableColumn col in table.Columns)
            {
                string colName = col.ColumnName;
                if( !( col.DataType == DbType.Guid && 
                       col.DefaultSetting != null && 
                       col.DefaultSetting == SqlSchemaVariable.DEFAULT ) )
                {
                    if(!col.AutoIncrement && !col.IsReadOnly)
                    {
                        if(!isFirstColumn)
                        {
                            cols += ",";
                            pars += ",";
                        }

                        isFirstColumn = false;

                        cols += DelimitDbName(colName);
                        pars += Utility.PrefixParameter(colName, this);
                    }
                    if(col.IsPrimaryKey && col.DataType == DbType.Guid)
                    {
                        primaryKeyName = col.ColumnName;
                        primaryKeyIsGuid = true;
                    }
                }
            }

            insertSQL += "(" + cols + ") ";

            //Non Guid's
            // SCOPE_IDENTITY() not supported in VistaDB.
            //string getInsertValue = SqlFragment.SELECT + "SCOPE_IDENTITY()" + SqlFragment.AS + "newID;";
            string getInsertValue = SqlFragment.SELECT + "@@IDENTITY" + SqlFragment.AS + "newID;";
            // SQL Server 2005
            if (primaryKeyIsGuid)
            {
                if (Utility.IsSql2005(this))
                {
                    insertSQL += " OUTPUT INSERTED.[" + primaryKeyName + "]";
                }
                else
                {
                    getInsertValue = " SELECT @" + primaryKeyName + " as newID;";
                }
            }

            insertSQL += "VALUES(" + pars + ");";
            insertSQL += getInsertValue;

            // insertSQL = "INSERT INTO [Item_attribute]() VALUES();SELECT @@IDENTITY AS newID;"

            return insertSQL;
        }

        #endregion

        #region SQL Scripters

        /// <summary>
        /// Scripts the data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public override string ScriptData(string tableName, string providerName)
        {
            StringBuilder fieldList = new StringBuilder();
            StringBuilder insertStatement = new StringBuilder();
            StringBuilder statements = new StringBuilder();
            StringBuilder result = new StringBuilder();
            StringBuilder disableConstraint = new StringBuilder();
            disableConstraint.AppendLine("ALTER TABLE [" + tableName + "] NOCHECK CONSTRAINT ALL");
            disableConstraint.AppendLine("GO");

            StringBuilder enableConstraint = new StringBuilder();
            enableConstraint.AppendLine("ALTER TABLE [" + tableName + "] CHECK CONSTRAINT ALL");
            enableConstraint.AppendLine("GO");

            //QueryCommand cmd = new QueryCommand("SELECT CONSTRAINT_NAME FROM DBO.KEY_COLUMN_USAGE WHERE TABLE_NAME='"+ tableName +"'");
            //List<string> constraints = new List<string>();
            //using (IDataReader rdr = GetReader(cmd))
            //{
            //    while (rdr.Read())
            //    {
            //        constraints.Add(rdr["CONSTRAINT_NAME"].ToString());
            //    }
            //    if (!rdr.IsClosed)
            //    {
            //        rdr.Close();
            //    }
            //}

            insertStatement.Append("INSERT INTO [" + tableName + "] ");

            //pull the schema for this table
            TableSchema.Table table = Query.BuildTableSchema(tableName, providerName);

            //build the insert list.
            string lastColumnName = table.Columns[table.Columns.Count - 1].ColumnName;
            foreach (TableSchema.TableColumn col in table.Columns)
            {
                fieldList.Append("[");
                fieldList.Append(col.ColumnName);
                fieldList.Append("]");

                if (!Utility.IsMatch(col.ColumnName, lastColumnName))
                    fieldList.Append(", ");
            }

            //complete the insert statement
            insertStatement.Append("(");
            insertStatement.Append(fieldList);
            insertStatement.AppendLine(")");

            //get the table data
            IDataReader rdr = new Query(table).ExecuteReader();
            //bool isNumeric = false;
            //TableSchema.TableColumn thisColumn=null;

            while (rdr.Read())
            {
                StringBuilder thisStatement = new StringBuilder();
                thisStatement.Append(insertStatement);
                thisStatement.Append("VALUES(");
                //loop the schema and pull out the values from the reader
                foreach (TableSchema.TableColumn col in table.Columns)
                {
                    object oData = rdr[col.ColumnName];
                    if (oData != null && oData != DBNull.Value)
                    {
                        if (col.DataType == DbType.Boolean)
                        {
                            bool bData = Convert.ToBoolean(oData);
                            thisStatement.Append(bData ? "1" : " 0");
                        }
                        else if (col.DataType == DbType.Byte || col.DataType == DbType.Binary)
                        {
                            thisStatement.Append("0x");
                            thisStatement.Append(Utility.ByteArrayToString((Byte[])oData).ToUpper());
                        }
                        else if (col.IsNumeric)
                        {
                            thisStatement.Append(oData);
                        }
                        else
                        {
                            thisStatement.Append("'");
                            thisStatement.Append(oData.ToString().Replace("'", "''"));
                            thisStatement.Append("'");
                        }
                    }
                    else
                    {
                        thisStatement.Append("NULL");
                    }

                    if (!Utility.IsMatch(col.ColumnName, lastColumnName))
                        thisStatement.Append(", ");
                }

                //add in a closing paren
                thisStatement.AppendLine(")");
                statements.Append(thisStatement);
            }
            rdr.Close();


            //if identity is set for the PK, set IDENTITY INSERT to true
            result.AppendLine(disableConstraint.ToString());
            if (table.PrimaryKey != null)
            {
                if (table.PrimaryKey.AutoIncrement)
                {
                    result.AppendLine("SET IDENTITY_INSERT [" + tableName + "] ON ");
                }
            }
            result.AppendLine("PRINT 'Begin inserting data in " + tableName + "'");
            result.Append(statements);

            if (table.PrimaryKey != null)
            {
                if (table.PrimaryKey.AutoIncrement)
                {
                    result.AppendLine("SET IDENTITY_INSERT [" + tableName + "] OFF ");
                }
            }
            result.AppendLine(enableConstraint.ToString());
            return result.ToString();
        }



        #endregion

        #region DataProvider required override methods

        /// <summary>
        /// Gets the db command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override DbCommand GetDbCommand(QueryCommand qry)
        {
            DbCommand cmd;
            //VistaDBConnection conn = new VistaDBConnection(connectionString);
            AutomaticConnectionScope conn = new AutomaticConnectionScope(this);

            cmd = conn.Connection.CreateCommand();
            cmd.CommandText = qry.CommandSql;
            cmd.CommandType = qry.CommandType;

            foreach (QueryParameter par in qry.Parameters)
            {
                cmd.Parameters.Add(par);
            }

            return cmd;
        }


        /// <summary>
        /// Gets the single record reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDataReader GetSingleRecordReader(QueryCommand cmd)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Gets the type of the named provider.
        /// </summary>
        /// <value>The type of the named provider.</value>
        public override string NamedProviderType
        {
            get { return "AccessDataProvider"; }
        }

        /// <summary>
        /// Force-reloads a provider's schema
        /// </summary>
        public override void ReloadSchema()
        {
            //Nothing to do here
        }

        #endregion




    }
}
