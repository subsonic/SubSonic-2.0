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
using System.Reflection;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertyComparer<T> : IComparer<T>
    {
        //// The following code contains code implemented by Rockford Lhotka:
        //// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnadvnet/html/vbnet01272004.asp

        private readonly ListSortDirection _direction;

        [NonSerialized]
        private readonly PropertyDescriptor _property;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="direction">The direction.</param>
        public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
        {
            _property = property;
            _direction = direction;
        }


        #region IComparer<T>

        /// <summary>
        /// Compares the specified x word.
        /// </summary>
        /// <param name="xWord">The x word.</param>
        /// <param name="yWord">The y word.</param>
        /// <returns></returns>
        public int Compare(T xWord, T yWord)
        {
            // Get property values
            object xValue = GetPropertyValue(xWord, _property.Name);
            object yValue = GetPropertyValue(yWord, _property.Name);

            // Determine sort order
            return _direction == ListSortDirection.Ascending ? CompareAscending(xValue, yValue) : CompareDescending(xValue, yValue);
        }

        /// <summary>
        /// Equalses the specified x word.
        /// </summary>
        /// <param name="xWord">The x word.</param>
        /// <param name="yWord">The y word.</param>
        /// <returns></returns>
        public bool Equals(T xWord, T yWord)
        {
            return xWord.Equals(yWord);
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }

        #endregion


        /// <summary>
        /// Compare two property values of any type
        /// </summary>
        /// <param name="xValue">The x value.</param>
        /// <param name="yValue">The y value.</param>
        /// <returns></returns>
        private static int CompareAscending(object xValue, object yValue)
        {
            int result;

            if(xValue == null && yValue == null)
                return 0;
            if(xValue == null)
                return -1;
            if(yValue == null)
                return 1;

            // If values implement IComparable
            IComparable comparableValue = xValue as IComparable;
            if(comparableValue != null)
                result = comparableValue.CompareTo(yValue);
            else
                result = xValue.Equals(yValue) ? 0 : xValue.ToString().CompareTo(yValue.ToString()); // If values don't implement IComparer but are equivalent

            // Return result
            return result;
        }

        private static int CompareDescending(object xValue, object yValue)
        {
            // Return result adjusted for ascending or descending sort order ie
            // multiplied by 1 for ascending or -1 for descending
            return CompareAscending(xValue, yValue) * -1;
        }

        private static object GetPropertyValue(T value, string property)
        {
            // Get property
            PropertyInfo propertyInfo = value.GetType().GetProperty(property);

            // Return value
            return propertyInfo.GetValue(value, null);
        }
    }
}