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

using MbUnit.Framework;

namespace SubSonic.Tests
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
    /// public void MyTestInitialize() { }
    ///
    /// Use TestCleanup to run code after each test has run
    /// [TestCleanup()]
    /// public void MyTestCleanup() { }
    ///
    /// </summary>
    [TestFixture]
    public class ScriptersTest
    {
        /// <summary>
        /// Script_s the data.
        /// </summary>
        [Test]
        [Rollback]
        public void Script_Data()
        {
            string sql = DataService.ScriptData("Categories", "Northwind");
            Assert.IsTrue(sql.Length > 0);
        }

        /// <summary>
        /// Script_s all data.
        /// </summary>
        [Test]
        [Rollback]
        public void Script_AllData()
        {
            string sql = DataService.ScriptData("Northwind");
            Assert.IsTrue(sql.Length > 0);
        }
    }
}