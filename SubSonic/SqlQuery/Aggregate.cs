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
    public class Aggregate
    {
        #region Aggregates Factories

        /// <summary>
        /// Counts the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public static Aggregate Count(TableSchema.TableColumn tableColumn)
        {
            Aggregate agg = new Aggregate(tableColumn, AggregateFunction.Count);
            return agg;
        }

        /// <summary>
        /// Counts the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Count(TableSchema.TableColumn tableColumn, string alias)
        {
            Aggregate agg = new Aggregate(tableColumn, alias, AggregateFunction.Count);
            return agg;
        }

        /// <summary>
        /// Counts the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static Aggregate Count(string columnName)
        {
            Aggregate agg = new Aggregate(columnName, AggregateFunction.Count);
            return agg;
        }

        /// <summary>
        /// Counts the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Count(string columnName, string alias)
        {
            Aggregate agg = new Aggregate(columnName, alias, AggregateFunction.Count);
            return agg;
        }

        /// <summary>
        /// Sums the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public static Aggregate Sum(TableSchema.TableColumn tableColumn)
        {
            Aggregate agg = new Aggregate(tableColumn, AggregateFunction.Sum);
            return agg;
        }

        /// <summary>
        /// Sums the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static Aggregate Sum(string columnName)
        {
            Aggregate agg = new Aggregate(columnName, AggregateFunction.Sum);
            return agg;
        }

        /// <summary>
        /// Sums the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Sum(TableSchema.TableColumn tableColumn, string alias)
        {
            Aggregate agg = new Aggregate(tableColumn, alias, AggregateFunction.Sum);
            return agg;
        }

        /// <summary>
        /// Sums the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Sum(string columnName, string alias)
        {
            Aggregate agg = new Aggregate(columnName, alias, AggregateFunction.Sum);
            return agg;
        }

        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public static Aggregate GroupBy(TableSchema.TableColumn tableColumn)
        {
            Aggregate agg = new Aggregate(tableColumn, AggregateFunction.GroupBy);
            return agg;
        }

        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static Aggregate GroupBy(string columnName)
        {
            Aggregate agg = new Aggregate(columnName, AggregateFunction.GroupBy);
            return agg;
        }

        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate GroupBy(TableSchema.TableColumn tableColumn, string alias)
        {
            Aggregate agg = new Aggregate(tableColumn, alias, AggregateFunction.GroupBy);
            return agg;
        }

        /// <summary>
        /// Groups the by.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate GroupBy(string columnName, string alias)
        {
            Aggregate agg = new Aggregate(columnName, alias, AggregateFunction.GroupBy);
            return agg;
        }

        /// <summary>
        /// Avgs the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public static Aggregate Avg(TableSchema.TableColumn tableColumn)
        {
            Aggregate agg = new Aggregate(tableColumn, AggregateFunction.Avg);
            return agg;
        }

        /// <summary>
        /// Avgs the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static Aggregate Avg(string columnName)
        {
            Aggregate agg = new Aggregate(columnName, AggregateFunction.Avg);
            return agg;
        }

        /// <summary>
        /// Avgs the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Avg(TableSchema.TableColumn tableColumn, string alias)
        {
            Aggregate agg = new Aggregate(tableColumn, alias, AggregateFunction.Avg);
            return agg;
        }

        /// <summary>
        /// Avgs the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Avg(string columnName, string alias)
        {
            Aggregate agg = new Aggregate(columnName, alias, AggregateFunction.Avg);
            return agg;
        }

        /// <summary>
        /// Maxes the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public static Aggregate Max(TableSchema.TableColumn tableColumn)
        {
            Aggregate agg = new Aggregate(tableColumn, AggregateFunction.Max);
            return agg;
        }

        /// <summary>
        /// Maxes the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static Aggregate Max(string columnName)
        {
            Aggregate agg = new Aggregate(columnName, AggregateFunction.Max);
            return agg;
        }

        /// <summary>
        /// Maxes the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Max(TableSchema.TableColumn tableColumn, string alias)
        {
            Aggregate agg = new Aggregate(tableColumn, alias, AggregateFunction.Max);
            return agg;
        }

        /// <summary>
        /// Maxes the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Max(string columnName, string alias)
        {
            Aggregate agg = new Aggregate(columnName, alias, AggregateFunction.Max);
            return agg;
        }

        /// <summary>
        /// Mins the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public static Aggregate Min(TableSchema.TableColumn tableColumn)
        {
            Aggregate agg = new Aggregate(tableColumn, AggregateFunction.Min);
            return agg;
        }

        /// <summary>
        /// Mins the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static Aggregate Min(string columnName)
        {
            Aggregate agg = new Aggregate(columnName, AggregateFunction.Min);
            return agg;
        }

        /// <summary>
        /// Mins the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Min(TableSchema.TableColumn tableColumn, string alias)
        {
            Aggregate agg = new Aggregate(tableColumn, alias, AggregateFunction.Min);
            return agg;
        }

        /// <summary>
        /// Mins the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Min(string columnName, string alias)
        {
            Aggregate agg = new Aggregate(columnName, alias, AggregateFunction.Min);
            return agg;
        }

        /// <summary>
        /// Variances the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public static Aggregate Variance(TableSchema.TableColumn tableColumn)
        {
            Aggregate agg = new Aggregate(tableColumn, AggregateFunction.Var);
            return agg;
        }

        /// <summary>
        /// Variances the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static Aggregate Variance(string columnName)
        {
            Aggregate agg = new Aggregate(columnName, AggregateFunction.Var);
            return agg;
        }

        /// <summary>
        /// Variances the specified col.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Variance(TableSchema.TableColumn tableColumn, string alias)
        {
            Aggregate agg = new Aggregate(tableColumn, alias, AggregateFunction.Var);
            return agg;
        }

        /// <summary>
        /// Variances the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate Variance(string columnName, string alias)
        {
            Aggregate agg = new Aggregate(columnName, alias, AggregateFunction.Var);
            return agg;
        }

        /// <summary>
        /// Standards the deviation.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public static Aggregate StandardDeviation(TableSchema.TableColumn tableColumn)
        {
            Aggregate agg = new Aggregate(tableColumn, AggregateFunction.StDev);
            return agg;
        }

        /// <summary>
        /// Standards the deviation.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static Aggregate StandardDeviation(string columnName)
        {
            Aggregate agg = new Aggregate(columnName, AggregateFunction.StDev);
            return agg;
        }

        /// <summary>
        /// Standards the deviation.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate StandardDeviation(TableSchema.TableColumn tableColumn, string alias)
        {
            Aggregate agg = new Aggregate(tableColumn, alias, AggregateFunction.StDev);
            return agg;
        }

        /// <summary>
        /// Standards the deviation.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="alias">The alias.</param>
        /// <returns></returns>
        public static Aggregate StandardDeviation(string columnName, string alias)
        {
            Aggregate agg = new Aggregate(columnName, alias, AggregateFunction.StDev);
            return agg;
        }

        #endregion


        #region .ctors

        private AggregateFunction _aggregateType = AggregateFunction.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        public Aggregate(string columnName, AggregateFunction aggregateType)
        {
            _columnName = columnName;
            _aggregateType = aggregateType;
            _alias = String.Concat(GetFunctionType(this), "Of", columnName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate"/> class.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        public Aggregate(string columnName, string alias, AggregateFunction aggregateType)
        {
            _columnName = columnName;
            _aggregateType = aggregateType;
            _alias = alias;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        public Aggregate(TableSchema.TableColumn column, AggregateFunction aggregateType)
        {
            _columnName = column.QualifiedName;
            _aggregateType = aggregateType;
            _alias = String.Concat(GetFunctionType(this), "Of", column.ColumnName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Aggregate"/> class.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="aggregateType">Type of the aggregate.</param>
        public Aggregate(TableSchema.TableColumn column, string alias, AggregateFunction aggregateType)
        {
            _columnName = column.QualifiedName;
            _alias = alias;
            _aggregateType = aggregateType;
        }

        /// <summary>
        /// Gets the type of the function.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        public static string GetFunctionType(Aggregate aggregate)
        {
            return Enum.GetName(typeof(AggregateFunction), aggregate.AggregateType);
        }

        #endregion


        private string _alias;
        private string _columnName;

        /// <summary>
        /// Gets or sets the type of the aggregate.
        /// </summary>
        /// <value>The type of the aggregate.</value>
        public AggregateFunction AggregateType
        {
            get { return _aggregateType; }
            set { _aggregateType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        /// <summary>
        /// Gets or sets the alias.
        /// </summary>
        /// <value>The alias.</value>
        public string Alias
        {
            get { return _alias; }
            set { _alias = value; }
        }

        /// <summary>
        /// Gets the SQL function call without an alias.  Example: AVG(UnitPrice).
        /// </summary>
        /// <returns></returns>
        public string WithoutAlias()
        {
            string result;

            if(AggregateType == AggregateFunction.GroupBy)
                result = ColumnName;
            else
            {
                string functionName = GetFunctionType(this);
                functionName = functionName.ToUpperInvariant();
                const string aggFormat = " {0}({1})";
                result = string.Format(aggFormat, functionName, ColumnName);
            }

            return result;
        }

        /// <summary>
        /// Overrides ToString() to return the SQL Function call
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string result;

            if(AggregateType == AggregateFunction.GroupBy)
                result = ColumnName;
            else
            {
                string functionName = GetFunctionType(this);
                functionName = functionName.ToUpperInvariant();
                const string aggFormat = " {0}({1}) as '{2}'";
                result = string.Format(aggFormat, functionName, ColumnName, Alias);
            }

            return result;
        }
    }
}