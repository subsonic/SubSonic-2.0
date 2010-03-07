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

namespace SubSonic.Tests.SqlQuery
{
    [TestFixture]
    public class SQLiteParserTests
    {
        [Test]
        public void Select_ColumnList_Specified()
        {
            SubSonic.SqlQuery qry = new Select("productid", "productname").From(Product.Schema);
            ANSISqlGenerator gen = new ANSISqlGenerator(qry);

            string selectList = gen.GenerateCommandLine();

            Assert.AreEqual("SELECT `main`.`Products`.`ProductID`, `main`.`Products`.`ProductName`\r\n", selectList);
        }

        [Test]
        public void Select_Generate_FromList()
        {
            Select qry = Select.AllColumnsFrom<Product>();
            ANSISqlGenerator gen = new ANSISqlGenerator(qry);

            string from = gen.GenerateFromList();

            Assert.AreEqual(" FROM `main`.`Products`\r\n", from);
        }

        [Test]
        public void Select_Generate_JoinList()
        {
            SubSonic.SqlQuery qry = Select.AllColumnsFrom<Product>()
                .InnerJoin(Category.Schema)
                .InnerJoin(Supplier.Schema);

            ANSISqlGenerator gen = new ANSISqlGenerator(qry);

            string joins = gen.GenerateJoins();

            Assert.AreEqual(
                " INNER JOIN `main`.`Categories` ON `main`.`Products`.`CategoryID` = `main`.`Categories`.`CategoryID`\r\n INNER JOIN `main`.`Suppliers` ON `main`.`Products`.`SupplierID` = `main`.`Suppliers`.`SupplierID`\r\n",
                joins);
        }

        [Test]
        public void Join_CreateImplicit()
        {
            Join j = new Join(Product.Schema, Category.Schema, Join.JoinType.Inner);
            Assert.IsTrue(j.FromColumn.ColumnName == "CategoryID" && j.FromColumn.Table.Name == "Products");
            Assert.IsTrue(j.ToColumn.ColumnName == "CategoryID" && j.ToColumn.Table.Name == "Categories");
        }

        [Test]
        public void Join_CreateImplicit_Reversed()
        {
            Join j = new Join(Category.Schema, Product.Schema, Join.JoinType.Inner);
            Assert.IsTrue(j.FromColumn.ColumnName == "CategoryID" && j.FromColumn.Table.Name == "Categories");
            Assert.IsTrue(j.ToColumn.ColumnName == "CategoryID" && j.ToColumn.Table.Name == "Products");
        }

        [Test]
        public void Where_Simple()
        {
            SubSonic.SqlQuery qry = Select.AllColumnsFrom<Product>().Where("productID").IsGreaterThan(5);
            ANSISqlGenerator gen = new ANSISqlGenerator(qry);
            string w = gen.GenerateWhere();
            Assert.AreEqual(" WHERE `main`.`Products`.`ProductID` > @ProductID0\r\n", w);
        }

        [Test]
        public void Where_And()
        {
            SubSonic.SqlQuery qry = Select.AllColumnsFrom<Product>().Where("productID").IsGreaterThan(5).And("categoryID").IsEqualTo(5);
            ANSISqlGenerator gen = new ANSISqlGenerator(qry);
            string w = gen.GenerateWhere();
            Assert.AreEqual(
                " WHERE `main`.`Products`.`ProductID` > @ProductID0\r\n AND `main`.`Products`.`CategoryID` = @CategoryID1\r\n",
                w);
        }

        [Test]
        public void Select_BetweenAnd()
        {
            SubSonic.SqlQuery q = new Select("productid").From(Product.Schema).Where("productid").IsBetweenAnd(2, 5);
            string sql = q.BuildSqlStatement();
            Assert.AreEqual(
                "SELECT `main`.`Products`.`ProductID`\r\n FROM `main`.`Products`\r\n WHERE `main`.`Products`.`ProductID` BETWEEN @ProductID0_start AND @ProductID0_end\r\n",
                sql);
        }

        [Test]
        public void Aggregate_Count()
        {
            SubSonic.SqlQuery q = new Select(
                Aggregate.Count("ProductID"),
                Aggregate.Sum("UnitPrice", "boots"),
                Aggregate.GroupBy("categoryID"))
                .From("Products").Where("CategoryID").IsGreaterThan(5)
                .OrderAsc("categoryID").OrderDesc("boots");
            string sql = q.BuildSqlStatement();
            Assert.AreEqual(
                "SELECT COUNT(ProductID) AS 'CountOfProductID', SUM(UnitPrice) AS 'boots', categoryID AS 'GroupByOfcategoryID'\r\n FROM `main`.`Products`\r\n WHERE `main`.`Products`.`CategoryID` > @CategoryID0\r\n GROUP BY categoryID\r\n ORDER BY categoryID ASC,boots DESC\r\n",
                sql);
        }

        [Test]
        public void Where_Expression()
        {
            SubSonic.SqlQuery q = Select.AllColumnsFrom<Product>()
                .WhereExpression("categoryID").IsLessThan(5).And("ProductID").IsGreaterThan(3).CloseExpression()
                .OrExpression("categoryID").IsGreaterThan(8).And("productID").IsLessThan(2).CloseExpression();
            string sql = q.ToString();


            string expected = 
            "SELECT `main`.`Products`.`ProductID`, `main`.`Products`.`ProductName`, `main`.`Products`.`SupplierID`, `main`.`Products`.`CategoryID`, `main`.`Products`.`QuantityPerUnit`, `main`.`Products`.`UnitPrice`, `main`.`Products`.`UnitsInStock`, `main`.`Products`.`UnitsOnOrder`, `main`.`Products`.`ReorderLevel`, `main`.`Products`.`Discontinued`, `main`.`Products`.`AttributeXML`, `main`.`Products`.`DateCreated`, `main`.`Products`.`ProductGUID`, `main`.`Products`.`CreatedOn`, `main`.`Products`.`CreatedBy`, `main`.`Products`.`ModifiedOn`, `main`.`Products`.`ModifiedBy`, `main`.`Products`.`Deleted`\r\n\r\n" +
            " FROM `main`.`Products`\r\n" +
            " WHERE (`main`.`Products`.`CategoryID` < @CategoryID0\r\n" +
            " AND `main`.`Products`.`ProductID` > @ProductID1\r\n" +
            ")\r\n" +
            " OR (`main`.`Products`.`CategoryID` > @CategoryID3\r\n" +
            " AND `main`.`Products`.`ProductID` < @ProductID4\r\n" +
            ")\r\n";

            Assert.AreEqual(expected, sql);

        }

        [Test]
        public void Select_Expression()
        {
            SubSonic.SqlQuery q = new Select().Expression("dbo.MyFunction(ProductID) as rows")
                .From("Products").WhereExpression("categoryID")
                .IsLessThan(5).And("ProductID")
                .IsGreaterThan(3)
                .CloseExpression()
                .OrExpression("categoryID")
                .IsGreaterThan(8)
                .And("productID")
                .IsLessThan(2)
                .CloseExpression();
            string sql = q.ToString();
            
            Assert.AreEqual(
                "SELECT `main`.`Products`.`ProductID`, `main`.`Products`.`ProductName`, `main`.`Products`.`SupplierID`, `main`.`Products`.`CategoryID`, `main`.`Products`.`QuantityPerUnit`, `main`.`Products`.`UnitPrice`, `main`.`Products`.`UnitsInStock`, `main`.`Products`.`UnitsOnOrder`, `main`.`Products`.`ReorderLevel`, `main`.`Products`.`Discontinued`, `main`.`Products`.`AttributeXML`, `main`.`Products`.`DateCreated`, `main`.`Products`.`ProductGUID`, `main`.`Products`.`CreatedOn`, `main`.`Products`.`CreatedBy`, `main`.`Products`.`ModifiedOn`, `main`.`Products`.`ModifiedBy`, `main`.`Products`.`Deleted`\r\n,dbo.MyFunction(ProductID) as rows\r\n FROM `main`.`Products`\r\n WHERE (`main`.`Products`.`CategoryID` < @CategoryID0\r\n AND `main`.`Products`.`ProductID` > @ProductID1\r\n)\r\n OR (`main`.`Products`.`CategoryID` > @CategoryID3\r\n AND `main`.`Products`.`ProductID` < @ProductID4\r\n)\r\n",
                sql);
        }

        [Test]
        public void Select_Paged()
        {
            SubSonic.SqlQuery q = Select.AllColumnsFrom<Product>()
                .Paged(1, 20)
                .Where("productid").IsLessThan(10);
            string sql = q.ToString();
        }

        [Test]
        public void Select_IN()
        {
            SubSonic.SqlQuery q = Select.AllColumnsFrom<Product>()
                .Where("productid").In(1, 2, 3, 4, 5);
            string sql = q.BuildSqlStatement();
        }

        //[Test]
        //public void Join_RightOuter()
        //{
        //    SubSonic.SqlQuery q = new Select("productID", "categoryName")
        //        .From(Product.Schema)
        //        .RightOuterJoin(Category.Schema);
        //    string sql = q.BuildSqlStatement();
        //    Assert.AreEqual(
        //        "SELECT `main`.`Products`.`ProductID`, categoryName\r\n FROM `main`.`Products`\r\n RIGHT OUTER JOIN `main`.`Categories` ON `main`.`Products`.`CategoryID` = `main`.`Categories`.`CategoryID`\r\n",
        //        sql);
        //}

        [Test]
        public void Join_Cross()
        {
            SubSonic.SqlQuery q = new Select("productID", "categoryName")
                .From(Product.Schema)
                .CrossJoin(Category.Schema);
            string sql = q.BuildSqlStatement();
            
            //Assert.Fail("sql = " + sql);

            //SELECT `main`.`Products`.`ProductID`, `main`.`Categories`.`CategoryName`
	        // FROM `main`.`Products`
	        // CROSS JOIN `main`.`Categories`

            Assert.AreEqual(
                "SELECT `main`.`Products`.`ProductID`, `main`.`Categories`.`CategoryName`\r\n FROM `main`.`Products`\r\n CROSS JOIN `main`.`Categories`\r\n",
                sql);
        }

        [Test]
        public void Join_Unequal()
        {
            SubSonic.SqlQuery q = new Select("productID", "categoryName")
                .From(Product.Schema)
                .NotEqualJoin(Category.Schema);
            string sql = q.BuildSqlStatement();
            Assert.AreEqual(
                "SELECT `main`.`Products`.`ProductID`, `main`.`Categories`.`CategoryName`\r\n FROM `main`.`Products`\r\n JOIN `main`.`Categories` ON `main`.`Products`.`CategoryID` <> `main`.`Categories`.`CategoryID`\r\n",
                sql);
        }

        [Test]
        public void Drop_Table()
        {
            ANSISqlGenerator gen = new ANSISqlGenerator(null);
            string sql = gen.BuildDropTableStatement(Product.Schema);
            Assert.AreEqual("DROP TABLE `main`.`Products`", sql);
        }

        [Test]
        public void Create_Table()
        {
            ANSISqlGenerator gen = new ANSISqlGenerator(null);
            string sql = gen.BuildCreateTableStatement(Product.Schema);

            //Assert.Fail("sql = " + sql);
string expected = 
@"CREATE TABLE `main`.`Products` (
  `ProductID` int NOT NULL PRIMARY KEY IDENTITY(1,1),
  `ProductName` nvarchar(150) NOT NULL,
  `SupplierID` int NULL,
  `CategoryID` int NULL,
  `QuantityPerUnit` nvarchar(MAX) NULL,
  `UnitPrice` real NULL,
  `UnitsInStock` int NULL,
  `UnitsOnOrder` int NULL,
  `ReorderLevel` int NULL,
  `Discontinued` bit NOT NULL,
  `AttributeXML` nvarchar(MAX) NULL,
  `DateCreated` datetime NULL,
  `ProductGUID` uniqueidentifier NULL,
  `CreatedOn` datetime NOT NULL,
  `CreatedBy` nvarchar(MAX) NULL,
  `ModifiedOn` datetime NOT NULL,
  `ModifiedBy` nvarchar(MAX) NULL,
  `Deleted` bit NOT NULL 
)";
            
            Assert.AreEqual(expected, sql);


        }

        [Test]
        public void Add_Column()
        {
            TableSchema.Table productSchema = Product.Schema;
            TableSchema.TableColumn column = new TableSchema.TableColumn(productSchema);
            column.ColumnName = "Address4";
            column.DataType = DbType.String;
            column.MaxLength = 50;
            column.IsNullable = true;

            ANSISqlGenerator gen = new ANSISqlGenerator(null);
            string sql = gen.BuildAddColumnStatement(Product.Schema, column);
            Assert.AreEqual("ALTER TABLE `main`.`Products` ADD `Address4` nvarchar(50) NULL", sql);
        }

        [Test]
        public void Alter_Column()
        {
            TableSchema.Table productSchema = Product.Schema;
            TableSchema.TableColumn column = productSchema.GetColumn("ProductName");
            column.MaxLength = 150;

            ANSISqlGenerator gen = new ANSISqlGenerator(null);
            string sql = gen.BuildAlterColumnStatement(column);
            Assert.AreEqual("ALTER TABLE `main`.`Products` ALTER COLUMN `ProductName` nvarchar(150) NOT NULL", sql);
        }

        [Test]
        public void Remove_Column()
        {
            TableSchema.Table productSchema = Product.Schema;
            TableSchema.TableColumn column = productSchema.GetColumn("ProductName");

            ANSISqlGenerator gen = new ANSISqlGenerator(null);
            string sql = gen.BuildDropColumnStatement(productSchema, column);
            Assert.AreEqual("ALTER TABLE `main`.`Products` DROP COLUMN `ProductName`", sql);
        }
    }
}