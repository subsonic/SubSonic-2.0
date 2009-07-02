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

using System.CodeDom.Compiler;
using System.Data;

namespace SubSonic
{
    /// <summary>
    /// Summary for the ICodeLanguage interface
    /// </summary>
    public interface ICodeLanguage
    {
        #region Properties

        /// <summary>
        /// Gets the code provider.
        /// </summary>
        /// <value>The code provider.</value>
        string CodeProvider { get; }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <value>The file extension.</value>
        string FileExtension { get; }

        /// <summary>
        /// Gets the template prefix.
        /// </summary>
        /// <value>The template prefix.</value>
        string TemplatePrefix { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        string Identifier { get; }

        /// <summary>
        /// Gets the short name.
        /// </summary>
        /// <value>The short name.</value>
        string ShortName { get; }

        /// <summary>
        /// Gets the default using statements.
        /// </summary>
        /// <value>The default using statements.</value>
        string DefaultUsingStatements { get; }

        #endregion


        #region Methods

        /// <summary>
        /// Creates the code provider.
        /// </summary>
        /// <returns></returns>
        CodeDomProvider CreateCodeProvider();

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <param name="colName">Name of the col.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="isNullableColumn">if set to <c>true</c> [is nullable column].</param>
        /// <returns></returns>
        string GetDefaultValue(string colName, DbType dbType, bool isNullableColumn);

        /// <summary>
        /// Gets the using statements.
        /// </summary>
        /// <param name="namespaces">The namespaces.</param>
        /// <returns></returns>
        string GetUsingStatements(string[] namespaces);

        /// <summary>
        /// Gets the type of the variable.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="isNullableColumn">if set to <c>true</c> [is nullable column].</param>
        /// <returns></returns>
        string GetVariableType(DbType dbType, bool isNullableColumn);

        /// <summary>
        /// Determines whether the specified word is keyword.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns>
        /// 	<c>true</c> if the specified word is keyword; otherwise, <c>false</c>.
        /// </returns>
        bool IsKeyword(string word);

        #endregion
    }
}