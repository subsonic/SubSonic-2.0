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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;
using SubSonic.Sugar;
using SubSonic.Utilities;

namespace SubSonic
{


    #region enums

    /// <summary>
    /// Enum for General SQL Functions
    /// </summary>
    [Serializable]
    public enum AggregateFunction
    {
        Count,
        Sum,
        Avg,
        Min,
        Max,
        StDev,
        Var,
        GroupBy
    }

    /// <summary>
    /// SQL Comparison Operators
    /// </summary>
    [Serializable]
    public enum Comparison
    {
        Equals,
        NotEquals,
        Like,
        NotLike,
        GreaterThan,
        GreaterOrEquals,
        LessThan,
        LessOrEquals,
        Blank,
        Is,
        IsNot,
        In,
        NotIn,
        OpenParentheses,
        CloseParentheses,
        BetweenAnd
    }

    #endregion


    #region Support Classes

    /// <summary>
    /// Summary for the BetweenAnd class
    /// </summary>
    [Serializable]
    public class BetweenAnd
    {
        private string columnName;
        private Where.WhereCondition condition = Where.WhereCondition.AND;
        private DateTime endDate;
        private string endParameterName;
        private DateTime startDate;
        private string startParameterName;
        private string tableName;

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        /// <value>The start date.</value>
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        /// <value>The end date.</value>
        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public Where.WhereCondition Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        /// <summary>
        /// Gets or sets the start name of the parameter.
        /// </summary>
        /// <value>The start name of the parameter.</value>
        public string StartParameterName
        {
            get { return startParameterName; }
            set { startParameterName = value; }
        }

        /// <summary>
        /// Gets or sets the end name of the parameter.
        /// </summary>
        /// <value>The end name of the parameter.</value>
        public string EndParameterName
        {
            get { return endParameterName; }
            set { endParameterName = value; }
        }
    }

    /// <summary>
    /// Creates a WHERE clause for a SQL Statement
    /// </summary>
    [Serializable]
    public class Where
    {
        #region WhereCondition enum

        public enum WhereCondition
        {
            AND,
            OR
        }

        #endregion


        private string columnName;
        private Comparison comp;

        private WhereCondition condition = WhereCondition.AND;
        private DbType dbType;
        private string parameterName;
        private object paramValue;

        private string tableName;

        /// <summary>
        /// Gets or sets the condition.
        /// </summary>
        /// <value>The condition.</value>
        public WhereCondition Condition
        {
            get { return condition; }
            set { condition = value; }
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        /// <summary>
        /// Gets or sets the comparison.
        /// </summary>
        /// <value>The comparison.</value>
        public Comparison Comparison
        {
            get { return comp; }
            set { comp = value; }
        }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>The parameter value.</value>
        public object ParameterValue
        {
            get { return paramValue; }
            set { paramValue = value; }
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string ParameterName
        {
            get { return parameterName ?? ColumnName; }
            set { parameterName = value; }
        }

        /// <summary>
        /// Gets or sets the type of the db.
        /// </summary>
        /// <value>The type of the db.</value>
        public DbType DbType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        /// <summary>
        /// Parses the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public static Where ParseExpression(string expression, WhereCondition condition)
        {
            Comparison comp = Comparison.Blank;
            Where result = null;

            if(Utility.IsRegexMatch(expression, RegexPattern.SQL_NOT_EQUAL))
                comp = Comparison.NotEquals;
            else if(Utility.IsRegexMatch(expression, RegexPattern.SQL_GREATER_OR_EQUAL))
                comp = Comparison.GreaterOrEquals;
            else if(Utility.IsRegexMatch(expression, RegexPattern.SQL_LESS_OR_EQUAL))
                comp = Comparison.LessOrEquals;
            else if(Utility.IsRegexMatch(expression, RegexPattern.SQL_LESS))
                comp = Comparison.LessThan;
            else if(Utility.IsRegexMatch(expression, RegexPattern.SQL_GREATER))
                comp = Comparison.GreaterThan;
            else if(Utility.IsRegexMatch(expression, RegexPattern.SQL_EQUAL))
                comp = Comparison.Equals;
            else if(Utility.IsRegexMatch(expression, RegexPattern.SQL_NOT_LIKE))
                comp = Comparison.NotLike;
            else if(Utility.IsRegexMatch(expression, RegexPattern.SQL_LIKE))
                comp = Comparison.Like;
            else if(Utility.IsRegexMatch(expression, RegexPattern.SQL_IS_NOT))
                comp = Comparison.IsNot;
            else if(Utility.IsRegexMatch(expression, RegexPattern.SQL_IS))
                comp = Comparison.Is;

            if(comp != Comparison.Blank)
            {
                string comparisonOperator = GetComparisonOperator(comp).Trim();
				int comparisonStart = expression.IndexOf(comparisonOperator, StringComparison.InvariantCultureIgnoreCase);
				int comparisonEnd = comparisonStart + comparisonOperator.Length;

				string columnName = expression.Substring(0, comparisonStart).Trim();
				string paramValue = expression.Substring(comparisonEnd).Trim();
		
				result = new Where
                             {
                                 ColumnName = columnName,
                                 Comparison = comp,
                                 condition = condition,
                                 ParameterValue = paramValue
                             };
            }
            return result;
        }

        /// <summary>
        /// Gets the comparison operator.
        /// </summary>
        /// <param name="comp">The comp.</param>
        /// <returns></returns>
        public static string GetComparisonOperator(Comparison comp)
        {
            string sOut;
            switch(comp)
            {
                case Comparison.Blank:
                    sOut = SqlComparison.BLANK;
                    break;
                case Comparison.GreaterThan:
                    sOut = SqlComparison.GREATER;
                    break;
                case Comparison.GreaterOrEquals:
                    sOut = SqlComparison.GREATER_OR_EQUAL;
                    break;
                case Comparison.LessThan:
                    sOut = SqlComparison.LESS;
                    break;
                case Comparison.LessOrEquals:
                    sOut = SqlComparison.LESS_OR_EQUAL;
                    break;
                case Comparison.Like:
                    sOut = SqlComparison.LIKE;
                    break;
                case Comparison.NotEquals:
                    sOut = SqlComparison.NOT_EQUAL;
                    break;
                case Comparison.NotLike:
                    sOut = SqlComparison.NOT_LIKE;
                    break;
                case Comparison.Is:
                    sOut = SqlComparison.IS;
                    break;
                case Comparison.IsNot:
                    sOut = SqlComparison.IS_NOT;
                    break;
                case Comparison.In:
                    sOut = SqlComparison.IN;
                    break;
                case Comparison.NotIn:
                    sOut = SqlComparison.NOT_IN;
                    break;
                default:
                    sOut = SqlComparison.EQUAL;
                    break;
            }
            return sOut;
        }
    }

    /// <summary>
    /// Creates an ORDER BY statement for ANSI SQL
    /// </summary>
    [Serializable]
    public class OrderBy {
        private string orderColName;
        private bool isAscending = true;

        private OrderBy() { }

        /// <summary>
        /// Gets or sets the order column name only.
        /// </summary>
        /// <value>The order string.</value>
        public string OrderColumnName {
            get { return orderColName; }
            set { this.orderColName = value; }
        }

        /// <summary>
        /// Gets or sets the order column sort direction.
        /// </summary>
        /// <value>The order string.</value>
        public bool OrderColumnIsSortAsc {
            get { return isAscending; }
            set { isAscending = value; }
        }

        /// <summary>
        /// Gets or sets the order string including the ORDER BY fragment.
        /// </summary>
        /// <value>The order string.</value>
        public string OrderString {
            get { return orderColName + (isAscending ? SqlFragment.ASC : SqlFragment.DESC); }
            set {
                isAscending = true;
                orderColName = value;

                if (value.TrimEnd().ToLower().EndsWith(SqlFragment.DESC.ToLower())) {
                    isAscending = false;
                    orderColName = value.TrimEnd().Substring(0, value.Length - SqlFragment.DESC.Length).Trim();
                }
                if (value.TrimEnd().ToLower().EndsWith(SqlFragment.ASC.ToLower())) {
                    orderColName = value.TrimEnd().Substring(0, value.Length - SqlFragment.ASC.Length).Trim();
                }
            }
        }

        /// <summary>
        /// Gets reversed order string including the ORDER BY fragment.
        /// </summary>
        /// <value>The order string.</value>
        public string OrderStringReversed {
            get { return orderColName + (!isAscending ? SqlFragment.ASC : SqlFragment.DESC); }
        }

        /// <summary>
        /// Specifies that query will ordered by the passed column in descending order. Allows table alias to explicity set.
        /// This is the preferred method for specifying order. It is provider neutral and will ensure full qualification of column names.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <returns></returns>
        public static OrderBy Desc(TableSchema.TableColumn col, string tableAlias) {
            OrderBy orderBy = new OrderBy();
            orderBy.orderColName = col.Table.Provider.QualifyColumnName("", tableAlias, col.ColumnName);
            orderBy.isAscending = false;
            return orderBy;
        }

        /// <summary>
        /// Specifies that query will ordered by the passed column in descending order.
        /// This is the preferred method for specifying order. It is provider neutral and will ensure full qualification of column names.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public static OrderBy Desc(TableSchema.TableColumn col) {
            OrderBy orderBy = new OrderBy();
            orderBy.orderColName = col.QualifiedName;
            orderBy.isAscending = false;
            return orderBy;
        }

        /// <summary>
        /// Specifies that query will ordered by the passed column in descending order.
        /// This method is NOT provider neutral! Pass a TableColumn instead to ensure provider-neutral unambiguous column definition.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static OrderBy Desc(string columnName) {
            OrderBy orderBy = new OrderBy();
            orderBy.orderColName = SquareBracket(columnName);
            orderBy.isAscending = false;
            return orderBy;
        }

        /// <summary>
        /// Specifies that query will ordered by the passed column in ascending order. Allows table alias to explicity set.
        /// This is the preferred method for specifying order. It is provider neutral and will ensure full qualification of column names.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <returns></returns>
        public static OrderBy Asc(TableSchema.TableColumn col, string tableAlias) {
            OrderBy orderBy = new OrderBy();
            orderBy.orderColName = col.Table.Provider.QualifyColumnName("", tableAlias, col.ColumnName);
            orderBy.isAscending = true;
            return orderBy;
        }

        /// <summary>
        /// Specifies that query will ordered by the passed column in ascending order.
        /// This is the preferred method for specifying order. It is provider neutral and will ensure full qualification of column names.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public static OrderBy Asc(TableSchema.TableColumn col) {
            OrderBy orderBy = new OrderBy();
            orderBy.orderColName = col.QualifiedName;
            orderBy.isAscending = true;
            return orderBy;
        }

        /// <summary>
        /// Specifies that query will ordered by the passed column in ascending order.
        /// This method is NOT provider neutral! Pass a TableColumn instead to ensure provider-neutral unambiguous column definition.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public static OrderBy Asc(string columnName) {
            OrderBy orderBy = new OrderBy();
            orderBy.orderColName = SquareBracket(columnName);
            orderBy.isAscending = true;
            return orderBy;
        }

        /// <summary>
        /// Passeds the value.
        /// </summary>
        /// <param name="orderByValue">The order by value.</param>
        /// <returns></returns>
        public static OrderBy PassedValue(string orderByValue) {
            OrderBy orderBy = new OrderBy();
            orderBy.OrderString = orderByValue;
            return orderBy;
        }

        private static string SquareBracket(string columnName) {
            if (!String.IsNullOrEmpty(columnName) && !columnName.StartsWith("[")
            && !columnName.EndsWith("]") && !columnName.Contains(".")) {
                return "[" + columnName + "]";
            }
            return String.Empty;
        }
    }

    /// <summary>
    /// Summary for the OrderByCollection class
    /// </summary>
    [Serializable]
    public class OrderByCollection : List<OrderBy> {}

    #endregion


    /// <summary>
    /// Creates a SQL Statement and SQL Commands
    /// </summary>
    public class Query
    {
        private Hashtable updateSettings;

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        /// <value>The schema.</value>
        public TableSchema.Table Schema
        {
            get { return table; }
            set { table = value; }
        }

        /// <summary>
        /// Gets the update settings.
        /// </summary>
        /// <value>The update settings.</value>
        internal Hashtable UpdateSettings
        {
            get { return updateSettings; }
        }

        /// <summary>
        /// Parses the and run SQL file.
        /// </summary>
        /// <param name="sqlFilePath">The SQL file path.</param>
        /// <param name="providerName">Name of the provider.</param>
        public static void ParseAndRunSqlFile(string sqlFilePath, string providerName)
        {
            string sqlText = Files.GetFileText(sqlFilePath);

            ParseAndRunSqlCommand(sqlText, providerName);
        }

        /// <summary>
        /// Parses the and run SQL command.
        /// </summary>
        /// <param name="sqlText">The SQL text.</param>
        /// <param name="providerName">Name of the provider.</param>
        public static void ParseAndRunSqlCommand(string sqlText, string providerName)
        {
            Regex regex = new Regex("^GO", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            string[] SqlLines = regex.Split(sqlText);
            foreach(string sql in SqlLines)
            {
                QueryCommand cmd = new QueryCommand(sql, providerName);
                DataService.ExecuteQuery(cmd);
            }
        }

        /// <summary>
        /// Builds the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public static TableSchema.Table BuildTableSchema(string tableName)
        {
            return BuildTableSchema(tableName, String.Empty);
        }

        /// <summary>
        /// Builds the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static TableSchema.Table BuildTableSchema(string tableName, string providerName)
        {
            return DataService.GetTableSchema(tableName, providerName);
        }

        /// <summary>
        /// Takes the enum value and returns the proper SQL
        /// </summary>
        /// <param name="comp">The Comparison enum whose SQL equivalent will be returned</param>
        /// <returns></returns>
        public static string GetComparisonOperator(Comparison comp)
        {
            return Where.GetComparisonOperator(comp);
        }

        /// <summary>
        /// Adds the update setting.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Query AddUpdateSetting(string columnName, object value)
        {
            //columnName = Provider.FormatIdentifier(columnName);
            //boolean massage for MySQL
            if(Utility.IsMatch(value.ToString(), Boolean.FalseString))
                value = 0;
            else if(Utility.IsMatch(value.ToString(), Boolean.TrueString))
                value = 1;

            if(updateSettings == null)
                updateSettings = new Hashtable();

            if(updateSettings.Contains(columnName))
                updateSettings.Remove(columnName);

            updateSettings.Add(columnName, value);

            //set the query type since this is probably an update query
            QueryType = QueryType.Update;

            return this;
        }

        /// <summary>
        /// Gets the type of the db.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        private DbType GetDbType(string columnName)
        {
            TableSchema.TableColumn column = table.GetColumn(columnName);
            if(column == null)
                throw new ArgumentException("There is no column named '" + columnName + "' in table " + table.Name, "columnName");

            return column.DataType;
        }


        #region Conditionals (WHERE, AND, IN, OR, BETWEEN)

        /// <summary>
        /// DISTINCTs this instance.
        /// </summary>
        /// <returns></returns>
        public Query DISTINCT()
        {
            isDistinct = true;
            return this;
        }

        /// <summary>
        /// Creates an IN statement based on the passed-in object array.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="listItems">The list items.</param>
        /// <returns></returns>
        public Query IN(string columnName, object[] listItems)
        {
            inColumn = columnName;

            if(listItems == null || listItems.Length == 0)
                listItems = new object[] {"NULL"};

            inList = listItems;
            return this;
        }

        /// <summary>
        /// Creates an IN list based on a passed in ArrayList
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="listItems">The list items.</param>
        /// <returns></returns>
        public Query IN(string columnName, ArrayList listItems)
        {
            inColumn = columnName;

            if(listItems == null)
                listItems = new ArrayList();
            if(listItems.Count == 0)
                listItems.Add("NULL");

            inList = new object[listItems.Count];

            for(int i = 0; i < listItems.Count; i++)
                inList[i] = listItems[i];

            return this;
        }

        /// <summary>
        /// Creates an IN statement from a passed-in ListItemCollection. Only the selected list items will be included.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="listItems">The list items.</param>
        /// <returns></returns>
        public Query IN(string columnName, ListItemCollection listItems)
        {
            inColumn = columnName;
            inList = new object[listItems.Count];
            int inCounter = 0;
            foreach(ListItem item in listItems)
            {
                if(item.Selected)
                {
                    inList[inCounter] = item.Value;
                    inCounter++;
                }
            }

            return this;
        }

        /// <summary>
        /// Creates an NOT IN statement based on the passed-in object array.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="listItems">The list items.</param>
        /// <returns></returns>
        public Query NOT_IN(string columnName, object[] listItems)
        {
            notInColumn = columnName;

            if(listItems == null || listItems.Length == 0)
                listItems = new object[] {"NULL"};

            notInList = listItems;
            return this;
        }

        /// <summary>
        /// Creates an NOT IN list based on a passed in ArrayList
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="listItems">The list items.</param>
        /// <returns></returns>
        public Query NOT_IN(string columnName, ArrayList listItems)
        {
            notInColumn = columnName;

            if(listItems == null)
                listItems = new ArrayList();
            if(listItems.Count == 0)
                listItems.Add("NULL");

            notInList = new object[listItems.Count];

            for(int i = 0; i < listItems.Count; i++)
                notInList[i] = listItems[i];

            return this;
        }

        /// <summary>
        /// Creates an NOT IN statement from a passed-in ListItemCollection. Only the selected list items will be included.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="listItems">The list items.</param>
        /// <returns></returns>
        public Query NOT_IN(string columnName, ListItemCollection listItems)
        {
            notInColumn = columnName;
            notInList = new object[listItems.Count];
            int notInCounter = 0;
            foreach(ListItem item in listItems)
            {
                if(item.Selected)
                {
                    notInList[notInCounter] = item.Value;
                    notInCounter++;
                }
            }

            return this;
        }


        #region ANDs

        /// <summary>
        /// ANDs the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public Query AND(string expression)
        {
            Where w = Where.ParseExpression(expression, Where.WhereCondition.AND);

            if(w != null)
            {
                w.TableName = table.Name;
                w.DbType = GetDbType(w.ColumnName);
                //add this in
                AddWhere(w);
            }

            return this;
        }

        /// <summary>
        /// ORs the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public Query OR(string expression)
        {
            Where w = Where.ParseExpression(expression, Where.WhereCondition.OR);

            if(w != null)
            {
                w.TableName = table.Name;
                w.DbType = GetDbType(w.ColumnName);
                //add this in
                AddWhere(w);
            }

            return this;
        }

        /// <summary>
        /// ANDs the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query AND(string columnName, object paramValue)
        {
            AddWhere(table.Name, columnName, Comparison.Equals, paramValue);
            return this;
        }

        /// <summary>
        /// ANDs the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query AND(string columnName, Comparison comp, object paramValue)
        {
            AddWhere(table.Name, columnName, comp, paramValue);
            return this;
        }

        /// <summary>
        /// ANDs the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query AND(string tableName, string columnName, object paramValue)
        {
            AddWhere(tableName, columnName, Comparison.Equals, paramValue);
            return this;
        }

        /// <summary>
        /// ANDs the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query AND(string tableName, string columnName, Comparison comp, object paramValue)
        {
            return AddWhere(tableName, columnName, columnName, comp, paramValue);
        }

        #endregion


        #region ORs

        //Thanks to DataCop for help with this!

        /// <summary>
        /// ORs the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query OR(string columnName, object paramValue)
        {
            AddWhere(table.Name, columnName, columnName, Comparison.Equals, paramValue, Where.WhereCondition.OR);
            return this;
        }

        /// <summary>
        /// ORs the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query OR(string columnName, Comparison comp, object paramValue)
        {
            AddWhere(table.Name, columnName, columnName, comp, paramValue, Where.WhereCondition.OR);
            return this;
        }

        /// <summary>
        /// ORs the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query OR(string tableName, string columnName, object paramValue)
        {
            AddWhere(tableName, columnName, columnName, Comparison.Equals, paramValue, Where.WhereCondition.OR);
            return this;
        }

        /// <summary>
        /// ORs the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query OR(string tableName, string columnName, Comparison comp, object paramValue)
        {
            AddWhere(tableName, columnName, columnName, comp, paramValue, Where.WhereCondition.OR);
            return this;
        }

        #endregion


        #region WHERE

        /// <summary>
        /// WHEREs the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns></returns>
        public Query WHERE(string expression)
        {
            Where w = Where.ParseExpression(expression, Where.WhereCondition.AND);

            if(w != null)
            {
                w.TableName = table.Name;
                w.DbType = GetDbType(w.ColumnName);
                AddWhere(w);
            }
            return this;
        }

        /// <summary>
        /// WHEREs the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query WHERE(string columnName, object paramValue)
        {
            AddWhere(table.Name, columnName, Comparison.Equals, paramValue);
            return this;
        }

        /// <summary>
        /// WHEREs the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query WHERE(string tableName, string columnName, object paramValue)
        {
            AddWhere(tableName, columnName, Comparison.Equals, paramValue);
            return this;
        }

        /// <summary>
        /// WHEREs the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query WHERE(string columnName, Comparison comp, object paramValue)
        {
            AddWhere(table.Name, columnName, comp, paramValue);
            return this;
        }

        /// <summary>
        /// WHEREs the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="constraint">The constraint.</param>
        /// <returns></returns>
        public Query WHERE(string columnName, IConstraint constraint)
        {
            AddWhere(table.Name, columnName, constraint.Comparison, constraint.Value);
            return this;
        }

        /// <summary>
        /// WHEREs the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query WHERE(string tableName, string columnName, Comparison comp, object paramValue)
        {
            return AddWhere(tableName, columnName, columnName, comp, paramValue);
        }

        /// <summary>
        /// WHEREs the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query WHERE(string tableName, string parameterName, string columnName, Comparison comp, object paramValue)
        {
            AddWhere(tableName, parameterName, columnName, comp, paramValue, Where.WhereCondition.AND);
            return this;
        }

        /// <summary>
        /// WHEREs the specified table name.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public Query WHERE(string tableName, string parameterName, string columnName, Comparison comp, object paramValue, Where.WhereCondition condition)
        {
            AddWhere(tableName, parameterName, columnName, comp, paramValue, condition);
            return this;
        }

        /// <summary>
        /// ORDEs the r_ BY.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public Query ORDER_BY(TableSchema.TableColumn tableColumn)
        {
            return ORDER_BY(tableColumn, SqlFragment.ASC);
        }

        /// <summary>
        /// ORDEs the r_ BY.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns></returns>
        public Query ORDER_BY(TableSchema.TableColumn tableColumn, string sortDirection)
        {
            if(String.IsNullOrEmpty(sortDirection) || Utility.IsMatch(sortDirection, SqlFragment.ASC, true))
                AddQueryToCollection(OrderBy.Asc(tableColumn));
            else if(Utility.IsMatch(sortDirection, SqlFragment.DESC, true))
                AddQueryToCollection(OrderBy.Desc(tableColumn));
            return this;
        }

        /// <summary>
        /// ORDEs the r_ BY.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <returns></returns>
        public Query ORDER_BY(TableSchema.TableColumn tableColumn, string sortDirection, string tableAlias)
        {
            if(!String.IsNullOrEmpty(tableAlias))
            {
                if(String.IsNullOrEmpty(sortDirection) || Utility.IsMatch(sortDirection, SqlFragment.ASC, true))
                    AddQueryToCollection(OrderBy.Asc(tableColumn, tableAlias));
                else if(Utility.IsMatch(sortDirection, SqlFragment.DESC, true))
                    AddQueryToCollection(OrderBy.Desc(tableColumn, tableAlias));
            }
            else
                return ORDER_BY(tableColumn, sortDirection);
            return this;
        }

        /// <summary>
        /// ORDEs the r_ BY.
        /// </summary>
        /// <param name="orderExpression">The order expression.</param>
        /// <returns></returns>
        public Query ORDER_BY(string orderExpression)
        {
            AddQueryToCollection(OrderBy.PassedValue(orderExpression));
            return this;
        }

        /// <summary>
        /// ORDEs the r_ BY.
        /// </summary>
        /// <param name="orderExpression">The order expression.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns></returns>
        public Query ORDER_BY(string orderExpression, string sortDirection)
        {
            sortDirection = sortDirection.Trim();
            if(!String.IsNullOrEmpty(sortDirection))
            {
                if(Utility.IsMatch(sortDirection, SqlFragment.ASC, true))
                    AddQueryToCollection(OrderBy.Asc(orderExpression));
                else if(Utility.IsMatch(sortDirection, SqlFragment.DESC, true))
                    AddQueryToCollection(OrderBy.Desc(orderExpression));
            }
            else
                ORDER_BY(orderExpression);
            return this;
        }

        /// <summary>
        /// Adds the query to collection.
        /// </summary>
        /// <param name="orderBy">The order by.</param>
        private void AddQueryToCollection(OrderBy orderBy)
        {
            OrderByCollection.Add(orderBy);
        }

        #endregion


        #region Between/And

        /// <summary>
        /// Os the r_ BETWEE n_ AND.
        /// </summary>
        /// <param name="columName">Name of the colum.</param>
        /// <param name="dateTimeStart">The date time start.</param>
        /// <param name="dateTimeEnd">The date time end.</param>
        /// <returns></returns>
        public Query OR_BETWEEN_AND(string columName, string dateTimeStart, string dateTimeEnd)
        {
            OR_BETWEEN_AND(columName, DateTime.Parse(dateTimeStart), DateTime.Parse(dateTimeEnd));
            return this;
        }

        /// <summary>
        /// BETWEEs the n_ AND.
        /// </summary>
        /// <param name="columName">Name of the colum.</param>
        /// <param name="dateTimeStart">The date time start.</param>
        /// <param name="dateTimeEnd">The date time end.</param>
        /// <returns></returns>
        public Query BETWEEN_AND(string columName, string dateTimeStart, string dateTimeEnd)
        {
            BETWEEN_AND(columName, DateTime.Parse(dateTimeStart), DateTime.Parse(dateTimeEnd));
            return this;
        }

        /// <summary>
        /// Os the r_ BETWEE n_ AND.
        /// </summary>
        /// <param name="columName">Name of the colum.</param>
        /// <param name="dateStart">The date start.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        public Query OR_BETWEEN_AND(string columName, DateTime dateStart, DateTime dateEnd)
        {
            AddBetweenAnd(table.Name, columName, dateStart, dateEnd, Where.WhereCondition.OR);
            return this;
        }

        /// <summary>
        /// BETWEEs the n_ AND.
        /// </summary>
        /// <param name="columName">Name of the colum.</param>
        /// <param name="dateStart">The date start.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        public Query BETWEEN_AND(string columName, DateTime dateStart, DateTime dateEnd)
        {
            AddBetweenAnd(table.Name, columName, dateStart, dateEnd);
            return this;
        }

        /// <summary>
        /// BETWEEs the n_ AND.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columName">Name of the colum.</param>
        /// <param name="dateStart">The date start.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public Query BETWEEN_AND(string tableName, string columName, DateTime dateStart, DateTime dateEnd, Where.WhereCondition condition)
        {
            AddBetweenAnd(tableName, columName, dateStart, dateEnd, condition);
            return this;
        }

        /// <summary>
        /// BETWEEs the n_ VALUES.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns></returns>
        public Query BETWEEN_VALUES(string columnName, object value1, object value2)
        {
            AddBetweenValues(columnName, value1, value2);
            return this;
        }

        /// <summary>
        /// Adds the between values.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value1">The value1.</param>
        /// <param name="value2">The value2.</param>
        /// <returns></returns>
        public Query AddBetweenValues(string columnName, object value1, object value2)
        {
            AddWhere(columnName, Comparison.GreaterOrEquals, value1);
            AddWhere(table.Name, columnName + "2", columnName, Comparison.LessOrEquals, value2);
            return this;
        }

        #endregion


        #region DEPRECATED Add Methods

        /// <summary>
        /// Adds the where.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public Query AddWhere(Where where)
        {
            //fix up the parameter naming
            where.ParameterName = Provider.PreformatParameterName(where.ColumnName.Trim() + wheres.Count);
            where.DbType = GetDbType(where.ColumnName.Trim());
            wheres.Add(where);
            if(String.IsNullOrEmpty(where.TableName))
            {
                where.TableName = table.Name;
                TableSchema.TableColumn tableColumn = table.GetColumn(where.ColumnName);
                if(tableColumn != null)
                    where.DbType = tableColumn.DataType;
            }

            return this;
        }

        /// <summary>
        /// Adds the where.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query AddWhere(string columnName, object paramValue)
        {
            return AddWhere(table.Name, columnName, Comparison.Equals, paramValue);
        }

        /// <summary>
        /// Adds the where.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query AddWhere(string tableName, string columnName, object paramValue)
        {
            return AddWhere(tableName, columnName, Comparison.Equals, paramValue);
        }

        /// <summary>
        /// Adds the where.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query AddWhere(string columnName, Comparison comp, object paramValue)
        {
            return AddWhere(table.Name, columnName, comp, paramValue);
        }

        /// <summary>
        /// Adds the where.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query AddWhere(string tableName, string columnName, Comparison comp, object paramValue)
        {
            return AddWhere(tableName, columnName, columnName, comp, paramValue, Where.WhereCondition.AND);
        }

        /// <summary>
        /// Adds the where.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <returns></returns>
        public Query AddWhere(string tableName, string parameterName, string columnName, Comparison comp, object paramValue)
        {
            return AddWhere(tableName, parameterName, columnName, comp, paramValue, Where.WhereCondition.AND);
        }

        /// <summary>
        /// Adds the where.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="paramValue">The param value.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public Query AddWhere(string tableName, string parameterName, string columnName, Comparison comp, object paramValue, Where.WhereCondition condition)
        {
            //TODO: shouldn't parameterName actually be used?!  Need to write a test to make sure this won't break anything
            Where w = new Where
                          {
                              ColumnName = columnName,
                              ParameterValue = paramValue,
                              Comparison = comp,
                              TableName = tableName,
                              Condition = condition
                          };

            return AddWhere(w);
        }

        /// <summary>
        /// Adds the between and.
        /// </summary>
        /// <param name="columName">Name of the colum.</param>
        /// <param name="dateStart">The date start.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        public Query AddBetweenAnd(string columName, DateTime dateStart, DateTime dateEnd)
        {
            return AddBetweenAnd(table.Name, columName, dateStart, dateEnd);
        }

        /// <summary>
        /// Adds the between and.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columName">Name of the colum.</param>
        /// <param name="dateStart">The date start.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public Query AddBetweenAnd(string tableName, string columName, DateTime dateStart, DateTime dateEnd, Where.WhereCondition condition)
        {
            BetweenAnd between = new BetweenAnd
                                     {
                                         ColumnName = columName,
                                         TableName = tableName,
                                         StartDate = dateStart,
                                         EndDate = dateEnd,
                                         Condition = condition
                                     };
            between.StartParameterName = "start" + between.ColumnName + betweens.Count;
            between.EndParameterName = "end" + between.ColumnName + betweens.Count;
            return AddBetweenAnd(between);
        }

        /// <summary>
        /// Adds the between and.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columName">Name of the colum.</param>
        /// <param name="dateStart">The date start.</param>
        /// <param name="dateEnd">The date end.</param>
        /// <returns></returns>
        public Query AddBetweenAnd(string tableName, string columName, DateTime dateStart, DateTime dateEnd)
        {
            return AddBetweenAnd(tableName, columName, dateStart, dateEnd, Where.WhereCondition.AND);
        }

        /// <summary>
        /// Adds the between and.
        /// </summary>
        /// <param name="between">The between.</param>
        /// <returns></returns>
        public Query AddBetweenAnd(BetweenAnd between)
        {
            if(String.IsNullOrEmpty(between.TableName))
                between.TableName = table.Name;

            betweens.Add(between);
            return this;
        }

        #endregion


        #endregion


        #region Command Builders

        /// <summary>
        /// Creates a SELECT command based on the Query object's settings.
        /// If you need a more complex query you should consider using a Stored Procedure
        /// </summary>
        /// <returns>System.Data.Common.SqlCommand</returns>
        public QueryCommand BuildSelectCommand()
        {
            //get the SQL
            queryType = QueryType.Select;
            return DataService.BuildCommand(this);
        }

        /// <summary>
        /// Builds a Delete command based on a give WHERE condition
        /// </summary>
        /// <returns></returns>
        public QueryCommand BuildDeleteCommand()
        {
            queryType = QueryType.Delete;
            return DataService.BuildCommand(this);
        }

        /// <summary>
        /// Builds an update query for this table with the passed-in hash values
        /// </summary>
        /// <returns></returns>
        public QueryCommand BuildUpdateCommand()
        {
            queryType = QueryType.Update;
            return DataService.BuildCommand(this);
        }

        /// <summary>
        /// Builds an query for this table based on the QueryType
        /// </summary>
        /// <returns></returns>
        public QueryCommand BuildCommand()
        {
            return DataService.BuildCommand(this);
        }

        #endregion


        #region SQL Builders

        /// <summary>
        /// Returns the SQL generated for this command
        /// </summary>
        /// <returns></returns>
        public string GetSql()
        {
            return DataService.GetSql(this);
        }

        #endregion


        #region Execution

        /// <summary>
        /// Inspects this instance.
        /// </summary>
        /// <returns></returns>
        public string Inspect()
        {
            bool isWeb = HttpContext.Current != null;
            DateTime execStart = DateTime.Now;
            StringBuilder result = new StringBuilder();
            //get a dataset
            DataSet ds = DataService.GetDataSet(GetCommand());
            DateTime execEnd = DateTime.Now;
            TimeSpan ts = new TimeSpan(execEnd.Ticks - execStart.Ticks);

            if(isWeb)
            {
                result.Append("<h2>Query Inspection: " + Schema.Name + "</h2>");
                result.Append("<b>Execution Time:</b> " + ts.Milliseconds + " milliseconds <br/><br/>");
                result.Append("<b>Query: </b><xmp>" + GetSql() + "</xmp><br/><br/>");
                result.Append(DataProvider.BuildWhere(this));
                result.Append("<br/>");
            }
            else
            {
                result.Append("Query Inspection: " + Schema.Name + Environment.NewLine);
                result.Append("Execution Time: " + ts.Milliseconds + " milliseconds" + Environment.NewLine + Environment.NewLine);
                result.Append("Query: " + GetSql() + Environment.NewLine + Environment.NewLine);
                result.Append(DataProvider.BuildWhere(this));
                result.Append(Environment.NewLine);
            }
            if(ds != null)
            {
                if(ds.Tables.Count > 0)
                {
                    DataTable tbl = ds.Tables[0];

                    if(isWeb)
                    {
                        result.Append("<b>Total Records:</b> ");
                        result.Append(ds.Tables[0].Rows.Count.ToString());
                        result.Append("<br/><br/>");
                        result.Append(Utility.DataTableToHtmlTable(tbl, "80%"));
                    }
                    else
                    {
                        result.Append("Total Records: ");
                        result.Append(ds.Tables[0].Rows.Count.ToString());
                    }
                }
            }
            else
                result.Append("No Data");

            return result.ToString();
        }

        /// <summary>
        /// Returns the number of records matching the current query.
        /// </summary>
        /// <returns></returns>
        public int GetRecordCount()
        {
            return DataService.GetRecordCount(this);
        }

        /// <summary>
        /// Returns an IDataReader using the passed-in command
        /// </summary>
        /// <returns>IDataReader</returns>
        public IDataReader ExecuteReader()
        {
            return DataService.GetReader(GetCommand());
        }

        /// <summary>
        /// Returns a DataSet based on the passed-in command
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteDataSet()
        {
            return DataService.GetDataSet(GetCommand());
        }

        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteDataSet<T>() where T : DataSet, new()
        {
            return DataService.GetDataSet<T>(GetCommand());
        }

        /// <summary>
        /// Returns a DataSet based on the passed-in command
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteJoinedDataSet()
        {
            return ExecuteJoinedDataSet<DataSet>();
        }

        /// <summary>
        /// Executes the joined data set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteJoinedDataSet<T>() where T : DataSet, new()
        {
            StringBuilder strSelect = new StringBuilder(SqlFragment.SELECT);
            //string strFrom = SqlFragment.FROM + table.QualifiedName;
            int joinCount = 0;
            StringBuilder strJoin = new StringBuilder();
            for(int i = 0; i < table.Columns.Count; i++)
            {
                string joinType = SqlFragment.INNER_JOIN;
                StringBuilder col;
                TableSchema.TableColumn tblCol = table.Columns[i];
                if(tblCol.IsNullable)
                    joinType = SqlFragment.LEFT_JOIN;

                if(i == 0 && tblCol.IsForeignKey && !String.IsNullOrEmpty(tblCol.ForeignKeyTableName) && Utility.IsMappingTable(table))
                {
                    col = new StringBuilder(table.Provider.QualifyColumnName(table.SchemaName, table.Name, tblCol.ColumnName));
                    col.Append(SqlFragment.AS);
                    col.Append(String.Concat("PK", tblCol.ColumnName));
                    if(i + 1 != table.Columns.Count)
                        col.Append(", ");

                    strSelect.Append(col);
                }

                if(tblCol.IsForeignKey && !tblCol.IsPrimaryKey && !String.IsNullOrEmpty(tblCol.ForeignKeyTableName))
                {
                    joinCount++; 
                    string fkTableAlias = String.Concat(SqlFragment.JOIN_PREFIX, i);
                    TableSchema.Table fkTable = DataService.GetSchema(tblCol.ForeignKeyTableName, ProviderName, TableType.Table);
                    TableSchema.TableColumn displayCol = Utility.GetDisplayTableColumn(fkTable);

                    bool isSortable = Utility.GetEffectiveMaxLength(displayCol) < 256;
                    string dataCol = displayCol.ColumnName;
                    string selectCol = table.Provider.QualifyColumnName("", fkTableAlias, dataCol);
                    col = new StringBuilder(selectCol);
                    strJoin.Append(joinType);
                    strJoin.Append(fkTable.Provider.QualifyTableName(fkTable.SchemaName, fkTable.TableName));
                    strJoin.Append(SqlFragment.AS);
                    strJoin.Append(fkTableAlias);
                    strJoin.Append(SqlFragment.ON);
                    string columnReference = tblCol.QualifiedName;
                    strJoin.Append(columnReference);
                    strJoin.Append(SqlFragment.EQUAL_TO);
                    string joinReference = table.Provider.QualifyColumnName("", fkTableAlias, fkTable.PrimaryKey.ColumnName);
                    strJoin.Append(joinReference);
                    if (table.Provider.DatabaseRequiresBracketedJoins) strJoin.Append(")");
                    if(isSortable && OrderByCollection.Count > 0) {
						foreach (OrderBy ob in OrderByCollection) {
							if (ob.OrderColumnName.ToLower() == tblCol.QualifiedName.ToLower()
								|| ob.OrderColumnName.ToLower() == table.Provider.FormatIdentifier(tblCol.ColumnName).ToLower()
								|| ob.OrderColumnName.ToLower() == tblCol.ColumnName.ToLower()
							) { ob.OrderColumnName = selectCol;	}
						}
                    }
                }
                else
                    col = new StringBuilder(table.Provider.QualifyColumnName(table.SchemaName, table.Name, tblCol.ColumnName));
                col.Append(SqlFragment.AS);
                col.Append(tblCol.Table.Provider.FormatIdentifier(tblCol.ColumnName));

                if(i + 1 != table.Columns.Count)
                    col.Append(", ");

                strSelect.Append(col);
            }

            string strFrom = SqlFragment.FROM;
            if (table.Provider.DatabaseRequiresBracketedJoins) strFrom += new string('(', joinCount);
            strFrom += table.QualifiedName;

            StringBuilder strSQL = new StringBuilder();
            strSQL.Append(strSelect);
            strSQL.Append(strFrom);
            strSQL.Append(strJoin);

            if(wheres.Count > 0 || betweens.Count > 0 || inList != null || notInList != null)
            {
                string strWhere = DataProvider.BuildWhere(this);
                strSQL.Append(strWhere);
            }

            if(OrderByCollection.Count > 0)
            {
                strSQL.Append(SqlFragment.ORDER_BY);
                for(int j = 0; j < orderByCollection.Count; j++)
                {
                    string orderString = OrderByCollection[j].OrderString;
                    if(!String.IsNullOrEmpty(orderString))
                    {
                        strSQL.Append(orderString);
                        if(j + 1 != OrderByCollection.Count)
                            strSQL.Append(", ");
                    }
                }
            }

            QueryCommand qry = new QueryCommand(strSQL.ToString(), table.Provider.Name);

            foreach(Where where in wheres)
                qry.AddParameter(where.ParameterName, where.ParameterValue, where.DbType);

            foreach(BetweenAnd between in betweens)
            {
                qry.AddParameter(between.StartParameterName, between.StartDate, DbType.DateTime);
                qry.AddParameter(between.EndParameterName, between.EndDate, DbType.DateTime);
            }

            AddInNotInParameters(qry, inList, "in");
            AddInNotInParameters(qry, notInList, "notIn");
            return DataService.GetDataSet<T>(qry);
        }

        /// <summary>
        /// Add In/NotIn parameters to a QueryCommand
        /// </summary>
        /// <returns></returns>
        private static void AddInNotInParameters(QueryCommand qryCommand, object[] oList, string prefix)
        {
            if(oList != null)
            {
                if(oList.Length > 0)
                {
                    int iCount = 1;
                    foreach(object iItem in oList)
                    {
                        qryCommand.AddParameter(String.Concat(prefix, iCount), iItem, DbType.AnsiString);
                        iCount++;
                    }
                }
            }
        }

        /// <summary>
        /// Returns a scalar object based on the passed-in command
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            return DataService.ExecuteScalar(GetCommand());
        }

        /// <summary>
        /// Executes a pass-through query on the DB
        /// </summary>
        public void Execute()
        {
            DataService.ExecuteQuery(GetCommand());
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <returns></returns>
        private QueryCommand GetCommand()
        {
            QueryCommand cmd;
            switch(QueryType)
            {
                case QueryType.Select:
                    cmd = BuildSelectCommand();
                    break;
                case QueryType.Update:
                    cmd = BuildUpdateCommand();
                    break;
                case QueryType.Insert:
                    cmd = null;
                    break;
                case QueryType.Delete:
                    cmd = BuildDeleteCommand();
                    break;
                default:
                    cmd = null;
                    break;
            }

            if(cmd != null)
                cmd.ProviderName = ProviderName;

            return cmd;
        }

        #endregion


        #region Utility

        /// <summary>
        /// Adjusts the where query if the affected table contains a logical delete column.
        /// </summary>
        public void CheckLogicalDelete()
        {
            foreach(Where w in wheres)
            {
                if(Utility.IsLogicalDeleteColumn(w.ColumnName))
                    return;
            }
            foreach(TableSchema.TableColumn column in Schema.Columns)
            {
                if(Utility.IsLogicalDeleteColumn(column.ColumnName))
                {
                    LogicalDeleteColumn = column.ColumnName;
                    break;
                }
            }
        }

        #endregion


        #region props

        private string _providerName = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        internal List<BetweenAnd> betweens;

        /// <summary>
        /// 
        /// </summary>
        internal List<TableSchema.TableColumn> columns = new List<TableSchema.TableColumn>();

        private int commandTimeout = 60;

        /// <summary>
        /// 
        /// </summary>
        internal string inColumn = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        internal object[] inList;

        private bool isDistinct;

        /// <summary>
        /// 
        /// </summary>
        internal string notInColumn = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        internal object[] notInList;

        private OrderByCollection orderByCollection = new OrderByCollection();
        private int pageIndex = -1;

        private int pageSize = 20;
        private QueryType queryType = QueryType.Select;
        private string selectList = " * ";
        private TableSchema.Table table;
        private string top = "100 PERCENT";

        /// <summary>
        /// 
        /// </summary>
        internal List<Where> wheres;

        /// <summary>
        /// Gets the betweens.
        /// </summary>
        /// <value>The betweens.</value>
        public List<BetweenAnd> Betweens
        {
            get { return betweens; }
        }

        /// <summary>
        /// Gets the columns.
        /// </summary>
        /// <value>The columns.</value>
        public List<TableSchema.TableColumn> Columns
        {
            get { return columns; }
        }

        /// <summary>
        /// Gets the in column.
        /// </summary>
        /// <value>The in column.</value>
        public string InColumn
        {
            get { return inColumn; }
        }

        /// <summary>
        /// Gets the in list.
        /// </summary>
        /// <value>The in list.</value>
        public object[] InList
        {
            get { return inList; }
        }

        /// <summary>
        /// Gets the wheres.
        /// </summary>
        /// <value>The wheres.</value>
        public List<Where> Wheres
        {
            get { return wheres; }
        }

        /// <summary>
        /// Connection timeout in seconds. For you Phil...
        /// </summary>
        /// <value>The command timeout.</value>
        public int CommandTimeout
        {
            get { return commandTimeout; }
            set { commandTimeout = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is distinct.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is distinct; otherwise, <c>false</c>.
        /// </value>
        public bool IsDistinct
        {
            get { return isDistinct; }
            set { isDistinct = value; }
        }

        /// <summary>
        /// Controls the number of records returned when paging.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value; }
        }

        /// <summary>
        /// Returns a particular page. Index is zero based. -1 (default) returns all results
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }

        /// <summary>
        /// Gets or sets the type of the query.
        /// </summary>
        /// <value>The type of the query.</value>
        public QueryType QueryType
        {
            get { return queryType; }
            set { queryType = value; }
        }

        /// <summary>
        /// Gets or sets the top.
        /// </summary>
        /// <value>The top.</value>
        public string Top
        {
            get { return top; }
            set { top = value; }
        }

        /// <summary>
        /// Gets or sets the select list.
        /// </summary>
        /// <value>The select list.</value>
        public string SelectList
        {
            get { return selectList; }
            set { selectList = value; }
        }

        /// <summary>
        /// Gets or sets the order by.
        /// </summary>
        /// <value>The order by.</value>
        public OrderBy OrderBy
        {
            get
            {
                if(orderByCollection.Count > 0)
                    return orderByCollection[0];
                return null;
            }
            set
            {
                orderByCollection.Clear();
                orderByCollection.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the order by collection.
        /// </summary>
        /// <value>The order by collection.</value>
        public OrderByCollection OrderByCollection
        {
            get { return orderByCollection; }
            set { orderByCollection = value; }
        }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public DataProvider Provider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether foreign keys should be aliased when constructing queries.
        /// </summary>
        /// <value><c>true</c> if they should be aliased; otherwise, <c>false</c>.</value>
        public bool AliasForeignKeys { get; set; }

        /// <summary>
        /// Gets or sets the logical delete column.
        /// </summary>
        /// <value>The logical delete column.</value>
        public string LogicalDeleteColumn { get; set; }

        /// <summary>
        /// Returns a Query object from the passed comma-separated list of column names
        /// </summary>
        /// <param name="list">The comma-separated list of column names.</param>
        /// <returns></returns>
        public Query SetSelectList(string list)
        {
            selectList = list;
            return this;
        }

        /// <summary>
        /// Sets the top.
        /// </summary>
        /// <param name="tops">The tops.</param>
        /// <returns></returns>
        public Query SetTop(string tops)
        {
            top = tops;
            return this;
        }

        #endregion


        #region .ctors

        /// <summary>
        /// Builds the internal schema structure by querying the database for the given table name as
        /// part of the query process.
        /// WARNING: This method incurs more overhead than Query(TableSchema.Table).
        /// It is HIGHLY recommended that if possible, you pass a Schema to Query() instead.
        /// </summary>
        /// <param name="tableName">The name of the table that the database will be queried for.</param>
        public Query(string tableName)
        {
            table = BuildTableSchema(tableName);
            Provider = table.Provider;
            SetLists();
        }

        /// <summary>
        /// Builds the internal schema structure by querying the database for the given table name as
        /// part of the query process.
        /// WARNING: This method incurs more overhead than Query(TableSchema.Table).
        /// It is HIGHLY recommended that if possible, you pass a Schema to Query() instead.
        /// </summary>
        /// <param name="tableName">The name of the table that the database will be queried for.</param>
        /// <param name="providerName">The provider used for this query.</param>
        public Query(string tableName, string providerName)
        {
            table = BuildTableSchema(tableName, providerName);
            ProviderName = providerName;
            Provider = table.Provider;
            SetLists();
        }

        /// <summary>
        /// Executes a query by deriving parameters from the passed schema.
        /// This is the fastest and most efficient way to execute a query.
        /// It is HIGHLY recommended that you use this method instead of Query(string tableName)
        /// </summary>
        /// <param name="tbl">The table schema that will be used to derive parameters for the query</param>
        public Query(TableSchema.Table tbl)
        {
            if(tbl == null)
            {
                throw new ArgumentNullException("tbl",
                    "The Schema Table you passed in is null. If you've added a constructor to a class, make sure you reference 'GetTableSchema()' on the top line, so that the static schema is loaded.");
            }

            if(tbl.Columns == null)
                throw new ArgumentException("The Schema Table you passed in has no columns");

            ProviderName = tbl.Provider.Name;
            Provider = tbl.Provider;
            table = tbl;
            SetLists();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Query"/> class.
        /// </summary>
        /// <param name="primaryTable">The primary table.</param>
        /// <param name="cols">The cols.</param>
        public Query(TableSchema.Table primaryTable, params TableSchema.TableColumn[] cols)
        {
            if(primaryTable == null)
            {
                throw new ArgumentNullException("primaryTable",
                    "The Schema Table you passed in is null. If you've added a constructor to a class, make sure you reference 'GetTableSchema()' on the top line, so that the static schema is loaded.");
            }

            ProviderName = primaryTable.Provider.Name;
            Provider = primaryTable.Provider;
            table = primaryTable;
            SetLists();
            columns.AddRange(cols);
        }

        /// <summary>
        /// Sets the lists.
        /// </summary>
        private void SetLists()
        {
            wheres = new List<Where>();
            betweens = new List<BetweenAnd>();
            updateSettings = new Hashtable();
        }

        #endregion


        #region Aggregates

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public int GetCount(string columnName)
        {
            return Convert.ToInt32(ExecuteAggregate(columnName, AggregateFunctionName.COUNT, IsDistinct));
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public int GetCount(string columnName, Where where)
        {
            return Convert.ToInt32(ExecuteAggregate(columnName, where, AggregateFunctionName.COUNT, IsDistinct));
        }

        /// <summary>
        /// Gets the sum.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public object GetSum(string columnName)
        {
            return ExecuteAggregate(columnName, AggregateFunctionName.SUM, IsDistinct);
        }

        /// <summary>
        /// Gets the sum.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public object GetSum(string columnName, Where where)
        {
            return ExecuteAggregate(columnName, where, AggregateFunctionName.SUM, IsDistinct);
        }

        /// <summary>
        /// Gets the average.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public object GetAverage(string columnName)
        {
            return ExecuteAggregate(columnName, AggregateFunctionName.AVERAGE, IsDistinct);
        }

        /// <summary>
        /// Gets the average.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public object GetAverage(string columnName, Where where)
        {
            return ExecuteAggregate(columnName, where, AggregateFunctionName.AVERAGE, IsDistinct);
        }

        /// <summary>
        /// Gets the max.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public object GetMax(string columnName)
        {
            return ExecuteAggregate(columnName, AggregateFunctionName.MAX, false);
        }

        /// <summary>
        /// Gets the max.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public object GetMax(string columnName, Where where)
        {
            return ExecuteAggregate(columnName, where, AggregateFunctionName.MAX, false);
        }

        /// <summary>
        /// Gets the min.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public object GetMin(string columnName)
        {
            return ExecuteAggregate(columnName, AggregateFunctionName.MIN, false);
        }

        /// <summary>
        /// Gets the min.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public object GetMin(string columnName, Where where)
        {
            return ExecuteAggregate(columnName, where, AggregateFunctionName.MIN, false);
        }

        /// <summary>
        /// Executes the aggregate.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <param name="isDistinctQuery">if set to <c>true</c> [is distinct query].</param>
        /// <returns></returns>
        private object ExecuteAggregate(string columnName, string aggregateFunction, bool isDistinctQuery)
        {
            StringBuilder commandSql = new StringBuilder(SqlFragment.SELECT);
            commandSql.Append(Utility.MakeFunction(aggregateFunction, columnName, isDistinctQuery, Provider));
            commandSql.Append(SqlFragment.FROM);
            commandSql.Append(Provider.FormatIdentifier(Schema.Name));
            commandSql.Append(DataProvider.BuildWhere(this));
            QueryCommand cmd = new QueryCommand(commandSql.ToString(), ProviderName);
            DataProvider.AddWhereParameters(cmd, this);
            return DataService.ExecuteScalar(cmd);
        }

        /// <summary>
        /// Executes the aggregate.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="where">The where.</param>
        /// <param name="aggregateFunction">The aggregate function.</param>
        /// <param name="isDistinctQuery">if set to <c>true</c> [is distinct query].</param>
        /// <returns></returns>
        private object ExecuteAggregate(string columnName, Where where, string aggregateFunction, bool isDistinctQuery)
        {
            StringBuilder sql = new StringBuilder(SqlFragment.SELECT);
            sql.Append(Utility.MakeFunction(aggregateFunction, columnName, isDistinctQuery, Provider));
            sql.Append(SqlFragment.FROM);
            sql.Append(Provider.FormatIdentifier(Schema.Name));

            if(where != null)
            {
                sql.Append(SqlFragment.WHERE);
                sql.Append(Provider.FormatIdentifier(where.ColumnName));
                sql.Append(Where.GetComparisonOperator(where.Comparison));
                sql.Append(Provider.FormatParameterNameForSQL("p1"));
            }

            QueryCommand cmd = new QueryCommand(sql.ToString(), ProviderName);

            if(where != null)
                cmd.AddParameter("p1", where.ParameterValue, where.DbType);

            return DataService.ExecuteScalar(cmd);
        }

        #endregion
    }

    /// <summary>
    /// Summary for the UpdateQuery class
    /// </summary>
    public class UpdateQuery : Query
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateQuery"/> class.
        /// </summary>
        /// <param name="tableName">The name of the table that the database will be queried for.</param>
        public UpdateQuery(string tableName)
            : base(tableName)
        {
            QueryType = QueryType.Update;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateQuery"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public UpdateQuery(TableSchema.Table table)
            : base(table)
        {
            QueryType = QueryType.Update;
        }
    }

    /// <summary>
    /// Summary for the DeleteQuery class
    /// </summary>
    public class DeleteQuery : Query
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteQuery"/> class.
        /// </summary>
        /// <param name="tableName">The name of the table that the database will be queried for.</param>
        public DeleteQuery(string tableName)
            : base(tableName)
        {
            QueryType = QueryType.Delete;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteQuery"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public DeleteQuery(TableSchema.Table table)
            : base(table)
        {
            QueryType = QueryType.Delete;
        }
    }

    public enum QueryType
    {
        Select,
        Update,
        Insert,
        Delete
    }
}