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
    /// Summary for the ReadOnlyList&lt;ItemType, ListType&gt; class
    /// </summary>
    /// <typeparam name="ItemType">The type of the tem type.</typeparam>
    /// <typeparam name="ListType">The type of the ist type.</typeparam>
    [Serializable]
    public class ReadOnlyList<ItemType, ListType> : AbstractList<ItemType, ListType>
        where ItemType : ReadOnlyRecord<ItemType>, new()
        where ListType : AbstractList<ItemType, ListType>, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyList&lt;ItemType, ListType&gt;"/> class.
        /// </summary>
        public ReadOnlyList() {}

        /// <summary>
        /// Creates and loads the collection, leaving the IDataReader open.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        public ReadOnlyList(IDataReader dataReader)
        {
            Load(dataReader);
        }

        /// <summary>
        /// Creates and loads the collection, with option to set IDataReader close behavior.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="closeReader">if set to <c>true</c> [close reader].</param>
        public ReadOnlyList(IDataReader dataReader, bool closeReader)
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
    }
}