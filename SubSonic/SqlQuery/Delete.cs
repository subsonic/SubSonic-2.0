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
    public class Destroy : Delete {}

    /// <summary>
    /// 
    /// </summary>
    public class Delete : SqlQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Delete"/> class.
        /// </summary>
        public Delete()
        {
            QueryCommandType = QueryType.Delete;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Delete"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        public Delete(string providerName)
        {
            QueryCommandType = QueryType.Delete;
            Provider = DataService.Providers[providerName];
            ProviderName = providerName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Delete"/> class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public Delete(DataProvider dataProvider)
        {
            QueryCommandType = QueryType.Delete;
            Provider = dataProvider;
            ProviderName = dataProvider.Name;
        }
    }
}