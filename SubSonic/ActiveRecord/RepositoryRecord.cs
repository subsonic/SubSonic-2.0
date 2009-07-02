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

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RepositoryRecord<T> : RecordBase<T>, IEquatable<T> where T : RepositoryRecord<T>, new()
    {
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


        #region IEquatable

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(T other)
        {
            return GetPrimaryKeyValue().Equals(other.GetPrimaryKeyValue());
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            RepositoryRecord<T> repositoryObj = obj as RepositoryRecord<T>;
            if(repositoryObj != null)
                return GetPrimaryKeyValue().Equals(repositoryObj.GetPrimaryKeyValue());

            return base.Equals(obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return GetPrimaryKeyValue().GetHashCode();
        }

        #endregion

        // Thanks JGuerts!
    }
}