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
using System.Collections.Generic;
using System.Data;
using System.Text;
using SubSonic.Sugar;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class InsertSetting
    {
        internal string _parameterName = String.Empty;
        internal string ColumnName = String.Empty;
        internal DbType DataType = DbType.AnsiString;

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string ParameterName
        {
            get { return _parameterName; }
            internal set { _parameterName = value; }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expression.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expression; otherwise, <c>false</c>.
        /// </value>
        public bool IsExpression { get; internal set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Insert
    {
        private List<InsertSetting> _inserts = new List<InsertSetting>();
        private DataProvider provider;
        internal List<string> SelectColumnList = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Insert"/> class.
        /// </summary>
        public Insert() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Insert"/> class.
        /// WARNING: This overload should only be used with applications that use a single provider!
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public Insert(string tableName)
        {
            TableSchema.Table tbl = DataService.GetSchema(tableName, String.Empty);
            Init(tbl);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Insert"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        public Insert(string tableName, string providerName)
        {
            TableSchema.Table tbl = DataService.GetSchema(tableName, providerName);
            Init(tbl);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Insert"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        public Insert(TableSchema.Table table)
        {
            Init(table);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Insert"/> class.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="initializeAllColumns">if set to <c>true</c> [initialize all columns].</param>
        public Insert(TableSchema.Table table, bool initializeAllColumns)
        {
            if(initializeAllColumns)
                Init(table);
            else
            {
                Table = table;
                provider = table.Provider;
            }
        }

        /// <summary>
        /// Gets or sets the select values.
        /// </summary>
        /// <value>The select values.</value>
        public SqlQuery SelectValues { get; internal set; }

        /// <summary>
        /// Gets or sets the table.
        /// </summary>
        /// <value>The table.</value>
        public TableSchema.Table Table { get; internal set; }

        /// <summary>
        /// Gets or sets the inserts.
        /// </summary>
        /// <value>The inserts.</value>
        public List<InsertSetting> Inserts
        {
            get { return _inserts; }
            internal set { _inserts = value; }
        }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public DataProvider Provider
        {
            get { return provider; }
            set { provider = value; }
        }

        /// <summary>
        /// Gets the select columns.
        /// </summary>
        /// <value>The select columns.</value>
        public string SelectColumns
        {
            get { return Strings.ToDelimitedList(SelectColumnList); }
        }

        /// <summary>
        /// Adds the specified columns into a new Insert object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public Insert Into<T>(params string[] columns) where T : ActiveRecord<T>, new()
        {
            SelectColumnList.Clear();
            SelectColumnList.AddRange(columns);
            return Init(new T().GetSchema());
        }

        /// <summary>
        /// Adds the specified columns into a new Insert object.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public Insert Into(TableSchema.Table table, params string[] columns)
        {
            SelectColumnList.Clear();
            SelectColumnList.AddRange(columns);
            return Init(table);
        }

        /// <summary>
        /// Returns a new Insert object using the specified table.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public Insert Into(TableSchema.Table table)
        {
            return Init(table);
        }

        private Insert Init(TableSchema.Table table)
        {
            if(table == null)
                throw new SqlQueryException("Can't find the table schema - please specify the provider if there is more than one, or check the spelling");

            if(SelectColumnList.Count == 0)
            {
                string columnList = table.GetDelimitedColumnList(",", false, true);
                SelectColumnList.AddRange(columnList.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries));
                //SelectColumns = columnList;
            }
            else
            {
                bool isFirst = true;
                StringBuilder sb = new StringBuilder();
                foreach(string s in SelectColumnList)
                {
                    if(!isFirst)
                        sb.Append(",");
                    sb.Append(s);

                    isFirst = false;
                }
                //SelectColumns = sb.ToString();
            }

            if(Table == null)
                Table = table;

            provider = table.Provider;
            return this;
        }

        private Insert AddValues(bool isExpression, params object[] values)
        {
            //this is a lineup game
            //make sure that the count of values
            //is equal to the columns
            if(values.Length != SelectColumnList.Count)
                throw new SqlQueryException("The Select list and value list don't match - they need to match exactly if you're creating an INSERT VALUES query");

            int itemIndex = 0;
            foreach(string s in SelectColumnList)
            {
                AddInsertSetting(s, values[itemIndex], DbType.AnsiString, isExpression);
                itemIndex++;
            }

            return this;
        }

        private void AddInsertSetting(string columnName, object columnValue, DbType dbType, bool isExpression)
        {
            InsertSetting setting = new InsertSetting
                                        {
                                            ColumnName = columnName,
                                            ParameterName = provider.FormatParameterNameForSQL("ins_" + columnName),
                                            Value = columnValue,
                                            IsExpression = isExpression,
                                            DataType = dbType
                                        };
            Inserts.Add(setting);
        }

        /// <summary>
        /// Values the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <param name="columnValue">The column value.</param>
        /// <returns></returns>
        public Insert Value(TableSchema.TableColumn column, object columnValue)
        {
            AddInsertSetting(column.ColumnName, columnValue, column.DataType, false);
            SelectColumnList.Add(column.ColumnName);
            return this;
        }

        /// <summary>
        /// Valueses the specified values.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public Insert Values(params object[] values)
        {
            return AddValues(false, values);
        }

        /// <summary>
        /// Values the expression.
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public Insert ValueExpression(params object[] values)
        {
            return AddValues(true, values);
        }

        /// <summary>
        /// Selects the specified select results.
        /// </summary>
        /// <param name="selectResults">The select results.</param>
        /// <returns></returns>
        public Insert Select(SqlQuery selectResults)
        {
            //validate the query
            //hard-set the select list
            //most of the time we don't want to insert into the PK
            if(selectResults.SelectColumnList.Length == 0)
            {
                bool omitPK = selectResults.FromTables[0].PrimaryKey.AutoIncrement;

                string columns = selectResults.FromTables[0].GetDelimitedColumnList(",", false, omitPK);
                selectResults.SelectColumnList = columns.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            }

            SelectValues = selectResults;
            return this;
        }

        /// <summary>
        /// Builds the SQL statement.
        /// </summary>
        /// <returns></returns>
        public string BuildSqlStatement()
        {
            SqlQuery q = new SqlQuery(provider);
            ISqlGenerator generator = q.GetGenerator();
            generator.SetInsertQuery(this);

            string sql = generator.BuildInsertStatement();
            return sql;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return BuildSqlStatement();
        }


        #region Execution

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            int result;
            string sql = BuildSqlStatement();
            QueryCommand cmd = provider != null ? new QueryCommand(sql, provider.Name) : new QueryCommand(sql, DataService.Provider.Name);

            //add in the commands
            foreach(InsertSetting s in Inserts)
                cmd.Parameters.Add(s.ParameterName, s.Value, s.DataType);

            //set the contstraints, if we're using a Select statement
            if(Inserts.Count == 0 && SelectValues != null)
                SqlQuery.SetConstraintParams(SelectValues, cmd);

            try
            {
                result = DataService.ExecuteQuery(cmd);
            }
            catch(Exception x)
            {
                SqlQueryException ex = new SqlQueryException(x.Message);
                throw ex;
            }
            return result;
        }

        #endregion
    }
}