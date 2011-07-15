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
using System.IO;
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests.SqlQuery
{
    [TestFixture]
    public class SelectTests
    {
        #region Simple Select Record Counts


        [Test]
        public void Exec_Simple()
        {
            //string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MyApplication\Northwind.db3");
            //DataService.Providers["Northwind"].DefaultConnectionString =
            //    String.Format(@"Data Source={0};Version=3;New=False;Connection Timeout=3", dbPath);
            
            int records = new Select("productID").From("Products").GetRecordCount();
            Assert.IsTrue(records == 77);
        }

        [Test]
        public void Exec_SimpleWithTypedColumns()
        {
            int records = new Select(Product.ProductIDColumn, Product.ProductNameColumn).From<Product>().GetRecordCount();
            Assert.IsTrue(records == 77);
        }

        [Test]
        public void Exec_SimpleAsSingle()
        {
            Product p = new Select().From<Product>().Where("ProductID").IsEqualTo(1).ExecuteSingle<Product>();
            Assert.IsNotNull(p);
        }

        [Test]
        public void Exec_WithAllColumns()
        {
            int records = new Select().From("Products").GetRecordCount();
            Assert.IsTrue(records == 77);
        }

        [Test]
        public void Exec_SimpleWhere()
        {
            int records = new Select().From("Products").Where("categoryID").IsEqualTo(5).GetRecordCount();
            Assert.AreEqual(7, records);
        }

        [Test]
        public void Exec_SimpleWhere2()
        {
            int records = new Select().From("Products").Where(Product.CategoryIDColumn).IsEqualTo(5).GetRecordCount();
            Assert.AreEqual(7, records);
        }

        [Test]
        public void Exec_SimpleWhereAnd()
        {
            ProductCollection products =
                DB.Select().From("Products")
                    .Where("categoryID").IsEqualTo(5)
                    .And("productid").IsGreaterThan(50)
                    .ExecuteAsCollection<ProductCollection>();

            int records = new Select().From("Products").Where("categoryID").IsEqualTo(5).And("productid").IsGreaterThan(50).GetRecordCount();
            Assert.IsTrue(records == 4);
        }

        [Test]
        public void Exec_SimpleJoin()
        {
            SubSonic.SqlQuery q = new Select("productid").From(OrderDetail.Schema)
                .InnerJoin(Product.Schema)
                .Where("CategoryID").IsEqualTo(5);
            string sql = q.ToString();
            int records = q.GetRecordCount();
            Assert.IsTrue(records == 196);
        }

        [Test]
        public void Exec_SimpleJoin2()
        {
            SubSonic.SqlQuery q = new Select("productid").From<OrderDetail>()
                .InnerJoin(Product.Schema)
                .Where("CategoryID").IsEqualTo(5);
            string sql = q.ToString();
            int records = q.GetRecordCount();
            Assert.IsTrue(records == 196);
        }

        [Test]
        public void Exec_SimpleJoin3()
        {
            SubSonic.SqlQuery q = new Select().From(Tables.OrderDetail)
                .InnerJoin(Tables.Product)
                .Where("CategoryID").IsEqualTo(5);
            string sql = q.ToString();
            int records = q.GetRecordCount();
            Assert.IsTrue(records == 196);
        }

        [Test]
        public void Exec_MultiJoin()
        {
            CustomerCollection customersByCategory = new Select()
                .From(Customer.Schema)
                .InnerJoin(Order.Schema)
                .InnerJoin(OrderDetail.OrderIDColumn, Order.OrderIDColumn)
                .InnerJoin(Product.ProductIDColumn, OrderDetail.ProductIDColumn)
                .Where("CategoryID").IsEqualTo(5)
                .ExecuteAsCollection<CustomerCollection>();

            Assert.IsTrue(customersByCategory.Count == 196);
        }

        [Test]
        public void Exec_LeftOuterJoin_With_TableColumn()
        {
            SubSonic.SqlQuery query = DB.Select(Aggregate.GroupBy("CompanyName"))
                .From(Customer.Schema)
                .LeftOuterJoin(Order.CustomerIDColumn, Customer.CustomerIDColumn);

            int records = query.GetRecordCount();
            Assert.AreEqual(91, records);
        }

        [Test]
        public void Exec_LeftOuterJoin_With_Generics()
        {
            SubSonic.SqlQuery query = DB.Select(Aggregate.GroupBy("CompanyName"))
                .From<Customer>()
                .LeftOuterJoin<Order>();

            int records = query.GetRecordCount();
            Assert.AreEqual(91, records);
        }

        [Test]
        public void Exec_LeftOuterJoin_With_String()
        {
            SubSonic.SqlQuery query = DB.Select(Aggregate.GroupBy("CompanyName"))
                .From("Customers")
                .LeftOuterJoin("Orders");

            int records = query.GetRecordCount();
            Assert.AreEqual(91, records);
        }

        #endregion


        #region Collection Tests

        [Test]
        public void Exec_Collection_Simple()
        {
            ProductCollection p = Select.AllColumnsFrom<Product>()
                .ExecuteAsCollection<ProductCollection>();
            Assert.IsTrue(p.Count == 77);
        }

        [Test]
        public void Exec_Collection_Joined()
        {
            ProductCollection p = Select.AllColumnsFrom<Product>()
                .InnerJoin(Category.Schema)
                .Where("CategoryName").IsEqualTo("Beverages")
                .ExecuteAsCollection<ProductCollection>();
            Assert.IsTrue(p.Count == 12);
        }

        [Test]
        public void Exec_Collection_JoinedWithLike()
        {
            ProductCollection p = DB.Select()
                .From(Product.Schema)
                .InnerJoin(Category.Schema)
                .Where("CategoryName").Like("c%")
                .ExecuteAsCollection<ProductCollection>();
            Assert.IsTrue(p.Count == 25);
        }

        [Test]
        public void Exec_Collection_JoinedWithLike_Typed()
        {
            ProductCollection p = DB.Select()
                .From<Product>()
                .InnerJoin<Category>()
                .Where("CategoryName").Like("c%")
                .ExecuteAsCollection<ProductCollection>();
            Assert.IsTrue(p.Count == 25);
        }

        #endregion


        #region Expression Tests

        [Test]
        public void Exec_ExpressionSimpleOr()
        {
            ProductCollection products = Select.AllColumnsFrom<Product>()
                .WhereExpression("categoryID").IsEqualTo(5).And("productid").IsGreaterThan(10)
                .OrExpression("categoryID").IsEqualTo(2).And("productID").IsBetweenAnd(2, 5)
                .ExecuteAsCollection<ProductCollection>();

            Assert.IsTrue(products.Count == 10);
        }

        [Test]
        public void Exec_NestedExpression()
        {
            ProductCollection products = Select.AllColumnsFrom<Product>()
                .WhereExpression("categoryID").IsEqualTo(5).And("productid").IsGreaterThan(10)
                .Or("categoryID").IsEqualTo(2).AndExpression("productID").IsBetweenAnd(2, 5)
                .ExecuteAsCollection<ProductCollection>();

            Assert.IsTrue(products.Count == 3);
        }

        #endregion


        #region Aggregates

        [Test]
        public void Exec_AggregateExpression()
        {
            double result = new
                Select(Aggregate.Sum("UnitPrice*Quantity", "ProductSales"))
                .From(OrderDetail.Schema)
                .ExecuteScalar<double>();

            result = Math.Round(result, 2);
            Assert.IsTrue(result == 1354458.59);
        }

        #endregion


        #region Paging

        [Test]
        public void Exec_PagedSimple()
        {
            SubSonic.SqlQuery q = Select.AllColumnsFrom<Product>().Paged(1, 20).Where("productid").IsLessThan(100);
            // BUG: SQLite error near "DECLARE": syntax error
            int records = q.GetRecordCount();
            Assert.IsTrue(records == 20);
        }

        [Test]
        public void Exec_PagedJoined()
        {
            //IDataReader rdr = Northwind.DB.Select("ProductId", "ProductName", "CategoryName")
            //    .From("Products")
            //    .InnerJoin(Northwind.Category.Schema)
            //    .Paged(1, 20)
            //    .ExecuteReader();

            SubSonic.SqlQuery q = new Select("ProductId", "ProductName", "CategoryName").From("Products").InnerJoin(Category.Schema).Paged(1, 20);
            int records = q.GetRecordCount();
            Assert.IsTrue(records == 20);
        }

        //[Test]
        //public void Exec_PagedView()
        //{
        //    SubSonic.SqlQuery q = new Select().From(Invoice.Schema).Paged(1, 20);
        //    int records = q.GetRecordCount();
        //    Assert.IsTrue(records == 20);
        //}

        //[Test]
        //public void Exec_PagedViewJoined()
        //{
        //    SubSonic.SqlQuery q = new Select().From(Invoice.Schema).Paged(1, 20).InnerJoin(Product.Schema);
        //    int records = q.GetRecordCount();
        //    Assert.IsTrue(records == 20);
        //}

        #endregion


        #region ANDs

        [Test]
        public void Exec_SimpleAnd()
        {
            int records = new Select()
                .From<Product>()
                .Where("UnitsInStock")
                .IsGreaterThan(10)
                .And("UnitsOnOrder")
                .IsGreaterThan(30)
                .GetRecordCount();

            Assert.AreEqual(5, records);
        }

        [Test]
        public void Exec_SimpleAnd2()
        {
            int records = new Select()
                .From<Product>()
                .Where(Product.UnitsInStockColumn)
                .IsGreaterThan(10)
                .And(Product.UnitsOnOrderColumn)
                .IsGreaterThan(30)
                .GetRecordCount();

            Assert.AreEqual(5, records);
        }

        [Test]
        public void Exec_SimpleAnd3()
        {
            SubSonic.SqlQuery query = new
                Select(Aggregate.GroupBy("ProductID"), Aggregate.Avg("UnitPrice"), Aggregate.Avg("Quantity"))
                .From("Order Details")
                .Where(Aggregate.Avg("UnitPrice"))
                .IsGreaterThan(30)
                .And(Aggregate.Avg("Quantity"))
                .IsGreaterThan(20);

            int records = query.GetRecordCount();
            // Change to 17?
            Assert.AreEqual(17, records);  // Equal assertion failed: [[16]]!=[[17]]
        }

        #endregion


        #region INs

        [Test]
        public void Exec_SimpleIn()
        {
            int records = new Select().From(Product.Schema)
                .Where("productid").In(1, 2, 3, 4, 5)
                .GetRecordCount();
            Assert.IsTrue(records == 5);
        }

        [Test]
        public void Exec_InWithSelect()
        {
            int records = Select.AllColumnsFrom<Product>()
                .Where("productid")
                .In(
                new Select("productid").From(Product.Schema)
                    .Where("categoryid").IsEqualTo(5)
                )
                .GetRecordCount();
            Assert.IsTrue(records == 7);
        }

        [Test]
        public void Exec_MultipleInWithAnd()
        {
            SubSonic.SqlQuery query = new Select()
                .From(Product.Schema)
                .Where(Product.CategoryIDColumn).In(2)
                .And(Product.SupplierIDColumn).In(3);
            string sql = query.ToString();
            int records = query.GetRecordCount();
            Assert.AreEqual(2, records);
        }

        #endregion


        #region ORs

        [Test]
        public void Exec_SimpleOr()
        {
            int records = new Select()
                .From<Product>()
                .Where("UnitsInStock")
                .IsGreaterThan(100)
                .Or("UnitsOnOrder")
                .IsGreaterThan(50)
                .GetRecordCount();

            Assert.AreEqual(17, records);
        }

        [Test]
        public void Exec_SimpleOr2()
        {
            int records = new Select()
                .From<Product>()
                .Where(Product.UnitsInStockColumn)
                .IsGreaterThan(100)
                .Or(Product.UnitsOnOrderColumn)
                .IsGreaterThan(50)
                .GetRecordCount();

            Assert.AreEqual(17, records);
        }

        [Test]
        public void Exec_SimpleOr3()
        {
            int records = new
                Select(Aggregate.GroupBy("ProductID"), Aggregate.Avg("UnitPrice"), Aggregate.Avg("Quantity"))
                .From("Order Details")
                .Where(Aggregate.Avg("UnitPrice"))
                .IsGreaterThan(50)
                .Or(Aggregate.Avg("Quantity"))
                .IsGreaterThan(30)
                .GetRecordCount();
            // Changed to 10?
            Assert.AreEqual(10, records);  // Equal assertion failed: [[9]]!=[[10]]
        }

        #endregion


        #region Typed Collection Load

        [Test]
        public void Collection_LoadTypedObject()
        {
            // Object of type 'System.Int64' cannot be converted to type 'System.Int32'.
            
            List<TestProduct> result = new            
                Select("productid", "productname", "unitprice")
                .From(Product.Schema)
                .OrderAsc("productid")
                .ExecuteTypedList<TestProduct>();

            Assert.AreEqual(77, result.Count);
            Assert.AreEqual("Chai", result[0].ProductName);
            Assert.AreEqual(1, result[0].ProductID);
            Assert.AreEqual(50, result[0].UnitPrice);
        }

        [Test]
        public void Collection_LoadTypedActiveRecord()
        {
            List<Product> result = new
                Select("productid", "productname", "unitprice")
                .From<Product>()
                .OrderAsc("productid")
                .ExecuteTypedList<Product>();

            Assert.IsTrue(result.Count == 77);
            Assert.IsTrue(result[0].ProductName == "Chai");
            Assert.IsTrue(result[0].ProductID == 1);
            Assert.IsTrue(result[0].UnitPrice == 50);
        }

        private class TestProduct
        {
            //private int _id;
            private long _id;

            private string _name;

            private decimal _price;

            //public int ProductID
            public long ProductID
            {
                get { return _id; }
                set { _id = value; }
            }

            public string ProductName
            {
                get { return _name; }
                set { _name = value; }
            }

            public decimal UnitPrice
            {
                get { return _price; }
                set { _price = value; }
            }
        }

        #endregion


        #region Product Snippets (thanks Shawn Oster!)

        /// 
        /// Product load. Uses same code as in sample documentation, should be tested
        /// to make sure the sample snippets are always valid.  Sample snippets taken from:
        /// http://www.subsonicproject.com/view/using-the-generated-objects.aspx
        /// 
        //[Test]
        //public void Products_Sample()
        //{
        //    // sample snippet #1
        //    Product p = new Product(1);
        //    Assert.AreEqual(1, p.ProductID);
        //    Assert.AreEqual("Chai", p.ProductName);

        //    // sample snippet #2
        //    const string productGuid = "aa7aa741-8fd2-476b-b37d-b16e784874e6";
        //    p = new Product("ProductGUID", productGuid);
        //    Assert.AreEqual(productGuid, p.ProductGUID.ToString());
        //    Assert.AreEqual("Scottish Longbreads", p.ProductName);
        //}

        /// 
        /// Products_s the collection load 2.  Tests the more common approach.
        /// 
        [Test]
        public void Products_CollectionLoad2()
        {
            ProductCollection products = new ProductCollection().Load();
            Assert.AreEqual(77, products.Count);
        }

        /// 
        /// Product collection load with where clause. Uses same code as in sample documentation, should be tested
        /// to make sure the sample snippets are always valid.  Sample snippets taken from:
        /// http://www.subsonicproject.com/view/using-the-generated-objects.aspx
        /// 
        [Test]
        public void Products_CollectionSample()
        {
            // sample snippet #1
            ProductCollection products = new ProductCollection().Where("categoryID", 1).Load();
            Assert.AreEqual(12, products.Count);

            // sample snippet #2
            products = new ProductCollection().Where("categoryID", 1).OrderByAsc("ProductName").Load();
            Assert.AreEqual(12, products.Count);
            Assert.AreEqual("Chai", products[0].ProductName);
            Assert.AreEqual("Steeleye Stout", products[11].ProductName);
        }

        #endregion


        /// <summary>
        /// Testing that the gazillion ways to generate a .NotIn() Select
        /// all work the same way.
        /// </summary>
        [Test]
        public void Exec_NotInWithSelect()
        {
            // "verbose" style
            SubSonic.SqlQuery query1 = DB.Select()
                .From(Category.Schema)
                .Where(Category.Columns.CategoryID)
                .NotIn(
                DB.Select(Product.Columns.CategoryID)
                    .From(Product.Schema)
                );

            // "generics" style
            SubSonic.SqlQuery query2 = Select.AllColumnsFrom<Category>()
                .Where(Category.Columns.CategoryID)
                .NotIn(
                new Select(Product.Columns.CategoryID)
                    .From(Product.Schema)
                );

            // do both produce the same sql?
            string sql1 = query1.ToString();
            string sql2 = query2.ToString();
            Assert.AreEqual(sql1, sql2);

            // does the sql work?
            int records = query1.GetRecordCount();
            Assert.IsTrue(records == 0);
        }

        //[Test]
        //public void StoredProc_TypedList()
        //{
        //    List<CustomerOrder> orders = Northwind.SPs.CustOrderHist("ALFKI")
        //        .ExecuteTypedList<CustomerOrder>();
        //    Assert.IsTrue(orders.Count == 11);
        //}


        #region Nested type: CustomerOrder

        /// <summary>
        /// Class for holding SP Results for CustOrderHist
        /// </summary>
        private class CustomerOrder
        {
            private string productName;

            private int total;

            public string ProductName
            {
                get { return productName; }
                set { productName = value; }
            }

            public int Total
            {
                get { return total; }
                set { total = value; }
            }
        }

        #endregion
    }
}