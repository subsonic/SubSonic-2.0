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
using System.Text;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class Select : SqlQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Select"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="columns">The columns.</param>
        public Select(DataProvider provider, params string[] columns)
        {
            Provider = provider;
            ProviderName = provider.Name;
            SelectColumnList = columns;
            SQLCommand = SqlFragment.SELECT;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Select"/> class.
        /// </summary>
        public Select() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="Select"/> class.
        /// </summary>
        /// <param name="aggregates">The aggregates.</param>
        public Select(params Aggregate[] aggregates)
        {
            foreach(Aggregate agg in aggregates)
                Aggregates.Add(agg);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Select"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        /// <param name="aggregates">The aggregates.</param>
        public Select(DataProvider provider, params Aggregate[] aggregates)
        {
            Provider = provider;
            ProviderName = provider.Name;
            foreach(Aggregate agg in aggregates)
                Aggregates.Add(agg);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Select"/> class.
        /// </summary>
        /// <param name="columns">The columns.</param>
        public Select(params TableSchema.TableColumn[] columns)
        {
            if(columns.Length > 0)
            {
                Provider = columns[0].Table.Provider;
                ProviderName = columns[0].Table.Provider.Name;
                SQLCommand = SqlFragment.SELECT;

                SelectColumnList = new string[columns.Length];
                for (int i = 0; i < columns.Length; i++)
                    SelectColumnList[i] = columns[i].QualifiedName;

                //user entered an array
                //StringBuilder sb = new StringBuilder();
                //foreach(TableSchema.TableColumn col in columns)
                //    sb.AppendFormat("{0}|", col.QualifiedName);

                //SelectColumnList = sb.ToString().Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Select"/> class.
        /// WARNING: This overload should only be used with applications that use a single provider!
        /// </summary>
        /// <param name="columns">The columns.</param>
        public Select(params string[] columns)
        {
            SQLCommand = SqlFragment.SELECT;
            if(columns.Length == 1 && columns[0].Contains(","))
            {
                //user entered a single string column list: "col1, col2, col3"
                SelectColumnList = columns[0].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < SelectColumnList.Length; i++)
                    SelectColumnList[i] = SelectColumnList[i].Trim();
            }
            else
            {
                //user entered an array
                SelectColumnList = columns;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Select"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public Select(DataProvider provider)
        {
            Provider = provider;
            ProviderName = provider.Name;
            SQLCommand = SqlFragment.SELECT;
        }

        /// <summary>
        /// Alls the columns from.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Select AllColumnsFrom<T>() where T : RecordBase<T>, new()
        {
            T item = new T();
            TableSchema.Table tbl = item.GetSchema();
            Select s = new Select(tbl.Provider);
            s.FromTables.Add(tbl);
            return s;
        }

        /// <summary>
        /// Expressions the specified SQL expression.
        /// </summary>
        /// <param name="sqlExpression">The SQL expression.</param>
        /// <returns></returns>
        public Select Expression(string sqlExpression)
        {
            Expressions.Add(sqlExpression);
            return this;
        }

        /// <summary>
        /// Tops the specified top.
        /// </summary>
        /// <param name="top">The top.</param>
        /// <returns></returns>
        public Select Top(string top)
        {
            if(!top.ToLower().Trim().Contains("top"))
                top = String.Format(" TOP {0} ", top);
            TopSpec = top;
            return this;
        }

        /// <summary>
        /// Distincts this instance.
        /// </summary>
        /// <returns></returns>
        public Select Distinct()
        {
            DistinctSpec = SqlFragment.DISTINCT;
            IsDistinct = true;
            return this;
        }


    }
}