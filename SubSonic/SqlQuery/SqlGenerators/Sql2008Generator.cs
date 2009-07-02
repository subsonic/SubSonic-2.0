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
    public class Sql2008Generator : Sql2005Generator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Sql2008Generator"/> class.
        /// </summary>
        /// <param name="query">The query.</param>
        public Sql2008Generator(SqlQuery query)
            : base(query) {}
    }
}