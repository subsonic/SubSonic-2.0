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
using System.Data;
using MbUnit.Framework;
using System.Xml;

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
    public class SPs
    {
        [Test]
        [Rollback]
        public void SP_Schema_Should_Load_10_SPs()
        {
            List<StoredProcedure> sps = DataService.GetSPSchemaCollection("Northwind");
            Assert.AreEqual(10, sps.Count);
        }

        [Test]
        [Rollback]
        public void SP_Schema_Should_Load_With_Schema_Name()
        {
            List<StoredProcedure> sps = DataService.GetSPSchemaCollection("Northwind");
            Assert.AreEqual("dbo", sps[0].SchemaName);
        }

        [Test]
        [Rollback]
        public void SP_CustOrderHist_Should_Have_QualifiedName()
        {
            List<StoredProcedure> sps = DataService.GetSPSchemaCollection("Northwind");
            Assert.AreEqual("[dbo].[CustOrderHist]", sps[0].QualifiedName);
        }

        [Test]
        [Rollback]
        public void SP_CustOrderHist_Should_Have_Qualified_CommandSQL()
        {
            List<StoredProcedure> sps = DataService.GetSPSchemaCollection("Northwind");
            Assert.AreEqual("[dbo].[CustOrderHist]", sps[0].Command.CommandSql);
        }

        [Test]
        [Rollback]
        public void SP_CustOrderHist_TypedSP_Should_Have_Qualified_CommandSQL()
        {
            StoredProcedure sp = Northwind.SPs.CustOrderHist("ALFKI").GetBuiltProcedure();
            Assert.AreEqual("[dbo].[CustOrderHist]", sp.Command.CommandSql);
        }

        /// <summary>
        /// Ss the p_ reader test.
        /// </summary>
        [Test]
        [Rollback]
        public void SP_ReaderTest()
        {
            StoredProcedure sp = new StoredProcedure("CustOrderHist", DataService.GetInstance("Northwind"));
            sp.Command.AddParameter("@CustomerID", "ALFKI", DbType.AnsiString);

            int counter = 0;
            using(IDataReader rdr = sp.GetReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter > 0);
        }

        /// <summary>
        /// Ss the p_ scalar test.
        /// </summary>
        [Test]
        [Rollback]
        public void SP_ScalarTest()
        {
            StoredProcedure sp = new StoredProcedure("CustOrderHist", DataService.GetInstance("Northwind"));
            sp.Command.AddParameter("@CustomerID", "ALFKI", DbType.AnsiString);
            object result = sp.ExecuteScalar();

            Assert.IsTrue(result != null);
        }

        /// <summary>
        /// Ss the p_ DS test.
        /// </summary>
        [Test]
        [Rollback]
        public void SP_DSTest()
        {
            StoredProcedure sp = new StoredProcedure("CustOrderHist", DataService.GetInstance("Northwind"));
            sp.Command.AddParameter("@CustomerID", "ALFKI", DbType.AnsiString);
            DataSet ds = sp.GetDataSet();

            Assert.IsTrue(ds.Tables[0].Rows.Count > 0);
        }

        /// <summary>
        /// Ss the p_ outputs.
        /// </summary>
        [Test]
        [Rollback]
        public void SP_Outputs()
        {
            StoredProcedure sp = new StoredProcedure("SubSonicTest", DataService.GetInstance("Northwind"));
            sp.Command.AddOutputParameter("@param");
            sp.Execute();

            //make sure there's outputs
            Assert.IsTrue(sp.OutputValues.Count > 0);

            //this SP just returns today's date.
            //make sure it's right now!
            DateTime dTest = Convert.ToDateTime(sp.OutputValues[0]);
            Assert.IsTrue(dTest.Date == DateTime.Now.Date);
        }

        /// <summary>
        /// Ss the p_ outputs_ default provider.
        /// </summary>
        [Test]
        [Rollback]
        public void SP_Outputs_DefaultProvider()
        {
            StoredProcedure sp = new StoredProcedure("SubSonicTestNW", DataService.GetInstance("Northwind"));
            sp.Command.AddOutputParameter("@param");
            sp.Execute();

            //make sure there's outputs
            Assert.IsTrue(sp.OutputValues.Count > 0);

            //this SP just returns today's date.
            //make sure it's right now!
            DateTime dTest = Convert.ToDateTime(sp.OutputValues[0]);
            Assert.IsTrue(dTest.Date == DateTime.Now.Date);
        }

        /// <summary>
        /// Ss the p_ quick reader.
        /// </summary>
        [Test]
        [Rollback]
        public void SP_QuickReader()
        {
            int count = 0;
            using(IDataReader rdr = StoredProcedure.GetReader("CustOrderHist 'ALFKI'"))
            {
                while(rdr.Read())
                    count++;
                rdr.Close();
            }
            Assert.IsTrue(count == 11);
        }

        /// <summary>
        /// Ss the p_ quick data set.
        /// </summary>
        [Test]
        [Rollback]
        public void SP_QuickDataSet()
        {
            DataSet ds = StoredProcedure.GetDataSet("CustOrderHist 'ALFKI'");
            Assert.IsTrue(ds.Tables[0].Rows.Count == 11);
        }

        [Test]
        public void SP_Should_Execute_Without_Parameter() {
            int count = 0;
            using (IDataReader rdr = Northwind.SPs.TenMostExpensiveProducts().GetReader()) {
                while (rdr.Read())
                    count++;
                rdr.Close();
            }

            Assert.IsTrue(count > 0);
        }
        [Test]
        public void SP_Should_Execute_Without_Parameter2()
        {
            StoredProcedure sp = new StoredProcedure("Ten Most Expensive Products", DataService.Providers["Northwind"]);
            int count = 0;
            using (IDataReader rdr =sp.GetReader())
            {
                while (rdr.Read())
                    count++;
                rdr.Close();
            }
            sp.Execute();
            Assert.IsTrue(count > 0);
        }

        [Test]
        public void SP_Can_Execute_JSON_Result() {


            string result = Northwind.SPs.TenMostExpensiveProducts().GetDataSet().GetXml();
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(result);
            string json=SubSonic.Parser.XmlToJSONParser.XmlToJSON(xdoc);


            Assert.IsTrue(json.Length > 0);

        }

    }
}