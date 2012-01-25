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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SubSonic.Sugar;
using Calendar=System.Web.UI.WebControls.Calendar;

namespace SubSonic.Utilities
{
    /// <summary>
    /// A class containing support methods used internally to SubSonic
    /// </summary>
    public class Utility
    {
        private static bool? _isRunningInMediumTrust;

        /// <summary>
        /// Gets a value indicating whether this instance is running in medium trust.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is running in medium trust; otherwise, <c>false</c>.
        /// </value>
        public static bool IsRunningInMediumTrust
        {
            get
            {
                if(!_isRunningInMediumTrust.HasValue)
                    _isRunningInMediumTrust = (GetCurrentTrustLevel() == AspNetHostingPermissionLevel.Medium);
                return _isRunningInMediumTrust.Value;
            }
        }

        /// <summary>
        /// Writes out the passed string to the current context trace or debug console, if applicable and enabled.
        /// </summary>
        /// <param name="message">The message to write</param>
        public static void WriteTrace(string message)
        {
            if(DataService.EnableTrace)
            {
                if(HttpContext.Current != null && HttpContext.Current.Trace.IsEnabled)
                {
                    message = String.Concat(DateTime.Now.ToString("H:mm:ss:fff"), " > ", message);
                    HttpContext.Current.Trace.Write("SubSonic", message);
                }
                else if(!IsRunningInMediumTrust)
                    FullTrustTrace(message);
            }
        }

        private static void FullTrustTrace(string message)
        {
            if(Debug.Listeners.Count > 0)
            {
                message = String.Concat(DateTime.Now.ToString("H:mm:ss:fff"), " > ", message);
                Debug.WriteLine(message, "SubSonic");
                Console.WriteLine(message);
            }
        }

        private static AspNetHostingPermissionLevel GetCurrentTrustLevel()
        {
            foreach(AspNetHostingPermissionLevel trustLevel in
                new[]
                    {
                        AspNetHostingPermissionLevel.Unrestricted,
                        AspNetHostingPermissionLevel.High,
                        AspNetHostingPermissionLevel.Medium,
                        AspNetHostingPermissionLevel.Low,
                        AspNetHostingPermissionLevel.Minimal
                    })
            {
                try
                {
                    new AspNetHostingPermission(trustLevel).Demand();
                }
                catch(SecurityException)
                {
                    continue;
                }

                return trustLevel;
            }

            return AspNetHostingPermissionLevel.None;
        }


        #region WebUtility

        /// <summary>
        /// Builds a simple HTML table from the passed-in datatable
        /// </summary>
        /// <param name="tbl">System.Data.DataTable</param>
        /// <param name="tableWidth">The width of the table</param>
        /// <returns>System.String</returns>
        public static string DataTableToHtmlTable(DataTable tbl, string tableWidth)
        {
            StringBuilder sb = new StringBuilder();
            if(String.IsNullOrEmpty(tableWidth))
                tableWidth = "70%";

            if(tbl != null)
            {
                sb.AppendFormat("<table style=\"width: {0}\" cellpadding=\"4\" cellspacing=\"0\"><thead style=\"background-color: #dcdcdc\">", tableWidth);

                //header
                foreach(DataColumn col in tbl.Columns)
                    sb.AppendFormat("<th><span style=\"font-weight: bold\">{0}</span></th>", col.ColumnName);
                sb.Append("</thead>");

                //rows
                bool isEven = false;
                foreach(DataRow dr in tbl.Rows)
                {
                    if(isEven)
                        sb.Append("<tr>");
                    else
                        sb.Append("<tr style=\"background-color: #f5f5f5\">");

                    foreach(DataColumn col in tbl.Columns)
                        sb.AppendFormat("<td>{0}</td>", dr[col]);
                    sb.Append("</tr>");

                    isEven = !isEven;
                }
                sb.Append("</table>");
            }
            return sb.ToString();
        }

        [Obsolete("Deprecated: Use DataTableToHtmlTable() instead")]
        public static string DataTableToHTML(DataTable tbl, string tableWidth)
        {
            return DataTableToHtmlTable(tbl, tableWidth);
        }

        #endregion


        #region Tests

        /// <summary>
        /// Determines whether the specified provider is SQL2000.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>
        /// 	<c>true</c> if the specified provider is SQL2000; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSql2000(DataProvider provider)
        {
			return provider.DatabaseVersion.IndexOf("SQL Server 2000") > -1;
        }

        /// <summary>
        /// Determines whether the specified provider is SQL2005.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>
        /// 	<c>true</c> if the specified provider is SQL2005; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSql2005(DataProvider provider)
        {
			return provider.DatabaseVersion.IndexOf("SQL Server 2005") > -1;
        }

        /// <summary>
        /// Determines whether the specified provider is SQL2008.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <returns>
        /// 	<c>true</c> if the specified provider is SQL2008; otherwise, <c>false</c>.
        /// </returns>

        public static bool IsSql2008(DataProvider provider)
        {
            return provider.DatabaseVersion.Contains("SQL Server 2008");
        }

        /// <summary>
        /// Determines whether the passed column name is a logical delete column, per the SubSonic naming conventions.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        /// 	<c>true</c> if it's a logical delete column; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLogicalDeleteColumn(string columnName)
        {
            return IsMatch(columnName, ReservedColumnName.DELETED) || IsMatch(columnName, ReservedColumnName.IS_DELETED);
        }

        /// <summary>
        /// Determines whether the passed column name is a audit column, per the SubSonic naming conventions.
        /// </summary>
        /// <param name="colName">Name of the column.</param>
        /// <returns>
        /// 	<c>true</c> if it's an audit column; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAuditField(string colName)
        {
            return
                IsMatch(colName, ReservedColumnName.CREATED_BY) ||
                IsMatch(colName, ReservedColumnName.CREATED_ON) ||
                IsMatch(colName, ReservedColumnName.MODIFIED_BY) ||
                IsMatch(colName, ReservedColumnName.MODIFIED_ON);
        }

        /// <summary>
        /// Checks to see if a column has any of the attributes that indicate it should not be written to.
        /// Special thanks to Damien, aka bouncingcastle, who identified an serious missing condition check,
        /// prompting us to create this method.
        /// </summary>
        /// <param name="col">The TableColumn to evaluate</param>
        /// <returns>
        /// 	<c>true</c> if column is writable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWritableColumn(TableSchema.TableColumn col)
        {
            return
                !IsMatch(col.ColumnName, ReservedColumnName.CREATED_BY) &&
                !IsMatch(col.ColumnName, ReservedColumnName.CREATED_ON) &&
                !col.IsReadOnly &&
                !col.IsPrimaryKey &&
                !col.AutoIncrement ||
                col.IsForeignKey;
        }

        /// <summary>
        /// Performs a case-insensitive comparison of two passed strings.
        /// </summary>
        /// <param name="stringA">The string to compare with the second parameter</param>
        /// <param name="stringB">The string to compare with the first parameter</param>
        /// <returns>
        /// 	<c>true</c> if the strings match; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMatch(string stringA, string stringB)
        {
            return String.Equals(stringA, stringB, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Evaluates an array of strings to determine if at least one item is a match
        /// </summary>
        /// <param name="stringA">The base comparison string.</param>
        /// <param name="matchStrings">The match strings.</param>
        /// <returns></returns>
        public static bool MatchesOne(string stringA, params string[] matchStrings)
        {
            for(int i = 0; i < matchStrings.Length; i++)
            {
                if(IsMatch(stringA, matchStrings[i]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Performs a case-insensitive comparison of two passed strings, 
        /// with an option to trim the strings before comparison.
        /// </summary>
        /// <param name="stringA">The string to compare with the second parameter</param>
        /// <param name="stringB">The string to compare with the first parameter</param>
        /// <param name="trimStrings">if set to <c>true</c> strings will be trimmed before comparison.</param>
        /// <returns>
        /// 	<c>true</c> if the strings match; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMatch(string stringA, string stringB, bool trimStrings)
        {
            if(trimStrings)
                return String.Equals(stringA.Trim(), stringB.Trim(), StringComparison.InvariantCultureIgnoreCase);

            return String.Equals(stringA, stringB, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Determines whether the passed string matches the passed RegEx pattern.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="matchPattern">The RegEx match pattern.</param>
        /// <returns>
        /// 	<c>true</c> if the string matches the pattern; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRegexMatch(string inputString, string matchPattern)
        {
            return Regex.IsMatch(inputString, matchPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        /// <summary>
        /// Strips any whitespace from the passed string using RegEx
        /// </summary>
        /// <param name="inputString">The string to strip of whitespace</param>
        /// <returns></returns>
        public static string StripWhitespace(string inputString)
        {
            if(!String.IsNullOrEmpty(inputString))
                return Regex.Replace(inputString, @"\s", String.Empty);

            return inputString;
        }

        /// <summary>
        /// Strips square brackets from an identifier
        /// </summary>
        /// <param name="inputString">The string containing the identifier</param>
        /// <returns></returns>
        public static string StripSquareBrackets(string inputString)
        {
            string s = inputString;
            if (!String.IsNullOrEmpty(s))
            {
                s = s.Trim();
                if (s.StartsWith("[") && s.EndsWith("]"))
                    return s.Substring(1,s.Length - 1);
            }
            return s;
        }

        /// <summary>
        /// Determine whether the passed string is numeric, by attempting to parse it to a double
        /// </summary>
        /// <param name="str">The string to evaluated for numeric conversion</param>
        /// <returns>
        /// 	<c>true</c> if the string can be converted to a number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsStringNumeric(string str)
        {
            double result;
            return (double.TryParse(str, NumberStyles.Float, NumberFormatInfo.CurrentInfo, out result));
        }

        /// <summary>
        /// Determines whether the specified column is a numeric data type.
        /// </summary>
        /// <param name="column">The TableColumn whose DbType will be evaluated</param>
        /// <returns>
        /// 	<c>true</c> if the specified column has a numeric DbType; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumeric(TableSchema.TableColumn column)
        {
            switch(column.DataType)
            {
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.Double:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the specified column is a string data type.
        /// </summary>
        /// <param name="column">The TableColumn whose DbType will be evaluated</param>
        /// <returns>
        /// 	<c>true</c> if the specified column has a string DbType; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsString(TableSchema.TableColumn column)
        {
            switch(column.DataType)
            {
                case DbType.String:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Determines whether the passed DbType supports null values.
        /// </summary>
        /// <param name="dbType">The DbType to evaluate</param>
        /// <returns>
        /// 	<c>true</c> if the DbType supports null values; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullableDbType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.Binary:
                    //case DbType.Byte:
                case DbType.Object:
                case DbType.String:
                case DbType.StringFixedLength:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Determines whether the current web user is authenticated by evaluating the HTTP context.
        /// </summary>
        /// 	<c>true</c> if the user is authenticaed; otherwise, <c>false</c>.
        public static bool UserIsAuthenticated()
        {
            HttpContext context = HttpContext.Current;

            if(context.User != null && context.User.Identity != null && !String.IsNullOrEmpty(context.User.Identity.Name))
                return true;

            return false;
        }

        /// <summary>
        /// Determines if the passed-in type is AbstractRecord or ActiveRecord
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsSubSonicType<T>()
        {
            Type iType = typeof(T);
            bool result = iType.GetInterface(typeof(IRecordBase).FullName) != null ||
                          iType.GetInterface(typeof(IActiveRecord).FullName) != null;
            return result;
        }

        #endregion


        #region Types

        /// <summary>
        /// I can't remember why I created this method. :P
        /// </summary>
        /// <param name="dbType">The DbType to evaluate</param>
        /// <returns>
        /// 	<c>true</c> if the specified DbType is parsable; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsParsable(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.Binary:
                case DbType.Guid:
                case DbType.Object:
                case DbType.String:
                case DbType.StringFixedLength:
                    return false;
                default:
                    return true;
            }
        }

        // TODO: Refactor out
        /// <summary>
        /// Gets the default value for the DbType of a given TableColumn.
        /// </summary>
        /// <param name="col">The TableColumn to evaluate</param>
        /// <param name="language">The code generation language</param>
        /// <returns></returns>
        public static string GetDefaultValue(TableSchema.TableColumn col, ICodeLanguage language)
        {
            return language.GetDefaultValue(col.ColumnName, col.DataType, false);
        }

        // TODO: Refactor out
        /// <summary>
        /// Gets the string representation of a .NET type for a given DbType.
        /// </summary>
        /// <param name="dbType">The DbType to be evaluated</param>
        /// <param name="isNullableColumn">if set to <c>true</c> the database column is designated nullable</param>
        /// <param name="language">The code generation language</param>
        /// <returns></returns>
        public static string GetVariableType(DbType dbType, bool isNullableColumn, ICodeLanguage language)
        {
            return language.GetVariableType(dbType, isNullableColumn);
        }

        /// <summary>
        /// Returns the SqlDbType for a give DbType
        /// </summary>
        /// <param name="dbType">The DbType to evaluate</param>
        /// <returns></returns>
        public static SqlDbType GetSqlDBType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                    return SqlDbType.VarChar;
                case DbType.AnsiStringFixedLength:
                    return SqlDbType.Char;
                case DbType.Binary:
                    return SqlDbType.VarBinary;
                case DbType.Boolean:
                    return SqlDbType.Bit;
                case DbType.Byte:
                    return SqlDbType.TinyInt;
                case DbType.Currency:
                    return SqlDbType.Money;
                case DbType.Date:
                    return SqlDbType.DateTime;
                case DbType.DateTime:
                    return SqlDbType.DateTime;
                case DbType.Decimal:
                    return SqlDbType.Decimal;
                case DbType.Double:
                    return SqlDbType.Float;
                case DbType.Guid:
                    return SqlDbType.UniqueIdentifier;
                case DbType.Int16:
                    return SqlDbType.Int;
                case DbType.Int32:
                    return SqlDbType.Int;
                case DbType.Int64:
                    return SqlDbType.BigInt;
                case DbType.Object:
                    return SqlDbType.Variant;
                case DbType.SByte:
                    return SqlDbType.TinyInt;
                case DbType.Single:
                    return SqlDbType.Real;
                case DbType.String:
                    return SqlDbType.NVarChar;
                case DbType.StringFixedLength:
                    return SqlDbType.NChar;
                case DbType.Time:
                    return SqlDbType.DateTime;
                case DbType.UInt16:
                    return SqlDbType.Int;
                case DbType.UInt32:
                    return SqlDbType.Int;
                case DbType.UInt64:
                    return SqlDbType.BigInt;
                case DbType.VarNumeric:
                    return SqlDbType.Decimal;

                default:
                    return SqlDbType.VarChar;
            }
        }

        /// <summary>
        /// Gets the string representation of the .NET type for a given DbType 
        /// </summary>
        /// <param name="dbType">The DbType to evaluate</param>
        /// <returns></returns>
        public static string GetSystemType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                    return "System.String";
                case DbType.AnsiStringFixedLength:
                    return "System.String";
                case DbType.Binary:
                    return "System.Byte[]";
                case DbType.Boolean:
                    return "System.Boolean";
                case DbType.Byte:
                    return "System.Byte";
                case DbType.Currency:
                    return "System.Decimal";
                case DbType.Date:
                    return "System.DateTime";
                case DbType.DateTime:
                    return "System.DateTime";
                case DbType.Decimal:
                    return "System.Decimal";
                case DbType.Double:
                    return "System.Double";
                case DbType.Guid:
                    return "System.Guid";
                case DbType.Int16:
                    return "System.Int16";
                case DbType.Int32:
                    return "System.Int32";
                case DbType.Int64:
                    return "System.Int64";
                case DbType.Object:
                    return "System.Object";
                case DbType.SByte:
                    return "System.SByte";
                case DbType.Single:
                    return "System.Single";
                case DbType.String:
                    return "System.String";
                case DbType.StringFixedLength:
                    return "System.String";
                case DbType.Time:
                    return "System.TimeSpan";
                case DbType.UInt16:
                    return "System.UInt16";
                case DbType.UInt32:
                    return "System.UInt32";
                case DbType.UInt64:
                    return "System.UInt64";
                case DbType.VarNumeric:
                    return "System.Decimal";
                default:
                    return "System.String";
            }
        }

        /// <summary>
        /// Converts the passed byte array to a string
        /// </summary>
        /// <param name="arrInput">The byte array to convert to a string</param>
        /// <returns></returns>
        public static string ByteArrayToString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length * 2);
            for(int i = 0; i < arrInput.Length; i++)
                sOutput.Append(arrInput[i].ToString("x2"));

            return sOutput.ToString();
        }

        /// <summary>
        /// Converts the passed string to a byte array.
        /// </summary>
        /// <param name="str">The string to convert to a byte array</param>
        /// <returns></returns>
        public static byte[] StringToByteArray(string str)
        {
            Encoding ascii = Encoding.ASCII;
            Encoding unicode = Encoding.Unicode;

            byte[] unicodeBytes = unicode.GetBytes(str);
            byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);
            return asciiBytes;
        }

        ///// <summary>
        ///// Prefixes the passed parameter value with the provider specific parameter prefix.
        ///// </summary>
        ///// <param name="parameter">The parameter to evaluate</param>
        ///// <param name="provider">The provider where the parameter will be used</param>
        ///// <returns></returns>
		public static string PrefixParameter(string parameter, DataProvider provider) {
			//added this check for Unit Testing weirdness - RC
			if (provider == null)
				provider = DataService.Provider;

			string prefix = provider.GetParameterPrefix();
			if (!parameter.StartsWith(prefix))
				parameter = String.Concat(prefix, parameter.Replace(" ", String.Empty));
			return parameter;
		}

        /// <summary>
        /// Helper method to format a specific passed string as a SQL function
        /// </summary>
        /// <param name="functionName">Name of the function</param>
        /// <param name="columnName">Name of the column that the function will evaluate</param>
        /// <param name="isDistinct">If set to <c>true</c> a DISTINCT query is performed</param>
        /// <param name="provider">The provider that the function is being created for</param>
        /// <returns></returns>
        public static string MakeFunction(string functionName, string columnName, bool isDistinct, DataProvider provider)
        {
            if(isDistinct)
                return String.Concat(functionName, "(", SqlFragment.DISTINCT, provider.FormatIdentifier(columnName), ")");

            return MakeFunction(functionName, columnName, provider);
        }

        /// <summary>
        /// Helper method to format a specific passed string as a SQL function
        /// </summary>
        /// <param name="functionName">Name of the function</param>
        /// <param name="columnName">Name of the column that the function will evaluate</param>
        /// <param name="provider">The provider that the function is being created for</param>
        /// <returns></returns>
        public static string MakeFunction(string functionName, string columnName, DataProvider provider)
        {
            return String.Concat(functionName, "(", provider.FormatIdentifier(columnName), ")");
        }

        /// <summary>
        /// Creates provider specfic string representation of a parameter assignment.
        /// For example, '@param = value'
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="provider">The provider that the parameter assignment is being created for</param>
        /// <returns></returns>
        public static string MakeParameterAssignment(string columnName, string parameterName, DataProvider provider)
        {
            return String.Concat(provider.FormatIdentifier(columnName), " = ", provider.FormatParameterNameForSQL(parameterName));
        }

		/// <summary>
		/// Fully qualifies qualifies a column name using the [table].[column] format (SQL Server),
		/// or other format appropriate to a given provider.
		/// </summary>
		/// <param name="tableName">Name of the table</param>
		/// <param name="columnName">Name of the column</param>
		/// <param name="provider">The provider that the format is being qualified for</param>
		/// <returns></returns>
		public static string QualifyColumnName(string tableName, string columnName, DataProvider provider) {
			return provider.QualifyColumnName("", tableName, columnName);
		}

		/// <summary>
		/// Fully qualifies a table reference using the [schema].[table] format (SQL Server),
		/// or other format appropriate to a given provider.
		/// </summary>
		/// <param name="schemaName">Name of the schema</param>
		/// <param name="tableName">Name of the table</param>
		/// <param name="provider">The provider that the format is being qualified for</param>
		/// <returns></returns>
		public static string QualifyTableName(string schemaName, string tableName, DataProvider provider) {
			return provider.QualifyTableName(schemaName, tableName);
		}

		/// <summary>
		/// Fully qualifies a table reference using the [schema].[table] format (SQL Server),
		/// or other format appropriate to a given provider.
		/// </summary>
		/// <param name="table">The TableSchema.Table whose name will be qualified</param>
		/// <returns></returns>
		public static string QualifyTableName(TableSchema.Table table) {
			return table.Provider.QualifyTableName(table.SchemaName, table.TableName);
		}

        /// <summary>
		/// Adds a string to a qualified name, inserting inside enclosing square brackets if necessary
		/// </summary>
		/// <param name="qname">The qualified name</param>
		/// <param name="addTo">The string to add</param>
		/// <returns></returns>
		public static string AddStringToQualifiedName(string qname, string addTo) {
			if (qname.EndsWith("]")) {
				return String.Concat(qname.Substring(0, qname.Length - 1), addTo, "]");
			}
			return qname + addTo;
		}

        /// <summary>
        /// Using a set of criteria including primary/foreign key references and positions,
        /// determines whether or not a given TableSchema.Table should be considered a mapping table.
        /// </summary>
        /// <param name="schema">The TableSchema.Table to evaluate.</param>
        /// <param name="relatedTableColumn">The name of a the related table column for evaluation</param>
        /// <returns>
        /// 	<c>true</c> if the it's a mapping table ; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMappingTable(TableSchema.Table schema, string relatedTableColumn)
        {
            if(schema.Columns.Count == 2)
            {
                if(schema.Columns[0].IsPrimaryKey && schema.Columns[0].IsForeignKey && schema.Columns[1].IsPrimaryKey && schema.Columns[1].IsForeignKey)
                {
                    if(IsMatch(schema.Columns[0].ColumnName, relatedTableColumn) || IsMatch(schema.Columns[1].ColumnName, relatedTableColumn))
                        return true;
                }
            }
            if(schema.Columns.Count == 3)
            {
                if((schema.Columns[0].IsPrimaryKey && !schema.Columns[0].IsForeignKey) && (schema.Columns[1].IsPrimaryKey && schema.Columns[1].IsForeignKey) &&
                   (schema.Columns[2].IsPrimaryKey && schema.Columns[2].IsForeignKey))
                {
                    if(IsMatch(schema.Columns[1].ColumnName, relatedTableColumn) || IsMatch(schema.Columns[2].ColumnName, relatedTableColumn))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Using a set of criteria including primary/foreign key references and positions,
        /// determines whether or not a given TableSchema.Table should be considered a mapping table.
        /// </summary>
        /// <param name="schema">The TableSchema.Table to evaluate.</param>
        /// <returns>
        /// 	<c>true</c> if the it's a mapping table ; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMappingTable(TableSchema.Table schema)
        {
            if(schema.Columns.Count == 2 && schema.Columns[0].IsPrimaryKey && schema.Columns[0].IsForeignKey && schema.Columns[1].IsPrimaryKey && schema.Columns[1].IsForeignKey)
                return true;

            if(schema.Columns.Count == 3 &&
               (schema.Columns[0].IsPrimaryKey && !schema.Columns[0].IsForeignKey) &&
               (schema.Columns[1].IsPrimaryKey && schema.Columns[1].IsForeignKey) &&
               (schema.Columns[2].IsPrimaryKey && schema.Columns[2].IsForeignKey))
                return true;

            return false;
        }

        /// <summary>
        /// Returns a default setting per data type
        /// </summary>
        /// <param name="column">The TableSchema.TableColumn to evaluate</param>
        /// <returns></returns>
        public static object GetDefaultSetting(TableSchema.TableColumn column)
        {
            if(column.IsNullable)
                return null;
            if(IsMatch(column.ColumnName, ReservedColumnName.CREATED_ON) || IsMatch(column.ColumnName, ReservedColumnName.MODIFIED_ON))
                return column.Table.Provider.Now;
            if(IsLogicalDeleteColumn(column.ColumnName))
                return false;

            switch(column.DataType)
            {
                case DbType.Xml:
                case DbType.String:
                case DbType.AnsiString:
                case DbType.StringFixedLength:
                case DbType.AnsiStringFixedLength:
                    return String.Empty;
                case DbType.Date:
                case DbType.DateTime:
                    return new DateTime(1900, 01, 01);
                case DbType.Boolean:
                    return false;
                case DbType.Guid:
                    return Guid.Empty;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Gets the default value for System.Web.UI.Control based on Control type and underly DbType of the
        /// mapped TableSchema.TableColumn
        /// </summary>
        /// <param name="col">The TableColumn mapped to the control</param>
        /// <param name="ctrl">The control that is being evaluated</param>
        /// <param name="isAdd">if set to <c>true</c> returns the value for a new object; otherwise the value for an updated one.</param>
        /// <param name="returnDBNull">If set to <c>true</c> will return DbNull values; otherwise return null</param>
        /// <returns></returns>
        public static object GetDefaultControlValue(TableSchema.TableColumn col, Control ctrl, bool isAdd, bool returnDBNull)
        {
            object oVal = null;
            string colName = col.ColumnName;

            if(IsMatch(colName, ReservedColumnName.MODIFIED_BY))
                oVal = HttpContext.Current.User.Identity.Name;
            else if(IsMatch(colName, ReservedColumnName.MODIFIED_ON))
                oVal = col.Table.Provider.Now;
            else if(IsMatch(colName, ReservedColumnName.CREATED_BY))
            {
                if(isAdd)
                    oVal = HttpContext.Current.User.Identity.Name;
                else if(ctrl != null)
                    oVal = ((Label)ctrl).Text;
            }
            else if(IsMatch(colName, ReservedColumnName.CREATED_ON))
            {
                if(isAdd)
                    oVal = col.Table.Provider.Now;
                else if(ctrl != null)
                    oVal = ((Label)ctrl).Text;
            }
            else if(ctrl is TextBox)
                oVal = ((TextBox)ctrl).Text;
            else if(ctrl is CheckBox)
                oVal = ((CheckBox)ctrl).Checked;
            else if(ctrl is DropDownList)
                oVal = ((DropDownList)ctrl).SelectedValue;
            else if(ctrl is Calendar)
            {
                Calendar cal = (Calendar)ctrl;
                if(cal.SelectedDate > DateTime.MinValue)
                    oVal = ((Calendar)ctrl).SelectedDate;
                else
                {
                    if(!col.IsNullable)
                        oVal = col.Table.Provider.Now;
                }
            }
            else if(ctrl is Label)
                oVal = ((Label)ctrl).Text;

            if(!col.IsPrimaryKey && !col.AutoIncrement)
            {
                if(oVal == null || oVal.ToString().Length == 0)
                {
                    if(col.IsNullable)
                        oVal = returnDBNull ? DBNull.Value : null;
                    else
                        oVal = GetDefaultSetting(col);
                }
            }
            return oVal;
        }

        /// <summary>
        /// Returns an Object with the specified Type and whose value is equivalent to the specified object.
        /// </summary>
        /// <param name="value">An Object that implements the IConvertible interface.</param>
        /// <param name="conversionType">The Type to which value is to be converted.</param>
        /// <returns>
        /// An object whose Type is conversionType (or conversionType's underlying type if conversionType
        /// is Nullable&lt;&gt;) and whose value is equivalent to value. -or- a null reference, if value is a null
        /// reference and conversionType is not a value type.
        /// </returns>
        /// <remarks>
        /// This method exists as a workaround to System.Convert.ChangeType(Object, Type) which does not handle
        /// nullables as of version 2.0 (2.0.50727.42) of the .NET Framework. The idea is that this method will
        /// be deleted once Convert.ChangeType is updated in a future version of the .NET Framework to handle
        /// nullable types, so we want this to behave as closely to Convert.ChangeType as possible.
        /// This method was written by Peter Johnson at:
        /// http://aspalliance.com/author.aspx?uId=1026.
        /// </remarks>
        public static object ChangeType(object value, Type conversionType)
        {
            // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
            // checking properties on conversionType below.
            if(conversionType == null)
                throw new ArgumentNullException("conversionType");

            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType

            if(conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
                // InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
                // determine what the underlying type is
                // If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
                // have a type--so just return null
                // Note: We only do this check if we're converting to a nullable type, since doing it outside
                // would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
                // value is null and conversionType is a value type.
                if(value == null)
                    return null;

                // It's a nullable type, and not null, so that means it can be converted to its underlying type,
                // so overwrite the passed-in conversion type with this underlying type
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
            // nullable type), pass the call on to Convert.ChangeType
            return Convert.ChangeType(value, conversionType);
        }

        #endregion


        #region String Manipulations

        /// <summary>
        /// Reformats the passed string from camelCase to ProperCase
        /// </summary>
        /// <param name="sIn">The string to reformat to proper case</param>
        /// <returns></returns>
        public static string ParseCamelToProper(string sIn)
        {
            //No transformation if string is alread all caps
            if(Validation.IsUpperCase(sIn))
                return sIn;

            char[] letters = sIn.ToCharArray();
            StringBuilder sOut = new StringBuilder();
            int index = 0;

            if(sIn.Contains("ID"))
            {
                //just upper the first letter
                sOut.Append(letters[0]);
                sOut.Append(sIn.Substring(1, sIn.Length - 1));
            }
            else
            {
                foreach(char c in letters)
                {
                    if(index == 0)
                    {
                        sOut.Append(" ");
                        sOut.Append(c.ToString().ToUpper());
                    }
                    else if(Char.IsUpper(c))
                    {
                        //it's uppercase, add a space
                        sOut.Append(" ");
                        sOut.Append(c);
                    }
                    else
                        sOut.Append(c);
                    index++;
                }
            }
            string strClean = sOut.ToString().Trim();
            return Regex.Replace(strClean, "(?<=[A-Z]) (?=[A-Z])", String.Empty);
            //return sOut.ToString().Trim();
        }

        /// <summary>
        /// Reformats the passed string to ProperCase
        /// </summary>
        /// <param name="sIn">The string to reformat to proper case</param>
        /// <returns></returns>
        public static string GetProperName(string sIn)
        {
            string propertyName = Inflector.ToPascalCase(sIn);
            //if(propertyName.EndsWith("TypeCode"))
            //    propertyName = propertyName.Substring(0, propertyName.Length - 4);

            return propertyName;
        }

        /// <summary>
        /// Gets the name of the proper.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string GetProperName(string sIn, DataProvider provider)
        {
            string propertyName = sIn;
            if(provider.FixDatabaseObjectCasing)
                propertyName = Inflector.ToPascalCase(propertyName, provider.RemoveUnderscores);
            //if(propertyName.EndsWith("TypeCode"))
            //    propertyName = propertyName.Substring(0, propertyName.Length - 4);
            return propertyName;
        }

        /// <summary>
        /// Plurals to singular.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <returns></returns>
        public static string PluralToSingular(string sIn)
        {
            return Inflector.MakeSingular(sIn);
        }

        /// <summary>
        /// Singulars to plural.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <returns></returns>
        public static string SingularToPlural(string sIn)
        {
            return Inflector.MakePlural(sIn);
        }

        /// <summary>
        /// Keys the word check.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="table">The table.</param>
        /// <param name="appendWith">The append with.</param>
        /// <returns></returns>
        public static string KeyWordCheck(string word, string table, string appendWith)
        {
            string newWord = String.Concat(word, appendWith);

            //if(word == "Schema")
            //    newWord =  word + appendWith;

            //Can't have a property with same name as class.
            if(word == table)
                return newWord;

            string evalWord = word.ToLower();

            switch(evalWord)
            {
                    // C# keywords
                case "abstract":
                case "as":
                case "base":
                case "bool":
                case "break":
                case "byte":
                case "case":
                case "catch":
                case "char":
                case "checked":
                case "class":
                case "const":
                case "continue":
                case "date":
                case "datetime":
                case "decimal":
                case "default":
                case "delegate":
                case "do":
                case "double":
                case "else":
                case "enum":
                case "event":
                case "explicit":
                case "extern":
                case "false":
                case "finally":
                case "fixed":
                case "float":
                case "for":
                case "foreach":
                case "goto":
                case "if":
                case "implicit":
                case "in":
                case "int":
                case "interface":
                case "internal":
                case "is":
                case "lock":
                case "long":
                case "namespace":
                case "new":
                case "null":
                case "object":
                case "operator":
                case "out":
                case "override":
                case "params":
                case "private":
                case "protected":
                case "public":
                case "readonly":
                case "ref":
                case "return":
                case "sbyte":
                case "sealed":
                case "short":
                case "sizeof":
                case "stackalloc":
                case "static":
                case "string":
                case "struct":
                case "switch":
                case "this":
                case "throw":
                case "true":
                case "try":
                case "typecode":
                case "typeof":
                case "uint":
                case "ulong":
                case "unchecked":
                case "unsafe":
                case "ushort":
                case "using":
                case "virtual":
                case "volatile":
                case "void":
                case "while":
                    // C# contextual keywords
                case "get":
                case "partial":
                case "set":
                case "value":
                case "where":
                case "yield":
                    // VB.NET keywords (commented out keywords that are the same as in C#)
                case "alias":
                case "addHandler":
                case "ansi":
                    //case "as": 
                case "assembly":
                case "auto":
                case "binary":
                case "byref":
                case "byval":
                case "custom":
                case "directcast":
                case "each":
                case "elseif":
                case "end":
                case "error":
                case "friend":
                case "global":
                case "handles":
                case "implements":
                case "lib":
                case "loop":
                case "me":
                case "module":
                case "mustinherit":
                case "mustoverride":
                case "mybase":
                case "myclass":
                case "narrowing":
                case "next":
                case "nothing":
                case "notinheritable":
                case "notoverridable":
                case "of":
                case "off":
                case "on":
                case "option":
                case "optional":
                case "overloads":
                case "overridable":
                case "overrides":
                case "paramarray":
                case "preserve":
                case "property":
                case "raiseevent":
                case "resume":
                case "shadows":
                case "shared":
                case "step":
                case "structure":
                case "text":
                case "then":
                case "to":
                case "trycast":
                case "unicode":
                case "until":
                case "when":
                case "widening":
                case "withevents":
                case "writeonly":
                    // VB.NET unreserved keywords
                case "compare":
                case "isfalse":
                case "istrue":
                case "mid":
                case "strict":
                case "schema":
                    return newWord;
                default:
                    return word;
            }
        }

        /// <summary>
        /// Keys the word check.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="table">The table.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string KeyWordCheck(string word, string table, DataProvider provider)
        {
            string appendWith = "X";
            if(!String.IsNullOrEmpty(provider.AppendWith))
                appendWith = provider.AppendWith;

            return KeyWordCheck(word, table, appendWith);
        }

        /// <summary>
        /// Keys the word VB check.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="table">The table.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string KeyWordVBCheck(string word, string table, DataProvider provider)
        {
            return KeyWordCheck(word, table, provider);
        }

        /// <summary>
        /// Fasts the replace.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns></returns>
        public static string FastReplace(string original, string pattern, string replacement)
        {
            return FastReplace(original, pattern, replacement, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Fasts the replace.
        /// </summary>
        /// <param name="original">The original.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="replacement">The replacement.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <returns></returns>
        public static string FastReplace(string original, string pattern, string replacement, StringComparison comparisonType)
        {
            if(original == null)
                return null;

            if(String.IsNullOrEmpty(pattern))
                return original;

            int lenPattern = pattern.Length;
            int idxPattern = -1;
            int idxLast = 0;

            StringBuilder result = new StringBuilder();

            while(true)
            {
                idxPattern = original.IndexOf(pattern, idxPattern + 1, comparisonType);

                if(idxPattern < 0)
                {
                    result.Append(original, idxLast, original.Length - idxLast);
                    break;
                }

                result.Append(original, idxLast, idxPattern - idxLast);
                result.Append(replacement);

                idxLast = idxPattern + lenPattern;
            }

            return result.ToString();
        }

        /// <summary>
        /// Strips the text.
        /// </summary>
        /// <param name="inputString">The input string.</param>
        /// <param name="stripString">The strip string.</param>
        /// <returns></returns>
        public static string StripText(string inputString, string stripString)
        {
            if(!String.IsNullOrEmpty(stripString))
            {
                string[] replace = stripString.Split(new[] {','});
                for(int i = 0; i < replace.Length; i++)
                {
                    if(!String.IsNullOrEmpty(inputString))
                        inputString = Regex.Replace(inputString, replace[i], String.Empty);
                }
            }
            return inputString;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string GetParameterName(string name, DataProvider provider)
        {
            if(String.IsNullOrEmpty(name))
                return String.Empty;

            string newName = name;
            newName = Replace(newName, provider.StripParamText, String.Empty, provider.RemoveUnderscores);
            newName = GetProperName(newName, provider);
            newName = IsStringNumeric(newName) ? String.Concat("_", newName) : newName;
            newName = StripNonAlphaNumeric(newName);
            newName = newName.Replace("@", String.Empty);
            newName = newName.Trim();
            return KeyWordCheck(newName, String.Empty, provider);
        }

        /// <summary>
        /// Replaces most non-alpha-numeric chars
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <returns></returns>
        public static string StripNonAlphaNumeric(string sIn)
        {
            return StripNonAlphaNumeric(sIn, " ".ToCharArray()[0]);
        }

        /// <summary>
        /// Replaces most non-alpha-numeric chars
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <param name="cReplace">The placeholder character to use for replacement</param>
        /// <returns></returns>
        public static string StripNonAlphaNumeric(string sIn, char cReplace)
        {
            StringBuilder sb = new StringBuilder(sIn);
            //these are illegal characters - remove zem
            const string stripList = ".'?\\/><$!@%^*&+,;:\"(){}[]|-#";

            for(int i = 0; i < stripList.Length; i++)
                sb.Replace(stripList[i], cReplace);

            sb.Replace(cReplace.ToString(), String.Empty);
            return sb.ToString();
        }

        /// <summary>
        /// Replaces any matches found in word from list.
        /// </summary>
        /// <param name="word">The string to check against.</param>
        /// <param name="find">A comma separated list of values to replace.</param>
        /// <param name="replaceWith">The value to replace with.</param>
        /// <param name="removeUnderscores">Whether or not underscores will be kept.</param>
        /// <returns></returns>
        public static string Replace(string word, string find, string replaceWith, bool removeUnderscores)
        {
            string[] findList = Split(find);
            string newWord = word;
            foreach(string f in findList)
            {
                if(f.Length > 0)
                    newWord = newWord.Replace(f, replaceWith);
            }

            if(removeUnderscores)
                return newWord.Replace(" ", String.Empty).Replace("_", String.Empty).Trim();
            return newWord.Replace(" ", String.Empty).Trim();
        }

        /// <summary>
        /// Finds a match in word using comma separted list.
        /// </summary>
        /// <param name="word">The string to check against.</param>
        /// <param name="list">A comma separted list of values to find.</param>
        /// <returns>
        /// true if a match is found or list is empty, otherwise false.
        /// </returns>
        public static bool StartsWith(string word, string list)
        {
            if(string.IsNullOrEmpty(list))
                return true;

            string[] find = Split(list);

            foreach(string f in find)
            {
                if(word.StartsWith(f, StringComparison.CurrentCultureIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// A custom split method
        /// </summary>
        /// <param name="list">A list of values separated by either ", " or ","</param>
        /// <returns></returns>
        public static string[] Split(string list)
        {
            string[] find;
            try
            {
                find = list.Split(new[] {", ", ","}, StringSplitOptions.RemoveEmptyEntries);
            }
            catch
            {
                find = new[] {String.Empty};
            }
            return find;
        }

        /// <summary>
        /// Shortens the text.
        /// </summary>
        /// <param name="sIn">The s in.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        public static string ShortenText(object sIn, int length)
        {
            string sOut = sIn.ToString();
            if(sOut.Length > length)
                sOut = String.Concat(sOut.Substring(0, length), " ...");

            return sOut;
        }

        /// <summary>
        /// Gets the length of the effective max.
        /// </summary>
        /// <param name="tableColumn">The table column.</param>
        /// <returns></returns>
        public static int GetEffectiveMaxLength(TableSchema.TableColumn tableColumn)
        {
            if(tableColumn.DataType == DbType.String && tableColumn.MaxLength == -1)
                return Int32.MaxValue;

            return tableColumn.MaxLength;
        }

        public static TableSchema.TableColumn GetDisplayTableColumn(TableSchema.Table table)
        {
            if(table.Columns.Count == 1)
                return table.Columns[0];
            if(table.Columns.Count > 1 && table.Columns[1].IsString)
                return table.Columns[1];

            foreach(TableSchema.TableColumn column in table.Columns)
            {
                if(column.IsString)
                    return column;
            }
            return table.Columns[1];
        }

        /// <summary>
        /// Checks the length of the string.
        /// </summary>
        /// <param name="stringToCheck">The string to check.</param>
        /// <param name="maxLength">Length of the max.</param>
        /// <returns></returns>
        public static string CheckStringLength(string stringToCheck, int maxLength)
        {
            string checkedString;

            if(stringToCheck.Length <= maxLength)
                return stringToCheck;

            // If the string to check is longer than maxLength 
            // and has no whitespace we need to trim it down.
            if((stringToCheck.Length > maxLength) && (stringToCheck.IndexOf(" ") == -1))
                checkedString = String.Concat(stringToCheck.Substring(0, maxLength), "...");
            else if(stringToCheck.Length > 0)
                checkedString = String.Concat(stringToCheck.Substring(0, maxLength), "...");
            else
                checkedString = stringToCheck;

            return checkedString;
        }

        [Obsolete("Obsolete and marked for removal. Update references to use Sugar.Strings.StripHTML()")]
        public static string StripHTML(string htmlString)
        {
            return Strings.StripHTML(htmlString);
        }

        [Obsolete("Obsolete and marked for removal. Update references to use Sugar.Strings.StripHTML()")]
        public static string StripHTML(string htmlString, string htmlPlaceHolder)
        {
            return Strings.StripHTML(htmlString, htmlPlaceHolder);
        }

        [Obsolete("Obsolete and marked for removal.")]
        public static string StripHTML(string htmlString, string htmlPlaceHolder, bool stripExcessSpaces)
        {
            const string pattern = @"<(.|\n)*?>";
            string sOut = Regex.Replace(htmlString, pattern, htmlPlaceHolder);
            sOut = sOut.Replace("&nbsp;", String.Empty);
            sOut = sOut.Replace("&amp;", "&");

            if(stripExcessSpaces)
            {
                // If there is excess whitespace, this will remove
                // like "THE      WORD".
                char[] delim = {' '};
                string[] lines = sOut.Split(delim, StringSplitOptions.RemoveEmptyEntries);

                //sOut = "";
                StringBuilder sb = new StringBuilder();
                foreach(string s in lines)
                {
                    sb.Append(s);
                    sb.Append(" ");
                }
                return sb.ToString().Trim();
            }
            return sOut;
        }

        #endregion


        #region Conversions

        /// <summary>
        /// Strings to enum.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        public static object StringToEnum(Type t, string Value)
        {
            object oOut = null;

            foreach(FieldInfo fi in t.GetFields())
            {
                if(IsMatch(fi.Name, Value))
                    oOut = fi.GetValue(null);
            }

            return oOut;
        }

        #endregion


        #region URL Related

        /// <summary>
        /// Gets the site root.
        /// </summary>
        /// <returns></returns>
        public static string GetSiteRoot()
        {
            string port = HttpContext.Current.Request.ServerVariables[ServerVariable.SERVER_PORT];

            if(port == null || port == Ports.HTTP || port == Ports.HTTPS)
                port = String.Empty;
            else
                port = String.Concat(":", port);

            string protocol = HttpContext.Current.Request.ServerVariables[ServerVariable.SERVER_PORT_SECURE];
            if(protocol == null || protocol == "0")
                protocol = ProtocolPrefix.HTTP;
            else
                protocol = ProtocolPrefix.HTTPS;

            string appPath = HttpContext.Current.Request.ApplicationPath;

            if(appPath == "/")
                appPath = String.Empty;

            string sOut = protocol + HttpContext.Current.Request.ServerVariables[ServerVariable.SERVER_NAME] + port + appPath;

            return sOut;
        }

        /// <summary>
        /// Gets the parameter.
        /// </summary>
        /// <param name="sParam">The s param.</param>
        /// <returns></returns>
        public static string GetParameter(string sParam)
        {
            if(HttpContext.Current.Request.QueryString[sParam] != null)
                return HttpContext.Current.Request[sParam];
            return String.Empty;
        }

        /// <summary>
        /// Gets the int parameter.
        /// </summary>
        /// <param name="sParam">The s param.</param>
        /// <returns></returns>
        public static int GetIntParameter(string sParam)
        {
            int iOut = 0;
            if(HttpContext.Current.Request.QueryString[sParam] != null)
            {
                string sOut = HttpContext.Current.Request[sParam];
                if(!String.IsNullOrEmpty(sOut))
                    int.TryParse(sOut, out iOut);
            }
            return iOut;
        }

        /// <summary>
        /// Gets the GUID parameter.
        /// </summary>
        /// <param name="sParam">The s param.</param>
        /// <returns></returns>
        public static Guid GetGuidParameter(string sParam)
        {
            Guid gOut = Guid.Empty;
            if(HttpContext.Current.Request.QueryString[sParam] != null)
            {
                string sOut = HttpContext.Current.Request[sParam];
                if(Validation.IsGuid(sOut))
                    gOut = new Guid(sOut);
            }
            return gOut;
        }

        #endregion


        #region Random Generators

        /// <summary>
        /// Gets the random string.
        /// </summary>
        /// <returns></returns>
        public static string GetRandomString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(RandomString(4, false));
            builder.Append(RandomInt(1000, 9999));
            builder.Append(RandomString(2, false));
            return builder.ToString();
        }

        /// <summary>
        /// Randoms the string.
        /// </summary>
        /// <param name="size">The size.</param>
        /// <param name="lowerCase">if set to <c>true</c> [lower case].</param>
        /// <returns></returns>
        private static string RandomString(int size, bool lowerCase)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            for(int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(26 * random.NextDouble() + 65));
                builder.Append(ch);
            }
            if(lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        /// <summary>
        /// Randoms the int.
        /// </summary>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns></returns>
        private static int RandomInt(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        #endregion


        #region Lists

        /// <summary>
        /// Loads the drop down.
        /// </summary>
        /// <param name="ddl">The DDL.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="textField">The text field.</param>
        /// <param name="valueField">The value field.</param>
        /// <param name="initialSelection">The initial selection.</param>
        public static void LoadDropDown(ListControl ddl, ICollection collection, string textField, string valueField, string initialSelection)
        {
            ddl.DataSource = collection;
            ddl.DataTextField = textField;
            ddl.DataValueField = valueField;
            ddl.DataBind();

            ddl.SelectedValue = initialSelection;
        }

        /// <summary>
        /// Loads the drop down.
        /// </summary>
        /// <param name="listControl">The list control.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="closeReader">if set to <c>true</c> [close reader].</param>
        public static void LoadDropDown(ListControl listControl, IDataReader dataReader, bool closeReader)
        {
            listControl.Items.Clear();

            if(closeReader)
            {
                using(dataReader)
                {
                    while(dataReader.Read())
                    {
                        string sText = dataReader[1].ToString();
                        string sVal = dataReader[0].ToString();
                        listControl.Items.Add(new ListItem(sText, sVal));
                    }
                    dataReader.Close();
                }
            }
            else
            {
                while(dataReader.Read())
                {
                    string sText = dataReader[1].ToString();
                    string sVal = dataReader[0].ToString();
                    listControl.Items.Add(new ListItem(sText, sVal));
                }
            }
        }

        /// <summary>
        /// Loads the list items.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="tblBind">The TBL bind.</param>
        /// <param name="tblVals">The TBL vals.</param>
        /// <param name="textField">The text field.</param>
        /// <param name="valField">The val field.</param>
        public static void LoadListItems(ListItemCollection list, DataTable tblBind, DataTable tblVals, string textField, string valField)
        {
            for(int i = 0; i < tblBind.Rows.Count; i++)
            {
                ListItem l = new ListItem(tblBind.Rows[i][textField].ToString(), tblBind.Rows[i][valField].ToString());

                for(int x = 0; x < tblVals.Rows.Count; x++)
                {
                    DataRow dr = tblVals.Rows[x];
                    if(IsMatch(dr[valField].ToString(), l.Value))
                        l.Selected = true;
                }
                list.Add(l);
            }
        }

        /// <summary>
        /// Loads the list items.
        /// </summary>
        /// <param name="listCollection">The list.</param>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="textField">The text field.</param>
        /// <param name="valueField">The value field.</param>
        /// <param name="selectedValue">The selected value.</param>
        /// <param name="closeReader">if set to <c>true</c> [close reader].</param>
        public static void LoadListItems(ListItemCollection listCollection, IDataReader dataReader, string textField, string valueField, string selectedValue, bool closeReader)
        {
            listCollection.Clear();

            if(closeReader)
            {
                using(dataReader)
                {
                    while(dataReader.Read())
                    {
                        string sText = dataReader[textField].ToString();
                        string sVal = dataReader[valueField].ToString();

                        ListItem l = new ListItem(sText, sVal);
                        if(!String.IsNullOrEmpty(selectedValue))
                            l.Selected = IsMatch(selectedValue, sVal);
                        listCollection.Add(l);
                    }
                    dataReader.Close();
                }
            }
            else
            {
                while(dataReader.Read())
                {
                    string sText = dataReader[textField].ToString();
                    string sVal = dataReader[valueField].ToString();

                    ListItem l = new ListItem(sText, sVal);
                    if(!String.IsNullOrEmpty(selectedValue))
                        l.Selected = IsMatch(selectedValue, sVal);
                    listCollection.Add(l);
                }
            }
        }

        /// <summary>
        /// Sets the list selection.
        /// </summary>
        /// <param name="lc">The lc.</param>
        /// <param name="Selection">The selection.</param>
        public static void SetListSelection(ListItemCollection lc, string Selection)
        {
            for(int i = 0; i < lc.Count; i++)
            {
                if(lc[i].Value == Selection)
                {
                    lc[i].Selected = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Regexes the transform.
        /// </summary>
        /// <param name="inputText">The input text.</param>
        /// <param name="provider">The provider.</param>
        /// <returns></returns>
        public static string RegexTransform(string inputText, DataProvider provider)
        {
            if(provider.UseRegexReplace)
            {
                Regex rx;

                if(!String.IsNullOrEmpty(provider.RegexMatchExpression))
                {
                    rx = provider.RegexIgnoreCase ? new Regex(provider.RegexMatchExpression, RegexOptions.IgnoreCase) : new Regex(provider.RegexMatchExpression);
                    inputText = rx.Replace(inputText, provider.RegexReplaceExpression);
                }

                if(!String.IsNullOrEmpty(provider.RegexDictionaryReplace) && !String.IsNullOrEmpty(inputText))
                {
                    string regexString = Regex.Replace(provider.RegexDictionaryReplace, "[\r\n\t]", String.Empty);

                    string[] pairs = Regex.Split(regexString, ";");
                    foreach(string pair in pairs)
                    {
                        string[] keys = Regex.Split(pair, ",");
                        if(keys.Length == 2)
                        {
                            rx = provider.RegexIgnoreCase ? new Regex(keys[0], RegexOptions.IgnoreCase) : new Regex(keys[0]);
                            inputText = rx.Replace(inputText, keys[1]);
                        }
                    }
                }
            }
            return inputText;
        }

        #endregion


        #region Obsolete

        [Obsolete("Obsolete and marked for removal. Kept for compatibility with 2.0.1 Starter Site. Update references to use Sugar.Files.GetFileText()")]
        public static string GetFileText(string absolutePath)
        {
            return Files.GetFileText(absolutePath);
        }

        [Obsolete("Obsolete and marked for removal.")]
        public static string ToggleHtmlBR(string text, bool isOn)
        {
            string outS;

            if(isOn)
                outS = text.Replace(Environment.NewLine, "<br />");
            else
            {
                // TODO: do this with via regex
                //
                outS = text.Replace("<br />", Environment.NewLine);
                outS = outS.Replace("<br>", Environment.NewLine);
                outS = outS.Replace("<br >", Environment.NewLine);
            }

            return outS;
        }

        [Obsolete("Obsolete and marked for removal.")]
        public static string FormatDate(DateTime theDate)
        {
            return FormatDate(theDate, false, null);
        }

        [Obsolete("Obsolete and marked for removal.")]
        public static string FormatDate(DateTime theDate, bool showTime)
        {
            return FormatDate(theDate, showTime, null);
        }

        [Obsolete("Obsolete and marked for removal.")]
        public static string FormatDate(DateTime theDate, bool showTime, string pattern)
        {
            const string defaultDatePattern = "MMMM d, yyyy";
            const string defaultTimePattern = "hh:mm tt";

            if(pattern == null)
            {
                if(showTime)
                    pattern = defaultDatePattern + " " + defaultTimePattern;
                else
                    pattern = defaultDatePattern;
            }

            return theDate.ToString(pattern);
        }

        #endregion
    }
}