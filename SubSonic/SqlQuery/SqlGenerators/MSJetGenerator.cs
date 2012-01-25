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

using System.Text;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class MSJetGenerator : ANSISqlGenerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MSJetGenerator"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public MSJetGenerator(SqlQuery query) : base(query) {
            this.bracketEachJoin = true;
        }

        /// <summary>
        /// Qualifies the name of the table.
        /// </summary>
        /// <param name="tbl">The TBL.</param>
        /// <returns></returns>
        public override string QualifyTableName(TableSchema.Table tbl)
        {
            string result;
            // omit schemaname for Jet !
            result = string.Format("[{0}]", tbl.Name);
            return result;
        }

        /// <summary>
        /// Generates the reversed order by.
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateOrderByReversed()
        {
            StringBuilder sb = new StringBuilder();
            if (query.OrderBys.Count > 0)
            {
                sb.Append(SqlFragment.ORDER_BY);
                bool isFirst = true;
				foreach (OrderBySQ ob in query.OrderBys)
                {
                    if (!isFirst)
                        sb.Append(",");
					sb.Append(OrderBySQ.GetOrderDirectionValue(OrderBySQ.ReverseDirection(ob.Direction)));
                    isFirst = false;
                }
                sb.AppendLine();
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
            //string columnList = select.Replace("SELECT", "");
            string fromLine = GenerateFromList();
            string joins = GenerateJoins();
			string wheres = GenerateWhere();

            //have to doctor the wheres, since we're using a WHERE in the paging
            //bits. So change all "WHERE" to "AND"
            string tweakedWheres = wheres.Replace("WHERE", "AND");
            string orderby = GenerateOrderBy();
            string orderByReversed = GenerateOrderByReversed();

            if (query.Aggregates.Count > 0)
                joins += GenerateGroupBy();

            StringBuilder tempQuery = new StringBuilder();
            tempQuery.Append(select);
            tempQuery.Append(fromLine);
            tempQuery.Append(joins);
            tempQuery.Append(wheres);

            StringBuilder sql = new StringBuilder();
            if (query.CurrentPage == 1)
            {
                sql.Append(string.Format(
                        PAGING_SQL_FIRST_PAGE,
                        query.PageSize,
                        tempQuery));
                sql.Append(";");
            }
            else {
                sql.Append(string.Format(
                        PAGING_SQL_OTHER_PAGES,
                        query.PageSize,
                        query.PageSize * (query.CurrentPage + 1),
                        orderby,
                        orderByReversed,
                        tempQuery));
                sql.Append(";");
                }
            return sql.ToString();
        }

        #region Constants

        // Paging Template (sample page=3, pagelen=10) :
        //
        //  SELECT TOP 10 *
        //  FROM (
        //      SELECT TOP 30 *
        //      FROM (
        //          SELECT CategoryID, CategoryName, ProductName, ProductSales
        //          FROM [Sales by Category]
        //          ORDER BY CategoryName ASC, ProductName ASC
        //      ) AS t0
        //      ORDER BY CategoryName ASC, ProductName ASC
        //  ) AS t1
        //  ORDER BY CategoryName DESC , ProductName DESC;

        private const string PAGING_SQL_FIRST_PAGE =
            @"					
        SELECT TOP {0} *
        FROM ({1}) as t0
        ";

        private const string PAGING_SQL_OTHER_PAGES =
            @"
        SELECT *
        FROM (
            SELECT TOP {0} *
            FROM (
                SELECT TOP {1} *
                FROM ({4}) as t0
                {2}
                ) as t1
            {3}
            ) as t2
        {2}";

#endregion
   }
}
