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

namespace SubSonic.Tests
{
    /// <summary>
    ///
    /// You can use the following additional attributes as you write your tests:
    ///
    /// Use ClassInitialize to run code before running the first test in the class
    /// [ClassInitialize()]
    /// public static void MyClassInitialize(TestContext testContext) { }
    ///
    /// Use ClassCleanup to run code after all tests in a class have run
    /// [ClassCleanup()]
    /// public static void MyClassCleanup() { }
    ///
    /// Use TestInitialize to run code before running each test 
    /// [TestInitialize()]
    /// public void MyTestInitialize() { }
    ///
    /// Use TestCleanup to run code after each test has run
    /// [TestCleanup()]
    /// public void MyTestCleanup() { }
    ///
    /// </summary>
    [TestFixture]
    public class DataServiceTest
    {
        /// <summary>
        /// Datas the service_ get schema.
        /// </summary>
        [Test]
        public void DataService_GetSchema()
        {
            TableSchema.Table tbl = DataService.GetTableSchema("Products", DataService.Provider.Name);
            Assert.IsTrue(tbl.Columns.Count == 18, "Count is " + tbl.Columns.Count);
        }

        /// <summary>
        /// Datas the service_ get table names.
        /// </summary>
        [Test]
        public void DataService_GetTableNames()
        {
            // XP_PROC and sqlite_sequence not included.
            string[] tables = DataService.GetTableNames(DataService.Provider.Name);
            Assert.IsTrue(tables.Length == 18, "Count is " + tables.Length);   // changed from 15 to 17 for sqlite
        }

        /// <summary>
        /// Datas the provider_ get record count.
        /// </summary>
        [Test]
        public void DataProvider_GetRecordCount()
        {
            Query q = new Query(Product.Schema)
                .WHERE(Product.Columns.ProductID, 1)
                .WHERE(Product.Columns.ProductName, "Chai");
            int count = q.GetRecordCount();
            Assert.IsTrue(count == 1, "Count is " + count);
        }
    }
}