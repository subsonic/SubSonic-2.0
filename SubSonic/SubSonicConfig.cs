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

namespace SubSonic
{
    /// <summary>
    /// Summary for the SubSonicConfig class
    /// </summary>
    public static class SubSonicConfig
    {
        private static string templateDirectory = String.Empty;

        /// <summary>
        /// Gets the default data provider.
        /// </summary>
        /// <value>The default data provider.</value>
        public static DataProvider DefaultDataProvider
        {
            get { return DataService.Provider; }
        }

        /// <summary>
        /// Gets or sets the template directory.
        /// </summary>
        /// <value>The template directory.</value>
        public static string TemplateDirectory
        {
            get { return templateDirectory; }
            set { templateDirectory = value; }
        }
    }
}