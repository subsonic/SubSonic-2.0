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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SubSonic
{
    // Thank you photoz!

    /// <summary>
    /// Summary for the RadioButtons class
    /// </summary>
    [ToolboxData("<{0}:RadioButtons runat=server></{0}:RadioButtons>")]
    public class RadioButtons : RadioButtonList
    {
        private string orderField = String.Empty;
        private string promptText = "None";

        private string promptValue = String.Empty;
        private string providerName = String.Empty;

        private bool showPrompt;
        private string tableName = String.Empty;
        private string textField = String.Empty;
        private string valueField = String.Empty;

        /// <summary>
        /// Gets or sets the prompt text.
        /// </summary>
        /// <value>The prompt text.</value>
        public string PromptText
        {
            get { return promptText; }
            set { promptText = value; }
        }

        /// <summary>
        /// Gets or sets the prompt value.
        /// </summary>
        /// <value>The prompt value.</value>
        public string PromptValue
        {
            get { return promptValue; }
            set { promptValue = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show prompt].
        /// </summary>
        /// <value><c>true</c> if [show prompt]; otherwise, <c>false</c>.</value>
        public bool ShowPrompt
        {
            get { return showPrompt; }
            set { showPrompt = value; }
        }

        /// <summary>
        /// Gets or sets the name of the provider.
        /// </summary>
        /// <value>The name of the provider.</value>
        public string ProviderName
        {
            get { return providerName; }
            set { providerName = value; }
        }

        /// <summary>
        /// Gets or sets the name of the table.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }

        /// <summary>
        /// Gets or sets the text field.
        /// </summary>
        /// <value>The text field.</value>
        public string TextField
        {
            get { return textField; }
            set { textField = value; }
        }

        /// <summary>
        /// Gets or sets the value field.
        /// </summary>
        /// <value>The value field.</value>
        public string ValueField
        {
            get { return valueField; }
            set { valueField = value; }
        }

        /// <summary>
        /// Gets or sets the order field.
        /// </summary>
        /// <value>The order field.</value>
        public string OrderField
        {
            get { return orderField; }
            set { orderField = value; }
        }

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if(!DesignMode)
            {
                // load em up
                // cheap way to check for load state
                if(Items.Count == 0)
                {
                    if(!String.IsNullOrEmpty(tableName))
                    {
                        DataProvider provider = DataService.GetInstance(ProviderName);
                        SqlQuery q = new Select(provider).From(tableName);
                        q.CheckLogicalDelete();

                        if(String.IsNullOrEmpty(valueField) || String.IsNullOrEmpty(textField))
                        {
                            // look it up using the table schema
                            TableSchema.Table tbl = DataService.GetSchema(tableName, providerName, TableType.Table);
                            if(tbl != null)
                            {
                                if(String.IsNullOrEmpty(valueField))
                                    valueField = tbl.PrimaryKey.ColumnName;

                                if(String.IsNullOrEmpty(textField))
                                    textField = tbl.Columns.Count > 1 ? tbl.Columns[1].ColumnName : tbl.Columns[0].ColumnName;
                            }
                            else
                                throw new Exception("Table name '" + tableName + "' using Provider '" + providerName + "' doesn't work");
                        }

                        q.SelectColumnList = new[] {valueField, textField};
                        if(!String.IsNullOrEmpty(OrderField))
                            q.OrderAsc(OrderField);
                        else
                            q.OrderAsc(textField);

                        IDataReader rdr = null;
                        try
                        {
                            rdr = q.ExecuteReader();
                            while(rdr.Read())
                            {
                                ListItem item = new ListItem(rdr[1].ToString(), rdr[0].ToString());
                                Items.Add(item);
                            }
                        }
                        catch(DataException x)
                        {
                            throw new Exception("Error loading up ListItems for " + ClientID + ": " + x.Message);
                        }
                        finally
                        {
                            if(rdr != null)
                                rdr.Close();
                        }
                        ListItem prompt = new ListItem(promptText, PromptValue);
                        if(showPrompt)
                            Items.Insert(0, prompt);
                    }
                }
            }
        }
    }
}