using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Collections;

namespace SubSonic
{
    [DefaultProperty("Count")]
    [ToolboxData("<{0}:LoremIpsum runat=server></{0}:LoremIpsum>")]
    public class LoremIpsum : WebControl, INamingContainer
    {
        #region props

        private string method = string.Empty;
        public string Method
        {
            get { return method; }
            set { method = value; }
        }

        private int count;
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {

            string lorem = Sugar.Web.GenerateLoremIpsum(count, method);

            //base.Render(writer);
            writer.WriteLine(lorem.Trim());
        }

    }
}
