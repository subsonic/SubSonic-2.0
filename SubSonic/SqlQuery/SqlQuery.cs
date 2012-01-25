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
using System.Reflection;
using System.Text;
using System.Transactions;
using System.Xml;
using SubSonic.Parser;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class SqlQuery
    {
        #region QueryType enum

        /// <summary>
        /// 
        /// </summary>
        public enum QueryType
        {
            /// <summary>
            /// 
            /// </summary>
            Select,
            /// <summary>
            /// 
            /// </summary>
            Update,
            /// <summary>
            /// 
            /// </summary>
            Insert,
            /// <summary>
            /// 
            /// </summary>
            Delete
        }

        #endregion


        private List<Constraint> _constraints = new List<Constraint>();
        private string _distinctspec = String.Empty;
        private List<string> _expressions = new List<string>();
        private TableSchema.TableCollection _fromTables = new TableSchema.TableCollection();
        private List<OrderBySQ> _orderBys = new List<OrderBySQ>();
        private string[] _selectColumnList = new string[0];
        private string _sqlCommand = SqlFragment.SELECT;
        private string _topSpec = String.Empty;

        private DataProvider provider;
        internal TableSchema.TableColumnCollection LogicalDeleteColumns = new TableSchema.TableColumnCollection();
        internal TableSchema.TableColumnCollection SelectColumns = new TableSchema.TableColumnCollection();

        /// <summary>
        /// 
        /// </summary>
        /// 
        /// <summary>
        /// 
        /// </summary>
        public List<Join> Joins = new List<Join>();

        /// <summary>
        /// 
        /// </summary>
        public List<Aggregate> Aggregates = new List<Aggregate>();

        /// <summary>
        /// 
        /// </summary>
        public string IdColumn = String.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string ProviderName = DataService.Provider.Name;

        /// <summary>
        /// 
        /// </summary>
        public QueryType QueryCommandType = QueryType.Select;



        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQuery"/> class.
        /// </summary>
        public SqlQuery() : this(DataService.Provider) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlQuery"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public SqlQuery(DataProvider provider)
        {
            Provider = provider;
        }


        #region Validation

        /// <summary>
        /// Validates the query.
        /// </summary>
        public virtual void ValidateQuery()
        {
            //gotta have a "FROM"
            if(FromTables.Count == 0)
                throw new SqlQueryException("Need to have at least one From table specified");
        }

        #endregion


        #region Constraints


        /// <summary>
        /// Gets a value indicating whether this instance has where.
        /// </summary>
        /// <value><c>true</c> if this instance has where; otherwise, <c>false</c>.</value>
        public bool HasWhere
        {
            get
            {
                foreach(Constraint constraint in Constraints)
                {
                    if(constraint.Condition == ConstraintType.Where)
                        return true;
                }
                return false;
            }
        }


        #region Where

        /// <summary>
        /// Wheres the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public Constraint Where(string columnName)
        {
            return new Constraint(ConstraintType.Where, columnName, this);
        }

        /// <summary>
        /// Wheres the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public Constraint Where(TableSchema.TableColumn column)
        {
            Constraint c = new Constraint(ConstraintType.Where, column.ColumnName, column.QualifiedName, column.ColumnName, this)
                               {
                                   TableName = column.Table.TableName,
                                   DbType = column.DataType,
                                   Column = column
                               };
            return c;
        }

        /// <summary>
        /// Wheres the specified agg.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        public Constraint Where(Aggregate aggregate)
        {
            Constraint c = new Constraint(ConstraintType.Where,  aggregate.ColumnName, aggregate.ColumnName, aggregate.WithoutAlias(), this)
                               {
                                   IsAggregate = true
                               };
            return c;
        }

        /// <summary>
        /// Wheres the expression.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public Constraint WhereExpression(string columnName)
        {
            OpenParenCount++;
            return new Constraint(ConstraintType.Where, columnName, columnName, "(" + columnName, this);
        }

        #endregion


        #region Or

        /// <summary>
        /// Ors the expression.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public Constraint OrExpression(string columnName)
        {
            //as a convenience, check that the last constraint
            //is a close paren
            if(Constraints.Count > 0 && (ClosedParenCount < OpenParenCount))
            {
                Constraint last = Constraints[Constraints.Count - 1];
                if(last.Comparison != Comparison.CloseParentheses)
                    CloseExpression();
            }

            OpenParenCount++;
            return new Constraint(ConstraintType.Or, columnName, columnName, "(" + columnName, this);
        }

        /// <summary>
        /// Ors the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public Constraint Or(string columnName)
        {
            return new Constraint(ConstraintType.Or, columnName, this);
        }

        /// <summary>
        /// Ors the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public Constraint Or(TableSchema.TableColumn column)
        {
            Constraint c = new Constraint(ConstraintType.Or, column.ColumnName, column.QualifiedName, column.ColumnName, this)
                               {
                                   TableName = column.Table.TableName,
                                   DbType = column.DataType,
                                   Column = column
                               };
            return c;
        }

        /// <summary>
        /// Ors the specified agg.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        public Constraint Or(Aggregate aggregate)
        {
            Constraint c = new Constraint(ConstraintType.Or, aggregate.ColumnName, aggregate.ColumnName, aggregate.WithoutAlias(), this)
                               {
                                   IsAggregate = true
                               };
            return c;
        }

        #endregion


        #region And

        /// <summary>
        /// Ands the expression.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public Constraint AndExpression(string columnName)
        {
            //as a convenience, check that the last constraint
            //is a close paren
            if(Constraints.Count > 0 && (ClosedParenCount < OpenParenCount))
            {
                Constraint last = Constraints[Constraints.Count - 1];
                if(last.Comparison != Comparison.CloseParentheses)
                    CloseExpression();
            }
            OpenParenCount++;
            return new Constraint(ConstraintType.And, columnName, columnName, "(" + columnName, this);
        }

        /// <summary>
        /// Ands the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public Constraint And(string columnName)
        {
            return new Constraint(ConstraintType.And, columnName, this);
        }

        /// <summary>
        /// Ands the specified column.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public Constraint And(TableSchema.TableColumn column)
        {
            Constraint c = new Constraint(ConstraintType.And, column.ColumnName, column.QualifiedName, column.ColumnName, this)
                               {
                                   TableName = column.Table.Name,
                                   DbType = column.DataType,
                                   Column = column
                               };
            return c;
        }

        /// <summary>
        /// Ands the specified agg.
        /// </summary>
        /// <param name="aggregate">The aggregate.</param>
        /// <returns></returns>
        public Constraint And(Aggregate aggregate)
        {
            Constraint c = new Constraint(ConstraintType.And, aggregate.ColumnName, aggregate.ColumnName, aggregate.WithoutAlias(), this)
                               {
                                   IsAggregate = true
                               };
            return c;
        }

        #endregion


        /// <summary>
        /// Opens the expression.
        /// </summary>
        /// <returns></returns>
        public SqlQuery OpenExpression()
        {
            Constraint c = new Constraint(ConstraintType.Where, "##", "##", "##", this)
                               {
                                   Comparison = Comparison.OpenParentheses
                               };
            Constraints.Add(c);
            OpenParenCount++;
            return this;
        }

        /// <summary>
        /// Closes the expression.
        /// </summary>
        /// <returns></returns>
        public SqlQuery CloseExpression()
        {
            Constraint c = new Constraint(ConstraintType.Where, "##", "##", "##", this)
                               {
                                   Comparison = Comparison.CloseParentheses
                               };
            Constraints.Add(c);
            ClosedParenCount++;
            return this;
        }

        #endregion


        #region Exception Handling

        internal static SqlQueryException GenerateException(Exception fromException)
        {
            string message = fromException.Message;

            if(fromException.Message.ToLower().Contains("user instance"))
                message = "You're trying to connect to a database in your App_Data directory, and your SQL Server installation does not support User Instances.";
            else if(fromException.Message.Contains("use correlation names"))
            {
                message =
                    "The joins on your query are not ordered properly - make sure you're not repeating a table in the first (or 'from') position on a join that's also specified in FROM. Also - a JOIN can't have two of the same table in the 'from' first position. Check the SQL output to see how to order this properly";
            }

            return new SqlQueryException(message, fromException);
        }

        #endregion


        #region object overrides

        /// <summary>
        /// Returns the currently set SQL statement for this query object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return BuildSqlStatement();
        }

        #endregion


        #region Sql Builder

        internal ISqlGenerator GetGenerator()
        {
            // decide the provider
            if(Provider == null)
                Provider = DataService.Providers[ProviderName];

            return DataService.GetGenerator(provider, this);
        }

        /// <summary>
        /// Builds the SQL statement.
        /// </summary>
        /// <returns></returns>
        public virtual string BuildSqlStatement()
        {
            ValidateQuery();
            ISqlGenerator generator = GetGenerator();
            string sql;

            switch(QueryCommandType)
            {
                case QueryType.Update:
                    sql = generator.BuildUpdateStatement();
                    break;
                case QueryType.Insert:
                    sql = generator.BuildInsertStatement();
                    break;
                case QueryType.Delete:
                    sql = generator.BuildDeleteStatement();
                    break;
                default:
                    sql = PageSize > 0 ? generator.BuildPagedSelectStatement() : generator.BuildSelectStatement();
                    break;
            }

            return sql;
        }

        #endregion


        #region Command Builder

        internal static void SetConstraintParams(SqlQuery qry, QueryCommand cmd)
        {
            //loop the constraints and add the values
            foreach(Constraint c in qry.Constraints)
            {
                if(c.Comparison == Comparison.BetweenAnd)
                {
                    cmd.Parameters.Add(String.Concat(c.ParameterName, "_start"), c.StartValue, c.DbType);
                    cmd.Parameters.Add(String.Concat(c.ParameterName + "_end"), c.EndValue, c.DbType);
                }
                else if(c.Comparison == Comparison.In || c.Comparison == Comparison.NotIn)
                {
                    if(c.InSelect != null)
                    {
                        //set the parameters for the nested select
                        //this will support recursive INs... I hope
                        SetConstraintParams(c.InSelect, cmd);
                    }
                    else
                    {
                        int i = 1;
                        IEnumerator en = c.InValues.GetEnumerator();
                        while(en.MoveNext())
                        {
                            cmd.Parameters.Add(String.Concat(c.ParameterName, "In", i), en.Current, c.DbType);
                            i++;
                        }
                    }
                }
                else if ((c.Comparison == Comparison.Is || c.Comparison == Comparison.IsNot) && c.ParameterValue == DBNull.Value)
                { // bferrier Do not bind a varible for IsNull  as the test of Is NULL is already added and unbound variables in some DB's throws an error
                    continue;
                }
                else
                    cmd.Parameters.Add(c.ParameterName, c.ParameterValue, c.DbType);
            }
        }

        internal void SetConstraintParams(QueryCommand cmd)
        {
            //loop the constraints and add the values
            SetConstraintParams(this, cmd);
        }

        internal QueryCommand BuildCommand()
        {
            QueryCommand cmd = new QueryCommand(BuildSqlStatement(), provider.Name);
            SetConstraintParams(cmd);

            return cmd;
        }


        #endregion


        #region From

        /// <summary>
        /// Froms the specified TBL.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public SqlQuery From(string tableName)
        {
            TableSchema.Table t = DataService.GetSchema(tableName, ProviderName);
            if(t != null)
                FromTables.Add(t);

            return this;
        }

        public TableSchema.TableColumn GetFROMCol(string colName) {
            // Get reference to table
            if (FromTables.Count < 1) {
                SqlQueryException ex = new SqlQueryException("Query must have at least one table");
                throw ex;
            }
            TableSchema.Table tbl = FromTables[0];
            TableSchema.TableColumn col = tbl.GetColumn(colName);
            if (col == null) {
                SqlQueryException ex = new SqlQueryException("Column '" + colName + "' not found !");
                throw ex;
            }
            return col;
        }

        /// <summary>
        /// Froms the specified table schema.
        /// </summary>
        /// <param name="tableSchema">The table schema.</param>
        /// <returns></returns>
        public SqlQuery From(TableSchema.Table tableSchema)
        {
            FromTables.Add(tableSchema);
            Provider = tableSchema.Provider;
            return this;
        }

        /// <summary>
        /// Froms this instance.
        /// </summary>
        /// <typeparam name="TSchema">The type of the schema.</typeparam>
        /// <returns></returns>
        public SqlQuery From<TSchema>() where TSchema : RecordBase<TSchema>, new()
        {
            TSchema item = new TSchema();
            TableSchema.Table tbl = item.GetSchema();
			Provider = tbl.Provider;
            FromTables.Add(tbl);
            return this;
        }

        #endregion


        #region Joins


        #region Join Builders

        private void CreateJoin(TableSchema.Table from, Join.JoinType type)
        {
            if(FromTables.Count == 0)
                throw new InvalidOperationException("You must specify at least one FROM table so we know what to join on - use From()");

            TableSchema.Table to = FromTables[0];
            Join j = new Join(from, to, type);

            //if(j != null)
            Joins.Add(j);
            //else
            //    throw new Exception("Can't find join columns for these tables - you might need to be explicit here, spcecifying the columns to join on instead");
        }

        private void CreateJoin(TableSchema.TableColumn fromColumn, TableSchema.TableColumn toColumn, Join.JoinType type)
        {
            Join j = new Join(fromColumn, toColumn, type);
            Joins.Add(j);
        }

        private void CreateJoin(string fromTable, string fromColumn, string toTable, string toColumn, Join.JoinType type)
        {
            TableSchema.Table from = DataService.GetSchema(fromTable, provider.Name);
            TableSchema.Table to = DataService.GetSchema(toTable, provider.Name);

            //get the columns
            TableSchema.TableColumn fromCol = from.GetColumn(fromColumn);
            TableSchema.TableColumn toCol = to.GetColumn(toColumn);

            //set the join
            Join j = new Join(fromCol, toCol, type);

            Joins.Add(j);
        }

        #endregion


        #region Inner

        /// <summary>
        /// Inners the join.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SqlQuery InnerJoin<T>() where T : RecordBase<T>, new()
        {
            CreateJoin(new T().GetSchema(), Join.JoinType.Inner);
            return this;
        }

        /// <summary>
        /// Inners the join.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public SqlQuery InnerJoin(TableSchema.Table from)
        {
            CreateJoin(from, Join.JoinType.Inner);
            return this;
        }

        /// <summary>
        /// Inners the join.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public SqlQuery InnerJoin(string tableName)
        {
            TableSchema.Table schema = DataService.GetSchema(tableName, provider.Name);
            return InnerJoin(schema);
        }

        /// <summary>
        /// Inners the join.
        /// </summary>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery InnerJoin(TableSchema.TableColumn fromColumn, TableSchema.TableColumn toColumn)
        {
            CreateJoin(fromColumn, toColumn, Join.JoinType.Inner);
            return this;
        }

        /// <summary>
        /// Inners the join.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toTable">To table.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery InnerJoin(string fromTable, string fromColumn, string toTable, string toColumn)
        {
            CreateJoin(fromTable, fromColumn, toTable, toColumn, Join.JoinType.Inner);
            return this;
        }

        #endregion


        #region Outer

        /// <summary>
        /// Outers the join.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SqlQuery OuterJoin<T>() where T : RecordBase<T>, new()
        {
            CreateJoin(new T().GetSchema(), Join.JoinType.Outer);
            return this;
        }

        /// <summary>
        /// Outers the join.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public SqlQuery OuterJoin(TableSchema.Table from)
        {
            CreateJoin(from, Join.JoinType.Outer);
            return this;
        }

        /// <summary>
        /// Outers the join.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public SqlQuery OuterJoin(string tableName)
        {
            TableSchema.Table schema = DataService.GetSchema(tableName, provider.Name);
            return OuterJoin(schema);
        }

        /// <summary>
        /// Outers the join.
        /// </summary>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery OuterJoin(TableSchema.TableColumn fromColumn, TableSchema.TableColumn toColumn)
        {
            CreateJoin(fromColumn, toColumn, Join.JoinType.Outer);
            return this;
        }

        /// <summary>
        /// Outers the join.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toTable">To table.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery OuterJoin(string fromTable, string fromColumn, string toTable, string toColumn)
        {
            CreateJoin(fromTable, fromColumn, toTable, toColumn, Join.JoinType.Outer);
            return this;
        }

        #endregion


        #region Cross

        /// <summary>
        /// Crosses the join.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SqlQuery CrossJoin<T>() where T : RecordBase<T>, new()
        {
            CreateJoin(new T().GetSchema(), Join.JoinType.Cross);
            return this;
        }

        /// <summary>
        /// Crosses the join.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public SqlQuery CrossJoin(TableSchema.Table from)
        {
            CreateJoin(from, Join.JoinType.Cross);
            return this;
        }

        /// <summary>
        /// Crosses the join.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public SqlQuery CrossJoin(string tableName)
        {
            TableSchema.Table schema = DataService.GetSchema(tableName, provider.Name);
            CreateJoin(schema, Join.JoinType.Cross);
            return this;
        }

        /// <summary>
        /// Crosses the join.
        /// </summary>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery CrossJoin(TableSchema.TableColumn fromColumn, TableSchema.TableColumn toColumn)
        {
            CreateJoin(fromColumn, toColumn, Join.JoinType.Cross);
            return this;
        }

        /// <summary>
        /// Crosses the join.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toTable">To table.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery CrossJoin(string fromTable, string fromColumn, string toTable, string toColumn)
        {
            CreateJoin(fromTable, fromColumn, toTable, toColumn, Join.JoinType.Cross);
            return this;
        }

        #endregion


        #region LeftInner

        /// <summary>
        /// Lefts the inner join.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SqlQuery LeftInnerJoin<T>() where T : RecordBase<T>, new()
        {
            CreateJoin(new T().GetSchema(), Join.JoinType.LeftInner);
            return this;
        }

        /// <summary>
        /// Lefts the inner join.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public SqlQuery LeftInnerJoin(TableSchema.Table from)
        {
            CreateJoin(from, Join.JoinType.LeftInner);
            return this;
        }

        /// <summary>
        /// Lefts the inner join.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public SqlQuery LeftInnerJoin(string tableName)
        {
            TableSchema.Table schema = DataService.GetSchema(tableName, provider.Name);
            CreateJoin(schema, Join.JoinType.LeftInner);
            return this;
        }

        /// <summary>
        /// Lefts the inner join.
        /// </summary>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery LeftInnerJoin(TableSchema.TableColumn fromColumn, TableSchema.TableColumn toColumn)
        {
            CreateJoin(fromColumn, toColumn, Join.JoinType.LeftInner);
            return this;
        }

        /// <summary>
        /// Lefts the inner join.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toTable">To table.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery LeftInnerJoin(string fromTable, string fromColumn, string toTable, string toColumn)
        {
            CreateJoin(fromTable, fromColumn, toTable, toColumn, Join.JoinType.LeftInner);
            return this;
        }

        #endregion


        #region RightInner

        /// <summary>
        /// Rights the inner join.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SqlQuery RightInnerJoin<T>() where T : RecordBase<T>, new()
        {
            CreateJoin(new T().GetSchema(), Join.JoinType.RightInner);
            return this;
        }

        /// <summary>
        /// Rights the inner join.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public SqlQuery RightInnerJoin(TableSchema.Table from)
        {
            CreateJoin(from, Join.JoinType.RightInner);
            return this;
        }

        /// <summary>
        /// Rights the inner join.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public SqlQuery RightInnerJoin(string tableName)
        {
            TableSchema.Table schema = DataService.GetSchema(tableName, provider.Name);
            CreateJoin(schema, Join.JoinType.RightInner);
            return this;
        }

        /// <summary>
        /// Rights the inner join.
        /// </summary>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery RightInnerJoin(TableSchema.TableColumn fromColumn, TableSchema.TableColumn toColumn)
        {
            CreateJoin(fromColumn, toColumn, Join.JoinType.RightInner);
            return this;
        }

        /// <summary>
        /// Rights the inner join.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toTable">To table.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery RightInnerJoin(string fromTable, string fromColumn, string toTable, string toColumn)
        {
            CreateJoin(fromTable, fromColumn, toTable, toColumn, Join.JoinType.RightInner);
            return this;
        }

        #endregion


        #region LeftOuter

        /// <summary>
        /// Lefts the outer join.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SqlQuery LeftOuterJoin<T>() where T : RecordBase<T>, new()
        {
            CreateJoin(new T().GetSchema(), Join.JoinType.LeftOuter);
            return this;
        }

        /// <summary>
        /// Lefts the outer join.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public SqlQuery LeftOuterJoin(TableSchema.Table from)
        {
            CreateJoin(from, Join.JoinType.LeftOuter);
            return this;
        }

        /// <summary>
        /// Lefts the outer join.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public SqlQuery LeftOuterJoin(string tableName)
        {
            TableSchema.Table schema = DataService.GetSchema(tableName, provider.Name);
            CreateJoin(schema, Join.JoinType.LeftOuter);
            return this;
        }

        /// <summary>
        /// Lefts the outer join.
        /// </summary>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery LeftOuterJoin(TableSchema.TableColumn fromColumn, TableSchema.TableColumn toColumn)
        {
            CreateJoin(fromColumn, toColumn, Join.JoinType.LeftOuter);
            return this;
        }

        /// <summary>
        /// Lefts the outer join.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toTable">To table.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery LeftOuterJoin(string fromTable, string fromColumn, string toTable, string toColumn)
        {
            CreateJoin(fromTable, fromColumn, toTable, toColumn, Join.JoinType.LeftOuter);
            return this;
        }

        #endregion


        #region RightOuter

        /// <summary>
        /// Rights the outer join.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SqlQuery RightOuterJoin<T>() where T : RecordBase<T>, new()
        {
            CreateJoin(new T().GetSchema(), Join.JoinType.RightOuter);
            return this;
        }

        /// <summary>
        /// Rights the outer join.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public SqlQuery RightOuterJoin(TableSchema.Table from)
        {
            CreateJoin(from, Join.JoinType.RightOuter);
            return this;
        }

        /// <summary>
        /// Rights the outer join.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public SqlQuery RightOuterJoin(string tableName)
        {
            TableSchema.Table schema = DataService.GetSchema(tableName, provider.Name);
            CreateJoin(schema, Join.JoinType.RightOuter);
            return this;
        }

        /// <summary>
        /// Rights the outer join.
        /// </summary>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery RightOuterJoin(TableSchema.TableColumn fromColumn, TableSchema.TableColumn toColumn)
        {
            CreateJoin(fromColumn, toColumn, Join.JoinType.RightOuter);
            return this;
        }

        /// <summary>
        /// Rights the outer join.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toTable">To table.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery RightOuterJoin(string fromTable, string fromColumn, string toTable, string toColumn)
        {
            CreateJoin(fromTable, fromColumn, toTable, toColumn, Join.JoinType.RightOuter);
            return this;
        }

        #endregion


        #region NotEqual

        /// <summary>
        /// Nots the equal join.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public SqlQuery NotEqualJoin<T>() where T : RecordBase<T>, new()
        {
            CreateJoin(new T().GetSchema(), Join.JoinType.NotEqual);
            return this;
        }

        /// <summary>
        /// Nots the equal join.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public SqlQuery NotEqualJoin(TableSchema.Table from)
        {
            CreateJoin(from, Join.JoinType.NotEqual);
            return this;
        }

        /// <summary>
        /// Nots the equal join.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public SqlQuery NotEqualJoin(string tableName)
        {
            TableSchema.Table schema = DataService.GetSchema(tableName, provider.Name);
            CreateJoin(schema, Join.JoinType.NotEqual);
            return this;
        }

        /// <summary>
        /// Nots the equal join.
        /// </summary>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery NotEqualJoin(TableSchema.TableColumn fromColumn, TableSchema.TableColumn toColumn)
        {
            CreateJoin(fromColumn, toColumn, Join.JoinType.NotEqual);
            return this;
        }

        /// <summary>
        /// Nots the equal join.
        /// </summary>
        /// <param name="fromTable">From table.</param>
        /// <param name="fromColumn">From column.</param>
        /// <param name="toTable">To table.</param>
        /// <param name="toColumn">To column.</param>
        /// <returns></returns>
        public SqlQuery NotEqualJoin(string fromTable, string fromColumn, string toTable, string toColumn)
        {
            CreateJoin(fromTable, fromColumn, toTable, toColumn, Join.JoinType.NotEqual);
            return this;
        }

        #endregion


        #endregion


        #region Ordering

        /// <summary>
        /// Orders the asc.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public SqlQuery OrderAsc(params string[] columns)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach(string s in columns)
            {
                OrderBySQ o = new OrderBySQ(s, OrderBySQ.OrderDirection.ASC);
                OrderBys.Add(o);
            }
            return this;
        }

        /// <summary>
        /// Orders the desc.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <returns></returns>
        public SqlQuery OrderDesc(params string[] columns)
        {
            StringBuilder sb = new StringBuilder();
            bool isFirst = true;
            foreach(string s in columns)
            {
                OrderBySQ o = new OrderBySQ(s, OrderBySQ.OrderDirection.DESC);
                OrderBys.Add(o);
            }
            return this;
        }

        #endregion


        #region Paging

        /// <summary>
        /// Pageds the specified current page.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns></returns>
        public SqlQuery Paged(int currentPage, int pageSize)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            return this;
        }

        /// <summary>
        /// Pageds the specified current page.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="idColumn">The id column.</param>
        /// <returns></returns>
        public SqlQuery Paged(int currentPage, int pageSize, string idColumn)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            IdColumn = idColumn;
            return this;
        }

        #endregion


        #region Execution

        /// <summary>
        /// Executes this instance.
        /// </summary>
        /// <returns></returns>
        public virtual int Execute()
        {
            int result;
            try
            {
                result = DataService.ExecuteQuery(BuildCommand());
            }
            catch(Exception x)
            {
                SqlQueryException ex = GenerateException(x);
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <returns></returns>
        public virtual object ExecuteScalar()
        {
            object result;
            try
            {
                result = DataService.ExecuteScalar(BuildCommand());
            }
            catch(Exception x)
            {
                SqlQueryException ex = GenerateException(x);
                throw ex;
            }
            return result;
        }

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns></returns>
        public virtual TResult ExecuteScalar<TResult>()
        {
            TResult result = default(TResult);

            try
            {
                object queryResult = DataService.ExecuteScalar(BuildCommand());

                if(queryResult != null && queryResult != DBNull.Value)
                {
                    result = (TResult)Utility.ChangeType(queryResult, typeof(TResult));
                    //result = (TResult)queryResult;
                }
            }
            catch(Exception x)
            {
                SqlQueryException ex = GenerateException(x);
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Gets the record count.
        /// </summary>
        /// <returns></returns>
        public virtual int GetRecordCount()
        {
            int count = 0;
            try
            {
                if (PageSize > 0 )
                {
                    using(IDataReader rdr = ExecuteReader())
                    {
                        while(rdr.Read())
                            count++;
                        rdr.Close();
                    }
                }
                else
                {
                    QueryCommand command = new QueryCommand(GetGenerator().GetCountSelect(), provider.Name);
                    SetConstraintParams(this, command);
                    object scalar = DataService.ExecuteScalar(command);
                    if (scalar != null)
                        count = Convert.ToInt32(scalar);
                }
            }
            catch (Exception x)
            {
                SqlQueryException ex = GenerateException(x);
                throw ex;
            }

            return count;
        }

        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <returns></returns>
        public virtual IDataReader ExecuteReader()
        {
            IDataReader rdr;
            try
            {
                rdr = DataService.GetReader(BuildCommand());
            }
            catch(Exception x)
            {
                SqlQueryException ex = GenerateException(x);
                throw ex;
            }
            return rdr;
        }

        /// <summary>
        /// Executes the query and returns the data as XML
        /// </summary>
        /// <returns></returns>
        public string ExecuteXML()
        {
            return ExecuteXML("Query", "Item");
        }

        /// <summary>
        /// Executes the query and returns the data as XML, using the supplied names
        /// </summary>
        /// <param name="resultSetName">The name of the root element</param>
        /// <param name="itemName">The name of the item element</param>
        /// <returns></returns>
        public string ExecuteXML(string resultSetName, string itemName)
        {
            //pull a dataset
            DataSet ds = ExecuteDataSet();

            string xml = ds.GetXml();

            //the way the query engine runs a data set
            //is that it wraps the outer bits using "<NewDataSet>"
            //strip that
            xml = xml.Replace("<NewDataSet>", String.Concat("<", resultSetName, ">")).Replace("</NewDataSet>", String.Concat("</", resultSetName, ">"));

            if(itemName != String.Empty)
            {
                //next, replace the <table> tag with the name of the table/sp passed in
                xml = xml.Replace("<Table>", String.Concat("<", itemName, ">")).Replace("</Table>", String.Concat("</", itemName, ">"));
            }
            else
                xml = xml.Replace("<Table>", "").Replace("</Table>", String.Empty);
            return xml;
        }

        /// <summary>
        /// Executes a query and returns the results as JSON
        /// </summary>
        /// <param name="resultSetName">The name of the Result set</param>
        /// <param name="itemName">Name of the item.</param>
        /// <returns></returns>
        public string ExecuteJSON(string resultSetName, string itemName)
        {
            //pull a dataset
            string xml = ExecuteXML(resultSetName, itemName);

            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(xml);

            //parse to JSON
            string json = XmlToJSONParser.XmlToJSON(xdoc);

            //clean up and prep for delivery
            json = json.Replace(@"\", @"\\");

            return json;
        }

        /// <summary>
        /// Executes the data set.
        /// </summary>
        /// <returns></returns>
        public virtual DataSet ExecuteDataSet()
        {
            DataSet ds;
            try
            {
                ds = DataService.GetDataSet(BuildCommand());
            }
            catch(Exception x)
            {
                SqlQueryException ex = GenerateException(x);
                throw ex;
            }
            return ds;
        }

        /// <summary>
        /// Returns a DataSet based on the passed-in command
        /// </summary>
        /// <returns></returns>
        public DataSet ExecuteJoinedDataSet()
        {
            return ExecuteJoinedDataSet<DataSet>();
        }

        /// <summary>
        /// Executes the joined data set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T ExecuteJoinedDataSet<T>() where T : DataSet, new()
        {
            if(FromTables.Count > 0)
            {
                TableSchema.Table table = FromTables[0];
                StringBuilder strSelect = new StringBuilder(SqlFragment.SELECT);
                string strFrom = table.QualifiedName;
                StringBuilder strJoin = new StringBuilder();
                for(int i = 0; i < table.Columns.Count; i++)
                {
                    string joinType = SqlFragment.INNER_JOIN;
                    StringBuilder col;
                    TableSchema.TableColumn tblCol = table.Columns[i];
                    if(tblCol.IsNullable)
                        joinType = SqlFragment.LEFT_JOIN;

                    if(i == 0 && tblCol.IsForeignKey && !String.IsNullOrEmpty(tblCol.ForeignKeyTableName) && Utility.IsMappingTable(table))
                    {
                        col = new StringBuilder(table.Provider.QualifyColumnName(table.SchemaName, table.Name, tblCol.ColumnName));
                        col.Append(SqlFragment.AS);
                        col.Append(String.Concat("PK", tblCol.ColumnName));
                        if(i + 1 != table.Columns.Count)
                            col.Append(", ");

                        strSelect.Append(col);
                    }

                    if(tblCol.IsForeignKey && !tblCol.IsPrimaryKey && !String.IsNullOrEmpty(tblCol.ForeignKeyTableName))
                    {
                        string strJoinPrefix = String.Concat(SqlFragment.JOIN_PREFIX, i);
                        TableSchema.Table fkTable = DataService.GetSchema(tblCol.ForeignKeyTableName, ProviderName, TableType.Table);
                        TableSchema.TableColumn displayCol = Utility.GetDisplayTableColumn(fkTable);

                        bool isSortable = Utility.GetEffectiveMaxLength(displayCol) < 256;
                        string dataCol = displayCol.ColumnName;
                        string selectCol = table.Provider.QualifyColumnName("", strJoinPrefix, dataCol);
                        col = new StringBuilder(selectCol);
						if (tblCol.Table.Provider.DatabaseRequiresBracketedJoins) { strFrom = "(" + strFrom; }
                        strJoin.Append(joinType);
                        strJoin.Append(fkTable.Provider.QualifyTableName(fkTable.SchemaName, fkTable.TableName));
                        strJoin.Append(SqlFragment.SPACE);
                        strJoin.Append(strJoinPrefix);
                        strJoin.Append(SqlFragment.ON);
                        string columnReference = tblCol.QualifiedName;
                        strJoin.Append(columnReference);
                        strJoin.Append(SqlFragment.EQUAL_TO);
                        string joinReference =  table.Provider.QualifyColumnName("", strJoinPrefix, fkTable.PrimaryKey.ColumnName);
                        strJoin.Append(joinReference);
						if (tblCol.Table.Provider.DatabaseRequiresBracketedJoins) { strJoin.Append(")"); }
						if (isSortable && OrderBys.Count > 0)
                        {
                            //for(int o = 0; o < OrderBys.Count; o++)
                            //    OrderBys[o] = OrderBys[o].Replace(columnReference, selectCol);
                            foreach (OrderBySQ ob in OrderBys)
                                if (Utility.StripSquareBrackets(ob.ColumnNameOrExpression) == columnReference) ob.ColumnNameOrExpression = selectCol;
                        }                    
                    }
                    else
                        col = new StringBuilder(table.Provider.QualifyColumnName("", table.Name, tblCol.ColumnName));
                    col.Append(SqlFragment.AS);
                    col.Append(tblCol.Table.Provider.FormatIdentifier(tblCol.ColumnName));

                    if(i + 1 != table.Columns.Count)
                        col.Append(", ");

                    strSelect.Append(col);
                }

                StringBuilder strSQL = new StringBuilder();
                strSQL.Append(strSelect);
				strSQL.Append(SqlFragment.FROM + strFrom);
                strSQL.Append(strJoin);

                ISqlGenerator generator = GetGenerator();
                strSQL.Append(generator.GenerateWhere());
                strSQL.Append(generator.GenerateOrderBy());

                QueryCommand qry = new QueryCommand(strSQL.ToString(), table.Provider.Name);

				SetConstraintParams(qry);
                //foreach(Where where in wheres)
                //    qry.AddParameter(where.ParameterName, where.ParameterValue, where.DbType);

                //foreach(BetweenAnd between in betweens)
                //{
                //    qry.AddParameter(between.StartParameterName, between.StartDate, DbType.DateTime);
                //    qry.AddParameter(between.EndParameterName, between.EndDate, DbType.DateTime);
                //}

                return DataService.GetDataSet<T>(qry);
            }
            return null;
        }

        internal static List<T> BuildTypedResult<T>(IDataReader rdr) where T : new()
        {
            List<T> result = new List<T>();
            Type iType = typeof(T);

            if(Utility.IsSubSonicType<T>())
            {
                //load it
                while(rdr.Read())
                {
                    T item = new T();
                    //set to RecordBase
                    IRecordBase arItem = (IRecordBase)item;

                    arItem.Load(rdr);
                    result.Add(item);
                }
            } //bferrier added this to get a list of values back if only selecting one column
            else if(iType.IsValueType)
            {
                while (rdr.Read())
                {
                    T item = new T();

                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        if (!DBNull.Value.Equals(rdr.GetValue(0)))
                            item = (T)rdr.GetValue(0);
                    }
                    result.Add(item);
                }
            }
            else
            {
                //coerce the values, using some light reflection            

                //cache property info so we're not banging on reflection in a long loop
                PropertyInfo[] cachedProps = new PropertyInfo[rdr.FieldCount];
                for(int i = 0; i < rdr.FieldCount; i++)
                    cachedProps[i] = iType.GetProperty(rdr.GetName(i), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                //set the values        
                PropertyInfo prop;
                while(rdr.Read())
                {
                    T item = new T();

                    for(int i = 0; i < rdr.FieldCount; i++)
                    {
                        prop = cachedProps[i];
                        if(prop != null && !DBNull.Value.Equals(rdr.GetValue(i)))
                            prop.SetValue(item, rdr.GetValue(i), null);
                    }
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// Executes the typed list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> ExecuteTypedList<T>() where T : new()
        {
            IDataReader rdr = null;

            try
            {
                rdr = ExecuteReader();
            }
            catch(Exception x)
            {
                if(rdr != null && !rdr.IsClosed)
                    rdr.Close();
                SqlQueryException ex = GenerateException(x);
                throw ex;
            }

            List<T> result = BuildTypedResult<T>(rdr);

            if(rdr != null && !rdr.IsClosed)
                rdr.Close();

            return result;
        }

        /// <summary>
        /// Executes as collection.
        /// </summary>
        /// <typeparam name="ListType">The type of the ist type.</typeparam>
        /// <returns></returns>
        public virtual ListType ExecuteAsCollection<ListType>()
            where ListType : IAbstractList, new()
        {
            ListType list = new ListType();
            try
            {
                IDataReader rdr = ExecuteReader();
                list.LoadAndCloseReader(rdr);
            }
            catch(Exception x)
            {
                SqlQueryException ex = GenerateException(x);
                throw ex;
            }

            return list;
        }

        /// <summary>
        /// Executes the query and returns the result as a single item of T
        /// </summary>
        /// <typeparam name="T">The type of item to return</typeparam>
        public virtual T ExecuteSingle<T>() where T : new()
        {
            T result = default(T);
            using(IDataReader rdr = ExecuteReader())
            {
                List<T> items = BuildTypedResult<T>(rdr);

                if(items.Count > 0)
                    result = items[0];
                if(!rdr.IsClosed)
                    rdr.Close();
            }
            return result;
        }

        #endregion


        #region Transactions

        /// <summary>
        /// Executes the transaction.
        /// </summary>
        /// <param name="queries">The queries.</param>
        public static void ExecuteTransaction(List<Insert> queries)
        {
			DataProvider p = DataService.Provider;
			if (queries.Count > 0) { p = queries[0].Provider;  }

            using(SharedDbConnectionScope scope = new SharedDbConnectionScope(p))
            {
                using(TransactionScope ts = new TransactionScope())
                {
                    foreach(Insert q in queries)
                        q.Execute();
                }
            }
        }

        /// <summary>
        /// Executes the transaction.
        /// </summary>
        /// <param name="queries">The queries.</param>
        /// <param name="provider">The provider.</param>
        public static void ExecuteTransaction(List<Insert> queries, DataProvider provider)
        {
            using(SharedDbConnectionScope scope = new SharedDbConnectionScope(provider))
            {
                using(TransactionScope ts = new TransactionScope())
                {
                    foreach(Insert q in queries)
                        q.Execute();
                }
            }
        }

        /// <summary>
        /// Executes the transaction.
        /// </summary>
        /// <param name="queries">The queries.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void ExecuteTransaction(List<Insert> queries, string connectionString)
        {
            using(SharedDbConnectionScope scope = new SharedDbConnectionScope(connectionString))
            {
                using(TransactionScope ts = new TransactionScope())
                {
                    foreach(Insert q in queries)
                        q.Execute();
                }
            }
        }

        /// <summary>
        /// Executes the transaction.
        /// </summary>
        /// <param name="queries">The queries.</param>
        public static void ExecuteTransaction(List<SqlQuery> queries)
        {
            using(SharedDbConnectionScope scope = new SharedDbConnectionScope())
            {
                using(TransactionScope ts = new TransactionScope())
                {
                    foreach(SqlQuery q in queries)
                        q.Execute();
                }
            }
        }

        /// <summary>
        /// Executes the transaction.
        /// </summary>
        /// <param name="queries">The queries.</param>
        /// <param name="provider">The provider.</param>
        public static void ExecuteTransaction(List<SqlQuery> queries, DataProvider provider)
        {
            using(SharedDbConnectionScope scope = new SharedDbConnectionScope(provider))
            {
                using(TransactionScope ts = new TransactionScope())
                {
                    foreach(SqlQuery q in queries)
                        q.Execute();
                }
            }
        }

        /// <summary>
        /// Executes the transaction.
        /// </summary>
        /// <param name="queries">The queries.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void ExecuteTransaction(List<SqlQuery> queries, string connectionString)
        {
            using(SharedDbConnectionScope scope = new SharedDbConnectionScope(connectionString))
            {
                using(TransactionScope ts = new TransactionScope())
                {
                    foreach(SqlQuery q in queries)
                        q.Execute();
                }
            }
        }

        /// <summary>
        /// Executes the transaction.
        /// </summary>
        /// <param name="queries">The queries.</param>
        /// <param name="provider">The provider.</param>
        /// <param name="connectionString">The connection string.</param>
        public static void ExecuteTransaction(List<SqlQuery> queries, DataProvider provider, string connectionString)
        {
            using(SharedDbConnectionScope scope = new SharedDbConnectionScope(provider, connectionString))
            {
                using(TransactionScope ts = new TransactionScope())
                {
                    foreach(SqlQuery q in queries)
                        q.Execute();
                }
            }
        }

        #endregion



        /// <summary>
        /// Gets or sets the select column list.
        /// </summary>
        /// <value>The select column list.</value>
        public string[] SelectColumnList
        {
            get { return _selectColumnList; }
            internal set { _selectColumnList = value; }
        }

        /// <summary>
        /// Gets or sets the SQL command.
        /// </summary>
        /// <value>The SQL command.</value>
        public string SQLCommand
        {
            get { return _sqlCommand; }
            internal set { _sqlCommand = value; }
        }

        /// <summary>
        /// Gets or sets the constraints.
        /// </summary>
        /// <value>The constraints.</value>
        public List<Constraint> Constraints
        {
            get { return _constraints; }
            internal set { _constraints = value; }
        }

        /// <summary>
        /// Gets or sets from tables.
        /// </summary>
        /// <value>From tables.</value>
        public TableSchema.TableCollection FromTables
        {
            get { return _fromTables; }
            internal set { _fromTables = value; }
        }

        /// <summary>
        /// Gets or sets the distinct spec.
        /// </summary>
        /// <value>The distinct spec.</value>
        public string DistinctSpec
        {
             get { 	return _distinctspec; }
             set { _distinctspec = value; }
        }

        /// <summary>
        /// Gets or sets the top spec.
        /// </summary>
        /// <value>The top spec.</value>
        public string TopSpec
        {
            get { return _topSpec; }
            internal set { _topSpec = value; }
        }

        /// <summary>
        /// Gets or sets the order bys.
        /// </summary>
        /// <value>The order bys.</value>
        public List<OrderBySQ> OrderBys
        {
            get { return _orderBys; }
            internal set { _orderBys = value; }
        }
        
        /// <summary>
        /// Gets or sets the expressions.
        /// </summary>
        /// <value>The expressions.</value>
        public List<string> Expressions
        {
            get { return _expressions; }
            internal set { _expressions = value; }
        }

        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; internal set; }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        /// <value>The current page.</value>
        public int CurrentPage { get; internal set; }

        /// <summary>
        /// Gets or sets the open paren count.
        /// </summary>
        /// <value>The open paren count.</value>
        public int OpenParenCount { get; internal set; }

        /// <summary>
        /// Gets or sets the closed paren count.
        /// </summary>
        /// <value>The closed paren count.</value>
        public int ClosedParenCount { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is distinct.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is distinct; otherwise, <c>false</c>.
        /// </value>
        public bool IsDistinct { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        /// <value>The provider.</value>
        public DataProvider Provider
        {
            get { return provider; }
            internal set
            {
                provider = value;
                ProviderName = provider.Name;
            }
        }

        internal DbType GetConstraintDbType(string tableName, string columnName, object constraintValue)
        {
            string providerTable = null;
            string providerColumn = null;

            if(!String.IsNullOrEmpty(tableName))
                providerTable = tableName;
            else if(FromTables.Count > 0)
                providerTable = FromTables[0].TableName;

            if(!String.IsNullOrEmpty(columnName))
                providerColumn = columnName;

            if(!String.IsNullOrEmpty(ProviderName) && !String.IsNullOrEmpty(providerTable) && !String.IsNullOrEmpty(providerColumn))
            {
                TableSchema.Table table = DataService.GetSchema(providerTable, ProviderName);
                if(table != null)
                {
                    TableSchema.TableColumn column = table.GetColumn(providerColumn);
                    if(column != null)
                        return column.DataType;
                }
            }
            return DbType.AnsiString;
        }

        /// <summary>
        /// Adjusts the where query if the affected table contains a logical delete column.
        /// </summary>
        public void CheckLogicalDelete()
        {
            foreach(Constraint c in Constraints)
            {
                if(Utility.IsLogicalDeleteColumn(c.ColumnName))
                    return;
            }
            foreach(TableSchema.Table table in FromTables)
            {
                foreach(TableSchema.TableColumn column in table.Columns)
                {
                    if(Utility.IsLogicalDeleteColumn(column.ColumnName))
                    {
                        LogicalDeleteColumns.Add(column);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Distincts this instance.
        /// </summary>
        /// <returns></returns>
        public SqlQuery Distinct()
        {
            DistinctSpec = SqlFragment.DISTINCT;
            IsDistinct = true;
            return this;
        }
    }
}
