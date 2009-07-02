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
using System.Xml.Serialization;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public interface IActiveRecord : IRecordBase
    {
        /// <summary>
        /// Gets the null exception message.
        /// </summary>
        /// <value>The null exception message.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string NullExceptionMessage { get; }

        /// <summary>
        /// Gets the invalid type exception message.
        /// </summary>
        /// <value>The invalid type exception message.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string InvalidTypeExceptionMessage { get; }

        /// <summary>
        /// Gets the length exception message.
        /// </summary>
        /// <value>The length exception message.</value>
        [XmlIgnore]
        [HiddenForDataBinding(true)]
        string LengthExceptionMessage { get; }

        /// <summary>
        /// Made Public for use with transactions
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        QueryCommand GetInsertCommand(string userName);

        /// <summary>
        /// Gets the update command.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        QueryCommand GetUpdateCommand(string userName);

        /// <summary>
        /// Saves this object's state to the selected Database.
        /// </summary>
        void Save();

        /// <summary>
        /// Saves this object's state to the selected Database.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        void Save(int userID);

        /// <summary>
        /// Saves this object's state to the selected Database.
        /// </summary>
        /// <param name="userID">The user ID.</param>
        void Save(Guid userID);

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        bool Validate();

        /// <summary>
        /// Saves this object's state to the selected Database.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        void Save(string userName);

        /// <summary>
        /// Gets the save command.
        /// </summary>
        /// <returns></returns>
        QueryCommand GetSaveCommand();

        /// <summary>
        /// Gets the save command.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        QueryCommand GetSaveCommand(string userName);

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Loads the by param.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        void LoadByParam(string columnName, object paramValue);

        /// <summary>
        /// Loads the by key.
        /// </summary>
        /// <param name="keyID">The key ID.</param>
        void LoadByKey(object keyID);

        /// <summary>
        /// Gets the select command.
        /// </summary>
        /// <returns></returns>
        QueryCommand GetSelectCommand();
    }
}