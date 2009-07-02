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

namespace SubSonic
{
    /// <summary>
    /// Summary for the ActiveController&lt;ItemType, ListType&gt; class
    /// </summary>
    /// <typeparam name="ItemType">The type of the tem type.</typeparam>
    /// <typeparam name="ListType">The type of the ist type.</typeparam>
    public class ActiveController<ItemType, ListType>
        where ItemType : AbstractRecord<ItemType>, new()
        where ListType : List<ItemType>, new()
    {
        /// <summary>
        /// Lists this instance.
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, true)]
        public static ListType List()
        {
            Query q = Query();
            return List(q);
        }

        /// <summary>
        /// Lists the specified q.
        /// </summary>
        /// <param name="q">The q.</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static ListType List(Query q)
        {
            ListType list = new ListType();
            using(IDataReader rdr = q.ExecuteReader())
            {
                while(rdr.Read())
                {
                    ItemType item = new ItemType();
                    item.Load(rdr);
                    list.Add(item);
                }

                rdr.Close();
            }
            return list;
        }

        /// <summary>
        /// Lists the specified IDataReader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static ListType List(IDataReader dataReader)
        {
            ListType list = new ListType();
            using(dataReader)
            {
                while(dataReader.Read())
                {
                    ItemType item = new ItemType();
                    item.Load(dataReader);
                    list.Add(item);
                }

                dataReader.Close();
            }

            return list;
        }

        /// <summary>
        /// Queries this instance.
        /// </summary>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static Query Query()
        {
            ItemType item = new ItemType();
            Query q = new Query(item.GetSchema().TableName);
            q.CheckLogicalDelete();
            return q;
        }

        /// <summary>
        /// Gets the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static ItemType Get(object key)
        {
            ItemType item = new ItemType();
            TableSchema.Table schema = item.GetSchema();

            return Get(schema.PrimaryKey.ColumnName, key);
        }

        /// <summary>
        /// Gets the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        [DataObjectMethod(DataObjectMethodType.Select, false)]
        public static ItemType Get(string columnName, object param)
        {
            ItemType item = new ItemType();
            TableSchema.Table schema = item.GetSchema();
            Query q = new Query(schema).WHERE(columnName, param);
            item.LoadAndCloseReader(q.ExecuteReader());
            return item;
        }

        /// <summary>
        /// Saves this object's state to the selected Database.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        public static void Save(ItemType item, string userName)
        {
            TableSchema.Table schema = item.GetSchema();
            QueryCommand cmd = item.IsNew ? GetInsertCommand(item, userName) : GetUpdateCommand(item, userName);

            if(cmd == null)
                return;

            // reset the Primary Key with the id passed back by the operation
            object pkVal = DataService.ExecuteScalar(cmd);

            if(pkVal != null)
            {
                if(pkVal.GetType() == typeof(decimal))
                    pkVal = Convert.ToInt32(pkVal);

                // set the primaryKey, only if an auto-increment
                if(schema.PrimaryKey.AutoIncrement || schema.PrimaryKey.DataType == DbType.Guid)
                {
                    try
                    {
                        item.SetColumnValue(schema.PrimaryKey.ColumnName, pkVal);
                    }
                    catch
                    {
                        // this will happen if there is no PK defined on a table. Catch this and notify
                        throw new Exception("No Primary Key is defined for this table. A primary key is required to use SubSonic");
                    }
                }
            }
        }

        /// <summary>
        /// Made Public for use with transactions
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static QueryCommand GetInsertCommand(ItemType item, string userName)
        {
            return ActiveHelper<ItemType>.GetInsertCommand(item, userName);
        }

        /// <summary>
        /// Gets the update command.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static QueryCommand GetUpdateCommand(ItemType item, string userName)
        {
            return ActiveHelper<ItemType>.GetUpdateCommand(item, userName);
        }

        /// <summary>
        /// Deletes the specified key ID.
        /// </summary>
        /// <param name="keyID">The key ID.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public static void Delete(object keyID)
        {
            ItemType item = new ItemType();
            ActiveHelper<ItemType>.Delete(item.GetSchema().PrimaryKey.ColumnName, keyID, String.Empty);
        }

        /// <summary>
        /// Deletes the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="param">The param.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public static void Delete(string columnName, object param)
        {
            new ItemType();
            ActiveHelper<ItemType>.Delete(columnName, param, String.Empty);
        }

        /// <summary>
        /// Destroys the specified key ID.
        /// </summary>
        /// <param name="keyID">The key ID.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public static void Destroy(object keyID)
        {
            ItemType item = new ItemType();
            Destroy(item.GetSchema().PrimaryKey.ColumnName, keyID);
        }

        /// <summary>
        /// Destroys the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="param">The param.</param>
        [DataObjectMethod(DataObjectMethodType.Delete, false)]
        public static void Destroy(string columnName, object param)
        {
            ActiveHelper<ItemType>.DestroyByParameter(columnName, param);
        }

        /// <summary>
        /// Deletes the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="oValue">The o value.</param>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public static int Delete(string columnName, object oValue, string userName)
        {
            return ActiveHelper<ItemType>.Delete(columnName, oValue, userName);
        }

        /// <summary>
        /// Gets the delete command.
        /// </summary>
        /// <param name="keyID">The key ID.</param>
        /// <returns></returns>
        public static QueryCommand GetDeleteCommand(object keyID)
        {
            return ActiveHelper<ItemType>.GetDeleteCommand(keyID);
        }

        /// <summary>
        /// Gets the delete command.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="oValue">The o value.</param>
        /// <returns></returns>
        public static QueryCommand GetDeleteCommand(string columnName, object oValue)
        {
            return ActiveHelper<ItemType>.GetDeleteCommand(columnName, oValue);
        }
    }
}