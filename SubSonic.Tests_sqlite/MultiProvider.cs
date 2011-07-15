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
using Northwind;

namespace SubSonic.Tests
{
    /// <summary>
    /// Summary for the MultiProvider class
    /// </summary>
    [TestFixture]
    public class MultiProvider
    {
        /// <summary>
        /// Multi_s the query test.
        /// </summary>
        //[Test]
        //public void Multi_QueryTest()
        //{
        //    //hit both the Northwind and Southwind databases
        //    Query query1 = new Query("Products", "Northwind");
        //    DataSet ds1 = query1.ExecuteDataSet();
        //    Assert.IsTrue(ds1.Tables[0].Rows.Count > 0);

        //    Query query2 = new Query("Products", "Southwind");
        //    DataSet ds2 = query2.ExecuteDataSet();
        //    Assert.IsTrue(ds2.Tables[0].Rows.Count > 0);
        //}

        [Test]
        public void Multiple_Providers_Should_Return_Correct_ProviderName()
        {
            SubSonic.SqlQuery query1 = DB.Select();
            //SubSonic.SqlQuery query2 = Southwind.DB.Select();
            Assert.AreEqual("Northwind", query1.ProviderName);
            //Assert.AreEqual("Southwind", query2.ProviderName);
        }
    }
}