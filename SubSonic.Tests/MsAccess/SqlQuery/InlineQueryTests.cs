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
using NorthwindAccess;

namespace SubSonic.Tests.MsAccess.SqlQuery
{
    [TestFixture]
    public class InlineQueryTests
    {
        #region INLINE

        [Test]
        public void Acc_Inline_Simple()
        {
            QueryCommand cmd = new InlineQuery("NorthwindAccess").GetCommand("SELECT productID from products");
            Assert.IsTrue(cmd.CommandSql == "SELECT productID from products");
        }

        [Test]
        public void Acc_Inline_WithCommands()
        {
			QueryCommand cmd = new InlineQuery("NorthwindAccess").GetCommand("SELECT productID from products WHERE productid=[PARM__productid]", 1);

			Assert.IsTrue(cmd.Parameters[0].ParameterName == "[PARM__productid]");
        }

        [Test]
        public void Acc_Inline_AsCollection()
        {
			ProductCollection products =
				new InlineQuery("NorthwindAccess")
					.ExecuteAsCollection<ProductCollection>("SELECT productID from products WHERE categoryid=[PARM__madeupname]", 7);
        }

        #endregion
    }
}