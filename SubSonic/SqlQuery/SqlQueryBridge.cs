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

using System.Collections.Generic;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlQueryBridge
    {
        /// <summary>
        /// Adds the legacy where collection.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="whereCollection">The where collection.</param>
        public static void AddLegacyWhereCollection(SqlQuery sqlQuery, List<Where> whereCollection)
        {
            if(whereCollection != null)
            {
                foreach(Where where in whereCollection)
                {
                    ConstraintType constraintType;
                    if(where.Condition == Where.WhereCondition.AND)
                        constraintType = ConstraintType.And;
                    else if(where.Condition == Where.WhereCondition.OR)
                        constraintType = ConstraintType.Or;
                    else
                        constraintType = !sqlQuery.HasWhere ? ConstraintType.Where : ConstraintType.And;

                    Constraint c = new Constraint(constraintType, where.ColumnName);
                    c.Comparison = where.Comparison;
                    c.ParameterName = where.ParameterName;
                    c.ParameterValue = where.ParameterValue;
                    sqlQuery.Constraints.Add(c);
                }
            }
        }
    }
}