using System;
using System.Text;
using System.Web.UI.WebControls;
using SubSonic;
using SubSonic.Utilities;
using System.Data;



namespace SubSonic
{
    public class ScaffoldCodeGenerator
    {
        #region Page Builders
        /// <summary>
        /// Generates the "In Front" code for an ASPX page
        /// </summary>
        /// <param name="providerName">Name of the DataProvider to use</param>
        /// <param name="tableName">Name of Table</param>
        /// <param name="pageFileName">The name of the ASPX page - like "Editor.aspx"</param>
        /// <param name="masterPage">The Master Page File, if any</param>
        /// <param name="languageType">Output language, i.e C# or that other one</param>
        /// <returns></returns>
        public static string GeneratePage(string providerName, string tableName, string pageFileName, string masterPage, LanguageType languageType)
        {
            //Grab the tableschema
            TableSchema.Table tbl = DataService.GetTableSchema(tableName, providerName);

            if (tbl.PrimaryKey == null)
            {
                return tableName + " does not contain a primary key. Primary keys are required for scaffold generation."; 
            }

            string pageTemplate = ResourceHelper.GetString(TemplateName.EDITOR);
                /*
                #PK# = Primary Key of the table
                #GRIDROWS# = all the rows in the GridView
                #EDITORROWS# = the rows inside the editor table
                #TABLENAME# = the name of the table
                */

                //this part's simple enough - just straight up text replacement
                pageTemplate = pageTemplate.Replace(TemplateVariable.TABLE_NAME, tbl.DisplayName);
                pageTemplate = pageTemplate.Replace(TemplateVariable.PK, tbl.PrimaryKey.ColumnName);
                pageTemplate = pageTemplate.Replace(TemplateVariable.PAGE_FILE, pageFileName);

                //now loop out the columns for each row of the table editor and grid
                StringBuilder gridRows = new StringBuilder();
                StringBuilder editorRows = new StringBuilder();

                foreach(TableSchema.TableColumn col in tbl.Columns)
                {
                    if(!col.IsPrimaryKey)
                    {
                        string control;
                        string properties = String.Empty;
                        control = GetControlText(typeof(BoundField).Name, null);
                        AddControlProperty(ref properties, "DataField", col.ColumnName);
                        AddControlProperty(ref properties, "HeaderText", Utility.ParseCamelToProper(col.ColumnName));
                        AddControlProperty(ref properties, "SortExpression", col.ColumnName);
                        ApplyControlProperties(ref control, properties);
                        gridRows.AppendLine(AddTabs(2) + control);
                        editorRows.AppendLine("\t\t<tr>" + Environment.NewLine + "\t\t\t<td class=\"scaffoldEditItemCaption\">" + Utility.ParseCamelToProper(col.ColumnName) + "</td>" + Environment.NewLine + "\t\t\t<td class=\"scaffoldEditItem\">" + GetEditControl(col) + "</td>" + Environment.NewLine + "\t\t</tr>");
                    }
                }

                pageTemplate = pageTemplate.Replace(TemplateVariable.EDITOR_ROWS, editorRows.ToString());
                pageTemplate = pageTemplate.Replace(TemplateVariable.GRID_ROWS, gridRows.ToString());


                //get the wrapper
                bool useMaster = !String.IsNullOrEmpty(masterPage);
                string wrapper = GetPageWrapper(useMaster);

                string jsBlock = CodeBlock.JS_BEGIN_SCRIPT + ResourceHelper.GetString(TemplateName.SCAFFOLD_DELETE_CONFIRMATION) + ResourceHelper.GetString(TemplateName.SCAFFOLD_CHARACTER_COUNTER) + CodeBlock.JS_END_SCRIPT;
                wrapper = wrapper.Replace(TemplateVariable.JAVASCRIPT_BLOCK, jsBlock);
                wrapper = wrapper.Replace(TemplateVariable.PAGE_CODE, pageTemplate.ToString());
                wrapper = wrapper.Replace(TemplateVariable.MASTER_PAGE, masterPage);
                wrapper = wrapper.Replace(TemplateVariable.TABLE_NAME, pageFileName.Replace(FileExtension.DOT_ASPX, String.Empty));

                //determine the language
                string codeFileExt = FileExtension.CS;
                if(languageType == LanguageType.VB)
                {
                    codeFileExt = FileExtension.VB;
                }
                wrapper = wrapper.Replace(TemplateVariable.LANGUAGE_EXTENSION, codeFileExt);

                return wrapper;

        }

        static string GetPageWrapper(bool useMaster)
        {
            string result;

            if (useMaster)
            {
                result = ResourceHelper.GetString(TemplateName.EDITOR_WRAPPER_MASTERPAGE);
            }
            else
            {
                result = ResourceHelper.GetString(TemplateName.EDITOR_WRAPPER);
            }
            return result;
        }

        private static string GetControlText(string controlType, string controlId)
        {
            if(!String.IsNullOrEmpty(controlId))
            {
                return "<asp:" + controlType + " ID=\"" + controlId + "\" runat=\"server\" " + TemplateVariable.CONTROL_PROPERTY + "></asp:" + controlType + ">";
            }
            else
            {
                return "<asp:" + controlType + " " + TemplateVariable.CONTROL_PROPERTY + "></asp:" + controlType + ">";
            }
        }

        private static string AddTabs(int count)
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < count; i++)
            {
                sb.Append("\t");
            }
            return sb.ToString();
        }

        private static void AddControlProperty(ref string variableText, string propertyName, string propertyValue)
        {
            variableText += propertyName + "=\"" + propertyValue + "\" ";
        }

        private static void ApplyControlProperties(ref string controlText, string variableText)
        {
            controlText = controlText.Replace(TemplateVariable.CONTROL_PROPERTY, variableText.Trim());
        }

        private static string GetEditControl(TableSchema.TableColumn col)
        {
            string result;
            string variableText = String.Empty;
            string controlID = col.PropertyName;
            bool isTextBox = false;

            if(col.IsForeignKey)
            {
                result = GetControlText(typeof(DropDownList).Name, controlID);
            }
            else
            {
                switch(col.DataType)
                {
                    case DbType.Binary:
                    case DbType.Byte:
                        result = String.Empty;
                        break;
                    case DbType.Guid:
                    case DbType.AnsiString:
                    case DbType.String:
                    case DbType.StringFixedLength:
                    case DbType.Xml:
                    case DbType.Object:
                    case DbType.AnsiStringFixedLength:
                        if(Utility.IsMatch(col.ColumnName, ReservedColumnName.CREATED_BY) || Utility.IsMatch(col.ColumnName, ReservedColumnName.MODIFIED_BY))
                        {
                            result = GetControlText(typeof(Label).Name, controlID);
                        }
                        else
                        {
                            isTextBox = true;
                            result = GetControlText(typeof(TextBox).Name, controlID);
                            if(col.MaxLength > 250)
                            {
                                AddControlProperty(ref variableText, "TextMode", "MultiLine");
                                AddControlProperty(ref variableText, "Height", "100px");
                                AddControlProperty(ref variableText, "Width", "500px");
                            }
                        }
                        break;
                    case DbType.Date:
                    case DbType.Time:
                    case DbType.DateTime:
                        if(Utility.IsMatch(col.ColumnName, ReservedColumnName.MODIFIED_ON) || Utility.IsMatch(col.ColumnName, ReservedColumnName.CREATED_ON))
                        {
                            result = GetControlText(typeof(Label).Name, controlID);
                        }
                        else
                        {
                            result = GetControlText(typeof(Calendar).Name, controlID);
                        }
                        break;
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.UInt16:
                    case DbType.Int64:
                    case DbType.UInt32:
                    case DbType.UInt64:
                    case DbType.VarNumeric:
                    case DbType.Single:
                    case DbType.Currency:
                    case DbType.Decimal:
                    case DbType.Double:
                        isTextBox = true;
                        result = GetControlText(typeof(TextBox).Name, controlID);
                        AddControlProperty(ref variableText, "Width", "50px");
                        if(col.DataType == DbType.Currency)
                        {
                            result = "$" + result;
                        }
                        break;
                    case DbType.Boolean:
                        bool isChecked = false;
                        if(Utility.IsMatch(col.ColumnName, ReservedColumnName.IS_ACTIVE))
                        {
                            isChecked = true;
                        }
                        result = GetControlText(typeof(CheckBox).Name, controlID);
                        AddControlProperty(ref variableText, ControlValueProperty.CHECK_BOX, isChecked.ToString());
                        break;
                    default:
                        isTextBox = true;
                        result = GetControlText(typeof(TextBox).Name, controlID);
                        break;
                }
            }
            if(isTextBox && col.MaxLength > 0)
            {
                AddControlProperty(ref variableText, "MaxLength", col.MaxLength.ToString());
            }
            ApplyControlProperties(ref result, variableText);
            return result;
        }
        #endregion


        #region Code Builder

        private static void CreatePropertySetter(ref string controlText, string controlId, string itemPropertyName, string controlPropertyName, bool toString, bool nullableType)
        {
            StringBuilder sb = new StringBuilder();
            if(nullableType)
            {
                sb.AppendLine("if(item." + itemPropertyName + ".HasValue)");
                sb.AppendLine(AddTabs(2) + "{");
                if(toString)
                {
                    sb.AppendLine(AddTabs(3) + controlId + "." + controlPropertyName + " = item." + itemPropertyName + ".Value.ToString();");
                }
                else
                {
                    sb.AppendLine(AddTabs(3) + controlId + "." + controlPropertyName + " = item." + itemPropertyName + ".Value;");
                }
                sb.AppendLine(AddTabs(2) + "}");
            }
            else
            {
                if (toString)
                {
                    sb.AppendLine(AddTabs(2) + controlId + "." + controlPropertyName + " = item." + itemPropertyName + ".ToString();");
                }
                else
                {
                    sb.AppendLine(AddTabs(2) + controlId + "." + controlPropertyName + " = item." + itemPropertyName + ";");
                }
            }
            controlText = sb.ToString();
        }

        static string GetControlValue(TableSchema.TableColumn col, string className)
        {
            StringBuilder result = new StringBuilder();
            string controlID = col.PropertyName;
            string propName = col.PropertyName;
            string converterType = (col.DataType == DbType.Currency || col.DataType == DbType.VarNumeric) ? "Decimal" : col.DataType.ToString();
            int tabIndent = 2;

            result.AppendLine(AddTabs(2) + "object val" + controlID + " = Utility.GetDefaultControlValue(" + className + ".Schema.GetColumn(\"" + propName + "\"), Page.FindControl(\"" + controlID + "\"), isAdd, false);");
            if(col.IsNullable)
            {
                result.AppendLine(AddTabs(2) + "if(val" + controlID + " == null)");
                result.AppendLine(AddTabs(2) + "{");
                result.AppendLine(AddTabs(3) + "item." + propName + " = null;");
                result.AppendLine(AddTabs(2) + "}");
                result.AppendLine(AddTabs(2) + "else");
                result.AppendLine(AddTabs(2) + "{");
                tabIndent++;
            }

            if (col.DataType != DbType.Guid)
            {
                result.AppendLine(AddTabs(tabIndent) + "item." + propName + " = Convert.To" + converterType + "(val" + controlID + ");");
            }
            else
            {
                result.AppendLine(AddTabs(tabIndent) + "item." + propName + " = new " + converterType + "(val" + controlID + ".ToString());");
            }

            if(col.IsNullable)
            {
                result.AppendLine(AddTabs(2) + "}");
            }
            return result.ToString();
        }

        static string SetControlValue(TableSchema.TableColumn col, string tableName)
        {
            string result = String.Empty;
            string propName = col.PropertyName;
            string controlID = col.PropertyName;
            bool isNullableType = (col.IsNullable && Utility.IsNullableDbType(col.DataType));

            if (col.IsForeignKey)
            {
                CreatePropertySetter(ref result, controlID, propName, ControlValueProperty.DROP_DOWN_LIST, true, isNullableType);
            }
            else
            {
                
                switch(col.DataType)
                {
                    case DbType.Binary:
                    case DbType.Byte:
                        result = String.Empty;
                        break;
                    case DbType.DateTime:
                        CreatePropertySetter(ref result, controlID, propName, ControlValueProperty.CALENDAR, false, isNullableType);
                        break;
                    case DbType.Boolean:
                        CreatePropertySetter(ref result, controlID, propName, ControlValueProperty.CHECK_BOX, false, isNullableType);
                        break;
                    case DbType.Currency:
                    case DbType.VarNumeric:
                        CreatePropertySetter(ref result, controlID, propName, ControlValueProperty.TEXT_BOX, true, isNullableType);
                        break;
                    case DbType.Int16:
                    case DbType.Int32:
                    case DbType.UInt16:
                    case DbType.Int64:
                    case DbType.UInt32:
                    case DbType.UInt64:
                    case DbType.Single:
                    case DbType.Decimal:
                    case DbType.Double:
                        CreatePropertySetter(ref result, controlID, propName, ControlValueProperty.TEXT_BOX, true, isNullableType);
                        break;
                    case DbType.String:
                        CreatePropertySetter(ref result, controlID, propName, ControlValueProperty.TEXT_BOX, false, isNullableType);
                        break;
                    default:
                        CreatePropertySetter(ref result, controlID, propName, ControlValueProperty.TEXT_BOX, true, isNullableType);
                        break;
                }
            }
            return AddTabs(2) + result;
        }

        /// <summary>
        /// Generates the CodeBehind for the Editor
        /// </summary>
        /// <param name="pageName">The name of the page - can end with ASPX if you like but it will be stripped</param>
        /// <param name="providerName">The Data provider to use</param>
        /// <param name="tableName">Name of the table</param>
        /// <param name="languageType">Output language, i.e C# or that other one</param>
        /// <returns></returns>
        public static string GenerateCode(string pageName, string providerName, string tableName, LanguageType languageType)
        {
            //Grab the tableschema
            TableSchema.Table tbl = DataService.GetTableSchema(tableName, providerName);
            string pageTemplate = ResourceHelper.GetString(TemplateName.EDITOR_CODE);
            /*
            #NAMESPACE_USING# = if there is more than one provider, include this Using statement
            #PAGEBINDLIST# = The list that binds the page to the object
            #BINDLIST# = List that binds the object to the page
            #DROPLIST# = Loads each dropdown
            #CLASSNAME# = the name of the class
            */

            //make sure the providers are loaded
            DataService.LoadProviders();

            //Set the Namespace
            string namespaceUsing;
            //if (DataService.Databases.Count > 1)
            //{
                //have to use the namespace
                namespaceUsing = "using " + Utility.CheckNamingRules(tbl.Provider) + ";";
            //}
            pageTemplate = pageTemplate.Replace(TemplateVariable.NAMESPACE_USING, namespaceUsing);

            //Set the class name
            string className = tbl.ClassName;
            pageTemplate = pageTemplate.Replace(TemplateVariable.CLASS_NAME, className);

            //Set the page class name
            string pageClass = pageName.Replace(FileExtension.DOT_ASPX, String.Empty);
            pageTemplate = pageTemplate.Replace(TemplateVariable.PAGE_CLASS, pageClass);


            //now loop out the columns for each row of the table editor and grid
            StringBuilder bindRows = new StringBuilder();
            StringBuilder pageBindRows = new StringBuilder();
            StringBuilder dropList = new StringBuilder();

            foreach (TableSchema.TableColumn col in tbl.Columns)
            {
                string controlName = col.PropertyName;

                if (!col.IsPrimaryKey)
                {
                    if(col.IsForeignKey)
                    {
                        //dropdown lists

                        TableSchema.Table FKTable = DataService.GetForeignKeyTable(col, tbl);
                        if(FKTable != null)
                        {
                            dropList.AppendLine(AddTabs(2) + "Query qry" + controlName + " = " + FKTable.ClassName + ".CreateQuery();");
                            dropList.AppendLine(AddTabs(2) + "qry" + controlName + ".OrderBy = OrderBy.Asc(\"" + FKTable.Columns[1].ColumnName + "\");");
                            dropList.AppendLine(AddTabs(2) + "Utility.LoadDropDown(" + controlName + ", " + "qry" + controlName + ".ExecuteReader(), true);");
                            if(col.IsNullable)
                            {
                                dropList.AppendLine(AddTabs(2) + controlName + ".Items.Insert(0, new ListItem(\"(Not Specified)\", String.Empty));");
                            }
                        }
                    }
                    pageBindRows.AppendLine(SetControlValue(col, className));
                    if(col.DataType != DbType.Binary && col.DataType != DbType.Byte)
                    {
                        bindRows.AppendLine(GetControlValue(col, className));
                    }
                }
            }
            
            pageTemplate = pageTemplate.Replace(TemplateVariable.BIND_LIST, bindRows.ToString());
            pageTemplate = pageTemplate.Replace(TemplateVariable.PAGE_BIND_LIST, pageBindRows.ToString());
            pageTemplate = pageTemplate.Replace(TemplateVariable.DROP_LIST, dropList.ToString());
            

            return pageTemplate;

        }
        #endregion
    }
}