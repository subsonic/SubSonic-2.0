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
    /// Summary for the IConstraint interface
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// The type of comparison represented by this
        /// constraint.
        /// </summary>
        /// <value>The comparison.</value>
        Comparison Comparison { get; }

        /// <summary>
        /// The value of the constraint. This is what
        /// is being compared.
        /// </summary>
        /// <value>The value.</value>
        object Value { get; }
    }
}