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
#if ALLPROVIDERS
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;
using MySql.Data.MySqlClient;

namespace SubSonic
{
    /// <summary>
    /// A Data Provider for MySQL. You can thank Larry Beall for his work here.
    /// </summary>
    public class MySqlDataProvider : DataProvider
    {
        /// <summary>
        /// Gets the type of the named provider.
        /// </summary>
        /// <value>The type of the named provider.</value>
        public override string NamedProviderType
        {
            get { return DataProviderTypeName.MY_SQL; }
        }

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
            MySqlConnection retVal = new MySqlConnection(newConnectionString);
            retVal.Open();

            return retVal;
        }

        /// <summary>
        /// Reloads the cached schema
        /// </summary>
        public override void ReloadSchema()
        {
            //not sure how to do this here
        }

        /// <summary>
        /// Adds the params.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="qry">The qry.</param>
        private static void AddParams(MySqlCommand cmd, QueryCommand qry)
        {
            if(qry.Parameters != null)
            {
                foreach(QueryParameter param in qry.Parameters)
                {
                    MySqlParameter sqlParam = new MySqlParameter();
                    sqlParam.DbType = param.DataType;
                    sqlParam.ParameterName = param.ParameterName.Replace('@', '?');
                    sqlParam.Value = param.ParameterValue;

                    if(qry.CommandType == CommandType.StoredProcedure)
                    {
                        switch(param.Mode)
                        {
                            case ParameterDirection.InputOutput:
                                sqlParam.Direction = ParameterDirection.InputOutput;
                                break;
                            case ParameterDirection.Output:
                                sqlParam.Direction = ParameterDirection.Output;
                                break;
                            case ParameterDirection.ReturnValue:
                                sqlParam.Direction = ParameterDirection.ReturnValue;
                                break;
                            case ParameterDirection.Input:
                                sqlParam.Direction = ParameterDirection.Input;
                                break;
                        }
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
        private static void CheckoutOutputParams(MySqlCommand cmd, QueryCommand qry)
        {
            if(qry.CommandType == CommandType.StoredProcedure && qry.HasOutputParams())
            {
                //loop the params, getting the values and setting them for the return
                foreach(QueryParameter param in qry.Parameters)
                {
                    if(param.Mode == ParameterDirection.InputOutput || param.Mode == ParameterDirection.Output || param.Mode == ParameterDirection.ReturnValue)
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
        /// <param name="dataReader">The data reader.</param>
        /// <param name="parameter">The parameter.</param>
        public override void SetParameter(IDataReader dataReader, StoredProcedure.Parameter parameter)
        {
            parameter.DBType = GetDbType(dataReader[1].ToString());
            parameter.Name = dataReader[0].ToString();
            ParameterDirection direction = (ParameterDirection)Enum.Parse(typeof(ParameterDirection), dataReader[2].ToString());
            if(direction == ParameterDirection.InputOutput)
                parameter.Mode = ParameterDirection.InputOutput;
        }

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        /// <returns></returns>
        public override string GetParameterPrefix()
        {
            return MySqlSchemaVariable.PARAMETER_PREFIX;
        }

        /// <summary>
        /// Delimits the name of the db.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public override string FormatIdentifier(string columnName)
        {
            if(!String.IsNullOrEmpty(columnName))
                return "`" + columnName + "`";
            return String.Empty;
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDbCommand GetCommand(QueryCommand qry)
        {
            MySqlCommand cmd = new MySqlCommand(qry.CommandSql);
            AddParams(cmd, qry);
            return cmd;
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDataReader GetReader(QueryCommand qry)
        {
            AutomaticConnectionScope conn = new AutomaticConnectionScope(this);

            MySqlCommand cmd = new MySqlCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;

            AddParams(cmd, qry);

            cmd.Connection = (MySqlConnection)conn.Connection;

            IDataReader rdr;

            try
            {
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch(MySqlException x)
            {
                conn.Dispose();
                throw x;
            }

            CheckoutOutputParams(cmd, qry);

            return rdr;
        }

        /// <summary>
        /// Gets the single record reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDataReader GetSingleRecordReader(QueryCommand qry)
        {
            AutomaticConnectionScope conn = new AutomaticConnectionScope(this);

            MySqlCommand cmd = new MySqlCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;

            AddParams(cmd, qry);

            cmd.Connection = (MySqlConnection)conn.Connection;

            IDataReader rdr;
            // if it is a shared connection, we shouldn't be telling the reader to close it when it is done
            if(conn.IsUsingSharedConnection)
                rdr = cmd.ExecuteReader();
            else
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult | CommandBehavior.SingleRow);

            return rdr;
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override DataSet GetDataSet(QueryCommand qry)
        {
            DataSet ds = new DataSet();
            MySqlCommand cmd = new MySqlCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;

            AddParams(cmd, qry);
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);

            using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
            {
                cmd.Connection = (MySqlConnection)conn.Connection;

                da.Fill(ds);

                cmd.Dispose();
                da.Dispose();

                return ds;
            }
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override object ExecuteScalar(QueryCommand qry)
        {
            using(AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
            {
                MySqlCommand cmd = new MySqlCommand(qry.CommandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout;
                AddParams(cmd, qry);
                cmd.Connection = (MySqlConnection)automaticConnectionScope.Connection;
                object result = cmd.ExecuteScalar();
                CheckoutOutputParams(cmd, qry);
                return result;
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override int ExecuteQuery(QueryCommand qry)
        {
            using(AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
            {
                MySqlCommand cmd = new MySqlCommand(qry.CommandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout;
                AddParams(cmd, qry);
                cmd.Connection = (MySqlConnection)automaticConnectionScope.Connection;
                int result = cmd.ExecuteNonQuery();
                CheckoutOutputParams(cmd, qry);
                return result;
            }
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <returns></returns>
        public override TableSchema.Table GetTableSchema(string tableName, TableType tableType)
        {
            TableSchema.TableColumnCollection columns = new TableSchema.TableColumnCollection();
            TableSchema.Table table = new TableSchema.Table(tableName, tableType, this);
            table.Name = tableName;
            string sql = " DESCRIBE `" + tableName + "`";
            using(MySqlCommand cmd = new MySqlCommand(sql))
            {
                //get information about both the table and it's columns
                MySqlConnection conn = new MySqlConnection(DefaultConnectionString);
                cmd.Connection = conn;
                conn.Open();

                try
                {
                    using(IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while(rdr.Read())
                        {
                            TableSchema.TableColumn column = new TableSchema.TableColumn(table);
                            column.IsPrimaryKey = rdr["Key"].ToString() == "PRI";
                            column.IsForeignKey = rdr["Key"].ToString() == "MUL";
                            column.ColumnName = rdr["Field"].ToString();

                            string sType = rdr["Type"].ToString();
                            string sSize;
                            if(sType.IndexOf("(") > 0)
                            {
                                sSize = sType.Substring(sType.IndexOf("("), sType.Length - sType.IndexOf("(")).Replace(")", String.Empty).Replace("(", String.Empty);
                                sType = sType.Substring(0, sType.IndexOf("("));
                            }
                            else
                                sSize = "0";
                            int size;
                            int.TryParse(sSize, out size);
                            column.MaxLength = size;
                            column.DataType = GetDbType(sType);
                            //column.DataType = sType.Substring(0,sType.IndexOf("("));
                            column.AutoIncrement = rdr["Extra"].ToString() == "auto_increment";
                            //string nullable = rdr["Null"].ToString();
                            column.IsNullable = (rdr["Null"].ToString().ToLower() == "yes");
                            column.IsReadOnly = false;
                            columns.Add(column);
                        }
                        rdr.Close();
                    }

                    if(conn.State != ConnectionState.Closed)
                        conn.Close();
                }
                catch
                {
                    //there's no table here
                    //let fall through to return a null
                }
            }

            if(columns.Count > 0)
            {
                table.Columns = columns;
                return table;
            }
            return null;
        }

        /// <summary>
        /// Gets the type of the db.
        /// </summary>
        /// <param name="mySqlType">Type of my SQL.</param>
        /// <returns></returns>
        public override DbType GetDbType(string mySqlType)
        {
            switch(mySqlType.ToLowerInvariant())
            {
                case "longtext":
                case "nchar":
                case "ntext":
                case "text":
                case "sysname":
                case "varchar":
                case "nvarchar":
                    return DbType.String;
                case "bit":
                case "tinyint":
                    return DbType.Boolean;
                case "decimal":
                case "float":
                case "newdecimal":
                case "numeric":
                case "double":
                case "real":
                    return DbType.Decimal;
                case "bigint":
                    return DbType.Int64;
                case "int":
                case "int32":
                case "integer":
                    return DbType.Int32;
                case "int16":
                case "smallint":
                    return DbType.Int16;
                case "date":
                case "time":
                case "datetime":
                case "smalldatetime":
                    return DbType.DateTime;
                case "image":
                case "varbinary":
                case "binary":
                case "blob":
                case "longblob":
                    return DbType.Binary;
                case "char":
                    return DbType.AnsiStringFixedLength;
                case "currency":
                case "money":
                case "smallmoney":
                    return DbType.Currency;
                case "timestamp":
                    return DbType.DateTime;
                case "uniqueidentifier":
                    return DbType.Guid;
                case "uint16":
                    return DbType.UInt16;
                case "uint32":
                    return DbType.UInt32;
                default:
                    return DbType.String;
            }
        }

        /// <summary>
        /// Gets the SP list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSPList() {
            const string sql = "SELECT routine_name FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_SCHEMA = ?databaseName";
            StringBuilder sList = new StringBuilder();

            using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
            {
                if(!SupportsInformationSchema(GetDatabaseVersion(Name)))
                {
                    conn.Connection.Close();
                    return new string[0];
                }

                MySqlCommand cmd = new MySqlCommand(sql, (MySqlConnection)conn.Connection);

                cmd.Parameters.AddWithValue("?databaseName", conn.Connection.Database);

                using(IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    bool isFirst = true;

                    while(rdr.Read())
                    {
                        if(!isFirst)
                            sList.Append('|');

                        isFirst = false;
                        sList.Append(rdr[0]);
                    }
                    rdr.Close();
                }
            }
            return sList.ToString().Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets the SP params.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        public override IDataReader GetSPParams(string spName)
        {
            DataTable parametersDataTable = CreateParameterTable();
            MySqlCommand cmd = new MySqlCommand();
            MySqlConnection conn = (MySqlConnection)CreateConnection();
            cmd.Connection = conn;

            if(!SupportsInformationSchema(conn.ServerVersion))
            {
                conn.Close();
                return null;
            }

            cmd.CommandText = spName;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                MySqlCommandBuilder.DeriveParameters(cmd);
            }
            catch
            {
                //string foo = ex.Message;
            }

            if(cmd.Parameters.Count > 0)
            {
                foreach(MySqlParameter param in cmd.Parameters)
                {
                    DataRow row = parametersDataTable.NewRow();
                    row[0] = param.ParameterName;
                    row[1] = param.MySqlDbType.ToString();
                    row[2] = param.Direction.ToString();

                    parametersDataTable.Rows.Add(row);
                }
                //parametersDataTable.AcceptChanges();
            }
            conn.Close();

            return parametersDataTable.CreateDataReader();
        }

        /// <summary>
        /// Gets the table name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetTableNameList()
        {
            const string sql = "SHOW TABLES";
            StringBuilder sList = new StringBuilder();

            using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
            {
                using(MySqlCommand cmd = new MySqlCommand(sql))
                {
                    //get information about both the table and it's columns
                    cmd.Connection = (MySqlConnection)conn.Connection;

                    if(SupportsInformationSchema(GetDatabaseVersion(Name)))
                        cmd.CommandText = "select table_name from information_schema.tables where table_schema = ?databaseName and table_type <> 'VIEW'";

                    cmd.Parameters.Add("?databaseName", MySqlDbType.VarChar, 64).Value = conn.Connection.Database;

                    using(IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        bool isFirst = true;

                        while(rdr.Read())
                        {
                            if(!isFirst)
                                sList.Append('|');

                            isFirst = false;
                            sList.Append(rdr[0]);
                        }
                        rdr.Close();
                    }
                }
            }

            string[] strArray = sList.ToString().Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(strArray);
            return strArray;
        }

        /// <summary>
        /// Gets the primary key table names.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override ArrayList GetPrimaryKeyTableNames(string tableName)
        {
            // Relationships are only supported in the InnoDB engine.
            // Being that most databases are implemented via MyISAM
            // I will code for majority not minority.
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
            return String.Empty;
        }

        /// <summary>
        /// Gets the name of the foreign key table.
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName)
        {
            return String.Empty;
        }

        /// <summary>
        /// Executes the transaction.
        /// </summary>
        /// <param name="commands">The commands.</param>
        public override void ExecuteTransaction(QueryCommandCollection commands)
        {
            //make sure we have at least one
            if(commands.Count > 0)
            {
                MySqlCommand cmd;

                //a using statement will make sure we close off the connection
                using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
                {
                    //open up the connection and start the transaction
                    if(conn.Connection.State == ConnectionState.Closed)
                        conn.Connection.Open();

                    MySqlTransaction trans = (MySqlTransaction)conn.Connection.BeginTransaction();

                    foreach(QueryCommand qry in commands)
                    {
                        cmd = new MySqlCommand(qry.CommandSql, (MySqlConnection)conn.Connection);
                        cmd.CommandType = qry.CommandType;

                        try
                        {
                            AddParams(cmd, qry);
                            cmd.ExecuteNonQuery();
                        }
                        catch(MySqlException x)
                        {
                            //if there's an error, roll everything back
                            trans.Rollback();

                            //throw the error
                            throw new Exception(x.Message);
                        }
                    }
                    //if we get to this point, we're good to go
                    trans.Commit();
                }
            }
            else
                throw new Exception("No commands present");
        }

        /// <summary>
        /// Scripts the data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public override string ScriptData(string tableName, string providerName)
        {
            // TODO: Implement Method.
            //throw new Exception("The method or operation is not implemented. (ScriptData)");
            return String.Empty;
        }

        /// <summary>
        /// Gets the view name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetViewNameList()
        {
            StringBuilder viewList = new StringBuilder();

            const string sql = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS WHERE TABLE_SCHEMA = ?databaseName";

            using(MySqlCommand cmd = new MySqlCommand())
            {
                MySqlConnection conn = new MySqlConnection(DefaultConnectionString);
                cmd.Connection = conn;
                cmd.CommandText = sql;
                conn.Open();

                if(!SupportsInformationSchema(GetDatabaseVersion(Name)))
                {
                    conn.Close();
                    return new string[0];
                }

                cmd.Parameters.Add("?databaseName", MySqlDbType.String).Value = conn.Database;

                using(IDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while(rdr.Read())
                    {
                        string viewName = rdr[0].ToString();

                        if(String.IsNullOrEmpty(ViewStartsWith) || viewName.StartsWith(ViewStartsWith))
                        {
                            viewList.Append(rdr[0]);
                            viewList.Append("|");
                        }
                    }
                    rdr.Close();
                }

                if(conn.State != ConnectionState.Closed)
                    conn.Close();
            }
            string strList = viewList.ToString();
            string[] result = strList.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(result);
            return result;
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
            MySqlCommand cmd = new MySqlCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);

            AddTableMappings(da, ds);
            using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
            {
                cmd.Connection = conn.GetConnection<MySqlConnection>();
                AddParams(cmd, qry);
                da.Fill(ds);

                CheckoutOutputParams(cmd, qry);

                cmd.Dispose();
                da.Dispose();

                return ds;
            }
        }

        /// <summary>
        /// Private helper method to check the version of MySQL.
        /// 
        /// This is important because MySQL versions prior to 5.x
        /// did not support the standard INFORMATION_SCHEMA views.
        /// </summary>
        /// <param name="VersionLine">Version line returned from the server.</param>
        /// <returns></returns>
        internal static bool SupportsInformationSchema(string VersionLine)
        {
            int majorVersion;
            int.TryParse(VersionLine.Substring(0, 1), out majorVersion);
            return majorVersion > 4;
        }

        /// <summary>
        /// Private helper to create a parameter table for returning
        /// from the GetSPParams method.
        /// 
        /// There is no standard way to get parameters from MySQL 
        /// stored procedures.  We have to do a string level parse to
        /// hack out the parameters.  Unfortunately this does not give
        /// us an IDataReader interface to return from teh GetSPParams
        /// method.  Inorder to provide this interface return we build 
        /// a datatable with the SP Params and returna DataTableReader.
        /// </summary>
        /// <returns></returns>
        internal static DataTable CreateParameterTable()
        {
            DataTable dt = new DataTable("parameters");

            DataColumn dc = new DataColumn("Name", typeof(string));
            dt.Columns.Add(dc);

            dc = new DataColumn("DataType", typeof(string));
            dt.Columns.Add(dc);

            dc = new DataColumn("Mode", typeof(string));
            dt.Columns.Add(dc);

            dt.AcceptChanges();

            return dt;
        }

        /// <summary>
        /// Gets the foreign key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override string[] GetForeignKeyTables(string tableName)
        {
            return new string[] {String.Empty};
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
            return String.Empty;
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        protected override string GetDatabaseVersion(string providerName)
        {
            string retVal = "Unknown";

            AutomaticConnectionScope conn = new AutomaticConnectionScope(this);

            try
            {
                retVal = conn.GetConnection<MySqlConnection>().ServerVersion;
            }
            catch {}
            finally
            {
                if(!conn.IsUsingSharedConnection)
                    conn.Dispose();
            }

            return retVal;
        }


        #region SQL Generation Support

        public override ISqlGenerator GetSqlGenerator(SqlQuery sqlQuery)
        {
            return new MySqlGenerator(sqlQuery);
        }

        #endregion


        #region SQL Builders

        /// <summary>
        /// Helper method to build out the limit string of a given query.
        /// </summary>
        /// <param name="qry">Query to build the limit string from.</param>
        /// <returns></returns>
        private string GetLimit(Query qry)
        {
            string limit = String.Empty;

            // We will only implement the top function
            // when we are not paging. Sorry it is too
            // sticky otherwise.  Maybe if I get more 
            // time I will try to work out using top and
            // paging, but for the time being this should
            // suffice.
            if(qry.PageIndex == -1)
            {
                // By default MySQL will return 100% of the results
                // there is no need to apply a limit so we will
                // return an empty string.
                if(qry.Top == "100 PERCENT" || String.IsNullOrEmpty(qry.Top))
                    return limit;

                // If the Top property of the query contains either
                // a % character or the word percent we need to do
                // some extra work
                if(qry.Top.Contains("%") || qry.Top.ToLower().Contains("percent"))
                {
                    // strip everything but the numeric portion of
                    // the top property.
                    limit = qry.Top.ToLower().Replace("%", String.Empty).Replace("percent", String.Empty).Trim();

                    // we will try/catch just incase something fails
                    // fails a conversion.  This gives us an easy out
                    try
                    {
                        // Convert the percetage to a decimal
                        decimal percentTop = Convert.ToDecimal(limit) / 100;

                        // Get the total count of records to
                        // be returned.
                        int count = GetRecordCount(qry);

                        // Using the new decimal and the amount
                        // of records to be returned calculate
                        // what percentage of the records are
                        // to be returned
                        limit = " LIMIT " + Convert.ToString((int)(count * percentTop));
                    }
                    catch
                    {
                        // If something fails in the try lets
                        // just return an empty string and
                        // move on.
                        limit = String.Empty;
                    }
                }
                    // The top parameter only contains an integer.
                    // Wrap the integer in the limit string and return.
                else
                    limit = " LIMIT " + qry.Top;
            }
                // Paging in MySQL is actually quite simple. 
                // Using limit we will set the starting record 
                // to PageIndex * PageSize and the amount of 
                // records returned to PageSize.
            else
            {
                int start = (qry.PageIndex - 1) * qry.PageSize;
                limit = string.Format(" LIMIT {0},{1} ", start, qry.PageSize);
            }

            return limit;
        }

        /// <summary>
        /// Gets the select SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override string GetSelectSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;

            //different rules for how to do TOP
            string select = SqlFragment.SELECT;
            select += qry.IsDistinct ? SqlFragment.DISTINCT : String.Empty;

            const string groupBy = "";
            string order = String.Empty;
            const string join = "";

            select += qry.SelectList;
            select += " FROM `" + table.Name + "`";

            string where = BuildWhere(qry);

            if(qry.OrderByCollection.Count > 0)
            {
                order += SqlFragment.ORDER_BY;
                for(int j = 0; j < qry.OrderByCollection.Count; j++)
                {
                    string orderString = qry.OrderByCollection[j].OrderString;
                    if(!String.IsNullOrEmpty(orderString))
                    {
                        order += orderString;
                        if(j + 1 != qry.OrderByCollection.Count)
                            order += ", ";
                    }
                }
                order = order.Replace("[", String.Empty);
                order = order.Replace("]", String.Empty);
            }

            string limit = GetLimit(qry);

            string query = select + join + groupBy + where + order + limit;
            return query + ";";
        }

        /// <summary>
        /// Gets the insert SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override string GetInsertSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;

            //split the TablNames and loop out the SQL
            string insertSQL = String.Format("INSERT INTO `{0}`", table.Name);
            //string client = DataService.GetClientType();

            string cols = String.Empty;
            string pars = String.Empty;

            //int loopCount = 1;

            //if table columns are null toss an exception
            foreach(TableSchema.TableColumn col in table.Columns)
            {
                if(!col.AutoIncrement && !col.IsReadOnly)
                {
                    cols += col.ColumnName + ",";
                    pars += "?" + col.ColumnName + ",";
                }
            }
            cols = cols.Remove(cols.Length - 1, 1);
            pars = pars.Remove(pars.Length - 1, 1);
            insertSQL += "(" + cols + ") ";

            insertSQL += "VALUES(" + pars + ");";

            //get the newly-inserted ID
            //insertSQL += " SELECT MAX(" + table.PrimaryKey.ColumnName + ") as newID;";
            insertSQL += " SELECT LAST_INSERT_ID() as newID;";

            return insertSQL;
        }

        #endregion
    }
}

#endif