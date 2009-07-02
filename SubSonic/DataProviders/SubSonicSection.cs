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

using System.Configuration;

namespace SubSonic
{
    /// <summary>
    /// Summary for the SubSonicSection class
    /// </summary>
    public class SubSonicSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the providers.
        /// </summary>
        /// <value>The providers.</value>
        [ConfigurationProperty(ConfigurationSectionName.PROVIDERS)]
        public ProviderSettingsCollection Providers
        {
            get { return (ProviderSettingsCollection)base[ConfigurationSectionName.PROVIDERS]; }
        }

        /// <summary>
        /// Gets or sets the default provider.
        /// </summary>
        /// <value>The default provider.</value>
        [ConfigurationProperty(ConfigurationPropertyName.DEFAULT_PROVIDER, DefaultValue = DataProviderTypeName.SQL_SERVER)]
        public string DefaultProvider
        {
            get { return (string)base[ConfigurationPropertyName.DEFAULT_PROVIDER]; }
            set { base[ConfigurationPropertyName.DEFAULT_PROVIDER] = value; }
        }

        /// <summary>
        /// Gets or sets the enable trace.
        /// </summary>
        /// <value>The enable trace.</value>
        [ConfigurationProperty(ConfigurationPropertyName.ENABLE_TRACE, DefaultValue = "false")]
        public string EnableTrace
        {
            get { return (string)base[ConfigurationPropertyName.ENABLE_TRACE]; }
            set { base[ConfigurationPropertyName.ENABLE_TRACE] = value; }
        }

        /// <summary>
        /// Gets or sets the template directory.
        /// </summary>
        /// <value>The template directory.</value>
        [ConfigurationProperty(ConfigurationPropertyName.TEMPLATE_DIRECTORY, DefaultValue = "")]
        public string TemplateDirectory
        {
            get { return (string)base[ConfigurationPropertyName.TEMPLATE_DIRECTORY]; }
            set { base[ConfigurationPropertyName.TEMPLATE_DIRECTORY] = value; }
        }
    }
}