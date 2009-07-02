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
using MySql.Data.MySqlClient;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the MySqlInnoDBDataProvider class
    /// </summary>
    public class MySqlInnoDBDataProvider : MySqlDataProvider
    {
        private const string ALL_TABLE_COLUMNS_SQL =
            @"SELECT
      TABLE_SCHEMA as `Database`,
      TABLE_NAME as TableName,
      COLUMN_NAME as ColumnName,
      ORDINAL_POSITION as OrdinalPosition,
      COLUMN_DEFAULT as DefaultSetting,
      IS_NULLABLE as IsNullable,
      DATA_TYPE as DataType,
      CHARACTER_MAXIMUM_LENGTH as MaxLength,
      IF(EXTRA = 'auto_increment', 1, 0) as IsIdentity
FROM
      INFORMATION_SCHEMA.COLUMNS
WHERE
      TABLE_SCHEMA = ?DatabaseName
ORDER BY
      OrdinalPosition ASC";

        private const string ALL_TABLE_FOREIGN_TABLES =
            @"SELECT
      table_name as FK_TABLE,
      referenced_column_name as PK_COLUMN,
      referenced_table_name as PK_TABLE,
      column_name as FK_Column,
      constraint_name as CONSTRAINT_NAME

FROM
      INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE
      TABLE_SCHEMA = ?DatabaseName
      AND REFERENCED_TABLE_NAME IS NOT NULL";

        private const string ALL_TABLE_INDEXES_SQL =
            @"SELECT
      tc.table_name as TableName,
      tc.table_schema as Owner,
      kc.column_name as ColumnName,
      tc.constraint_type as ConstraintType,
      tc.constraint_name as ConstraintName
FROM
      INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
      INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kc
            ON tc.table_schema = kc.table_schema
            AND tc.table_name = kc.table_name
            AND tc.constraint_name = kc.constraint_name
WHERE
       tc.table_schema = ?DatabaseName";

        private const string ALL_TABLE_PRIMARY_TABLES =
            @"SELECT
      referenced_table_name as PK_TABLE,
      table_name as FK_TABLE,
      column_name as FK_COLUMN
FROM
      INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE
      TABLE_SCHEMA = ?DatabaseName
      AND REFERENCED_TABLE_NAME IS NOT NULL";

        private const string ALL_TABLES_SQL =
            @"SELECT
      TABLE_NAME as Table_Name
FROM
      INFORMATION_SCHEMA.TABLES
WHERE
      TABLE_SCHEMA = ?DatabaseName";

        private const string MANY_TO_MANY_CHECK_ALL =
            @"SELECT 
    FK.TABLE_NAME FK_Table, 
    KC.COLUMN_NAME FK_Column,
    KC.REFERENCED_TABLE_NAME PK_Table,
    KC.REFERENCED_COLUMN_NAME PK_Column,     
    FK.CONSTRAINT_NAME  Constraint_Name    
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK  
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KC ON KC.CONSTRAINT_NAME  = FK.CONSTRAINT_NAME
AND KC.TABLE_NAME = FK.TABLE_NAME AND KC.TABLE_SCHEMA = FK.TABLE_SCHEMA
AND FK.TABLE_SCHEMA = ?DatabaseName
AND FK.CONSTRAINT_TYPE = 'FOREIGN KEY'
JOIN    (
        SELECT tc.TABLE_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
        JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu ON tc.Constraint_name = kcu.Constraint_Name AND kcu.TABLE_NAME = tc.TABLE_NAME AND kcu.TABLE_SCHEMA = tc.TABLE_SCHEMA
        AND tc.Constraint_Type = 'PRIMARY KEY' 
        AND tc.TABLE_SCHEMA = ?DatabaseName
        JOIN 
        (
            SELECT tc1.Table_Name, kcu1.Column_Name AS Column_Name FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc1
            JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS kcu1 ON tc1.Constraint_name = kcu1.Constraint_Name AND kcu1.TABLE_NAME = tc1.TABLE_NAME AND kcu1.TABLE_SCHEMA = tc1.TABLE_SCHEMA
            AND tc1.Constraint_Type = 'FOREIGN KEY' 
            AND tc1.TABLE_SCHEMA = ?DatabaseName
        ) 
        AS t ON t.Table_Name = tc.table_Name AND t.Column_Name = kcu.Column_Name  
       
        GROUP BY tc.Constraint_Name, tc.Table_Name HAVING COUNT(*) > 1
        ) AS ManyMany ON ManyMany.TABLE_NAME = FK.TABLE_NAME";

        private const string MANY_TO_MANY_FOREIGN_MAP_ALL =
            @"SELECT 
    FK.TABLE_NAME FK_Table, 
    KC.COLUMN_NAME FK_Column,
    KC.REFERENCED_TABLE_NAME PK_Table,
    KC.REFERENCED_COLUMN_NAME PK_Column,     
    FK.CONSTRAINT_NAME  Constraint_Name    
FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK  
INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KC ON KC.CONSTRAINT_NAME  = FK.CONSTRAINT_NAME
AND KC.TABLE_NAME = FK.TABLE_NAME AND KC.TABLE_SCHEMA = FK.TABLE_SCHEMA
AND FK.TABLE_SCHEMA = ?DatabaseName
AND FK.CONSTRAINT_TYPE = 'FOREIGN KEY'
";

        private static readonly object _lockColumns = new object();

        private static readonly object _lockFK = new object();
        private static readonly object _lockIndex = new object();
        private static readonly object _lockManyToManyCheck = new object();
        private static readonly object _lockManyToManyMap = new object();
        private static readonly object _lockPK = new object();

        private static readonly DataSet dsColumns = new DataSet();
        private static readonly DataSet dsFK = new DataSet();
        private static readonly DataSet dsIndex = new DataSet();
        private static readonly DataSet dsManyToManyCheck = new DataSet();
        private static readonly DataSet dsManyToManyMap = new DataSet();
        private static readonly DataSet dsPK = new DataSet();
        private static readonly DataSet dsTables = new DataSet();

        /// <summary>
        /// Gets the name of the foreign key table.
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName)
        {
            string tableName = String.Empty;

            if(SupportsInformationSchema(GetDatabaseVersion(Name)))
            {
                string sql =
                    "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE COLUMN_NAME = ?ColumnName AND CONSTRAINT_NAME = 'PRIMARY' AND TABLE_SCHEMA = ?DatabaseName";

                using(AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
                {
                    MySqlCommand cmd = new MySqlCommand(sql);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = (MySqlConnection)automaticConnectionScope.Connection;

                    cmd.Parameters.AddWithValue("?ColumnName", fkColumnName);
                    cmd.Parameters.AddWithValue("?DatabaseName", cmd.Connection.Database);

                    object result = cmd.ExecuteScalar();
                    if(result != null)
                        tableName = result.ToString();
                }
            }

            return tableName;
        }

        /// <summary>
        /// Gets the name of the foreign key table.
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName, string tableName)
        {
            string returnTableName = String.Empty;

            if(SupportsInformationSchema(GetDatabaseVersion(Name)))
            {
                string sql =
                    "SELECT REFERENCED_TABLE_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE COLUMN_NAME = ?ColumnName AND TABLE_NAME = ?TableName AND TABLE_SCHEMA = ?DatabaseName";

                using(AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
                {
                    MySqlCommand cmd = new MySqlCommand(sql);
                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = (MySqlConnection)automaticConnectionScope.Connection;

                    cmd.Parameters.AddWithValue("?ColumnName", fkColumnName);
                    cmd.Parameters.AddWithValue("?TableName", tableName);
                    cmd.Parameters.AddWithValue("?DatabaseName", cmd.Connection.Database);

                    object result = cmd.ExecuteScalar();
                    if(result != null)
                        returnTableName = result.ToString();
                }
            }

            return returnTableName;
        }

        /// <summary>
        /// Reloads the cached schema
        /// </summary>
        public override void ReloadSchema()
        {
            //not sure how to do this here
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <returns></returns>
        public override TableSchema.Table GetTableSchema(string tableName, TableType tableType)
        {
            //return base.GetTableSchema(tableName, tableType);

            MySqlConnection conn = new MySqlConnection(DefaultConnectionString);

            if(dsColumns.Tables[Name] == null)
            {
                lock(_lockColumns)
                {
                    QueryCommand cmdColumns = new QueryCommand(ALL_TABLE_COLUMNS_SQL, Name);
                    cmdColumns.Parameters.Add("?DatabaseName", conn.Database, DbType.AnsiString);

                    DataTable dt = new DataTable(Name);
                    dt.Load(GetReader(cmdColumns));
                    dsColumns.Tables.Add(dt);
                }
            }

            DataRow[] drColumns = dsColumns.Tables[Name].Select("TableName ='" + tableName + "'", "OrdinalPosition ASC");

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

                if(SetPropertyDefaultsFromDatabase && drColumns[i][SqlSchemaVariable.COLUMN_DEFAULT] != DBNull.Value &&
                   drColumns[i][SqlSchemaVariable.COLUMN_DEFAULT].ToString() != "\0")
                    column.DefaultSetting = drColumns[i][SqlSchemaVariable.COLUMN_DEFAULT].ToString().Trim();

                //thanks rauchy!
                bool autoIncrement;
                bool successfullyParsed = bool.TryParse(drColumns[i][SqlSchemaVariable.IS_IDENTITY].ToString(), out autoIncrement);
                if(!successfullyParsed)
                    autoIncrement = Convert.ToBoolean(drColumns[i][SqlSchemaVariable.IS_IDENTITY]);
                column.AutoIncrement = autoIncrement;

                int maxLength;
                int.TryParse(drColumns[i][SqlSchemaVariable.MAX_LENGTH].ToString(), out maxLength);
                column.MaxLength = maxLength;

                column.IsNullable = (drColumns[i][SqlSchemaVariable.IS_NULLABLE].ToString() == "YES");
                //column.IsReadOnly = (nativeDataType == "timestamp");

                columns.Add(column);
            }

            if(dsIndex.Tables[Name] == null)
            {
                lock(_lockIndex)
                {
                    QueryCommand cmdIndex = new QueryCommand(ALL_TABLE_INDEXES_SQL, Name);
                    cmdIndex.Parameters.Add("?DatabaseName", conn.Database, DbType.AnsiString);
                    DataTable dt = new DataTable(Name);
                    dt.Load(GetReader(cmdIndex));
                    dsIndex.Tables.Add(dt);
                }
            }

            DataRow[] drIndexes = dsIndex.Tables[Name].Select("TableName = '" + tableName + "'");
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
            }

            if(dsPK.Tables[Name] == null)
            {
                lock(_lockPK)
                {
                    QueryCommand cmdPk = new QueryCommand(ALL_TABLE_PRIMARY_TABLES, Name);
                    cmdPk.Parameters.Add("?DatabaseName", conn.Database, DbType.AnsiString);
                    DataTable dt = new DataTable(Name);
                    dt.Load(GetReader(cmdPk));
                    dsPK.Tables.Add(dt);
                }
            }

            DataRow[] drPK = dsPK.Tables[Name].Select("PK_Table ='" + tableName + "'");
            for(int i = 0; i < drPK.Length; i++)
            {
                string colName = drPK[i]["FK_Column"].ToString();
                string fkName = drPK[i]["FK_Table"].ToString();

                TableSchema.PrimaryKeyTable pkTable = new TableSchema.PrimaryKeyTable(this);
                pkTable.ColumnName = colName;
                pkTable.TableName = fkName;
                tbl.PrimaryKeyTables.Add(pkTable);
            }

            if(dsFK.Tables[Name] == null)
            {
                lock(_lockFK)
                {
                    QueryCommand cmdFK = new QueryCommand(ALL_TABLE_FOREIGN_TABLES, Name);
                    cmdFK.Parameters.Add("?DatabaseName", conn.Database, DbType.AnsiString);
                    DataTable dt = new DataTable(Name);
                    dt.Load(GetReader(cmdFK));
                    dsFK.Tables.Add(dt);
                }
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
                    string fkName = CorrectTableCasing(drFK[i]["PK_Table"].ToString(), conn.Database);
                    TableSchema.TableColumn column = columns.GetColumn(colName);

                    if(column != null)
                        column.ForeignKeyTableName = fkName;
                    else
                        continue;

                    TableSchema.ForeignKeyTable fkTable = new TableSchema.ForeignKeyTable(this);
                    fkTable.ColumnName = colName;
                    fkTable.TableName = fkName;
                    tbl.ForeignKeys.Add(fkTable);
                }
            }

            if(dsManyToManyCheck.Tables[Name] == null)
            {
                lock(_lockManyToManyCheck)
                {
                    QueryCommand cmdM2M = new QueryCommand(MANY_TO_MANY_CHECK_ALL, Name);
                    cmdM2M.Parameters.Add("?DatabaseName", conn.Database, DbType.AnsiString);
                    DataTable dt = new DataTable(Name);
                    dt.Load(GetReader(cmdM2M));
                    dsManyToManyCheck.Tables.Add(dt);
                }
            }

            DataRow[] drs = dsManyToManyCheck.Tables[Name].Select("PK_Table = '" + tableName + "'");
            if(drs.Length > 0)
            {
                for(int count = 0; count < drs.Length; count++)
                {
                    string mapTable = drs[count]["FK_Table"].ToString();
                    string localKey = drs[count]["FK_Column"].ToString();
                    if(dsManyToManyMap.Tables[Name] == null)
                    {
                        lock(_lockManyToManyMap)
                        {
                            QueryCommand cmdM2MMap = new QueryCommand(MANY_TO_MANY_FOREIGN_MAP_ALL, Name);
                            cmdM2MMap.Parameters.Add("?DatabaseName", conn.Database, DbType.AnsiString);
                            DataTable dt = new DataTable(Name);
                            dt.Load(GetReader(cmdM2MMap));
                            dsManyToManyMap.Tables.Add(dt);
                        }
                    }

                    DataRow[] drMap = dsManyToManyMap.Tables[Name].Select("FK_Table = '" + mapTable + "' AND PK_Table <> '" + tableName + "'");

                    for(int i = 0; i < drMap.Length; i++)
                    {
                        TableSchema.ManyToManyRelationship m = new TableSchema.ManyToManyRelationship(mapTable, tbl.Provider);
                        m.ForeignTableName = drMap[i]["PK_Table"].ToString();
                        m.ForeignPrimaryKey = drMap[i]["PK_Column"].ToString();
                        m.MapTableLocalTableKeyColumn = localKey;
                        m.MapTableForeignTableKeyColumn = drMap[i]["FK_Column"].ToString();
                        tbl.ManyToManys.Add(m);
                    }
                }
            }

            tbl.Columns = columns;

            return tbl;
        }

        /// <summary>
        /// Hack for windows on mysql and the fact that it does
        /// not keep casing in some fields of the information_schema
        /// </summary>
        /// <param name="TableName">Name of the table.</param>
        /// <param name="DatabaseName">Name of the database.</param>
        /// <returns></returns>
        private string CorrectTableCasing(string TableName, string DatabaseName)
        {
            if(dsTables.Tables[Name] == null)
            {
                QueryCommand cmdTables = new QueryCommand(ALL_TABLES_SQL, Name);
                cmdTables.Parameters.Add("?DatabaseName", DatabaseName, DbType.AnsiString);
                DataTable dt = new DataTable(Name);
                dt.Load(GetReader(cmdTables));
                dsTables.Tables.Add(dt);
            }

            DataRow[] table = dsTables.Tables[Name].Select("Table_Name ='" + TableName + "'");
            if(table.Length == 1)
                return table[0]["Table_Name"].ToString();

            return TableName;
        }
    }
}

#endif