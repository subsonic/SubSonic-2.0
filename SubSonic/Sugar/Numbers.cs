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
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace SubSonic.Sugar
{
    /// <summary>
    /// Summary for the Numbers class
    /// </summary>
    public static class Numbers
    {
        /// <summary>
        /// Determines whether [is natural number] [the specified s item].
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if [is natural number] [the specified s item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNaturalNumber(string sItem)
        {
            Regex notNaturalPattern = new Regex("[^0-9]");
            Regex naturalPattern = new Regex("0*[1-9][0-9]*");

            return !notNaturalPattern.IsMatch(sItem) && naturalPattern.IsMatch(sItem);
        }

        /// <summary>
        /// Determines whether [is whole number] [the specified s item].
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if [is whole number] [the specified s item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsWholeNumber(string sItem)
        {
            Regex notWholePattern = new Regex("[^0-9]");
            return !notWholePattern.IsMatch(sItem);
        }

        /// <summary>
        /// Determines whether the specified s item is integer.
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if the specified s item is integer; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInteger(string sItem)
        {
            Regex notIntPattern = new Regex("[^0-9-]");
            Regex intPattern = new Regex("^-[0-9]+$|^[0-9]+$");

            return !notIntPattern.IsMatch(sItem) && intPattern.IsMatch(sItem);
        }

        /// <summary>
        /// Determines whether the specified s item is number.
        /// </summary>
        /// <param name="sItem">The s item.</param>
        /// <returns>
        /// 	<c>true</c> if the specified s item is number; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNumber(string sItem)
        {
            double result;
            return (double.TryParse(sItem, NumberStyles.Float, NumberFormatInfo.CurrentInfo, out result));
        }

        /// <summary>
        /// Determines whether the specified value is an even number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is even; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsEven(int value)
        {
            return ((value & 1) == 0);
        }

        /// <summary>
        /// Determines whether the specified value is an odd number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is odd; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsOdd(int value)
        {
            return ((value & 1) == 1);
        }

        /// <summary>
        /// Generates a random number
        /// </summary>
        /// <param name="noZeros"></param>
        /// <returns></returns>
        public static int Random(bool noZeros)
        {
            byte[] random = new Byte[1];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            if(noZeros)
                rng.GetNonZeroBytes(random);
            else
                rng.GetBytes(random);

            return random[0];
        }

        /// <summary>
        /// Generates a random number with an upper bound
        /// </summary>
        /// <param name="high">The high.</param>
        /// <returns></returns>
        public static int Random(int high)
        {
            byte[] random = new Byte[4];
            new RNGCryptoServiceProvider().GetBytes(random);
            int randomNumber = BitConverter.ToInt32(random, 0);

            return Math.Abs(randomNumber % high);
        }

        /// <summary>
        /// Generates a random number between the specified bounds
        /// </summary>
        /// <param name="low">The low.</param>
        /// <param name="high">The high.</param>
        /// <returns></returns>
        public static int Random(int low, int high)
        {
            return new Random().Next(low, high);
        }

        /// <summary>
        /// Generates a random double
        /// </summary>
        /// <returns></returns>
        public static double Random()
        {
            return new Random().NextDouble();
        }
    }
}