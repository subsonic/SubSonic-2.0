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
using Northwind;

namespace SubSonic.Tests.SqlQuery
{
    [TestFixture]
    public class UpdateTests
    {
        #region UPDATE

        [Test]
        public void Update_SimpleSqlCheck()
        {
            SubSonic.SqlQuery u = new Update(Product.Schema).Set("UnitPrice").EqualTo(100).Where("productid").IsEqualTo(1);
            string sql = u.BuildSqlStatement();
            Assert.IsTrue(sql == "UPDATE [dbo].[Products] SET [UnitPrice]=@up_UnitPrice\r\n WHERE [dbo].[Products].[ProductID] = @ProductID0\r\n");
        }

        [Test]
        public void Update_Simple()
        {
            int records = new Update(Product.Schema).Set("UnitPrice").EqualTo(100).Where("productid").IsEqualTo(1).Execute();
            Assert.IsTrue(records == 1);

            //pull it back out
            Product p = new Product(1);
            Assert.IsTrue(p.UnitPrice == 100);

            
            
            //reset it to 50
            p.UnitPrice = 50;
            p.Save("unit test");
        }

        [Test]
        public void Update_Expression()
        {
            Product p = new Product(1);
            p.UnitPrice = 50;
            p.Save("unit test");

            int records = new Update(Product.Schema)
                .SetExpression("UnitPrice").EqualTo("UnitPrice * 3")
                .Where("productid").IsEqualTo(1)
                .Execute();
            Assert.IsTrue(records == 1);

            //pull it back out
            p = new Product(1);
            Assert.IsTrue(p.UnitPrice == 150);

            //reset it to 50
            p.UnitPrice = 50;
            p.Save("unit test");
        }

        [Test]
        public void Update_SimpleTyped()
        {
            int records = DB.Update<Product>().Set("UnitPrice").EqualTo(100).Where("productid").IsEqualTo(1).Execute();
            Assert.IsTrue(records == 1);

            //pull it back out
            Product p = new Product(1);
            Assert.IsTrue(p.UnitPrice == 100);

            //reset it to 50
            p.UnitPrice = 50;
            p.Save("unit test");
        }

        #endregion
    }
}