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
using System.Data.Common;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// This set of classes abstracts out commands and their parameters so that
    /// the DataProviders can work their magic regardless of the client type. The
    /// System.Data.Common class was supposed to do this, but sort of fell flat
    /// when it came to MySQL and other DB Providers that don't implement the Data
    /// Factory pattern. Abstracts out the assignment of parameters, etc
    /// </summary>
    public class QueryParameter
    {
        internal const ParameterDirection DefaultParameterDirection = ParameterDirection.Input;
        internal const int DefaultSize = 50;
        private ParameterDirection _mode = DefaultParameterDirection;

        private int _size = DefaultSize;

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <value>The size.</value>
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        public ParameterDirection Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string ParameterName { get; set; }

        /// <summary>
        /// Gets or sets the parameter value.
        /// </summary>
        /// <value>The parameter value.</value>
        public object ParameterValue { get; set; }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public DbType DataType { get; set; }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public int? Scale { get; set; }

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>The precision.</value>
        public int? Precision { get; set; }
    }

    /// <summary>
    /// Summary for the QueryParameterCollection class
    /// </summary>
    public class QueryParameterCollection : List<QueryParameter>
    {
        /// <summary>
        /// Checks to see if specified parameter exists in the current collection
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public bool Contains(string parameterName)
        {
            foreach(QueryParameter p in this)
            {
                if(Utility.IsMatch(p.ParameterName, parameterName, true))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// returns the specified QueryParameter, if it exists in this collection
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public QueryParameter GetParameter(string parameterName)
        {
            foreach(QueryParameter p in this)
            {
                if(Utility.IsMatch(p.ParameterName, parameterName, true))
                    return p;
            }
            return null;
        }

        /// <summary>
        /// Adds the specified parameter name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        public void Add(string parameterName, object value)
        {
            Add(parameterName, value, DbType.AnsiString, ParameterDirection.Input);
        }

        /// <summary>
        /// Adds the specified parameter name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="dataType">Type of the data.</param>
        public void Add(string parameterName, object value, DbType dataType)
        {
            Add(parameterName, value, dataType, ParameterDirection.Input);
        }

        /// <summary>
        /// Adds the specified parameter name.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="mode">The mode.</param>
        public void Add(string parameterName, object value, DbType dataType, ParameterDirection mode)
        {
            //remove if already added, and replace with last in
            if(Contains(parameterName))
                Remove(GetParameter(parameterName));

            QueryParameter param = new QueryParameter
                                       {
                                           ParameterName = parameterName,
                                           ParameterValue = value,
                                           DataType = dataType,
                                           Mode = mode
                                       };
            Add(param);
        }
    }

    /// <summary>
    /// Summary for the QueryCommandCollection class
    /// </summary>
    public class QueryCommandCollection : List<QueryCommand> {}

    /// <summary>
    /// Summary for the QueryCommand class
    /// </summary>
    public class QueryCommand
    {
        private string _providerName = String.Empty;
        private int commandTimeout = 60;

        /// <summary>
        /// 
        /// </summary>
        public List<object> OutputValues;

        private QueryParameterCollection parameters;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommand"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="providerName">Name of the provider.</param>
        public QueryCommand(string sql, string providerName)
        {
            ProviderName = providerName;
            Provider = DataService.GetInstance(providerName);
            CommandSql = sql;
            CommandType = CommandType.Text;
            parameters = new QueryParameterCollection();
            OutputValues = new List<object>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryCommand"/> class.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        [Obsolete("Deprecated: Use QueryCommand(string sql, string providerName) instead.")]
        public QueryCommand(string sql)
        {
            //use the default
            ProviderName = DataService.Provider.Name;
            Provider = DataService.Provider;
            CommandSql = sql;
            CommandType = CommandType.Text;
            parameters = new QueryParameterCollection();
            OutputValues = new List<object>();
        }

        /// <summary>
        /// Gets or sets the command timeout (in seconds).
        /// </summary>
        /// <value>The command timeout.</value>
        public int CommandTimeout
        {
            get { return commandTimeout; }
            set { commandTimeout = value; }
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
        /// Gets or sets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public CommandType CommandType { get; set; }

        /// <summary>
        /// Gets or sets the command SQL.
        /// </summary>
        /// <value>The command SQL.</value>
        public string CommandSql { get; set; }

        /// <summary>
        /// Gets or sets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public QueryParameterCollection Parameters
        {
            get { return parameters; }
            set { parameters = value; }
        }

        /// <summary>
        /// Determines whether [has output params].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has output params]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasOutputParams()
        {
            bool bOut = false;

            //loop the params and see if one is in/out
            foreach(QueryParameter param in Parameters)
            {
                if(param.Mode != ParameterDirection.Input)
                {
                    bOut = true;
                    break;
                }
            }

            return bOut;
        }

        /// <summary>
        /// Adds the parameter. The public AddParameter methods should call this one.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        private void AddParameter(string parameterName, object parameterValue, int maxSize, DbType dbType, ParameterDirection direction, int? scale, int? precision)
        {
            if(parameters == null)
                parameters = new QueryParameterCollection();

            QueryParameter param = new QueryParameter
                                       {
                                           ParameterName = CommandType == CommandType.StoredProcedure ? parameterName : Provider.FormatParameterNameForSQL(parameterName),
                                           ParameterValue = parameterValue ?? DBNull.Value,
                                           Mode = direction,
                                           DataType = dbType,
                                           Scale = scale,
                                           Precision = precision
                                       };

            if(maxSize > -1 && direction != ParameterDirection.Output)
                param.Size = maxSize;

            parameters.Add(param);
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        public void AddParameter(string parameterName, object parameterValue, DbType dataType, ParameterDirection parameterDirection)
        {
            AddParameter(parameterName, parameterValue, QueryParameter.DefaultSize, dataType, parameterDirection, null, null);
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="parameterDirection">The parameter direction.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public void AddParameter(string parameterName, object parameterValue, DbType dataType, ParameterDirection parameterDirection, int? scale, int? precision)
        {
            AddParameter(parameterName, parameterValue, QueryParameter.DefaultSize, dataType, parameterDirection, scale, precision);
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="dataType">Type of the data.</param>
        public void AddParameter(string parameterName, object parameterValue, DbType dataType)
        {
            AddParameter(parameterName, parameterValue, QueryParameter.DefaultSize, dataType, QueryParameter.DefaultParameterDirection, null, null);
        }

        /// <summary>
        /// Adds the parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parameterValue">The parameter value.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public void AddParameter(string parameterName, object parameterValue, DbType dataType, int? scale, int? precision)
        {
            AddParameter(parameterName, parameterValue, QueryParameter.DefaultSize, dataType, QueryParameter.DefaultParameterDirection, scale, precision);
        }

        /// <summary>
        /// Adds the output parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <param name="dbType">Type of the db.</param>
        public void AddOutputParameter(string parameterName, int maxSize, DbType dbType)
        {
            AddOutputParameter(parameterName, maxSize, dbType, null, null);
        }

        /// <summary>
        /// Adds the output parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="maxSize">Size of the max.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public void AddOutputParameter(string parameterName, int maxSize, DbType dbType, int? scale, int? precision)
        {
            AddParameter(parameterName, DBNull.Value, maxSize, dbType, ParameterDirection.Output, scale, precision);
        }

        /// <summary>
        /// Adds the output parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="maxSize">Size of the max.</param>
        public void AddOutputParameter(string parameterName, int maxSize)
        {
            AddOutputParameter(parameterName, maxSize, DbType.AnsiString, null, null);
        }

        /// <summary>
        /// Adds the output parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        public void AddOutputParameter(string parameterName)
        {
            AddOutputParameter(parameterName, -1, DbType.AnsiString, null, null);
        }

        /// <summary>
        /// Adds the output parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="dbType">Type of the db.</param>
        public void AddOutputParameter(string parameterName, DbType dbType)
        {
            AddOutputParameter(parameterName, -1, dbType, null, null);
        }

        /// <summary>
        /// Adds the output parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public void AddOutputParameter(string parameterName, DbType dbType, int? scale, int? precision)
        {
            AddOutputParameter(parameterName, -1, dbType, scale, precision);
        }

        /// <summary>
        /// Adds a return parameter (RETURN_VALUE) to the command.
        /// </summary>
        public void AddReturnParameter()
        {
            if(Provider != null)
                AddParameter(String.Concat(Provider.GetParameterPrefix(), "RETURN_VALUE"), null, DbType.Int32, ParameterDirection.ReturnValue);
            else
                AddParameter("@RETURN_VALUE", null, DbType.Int32, ParameterDirection.ReturnValue);
        }

        /// <summary>
        /// Converts the QueryCommand to an IDbCommand.
        /// </summary>
        /// <returns></returns>
        public IDbCommand ToIDbCommand()
        {
            return DataService.GetIDbCommand(this);
        }

        /// <summary>
        /// Converts the QueryCommand to a DbCommand.
        /// </summary>
        /// <returns></returns>
        public DbCommand ToDbCommand()
        {
            return DataService.GetDbCommand(this);
        }
    }
}
