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
using System.Runtime.Serialization;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class BindingListEx<T> : BindingList<T>
    {
        // Fix Serialization bug in BindingList<T>
        // http://www.gavaghan.org/blog/2007/07/17/fixing-bindinglist-deserialization/
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            List<T> items = new List<T>(Items);

            int index = 0;

            // call SetItem again on each item to re-establish event hookups
            foreach(T item in items)
            {
                // explicitly call the base version in case SetItem is overridden
                SetItem(index++, item);
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public void AddRange(IEnumerable<T> collection)
        {
            ((List<T>)Items).AddRange(collection);
        }

        /// <summary>
        /// Gets the list.
        /// </summary>
        /// <returns></returns>
        public List<T> GetList()
        {
            return (List<T>)Items;
        }


        #region Sorting

        private bool _isSorted;
        private ListSortDirection _sortDirection;

        [NonSerialized]
        private PropertyDescriptor _sortProperty;

        /// <summary>
        /// Gets a value indicating whether the list supports sorting.
        /// </summary>
        /// <value></value>
        /// <returns>true if the list supports sorting; otherwise, false. The default is false.</returns>
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the list is sorted.
        /// </summary>
        /// <value></value>
        /// <returns>true if the list is sorted; otherwise, false. The default is false.</returns>
        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        /// <summary>
        /// Gets the direction the list is sorted.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.ComponentModel.ListSortDirection"/> values. The default is <see cref="F:System.ComponentModel.ListSortDirection.Ascending"/>. </returns>
        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }

        /// <summary>
        /// Gets the property descriptor that is used for sorting the list if sorting is implemented in a derived class; otherwise, returns null.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.ComponentModel.PropertyDescriptor"/> used for sorting the list.</returns>
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortProperty; }
        }

        /// <summary>
        /// Applies the sort core.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="direction">The direction.</param>
        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            _sortDirection = direction;
            _sortProperty = property;

            // Get list to sort
            List<T> items = Items as List<T>;

            // Apply and set the sort, if items to sort
            if(items != null)
            {
                PropertyComparer<T> pc = new PropertyComparer<T>(property, direction);
                items.Sort(pc);
                _isSorted = true;
            }
            else
                _isSorted = false;

            // Let bound controls know they should refresh their views
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <summary>
        /// Removes any sort applied with <see cref="M:System.ComponentModel.BindingList`1.ApplySortCore(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)"/> if sorting is implemented in a derived class; otherwise, raises <see cref="T:System.NotSupportedException"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">Method is not overridden in a derived class. </exception>
        protected override void RemoveSortCore()
        {
            _isSorted = false;
        }

        #endregion


        #region Searching

        /// <summary>
        /// Gets a value indicating whether the list supports searching.
        /// </summary>
        /// <value></value>
        /// <returns>true if the list supports searching; otherwise, false. The default is false.</returns>
        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        /// <summary>
        /// Finds the core.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        protected override int FindCore(PropertyDescriptor property, object key)
        {
            // Specify search columns
            if(property == null)
                return -1;

            // Get list to search
            List<T> items = Items as List<T>;

            // Traverse list for value
            if(items != null)
            {
                foreach(T item in items)
                {
                    // Test column search value
                    object value = property.GetValue(item);

                    // If value is the search value, return the 
                    // index of the data item
                    if(key.Equals(value))
                        return IndexOf(item);
                }
            }

            return -1;
        }

        #endregion
    }
}