﻿/*
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

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderBySQ
    {
        /// <summary>
        /// 
        /// </summary>
        public enum OrderDirection
        {
            /// <summary>
            /// 
            /// </summary>
            ASC,
            /// <summary>
            /// 
            /// </summary>
            DESC
        }

        /// <summary>
        /// Gets the order direction SQL fragment.
        /// </summary>
        /// <param name="j">The j.</param>
        /// <returns></returns>
        public static string GetOrderDirectionValue(OrderDirection o)
        {
            string result = SqlFragment.ASC;
            switch (o) {
                case OrderDirection.ASC:
                    result = SqlFragment.ASC;
                    break;
                case OrderDirection.DESC:
                    result = SqlFragment.DESC;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Reverses an 'Order By' direction.
        /// </summary>
        /// <param name="j">The j.</param>
        /// <returns></returns>
        public static OrderDirection ReverseDirection(OrderDirection o)
        {
            if (o == OrderDirection.ASC)
                return  OrderDirection.DESC;
            else
                return  OrderDirection.ASC; 
        }

        private OrderDirection _orderDirection = OrderDirection.ASC;

        /// <summary>
        /// Gets or sets the 'Order By' direction.
        /// </summary>
        /// <value>The direction (ASC or DESC).</value>
        public OrderDirection Direction
        {
            get { return _orderDirection; }
            set { _orderDirection = value; }
        }

        private string _columnNameOrExpression;

        /// <summary>
        /// Gets or sets the 'Order By' column name or expression.
        /// </summary>
        /// <value>From column.</value>
        public string ColumnNameOrExpression
        {
            get { return _columnNameOrExpression; }
            set { _columnNameOrExpression = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBy"/> class.
        /// </summary>
        /// <param name="from">Column Name or Expression.</param>
        /// <param name="to">Order By Direction.</param>
        public OrderBySQ(string columnOrExpression, OrderDirection orderDirection)
        {
            Direction = orderDirection;
            ColumnNameOrExpression = columnOrExpression;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderBy"/> class.
        /// </summary>
        /// <param name="from">Column Name or Expression.</param>
        /// <param name="to">Order By Direction.</param>
        public OrderBySQ(TableSchema.TableColumn column, OrderDirection orderDirection)
        {
            Direction = orderDirection;
            ColumnNameOrExpression = column.ColumnName;
        }

    }
}
