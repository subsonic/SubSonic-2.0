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
using System.Collections.Generic;

namespace SubSonic
{
    /// <summary>
    /// Summary for the CodeLanguageFactory class
    /// </summary>
    public static class CodeLanguageFactory
    {
        #region Static properties

        private static readonly List<ICodeLanguage> languages = new List<ICodeLanguage>();

        #endregion


        #region Properties

        /// <summary>
        /// Gets all code languages.
        /// </summary>
        /// <value>All code languages.</value>
        public static IEnumerable<ICodeLanguage> AllCodeLanguages
        {
            get { return languages; }
        }

        #endregion


        #region Static methods

        /// <summary>
        /// Initializes the <see cref="CodeLanguageFactory"/> class.
        /// </summary>
        static CodeLanguageFactory()
        {
            // TODO: Add dynamic discovery of all subclasses via reflection or config
            languages.Add(new CSharpCodeLanguage());
            languages.Add(new VBCodeLanguage());
        }

        /// <summary>
        /// Gets the default code language.
        /// </summary>
        /// <value>The default code language.</value>
        public static ICodeLanguage DefaultCodeLanguage
        {
            get { return languages[0]; }
        }

        /// <summary>
        /// Gets the name of the by code provider.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static ICodeLanguage GetByCodeProviderName(string providerName)
        {
            if(String.IsNullOrEmpty(providerName))
                return DefaultCodeLanguage;

            foreach(ICodeLanguage language in languages)
            {
                if(language.CodeProvider.Equals(providerName, StringComparison.InvariantCultureIgnoreCase))
                    return language;
            }

            throw new ArgumentException("Unknown language", "providerName");
        }

        /// <summary>
        /// Gets the short name of the by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static ICodeLanguage GetByShortName(string name)
        {
            if(String.IsNullOrEmpty(name))
                return DefaultCodeLanguage;

            foreach(ICodeLanguage language in languages)
            {
                if(language.ShortName.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return language;
            }

            throw new ArgumentException("Unknown language", "name");
        }

        /// <summary>
        /// Ideally this would not be static and called against the correct language
        /// but callers do not have this information available at this time.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="table">The table.</param>
        /// <param name="appendWith">The append with.</param>
        /// <returns></returns>
        public static string AvoidKeyWord(string word, string table, string appendWith)
        {
            string newWord = word + appendWith;

            // Can't have a property with same name as class.
            if(word.ToLower() == table)
                return newWord;

            switch(word.ToLower())
            {
                    // SubSonic keywords
                case "schema":
                    return newWord;

                default:
                    {
                    foreach(ICodeLanguage language in languages)
                    {
                        if(language.IsKeyword(word))
                            return newWord;
                    }

                    return word;
                    }
            }
        }

        #endregion
    }
}