using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Northwind;
using SubSonic;

namespace LoadTester {
    class Program {
        static void Main(string[] args) {
            //LoadOrderCollections();
            LoadOrders();
            /*
            Console.WriteLine(DateTime.Now.ToString());
            DateTime dtStart = DateTime.Now;

            //create an Order
            //let's save a meeeeellllionnn items shall we?
            for (int i = 1; i < 1000000; i++) {
                Console.WriteLine("Creating order " + i.ToString());
                Order o = new Order();
                o.CustomerID = "ALFKI";
                o.EmployeeID = 5;
                o.Freight = 10;
                o.OrderDate = DateTime.Now;
                o.RequiredDate = DateTime.Now;
                o.ShipAddress = "Somwhere, Someday";
                o.ShipCity = "City";
                o.ShipCountry = "US";
                o.ShipName = "Chris Cyvas";
                o.ShippedDate = DateTime.Now;
                o.ShipPostalCode = "99999";
                o.ShipRegion = "KS";

                o.Save("me");

                OrderDetail detail = new OrderDetail();
                detail.OrderID = o.OrderID;
                detail.ProductID = 13;
                detail.Quantity = 1;
                detail.UnitPrice = 100;
                detail.Save("me");
            }
            DateTime dtEnd = DateTime.Now;
            Console.WriteLine("Done!" );
            Console.WriteLine("Started on " + dtStart.ToString());
            Console.WriteLine("Ended on "+dtEnd.ToString());
            Console.Read();
             * */
        }

       static  void LoadOrders() {
            
            //this is a record we just inserted above
            DateTime dtStart = DateTime.Now;
            for (int i = 10248; i < 1010248; i++) {
                Order o = new Order(i);
                Console.WriteLine("Hello from Order " + i.ToString());

            }
            DateTime dtEnd = DateTime.Now;
            Console.WriteLine("Done!");
            Console.WriteLine("Started on " + dtStart.ToString());
            Console.WriteLine("Ended on " + dtEnd.ToString());
            Console.Read();
        }

       static void LoadOrderCollections() {

           //Collection Loading test with 10 records
           DateTime dtStart = DateTime.Now;
           for (int i = 10248; i < 1010000; i++) {
               int nextTen=i+10;
               OrderCollection coll = new Select().From<Order>().Where("orderid")
                   .IsBetweenAnd(i, nextTen).ExecuteAsCollection<OrderCollection>();
               Console.WriteLine("Hello from Orders " + i.ToString()+" - "+nextTen);


           }
           DateTime dtEnd = DateTime.Now;
           Console.WriteLine("Done!");
           Console.WriteLine("Started on " + dtStart.ToString());
           Console.WriteLine("Ended on " + dtEnd.ToString());
           Console.Read();
       }

    }
}
