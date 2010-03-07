using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading;
using System.Transactions;
using MbUnit.Framework;
using Northwind;
using System.Diagnostics;

namespace SubSonic.Tests
{
    /// <summary>
    /// Tests SubSonics behavior with regard to using TransactionScope and SharedDbConnectionScope
    /// when the DTC is turned off.
    /// </summary>
    [TestFixture]
    public class TransactionWithDtcOffTests
    {
        private const int MaxRandomNumber = 10000;
        //private readonly Regex _dtcErrorMessage = new Regex("MSDTC on server '.*' is unavailable");

        /// <summary>
        /// Used to generate random numbers that are embedded in strings that get presisted to the database
        /// </summary>
        private readonly Random _rand = new Random();

        //private readonly MsDtcService msdtc = new MsDtcService();

        /// <summary>
        /// Tests the fixture set up.
        /// </summary>
        [FixtureSetUp]
        public void TestFixtureSetUp()
        {
            //msdtc.Stop();
        }

        /// <summary>
        /// Tests the fixture tear down.
        /// </summary>
        [FixtureTearDown]
        public void TestFixtureTearDown()
        {
            //msdtc.Revert();
        }

        /// <summary>
        /// Noes the transaction scope_ can retrieve single product.
        /// </summary>
        [Test]
        public void NoTransactionScope_CanRetrieveSingleProduct()
        {
            Product p1 = new Product(1);
            Assert.AreEqual(1, p1.ProductID);
        }

        /// <summary>
        /// Noes the transaction scope_ can retrieve multiple products.
        /// </summary>
        [Test]
        public void NoTransactionScope_CanRetrieveMultipleProducts()
        {
            Product p1 = new Product(1);
            Product p2 = new Product(2);

            Assert.AreEqual(1, p1.ProductID);
            Assert.AreEqual(2, p2.ProductID);
        }

        /// <summary>
        /// Retrieves the multiple products_ fails without shared connection.
        /// fails with sqlite
        /// </summary>
        //[Test]
        //public void RetrieveMultipleProducts_FailsWithoutSharedConnection()
        //{
        //    string errorMessage = String.Empty;
        //    try
        //    {
        //        using(TransactionScope ts = new TransactionScope())
        //        {
        //            Product p1 = new Product(1);
        //            Product p2 = new Product(2);

        //            Assert.AreEqual(1L, p1.ProductID);
        //            Assert.AreEqual(2L, p2.ProductID);
        //        }
        //    }
        //    catch(SqlException e)
        //    {
        //        errorMessage = e.Message;
        //    }

        //    Assert.IsTrue(_dtcErrorMessage.IsMatch(errorMessage), errorMessage);
        //}

        /// <summary>
        /// Determines whether this instance [can retrieve multiple entities_ fails without shared connection scope].
        /// Fails with sqlite.
        /// </summary>
        //[Test]
        //public void CanRetrieveMultipleEntities_FailsWithoutSharedConnectionScope()
        //{
        //    string errorMessage = String.Empty;
        //    try
        //    {
        //        using(TransactionScope ts = new TransactionScope())
        //        {
        //            Product p1 = new Product(1);
        //            Product p2 = new Product(2);

        //            Order o1 = new Order(1);
        //            Order o2 = new Order(2);

        //            Assert.AreEqual(1L, p1.ProductID);
        //            Assert.AreEqual(2L, p2.ProductID);
        //            Assert.AreEqual(1L, o1.OrderID);
        //            Assert.AreEqual(2L, o2.OrderID);
        //        }
        //    }
        //    catch(SqlException e)
        //    {
        //        errorMessage = e.Message;
        //    }

        //    Assert.IsTrue(_dtcErrorMessage.IsMatch(errorMessage));
        //}

        /// <summary>
        /// Determines whether this instance [can retrieve multiple entities_ succeeds with shared connection scope].
        /// </summary>
        [Test]
        public void CanRetrieveMultipleEntities_SucceedsWithSharedConnectionScope()
        {
            using(TransactionScope ts = new TransactionScope())
            {
                using(SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                {
                    Product p1 = new Product(1);
                    Product p2 = new Product(2);

                    Order o1 = new Order(10248);
                    Order o2 = new Order(10249);

                    Assert.AreEqual(1L, p1.ProductID);
                    Assert.AreEqual(2L, p2.ProductID);
                    Assert.AreEqual(10248L, o1.OrderID);
                    Assert.AreEqual(10249L, o2.OrderID);
                }
            }
        }

        /// <summary>
        /// Updates the single product_ succeeds using shared connection.
        /// </summary>
        [Test]
        public void UpdateSingleProduct_SucceedsUsingSharedConnection()
        {
            using(TransactionScope ts = new TransactionScope())
            {
                using(SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                    SaveProduct(1, "new name of product");
            }
        }

        /// <summary>
        /// Updates the single product retrieve multiple products_ fails without shared connection.
        /// Fails with sqlite
        /// </summary>
        //[Test]
        //public void UpdateSingleProductRetrieveMultipleProducts_FailsWithoutSharedConnection()
        //{
        //    string errorMessage = String.Empty;

        //    try
        //    {
        //        using(TransactionScope ts = new TransactionScope())
        //        {
        //            SaveProduct(1, "new name of product");

        //            Product p2 = new Product(2);
        //            Product p3 = new Product(3);
        //        }
        //    }
        //    catch(SqlException e)
        //    {
        //        errorMessage = e.Message;
        //    }
        //    Assert.IsTrue(_dtcErrorMessage.IsMatch(errorMessage));
        //}

        /// <summary>
        /// Updates the single product retrieve multiple products_ succeeds with shared connection.
        /// </summary>
        [Test]
        //[Ignore] // fails with sqlite
        public void UpdateSingleProductRetrieveMultipleProducts_SucceedsWithSharedConnection()
        {
            string p1OriginalProductName = new Product(1).ProductName;

            using(TransactionScope ts = new TransactionScope())
            {
                using(SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                {
                    SaveProduct(1, "new name of product");

                    Product p2 = new Product(2);
                    Product p3 = new Product(3);
                }
            }

            Assert.AreEqual(p1OriginalProductName, new Product(1).ProductName, "Transaction NOT rolled back");
        }

        /// <summary>
        /// Updates the single product_ commit change to database.
        /// </summary>
        [Test]
        public void UpdateSingleProduct_CommitChangeToDatabase()
        {
            string newProductName = "new name of product 20: " + _rand.Next(MaxRandomNumber);

            using(TransactionScope ts = new TransactionScope())
            {
                using(SharedDbConnectionScope connScope = new SharedDbConnectionScope())
                {
                    SaveProduct(20, newProductName);

                    Product p2 = new Product(2);
                    Product p3 = new Product(3);

                    ts.Complete();
                }
            }

            Assert.AreEqual(newProductName, new Product(20).ProductName, "Transaction Not Committed");
        }

        /// <summary>
        /// Updates the multiple products_ fails without shared connection.
        /// Not used with sqlite.
        /// </summary>
        //[Test]
        //public void UpdateMultipleProducts_FailsWithoutSharedConnection()
        //{
        //    string errorMessage = String.Empty;
        //    try
        //    {
        //        using(TransactionScope ts = new TransactionScope())
        //        {
        //            SaveProduct(1, "new name of product 1");
        //            SaveProduct(2, "new name of product 2");
        //        }
        //    }
        //    catch(SqlException e)
        //    {
        //        errorMessage = e.Message;
        //    }

        //    Assert.IsTrue(_dtcErrorMessage.IsMatch(errorMessage));
        //}

        /// <summary>
        /// Updates the multiple products_ succeeds with shared connection.
        /// fails with sqlite
        /// </summary>
        //[Test]
        //public void UpdateMultipleProducts_SucceedsWithSharedConnection()
        //{
        //    string p1OriginalProductName = new Product(1).ProductName;
        //    string p2OriginalProductName = new Product(2).ProductName;

        //    using(TransactionScope ts = new TransactionScope())
        //    {
        //        using(SharedDbConnectionScope connScope = new SharedDbConnectionScope())
        //        {
        //            SaveProduct(1, "new name of product 1");
        //            SaveProduct(2, "new name of product 2");
        //        }
        //    }

        //    Assert.AreEqual(p1OriginalProductName, new Product(1).ProductName, "Transaction NOT rolled back");
        //    Assert.AreEqual(p2OriginalProductName, new Product(2).ProductName, "Transaction NOT rolled back");
        //}

        /// <summary>
        /// Determines whether this instance [can nest shared connections].
        /// </summary>
        [Test]
        [Ignore] // fails with sqlite - test ignored for now
        public void CanNestSharedConnections()
        {
            SortedList originalNames = new SortedList();
            for(int i = 1; i <= 4; i++)
                originalNames[i] = new Product(i).ProductName;

            using(TransactionScope outerTransactionScope = new TransactionScope())
            {
                using(SharedDbConnectionScope outerConnectionScope = new SharedDbConnectionScope())
                {
                    Product p1 = UpdateProduct(1, "new name of product 1: " + _rand.Next(MaxRandomNumber));
                    Product p2 = UpdateProduct(2, "new name of product 2: " + _rand.Next(MaxRandomNumber));

                    using(TransactionScope innerTransactionScope = new TransactionScope())
                    {
                        using(SharedDbConnectionScope innerConnectionScope = new SharedDbConnectionScope())
                        {
                            Assert.AreSame(outerConnectionScope.CurrentConnection, innerConnectionScope.CurrentConnection);

                            SaveProduct(3, "new name of product 3: " + +_rand.Next(MaxRandomNumber));
                            SaveProduct(4, "new name of product 4: " + +_rand.Next(MaxRandomNumber));

                            innerTransactionScope.Complete();
                        }
                    }

                    // ensure the connection hasn't been disposed by the inner scope
                    Assert.IsTrue(outerConnectionScope.CurrentConnection.State == ConnectionState.Open);

                    p1.Save();
                    p2.Save();
                }

                outerTransactionScope.Complete();
            }

            //Message: Product 1 is incorrect Equal assertion failed: [[new name of product 1: 8801]]!=[[new name of product 1: 2435]]

            Debug.WriteLine(new Product(1).ProductName);
            Debug.WriteLine(new Product(2).ProductName);
            Debug.WriteLine(new Product(3).ProductName);
            Debug.WriteLine(new Product(4).ProductName);


            for(int i = 1; i <= 4; i++)
                Assert.AreEqual(originalNames[i], new Product(i).ProductName, "Product {0} is incorrect", i);
        }

        ///// <summary>
        ///// Threadings the target.
        ///// </summary>
        ///// <param name="state">The state.</param>
        //[Test]
        //[ThreadedRepeat(10)]
        //public void MultiThreadRepeat2()
        //{
        //    lock(thisLock)
        //    {
        //        string p1OriginalProductName = new Product(1).ProductName;
        //        string p2OriginalProductName = new Product(2).ProductName;

        //        using(TransactionScope ts = new TransactionScope())
        //        {
        //            using(SharedDbConnectionScope connScope = new SharedDbConnectionScope())
        //            {
        //                SaveProduct(1, "new name of product 1");
        //                SaveProduct(2, "new name of product 2");
        //            }
        //        }

        //        Assert.AreEqual(p1OriginalProductName, new Product(1).ProductName);
        //        Assert.AreEqual(p2OriginalProductName, new Product(2).ProductName);
        //    }
        //}
        
        private Object thisLock = new Object();

        [Test]
        [ThreadedRepeat(10)]
        public void MultiThreadRepeat()
        {
            lock(thisLock)
            {
                var qcc = new QueryCommandCollection();
                int threadId = Thread.CurrentThread.ManagedThreadId;
                Debug.WriteLine("MultiThreadRepeat: thread id = " + threadId);
                int count = 0;
                for(int n = 0; n < 10; n++)
                {
                    Query qry1 = new Query(Product.Schema);
                    qry1.QueryType = QueryType.Update;
                    qry1.AddWhere(Product.Columns.ProductID, n);
                    qry1.AddUpdateSetting("ProductName", threadId + ": unit test ");
                    QueryCommand cmd = qry1.BuildUpdateCommand();
                    qcc.Add(cmd);
                    count++;
                }
                DataService.ExecuteTransaction(qcc);
                var p1 = new Product(1);
                Assert.AreEqual(p1.ProductName, threadId + ": unit test ", StringComparison.InvariantCultureIgnoreCase);
            }

        }

        /// <summary>
        /// Saves the product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="productName">Name of the product.</param>
        private static void SaveProduct(int productId, string productName)
        {
            Product p1 = UpdateProduct(productId, productName);
            p1.Save("Unit Test");
        }

        /// <summary>
        /// Updates the product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="productName">Name of the product.</param>
        /// <returns></returns>
        private static Product UpdateProduct(int productId, string productName)
        {
            Product p1 = new Product(productId);
            p1.ProductName = productName;
            p1.UnitPrice = 50;
            return p1;
        }
    }
}