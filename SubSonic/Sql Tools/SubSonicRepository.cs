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
using System.Data.Common;

namespace SubSonic
{
    public class SubSonicRepository : ISubSonicRepository
    {
        private DataProvider _provider;

        public SubSonicRepository(DataProvider provider)
        {
            _provider = provider;
        }


        #region ISubSonicRepository Members

        public DataProvider Provider
        {
            get { return _provider; }
            set { _provider = value; }
        }

        public Select SelectAllColumnsFrom<T>() where T : RecordBase<T>, new()
        {
            return SubSonic.Select.AllColumnsFrom<T>();
        }

        public Select Select()
        {
            return new Select(_provider);
        }

        public Select Select(params string[] columns)
        {
            return new Select(_provider, columns);
        }

        public Select Select(params Aggregate[] aggregates)
        {
            return new Select(_provider, aggregates);
        }

        public Update Update<T>() where T : RecordBase<T>, new()
        {
            return new Update(new T().GetSchema());
        }

        public Insert Insert()
        {
            Insert i = new Insert();
            i.Provider = _provider;
            return i;
        }

        public Delete Delete()
        {
            return new Delete(_provider.Name);
        }

        public InlineQuery Query()
        {
            return new InlineQuery(_provider.Name);
        }

        public bool IsOnline() {
            bool result = false;

            using(DbConnection conn=this.Provider.CreateConnection()){
                try {
                    conn.Open();
                    result = true;
                } catch {
                    result = false;
                }
            }
            return true;
        }
        public bool IsOnline(string connectionString) {
            bool result = false;

            using (DbConnection conn = this.Provider.CreateConnection(connectionString)) {
                try {
                    conn.Open();
                    result = true;
                } catch {
                    result = false;
                }
            }
            return true;
        }

        public T Get<T>(object primaryKeyValue) where T : RepositoryRecord<T>, new()
        {
            T item = new T();
            TableSchema.Table tbl = item.GetSchema();
            string pkColumn = tbl.PrimaryKey.ColumnName;
            using(IDataReader rdr = SelectAllColumnsFrom<T>().Where(pkColumn).IsEqualTo(primaryKeyValue).ExecuteReader())
            {
                if (rdr.Read()) {
                    item.Load(rdr);
                } else {
                    item = null;
                }
                rdr.Close();
            }

            return item;
        }

        public T Get<T>(string columnName, object columnValue) where T : RepositoryRecord<T>, new()
        {
            T item = new T();
            using (IDataReader rdr = SelectAllColumnsFrom<T>().Where(columnName).IsEqualTo(columnValue).ExecuteReader())
            {
                if (rdr.Read()) {
                    item.Load(rdr);
                } else {
                    item = null;
                }
                rdr.Close();
            }

            return item;
        }

        public void Delete<T>(string columnName, object columnValue) where T : RepositoryRecord<T>, new()
        {
            T item = new T();
            TableSchema.Table tbl = item.GetSchema();
            if(tbl.GetColumn(ReservedColumnName.DELETED) != null)
            {
                //create an update command
                new Update(tbl).Set(ReservedColumnName.DELETED).EqualTo(true).Where(columnName).IsEqualTo(columnValue).Execute();
            }
            else if(tbl.GetColumn(ReservedColumnName.IS_DELETED) != null)
                new Update(tbl).Set(ReservedColumnName.IS_DELETED).EqualTo(true).Where(columnName).IsEqualTo(columnValue).Execute();
            else
            {
                QueryCommand del = ActiveHelper<T>.GetDeleteCommand(columnName, columnValue);
                DataService.ExecuteQuery(del);
            }
        }

        public void Delete<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new()
        {
            TableSchema.Table tbl = item.GetSchema();
            string pkColumn = tbl.PrimaryKey.ColumnName;
            object pkValue = item.GetColumnValue(pkColumn);
            Delete<T>(pkColumn, pkValue);
        }

		public void DeleteByKey<T>(object itemId) where T : RepositoryRecord<T>, new() {
			T item = new T();
			TableSchema.Table tbl = item.GetSchema();
			string pkColumn = tbl.PrimaryKey.ColumnName;
			Delete<T>(pkColumn, itemId);
		}

		public void Destroy<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new()
        {
            TableSchema.Table tbl = item.GetSchema();
            string pkColumn = tbl.PrimaryKey.ColumnName;
            object pkValue = item.GetColumnValue(pkColumn);
            new Destroy().From(tbl).Where(pkColumn).IsEqualTo(pkValue).Execute();
        }

        public void Destroy<T>(string columnName, object value) where T : RepositoryRecord<T>, new()
        {
            T item = new T();
            TableSchema.Table tbl = item.GetSchema();
            new Destroy().From(tbl).Where(columnName).IsEqualTo(value).Execute();
        }

		public void DestroyByKey<T>(object itemId) where T : RepositoryRecord<T>, new() {
			T item = new T();
			TableSchema.Table tbl = item.GetSchema();
			string pkColumn = tbl.PrimaryKey.ColumnName;
			Destroy<T>(pkColumn, itemId);
		}

		public int Save<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new()
        {
            return Save(item, String.Empty);
        }

        public int Save<T>(RepositoryRecord<T> item, string userName) where T : RepositoryRecord<T>, new()
        {
            //validation
            item.ValidateColumnSettings();
            if(item.HasErrors())
            {
                StringBuilder sb = new StringBuilder();
                List<string> errors = item.GetErrors();
                foreach(string error in errors)
                    sb.Append(error + Environment.NewLine);

                throw new SqlQueryException("There are errors saving this item: " + sb);
            }

            int result = item.IsNew ? Insert(item, userName) : Update(item, userName);
            return result;
        }

        public int SaveAll<ItemType, ListType>(RepositoryList<ItemType, ListType> itemList, string userName)
            where ItemType : RepositoryRecord<ItemType>, new()
            where ListType : RepositoryList<ItemType, ListType>, new()
        {
            int lCount = 0;
            foreach(ItemType item in itemList)
                lCount += Save(item, userName);
            return lCount;
        }

        public int SaveAll<ItemType, ListType>(RepositoryList<ItemType, ListType> itemList)
            where ItemType : RepositoryRecord<ItemType>, new()
            where ListType : RepositoryList<ItemType, ListType>, new()
        {
            return SaveAll(itemList, string.Empty);
        }

        public int Update<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new()
        {
            return Update(item, String.Empty);
        }

        public int Update<T>(RepositoryRecord<T> item, string userName) where T : RepositoryRecord<T>, new()
        {
            if(userName == null)
                throw new ArgumentNullException("userName");

            int result = 0;
            if(item.IsDirty)
            {
                QueryCommand cmd = ActiveHelper<T>.GetUpdateCommand(item, userName);
                result = DataService.ExecuteQuery(cmd);
            }
            item.DirtyColumns.Clear();
            item.MarkOld();
            item.MarkClean();
            return result;
        }

        public int Insert<T>(RepositoryRecord<T> item) where T : RepositoryRecord<T>, new()
        {
            return Insert(item, String.Empty);
        }

        public int Insert<T>(RepositoryRecord<T> item, string userName) where T : RepositoryRecord<T>, new()
        {
            if(userName == null)
                throw new ArgumentNullException("userName");

            int result = 0;
            QueryCommand cmd = ActiveHelper<T>.GetInsertCommand(item, userName);
            TableSchema.Table schema = item.GetSchema();
            if(schema.PrimaryKey != null)
            {
                if (schema.PrimaryKey.AutoIncrement || schema.PrimaryKey.DataType == DbType.Guid)
                {
                    object qResult = DataService.ExecuteScalar(cmd);
                    item.SetColumnValue(schema.PrimaryKey.ColumnName, qResult);
                    if(qResult != null)
                        int.TryParse(qResult.ToString(), out result);
                }
                else
                    result = DataService.ExecuteQuery(cmd);
            }
            else
                result = DataService.ExecuteQuery(cmd);

            item.DirtyColumns.Clear();
            item.MarkOld();
            item.MarkClean();
            return result;
        }

        #endregion
    }
}