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
    /// Summary for the GenericConstraint class
    /// </summary>
    public class GenericConstraint : IConstraint
    {
        private readonly Comparison comparison;
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericConstraint"/> class.
        /// </summary>
        /// <param name="comparison">The comparison.</param>
        /// <param name="value">The value.</param>
        public GenericConstraint(Comparison comparison, object value)
        {
            this.comparison = comparison;
            this.value = value;
        }


        #region IConstraint Members

        /// <summary>
        /// The type of comparison represented by this
        /// constraint.
        /// </summary>
        /// <value></value>
        public Comparison Comparison
        {
            get { return comparison; }
        }

        /// <summary>
        /// The value of the constraint. This is what
        /// is being compared.
        /// </summary>
        /// <value></value>
        public object Value
        {
            get { return value; }
        }

        #endregion
    }
}