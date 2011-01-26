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
            Assert.AreEqual(
                "CREATE TABLE [Distribution] (\r\n  [Id] int NOT NULL PRIMARY KEY IDENTITY(1,1),\r\n  [Name] nvarchar(20) NULL,\r\n  [Capacity] int NULL \r\n);\r\nCREATE TABLE [ShipStatus] (\r\n  [ShipStatusId] int NOT NULL PRIMARY KEY IDENTITY(1,1),\r\n  [Status] nvarchar(50) NULL,\r\n  [Code] nvarchar(4) NULL,\r\n  [CreatedOn] datetime NOT NULL CONSTRAINT DF_ShipStatus_CreatedOn DEFAULT (getdate()),\r\n  [ModifiedOn] datetime NOT NULL CONSTRAINT DF_ShipStatus_ModifiedOn DEFAULT (getdate()),\r\n  [CreatedBy] nvarchar(64) NULL,\r\n  [ModifiedBy] nvarchar(64) NULL \r\n);\r\n",
                sql);
        }

        [Test]
        [Rollback]
        public void MigrationGenerateDropTablesSql()
        {
            //get a table in there to drop
            Migration m = new MigrationTest001();
            m.Migrate("Northwind", Migration.MigrationDirection.Up);
            DataService.ClearSchemaCache("Northwind");

            string sql = m.BuildSqlStatement(Migration.MigrationDirection.Down);
            Assert.AreEqual("DROP TABLE [dbo].[Distribution];\r\nDROP TABLE [dbo].[ShipStatus];\r\n", sql);
        }

        [Test]
        public void Migration_SQL_Generation_AddRemoveColumns()
        {
            Migration m = new MigrationTest003();
            string sql = m.BuildSqlStatement(Migration.MigrationDirection.Up);
            Assert.AreEqual("ALTER TABLE [dbo].[Products] ADD [ProductExpiration] datetime NULL;\r\nALTER TABLE [dbo].[Products] DROP COLUMN [ProductName];\r\n", sql);
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

        [Test, Rollback]
        public void MigrateDownFromZero()
        {
            Assert.AreEqual(Migrator.GetCurrentVersion("Northwind"), 0);
            Migrator.Migrate("Northwind", "Migrations", -1);
            Assert.AreEqual(Migrator.GetCurrentVersion("Northwind"), 0);
        }

        [Test,Rollback]
        public void MigrateUpDownNegative1()
        {
            string p = "Northwind";

            Assert.AreEqual(Migrator.GetCurrentVersion(p), 0);

            Migrator.Migrate(p, MigrationDirectory, 1);

            Assert.AreEqual(Migrator.GetCurrentVersion(p), 1);

            Migrator.Migrate(p, MigrationDirectory, -1);

            Assert.AreEqual(Migrator.GetCurrentVersion(p), 0);

            Migrator.Migrate(p, MigrationDirectory, -1);
            
            Assert.AreEqual(Migrator.GetCurrentVersion(p), 0);
        }

        [Test]
        [Rollback]
        public void MigrationUpDown()
        {
            Migration m = new MigrationTest001();

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

        [Test, Rollback]
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
        [Rollback]
        public void MigrationAlterColumn()
        {
            Migration m = new AlterProductNameMigration();

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
        [Rollback]
        public void MigrationAddRemoveColumns()
        {
            Migration m = new MigrationTest003();
            m.Migrate("Northwind", Migration.MigrationDirection.Up);

            DataService.ClearSchemaCache("Northwind");
            TableSchema.Table table = DataService.GetSchema("Products", "Northwind");
            Assert.IsNotNull(table.GetColumn("ProductExpiration"));
            Assert.IsNull(table.GetColumn("ProductName"));
        }

        [Test]
        [Rollback]
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
        [Rollback]
        public void MigrationShouldExecMultipleMigrations()
        {
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
        [Rollback]
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

            DataService.ClearSchemaCache("Northwind");
            TableSchema.Table table = DataService.GetSchema("DisposeTable", "Northwind");
            Assert.IsNotNull(table);
        }

        [Test]
        [Rollback]
        public void MigrationExecute()
        {
            int expectedRegions = new Select("RegionDescription").From("Region").GetRecordCount();

            using(Migration m = new Migration("Northwind"))
            {
                m.Execute("INSERT INTO Region (RegionDescription) VALUES ('Ireland')");
                m.Execute("INSERT INTO Region (RegionDescription) VALUES ('Scotland')");
            }

            int actualRegions = new Select("RegionDescription").From("Region").GetRecordCount();
            Assert.AreEqual(expectedRegions + 2, actualRegions);
        }

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

        [Test]
        public void CreateTable_Should_Allow_Char3_As_PrimaryKey() {

            new InlineQuery("Northwind").Execute("IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MyTb]') AND type in (N'U')) \r\n DROP TABLE [dbo].[MyTb]");


            using (Migration m = new Migration("Northwind")) {
                TableSchema.Table tb = m.CreateTable("MyTb");
                TableSchema.TableColumn col = new TableSchema.TableColumn(tb);
                col.ColumnName = "Id";
                col.DataType = System.Data.DbType.AnsiStringFixedLength;
                col.MaxLength = 3;
                col.IsPrimaryKey = true;
                tb.AddColumn(col);
            }


            //pull the table out
            DataService.ClearSchemaCache("Northwind");
            TableSchema.Table table = DataService.GetSchema("MyTb", "Northwind");

            Assert.IsNotNull(table);
            Assert.AreEqual(3, table.PrimaryKey.MaxLength);
        }
        [Test]
        public void Dual_ForeignKey_Relationships_ShouldBe_Possible_From_One_To_Many() {


            //load em
            using (Migration m = new Migration("Northwind")) {

                TableSchema.Table one_table = m.CreateTableWithKey("One");

                TableSchema.Table many_table = m.CreateTableWithKey("ManyTable");
                many_table.AddColumn("first_reference_to_table_one", System.Data.DbType.Int32);
                many_table.AddColumn("second_reference_to_table_one", System.Data.DbType.Int32);

                m.CreateForeignKey(one_table.GetColumn("Id"), many_table.GetColumn("first_reference_to_table_one"));
                m.CreateForeignKey(one_table.GetColumn("Id"), many_table.GetColumn("second_reference_to_table_one"));
            }

            DataService.ClearSchemaCache("Northwind");
            
            //drop em
            using (Migration m = new Migration("Northwind")) {

                m.DropTable("ManyTable");
                m.DropTable("One");
            }


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

        #region String as PK
        private class MigrationTest004 : Migration {
            public override void Up() {
                TableSchema.Table tb = this.CreateTable("MyTb");
                TableSchema.TableColumn col = new TableSchema.TableColumn(tb);
                col.ColumnName = "Id";
                col.DataType = System.Data.DbType.AnsiStringFixedLength;
                col.MaxLength = 3;
                col.IsPrimaryKey = true;
                tb.AddColumn(col);
            }

            public override void Down() {
                DropTable("MyTb");
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