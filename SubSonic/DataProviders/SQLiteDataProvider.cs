#if ALLPROVIDERS

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Text;
using SubSonic.Utilities;
using System.Diagnostics;

namespace SubSonic
{
    /// <summary>
    /// A Data Provider for SQLite. 
    /// 
    /// Loosely based on the MySQL data provider (by Larry Beall) with
    /// lots of consultation of the SqlDataProvider (no name attributed).
    /// 
    /// Started on 07/2007 - CodeForNothing
    /// 
    /// Release of Version 0.1 - 18/07/2007 - CodeForNothing
    /// Code complete (v0.3) - 23/07/2007 - CodeForNothing
    ///
    /// Code changes Jan 19 2009 - Paul Shaffer
    ///    It still won't do many-to-many tables.
    ///
    /// Code changes Apr 11 2009 - Paul Shaffer
    ///    There is an accompanying test project 'Subsonic.Test_sqlite' with a SQLite version
    ///    of the Northwind database.
    ///    Results: 222 run, 222 passed, 0 failed, 0 inconclusive, 8 skipped (8 ignored)
    ///    The tests that are failing are related to SharedDBConnection scope, it currently
    ///    doesn't appear to work with SQLite.
    ///
    ///    The only way I could find to pass most of the tests without locking 
    ///    errors was to never close the connection. This is exactly the opposite
    ///    approach taken in the sql server provider. SQLite only allows 1 shared
    ///    connection at a time, and there is some problem with repeated open/close
    ///    cycles on the database file here.
    ///
    /// </summary>
    public class SQLiteDataProvider: DataProvider   
    {
        private DataTable _types = null;
        private DataTable _columns = null;
        private DataTable _foreignkeys;
        private DataTable _indexes;
        private string _catalog = "main";
        private SQLiteConnection _conn;

        /// <summary>
        /// Gets the type of the named provider.
        /// </summary>
        /// <value>The type of the named provider.</value>
        public override string NamedProviderType
        {
            get { return DataProviderTypeName.SQLITE; }
        }

        /// <summary>
        /// Force-reloads a provider's schema
        /// </summary>
        public override void ReloadSchema()
        {
            if(_types != null)
                _types.Clear();
            if(_columns != null)
                _columns.Clear();
            if(_foreignkeys != null)
                _foreignkeys.Clear();
            if(_indexes != null)
                _indexes.Clear();
        }


        /// <summary>
        /// Catalog of the current database. main by default.
        /// </summary>
        public string Catalog
        {
            get { return _catalog; }
            set { _catalog = value; }
        }

        /// <summary>
        /// Load and cache all foreign keys
        /// </summary>
        /// <returns></returns>
        private DataTable AllForeignKeys
        {
            get
            {
                //using(SQLiteConnection con = (SQLiteConnection)CreateConnection())
                SQLiteConnection con = (SQLiteConnection)CreateConnection();
                _foreignkeys = con.GetSchema("FOREIGNKEYS");

                return _foreignkeys;
            }

        }

        // MetaDataCollections
        private DataTable MetaDataCollections
        {
            get
            {
                DataTable data;
                //using(SQLiteConnection con = (SQLiteConnection)CreateConnection())
                SQLiteConnection con = (SQLiteConnection)CreateConnection();
                data = con.GetSchema("MetaDataCollections");

                return data;
            }

        }

        private DataTable AllIndexes
        {
            get
            {
                //using(SQLiteConnection con = (SQLiteConnection)CreateConnection())
                SQLiteConnection con = (SQLiteConnection)CreateConnection();
                _indexes = con.GetSchema("indexes");

                return _indexes;
            }

        }

        private DataTable AllColumns
        {
            get
            {
                //using(SQLiteConnection con = (SQLiteConnection)CreateConnection())
                SQLiteConnection con = (SQLiteConnection)CreateConnection();
                _columns = con.GetSchema("COLUMNS");

                return _columns;
            }
        }

        public override DbConnection CreateConnection()
        {
            return CreateConnection(DefaultConnectionString);
        }
        
        public override DbConnection CreateConnection(string newConnectionString)
        {
            if(_conn == null)
                _conn = new SQLiteConnection(newConnectionString);

            if(_conn.State != ConnectionState.Open)
                _conn.Open();

            return _conn;

        }

        /// <summary>
        /// Add the Query parameters to a SQLiteCommand command
        /// </summary>
        /// <param name="qry"></param>
        /// <param name="cmd"></param>
        static void AddParams(QueryCommand qry, SQLiteCommand cmd)
        {
            if(qry.Parameters != null)
            {
                foreach(QueryParameter param in qry.Parameters)
                {
                    SQLiteParameter sqlParam = new SQLiteParameter();
                    sqlParam.DbType = param.DataType;
                    sqlParam.ParameterName = param.ParameterName; //.Replace('@', '?');
                    sqlParam.Value = param.ParameterValue;

                    cmd.Parameters.Add(sqlParam);
                }
            }
        }

        /// <summary>
        /// SQLite does not support Stored Procedures.
        /// </summary>
        /// <param name="rdr"></param>
        /// <param name="par"></param>
        public override void SetParameter(IDataReader rdr, StoredProcedure.Parameter par)
        {
        }

        /// <summary>
        /// This is a simple question mark for SQLite.
        /// </summary>
        /// <returns></returns>
        public override string GetParameterPrefix()
        {
            return "@";
        }

        /// <summary>
        /// The delimiters are the single quote
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public override string DelimitDbName(string columnName)
        {
            if(!String.IsNullOrEmpty(columnName))
            {
                return "`" + columnName + "`";
            }
            return String.Empty;
        }

        /// <summary>
        /// Return a command from a Query.
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        public override IDbCommand GetCommand(QueryCommand qry)
        {
            SQLiteCommand cmd = new SQLiteCommand(qry.CommandSql);
            AddParams(qry, cmd);
            return cmd;
        }

        /// <summary>
        /// Return a IDataReader from a Query.
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        public override IDataReader GetReader(QueryCommand qry)
        {
            SQLiteConnection conn = (SQLiteConnection)CreateConnection();

            SQLiteCommand cmd = new SQLiteCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;

            AddParams(qry, cmd);

            cmd.Connection = conn;

            return cmd.ExecuteReader();
        }

        public override IDataReader GetSingleRecordReader(QueryCommand qry)
        {
            return GetReader(qry);
        }

        /// <summary>
        /// Return a Dataset from a Query.
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        public override DataSet GetDataSet(QueryCommand qry)
        {
            DataSet ds = new DataSet();
            SQLiteCommand cmd = new SQLiteCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;

            AddParams(qry, cmd);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

            SQLiteConnection conn = (SQLiteConnection)CreateConnection();
            //using(SQLiteConnection conn = (SQLiteConnection)CreateConnection())
            {
                cmd.Connection = conn;
                try
                {
                    cmd.Connection.Open();
                }
                catch
                {

                }

                da.Fill(ds);

                cmd.Dispose();
                da.Dispose();

                return ds;
            }
        }

        /// <summary>
        /// Return a scalar from a Query.
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        public override object ExecuteScalar(QueryCommand qry)
        {
            SQLiteConnection conn = (SQLiteConnection)CreateConnection();
            //using(SQLiteConnection conn = (SQLiteConnection)CreateConnection())
            {
                SQLiteCommand cmd = new SQLiteCommand(qry.CommandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout;
                AddParams(qry, cmd);
                cmd.Connection = conn;

                try
                {
                    cmd.Connection.Open();
                }
                catch(Exception e)
                {
                    //Debug.WriteLine("ExecuteScalar: " + e.Message + " state = " + cmd.Connection.State.ToString());
                }

                // BUG: Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
                object result = cmd.ExecuteScalar();

                return result;
            }
        }

        /// <summary>
        /// Runs a Query and returns the number of rows affected.
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        public override int ExecuteQuery(QueryCommand qry)
        {
            SQLiteConnection conn = (SQLiteConnection)CreateConnection();
            using(SQLiteCommand cmd = new SQLiteCommand(qry.CommandSql))
            {
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout; // will always = 15
                AddParams(qry, cmd);
                cmd.Connection = conn;
                try
                {
                    cmd.Connection.Open();
                }
                catch(Exception e)
                {
                    //Debug.WriteLine("ExecuteQuery: " + e.Message + " state = " + cmd.Connection.State.ToString());
                }
                int result = cmd.ExecuteNonQuery();
                return result;
            }

        }

        /// <summary>
        /// Returns the complete schemma of a table.
        /// 
        /// In this version it assumes that a primary key of type integer is autoincrementable.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="tableType"></param>
        /// <returns></returns>
        public override TableSchema.Table GetTableSchema(string tableName, TableType tableType)
        {
            TableSchema.Table table = new TableSchema.Table(tableName, tableType, this);

            table.Name = tableName;
            table.Columns = new TableSchema.TableColumnCollection();
            table.ForeignKeys = new TableSchema.ForeignKeyTableCollection();

            table.SchemaName = Catalog;

            DataTable dtcols = AllColumns;
            DataTable fks = AllForeignKeys;
            DataTable indexes = AllIndexes;

            // Add all the columns
            foreach(DataRow row in dtcols.Select(string.Format("TABLE_NAME = '{0}'", tableName)))
            {
                TableSchema.TableColumn column = new TableSchema.TableColumn(table);
                column.ColumnName = row["COLUMN_NAME"].ToString();

                column.IsPrimaryKey = Convert.ToBoolean(row["PRIMARY_KEY"]);
                column.IsForeignKey = 
                    (fks.Select(string.Format("TABLE_NAME = '{0}' AND FKEY_FROM_COLUMN = '{1}'", tableName, column.ColumnName)).Length > 0);

                column.MaxLength = Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]);

                column.DataType = GetDbType(row["DATA_TYPE"].ToString());

                // These must types have max length 0 in generated classes. -- paul
                if( column.DataType == DbType.Boolean ||
                    column.DataType == DbType.Currency ||
                    column.DataType == DbType.Date ||
                    column.DataType == DbType.DateTime ||
                    column.DataType == DbType.DateTime2 ||
                    column.DataType == DbType.DateTimeOffset ||
                    column.DataType == DbType.Decimal ||
                    column.DataType == DbType.Double ||
                    column.DataType == DbType.Guid ||
                    column.DataType == DbType.Int16 ||
                    column.DataType == DbType.Int32 ||
                    column.DataType == DbType.Int64 ||
                    column.DataType == DbType.Single ||
                    column.DataType == DbType.Time ||
                    column.DataType == DbType.UInt16 ||
                    column.DataType == DbType.UInt32 ||
                    column.DataType == DbType.UInt64 ||
                    column.DataType == DbType.VarNumeric )
                    column.MaxLength = 0; // must = 0 for subsonic validation method

                // Autoincrement detection now available in recent System.Data.SQLite. 1.0.60.0 -- paul
                column.AutoIncrement = Convert.ToBoolean(row["AUTOINCREMENT"]);

                column.IsNullable = Convert.ToBoolean(row["IS_NULLABLE"]);
                column.IsReadOnly = false;
                table.Columns.Add(column);
            }

            //Add all the foreign keys from other tables that have foreign keys to this table's primary key
            foreach(DataRow row in fks.Select(string.Format("FKEY_TO_TABLE = '{0}'", tableName)))
            {
                TableSchema.TableColumn column = table.Columns.GetColumn(row["FKEY_TO_COLUMN"].ToString());

                if(!column.IsForeignKey)
                {
                    TableSchema.PrimaryKeyTable pkTable = new TableSchema.PrimaryKeyTable(this);

                    pkTable.ColumnName = row["FKEY_FROM_COLUMN"].ToString();
                    pkTable.TableName = row["TABLE_NAME"].ToString();
                    table.PrimaryKeyTables.Add(pkTable);
                }
            }
            
            //Add all the foreign keys
            foreach(DataRow row in fks.Select(string.Format("TABLE_NAME = '{0}'", tableName)))
            {
                TableSchema.ForeignKeyTable fk = new TableSchema.ForeignKeyTable(this);
                fk.ColumnName = row["FKEY_FROM_COLUMN"].ToString();
                fk.TableName = row["FKEY_TO_TABLE"].ToString();

                table.ForeignKeys.Add(fk);
            }
            return table;
        }

        /// <summary>
        /// Simple type conversion.
        /// </summary>
        /// <param name="sqliteType"></param>
        /// <returns></returns>
        public override DbType GetDbType(string sqliteType)
        {
            switch(sqliteType.ToLower())
            {
                case "text":
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                    return DbType.String;
                case "bigint":
                    return DbType.Int64;
                case "int":
                case "integer":
                    return DbType.Int32;
                case "real":
                case "numeric":
                case "double":
                case "single":
                case "float":
                    return DbType.Single;
                case "smallint":
                    return DbType.Int16;
                case "date":
                case "time":
                case "datetime":
                case "smalldatetime":
                    return DbType.DateTime;
                case "binary":
                case "blob":
                case "image":
                    return DbType.Binary;
                case "guid":
                    return DbType.Guid;
                case "bit":
                    return DbType.Boolean;
                default:
                    return DbType.String;
            }
        }

        /// <summary>
        /// Type conversion to .NET types.
        /// </summary>
        /// <param name="sqliteType"></param>
        /// <returns></returns>
        public Type GetType(string sqliteType)
        {
            if(_types == null)
            {
                SQLiteConnection conn = (SQLiteConnection)CreateConnection();
                //using(SQLiteConnection con = (SQLiteConnection)CreateConnection())
                conn.Open();
                _types = conn.GetSchema("DATATYPES");
            }

            DataRow[] type = _types.Select(string.Format("Typename = '{0}'", sqliteType));

            const int DATATYPE_INDEX = 5;
            if(type.Length == 1)
                return Type.GetType(type[0][DATATYPE_INDEX].ToString());
            else
                throw new ArgumentOutOfRangeException("sqliteType", sqliteType, "Invalid SQLite type.");
        }

        /// <summary>
        /// Returns an empty array as SQLite does not support stored procedures.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSPList()
        {
            //SQLite does not support stored procedures
            return new string[0];
        }

        /// <summary>
        /// Returns null as SQLite does not support stored procedures.
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public override IDataReader GetSPParams(string spName)
        {
            //SQLite does not support stored procedures
            return null;
        }

        /// <summary>
        /// Returns a sorted array with all the table names in the database.
        /// </summary>
        /// <returns></returns>
        public override string[] GetTableNameList()
        {
            string sql = "select tbl_name from SQLITE_MASTER where type = 'table' and tbl_name <> 'XP_PROC' and tbl_name <> 'sqlite_sequence' ORDER BY tbl_name";
            StringBuilder sList = new StringBuilder();

            //using(SQLiteConnection conn = (SQLiteConnection)CreateConnection())
            SQLiteConnection conn = (SQLiteConnection)CreateConnection();

            using(SQLiteCommand cmd = new SQLiteCommand(sql, conn))
            using(IDataReader rdr = cmd.ExecuteReader())
            {
                List<string> names = new List<string>();

                while(rdr.Read())
                    names.Add(rdr[0].ToString());

                return names.ToArray();
            }
        }

        /// <summary>
        /// Return a list with all the primary keys of the table and the tables that reference it.
        /// 
        /// For example, and using the pubs database:
        /// 
        /// GetPrimaryKeyTableNames("authors") returns:
        /// 
        /// TableName   ColumnName
        /// -----------------------
        /// titleauthor	au_id
        /// 
        /// GetPrimaryKeyTableNames("publishers") returns:
        /// 
        /// TableName   ColumnName
        /// -----------------------
        /// pub_info	pub_id
        /// titles	    pub_id
        /// employee	pub_id
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override ArrayList GetPrimaryKeyTableNames(string tableName)
        {
            ArrayList pks = new ArrayList();
            const int TABLE_NAME_INDEX = 5;
            const int FKEY_FROM_COLUMN_INDEX = 9;

            DataTable fks = AllForeignKeys;
            DataRow[] pksCols = fks.Select(string.Format("FKEY_TO_TABLE = '{0}'", tableName));

            foreach(DataRow row in pksCols)
                pks.Add(new string[] { row[TABLE_NAME_INDEX].ToString(), row[FKEY_FROM_COLUMN_INDEX].ToString() });

            return pks;
        }

        /// <summary>
        /// Return a list with all the primary keys of the table and the tables that reference it.
        /// See the other overload for a detailed explanation.
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override TableSchema.Table[] GetPrimaryKeyTables(string tableName)
        {
            ArrayList pks = GetPrimaryKeyTableNames(tableName);

            if(pks.Count > 0)
            {
                const int REF_TABLENAME_INDEX = 0;

                TableSchema.Table[] tables = new TableSchema.Table[pks.Count];

                for(int i = 0; i < pks.Count; i++)
                {
                    string[] refTable = (string[])pks[i];

                    tables[i] = DataService.GetSchema(refTable[REF_TABLENAME_INDEX], Name, TableType.Table);
                }
                return tables;
            }
            return null;
        }

        /// <summary>
        /// Return the name of the referenced table of fkColumnName in table tableName.
        /// 
        /// Example using the pubs database:
        /// 
        /// fkColumnName = au_id
        /// tableName = titleauthor
        /// returns: authors
        /// </summary>
        /// <param name="fkColumnName"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName, string tableName)
        {
            DataTable fks = AllForeignKeys;
            DataRow[] pksCols = fks.Select(string.Format("TABLE_NAME = '{0}' AND FKEY_FROM_COLUMN = '{1}'", tableName, fkColumnName));

            const int FKEY_TO_TABLE_INDEX = 13;

            if(pksCols.Length == 0)
                return string.Empty;
            else
                return pksCols[0][FKEY_TO_TABLE_INDEX].ToString();
        }

        /// <summary>
        /// Returns the first table that contains a primary key called fkColumnName.
        /// </summary>
        /// <param name="fkColumnName"></param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName)
        {
            DataTable cols = AllColumns;
            DataRow[] pks = cols.Select(string.Format("COLUMN_NAME = '{0}' AND PRIMARY_KEY = True", fkColumnName));

            const int TABLE_NAME_INDEX = 2;

            if(pks.Length == 0)
                return string.Empty;
            else
                return pks[0][TABLE_NAME_INDEX].ToString();
        }

        /// <summary>
        /// Execute all commands within a single transaction.
        /// </summary>
        /// <param name="commands"></param>
        public override void ExecuteTransaction(QueryCommandCollection commands)
        {
            //make sure we have at least one
            if(commands.Count < 1)
            {
                throw new Exception("No commands present");
            }

            SQLiteCommand cmd = null;

            SQLiteConnection conn = (SQLiteConnection)CreateConnection();
            SQLiteTransaction trans = (SQLiteTransaction)conn.BeginTransaction();

            foreach(QueryCommand qry in commands)
            {
                cmd = new SQLiteCommand(qry.CommandSql, conn);
                cmd.CommandType = qry.CommandType;
                try
                {
                    cmd.Connection.Open();
                }
                catch
                {
                }

                try
                {
                    AddParams(qry, cmd);
                    cmd.ExecuteNonQuery();
                }
                catch(SQLiteException ex)
                {
                    //if there's an error, roll everything back
                    trans.Rollback();

                    //clean up
                    cmd.Dispose();

                    throw;
                }
            }
            //if we get to this point, we're good to go
            trans.Commit();

            if(cmd != null)
                cmd.Dispose();

        }


        #region SQL Builders

        public string GetSql(Query qry)
        {
            string result = "";
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
        /// Helper method to build out the limit string of a given query.
        /// </summary>
        /// <param name="qry">Query to build the limit string from.</param>
        /// <returns></returns>
        private string GetLimit(Query qry)
        {
            string limit = string.Empty;

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
                    limit = qry.Top.ToLower().Replace("%", string.Empty).Replace("percent", string.Empty).Trim();

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
                        limit = string.Empty;
                    }
                }
                // The top parameter only contains an integer.
                // Wrap the integer in the limit string and return.
                else
                {
                    limit = " LIMIT " + qry.Top;
                }
            }
            // Paging in MySQL is actually quite simple. 
            // Using limit we will set the starting record 
            // to PageIndex * PageSize and the amount of 
            // records returned to PageSize.
            else
            {
                int start = qry.PageIndex * qry.PageSize;

                limit = string.Format(" LIMIT {0},{1} ", start, qry.PageSize);
            }

            return limit;
        }

        /// <summary>
        /// Creates a SELECT statement based on the Query object settings
        /// </summary>
        /// <returns></returns>
        public override string GetSelectSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;

            //different rules for how to do TOP
            string select = SqlFragment.SELECT;
            select += qry.IsDistinct ? SqlFragment.DISTINCT : String.Empty;

            //string groupBy = "";
            //string where;
            //string join = "";
            //string query;

            StringBuilder order = new StringBuilder();
            StringBuilder query = new StringBuilder();
            string columns;

            //append on the selectList, which is a property that can be set
            //and is "*" by default
            select += qry.SelectList;

            select += string.Format(" FROM `{0}`.`{1}`", Catalog, table.Name);

            //string where = BuildWhereSQLite(qry);
            string where = BuildWhere(qry);

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
                        {
                            order.Append(", ");
                        }
                    }
                }
                order = order.Replace("[", "");
                order = order.Replace("]", "");
            }

            string limit = GetLimit(qry);

            query.Append(select);
            query.Append(where);
            query.Append(order);
            query.Append(limit);

            return query.ToString();
        }

        /// <summary>
        /// This method is a copy of the static BuildWhere. I am not using BuildWhere because
        /// SQLite apparently does not support named parameters (as in ?AU_NAME), only positional
        /// parameters.
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        /*
        private static string BuildWhereSQLite(Query qry)
        {
            StringBuilder where = new StringBuilder();
            //string where = String.Empty;
            string whereOperator = SqlFragment.WHERE;
            bool isFirstPass = true;

            foreach(Where wWhere in qry.wheres)
            {
                whereOperator = isFirstPass ? SqlFragment.WHERE : whereOperator = " " + Enum.GetName(typeof(Where.WhereCondition), wWhere.Condition) + " ";

                where.Append(whereOperator);
                where.Append(Utility.QualifyColumnName(wWhere.TableName, wWhere.ColumnName, qry.Provider));
                where.Append(Where.GetComparisonOperator(wWhere.Comparison));
                if(wWhere.ParameterValue != DBNull.Value && wWhere.ParameterValue != null)
                    where.Append("?");
                else
                    where.Append(" NULL");


                isFirstPass = false;
            }
            //isFirstPass = true;
            foreach(BetweenAnd between in qry.betweens)
            {
                if(qry.wheres.Count == 0 && isFirstPass)
                {
                    whereOperator = SqlFragment.WHERE;
                }
                else
                {
                    whereOperator = isFirstPass ? SqlFragment.WHERE : whereOperator = " " + Enum.GetName(typeof(Where.WhereCondition), between.Condition) + " ";
                }
                where.Append(whereOperator);
                where.Append(Utility.QualifyColumnName(between.TableName, between.ColumnName, qry.Provider));
                where.Append(SqlFragment.BETWEEN + " ? ");
                where.Append(SqlFragment.AND + " ? ");

                isFirstPass = false;
            }

            for(int i = qry.wheres.Count - 1; i >= 0; i--)
            {
                if(qry.wheres[i].ParameterValue == DBNull.Value)
                {
                    qry.wheres.RemoveAt(i);
                }
            }

            if(qry.inList != null)
            {
                if(qry.inList.Length > 0)
                {
                    if(isFirstPass)
                    {
                        where.Append(whereOperator);
                    }
                    else
                    {
                        where.Append(SqlFragment.AND);
                    }

                    where.Append(qry.Provider.DelimitDbName(qry.inColumn) + SqlFragment.IN + "(");
                    bool isFirst = true;

                    for(int i = 1; i <= qry.inList.Length; i++)
                    {
                        if(!isFirst)
                        {
                            where.Append(", ");
                        }
                        isFirst = false;

                        where.Append("?");
                    }
                    where.Append(")");
                }
            }

            if(qry.notInList != null && qry.notInList.Length > 0)
            {
                if(isFirstPass)
                    where.Append(whereOperator);
                else
                    where.Append(SqlFragment.AND);

                where.Append(qry.Provider.DelimitDbName(qry.notInColumn));
                where.Append(SqlFragment.NOT_IN);
                where.Append("(");
                bool isFirst = true;

                for(int i = 1; i <= qry.notInList.Length; i++)
                {
                    if(!isFirst)
                        where.Append(", ");
                    isFirst = false;

                    where.Append(Utility.PrefixParameter(String.Concat("notIn", i), qry.Provider));
                }
                where.Append(")");
            }


            return where.ToString();
        }
        */

        /// <summary>
        /// Loops the TableColums[] array for the object, creating a SQL string
        /// for use as an INSERT statement
        /// </summary>
        /// <returns></returns>
        public override string GetInsertSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;

            //split the TablNames and loop out the SQL
            string insertSQL = "INSERT INTO `" + table.Name +"`";

            string cols = "";
            string pars = "";

            //int loopCount = 1;

            //if table columns are null toss an exception
            foreach(TableSchema.TableColumn col in table.Columns)
            {
                if(!col.AutoIncrement && !col.IsReadOnly)
                {
                    cols += col.ColumnName + ",";
                    pars += "?,";
                }
            }
            cols = cols.Remove(cols.Length - 1, 1);
            pars = pars.Remove(pars.Length - 1, 1);
            insertSQL += "(" + cols + ") ";

            insertSQL += "VALUES(" + pars + ");";

            insertSQL += " SELECT LAST_INSERT_ROWID() as newID";

            return insertSQL;
        }

        /// <summary>
        /// This method is a copy of the static GetDeleteSql that uses BuildWhereSQLite.
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        public static string GetDeleteSqlite(Query qry)
        {
            TableSchema.Table table = qry.Schema;
            string sql = SqlFragment.DELETE_FROM + Utility.QualifyColumnName(table.SchemaName, table.Name, qry.Provider);
            if(qry.wheres.Count == 0)
            {
                // Thanks Jason!
                TableSchema.TableColumn[] keys = table.PrimaryKeys;
                for(int i = 0; i < keys.Length; i++)
                {
                    sql += SqlFragment.WHERE +
                           Utility.MakeParameterAssignment(keys[i].ColumnName, keys[i].ColumnName, qry.Provider);
                    if(i + 1 != keys.Length)
                        sql += SqlFragment.AND;
                }
            }
            else
                sql += BuildWhere(qry);
            //sql += BuildWhereSQLite(qry);

            return sql;
        }

        #endregion

        #region Command Builders

        public QueryCommand BuildCommand(Query qry)
        {
            QueryCommand cmd = null;
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
            }
            return cmd;
        }

        #endregion


        /// <summary>
        /// This is a copy and adaptation from the SQL Server provider and it
        /// needs more work as it does NOT support auto incrementing columns.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public override string ScriptData(string tableName, string providerName)
        {
            return "fail"; // TODO:
            
            StringBuilder result = new StringBuilder();
            if(CodeService.ShouldGenerate(tableName, providerName))
            {
                StringBuilder fieldList = new StringBuilder();
                StringBuilder insertStatement = new StringBuilder();
                StringBuilder statements = new StringBuilder();


                insertStatement.Append("INSERT INTO [" + tableName + "] ");

                //pull the schema for this table
                TableSchema.Table table = Query.BuildTableSchema(tableName, providerName);

                //build the insert list.
                string lastColumnName = table.Columns[table.Columns.Count - 1].ColumnName;
                foreach(TableSchema.TableColumn col in table.Columns)
                {
                    fieldList.Append("[");
                    fieldList.Append(col.ColumnName);
                    fieldList.Append("]");

                    if(!Utility.IsMatch(col.ColumnName, lastColumnName))
                        fieldList.Append(", ");
                }

                //complete the insert statement
                insertStatement.Append("(");
                insertStatement.Append(fieldList);
                insertStatement.AppendLine(")");

                //get the table data
                IDataReader rdr = new Query(table).ExecuteReader();

                while(rdr.Read())
                {
                    StringBuilder thisStatement = new StringBuilder();
                    thisStatement.Append(insertStatement);
                    thisStatement.Append("VALUES(");
                    //loop the schema and pull out the values from the reader
                    foreach(TableSchema.TableColumn col in table.Columns)
                    {
                        if(!col.IsReadOnly)
                        {
                            object oData = rdr[col.ColumnName];
                            if(oData != null && oData != DBNull.Value)
                            {
                                if(col.DataType == DbType.Boolean)
                                {
                                    bool bData = Convert.ToBoolean(oData);
                                    thisStatement.Append(bData ? "1" : " 0");
                                }
                                else if(col.DataType == DbType.Byte)
                                {
                                    thisStatement.Append(oData);
                                }
                                else if(col.DataType == DbType.Binary)
                                {
                                    thisStatement.Append("0x");
                                    thisStatement.Append(Utility.ByteArrayToString((Byte[])oData).ToUpper());
                                }
                                else if(col.IsNumeric)
                                {
                                    thisStatement.Append(oData);
                                }
                                else if(col.IsDateTime)
                                {
                                    DateTime dt = DateTime.Parse(oData.ToString());
                                    thisStatement.Append("'");
                                    thisStatement.Append(dt.ToString("yyyy-MM-dd HH:mm:ss"));
                                    thisStatement.Append("'");
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

                            if(!Utility.IsMatch(col.ColumnName, lastColumnName))
                                thisStatement.Append(", ");
                        }
                    }

                    //add in a closing paren
                    thisStatement.AppendLine(");");
                    statements.Append(thisStatement);
                }
                rdr.Close();

                result.AppendLine("-- Begin inserting data in " + tableName);
                result.Append(statements);
            }
            return result.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qry"></param>
        /// <returns></returns>
        public override DbCommand GetDbCommand(QueryCommand qry)
        {
            DbCommand cmd;
            SQLiteConnection conn = (SQLiteConnection)CreateConnection();
            cmd = conn.CreateCommand();

            cmd.CommandText = qry.CommandSql;
            cmd.CommandType = qry.CommandType;

            foreach(QueryParameter par in qry.Parameters)
            {
                //QuerParameter cannot be cast to SQLite parameter so
                //we create new parameters.
                int param = cmd.Parameters.Add(new SQLiteParameter(par.ParameterName));
                cmd.Parameters[param].Direction = par.Mode;
                cmd.Parameters[param].Size = par.Size;
                cmd.Parameters[param].Value = par.ParameterValue;
            }

            return cmd;
        }

        /// <summary>
        /// Returns the names of all the views in the database sorted by name.
        /// </summary>
        /// <returns></returns>
        public override string[] GetViewNameList()
        {
            DataTable views;
            SQLiteConnection conn = (SQLiteConnection)CreateConnection();
            //using(SQLiteConnection conn = (SQLiteConnection)CreateConnection())
                views = conn.GetSchema("VIEWS");

            string[] allViews = new string[views.Rows.Count];

            for(int n = 0; n < allViews.Length; n++)
                allViews[n] = views.Rows[n]["TABLE_NAME"].ToString();

            Array.Sort(allViews);
            return allViews;
        }

        /// <summary>
        /// Return a dataset for a query
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry"></param>
        /// <returns></returns>
        public override T GetDataSet<T>(QueryCommand qry)
        {
            T ds = new T();
            SQLiteCommand cmd = new SQLiteCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);

            AddTableMappings(da, ds);
            SQLiteConnection conn = (SQLiteConnection)CreateConnection();
            //using(SQLiteConnection conn = (SQLiteConnection)CreateConnection())
            {
                cmd.Connection = conn;
                AddParams(qry, cmd);
                da.Fill(ds);

                cmd.Dispose();
                da.Dispose();

                return ds;
            }
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
        /// Return the tables that have a key referencing this table.
        /// 
        /// For example:
        /// tableName = employee
        /// returns: { "publishers", "jobs" }
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public override string[] GetForeignKeyTables(string tableName)
        {
            DataTable fks = AllForeignKeys;
            DataRow[] pksCols = fks.Select(string.Format("TABLE_NAME = '{0}'", tableName));

            const int FKEY_TO_TABLE_INDEX = 13;

            if(pksCols.Length == 0)
                return new string[] { "" };
            else
            {
                string[] names = new string[pksCols.Length];

                for(int n = 0; n < pksCols.Length; n++)
                    names[n] = pksCols[n][FKEY_TO_TABLE_INDEX].ToString();

                return names;
            }
        }

        /// <summary>
        /// Returns the first table with a primary key column named pkName
        /// </summary>
        /// <param name="pkName"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public override string GetTableNameByPrimaryKey(string pkName, string providerName)
        {
            const int INDEX_NAME_INDEX = 8;
            const int TABLE_NAME_INDEX = 2;

            DataTable indexes, indexColumns;
            SQLiteConnection con = (SQLiteConnection)CreateConnection();
            //using(SQLiteConnection con = (SQLiteConnection)CreateConnection())
            {
                indexColumns = con.GetSchema("INDEXCOLUMNS");
                indexes = con.GetSchema("INDEXES");
            }

            DataRow[] index = indexColumns.Select(string.Format("COLUMN_NAME = '{0}'", pkName));

            if(index.Length == 0)
                return string.Empty;
            else
            {
                DataRow[] tables = indexes.Select(string.Format("PRIMARY_KEY = True AND INDEX_NAME = '{0}'", index[0][INDEX_NAME_INDEX]));

                if(tables.Length == 0)
                    return string.Empty;
                else
                    return tables[0][TABLE_NAME_INDEX].ToString();
            }

        }

        protected override string GetDatabaseVersion(string providerName)
        {
            string retVal = "Unknown";

            SQLiteConnection conn = (SQLiteConnection)CreateConnection();

            try
            {
                return conn.ServerVersion;
            }
            catch
            {
                return "UKNOWN";
            }
            finally
            {
                if(conn != null)
                    conn.Dispose();
            }
        }




    }
}


#endif
