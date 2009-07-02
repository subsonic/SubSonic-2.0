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

using System.Collections.Generic;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISqlGenerator
    {
        /// <summary>
        /// Generates the command line.
        /// </summary>
        /// <returns></returns>
        string GenerateCommandLine();

        /// <summary>
        /// Generates the constraints.
        /// </summary>
        /// <returns></returns>
        string GenerateWhere();

        /// <summary>
        /// Generates from list.
        /// </summary>
        /// <returns></returns>
        string GenerateFromList();

        /// <summary>
        /// Generates the order by.
        /// </summary>
        /// <returns></returns>
        string GenerateOrderBy();

        /// <summary>
        /// Generates the group by.
        /// </summary>
        /// <returns></returns>
        string GenerateGroupBy();

        /// <summary>
        /// Generates the joins.
        /// </summary>
        /// <returns></returns>
        string GenerateJoins();

        /// <summary>
        /// Gets the paging SQL wrapper.
        /// </summary>
        /// <returns></returns>
        string GetPagingSqlWrapper();

        /// <summary>
        /// Gets the select columns.
        /// </summary>
        /// <returns></returns>
        List<string> GetSelectColumns();

        /// <summary>
        /// Finds the column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        TableSchema.TableColumn FindColumn(string columnName);

        /// <summary>
        /// Builds the select statement.
        /// </summary>
        /// <returns></returns>
        string BuildSelectStatement();

        /// <summary>
        /// Builds a foreignkey statement
        /// </summary>
        /// <returns></returns>
        string BuildForeignKeyStatement(TableSchema.TableColumn from, TableSchema.TableColumn to);

        /// <summary>
        /// Builds a foreignkey statement
        /// </summary>
        /// <returns></returns>
        string BuildForeignKeyDropStatement(TableSchema.TableColumn from, TableSchema.TableColumn to);

        /// <summary>
        /// Builds the paged select statement.
        /// </summary>
        /// <returns></returns>
        string BuildPagedSelectStatement();

        /// <summary>
        /// Builds the update statement.
        /// </summary>
        /// <returns></returns>
        string BuildUpdateStatement();

        /// <summary>
        /// Builds the insert statement.
        /// </summary>
        /// <returns></returns>
        string BuildInsertStatement();

        /// <summary>
        /// Builds the delete statement.
        /// </summary>
        /// <returns></returns>
        string BuildDeleteStatement();

        /// <summary>
        /// Builds the create table statement.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        string BuildCreateTableStatement(TableSchema.Table table);

        /// <summary>
        /// Builds the drop table statement.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        string BuildDropTableStatement(TableSchema.Table table);

        /// <summary>
        /// Builds the add column statement.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        string BuildAddColumnStatement(TableSchema.Table table, TableSchema.TableColumn column);

        /// <summary>
        /// Builds the alter column statement.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        string BuildAlterColumnStatement(TableSchema.TableColumn column);

        /// <summary>
        /// Builds the drop column statement.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        string BuildDropColumnStatement(TableSchema.Table table, TableSchema.TableColumn column);

        /// <summary>
        /// Sets the insert query.
        /// </summary>
        /// <param name="q">The q.</param>
        void SetInsertQuery(Insert q);

        /// <summary>
        /// Gets the count select.
        /// </summary>
        /// <returns></returns>
        string GetCountSelect();
    }
}
