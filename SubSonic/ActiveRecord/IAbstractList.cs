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

using System.Data;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAbstractList
    {
        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        string ProviderName { get; set; }

        /// <summary>
        /// Loads the specified IDataReader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        void Load(IDataReader dataReader);

        /// <summary>
        /// Loads the specified TBL.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        void Load(DataTable dataTable);

        /// <summary>
        /// Loads the and close IDataReader.
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        void LoadAndCloseReader(IDataReader dataReader);

        /// <summary>
        /// Sorts the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="ascending">if set to <c>true</c> [ascending].</param>
        void Sort(string columnName, bool ascending);

        /// <summary>
        /// Convert the list to a data table.
        /// </summary>
        /// <returns></returns>
        DataTable ToDataTable();
    }
}