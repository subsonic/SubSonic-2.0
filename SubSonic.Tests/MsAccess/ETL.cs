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

using System.Data;
using MbUnit.Framework;
using NorthwindAccess;

namespace SubSonic.Tests.MsAccess
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
    /// public void Acc_MyTestInitialize() { }
    ///
    /// Use TestCleanup to run code after each test has run
    /// [TestCleanup()]
    /// public void Acc_MyTestCleanup() { }
    ///
    /// </summary>
    [TestFixture]
    public class ETL
    {
        /// <summary>
        /// ETs the l_ clone instance.
        /// </summary>
        [Test]
        public void Acc_ETL_CloneInstance()
        {
            Product product1 = new Product(1);
            Product product2 = product1.Clone();

            product2.ProductName = "Unit Test";
            Assert.IsTrue(product2.ProductName != product1.ProductName);
        }

        /// <summary>
        /// ETs the l_ clone collection.
        /// </summary>
        [Test]
        public void Acc_ETL_CloneCollection()
        {
            ProductCollection coll = new ProductCollection().Where("CategoryID", 1);
            coll.Load();
            ProductCollection coll2 = new ProductCollection();
            coll2.AddRange(coll.Clone());
            Assert.IsTrue(coll.Count == coll2.Count);
        }

        /// <summary>
        /// ETs the l_ copy collection.
        /// </summary>
        [Test]
        public void Acc_ETL_CopyCollection()
        {
            ProductCollection coll = new ProductCollection().Where("CategoryID", 1);
            coll.Load();
            ProductCollection coll2 = new ProductCollection();
            coll2.CopyFrom(coll);
            Assert.IsTrue(coll.Count == coll2.Count);
        }

        /// <summary>
        /// ETs the l_ collection to table.
        /// </summary>
        [Test]
        public void Acc_ETL_CollectionToTable()
        {
            ProductCollection coll = new ProductCollection().Where("CategoryID", 1);
            coll.Load();
            DataTable tbl = coll.ToDataTable();
            Assert.IsTrue(coll.Count == tbl.Rows.Count);
        }
    }
}