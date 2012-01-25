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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text;
using SubSonic.Sugar;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the TableSchema class
    /// </summary>
    [Serializable]
    public class TableSchema
    {
        #region Nested type: AbstractTableSchema

        /// <summary>
        /// Summary for the AbstractTableSchema class
        /// </summary>
        [Serializable]
        public abstract class AbstractTableSchema
        {
            private string _tableName;
            private string className;
            private string classNamePlural;
            private string displayName;
            private ExtendedPropertyCollection extendedProperties = new ExtendedPropertyCollection();
            private string propertyName;

            [NonSerialized]
            private DataProvider provider;

            private string schemaName;
            private TableType tableType;

            /// <summary>
            /// SQL-qualified name of this table - e.g. "[dbo].[Products]"
            /// </summary>
            public string QualifiedName
            {
                get { return Provider.QualifyTableName(SchemaName, Name); }
            }

            /// <summary>
            /// Gets or sets the provider.
            /// </summary>
            /// <value>The provider.</value>
            public DataProvider Provider
            {
                get { return provider; }
                protected set { provider = value; }
            }

            /// <summary>
            /// Gets or sets the name of the table.
            /// </summary>
            /// <value>The name of the table.</value>
            public string TableName
            {
                get { return _tableName; }
                set
                {
                    _tableName = value;
                    className = TransformClassName(Name, false, tableType, Provider);
                    classNamePlural = TransformClassName(Name, true, tableType, Provider);
                    displayName = Utility.ParseCamelToProper(ClassName);
                }
            }

            /// <summary>
            /// Gets or sets the type of the table.
            /// </summary>
            /// <value>The type of the table.</value>
            public TableType TableType
            {
                get { return tableType; }
                set { tableType = value; }
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name
            {
                get { return _tableName; }
                set { TableName = value; }
            }

            /// <summary>
            /// Gets or sets the name of the class.
            /// </summary>
            /// <value>The name of the class.</value>
            public string ClassName
            {
                get { return className; }
                protected set { className = value; }
            }

            /// <summary>
            /// Gets or sets the class name plural.
            /// </summary>
            /// <value>The class name plural.</value>
            public string ClassNamePlural
            {
                get { return classNamePlural; }
                protected set { classNamePlural = value; }
            }

            /// <summary>
            /// Gets or sets the name of the property.
            /// </summary>
            /// <value>The name of the property.</value>
            public string PropertyName
            {
                get { return propertyName; }
                protected set { propertyName = value; }
            }

            /// <summary>
            /// Gets or sets the display name.
            /// </summary>
            /// <value>The display name.</value>
            public string DisplayName
            {
                get { return displayName; }
                protected set { displayName = value; }
            }

            /// <summary>
            /// Gets or sets the name of the schema.
            /// </summary>
            /// <value>The name of the schema.</value>
            public string SchemaName
            {
                get { return schemaName; }
                set { schemaName = value; }
            }

            /// <summary>
            /// Gets or sets the extended properties.
            /// </summary>
            /// <value>The extended properties.</value>
            public ExtendedPropertyCollection ExtendedProperties
            {
                get { return extendedProperties; }
                set { extendedProperties = value; }
            }

            /// <summary>
            /// Override of ToString() returns the table name
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return QualifiedName;
            }

            /// <summary>
            /// Transforms the name of the class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="isPlural">if set to <c>true</c> [is plural].</param>
            /// <param name="tableType">Type of the table.</param>
            /// <param name="provider">The provider.</param>
            /// <returns></returns>
            public static string TransformClassName(string name, bool isPlural, TableType tableType, DataProvider provider)
            {
                if(String.IsNullOrEmpty(name))
                    return String.Empty;

                string newName = name;
                if(tableType == TableType.Table)
                    newName = Utility.StripText(newName, provider.StripTableText);
                else if(tableType == TableType.View)
                    newName = Utility.StripText(newName, provider.StripViewText);

                newName = Utility.RegexTransform(newName, provider);
                newName = Utility.GetProperName(newName, provider);
                newName = Utility.IsStringNumeric(newName) ? String.Concat("_", newName) : newName;
                newName = Utility.StripNonAlphaNumeric(newName);
                newName = newName.Trim();

                if(!isPlural)
                    newName = provider.FixPluralClassNames ? Utility.PluralToSingular(newName) : newName;

                return Utility.KeyWordCheck(newName, String.Empty, provider);
            }

            /// <summary>
            /// Applies the extended properties.
            /// </summary>
            public void ApplyExtendedProperties()
            {
                ExtendedProperty epClassName = ExtendedProperty.GetExtendedProperty(ExtendedProperties, ExtendedPropertyName.SSX_TABLE_CLASS_NAME_SINGULAR);
                ExtendedProperty epClassNamePlural = ExtendedProperty.GetExtendedProperty(ExtendedProperties, ExtendedPropertyName.SSX_TABLE_CLASS_NAME_PLURAL);
                ExtendedProperty epDisplayName = ExtendedProperty.GetExtendedProperty(ExtendedProperties, ExtendedPropertyName.SSX_TABLE_DISPLAY_NAME);

                if(epClassName != null)
                    className = epClassName.PropertyValue;
                if(epClassNamePlural != null)
                    classNamePlural = epClassNamePlural.PropertyValue;
                if(epDisplayName != null)
                    displayName = epDisplayName.PropertyValue;
            }
        }

        #endregion


        #region Nested type: ExtendedProperty

        /// <summary>
        /// Summary for the ExtendedProperty class
        /// </summary>
        [Serializable]
        public class ExtendedProperty
        {
            private string propertyName;

            private string propertyValue;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExtendedProperty"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="value">The value.</param>
            public ExtendedProperty(string name, string value)
            {
                propertyName = name;
                propertyValue = value;
            }

            /// <summary>
            /// Gets or sets the name of the property.
            /// </summary>
            /// <value>The name of the property.</value>
            public string PropertyName
            {
                get { return propertyName; }
                set { propertyName = value; }
            }

            /// <summary>
            /// Gets or sets the property value.
            /// </summary>
            /// <value>The property value.</value>
            public string PropertyValue
            {
                get { return propertyValue; }
                set { propertyValue = value; }
            }

            /// <summary>
            /// Gets the extended property.
            /// </summary>
            /// <param name="exPropCol">The ex prop col.</param>
            /// <param name="extendedPropertyName">Name of the extended property.</param>
            /// <returns></returns>
            public static ExtendedProperty GetExtendedProperty(ExtendedPropertyCollection exPropCol, string extendedPropertyName)
            {
                ExtendedProperty exProperty = null;
                if(exPropCol.Contains(extendedPropertyName))
                {
                    exProperty = exPropCol[extendedPropertyName];
                    if(String.IsNullOrEmpty(exProperty.PropertyValue))
                        exProperty = null;
                }
                return exProperty;
            }
        }

        #endregion


        #region Nested type: ExtendedPropertyCollection

        /// <summary>
        /// Summary for the ExtendedPropertyCollection class
        /// </summary>
        [Serializable]
        public class ExtendedPropertyCollection : KeyedCollection<string, ExtendedProperty>
        {
            /// <summary>
            /// Gets the key for item.
            /// </summary>
            /// <param name="extendedProperty">The extended property.</param>
            /// <returns></returns>
            protected override string GetKeyForItem(ExtendedProperty extendedProperty)
            {
                return extendedProperty.PropertyName;
            }
        }

        #endregion


        #region Nested type: ForeignKeyTable

        /// <summary>
        /// Summary for the ForeignKeyTable class
        /// </summary>
        [Serializable]
        public class ForeignKeyTable : AbstractTableSchema
        {
            private string _columnName;
            private string _foreignColumnName;

            private string _primaryColumnName;

            /// <summary>
            /// Initializes a new instance of the <see cref="ForeignKeyTable"/> class.
            /// </summary>
            /// <param name="dataProvider">The data provider.</param>
            public ForeignKeyTable(DataProvider dataProvider)
            {
                Provider = dataProvider;
            }

            /// <summary>
            /// The PK on the Primary table - CategoryID in Categories.
            /// </summary>
            public string PrimaryColumnName
            {
                get { return _primaryColumnName; }
                set { _primaryColumnName = value; }
            }

            /// <summary>
            /// The foreign key column on the foreign table - CategoryID in Products for instance
            /// </summary>
            public string ForeignColumnName
            {
                get { return _foreignColumnName; }
                set { _foreignColumnName = value; }
            }

            /// <summary>
            /// Gets or sets the name of the column.
            /// </summary>
            /// <value>The name of the column.</value>
            public string ColumnName
            {
                get { return _columnName; }
                set
                {
                    _columnName = value;
                    PropertyName = TableColumn.TransformPropertyName(_columnName, TableName, Provider);
                }
            }
        }

        #endregion


        #region Nested type: ForeignKeyTableCollection

        /// <summary>
        /// Summary for the ForeignKeyTableCollection class
        /// </summary>
        [Serializable]
        public class ForeignKeyTableCollection : List<ForeignKeyTable> {}

        #endregion


        #region Nested type: ManyToManyDetails

        /// <summary>
        /// Summary for the ManyToManyDetails class
        /// </summary>
        [Serializable]
        public class ManyToManyDetails
        {
            private string linksToColum;
            private string linksToTable;
            private string mapTableName;

            /// <summary>
            /// Gets or sets the links to table.
            /// </summary>
            /// <value>The links to table.</value>
            public string LinksToTable
            {
                get { return linksToTable; }
                set { linksToTable = value; }
            }

            /// <summary>
            /// Gets or sets the links to column.
            /// </summary>
            /// <value>The links to column.</value>
            public string LinksToColumn
            {
                get { return linksToColum; }
                set { linksToColum = value; }
            }

            /// <summary>
            /// Gets or sets the name of the map table.
            /// </summary>
            /// <value>The name of the map table.</value>
            public string MapTableName
            {
                get { return mapTableName; }
                set { mapTableName = value; }
            }
        }

        #endregion


        #region Nested type: ManyToManyDetailsCollection

        /// <summary>
        /// Summary for the ManyToManyDetailsCollection class
        /// </summary>
        [Serializable]
        public class ManyToManyDetailsCollection : List<ManyToManyDetails> {}

        #endregion


        #region Nested type: ManyToManyRelationship

        /// <summary>
        /// Summary for the ManyToManyRelationship class
        /// </summary>
        [Serializable]
        public class ManyToManyRelationship : AbstractTableSchema
        {
            private readonly string mapTableName;
            private string foreignPrimaryKey;
            private string foreignTableName;
            private string mapTableForeignTableKeyColumn;

            private string mapTableLocalTableKeyColumn;

            /// <summary>
            /// Initializes a new instance of the <see cref="ManyToManyRelationship"/> class.
            /// </summary>
            /// <param name="tableName">Name of the table.</param>
            /// <param name="dataProvider">The data provider.</param>
            public ManyToManyRelationship(string tableName, DataProvider dataProvider)
            {
                Provider = dataProvider;
                mapTableName = tableName;
                Name = mapTableName;
                ClassName = TransformClassName(Name, false, TableType, Provider);
                ClassNamePlural = TransformClassName(Name, false, TableType, Provider);
                DisplayName = Utility.ParseCamelToProper(ClassName);
                PropertyName = TransformClassName(Name, false, TableType, Provider);
            }

            /// <summary>
            /// Gets the name of the map table.
            /// </summary>
            /// <value>The name of the map table.</value>
            public string MapTableName
            {
                get { return mapTableName; }
                //set { mapTableName = value; }
            }

            /// <summary>
            /// Gets or sets the map table local table key column.
            /// </summary>
            /// <value>The map table local table key column.</value>
            public string MapTableLocalTableKeyColumn
            {
                get { return mapTableLocalTableKeyColumn; }
                set { mapTableLocalTableKeyColumn = value; }
            }

            /// <summary>
            /// Gets or sets the map table foreign table key column.
            /// </summary>
            /// <value>The map table foreign table key column.</value>
            public string MapTableForeignTableKeyColumn
            {
                get { return mapTableForeignTableKeyColumn; }
                set { mapTableForeignTableKeyColumn = value; }
            }

            /// <summary>
            /// Gets or sets the foreign primary key.
            /// </summary>
            /// <value>The foreign primary key.</value>
            public string ForeignPrimaryKey
            {
                get { return foreignPrimaryKey; }
                set { foreignPrimaryKey = value; }
            }

            /// <summary>
            /// Gets or sets the name of the foreign table.
            /// </summary>
            /// <value>The name of the foreign table.</value>
            public string ForeignTableName
            {
                get { return foreignTableName; }
                set { foreignTableName = value; }
            }
        }

        #endregion


        #region Nested type: ManyToManyRelationshipCollection

        /// <summary>
        /// Summary for the ManyToManyRelationshipCollection class
        /// </summary>
        [Serializable]
        public class ManyToManyRelationshipCollection : List<ManyToManyRelationship> {}

        #endregion


        #region Nested type: PrimaryKeyTable

        /// <summary>
        /// Summary for the PrimaryKeyTable class
        /// </summary>
        [Serializable]
        public class PrimaryKeyTable : AbstractTableSchema
        {
            private string _columnName;

            /// <summary>
            /// Initializes a new instance of the <see cref="PrimaryKeyTable"/> class.
            /// </summary>
            /// <param name="dataProvider">The data provider.</param>
            public PrimaryKeyTable(DataProvider dataProvider)
            {
                Provider = dataProvider;
            }

            /// <summary>
            /// Gets or sets the name of the column.
            /// </summary>
            /// <value>The name of the column.</value>
            public string ColumnName
            {
                get { return _columnName; }
                set
                {
                    _columnName = value;
                    PropertyName = TableColumn.TransformPropertyName(_columnName, TableName, Provider);
                }
            }
        }

        #endregion


        #region Nested type: PrimaryKeyTableCollection

        /// <summary>
        /// Summary for the PrimaryKeyTableCollection class
        /// </summary>
        [Serializable]
        public class PrimaryKeyTableCollection : List<PrimaryKeyTable> {}

        #endregion


        #region Nested type: Table

        /// <summary>
        /// Holds information about the base table - this class should be
        /// static for each object
        /// </summary>
        [Serializable]
        public class Table : AbstractTableSchema
        {
            private ForeignKeyTableCollection _foreignKeys = new ForeignKeyTableCollection();
            private bool _hasManyToMany;
            private PrimaryKeyTableCollection _primaryKeys = new PrimaryKeyTableCollection();
            private TableColumnCollection columns;
            private ManyToManyRelationshipCollection manyToManys = new ManyToManyRelationshipCollection();
            private TableColumn primaryKey;
            private TableColumn[] primaryKeys;

            /// <summary>
            /// Initializes a new instance of the <see cref="Table"/> class.
            /// </summary>
            /// <param name="dataProvider">The data provider.</param>
            public Table(DataProvider dataProvider)
            {
                Provider = dataProvider;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Table"/> class.
            /// </summary>
            /// <param name="providerName">Name of the provider.</param>
            /// <param name="tableName">Name of the table.</param>
            public Table(string providerName, string tableName)
            {
                Provider = DataService.Providers[providerName];
                Name = tableName;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Table"/> class.
            /// </summary>
            /// <param name="tableName">Name of the table.</param>
            public Table(string tableName)
            {
                Provider = DataService.Provider;
                Name = tableName;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Table"/> class.
            /// </summary>
            /// <param name="tableName">Name of the table.</param>
            /// <param name="tableType">Type of the table.</param>
            /// <param name="dataProvider">The data provider.</param>
            public Table(string tableName, TableType tableType, DataProvider dataProvider)
            {
                Provider = dataProvider;
                TableType = tableType;
                Name = tableName;
            }

            /// <summary>
            /// Gets or sets the many to manys.
            /// </summary>
            /// <value>The many to manys.</value>
            public ManyToManyRelationshipCollection ManyToManys
            {
                get { return manyToManys; }
                set { manyToManys = value; }
            }

            /// <summary>
            /// Gets or sets the primary key tables.
            /// </summary>
            /// <value>The primary key tables.</value>
            public PrimaryKeyTableCollection PrimaryKeyTables
            {
                get { return _primaryKeys; }
                set { _primaryKeys = value; }
            }

            /// <summary>
            /// Gets or sets the foreign keys.
            /// </summary>
            /// <value>The foreign keys.</value>
            public ForeignKeyTableCollection ForeignKeys
            {
                get { return _foreignKeys; }
                set { _foreignKeys = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance has many to many.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance has many to many; otherwise, <c>false</c>.
            /// </value>
            public bool HasManyToMany
            {
                get { return _hasManyToMany; }
                set { _hasManyToMany = value; }
            }

            /// <summary>
            /// Gets or sets the columns.
            /// </summary>
            /// <value>The columns.</value>
            public TableColumnCollection Columns
            {
                get { return columns; }
                set { columns = value; }
            }

            /// <summary>
            /// Gets the primary key.
            /// </summary>
            /// <value>The primary key.</value>
            public TableColumn PrimaryKey
            {
                get
                {
                    if(columns != null)
                        primaryKey = columns.GetPrimaryKey();
                    return primaryKey;
                }
            }

            /// <summary>
            /// A descriptive column for this schema. An example would be "ProductName" for Northwind products
            /// </summary>
            public TableColumn Descriptor
            {
                get
                {
                    TableColumn result = null;
                    foreach(TableColumn col in columns)
                    {
                        if(col.IsString && !col.IsPrimaryKey && !col.IsForeignKey)
                        {
                            result = col;
                            break;
                        }
                    }

                    if(result == null)
                        result = PrimaryKey;
                    return result;
                }
            }

            /// <summary>
            /// Gets the primary keys.
            /// </summary>
            /// <value>The primary keys.</value>
            public TableColumn[] PrimaryKeys
            {
                get
                {
                    if(columns != null)
                        primaryKeys = columns.GetPrimaryKeys();
                    return primaryKeys;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this instance has a primary key.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance has primary key; otherwise, <c>false</c>.
            /// </value>
            public bool HasPrimaryKey
            {
                get { return PrimaryKeys.Length > 0; }
            }

            /// <summary>
            /// Determines whether [has foreign keys].
            /// </summary>
            /// <returns>
            /// 	<c>true</c> if [has foreign keys]; otherwise, <c>false</c>.
            /// </returns>
            public bool HasForeignKeys
            {
                get { return ForeignKeys.Count > 0; }
            }

            /// <summary>
            /// Returns a delimited string list of each column
            /// </summary>
            /// <param name="delim">The delim.</param>
            /// <param name="useQualified">if set to <c>true</c> [use qualified].</param>
            /// <param name="omitPrimaryKey">if set to <c>true</c> [omit primary key].</param>
            /// <returns></returns>
            public string GetDelimitedColumnList(string delim, bool useQualified, bool omitPrimaryKey)
            {
                bool isFirst = true;
                StringBuilder sb = new StringBuilder();
                foreach(TableColumn col in columns)
                {
                    bool shouldAppend = true;
                    if(col.IsPrimaryKey && omitPrimaryKey)
                        shouldAppend = false;
                    if(shouldAppend)
                    {
                        if(!isFirst)
                            sb.Append(delim);
                        if(useQualified)
                            sb.Append(col.QualifiedName);
                        else
                            sb.Append(col.ColumnName);
                        isFirst = false;
                    }
                }

                return sb.ToString();
            }

            /// <summary>
            /// Override Equals() to test and see if the table name is the same, and the column
            /// names are the same
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(object obj)
            {
                bool result = false;
                if(obj is Table)
                {
                    Table compareTo = (Table)obj;

                    //test the table name
                    //this will set the output to true if the names match
                    result = compareTo.Name == Name;

                    if(result)
                    {
                        //the names match, so let's be sure that the comparing object
                        //has the same number of columns
                        result = columns.Count == compareTo.columns.Count;
                    }

                    if(result)
                    {
                        //first two tests passed, now make sure the columns
                        //are named the same
                        foreach(TableColumn col in columns)
                        {
                            if(!compareTo.columns.Contains(col.ColumnName))
                            {
                                result = false;
                                break;
                            }
                        }
                    }
                }
                return result;
            }

            /// <summary>
            /// Serves as a hash function for a particular type.
            /// </summary>
            /// <returns>
            /// A hash code for the current <see cref="T:System.Object"/>.
            /// </returns>
            [EditorBrowsable(EditorBrowsableState.Never)]
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            /// <summary>
            /// Adds the column.
            /// </summary>
            /// <param name="tableColumn">The table column.</param>
            public void AddColumn(TableColumn tableColumn)
            {
                if(Columns == null)
                    Columns = new TableColumnCollection();

                //make sure there's no duplicates
                if(tableColumn.IsPrimaryKey)
                {
                    if(PrimaryKey == null)
                        columns.Insert(0, tableColumn);
                    else
                        throw new Exception("This schema already has a primary key");
                }
                else
                {
                    if(!Columns.Contains(tableColumn.ColumnName))
                        columns.Add(tableColumn);
                    else
                        throw new Exception("Column '" + tableColumn.ColumnName + "' defined twice");
                }
            }

            /// <summary>
            /// Adds the long text.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <param name="nullable">if set to <c>true</c> [nullable].</param>
            /// <param name="defaultValue">The default value.</param>
            public void AddLongText(string columnName, bool nullable, string defaultValue)
            {
                const int length = 9000;
                AddColumn(columnName, DbType.String, length, nullable, defaultValue);
            }

            /// <summary>
            /// Adds the column.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            public void AddColumn(string columnName)
            {
                //For you Phil :)
                AddColumn(columnName, DbType.String, 64, true, String.Empty);
            }

            /// <summary>
            /// Adds the column.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <param name="dbType">Type of the db.</param>
            public void AddColumn(string columnName, DbType dbType)
            {
                int length = 0;
                if(dbType == DbType.String)
                    //For you Phil :)
                    length = 64;
                AddColumn(columnName, dbType, length, true, String.Empty);
            }

            /// <summary>
            /// Adds the column.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <param name="dbType">Type of the db.</param>
            /// <param name="length">The length.</param>
            public void AddColumn(string columnName, DbType dbType, int length)
            {
                AddColumn(columnName, dbType, length, true, String.Empty);
            }

            /// <summary>
            /// Adds the column.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <param name="dbType">Type of the db.</param>
            /// <param name="length">The length.</param>
            /// <param name="nullable">if set to <c>true</c> [nullable].</param>
            public void AddColumn(string columnName, DbType dbType, int length, bool nullable)
            {
                AddColumn(columnName, dbType, length, nullable, String.Empty);
            }

            /// <summary>
            /// Adds the column.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <param name="dbType">Type of the db.</param>
            /// <param name="length">The length.</param>
            /// <param name="nullable">if set to <c>true</c> [nullable].</param>
            /// <param name="defaultValue">The default value.</param>
            public void AddColumn(string columnName, DbType dbType, int length, bool nullable, string defaultValue)
            {
                TableColumn c = new TableColumn(this)
                                    {
                                        ColumnName = columnName,
                                        MaxLength = length,
                                        DataType = dbType,
                                        IsNullable = nullable,
                                        DefaultSetting = defaultValue
                                    };

                AddColumn(c);
            }

            /// <summary>
            /// Removes the column.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            public void RemoveColumn(string columnName)
            {
                TableColumn col = GetColumn(columnName);
                if(col != null)
                    col.Deleted = true;
            }

            /// <summary>
            /// Adds the primary key column.
            /// </summary>
            public void AddPrimaryKeyColumn()
            {
                //for you Phil :)
                AddPrimaryKeyColumn(String.Concat(Name, "Id"));
            }

            /// <summary>
            /// Adds the primary key column.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            public void AddPrimaryKeyColumn(string columnName)
            {
                //for you Phil :)
                TableColumn c = new TableColumn(this)
                                    {
                                        ColumnName = columnName,
                                        AutoIncrement = true,
                                        DataType = DbType.Int32,
                                        IsPrimaryKey = true
                                    };
                AddColumn(c);
            }

            /// <summary>
            /// Gets the column.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <returns></returns>
            public TableColumn GetColumn(string columnName)
            {
                TableColumn col = null;
                foreach(TableColumn column in Columns)
                {
                    if(Utility.IsMatch(column.ColumnName.Trim(), columnName.Trim()))
                    {
                        col = column;
                        break;
                    }
                }
                return col;
            }
        }

        #endregion


        #region Nested type: TableCollection

        /// <summary>
        /// Summary for the TableCollection class
        /// </summary>
        [Serializable]
        public class TableCollection : List<Table>
        {
            /// <summary>
            /// Gets or sets the <see cref="SubSonic.TableSchema.Table"/> with the specified table name.
            /// </summary>
            /// <value></value>
            public Table this[string tableName]
            {
                get
                {
                    Table result = null;
                    foreach(Table tbl in this)
                    {
                        if(Utility.IsMatch(tbl.Name, tableName))
                        {
                            result = tbl;
                            break;
                        }
                    }
                    return result;
                }
                set
                {
                    int index = 0;
                    foreach(Table tbl in this)
                    {
                        if(Utility.IsMatch(tbl.Name, tableName))
                        {
                            this[index] = value;
                            break;
                        }
                        index++;
                    }
                }
            }
        }

        #endregion


        #region Nested type: TableColumn

        /// <summary>
        /// A helper class to help define the columns in an underlying table
        /// </summary>
        [Serializable]
        public class TableColumn
        {
            private bool _altered;
            private string _auditMessage;
            private bool _deleted;
            private string argumentName;
            private bool autoIncrement;
            private string columnName;
            private DbType dbType;
            private string defaultSetting;
            private string displayName;
            private ExtendedPropertyCollection extendedProperties = new ExtendedPropertyCollection();
            private string foreignKeyTableName;
            private bool isForeignKey;
            private bool isNullable;
            private bool isPrimaryKey;
            private bool isReadOnly;
            private int maxLength;
            private int numberPrecision;
            private int numberScale;
            private string parameterName;
            private string propertyName;
            private string schemaName;
            private Table table;

            /// <summary>
            /// Initializes a new instance of the <see cref="TableColumn"/> class.
            /// </summary>
            /// <param name="tableSchema">The table schema.</param>
            public TableColumn(Table tableSchema)
            {
                table = tableSchema;
            }

            /// <summary>
            /// Gets or sets the audit message.
            /// </summary>
            /// <value>The audit message.</value>
            public string AuditMessage
            {
                get { return _auditMessage; }
                set { _auditMessage = value; }
            }

            /// <summary>
            /// Gets the name of the column in delimited format, based on the provider.
            /// </summary>
            /// <value>The delimited name of the column</value>
            public string DelimitedName
            {
                get { return Table.Provider.FormatIdentifier(columnName); }
            }

            /// <summary>
            /// Gets the name of column in fully qualified format, based on the provider.
            /// </summary>
            /// <value>The fully qualified name of the column.</value>
            public string QualifiedName
            {
                get
                {
                    return Table.Provider.QualifyColumnName(Table.SchemaName, Table.Name, ColumnName);
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="TableColumn"/> is deleted.
            /// </summary>
            /// <value><c>true</c> if deleted; otherwise, <c>false</c>.</value>
            public bool Deleted
            {
                get { return _deleted; }
                set { _deleted = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="TableColumn"/> is altered.
            /// </summary>
            /// <value><c>true</c> if altered; otherwise, <c>false</c>.</value>
            public bool Altered
            {
                get { return _altered; }
                set { _altered = value; }
            }

            /// <summary>
            /// Gets or sets the default setting.
            /// </summary>
            /// <value>The default setting.</value>
            public string DefaultSetting
            {
                get { return defaultSetting; }
                set { defaultSetting = value; }
            }

            /// <summary>
            /// Gets or sets the table.
            /// </summary>
            /// <value>The table.</value>
            public Table Table
            {
                get { return table; }
                set { table = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is foreign key.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is foreign key; otherwise, <c>false</c>.
            /// </value>
            public bool IsForeignKey
            {
                get { return isForeignKey; }
                set { isForeignKey = value; }
            }

            /// <summary>
            /// Gets or sets the name of the foreign key table.
            /// </summary>
            /// <value>The name of the foreign key table.</value>
            public string ForeignKeyTableName
            {
                get
                {
                    if(IsForeignKey && foreignKeyTableName == null)
                        foreignKeyTableName = DataService.GetForeignKeyTableName(ColumnName, Table.Name, Table.Provider.Name);
                    return foreignKeyTableName;
                }
                set { foreignKeyTableName = value; }
            }

            /// <summary>
            /// Gets or sets the name of the schema.
            /// </summary>
            /// <value>The name of the schema.</value>
            public string SchemaName
            {
                get { return schemaName; }
                set { schemaName = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is primary key.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is primary key; otherwise, <c>false</c>.
            /// </value>
            public bool IsPrimaryKey
            {
                get { return isPrimaryKey; }
                set { isPrimaryKey = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is nullable.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is nullable; otherwise, <c>false</c>.
            /// </value>
            public bool IsNullable
            {
                get { return isNullable; }
                set { isNullable = value; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is read only.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is read only; otherwise, <c>false</c>.
            /// </value>
            public bool IsReadOnly
            {
                get { return isReadOnly; }
                set { isReadOnly = value; }
            }

            /// <summary>
            /// Gets or sets the type of the data.
            /// </summary>
            /// <value>The type of the data.</value>
            public DbType DataType
            {
                get { return dbType; }
                set { dbType = value; }
            }

            /// <summary>
            /// Gets or sets the length of the max.
            /// </summary>
            /// <value>The length of the max.</value>
            public int MaxLength
            {
                get { return maxLength; }
                set { maxLength = value; }
            }

            /// <summary>
            /// Gets or sets the name of the column.
            /// </summary>
            /// <value>The name of the column.</value>
            public string ColumnName
            {
                get { return columnName; }
                set
                {
                    columnName = value;
                    string transformColumnName = columnName;

                    propertyName = TransformPropertyName(transformColumnName, Table.ClassName, table.Provider);
                    displayName = TransformPropertyName(transformColumnName, Table.ClassName, table.Provider);

                    displayName = Utility.ParseCamelToProper(displayName);
                    if((!Validation.IsUpperCase(displayName)) && (IsPrimaryKey || IsForeignKey) && displayName.Length > 1)
                    {
                        string strEnd = displayName.Substring(displayName.Length - 2, 2);
                        if(Utility.IsMatch(strEnd, "id") && strEnd[0].ToString() == "I")
                            displayName = displayName.Substring(0, displayName.Length - 2);
                    }
                    parameterName = Table.Provider.PreformatParameterName(columnName);
                    argumentName = String.Concat("var", propertyName);
                }
            }

            /// <summary>
            /// Gets the name of the parameter.
            /// </summary>
            /// <value>The name of the parameter.</value>
            public string ParameterName
            {
                get { return parameterName; }
            }

            /// <summary>
            /// Gets the name of the property.
            /// </summary>
            /// <value>The name of the property.</value>
            public string PropertyName
            {
                get { return propertyName; }
            }

            /// <summary>
            /// Gets the display name.
            /// </summary>
            /// <value>The display name.</value>
            public string DisplayName
            {
                get { return displayName; }
            }

            /// <summary>
            /// Gets the name of the argument.
            /// </summary>
            /// <value>The name of the argument.</value>
            public string ArgumentName
            {
                get { return argumentName; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether [auto increment].
            /// </summary>
            /// <value><c>true</c> if [auto increment]; otherwise, <c>false</c>.</value>
            public bool AutoIncrement
            {
                get { return autoIncrement; }
                set { autoIncrement = value; }
            }

            /// <summary>
            /// Gets or sets the number scale.
            /// </summary>
            /// <value>The number scale.</value>
            public int NumberScale
            {
                get { return numberScale; }
                set { numberScale = value; }
            }

            /// <summary>
            /// Gets or sets the number precision.
            /// </summary>
            /// <value>The number precision.</value>
            public int NumberPrecision
            {
                get { return numberPrecision; }
                set { numberPrecision = value; }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is a reserved column (CreatedOn, ModifiedOn, etc).
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is reserved; otherwise, <c>false</c>.
            /// </value>
            public bool IsReservedColumn
            {
                get
                {
                    bool result = Utility.IsMatch(ColumnName, ReservedColumnName.CREATED_BY) ||
                                  Utility.IsMatch(ColumnName, ReservedColumnName.MODIFIED_BY) ||
                                  Utility.IsMatch(ColumnName, ReservedColumnName.MODIFIED_ON) ||
                                  Utility.IsMatch(ColumnName, ReservedColumnName.CREATED_ON) ||
                                  Utility.IsMatch(ColumnName, ReservedColumnName.DELETED) ||
                                  Utility.IsMatch(ColumnName, ReservedColumnName.IS_ACTIVE) ||
                                  Utility.IsMatch(ColumnName, ReservedColumnName.IS_DELETED);
                    return result;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is numeric.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is numeric; otherwise, <c>false</c>.
            /// </value>
            public bool IsNumeric
            {
                get
                {
                    return DataType == DbType.Currency ||
                           DataType == DbType.Decimal ||
                           DataType == DbType.Double ||
                           DataType == DbType.Int16 ||
                           DataType == DbType.Int32 ||
                           DataType == DbType.Int64 ||
                           DataType == DbType.Single ||
                           DataType == DbType.UInt16 ||
                           DataType == DbType.UInt32 ||
                           DataType == DbType.UInt64 ||
                           DataType == DbType.VarNumeric;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is date time.
            /// </summary>
            /// <value>
            /// 	<c>true</c> if this instance is date time; otherwise, <c>false</c>.
            /// </value>
            public bool IsDateTime
            {
                get
                {
                    return DataType == DbType.DateTime ||
                           DataType == DbType.Time ||
                           DataType == DbType.Date;
                }
            }

            /// <summary>
            /// Gets a value indicating whether this instance is string.
            /// </summary>
            /// <value><c>true</c> if this instance is string; otherwise, <c>false</c>.</value>
            public bool IsString
            {
                get
                {
                    return DataType == DbType.AnsiString ||
                           DataType == DbType.AnsiStringFixedLength ||
                           DataType == DbType.String ||
                           DataType == DbType.StringFixedLength;
                }
            }

            /// <summary>
            /// Gets or sets the extended properties.
            /// </summary>
            /// <value>The extended properties.</value>
            public ExtendedPropertyCollection ExtendedProperties
            {
                get { return extendedProperties; }
                set { extendedProperties = value; }
            }

            /// <summary>
            /// Gets the type of the property.
            /// </summary>
            /// <returns></returns>
            public Type GetPropertyType()
            {
                string systemType = Utility.GetSystemType(dbType);
                Type t = Type.GetType(systemType);
                return t;
            }

            /// <summary>
            /// Transforms the name of the property.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="table">The table.</param>
            /// <param name="provider">The provider.</param>
            /// <returns></returns>
            public static string TransformPropertyName(string name, string table, DataProvider provider)
            {
                if(String.IsNullOrEmpty(name))
                    return String.Empty;

                string newName = name;

                newName = Utility.StripText(newName, provider.StripColumnText);
                newName = Utility.RegexTransform(newName, provider);
                newName = Utility.GetProperName(newName, provider);
                newName = Utility.IsStringNumeric(newName) ? String.Concat("_", newName) : newName;
                newName = Utility.StripNonAlphaNumeric(newName);
                newName = newName.Trim();
                return Utility.KeyWordCheck(newName, table, provider);
            }

            /// <summary>
            /// Applies the extended properties.
            /// </summary>
            public void ApplyExtendedProperties()
            {
                ExtendedProperty epPropertyName = ExtendedProperty.GetExtendedProperty(ExtendedProperties, ExtendedPropertyName.SSX_COLUMN_PROPERTY_NAME);
                ExtendedProperty epDisplayName = ExtendedProperty.GetExtendedProperty(ExtendedProperties, ExtendedPropertyName.SSX_COLUMN_DISPLAY_NAME);

                if(epPropertyName != null)
                    propertyName = epPropertyName.PropertyValue;
                if(epDisplayName != null)
                    displayName = epDisplayName.PropertyValue;
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </returns>
            public override string ToString()
            {
                return ColumnName;
            }
        }

        #endregion


        #region Nested type: TableColumnCollection

        /// <summary>
        /// Summary for the TableColumnCollection class
        /// </summary>
        [Serializable]
        public class TableColumnCollection : List<TableColumn>
        {
            #region Collection Methods

            /// <summary>
            /// Determines whether [contains] [the specified column name].
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <returns>
            /// 	<c>true</c> if [contains] [the specified column name]; otherwise, <c>false</c>.
            /// </returns>
            public bool Contains(string columnName)
            {
                bool bOut = false;
                foreach(TableColumn col in this)
                {
                    if(Utility.IsMatch(col.ColumnName, columnName))
                    {
                        bOut = true;
                        break;
                    }
                }
                return bOut;
            }

            /// <summary>
            /// Adds the specified TBL.
            /// </summary>
            /// <param name="tableSchema">The table schema.</param>
            /// <param name="name">The name.</param>
            /// <param name="dbType">Type of the db.</param>
            /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
            /// <param name="isPrimaryKey">if set to <c>true</c> [is primary key].</param>
            /// <param name="isForeignKey">if set to <c>true</c> [is foreign key].</param>
            public void Add(Table tableSchema, string name, DbType dbType, bool isNullable, bool isPrimaryKey, bool isForeignKey)
            {
                TableColumn col = new TableColumn(tableSchema)
                                      {
                                          IsPrimaryKey = isPrimaryKey,
                                          IsForeignKey = isForeignKey,
                                          IsNullable = isNullable,
                                          DataType = dbType,
                                          ColumnName = name
                                      };

                if(!Contains(name))
                    Add(col);
            }

            /// <summary>
            /// Adds the specified TBL.
            /// </summary>
            /// <param name="tableSchema">The table schema.</param>
            /// <param name="name">The name.</param>
            /// <param name="dbType">Type of the db.</param>
            /// <param name="isNullable">if set to <c>true</c> [is nullable].</param>
            public void Add(Table tableSchema, string name, DbType dbType, bool isNullable)
            {
                Add(tableSchema, name, dbType, isNullable, false, false);
            }

            #endregion


            /// <summary>
            /// Gets the column.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <returns></returns>
            public TableColumn GetColumn(string columnName)
            {
                TableColumn coll = null;
                foreach(TableColumn child in this)
                {
                    if(Utility.IsMatch(child.ColumnName, columnName))
                    {
                        coll = child;
                        break;
                    }
                }
                return coll;
            }

            /// <summary>
            /// Gets the primary key.
            /// </summary>
            /// <returns></returns>
            public TableColumn GetPrimaryKey()
            {
                TableColumn coll = null;
                foreach(TableColumn child in this)
                {
                    if(child.IsPrimaryKey)
                    {
                        coll = child;
                        break;
                    }
                }
                return coll;
            }

            /// <summary>
            /// Gets the primary keys.
            /// </summary>
            /// <returns></returns>
            public TableColumn[] GetPrimaryKeys()
            {
                List<TableColumn> list = new List<TableColumn>();
                foreach(TableColumn child in this)
                {
                    if(child.IsPrimaryKey)
                        list.Add(child);
                }

                return list.ToArray();
            }
        }

        #endregion


        #region Nested type: TableColumnSetting

        /// <summary>
        /// This is an intermediary class that holds the current value of a table column
        /// for each object instance.
        /// </summary>
        [Serializable]
        public class TableColumnSetting
        {
            private readonly string _columnName;

            private object _currentValue;

            private bool _isDirty;

            /// <summary>
            /// Initializes a new instance of the <see cref="TableColumnSetting"/> class.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <param name="currentValue">The current value.</param>
            public TableColumnSetting(string columnName, object currentValue)
            {
                _columnName = columnName;
                _currentValue = currentValue;
            }

            /// <summary>
            /// Gets the name of the column.
            /// </summary>
            /// <value>The name of the column.</value>
            public string ColumnName
            {
                get { return _columnName; }
            }

            /// <summary>
            /// Gets or sets the current value.
            /// </summary>
            /// <value>The current value.</value>
            public object CurrentValue
            {
                get { return _currentValue; }
                set
                {
                    if(value == null && _currentValue == null)
                        return;

                    if(value != null)
                    {
                        if(value.Equals(_currentValue))
                            return;
                    }

                    _currentValue = value;
                    _isDirty = true;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this instance is dirty.
            /// </summary>
            /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
            public bool IsDirty
            {
                get { return _isDirty; }
                set { _isDirty = value; }
            }
        }

        #endregion


        #region Nested type: TableColumnSettingCollection

        /// <summary>
        /// Summary for the TableColumnSettingCollection class
        /// </summary>
        [Serializable]
        public class TableColumnSettingCollection : KeyedCollection<string, TableColumnSetting>
        {
            /// <summary>
            /// Gets or sets a value indicating whether this instance is dirty.
            /// </summary>
            /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
            public bool IsDirty
            {
                get
                {
                    foreach(TableColumnSetting setting in this)
                    {
                        if(setting.IsDirty)
                            return true;
                    }
                    return false;
                }
                //set
                //{
                //    foreach (TableColumnSetting setting in this)
                //        setting.IsDirty = value;
                //}
            }

            /// <summary>
            /// When implemented in a derived class, extracts the key from the specified element.
            /// </summary>
            /// <param name="item">The element from which to extract the key.</param>
            /// <returns>The key for the specified element.</returns>
            protected override string GetKeyForItem(TableColumnSetting item)
            {
                return item.ColumnName;
            }

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <returns></returns>
            public object GetValue(string columnName)
            {
                return this[columnName.ToLower()].CurrentValue;
            }

            /// <summary>
            /// Gets the value.
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="columnName">Name of the column.</param>
            /// <returns></returns>
            public T GetValue<T>(string columnName)
            {
                columnName = columnName.ToLowerInvariant();
                object oVal;

                try
                {
                    oVal = this[columnName].CurrentValue;
                }
                catch
                {
                    throw new ArgumentException("There's no column called '" + columnName + "' for this object", "columnName");
                }

                if(oVal == null || oVal == DBNull.Value)
                    return default(T);

                Type type = typeof(T);

                if(type == typeof(object))
                    return (T)oVal;

                // handle nullable type conversion
                if(IsNullable(type))
                {
                    NullableConverter converter = new NullableConverter(type);
                    type = converter.UnderlyingType;
                }

                //if (IsNullable(type) || type == typeof(object))
                //{
                //    if (type == typeof(bool?))
                //        return (T)(object)Convert.ToBoolean(oVal);
                //    return (T)oVal;
                //}

                //if (type == typeof(Guid))
                //    return (T)(object)new Guid(oVal.ToString());

                Type valType = oVal.GetType();
                if(valType == typeof(Byte[]))
                    return (T)Convert.ChangeType(oVal, valType);

                return (T)Convert.ChangeType(oVal, type);
            }

            /// <summary>
            /// Determines whether the specified obj type is nullable.
            /// </summary>
            /// <param name="objType">Type of the obj.</param>
            /// <returns>
            /// 	<c>true</c> if the specified obj type is nullable; otherwise, <c>false</c>.
            /// </returns>
            public static bool IsNullable(Type objType)
            {
                if(!objType.IsGenericType)
                    return false;

                return objType.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
            }

            /// <summary>
            /// Sets the value.
            /// </summary>
            /// <param name="columnName">Name of the column.</param>
            /// <param name="oVal">The o val.</param>
            public void SetValue(string columnName, object oVal)
            {
                columnName = columnName.ToLowerInvariant();
                if(!Contains(columnName))
                    Add(new TableColumnSetting(columnName, oVal));
                else
                    this[columnName].CurrentValue = oVal;
            }
        }

        #endregion
    }
}