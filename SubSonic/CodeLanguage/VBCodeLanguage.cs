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
using System.CodeDom.Compiler;
using System.Data;
using System.Text;
using Microsoft.VisualBasic;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the VBCodeLanguage class
    /// </summary>
    public class VBCodeLanguage : ICodeLanguage
    {
        private readonly string[] keywords =
            {
                // keywords
                "alias", "addHandler", "ansi", "as", "assembly", "auto", "binary", "byref", "byval", "case", "catch", "class",
                "custom", "date", "datetime", "default", "directcast", "each", "else", "elseif", "end", "error", "false",
                "finally", "for", "friend", "global", "handles", "implements", "in", "is", "lib", "loop", "me", "module",
                "mustinherit", "mustoverride", "mybase", "myclass", "narrowing", "new", "next", "nothing", "notinheritable",
                "notoverridable", "of", "off", "on", "option", "optional", "overloads", "overridable", "overrides", "paramarray",
                "partial", "preserve", "private", "property", "protected", "public", "raiseevent", "readonly", "resume", "shadows",
                "shared", "static", "step", "structure", "text", "then", "to", "true", "trycast", "unicode", "until", "when",
                "while", "widening", "withevents", "writeonly",
                // unreserved keywords
                "compare", "explicit", "isfalse", "istrue", "mid", "strict"
            };


        #region Properties

        /// <summary>
        /// Gets the default using statements.
        /// </summary>
        /// <value>The default using statements.</value>
        public string DefaultUsingStatements
        {
            get
            {
                return
                    @"Imports System 
Imports System.Text 
Imports System.Data
Imports System.Data.SqlClient
Imports System.Data.Common
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Configuration 
Imports System.Xml 
Imports System.Xml.Serialization
Imports SubSonic 
Imports SubSonic.Utilities
";
            }
        }

        /// <summary>
        /// Gets the code provider.
        /// </summary>
        /// <value>The code provider.</value>
        public string CodeProvider
        {
            get { return "VBCodeProvider"; }
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <value>The file extension.</value>
        public string FileExtension
        {
            get { return SubSonic.FileExtension.DOT_VB; }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier
        {
            get { return "VB.NET"; }
        }

        /// <summary>
        /// Gets the short name.
        /// </summary>
        /// <value>The short name.</value>
        public string ShortName
        {
            get { return "vb"; }
        }

        /// <summary>
        /// Gets the template prefix.
        /// </summary>
        /// <value>The template prefix.</value>
        public string TemplatePrefix
        {
            get { return "VB_"; }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates the code provider.
        /// </summary>
        /// <returns></returns>
        public CodeDomProvider CreateCodeProvider()
        {
            return new VBCodeProvider();
        }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="colName">Name of the col.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="isNullableColumn">if set to <c>true</c> [is nullable column].</param>
        /// <returns></returns>
        public string GetDefaultValue(string colName, DbType dbType, bool isNullableColumn)
        {
            string variableType = GetDefaultValue(colName, dbType);
            if(isNullableColumn && Utility.IsNullableDbType(dbType))
                variableType = String.Format(CodeFragment.NULLABLE_VARIABLE_VB, variableType);
            return variableType;
        }

        /// <summary>
        /// Gets the using statements.
        /// </summary>
        /// <param name="namespaces">The namespaces.</param>
        /// <returns></returns>
        public string GetUsingStatements(string[] namespaces)
        {
            StringBuilder usingStatements = new StringBuilder();

            foreach(string space in namespaces)
            {
                usingStatements.AppendFormat("Imports {0}", space);
                usingStatements.Append(Environment.NewLine);
            }

            return usingStatements.ToString();
        }

        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="isNullableColumn">if set to <c>true</c> [is nullable column].</param>
        /// <returns></returns>
        public string GetVariableType(DbType dbType, bool isNullableColumn)
        {
            string variableType = GetVariableType(dbType);
            if(isNullableColumn && Utility.IsNullableDbType(dbType))
                variableType = String.Format(CodeFragment.NULLABLE_VARIABLE_VB, variableType);
            return variableType;
        }

        /// <summary>
        /// Determines whether the specified word is keyword.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>
        /// 	<c>true</c> if the specified word is keyword; otherwise, <c>false</c>.
        /// </returns>
        public bool IsKeyword(string word)
        {
            return Array.IndexOf(keywords, word.ToLower()) != -1;
        }

        #endregion


        #region Private methods

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="colName">Name of the col.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <returns></returns>
        private static string GetDefaultValue(string colName, DbType dbType)
        {
            if(Utility.IsLogicalDeleteColumn(colName))
                return "False";

            switch(dbType)
            {
                case DbType.Guid:
                    return "Guid.Empty";
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return "Nothing";
                case DbType.Boolean:
                    return "False";
                case DbType.Binary:
                    return "Nothing";
                case DbType.Xml:
                    return String.Empty;
                case DbType.Date:
                case DbType.DateTime:
                    return "New Date(1900,01,01)";
                default:
                    return "0";
            }
        }

        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <returns></returns>
        private static string GetVariableType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return "String";

                case DbType.Binary:
                    return "Byte()";
                case DbType.Boolean:
                    return "Boolean";
                case DbType.Byte:
                    return "Byte";

                case DbType.Currency:
                case DbType.Decimal:
                case DbType.VarNumeric:
                    return "Decimal";

                case DbType.Date:
                case DbType.DateTime:
                    return "DateTime";

                case DbType.Double:
                    return "Double";
                case DbType.Guid:
                    return "Guid";
                case DbType.Int16:
                    return "Short";
                case DbType.Int32:
                    return "Integer";
                case DbType.Int64:
                    return "Long";
                case DbType.Object:
                    return "Object";
                case DbType.SByte:
                    return "SByte";
                case DbType.Single:
                    return "Single";

                case DbType.Time:
                    return "TimeSpan";
                case DbType.UInt16:
                    return "UShort";
                case DbType.UInt32:
                    return "UInteger";
                case DbType.UInt64:
                    return "ULong";

                default:
                    return "String";
            }
        }

        #endregion
    }
}