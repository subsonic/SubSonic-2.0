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
    /// This is a syntax helper for constraint based querying.
    /// </summary>
    public static class Is
    {
        /// <summary>
        /// Equals to.
        /// </summary>
        /// <param name="comparand">The comparand.</param>
        /// <returns></returns>
        public static IConstraint EqualTo(object comparand)
        {
            return new GenericConstraint(Comparison.Equals, comparand);
        }

        /// <summary>
        /// Not Equals to.
        /// </summary>
        /// <param name="comparand">The comparand.</param>
        /// <returns></returns>
        public static IConstraint NotEqualTo(object comparand)
        {
            return new GenericConstraint(Comparison.NotEquals, comparand);
        }

        /// <summary>
        /// Like
        /// </summary>
        /// <param name="comparand">The comparand.</param>
        /// <returns></returns>
        public static IConstraint Like(object comparand)
        {
            return new GenericConstraint(Comparison.Like, comparand);
        }

        /// <summary>
        /// Not Like
        /// </summary>
        /// <param name="comparand">The comparand.</param>
        /// <returns></returns>
        public static IConstraint NotLike(object comparand)
        {
            return new GenericConstraint(Comparison.NotLike, comparand);
        }

        /// <summary>
        /// Less than
        /// </summary>
        /// <param name="comparand">The comparand.</param>
        /// <returns></returns>
        public static IConstraint LessThan(object comparand)
        {
            return new GenericConstraint(Comparison.LessThan, comparand);
        }

        /// <summary>
        /// Less than or equal to.
        /// </summary>
        /// <param name="comparand">The comparand.</param>
        /// <returns></returns>
        public static IConstraint LessThanOrEqualTo(object comparand)
        {
            return new GenericConstraint(Comparison.LessOrEquals, comparand);
        }

        /// <summary>
        /// Greater than
        /// </summary>
        /// <param name="comparand">The comparand.</param>
        /// <returns></returns>
        public static IConstraint GreaterThan(object comparand)
        {
            return new GenericConstraint(Comparison.GreaterThan, comparand);
        }

        /// <summary>
        /// Less than or equal to.
        /// </summary>
        /// <param name="comparand">The comparand.</param>
        /// <returns></returns>
        public static IConstraint GreaterThanOrEqualTo(object comparand)
        {
            return new GenericConstraint(Comparison.GreaterOrEquals, comparand);
        }
    }
}