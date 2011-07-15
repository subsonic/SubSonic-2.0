using System;
using System.Data;
using System.IO;
using System.Reflection;
using MbUnit.Framework;
using SubSonic.Migrations;
using SubSonic.Sugar;

namespace SubSonic.Tests.Migrations
{
    [TestFixture]
    public class MigrationTests
    {
        #region Sql Generation

        [Test]
        public void MigrationGenerateCreateTablesSql()
        {
            Migration m = new MigrationTest001();
            string sql = m.BuildSqlStatement(Migration.MigrationDirection.Up);

            string expected =
                "CREATE TABLE `Distribution` (\r\n" +
                "  `Id` INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,\r\n" +
                "  `Name` nvarchar(20) NULL,\r\n" +
                "  `Capacity` INTEGER NULL \r\n" + 
                ");\r\n" +
                "CREATE TABLE `ShipStatus` (\r\n" +
                "  `ShipStatusId` INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,\r\n" +
                "  `Status` nvarchar(50) NULL,\r\n" +
                "  `Code` nvarchar(4) NULL,\r\n" +
                "  `CreatedOn` datetime ,\r\n" +
                "  `ModifiedOn` datetime ,\r\n" +
                "  `CreatedBy` nvarchar(64) NULL,\r\n" +
                "  `ModifiedBy` nvarchar(64) NULL \r\n" + 
                ");\r\n";
            
            Assert.AreEqual(expected, sql);

        }

        [Test]
        [RollBack]
        public void MigrationGenerateDropTablesSql()
        {   // Attempted to read or write protected memory. This is often an indication that other memory is corrupt.
            //get a table in there to drop         // SQLite error near "AUTOINCREMENT": syntax error
            Migration m = new MigrationTest001();
            m.Migrate("Northwind", Migration.MigrationDirection.Up);
            // The database file is locked
            DataService.ClearSchemaCache("Northwind");
            
            string sql = m.BuildSqlStatement(Migration.MigrationDirection.Down);
            Assert.AreEqual("DROP TABLE `main`.`Distribution`;\r\nDROP TABLE `main`.`ShipStatus`;\r\n", sql);
        }

        [Test]
        public void Migration_SQL_Generation_AddRemoveColumns()
        {
            Migration m = new MigrationTest003();  // Object reference not set to an instance of an object.
            string sql = m.BuildSqlStatement(Migration.MigrationDirection.Up);

            string expected = 
                "ALTER TABLE `main`.`Products` ADD `ProductExpiration` datetime NULL;\r\n" +
                "ALTER TABLE `main`.`Products` DROP COLUMN `ProductName`;\r\n";
            
            Assert.AreEqual(expected, sql);
        }

        #endregion


        private string migrationDir;

        private string MigrationDirectory
        {
            get
            {
                if(String.IsNullOrEmpty(migrationDir))
                {
                    Uri codeBase = new Uri(Assembly.GetExecutingAssembly().CodeBase);
                    string rootPath = Path.GetDirectoryName(codeBase.AbsolutePath);
                    rootPath = Strings.Chop(rootPath, @"\bin\Debug");
                    migrationDir = rootPath + @"\Migrations\MigrationFiles";
                }
                return migrationDir;
            }
        }

        [Test]
        [RollBack]
        public void MigrationUpDown()
        {
            Migration m = new MigrationTest001();
            // The database file is locked

            //test the up
            m.Migrate("Northwind", Migration.MigrationDirection.Up);
            Assert.IsTrue(DataService.TableExists("Northwind", "Distribution"));
            Assert.IsTrue(DataService.TableExists("Northwind", "ShipStatus"));

            //needed to clear schema cache for the DropTable call
            //in the Down() method of the migration.  Not needed
            //for the above TableExists because that always
            //goes to the DB to check
            DataService.ClearSchemaCache("Northwind");

            //test the down
            m.Migrate("Northwind", Migration.MigrationDirection.Down);
            Assert.IsFalse(DataService.TableExists("Northwind", "Distribution"));
            Assert.IsFalse(DataService.TableExists("Northwind", "ShipStatus"));
        }

        [Test]
        public void MigrationUpwithErrors()
        {
            //test that the two correct tables don't get added to the database
            Migration m = new MigrationTest002();
            try
            {
                m.Migrate("Northwind", Migration.MigrationDirection.Up);
            }
            catch
            {
                //gobble errror
            }

            Assert.IsFalse(DataService.TableExists("Northwind", "Distribution"));
            Assert.IsFalse(DataService.TableExists("Northwind", "ShipStatus"));
        }

        [Test]
        [RollBack]
        public void MigrationAlterColumn()
        {
            Migration m = new AlterProductNameMigration();
            // The database file is locked
            //Up
            m.Migrate("Northwind", Migration.MigrationDirection.Up);
            DataService.ClearSchemaCache("Northwind");
            TableSchema.Table table = DataService.GetSchema("Products", "Northwind");
            TableSchema.TableColumn column = table.GetColumn("ProductName");
            Assert.AreEqual(100, column.MaxLength);

            //Down
            m.Migrate("Northwind", Migration.MigrationDirection.Down);
            DataService.ClearSchemaCache("Northwind");
            table = DataService.GetSchema("Products", "Northwind");
            column = table.GetColumn("ProductName");
            Assert.AreEqual(50, column.MaxLength);
        }

        [Test]
        [RollBack]
        public void MigrationAddRemoveColumns()
        {
            Migration m = new MigrationTest003();
            m.Migrate("Northwind", Migration.MigrationDirection.Up);
            // The database file is locked
            DataService.ClearSchemaCache("Northwind");
            TableSchema.Table table = DataService.GetSchema("Products", "Northwind");
            Assert.IsNotNull(table.GetColumn("ProductExpiration"));
            Assert.IsNull(table.GetColumn("ProductName"));
        }

        [Test]
        [RollBack]
        public void MigrationShouldCreateAndDropTestTable()
        {
            //up
            Migrator.Migrate("Northwind", MigrationDirectory, 1);
            DataService.ClearSchemaCache("Northwind");
            TableSchema.Table table = DataService.GetSchema("Test1", "Northwind");
            Assert.IsNotNull(table.GetColumn("Name"));
            Assert.IsNotNull(table.GetColumn("Description"));
            Assert.IsNotNull(table.GetColumn("DateEntered"));
            int schemaVersion = Migrator.GetCurrentVersion("Northwind");
            Assert.AreEqual(1, schemaVersion);

            //down
            Migrator.Migrate("Northwind", MigrationDirectory, 0);
            Assert.IsNull(DataService.GetSchema("Test1", "Northwind"));
            schemaVersion = Migrator.GetCurrentVersion("Northwind");
            Assert.AreEqual(0, schemaVersion);
        }

        [Test]
        [RollBack]
        public void MigrationShouldExecMultipleMigrations()
        {
            // The database file is locked
            Migrator.Migrate("Northwind", MigrationDirectory, null);
            DataService.ClearSchemaCache("Northwind");
            TableSchema.Table table = DataService.GetSchema("Test1", "Northwind");
            Assert.IsNotNull(table.GetColumn("Name"));
            Assert.IsNotNull(table.GetColumn("Description"));
            Assert.IsNotNull(table.GetColumn("DateEntered"));
            Assert.IsNotNull(table.GetColumn("MyNewColumn"));
            Assert.IsNotNull(table.GetColumn("MaxInventory"));

            // Down()
            Migrator.Migrate("Northwind", MigrationDirectory, 1);

            DataService.ClearSchemaCache("Northwind");
            table = DataService.GetSchema("Test1", "Northwind");
            Assert.IsNotNull(table.GetColumn("Name"));
            Assert.IsNotNull(table.GetColumn("Description"));
            Assert.IsNotNull(table.GetColumn("DateEntered"));
            Assert.IsNull(table.GetColumn("MyNewColumn"));
            Assert.IsNull(table.GetColumn("MaxInventory"));
        }

        [Test]
        [RollBack]
        public void MigrationOnDispose()
        {
            //testing Rob's super-cool migration on dispose pattern
            using(Migration m = new Migration("Northwind"))
            {
                TableSchema.Table t = m.CreateTable("DisposeTable");
                t.AddPrimaryKeyColumn();
                t.AddColumn("Name", DbType.String);
                m.AddSubSonicStateColumns(t);
            }

            // The database file is locked
            DataService.ClearSchemaCache("Northwind");
            TableSchema.Table table = DataService.GetSchema("DisposeTable", "Northwind");
            Assert.IsNotNull(table);
        }

        [Test]
        [RollBack]
        public void MigrationExecute()
        {
            int expectedRegions = new Select("RegionDescription").From("Region").GetRecordCount();
            // The database file is locked
            using(Migration m = new Migration("Northwind"))
            {
                m.Execute("INSERT INTO Region (RegionDescription) VALUES ('Ireland')");
                m.Execute("INSERT INTO Region (RegionDescription) VALUES ('Scotland')");
            }

            int actualRegions = new Select("RegionDescription").From("Region").GetRecordCount();
            Assert.AreEqual(expectedRegions + 2, actualRegions);
        }

        [Ignore]
        [Test]
        public void MigrationMySql()
        {
            Migration m = new MigrationTest001();

            //test the up
            m.Migrate("Southwind", Migration.MigrationDirection.Up);
            Assert.IsTrue(DataService.TableExists("Southwind", "Distribution"));
            Assert.IsTrue(DataService.TableExists("Southwind", "ShipStatus"));

            //needed to clear schema cache for the DropTable call
            //in the Down() method of the migration.  Not needed
            //for the above TableExists because that always
            //goes to the DB to check
            DataService.ClearSchemaCache("Southwind");

            //test the down
            m.Migrate("Southwind", Migration.MigrationDirection.Down);
            Assert.IsFalse(DataService.TableExists("Southwind", "Distribution"));
            Assert.IsFalse(DataService.TableExists("Southwind", "ShipStatus"));
        }


        #region Mock Migrations


        #region Nested type: AlterProductNameMigration

        private class AlterProductNameMigration : Migration
        {
            public override void Up()
            {
                AlterColumn("Products", "ProductName", DbType.String, 100);
            }

            public override void Down()
            {
                AlterColumn("Products", "ProductName", DbType.String, 50);
            }
        }

        #endregion


        #region Nested type: MigrationTest001

        private class MigrationTest001 : Migration
        {
            public override void Up()
            {
                TableSchema.Table table = CreateTable("Distribution");
                table.AddPrimaryKeyColumn("Id");
                table.AddColumn("Name", DbType.String, 20);
                table.AddColumn("Capacity", DbType.Int32);

                table = CreateTable("ShipStatus");
                table.AddColumn("Status", DbType.String, 50);
                table.AddColumn("Code", DbType.String, 4);
                AddSubSonicStateColumns(table);
            }

            public override void Down()
            {
                DropTable("Distribution");
                DropTable("ShipStatus");
            }
        }

        #endregion


        #region Nested type: MigrationTest002

        private class MigrationTest002 : Migration
        {
            public override void Up()
            {
                TableSchema.Table table = CreateTable("Distribution");
                table.AddColumn("Name", DbType.String, 20);
                table.AddColumn("Capacity", DbType.Int32);

                table = CreateTable("ShipStatus");
                table.AddColumn("Status", DbType.String, 50);
                table.AddColumn("Code", DbType.String, 4);
                AddSubSonicStateColumns(table);

                //this should cause the migration to fail, the previous two tables
                //shouldn't end up in the db
                table = CreateTable("Products");
                table.AddColumn("ID", DbType.Int32);
                table.AddColumn("Name", DbType.String, 40);
            }

            public override void Down()
            {
                DropTable("Distribution");
                DropTable("ShipStatus");
            }
        }

        #endregion


        #region Nested type: MigrationTest003

        private class MigrationTest003 : Migration
        {
            public override void Up()
            {
                AddColumn("Products", "ProductExpiration", DbType.DateTime);
                RemoveColumn("Products", "ProductName");
            }

            public override void Down()
            {
                RemoveColumn("Products", "ProductExpiration");
                AddColumn("Products", "ProductName", DbType.String, 50);
            }
        }

        #endregion


        //private class MigrationTest004 : Migration
        //{
        //    public override void Up()
        //    {

        //        TableSchema.Table records = CreateTableWithKey("Records");
        //        records.AddColumn("RecordName");
        //        records.AddColumn("GroupID", System.Data.DbType.Int32);
        //        records.AddColumn("LabelID", System.Data.DbType.Int32);

        //        AddSubSonicStateColumns(records);

        //        TableSchema.Table groups = CreateTableWithKey("Groups");
        //        groups.AddColumn("GroupName");
        //        AddSubSonicStateColumns(groups);

        //        CreateForeignKey(records.GetColumn("groupID"), groups.GetColumn("id"));

        //    }

        //    public override void Down()
        //    {
        //        CreateForeignKey(records.GetColumn("groupID"), groups.GetColumn("id"));
        //        DropTable("Records");
        //        DropTable("Groups");
        //    }
        //}

        #endregion


        //[Test]
        //public void Migration_Should_Add_Foreign_Keys() {
        //    Migration m = new MigrationTest004();

        //    //test the up
        //    m.Migrate("Northwind", Migration.MigrationDirection.Up);
        //    Assert.IsTrue(DataService.TableExists("Northwind", "Records"));
        //    Assert.IsTrue(DataService.TableExists("Northwind", "Groups"));

        //    //needed to clear schema cache for the DropTable call
        //    //in the Down() method of the migration.  Not needed
        //    //for the above TableExists because that always
        //    //goes to the DB to check
        //    DataService.ClearSchemaCache("Northwind");

        //    //test the down
        //    m.Migrate("Northwind", Migration.MigrationDirection.Down);
        //    Assert.IsFalse(DataService.TableExists("Northwind", "Records"));
        //    Assert.IsFalse(DataService.TableExists("Northwind", "Groups"));
        //}
    }
}