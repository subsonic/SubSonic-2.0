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

using System.ServiceProcess;
using System.Transactions;
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests
{
    /// <summary>
    /// Summary for the TransactionTests class
    /// </summary>
    [TestFixture]
    public class TransactionTests
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly MsDtcService msdtc = new MsDtcService();

        /// <summary>
        /// Tests the fixture set up.
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            msdtc.Start();
        }

        /// <summary>
        /// Tests the fixture tear down.
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            msdtc.Revert();
        }

        /// <summary>
        /// Transaction_s the rollback product.
        /// </summary>
        [Test]
        public void Transaction_RollbackProduct()
        {
            //use the product object to test the transaction scope
            using(TransactionScope scope = new TransactionScope())
            {
                UpdateProduct(1, "20 pounds");
                UpdateProduct(2, "20 pounds");
                UpdateProduct(3, "20 pounds");
            }

            //the saves shouldn't be committed, hopefully!
            Product pVal = new Product(1);
            Assert.IsTrue(pVal.QuantityPerUnit != "20 pounds");
        }

        /// <summary>
        /// Transaction_s the commit product.
        /// </summary>
        [Test]
        public void Transaction_CommitProduct()
        {
            using(TransactionScope scope = new TransactionScope())
            {
                //try
                //{
                UpdateProduct(1, "10 boxes x 30 bags");
                UpdateProduct(2, "24 - 42 oz bottles");
                UpdateProduct(3, "12 - 5500 ml bottles");
                scope.Complete();
                //}
                //catch(System.Data.SqlClient.SqlException x)
                //{
                //    //trap/trace/log as needed
                //    throw x;
                //}
            }

            //the saves shouldn't be committed, hopefully!
            Product pVal = new Product(1);
            Assert.AreEqual("10 boxes x 30 bags", pVal.QuantityPerUnit);

            //set them back
            UpdateProduct(1, "10 boxes x 20 bags");
            UpdateProduct(2, "24 - 12 oz bottles");
            UpdateProduct(3, "12 - 550 ml bottles");
        }

        /// <summary>
        /// Updates the product.
        /// </summary>
        /// <param name="productId">The product id.</param>
        /// <param name="quantityPerUnit">The quantity per unit.</param>
        private static void UpdateProduct(int productId, string quantityPerUnit)
        {
            Product p1 = new Product(productId);
            p1.QuantityPerUnit = quantityPerUnit;
            p1.UnitPrice = 50;
            p1.Save("Unit Test");
        }
    }

    /// <summary>
    /// Summary for the MsDtcService class
    /// </summary>
    public class MsDtcService
    {
        private readonly ServiceControllerStatus _originalDtcStatus;
        // instantiate ServiceController each time to get the correct Status
        private ServiceController serviceController;

        /// <summary>
        /// Initializes a new instance of the <see cref="MsDtcService"/> class.
        /// </summary>
        public MsDtcService()
        {
            _originalDtcStatus = Controller.Status;
        }

        /// <summary>
        /// Gets the controller.
        /// </summary>
        /// <value>The controller.</value>
        public ServiceController Controller
        {
            get
            {
                if(serviceController == null)
                    serviceController = new ServiceController("MSDTC");
                return serviceController;
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if(Controller.Status != ServiceControllerStatus.Running && Controller.Status != ServiceControllerStatus.StartPending)
                Controller.Start();

            Controller.WaitForStatus(ServiceControllerStatus.Running);
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            if(Controller.Status == ServiceControllerStatus.Stopped)
                return;

            if(Controller.Status != ServiceControllerStatus.StopPending)
                Controller.Stop();

            Controller.WaitForStatus(ServiceControllerStatus.Stopped);
        }

        /// <summary>
        /// Pauses this instance.
        /// </summary>
        public void Pause()
        {
            if(Controller.Status != ServiceControllerStatus.Paused && Controller.Status != ServiceControllerStatus.PausePending)
            {
                if(Controller.Status != ServiceControllerStatus.Running && Controller.Status != ServiceControllerStatus.StartPending)
                    Controller.Start();
                Controller.Pause();
            }
            Controller.WaitForStatus(ServiceControllerStatus.Paused);
        }

        /// <summary>
        /// Reverts this instance.
        /// </summary>
        public void Revert()
        {
            switch(_originalDtcStatus)
            {
                case ServiceControllerStatus.Stopped:
                    Stop();
                    break;
                case ServiceControllerStatus.Paused:
                case ServiceControllerStatus.PausePending:
                    Pause();
                    break;
                case ServiceControllerStatus.ContinuePending:
                case ServiceControllerStatus.Running:
                case ServiceControllerStatus.StartPending:
                    Start();
                    break;
            }
        }
    }
}