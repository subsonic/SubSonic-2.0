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
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class OracleGenerator : ANSISqlGenerator
    {
        private const string PAGING_SQL =
            @"		SELECT *
                    FROM(
						{0}
                    )
					WHERE row_number BETWEEN {1} AND {2}              
                    ";
        /// <summary>
        /// Initializes a new instance of the <see cref="OracleGenerator"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public OracleGenerator(SqlQuery query)
            : base(query) {}


        #region Base Class Overrides

        /// <summary>
        /// Qualifies the name of the table.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <returns></returns>
        public override string QualifyTableName(TableSchema.Table tableSchema)
        {
            string result = string.Format("\"{0}\".\"{1}\"", tableSchema.SchemaName, tableSchema.Name);
            return result;
        }

        /// <summary>
        /// Generates the 'SELECT' part of an <see cref="Aggregate"/>
        /// </summary>
        /// <param name="aggregate">The aggregate to include in the SELECT clause</param>
        /// <returns>
        /// The portion of the SELECT clause represented by this <see cref="Aggregate"/>
        /// </returns>
        /// <remarks>
        /// The ToString() logic moved from <see cref="Aggregate.ToString"/>, rather than
        /// including it in the Aggregate class itself...
        /// </remarks>
        protected override string GenerateAggregateSelect(Aggregate aggregate)
        {
            bool hasAlias = !String.IsNullOrEmpty(aggregate.Alias);

            if(aggregate.AggregateType == AggregateFunction.GroupBy && hasAlias)
                return String.Format("{0} AS \"{1}\"", aggregate.ColumnName, aggregate.Alias);
            if(aggregate.AggregateType == AggregateFunction.GroupBy)
                return aggregate.ColumnName;
            if(hasAlias)
                return String.Format("{0}({1}) AS \"{2}\"", Aggregate.GetFunctionType(aggregate).ToUpper(), aggregate.ColumnName, aggregate.Alias);
            return string.Format("{0}({1})", Aggregate.GetFunctionType(aggregate).ToUpper(), aggregate.ColumnName);
        }

        /// <summary>
        /// Builds the paged select statement.
        /// </summary>
        /// <returns></returns>
        public override string BuildPagedSelectStatement()
        {
            string idColumn = GetSelectColumns()[0];
            string sqlType;

            TableSchema.TableColumn idCol = FindColumn(idColumn);
            if (idCol != null)
            {
                string pkType = String.Empty;
                if (Utility.IsString(idCol))
                    pkType = String.Concat("(", idCol.MaxLength, ")");
                sqlType = Enum.GetName(typeof(SqlDbType), Utility.GetSqlDBType(idCol.DataType));
                sqlType = String.Concat(sqlType, pkType);
            }
            else
            {
                //assume it's an integer
                sqlType = Enum.GetName(typeof(SqlDbType), SqlDbType.Int);
            }

            string select = GenerateCommandLine();
            string fromLine = GenerateFromList();
            string joins = GenerateJoins();
            string wheres = GenerateWhere();
            string havings = string.Empty;
            string groupby = string.Empty;

            string orderby = GenerateOrderBy();

            if (query.Aggregates.Count > 0)
            {
                groupby = GenerateGroupBy();
                havings = GenerateHaving();
            }

            int startnum = query.PageSize*query.CurrentPage +1;
            int endnum = query.PageSize*query.CurrentPage + query.PageSize;
            string sql = string.Format(PAGING_SQL
                , String.Concat(select, fromLine.Replace("FROM", ", ROWNUM as row_number  FROM"), joins, wheres, groupby, havings)
                , startnum, endnum);

            return sql;
        }

        #endregion
    }
}