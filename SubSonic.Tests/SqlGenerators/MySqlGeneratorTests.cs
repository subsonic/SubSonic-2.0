using System.Data;
using MbUnit.Framework;
using Southwind;

namespace SubSonic.Tests.SqlGenerators
{
    [TestFixture]
    public class MySqlGeneratorTests
    {
        [Test]
        public void Select_ColumnList_Specified()
        {
            SubSonic.SqlQuery qry = new Select("productid", "productname").From(Product.Schema);
            ISqlGenerator generator = new MySqlGenerator(qry);

            string selectList = generator.GenerateCommandLine();

            Assert.AreEqual("SELECT `products`.`ProductID`, `products`.`ProductName`\r\n", selectList);
        }

        [Test]
        public void Drop_Table()
        {
            ISqlGenerator gen = new MySqlGenerator(null);
            string sql = gen.BuildDropTableStatement(Product.Schema);
            Assert.AreEqual("DROP TABLE `products`", sql);
        }

        [Test]
        public void Create_Table()
        {
            TableSchema.Table testSchema = new TableSchema.Table("Southwind", "ShippingCarriers");
            testSchema.AddPrimaryKeyColumn("Id");
            testSchema.AddColumn("Name", DbType.String, 50, false);
            testSchema.AddColumn("Description", DbType.String, 100, false);

            ISqlGenerator generator = new MySqlGenerator(null);
            string sql = generator.BuildCreateTableStatement(testSchema);
            Assert.AreEqual(
                "CREATE TABLE `ShippingCarriers` (\r\n  `Id` INTEGER NOT NULL AUTO_INCREMENT,\r\n  `Name` nvarchar(50) NOT NULL,\r\n  `Description` nvarchar(100) NOT NULL,\r\n  PRIMARY KEY (`Id`) \r\n)",
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

            ISqlGenerator generator = new MySqlGenerator(null);
            string sql = generator.BuildAddColumnStatement(Product.Schema, column);
            Assert.AreEqual("ALTER TABLE `products` ADD `Address4` nvarchar(50) NULL", sql);
        }

        [Test]
        public void Alter_Column()
        {
            TableSchema.Table productSchema = Product.Schema;
            TableSchema.TableColumn column = productSchema.GetColumn("ProductName");
            column.MaxLength = 150;

            ISqlGenerator generator = new MySqlGenerator(null);
            string sql = generator.BuildAlterColumnStatement(column);
            Assert.AreEqual("ALTER TABLE `products` ALTER COLUMN `ProductName` nvarchar(150) NOT NULL", sql);
        }

        [Test]
        public void Remove_Column()
        {
            TableSchema.Table productSchema = Product.Schema;
            TableSchema.TableColumn column = productSchema.GetColumn("ProductName");

            ISqlGenerator generator = new MySqlGenerator(null);
            string sql = generator.BuildDropColumnStatement(productSchema, column);
            Assert.AreEqual("ALTER TABLE `products` DROP COLUMN `ProductName`", sql);
        }
    }
}