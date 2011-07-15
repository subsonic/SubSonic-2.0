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
using NorthwindRepository;

namespace SubSonic.Tests.Repository
{
    [TestFixture]
    public class RepositoryTests
    {
        [Test]
        public void Repository_Simple_Select()
        {
            Product p = DB.Get<Product>(1);
            Assert.IsTrue(p.ProductName == "Chai");
        }

        [Test]
        public void Repository_FullCRUD()
        {
            //Get a record
            Product p = DB.Get<Product>(1);
            Assert.IsTrue(p.ProductName == "Chai", "p.ProductName == \"Chai\" fail");

            decimal? oldUnitPrice = (decimal)p.UnitPrice;   // paul
            p.UnitPrice = 200;
            DB.Save(p);

            //pull it back out and test
            p = DB.Get<Product>(1);
            Assert.IsTrue(p.UnitPrice == 200, "p.UnitPrice == 200 fail");

            p.UnitPrice = (float)oldUnitPrice;  // paul
            DB.Save(p);

            //add a new product
            p = new Product();
            p.ProductName = "Test Product";
            p.SupplierID = 1;
            p.CategoryID = 1;
            p.QuantityPerUnit = "0";
            p.UnitPrice = 0;
            p.UnitsInStock = 0;
            p.UnitsOnOrder = 0;
            p.ReorderLevel = 0;
            p.Discontinued = false;
            p.DateCreated = DateTime.Today;

            //save
            DB.Save(p);
            int newID = p.ProductID;
            Assert.IsTrue(newID > 0, "newID > 0 fails");

            //pull the new record back out
            p = DB.Get<Product>(newID);
            Assert.IsTrue(p.ProductName == "Test Product");

            //delete it - this sets Deleted to "true"
            ////DB.Delete(p);
            ////p = DB.Get<Product>(newID);
            ////Assert.IsTrue(p.Deleted);  // fails

            //destroy it
            DB.Destroy(p);
            p = DB.Get<Product>(newID);
            //Assert.IsTrue(p.IsLoaded == false);
            Assert.IsNull(p, "p not null");

            //now destroy all test data
            DB.Destroy<Product>("ProductName", "Test Product");

        }

        [Test]
        public void Repository_Should_UpdateProductName()
        {
            //check inline as well...
            DB.Update<Product>().Set("ProductName").EqualTo("Test Product").Where("ProductID").IsEqualTo(1).Execute();
            Product p = DB.Get<Product>(1);
            Assert.IsTrue(p.ProductName == "Test Product");
            //reset it
            DB.Update<Product>().Set("ProductName").EqualTo("Chai").Where("ProductID").IsEqualTo(1).Execute();
        }

        [Test]
        public void Repository_ShouldNot_ThrowWhenNoColumnsChanged()
        {
            Category c = DB.Get<Category>(1);

            DB.Save(c);
        }

        [Test]
        public void Respository_CollectionLoad()
        {
            ProductCollection ps = DB.Select().From<Product>().ExecuteAsCollection<ProductCollection>();
            Assert.IsTrue(ps.Count > 0);
        }
    }
}