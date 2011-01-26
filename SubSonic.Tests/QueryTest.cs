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
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests
{
    /// <summary>
    /// Summary for the QueryTest class
    /// </summary>
    [TestFixture]
    public class QueryTest
    {
        /// <summary>
        /// Query_s the select.
        /// </summary>
        [Test]
        public void Query_Select()
        {
            Query qry = new Query(Product.Schema);
            qry.AddWhere("productID", 1);
            int pk = (int)qry.ExecuteScalar();

            Assert.IsTrue(pk == 1, "Bad Select");
        }

        /// <summary>
        /// Query_s the select top.
        /// </summary>
        [Test]
        public void Query_SelectTop()
        {
            Where LikeUnitTestTerritory = new Where();
            LikeUnitTestTerritory.TableName = Territory.Schema.TableName;
            LikeUnitTestTerritory.ColumnName = Territory.Columns.TerritoryDescription;
            LikeUnitTestTerritory.Comparison = Comparison.Like;
            LikeUnitTestTerritory.ParameterValue = "%ville%";

            Query qry = new Query(Territory.Schema);
            qry.Top = "3";
            qry.AddWhere(LikeUnitTestTerritory);
            int counter = 0;

            using(IDataReader rdr = qry.ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.AreEqual(3, counter, "Count is " + counter);
        }

        /// <summary>
        /// Query_s the updates.
        /// </summary>
        [Test]
        [Rollback]
        public void Query_Updates()
        {
            Query qry = new Query(Product.Schema);
            qry.AddUpdateSetting("Discontinued", true);
            qry.AddWhere(Product.Columns.ProductName, "Unit Test Product 3");
            qry.Execute();

            //verify
            qry = new Query(Product.Schema);
            qry.AddWhere(Product.Columns.ProductName, "Unit Test Product 3");

            ProductCollection coll = new ProductCollection();

            using(IDataReader rdr = qry.ExecuteReader())
            {
                coll.Load(rdr);
                rdr.Close();
            }
            foreach(Product prod in coll)
                Assert.IsTrue(prod.Discontinued);
        }

        /// <summary>
        /// Query_s the between and.
        /// </summary>
        [Test]
        public void Query_BetweenAnd()
        {
            int counter = 0;
            using(
                IDataReader rdr =
                    new Query(DataService.GetTableSchema("Orders", DataService.Provider.Name)).AddBetweenAnd("OrderDate", new DateTime(1996, 7, 4), new DateTime(1996, 7, 16)).
                        ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter == 10, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the view.
        /// </summary>
        [Test]
        public void Query_View()
        {
            int counter = 0;
            using(
                IDataReader rdr = new Query(DataService.GetTableSchema("Invoices", DataService.Provider.Name)).AddWhere("ShipPostalCode", "51100").ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter == 10, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the between numbers.
        /// </summary>
        [Test]
        public void Query_BetweenNumbers()
        {
            int counter = 0;

            using(IDataReader rdr = new Query(Product.Schema).AddBetweenValues("productID", 1, 7).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter == 7, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the I n_ object array.
        /// </summary>
        [Test]
        public void Query_IN_ObjectArray()
        {
            int counter = 0;

            using(IDataReader rdr = new Query("products").IN("ProductID", new object[] {1, 2, 3, 4, 5}).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter == 5, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the I n_ list collection.
        /// </summary>
        [Test]
        public void Query_IN_ListCollection()
        {
            ListItemCollection coll = new ListItemCollection();
            for(int i = 1; i <= 5; i++)
            {
                ListItem item = new ListItem(i.ToString(), i.ToString());
                item.Selected = true;
                coll.Add(item);
            }

            int counter = 0;
            using(IDataReader rdr = new Query("products").IN("ProductID", coll).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter == 5, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the I n_ array list.
        /// </summary>
        [Test]
        public void Query_IN_ArrayList()
        {
            ArrayList list = new ArrayList();
            for(int i = 1; i <= 5; i++)
                list.Add(i);

            int counter = 0;
            using(IDataReader rdr = new Query("products").IN("ProductID", list).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter == 5, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the Not In_ object array.
        /// </summary>
        [Test]
        public void Query_NOT_IN_ObjectArray()
        {
            int counter = 0;

            using(IDataReader rdr = new Query("products").NOT_IN("ProductID", new object[] {1, 2, 3, 4, 5}).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter == 72, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the NOT In_ list collection.
        /// </summary>
        [Test]
        public void Query_NOT_IN_ListCollection()
        {
            ListItemCollection coll = new ListItemCollection();
            for(int i = 1; i <= 5; i++)
            {
                ListItem item = new ListItem(i.ToString(), i.ToString());
                item.Selected = true;
                coll.Add(item);
            }

            int counter = 0;
            using(IDataReader rdr = new Query("products").NOT_IN("ProductID", coll).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter == 72, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the Not In_ array list.
        /// </summary>
        [Test]
        public void Query_NOT_IN_ArrayList()
        {
            ArrayList list = new ArrayList();
            for(int i = 1; i <= 5; i++)
                list.Add(i);

            int counter = 0;
            using(IDataReader rdr = new Query("products").NOT_IN("ProductID", list).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            Assert.IsTrue(counter == 72, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the is not null.
        /// </summary>
        [Test]
        public void Query_IsNotNull()
        {
            int counter = 0;
            using(IDataReader rdr = new Query("Products").AddWhere("ProductID", Comparison.IsNot, null).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            //should bring back all records
            Assert.AreEqual(new Query(Product.Schema).GetCount(Product.Columns.ProductID), counter, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the O r_ simple.
        /// </summary>
        [Test]
        public void Query_OR_Simple()
        {
            int counter = 0;
            using(IDataReader rdr = new Query("Categories").WHERE("CategoryID", 5).OR("CategoryID", 1).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            //should bring back all records
            Assert.IsTrue(counter == 2, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the O r_ moderate.
        /// </summary>
        [Test]
        public void Query_OR_Moderate()
        {
            int counter = 0;
            using(
                IDataReader rdr =
                    new Query("Products").WHERE("CategoryID", 5).AND("UnitPrice", Comparison.GreaterThan, 50).OR("CategoryID", 1).AND("UnitPrice", Comparison.GreaterThan, 50).
                        ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
            }

            Assert.AreEqual(5, counter, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the O r_ moderate with expressions.
        /// </summary>
        [Test]
        public void Query_OR_ModerateWithExpressions()
        {
            int counter = 0;
            using(
                IDataReader rdr =
                    new Query("Products").WHERE("CategoryID = 5").AND("UnitPrice > 50").OR("CategoryID = 1").AND(
                        "UnitPrice > 50").ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
            }
            Assert.AreEqual(5, counter, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the O r_ between.
        /// </summary>
        [Test]
        public void Query_OR_Between()
        {
            int counter = 0;
            string[] sFormat = {"M/d/yyyy"};

            using(IDataReader rdr =
                new Query("Orders").BETWEEN_AND("OrderDate",
                    DateTime.ParseExact("7/4/1996", sFormat, CultureInfo.CurrentCulture, DateTimeStyles.None),
                    DateTime.ParseExact("7/10/1996", sFormat, CultureInfo.CurrentCulture, DateTimeStyles.None)).OR_BETWEEN_AND("OrderDate",
                        DateTime.ParseExact("7/14/1996", sFormat, CultureInfo.CurrentCulture, DateTimeStyles.None),
                        DateTime.ParseExact("7/20/1996", sFormat, CultureInfo.CurrentCulture, DateTimeStyles.None)).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }

            //should bring back all records
            Assert.IsTrue(counter == 12, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the expression.
        /// </summary>
        [Test]
        public void Query_Expression()
        {
            int counter = 0;
            using(IDataReader rdr = new Query("Products", "Northwind").WHERE("ProductID < 5").ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            //should bring back all records
            Assert.IsTrue(counter == 4, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the paging test_ table.
        /// </summary>
        [Test]
        public void Query_PagingTest_Table()
        {
            Query q = new Query("Products", "Northwind");
            q.PageSize = 10;
            q.PageIndex = 1;
            int counter = 0;

            using(IDataReader rdr = q.ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            //should bring back all records
            Assert.IsTrue(counter == 10, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the paging test_ view.
        /// </summary>
        [Test]
        public void Query_PagingTest_View()
        {
            Query q = new Query("Sales By Category", "Northwind");
            q.PageSize = 10;
            q.PageIndex = 1;
            q.ORDER_BY("CategoryID", "ASC");
            int counter = 0;
            using(IDataReader rdr = q.ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
                rdr.Close();
            }
            //should bring back all records
            Assert.IsTrue(counter == 10, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the paging test_ view.
        /// </summary>
        [Test]
        public void Query_JoinedDataSet()
        {
            Query q = new Query("Products", "Northwind");
            DataSet ds = q.ExecuteJoinedDataSet();

            //should bring back all records
            Assert.IsTrue(ds.Tables[0].Columns["SupplierID"].DataType == typeof(string));
        }

        [Test]
        public void ExecuteJoinedDataSet_Should_Accept_Parameters() {
            int someProduct = 1;
            SubSonic.SqlQuery sq = new Select()
                 .From(Product.Schema)
                 .Where(Product.ProductIDColumn).IsEqualTo(someProduct);

            DataSet ds = sq.ExecuteJoinedDataSet();
            Assert.AreEqual(1, ds.Tables[0].Rows.Count);
        }


        /// <summary>
        /// Query_s the constraint expression.
        /// </summary>
        [Test]
        public void Query_ConstraintExpression()
        {
            int counter = 0;
            using(IDataReader rdr = new Query(Product.Schema.TableName, "Northwind").WHERE(Product.Columns.ProductID, Is.LessThan(5)).ExecuteReader())
            {
                while(rdr.Read())
                    counter++;
            }

            Assert.AreEqual(4, counter, "Nope - it's " + counter);
        }

        /// <summary>
        /// Test Order by FK on Joined Tables.
        /// </summary>
        [Test]
        public void Query_JoinedDataSet_OrderByFK()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");
            q.OrderBy = OrderBy.Desc(ts.GetColumn("CategoryID"));
            DataSet ds = q.ExecuteJoinedDataSet();

            //should bring 10 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(10));
        }

        /// <summary>
        /// Query_s the between and over ExecuteJoinedDataSet
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_AddBetweenAnd()
        {
            DataSet ds = new Query(DataService.GetTableSchema("Orders", DataService.Provider.Name))
                .AddBetweenAnd("OrderDate", new DateTime(1996, 7, 4), new DateTime(1996, 7, 16)).
                ExecuteJoinedDataSet();
            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 10, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s the between and over ExecuteJoinedDataSet with more than one Where Condition
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_AddBetweenAnd_WithExtraWhereCondition()
        {
            DataSet ds = new Query(DataService.GetTableSchema("Orders", DataService.Provider.Name)).
                AddWhere("OrderID", Comparison.GreaterThan, 0).
                AddBetweenAnd("OrderDate", new DateTime(1996, 7, 4), new DateTime(1996, 7, 16)).
                ExecuteJoinedDataSet();
            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 10, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s with BETWEEN_AND clause over ExecuteJoinedDataSet
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_BETWEEN_AND()
        {
            DataSet ds = new Query(DataService.GetTableSchema("Orders", DataService.Provider.Name))
                .BETWEEN_AND("OrderDate", new DateTime(1996, 7, 4), new DateTime(1996, 7, 16)).
                ExecuteJoinedDataSet();

            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 10, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s with BETWEEN_AND over ExecuteJoinedDataSet with and extra Where Condition
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_BETWEEN_AND_WithExtraWhereCondition()
        {
            DataSet ds =
                new Query(DataService.GetTableSchema("Orders", DataService.Provider.Name)).
                    AddWhere("OrderID", Comparison.GreaterThan, 0).
                    BETWEEN_AND("OrderDate", new DateTime(1996, 7, 4), new DateTime(1996, 7, 16)).
                    ExecuteJoinedDataSet();

            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 10, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s ExecuteJoinedDataSet In_ object array.
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_IN_ObjectArray()
        {
            DataSet ds = new Query("products").IN("ProductID", new object[] {1, 2, 3, 4, 5}).ExecuteJoinedDataSet();
            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 5, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s ExecuteJoinedDataSet the In_ list collection.
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_IN_ListCollection()
        {
            ListItemCollection coll = new ListItemCollection();
            for(int i = 1; i <= 5; i++)
            {
                ListItem item = new ListItem(i.ToString(), i.ToString());
                item.Selected = true;
                coll.Add(item);
            }

            DataSet ds = new Query("products").IN("ProductID", coll).ExecuteJoinedDataSet();
            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 5, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s ExecuteJoinedDataSet In_ array list.
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_IN_ArrayList()
        {
            ArrayList list = new ArrayList();
            for(int i = 1; i <= 5; i++)
                list.Add(i);

            DataSet ds = new Query("products").IN("ProductID", list).ExecuteJoinedDataSet();
            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 5, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s ExecuteJoinedDataSet NOT_In_ object array.
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_NOT_IN_ObjectArray()
        {
            DataSet ds = new Query("products").NOT_IN("ProductID", new object[] {1, 2, 3, 4, 5}).ExecuteJoinedDataSet();
            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 72, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s ExecuteJoinedDataSet the NOT In_ list collection.
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_NOT_IN_ListCollection()
        {
            ListItemCollection coll = new ListItemCollection();
            for(int i = 1; i <= 5; i++)
            {
                ListItem item = new ListItem(i.ToString(), i.ToString());
                item.Selected = true;
                coll.Add(item);
            }

            DataSet ds = new Query("products").NOT_IN("ProductID", coll).ExecuteJoinedDataSet();
            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 72, "Nope - it's " + counter);
        }

        /// <summary>
        /// Query_s ExecuteJoinedDataSet NOT_In_ array list.
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_NOT_IN_ArrayList()
        {
            ArrayList list = new ArrayList();
            for(int i = 1; i <= 5; i++)
                list.Add(i);

            DataSet ds = new Query("products").NOT_IN("ProductID", list).ExecuteJoinedDataSet();
            int counter = ds.Tables[0].Rows.Count;
            Assert.IsTrue(counter == 72, "Nope - it's " + counter);
        }

        /// <summary>
        /// Test JoinedDataSet Order by Collection
        /// </summary>
        [Test]
        public void Query_JoinedDataSet_OrderByCollection()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");
            q.OrderByCollection.Add(OrderBy.Desc(ts.GetColumn("CategoryID")));
            q.OrderByCollection.Add(OrderBy.Desc(ts.GetColumn("ProductID")));

            DataSet ds = q.ExecuteJoinedDataSet();

            //should bring 73 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(73));
        }

        /// <summary>
        /// Test Order by Collection
        /// </summary>
        [Test]
        public void Query_OrderByCollection()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");
            q.OrderByCollection.Add(OrderBy.Desc(ts.GetColumn("CategoryID")));
            q.OrderByCollection.Add(OrderBy.Desc(ts.GetColumn("ProductID")));

            DataSet ds = q.ExecuteDataSet();

            //should bring 73 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(73));
        }

        /// <summary>
        /// Test Order by 
        /// </summary>
        [Test]
        public void Query_OrderBy()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");
            q.OrderBy = OrderBy.Desc(ts.GetColumn("ProductID"));

            DataSet ds = q.ExecuteDataSet();

            //should bring 77 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(77));
        }

        /// <summary>
        /// Test Query_ORDER_BY
        /// </summary>
        [Test]
        public void Query_ORDER_BY()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");

            DataSet ds = q.ORDER_BY(ts.GetColumn("ProductID")).ExecuteDataSet();

            //should bring 1 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(1));
        }

        /// <summary>
        /// Test Query_ORDER_BY_DESC
        /// </summary>
        [Test]
        public void Query_ORDER_BY_DESC()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");

            DataSet ds = q.ORDER_BY(ts.GetColumn("ProductID"), SqlFragment.DESC).ExecuteDataSet();

            //should bring 77 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(77));
        }

        /// <summary>
        /// Test Query_ORDER_BY_Expression
        /// </summary>
        [Test]
        public void Query_ORDER_BY_Expression()
        {
            Query q = new Query("Products", "Northwind");

            DataSet ds = q.ORDER_BY("ProductID DESC").ExecuteDataSet();
            //should bring 77 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(77));
        }

        /// <summary>
        /// Test Query_ORDER_BY_ExpressionDESC
        /// </summary>
        [Test]
        public void Query_ORDER_BY_ExpressionDESC()
        {
            Query q = new Query("Products", "Northwind");

            DataSet ds = q.ORDER_BY("ProductID", SqlFragment.DESC).ExecuteDataSet();
            //should bring 77 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(77));
        }

        /// <summary>
        /// Test QueryExecuteJoinedDataSet_ORDER_BY
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_ORDER_BY()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");

            DataSet ds = q.ORDER_BY(ts.GetColumn("ProductID")).ExecuteJoinedDataSet();

            //should bring 1 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(1));
        }

        /// <summary>
        /// Test QueryExecuteJoinedDataSet_ORDER_BY_DESC
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_ORDER_BY_DESC()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");

            DataSet ds = q.ORDER_BY(ts.GetColumn("ProductID"), SqlFragment.DESC).ExecuteJoinedDataSet();

            //should bring 77 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(77));
        }

        /// <summary>
        /// Test QueryExecuteJoinedDataSet_ORDER_BY_Expression
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_ORDER_BY_Expression()
        {
            Query q = new Query("Products", "Northwind");

            DataSet ds = q.ORDER_BY("ProductID DESC").ExecuteJoinedDataSet();
            //should bring 77 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(77));
        }

        /// <summary>
        /// Test QueryExecuteJoinedDataSet_ORDER_BY_ExpressionDESC
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_ORDER_BY_ExpressionDESC()
        {
            Query q = new Query("Products", "Northwind");

            DataSet ds = q.ORDER_BY("ProductID", SqlFragment.DESC).ExecuteJoinedDataSet();
            //should bring 77 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(77));
        }

        /// <summary>
        /// Test Query_ORDER_BY_Collection
        /// </summary>
        [Test]
        public void Query_ORDER_BY_Collection()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");

            DataSet ds = q.ORDER_BY(ts.GetColumn("SupplierID")).
                ORDER_BY(ts.GetColumn("ProductID")).
                ExecuteDataSet();

            //should bring 2 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(2));
        }

        /// <summary>
        /// Test Query_ORDER_BY_DESC_Collection
        /// </summary>
        [Test]
        public void Query_ORDER_BY_DESC_Collection()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");

            DataSet ds = q.ORDER_BY(ts.GetColumn("CategoryID"), SqlFragment.DESC).
                ORDER_BY(ts.GetColumn("ProductID"), SqlFragment.DESC).
                ExecuteDataSet();

            //should bring 73 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(73));
        }

        /// <summary>
        /// Test Query_ORDER_BY_Expression_Collection
        /// </summary>
        [Test]
        public void Query_ORDER_BY_Expression_Collection()
        {
            Query q = new Query("Products", "Northwind");

            DataSet ds = q.ORDER_BY("CategoryID DESC").
                ORDER_BY("ProductID DESC").
                ExecuteDataSet();

            //should bring 73 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(73));
        }

        /// <summary>
        /// Test Query_ORDER_BY_ExpressionDESC_Collection
        /// </summary>
        [Test]
        public void Query_ORDER_BY_ExpressionDESC_Collection()
        {
            Query q = new Query("Products", "Northwind");

            DataSet ds = q.ORDER_BY("CategoryID", SqlFragment.DESC).
                ORDER_BY("ProductID", SqlFragment.DESC).
                ExecuteDataSet();

            //should bring 73 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(73));
        }

        /// <summary>
        /// Test QueryExecuteJoinedDataSet_ORDER_BY_Collection
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_ORDER_BY_Collection()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");

            DataSet ds = q.ORDER_BY(ts.GetColumn("SupplierID")).
                ORDER_BY(ts.GetColumn("ProductID")).
                ExecuteJoinedDataSet();

            //should bring 38 as first (SupplierID is replaced by CompanyName)
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(38));
        }

        /// <summary>
        /// Test QueryExecuteJoinedDataSet_ORDER_BY_DESC_Collection
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_ORDER_BY_DESC_Collection()
        {
            Query q = new Query("Products", "Northwind");
            TableSchema.Table ts = DataService.GetTableSchema("Products", "Northwind");

            DataSet ds = q.ORDER_BY(ts.GetColumn("SupplierID"), SqlFragment.DESC).
                ORDER_BY(ts.GetColumn("ProductID"), SqlFragment.DESC).
                ExecuteJoinedDataSet();

            //should bring 48 as first (SupplierID is replaced by CompanyName)
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(48));
        }

        /// <summary>
        /// Test QueryExecuteJoinedDataSet_ORDER_BY_Expression_Collection
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_ORDER_BY_Expression_Collection()
        {
            Query q = new Query("Products", "Northwind");

            DataSet ds = q.ORDER_BY("SupplierID DESC").
                ORDER_BY("ProductID DESC").
                ExecuteJoinedDataSet();

            //should bring 48 as first 
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(48));
        }

        /// <summary>
        /// Test QueryExecuteJoinedDataSet_ORDER_BY_ExpressionDESC_Collection
        /// </summary>
        [Test]
        public void QueryExecuteJoinedDataSet_ORDER_BY_ExpressionDESC_Collection()
        {
            Query q = new Query("Products", "Northwind");

            DataSet ds = q.ORDER_BY("SupplierID", SqlFragment.DESC).
                ORDER_BY("ProductID", SqlFragment.DESC).
                ExecuteJoinedDataSet();

            //should bring 48 as first
            Assert.IsTrue(ds.Tables[0].Rows[0]["ProductID"].Equals(48));
        }
    }
}