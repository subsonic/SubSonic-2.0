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
using System.IO;

namespace SubSonic.Sugar
{
    /// <summary>
    /// Summary for the Files class
    /// </summary>
    public static class Files
    {
        /// <summary>
        /// Read a text file and obtain it's contents.
        /// </summary>
        /// <param name="absolutePath">The complete file path to write to.</param>
        /// <returns>String containing the content of the file.</returns>
        public static string GetFileText(string absolutePath)
        {
            using(StreamReader sr = new StreamReader(absolutePath))
                return sr.ReadToEnd();
        }

        /// <summary>
        /// Creates or opens a file for writing and writes text to it.
        /// </summary>
        /// <param name="absolutePath">The complete file path to write to.</param>
        /// <param name="fileText">A String containing text to be written to the file.</param>
        public static void CreateToFile(string absolutePath, string fileText)
        {
            using(StreamWriter sw = File.CreateText(absolutePath))
                sw.Write(fileText);
        }

        /// <summary>
        /// Update text within a file by replacing a substring within the file.
        /// </summary>
        /// <param name="absolutePath">The complete file path to write to.</param>
        /// <param name="lookFor">A String to be replaced.</param>
        /// <param name="replaceWith">A String to replace all occurrences of lookFor.</param>
        public static void UpdateFileText(string absolutePath, string lookFor, string replaceWith)
        {
            string newText = GetFileText(absolutePath).Replace(lookFor, replaceWith);
            WriteToFile(absolutePath, newText);
        }

        /// <summary>
        /// Writes out a string to a file.
        /// </summary>
        /// <param name="absolutePath">The complete file path to write to.</param>
        /// <param name="fileText">A String containing text to be written to the file.</param>
        public static void WriteToFile(string absolutePath, string fileText)
        {
            using(StreamWriter sw = new StreamWriter(absolutePath, false))
                sw.Write(fileText);
        }

        /// <summary>
        /// Display bytes as a suitable unit of measurement.
        /// </summary>
        /// <remarks>
        /// Uses 1024^x for the units as used by memory and file sizing. Do not use in
        /// telecommunications where 1000^x is the norm.
        /// </remarks>
        /// <param name="bytes">Number of bytes to display.</param>
        /// <returns>A String formatted with the number of bytes and suitable unit of measurement.</returns>
        public static string FromByteCount(long bytes)
        {
            const decimal kilobyte = 1024m;
            const decimal megabyte = kilobyte * 1024;
            const decimal gigabyte = megabyte * 1024;

            if(bytes > gigabyte)
                return String.Format("{0:0.00} GB", bytes / gigabyte);
            if(bytes > megabyte)
                return String.Format("{0:0.00} MB", bytes / megabyte);
            if(bytes > kilobyte)
                return String.Format("{0:0.00} KB", bytes / kilobyte);
            return String.Format("{0} B", bytes);
        }
    }
}