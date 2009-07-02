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

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public interface IReadOnlyRecord
    {
        /// <summary>
        /// Loads the by key.
        /// </summary>
        /// <param name="keyID">The key ID.</param>
        void LoadByKey(object keyID);

        /// <summary>
        /// Loads the by param.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="paramValue">The param value.</param>
        void LoadByParam(string columnName, object paramValue);
    }
}