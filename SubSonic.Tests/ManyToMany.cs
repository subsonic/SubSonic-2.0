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
    /// Summary for the ManyToMany class
    /// </summary>
    [TestFixture]
    public class ManyToMany
    {
        /*
        /// <summary>
        /// M2s the m_ full test.
        /// </summary>
        [Test]
        public void M2M_FullTest()
        {
            //using the Product, make sure that all the M2M bits work
            Product.DeleteCategoryMap(1);

            int[] cats = new int[] {1, 2, 3};
            Product.SaveCategoryMap(1, cats);

            CategoryCollection coll = Product.GetCategoryCollection(1);
            Assert.IsTrue(coll.Count == 3);
        }

        /// <summary>
        /// M2s the m_ object definitions.
        /// </summary>
        [Test]
        public void M2M_ObjectDefinitions()
        {
            //using the Product and Category classes, make sure that all the M2M bits work
            TableSchema.Table tbl = DataService.GetTableSchema("Products", "Northwind");

            AssertM2M_ForeignKey(tbl.ManyToManys, "Categories", "CategoryID");
            AssertM2M_ForeignKey(tbl.ManyToManys, "Orders", "OrderID");

            tbl = DataService.GetTableSchema("Categories", "Northwind");
            AssertM2M_ForeignKey(tbl.ManyToManys, "Products", "ProductID");
        }

        /// <summary>
        /// Asserts the m2 m_ foreign key.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fkName">Name of the fk.</param>
        private static void AssertM2M_ForeignKey(IEnumerable<TableSchema.ManyToManyRelationship> collection, string tableName, string fkName)
        {
            foreach(TableSchema.ManyToManyRelationship mm in collection)
            {
                if(mm.ForeignPrimaryKey == fkName && mm.ForeignTableName == tableName)
                    return;
            }

            Assert.Fail("ForeignKey PrimaryKey {0}.{1} not found", tableName, fkName);
        }
         * 
         * */
    }
}