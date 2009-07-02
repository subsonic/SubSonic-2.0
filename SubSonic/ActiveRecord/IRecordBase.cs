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
using System.Xml.Serialization;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRecordBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        bool IsLoaded { get; set; }

        /// <summary>
        /// True if data in the object needs to be saved
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        bool IsNew { get; set; }

        /// <summary>
        /// True if data in the object has been changed and differs from DB.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        bool IsDirty { get; }

        /// <summary>
        /// Name of the table
        /// </summary>
        /// <value>The name of the table.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string TableName { get; }

        /// <summary>
        /// Gets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string ProviderName { get; }

        /// <summary>
        /// Automatically called upon object creation. Sets IsNew to true;
        /// </summary>
        void MarkNew();

        /// <summary>
        /// Called after any property is set. Sets IsDirty to True.
        /// </summary>
        void MarkClean();

        /// <summary>
        /// Marks the old.
        /// </summary>
        void MarkOld();

        /// <summary>
        /// Inspects this instance.
        /// </summary>
        /// <returns></returns>
        string Inspect();

        /// <summary>
        /// Inspects the specified use HTML.
        /// </summary>
        /// <param name="useHtml">if set to <c>true</c> [use HTML].</param>
        /// <returns></returns>
        string Inspect(bool useHtml);

        /// <summary>
        /// Convert the record to a string.
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        /// Gets the type of the DB.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        DbType GetDBType(string columnName);

        /// <summary>
        /// Returns the current value of the primary key
        /// </summary>
        /// <returns></returns>
        object GetPrimaryKeyValue();

        /// <summary>
        /// Sets the column value.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="oValue">The o value.</param>
        void SetColumnValue(string columnName, object oValue);

        /// <summary>
        /// Returns the current value of a column.
        /// </summary>
        /// <typeparam name="CT">The type of the T.</typeparam>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        CT GetColumnValue<CT>(string columnName);

        /// <summary>
        /// Gets the column value.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        object GetColumnValue(string columnName);

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <returns></returns>
        TableSchema.Table GetSchema();

        /// <summary>
        /// Converts the record to XML.
        /// </summary>
        /// <returns></returns>
        string ToXML();

        /// <summary>
        /// Returns an object based on the passed-in XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        object NewFromXML(string xml);

        /// <summary>
        /// Loads the object with the current reader's values. Assumes the reader is already moved to
        /// first position in recordset (aka has been "Read()")
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        void Load(IDataReader dataReader);

        /// <summary>
        /// Loads the object with the current reader's values. Assumes the reader is already moved to
        /// first position in recordset (aka has been "Read()")
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        void Load(DataTable dataTable);

        /// <summary>
        /// Loads the object with the current DataRow's values.
        /// </summary>
        /// <param name="dataRow">The data row.</param>
        void Load(DataRow dataRow);

        /// <summary>
        /// Opens the IDataReader, loads the object and closes the IDataReader. Unlike AbstractList.LoadAndCloseReader,
        /// this method does not assume that reader is open and in the first position!
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        void LoadAndCloseReader(IDataReader dataReader);

        /// <summary>
        /// Gets the column settings.
        /// </summary>
        /// <returns></returns>
        TableSchema.TableColumnSettingCollection GetColumnSettings();

        /// <summary>
        /// Adds a new row to a Datatable with this record
        /// You must be sure the column names are the same
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        void CopyTo(DataTable dataTable);

        /// <summary>
        /// Copies a record from a DataTable to this instance. Column names must match.
        /// </summary>
        /// <param name="row">The row.</param>
        void CopyFrom(DataRow row);

        /// <summary>
        /// Determines whether this instance has errors.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </returns>
        bool HasErrors();

        /// <summary>
        /// Loops the underlying settings collection to validate type, nullability, and length
        /// </summary>
        void ValidateColumnSettings();

        /// <summary>
        /// Loads your object from a form postback
        /// </summary>
        void LoadFromPost();

        /// <summary>
        /// Loads your object from a form postback
        /// </summary>
        /// <param name="validatePost">Set this to false to skip validation</param>
        void LoadFromPost(bool validatePost);
    }
}