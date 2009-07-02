using System;
using System.Data;
using System.Xml.Serialization;

namespace SubSonic
{
    public interface IRepositoryRecord
    {
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        bool IsLoaded { get; set; }

        /// <summary>
        /// True if data in the object needs to be saved
        /// </summary>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        bool IsNew { get; set; }

        /// <summary>
        /// Automatically called upon object creation. Sets IsNew to true;
        /// </summary>
        void MarkNew();

        /// <summary>
        /// True if data in the object has been changed and differs from DB.
        /// </summary>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        bool IsDirty { get; }

        /// <summary>
        /// Called after any property is set. Sets IsDirty to True.
        /// </summary>
        void MarkClean();

        void MarkOld();

        /// <summary>
        /// Name of the table
        /// </summary>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string TableName { get; }

        string Inspect();
        string Inspect(bool useHtml);
        string ToString();
        DbType GetDBType(string columnName);
        System.Collections.Generic.List<string> GetErrors();
        /// <summary>
        /// Returns the current value of the primary key
        /// </summary>
        /// <returns></returns>
        object GetPrimaryKeyValue();

        void SetColumnValue(string columnName, object oValue);

        /// <summary>
        /// Returns the current value of a column.
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        CT GetColumnValue<CT>(string columnName);

        Object GetColumnValue(string columnName);

        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string ProviderName { get; }

        TableSchema.Table GetSchema();
        string ToXML();

        /// <summary>
        /// Returns an object based on the passed-in XML.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        object NewFromXML(string xml);

        /// <summary>
        /// Loads the object with the current reader's values. Assumes the reader is already moved to
        /// first position in recordset (aka has been "Read()")
        /// </summary>
        /// <param name="rdr">The RDR.</param>
        void Load(IDataReader rdr);

        /// <summary>
        /// Loads the object with the current reader's values. Assumes the reader is already moved to 
        /// first position in recordset (aka has been "Read()")
        /// </summary>
        /// <param name="tbl"></param>
        void Load(DataTable tbl);

        /// <summary>
        /// Loads the object with the current DataRow's values. 
        /// </summary>
        /// <param name="dr"></param>
        void Load(DataRow dr);

        /// <summary>
        /// Opens the IDataReader, loads the object and closes the IDataReader. Unlike AbstractList.LoadAndCloseReader,
        /// this method does not assume that reader is open and in the first position!
        /// </summary>
        /// <param name="rdr"></param>
        void LoadAndCloseReader(IDataReader rdr);

        TableSchema.TableColumnSettingCollection GetColumnSettings();

        /// <summary>
        /// Adds a new row to a Datatable with this record
        /// You must be sure the column names are the same
        /// </summary>
        /// <param name="dataTable"></param>
        void CopyTo(DataTable dataTable);

        /// <summary>
        /// Copies a record from a DataTable to this instance. Column names must match.
        /// </summary>
        /// <param name="row"></param>
        void CopyFrom(DataRow row);

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

        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string NullExceptionMessage { get;  }

        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string InvalidTypeExceptionMessage { get;  }

        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string LengthExceptionMessage { get;  }

    }
}