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

//TODO CH Add Foreign Key Checks and support
//TODO CH Clean up code left over from SqlDataProvider
#if ALLPROVIDERS
using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Text;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the SqlCEProvider class
    /// </summary>
    public class SqlCEProvider : DataProvider
    {
        #region Constants

        private const string EXTENDED_PROPERTIES_ALL =
            @"SELECT 
    t.name AS [TABLE_NAME], 
    c.name AS [COLUMN_NAME], 
    ep.name AS [EXTENDED_NAME],
    value AS [EXTENDED_VALUE]
FROM sys.extended_properties AS ep
INNER JOIN sys.tables AS t ON ep.major_id = t.object_id 
LEFT JOIN sys.columns AS c ON ep.major_id = c.object_id AND ep.minor_id = c.column_id
WHERE class = 1 OR class = 3";

        private const string EXTENDED_PROPERTIES_COLUMNS_2000 =
            @"SELECT 
    objname AS COLUMN_NAME,
    [name] AS EXTENDED_NAME,
    [value] AS EXTENDED_VALUE
FROM fn_listextendedproperty (NULL, 'SCHEMA', '{0}', 'TABLE', '{1}', 'COLUMN', DEFAULT);";

        private const string EXTENDED_PROPERTIES_TABLES_2000 =
            @"SELECT 
    objname AS TABLE_NAME,
    [name] AS EXTENDED_NAME,
    [value] AS EXTENDED_VALUE
FROM fn_listextendedproperty (NULL, 'SCHEMA', '{0}', 'TABLE', default, NULL, NULL);";

        private const string FOREIGN_TABLE_LIST_ALL =
            @"select c.CONSTRAINT_NAME as Constraint_Name, c.COLUMN_NAME as PK_Column, cr.COLUMN_NAME as FK_Column, cr.TABLE_NAME as FK_Table
from INFORMATION_SCHEMA.TABLE_CONSTRAINTS fk 
join INFORMATION_SCHEMA.KEY_COLUMN_USAGE c on c.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
join INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS rc on rc.CONSTRAINT_NAME = fk.CONSTRAINT_NAME
join INFORMATION_SCHEMA.KEY_COLUMN_USAGE cr on cr.CONSTRAINT_NAME = rc.UNIQUE_CONSTRAINT_NAME 
	and cr.ORDINAL_POSITION = c.ORDINAL_POSITION
where fk.CONSTRAINT_TYPE = 'FOREIGN KEY'
order by c.CONSTRAINT_NAME, c.ORDINAL_POSITION";

        private const string INDEX_SQL_ALL =
            @"SELECT
    KCU.TABLE_NAME as TableName,
    KCU.COLUMN_NAME as ColumnName, 
    TC.CONSTRAINT_TYPE as ConstraintType 
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
ON KCU.CONSTRAINT_NAME=TC.CONSTRAINT_NAME";

        private const string MANY_TO_MANY_CHECK_ALL =
            @"SELECT 
    FK_Table = FK.TABLE_NAME, 
    FK_Column = CU.COLUMN_NAME, 
    PK_Table  = PK.TABLE_NAME, 
    PK_Column = PT.COLUMN_NAME, 
    Constraint_Name = C.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN    
(    
    SELECT i1.TABLE_NAME, i2.COLUMN_NAME
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
    WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
)
PT ON PT.TABLE_NAME = PK.TABLE_NAME

WHERE FK.TABLE_NAME IN
    (
        SELECT tc.table_Name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
        JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu ON tc.Constraint_name = kcu.Constraint_Name
        JOIN 
        (
            SELECT tc.Table_Name, kcu.Column_Name AS Column_Name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu ON tc.Constraint_name = kcu.Constraint_Name
            WHERE Constraint_Type = 'FOREIGN KEY'
        ) 
        AS t ON t.Table_Name = tc.table_Name AND t.Column_Name = kcu.Column_Name
        WHERE Constraint_Type = 'PRIMARY KEY'
        GROUP BY tc.Constraint_Name, tc.Table_Name HAVING COUNT(*) > 1   
    )";

        private const string MANY_TO_MANY_FOREIGN_MAP_ALL =
            @"SELECT 
    FK_Table  = FK.TABLE_NAME,
    FK_Column = CU.COLUMN_NAME,
    PK_Table  = PK.TABLE_NAME,
    PK_Column = PT.COLUMN_NAME, Constraint_Name = C.CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C
INNER JOIN 	INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME
INNER JOIN  INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
INNER JOIN  INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
INNER JOIN	
    (
        SELECT i1.TABLE_NAME, i2.COLUMN_NAME
        FROM  INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1
        INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME
        WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY'
    ) 
PT ON PT.TABLE_NAME = PK.TABLE_NAME";

        private const string PRIMARY_TABLE_LIST_ALL =
            @"SELECT 
    TC.TABLE_NAME AS PK_TABLE,
    KCU.TABLE_NAME AS FK_TABLE,
    KCU.COLUMN_NAME AS FK_COLUMN
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ON KCU.CONSTRAINT_NAME=RC.CONSTRAINT_NAME
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON RC.UNIQUE_CONSTRAINT_NAME=TC.CONSTRAINT_NAME";

        private const string SP_PARAM_SQL_ALL =
            @"SELECT SPECIFIC_NAME AS SPName, ORDINAL_POSITION AS OrdinalPosition, 
                                                PARAMETER_MODE AS ParamType, IS_RESULT AS IsResult, PARAMETER_NAME AS Name, 
                                                DATA_TYPE AS DataType, CHARACTER_MAXIMUM_LENGTH AS DataLength, REPLACE(PARAMETER_NAME, '@', '') 
                                                AS CleanName, PARAMETER_MODE as [mode], NUMERIC_PRECISION as NumericPrecision, NUMERIC_SCALE as NumericScale
                                                FROM INFORMATION_SCHEMA.PARAMETERS";

        private const string TABLE_COLUMN_SQL_ALL =
            @"SELECT 
    'DataBase' AS [Database],
    'dbo' AS Owner, 
    TABLE_NAME AS TableName, 
    COLUMN_NAME AS ColumnName, 
    ORDINAL_POSITION AS OrdinalPosition, 
    COLUMN_DEFAULT AS DefaultSetting, 
    IS_NULLABLE AS IsNullable, DATA_TYPE AS DataType, 
    CHARACTER_MAXIMUM_LENGTH AS MaxLength, 
    DATETIME_PRECISION AS DatePrecision,
	case when AUTOINC_SEED is not null then 1 else 0 end AS IsIdentity,
	0 as IsComputed
FROM  INFORMATION_SCHEMA.COLUMNS";

        #endregion


        private static readonly DataSet dsColumns = new DataSet();
        private static readonly DataSet dsExtendedProperties = new DataSet();
        private static readonly DataSet dsFK = new DataSet();
        private static readonly DataSet dsIndex = new DataSet();
        private static readonly DataSet dsManyToManyCheck = new DataSet();
        private static readonly DataSet dsManyToManyMap = new DataSet();
        private static readonly DataSet dsPK = new DataSet();
        private DataTable dtParamSql;

        /// <summary>
        /// Gets the type of the named provider.
        /// </summary>
        /// <value>The type of the named provider.</value>
        public override string NamedProviderType
        {
            get { return DataProviderTypeName.SQL_CE; }
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
        /// Reloads the cached schema
        /// </summary>
        public override void ReloadSchema()
        {
            dsColumns.Tables.Clear();
            dsExtendedProperties.Tables.Clear();
            dsFK.Tables.Clear();
            dsIndex.Tables.Clear();
            dsManyToManyCheck.Tables.Clear();
            dsManyToManyMap.Tables.Clear();
            dsPK.Tables.Clear();
            if(dtParamSql != null)
                dtParamSql.Clear();
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="newConnectionString">The new connection string.</param>
        /// <returns></returns>
        public override DbConnection CreateConnection(string newConnectionString)
        {
            SqlCeConnection retVal = new SqlCeConnection(newConnectionString);
            retVal.Open();
            return retVal;
        }

        /// <summary>
        /// Adds the params.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="qry">The qry.</param>
        private static void AddParams(SqlCeCommand cmd, QueryCommand qry)
        {
            if(qry.Parameters != null)
            {
                foreach(QueryParameter param in qry.Parameters)
                {
                    SqlCeParameter sqlParam = new SqlCeParameter(param.ParameterName, Utility.GetSqlDBType(param.DataType));
                    sqlParam.Direction = param.Mode;

                    //output parameters need to define a size
                    //our default is 50
                    if(sqlParam.Direction == ParameterDirection.Output || sqlParam.Direction == ParameterDirection.InputOutput)
                        sqlParam.Size = param.Size;

                    if(param.Scale != null)
                        sqlParam.Scale = Convert.ToByte(param.Scale);

                    if (param.Precision != null)
                        sqlParam.Precision = Convert.ToByte(param.Precision);

                    //fix for NULLs as parameter values
                    if(param.ParameterValue == null || Utility.IsMatch(param.ParameterValue.ToString(), "null"))
                        sqlParam.Value = DBNull.Value;
                    else if(param.DataType == DbType.Guid)
                    {
                        string paramValue = param.ParameterValue.ToString();
                        if(!String.IsNullOrEmpty(paramValue))
                        {
                            if(!Utility.IsMatch(paramValue, SqlSchemaVariable.DEFAULT))
                                sqlParam.Value = new Guid(param.ParameterValue.ToString());
                        }
                        else
                            sqlParam.Value = DBNull.Value;
                    }
                    else
                        sqlParam.Value = param.ParameterValue;

                    cmd.Parameters.Add(sqlParam);
                }
            }
        }

        /// <summary>
        /// Checkouts the output params.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="qry">The qry.</param>
        private static void CheckoutOutputParams(SqlCeCommand cmd, QueryCommand qry)
        {
            if(qry.CommandType == CommandType.StoredProcedure && qry.HasOutputParams())
            {
                //loop the params, getting the values and setting them for the return
                foreach(QueryParameter param in qry.Parameters)
                {
                    if(param.Mode == ParameterDirection.InputOutput || param.Mode == ParameterDirection.Output)
                    {
                        object oVal = cmd.Parameters[param.ParameterName].Value;
                        qry.OutputValues.Add(oVal);
                    }
                }
            }
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDataReader GetReader(QueryCommand qry)
        {
            AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this);
            SqlCeCommand cmd = new SqlCeCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            //SqlCE does not support CommandTimeout higher than 0
            cmd.CommandTimeout = 0;
            AddParams(cmd, qry);

            cmd.Connection = automaticConnectionScope.GetConnection<SqlCeConnection>();
            //let this bubble up
            IDataReader rdr;

            //Thanks jcoenen!
            try
            {
                // if it is a shared connection, we shouldn't be telling the reader to close it when it is done
                rdr = automaticConnectionScope.IsUsingSharedConnection ? cmd.ExecuteReader() : cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch(SqlCeException)
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
            if(qry.CommandType == CommandType.Text)
                qry.CommandSql = "/* GetDataSet() */ " + qry.CommandSql;

            SqlCeCommand cmd = new SqlCeCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            //SqlCE does not support CommandTimeout higher than 0 CH
            cmd.CommandTimeout = 0;
            SqlCeDataAdapter da = new SqlCeDataAdapter(cmd);

            AddTableMappings(da, ds);
            using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
            {
                cmd.Connection = conn.GetConnection<SqlCeConnection>();
                AddParams(cmd, qry);
                da.Fill(ds);

                CheckoutOutputParams(cmd, qry);

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
                SqlCeCommand cmd = new SqlCeCommand(qry.CommandSql);
                cmd.CommandType = qry.CommandType;
                //SqlCE does not support CommandTimeout higher than 0 CH

                cmd.CommandTimeout = 0;
                AddParams(cmd, qry);
                cmd.Connection = automaticConnectionScope.GetConnection<SqlCeConnection>();

                object result;
                if(qry.CommandSql.ToLower().Contains("insert"))
                {
                    cmd.ExecuteScalar();
                    SqlCeCommand outcmd = new SqlCeCommand(SqlFragment.SELECT + " @@IDENTITY " + SqlFragment.AS + "newID;");
                    outcmd.CommandType = CommandType.Text;
                    outcmd.Connection = automaticConnectionScope.GetConnection<SqlCeConnection>();
                    result = outcmd.ExecuteScalar();
                }
                else
                    result = cmd.ExecuteScalar();
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
                SqlCeCommand cmd = new SqlCeCommand(qry.CommandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = 0;

                AddParams(cmd, qry);
                cmd.Connection = automaticConnectionScope.GetConnection<SqlCeConnection>();
                Console.WriteLine(cmd.CommandText);
                int result = cmd.ExecuteNonQuery();
                CheckoutOutputParams(cmd, qry);
                return result;
            }
        }

        /// <summary>
        /// Loads the extended property data set.
        /// </summary>
        private void LoadExtendedPropertyDataSet()
        {
            if(dsExtendedProperties.Tables[Name] == null)
            {
                QueryCommand cmdExtProps = new QueryCommand(EXTENDED_PROPERTIES_ALL, Name);
                DataTable dt = new DataTable(Name);
                dt.Load(GetReader(cmdExtProps));
                dsExtendedProperties.Tables.Add(dt);
            }
        }

        /// <summary>
        /// Sets the extended table properties.
        /// </summary>
        /// <param name="tblSchema">The TBL schema.</param>
        private void SetExtendedTableProperties(TableSchema.AbstractTableSchema tblSchema)
        {
            if(UseExtendedProperties)
            {
                DataRow[] drTableProps = null;
                if(Utility.IsSql2005(this))
                {
                    LoadExtendedPropertyDataSet();
                    drTableProps = dsExtendedProperties.Tables[Name].Select("TABLE_NAME ='" + tblSchema.TableName + "' AND COLUMN_NAME IS NULL");
                }
                else if(Utility.IsSql2000(this))
                {
                    string query = String.Format(EXTENDED_PROPERTIES_TABLES_2000, tblSchema.SchemaName);
                    QueryCommand cmdTable = new QueryCommand(query, Name);
                    DataTable dt = new DataTable(Name);
                    dt.Load(GetReader(cmdTable));
                    drTableProps = dt.Select("TABLE_NAME ='" + tblSchema.TableName + "'");
                }
                if(drTableProps != null)
                {
                    for(int i = 0; i < drTableProps.Length; i++)
                        tblSchema.ExtendedProperties.Add(new TableSchema.ExtendedProperty(drTableProps[i]["EXTENDED_NAME"].ToString(), drTableProps[i]["EXTENDED_VALUE"].ToString()));

                    tblSchema.ApplyExtendedProperties();
                }
            }
        }

        /// <summary>
        /// Sets the extended column properties.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <param name="tableColumn">The table column.</param>
        private void SetExtendedColumnProperties(TableSchema.AbstractTableSchema tableSchema, TableSchema.TableColumn tableColumn)
        {
            if(UseExtendedProperties && Utility.IsSql2005(this))
            {
                DataRow[] drColumnProps = null;
                if(Utility.IsSql2005(this))
                {
                    LoadExtendedPropertyDataSet();
                    drColumnProps = dsExtendedProperties.Tables[Name].Select("TABLE_NAME ='" + tableSchema.TableName + "' AND COLUMN_NAME = '" + tableColumn.ColumnName + "'");
                }
                else if(Utility.IsSql2000(this))
                {
                    string query = String.Format(EXTENDED_PROPERTIES_COLUMNS_2000, tableSchema.SchemaName, tableSchema.TableName);
                    QueryCommand cmdColumn = new QueryCommand(query, Name);
                    DataTable dt = new DataTable(Name);
                    dt.Load(GetReader(cmdColumn));
                    drColumnProps = dt.Select("COLUMN_NAME ='" + tableColumn.ColumnName + "'");
                }
                if(drColumnProps != null)
                {
                    for(int j = 0; j < drColumnProps.Length; j++)
                    {
                        tableColumn.ExtendedProperties.Add(new TableSchema.ExtendedProperty(drColumnProps[j]["EXTENDED_NAME"].ToString(),
                            drColumnProps[j]["EXTENDED_VALUE"].ToString()));
                        tableColumn.ApplyExtendedProperties();
                    }
                }
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
            const string columnqry =
                @"SELECT
    TABLE_NAME AS TableName, 
    COLUMN_NAME AS ColumnName,
 
    DATA_TYPE AS DataType,     IS_NULLABLE AS IsNullable,
    case when CHARACTER_MAXIMUM_LENGTH is null then 0  else CHARACTER_MAXIMUM_LENGTH end AS MaxLength,
    case when AUTOINC_SEED is null then 0 else 1 end as IsIdentity

    FROM  INFORMATION_SCHEMA.COLUMNS";

            if(dsColumns.Tables[Name] == null)
            {
                QueryCommand cmdColumns = new QueryCommand(columnqry, Name);
                cmdColumns.CommandTimeout = 0;
                DataTable dt = new DataTable(Name);
                dt.Load(GetReader(cmdColumns));
                dsColumns.Tables.Add(dt);
            }

            DataRow[] drColumns = dsColumns.Tables[Name].Select("TableName ='" + tableName + "'");

            if(drColumns.Length == 0)
                return null;

            TableSchema.TableColumnCollection columns = new TableSchema.TableColumnCollection();
            TableSchema.Table tbl = new TableSchema.Table(tableName, tableType, this);
            tbl.ForeignKeys = new TableSchema.ForeignKeyTableCollection();

            for(int i = 0; i < drColumns.Length; i++)
            {
                string nativeDataType = drColumns[i][SqlSchemaVariable.DATA_TYPE].ToString().ToLower();
                TableSchema.TableColumn column = new TableSchema.TableColumn(tbl);
                column.ColumnName = drColumns[i][SqlSchemaVariable.COLUMN_NAME].ToString();
                column.DataType = GetDbType(nativeDataType);
                column.AutoIncrement = Convert.ToBoolean(drColumns[i][SqlSchemaVariable.IS_IDENTITY]);
                int maxLength;
                int.TryParse(drColumns[i][SqlSchemaVariable.MAX_LENGTH].ToString(), out maxLength);
                column.MaxLength = maxLength;
                column.IsNullable = drColumns[i][SqlSchemaVariable.IS_NULLABLE].ToString() == "YES";
                //Does Not Support Computed Columns CH
                //bool isComputed = false;
                column.IsReadOnly = (nativeDataType == "timestamp");
                columns.Add(column);
                SetExtendedColumnProperties(tbl, column);
            }

            if(dsIndex.Tables[Name] == null)
            {
                QueryCommand cmdIndex = new QueryCommand(INDEX_SQL_ALL, Name);
                cmdIndex.CommandTimeout = 0;
                DataTable dt = new DataTable(Name);
                dt.Load(GetReader(cmdIndex));
                dsIndex.Tables.Add(dt);
            }

            DataRow[] drIndexes = dsIndex.Tables[Name].Select("TableName ='" + tableName + "'");
            for(int i = 0; i < drIndexes.Length; i++)
            {
                string colName = drIndexes[i][SqlSchemaVariable.COLUMN_NAME].ToString();
                string constraintType = drIndexes[i][SqlSchemaVariable.CONSTRAINT_TYPE].ToString();
                TableSchema.TableColumn column = columns.GetColumn(colName);

                if(Utility.IsMatch(constraintType, SqlSchemaVariable.PRIMARY_KEY))
                    column.IsPrimaryKey = true;
                else if(Utility.IsMatch(constraintType, SqlSchemaVariable.FOREIGN_KEY))
                    column.IsForeignKey = true;

                //HACK: Allow second pass naming adjust based on whether a column is keyed
                column.ColumnName = column.ColumnName;
                SetExtendedColumnProperties(tbl, column);
            }

            if(dsPK.Tables[Name] == null)
            {
                QueryCommand cmdPK = new QueryCommand(PRIMARY_TABLE_LIST_ALL, Name);
                cmdPK.CommandTimeout = 0;
                DataTable dt = new DataTable(Name);
                dt.Load(GetReader(cmdPK));
                dsPK.Tables.Add(dt);
            }

            DataRow[] drPK = dsPK.Tables[Name].Select("PK_Table ='" + tableName + "'");
            for(int i = 0; i < drPK.Length; i++)
            {
                string colName = drPK[i]["FK_Column"].ToString();
                string fkName = drPK[i]["FK_Table"].ToString();

                //columns.GetColumn(colName).PrimaryKeyTableName = fkName;
                TableSchema.PrimaryKeyTable pkTable = new TableSchema.PrimaryKeyTable(this);
                pkTable.ColumnName = colName;
                pkTable.TableName = fkName;
                SetExtendedTableProperties(pkTable);
                tbl.PrimaryKeyTables.Add(pkTable);
            }

            if(dsFK.Tables[Name] == null)
            {
                QueryCommand cmdFK = new QueryCommand(FOREIGN_TABLE_LIST_ALL, Name);
                DataTable dt = new DataTable(Name);
                dt.Load(GetReader(cmdFK));
                dsFK.Tables.Add(dt);
            }

            DataRow[] drFK = dsFK.Tables[Name].Select("FK_Table ='" + tableName + "'");
            ArrayList usedConstraints = new ArrayList();
            for(int i = 0; i < drFK.Length; i++)
            {
                string constraintName = drFK[i]["Constraint_Name"].ToString();
                if(!usedConstraints.Contains(constraintName))
                {
                    usedConstraints.Add(constraintName);
                    string colName = drFK[i]["FK_Column"].ToString();
                    string fkName = drFK[i]["PK_Table"].ToString();
                    columns.GetColumn(colName).ForeignKeyTableName = fkName;
                    TableSchema.ForeignKeyTable fkTable = new TableSchema.ForeignKeyTable(this);
                    fkTable.ColumnName = colName;
                    fkTable.TableName = fkName;
                    SetExtendedTableProperties(fkTable);
                    tbl.ForeignKeys.Add(fkTable);
                }
            }

            tbl.Columns = columns;
            return tbl;
        }

        /// <summary>
        /// Gets the SP params.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        public override IDataReader GetSPParams(string spName)
        {
            if(dtParamSql == null)
            {
                QueryCommand cmdSP = new QueryCommand(SP_PARAM_SQL_ALL, Name);
                dtParamSql = new DataTable();
                dtParamSql.Load(GetReader(cmdSP));
            }

            DataView dv = new DataView(dtParamSql);
            dv.RowFilter = "SPName = '" + spName + "'";
            dv.Sort = "OrdinalPosition";
            DataTable dtNew = dv.ToTable();
            return dtNew.CreateDataReader();
        }

        /// <summary>
        /// Gets the SP list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSPList()
        {
            QueryCommand cmd = new QueryCommand("/* GetSPList() */ " + SP_SQL, Name);
            StringBuilder sList = new StringBuilder();
            using(IDataReader rdr = GetReader(cmd))
            {
                while(rdr.Read())
                {
                    sList.Append(rdr[0]);
                    sList.Append("|");
                }
                rdr.Close();
            }
            string strList = sList.ToString();
            return strList.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets the view name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetViewNameList()
        {
            if(ViewNames == null || !CurrentConnectionStringIsDefault)
            {
                QueryCommand cmd = new QueryCommand("/* GetViewNameList() */ " + VIEW_SQL, Name);
                StringBuilder sList = new StringBuilder();
                using(IDataReader rdr = GetReader(cmd))
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
                string strList = sList.ToString();
                string[] tempViewNames = strList.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                Array.Sort(tempViewNames);

                if(CurrentConnectionStringIsDefault)
                    ViewNames = tempViewNames;
                else
                    return tempViewNames;
            }
            return ViewNames;
        }

        /// <summary>
        /// Gets the table name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetTableNameList()
        {
            if(TableNames == null || !CurrentConnectionStringIsDefault)
            {
                QueryCommand cmd = new QueryCommand("select TABLE_NAME from INFORMATION_SCHEMA.TABLES where TABLE_TYPE = 'TABLE'", Name);
                cmd.CommandTimeout = 0;
                StringBuilder sList = new StringBuilder();
                using(IDataReader rdr = GetReader(cmd))
                {
                    while(rdr.Read())
                    {
                        sList.Append(rdr[0]);
                        sList.Append("|");
                    }
                    rdr.Close();
                }
                string strList = sList.ToString();
                string[] tempTableNames = strList.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                Array.Sort(tempTableNames);

                if(CurrentConnectionStringIsDefault)
                    TableNames = tempTableNames;
                else
                    return tempTableNames;
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
            QueryCommand cmd = new QueryCommand("/* GetPrimaryKeyTableNames(" + tableName + ") */ " + GET_PRIMARY_KEY_SQL, Name);
            cmd.AddParameter("tblName", tableName, DbType.AnsiString);
            ArrayList list = new ArrayList();

            using(IDataReader rdr = GetReader(cmd))
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
            QueryCommand cmd = new QueryCommand("/* GetPrimaryKeyTables(" + tableName + ") */ " + GET_PRIMARY_KEY_SQL, Name);
            cmd.AddParameter("tblName", tableName, DbType.AnsiString);
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
            QueryCommand cmd = new QueryCommand("/* GetForeignKeyTableName(" + fkColumnName + "," + tableName + ") */ " + GET_FOREIGN_KEY_SQL, Name);
            cmd.AddParameter("columnName", fkColumnName, DbType.AnsiString);
            cmd.AddParameter("tblName", tableName, DbType.AnsiString);

            object result = ExecuteScalar(cmd);
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
            QueryCommand cmd = new QueryCommand("/* GetForeignKeyTableName(" + fkColumnName + ") */ " + GET_TABLE_SQL, Name);
            cmd.AddParameter("columnName", fkColumnName, DbType.AnsiString);

            object result = ExecuteScalar(cmd);
            if(result == null)
                return null;

            return result.ToString();
        }

        /// <summary>
        /// Gets the foreign key tables.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override string[] GetForeignKeyTables(string tableName)
        {
            QueryCommand cmd = new QueryCommand("/* GetForeignKeyTables(" + tableName + ") */ " + FOREIGN_TABLE_LIST, Name);
            cmd.AddParameter("@tblName", tableName, DbType.AnsiString);
            StringBuilder sList = new StringBuilder();
            using(IDataReader rdr = GetReader(cmd))
            {
                while(rdr.Read())
                {
                    sList.Append(rdr["TABLE_NAME"]);
                    sList.Append("|");
                }
                rdr.Close();
            }
            string strList = sList.ToString();
            return strList.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Gets the type of the db.
        /// </summary>
        /// <param name="sqlType">Type of the SQL.</param>
        /// <returns></returns>
        public override DbType GetDbType(string sqlType)
        {
            switch(sqlType)
            {
                case "varchar":
                    return DbType.AnsiString;
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
                    return DbType.AnsiString;
                case "timestamp":
                    return DbType.Byte;
                case "tinyint":
                    return DbType.Int16;
                case "varbinary":
                    return DbType.Binary;
                default:
                    return DbType.AnsiString;
            }
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDbCommand GetCommand(QueryCommand qry)
        {
            SqlCeCommand cmd = new SqlCeCommand(qry.CommandSql);
            AddParams(cmd, qry);
            return cmd;
        }

        /// <summary>
        /// Executes a transaction using the passed-commands
        /// </summary>
        /// <param name="commands">The commands.</param>
        public override void ExecuteTransaction(QueryCommandCollection commands)
        {
            //make sure we have at least one
            if(commands.Count > 0)
            {
                SqlCeCommand cmd;

                //a using statement will make sure we close off the connection

                using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
                {
                    //open up the connection and start the transaction
                    if(conn.GetConnection<SqlCeConnection>().State == ConnectionState.Closed)
                        conn.GetConnection<SqlCeConnection>().Open();

                    SqlCeTransaction trans = conn.GetConnection<SqlCeConnection>().BeginTransaction();

                    foreach(QueryCommand qry in commands)
                    {
                        if(qry.CommandType == CommandType.Text)
                            qry.CommandSql = "/* ExecuteTransaction() */ " + qry.CommandSql;

                        cmd = new SqlCeCommand(qry.CommandSql, conn.GetConnection<SqlCeConnection>(), trans);
                        cmd.CommandType = qry.CommandType;

                        foreach(QueryParameter param in qry.Parameters)
                        {
                            SqlCeParameter sqlParam = new SqlCeParameter();
                            sqlParam.Value = param.ParameterValue;
                            sqlParam.ParameterName = param.ParameterName;
                            sqlParam.DbType = param.DataType;
                            cmd.Transaction = trans;
                            cmd.Parameters.Add(sqlParam);
                        }

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch(SqlCeException x)
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
        /// Gets the table name by primary key.
        /// </summary>
        /// <param name="pkName">Name of the pk.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public override string GetTableNameByPrimaryKey(string pkName, string providerName)
        {
            QueryCommand cmd = new QueryCommand("/* GetTableNameByPrimaryKey(" + pkName + ") */ " + TABLE_BY_PK, providerName);
            cmd.Parameters.Add("@columnName", pkName, DbType.AnsiString);
            cmd.Parameters.Add("@mapSuffix", ManyToManySuffix, DbType.AnsiString);
            object oResult = DataService.ExecuteScalar(cmd);
            string result = String.Empty;
            if(oResult != null)
                result = oResult.ToString();

            return result;
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        protected override string GetDatabaseVersion(string providerName)
        {
            //Only SqlCE 3.5 is supported
            return "3.5";
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="parameter">The parameter.</param>
        public override void SetParameter(IDataReader dataReader, StoredProcedure.Parameter parameter)
        {
            parameter.DBType = GetDbType(dataReader[SqlSchemaVariable.DATA_TYPE].ToString());
            string sMode = dataReader[SqlSchemaVariable.MODE].ToString();
            if(sMode == SqlSchemaVariable.MODE_INOUT)
                parameter.Mode = ParameterDirection.InputOutput;
            parameter.Name = dataReader[SqlSchemaVariable.NAME].ToString();

            object objPrecision = dataReader[SqlSchemaVariable.NUMERIC_PRECISION];
            if(objPrecision != null && objPrecision != DBNull.Value)
                parameter.Precision = Convert.ToInt32(objPrecision);

            object objScale = dataReader[SqlSchemaVariable.NUMERIC_SCALE];
            if(objScale != null && objScale != DBNull.Value)
                parameter.Scale = Convert.ToInt32(objScale);
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
        public override string FormatIdentifier(string columnName)
        {
            if(!String.IsNullOrEmpty(columnName) && !columnName.StartsWith("[") && !columnName.EndsWith("]"))
                return "[" + columnName + "]";

            return String.Empty;
        }

        /// <summary>
        /// Gets the single record reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDataReader GetSingleRecordReader(QueryCommand qry)
        {
            AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this);
            SqlCeCommand cmd = new SqlCeCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = 0;
            AddParams(cmd, qry);

            cmd.Connection = (SqlCeConnection)automaticConnectionScope.Connection;

            // if it is a shared connection, we shouldn't be telling the reader to close it when it is done
            if(automaticConnectionScope.IsUsingSharedConnection)
                return cmd.ExecuteReader();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleRow);
        }


        #region SQL Generation Support

        public override ISqlGenerator GetSqlGenerator(SqlQuery sqlQuery)
        {
            return new SqlLiteGenerator(sqlQuery);
        }

        #endregion


        #region SQL Builders

        //this is only used with the SQL constructors below
        //it's not used in the command builders above, which need to set the parameters
        //right at the time of the command build

        /// <summary>
        /// Creates a SELECT statement based on the Query object settings
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override string GetSelectSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;

            //different rules for how to do TOP
            string select = "/* GetSelectSql(" + table.Name + ") */ " + SqlFragment.SELECT + " ";

            StringBuilder order = new StringBuilder();
            StringBuilder query = new StringBuilder();
            string columns;

            //append on the selectList, which is a property that can be set
            //and is "*" by default

            if(qry.SelectList != null && qry.SelectList.Trim().Length >= 2)
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
                if (table.PrimaryKey != null && (!qry.IsDistinct || columns.Contains(table.PrimaryKey.ColumnName)))
                    order.Append(SqlFragment.ORDER_BY + OrderBy.Asc(table.PrimaryKey.ColumnName).OrderString);
            }

            if(qry.PageIndex < 0)
            {
                query.Append(select);
                query.Append(columns);
                query.Append(SqlFragment.FROM);
                query.Append(table.QualifiedName);
                query.Append(where);
                query.Append(order);
                query.Append(";");
            }
            else
            {
                if(table.PrimaryKey != null)
                {
                    query.Append(
                        string.Format(PAGING_SQL, table.PrimaryKey.ColumnName, table.Name, columns, where, order, qry.PageIndex, qry.PageSize,
                            Utility.GetSqlDBType(table.PrimaryKey.DataType)));
                    query.Append(";");
                }
                else
                {
                    //pretend it's a view
                    query.Append(string.Format(
                        PAGING_VIEW_SQL,
                        qry.Provider.QualifyTableName(table.SchemaName, table.Name),
                        GetQualifiedSelect(table),
                        where,
                        order,
                        qry.PageIndex,
                        qry.PageSize));
                    query.Append(";");
                }
            }

            return query.ToString();
        }

        /// <summary>
        /// Gets the qualified select.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        protected static string GetQualifiedSelect(TableSchema.Table table)
        {
            StringBuilder sb = new StringBuilder();
            foreach(TableSchema.TableColumn tc in table.Columns)
                sb.AppendFormat(",[{0}].[{1}]", table.Name, tc.ColumnName);

            string result = sb.ToString();
            if(result.Length > 1)
                result = sb.ToString().Substring(1);

            return result;
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
            string insertSQL = "/* GetInsertSql(" + table.Name + ") */ " + SqlFragment.INSERT_INTO + table.QualifiedName;
            //string client = DataService.GetClientType();

            string cols = String.Empty;
            string pars = String.Empty;

            //returns Guid from VS2005 only!
            //bool primaryKeyIsGuid = false;
            //string primaryKeyName = "";

            bool isFirstColumn = true;
            //if table columns are null toss an exception
            foreach(TableSchema.TableColumn col in table.Columns)
            {
                string colName = col.ColumnName;
                if(!(col.DataType == DbType.Guid && col.DefaultSetting != null && col.DefaultSetting == SqlSchemaVariable.DEFAULT))
                {
                    if(!col.AutoIncrement && !col.IsReadOnly)
                    {
                        if(!isFirstColumn)
                        {
                            cols += ",";
                            pars += ",";
                        }

                        isFirstColumn = false;

                        cols += FormatIdentifier(colName);
                        pars += FormatParameterNameForSQL(colName);
                    }
                    if(col.IsPrimaryKey && col.DataType == DbType.Guid)
                    {
                        //primaryKeyName = col.ColumnName;
                        //primaryKeyIsGuid = true;
                    }
                }
            }

            insertSQL += "(" + cols + ") ";
            insertSQL += "VALUES(" + pars + ");";

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
            //bool isNumeric = false;
            //TableSchema.TableColumn thisColumn=null;

            while(rdr.Read())
            {
                StringBuilder thisStatement = new StringBuilder();
                thisStatement.Append(insertStatement);
                thisStatement.Append("VALUES(");
                //loop the schema and pull out the values from the reader
                foreach(TableSchema.TableColumn col in table.Columns)
                {
                    object oData = rdr[col.ColumnName];
                    if(oData != null && oData != DBNull.Value)
                    {
                        if(col.DataType == DbType.Boolean)
                        {
                            bool bData = Convert.ToBoolean(oData);
                            thisStatement.Append(bData ? "1" : " 0");
                        }
                        else if(col.DataType == DbType.Byte || col.DataType == DbType.Binary)
                        {
                            thisStatement.Append("0x");
                            thisStatement.Append(Utility.ByteArrayToString((Byte[])oData).ToUpper());
                        }
                        else if(col.IsNumeric)
                            thisStatement.Append(oData);
                        else
                        {
                            thisStatement.Append("'");
                            thisStatement.Append(oData.ToString().Replace("'", "''"));
                            thisStatement.Append("'");
                        }
                    }
                    else
                        thisStatement.Append("NULL");

                    if(!Utility.IsMatch(col.ColumnName, lastColumnName))
                        thisStatement.Append(", ");
                }

                //add in a closing paren
                thisStatement.AppendLine(")");
                statements.Append(thisStatement);
            }
            rdr.Close();

            //if identity is set for the PK, set IDENTITY INSERT to true
            result.AppendLine(disableConstraint.ToString());
            if(table.PrimaryKey != null)
            {
                if(table.PrimaryKey.AutoIncrement)
                    result.AppendLine("SET IDENTITY_INSERT [" + tableName + "] ON ");
            }
            result.AppendLine("PRINT 'Begin inserting data in " + tableName + "'");
            result.Append(statements);

            if(table.PrimaryKey != null)
            {
                if(table.PrimaryKey.AutoIncrement)
                    result.AppendLine("SET IDENTITY_INSERT [" + tableName + "] OFF ");
            }
            result.AppendLine(enableConstraint.ToString());
            return result.ToString();
        }

        #endregion


        #region Schema Bits

        private const string FOREIGN_TABLE_LIST =
            @"SELECT KCU.COLUMN_NAME,TC.TABLE_NAME
                    FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
                    JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ON KCU.CONSTRAINT_NAME=RC.CONSTRAINT_NAME
                    JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC  ON RC.UNIQUE_CONSTRAINT_NAME=TC.CONSTRAINT_NAME
                    WHERE KCU.TABLE_NAME=@tblName";

        private const string GET_FOREIGN_KEY_SQL =
            @"SELECT TC.TABLE_NAME AS TableName
                                           FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
                                           JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ON KCU.CONSTRAINT_NAME=RC.CONSTRAINT_NAME
                                           JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC  ON RC.UNIQUE_CONSTRAINT_NAME=TC.CONSTRAINT_NAME
                                           WHERE KCU.COLUMN_NAME=@columnName AND KCU.TABLE_NAME = @tblName";

        private const string GET_PRIMARY_KEY_SQL =
            @"SELECT KCU.TABLE_NAME AS TableName, KCU.COLUMN_NAME AS ColumnName
                                           FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
                                           JOIN INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC ON KCU.CONSTRAINT_NAME=RC.CONSTRAINT_NAME
                                           JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC ON RC.UNIQUE_CONSTRAINT_NAME=TC.CONSTRAINT_NAME
                                           WHERE TC.TABLE_NAME = @tblName";

        private const string GET_TABLE_SQL =
            @"SELECT KCU.TABLE_NAME as TableName
                                            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
                                            JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
                                            ON KCU.CONSTRAINT_NAME=TC.CONSTRAINT_NAME
                                            WHERE KCU.COLUMN_NAME=@columnName AND TC.CONSTRAINT_TYPE='PRIMARY KEY'";

        //thanks Jon G!

        private const string PAGING_SQL =
            @"					
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
						[{1}]
					{3}
					{4}

					-- select only those rows belonging to the proper page
					SELECT 
					{2}
					FROM {1}
					INNER JOIN @TempTable t ON [{1}].[{0}] = t.[_keyID]
					WHERE t.IndexId BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)";

        private const string PAGING_VIEW_SQL =
            @"					
					DECLARE @Page int
					DECLARE @PageSize int

					SET @Page = {4}
					SET @PageSize = {5}

					SET NOCOUNT ON

                    SELECT _indexID = IDENTITY(int, 1, 1), {1} INTO #temp FROM {0} WHERE 1 = 0
                    INSERT INTO #temp ({1}) SELECT {1} FROM {0} {2} {3}

                    SELECT * FROM #temp
                    WHERE _indexID BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)
					
                    --clean up
                    DROP TABLE #temp
                    ";

        private const string SP_SQL =
            @"SELECT SPECIFIC_NAME AS SPName, ROUTINE_DEFINITION AS SQL, CREATED AS CreatedOn, LAST_ALTERED AS ModifiedOn 
                                    FROM INFORMATION_SCHEMA.ROUTINES
                                    WHERE ROUTINE_TYPE='PROCEDURE' AND SPECIFIC_NAME NOT LIKE 'sp_%diagram%'
                                    ORDER BY SPName ASC";

        private const string TABLE_BY_PK =
            @"SELECT TC.TABLE_NAME as tableName 
                        FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE KCU
                        JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC
                        ON KCU.CONSTRAINT_NAME=TC.CONSTRAINT_NAME 
                        WHERE column_name=@columnName
                        AND CONSTRAINT_TYPE='PRIMARY KEY'
                        AND TC.TABLE_NAME NOT LIKE '%'+@mapSuffix
                        AND KCU.ORDINAL_POSITION=1";

        private const string TABLE_SQL =
            @"SELECT TABLE_CATALOG AS [Database], TABLE_SCHEMA AS Owner, TABLE_NAME AS Name, TABLE_TYPE 
                                        FROM INFORMATION_SCHEMA.TABLES
                                        WHERE (TABLE_TYPE = 'BASE TABLE') AND (TABLE_NAME <> N'sysdiagrams') AND (TABLE_NAME <> N'dtproperties')";

        private const string VIEW_SQL =
            @"SELECT TABLE_CATALOG AS [Database], TABLE_SCHEMA AS Owner, TABLE_NAME AS Name, TABLE_TYPE
                                        FROM INFORMATION_SCHEMA.TABLES
                                        WHERE (TABLE_TYPE = 'VIEW') AND (TABLE_NAME <> N'sysdiagrams')";

        #endregion
    }
}

#endif