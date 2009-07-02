using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace SubSonic {
    [ParseChildren(false)]
    [ToolboxData("<{0}:EditBox runat=server></{0}:EditBox>")]
    public class EditBox : WebControl,INamingContainer {
        
        private string editRole="Content Editor";
        public string EditRole {
            get { return editRole; }
            set { editRole = value; }
        }
	

        Panel pnlDisplay = new Panel();
        Panel pnlEdit = new Panel();
        Button btnToggle = new Button();
        Button btnSave = new Button();
        Button btnCancel = new Button();
        LinkButton lnkEdit = new LinkButton();

        TextBox tbEdit = new TextBox();
        Literal litDisplay = new Literal();


        protected override void CreateChildControls() {
            base.CreateChildControls();

            btnSave.Click += new EventHandler(btnSave_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            lnkEdit.Click += new EventHandler(lnkEdit_Click);


            tbEdit.TextMode = TextBoxMode.MultiLine;
            tbEdit.Height = Unit.Pixel(400);
            tbEdit.Width = Unit.Pixel(300);
            
            if (!Page.IsPostBack) {
                tbEdit.Visible = false;
                ToggleEditMode(false);
            }

            btnSave.Text = "Save";
            btnCancel.Text = "Cancel";
            lnkEdit.Text = "Edit";


            this.Controls.Add(tbEdit);
            this.Controls.Add(litDisplay);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            this.Controls.Add(lnkEdit);

        }
        void ToggleEditMode(bool showEditor) {
            
            //editor bits
            tbEdit.Visible = showEditor;
            btnSave.Visible = showEditor;
            btnCancel.Visible = showEditor;
            litDisplay.Visible = !showEditor;

            //if this user can edit, show the link
            //lnkEdit.Visible = Page.User.IsInRole(this.editRole);
        }
        void lnkEdit_Click(object sender, EventArgs e) {
            ToggleEditMode(true);
        }

        void btnCancel_Click(object sender, EventArgs e) {
            ToggleEditMode(false);
        }

        void btnSave_Click(object sender, EventArgs e) {

            //find the control, and store the text on the page, between the control tags
            //this can be tricky...
            string pageFile = Page.Request.CurrentExecutionFilePath;
            string pagePath = Page.Server.MapPath(pageFile);

            //now, find the tag in the page's text
            string regPattern = @"(?<="+this.ID+".*?>).*?(?=</)";

            //run a replace
            string pageText = Utilities.Utility.GetFileText(pagePath);

            Regex reg = new Regex(regPattern);
            pageText=reg.Replace(pageText, tbEdit.Text);

            //put the pageText back
            Utilities.Utility.WriteToFile(pagePath, pageText);

            //redirect to the page
            Page.Response.Redirect(Page.Request.CurrentExecutionFilePath);


        }
    }
}
