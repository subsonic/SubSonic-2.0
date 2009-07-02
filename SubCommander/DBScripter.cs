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

using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace SubSonic.SubCommander
{
    /// <summary>
    /// 
    /// </summary>
    public class DBScripter
    {
        /// <summary>
        /// Scripts the data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public static string ScriptData(string tableName, string providerName)
        {
            return DataService.ScriptData(tableName, providerName);
        }

        /// <summary>
        /// Scripts the schema.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static string ScriptSchema(string connectionString)
        {
            StringBuilder result = new StringBuilder();

            SqlConnection conn = new SqlConnection(connectionString);
            SqlConnectionStringBuilder cString = new SqlConnectionStringBuilder(connectionString);
            ServerConnection sconn = new ServerConnection(conn);
            Server server = new Server(sconn);
            Database db = server.Databases[cString.InitialCatalog];
            Transfer trans = new Transfer(db);

            //set the objects to copy
            trans.CopyAllTables = true;
            trans.CopyAllDefaults = true;
            trans.CopyAllUserDefinedFunctions = true;
            trans.CopyAllStoredProcedures = true;
            trans.CopyAllViews = true;
            trans.CopyData = true;
            trans.CopySchema = true;
            trans.DropDestinationObjectsFirst = true;
            trans.UseDestinationTransaction = true;

            trans.Options.AnsiFile = true;
            trans.Options.ClusteredIndexes = true;
            trans.Options.DriAll = true;
            trans.Options.IncludeHeaders = true;
            trans.Options.IncludeIfNotExists = true;
            trans.Options.SchemaQualify = true;

            StringCollection script = trans.ScriptTransfer();

            foreach(string s in script)
                result.AppendLine(s);

            result.AppendLine();
            result.AppendLine();

            return result.ToString();
        }
    }
}