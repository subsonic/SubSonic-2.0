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
using System.Data;
using System.Web.UI.WebControls;
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests
{
    /// <summary>
    /// 
    /// You can use the following additional attributes as you write your tests:
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
    public class GeneratedObjectTests
    {
        /// <summary>
        /// Collections_s the deleted test.
        /// </summary>
        [Test]
        public void Collections_DeletedTest()
        {
            EmployeeCollection emps = new EmployeeCollection().Load();

            Assert.IsTrue(emps.Count == 8, "Nope, it's " + emps.Count);
        }

        /// <summary>
        /// Objects_s the load list items.
        /// </summary>
        [Test]
        public void Objects_LoadListItems()
        {
            ListItemCollection coll = ReadOnlyRecord<Product>.GetListItems();
            Assert.AreEqual(new Query(Product.Schema).GetCount(Product.Columns.ProductID), coll.Count, "Nope, it's " + coll.Count);
        }

        /// <summary>
        /// Objects_s the deleted test.
        /// </summary>
        [Test]
        public void Objects_DeletedTest()
        {
            IDataReader rdr = ReadOnlyRecord<Employee>.FetchAll();
            int count = 0;
            while(rdr.Read())
                count++;
            Assert.IsTrue(count == 8, "Nope, it's " + count);
        }

        /// <summary>
        /// Object_s the query.
        /// </summary>
        [Test]
        public void Object_Query()
        {
            IDataReader rdr = ReadOnlyRecord<Product>.Query().WHERE("CategoryID = 5").ExecuteReader();
            int count = 0;
            while(rdr.Read())
                count++;
            Assert.IsTrue(count == 7, "Nope, it's " + count);
        }

        /// <summary>
        /// Object_s the CRUD.
        /// </summary>
        [Test]
        [Rollback]
        public void Object_CRUD()
        {
            //Add a product
            Product p = new Product();
            p.ProductName = "Test";
            p.QuantityPerUnit = "test amount";
            p.ReorderLevel = null;
            p.SupplierID = 1;
            p.UnitPrice = 100;
            p.UnitsInStock = 0;
            p.UnitsOnOrder = 0;
            p.CategoryID = 1;
            p.Discontinued = false;

            Assert.AreEqual(0, p.ProductID);

            p.Save("Unit Test");

            Assert.IsTrue(p.ProductID > 0);
        }

        /// <summary>
        /// Object_s the validation.
        /// </summary>
        [Test]
        public void Object_Validation()
        {
            //AssertValidation(10, false); EK: This used to pass (prior to Rev. 370... But why?)
            AssertValidation(21, true);
        }

        /// <summary>
        /// Asserts the validation.
        /// </summary>
        /// <param name="unitPrice">The unit price.</param>
        /// <param name="expected">if set to <c>true</c> [expected].</param>
        private static void AssertValidation(decimal unitPrice, bool expected)
        {
            Product p = new Product();
            p.ProductName = "Test";
            p.QuantityPerUnit = "test amount";
            p.ReorderLevel = null;
            p.SupplierID = 1;
            p.UnitsInStock = 0;
            p.UnitsOnOrder = 0;
            p.CategoryID = 1;
            p.Discontinued = false;

            p.UnitPrice = unitPrice;
            Assert.AreEqual(expected, p.Validate());
        }

        /// <summary>
        /// Object_s the default settings.
        /// </summary>
        [Test]
        public void Object_DefaultSettings()
        {
            Product p = new Product(true);
            Assert.IsNotNull(p.DateCreated, "Should not be NULL");
            Assert.AreEqual(DateTime.Now.ToString(), p.DateCreated.ToString(), "Should not be NULL");
        }

        /// <summary>
        /// Object_s the schema definition.
        /// </summary>
        [Test]
        public void Object_SchemaDefinition()
        {
            //Add a product
            DataService.GetTableSchema("Products", "Northwind");
        }
    }
}