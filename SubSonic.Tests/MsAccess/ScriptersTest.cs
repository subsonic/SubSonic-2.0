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

namespace SubSonic.Tests.MsAccess
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
    /// public void Acc_MyTestInitialize() { }
    ///
    /// Use TestCleanup to run code after each test has run
    /// [TestCleanup()]
    /// public void Acc_MyTestCleanup() { }
    ///
    /// </summary>
    [TestFixture]
    public class ScriptersTest
    {
        /// <summary>
        /// Script_s the data.
        /// </summary>
        [Test]
        public void Acc_Script_Data()
        {
            string sql = DataService.ScriptData("Categories", "NorthwindAccess");
            Assert.IsTrue(sql.Length > 0);
        }

        /// <summary>
        /// Script_s all data.
        /// </summary>
        [Test]
        public void Acc_Script_AllData()
        {
            string sql = DataService.ScriptData("NorthwindAccess");
            Assert.IsTrue(sql.Length > 0);
        }
    }
}