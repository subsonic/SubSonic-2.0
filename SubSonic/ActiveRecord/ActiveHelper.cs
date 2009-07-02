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
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the ActiveHelper&lt;T&gt; class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ActiveHelper<T> where T : RecordBase<T>, new()
    {
        /// <summary>
        /// Given an AbstractRecord object, returns a INSERT QueryCommand object used to generate SQL.
        /// </summary>
        /// <param name="item">The AbstractRecord object</param>
        /// <param name="userName">An optional username to be used if audit fields are present</param>
        /// <returns></returns>
        public static QueryCommand GetInsertCommand(RecordBase<T> item, string userName)
        {
            TableSchema.Table BaseSchema = item.GetSchema();
            Query q = new Query(BaseSchema)
                          {
                              QueryType = QueryType.Insert
                          };
            QueryCommand cmd = new QueryCommand(DataService.GetSql(q), BaseSchema.Provider.Name)
                                   {
                                       ProviderName = BaseSchema.Provider.Name
                                   };

            // loop the Columns and addin the params
            foreach(TableSchema.TableColumn column in BaseSchema.Columns)
            {
                if(!column.AutoIncrement && !column.IsReadOnly)
                {
                    object oVal;
                    bool insertValue = true;
                    if(Utility.MatchesOne(column.ColumnName, ReservedColumnName.CREATED_BY, ReservedColumnName.MODIFIED_BY))
                        oVal = userName;
                    else if(Utility.MatchesOne(column.ColumnName, ReservedColumnName.CREATED_ON, ReservedColumnName.MODIFIED_ON))
                        oVal = BaseSchema.Provider.Now;
                    else if(column.DataType == DbType.Guid)
                    {
                        if(!Utility.IsMatch(column.DefaultSetting, SqlSchemaVariable.DEFAULT))
                        {
                            oVal = item.GetColumnValue<Guid>(column.ColumnName);
                            bool isEmptyGuid = Utility.IsMatch(oVal.ToString(), Guid.Empty.ToString());

                            if(column.IsNullable && isEmptyGuid)
                                oVal = null;
                            else if(column.IsPrimaryKey && isEmptyGuid)
                                oVal = Guid.NewGuid();
                        }
                        else
                        {
                            oVal = null;
                            insertValue = false;
                        }
                    }
                    else
                    {
                        oVal = item.GetColumnValue<object>(column.ColumnName);

                        // if the value is a boolean, it can be read improperly so reset it to 0 or 1
                        if(oVal != null && column.DataType == DbType.Boolean)
                        {
                            if(Utility.IsMatch(oVal.ToString(), Boolean.FalseString))
                                oVal = 0;
                            else if(Utility.IsMatch(oVal.ToString(), Boolean.TrueString))
                                oVal = 1;
                        }
                    }

                    if(oVal == null)
                        oVal = DBNull.Value;
                    if(insertValue)
                        cmd.Parameters.Add(column.ParameterName, oVal, column.DataType);
                }
            }

            return cmd;
        }

        /// <summary>
        /// Given an AbstractRecord object, returns a UPDATE QueryCommand object used to generate SQL.
        /// </summary>
        /// <param name="item">The AbstractRecord object</param>
        /// <param name="userName">An optional username to be used if audit fields are present</param>
        /// <returns></returns>
        public static QueryCommand GetUpdateCommand(RecordBase<T> item, string userName)
        {
            TableSchema.Table BaseSchema = item.GetSchema();
            Query q = new Query(BaseSchema)
                          {
                              QueryType = QueryType.Update
                          };
            string sql = DataService.Providers[BaseSchema.Provider.Name].GetUpdateSql(q, item.DirtyColumns);
            QueryCommand cmd = new QueryCommand(sql, BaseSchema.Provider.Name)
                                   {
                                       ProviderName = BaseSchema.Provider.Name
                                   };
            bool foundModifiedBy = false;
            bool foundModifiedOn = false;

            // loop the Columns and addin the params
            foreach(TableSchema.TableColumn column in item.DirtyColumns)
            {
                if(!column.IsReadOnly)
                {
                    object oVal;
                    if (Utility.IsMatch(column.ColumnName, ReservedColumnName.MODIFIED_BY))
                    {
                        oVal = userName;
                        foundModifiedBy = true;
                    }
                    else if (Utility.IsMatch(column.ColumnName, ReservedColumnName.MODIFIED_ON))
                    {
                        oVal = BaseSchema.Provider.Now;
                        foundModifiedOn = true;
                    }
                    else if (column.DataType == DbType.Guid)
                    {
                        oVal = item.GetColumnValue<Guid>(column.ColumnName);
                        if (column.IsNullable && Utility.IsMatch(oVal.ToString(), Guid.Empty.ToString()))
                            oVal = null;
                    }
                    else
                        oVal = item.GetColumnValue<object>(column.ColumnName);

                    if(oVal == null)
                        oVal = DBNull.Value;
                    //escape this so we don't set the value to NULL
                    cmd.Parameters.Add(column.ParameterName, oVal, column.DataType);
                }
            }

            // if there are ModifiedOn and ModifiedBy, add them in as well
            if(BaseSchema.Columns.Contains(ReservedColumnName.MODIFIED_ON) && !foundModifiedOn)
            {
                string modOnParamName = String.Concat(BaseSchema.Provider.GetParameterPrefix(), ReservedColumnName.MODIFIED_ON);
                if(!cmd.Parameters.Contains(modOnParamName))
                    cmd.Parameters.Add(modOnParamName, BaseSchema.Provider.Now, DbType.DateTime);
            }

            if (BaseSchema.Columns.Contains(ReservedColumnName.MODIFIED_BY) && !foundModifiedBy)
            {
                string modByParamName = String.Concat(BaseSchema.Provider.GetParameterPrefix(), ReservedColumnName.MODIFIED_BY);
                if(!cmd.Parameters.Contains(modByParamName))
                    cmd.Parameters.Add(modByParamName, userName, DbType.String);
            }

            //if nothing has changed...
            if (cmd.Parameters.Count == 0)
                return null;

            // make sure to set the PKs
            foreach(TableSchema.TableColumn col in BaseSchema.PrimaryKeys)
                cmd.Parameters.Add(col.ParameterName, item.GetColumnValue<object>(col.ColumnName), col.DataType);
            return cmd;
        }

        /// <summary>
        /// Given an AbstractRecord object, returns a SELECT QueryCommand object used to generate SQL.
        /// </summary>
        /// <param name="item">The AbstractRecord object</param>
        /// <returns></returns>
        public static QueryCommand GetSelectCommand(RecordBase<T> item)
        {
            Query q = new Query(item.GetSchema())
                          {
                              QueryType = QueryType.Select
                          };
            QueryCommand cmd = DataService.BuildCommand(q);
            return cmd;
        }

        /// <summary>
        /// Deletes underlying persisted records with the specified column name/value combination.
        /// </summary>
        /// <param name="columnName">Name of the database column</param>
        /// <param name="oValue">The value to match for record deletes</param>
        /// <param name="userName">An optional username to be used if audit fields are present</param>
        /// <returns></returns>
        public static int Delete(string columnName, object oValue, string userName)
        {
            int iOut = 0;
            T item = new T();
            TableSchema.Table schema = item.GetSchema();
            bool containsDeleted = schema.Columns.Contains(ReservedColumnName.DELETED);
            bool containsIsDeleted = schema.Columns.Contains(ReservedColumnName.IS_DELETED);
            bool containsModifiedBy = schema.Columns.Contains(ReservedColumnName.MODIFIED_BY);
            bool containsModifiedOn = schema.Columns.Contains(ReservedColumnName.MODIFIED_ON);

            if(containsDeleted || containsIsDeleted)
            {
                // update the column and set deleted=true;
                Query qry = new Query(schema);

                if(containsDeleted)
                    qry.AddUpdateSetting(ReservedColumnName.DELETED, true);

                if(containsIsDeleted)
                    qry.AddUpdateSetting(ReservedColumnName.IS_DELETED, true);

                if(containsModifiedBy && !String.IsNullOrEmpty(userName))
                    qry.AddUpdateSetting(ReservedColumnName.MODIFIED_BY, userName);

                if(containsModifiedOn)
                    qry.AddUpdateSetting(ReservedColumnName.MODIFIED_ON, schema.Provider.Now);

                qry.AddWhere(columnName, oValue);
                qry.Execute();
            }
            else
                iOut = DestroyByParameter(columnName, oValue);
            return iOut;
        }

        /// <summary>
        /// Deletes the record in the table, even if it contains Deleted or IsDeleted flag columns
        /// </summary>
        /// <param name="columnName">The name of the column that whose value will be evaluated for deletion</param>
        /// <param name="oValue">The value that will be compared against columnName to determine deletion</param>
        /// <returns>Number of rows affected by the operation</returns>
        public static int DestroyByParameter(string columnName, object oValue)
        {
            T item = new T();
            Query q = new Query(item.GetSchema())
                          {
                              QueryType = QueryType.Delete
                          };
            q.AddWhere(columnName, oValue);
            QueryCommand cmd = DataService.BuildCommand(q);
            return DataService.ExecuteQuery(cmd);
        }

        /// <summary>
        /// Returns a DELETE QueryCommand object to delete the record with
        /// the primary key value matching the passed value
        /// </summary>
        /// <param name="keyID">The primary key record value to match for the delete</param>
        /// <returns></returns>
        public static QueryCommand GetDeleteCommand(object keyID)
        {
            T item = new T();
            Query q = new Query(item.GetSchema())
                          {
                              QueryType = QueryType.Delete
                          };
            q.AddWhere(q.Schema.PrimaryKey.ColumnName, keyID);

            return DataService.BuildCommand(q);
        }

        /// <summary>
        /// Returns a DELETE QueryCommand object to delete the records with
        /// matching passed column/value criteria
        /// </summary>
        /// <param name="columnName">Name of the column to match</param>
        /// <param name="oValue">Value of column to match</param>
        /// <returns></returns>
        public static QueryCommand GetDeleteCommand(string columnName, object oValue)
        {
            T item = new T();
            QueryCommand cmd;
            TableSchema.Table tbl = item.GetSchema();
            TableSchema.TableColumn colDeleted = tbl.GetColumn(ReservedColumnName.DELETED);
            TableSchema.TableColumn colIsDeleted = tbl.GetColumn(ReservedColumnName.IS_DELETED);

            if(colDeleted != null)
                cmd = new Update(tbl).Set(colDeleted).EqualTo(true).Where(columnName).IsEqualTo(oValue).BuildCommand();
            else if(colIsDeleted != null)
                cmd = new Update(tbl).Set(colIsDeleted).EqualTo(true).Where(columnName).IsEqualTo(oValue).BuildCommand();
            else
            {
                Query q = new Query(item.GetSchema())
                              {
                                  QueryType = QueryType.Delete
                              };
                q.AddWhere(columnName, oValue);
                cmd = DataService.BuildCommand(q);
            }

            return cmd;
        }
    }
}