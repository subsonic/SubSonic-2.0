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
#if ALLPROVIDERS
using MbUnit.Framework;
using SubSonic.Utilities;

namespace SubSonic.Tests.MsAccess
{
    /// <summary>
    /// Summary for the MultiProvider class
    /// </summary>
    [TestFixture]
    public class MySqlTests
    {
        [Test]
        public void Acc_CorrectSchemaLoad()
        {
            MySqlDataProvider prov = (MySqlDataProvider)DataService.Providers["Southwind"];
            TableSchema.Table products = prov.GetTableSchema("Products", TableType.Table);
            //Assert.IsTrue(products.GetColumn("IsDeleted").IsNullable == true); Not in MySql schema
            Assert.IsTrue(products.GetColumn("Discontinued").IsNullable == false);

            TableSchema.TableColumn col = products.GetColumn("Discontinued");

            ICodeLanguage lang = new CSharpCodeLanguage();
            string varTypeNonNull = Utility.GetVariableType(col.DataType, col.IsNullable, lang);
            Assert.IsTrue(varTypeNonNull == "bool");

            //Not in MySql Schema
            //col = products.GetColumn("IsDeleted");

            //string varTypeNullable = Utilities.Utility.GetVariableType(col.DataType, col.IsNullable, lang);
            //Assert.IsTrue(varTypeNullable == "bool?");
        }
    }
}

#endif