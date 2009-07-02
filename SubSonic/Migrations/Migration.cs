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
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SubSonic
{
    /// <summary>
    /// Encapsulates a migration
    /// </summary>
    public class Migration : IDisposable
    {
        #region MigrationDirection enum

        /// <summary>
        /// 
        /// </summary>
        public enum MigrationDirection
        {
            /// <summary>
            /// 
            /// </summary>
            Up,
            /// <summary>
            /// 
            /// </summary>
            Down
        }

        #endregion


        private readonly bool migrateOnDispose;
        private readonly List<MigrationStep> steps = new List<MigrationStep>();
        public DataProvider Provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="Migration"/> class.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        public Migration(string providerName)
        {
            Provider = DataService.Providers[providerName];
            migrateOnDispose = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Migration"/> class.
        /// </summary>
        protected Migration()
        {
            if(DataService.Providers.Count > 0)
                Provider = DataService.Provider;
            else
                throw new InvalidOperationException("No providers are setup - check your config file and make sure at least is one provider is set for this migration");
        }


        #region Migration Step Methods

        private void AddMigrationStep(MigrationStepType stepType, TableSchema.Table table)
        {
            MigrationStep step = new MigrationStep(stepType);
            step.Table = table;
            steps.Add(step);
        }

        private void AddMigrationStep(MigrationStepType stepType, TableSchema.TableColumn col1, TableSchema.TableColumn col2)
        {
            MigrationStep step = new MigrationStep(stepType);
            step.Column = col1;
            step.Column2 = col2;
            steps.Add(step);
        }

        private void AddMigrationStep(MigrationStepType stepType, TableSchema.Table table, TableSchema.TableColumn column)
        {
            MigrationStep step = new MigrationStep(stepType);
            step.Table = table;
            step.Column = column;
            steps.Add(step);
        }

        private void AddMigrationSql(string sql)
        {
            MigrationStep step = new MigrationStep(MigrationStepType.ExecuteSql);
            step.Sql = sql;
            steps.Add(step);
        }

        /// <summary>
        /// Adds the SubSonic state columns.
        /// </summary>
        /// <param name="table">The table to which the columns will be added.</param>
        public void AddSubSonicStateColumns(TableSchema.Table table)
        {
            table.AddColumn(ReservedColumnName.CREATED_ON, DbType.DateTime, 0, false, "getdate()");
            table.AddColumn(ReservedColumnName.MODIFIED_ON, DbType.DateTime, 0, false, "getdate()");
            table.AddColumn(ReservedColumnName.CREATED_BY, DbType.String);
            table.AddColumn(ReservedColumnName.MODIFIED_BY, DbType.String);
        }

        #endregion


        #region Migration Actions

        /// <summary>
        /// Gets an existing tableschema
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <returns>TableSchema.Table</returns>
        public TableSchema.Table GetTable(string tableName)
        {
            return GetTable(tableName, DataService.Provider.Name);
        }

        /// <summary>
        /// Gets an existing tableschema
        /// </summary>
        /// <param name="tableName">The name of the table</param>
        /// <param name="providerName">The name of the provider</param>
        /// <returns>TableSchema.Table</returns>
        public TableSchema.Table GetTable(string tableName, string providerName)
        {
            TableSchema.Table result = DataService.Providers[providerName].GetTableSchema(tableName, TableType.Table);
            if(result == null)
                throw new InvalidOperationException("Cannot find table " + tableName + " in the DB with provider " + providerName);
            return result;
        }

        /// <summary>
        /// Creates a new table
        /// </summary>
        /// <param name="tableName">Name of table to create</param>
        /// <returns>An empty table schema object.</returns>
        public TableSchema.Table CreateTable(string tableName)
        {
            TableSchema.Table table = new TableSchema.Table(tableName);
            AddMigrationStep(MigrationStepType.CreateTable, table);

            return table;
        }

        /// <summary>
        /// Creates a new table with a default integer primary key called "id"
        /// </summary>
        /// <param name="tableName">Name of table to create</param>
        /// <returns>An empty table schema object.</returns>
        public TableSchema.Table CreateTableWithKey(string tableName)
        {
            return CreateTableWithKey(tableName, "id");
        }

        /// <summary>
        /// Creates a new table with a default integer primary key with the supplied name
        /// </summary>
        /// <param name="tableName">Name of table to create</param>
        /// <param name="keyName">Name of primary key</param>
        /// <returns>An empty table schema object.</returns>
        public TableSchema.Table CreateTableWithKey(string tableName, string keyName)
        {
            TableSchema.Table table = new TableSchema.Table(tableName);
            table.AddPrimaryKeyColumn(keyName);
            AddMigrationStep(MigrationStepType.CreateTable, table);
            return table;
        }

        /// <summary>
        /// Creates a foreign key between the supplied columns
        /// </summary>
        public void CreateForeignKey(TableSchema.TableColumn oneTable, TableSchema.TableColumn manyTable)
        {
            AddMigrationStep(MigrationStepType.AddForeignKey, oneTable, manyTable);
        }

        /// <summary>
        /// Removes a foreign key from the dB
        /// </summary>
        public void DropForeignKey(TableSchema.TableColumn from, TableSchema.TableColumn to)
        {
            AddMigrationStep(MigrationStepType.DropForeignKey, from, to);
        }

        /// <summary>
        /// Drops a table
        /// </summary>
        /// <param name="tableName">Name of table to drop</param>        
        public void DropTable(string tableName)
        {
            TableSchema.Table table = Provider.GetTableSchema(tableName, TableType.Table);
            if(table == null)
                throw new ArgumentException("Unknown table name " + tableName);

            AddMigrationStep(MigrationStepType.DropTable, table);
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column to add.</param>
        /// <param name="dbType">Type of the db.</param>
        public void AddColumn(string tableName, string columnName, DbType dbType)
        {
            int length = 0;
            if(dbType == DbType.String)
                length = 64; // For you Phil :)
            AddColumn(tableName, columnName, dbType, length, true, String.Empty);
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column to add.</param>        
        /// <param name="dbType">Type of the db.</param>
        /// <param name="length">The length.</param>
        public void AddColumn(string tableName, string columnName, DbType dbType, int length)
        {
            AddColumn(tableName, columnName, dbType, length, true, String.Empty);
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column to add.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="length">The length.</param>
        /// <param name="nullable">if set to <c>true</c> [nullable].</param>
        public void AddColumn(string tableName, string columnName, DbType dbType, int length, bool nullable)
        {
            AddColumn(tableName, columnName, dbType, length, nullable, String.Empty);
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column to add.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="length">The length.</param>
        /// <param name="nullable">if set to <c>true</c> [nullable].</param>
        /// <param name="defaultValue">The default value.</param>
        public void AddColumn(string tableName, string columnName, DbType dbType, int length, bool nullable, string defaultValue)
        {
            TableSchema.Table table = Provider.GetTableSchema(tableName, TableType.Table);
            if(table == null)
                throw new ArgumentException("Unknown table name " + tableName);
            table.AddColumn(columnName, dbType, length, nullable, defaultValue);
            TableSchema.TableColumn column = table.GetColumn(columnName);
            AddMigrationStep(MigrationStepType.AddColumn, table, column);
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column to add.</param>
        /// <param name="dbType">Type of the db.</param>
        public void AlterColumn(string tableName, string columnName, DbType dbType)
        {
            //For you Phil :)
            int length = 0;
            if(dbType == DbType.String)
                length = 64;
            AlterColumn(tableName, columnName, dbType, length, true, String.Empty);
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column to add.</param>        
        /// <param name="dbType">Type of the db.</param>
        /// <param name="length">The length.</param>
        public void AlterColumn(string tableName, string columnName, DbType dbType, int length)
        {
            AlterColumn(tableName, columnName, dbType, length, true, String.Empty);
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column to add.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="length">The length.</param>
        /// <param name="nullable">if set to <c>true</c> [nullable].</param>
        public void AlterColumn(string tableName, string columnName, DbType dbType, int length, bool nullable)
        {
            AlterColumn(tableName, columnName, dbType, length, nullable, String.Empty);
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column to add.</param>
        /// <param name="dbType">Type of the db.</param>
        /// <param name="length">The length.</param>
        /// <param name="nullable">if set to <c>true</c> [nullable].</param>
        /// <param name="defaultValue">The default value.</param>
        public void AlterColumn(string tableName, string columnName, DbType dbType, int length, bool nullable, string defaultValue)
        {
            TableSchema.Table table = Provider.GetTableSchema(tableName, TableType.Table);
            if(table == null)
                throw new ArgumentException("Unknown table " + tableName);

            TableSchema.TableColumn column = table.GetColumn(columnName);
            if(column == null)
                throw new ArgumentException("Unknown column " + columnName);

            column.DataType = dbType;
            column.MaxLength = length;
            column.IsNullable = nullable;
            column.DefaultSetting = defaultValue;

            AddMigrationStep(MigrationStepType.AlterColumn, table, column);
        }

        /// <summary>
        /// Removes the column named columnName from the table named tableName.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column to remove.</param>
        public void RemoveColumn(string tableName, string columnName)
        {
            TableSchema.Table table = Provider.GetTableSchema(tableName, TableType.Table);
            if(table == null)
                throw new ArgumentException("Unknown table name " + tableName);
            TableSchema.TableColumn column = table.GetColumn(columnName);
            AddMigrationStep(MigrationStepType.DropColumn, table, column);
        }

        /// <summary>
        /// Executes the passed in sql directly.  Be careful as the sql is executed directly, without any
        /// any checking for sql injection.
        /// 
        /// NOTE: Be very careful of DB-specific sql when using Execute().
        /// </summary>
        /// <param name="sql">The sql to execute.</param>
        public void Execute(string sql)
        {
            AddMigrationSql(sql);
        }

        #endregion


        #region Core Methods (Up/Down)

        /// <summary>
        /// Ups this instance.
        /// </summary>
        public virtual void Up() {}

        /// <summary>
        /// Downs this instance.
        /// </summary>
        public virtual void Down() {}

        #endregion


        #region Sql Builder

        /// <summary>
        /// Builds the SQL statement.
        /// </summary>
        /// <returns></returns>
        public string BuildSqlStatement(MigrationDirection direction)
        {
            if(Provider == null)
                Provider = DataService.Provider;

            ISqlGenerator generator = DataService.GetGenerator(Provider);
            StringBuilder sql = new StringBuilder();

            // based on direction build migration steps only when dealing with a
            // decendent class, there is no overridden Up or Down if doing a 
            // migrate on dispose style so the step.Clear() would destroy all
            // our hard work.
            if(!migrateOnDispose)
            {
                steps.Clear();

                switch(direction)
                {
                    case MigrationDirection.Up:
                        Up();
                        break;
                    case MigrationDirection.Down:
                        Down();
                        break;
                }
            }

            //build sql
            foreach(MigrationStep step in steps)
            {
                switch(step.StepType)
                {
                    case MigrationStepType.CreateTable:
                        // need to make sure this table has a pk defined
                        // if not, add one
                        // we'll do yer job for ya :)
                        if(step.Table.PrimaryKey == null)
                            step.Table.AddPrimaryKeyColumn();
                        sql.Append(generator.BuildCreateTableStatement(step.Table));
                        break;
                    case MigrationStepType.DropTable:
                        sql.Append(generator.BuildDropTableStatement(step.Table));
                        break;
                    case MigrationStepType.AddForeignKey:
                        sql.Append(generator.BuildForeignKeyStatement(step.Column, step.Column2));
                        break;
                    case MigrationStepType.DropForeignKey:
                        sql.Append(generator.BuildForeignKeyDropStatement(step.Column, step.Column2));
                        break;
                    case MigrationStepType.AddColumn:
                        sql.Append(generator.BuildAddColumnStatement(step.Table, step.Column));
                        break;
                    case MigrationStepType.AlterColumn:
                        sql.Append(generator.BuildAlterColumnStatement(step.Column));
                        break;
                    case MigrationStepType.DropColumn:
                        sql.Append(generator.BuildDropColumnStatement(step.Table, step.Column));
                        break;
                    case MigrationStepType.ExecuteSql:
                        sql.Append(step.Sql);
                        break;

                    default:
                        break;
                }

                sql.Append(";\r\n");
            }

            return sql.ToString();
        }

        #endregion


        #region Command Builder

        internal QueryCommand BuildCommands(MigrationDirection direction)
        {
            QueryCommand cmd = new QueryCommand(BuildSqlStatement(direction), Provider.Name);
            return cmd;
        }

        #endregion


        #region IDisposable Members

        /// <summary>
        /// When a non-decendent Migration is created then any migration methods will be excuted.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion


        /// <summary>
        /// Migrates the specified provider name.
        /// </summary>
        /// <param name="thisProviderName">Name of the provider.</param>
        /// <param name="direction">The direction.</param>
        public void Migrate(string thisProviderName, MigrationDirection direction)
        {
            Provider = DataService.GetInstance(thisProviderName);

            // actually do migration, this is a hack since there isn't
            // an ExecuteTransaction that takes a single cmd.  Will
            // probably be refactored to generated a single QueryCommand
            // per MigrationStep but for now this works.            
            QueryCommand cmd = BuildCommands(direction);
            QueryCommandCollection cmds = new QueryCommandCollection();
            cmds.Add(cmd);
            DataService.ExecuteTransaction(cmds);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                if(migrateOnDispose)
                    Migrate(Provider.Name, MigrationDirection.Up);
            }
        }


        #region Nested type: MigrationStep

        /// <summary>
        /// Encapsulates a single migration action within a migration.
        /// </summary>
        private class MigrationStep
        {
            public readonly MigrationStepType StepType;
            public TableSchema.TableColumn Column;
            public TableSchema.TableColumn Column2;
            public string Sql;
            public TableSchema.Table Table;
            //public TableSchema.Table Table2;

            public MigrationStep(MigrationStepType stepType)
            {
                StepType = stepType;
            }
        }

        #endregion


        #region Nested type: MigrationStepType

        /// <summary>
        /// Supported migration actions.
        /// </summary>
        private enum MigrationStepType
        {
            CreateTable,
            DropTable,
            AddForeignKey,
            DropForeignKey,
            AddColumn,
            AlterColumn,
            DropColumn,
            ExecuteSql
        }

        #endregion
    }
}