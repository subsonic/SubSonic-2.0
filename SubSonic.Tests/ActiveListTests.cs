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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests
{
    /// <summary>
    /// Summary for the ActiveListTests class
    /// </summary>
    [TestFixture]
    public class ActiveListTests
    {
        private bool Listchanged;

        /// <summary>
        /// Sets the up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            Query qry = new Query(Product.Schema);
            qry.QueryType = QueryType.Delete;
            qry.AddWhere(Product.Columns.ProductName, Comparison.Like, "Unit Test%");
            qry.Execute();
        }

        /// <summary>
        /// Batches the save insert.
        /// </summary>
        [Test]
        [Rollback]
        public void BatchSaveInsert()
        {
            ProductCollection c = CreateProductCollection();
            c.BatchSave();

            c = new ProductCollection();
            c.Load(ReadOnlyRecord<Product>.FetchByParameter(Product.Columns.ProductName, Comparison.Like, "Unit Test Product%"));
            Assert.AreEqual(1000, c.Count, "Expected 1000 - After Save: " + c.Count);
        }

        /// <summary>
        /// Tests deferred deletes
        /// </summary>
        [Test]
        [Rollback]
        public void DeferredDelete()
        {
            ProductCollection c = CreateProductCollection();
            c.BatchSave();

            c = new ProductCollection();
            c.Load(ReadOnlyRecord<Product>.FetchByParameter(Product.Columns.ProductName, Comparison.Like, "Unit Test Product%"));
            Assert.AreEqual(1000, c.Count, "Expected 1000 - After Save: " + c.Count);

            while(c.Count > 0)
                c.RemoveAt(0); // RemoveItem() gets called
            c.SaveAll();

            c = new ProductCollection();
            c.Load(ReadOnlyRecord<Product>.FetchByParameter(Product.Columns.ProductName, Comparison.Like, "Unit Test Product%"));
            Assert.AreEqual(0, c.Count, "Expected 0 - After Save: " + c.Count);
        }

        /// <summary>
        /// Lists the T helper.
        /// </summary>
        [Test]
        public void ListTHelper()
        {
            ProductCollection c = CreateProductCollection();

            List<Product> products = c.GetList();

            Predicate<Product> Units9 = IsUnitsEqual9;
            Predicate<Product> UnitsMinus9 = IsUnitsEqualMinus9;

            Assert.AreEqual(products.Exists(Units9), true);
            Assert.AreEqual(products.Exists(UnitsMinus9), false);
        }

        /// <summary>
        /// Determines whether [is units equal9] [the specified p].
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>
        /// 	<c>true</c> if [is units equal9] [the specified p]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUnitsEqual9(Product p)
        {
            return p.UnitsOnOrder == 9;
        }

        /// <summary>
        /// Determines whether [is units equal minus9] [the specified p].
        /// </summary>
        /// <param name="p">The p.</param>
        /// <returns>
        /// 	<c>true</c> if [is units equal minus9] [the specified p]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUnitsEqualMinus9(Product p)
        {
            return p.UnitsOnOrder == -9;
        }

        /// <summary>
        /// Batches the save update.
        /// </summary>
        [Test]
        [Rollback]
        public void BatchSaveUpdate()
        {
            ProductCollection c = CreateProductCollection();
            c.BatchSave();

            c = FetchProductCollection();

            foreach(Product product in c)
                product.UnitsOnOrder = 888;
            c.BatchSave();

            c = FetchProductCollection();
            Assert.AreEqual(1000, c.Count, "Expected 1000 - After Update: " + c.Count);
            foreach(Product product in c)
                Assert.AreEqual((Int16)888, product.UnitsOnOrder, product.ProductName);
        }

        /// <summary>
        /// Batches the save update no changes.
        /// </summary>
        [Test]
        [Rollback]
        public void BatchSaveUpdateNoChanges()
        {
            ProductCollection c = CreateProductCollection();
            c.BatchSave();

            c = FetchProductCollection();

            foreach(Product product in c)
                product.UnitsOnOrder = product.UnitsOnOrder;
            c.BatchSave();

            c = FetchProductCollection();
            Assert.AreEqual(1000, c.Count, "Expected 1000 - After Save: " + c.Count);
            foreach(Product product in c)
                Assert.AreEqual((Int16)9, product.UnitsOnOrder, product.ProductName);
        }

        /// <summary>
        /// Lists the serialization.
        /// </summary>
        [Test]
        public void ListSerialization()
        {
            ProductCollection products1 = FetchProductCollection();

            using(Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, products1);

                stream.Position = 0;
                ProductCollection products2 = (ProductCollection)formatter.Deserialize(stream);
                stream.Close();

                Assert.AreEqual(products1.Count, products2.Count);

                products2.ListChanged += products2_ListChanged;

                Product p = CreateProduct(1);
                products2.Add(p);

                Assert.AreEqual(Listchanged, true);
            }
        }

        private void products2_ListChanged(object sender, ListChangedEventArgs e)
        {
            Listchanged = true;
        }

        /// <summary>
        /// Fetches the product collection.
        /// </summary>
        /// <returns></returns>
        private static ProductCollection FetchProductCollection()
        {
            ProductCollection c = new ProductCollection();
            c.Load(ReadOnlyRecord<Product>.FetchByParameter(Product.Columns.ProductName, Comparison.Like, "Unit Test Product%"));
            return c;
        }

        /// <summary>
        /// Creates the product collection.
        /// </summary>
        /// <returns></returns>
        private static ProductCollection CreateProductCollection()
        {
            ProductCollection c = new ProductCollection();

            for(int i = 0; i < 1000; i++)
                c.Add(CreateProduct(i));
            Assert.AreEqual(1000, c.Count, "Before Save");
            return c;
        }

        /// <summary>
        /// Creates the product.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        private static Product CreateProduct(int index)
        {
            Product product = new Product();
            product.CategoryID = 1;
            product.Discontinued = false;
            product.ProductName = string.Format("Unit Test Product {0}", index);
            product.QuantityPerUnit = "Qty";
            product.ReorderLevel = 3;
            product.SupplierID = 1;
            product.UnitPrice = 99;
            product.UnitsInStock = 20;
            product.UnitsOnOrder = 9;
            return product;
        }
    }
}