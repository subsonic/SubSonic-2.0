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
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SubSonic.Utilities;

namespace SubSonic.Sugar
{
    /// <summary>
    /// Summary for the Web class
    /// </summary>
    public static class Web
    {
        /// <summary>
        /// Whether or not the request originated from the local network, or more specifically from localhost or a NAT address.
        /// This property is only accurate if NAT addresses are a valid indicators of a request being from within the internal network.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is local network request; otherwise, <c>false</c>.
        /// </value>
        public static bool IsLocalNetworkRequest
        {
            get
            {
                if(HttpContext.Current != null)
                {
                    if(HttpContext.Current.Request.IsLocal)
                        return true;

                    string hostPrefix = HttpContext.Current.Request.UserHostAddress;
                    string[] ipClass = hostPrefix.Split(new char[] {'.'});
                    int classA = Convert.ToInt16(ipClass[0]);
                    int classB = Convert.ToInt16(ipClass[1]);

                    if(classA == 10 || classA == 127)
                        return true;
                    if(classA == 192 && classB == 168)
                        return true;
                    return classA == 172 && (classB > 15 && classB < 33);
                }
                return false;
            }
        }

        /// <summary>
        /// Queries the string.
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public static t QueryString<t>(string param)
        {
            t result = default(t);

            if(HttpContext.Current.Request.QueryString[param] != null)
            {
                object paramValue = HttpContext.Current.Request[param];
                result = (t)Utility.ChangeType(paramValue, typeof(t));
            }

            return result;
        }

        /// <summary>
        /// Cookies the specified param.
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public static t Cookie<t>(string param)
        {
            t result = default(t);
            HttpCookie cookie = HttpContext.Current.Request.Cookies[param];
            if(cookie != null)
            {
                string paramValue = cookie.Value;
                result = (t)Utility.ChangeType(paramValue, typeof(t));
            }

            return result;
        }

        /// <summary>
        /// Sessions the value.
        /// </summary>
        /// <typeparam name="t"></typeparam>
        /// <param name="param">The param.</param>
        /// <returns></returns>
        public static t SessionValue<t>(string param)
        {
            t result = default(t);
            if(HttpContext.Current.Session[param] != null)
            {
                object paramValue = HttpContext.Current.Session[param];
                result = (t)Utility.ChangeType(paramValue, typeof(t));
            }

            return result;
        }

        //many thanks to ASP Alliance for the code below
        //http://authors.aspalliance.com/olson/methods/

        /// <summary>
        /// Fetches a web page
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string ReadWebPage(string url)
        {
            string webPage;
            WebRequest request = WebRequest.Create(url);
            using(Stream stream = request.GetResponse().GetResponseStream())
            {
                StreamReader sr = new StreamReader(stream);
                webPage = sr.ReadToEnd();
                sr.Close();
            }
            return webPage;
        }

        /// <summary>
        /// Gets DNS information about a url and puts it in an array of strings
        /// </summary>
        /// <param name="url">either an ip address or a host name</param>
        /// <returns>
        /// a list with the host name, all the aliases, and all the addresses.
        /// </returns>
        public static string[] DNSLookup(string url)
        {
            ArrayList al = new ArrayList();

            //check whether url is ipaddress or hostname
            IPHostEntry ipEntry = Dns.GetHostEntry(url);

            al.Add("HostName," + ipEntry.HostName);

            foreach(string name in ipEntry.Aliases)
                al.Add("Aliases," + name);

            foreach(IPAddress ip in ipEntry.AddressList)
                al.Add("Address," + ip);

            string[] ipInfo = (string[])al.ToArray(typeof(string));

            return ipInfo;
        }

        /// <summary>
        /// Return the images links in a given URL as an array of strings.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="returnWithTag">if set to <c>true</c> the string are returned as an XHTML compliant img tag; otherwise returns a list of img URLs only</param>
        /// <returns>string array of all images on a page</returns>
        public static string[] ScrapeImages(string url, bool returnWithTag)
        {
            //get the content of the url
            //ReadWebPage is another method in this useful methods collection
            string htmlPage = ReadWebPage(url);

            //set up the regex for finding images
            StringBuilder imgPattern = new StringBuilder();
            imgPattern.Append("<img[^>]+"); //start 'img' tag
            imgPattern.Append("src\\s*=\\s*"); //start src property
            //three possibilities  for what src property --
            //(1) enclosed in double quotes
            //(2) enclosed in single quotes
            //(3) enclosed in spaces
            imgPattern.Append("(?:\"(?<src>[^\"]*)\"|'(?<src>[^']*)'|(?<src>[^\"'>\\s]+))");
            imgPattern.Append("[^>]*>"); //end of tag
            Regex imgRegex = new Regex(imgPattern.ToString(), RegexOptions.IgnoreCase);

            //look for matches 
            Match imgcheck = imgRegex.Match(htmlPage);
            ArrayList imagelist = new ArrayList();
            //add base href for relative urls
            imagelist.Add("<BASE href=\"" + url + "\">" + url);

            while(imgcheck.Success)
            {
                string src = imgcheck.Groups["src"].Value;
                string image = returnWithTag ? "<img src=\"" + src + "\" alt=\"\" />" : src;
                imagelist.Add(image);
                imgcheck = imgcheck.NextMatch();
            }

            string[] images = new string[imagelist.Count];
            imagelist.CopyTo(images);

            return images;
        }

        /// <summary>
        /// Return the images links in a given URL as an array of XHTML-compliant img tags.
        /// </summary>
        /// <param name="url">The URL to extract the images from.</param>
        /// <returns></returns>
        public static string[] ScrapeImages(string url)
        {
            return ScrapeImages(url, true);
        }

        /// <summary>
        /// Scrapes a web page and parses out all the links.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="makeLinkable">if set to <c>true</c> [make linkable].</param>
        /// <returns></returns>
        public static string[] ScrapeLinks(string url, bool makeLinkable)
        {
            //get the content of the url
            //ReadWebPage is another method in this useful methods collection
            string htmlPage = ReadWebPage(url);

            //set up the regex for finding the link urls
            StringBuilder hrefPattern = new StringBuilder();
            hrefPattern.Append("<a[^>]+"); //start 'a' tag and anything that comes before 'href' tag
            hrefPattern.Append("href\\s*=\\s*"); //start href property
            //three possibilities  for what href property --
            //(1) enclosed in double quotes
            //(2) enclosed in single quotes
            //(3) enclosed in spaces
            hrefPattern.Append("(?:\"(?<href>[^\"]*)\"|'(?<href>[^']*)'|(?<href>[^\"'>\\s]+))");
            hrefPattern.Append("[^>]*>.*?</a>"); //end of 'a' tag
            Regex hrefRegex = new Regex(hrefPattern.ToString(), RegexOptions.IgnoreCase);

            //look for matches 
            Match hrefcheck = hrefRegex.Match(htmlPage);
            ArrayList linklist = new ArrayList();
            //add base href for relative links
            linklist.Add("<BASE href=\"" + url + "\">" + url);
            while(hrefcheck.Success)
            {
                string href = hrefcheck.Groups["href"].Value; //link url
                string link = (makeLinkable)
                                  ? "<a href=\"" + href + "\" target=\"_blank\">" + href + "</a>"
                                  : href;
                linklist.Add(link);
                hrefcheck = hrefcheck.NextMatch();
            }
            string[] links = new string[linklist.Count];
            linklist.CopyTo(links);

            return links;
        }

        /// <summary>
        /// Calls the Gravatar service to and returns an HTML <img></img> tag for use on your pages.
        /// </summary>
        /// <param name="email">The email of the user</param>
        /// <param name="size">The size of the Gravatar image - 60 is standard</param>
        /// <returns>HTML image tag</returns>
        public static string GetGravatar(string email, int size)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = md5.ComputeHash(Encoding.ASCII.GetBytes(email));

            StringBuilder hash = new StringBuilder();
            for(int i = 0; i < result.Length; i++)
                hash.Append(result[i].ToString("x2"));

            StringBuilder image = new StringBuilder();
            image.Append("<img src=\"");
            image.Append("http://www.gravatar.com/avatar.php?");
            image.Append("gravatar_id=" + hash);
            image.Append("&amp;rating=G");
            image.Append("&amp;size=" + size);
            image.Append("&amp;default=");
            image.Append(HttpContext.Current.Server.UrlEncode("http://example.com/noavatar.gif"));
            image.Append("\" alt=\"\" />");
            return image.ToString();
        }

        /// <summary>
        /// Given a valid email address, returns a short javascript block that will emit a valid
        /// mailto: link that can't be picked up by address harvesters. Call this method where you
        /// would normally place the link in your html code.
        /// </summary>
        /// <param name="emailText">The email address to convert to spam-free format</param>
        /// <returns></returns>
        public static string CreateSpamFreeEmailLink(string emailText)
        {
            if(!String.IsNullOrEmpty(emailText))
            {
                string[] parts = emailText.Split(new char[] {'@'});
                if(parts.Length == 2)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<script type='text/javascript'>");
                    sb.Append("var m = '" + parts[0] + "';");
                    sb.Append("var a = '" + parts[1] + "';");
                    sb.Append("var l = '" + emailText + "';");
                    sb.Append("document.write('<a href=\"mailto:' + m + '@' + a + '\">' + l + '</a>');");
                    sb.Append("</script>");
                    return sb.ToString();
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// A simple utility to output Lorem Ipsum text to your page.
        /// </summary>
        /// <param name="count">int count, the number of either paragraphs, words, or characters you would like to display.</param>
        /// <param name="method">string method, 'p' for paragraphs or 'w' for words or 'c'c for characters</param>
        /// <returns>
        /// string with the resulting lorem ipsum text
        /// </returns>
        public static string GenerateLoremIpsum(int count, string method)
        {
            Random r = new Random();
            if(method.ToLower() == "p" && count > 1000)
                throw new ArgumentOutOfRangeException("count", "Sorry, lorem ipsum control only allows 1000 or less paragraphs.");

            string loremIpsum = LoadTextFromManifest("loremIpsum.txt");

            if(null == loremIpsum)
                throw new Exception("Could not load loremipsum.txt");

            StringBuilder sb = new StringBuilder();

            if(String.IsNullOrEmpty(method) || method.ToLower() == "p" || (method.ToLower() != "p" && method.ToLower() != "c" && method.ToLower() != "w"))
            {
                char[] split = {'|'};
                string[] paras = loremIpsum.Split(split);

                ArrayList paraList = new ArrayList();
                //need a nonfixed array
                foreach(string s in paras)
                    paraList.Add(s);

                int parasLength = paras.Length;
                if(count > parasLength)
                {
                    int needmore = count - parasLength;
                    for(int x = 0; x < needmore; x++)
                    {
                        int pickme = r.Next(0, parasLength - 1);
                        paraList.Add(paras[pickme]);
                    }
                }

                for(int i = 0; i < count; i++)
                {
                    sb.Append("<p>");
                    sb.Append(paraList[i]);
                    sb.Append("</p>");
                }
            }

            if(method.ToLower() == "c")
            {
                int loremLength = loremIpsum.Length;

                if(count > loremLength)
                {
                    int needmore = (count / loremLength);

                    for(int x = 0; x < needmore; x++)
                    {
                        string newLoremIpsum = loremIpsum;
                        loremIpsum += " ";
                        loremIpsum += newLoremIpsum;
                    }
                }
                //note: I don't catch the exception here
                sb.Append(Utility.ShortenText(loremIpsum, count));
            }

            if(method.ToLower() == "w")
            {
                string[] words = loremIpsum.Split(' ');
                ArrayList wordList = new ArrayList();
                //need a nonfixed array
                foreach(string s in words)
                    wordList.Add(s);

                int wordLength = words.Length;
                if(count > wordLength)
                {
                    int needmore = count - wordLength;
                    for(int x = 0; x < needmore; x++)
                    {
                        int pickme = r.Next(0, wordLength - 1);
                        wordList.Add(words[pickme]);
                    }
                }

                for(int i = 0; i < count; i++)
                {
                    sb.Append(wordList[i]);
                    sb.Append(" ");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Loads the text from manifest.
        /// </summary>
        /// <param name="templateFileName">Name of the template file.</param>
        /// <returns></returns>
        private static string LoadTextFromManifest(string templateFileName)
        {
            string templateText = null;
            Assembly asm = Assembly.GetExecutingAssembly();
            using(Stream stream = asm.GetManifestResourceStream("SubSonic.Sugar." + templateFileName))
            {
                if(stream != null)
                {
                    StreamReader sReader = new StreamReader(stream);
                    templateText = sReader.ReadToEnd();
                    sReader.Close();
                }
            }
            return templateText;
        }
    }
}