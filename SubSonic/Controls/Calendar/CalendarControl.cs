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
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SubSonic
{
    /// <summary>
    /// Summary description for CalendarControl.
    /// </summary>
    public class CalendarControl : TextBox
    {
        private const string DEFAULT_FORMAT = "MM/dd/yyyy";
        private const string DEFAULT_INVALID_DATE = "Please enter a valid date.";
        private const string DEFAULT_JAVASCRIPT_FORMAT = "%m/%d/%Y %I:%M %p";
        private const string DEFAULT_LANGUAGE = "en";
        private string displayFormat;
        private Image imgCalendar;
        private string javaScriptFormat;
        private string language;
        private DateTime? selectedDate;
        private bool showTime = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarControl"/> class.
        /// </summary>
        public CalendarControl()
        {
            DisplayFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
            if(!String.IsNullOrEmpty(DisplayFormat))
            {
                char splitter = CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator[0];
                string[] format = DisplayFormat.Split(new char[] {splitter});
                StringBuilder sbFormat = new StringBuilder();
                for(int i = 0; i < format.Length; i++)
                {
                    sbFormat.Append("%");
                    sbFormat.Append(format[i][0]);
                    if(i + 1 != format.Length)
                        sbFormat.Append(splitter);
                }

                JavaScriptFormat = sbFormat.ToString().ToLower() + " %I:%M %p";
            }
            else
            {
                DisplayFormat = DEFAULT_FORMAT;
                JavaScriptFormat = DEFAULT_JAVASCRIPT_FORMAT;
            }

            Language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        }

        /// <summary>
        /// Gets the calendar image.
        /// </summary>
        /// <value>The calendar image.</value>
        private Image CalendarImage
        {
            get
            {
                if(imgCalendar == null)
                {
                    imgCalendar = new Image();

                    imgCalendar.ID = "imgCalendar" + ClientID;
                    imgCalendar.ImageAlign = ImageAlign.AbsMiddle;
                    imgCalendar.ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "SubSonic.Controls.Calendar.skin.calendar.gif");
                    imgCalendar.Style.Add("margin-left", "5px");
                }

                return imgCalendar;
            }
        }

        /// <summary>
        /// Gets or sets the display format.
        /// </summary>
        /// <value>The display format.</value>
        [DefaultValue(DEFAULT_FORMAT)]
        public string DisplayFormat
        {
            get { return displayFormat; }
            set { displayFormat = value; }
        }

        /// <summary>
        /// Gets or sets the java script format.
        /// </summary>
        /// <value>The java script format.</value>
        [DefaultValue(DEFAULT_JAVASCRIPT_FORMAT)]
        public string JavaScriptFormat
        {
            get { return javaScriptFormat; }
            set { javaScriptFormat = value; }
        }

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        [DefaultValue(DEFAULT_LANGUAGE)]
        public string Language
        {
            get { return language; }
            set { language = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show time].
        /// </summary>
        /// <value><c>true</c> if [show time]; otherwise, <c>false</c>.</value>
        public bool ShowTime
        {
            get { return showTime; }
            set { showTime = value; }
        }

        /// <summary>
        /// Gets or sets the selected date.
        /// </summary>
        /// <value>The selected date.</value>
        public DateTime? SelectedDate
        {
            get
            {
                string selDate = Text.Trim();
                if(!String.IsNullOrEmpty(selDate))
                {
                    DateTime parseDate;
                    if(DateTime.TryParse(selDate, out parseDate))
                        selectedDate = parseDate;
                }
                else
                    selectedDate = null;

                return selectedDate;
            }
            set
            {
                selectedDate = value;
                if(selectedDate.HasValue)
                {
                    DateTime dt = selectedDate.Value;
                    Text = dt.ToString(DisplayFormat);
                }
                else
                    Text = String.Empty;
            }
        }

        /// <summary>
        /// Registers client script for generating postback events prior to rendering on the client, if <see cref="P:System.Web.UI.WebControls.TextBox.AutoPostBack"/> is true.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            string csslink = "<link href='" + Page.ClientScript.GetWebResourceUrl(GetType(), "SubSonic.Controls.Calendar.skin.theme.css") + "' rel='stylesheet' type='text/css' />";
            Page.Header.Controls.Add(new LiteralControl(csslink));
            Page.ClientScript.RegisterClientScriptInclude("CalendarMain", Page.ClientScript.GetWebResourceUrl(GetType(), "SubSonic.Controls.Calendar.calendar.js"));
            Page.ClientScript.RegisterClientScriptInclude("CalendarSetup", Page.ClientScript.GetWebResourceUrl(GetType(), "SubSonic.Controls.Calendar.calendar-setup.js"));

            const string langPrefix = "SubSonic.Controls.Calendar.lang.calendar-";

            if(String.IsNullOrEmpty(Language))
                Language = DEFAULT_LANGUAGE;

            if(Assembly.GetExecutingAssembly().GetManifestResourceStream(langPrefix + Language + ".js") == null)
                Page.ClientScript.RegisterClientScriptInclude("CalendarLanguage", Page.ClientScript.GetWebResourceUrl(GetType(), langPrefix + DEFAULT_LANGUAGE + ".js"));
            else
                Page.ClientScript.RegisterClientScriptInclude("CalendarLanguage", Page.ClientScript.GetWebResourceUrl(GetType(), langPrefix + Language + ".js"));

            base.OnPreRender(e);
        }

        /// <summary>
        /// Renders the <see cref="T:System.Web.UI.WebControls.TextBox"/> control to the specified <see cref="T:System.Web.UI.HtmlTextWriter"/> object.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> that receives the rendered output.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if(SelectedDate == DateTime.MinValue)
                SelectedDate = DateTime.Now;

            writer.WriteLine("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            writer.WriteLine("<tr>");
            writer.WriteLine("<td>");

            base.Render(writer);

            writer.WriteLine("</td><td>");

            if(Enabled)
                CalendarImage.RenderControl(writer); // render CalendarButton object

            writer.WriteLine("</td>");
            writer.WriteLine("</tr>");
            writer.WriteLine("</table>");

            if(Enabled)
            {
                Page.ClientScript.RegisterStartupScript(typeof(Page), "Calendar" + ClientID,
                    "<script type=\"text/javascript\">" +
                    "Calendar.setup( { " +
                    "inputField: \"" + ClientID + "\", " +
                    "ifFormat: \"" + JavaScriptFormat + "\", " +
                    "button: \"" + CalendarImage.ClientID + "\", " +
                    "date: \"" + SelectedDate + "\", " +
                    "showsTime: " + (ShowTime ? "true" : "false") + " " +
                    "} );" +
                    "</script>");
            }
        }
    }
}