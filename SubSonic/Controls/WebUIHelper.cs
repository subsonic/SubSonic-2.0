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

using System.Web.UI;

namespace SubSonic
{
    /// <summary>
    /// Summary for the WebUIHelper class
    /// </summary>
    public class WebUIHelper
    {
        /// <summary>
        /// Emits the client scripts.
        /// </summary>
        /// <param name="renderedPage">The rendered page.</param>
        public static void EmitClientScripts(Page renderedPage)
        {
            renderedPage.ClientScript.RegisterClientScriptInclude("SubSonicClientScripts",
                renderedPage.ClientScript.GetWebResourceUrl(typeof(WebUIHelper), "SubSonic.Controls.Resources.ClientScripts.js"));
        }
    }
}