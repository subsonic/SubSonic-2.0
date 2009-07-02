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

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class Sql2005Generator : ANSISqlGenerator
    {
        private const string PAGING_SQL =
            @"
SELECT {7}
FROM     (SELECT ROW_NUMBER() OVER ({1}) AS Row, 
{0} 
{2}
{3}
{4}
)
            AS PagedResults
WHERE  Row >= {5} AND Row <= {6}";

        /// <summary>
        /// Initializes a new instance of the <see cref="Sql2005Generator"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public Sql2005Generator(SqlQuery query)
            : base(query) {}

        /// <summary>
        /// Builds the paged select statement.
        /// </summary>
        /// <returns></returns>
        public override string BuildPagedSelectStatement()
        {
            SqlQuery qry = query;

            string top = "*";
            string idColumn = GetSelectColumns()[0];

            string select = GenerateCommandLine();
            string columnList = select.Replace("SELECT", String.Empty);
            string fromLine = GenerateFromList();
            string joins = GenerateJoins();
            string wheres = GenerateWhere();
            string orderby = GenerateOrderBy();

            if(String.IsNullOrEmpty(orderby.Trim()))
                orderby = String.Concat(SqlFragment.ORDER_BY, idColumn);

            if(qry.Aggregates.Count > 0)
                joins = String.Concat(joins, GenerateGroupBy());

            // If the query has a top defined
            if (!String.IsNullOrEmpty(qry.TopSpec))
            {
                // Remove the top string from the column list
                columnList = columnList.Replace(qry.TopSpec, String.Empty).Trim();

                // Format the top statement (as in "top 1 *")
                top = String.Concat(String.Format("{0} {1}", qry.TopSpec, top));
            }

            int pageStart = (qry.CurrentPage - 1) * qry.PageSize + 1;
            int pageEnd = qry.CurrentPage * qry.PageSize;

            string sql = string.Format(PAGING_SQL, columnList, orderby, fromLine, joins, wheres, pageStart, pageEnd, top);
            return sql;
        }
    }
}
