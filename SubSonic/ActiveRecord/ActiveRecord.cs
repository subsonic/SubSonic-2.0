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

namespace SubSonic
{
    /// <summary>
    /// Summary for the ActiveRecord&lt;T&gt; class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class ActiveRecord<T> : ReadOnlyRecord<T> where T : ReadOnlyRecord<T>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveRecord&lt;T&gt;"/> class.
        /// </summary>
        protected ActiveRecord()
        {
            MarkNew();
        }


        #region CommandMethods

        /// <summary>
        /// Gets the provider specific (SQL Server, Oracle, etc.) SQL parameter prefix.
        /// </summary>
        /// <value>The parameter prefix.</value>
        protected static string ParameterPrefix
        {
            get { return BaseSchema.Provider.GetParameterPrefix(); }
        }

        /// <summary>
        /// Returns a INSERT QueryCommand object used to generate SQL.
        /// </summary>
        /// <param name="userName">An optional username to be used if audit fields are present</param>
        /// <returns></returns>
        public QueryCommand GetInsertCommand(string userName)
        {
            return ActiveHelper<T>.GetInsertCommand(this, userName);
        }

        /// <summary>
        /// Returns a UPDATE QueryCommand object used to generate SQL.
        /// </summary>
        /// <param name="userName">An optional username to be used if audit fields are present</param>
        /// <returns></returns>
        public QueryCommand GetUpdateCommand(string userName)
        {
            return ActiveHelper<T>.GetUpdateCommand(this, userName);
        }

        /// <summary>
        /// Returns a DELETE QueryCommand object to delete the record with
        /// the primary key value matching the passed value
        /// </summary>
        /// <param name="keyID">The primary key record value to match for the delete</param>
        /// <returns></returns>
        public static QueryCommand GetDeleteCommand(object keyID)
        {
            return ActiveHelper<T>.GetDeleteCommand(keyID);
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
            return ActiveHelper<T>.GetDeleteCommand(columnName, oValue);
        }

        #endregion


        #region Persistence

        /// <summary>
        /// Executes before any operations occur within ActiveRecord Save() 
        /// </summary>
        protected virtual void BeforeValidate() {}

        /// <summary>
        /// Executes on existing records after validation and before the update command has been generated.
        /// </summary>
        protected virtual void BeforeUpdate() {}

        /// <summary>
        /// Executes on existing records after validation and before the insert command has been generated.
        /// </summary>
        protected virtual void BeforeInsert() {}

        /// <summary>
        /// Executes after all steps have been performed for a successful ActiveRecord Save()
        /// </summary>
        protected virtual void AfterCommit() {}

        [Obsolete("Deprecated: Use BeforeInsert() and/or BeforeUpdate() instead.")]
        protected virtual void PreUpdate() {}

        [Obsolete("Deprecated: Use AfterCommit() instead.")]
        protected virtual void PostUpdate() {}

        /// <summary>
        /// Saves this object's state to the selected Database.
        /// </summary>
        public void Save()
        {
            Save(String.Empty);
        }

        /// <summary>
        /// Saves this object's state to the selected Database.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public void Save(int userID)
        {
            Save(userID.ToString());
        }

        /// <summary>
        /// Saves this object's state to the selected Database.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public void Save(Guid userID)
        {
            string sUserID = userID.ToString();
            Save(sUserID);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            ValidateColumnSettings();
            return Errors.Count == 0;
        }

        /// <summary>
        /// Saves this object's state to the selected Database.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        public void Save(string userName)
        {
            bool isValid = true;

            if(ValidateWhenSaving)
            {
                BeforeValidate();

                isValid = Validate();
            }

            if(isValid)
            {
                if(IsNew)
                    BeforeInsert();
                else if(IsDirty)
                    BeforeUpdate();

                QueryCommand cmd = GetSaveCommand(userName);
                if(cmd == null)
                    return;

                // reset the Primary Key with the id passed back by the operation
                object pkVal = DataService.ExecuteScalar(cmd);

                // clear out the DirtyColumns
                DirtyColumns.Clear();

                if(pkVal != null)
                {
                    if(pkVal.GetType() == typeof(decimal))
                        pkVal = Convert.ToInt32(pkVal);

                    // set the primaryKey, only if an auto-increment
                    if(BaseSchema.PrimaryKey.AutoIncrement || BaseSchema.PrimaryKey.DataType == DbType.Guid)
                    {
                        try
                        {
							pkVal = Convert.ChangeType(pkVal, BaseSchema.PrimaryKey.GetPropertyType());
                            SetPrimaryKey(pkVal);
                        }
                        catch
                        {
                            // this will happen if there is no PK defined on a table. Catch this and notify
                            throw new Exception("No Primary Key is defined for this table. A primary key is required to use SubSonic");
                        }
                    }
                }

                // set this object as old
                MarkOld();
                MarkClean();
                AfterCommit();
            }
            else
            {
                // throw an Exception
                string notification = String.Empty;
                foreach(string message in Errors)
                    notification += message + Environment.NewLine;

                throw new Exception("Can't save: " + notification);
            }
        }

        /// <summary>
        /// Returns a QueryCommand object used to persist the instance to the underlying database.
        /// </summary>
        /// <returns></returns>
        public QueryCommand GetSaveCommand()
        {
            return GetSaveCommand(String.Empty);
        }

        /// <summary>
        /// Returns a QueryCommand object used to persist the instance to the underlying database.
        /// </summary>
        /// <param name="userName">An optional username to be used if audit fields are present</param>
        /// <returns></returns>
        public QueryCommand GetSaveCommand(string userName)
        {
            if(IsNew)
                return GetInsertCommand(userName);

            if(IsDirty)
                return GetUpdateCommand(userName);

            return null;
        }

        /// <summary>
        /// If the record contains Deleted or IsDeleted flag columns, sets them to true. If not, invokes Destroy()
        /// </summary>
        /// <param name="keyID">The key ID.</param>
        /// <returns>Number of rows affected by the operation</returns>
        public static int Delete(object keyID)
        {
            return DeleteByParameter(BaseSchema.PrimaryKey.ColumnName, keyID, null);
        }

        /// <summary>
        /// If the record contains Deleted or IsDeleted flag columns, sets them to true. If not, invokes Destroy()
        /// </summary>
        /// <param name="columnName">The name of the column that whose value will be evaluated for deletion</param>
        /// <param name="oValue">The value that will be compared against columnName to determine deletion</param>
        /// <returns>Number of rows affected by the operation</returns>
        public static int Delete(string columnName, object oValue)
        {
            return DeleteByParameter(columnName, oValue, null);
        }

        /// <summary>
        /// If the record contains Deleted or IsDeleted flag columns, sets them to true. If not, invokes Destroy()
        /// </summary>
        /// <param name="columnName">The name of the column that whose value will be evaluated for deletion</param>
        /// <param name="oValue">The value that will be compared against columnName to determine deletion</param>
        /// <param name="userName">The userName that the record will be updated with. Only relevant if the record contains Deleted or IsDeleted properties</param>
        /// <returns>Number of rows affected by the operation</returns>
        public static int Delete(string columnName, object oValue, string userName)
        {
            return DeleteByParameter(columnName, oValue, userName);
        }

        /// <summary>
        /// If the record contains Deleted or IsDeleted flag columns, sets them to true. If not, invokes Destroy()
        /// </summary>
        /// <param name="columnName">The name of the column that whose value will be evaluated for deletion</param>
        /// <param name="oValue">The value that will be compared against columnName to determine deletion</param>
        /// <param name="userName">The userName that the record will be updated with. Only relevant if the record contains Deleted or IsDeleted properties</param>
        /// <returns>Number of rows affected by the operation</returns>
        private static int DeleteByParameter(string columnName, object oValue, string userName)
        {
            return ActiveHelper<T>.Delete(columnName, oValue, userName);
        }

        /// <summary>
        /// Deletes the record in the table, even if it contains Deleted or IsDeleted flag columns
        /// </summary>
        /// <param name="keyID">The key ID.</param>
        /// <returns>Number of rows affected by the operation</returns>
        public static int Destroy(object keyID)
        {
            return ActiveHelper<T>.DestroyByParameter(BaseSchema.PrimaryKey.ColumnName, keyID);
        }

        /// <summary>
        /// Deletes the record in the table, even if it contains Deleted or IsDeleted flag columns
        /// </summary>
        /// <param name="columnName">The name of the column that whose value will be evaluated for deletion</param>
        /// <param name="oValue">The value that will be compared against columnName to determine deletion</param>
        /// <returns>Number of rows affected by the operation</returns>
        public static int Destroy(string columnName, object oValue)
        {
            return ActiveHelper<T>.DestroyByParameter(columnName, oValue);
        }

        /// <summary>
        /// Deletes the record in the table, even if it contains Deleted or IsDeleted flag columns
        /// </summary>
        /// <param name="columnName">The name of the column that whose value will be evaluated for deletion</param>
        /// <param name="oValue">The value that will be compared against columnName to determine deletion</param>
        /// <returns>Number of rows affected by the operation</returns>
        private static int DestroyByParameter(string columnName, object oValue)
        {
            return ActiveHelper<T>.DestroyByParameter(columnName, oValue);
        }

        #endregion


        /// <summary>
        /// Gets the column value.
        /// </summary>
        /// <typeparam name="CT">The type of the T.</typeparam>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public new CT GetColumnValue<CT>(string columnName)
        {
            return base.GetColumnValue<CT>(columnName);
        }

        /// <summary>
        /// Gets the column value.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public object GetColumnValue(string columnName)
        {
            return GetColumnValue<object>(columnName);
        }
    }
}