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

using MbUnit.Framework;

namespace SubSonic.Tests
{
    /// <summary>
    /// Summary for the NameTransformationTests class
    /// </summary>
    [TestFixture]
    public class NameTransformationTests
    {
        /// <summary>
        /// Names the transformation_ to pascal case.
        /// </summary>
        [Test]
        public void NameTransformation_ToPascalCase()
        {
            CheckPascalCase("order history", "Order_History", false);
            CheckPascalCase("order history", "OrderHistory", true);
            CheckPascalCase("order_history", "Order_History", false);
            CheckPascalCase("order_history", "OrderHistory", true);
            CheckPascalCase("Order_History", "Order_History", false);
            CheckPascalCase("Order_History", "OrderHistory", true);
            CheckPascalCase("ORDER_HISTORY", "Order_History", false);
            CheckPascalCase("ORDER_HISTORY", "OrderHistory", true);
            CheckPascalCase("ORDERHISTORY", "Orderhistory", false);
            CheckPascalCase("ORDERHISTORY", "Orderhistory", true);
            CheckPascalCase("orderhistory", "Orderhistory", false);
            CheckPascalCase("orderhistory", "Orderhistory", true);
            CheckPascalCase("orderHistory", "OrderHistory", false);
            CheckPascalCase("orderHistory", "OrderHistory", true);
            CheckPascalCase("order_HistoryRecord", "Order_HistoryRecord", false);
            CheckPascalCase("order_HistoryRecord", "OrderHistoryRecord", true);
        }

        /// <summary>
        /// Checks the pascal case.
        /// </summary>
        /// <param name="evalString">The eval string.</param>
        /// <param name="correctString">The correct string.</param>
        /// <param name="removeUnderscores">if set to <c>true</c> [remove underscores].</param>
        private static void CheckPascalCase(string evalString, string correctString, bool removeUnderscores)
        {
            string transform = Inflector.ToPascalCase(evalString, removeUnderscores);
            Assert.IsTrue(transform == correctString, evalString + " (remove underscores: " + removeUnderscores + ") - Should be: " + correctString + " - Actually is: " + transform);
        }
    }
}