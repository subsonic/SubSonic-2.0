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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SubSonic.Sugar;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class ANSISqlGenerator : ISqlGenerator
    {
        internal Insert insert;
        protected SqlQuery query;

        /// <summary>
        /// Initializes a new instance of the <see cref="ANSISqlGenerator"/> class.
        /// </summary>
        /// <param name="q">The q.</param>
        public ANSISqlGenerator(SqlQuery q)
        {
            query = q;
        }


        #region ISqlGenerator Members

        /// <summary>
        /// Sets the insert query.
        /// </summary>
        /// <param name="q">The q.</param>
        public void SetInsertQuery(Insert q)
        {
            insert = q;
        }

        /// <summary>
        /// Finds the column.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public TableSchema.TableColumn FindColumn(string columnName)
        {
            TableSchema.TableColumn result = null;
            foreach(TableSchema.Table t in query.FromTables)
            {
                foreach(TableSchema.TableColumn tc in t.Columns)
                {
                    if(Utility.IsMatch(tc.ColumnName, columnName) || Utility.IsMatch(tc.QualifiedName, columnName))
                    {
                        result = tc;
                        break;
                    }
                }
            }//bferrier added so that Where clauses can act on Tables in the join
            if (result == null)
            {
                foreach (Join join in query.Joins)
                {
                    foreach (TableSchema.TableColumn tc in join.FromColumn.Table.Columns)
                    {
                        if (Utility.IsMatch(tc.ColumnName, columnName) || Utility.IsMatch(tc.QualifiedName, columnName))
                        {
                            result = tc;
                            break;
                        }
                    }

                }


            }
            return result;
        }

        /// <summary>
        /// Generates the group by.
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateGroupBy()
        {
            StringBuilder sb = new StringBuilder();

            bool isFirst = true;
            foreach(Aggregate agg in query.Aggregates)
            {
                if(agg.AggregateType == AggregateFunction.GroupBy)
                {
                    if(!isFirst)
                        sb.Append(", ");
                    sb.Append(agg.ColumnName);
                    isFirst = false;
                }
            }

            string result = String.Empty;
            if(sb.Length > 0)
                result = String.Concat(SqlFragment.GROUP_BY, sb);

            return result;
        }

        /// <summary>
        /// Generates the command line.
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateCommandLine()
        {
            StringBuilder sb = new StringBuilder();

            //start with the SqlCommand - SELECT, UPDATE, INSERT, DELETE
            sb.Append(query.SQLCommand);
            string columnList;
            if(query.Aggregates.Count > 0)
                columnList = BuildAggregateCommands();
            else
            {
                if (query.IsDistinct)
                    sb.Append(query.DistinctSpec);
                //set "TOP"
                sb.Append(query.TopSpec);

                //decide the columns
                if(query.SelectColumnList.Length == 0)
                    columnList = GenerateSelectColumnList();
                else
                {
                    StringBuilder sbCols = new StringBuilder();
                    //loop each column - 
                    //there n tables in the select list
                    //need to get the schema
                    //so for each column, loop the FromList until we find the column
                    bool isFirst = true;
                    foreach(string s in query.SelectColumnList)
                    {
                        if(!isFirst)
                            sbCols.Append(", ");
                        isFirst = false;
                        //find the column
                        TableSchema.TableColumn c = FindColumn(s);

                        if(c != null)
                            sbCols.Append(c.QualifiedName);
                        else
                        {
                            //just append it in - allowing for function calls
                            //or literals in the command line
                            sbCols.Append(s);
                        }
                    }
                    columnList = sbCols.ToString();
                }
            }
            sb.Append(columnList);

            if(query.Expressions.Count > 0)
            {
                //add in expression                
                foreach(string s in query.Expressions)
                {
                    sb.Append(",");
                    sb.Append(s);
                }
            }
            sb.AppendLine();

            string result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Generates the joins.
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateJoins()
        {
            StringBuilder sb = new StringBuilder();

            if(query.Joins.Count > 0)
            {
                //build up the joins
                foreach(Join j in query.Joins)
                {
                    string joinType = Join.GetJoinTypeValue(j.Type);
                    string equality = " = ";
                    if(j.Type == Join.JoinType.NotEqual)
                        equality = " <> ";

                    sb.Append(joinType);
                    sb.Append(j.FromColumn.Table.QualifiedName);
                    if(j.Type != Join.JoinType.Cross)
                    {
                        sb.Append(" ON ");
                        sb.Append(j.ToColumn.QualifiedName);
                        sb.Append(equality);
                        sb.Append(j.FromColumn.QualifiedName);
                    }
                    sb.AppendLine(String.Empty);
                }
            }
            string result = sb.ToString();
            return result;
        }

        /// <summary>
        /// Generates from list.
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateFromList()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(SqlFragment.FROM);

            bool isFirst = true;
            foreach(TableSchema.Table tbl in query.FromTables)
            {
                if(!isFirst)
                    sb.Append(",");
                sb.Append(tbl.QualifiedName);
                isFirst = false;
            }
            sb.Append(Environment.NewLine);
            return sb.ToString();
        }

        private void BuildConstraintSQL(ref string constraintOperator, StringBuilder sb, bool isFirst, ref bool expressionIsOpen, Constraint c)
        {
            string columnName = String.Empty;
            bool foundColumn = false;
            if(c.ConstructionFragment == c.ColumnName && c.ConstructionFragment != "##")
            {
                TableSchema.TableColumn col = FindColumn(c.ColumnName);

                if (c.Column != null && col != null)
                {
                    columnName = c.Column.QualifiedName;
                    foundColumn = true;
                    c.ParameterName = String.Concat(col.ParameterName, query.Constraints.IndexOf(c));
                }
                else
                {
                    if(col != null)
                    {
                        columnName = col.QualifiedName;
                        c.DbType = col.DataType;
                        foundColumn = true;
                        c.ParameterName = String.Concat(col.ParameterName, query.Constraints.IndexOf(c));
                    }
                }
            }

            if(!foundColumn && c.ConstructionFragment != "##")
            {
                bool isAggregate = false;
                //this could be an expression
                string rawColumnName = c.ConstructionFragment;
                if(c.ConstructionFragment.StartsWith("("))
                {
                    rawColumnName = c.ConstructionFragment.Replace("(", String.Empty);
                    expressionIsOpen = true;
                }
                    //this could be an aggregate function

                else if(c.IsAggregate || (c.ConstructionFragment.Contains("(") && c.ConstructionFragment.Contains(")")))
                {
                    rawColumnName = c.ConstructionFragment.Replace("(", String.Empty).Replace(")", String.Empty);
                    isAggregate = true;
                }

                TableSchema.TableColumn col = FindColumn(c.ColumnName);
                if(!isAggregate && col != null)
                {
                    if (c.ConstructionFragment.Contains(col.QualifiedName))
                        columnName = c.ConstructionFragment;
                    else
                        columnName = Utility.FastReplace(c.ConstructionFragment, col.ColumnName, col.QualifiedName, StringComparison.InvariantCultureIgnoreCase);

                    c.ParameterName = String.Concat(col.ParameterName, query.Constraints.IndexOf(c));
                    c.DbType = col.DataType;
                }
                else
                {
                    c.ParameterName = Utility.PrefixParameter(rawColumnName, query.Provider) + query.Constraints.IndexOf(c);
                    columnName = c.ConstructionFragment;
                }
            }

            //paramCount++;

            if(!isFirst)
            {
                constraintOperator = Enum.GetName(typeof(ConstraintType), c.Condition);
                constraintOperator = String.Concat(" ", constraintOperator.ToUpper(), " ");
            }

            if(c.Comparison != Comparison.OpenParentheses && c.Comparison != Comparison.CloseParentheses)
                sb.Append(constraintOperator);

            if(c.Comparison == Comparison.BetweenAnd)
            {
                sb.Append(columnName);
                sb.Append(SqlFragment.BETWEEN);
                sb.Append(c.ParameterName + "_start");
                sb.Append(SqlFragment.AND);
                sb.Append(c.ParameterName + "_end");
            }
            else if(c.Comparison == Comparison.In || c.Comparison == Comparison.NotIn)
            {
                sb.Append(columnName);
                if(c.Comparison == Comparison.In)
                    sb.Append(SqlFragment.IN);
                else
                    sb.Append(SqlFragment.NOT_IN);

                sb.Append("(");

                if(c.InSelect != null)
                {
                    //create a sql statement from the passed-in select
                    string sql = c.InSelect.BuildSqlStatement();
                    sb.Append(sql);
                }
                else
                {
                    //enumerate INs
                    IEnumerator en = c.InValues.GetEnumerator();
                    StringBuilder sbIn = new StringBuilder();
                    int i = 1;
                    while(en.MoveNext())
                    {
                        sbIn.Append(String.Concat(c.ParameterName, "In", i, ","));
                        i++;
                    }
                    string inList = sbIn.ToString();
                    inList = Strings.Chop(inList);
                    sb.Append(inList);
                }

                sb.Append(")");
            }
            else if(c.Comparison == Comparison.OpenParentheses)
            {
                expressionIsOpen = true;
                sb.Append("(");
            }
            else if(c.Comparison == Comparison.CloseParentheses)
            {
                expressionIsOpen = false;
                sb.Append(")");
            }
            else
            {
                if(columnName.StartsWith("("))
                    expressionIsOpen = true;
                if(c.ConstructionFragment != "##")
                {
                    sb.Append(columnName);
                    sb.Append(Constraint.GetComparisonOperator(c.Comparison));
                    if(c.Comparison == Comparison.Is || c.Comparison == Comparison.IsNot)
                    {
                        if(c.ParameterValue == null || c.ParameterValue == DBNull.Value)
                            sb.Append("NULL");
                    }
                    else
                        sb.Append(c.ParameterName);
                }
            }

            sb.AppendLine(String.Empty);
        }

        /// <summary>
        /// Generates (Where) constraints.
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateWhere()
        {
            string whereOperator = SqlFragment.WHERE;

            StringBuilder sb = new StringBuilder();
            bool isFirst = true;

            //int paramCount;
            bool expressionIsOpen = false;

            List<Constraint> nonAggregateConstraints = query.Constraints.FindAll(delegate(Constraint cs) { return !cs.IsAggregate; });
            foreach(Constraint c in nonAggregateConstraints)
            {
                BuildConstraintSQL(ref whereOperator, sb, isFirst, ref expressionIsOpen, c);
                isFirst = false;
                //isFirst = sb.ToString().StartsWith(whereOperator);
            }

            string result = sb.ToString();
            //a little help...
            if(expressionIsOpen & !result.EndsWith(")"))
                result = String.Concat(result, ")");

            if(query.LogicalDeleteColumns.Count > 0)
            {
                isFirst = true;
                foreach(TableSchema.TableColumn column in query.LogicalDeleteColumns)
                {
                    string fragment = SqlFragment.WHERE;
                    if(query.Constraints.Count > 0 || !isFirst)
                        fragment = SqlFragment.AND;
                    isFirst = false;
                    string expression = String.Format("\r\n{0}({1} IS NULL OR {1} = 0)", fragment, column.QualifiedName);
                    result = String.Concat(result, expression);
                }
            }

            return result;
        }

        /// <summary>
        /// Generates (Having) constraints  
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateHaving()
        {
            string whereOperator = SqlFragment.HAVING;

            StringBuilder sb = new StringBuilder();
            bool isFirst = true;

            //int paramCount;
            bool expressionIsOpen = false;

            List<Constraint> aggregateConstraints = query.Constraints.FindAll(delegate(Constraint cs) { return cs.IsAggregate; });
            foreach(Constraint c in aggregateConstraints)
            {
                BuildConstraintSQL(ref whereOperator, sb, isFirst, ref expressionIsOpen, c);
                isFirst = false;
                //isFirst = sb.ToString().StartsWith(whereOperator);
            }

            string result = sb.ToString();
            //a little help...
            if(expressionIsOpen & !result.EndsWith(")"))
                result = String.Concat(result, ")");

            return result;
        }

        /// <summary>
        /// Generates the order by.
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateOrderBy()
        {
            StringBuilder sb = new StringBuilder();
            if(query.OrderBys.Count > 0)
            {
                sb.Append(SqlFragment.ORDER_BY);
                bool isFirst = true;
                foreach(string s in query.OrderBys)
                {
                    if(!isFirst)
                        sb.Append(",");
                    sb.Append(s);
                    isFirst = false;
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets the select columns.
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetSelectColumns()
        {
            List<string> result = new List<string>();
            string columns;

            if(query.SelectColumnList.Length == 0)
            {
                columns = GenerateSelectColumnList();
                string[] columnList = columns.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                foreach(string s in columnList)
                    result.Add(s);
            }
            else
            {
                foreach(string s in query.SelectColumnList)
                    result.Add(s);
            }

            return result;
        }

        /// <summary>
        /// Gets the count select.
        /// </summary>
        /// <returns></returns>
        public virtual string GetCountSelect()
        {
            string getCountSelect;
            string column = GetSelectColumns()[0];

            const string countSelect = @"SELECT COUNT(*) FROM ({0}) AS CountOfRecords";
            if (query.Aggregates.Count > 0 || query.IsDistinct)
            {
                getCountSelect = String.Format(countSelect, BuildSelectStatement());
            }
            else
            {
                getCountSelect = String.Concat(String.Concat(query.SQLCommand, Aggregate.Count("*", "CountOfRecords")), GenerateFromList(), GenerateJoins(),
                    GenerateWhere());
            }
            return getCountSelect;
        }

        /// <summary>
        /// Gets the paging SQL wrapper.
        /// </summary>
        /// <returns></returns>
        public virtual string GetPagingSqlWrapper()
        {
            return PAGING_SQL;
        }

        /// <summary>
        /// Builds the paged select statement.
        /// </summary>
        /// <returns></returns>
        public virtual string BuildPagedSelectStatement()
        {
            string idColumn = GetSelectColumns()[0];
            string sqlType;

            TableSchema.TableColumn idCol = FindColumn(idColumn);
            if(idCol != null)
            {
                string pkType = String.Empty;
                if(Utility.IsString(idCol))
                    pkType = String.Concat("(", idCol.MaxLength, ")");
                sqlType = Enum.GetName(typeof(SqlDbType), Utility.GetSqlDBType(idCol.DataType));
                sqlType = String.Concat(sqlType, pkType);
            }
            else
            {
                //assume it's an integer
                sqlType = Enum.GetName(typeof(SqlDbType), SqlDbType.Int);
            }

            string select = GenerateCommandLine();
            //string columnList = select.Replace("SELECT", "");
            string fromLine = GenerateFromList();
            string joins = GenerateJoins();
            string wheres = GenerateWhere();
            string havings = string.Empty;

            //have to doctor the wheres, since we're using a WHERE in the paging
            //bits. So change all "WHERE" to "AND"
            string tweakedWheres = wheres.Replace("WHERE", "AND");
            string orderby = GenerateOrderBy();

            if(query.Aggregates.Count > 0)
            {
                joins = String.Concat(joins, GenerateGroupBy());
                havings = GenerateHaving();
            }

            //this uses SQL2000-compliant paging
            //the arguments are...
            //1 - id column - this is the PK or identifier
            //2 - from/join/where
            //3 - select/from/joins
            //4 - where/order by
            //5 - page index
            //6 - page size
            //7 - PK Type (using Utility.GetSqlDBType)

            string sql = string.Format(PAGING_SQL, idColumn, String.Concat(fromLine, joins, wheres), String.Concat(tweakedWheres, orderby, havings),
                String.Concat(select, fromLine, joins), query.CurrentPage, query.PageSize, sqlType);

            return sql;
        }

        /// <summary>
        /// Builds the select statement.
        /// </summary>
        /// <returns></returns>
        public virtual string BuildSelectStatement()
        {
            StringBuilder sql = new StringBuilder();

            if(query.PageSize > 0)
                sql.Append(BuildPagedSelectStatement());
            else
            {
                //build the command string
                sql.Append(GenerateCommandLine());
                sql.Append(GenerateFromList());
                sql.Append(GenerateJoins());

                sql.Append(GenerateWhere());

                if(query.Aggregates.Count > 0)
                {
                    sql.Append(GenerateGroupBy());
                    sql.Append(Environment.NewLine);
                    sql.Append(GenerateHaving());
                }

                sql.Append(GenerateOrderBy());
            }
            //return
            return sql.ToString();
        }

        /// <summary>
        /// Builds the update statement.
        /// </summary>
        /// <returns></returns>
        public virtual string BuildUpdateStatement()
        {
            StringBuilder sb = new StringBuilder();

            //cast it

            Update u = (Update)query;
            sb.Append(SqlFragment.UPDATE);
            sb.Append(u.FromTables[0].QualifiedName);

            for(int i = 0; i < u.SetStatements.Count; i++)
            {
                if(i == 0)
                    sb.Append(SqlFragment.SET);

                if(!String.IsNullOrEmpty(u.ProviderName))
                    sb.Append(DataService.GetInstance(u.ProviderName).DelimitDbName(u.SetStatements[i].ColumnName));
                else
                    sb.Append(u.SetStatements[i].ColumnName);

                sb.Append("=");

                if(!u.SetStatements[i].IsExpression)
                    sb.Append(u.SetStatements[i].ParameterName);
                else
                    sb.Append(u.SetStatements[i].Value.ToString());

                if(i + 1 < u.SetStatements.Count)
                    sb.AppendLine(",");
                else
                    sb.AppendLine();
            }

            //wheres
            sb.Append(GenerateWhere());

            return sb.ToString();
        }

        /// <summary>
        /// Builds the insert statement.
        /// </summary>
        /// <returns></returns>
        public string BuildInsertStatement()
        {
            StringBuilder sb = new StringBuilder();

            //cast it
            Insert i = insert;
            sb.Append(SqlFragment.INSERT_INTO);
            sb.Append(i.Table.QualifiedName);
            sb.Append("(");
            sb.Append(i.SelectColumns);
            sb.AppendLine(")");

            //if the values list is set, use that
            if(i.Inserts.Count > 0)
            {
                sb.Append(" VALUES (");
                bool isFirst = true;
                foreach(InsertSetting s in i.Inserts)
                {
                    if(!isFirst)
                        sb.Append(",");
                    if(!s.IsExpression)
                        sb.Append(s.ParameterName);
                    else
                        sb.Append(s.Value);
                    isFirst = false;
                }
                sb.AppendLine(")");
            }
            else
            {
                if(i.SelectValues != null)
                {
                    string selectSql = i.SelectValues.BuildSqlStatement();
                    sb.AppendLine(selectSql);
                }
                else
                    throw new SqlQueryException("Need to specify Values or a Select query to insert - can't go on!");
            }
            return sb.ToString();
        }

        /// <summary>
        /// Builds the delete statement.
        /// </summary>
        /// <returns></returns>
        public string BuildDeleteStatement()
        {
            StringBuilder sb = new StringBuilder();

            //see if the from table has a "Deleted"
            TableSchema.Table tbl = query.FromTables[0];

            if(tbl.Columns.Contains(ReservedColumnName.DELETED) && query.GetType().Name != "Destroy")
            {
                TableSchema.TableColumn col = tbl.GetColumn("deleted");
                sb.Append(SqlFragment.UPDATE);
                sb.Append(tbl.QualifiedName);
                sb.Append(SqlFragment.SET);
                sb.Append(col.QualifiedName);

                //by default this is a bit in the DB
                //may have to rework this for MySQL
                sb.Append(" = 1");
            }
            else
            {
                sb.Append(SqlFragment.DELETE_FROM);
                sb.Append(query.FromTables[0].QualifiedName);
            }

            sb.Append(GenerateWhere());

            return sb.ToString();
        }

        #endregion


        #region Migration Support

        List<string> createdKeys = new List<string>();

        public virtual string BuildForeignKeyStatement(TableSchema.TableColumn oneTable, TableSchema.TableColumn manyTable)
        {
            if(oneTable == null)
                throw new InvalidOperationException("From column cannot be null");

            if(manyTable == null)
                throw new InvalidOperationException("To column cannot be null");

            const string sqlFormat = "ALTER TABLE {0} ADD CONSTRAINT {1} FOREIGN KEY({2}) REFERENCES {3}({4})";

            string fkName = string.Format("fk_{0}_{1}_{2}", oneTable.Table.Name, oneTable.ColumnName, manyTable.ColumnName);

            string sql = string.Format(sqlFormat, manyTable.Table.QualifiedName, fkName, manyTable.ColumnName, oneTable.Table.QualifiedName, oneTable.ColumnName);

            return sql;
        }

        public virtual string BuildForeignKeyDropStatement(TableSchema.TableColumn oneTable, TableSchema.TableColumn manyTable)
        {
            if(oneTable == null)
                throw new InvalidOperationException("From column cannot be null");

            if(manyTable == null)
                throw new InvalidOperationException("To column cannot be null");

            const string sqlFormat = "ALTER TABLE {0} DROP CONSTRAINT {1}";
            string fkName = string.Format("fk_{0}_{1}_{2}", oneTable.Table.Name, oneTable.ColumnName, manyTable.ColumnName);
            
            string sql = string.Format(sqlFormat, manyTable.Table.QualifiedName, fkName);

            return sql;
        }

        /// <summary>
        /// Builds a CREATE TABLE statement.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual string BuildCreateTableStatement(TableSchema.Table table)
        {
            string columnSql = GenerateColumns(table);
            return string.Format(CREATE_TABLE, table.QualifiedName, columnSql);
        }

        /// <summary>
        /// Builds a DROP TABLE statement.
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public virtual string BuildDropTableStatement(TableSchema.Table table)
        {
            return string.Format(DROP_TABLE, table.QualifiedName);
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        public virtual string BuildAddColumnStatement(TableSchema.Table table, TableSchema.TableColumn column)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(ADD_COLUMN, table.QualifiedName, column.DelimitedName, GenerateColumnAttributes(column));
            return sql.ToString();
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="column">The column.</param>
        public virtual string BuildAlterColumnStatement(TableSchema.TableColumn column)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(ALTER_COLUMN, column.Table.QualifiedName, column.DelimitedName, GenerateColumnAttributes(column));
            return sql.ToString();
        }

        /// <summary>
        /// Removes the column.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public virtual string BuildDropColumnStatement(TableSchema.Table table, TableSchema.TableColumn column)
        {
            StringBuilder sql = new StringBuilder();

            //check to see if there are any constraints
            //QueryCommand cmd;
            if(!string.IsNullOrEmpty(column.DefaultSetting))
            {
                sql.AppendFormat("ALTER TABLE {0} DROP CONSTRAINT DF_{0}_{1}", table.Name, column.ColumnName);
                sql.Append(";\r\n");

                //drop FK constraints ...

                //drop CHECK constraints ...
            }

            sql.AppendFormat(DROP_COLUMN, table.QualifiedName, column.DelimitedName);
            return sql.ToString();
        }

        /// <summary>
        /// Gets the type of the native.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <returns></returns>
        protected virtual string GetNativeType(DbType dbType)
        {
            switch(dbType)
            {
                case DbType.Object:
                case DbType.AnsiString:
                    return "varchar";
                case DbType.AnsiStringFixedLength:
                    return "char";
                case DbType.String:
                    return "nvarchar";
                case DbType.StringFixedLength:
                    return "nchar";
                case DbType.Boolean:
                    return "bit";
                case DbType.SByte:
                case DbType.Binary:
                case DbType.Byte:
                    return "tinyint";
                case DbType.Currency:
                    return "money";
                case DbType.Time:
                case DbType.Date:
                case DbType.DateTime:
                    //Orcas Only! case DbType.DateTime2:
                    //Orcas Only! case DbType.DateTimeOffset:
                    return "datetime";
                case DbType.Decimal:
                    return "decimal";
                case DbType.Double:
                    return "float";
                case DbType.Guid:
                    return "uniqueidentifier";
                case DbType.UInt32:
                case DbType.UInt16:
                case DbType.Int16:
                case DbType.Int32:
                    return "int";
                case DbType.UInt64:
                case DbType.Int64:
                    return "bigint";
                case DbType.Single:
                    return "real";
                case DbType.VarNumeric:
                    return "numeric";
                case DbType.Xml:
                    return "xml";
                default:
                    return "nvarchar";
            }
        }

        /// <summary>
        /// Generates SQL for all the columns in table
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <returns>
        /// SQL fragment representing the supplied columns.
        /// </returns>
        protected virtual string GenerateColumns(TableSchema.Table tableSchema)
        {
            StringBuilder createSql = new StringBuilder();

            foreach(TableSchema.TableColumn col in tableSchema.Columns)
                createSql.AppendFormat("\r\n  {0}{1},", tableSchema.Provider.DelimitDbName(col.ColumnName), GenerateColumnAttributes(col));
            string columnSql = createSql.ToString();
            return Strings.Chop(columnSql, ",");
        }

        /// <summary>
        /// Sets the column attributes.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        protected virtual string GenerateColumnAttributes(TableSchema.TableColumn column)
        {
            StringBuilder sb = new StringBuilder();
            if(column.DataType == DbType.String && column.MaxLength >= 4000)
            {
                //use nvarchar MAX 
                //TODO - this won't work for SQL 2000
                //need to tell the diff somehow
                sb.Append(" nvarchar(MAX)");
            }
            else
                sb.Append(" " + GetNativeType(column.DataType));

            if(column.IsPrimaryKey)
            {

                if (column.MaxLength > 0)
                    sb.AppendFormat("({0}) ", column.MaxLength);

                sb.Append(" NOT NULL PRIMARY KEY");
                if(column.IsNumeric)
                    sb.Append(" IDENTITY(1,1)");
            }
            else
            {
                //thanks to robbam for this
                if (column.DataType == DbType.Decimal)
                    sb.Append("(" + column.NumberPrecision + "," + column.NumberScale + ")");
                else if (column.MaxLength > 0 && column.MaxLength < 8000)
                    sb.Append("(" + column.MaxLength + ")");
                
                if(!column.IsNullable)
                    sb.Append(" NOT NULL");
                else
                    sb.Append(" NULL");

                if(!String.IsNullOrEmpty(column.DefaultSetting))
                    sb.Append(" CONSTRAINT DF_" + column.Table.Name + "_" + column.ColumnName + " DEFAULT (" + column.DefaultSetting + ")");
            }

            return sb.ToString();
        }

        #endregion


        #region Constants

        protected const string ADD_COLUMN = @"ALTER TABLE {0} ADD {1}{2}";
        protected const string ALTER_COLUMN = @"ALTER TABLE {0} ALTER COLUMN {1}{2}";
        protected const string CREATE_TABLE = "CREATE TABLE {0} ({1} \r\n)";
        protected const string DROP_COLUMN = @"ALTER TABLE {0} DROP COLUMN {1}";
        protected const string DROP_TABLE = @"DROP TABLE {0}";

        private const string PAGING_SQL =
            @"					
					DECLARE @Page int
					DECLARE @PageSize int

					SET @Page = {4}
					SET @PageSize = {5}

					SET NOCOUNT ON

					-- create a temp table to hold order ids
					DECLARE @TempTable TABLE (IndexId int identity, _keyID {6})

					-- insert the table ids and row numbers into the memory table
					INSERT INTO @TempTable
					(
					  _keyID
					)
					SELECT 
						{0}
					    {1}
                        {2}
					-- select only those rows belonging to the proper page
					    {3}
					INNER JOIN @TempTable t ON {0} = t._keyID
					WHERE t.IndexId BETWEEN ((@Page - 1) * @PageSize + 1) AND (@Page * @PageSize)
                    
                    ";

        #endregion


        /// <summary>
        /// Qualifies the name of the table.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <returns></returns>
        public virtual string QualifyTableName(TableSchema.Table tableSchema)
        {
            return tableSchema.QualifiedName;
        }

        /// <summary>
        /// Gets the qualified select.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <returns></returns>
        public string GetQualifiedSelect(TableSchema.Table table)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;

            foreach(TableSchema.TableColumn tc in table.Columns)
            {
                if(!isFirst)
                    sb.Append(", ");

                sb.Append(tc.QualifiedName);
                isFirst = false;
            }

            string result = sb.ToString();

            return result;
        }

        /// <summary>
        /// Generates the select column list.
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateSelectColumnList()
        {
            StringBuilder sbColumns = new StringBuilder();
            foreach(TableSchema.Table tbl in query.FromTables)
            {
                string columnList = GetQualifiedSelect(tbl);
                sbColumns.AppendLine(columnList);
            }
            return sbColumns.ToString();
        }

        /// <summary>
        /// Builds the aggregate commands.
        /// </summary>
        /// <returns></returns>
        protected virtual string BuildAggregateCommands()
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach(Aggregate agg in query.Aggregates)
            {
                if(!isFirst)
                    sb.Append(", ");
                sb.Append(GenerateAggregateSelect(agg));
                isFirst = false;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Generates the 'SELECT' part of an <see cref="Aggregate"/>
        /// </summary>
        /// <param name="aggregate">The aggregate to include in the SELECT clause</param>
        /// <returns>The portion of the SELECT clause represented by this <see cref="Aggregate"/></returns>
        /// <remarks>
        /// The ToString() logic moved from <see cref="Aggregate.ToString"/>, rather than
        /// including it in the Aggregate class itself...
        /// </remarks>
        protected virtual string GenerateAggregateSelect(Aggregate aggregate)
        {
            bool hasAlias = !String.IsNullOrEmpty(aggregate.Alias);

            if(aggregate.AggregateType == AggregateFunction.GroupBy && hasAlias)
                return String.Format("{0} AS '{1}'", aggregate.ColumnName, aggregate.Alias);
            if(aggregate.AggregateType == AggregateFunction.GroupBy)
                return aggregate.ColumnName;
            if(hasAlias)
                return String.Format("{0}({1}) AS '{2}'", Aggregate.GetFunctionType(aggregate).ToUpperInvariant(), aggregate.ColumnName, aggregate.Alias);

            return String.Format("{0}({1})", Aggregate.GetFunctionType(aggregate).ToUpperInvariant(), aggregate.ColumnName);
        }
    }
}
