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

using System.Text;
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests.SqlQuery
{
    [TestFixture]
    public class DeleteTests
    {
        #region DELETE

        [Test]
        public void Delete_SimpleSqlCheck()
        {
            SubSonic.SqlQuery q = new Delete().From(Region.Schema).Where("regiondescription").IsEqualTo("Test");
            string sql = q.BuildSqlStatement();
            //Assert.Fail("sql = " + sql);
            Assert.IsTrue(sql == "DELETE FROM `main`.`Region` WHERE `main`.`Region`.`RegionDescription` = @RegionDescription0\r\n");
        }

        [Test]
        public void Delete_Simple()
        {
            //insert a test product
            Region r = new Region();
            r.RegionDescription = "Test";
            r.Save("test");

            //delete it
            int records = new Delete().From(Region.Schema).Where("regiondescription").IsEqualTo("Test").Execute();

            Assert.IsTrue(records == 1);
        }

        [Test]
        public void Delete_MultipleWithInsertBatch()
        {
            //this will fail unless you turn IDENTITY on for RegionID
            //not sure why it's off

            StringBuilder sb = new StringBuilder();
            //insert a test product

            sb.AppendLine(new Insert().Into(Region.Schema).ValueExpression("'test1'").BuildSqlStatement());
            sb.AppendLine(new Insert().Into(Region.Schema).ValueExpression("'test2'").BuildSqlStatement());
            sb.AppendLine(new Insert().Into(Region.Schema).ValueExpression("'test3'").BuildSqlStatement());
            sb.AppendLine(new Insert().Into(Region.Schema).ValueExpression("'test4'").BuildSqlStatement());
            sb.AppendLine(new Insert().Into(Region.Schema).ValueExpression("'test5'").BuildSqlStatement());
            sb.AppendLine(new Insert().Into(Region.Schema).ValueExpression("'test6'").BuildSqlStatement());
            string sql = sb.ToString();

            //Assert.Fail("sql = " + sql);
            
            // Requires semicolon line separations, using override in SQLiteGenerator.

            //insert the values
            new InlineQuery().Execute(sql);

            //delete it
            int records = DB.Delete()
                .From(Region.Schema)
                .Where("regiondescription").Like("test%")
                .Execute();

            Assert.IsTrue(records == 6);
        }

        #endregion
    }
}