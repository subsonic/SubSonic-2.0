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
using System.Data;
using System.Web.UI.WebControls;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Base class for read-only, data-aware objects (Views). 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class ReadOnlyRecord<T> : RecordBase<T>, IReadOnlyRecord where T : RecordBase<T>, new()
    {
        #region Fetchers

        /// <summary>
        /// Loads a the AbstractRecord by specifying an arbitrary column name and value
        /// as the retrieval criteria
        /// </summary>
        /// <param name="columnName">Name of the column to use as the retrieval key</param>
        /// <param name="paramValue">Value of the column to use as the retrieval criteria</param>
        public void LoadByParam(string columnName, object paramValue)
        {
            MarkOld();
            IDataReader rdr = null;
            try
            {
                rdr = new Query(BaseSchema).AddWhere(columnName, paramValue).ExecuteReader();


                if (rdr.Read())
                    Load(rdr);
                else {
                    MarkNew();
                    IsLoaded = false;
                }
            }
            finally
            {
                if(rdr != null)
                    rdr.Close();
            }
        }

        /// <summary>
        /// Loads the record by fetching the underlying record with the passed value as the primary key
        /// </summary>
        /// <param name="keyID">The primary key value to retrieve the record for</param>
        public void LoadByKey(object keyID)
        {
            MarkOld();
            IDataReader rdr = null;
            try
            {
                Query q = new Query(BaseSchema).AddWhere(BaseSchema.PrimaryKey.ColumnName, keyID);

                rdr = q.ExecuteReader();

                if (rdr.Read())
                    Load(rdr);
                else
                {
                    MarkNew();
                    IsLoaded = false;
                }
            }
            finally
            {
                if(rdr != null)
                    rdr.Close();
            }
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        public static T FetchByID(int keyValue)
        {
            return FetchByIdInternal(keyValue);
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        public static T FetchByID(int? keyValue)
        {
            return FetchByIdInternal(keyValue);
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        public static T FetchByID(long keyValue)
        {
            return FetchByIdInternal(keyValue);
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        public static T FetchByID(long? keyValue)
        {
            return FetchByIdInternal(keyValue);
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        public static T FetchByID(decimal keyValue)
        {
            return FetchByIdInternal(keyValue);
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        public static T FetchByID(decimal? keyValue)
        {
            return FetchByIdInternal(keyValue);
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        public static T FetchByID(Guid keyValue)
        {
            return FetchByIdInternal(keyValue);
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        public static T FetchByID(Guid? keyValue)
        {
            return FetchByIdInternal(keyValue);
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        public static T FetchByID(string keyValue)
        {
            return FetchByIdInternal(keyValue);
        }

        /// <summary>
        /// Returns a record for this keyValue
        /// </summary>
        /// <param name="keyValue">Key Value</param>
        /// <returns></returns>
        private static T FetchByIdInternal(object keyValue)
        {
            if(keyValue == null)
                return null;

            // makes sure the table schema is loaded
            T item = new T();

            // build the query
            Query q = new Query(BaseSchema).WHERE(BaseSchema.PrimaryKey.ColumnName, Comparison.Equals, keyValue);

            // load the reader
            using(IDataReader rdr = DataService.GetSingleRecordReader(q.BuildSelectCommand()))
            {
                if(rdr.Read())
                    item.Load(rdr);
                rdr.Close();
            }

            if(!item._isNew && item.IsLoaded)
                return item;
            return null;
        }

        /// <summary>
        /// Returns all records for this table
        /// </summary>
        /// <returns>IDataReader</returns>
        public static IDataReader FetchAll()
        {
            Query q = new Query(BaseSchema);
            CheckLogicalDelete(q);

            IDataReader rdr = DataService.GetReader(q.BuildSelectCommand());
            return rdr;
        }

        /// <summary>
        /// Returns all records for this table, ordered
        /// </summary>
        /// <returns>Generic Typed List</returns>
        /// <param name="orderBy">OrderBy object used to specify sort behavior</param>
        public static IDataReader FetchAll(OrderBy orderBy)
        {
            Query q = new Query(BaseSchema)
                          {
                              OrderBy = orderBy
                          };
            CheckLogicalDelete(q);

            return DataService.GetReader(q.BuildSelectCommand());
        }

        /// <summary>
        /// Returns all records for the given column/parameter, ordered by the passed in orderBy
        /// The expression for this is always "column=parameter"
        /// </summary>
        /// <param name="columnName">Name of the column to use in parameter statement</param>
        /// <param name="oValue">Value of the column</param>
        /// <param name="orderBy">Ordering of results</param>
        /// <returns>IDataReader</returns>
        public static IDataReader FetchByParameter(string columnName, object oValue, OrderBy orderBy)
        {
            Query q = new Query(BaseSchema)
                          {
                              OrderBy = orderBy
                          };
            q.AddWhere(columnName, oValue);
            CheckLogicalDelete(q);

            return DataService.GetReader(q.BuildSelectCommand());
        }

        /// <summary>
        /// Returns all records for the given column/parameter
        /// The expression for this is always "column=parameter"
        /// </summary>
        /// <param name="columnName">The name of the column, as defined in the database</param>
        /// <param name="oValue">The value to match when fetching</param>
        /// <returns>IDataReader</returns>
        public static IDataReader FetchByParameter(string columnName, object oValue)
        {
            Query q = new Query(BaseSchema);
            q.AddWhere(columnName, oValue);
            CheckLogicalDelete(q);

            return DataService.GetReader(q.BuildSelectCommand());
        }

        /// <summary>
        /// Returns all records for the given column, comparison operator, and parameter
        /// This overload is used for queries that don't use and '=' operator, i.e. IS, IS NOT, etc.
        /// </summary>
        /// <param name="columnName">The name of the column, as defined in the database</param>
        /// <param name="comparison">The comparison operator used for the query</param>
        /// <param name="oValue">The value to match when fetching</param>
        /// <returns>IDataReader</returns>
        public static IDataReader FetchByParameter(string columnName, Comparison comparison, object oValue)
        {
            Query q = new Query(BaseSchema);
            q.AddWhere(columnName, comparison, oValue);
            CheckLogicalDelete(q);

            return DataService.GetReader(q.BuildSelectCommand());
        }

        /// <summary>
        /// Returns all records for the given column, comparison operator, and parameter
        /// in the order specified with the OrderBy parameter.
        /// This overload is used for queries that don't use and '=' operator, i.e. IS, IS NOT, etc,
        /// </summary>
        /// <param name="columnName">The name of the column, as defined in the database</param>
        /// <param name="comparison">The comparison operator used for the query</param>
        /// <param name="oValue">The value to match when fetching</param>
        /// <param name="orderBy">An OrderBy object used determine the order of results</param>
        /// <returns>IDataReader</returns>
        public static IDataReader FetchByParameter(string columnName, Comparison comparison, object oValue, OrderBy orderBy)
        {
            Query q = new Query(BaseSchema);
            q.AddWhere(columnName, comparison, oValue);
            q.OrderBy = orderBy;
            CheckLogicalDelete(q);

            return DataService.GetReader(q.BuildSelectCommand());
        }

        /// <summary>
        /// Returns all records for the given Query object
        /// </summary>
        /// <param name="query">Query for complex records</param>
        /// <returns>Generic Typed List</returns>
        public static IDataReader FetchByQuery(Query query)
        {
            CheckLogicalDelete(query);
            return DataService.GetReader(query.BuildSelectCommand());
        }

        /// <summary>
        /// Uses the passed-in object as a parameter set. Does not use the created/modified fields
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public static IDataReader Find(T item)
        {
            return Find(item, null);
        }

        /// <summary>
        /// Finds the specified item by building a SELECT query with conditions matching
        /// the values of the properties on the AbstractRecord object to the columns
        /// on the corresponding database table 
        /// </summary>
        /// <param name="item">The AbstractRecord derived type</param>
        /// <param name="orderBy">The OrberBy object used to return results</param>
        /// <returns></returns>
        public static IDataReader Find(T item, OrderBy orderBy)
        {
            Query q = new Query(BaseSchema);
            CheckLogicalDelete(q);

            // retrieve data from database
            foreach(TableSchema.TableColumn col in BaseSchema.Columns)
            {
                string columnName = col.ColumnName;
                object columnValue = item.GetColumnValue<object>(columnName);
                if(!Utility.IsAuditField(columnName))
                {
                    object defaultValue = String.Empty;
                    switch(col.DataType)
                    {
                        case DbType.Boolean:
                            defaultValue = false;
                            break;
                        case DbType.Currency:
                        case DbType.Decimal:
                        case DbType.Int16:
                        case DbType.Double:
                        case DbType.Int32:
                            defaultValue = 0;
                            break;
                        case DbType.Date:
                        case DbType.DateTime:
                            defaultValue = new DateTime(1900, 1, 1);
                            break;
                        case DbType.Guid:
                            defaultValue = Guid.Empty;
                            break;
                    }

                    if(columnValue != null)
                    {
                        if(!columnValue.Equals(defaultValue))
                            q.AddWhere(columnName, columnValue);
                    }
                }
            }

            if(orderBy != null)
                q.OrderBy = orderBy;

            return DataService.GetReader(q.BuildSelectCommand());
        }

        /// <summary>
        /// Return a new Query object based on the underlying TableSchema.Table type of the record
        /// </summary>
        /// <returns></returns>
        public static Query Query()
        {
            new T();
            return new Query(table);
        }

        #endregion


        #region Command Builder

        /// <summary>
        /// Gets the select command used to retrieve the AbstractRecord object.
        /// </summary>
        /// <returns></returns>
        public QueryCommand GetSelectCommand()
        {
            return ActiveHelper<T>.GetSelectCommand(this);
        }

        #endregion


        #region Utility

        /// <summary>
        /// If this object has a logical delete column, this method will append in the required parameter to avoid returning
        /// deleted records
        /// </summary>
        /// <param name="q">The q.</param>
        internal static void CheckLogicalDelete(Query q)
        {
            q.CheckLogicalDelete();
        }

        /// <summary>
        /// Returns an ordered ListItemCollection for use with DropDowns, RadioButtonLists, and CheckBoxLists
        /// </summary>
        /// <returns></returns>
        public static ListItemCollection GetListItems()
        {
            // get the textColumn based on position
            // which should be the second column of the table
            string textColumn = BaseSchema.Descriptor.ColumnName;
            return GetListItems(textColumn);
        }

        /// <summary>
        /// Returns an ordered ListItemCollection for use with DropDowns, RadioButtonLists, and CheckBoxLists
        /// </summary>
        /// <param name="textColumn">The name of the column which should be used as the text value column</param>
        /// <returns></returns>
        public static ListItemCollection GetListItems(string textColumn)
        {
            ListItemCollection list = new ListItemCollection();
            string pkCol = BaseSchema.PrimaryKey.ColumnName;
            string textCol = BaseSchema.GetColumn(textColumn).ColumnName;

            // run a query retrieving the two columns
            Query q = new Query(BaseSchema)
                          {
                              SelectList = String.Concat(pkCol, ",",textCol),
                              OrderBy = OrderBy.Asc(textCol)
                          };

            using(IDataReader rdr = q.ExecuteReader())
            {
                while(rdr.Read())
                {
                    ListItem listItem = new ListItem(rdr[1].ToString(), rdr[0].ToString());
                    list.Add(listItem);
                }

                rdr.Close();
            }
            return list;
        }

        #endregion
    }
}