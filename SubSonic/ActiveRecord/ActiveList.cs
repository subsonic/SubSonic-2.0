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

namespace SubSonic
{
    /// <summary>
    /// Summary for the ActiveList&lt;ItemType, ListType&gt; class
    /// </summary>
    /// <typeparam name="ItemType">The type of the tem type.</typeparam>
    /// <typeparam name="ListType">The type of the ist type.</typeparam>
    [Serializable]
    public class ActiveList<ItemType, ListType> : AbstractList<ItemType, ListType>
        where ItemType : ActiveRecord<ItemType>, new()
        where ListType : AbstractList<ItemType, ListType>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveList&lt;ItemType, ListType&gt;"/> class.
        /// </summary>
        public ActiveList() {}

        /// <summary>
        /// Creates and loads the collection, leaving the IDataReader open.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        public ActiveList(IDataReader dataReader)
        {
            Load(dataReader);
        }

        /// <summary>
        /// Creates and loads the collection, with option to set IDataReader close behavior.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="closeReader">if set to <c>true</c> [close reader].</param>
        public ActiveList(IDataReader dataReader, bool closeReader)
        {
            if(closeReader)
            {
                using(dataReader)
                {
                    Load(dataReader);
                    dataReader.Close();
                }
            }
            else
                Load(dataReader);
        }

        /// <summary>
        /// Finds the specified primary key value.
        /// </summary>
        /// <param name="primaryKeyValue">The primary key value.</param>
        /// <returns></returns>
        public ActiveRecord<ItemType> Find(object primaryKeyValue)
        {
            ActiveRecord<ItemType> result = null;
            foreach(ItemType item in this)
            {
                if(item.GetPrimaryKeyValue().Equals(primaryKeyValue))
                {
                    result = item;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Saves all the records in a collection to the DB
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        public void SaveAll(string userName)
        {
            ProcessDeletedItems();
            foreach(ItemType item in this)
                item.Save(userName);
        }

        /// <summary>
        /// Saves all the records in a collection to the DB
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public void SaveAll(Guid userID)
        {
            ProcessDeletedItems();
            foreach(ItemType item in this)
                item.Save(userID);
        }

        /// <summary>
        /// Saves all the records in a collection to the DB
        /// </summary>
        public void SaveAll()
        {
            ProcessDeletedItems();
            foreach(ItemType item in this)
                item.Save();
        }

        /// <summary>
        /// Uses a transaction to quickly persists all the records in a collection to the DB
        /// The primary keys of any new records are not updated in the collection
        /// You must reload the collection to get the latest data
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        public void BatchSave(string userName)
        {
            QueryCommandCollection commands = new QueryCommandCollection();
            commands.AddRange(GetSaveCommands(userName));
            commands.AddRange(GetDeleteCommands());

            DataService.ExecuteTransaction(commands);
        }

        /// <summary>
        /// Gets the save commands.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public QueryCommandCollection GetSaveCommands(string userName)
        {
            QueryCommandCollection commands = new QueryCommandCollection();
            foreach(ItemType item in this)
            {
                QueryCommand cmd = item.GetSaveCommand(userName);
                if(cmd != null)
                    commands.Add(cmd);
            }

            return commands;
        }

        /// <summary>
        /// Batches the save.
        /// </summary>
        public void BatchSave()
        {
            BatchSave(String.Empty);
        }

        /// <summary>
        /// Batches the save.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public void BatchSave(Guid userID)
        {
            BatchSave(userID.ToString());
        }

        /// <summary>
        /// Batches the save.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        public void BatchSave(int userID)
        {
            BatchSave(userID.ToString());
        }

        /// <summary>
        /// Saves all the records in a collection to the DB
        /// </summary>
        /// <param name="userId">The user id.</param>
        public void SaveAll(int userId)
        {
            foreach(ItemType item in this)
                item.Save(userId);
        }


        #region Deferred Delete Support

        private readonly List<ItemType> _deleteList = new List<ItemType>();

        /// <summary>
        /// Provides out-of-the box support delete using windows forms databinding.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        /// <exception cref="T:System.NotSupportedException">You are removing a newly added item and <see cref="P:System.ComponentModel.IBindingList.AllowRemove"/> is set to false. </exception>
        protected override void RemoveItem(int index)
        {
            _deleteList.Add(this[index]);
            base.RemoveItem(index);
        }

        /// <summary>
        /// Deletes records in the deleteList
        /// </summary>
        public void ProcessDeletedItems()
        {
            foreach(ItemType item in _deleteList)
                ActiveRecord<ItemType>.Delete(item.GetPrimaryKeyValue());

            _deleteList.Clear();
        }

        /// <summary>
        /// Uses a transaction to quickly delete all the records 
        /// marked for deletion
        /// </summary>
        public void BatchDelete()
        {
            QueryCommandCollection commands = GetDeleteCommands();

            DataService.ExecuteTransaction(commands);

            _deleteList.Clear();
        }

        /// <summary>
        /// Gets the delete commands.
        /// </summary>
        /// <returns></returns>
        public QueryCommandCollection GetDeleteCommands()
        {
            QueryCommandCollection commands = new QueryCommandCollection();
            foreach(ItemType item in _deleteList)
            {
                QueryCommand dcmd = ActiveRecord<ItemType>.GetDeleteCommand(item.GetPrimaryKeyValue());
                if(dcmd != null)
                    commands.Add(dcmd);
            }

            return commands;
        }

        #endregion
    }
}