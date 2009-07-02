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

using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace SubSonic.SubCommander
{
    /// <summary>
    /// Command Line Parsing Library.
    /// </summary>
    public class Arguments
    {
        private readonly StringDictionary parameters = new StringDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="Arguments"/> class.
        /// </summary>
        /// <param name="commandLine">The command line.</param>
        public Arguments(string commandLine)
        {
            const string pattern = @"/(?<arg>((?!/).)*?)(\s*""(?<value>[^""]*)""|\s+(?<value>(?!/).*?)([\s]|$)|(?<value>\s+))";
            Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(commandLine);
            foreach(Match match in matches)
            {
                string arg = match.Groups["arg"].Value;
                string value = match.Groups["value"].Value;
                parameters.Add(arg, value);
            }
        }

        /// <summary>
        /// Retrieve a parameter value if it exists (overriding C# indexer property)
        /// </summary>
        /// <value></value>
        public string this[string Param]
        {
            get { return (parameters[Param]); }
        }
    }
}