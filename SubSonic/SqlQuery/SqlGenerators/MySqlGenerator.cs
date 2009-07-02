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
using System.Data;
using System.Text;
using SubSonic.Sugar;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class MySqlGenerator : ANSISqlGenerator
    {
        private const string PAGING_SQL =
            @"{0}
        {1}
        LIMIT {2}, {3};";

        /// <summary>
        /// Initializes a new instance of the <see cref="MySqlGenerator"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public MySqlGenerator(SqlQuery query)
            : base(query) {}

        /// <summary>
        /// Gets the type of the native.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <returns></returns>
        protected override string GetNativeType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.Object:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return "nvarchar";
                case DbType.Boolean:
                    return "bit";
                case DbType.SByte:
                case DbType.Binary:
                case DbType.Byte:
                    return "image";
                case DbType.Currency:
                    return "money";
                case DbType.Time:
                case DbType.Date:
                case DbType.DateTime:
                    return "datetime";
                case DbType.Decimal:
                    return "decimal";
                case DbType.Double:
                    return "float";
                case DbType.Guid:
                    return "uniqueidentifier";
                case DbType.UInt32:
                case DbType.UInt16:
                case DbType.Int16:
                case DbType.Int32:
                    return "INTEGER";
                case DbType.UInt64:
                case DbType.Int64:
                    return "bigint";
                case DbType.Single:
                    return "real";
                case DbType.VarNumeric:
                    return "numeric";
                case DbType.Xml:
                    return "xml";
                default:
                    return "nvarchar";
            }
        }

        public override string BuildCreateTableStatement(TableSchema.Table table)
        {
            string columnSql = GenerateColumns(table);
            return string.Format(CREATE_TABLE, "`" + table.Name + "`", columnSql);
        }

        /// <summary>
        /// Generates SQL for all the columns in table
        /// </summary>
        /// <param name="table">Table containing the columns.</param>
        /// <returns>
        /// SQL fragment representing the supplied columns.
        /// </returns>
        protected override string GenerateColumns(TableSchema.Table table)
        {
            if(table.Columns.Count == 0)
                return String.Empty;

            StringBuilder columnsSql = new StringBuilder();

            foreach(TableSchema.TableColumn col in table.Columns)
                columnsSql.AppendFormat("\r\n  `{0}`{1},", col.ColumnName, GenerateColumnAttributes(col));

            if(table.HasPrimaryKey)
                columnsSql.AppendFormat("\r\n  PRIMARY KEY (`{0}`),", table.PrimaryKey.ColumnName);

            string sql = columnsSql.ToString();
            return Strings.Chop(sql, ",");
        }

        /// <summary>
        /// Sets the column attributes.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected override string GenerateColumnAttributes(TableSchema.TableColumn column)
        {
            StringBuilder sb = new StringBuilder();
            if(column.DataType == DbType.DateTime && column.DefaultSetting == "getdate()")
            {
                //there is no way to have two fields with a NOW or CURRENT_TIMESTAMP setting
                //so need to rely on the code to help here
                sb.Append(" datetime ");
            }
            else
            {
                sb.Append(" " + GetNativeType(column.DataType));

                if(column.IsPrimaryKey)
                {
                    sb.Append(" NOT NULL");
                    if(column.IsNumeric)
                        sb.Append(" AUTO_INCREMENT");
                }
                else
                {
                    if(column.MaxLength > 0 && column.MaxLength < 8000)
                        sb.Append("(" + column.MaxLength + ")");

                    if(!column.IsNullable)
                        sb.Append(" NOT NULL");
                    else
                        sb.Append(" NULL");

                    if(!String.IsNullOrEmpty(column.DefaultSetting))
                        sb.Append(" DEFAULT " + column.DefaultSetting + " ");
                }
            }
            return sb.ToString();
        }
        
        /// <summary>
        /// Builds the paged select statement.
        /// </summary>
        /// <returns></returns>
        public override string BuildPagedSelectStatement()
        {
            string select = GenerateCommandLine();
            string fromLine = GenerateFromList();
            string joins = GenerateJoins();
            string wheres = GenerateWhere();
            string orderby = GenerateOrderBy();
            string havings = String.Empty; 
            string groupby = String.Empty;

            if (query.Aggregates.Count > 0)
            {
                havings = GenerateHaving();
                groupby = GenerateGroupBy();
            }

            string sql = string.Format(PAGING_SQL, 
                String.Concat(select, fromLine, joins), 
                String.Concat(wheres, groupby, havings, orderby),
                (query.CurrentPage - 1) * query.PageSize, query.PageSize);

            return sql;
        }
    }
}
