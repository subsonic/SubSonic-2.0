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
using NorthwindAccess;

namespace SubSonic.Tests.MsAccess.SqlQuery
{
    [TestFixture]
    public class DeleteTests
    {
        #region DELETE

        [Test]
        public void Acc_Delete_SimpleSqlCheck()
        {
            SubSonic.SqlQuery q = new Delete().From(Region.Schema).Where("regiondescription").IsEqualTo("Test");
            string sql = q.BuildSqlStatement();
            Assert.IsTrue(sql == "DELETE FROM [Region] WHERE [Region].[RegionDescription] = [PARM__RegionDescription0]\r\n");
        }

        [Test]
        public void Acc_Delete_Simple()
        {
            //insert a test product
            Region r = new Region();
            r.RegionDescription = "Test";
            r.Save("test");

			int records = DB.Select()
				.From(Region.Schema)
				.Where("regiondescription").IsEqualTo("Test")
				.GetRecordCount();

			Assert.IsTrue(records == 1);

			DB.Delete()
				.From(Region.Schema)
				.Where("regiondescription").IsEqualTo("Test")
				.Execute();

			records = DB.Select()
				.From(Region.Schema)
				.Where("regiondescription").IsEqualTo("Test")
				.GetRecordCount();

			Assert.IsTrue(records == 0);
		}

        [Test]
        public void Acc_Delete_Multiple()
        {
			new InlineQuery("NorthwindAccess").Execute(new Insert().Into(Region.Schema).ValueExpression("'test1'").BuildSqlStatement());
			new InlineQuery("NorthwindAccess").Execute(new Insert().Into(Region.Schema).ValueExpression("'test2'").BuildSqlStatement());
			new InlineQuery("NorthwindAccess").Execute(new Insert().Into(Region.Schema).ValueExpression("'test3'").BuildSqlStatement());
			new InlineQuery("NorthwindAccess").Execute(new Insert().Into(Region.Schema).ValueExpression("'test4'").BuildSqlStatement());
			new InlineQuery("NorthwindAccess").Execute(new Insert().Into(Region.Schema).ValueExpression("'test5'").BuildSqlStatement());
			new InlineQuery("NorthwindAccess").Execute(new Insert().Into(Region.Schema).ValueExpression("'test6'").BuildSqlStatement());

			int records = DB.Select()
				.From(Region.Schema)
				.Where("regiondescription").Like("test%")
				.GetRecordCount();

			Assert.IsTrue(records == 6);
			
			DB.Delete()
				.From(Region.Schema)
				.Where("regiondescription").Like("test%")
				.Execute();

			records = DB.Select()
				.From(Region.Schema)
				.Where("regiondescription").Like("test%")
				.GetRecordCount();

			Assert.IsTrue(records == 0);
        }

        #endregion
    }
}