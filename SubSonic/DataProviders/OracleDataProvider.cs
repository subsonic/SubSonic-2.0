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
// By Default we always want Oracle
//#if ALLPROVIDERS
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Text;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the OracleDataProvider class
    /// </summary>
    public class OracleDataProvider : DataProvider
    {
        private const string COLUMN_NAME_PARAMETER = ":columnName";
        private const string DELIMITER = "|";
        private const string END_PARAMETER = ":end";
        private const string MAP_SUFFIX = ":mapSuffix";
        private const string OBJECT_NAME_PARAMETER = ":objectName";
        private const string START_PARAMETER = ":start";
        private const string TABLE_NAME_PARAMETER = ":tableName";

        /// <summary>
        /// Gets the type of the named provider.
        /// </summary>
        /// <value>The type of the named provider.</value>
        public override string NamedProviderType
        {
            get { return DataProviderTypeName.ORACLE; }
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
            OracleConnection retVal = new OracleConnection(newConnectionString);
            retVal.Open();
            return retVal;
        }

        /// <summary>
        /// Checkouts the output params.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="qry">The qry.</param>
        private static void CheckoutOutputParams(OracleCommand cmd, QueryCommand qry)
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
        private static void AddParams(OracleCommand cmd, QueryCommand qry)
        {
            if(qry.Parameters != null)
            {
                foreach(QueryParameter param in qry.Parameters)
                {
                    OracleParameter sqlParam = new OracleParameter();
                    sqlParam.DbType = param.DataType;
                    sqlParam.Direction = param.Mode;
                    sqlParam.OracleType = GetOracleType(param.DataType);
                    sqlParam.ParameterName = param.ParameterName;
                    sqlParam.Value = param.ParameterValue;

                    if (param.Mode == ParameterDirection.Output ||
                        param.Mode == ParameterDirection.InputOutput)
                    {
                        sqlParam.Size = param.Size;
                    }

                    cmd.Parameters.Add(sqlParam);
                }
            }
        }

        /// <summary>
        /// Gets the type of the oracle.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <returns></returns>
        public static OracleType GetOracleType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                    return OracleType.VarChar;
                case DbType.AnsiStringFixedLength:
                    return OracleType.Char;
                case DbType.Binary:
                    return OracleType.Raw;
                case DbType.Boolean:
                    return OracleType.Byte;
                case DbType.Byte:
                    return OracleType.Byte;
                case DbType.Currency:
                    return OracleType.Number;
                case DbType.Date:
                    return OracleType.DateTime;
                case DbType.DateTime:
                    return OracleType.DateTime;
                case DbType.Decimal:
                    return OracleType.Number;
                case DbType.Double:
                    return OracleType.Double;
                case DbType.Guid:
                    return OracleType.Raw;
                case DbType.Int16:
                    return OracleType.Int16;
                case DbType.Int32:
                    return OracleType.Int32;
                case DbType.Int64:
                    return OracleType.Number;
                case DbType.Object:
                    return OracleType.Cursor;
                case DbType.SByte:
                    return OracleType.SByte;
                case DbType.Single:
                    return OracleType.Float;
                case DbType.String:
                    return OracleType.NVarChar;
                case DbType.StringFixedLength:
                    return OracleType.NChar;
                case DbType.Time:
                    return OracleType.DateTime;
                case DbType.UInt16:
                    return OracleType.UInt16;
                case DbType.UInt32:
                    return OracleType.UInt32;
                case DbType.UInt64:
                    return OracleType.Number;
                case DbType.VarNumeric:
                    return OracleType.Number;

                default:
                    {
                    return OracleType.VarChar;
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
            parameter.DBType = GetDbType(dataReader[OracleSchemaVariable.DATA_TYPE].ToString());
            string sMode = dataReader[OracleSchemaVariable.MODE].ToString();

            if(sMode == OracleSchemaVariable.MODE_INOUT)
                parameter.Mode = ParameterDirection.InputOutput;

            parameter.Name = dataReader[OracleSchemaVariable.NAME].ToString();
        }

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        /// <returns></returns>
        public override string GetParameterPrefix()
        {
            return OracleSchemaVariable.PARAMETER_PREFIX;
        }

        /// <summary>
        /// Adjusts the update SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <param name="table">The table.</param>
        /// <param name="updateSql">The update SQL.</param>
        /// <returns></returns>
        protected override string AdjustUpdateSql(Query qry, TableSchema.Table table, string updateSql)
        {
            return updateSql;
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDataReader GetReader(QueryCommand qry)
        {
            AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this);
            OracleCommand cmd = new OracleCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;

            AddParams(cmd, qry);
            cmd.Connection = (OracleConnection)automaticConnectionScope.Connection;

            IDataReader rdr;

            try
            {
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch(OracleException)
            {
                automaticConnectionScope.Dispose();
                throw;
            }

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
            OracleCommand cmd = new OracleCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;

            AddParams(cmd, qry);
            cmd.Connection = (OracleConnection)conn.Connection;

            IDataReader rdr;
            // if it is a shared connection, we shouldn't be telling the reader to close it when it is done
            if (conn.IsUsingSharedConnection)
                rdr = cmd.ExecuteReader();
            else
                rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult | CommandBehavior.SingleRow);
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
            OracleCommand cmd = new OracleCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
            {
                cmd.Connection = (OracleConnection)conn.Connection;
                AddParams(cmd, qry);
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
        {// okay so this is a hack but I wanted to test this out I am going to try to retriev the id view the returning...
            using(AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
            {
                
                OracleCommand cmd = new OracleCommand(qry.CommandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout;
                AddParams(cmd, qry);
                cmd.Connection = (OracleConnection)automaticConnectionScope.Connection;
                object o;
                //BFerrier this is a hack to get the generated ID Back from Oracle
                if (qry.CommandSql.Contains("INTO :lllhhhmmm; COMMIT; END;"))
                {
                    OracleParameter op = new OracleParameter();
                    op.Direction = ParameterDirection.Output;
                    op.ParameterName = ":lllhhhmmm";
                    op.DbType = DbType.Decimal;
                    cmd.Parameters.Add(op);
                    cmd.ExecuteNonQuery();
                    o = cmd.Parameters[":lllhhhmmm"].Value;
                }
                else
                    o = cmd.ExecuteScalar();
                return o;
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
                OracleCommand cmd = new OracleCommand(qry.CommandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout;
                AddParams(cmd, qry);
                cmd.Connection = (OracleConnection)automaticConnectionScope.Connection;
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
            TableSchema.Table tbl = new TableSchema.Table(tableName, tableType, this);
            //tbl.ClassName = Convention.ClassName(tableName);
            //string sql = TABLE_COLUMN_SQL;
            QueryCommand cmd = new QueryCommand(TABLE_COLUMN_SQL, Name);
            cmd.AddParameter(TABLE_NAME_PARAMETER, tableName, DbType.AnsiString);
            TableSchema.TableColumn column;

            using(IDataReader rdr = DataService.GetReader(cmd))
            {
                //get information about both the table and it's columns                
                while(rdr.Read())
                {
                    tbl.SchemaName = rdr["USER"].ToString();

                    column = new TableSchema.TableColumn(tbl);
                    column.ColumnName = rdr[OracleSchemaVariable.COLUMN_NAME].ToString();

                    string scale = rdr[OracleSchemaVariable.NUMBER_SCALE].ToString();
                    string precision = rdr[OracleSchemaVariable.NUMBER_PRECISION].ToString();

                    column.NumberScale = 0;
                    column.NumberPrecision = 0;

                    if(!String.IsNullOrEmpty(scale) && scale != "0")
                        column.NumberScale = int.Parse(scale);

                    if(!String.IsNullOrEmpty(precision) && precision != "0")
                        column.NumberPrecision = int.Parse(precision);

                    column.DataType = GetDbType(rdr[OracleSchemaVariable.DATA_TYPE].ToString().ToLower());
                    column.AutoIncrement = false;
                    int maxLength;
                    int.TryParse(rdr[OracleSchemaVariable.MAX_LENGTH].ToString(), out maxLength);
                    column.MaxLength = maxLength;
                    column.IsNullable = Utility.IsMatch(rdr[OracleSchemaVariable.IS_NULLABLE].ToString(), "Y");
                    column.IsReadOnly = false;
                    columns.Add(column);
                }
            }

            cmd.CommandSql = INDEX_SQL;
            //cmd.AddParameter(TABLE_NAME_PARAMETER, tableName);

            using(IDataReader rdr = DataService.GetReader(cmd))
            {
                while(rdr.Read())
                {
                    string colName = rdr[OracleSchemaVariable.COLUMN_NAME].ToString();
                    string constraintType = rdr[OracleSchemaVariable.CONSTRAINT_TYPE].ToString();
                    column = columns.GetColumn(colName);

                    if(constraintType == SqlSchemaVariable.PRIMARY_KEY)
                        column.IsPrimaryKey = true;
                    else if(constraintType == SqlSchemaVariable.FOREIGN_KEY)
                        column.IsForeignKey = true;

                    //HACK: Allow second pass naming adjust based on whether a column is keyed
                    column.ColumnName = column.ColumnName;
                }
                rdr.Close();
            }
            if(columns.Count > 0)
            {
                tbl.Columns = columns;
                return tbl;
            }
            return null;
        }

        /// <summary>
        /// Gets the SP params.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        public override IDataReader GetSPParams(string spName)
        {
            QueryCommand cmd = new QueryCommand(SP_PARAM_SQL, Name);
            cmd.AddParameter(OBJECT_NAME_PARAMETER, spName, DbType.AnsiString);
            return DataService.GetReader(cmd);
        }

        /// <summary>
        /// Gets the SP list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSPList()
        {
            QueryCommand cmd = new QueryCommand(SP_SQL, Name);
            StringBuilder sList = new StringBuilder();
            using(IDataReader rdr = DataService.GetReader(cmd))
            {
                bool isFirst = true;
                while(rdr.Read())
                {
                    if(!isFirst)
                        sList.Append(DELIMITER);

                    isFirst = false;
                    sList.Append(rdr[0]);
                }
                rdr.Close();
                return sList.ToString().Split(DELIMITER.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Gets the view name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetViewNameList()
        {
            QueryCommand cmd = new QueryCommand(VIEW_SQL, Name);
            StringBuilder sList = new StringBuilder();
            using(IDataReader rdr = DataService.GetReader(cmd))
            {
                while(rdr.Read())
                {
                    string viewName = rdr[SqlSchemaVariable.NAME].ToString();

                    if(String.IsNullOrEmpty(ViewStartsWith) || viewName.StartsWith(ViewStartsWith))
                    {
                        sList.Append(rdr[SqlSchemaVariable.NAME]);
                        sList.Append("|");
                    }
                }
                rdr.Close();
            }
            string[] strArray = sList.ToString().Split(DELIMITER.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Array.Sort(strArray);
            return strArray;
        }

        /// <summary>
        /// Gets the table name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetTableNameList()
        {
            QueryCommand cmd = new QueryCommand(TABLE_SQL, Name);
            StringBuilder sList = new StringBuilder();
            using(IDataReader rdr = DataService.GetReader(cmd))
            {
                bool isFirst = true;
                while(rdr.Read())
                {
                    if(!isFirst)
                        sList.Append(DELIMITER);

                    isFirst = false;
                    sList.Append(rdr[SqlSchemaVariable.NAME]);
                }
                rdr.Close();
            }
            string[] strArray = sList.ToString().Split(DELIMITER.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
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
            QueryCommand cmd = new QueryCommand(GET_PRIMARY_KEY_SQL, Name);
            cmd.AddParameter(TABLE_NAME_PARAMETER, tableName, DbType.AnsiString);
            ArrayList list = new ArrayList();

            using(IDataReader rdr = DataService.GetReader(cmd))
            {
                while(rdr.Read())
                    list.Add(new string[] {rdr[SqlSchemaVariable.TABLE_NAME].ToString(), rdr[SqlSchemaVariable.COLUMN_NAME].ToString()});

                rdr.Close();
            }
            return list;
        }

        /// <summary>
        /// Gets the primary key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override TableSchema.Table[] GetPrimaryKeyTables(string tableName)
        {
            QueryCommand cmd = new QueryCommand(GET_PRIMARY_KEY_SQL, Name);
            cmd.AddParameter(TABLE_NAME_PARAMETER, tableName, DbType.AnsiString);
            ArrayList names = new ArrayList();

            using(IDataReader rdr = GetReader(cmd))
            {
                while(rdr.Read())
                    names.Add(rdr[SqlSchemaVariable.TABLE_NAME].ToString());
                rdr.Close();
            }

            if(names.Count > 0)
            {
                TableSchema.Table[] tables = new TableSchema.Table[names.Count];

                for(int i = 0; i < names.Count; i++)
                    tables[i] = DataService.GetSchema((string)names[i], Name, TableType.Table);

                return tables;
            }
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
            QueryCommand cmd = new QueryCommand(GET_FOREIGN_KEY_SQL, Name);
            cmd.AddParameter(COLUMN_NAME_PARAMETER, fkColumnName, DbType.AnsiString);
            cmd.AddParameter(TABLE_NAME_PARAMETER, tableName, DbType.AnsiString);

            object result = DataService.ExecuteScalar(cmd);
            if(result == null)
                return null;

            return result.ToString();
        }

        /// <summary>
        /// Gets the name of the foreign key table.
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName)
        {
            QueryCommand cmd = new QueryCommand(GET_TABLE_SQL, Name);
            cmd.AddParameter(COLUMN_NAME_PARAMETER, fkColumnName, DbType.AnsiString);

            object result = DataService.ExecuteScalar(cmd);

            if(result == null)
                return null;

            return result.ToString();
        }

        /// <summary>
        /// Maps Oracle-specific data types to generic System.Data.DbType types.
        /// </summary>
        /// <param name="sqlType">Oracle data type</param>
        /// <returns>DbType enum representing the specified Oracle data type.</returns>
        public override DbType GetDbType(string sqlType)
        {
            switch(sqlType)
            {
                case "char":
                case "varchar":
                case "varchar2":
                case "clob":
                    return DbType.AnsiString;
                case "nchar":
                case "nvarchar2":
                case "nclob":
                case "rowid": //Not sure about ROWID
                    return DbType.String;                
                case "number":
                    return DbType.Decimal;
                case "float":
                    return DbType.Double;
                case "raw":
                case "long raw":
                case "blob":
                    return DbType.Binary;
                case "date":
                    return DbType.DateTime;
                default:
                    //For whatever reason, Oracle9i (+ others?) stores the 
                    //precision with certain datatypes. Ex: "timestamp(3)"
                    //So having "timestamp" as a case statement will not work.
                    if(sqlType.StartsWith("timestamp"))
                        return DbType.DateTime;
                    else if (sqlType.StartsWith("interval"))
                        return DbType.String; //No idea how to handle this one
                    else
                        return DbType.String;
            }
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDbCommand GetCommand(QueryCommand qry)
        {
            OracleCommand cmd = new OracleCommand(qry.CommandSql);
            AddParams(cmd, qry);
            return cmd;
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
                OracleCommand cmd;

                //a using statement will make sure we close off the connection
                using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
                {
                    //open up the connection and start the transaction

                    if(conn.Connection.State == ConnectionState.Closed)
                        conn.Connection.Open();

                    OracleTransaction trans = (OracleTransaction)conn.Connection.BeginTransaction();

                    foreach(QueryCommand qry in commands)
                    {
                        cmd = new OracleCommand(qry.CommandSql, (OracleConnection)conn.Connection);
                        cmd.CommandType = qry.CommandType;
                        cmd.Transaction = trans;

                        AddParams(cmd, qry);

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch(OracleException x)
                        {
                            //if there's an error, roll everything back
                            trans.Rollback();

                            //throw the error retaining the stack.
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
        /// Gets the foreign key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override string[] GetForeignKeyTables(string tableName)
        {
            return new string[] {String.Empty};
        }

        /// <summary>
        /// Gets the table name by primary key.
        /// </summary>
        /// <param name="pkName">Name of the pk.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public override string GetTableNameByPrimaryKey(string pkName, string providerName)
        {
            return String.Empty;
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        protected override string GetDatabaseVersion(string providerName)
        {
            return "Unknown";
        }


        #region SQL Generation Support

        public override ISqlGenerator GetSqlGenerator(SqlQuery sqlQuery)
        {
            return new OracleGenerator(sqlQuery);
        }

        #endregion


        #region SQL Builders

        /// <summary>
        /// Gets the select SQL.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override string GetSelectSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;

            string select = SqlFragment.SELECT;
            select += qry.IsDistinct ? SqlFragment.DISTINCT : String.Empty;
            StringBuilder order = new StringBuilder();
            string query;
            string columns;

            if(!String.IsNullOrEmpty(qry.SelectList) && qry.SelectList.Trim().Length >= 2)
                columns = qry.SelectList;
            else
                columns = GetQualifiedSelect(table);

            string where = BuildWhere(qry);

            //Finally, do the orderby 
            if(qry.OrderByCollection.Count > 0)
            {
                order.Append(SqlFragment.ORDER_BY);
                for(int j = 0; j < qry.OrderByCollection.Count; j++)
                {
                    string orderString = qry.OrderByCollection[j].OrderString;
                    if(!String.IsNullOrEmpty(orderString))
                    {
                        order.Append(orderString);
                        if(j + 1 != qry.OrderByCollection.Count)
                            order.Append(", ");
                    }
                }
            }
            else
            {
                if(table.PrimaryKey != null)
                    order.Append(SqlFragment.ORDER_BY + OrderBy.Asc(table.PrimaryKey.ColumnName).OrderString);
            }

            if(order.Length > 0)
            {
                order = order.Replace("[", String.Empty);
                order = order.Replace("]", String.Empty);
            }

            if(qry.PageIndex < 0)
                query = String.Format("{0} {1} FROM {2}.{3} {4} {5}",select ,columns, table.SchemaName, table.Name, where, order);
            else
            {
                int start = qry.PageIndex * qry.PageSize;
                int end = (qry.PageIndex + 1) * qry.PageSize;

                const string cteFormat =
                    "WITH pagedtable AS (SELECT {0}, ROW_NUMBER () OVER ({1}) AS rowindex FROM {2}.{3} {4}) SELECT {5}, rowindex FROM pagedtable WHERE rowindex >= {6} AND rowindex < {7} ORDER BY rowindex";
                query = string.Format(cteFormat, columns, order,table.SchemaName, table.Name, where, columns.Replace(table.Name + ".", String.Empty), start, end);
            }
            return query;
        }

        /// <summary>
        /// Returns a qualified list of columns ([Table].[Column])
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        protected static string GetQualifiedSelect(TableSchema.Table table)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach(TableSchema.TableColumn tc in table.Columns)
            {
                if(!isFirst)
                    sb.Append(", ");

                isFirst = false;
                sb.AppendFormat("{0}.{1}", table.Name, tc.ColumnName);
            }

            return sb.ToString();
        }
        /// <summary>
        /// Method used to check for a Blob Field in the table
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        public Boolean containsBinaryField(TableSchema.Table table)
        {
            return table.Columns.Exists(delegate(TableSchema.TableColumn column) { return column.DataType == DbType.Binary; });
        }
        /// <summary>
        /// Loops the TableColums[] array for the object, creating a SQL string
        /// for use as an INSERT statement
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override string GetInsertSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;

            //split the TablNames and loop out the SQL
            StringBuilder insertSQL = new StringBuilder();
            //BFerrier I have converted the Insert statement into an anonymous block that returns the newly created records ID
            //After it has been inserted
            Boolean retrieveID = (qry.Schema.PrimaryKeys.Length == 1 && qry.Schema.PrimaryKey.DataType == DbType.Decimal && !this.containsBinaryField(qry.Schema));
            if(retrieveID)
                insertSQL.Append("BEGIN ");
            insertSQL.Append("INSERT INTO ");
            insertSQL.Append(table.Name);
            //string client = DataService.GetClientType();

            StringBuilder cols = new StringBuilder();
            StringBuilder pars = new StringBuilder();
            //OS not used with ORACLE bool primaryKeyIsGuid = false;
            //string primaryKeyName = "";

            //if table columns are null toss an exception
            bool isFirst = true;

            foreach(TableSchema.TableColumn col in table.Columns)
            {
                if(!col.AutoIncrement && !col.IsReadOnly)
                {
                    if(!isFirst)
                    {
                        cols.Append(",");
                        pars.Append(",");
                    }
                    isFirst = false;
                    cols.Append(col.ColumnName);
                    pars.Append(FormatParameterNameForSQL(col.ColumnName));
                }
            }
            insertSQL.Append("(");
            insertSQL.Append(cols);
            insertSQL.Append(") ");

            // OS : can't user for ORACLE string getInsertValue = " SELECT SCOPE_IDENTITY() as newID;";

            insertSQL.Append("VALUES(");
            insertSQL.Append(pars);
            insertSQL.Append(")");

            // OS : can't user for ORACLE insertSQL += " SELECT SCOPE_IDENTITY() as newID;";
            // OS : it seems this method is not used, however if used later, witth ORACLE one 
            // could use the RETURNING clause of and INSERT statement to get a new TRIGGER id generated
            // sequence or counter for primary key purpose
            // insertSQL += getInsertValue;
            if (retrieveID)
                insertSQL.AppendFormat("  RETURNING {0} INTO :lllhhhmmm; COMMIT; END; ", qry.Schema.PrimaryKey.ColumnName);
            return insertSQL.ToString();
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
            StringBuilder result = new StringBuilder();
            if(CodeService.ShouldGenerate(tableName, providerName))
            {
                StringBuilder fieldList = new StringBuilder();
                StringBuilder statements = new StringBuilder();
                StringBuilder insertStatement = new StringBuilder();

                insertStatement.Append("INSERT INTO ");
                insertStatement.Append(tableName);
                insertStatement.Append(" ");

                //pull the schema for this table
                TableSchema.Table table = Query.BuildTableSchema(tableName, providerName);

                //build the insert list.
                bool isFirst = true;
                foreach(TableSchema.TableColumn col in table.Columns)
                {
                    if(!isFirst)
                        fieldList.Append(",");

                    isFirst = false;
                    fieldList.Append(col.ColumnName);
                }

                //complete the insert statement
                insertStatement.Append("(");
                insertStatement.Append(fieldList.ToString());
                insertStatement.AppendLine(")");

                //get the table data
                using(IDataReader rdr = new Query(table).ExecuteReader())
                {
                    while(rdr.Read())
                    {
                        StringBuilder thisStatement = new StringBuilder();
                        thisStatement.Append(insertStatement);
                        thisStatement.Append("VALUES(");
                        //loop the schema and pull out the values from the reader
                        isFirst = true;
                        foreach(TableSchema.TableColumn col in table.Columns)
                        {
                            if(!isFirst)
                                thisStatement.Append(",");

                            isFirst = false;

                            if(Utility.IsNumeric(col))
                                thisStatement.Append(rdr[col.ColumnName]);
                            else
                            {
                                thisStatement.Append("'");
                                thisStatement.Append(rdr[col.ColumnName].ToString().Replace("'", "''"));
                            }
                        }
                        //add in a closing paren
                        thisStatement.AppendLine(")");
                        statements.Append(thisStatement.ToString());
                    }
                    rdr.Close();
                }

                result.Append("PRINT 'Begin inserting data in ");
                result.Append(tableName);
                result.AppendLine("'");

                result.Append(statements.ToString());
            }

            return result.ToString();
        }

        #endregion


        #region Schema Bits

        private const string GET_FOREIGN_KEY_SQL = "SELECT d.table_name " +
                                                   " FROM user_cons_columns c, user_constraints d, user_constraints e " +
                                                   " WHERE d.constraint_name = e.r_constraint_name " +
                                                   " AND c.constraint_name = e.constraint_name " +
                                                   " AND c.column_name = :columnName " +
                                                   " AND e.table_name = :tableName ";

        private const string GET_PRIMARY_KEY_SQL = "SELECT e.table_name AS TableName, c.column_name AS ColumnName " +
                                                   "  FROM user_cons_columns c, user_cons_columns d, user_constraints e " +
                                                   " WHERE d.constraint_name = e.r_constraint_name " +
                                                   "   AND c.constraint_name = e.constraint_name " +
                                                   "   AND d.table_name = :tableName ";

        private const string GET_TABLE_SQL = "SELECT b.table_name " +
                                             "  FROM user_constraints a, user_cons_columns b " +
                                             " WHERE a.constraint_name = b.constraint_name " +
                                             "   AND a.constraint_type IN ('R', 'P') " +
                                             "   AND b.column_name = :columnName " +
                                             "   AND a.constraint_type = 'P' ";

        private const string INDEX_SQL = "SELECT b.table_name, b.column_name, " +
                                         "       DECODE (a.constraint_type, " +
                                         "               'R', 'FOREIGN KEY', " +
                                         "               'P', 'PRIMARY KEY', " +
                                         "               'UNKNOWN' " +
                                         "              ) constraint_type " +
                                         "  FROM user_constraints a, user_cons_columns b " +
                                         " WHERE a.constraint_name = b.constraint_name " +
                                         "   AND a.constraint_type IN ('R', 'P') " +
                                         "   AND b.table_name = :tableName ";

        private const string MANY_TO_MANY_LIST = "SELECT b.table_name FROM user_constraints a, user_cons_columns b " +
                                                 "WHERE a.table_name = :tableName " +
                                                 "AND a.constraint_type = 'R' " +
                                                 "AND a.r_constraint_name = b.constraint_name " +
                                                 "AND b.table_name like '%' + :mapSuffix";

        private const string SP_PARAM_SQL =
            @"SELECT a.object_name, a.object_type, b.position, b.in_out, 
                                    b.argument_name, b.data_type, b.char_length, b.data_precision, b.data_scale 
                                    FROM user_objects a, user_arguments b 
                                    WHERE a.object_type IN ('PROCEDURE', 'PACKAGE') 
                                    AND a.object_id = b.object_id 
                                    AND a.object_name = :objectName";

        private const string SP_SQL =
            @"SELECT a.object_name, a.object_type, a.created, a.last_ddl_time 
                                FROM user_objects a 
                                WHERE a.object_type IN ('PROCEDURE', 'PACKAGE') 
                                ORDER BY a.object_name";

        private const string TABLE_COLUMN_SQL = "SELECT user, a.table_name, a.column_name, a.column_id, a.data_default, " +
                                                "       a.nullable, a.data_type, a.char_length, a.data_precision, a.data_scale " +
                                                "  FROM user_tab_columns a " +
                                                " WHERE a.table_name = :tableName";

        private const string TABLE_SQL = "SELECT a.table_name AS Name FROM user_tables a";

        private const string VIEW_SQL = "SELECT a.view_name AS Name FROM user_views a";

        #endregion
    }
}

//#endif
