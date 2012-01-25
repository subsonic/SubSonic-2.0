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
using MbUnit.Framework;
using NorthwindAccess;

// Removed 'ROLLBACK' attribute from this class as it was causing:
//   System.Data.OleDb.OleDbException: No error message available, result code: E_NOINTERFACE(0x80004002).
// which appears to be related to either multi-threading or DTE.
// Access is a simple system, we'll confine transactions to the specific tests for it.
// A fresh DB can be copied at the start of each test run.

namespace SubSonic.Tests.MsAccess
{
    /// <summary>
    /// Summary for the ActiveRecordTest class
    /// </summary>
    [TestFixture]
    public class ActiveRecordTest
    {
        /// <summary>
        /// Setups this instance.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            Query qry = new Query(Product.Schema);
            qry.QueryType = QueryType.Delete;
            qry.AddWhere("productName", Comparison.Like, "Unit Test%");
            qry.Execute();

            Query qry2 = new Query(Supplier.Schema);
            qry2.QueryType = QueryType.Delete;
            qry2.AddWhere("CompanyName", Comparison.Like, "Unit Test%");
            qry2.Execute();
        }

		[TearDown]
		public void TearDown() {
			DeleteTestProduct();
		}

        /// <summary>
        /// Products_s the crud.
        /// </summary>
        [Test]
        public void Acc_Products_Crud()
        {
            //add a new product
            Product product = CreateTestProduct();
            product.Save("");

            //get the new id
            int newID = product.ProductID;

            product = new Product(newID);
            product.ReorderLevel = 100;
            product.Save("unit test");

            //pull it out to confirm
            product = new Product(newID);
            Assert.IsTrue(product.ReorderLevel == 100, "Bad Save");

			DeleteTestProduct();
        }

        /// <summary>
        /// Products_s the null crud.
        /// </summary>
        [Test]
        public void Acc_Products_NullCrud()
        {
            //add a new product
            Product product = new Product();
            product.CategoryID = 1;
            product.Discontinued = false;
            product.ProductName = "Unit Test Product";
            product.QuantityPerUnit = null;
            product.ReorderLevel = null;
            product.SupplierID = null;
            product.UnitPrice = null;
            product.UnitsInStock = null;
            product.UnitsOnOrder = null;

            product.Save("");

            //get the new id
            int newID = product.ProductID;

            product = new Product(newID);
            product.ReorderLevel = 100;
            product.Save("unit test");

            //pull it out to confirm
            product = new Product(newID);
            Assert.IsTrue(product.ReorderLevel == 100, "Bad Save");
            Assert.IsTrue(product.SupplierID == null, "Bad Save, Null not inserted");

            //delete it
            ActiveRecord<Product>.Delete(newID);
        }

        /// <summary>
        /// Products_s the collection load.
        /// </summary>
        [Test]
        public void Acc_Products_CollectionLoad()
        {
            ProductCollection coll = new ProductCollection();
            using(IDataReader rdr = ReadOnlyRecord<Product>.FetchAll())
            {
                coll.Load(rdr);
                rdr.Close();
            }
            Assert.IsTrue(coll.Count > 0);
        }

        /// <summary>
        /// Gets the new command.
        /// </summary>
        [Test]
        public void Acc_GetNewCommand()
        {
            Product p = CreateTestProduct();
            Assert.IsTrue(p.IsNew, "Should be New");
            Assert.IsTrue(p.GetSaveCommand().CommandSql.Contains("INSERT INTO"), "Should be INSERT Statement");
            Assert.AreEqual("NorthwindAccess", p.GetSaveCommand().ProviderName, "Provider Name not set");
        }

        /// <summary>
        /// Gets the update command.
        /// </summary>
        [Test]
        public void Acc_GetUpdateCommand()
        {
            Product p = CreateTestProduct();
            p.Discontinued = true;
            p.IsNew = false;
            Assert.IsFalse(p.IsNew, "Should not be New");
            Assert.IsTrue(p.IsDirty, "Should be Dirty");
            Assert.IsTrue(p.GetSaveCommand().CommandSql.Contains("UPDATE"), "Should be UPDATE Statement");
            Assert.AreEqual("NorthwindAccess", p.GetSaveCommand().ProviderName, "Provider Name not set");
        }

        /// <summary>
        /// Gets the no changes command.
        /// </summary>
        [Test]
        public void Acc_GetNoChangesCommand()
        {
            Product p = CreateTestProduct();
            p.Save();
            p.IsNew = false;
            Assert.IsFalse(p.IsNew, "Should not be New");
            Assert.IsFalse(p.IsDirty, "Should not be Dirty");
            Assert.IsNull(p.GetSaveCommand(), "Should be NULL");
        }

        /// <summary>
        /// Saves the when not dirty.
        /// </summary>
        [Test]
        public void Acc_SaveWhenNotDirty()
        {
            Product p = CreateTestProduct();
            p.Save();
            int id = p.ProductID;

            p.Discontinued = true;
            p.ReorderLevel = 2112;
            p.MarkClean();
            Assert.IsFalse(p.IsDirty, "Should NOT be dirty");

            p.Save();

            p = new Product(id);

            Assert.AreEqual(false, p.Discontinued, "Should not be false");
            Assert.AreEqual(Int16.Parse("3"), p.ReorderLevel, "Should not be set");
        }

        //[Test]
        //public void Acc_TestNewRecordDeepSave()
        //{
        //    Supplier s = new Supplier();
        //    s.CompanyName = "Unit Test Supplier";

        //    Product p1 = CreateTestProduct();
        //    Product p2 = CreateTestProduct();

        //    s.Products().Add(p1);
        //    s.Products().Add(p2);

        //    s.DeepSave();

        //    Assert.IsNotNull(s.SupplierID, "SupplierID is null");
        //    Assert.AreEqual(s.SupplierID, p1.SupplierID, "SupplierID not set.");
        //    Assert.AreEqual(s.SupplierID, p2.SupplierID, "SupplierID not set.");
        //}

        //[Test]
        //public void Acc_TestExistingRecordDeepSave()
        //{
        //    Supplier s = new Supplier();
        //    s.CompanyName = "Unit Test Supplier";
        //    s.Save();
        //    Assert.IsNotNull(s.SupplierID, "SupplierID is null");

        //    Product p1 = CreateTestProduct();
        //    Product p2 = CreateTestProduct();

        //    s.Products.Add(p1);
        //    s.Products.Add(p2);

        //    s.DeepSave();

        //    Assert.AreEqual(s.SupplierID, p1.SupplierID, "SupplierID not set.");
        //    Assert.AreEqual(s.SupplierID, p2.SupplierID, "SupplierID not set.");
        //}

        /// <summary>
        /// Creates the test product.
        /// </summary>
        /// <returns></returns>
        private static Product CreateTestProduct()
        {
            Product product = new Product();
            product.CategoryID = 1;
            product.Discontinued = false;
            product.ProductName = "Unit Test Product";
            product.QuantityPerUnit = "Qty";
            product.ReorderLevel = 3;
            product.SupplierID = 1;
            product.UnitPrice = 99;
            product.UnitsInStock = 20;
            product.UnitsOnOrder = 9;
            return product;
        }

		private static void DeleteTestProduct() {
			//delete all unit tests
			Query qry = new Query(Product.Schema);
			qry.QueryType = QueryType.Delete;
			qry.AddWhere("productName", "Unit Test Product");
			qry.Execute();
		}

        /// <summary>
        /// Tests to make sure that our DirtyColumns change is tracking dirty columns properly
        /// </summary>
        [Test]
        public void Acc_DirtyColumnsExist()
        {
            Product p = new Product(1);

            //make sure no dirties
            Assert.IsTrue(p.DirtyColumns.Count == 0);

            //set the product name to something different
            p.ProductName = DateTime.Now.ToString();

            //see if the dirty col is set right
            Assert.IsTrue(p.DirtyColumns.Count == 1 && p.DirtyColumns[0].ColumnName == "ProductName");
            //p.Save("bbep");
        }
    }
}