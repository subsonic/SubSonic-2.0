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
    /// Internal class supporting the sorting of AbstractList objects.
    /// </summary>
    /// <typeparam name="ItemType">The ActiveRecord type of the collection members</typeparam>
    [Serializable]
    internal class ListComparer<ItemType> : Comparer<ItemType> where ItemType : RecordBase<ItemType>, new()
    {
        private bool ascending;
        private string columnName;
        private DbType dbType = DbType.String;

        /// <summary>
        /// Gets or sets the name of the column used to sort the collection.
        /// </summary>
        /// <value>The name of the column.</value>
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the sort to be performed is ascending.
        /// </summary>
        /// <value><c>true</c> if ascending; otherwise, <c>false</c>.</value>
        public bool Ascending
        {
            get { return ascending; }
            set { ascending = value; }
        }

        /// <summary>
        /// Gets or sets the data type of the sort column, determining the sorting logic employed.
        /// </summary>
        /// <value>System.Data.DBType</value>
        public DbType DBType
        {
            get { return dbType; }
            set { dbType = value; }
        }

        /// <summary>
        /// Compares values of individual items in the collection for list sorting. Sorting behavior is dependent on item data type.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value Condition Less than zero <paramref name="x"/> is less than <paramref name="y"/>.Zero <paramref name="x"/> equals <paramref name="y"/>.Greater than zero <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentException">Type <paramref name="T"/> does not implement either the <see cref="T:System.IComparable`1"/> generic interface or the <see cref="T:System.IComparable"/> interface.</exception>
        public override int Compare(ItemType x, ItemType y)
        {
            object xVal = x.GetColumnValue<object>(columnName);
            object yVal = y.GetColumnValue<object>(columnName);
            int result;

            if(dbType == DbType.String || dbType == DbType.Guid)
            {
                string sX = (null == xVal) ? String.Empty : xVal.ToString();
                string sY = (null == yVal) ? String.Empty : yVal.ToString();
                result = sX.CompareTo(sY);
            }
            else if(dbType == DbType.DateTime)
            {
                DateTime dX = Convert.ToDateTime(xVal);
                DateTime dY = Convert.ToDateTime(yVal);
                result = dX.CompareTo(dY);
            }
            else if(dbType == DbType.Boolean)
            {
                bool bX = Convert.ToBoolean(xVal);
                bool bY = Convert.ToBoolean(yVal);
                result = bX.CompareTo(bY);
            }
            else
            {
                double dX = Convert.ToDouble(xVal);
                double dY = Convert.ToDouble(yVal);
                result = dX.CompareTo(dY);
            }

            if(!ascending)
                result *= -1;

            return result;
        }
    }

    /// <summary>
    /// Abstract class for handling collections of AbstractRecord objects
    /// </summary>
    /// <typeparam name="ItemType">The ActiveRecord or ReadOnlyRecord derived type</typeparam>
    /// <typeparam name="ListType">The ActiveList or ReadOnlyList derived type</typeparam>
    [Serializable]
    public abstract class AbstractList<ItemType, ListType> : BindingListEx<ItemType>, ITypedList, IAbstractList
        where ItemType : RecordBase<ItemType>, new()
        where ListType : AbstractList<ItemType, ListType>, new()
    {
        private string _providerName;


        #region IAbstractList Members

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
        /// Standard sort via ApplySortCore
        /// </summary>
        /// <param name="columnName">Name of the column to sort by</param>
        /// <param name="ascending">If set to <c>true</c> and ascending sort is performed. If set to <c>false</c>, the sort is descending</param>
        public void Sort(string columnName, bool ascending)
        {
            if(!String.IsNullOrEmpty(columnName))
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(new ItemType());
                PropertyDescriptor myProperty = properties.Find(columnName, false);

                ListSortDirection sort = ListSortDirection.Descending;
                if(ascending)
                    sort = ListSortDirection.Ascending;

                ApplySortCore(myProperty, sort);
            }
        }

        /// <summary>
        /// Loads the collection and leaves the IDataReader open.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        public void Load(IDataReader dataReader)
        {
            while(dataReader.Read())
            {
                ItemType item = new ItemType();
                item.Load(dataReader);
                Add(item);
            }
        }

        /// <summary>
        /// Loads the collection by iterating through the rows in the supplied DataTable.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        public void Load(DataTable dataTable)
        {
            foreach(DataRow dr in dataTable.Rows)
            {
                ItemType item = new ItemType();
                item.Load(dr);
                Add(item);
            }
        }

        /// <summary>
        /// Loads the collection and closes the IDataReader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        public void LoadAndCloseReader(IDataReader dataReader)
        {
            using(dataReader)
            {
                Load(dataReader);
                dataReader.Close();
            }
        }

        #endregion


        #region ITypedList Members

        /// <summary>
        /// Returns the name of the list.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor"/> objects, for which the list name is returned. This can be null.</param>
        /// <returns>The name of the list.</returns>
        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return String.Empty;
        }

        /// <summary>
        /// Returns the <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties on each item used to bind data.
        /// </summary>
        /// <param name="listAccessors">An array of <see cref="T:System.ComponentModel.PropertyDescriptor"/> objects to find in the collection as bindable. This can be null.</param>
        /// <returns>
        /// The <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> that represents the properties on each item used to bind data.
        /// </returns>
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            // if((listAccessors == null) || listAccessors.Length == 0)
            //    return GetPropertyDescriptors(typeof(ItemType));
            // return null;

            // The code above breaks some bound controls.  Still looking into why.
            // maybe just returning a PDCollection containing the contents of the array
            // is the way to go.  This works for now.  -kh
            return GetPropertyDescriptors(typeof(ItemType));
        }

        #endregion


        /// <summary>
        /// Finds the first record matching the key
        /// </summary>
        /// <param name="columnName">Name of the column to find in</param>
        /// <param name="key">Key to search for</param>
        /// <returns></returns>
        public int Find(string columnName, object key)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(new ItemType());
            PropertyDescriptor myProperty = properties.Find(columnName, false);

            return FindCore(myProperty, key);
        }

        /// <summary>
        /// Gets the property descriptors. Used to evaluate the suitability 
        /// of properties on the passed type for data binding
        /// </summary>
        /// <param name="typeOfObject">The type of object to return property descriptors for</param>
        /// <returns></returns>
        private static PropertyDescriptorCollection GetPropertyDescriptors(Type typeOfObject)
        {
            // requires [Bindable] attribute in generated class properties
            Attribute[] attrs = new Attribute[] {new BindableAttribute(true)};

            PropertyDescriptorCollection typePropertiesCollection = TypeDescriptor.GetProperties(typeOfObject, attrs);

            return typePropertiesCollection;
        }


        #region Utility

        /// <summary>
        /// Returns the collection's items in a DataTable.
        /// </summary>
        /// <returns></returns>
        public DataTable ToDataTable()
        {
            DataTable tblOut = new DataTable();

            // create the columns
            ItemType schema = new ItemType();

            tblOut.TableName = schema.TableName;

            // get the schema from the object
            TableSchema.TableColumnSettingCollection settings = schema.GetColumnSettings();

            // add the columns
            foreach(TableSchema.TableColumnSetting setting in settings)
            {
                DataColumn col = new DataColumn(setting.ColumnName);
                TableSchema.TableColumn tableColumn = schema.GetSchema().GetColumn(setting.ColumnName);
                if(tableColumn != null)
                {
                    col.DataType = tableColumn.GetPropertyType();
                    col.Caption = tableColumn.DisplayName;
                    col.AllowDBNull = tableColumn.IsNullable;
                }

                tblOut.Columns.Add(col);
            }

            // set the values
            foreach(ItemType item in this)
                item.CopyTo(tblOut);

            return tblOut;
        }

        /// <summary>
        /// Checks the logical delete, suppressing results flagged as deleted as determined by
        /// the existence of columns named "deleted" or "isdeleted".
        /// </summary>
        /// <param name="q">The query to be to perform the logical delete on.</param>
        protected static void CheckLogicalDelete(Query q)
        {
            q.CheckLogicalDelete();
        }

        /// <summary>
        /// Populates the existing collection by cloning each item from the passed list.
        /// </summary>
        /// <param name="copyInstance">The copy instance.</param>
        public void CopyFrom(AbstractList<ItemType, ListType> copyInstance)
        {
            foreach(ItemType item in copyInstance)
            {
                ItemType newItem = item.Clone();
                Add(newItem);
            }
        }

        /// <summary>
        /// Populates the passed collection by cloning each item from the existing list.
        /// </summary>
        /// <param name="copyInstance">The copy instance.</param>
        public void CopyTo(AbstractList<ItemType, ListType> copyInstance)
        {
            foreach(ItemType item in this)
            {
                ItemType newItem = item.Clone();
                copyInstance.Add(newItem);
            }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public ListType Clone()
        {
            ListType coll = new ListType();
            foreach(ItemType item in this)
            {
                ItemType newItem = item.Clone();
                coll.Add(newItem);
            }

            return coll;
        }

        #endregion


        #region WHERE and ORDER BY

        protected List<BetweenAnd> betweens = new List<BetweenAnd>();
        protected OrderByCollection orderByCollection = new OrderByCollection();

        /// <summary>
        /// 
        /// </summary>
        protected List<Where> wheres = new List<Where>();

        /// <summary>
        /// Marks the collection for an ascending sort by the passed column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public ListType OrderByAsc(string columnName)
        {
            orderByCollection.Add(OrderBy.Asc(columnName));
            return this as ListType;
        }

        /// <summary>
        /// Marks the collection for an descending sort by the passed column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public ListType OrderByDesc(string columnName)
        {
            orderByCollection.Add(OrderBy.Desc(columnName));
            return this as ListType;
        }

        /// <summary>
        /// Adds the passed Where clause as a collection load condition.
        /// </summary>
        /// <param name="where">The where.</param>
        /// <returns></returns>
        public ListType Where(Where where)
        {
            wheres.Add(where);
            return this as ListType;
        }

        /// <summary>
        /// Adds the passed column and value as a Where clause for collection loading.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ListType Where(string columnName, object value)
        {
            if(value != DBNull.Value && value != null)
                return Where(columnName, Comparison.Equals, value);
            return Where(columnName, Comparison.Is, DBNull.Value);
        }

        /// <summary>
        /// Adds the passed column, comparision type, and value as a Where clause for collection loading.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="comp">The comp.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public ListType Where(string columnName, Comparison comp, object value)
        {
            Where where = new Where();
            where.ColumnName = columnName;
            where.Comparison = comp;
            where.ParameterValue = value;
            Where(where);
            return this as ListType;
        }

        /// <summary>
        /// Adds a date specific Where condition, limiting results to date values between the passed dates
        /// on the specfied column
        /// </summary>
        /// <param name="columnName">Name of the date column to evaluate for date matches</param>
        /// <param name="dateStart">Lower date boundary for inclusion in the collection</param>
        /// <param name="dateEnd">Upper date boundary for inclusion in the collection</param>
        /// <returns></returns>
        public ListType BetweenAnd(string columnName, DateTime dateStart, DateTime dateEnd)
        {
            BetweenAnd between = new BetweenAnd();
            between.ColumnName = columnName;
            between.StartDate = dateStart;
            between.EndDate = dateEnd;
            between.StartParameterName = "start" + columnName;
            between.EndParameterName = "end" + columnName;
            betweens.Add(between);
            return this as ListType;
        }

        /// <summary>
        /// Loads the collection using any specified conditional operators.
        /// </summary>
        /// <returns></returns>
        public ListType Load()
        {
            Query qry = new Query(new ItemType().GetSchema());
            CheckLogicalDelete(qry);

            foreach(Where where in wheres)
                qry.AddWhere(where);

            foreach(BetweenAnd between in betweens)
                qry.AddBetweenAnd(between);

            qry.OrderByCollection = orderByCollection;

            using(IDataReader rdr = qry.ExecuteReader())
            {
                LoadAndCloseReader(rdr);
                return this as ListType;
            }
        }

        #endregion
    }
}