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
        /// Gets the count select.
        /// </summary>
        /// <returns></returns>
        public override string GetCountSelect()
        {
            return base.GetCountSelect().Replace('\'', '"');
        }

        /// <summary>
        /// Builds the paged select statement.
        /// </summary>
        /// <returns></returns>
        public override string BuildPagedSelectStatement()
        {
            int startnum = query.PageSize * query.CurrentPage + 1;
            int endnum = query.PageSize * query.CurrentPage + query.PageSize;
            string orderBy = String.Empty;

            if (this.query.OrderBys.Count > 0)
                orderBy = GenerateOrderBy();

            //The ROW_NUMBER() function in Oracle requires an ORDER BY clause.
            //In case one is not specified, we need to halt and inform the caller.
            if(orderBy.Equals(String.Empty))
                throw new ArgumentException("There is no column specified for the ORDER BY clause", "OrderBys");
            
            System.Text.StringBuilder sql = new System.Text.StringBuilder();

            //Build the command string
            sql.Append("WITH pagedtable AS (");
            sql.Append(GenerateCommandLine());
            
            //Since this class is for Oracle-specific SQL, we can add a hint
            //which should help pagination queries return rows more quickly.
            //AFAIK, this is only valid for Oracle 9i or newer.
            sql.Replace("SELECT", "SELECT /*+ first_rows('" + query.PageSize + "') */");
            
            sql.Append(", ROW_NUMBER () OVER (");
            sql.Append(orderBy);
            sql.Append(") AS rowindex ");
            sql.Append(Environment.NewLine);
            sql.Append(GenerateFromList());
            sql.Append(GenerateJoins());

            sql.Append(GenerateWhere());

            if (query.Aggregates.Count > 0)
            {
                sql.Append(GenerateGroupBy());
                sql.Append(Environment.NewLine);
                sql.Append(GenerateHaving());
            }

            sql.Append(") SELECT * FROM pagedtable WHERE rowindex BETWEEN ");
            sql.Append(startnum);
            sql.Append(" AND ");
            sql.Append(endnum);
            sql.Append(" ORDER BY rowindex");

            return sql.ToString();
        }

        #endregion
    }
}