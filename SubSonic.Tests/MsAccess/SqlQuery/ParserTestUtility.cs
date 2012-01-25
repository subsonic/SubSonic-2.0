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

using SubSonic.Sugar;

namespace SubSonic.Tests.MsAccess.SqlQuery
{
    public static class ParserTestUtility
    {
        public static string PrepSql(string sql)
        {
            string result = sql;
            result = result.Replace("\r\n", " ");
            result = Strings.Squeeze(result);
            result = result.ToLower();

            return result;
        }
    }
}