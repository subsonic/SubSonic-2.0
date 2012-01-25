using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// For Jeff Atwood
    /// http://www.codinghorror.com/blog/archives/000989.html
    /// </summary>
    public class CodingHorror : InlineQuery {}

    /// <summary>
    /// A class which wraps an inline SQL call
    /// </summary>
    public class InlineQuery
    {
        private QueryCommand command;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineQuery"/> class.
        /// Warning: This method assumes the default provider is intended.
        /// Call InlineQuery(string providerName) if this is not the case.
        /// </summary>
        public InlineQuery()
        {
            command = new QueryCommand(String.Empty, DataService.Provider.Name); //Assumes default provider
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineQuery"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        public InlineQuery(string providerName)
        {
            command = new QueryCommand(String.Empty, providerName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineQuery"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public InlineQuery(ProviderBase dataProvider)
        {
            command = new QueryCommand(String.Empty, dataProvider.Name);
        }

        /// <summary>
        /// Executes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        public void Execute(string sql)
        {
            Execute(sql, new object[0]);
        }

        /// <summary>
        /// Executes the specified SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="values">The values.</param>
        public void Execute(string sql, params object[] values)
        {
            DataService.ExecuteQuery(GetCommand(sql, values));
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public TResult ExecuteScalar<TResult>(string sql)
        {
            return ExecuteScalar<TResult>(sql, new object[0]);
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public TResult ExecuteScalar<TResult>(string sql, params object[] values)
        {
            object oResult = DataService.ExecuteScalar(GetCommand(sql, values));
            TResult result = (TResult)Utility.ChangeType(oResult, typeof(TResult));
            return result;
        }

        /// <summary>
        /// Executes the typed list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public List<T> ExecuteTypedList<T>(string sql) where T : new()
        {
            return ExecuteTypedList<T>(sql, new object[0]);
        }

        /// <summary>
        /// Executes the typed list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public List<T> ExecuteTypedList<T>(string sql, params object[] values) where T : new()
        {
            QueryCommand cmd = GetCommand(sql, values);

            List<T> result = new List<T>();
            IDataReader rdr;
            Type iType = typeof(T);

            try
            {
                rdr = DataService.GetReader(cmd);
            }
            catch(Exception x)
            {
                SqlQueryException ex = new SqlQueryException(x.Message);
                throw ex;
            }

            if(rdr != null)
            {
                using(rdr)
                {
                    if(iType is IActiveRecord)
                    {
                        //load it
                        while(rdr.Read())
                        {
                            T item = new T();
                            //set to ActiveRecord
                            IActiveRecord arItem = (IActiveRecord)item;

                            arItem.Load(rdr);
                            result.Add(item);
                        }
                    }
                    else
                    {
                        //coerce the values, using some light reflection
                        iType.GetProperties();

                        //set the values
                        PropertyInfo prop;
                        while(rdr.Read())
                        {
                            T item = new T();

                            for(int i = 0; i < rdr.FieldCount; i++)
                            {
                                prop = iType.GetProperty(rdr.GetName(i));
                                if(prop != null && !DBNull.Value.Equals(rdr.GetValue(i)))
                                    prop.SetValue(item, rdr.GetValue(i), null);
                            }
                            result.Add(item);
                        }
                    }
                    rdr.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// Executes as collection.
        /// </summary>
        /// <typeparam name="ListType">The type of the ist type.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public virtual ListType ExecuteAsCollection<ListType>(string sql)
            where ListType : IAbstractList, new()
        {
            return ExecuteAsCollection<ListType>(sql, new object[0]);
        }

        /// <summary>
        /// Executes as collection.
        /// </summary>
        /// <typeparam name="ListType">The type of the ist type.</typeparam>
        /// <param name="sql">The SQL.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public virtual ListType ExecuteAsCollection<ListType>(string sql, params object[] values)
            where ListType : IAbstractList, new()
        {
            QueryCommand cmd = GetCommand(sql, values);

            ListType list = new ListType();
            try
            {
                IDataReader rdr = DataService.GetReader(cmd);
                list.LoadAndCloseReader(rdr);
            }
            catch(Exception x)
            {
                SqlQueryException ex = new SqlQueryException(x.Message);
                throw ex;
            }

            return list;
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql)
        {
            return ExecuteReader(sql, new object[0]);
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string sql, params object[] values)
        {
            IDataReader result = DataService.GetReader(GetCommand(sql, values));
            return result;
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns></returns>
        public QueryCommand GetCommand(string sql)
        {
            return GetCommand(sql, new object[0]);
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public QueryCommand GetCommand(string sql, params object[] values)
        {
            if(command == null)
                command = new QueryCommand(sql, DataService.Provider.Name); //Uses default provider

            command.CommandSql = sql;
            if(values.Length > 0)
                LoadCommandParams(sql, values);
            return command;
        }

        private void LoadCommandParams(string sql, object[] values)
        {
            //load up the params
            List<string> paramList = ParseParameters(sql);

            //validate it
            if(paramList.Count != values.Length)
            {
                throw new SqlQueryException(
                    "The parameter count doesn't match up with the values entered - this could be our fault with our parser; please check the list to make sure the count adds up, and if it does, please add some spacing around the parameters in the list");
            }

            int indexer = 0;
            foreach(string s in paramList)
            {
                command.Parameters.Add(s, values[indexer]);
                indexer++;
            }
        }

        private static List<string> ParseParameters(string sql)
        {
			//bferrier altered this so Inline Query works with Oracle
            Regex paramReg = new Regex(@"@\w*|:\w*|\[PARM__\w*\]");

            MatchCollection matches = paramReg.Matches(String.Concat(sql, " "));
            List<string> result = new List<string>(matches.Count);
			foreach (Match m in matches) { result.Add(m.Value); }

			// remove duplicate param names, leaving only the first occurrence
			for (int i = result.Count - 1; i >= 0; i--) {
				string s = result[i];
				for (int j = i - 1; j >= 0; j--) {
					if (result[j] == s) { result.RemoveAt(i); break; }
				}
			}

            return result;
        }
    }
}