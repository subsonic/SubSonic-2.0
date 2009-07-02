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
using Microsoft.CSharp;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the CSharpCodeLanguage class
    /// </summary>
    public class CSharpCodeLanguage : ICodeLanguage
    {
        private readonly string[] keywords =
            {
                // keywords
                "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class",
                "const", "continue", "date", "datetime", "decimal", "default", "delegate", "do", "double", "else",
                "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto",
                "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new",
                "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly",
                "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct",
                "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort",
                "using", "virtual", "volatile", "void", "while",
                // contextual keywords
                "get", "partial", "set", "value", "where", "yield"
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
                    @"using System; 
using System.Text; 
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration; 
using System.Xml; 
using System.Xml.Serialization;
using SubSonic; 
using SubSonic.Utilities;
";
            }
        }

        /// <summary>
        /// Gets the code provider.
        /// </summary>
        /// <value>The code provider.</value>
        public string CodeProvider
        {
            get { return "CSharpCodeProvider"; }
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <value>The file extension.</value>
        public string FileExtension
        {
            get { return SubSonic.FileExtension.DOT_CS; }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier
        {
            get { return "C#"; }
        }

        /// <summary>
        /// Gets the short name.
        /// </summary>
        /// <value>The short name.</value>
        public string ShortName
        {
            get { return "cs"; }
        }

        /// <summary>
        /// Gets the template prefix.
        /// </summary>
        /// <value>The template prefix.</value>
        public string TemplatePrefix
        {
            get { return "CS_"; }
        }

        #endregion


        #region Methods

        /// <summary>
        /// Creates the code provider.
        /// </summary>
        /// <returns></returns>
        public CodeDomProvider CreateCodeProvider()
        {
            return new CSharpCodeProvider();
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
                variableType = String.Concat(variableType, CodeFragment.NULLABLE_VARIABLE);
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
                usingStatements.AppendFormat("using {0};", space);
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
                variableType = String.Concat(variableType, CodeFragment.NULLABLE_VARIABLE);
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
                return "false";

            switch(dbType)
            {
                case DbType.Guid:
                    return "Guid.Empty";
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return "null";
                case DbType.Boolean:
                    return "false";
                case DbType.Binary:
                    return "null";
                case DbType.Xml:
                    return String.Empty;
                case DbType.Date:
                case DbType.DateTime:
                    return "new DateTime(1900, 01, 01)";
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
                    return "string";

                case DbType.Binary:
                    return "byte[]";
                case DbType.Boolean:
                    return "bool";
                case DbType.Byte:
                    return "byte";

                case DbType.Currency:
                case DbType.Decimal:
                case DbType.VarNumeric:
                    return "decimal";

                case DbType.Date:
                case DbType.DateTime:
                    return "DateTime";

                case DbType.Double:
                    return "double";
                case DbType.Guid:
                    return "Guid";
                case DbType.Int16:
                    return "short";
                case DbType.Int32:
                    return "int";
                case DbType.Int64:
                    return "long";
                case DbType.Object:
                    return "object";
                case DbType.SByte:
                    return "sbyte";
                case DbType.Single:
                    return "float";
                case DbType.Time:
                    return "TimeSpan";
                case DbType.UInt16:
                    return "ushort";
                case DbType.UInt32:
                    return "uint";
                case DbType.UInt64:
                    return "ulong";

                default:
                    return "string";
            }
        }

        #endregion
    }
}