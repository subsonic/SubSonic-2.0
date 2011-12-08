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
using System.Text.RegularExpressions;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the StoredProcedure class
    /// </summary>
    public class StoredProcedure
    {
        private readonly DataProvider _provider;
        private readonly string displayName;
        private readonly string name;
        private readonly string tableName;
        private QueryCommand _cmd;
        private ParameterCollection _parameters = new ParameterCollection();
        private int commandTimeout = 60;

        /// <summary>
        /// Output values returned by the StoredProcedure
        /// </summary>
        public List<object> OutputValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedure"/> class.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        public StoredProcedure(string spName) : this(spName, DataService.Provider) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedure"/> class.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="provider">The provider.</param>
        public StoredProcedure(string spName, DataProvider provider) : this(spName, provider, String.Empty) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="StoredProcedure"/> class.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="schemaName">Name of the schema.</param>
        public StoredProcedure(string spName, DataProvider provider, string schemaName)
        {
            name = spName;
            tableName = provider.SPClassName;

            _provider = provider;

            string newName = spName;
            SchemaName = schemaName;
            if(provider.ExtractClassNameFromSPName)
            {
                // Order of if/else matters here!
                Match underscoreMatch = Regex.Match(spName, RegexPattern.EMBEDDED_CLASS_NAME_UNDERSCORE_MATCH);
                if(underscoreMatch.Success)
                {
                    tableName = underscoreMatch.Value;
                    newName = Regex.Replace(spName, RegexPattern.EMBEDDED_CLASS_NAME_UNDERSCORE_REPLACE, String.Empty);
                }
                else
                {
                    Match match = Regex.Match(spName, RegexPattern.EMBEDDED_CLASS_NAME_MATCH);
                    if(match.Success)
                    {
                        tableName = match.Value;
                        newName = Regex.Replace(spName, RegexPattern.EMBEDDED_CLASS_NAME_REPLACE, String.Empty);
                    }
                }
            }

            displayName = TransformSPName(newName, provider);

            // init the list so the count comes back properly
            OutputValues = new List<object>();
        }

        /// <summary>
        /// Gets the name of column in fully qualified format, based on the provider.
        /// </summary>
        /// <value>The fully qualified name of the column.</value>
        public string QualifiedName
        {
            get { return _provider.QualifyTableName(SchemaName, name); }
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public QueryCommand Command
        {
            get
            {
                if(_cmd == null)
                {
                    _cmd = new QueryCommand(QualifiedName, Provider.Name)
                               {
                                   CommandType = CommandType.StoredProcedure,
                                   ProviderName = _provider.Name,
                                   CommandTimeout = commandTimeout
                               };
                }
                return _cmd;
            }
        }

        /// <summary>
        /// Gets or sets the parameter collection for the current instance
        /// </summary>
        /// <value></value>
        public ParameterCollection Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        /// <summary>
        /// Get the underlying DataProvider for the instance
        /// </summary>
        /// <value></value>
        public DataProvider Provider
        {
            get { return _provider; }
        }

        /// <summary>
        /// Gets or sets the command execution timeout (in seconds).
        /// </summary>
        /// <value>The command timeout.</value>
        public int CommandTimeout
        {
            get { return commandTimeout; }
            set
            {
                commandTimeout = value;
                Command.CommandTimeout = commandTimeout;
            }
        }

        /// <summary>
        /// Gets the name of the instance, as reflected in the underlying database
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// The DB Schema associated with this Stored Procedure
        /// </summary>
        public string SchemaName { get; set; }

        /// <summary>
        /// Gets the display name (the name of the stored procedure after any transformations are applied)
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return displayName; }
        }

        /// <summary>
        /// Gets the name of the table. 
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get { return tableName; }
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <returns></returns>
        public IDataReader GetReader()
        {
            IDataReader result = DataService.GetReader(Command);
            OutputValues = Command.OutputValues;
            return result;
        }

        /// <summary>
        /// Executes an SP using the default provider and returns a reader
        /// </summary>
        /// <param name="spAndParams">The SP name and each argument required - these must be in order</param>
        /// <returns>System.Data.IDataReader</returns>
        public static IDataReader GetReader(string spAndParams)
        {
            return GetReader(spAndParams, String.Empty);
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <param name="spAndParams">The sp and params.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static IDataReader GetReader(string spAndParams, string providerName)
        {
            QueryCommand cmd = new QueryCommand(String.Concat("EXEC ", spAndParams), providerName)
                                   {
                                       CommandType = CommandType.Text,
                                       ProviderName = providerName
                                   };

            IDataReader result = DataService.GetReader(cmd);
            return result;
        }

        /// <summary>
        /// Executes an SP using the default provider
        /// </summary>
        /// <param name="spAndParams">The SP name and each argument required - these must be in order</param>
        /// <returns>System.Data.DataSet</returns>
        public static DataSet GetDataSet(string spAndParams)
        {
            return GetDataSet(spAndParams, String.Empty);
        }

        /// <summary>
        /// Executes an SP using the specified provider
        /// </summary>
        /// <param name="spAndParams">The SP name and each argument required - these must be in order</param>
        /// <param name="providerName">Specifies the specific provider to use.</param>
        /// <returns>System.Data.DataSet</returns>
        public static DataSet GetDataSet(string spAndParams, string providerName)
        {
            QueryCommand cmd = new QueryCommand(String.Concat("EXEC ", spAndParams), providerName)
                                   {
                                       CommandType = CommandType.Text,
                                       ProviderName = providerName
                                   };
            return DataService.GetDataSet(cmd);
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <returns></returns>
        public DataSet GetDataSet()
        {
            return GetDataSet<DataSet>();
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetDataSet<T>() where T : DataSet, new()
        {
            T result = DataService.GetDataSet<T>(Command);
            OutputValues = Command.OutputValues;
            return result;
        }

        /// <summary>
        /// Executes the stored procedure and returns the number of rows affected.
        /// </summary>
        /// <returns></returns>
        public int Execute()
        {
            int rowsAffected = DataService.ExecuteQuery(Command);
            OutputValues = Command.OutputValues;
            return rowsAffected;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <returns></returns>
        public object ExecuteScalar()
        {
            object result = DataService.ExecuteScalar(Command);
            OutputValues = Command.OutputValues;
            return result;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public object ExecuteScalar<T>()
        {
            object result = DataService.ExecuteScalar(Command);
            OutputValues = Command.OutputValues;
            T converted = (T)Utility.ChangeType(result, typeof(T));
            return converted;
        }

        /// <summary>
        /// Executes the typed list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> ExecuteTypedList<T>() where T : new()
        {
            List<T> result;
            using(IDataReader rdr = DataService.GetReader(Command))
            {
                result = SqlQuery.BuildTypedResult<T>(rdr);
                rdr.Close();
            }

            return result;
        }

        /// <summary>
        /// Returns the currently built-up instance of this procedure
        /// </summary>
        /// <returns></returns>
        public StoredProcedure GetBuiltProcedure()
        {
            return this;
        }

        /// <summary>
        /// Transforms the name of the SP.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        private static string TransformSPName(string spName, DataProvider provider)
        {
            if(String.IsNullOrEmpty(spName))
                return String.Empty;

            string newName = spName;
            newName = Utility.RegexTransform(newName, provider);
            newName = Utility.StripText(newName, provider.StripSPText);
            newName = Utility.GetProperName(newName, provider);
            newName = Utility.IsStringNumeric(newName) ? "_" + newName : newName;
            newName = Utility.StripNonAlphaNumeric(newName);
            newName = newName.Trim();
            return Utility.KeyWordCheck(newName, String.Empty, provider);
        }


        #region Nested type: Parameter

        /// <summary>
        /// Summary for the Parameter class
        /// </summary>
        public class Parameter
        {
            private ParameterDirection mode = ParameterDirection.Input;

            /// <summary>
            /// Gets or sets the type of the DB.
            /// </summary>
            /// <value>The type of the DB.</value>
            public DbType DBType { get; set; }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the display name.
            /// </summary>
            /// <value>The display name.</value>
            public string DisplayName { get; set; }

            /// <summary>
            /// Gets or sets the query parameter.
            /// </summary>
            /// <value>The query parameter.</value>
            public string QueryParameter { get; set; }

            /// <summary>
            /// Gets or sets the mode.
            /// </summary>
            /// <value>The mode.</value>
            public ParameterDirection Mode
            {
                get { return mode; }
                set { mode = value; }
            }

            /// <summary>
            /// Gets or sets the precision.
            /// </summary>
            /// <value>The precision.</value>
            public int? Precision { get; set; }

            /// <summary>
            /// Gets or sets the scale.
            /// </summary>
            /// <value>The scale.</value>
            public int? Scale { get; set; }
        }

        #endregion


        #region Nested type: ParameterCollection

        /// <summary>
        /// Summary for the ParameterCollection class
        /// </summary>
        public class ParameterCollection : List<Parameter> {}

        #endregion
    }
}