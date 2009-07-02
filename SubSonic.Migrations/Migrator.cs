using System;
using System.Data;
using System.IO;
using SubSonic.Sugar;

namespace SubSonic.Migrations
{
    /// <summary>
    /// The Migrator class is responsible for running through a collection of migrationFiles 
    /// and applying them towards the specified provider.
    /// 
    /// This class is very similiar to the Rails Migrator class.  They did something very 
    /// interesting with a static method acting as a mini-factory for itself, notice how
    /// the static Migrate() method in turn creates a Migrator instance and calls methods
    /// on that.  Thought that was very neat, so I borrowed it.
    /// </summary>
    public class Migrator
    {
        private const string SCHEMA_INFO = "SubSonicSchemaInfo";
        private readonly int currentVersion;
        private readonly Migration.MigrationDirection direction;
        private readonly string migrationDirectory;
        private readonly string providerName;
        private readonly int? toVersion;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="direction">Direction to migrate.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="migrationDirectory">Directory to find the migrations.</param>
        /// <param name="toVersion">Version to migrate up to.</param>
        public Migrator(Migration.MigrationDirection direction, string providerName, string migrationDirectory, int? toVersion)
        {
            this.direction = direction;
            this.providerName = providerName;
            this.migrationDirectory = migrationDirectory;
            this.toVersion = toVersion;
            currentVersion = GetCurrentVersion(providerName);
        }

        /// <summary>
        /// Begins the migration.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="migrationDirectory">Directory to find the migrations.</param>
        /// <param name="toVersion">Version to migrate to.</param>
        public static void Migrate(string providerName, string migrationDirectory, int? toVersion)
        {
            int currentVersion = GetCurrentVersion(providerName);

            if (!toVersion.HasValue || currentVersion < toVersion.Value)
                Up(providerName, migrationDirectory, toVersion);
            else if (currentVersion > toVersion.Value)
            {
                if (toVersion.Value == -1)
                {
                    if (currentVersion - 1 > -1)
                    {
                        Down(providerName, migrationDirectory, currentVersion - 1);
                    }
                }
                else
                {
                    Down(providerName, migrationDirectory, toVersion);
                }
            }

            return;
        }

        /// <summary>
        /// Migreate Up
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="migrationDirectory">Directory to find the migrations.</param>
        /// <param name="toVersion">Version to migrate up to.</param>
        public static void Up(string providerName, string migrationDirectory, int? toVersion)
        {
            Migrator migrator = new Migrator(Migration.MigrationDirection.Up, providerName, migrationDirectory, toVersion);
            migrator.Migrate();
        }

        /// <summary>
        /// Migreate Down
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="migrationDirectory">Directory to find the migrations.</param>
        /// <param name="toVersion">Version to migrate down to.</param>
        public static void Down(string providerName, string migrationDirectory, int? toVersion)
        {
            Migrator migrator = new Migrator(Migration.MigrationDirection.Down, providerName, migrationDirectory, toVersion);
            migrator.Migrate();
        }

        /// <summary>
        /// Start migration.
        /// </summary>
        public void Migrate()
        {
            //grab all migration files from migrationDirectory, list will be returned sorted according to direction.
            string[] migrations = GetMigrations();

            if(migrations.Length == 0)
                Console.WriteLine("There are no migrations files found. You sure this is the right directory?");
            else
                Console.WriteLine("Found " + migrations.Length + " migration files");

            Console.WriteLine("Current DB Version is {0}", currentVersion);

            //if no version was passed in use the highest version we have
            int targetVersion = toVersion ?? GetMigrationVersion(migrations[migrations.Length - 1]);
            if(targetVersion == currentVersion)
                return;

            //determine the range of migrations to run so we're not looping through every single file
            int startIndex = Array.FindIndex(migrations, delegate(string m) { return GetMigrationVersion(m) == currentVersion; });
            int finishIndex = Array.FindIndex(migrations, delegate(string m) { return GetMigrationVersion(m) == targetVersion; });

            // skip the current migration if going up
            if(startIndex == -1)
                startIndex = 0;
            else if(direction == Migration.MigrationDirection.Up)
                startIndex++;

            // don't execute the very last migration when going down
            if(finishIndex == -1)
                finishIndex = migrations.Length - 1;
            else if(direction == Migration.MigrationDirection.Down)
                finishIndex--;

            //let the migrations begin!
            for(int i = startIndex; i <= finishIndex; i++)
            {
                string migrationFile = migrations[i];
                string migrationName = Path.GetFileNameWithoutExtension(migrationFile);
                Console.WriteLine("Migrating to {0} ({1})", migrationName, GetMigrationVersion(migrationFile));

                try
                {
                    ExecuteMigrationCode(migrationFile);
                }
                catch(Exception x)
                {
                    string rawError = x.Message;
                    if(x.InnerException != null)
                        rawError = x.InnerException.Message;

                    string errorMesage = string.Format("There was an error running migration ({0}): \r\n {1}", migrationName, rawError);
                    errorMesage += "\r\nStack Trace: \r\n" + x.StackTrace;

                    Console.WriteLine(errorMesage);
                    Console.ReadLine();
                    return;
                }

                UpdateSchemaVersion();
                DataService.ClearSchemaCache(providerName);
            }
        }

        private void ExecuteMigrationCode(string migrationFile)
        {
            //pull the whole code bits in - we're going to compile this to code
            //and execute it :):):):)
            string migrationCode = Files.GetFileText(migrationFile);

            //get the extension
            string codeExtension = Path.GetExtension(migrationFile);
            ICodeLanguage codeLang = new CSharpCodeLanguage();
            if(codeExtension.EndsWith(FileExtension.VB, StringComparison.InvariantCultureIgnoreCase))
                codeLang = new VBCodeLanguage();

            object[] parameters = new object[2];
            parameters[0] = providerName;
            parameters[1] = direction;
            CodeRunner.RunAndExecute(codeLang, migrationCode, "Migrate", parameters);
        }

        private string[] GetMigrations()
        {
            if(!Directory.Exists(migrationDirectory))
                throw new InvalidOperationException("Can't find the migration directory at " + migrationDirectory);

            string[] migrations = Directory.GetFiles(migrationDirectory);
            if(migrations.Length == 0)
                throw new Exception("There are no migration files in this directory: " + migrationDirectory);

            Array.Sort(migrations);
            if(direction == Migration.MigrationDirection.Down)
                Array.Reverse(migrations);

            return migrations;
        }

        /// <summary>
        /// Gets the version of the migration file.
        /// </summary>
        /// <param name="migration">Migration file name.</param>
        /// <returns></returns>
        private static int GetMigrationVersion(string migration)
        {
            int fileVersion;
            string fileName = Path.GetFileName(migration);
            string versionStub = fileName.Substring(0, 3);
            int.TryParse(versionStub, out fileVersion);
            return fileVersion;
        }


        #region Versioning Bits

        /// <summary>
        /// Gets the current schema version for the named provider.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns>Current version of the schema as stored in the schema info table.</returns>
        public static int GetCurrentVersion(string providerName)
        {
            int currentVersion = 0;

            DataProvider p = DataService.Providers[providerName];

            TableSchema.Table schemaTable = p.GetTableSchema(SCHEMA_INFO, TableType.Table);

            if(schemaTable != null && schemaTable.GetColumn("version") != null)
                currentVersion = new Select(p, "version").From(SCHEMA_INFO).ExecuteScalar<int>();
            else
            {
                //create schema table if it doesn't exist
                if(schemaTable == null)
                    CreateSchemaInfo(providerName);

                //delete all rows & add the version row
                new Delete(providerName).From(SCHEMA_INFO).Execute();
                new Insert(SCHEMA_INFO, providerName).Values(0).Execute();
            }

            return currentVersion;
        }

        private static void CreateSchemaInfo(string providerName)
        {
            TableSchema.Table tbl = new TableSchema.Table(providerName, SCHEMA_INFO);
            tbl.AddColumn("version", DbType.Int32, 0, false, "0");

            ISqlGenerator generator = DataService.GetGenerator(providerName);
            string sql = generator.BuildCreateTableStatement(tbl);
            DataService.ExecuteQuery(new QueryCommand(sql, providerName));
            DataService.Providers[providerName].ReloadSchema();
        }

        private void UpdateSchemaVersion()
        {
            if(direction == Migration.MigrationDirection.Up)
                IncrementVersion();
            else
                DecrementVersion();
        }

        /// <summary>
        /// Increments the version.
        /// </summary>
        private void IncrementVersion()
        {
            new Update(SCHEMA_INFO, providerName).SetExpression("version").EqualTo("version+1").Execute();
        }

        /// <summary>
        /// Decrements the version.
        /// </summary>
        private void DecrementVersion()
        {
            new Update(SCHEMA_INFO, providerName).SetExpression("version").EqualTo("version-1").Execute();
        }

        #endregion
    }
}