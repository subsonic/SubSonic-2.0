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
 * 
 * This class was altered and provided by jkealey - thanks!
 * 
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// 
    /// </summary>
    public class LavaBlastManyManyList : ManyManyList
    {
        private string _foreignOrderBy = String.Empty;
        private string _foreignTextColumnName = String.Empty;
        private string _foreignValueColumnName = String.Empty;
        private string _foreignWhere = String.Empty;
        private string _mapTableFkToForeignTable = String.Empty;
        private string _mapTableFkToPrimaryTable = String.Empty;
        private string _primaryKeyName = String.Empty;

        /// <summary>
        /// Gets or sets the name of the primary key.
        /// </summary>
        /// <value>The name of the primary key.</value>
        public string PrimaryKeyName
        {
            get { return _primaryKeyName; }
            set { _primaryKeyName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the foreign value column.
        /// </summary>
        /// <value>The name of the foreign value column.</value>
        public string ForeignValueColumnName
        {
            get { return _foreignValueColumnName; }
            set { _foreignValueColumnName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the foreign text column.
        /// </summary>
        /// <value>The name of the foreign text column.</value>
        public string ForeignTextColumnName
        {
            get { return _foreignTextColumnName; }
            set { _foreignTextColumnName = value; }
        }

        /// <summary>
        /// Gets or sets the foreign order by.
        /// </summary>
        /// <value>The foreign order by.</value>
        public string ForeignOrderBy
        {
            get { return _foreignOrderBy; }
            set { _foreignOrderBy = value; }
        }

        /// <summary>
        /// Gets or sets the foreign where.
        /// </summary>
        /// <value>The foreign where.</value>
        public string ForeignWhere
        {
            get { return _foreignWhere; }
            set { _foreignWhere = value; }
        }

        /// <summary>
        /// Gets or sets the map table fk to primary table.
        /// </summary>
        /// <value>The map table fk to primary table.</value>
        public string MapTableFkToPrimaryTable
        {
            get { return _mapTableFkToPrimaryTable; }
            set { _mapTableFkToPrimaryTable = value; }
        }

        /// <summary>
        /// Gets or sets the map table fk to foreign table.
        /// </summary>
        /// <value>The map table fk to foreign table.</value>
        public string MapTableFkToForeignTable
        {
            get { return _mapTableFkToForeignTable; }
            set { _mapTableFkToForeignTable = value; }
        }

        /// <summary>
        /// If the user does not specify any column names, they are inferred from the schema. 
        /// 
        /// </summary>
        protected virtual void LoadColumnNames()
        {
            // we don't have a user-defined key
            if(String.IsNullOrEmpty(PrimaryKeyName))
            {
                // load primary table 
                TableSchema.Table pkTable = DataService.GetSchema(PrimaryTableName, ProviderName);

                PrimaryKeyName = pkTable.PrimaryKey == null ? pkTable.Columns[0].ColumnName : pkTable.PrimaryKey.ColumnName;
            }

            // we don't have a user-defined key
            if(String.IsNullOrEmpty(ForeignValueColumnName) || String.IsNullOrEmpty(ForeignTextColumnName))
            {
                TableSchema.Table fkTable = DataService.GetSchema(ForeignTableName, ProviderName);

                // we don't have a user-defined key
                if(String.IsNullOrEmpty(ForeignValueColumnName))
                    ForeignValueColumnName = fkTable.PrimaryKey == null ? fkTable.Columns[0].ColumnName : fkTable.PrimaryKey.ColumnName;

                // use another column for the name if it is available
                if(String.IsNullOrEmpty(ForeignTextColumnName))
                    ForeignTextColumnName = fkTable.Columns[fkTable.Columns.Count >= 1 ? 1 : 0].ColumnName;
            }

            /* detect the mapping table column names */
            if(String.IsNullOrEmpty(_mapTableFkToPrimaryTable) || String.IsNullOrEmpty(_mapTableFkToForeignTable))
            {
                // load mapping table 
                TableSchema.Table mapTable = DataService.GetSchema(MapTableName, ProviderName);
                foreach(TableSchema.ForeignKeyTable fkt in mapTable.ForeignKeys)
                {
                    if(String.IsNullOrEmpty(_mapTableFkToPrimaryTable) && fkt.TableName.ToLower() == PrimaryTableName.ToLower())
                        _mapTableFkToPrimaryTable = fkt.ColumnName;
                    else if(String.IsNullOrEmpty(_mapTableFkToForeignTable) && fkt.TableName.ToLower() == ForeignTableName.ToLower())
                        _mapTableFkToForeignTable = fkt.ColumnName;
                }
            }
        }

        /// <summary>
        /// Selects the elements from the mapped table which are also in the filtered foreign query
        /// 
        /// </summary>
        /// <param name="provider">the provider</param>
        /// <param name="cmd">The command to which the select from the mapped table will be appended</param>
        protected virtual void BuildMappedElementCommand(DataProvider provider, QueryCommand cmd)
        {
            string userFilter = String.Empty;
            string idParam = provider.FormatParameterNameForSQL("id");

            if (!String.IsNullOrEmpty(ForeignWhere))
            {
                userFilter +=
                    String.Format("INNER JOIN {0} ON {0}.{1}={2}.{1} WHERE {2}.{3}={4} AND {5}", ForeignTableName, ForeignValueColumnName, MapTableName, MapTableFkToPrimaryTable,
                        idParam, ForeignWhere);
            }
            else
                userFilter += String.Format("WHERE {0}={1}", MapTableFkToPrimaryTable, idParam);

            cmd.CommandSql += String.Format("SELECT {1}.{0} FROM {1} {2}", MapTableFkToForeignTable, MapTableName, userFilter);
            cmd.Parameters.Add(idParam, PrimaryKeyValue, DataService.GetSchema(PrimaryTableName, ProviderName).PrimaryKey.DataType);
        }

        /// <summary>
        /// Builds a filtered and sorted list of elements to be put in the checkboxlist
        /// </summary>
        /// <returns></returns>
        protected virtual QueryCommand BuildForeignQueryCommand()
        {
            // filter and sort according to user preferences. 
            string userFilterAndSort = String.Empty;
            if(!String.IsNullOrEmpty(ForeignWhere))
                userFilterAndSort += SqlFragment.WHERE + ForeignWhere;
            if(!String.IsNullOrEmpty(ForeignOrderBy))
                userFilterAndSort += SqlFragment.ORDER_BY + ForeignOrderBy;

            QueryCommand cmd =
                new QueryCommand(String.Format("SELECT {0},{1} FROM {2} {3};", ForeignValueColumnName, ForeignTextColumnName, ForeignTableName, userFilterAndSort), ProviderName);
            return cmd;
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Items.Clear();

            // make sure the default props are set
            if(String.IsNullOrEmpty(PrimaryKeyValue) || String.IsNullOrEmpty(PrimaryTableName) || String.IsNullOrEmpty(ProviderName) || String.IsNullOrEmpty(MapTableName))
            {
                throw new Exception(
                    "Missing a setting. Please be sure to set the PrimaryKeyValue (e.g. 'ProductID', PrimaryTableName (e.g. 'Products'), ProviderName (e.g. 'Northwind'), and MapTableName (e.g. Product_Category_Map)");
            }

            DataProvider provider = DataService.GetInstance(ProviderName);

            // load column names
            LoadColumnNames();

            QueryCommand cmd = BuildForeignQueryCommand();

            BuildMappedElementCommand(provider, cmd);

            //load the list items
            using(IDataReader rdr = DataService.GetReader(cmd))
            {
                while(rdr.Read())
                {
                    ListItem item = new ListItem(rdr[ForeignTextColumnName].ToString(), rdr[ForeignValueColumnName].ToString());
                    Items.Add(item);
                }

                rdr.NextResult();
                while(rdr.Read())
                {
                    string thisVal = rdr[MapTableFkToForeignTable].ToString().ToLower();
                    foreach(ListItem loopItem in Items)
                    {
                        if(loopItem.Value.ToLower() == thisVal)
                            loopItem.Selected = true;
                    }
                }
                rdr.Close();
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public new virtual void Save()
        {
            DataProvider provider = DataService.GetInstance(ProviderName);
            LoadColumnNames();

            // read the current state of the checkboxes
            Dictionary<string, bool> newState = new Dictionary<string, bool>();
            foreach(ListItem l in Items)
                newState.Add(l.Value, l.Selected);

            // read what is in the database
            List<string> pastState = new List<string>();
            QueryCommand lookupCmd = new QueryCommand(String.Empty, ProviderName); // quick hack to re-use BuildMappedElementCommand
            BuildMappedElementCommand(provider, lookupCmd);

            using(IDataReader rdr = DataService.GetReader(lookupCmd))
            {
                while(rdr.Read())
                    pastState.Add(rdr[MapTableFkToForeignTable].ToString());
                rdr.Close();
            }

            // build the commands to be executed. 
            QueryCommandCollection coll = new QueryCommandCollection();

            string fkParam = provider.FormatParameterNameForSQL("fkID");
            string pkParam = provider.FormatParameterNameForSQL("pkID");

            foreach(KeyValuePair<string, bool> kvp in newState)
            {
                string sql;

                // if we have it now but did not before
                if(kvp.Value && !pastState.Contains(kvp.Key))
                    sql = String.Format("INSERT INTO {0} ({1},{2}) VALUES ({3},{4})", MapTableName, MapTableFkToForeignTable, MapTableFkToPrimaryTable, fkParam, pkParam);
                else if(!kvp.Value && pastState.Contains(kvp.Key)) // we don't have it now but had it before 
                    sql = String.Format("DELETE FROM {0} WHERE {1} = {2} AND {3} = {4}", MapTableName, MapTableFkToPrimaryTable, pkParam, MapTableFkToForeignTable, fkParam);
                else
                    continue; // nothing changed. 

                QueryCommand cmd = new QueryCommand(sql, ProviderName);
                cmd.Parameters.Add(fkParam, kvp.Key, DataService.GetSchema(ForeignTableName, ProviderName).PrimaryKey.DataType);
                cmd.Parameters.Add(pkParam, PrimaryKeyValue, DataService.GetSchema(PrimaryTableName, ProviderName).PrimaryKey.DataType);
                coll.Add(cmd);
            }

            //execute
            if(coll.Count > 0)
                DataService.ExecuteTransaction(coll);
        }
    }
}