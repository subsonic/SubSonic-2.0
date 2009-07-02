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
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class Join
    {
        #region JoinType enum

        /// <summary>
        /// 
        /// </summary>
        public enum JoinType
        {
            /// <summary>
            /// 
            /// </summary>
            Inner,
            /// <summary>
            /// 
            /// </summary>
            Outer,
            /// <summary>
            /// 
            /// </summary>
            LeftInner,
            /// <summary>
            /// 
            /// </summary>
            LeftOuter,
            /// <summary>
            /// 
            /// </summary>
            RightInner,
            /// <summary>
            /// 
            /// </summary>
            RightOuter,
            /// <summary>
            /// 
            /// </summary>
            Cross,
            /// <summary>
            /// 
            /// </summary>
            NotEqual
        }

        #endregion


        private TableSchema.TableColumn _fromColumn;
        private JoinType _joinType = JoinType.Inner;

        private TableSchema.TableColumn _toColumn;

        /// <summary>
        /// Initializes a new instance of the <see cref="Join"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="joinType">Type of the join.</param>
        public Join(TableSchema.TableColumn from, TableSchema.TableColumn to, JoinType joinType)
        {
            FromColumn = from;
            ToColumn = to;
            Type = joinType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Join"/> class.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="joinType">Type of the join.</param>
        public Join(TableSchema.Table from, TableSchema.Table to, JoinType joinType)
        {
            TableSchema.TableColumn fromCol = null;
            TableSchema.TableColumn toCol = null;

            foreach(TableSchema.TableColumn col in from.Columns)
            {
                if(col.IsForeignKey && !String.IsNullOrEmpty(col.ForeignKeyTableName))
                {
                    TableSchema.Table fkTable = col.Table.Provider.GetTableSchema(col.ForeignKeyTableName, col.Table.TableType);
                    if(Utility.IsMatch(fkTable.Name, to.Name))
                    {
                        fromCol = col;
                        //found it - use the PK
                        toCol = fkTable.PrimaryKey;
                        break;
                    }
                }
            }

            //reverse it - just in case we can't find a match
            if(fromCol == null || toCol == null)
            {
                foreach(TableSchema.TableColumn col in to.Columns)
                {
                    if(col.IsForeignKey && !String.IsNullOrEmpty(col.ForeignKeyTableName))
                    {
                        TableSchema.Table fkTable = col.Table.Provider.GetTableSchema(col.ForeignKeyTableName, col.Table.TableType);
                        if(Utility.IsMatch(fkTable.Name, from.Name))
                        {
                            toCol = col;
                            //found it - use the PK
                            fromCol = fkTable.PrimaryKey;
                            break;
                        }
                    }
                }
            }

            //if that fails, see if there is a matching column name
            if(fromCol == null || toCol == null)
            {
                //first, try to match the PK on the from table
                //to a column in the "to" table
                if(to.Columns.Contains(from.PrimaryKey.ColumnName))
                {
                    FromColumn = from.PrimaryKey;
                    ToColumn = to.GetColumn(from.PrimaryKey.ColumnName);
                    //if that doesn't work, see if the PK of the "to" table has a
                    //matching column in the "from" table
                }
                else if(from.Columns.Contains(to.PrimaryKey.ColumnName))
                {
                    FromColumn = from.GetColumn(to.PrimaryKey.ColumnName);
                    ToColumn = to.PrimaryKey;
                }
            }

            //if that fails - run a match on any column that matches in "from" to "to"
            if(fromCol == null || toCol == null)
            {
                foreach(TableSchema.TableColumn col in from.Columns)
                {
                    if(to.Columns.Contains(col.ColumnName))
                    {
                        toCol = to.GetColumn(col.ColumnName);
                        fromCol = col;
                        break;
                    }
                }
            }

            //still null? this seems exhausting, but the good thing is that these are indexed loops
            //so they execute fast :)
            if(fromCol == null || toCol == null)
            {
                foreach(TableSchema.TableColumn col in to.Columns)
                {
                    if(from.Columns.Contains(col.ColumnName))
                    {
                        fromCol = from.GetColumn(col.ColumnName);
                        toCol = col;
                        break;
                    }
                }
            }

            //if it's still null, throw since the join can't be made
            //and that's a failure of this method
            if(fromCol == null || toCol == null)
            {
                throw new SqlQueryException("Can't create a join for " + from.TableName + " to " + to.TableName +
                                            " - can't determine the columns to link on. Try specifying the columns (using their schema) or specifying the table/column pair");
            }
            FromColumn = fromCol;
            ToColumn = toCol;
            _joinType = joinType;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public JoinType Type
        {
            get { return _joinType; }
            set { _joinType = value; }
        }

        /// <summary>
        /// Gets or sets from column.
        /// </summary>
        /// <value>From column.</value>
        public TableSchema.TableColumn FromColumn
        {
            get { return _fromColumn; }
            set { _fromColumn = value; }
        }

        /// <summary>
        /// Gets or sets to column.
        /// </summary>
        /// <value>To column.</value>
        public TableSchema.TableColumn ToColumn
        {
            get { return _toColumn; }
            set { _toColumn = value; }
        }

        /// <summary>
        /// Gets the join type value.
        /// </summary>
        /// <param name="j">The j.</param>
        /// <returns></returns>
        public static string GetJoinTypeValue(JoinType j)
        {
            string result = SqlFragment.INNER_JOIN;
            switch(j)
            {
                case JoinType.Outer:
                    result = SqlFragment.OUTER_JOIN;
                    break;
                case JoinType.LeftInner:
                    result = SqlFragment.LEFT_INNER_JOIN;
                    break;
                case JoinType.LeftOuter:
                    result = SqlFragment.LEFT_OUTER_JOIN;
                    break;
                case JoinType.RightInner:
                    result = SqlFragment.RIGHT_INNER_JOIN;
                    break;
                case JoinType.RightOuter:
                    result = SqlFragment.RIGHT_OUTER_JOIN;
                    break;
                case JoinType.Cross:
                    result = SqlFragment.CROSS_JOIN;
                    break;
                case JoinType.NotEqual:
                    result = SqlFragment.UNEQUAL_JOIN;
                    break;
            }
            return result;
        }
    }
}