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
    /// Base class for persisting objects. Follows the "Active Record Design Pattern".
    /// You can read more on this pattern at http://en.wikipedia.org/wiki/Active_Record
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public abstract class AbstractRecord<T> : RecordBase<T>, IEquatable<T> where T : AbstractRecord<T>, new()
    {
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
            AbstractRecord<T> recordObj = obj as AbstractRecord<T>;
            if(recordObj != null)
                return GetPrimaryKeyValue().Equals(recordObj.GetPrimaryKeyValue());

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
    }
}