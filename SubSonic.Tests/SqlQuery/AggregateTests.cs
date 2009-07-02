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
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests.SqlQuery
{
    [TestFixture]
    public class AggregateTests
    {
        [Test]
        public void Exec_AggregateExpression()
        {
            double result = new
                Select(Aggregate.Sum("UnitPrice*Quantity", "ProductSales"))
                .From(OrderDetail.Schema)
                .ExecuteScalar<double>();

            result = Math.Round(result, 2);
            Assert.IsTrue(result == 1354458.59);
        }

        [Test]
        public void Exec_AggregateAvg()
        {
            const double expected = 55.5922;

            // overload #1
            double result = new
                Select(Aggregate.Avg("UnitPrice"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #2
            result = new
                Select(Aggregate.Avg(Product.UnitPriceColumn))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #3
            result = new
                Select(Aggregate.Avg("UnitPrice", "AverageUnitPrice"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #4
            result = new
                Select(Aggregate.Avg(Product.UnitPriceColumn, "AverageUnitPrice"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Exec_AggregateMax()
        {
            const double expected = 100.00;

            // overload #1
            double result = new
                Select(Aggregate.Max("UnitPrice"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #2
            result = new
                Select(Aggregate.Max(Product.UnitPriceColumn))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #3
            result = new
                Select(Aggregate.Max("UnitPrice", "MostExpensive"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #4
            result = new
                Select(Aggregate.Max(Product.UnitPriceColumn, "MostExpensive"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Exec_AggregateMin()
        {
            const double expected = 2.50;

            // overload #1
            double result = new
                Select(Aggregate.Min("UnitPrice"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #2
            result = new
                Select(Aggregate.Min(Product.UnitPriceColumn))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #3
            result = new
                Select(Aggregate.Min("UnitPrice", "CheapestProduct"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #4
            result = new
                Select(Aggregate.Min(Product.UnitPriceColumn, "CheapestProduct"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Exec_AggregateStandardDeviation()
        {
            const double expected = 42.7698669325723;

            // overload #1
            double result = new
                Select(Aggregate.StandardDeviation("UnitPrice"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #2
            result = new
                Select(Aggregate.StandardDeviation(Product.UnitPriceColumn))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #3
            result = new
                Select(Aggregate.StandardDeviation("UnitPrice", "CheapestProduct"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);

            // overload #4
            result = new
                Select(Aggregate.StandardDeviation(Product.UnitPriceColumn, "CheapestProduct"))
                .From(Product.Schema)
                .ExecuteScalar<double>();
            Assert.AreEqual(expected, result);
        }

        [Test]
        public void Exec_SingleAggregateWithWhere()
        {
            int records = new
                Select(Aggregate.GroupBy("ProductID"), Aggregate.Avg("UnitPrice"))
                .From("Order Details")
                .Where(Aggregate.Avg("UnitPrice"))
                .IsGreaterThan(50)
                .GetRecordCount();

            Assert.AreEqual(7, records);
        }

        [Test]
        public void Exec_AggregateWithWhereAndHaving()
        {
            int records = new
                Select(Aggregate.GroupBy("ProductID"), Aggregate.Avg("UnitPrice"))
                .From("Order Details")
                .Where("Quantity").IsEqualTo(120)
                .Where(Aggregate.Avg("UnitPrice"))
                .IsGreaterThan(10)
                .GetRecordCount();

            Assert.AreEqual(5, records);
        }

        [Test]
        public void Exec_AggregateWithWhereNotHaving()
        {
            int records = new
            Select(Aggregate.GroupBy("ProductID"), Aggregate.Avg("UnitPrice"))
            .From("Order Details")
            .Where("Quantity").IsEqualTo(120)
            .GetRecordCount();

            Assert.AreEqual(7, records);
        }
    }
}