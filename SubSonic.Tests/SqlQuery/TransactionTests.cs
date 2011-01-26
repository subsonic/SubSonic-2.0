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
using MbUnit.Framework;
using Northwind;

namespace SubSonic.Tests.SqlQuery
{
    [TestFixture]
    public class TransactionTests
    {
        [Test]
        [Rollback]
        public void Transaction_Simple()
        {
            List<Insert> queries = new List<Insert>();
            queries.Add(new Insert().Into(Region.Schema).Values("test1"));
            queries.Add(new Insert().Into(Region.Schema).Values("test2"));
            queries.Add(new Insert().Into(Region.Schema).Values("test3"));
            queries.Add(new Insert().Into(Region.Schema).Values("test4"));
            queries.Add(new Insert().Into(Region.Schema).Values("test5"));
            queries.Add(new Insert().Into(Region.Schema).Values("test6"));
            queries.Add(new Insert().Into(Region.Schema).Values("test7"));

            //execute in a transaction
            SubSonic.SqlQuery.ExecuteTransaction(queries);
        }
    }
}