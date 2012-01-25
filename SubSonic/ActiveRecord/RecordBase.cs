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
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class RecordBase<T> where T : RecordBase<T>, new()
    {
        #region State Properties

        private static readonly object _lockBaseSchema = new object();

        private TableSchema.TableColumnCollection _dirtyColumns = new TableSchema.TableColumnCollection();

        private bool _isLoaded;
        internal bool _isNew = true;
        private bool _validateWhenSaving = true;

        /// <summary>
        /// Gets or sets a value indicating whether [validate when saving].
        /// </summary>
        /// <value><c>true</c> if [validate when saving]; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public bool ValidateWhenSaving
        {
            get { return _validateWhenSaving; }
            set { _validateWhenSaving = value; }
        }

        /// <summary>
        /// A collection of column's who's values have been changed during instance scope. Used
        /// for Update commands.
        /// </summary>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public TableSchema.TableColumnCollection DirtyColumns
        {
            get { return _dirtyColumns; }
            set { _dirtyColumns = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loaded.
        /// </summary>
        /// <value><c>true</c> if this instance is loaded; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public bool IsLoaded
        {
            get { return _isLoaded; }
            set { _isLoaded = value; }
        }

        /// <summary>
        /// Whether or not this is a new record. The value is <c>true</c> if this object
        /// has yet to be persisted to a database, otherwise it's <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public bool IsNew
        {
            get { return _isNew; }
            set { _isNew = value; }
        }

        /// <summary>
        /// Whether or not the value of this object differs from that of the persisted
        /// database. This value is <c>true</c>if data has been changed and differs from DB,
        /// otherwise it is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public bool IsDirty
        {
            get { return columnSettings.IsDirty; }
        }

        /// <summary>
        /// Name of the table in the database that contains persisted data for this type
        /// </summary>
        /// <value>The name of the table.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public string TableName
        {
            get { return BaseSchema.TableName; }
        }

        /// <summary>
        /// Empty virtual method that may be overridden in user code to perform operations
        /// at the conclusion of SetDefaults() invocation
        /// </summary>
        public virtual void Initialize() {}

        /// <summary>
        /// Empty virtual method that may be overridden in user code to perform operations
        /// at the conclusion of Load() invocation
        /// </summary>
        protected virtual void Loaded() {}

        /// <summary>
        /// Automatically called upon object creation. Sets IsNew to <c>true</c>;
        /// </summary>
        public void MarkNew()
        {
            _isNew = true;
        }

        /// <summary>
        /// Called after any property is set. Sets IsDirty to <c>true</c>.
        /// </summary>
        public void MarkClean()
        {
            foreach(TableSchema.TableColumnSetting setting in columnSettings)
                setting.IsDirty = false;
            DirtyColumns.Clear();
        }

        /// <summary>
        /// Called after Update() invokation. Sets IsNew to <c>false</c>.
        /// </summary>
        public void MarkOld()
        {
            IsLoaded = true;
            _isNew = false;
        }

        #endregion


        #region Object Overrides

        /// <summary>
        /// Creates a HTML formatted text represententation of the contents of the current record
        /// </summary>
        /// <returns></returns>
        public string Inspect()
        {
            return Inspect(true);
        }

        /// <summary>
        /// Creates a formatted text represententation of the contents of the current record
        /// </summary>
        /// <param name="useHtml">Whether or not the results should be formatted using HTML</param>
        /// <returns></returns>
        public string Inspect(bool useHtml)
        {
            StringBuilder sb = new StringBuilder();
            string sOut;
            if(useHtml)
            {
                sb.Append(String.Format("<table><tr><td colspan=2><h3>{0} Inspection</h3></td></tr>", BaseSchema.Name));

                foreach(TableSchema.TableColumn col in BaseSchema.Columns)
                    sb.AppendFormat("<tr><td><span style=\"font-weight:bold\">{0}</span></td><td>{1}</td></tr>", col.ColumnName, GetColumnValue<object>(col.ColumnName));
                sb.Append("</table>");
                sOut = sb.ToString();
            }
            else
            {
                sb.AppendLine(String.Format("#################{0} Inspection ####################", BaseSchema.Name));

                foreach(TableSchema.TableColumn col in BaseSchema.Columns)
                    sb.AppendLine(String.Concat(col.ColumnName, ": ", GetColumnValue<object>(col.ColumnName)));
                sb.AppendLine("#############################################################################");
                sOut = sb.ToString();
            }

            return sOut;
        }

        /// <summary>
        /// Returns the simple string representation of the current record. By convention, this is the "descriptor" column (#2)
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return GetColumnValue<string>(GetSchema().Descriptor.ColumnName);
        }

        #endregion


        #region DB Properties/Methods

        /// <summary>
        /// 
        /// </summary>
        protected static TableSchema.Table table;

        private string _providerName;

        /// <summary>
        /// The column settings hold the current values of the object in a collection
        /// so that reflection is not needed in the base class to fill out the commands
        /// </summary>
        private TableSchema.TableColumnSettingCollection columnSettings;

        /// <summary>
        /// The base static class that holds all schema info for the table.
        /// The class must be instanced at least once to populate this table.
        /// </summary>
        protected static TableSchema.Table BaseSchema
        {
            get
            {
                if(table == null)
                {
                    lock(_lockBaseSchema)
                        if(table == null)
                            new T();
                }
                return table;
            }
            set { table = value; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is schema initialized.
        /// The schema is considered initialized if the underlying TableSchema.Table
        /// object is not null, and column collection has been loaded with one or more columns
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is schema initialized; otherwise, <c>false</c>.
        /// </value>
        protected static bool IsSchemaInitialized
        {
            get { return (table != null && table.Columns != null && table.Columns.Count > 0); }
        }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public string ProviderName
        {
            get { return _providerName; }
            protected set { _providerName = value; }
        }

        /// <summary>
        /// Returns a default setting per data type
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected static object GetDefaultSetting(TableSchema.TableColumn column)
        {
            return Utility.GetDefaultSetting(column);
        }

        /// <summary>
        /// Return the underlying DbType for a column
        /// </summary>
        /// <param name="columnName">The name of the column whose type is to be returned</param>
        /// <returns></returns>
        public DbType GetDBType(string columnName)
        {
            TableSchema.TableColumn col = BaseSchema.GetColumn(columnName);
            return col.DataType;
        }

        /// <summary>
        /// Sets the Primary Key of the object
        /// </summary>
        /// <param name="oValue">The o value.</param>
        protected virtual void SetPrimaryKey(object oValue)
        {
            columnSettings.SetValue(BaseSchema.PrimaryKey.ColumnName, oValue);
        }

        /// <summary>
        /// Returns the current value of the primary key
        /// </summary>
        /// <returns></returns>
        public object GetPrimaryKeyValue()
        {
            return columnSettings.GetValue<object>(BaseSchema.PrimaryKey.ColumnName);
        }

        /// <summary>
        /// Sets a value for a particular column in the record
        /// </summary>
        /// <param name="columnName">Name of the column, as defined in the database</param>
        /// <param name="oValue">The value to set the type to</param>
        public void SetColumnValue(string columnName, object oValue)
        {
            // Set DBNull to null
            if (oValue == DBNull.Value)
                oValue = null;

            columnSettings = columnSettings ?? new TableSchema.TableColumnSettingCollection();

            // add the column to the DirtyColumns
            // if this instance has already been loaded
            // and this is a change to existing values
            if(IsLoaded && !IsNew)
            {
                TableSchema.Table schema = GetSchema();
                object oldValue = null;
                string oldValueMsg = "NULL";
                string newValueMsg = "NULL";
                bool areEqualOrBothNull = false;

                try
                {
                    oldValue = columnSettings.GetValue(columnName);
                }
                catch {}

                if(oldValue == null && oValue == null)
                    areEqualOrBothNull = true;
                else
                {
                    if(oldValue != null)
                    {
                        oldValueMsg = oldValue.ToString();
                        areEqualOrBothNull = oldValue.Equals(oValue);
                    }

                    if(oValue != null)
                        newValueMsg = oValue.ToString();
                }

                TableSchema.TableColumn dirtyCol = schema.GetColumn(columnName);

                if(dirtyCol != null && !areEqualOrBothNull)
                {
                    string auditMessage = String.Format("Value changed from {0} to {1}{2}", oldValueMsg, newValueMsg, Environment.NewLine);
                    TableSchema.TableColumn dirtyEntry = DirtyColumns.GetColumn(columnName);
                    if(dirtyEntry != null)
                    {
                        DirtyColumns.Remove(dirtyEntry);
                        auditMessage = String.Concat(dirtyCol.AuditMessage, auditMessage);
                    }

                    dirtyCol.AuditMessage = auditMessage;
                    DirtyColumns.Add(dirtyCol);
                }
            }

            columnSettings.SetValue(columnName, oValue);
        }

        /// <summary>
        /// Returns the current value of a column.
        /// </summary>
        /// <typeparam name="CT">The type of the T.</typeparam>
        /// <param name="columnName">Name of the column, as defined in the database</param>
        /// <returns></returns>
        public CT GetColumnValue<CT>(string columnName)
        {
            CT oOut = default(CT);

            if(columnSettings != null)
                oOut = columnSettings.GetValue<CT>(columnName);

            return oOut;
        }

        /// <summary>
        /// Returns the underly TableSchema.Table object for the given record
        /// </summary>
        /// <returns></returns>
        public TableSchema.Table GetSchema()
        {
            return BaseSchema;
        }

        #endregion


        #region WebUI Helper

        private readonly List<string> errorList = new List<string>();
        private string invalidTypeExceptionMessage = "{0} is not a valid {1}";
        private string lengthExceptionMessage = "{0} exceeds the maximum length of {1}";
        private string nullExceptionMessage = "{0} requires a value";

        /// <summary>
        /// Gets or sets the exception message thrown for non-null conversion errors performed during record validation.
        /// </summary>
        /// <value>The null exception message.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public string NullExceptionMessage
        {
            get { return nullExceptionMessage; }
            protected set { nullExceptionMessage = value; }
        }

        /// <summary>
        /// Gets or sets the exception message thrown for type conversion errors performed during record validation.
        /// </summary>
        /// <value>The invalid type exception message.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public string InvalidTypeExceptionMessage
        {
            get { return invalidTypeExceptionMessage; }
            protected set { invalidTypeExceptionMessage = value; }
        }

        /// <summary>
        /// Gets or sets the exception message thrown for value length conversion errors performed during record validation.
        /// </summary>
        /// <value>The length exception message.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        public string LengthExceptionMessage
        {
            get { return lengthExceptionMessage; }
            protected set { lengthExceptionMessage = value; }
        }

        /// <summary>
        /// Gets the collection of error messages for the record.
        /// </summary>
        /// <value></value>
        [HiddenForDataBinding(true)]
        public List<string> Errors
        {
            get { return errorList; }
        }

        /// <summary>
        /// Determines whether this instance has errors.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </returns>
        public bool HasErrors()
        {
            return errorList.Count > 0;
        }

        /// <summary>
        /// Returns the list of error messages for the record.
        /// </summary>
        /// <returns></returns>
        public List<string> GetErrors()
        {
            // TODO: This does the *exact* same thing as the Errors property, why are there two of them?  If there is a good reason it should be documented.
            return errorList;
        }

        /// <summary>
        /// Loops the underlying settings collection to validate type, nullability, and length
        /// </summary>
        public void ValidateColumnSettings()
        {
            // loop the current settings and make sure they are valid for their type
            foreach(TableSchema.TableColumnSetting setting in GetColumnSettings())
            {
                Utility.WriteTrace(String.Format("Validating {0}", setting.ColumnName));
                object settingValue = setting.CurrentValue;
                bool isNullValue = (settingValue == null || settingValue == DBNull.Value);
                TableSchema.TableColumn col = table.GetColumn(setting.ColumnName);

                if(!col.IsReadOnly)
                {
                    string formattedName = Utility.ParseCamelToProper(col.ColumnName);
                    Type t = col.GetPropertyType();

                    //// Convert the existing value to the type for this column
                    //// if there's an error, report it.
                    //// OK to bypass if the column is nullable and this setting is null
                    //// just check for now if the value isn't null - it will be checked
                    //// later for nullability

                    if(!col.IsNullable && !isNullValue)
                    {
                        try
                        {
                            if(col.DataType != DbType.Guid)
                                Convert.ChangeType(settingValue, t);
                        }
                        catch
                        {
                            // there's a conversion problem here
                            // add it to the Exception List<>
                            if(col.IsNumeric)
                                errorList.Add(String.Format(InvalidTypeExceptionMessage, formattedName, "number"));
                            else if(col.IsDateTime)
                                errorList.Add(String.Format(InvalidTypeExceptionMessage, formattedName, "date"));
                            else
                                errorList.Add(String.Format(InvalidTypeExceptionMessage, formattedName, "value"));
                        }
                    }

                    bool isDbControlledAuditField = (Utility.IsAuditField(col.ColumnName) && !String.IsNullOrEmpty(col.DefaultSetting));

                    // now make sure that this column's null settings match with what's in the setting
                    Utility.WriteTrace(String.Format("Testing nullability of {0}", setting.ColumnName));
                    if(!col.IsNullable && isNullValue && !isDbControlledAuditField)
                    {
                        Utility.WriteTrace(String.Format("Null Error Caught {0}", setting.ColumnName));
                        errorList.Add(String.Format(NullExceptionMessage, formattedName));
                    }

                    // finally, check the length
                    Utility.WriteTrace(String.Format("Testing Max Length of {0}", setting.ColumnName));
                    if(!isNullValue && col.MaxLength > 0)
                    {
                        if(col.DataType != DbType.Boolean && settingValue.ToString().Length > col.MaxLength)
                        {
                            Utility.WriteTrace(String.Format("Max Length Exceeded {0} (can't exceed {1}); current value is set to {2}",
                                col.ColumnName,
                                col.MaxLength,
                                settingValue.ToString().Length));
                            errorList.Add(String.Format(LengthExceptionMessage, formattedName, col.MaxLength));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Loads your object from an ASP.NET form postback
        /// </summary>
        public void LoadFromPost()
        {
            LoadFromPost(true);
        }

        /// <summary>
        /// Loads your object from an ASP.NET form postback
        /// </summary>
        /// <param name="validatePost">Set this to false to skip validation</param>
        public void LoadFromPost(bool validatePost)
        {
            if(HttpContext.Current != null)
            {
                // use Request.form, since the ControlCollection can return weird results based on 
                // the container structure.
                NameValueCollection formPost = HttpContext.Current.Request.Form;
                TableSchema.TableColumnSettingCollection settings = GetColumnSettings();

                if(formPost != null && settings != null)
                {
                    foreach(string s in formPost.AllKeys)
                    {
                        Utility.WriteTrace(String.Format("Looking at form field {0}", s));

                        foreach(TableSchema.TableColumnSetting setting in settings)
                        {
                            if(s.EndsWith(String.Concat("_", setting.ColumnName), StringComparison.InvariantCultureIgnoreCase) ||
                               s.EndsWith(String.Concat("$", setting.ColumnName), StringComparison.InvariantCultureIgnoreCase) ||
                               Utility.IsMatch(s, setting.ColumnName))
                            {
                                SetColumnValue(setting.ColumnName, formPost[s]);
                                Utility.WriteTrace(String.Format("Matched {0} to {1}", s, setting.ColumnName));
                            }
                        }
                    }
                }

                // validate the settings, since we're setting the object values here, not
                // using the accessors as we should be.
                if(validatePost)
                {
                    ValidateColumnSettings();

                    if(errorList.Count > 0)
                    {
                        // format this for the web
                        if(HttpContext.Current != null)
                        {
                            // decorate the output
                            StringBuilder errorReport = new StringBuilder("<b>Validation Error:</b><ul>");
                            foreach(string s in errorList)
                                errorReport.AppendFormat("<li><em>{0}</em></li>", s);

                            errorReport.Append("</ul>");
                            throw new Exception(errorReport.ToString());
                        }

                        throw new Exception(
                            "Validation error - catch this and check the ExceptionList property to review the exceptions. You can change the output message as needed by accessing the ExceptionMessage properties of this object");
                    }
                }
            }
        }

        #endregion


        #region Serializers

        /// <summary>
        /// Returns and XML representation of the given record
        /// </summary>
        /// <returns></returns>
        public string ToXML()
        {
            Type type = typeof(T);
            XmlSerializer ser = new XmlSerializer(type);
            using(MemoryStream stm = new MemoryStream())
            {
                // serialize to a memory stream
                ser.Serialize(stm, this);

                // reset to beginning so we can read it.  
                stm.Position = 0;

                // Convert a string. 
                using(StreamReader stmReader = new StreamReader(stm))
                {
                    string xmlData = stmReader.ReadToEnd();
                    return xmlData;
                }
            }
        }

        /// <summary>
        /// Returns an object based on the passed-in XML.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public object NewFromXML(string xml)
        {
            object oOut = null;
            Type type = typeof(T);

            // hydrate based on private string var
            if(xml.Length > 0)
            {
                XmlSerializer serializer = new XmlSerializer(type);
                StringBuilder sb = new StringBuilder();
                sb.Append(xml);
                using(StringReader sReader = new StringReader(xml))
                {
                    oOut = serializer.Deserialize(sReader);
                    sReader.Close();
                }
            }

            return oOut;
        }

        #endregion


        #region Utility

        /// <summary>
        /// Returns the current collection of column settings for the given record.
        /// </summary>
        /// <returns></returns>
        public TableSchema.TableColumnSettingCollection GetColumnSettings()
        {
            GetSchema();
            return columnSettings;
        }

        /// <summary>
        /// Copies the current instance to a new instance
        /// </summary>
        /// <returns>New instance of current object</returns>
        public T Clone()
        {
            T thisInstance = new T();

            foreach(TableSchema.TableColumnSetting setting in columnSettings)
                thisInstance.SetColumnValue(setting.ColumnName, setting.CurrentValue);
            return thisInstance;
        }

        /// <summary>
        /// Copies current instance to the passed in instance
        /// </summary>
        /// <param name="copyInstance">The copy instance.</param>
        public void CopyTo(T copyInstance)
        {
            if(copyInstance == null)
                copyInstance = new T();

            foreach(TableSchema.TableColumnSetting setting in columnSettings)
                copyInstance.SetColumnValue(setting.ColumnName, setting.CurrentValue);
        }

        /// <summary>
        /// Adds a new row to a Datatable with this record.
        /// Column names must match for this to work properly.
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        public void CopyTo(DataTable dataTable)
        {
            DataRow newRow = dataTable.NewRow();
            foreach(TableSchema.TableColumnSetting setting in columnSettings)
            {
                try
                {
                    newRow[setting.ColumnName] = setting.CurrentValue;
                }
                catch {}
            }

            dataTable.Rows.Add(newRow);
        }

        /// <summary>
        /// Copies a record from a DataTable to this instance. Column names must match.
        /// </summary>
        /// <param name="row">The row.</param>
        public void CopyFrom(DataRow row)
        {
            foreach(TableSchema.TableColumnSetting setting in columnSettings)
            {
                try
                {
                    setting.CurrentValue = row[setting.ColumnName];
                }
                catch {}
            }
        }

        /// <summary>
        /// Copies the passed-in instance settings to this instance
        /// </summary>
        /// <param name="copyInstance">The copy instance.</param>
        public void CopyFrom(T copyInstance)
        {
            if(copyInstance == null)
                throw new ArgumentNullException("copyInstance");

            foreach(TableSchema.TableColumnSetting setting in copyInstance.columnSettings)
                SetColumnValue(setting.ColumnName, setting.CurrentValue);
        }

        #endregion


        #region Loaders

        /// <summary>
        /// Initializes the object using the default values for the underlying column data types.
        /// </summary>
        protected virtual void SetDefaults()
        {
            // initialize to default settings
            bool connectionClosed = true;
            bool setDbDefaults = DataService.GetInstance(ProviderName).SetPropertyDefaultsFromDatabase;

            if(DataService.GetInstance(ProviderName).CurrentSharedConnection != null)
                connectionClosed = DataService.GetInstance(ProviderName).CurrentSharedConnection.State == ConnectionState.Closed;

            foreach(TableSchema.TableColumn col in BaseSchema.Columns)
            {
                if(setDbDefaults && !String.IsNullOrEmpty(col.DefaultSetting) && connectionClosed)
                {
                    if(!Utility.IsMatch(col.DefaultSetting, SqlSchemaVariable.DEFAULT))
                    {
                        QueryCommand cmdDefault = new QueryCommand(String.Concat(SqlFragment.SELECT, col.DefaultSetting), col.Table.Provider.Name);
                        SetColumnValue(col.ColumnName, DataService.ExecuteScalar(cmdDefault));
                    }
                }
                else
                    SetColumnValue(col.ColumnName, Utility.GetDefaultSetting(col));
            }

            Initialize();
        }

        /// <summary>
        /// Forces object properties to be initialized using the defaults specified in the database schema.
        /// This method is called only if the provider level setting "useDatabaseDefaults" is set to <c>true</c>
        /// </summary>
        protected void ForceDefaults()
        {
            foreach(TableSchema.TableColumn col in BaseSchema.Columns)
            {
                if(!String.IsNullOrEmpty(col.DefaultSetting))
                {
                    if(!Utility.IsMatch(col.DefaultSetting, SqlSchemaVariable.DEFAULT))
                    {
						string s = col.DefaultSetting;
						if (s.StartsWith("=")) { s = s.Substring(1); }
                        QueryCommand cmdDefault = new QueryCommand(SqlFragment.SELECT + s, col.Table.Provider.Name);
                        SetColumnValue(col.ColumnName, DataService.ExecuteScalar(cmdDefault));
                    }
                }
                else
                    SetColumnValue(col.ColumnName, Utility.GetDefaultSetting(col));
            }
        }

        /// <summary>
        /// Sets initial record states when a record is loaded.
        /// </summary>
        protected void SetLoadState()
        {
            IsLoaded = true;
            IsNew = false;
            Loaded();
        }

        /// <summary>
        /// Loads the object with the current reader's values. Assumes the reader is already moved to
        /// first position in recordset (aka has been "Read()")
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        public virtual void Load(IDataReader dataReader)
        {
            foreach(TableSchema.TableColumn col in BaseSchema.Columns)
            {
                try
                {
                    SetColumnValue(col.ColumnName, dataReader[col.ColumnName]);
                }
                catch(Exception)
                {
                    // turning off the Exception for now
                    // to support partial loads

                    // throw new Exception("Unable to set column value for " + col.ColumnName + ": " + x.Message);
                }
            }

            SetLoadState();
            MarkClean();
        }

        /// <summary>
        /// Loads the object with the current reader's values. Assumes the reader is already moved to
        /// first position in recordset (aka has been "Read()")
        /// </summary>
        /// <param name="dataTable">The data table.</param>
        public virtual void Load(DataTable dataTable)
        {
            if(dataTable.Rows.Count > 0)
            {
                DataRow dr = dataTable.Rows[0];
                Load(dr);
            }
        }

        /// <summary>
        /// Loads the object with the current DataRow's values.
        /// </summary>
        /// <param name="dr">The dr.</param>
        public virtual void Load(DataRow dr)
        {
            foreach(TableSchema.TableColumn col in BaseSchema.Columns)
            {
                try
                {
                    SetColumnValue(col.ColumnName, dr[col.ColumnName]);
                }
                catch(Exception)
                {
                    // turning off the Exception for now
                    // TODO: move this to a SubSonicLoadException
                    // which collects the exceptions

                    // this will happen only if there's a reader error.
                    // throw new Exception("Unable to set column value for " + col.ColumnName + "; " + x.Message);
                }
            }

            SetLoadState();
        }

        /// <summary>
        /// Opens the IDataReader, loads the object and closes the IDataReader. Unlike AbstractList.LoadAndCloseReader,
        /// this method does not assume that reader is open and in the first position!
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        public void LoadAndCloseReader(IDataReader dataReader)
        {
            using(dataReader)
            {
                if(dataReader.Read())
                    Load(dataReader);
                if(!dataReader.IsClosed)
                    dataReader.Close();
            }
        }

        #endregion
    }

    /// <summary>
    /// Attribute class used to suppress "internal" properties when databinding. YMMV.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    internal sealed class HiddenForDataBindingAttribute : Attribute
    {
        private readonly bool _isHidden;

        /// <summary>
        /// Initializes a new instance of the <see cref="HiddenForDataBindingAttribute"/> class.
        /// </summary>
        public HiddenForDataBindingAttribute() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="HiddenForDataBindingAttribute"/> class.
        /// </summary>
        /// <param name="isHidden">if set to <c>true</c> [is hidden].</param>
        public HiddenForDataBindingAttribute(bool isHidden)
        {
            _isHidden = isHidden;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value><c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden
        {
            get { return _isHidden; }
        }
    }
}