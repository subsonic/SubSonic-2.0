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
using System.Globalization;
using MbUnit.Framework;
using SubSonic.Sugar;

namespace SubSonic.Tests
{
    /// <summary>
    /// Summary for the SugarTests class
    /// </summary>
    [TestFixture]
    public class SugarTests
    {
        /// <summary>
        /// Dates_s the days ago.
        /// </summary>
        [Test]
        public void Dates_DaysAgo()
        {
            TimeSpan t = new TimeSpan(30, 0, 0, 0);
            DateTime dtVerify = DateTime.Now.Subtract(t);

            DateTime dt = Dates.DaysAgo(30);
            Assert.IsTrue(dtVerify.Date == dt.Date);
        }

        /// <summary>
        /// Numbers_s the is numeric.
        /// </summary>
        [Test]
        public void Numbers_IsNumeric()
        {
            string separator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            string number1 = String.Format("12{0}3", separator);
            string number2 = String.Format("3{0}2", separator);
            string number3 = String.Format("3{0}40040400404", separator);
            const string number4 = "193293993939939";
            const string number5 = "Phil's a geek";

            Assert.IsTrue(Numbers.IsNumber(number1));
            Assert.IsTrue(Numbers.IsNumber(number2));
            Assert.IsTrue(Numbers.IsNumber(number3));
            Assert.IsTrue(Numbers.IsNumber(number4));
            Assert.IsFalse(Numbers.IsNumber(number5));
        }

        /// <summary>
        /// Numbers_s the is integer.
        /// </summary>
        [Test]
        public void Numbers_IsInteger()
        {
            string separator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;
            string number1 = String.Format("12{0}3", separator);
            const string number2 = "123";
            const string number3 = "Phil's a geek";

            Assert.IsFalse(Numbers.IsInteger(number1));
            Assert.IsTrue(Numbers.IsInteger(number2));
            Assert.IsFalse(Numbers.IsInteger(number3));
        }

        /// <summary>
        /// Strings_s the is alpha.
        /// </summary>
        [Test]
        public void Strings_IsAlpha()
        {
            const string s1 = "hi there, I'm $#$@! sm rob!";
            const string s2 = "hithereImrob";
            const string s3 = "hi there, I'm 50";
            const string s4 = "ha ha?";

            Assert.IsFalse(Validation.IsAlpha(s1));
            Assert.IsTrue(Validation.IsAlpha(s2));
            Assert.IsFalse(Validation.IsAlpha(s3));
            Assert.IsFalse(Validation.IsAlpha(s4));
        }

        // The MbUnit v3 data-driven testing features have been consolidated so the [RowTest] 
        // attribute is no longer necessary. Just use the [Test] attribute instead
        /// <summary>
        /// Dates_s the readable diff.
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="endTime">The end time.</param>
        /// <param name="expected">The expected.</param>
        [Row("01 Jan 2007", "01 Jan 2007", "0 seconds ago")]
        [Row("01 Jan 2007", "01 Jan 2007 00:00:01", "1 second ago")]
        [Row("01 Jan 2007", "01 Jan 2007 00:00:05", "5 seconds ago")]
        [Row("01 Jan 2007", "01 Jan 2007 00:01:00", "1 minute ago")]
        [Row("01 Jan 2007", "01 Jan 2007 00:15:00", "15 minutes ago")]
        [Row("01 Jan 2007", "01 Jan 2007 01:00:00", "1 hour ago")]
        [Row("01 Jan 2007", "01 Jan 2007 01:01:0", "1 hour, 1 minute ago")]
        [Row("01 Jan 2007", "01 Jan 2007 01:02:44", "1 hour, 2 minutes ago")]
        [Row("01 Jan 2007", "01 Jan 2007 02:40:00", "2 hours, 40 minutes ago")]
        [Row("01 Jan 2007", "02 Jan 2007 02:30:40", "1 day, 2 hours ago")]
        [Row("01 Jan 2007", "12 Jan 2007 01:30:40", "11 days, 1 hour ago")]
        [Row("01 Jan 2007", "01 Feb 2007 01:30:40", "1 month ago")]
        [Row("01 Jan 2007", "02 Feb 2007 01:30:40", "1 month, 1 day ago")]
        [Row("01 Jan 2007", "12 Feb 2007 01:30:40", "1 month, 11 days ago")]
        [Row("01 Jan 2007", "01 Jan 2008 01:30:40", "1 year ago")]
        [Row("01 Jan 2007", "01 Jan 2207 01:30:40", "200 years ago")]
        [Row("01 Jan 2007", "01 May 2207 01:30:40", "200 years, 4 months ago")]
        [Test]
        public void Dates_ReadableDiff(DateTime startTime, DateTime endTime, string expected)
        {
            Assert.AreEqual(expected, Dates.ReadableDiff(startTime, endTime));
        }

        /// <summary>
        /// Test IsCreditCardAny validation.  Test numbers pulled from this website:
        /// http://www.ameripayment.com/testcreditcard.htm
        /// </summary>
        /// <param name="creditCardNumber">The credit card number.</param>
        /// <param name="expected">Expected test result.</param>
        [Row("5105105105105100", true)] // MasterCard
        [Row("5555555555554444", true)] // MasterCard
        [Row("4222222222222", true)] // MasterCard
        [Row("4111111111111111", true)] // VISA
        [Row("4012888888881881", true)] // VISA
        [Row("378282246310005", true)] // American Express
        [Row("371449635398431", true)] // American Express
        [Row("378734493671000", true)] // Amex Corporate
        [Row("38520000023237", true)] // Dinners Club
        [Row("30569309025904", true)] // Dinners Club
        [Row("6011111111111117", true)] // Discover
        [Row("6011000990139424", true)] // Discover
        [Row("3530111333300000", true)] // JCB
        [Row("3566002020360505", true)] // JCB
        [Row("4111-1111-1111-1111", true)] // VISA
        [Row("5431-1111-1111-1111", true)] // MasterCard
        [Row("341-1111-1111-1111", true)] // Amex
        [Row("6011-6011-6011-6611", true)] // Discover
        [Row("6011-6011-6011", false)]
        [Row("0000000000000000", false)]
        [Row("000000000000000", false)]
        [Row("0000000000000", false)]
        [Test]
        public void Validation_IsCreditCardAny(string creditCardNumber, bool expected)
        {
            Assert.AreEqual(expected, Validation.IsCreditCardAny(creditCardNumber));
        }
    }
}