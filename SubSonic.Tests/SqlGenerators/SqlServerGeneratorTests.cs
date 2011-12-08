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
using System.Data;
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests.SqlQuery
{
    [TestFixture]
    public class SqlServerParserTests
    {
        [Test]
        public void Select_ColumnList_Specified()
        {
            SubSonic.SqlQuery qry = new Select("productid", "productname").From(Product.Schema);
            ANSISqlGenerator gen = new ANSISqlGenerator(qry);

            string selectList = gen.GenerateCommandLine();

            Assert.AreEqual("SELECT [dbo].[Products].[ProductID], [dbo].[Products].[ProductName]\r\n", selectList);
        }

        [Test]
        public void Select_Generate_FromList()
        {
            Select qry = Select.AllColumnsFrom<Product>();
            ANSISqlGenerator gen = new ANSISqlGenerator(qry);

            string from = gen.GenerateFromList();

            Assert.AreEqual(" FROM [dbo].[Products]\r\n", from);
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
                " INNER JOIN [dbo].[Categories] ON [dbo].[Products].[CategoryID] = [dbo].[Categories].[CategoryID]\r\n INNER JOIN [dbo].[Suppliers] ON [dbo].[Products].[SupplierID] = [dbo].[Suppliers].[SupplierID]\r\n",
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
            Assert.AreEqual(" WHERE [dbo].[Products].[ProductID] > @ProductID0\r\n", w);
        }

        [Test]
        public void Where_And()
        {
            SubSonic.SqlQuery qry = Select.AllColumnsFrom<Product>().Where("productID").IsGreaterThan(5).And("categoryID").IsEqualTo(5);
            ANSISqlGenerator gen = new ANSISqlGenerator(qry);
            string w = gen.GenerateWhere();
            Assert.AreEqual(
                " WHERE [dbo].[Products].[ProductID] > @ProductID0\r\n AND [dbo].[Products].[CategoryID] = @CategoryID1\r\n",
                w);
        }

        [Test]
        public void Select_BetweenAnd()
        {
            SubSonic.SqlQuery q = new Select("productid").From(Product.Schema).Where("productid").IsBetweenAnd(2, 5);
            string sql = q.BuildSqlStatement();
            Assert.AreEqual(
                "SELECT [dbo].[Products].[ProductID]\r\n FROM [dbo].[Products]\r\n WHERE [dbo].[Products].[ProductID] BETWEEN @ProductID0_start AND @ProductID0_end\r\n",
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
                "SELECT COUNT(ProductID) AS 'CountOfProductID', SUM(UnitPrice) AS 'boots', categoryID AS 'GroupByOfcategoryID'\r\n FROM [dbo].[Products]\r\n WHERE [dbo].[Products].[CategoryID] > @CategoryID0\r\n GROUP BY categoryID\r\n ORDER BY categoryID ASC,boots DESC\r\n",
                sql);
        }

        [Test]
        public void Where_Expression()
        {
            SubSonic.SqlQuery q = Select.AllColumnsFrom<Product>()
                .WhereExpression("categoryID").IsLessThan(5).And("ProductID").IsGreaterThan(3).CloseExpression()
                .OrExpression("categoryID").IsGreaterThan(8).And("productID").IsLessThan(2).CloseExpression();
            string sql = q.ToString();
            Assert.AreEqual(
                "SELECT [dbo].[Products].[ProductID], [dbo].[Products].[ProductName], [dbo].[Products].[SupplierID], [dbo].[Products].[CategoryID], [dbo].[Products].[QuantityPerUnit], [dbo].[Products].[UnitPrice], [dbo].[Products].[UnitsInStock], [dbo].[Products].[UnitsOnOrder], [dbo].[Products].[ReorderLevel], [dbo].[Products].[Discontinued], [dbo].[Products].[AttributeXML], [dbo].[Products].[DateCreated], [dbo].[Products].[ProductGUID], [dbo].[Products].[CreatedOn], [dbo].[Products].[CreatedBy], [dbo].[Products].[ModifiedOn], [dbo].[Products].[ModifiedBy], [dbo].[Products].[Deleted]\r\n\r\n FROM [dbo].[Products]\r\n WHERE ([dbo].[Products].[CategoryID] < @CategoryID0\r\n AND [dbo].[Products].[ProductID] > @ProductID1\r\n)\r\n OR ([dbo].[Products].[CategoryID] > @CategoryID3\r\n AND [dbo].[Products].[ProductID] < @ProductID4\r\n)\r\n",
                sql);
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
                "SELECT [dbo].[Products].[ProductID], [dbo].[Products].[ProductName], [dbo].[Products].[SupplierID], [dbo].[Products].[CategoryID], [dbo].[Products].[QuantityPerUnit], [dbo].[Products].[UnitPrice], [dbo].[Products].[UnitsInStock], [dbo].[Products].[UnitsOnOrder], [dbo].[Products].[ReorderLevel], [dbo].[Products].[Discontinued], [dbo].[Products].[AttributeXML], [dbo].[Products].[DateCreated], [dbo].[Products].[ProductGUID], [dbo].[Products].[CreatedOn], [dbo].[Products].[CreatedBy], [dbo].[Products].[ModifiedOn], [dbo].[Products].[ModifiedBy], [dbo].[Products].[Deleted]\r\n,dbo.MyFunction(ProductID) as rows\r\n FROM [dbo].[Products]\r\n WHERE ([dbo].[Products].[CategoryID] < @CategoryID0\r\n AND [dbo].[Products].[ProductID] > @ProductID1\r\n)\r\n OR ([dbo].[Products].[CategoryID] > @CategoryID3\r\n AND [dbo].[Products].[ProductID] < @ProductID4\r\n)\r\n",
                sql);
        }

        [Test]
        public void Select_Paged()
        {
            SubSonic.SqlQuery q = Select.AllColumnsFrom<Product>()
            .InnerJoin(Category.Schema)
            .Where("productid").IsLessThan(10)
            .OrderAsc("productid")
            .Paged(1, 20);

            ProductCollection pc = q.ExecuteAsCollection<ProductCollection>();
            Assert.GreaterThanOrEqualTo(pc.Count, 1);

            SubSonic.SqlQuery q2 = new SubSonic.SqlQuery().From(Product.Schema)
            .InnerJoin(Category.Schema)
            .Where("productid").IsLessThan(10)
            .Paged(1, 20);

            ProductCollection pc2 = q2.ExecuteAsCollection<ProductCollection>();
            Assert.GreaterThanOrEqualTo(pc2.Count, 1);

        }

        [Test]
        public void Select_PagedWithAggregate() {
             SubSonic.SqlQuery query = new SubSonic.Select(
                SubSonic.Aggregate.GroupBy(Product.Columns.CategoryID))
                .Paged(1, 3)
                .From(Product.Schema)
                .Where(Product.Columns.ProductID).IsGreaterThan(0);
            string exMsg = "";
            try {
                ProductCollection plist = query.ExecuteAsCollection<ProductCollection>();
            }
            catch (Exception ex) {
                exMsg = ex.Message;
            }
            Assert.IsTrue(!exMsg.Contains("syntax near the keyword 'WHERE'"), exMsg + "\r\n" + query.BuildSqlStatement());
        }

        [Test]
        public void Select_IN()
        {
            SubSonic.SqlQuery q = Select.AllColumnsFrom<Product>()
                .Where("productid").In(1, 2, 3, 4, 5);
            string sql = q.BuildSqlStatement();
        }

        [Test]
        public void Join_RightOuter()
        {
            SubSonic.SqlQuery q = new Select("productID", "categoryName")
                .From(Product.Schema)
                .RightOuterJoin(Category.Schema);
            string sql = q.BuildSqlStatement();
            Assert.AreEqual(
                "SELECT [dbo].[Products].[ProductID], [dbo].[Categories].[CategoryName]\r\n FROM [dbo].[Products]\r\n RIGHT OUTER JOIN [dbo].[Categories] ON [dbo].[Products].[CategoryID] = [dbo].[Categories].[CategoryID]\r\n",
                sql);
        }

        [Test]
        public void Join_Cross()
        {
            SubSonic.SqlQuery q = new Select("productID", "categoryName")
                .From(Product.Schema)
                .CrossJoin(Category.Schema);
            string sql = q.BuildSqlStatement();
            Assert.AreEqual(
                "SELECT [dbo].[Products].[ProductID], [dbo].[Categories].[CategoryName]\r\n FROM [dbo].[Products]\r\n CROSS JOIN [dbo].[Categories]\r\n",
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
                "SELECT [dbo].[Products].[ProductID], [dbo].[Categories].[CategoryName]\r\n FROM [dbo].[Products]\r\n JOIN [dbo].[Categories] ON [dbo].[Products].[CategoryID] <> [dbo].[Categories].[CategoryID]\r\n",
                sql);
        }

        [Test]
        public void Drop_Table()
        {
            ANSISqlGenerator gen = new ANSISqlGenerator(null);
            string sql = gen.BuildDropTableStatement(Product.Schema);
            Assert.AreEqual("DROP TABLE [dbo].[Products]", sql);
        }

        [Test]
        public void Create_Table()
        {
            ANSISqlGenerator gen = new ANSISqlGenerator(null);
            string sql = gen.BuildCreateTableStatement(Product.Schema);
            Assert.AreEqual(
                "CREATE TABLE [dbo].[Products] (\r\n  [ProductID] int NOT NULL PRIMARY KEY IDENTITY(1,1),\r\n  [ProductName] nvarchar(40) NOT NULL,\r\n  [SupplierID] int NULL,\r\n  [CategoryID] int NULL,\r\n  [QuantityPerUnit] nvarchar(20) NULL,\r\n  [UnitPrice] money NULL CONSTRAINT DF_Products_UnitPrice DEFAULT (((0))),\r\n  [UnitsInStock] int NULL CONSTRAINT DF_Products_UnitsInStock DEFAULT (((0))),\r\n  [UnitsOnOrder] int NULL CONSTRAINT DF_Products_UnitsOnOrder DEFAULT (((0))),\r\n  [ReorderLevel] int NULL CONSTRAINT DF_Products_ReorderLevel DEFAULT (((0))),\r\n  [Discontinued] bit NOT NULL CONSTRAINT DF_Products_Discontinued DEFAULT (((0))),\r\n  [AttributeXML] varchar NULL,\r\n  [DateCreated] datetime NULL CONSTRAINT DF_Products_DateCreated DEFAULT ((getdate())),\r\n  [ProductGUID] uniqueidentifier NULL CONSTRAINT DF_Products_ProductGUID DEFAULT ((newid())),\r\n  [CreatedOn] datetime NOT NULL CONSTRAINT DF_Products_CreatedOn DEFAULT ((getdate())),\r\n  [CreatedBy] nvarchar(50) NULL,\r\n  [ModifiedOn] datetime NOT NULL CONSTRAINT DF_Products_ModifiedOn DEFAULT ((getdate())),\r\n  [ModifiedBy] nvarchar(50) NULL,\r\n  [Deleted] bit NOT NULL CONSTRAINT DF_Products_Deleted DEFAULT (((0))) \r\n)",
                sql);
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
            Assert.AreEqual("ALTER TABLE [dbo].[Products] ADD [Address4] nvarchar(50) NULL", sql);
        }

        [Test]
        public void Alter_Column()
        {
            TableSchema.Table productSchema = Product.Schema;
            TableSchema.TableColumn column = productSchema.GetColumn("ProductName");
            column.MaxLength = 150;

            ANSISqlGenerator gen = new ANSISqlGenerator(null);
            string sql = gen.BuildAlterColumnStatement(column);
            Assert.AreEqual("ALTER TABLE [dbo].[Products] ALTER COLUMN [ProductName] nvarchar(150) NOT NULL", sql);

            // Set it back to 40 or Create_Table fails.
            column.MaxLength = 40;
        }

        [Test]
        public void Remove_Column()
        {
            TableSchema.Table productSchema = Product.Schema;
            TableSchema.TableColumn column = productSchema.GetColumn("ProductName");

            ANSISqlGenerator gen = new ANSISqlGenerator(null);
            string sql = gen.BuildDropColumnStatement(productSchema, column);
            Assert.AreEqual("ALTER TABLE [dbo].[Products] DROP COLUMN [ProductName]", sql);
        }
     }
}