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
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests.SqlQuery
{
    [TestFixture]
    public class InsertTests
    {
        #region INSERT

        [Test]
        [Rollback]
        public void Insert_Simple()
        {
            Insert i = new Insert().Into(Category.Schema).Values("Test", "TestDescription", DBNull.Value);
            string sql = i.BuildSqlStatement();
            Assert.IsTrue(sql == "INSERT INTO [dbo].[Categories](CategoryName,Description,Picture)\r\n VALUES (@ins_CategoryName,@ins_Description,@ins_Picture)\r\n");
        }

        [Test]
        [Rollback]
        public void Insert_SimpleWithSelect()
        {
            Insert i = new Insert().Into(Category.Schema)
                .Select(new Select("CategoryName", "Description", "Picture").From(Category.Schema));
            string sql = i.BuildSqlStatement();
            Assert.IsTrue(sql ==
                          "INSERT INTO [dbo].[Categories](CategoryName,Description,Picture)\r\nSELECT [dbo].[Categories].[CategoryName], [dbo].[Categories].[Description], [dbo].[Categories].[Picture]\r\n FROM [dbo].[Categories]\r\n\r\n");
        }

        [Test]
        [Rollback]
        public void Insert_SimpleWithSelectAndSchema()
        {
            Insert i = new Insert().Into(Category.Schema)
                .Select(Select.AllColumnsFrom<Category>());
            string sql = i.BuildSqlStatement();

            Assert.AreEqual(
                "INSERT INTO [dbo].[Categories](CategoryName,Description,Picture)\r\nSELECT [dbo].[Categories].[CategoryName], [dbo].[Categories].[Description], [dbo].[Categories].[Picture]\r\n FROM [dbo].[Categories]\r\n\r\n",
                sql);
        }

        #endregion
    }
}