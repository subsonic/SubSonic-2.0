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
using Northwind;
using SubSonic;

public partial class Examples : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //Set default visibility
        GridView1.Visible = true;
        litInspect.Text = String.Empty;
        pnlSubSonicControls.Visible = false;
    }

    protected void lnkAll_Click(object sender, EventArgs e)
    {
        GridView1.DataSource = new Query(Product.Schema).ExecuteReader();
        GridView1.DataBind();
        lblExampleName.Text = "Show All Data";
        lblCode.Text = @"        GridView1.DataSource = new Query(Product.Schema).ExecuteReader();
        GridView1.DataBind();";
    }

    protected void lnkTop20_Click(object sender, EventArgs e)
    {
        Query qry = new Query(Product.Schema);
        qry.Top = "20";
        GridView1.DataSource = qry.ExecuteReader();
        GridView1.DataBind();

        lblExampleName.Text = "Show Top 20";
        lblCode.Text =
            @"        Query qry = new Query(Product.Schema);
        qry.Top = &quot;20&quot;;
        GridView1.DataSource = qry.ExecuteReader();
        GridView1.DataBind();";
    }

    protected void lnkTopPrice_Click(object sender, EventArgs e)
    {
        Query qry = new Query(Product.Schema);
        qry.Top = "10";
        qry.SelectList = Product.Columns.ProductName + "," + Product.Columns.UnitPrice;
        qry.OrderBy = OrderBy.Desc(Product.Columns.UnitPrice);
        GridView1.DataSource = qry.ExecuteReader();
        GridView1.DataBind();

        lblExampleName.Text = "Show Top 10 by Price";
        lblCode.Text =
            @"        Query qry = new Query(Product.Schema);
        qry.Top = &quot;10&quot;;
        qry.SelectList = Product.Columns.ProductName + &quot;,&quot; + Product.Columns.UnitPrice;
        qry.OrderBy = OrderBy.Desc(Product.Columns.UnitPrice);
        GridView1.DataSource = qry.ExecuteReader();
        GridView1.DataBind();";
    }

    protected void lnkBatchUpdate_Click(object sender, EventArgs e)
    {
        Query qry = new Query(Product.Schema);
        qry.AddUpdateSetting(Product.Columns.UnitPrice, 100);
        qry.AddWhere(Product.Columns.UnitPrice, Comparison.GreaterThan, 20);
        qry.Execute();

        qry = new Query(Product.Schema);
        GridView1.DataSource = qry.ExecuteReader();
        GridView1.DataBind();

        lblExampleName.Text = "Batch Update";
        lblCode.Text =
            @"        Query qry = new Query(Product.Schema);
        qry.AddUpdateSetting(Product.Columns.UnitPrice, 100);
        qry.AddWhere(Product.Columns.UnitPrice, Comparison.GreaterThan, 20);
        qry.Execute();

        qry = new Query(Product.Schema);
        GridView1.DataSource = qry.ExecuteReader();
        GridView1.DataBind();";
    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        //this is a view from the NWind DB that is read-only, but can be
        //used just like an object, with a collection!
        GridView1.DataSource = new Query("Current Product List").ExecuteReader();
        GridView1.DataBind();

        lblExampleName.Text = "Use a View";
        lblCode.Text = @"        GridView1.DataSource = CurrentProductList.FetchAll();
        GridView1.DataBind();";
    }

    protected void LinkButton2_Click(object sender, EventArgs e)
    {
        GridView1.DataSource = new ProductCollection().Where(Product.Columns.UnitPrice, Comparison.GreaterOrEquals, 30).Load();
        GridView1.DataBind();

        lblExampleName.Text = "Use a Collection";
        lblCode.Text =
            @"        GridView1.DataSource = new ProductCollection()
            .Where(Product.Columns.UnitPrice, Comparison.GreaterOrEquals, 30).Load();
        GridView1.DataBind();";
    }

    protected void LinkButton3_Click(object sender, EventArgs e)
    {
        GridView1.DataSource = Northwind.SPs.CustOrderHist("ALFKI").GetReader();
        GridView1.DataBind();

        lblExampleName.Text = "Use a Stored Procedure";
        lblCode.Text = @"        GridView1.DataSource = Northwind.SPs.CustOrderHist(&quot;ALFKI&quot;).GetReader();
        GridView1.DataBind();";
    }

    protected void lbtInspect_Click(object sender, EventArgs e)
    {
        GridView1.Visible = false;
        litInspect.Text = new Query("Products").WHERE("CategoryID = 5").Inspect();

        lblExampleName.Text = "Use Query.Inspect()";
        lblCode.Text = @"        Query(&quot;Products&quot;).WHERE(&quot;CategoryID = 5&quot;).Inspect();";
    }

    protected void LinkButton4_Click(object sender, EventArgs e)
    {
        GridView1.DataSource = Product.Query().WHERE("CategoryID = 5").ExecuteReader();
        GridView1.DataBind();

        lblExampleName.Text = "Use Product.Query()";
        lblCode.Text = @"        GridView1.DataSource = Product.Query().WHERE(&quot;CategoryID = 5&quot;).ExecuteReader();
        GridView1.DataBind();";
    }

    protected void LinkButton5_Click(object sender, EventArgs e)
    {
        object[] ids = new object[] {1, 2, 3, 4, 5};

        GridView1.DataSource = new Query(Product.Schema).IN(Product.Columns.ProductID, ids).ExecuteReader();
        GridView1.DataBind();

        lblExampleName.Text = "Use IN()";
        lblCode.Text = @"        GridView1.DataSource = new Query(Product.Schema)
            .IN(Product.Columns.ProductID, ids).ExecuteReader();
        GridView1.DataBind();";
    }

    protected void LinkButton6_Click(object sender, EventArgs e)
    {
        object[] ids = new object[] {1, 2, 3, 4, 5};

        GridView1.DataSource = Product.Query().WHERE("CategoryID = 5").OR("CategoryID = 1").ExecuteReader();
        GridView1.DataBind();

        lblExampleName.Text = "Use OR()";
        lblCode.Text =
            @"        GridView1.DataSource = Product.Query().WHERE(&quot;CategoryID = 5&quot;)
            .OR(&quot;CategoryID = 1&quot;).ExecuteReader();
        GridView1.DataBind();";
    }

    protected void LinkButton7_Click(object sender, EventArgs e)
    {
        object[] ids = new object[] {1, 2, 3, 4, 5};

        //thanks datacop... this is your example homey!
        GridView1.DataSource = Product.Query().WHERE("UnitPrice > 20.00").AND("CategoryID = 1").OR("UnitPrice > 20.00").AND("CategoryID = 5").ExecuteReader();
        GridView1.DataBind();

        lblExampleName.Text = "Use a Complex OR()";
        lblCode.Text =
            @"        GridView1.DataSource = Product.Query().WHERE(&quot;UnitPrice > 20.00&quot;)
            .AND(&quot;CategoryID = 1&quot;)
            .OR(&quot;UnitPrice > 20.00&quot;)
            .AND(&quot;CategoryID = 5&quot;).ExecuteReader();
        GridView1.DataBind();";
    }

    protected void lnkControls_Click(object sender, EventArgs e)
    {
        GridView1.Visible = false;
        pnlSubSonicControls.Visible = true;
    }
}