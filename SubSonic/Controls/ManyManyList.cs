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
using System.Data;
using System.Web.UI.WebControls;
using SubSonic.Utilities;

namespace SubSonic
{
    /// <summary>
    /// Summary for the ManyManyList class
    /// </summary>
    public class ManyManyList : CheckBoxList
    {
        private string foreignTableName = String.Empty;
        private string foreignTextField = String.Empty;
        private string mapTableName = String.Empty;
        private string primaryKeyValue = "0";

        private string primaryTableName = String.Empty;
        private string providerName = String.Empty;

        /// <summary>
        /// Gets or sets the name of the map table.
        /// </summary>
        /// <value>The name of the map table.</value>
        public string MapTableName
        {
            get { return mapTableName; }
            set { mapTableName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the primary table.
        /// </summary>
        /// <value>The name of the primary table.</value>
        public string PrimaryTableName
        {
            get { return primaryTableName; }
            set { primaryTableName = value; }
        }

        /// <summary>
        /// Gets or sets the primary key value.
        /// </summary>
        /// <value>The primary key value.</value>
        public string PrimaryKeyValue
        {
            get { return primaryKeyValue; }
            set { primaryKeyValue = value; }
        }

        /// <summary>
        /// Gets or sets the name of the foreign table.
        /// </summary>
        /// <value>The name of the foreign table.</value>
        public string ForeignTableName
        {
            get { return foreignTableName; }
            set { foreignTableName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public string ProviderName
        {
            get
            {
                if(string.IsNullOrEmpty(providerName))
                    providerName = DataService.Provider.Name;

                return providerName;
            }
            set { providerName = value; }
        }

        /// <summary>
        /// Gets or sets the foreign text field.
        /// </summary>
        /// <value>The foreign text field.</value>
        public string ForeignTextField
        {
            get { return foreignTextField; }
            set { foreignTextField = value; }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Items.Clear();
            //make sure the default props are set
            if(String.IsNullOrEmpty(primaryKeyValue) || String.IsNullOrEmpty(primaryTableName) || String.IsNullOrEmpty(mapTableName))
            {
                throw new Exception(
                    "Missing a setting. Please be sure to set the PrimaryKeyValue (e.g. 'ProductID', PrimaryTableName (e.g. 'Products'), and MapTableName (e.g. Product_Category_Map)");
            }

            DataProvider provider = DataService.GetInstance(providerName);
            TableSchema.Table fkTable = DataService.GetSchema(foreignTableName, providerName, TableType.Table);
            TableSchema.Table pkTable = DataService.GetSchema(primaryTableName, providerName, TableType.Table);

            string fkPK = fkTable.PrimaryKey.ColumnName;
            string foreignTextColumn = fkTable.Columns[1].ColumnName;
            if(!string.IsNullOrEmpty(ForeignTextField))
            {
                if(!fkTable.Columns.Contains(ForeignTextField))
                    throw new Exception("Invalid ForeignTextField. Please be sure to set the value to a field name from " + foreignTableName);
                foreignTextColumn = ForeignTextField;
            }

            //batch this into one call
            string idParam = provider.FormatParameterNameForSQL("id");
            QueryCommand cmd = new QueryCommand("SELECT " + fkPK + "," + foreignTextColumn + " FROM " + foreignTableName + ";", providerName);
            cmd.CommandSql += "SELECT " + fkPK + " FROM " + mapTableName + " WHERE " + pkTable.PrimaryKey.ColumnName + " = " + idParam;
            cmd.Parameters.Add(idParam, primaryKeyValue, pkTable.PrimaryKey.DataType);

            //load the list items
            using(IDataReader rdr = DataService.GetReader(cmd))
            {
                while(rdr.Read())
                {
                    ListItem item = new ListItem(rdr[1].ToString(), rdr[0].ToString());
                    Items.Add(item);
                }

                rdr.NextResult();
                while(rdr.Read())
                {
                    string thisVal = rdr[fkPK].ToString().ToLower();
                    foreach(ListItem loopItem in Items)
                    {
                        if(Utility.IsMatch(loopItem.Value, thisVal))
                            loopItem.Selected = true;
                    }
                }

                rdr.Close();
            }
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
            QueryCommandCollection coll = new QueryCommandCollection();
            DataProvider provider = DataService.GetInstance(providerName);
            TableSchema.Table fkTable = DataService.GetSchema(foreignTableName, providerName, TableType.Table);
            TableSchema.Table pkTable = DataService.GetSchema(primaryTableName, providerName, TableType.Table);
            string fkPK = fkTable.PrimaryKey.ColumnName;
            string pk = pkTable.PrimaryKey.ColumnName;

            //delete out the existing
            string idParam = provider.FormatParameterNameForSQL("id");
            QueryCommand cmdDel = new QueryCommand("DELETE FROM " + mapTableName + " WHERE " + pk + " = " + idParam, providerName);
            cmdDel.AddParameter(idParam, primaryKeyValue, DbType.AnsiString);
            //cmdDel.ProviderName = Product.Schema.ProviderName;

            //add this in
            coll.Add(cmdDel);

            //loop the items and insert
            string fkParam = provider.FormatParameterNameForSQL("fkID");
            string pkParam = provider.FormatParameterNameForSQL("pkID");

            foreach(ListItem l in Items)
            {
                if(l.Selected)
                {
                    string iSql = "INSERT INTO " + mapTableName + " (" + fkPK + ", " + pk + ")" + " VALUES (" + fkParam + "," + pkParam + ")";

                    QueryCommand cmd = new QueryCommand(iSql, providerName);
                    cmd.Parameters.Add(fkParam, l.Value, fkTable.PrimaryKey.DataType);
                    cmd.Parameters.Add(pkParam, primaryKeyValue, pkTable.PrimaryKey.DataType);

                    coll.Add(cmd);
                }
            }
            //execute
            DataService.ExecuteTransaction(coll);
        }
    }
}