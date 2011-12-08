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
using System.ComponentModel;
using System.Data;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class Setting
    {
        private string _columnName = String.Empty;
        private string _parameterName = String.Empty;
        internal DbType DataType = DbType.AnsiString;

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get { return _columnName; }
            internal set { _columnName = value; }
        }

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

        /// <summary>
        /// Gets or sets the query.
        /// </summary>
        /// <value>The query.</value>
        public Update query { get; internal set; }

        /// <summary>
        /// Equals to.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Update EqualTo(object value)
        {
            Value = value;
            query.SetStatements.Add(this);
            return query;
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string ToString()
        {
            return base.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Update : SqlQuery
    {
        private List<Setting> _setStatements = new List<Setting>();

        /// <summary>
        /// Gets or sets the set statements.
        /// </summary>
        /// <value>The set statements.</value>
        public List<Setting> SetStatements
        {
            get { return _setStatements; }
            internal set { _setStatements = value; }
        }


        #region .ctors

        /// <summary>
        /// Initializes a new instance of the <see cref="Update"/> class.
        /// </summary>
        /// <param name="tbl">The TBL.</param>
        public Update(TableSchema.Table tbl)
        {
            Init(tbl);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Update"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public Update(string tableName)
        {
            TableSchema.Table tbl = DataService.GetSchema(tableName, "");
            Init(tbl);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Update"/> class.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        public Update(string tableName, string providerName)
        {
            TableSchema.Table tbl = DataService.GetSchema(tableName, providerName);
            Init(tbl);
        }

        private void Init(TableSchema.Table tbl)
        {
            if(tbl == null)
                throw new SqlQueryException("Can't find the table schema - please specify the provider if there is more than one, or check the spelling");
            Provider = tbl.Provider;
            FromTables.Add(tbl);
            QueryCommandType = QueryType.Update;
        }

        #endregion


        #region SET

        /// <summary>
        /// Sets the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public Setting Set(string columnName)
        {
            return CreateSetting(columnName, GetFROMCol(columnName).DataType, false);
        }

        /// <summary>
        /// Sets the specified col.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public Setting Set(TableSchema.TableColumn col)
        {
            return CreateSetting(col.ColumnName, col.DataType, false);
        }

        /// <summary>
        /// Sets the expression.
        /// </summary>
        /// <param name="columnName">The column.</param>
        /// <returns></returns>
        public Setting SetExpression(string columnName)
        {
            return CreateSetting(columnName, DbType.String, true);
        }

        /// <summary>
        /// Sets the expression.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        public Setting SetExpression(TableSchema.TableColumn col)
        {
            return CreateSetting(col.ColumnName, col.DataType, true);
        }

        private Setting CreateSetting(string columnName, DbType dbType, bool isExpression)
        {
            Setting s = new Setting
                            {
                                query = this,
                                ColumnName = columnName,
                                ParameterName = Provider.FormatParameterNameForSQL(String.Concat("up_", Utility.StripNonAlphaNumeric(columnName))),
                                IsExpression = isExpression,
                                DataType = dbType
                            };
            return s;
        }

        #endregion


        #region Execution

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public override int Execute()
        {
            int result;
            ISqlGenerator generator = GetGenerator();
            string sql = generator.BuildUpdateStatement();
            QueryCommand cmd = Provider != null ? new QueryCommand(sql, Provider.Name) : new QueryCommand(sql);

            //add in the commands
            foreach(Setting s in SetStatements)
                cmd.Parameters.Add(s.ParameterName, s.Value, s.DataType);

            //set the contstraints
            SetConstraintParams(cmd);

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