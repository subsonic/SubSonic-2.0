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

namespace SubSonic
{
    /// <summary>
    /// Summary for the SpecialString class
    /// </summary>
    public class SpecialString
    {
        public const string AGO = "ago";
        public const string DAY = "day";
        public const string HOUR = "hour";
        public const string MINUTE = "minute";
        public const string MONTH = "month";
        public const string SECOND = "second";
        public const string SPACE = " ";
        public const string YEAR = "year";
    }

    /// <summary>
    /// Summary for the TemplateName class
    /// </summary>
    public class TemplateName
    {
		public const string ENUM = "EnumTemplate";
		public const string CLASS = "ClassTemplate";
		public const string DYNAMIC_SCAFFOLD = "DynamicScaffold";
        public const string GENERATED_SCAFFOLD_CODE_BEHIND = "GeneratedScaffoldCodeBehind";
        public const string GENERATED_SCAFFOLD_MARKUP = "GeneratedScaffoldMarkup";
        public const string ODS_CONTROLLER = "ODSController";
        public const string STORED_PROCEDURE = "SPTemplate";
        public const string STRUCTS = "StructsTemplate";

        public const string VIEW = "ViewTemplate";
    }
    /// <summary>
    /// Summary for the TemplateVariable class
    /// </summary>
    public class TemplateVariable
    {
        public const string ARGUMENT_LIST = "#ARGLIST#";
        public const string BIND_LIST = "#BINDLIST#";
        public const string CLASS_NAME = "#CLASSNAME#";
        public const string CLASS_NAME_COLLECTION = "#CLASSNAMECOLLECTION#";
        public const string COLUMNS_STRUCT = "#COLUMNSSTRUCT#";
        public const string CONTROL_PROPERTY = "#CONTROLPROPERTY#";
        public const string DROP_LIST = "#DROPLIST#";
        public const string EDITOR_ROWS = "#EDITORROWS#";
        public const string FK_VAR = "#FKVAR#";
        public const string FOREIGN_CLASS = "#FOREIGNCLASS#";
        public const string FOREIGN_PK = "#FOREIGNPK#";
        public const string FOREIGN_TABLE = "#FOREIGNTABLE#";
        public const string GETTER = "#GETTER#";
        public const string GRID_ROWS = "#GRIDROWS#";
        public const string INSERT = "#INSERT#";
        public const string JAVASCRIPT_BLOCK = "#JAVASCRIPTBLOCK#";
        public const string LANGUAGE = "#LANGUAGE#";
        public const string LANGUAGE_EXTENSION = "#LANGEXTENSION#";
        public const string MANY_METHODS = "#MANYMETHODS#";
        public const string MAP_TABLE = "#MAPTABLE#";
        public const string MASTER_PAGE = "#MASTERPAGE#";
        public const string METHOD_BODY = "#METHODBODY#";
        public const string METHOD_LIST = "#METHODLIST#";
        public const string METHOD_NAME = "#METHODNAME#";
        public const string METHOD_TYPE = "#METHODTYPE#";
        public const string NAMESPACE_USING = "#NAMESPACE_USING#";
        public const string PAGE_BIND_LIST = "#PAGEBINDLIST#";
        public const string PAGE_CLASS = "#PAGECLASS#";
        public const string PAGE_CODE = "#PAGECODE#";
        public const string PAGE_FILE = "#PAGEFILE#";
        public const string PARAMETERS = "#PARAMS#";
        public const string PK = "#PK#";
        public const string PK_PROP = "#PKPROP#";
        public const string PK_VAR = "#PKVAR#";
        public const string PROPERTY_LIST = "#PROPLIST#";
        public const string PROPERTY_NAME = "#PROPNAME#";
        public const string PROPERTY_TYPE = "#PROPTYPE#";
        public const string PROVIDER = "#PROVIDER#";
        public const string SET_LIST = "#SETLIST#";
        public const string SETTER = "#SETTER#";
        public const string STORED_PROCEDURE_NAME = "#SPNAME#";
        public const string STRUCT_ASSIGNMENTS = "#STRUCTASSIGNMENTS#";
        public const string STRUCT_LIST = "#STRUCTLIST#";
        public const string SUMMARY = "#SUMMARY#";
        public const string TABLE = "#TABLE#";
        public const string TABLE_NAME = "#TABLENAME#";
        public const string TABLE_SCHEMA = "#TABLESCHEMA#";
        public const string UPDATE = "#UPDATE#";
    }

    /// <summary>
    /// Summary for the ReservedColumnName class
    /// </summary>
    public class ReservedColumnName
    {
        public const string CREATED_BY = "CreatedBy";
        public const string CREATED_ON = "CreatedOn";
        public const string DELETED = "Deleted";
        public const string IS_ACTIVE = "IsActive";
        public const string IS_DELETED = "IsDeleted";
        public const string MODIFIED_BY = "ModifiedBy";
        public const string MODIFIED_ON = "ModifiedOn";
    }

    /// <summary>
    /// Summary for the ConfigurationSectionName class
    /// </summary>
    public class ConfigurationSectionName
    {
        public const string PROVIDERS = "providers";
        public const string SUB_SONIC_SERVICE = "SubSonicService";
    }

    /// <summary>
    /// Summary for the ConfigurationPropertyName class
    /// </summary>
    public class ConfigurationPropertyName
    {        
        public const string ADDITIONAL_NAMESPACES = "additionalNamespaces";
        public const string APPEND_WITH = "appendWith";
        public const string CONNECTION_STRING_NAME = "connectionStringName";
        public const string DEFAULT_PROVIDER = "defaultProvider";
        public const string ENABLE_TRACE = "enableTrace";
        public const string EXCLUDE_PROCEDURE_LIST = "excludeProcedureList";
		public const string EXCLUDE_TABLE_LIST = "excludeTableList";
		public const string ENUM_INCLUDE_LIST = "enumIncludeList";
		public const string ENUM_EXCLUDE_LIST = "enumExcludeList";
		public const string ENUM_SHOW_DEBUG_INFO = "enumShowDebugInfo";
		public const string EXTRACT_CLASS_NAME_FROM_SP_NAME = "extractClassNameFromSPName";
        public const string FIX_DATABASE_OBJECT_CASING = "fixDatabaseObjectCasing";
        public const string FIX_PLURAL_CLASS_NAMES = "fixPluralClassNames";
        public const string GENERATE_LAZY_LOADS = "generateLazyLoads";
        public const string GENERATE_NULLABLE_PROPERTIES = "generateNullableProperties";
        public const string GENERATE_ODS_CONTROLLERS = "generateODSControllers";
        public const string GENERATE_RELATED_TABLES_AS_PROPERTIES = "generateRelatedTablesAsProperties";
        public const string GENERATED_NAMESPACE = "generatedNamespace";
        public const string INCLUDE_PROCEDURE_LIST = "includeProcedureList";
        public const string INCLUDE_TABLE_LIST = "includeTableList";
        public const string MANY_TO_MANY_SUFFIX = "manyToManySuffix";
        public const string PROVIDER_TO_USE = "provider";
        public const string REGEX_DICTIONARY_REPLACE = "regexDictionaryReplace";
        public const string REGEX_IGNORE_CASE = "regexIgnoreCase";
        public const string REGEX_MATCH_EXPRESSION = "regexMatchExpression";
        public const string REGEX_REPLACE_EXPRESSION = "regexReplaceExpression";
        public const string RELATED_TABLE_LOAD_PREFIX = "relatedTableLoadPrefix";
        public const string REMOVE_UNDERSCORES = "removeUnderscores";
        public const string SET_PROPERTY_DEFAULTS_FROM_DATABASE = "setPropertyDefaultsFromDatabase";
        public const string SP_STARTS_WITH = "spStartsWith";
        public const string STORED_PROCEDURE_BASE_CLASS = "spBaseClass";
        public const string STORED_PROCEDURE_CLASS_NAME = "spClassName";
        public const string STRIP_COLUMN_TEXT = "stripColumnText";
        public const string STRIP_PARAM_TEXT = "stripParamText";
        public const string STRIP_STORED_PROCEDURE_TEXT = "stripSPText";
        public const string STRIP_TABLE_TEXT = "stripTableText";
        public const string STRIP_VIEW_TEXT = "stripViewText";
        public const string TABLE_BASE_CLASS = "tableBaseClass";
        public const string TEMPLATE_DIRECTORY = "templateDirectory";
        public const string USE_EXTENDED_PROPERTIES = "useExtendedProperties"; //SQL 2000/2005 Only
        public const string USE_STORED_PROCEDURES = "useSPs";
        public const string USE_UTC_TIMES = "useUtc";
        public const string VIEW_BASE_CLASS = "viewBaseClass";
        public const string VIEW_STARTS_WITH = "viewStartsWith";
        public const string GROUP_OUTPUT = "groupOutput";
    }

    /// <summary>
    /// Summary for the DataProviderTypeName class
    /// </summary>
    public class DataProviderTypeName
    {
        public const string ENTERPRISE_LIBRARY = "ELib3DataProvider";
        public const string MY_SQL = "MySqlDataProvider";
        public const string ORACLE = "OracleDataProvider";
        public const string SQL_CE = "SqlCEProvider";
        public const string SQL_SERVER = "SqlDataProvider";
        public const string SQLITE = "SQLiteDataProvider";
        public const string VISTADB = "VistaDBDataProvider";
        public const string MSACCESS = "AccessDataProvider";
    }

    /// <summary>
    /// Summary for the ClassName class
    /// </summary>
    public class ClassName
    {
        public const string STORED_PROCEDURES = "SPs";
        public const string TABLES = "Tables";
        public const string VIEWS = "Views";
    }

    /// <summary>
    /// Summary for the CodeBlock class
    /// </summary>
    public class CodeBlock
    {
        public static readonly string JS_BEGIN_SCRIPT = "<script language=\"javascript\" type=\"text/javascript\">" + Environment.NewLine;
        public static readonly string JS_END_SCRIPT = "</script>" + Environment.NewLine;
    }

    /// <summary>
    /// Summary for the FileExtension class
    /// </summary>
    public static class FileExtension
    {
        public const string ASPX = "aspx";
        public const string CS = "cs";
        public const string DOT_ASPX = ".aspx";
        public const string DOT_CS = ".cs";
        public const string DOT_VB = ".vb";
        public const string VB = "vb";
        public const string VB_DOT_NET = "vb.net";
    }

    /// <summary>
    /// Summary for the ControlValueProperty class
    /// </summary>
    public class ControlValueProperty
    {
        public const string CALENDAR = "SelectedDate";
        public const string CHECK_BOX = "Checked";
        public const string DROP_DOWN_LIST = "SelectedValue";
        public const string LABEL = "Text";
        public const string TEXT_BOX = "Text";
    }

    /// <summary>
    /// 
    /// </summary>
    public class AggregateFunctionName
    {
        public const string AVERAGE = "AVG";
        public const string COUNT = "COUNT";
        public const string MAX = "MAX";
        public const string MIN = "MIN";
        public const string SUM = "SUM";
    }

    /// <summary>
    /// Summary for the SqlFragment class
    /// </summary>
    public class SqlFragment
    {
        public const string AND = " AND ";
        public const string AS = " AS ";
        public const string ASC = " ASC";
        public const string BETWEEN = " BETWEEN ";
        public const string CROSS_JOIN = " CROSS JOIN ";
        public const string DELETE_FROM = "DELETE FROM ";
        public const string DESC = " DESC";
        public const string DISTINCT = "DISTINCT ";
        public const string EQUAL_TO = " = ";
        public const string FROM = " FROM ";
        public const string GROUP_BY = " GROUP BY ";
        public const string HAVING = " HAVING ";
        public const string IN = " IN ";

        public const string INNER_JOIN = " INNER JOIN ";

        public const string INSERT_INTO = "INSERT INTO ";
        public const string JOIN_PREFIX = "J";
        public const string LEFT_INNER_JOIN = " LEFT INNER JOIN ";
        public const string LEFT_JOIN = " LEFT JOIN ";
        public const string LEFT_OUTER_JOIN = " LEFT OUTER JOIN ";
        public const string NOT_EQUAL_TO = " <> ";
        public const string NOT_IN = " NOT IN ";
        public const string ON = " ON ";
        public const string OR = " OR ";
        public const string ORDER_BY = " ORDER BY ";
        public const string OUTER_JOIN = " OUTER JOIN ";
        public const string RIGHT_INNER_JOIN = " RIGHT INNER JOIN ";
        public const string RIGHT_JOIN = " RIGHT JOIN ";
        public const string RIGHT_OUTER_JOIN = " RIGHT OUTER JOIN ";
        public const string SELECT = "SELECT ";
        public const string SET = " SET ";
        public const string SPACE = " ";
        public const string TOP = "TOP ";
        public const string UNEQUAL_JOIN = " JOIN ";
        public const string UPDATE = "UPDATE ";
        public const string WHERE = " WHERE ";
    }

    /// <summary>
    /// Summary for the SqlComparison class
    /// </summary>
    public class SqlComparison
    {
        public const string BLANK = " ";
        public const string EQUAL = " = ";
        public const string GREATER = " > ";
        public const string GREATER_OR_EQUAL = " >= ";
        public const string IN = " IN ";
        public const string IS = " IS ";
        public const string IS_NOT = " IS NOT ";
        public const string LESS = " < ";
        public const string LESS_OR_EQUAL = " <= ";
        public const string LIKE = " LIKE ";
        public const string NOT_EQUAL = " <> ";
        public const string NOT_IN = " NOT IN ";
        public const string NOT_LIKE = " NOT LIKE ";
    }

    /// <summary>
    /// Summary for the SqlSchemaVariable class
    /// </summary>
    public class SqlSchemaVariable
    {
        public const string COLUMN_DEFAULT = "DefaultSetting";
        public const string COLUMN_NAME = "ColumnName";
        public const string CONSTRAINT_TYPE = "constraintType";
        public const string DATA_TYPE = "DataType";
        public const string DEFAULT = "DEFAULT";
        public const string FOREIGN_KEY = "FOREIGN KEY";
        public const string IS_COMPUTED = "IsComputed";
        public const string IS_IDENTITY = "IsIdentity";
        public const string IS_NULLABLE = "IsNullable";
        public const string MAX_LENGTH = "MaxLength";
        public const string MODE = "mode";
        public const string MODE_INOUT = "INOUT";
        public const string NAME = "Name";
        public const string NUMERIC_PRECISION = "NumericPrecision";
        public const string NUMERIC_SCALE = "NumericScale";
        public const string PARAMETER_PREFIX = "@";
        public const string PRIMARY_KEY = "PRIMARY KEY";
        public const string TABLE_NAME = "TableName";
    }

    /// <summary>
    /// Summary for the OracleSchemaVariable class
    /// </summary>
    public class OracleSchemaVariable
    {
        public const string COLUMN_NAME = "COLUMN_NAME";
        public const string CONSTRAINT_TYPE = "CONSTRAINT_TYPE";
        public const string DATA_TYPE = "DATA_TYPE";
        public const string IS_NULLABLE = "NULLABLE";
        public const string MAX_LENGTH = "CHAR_LENGTH";
        public const string MODE = "IN_OUT";
        public const string MODE_INOUT = "IN/OUT";
        public const string NAME = "ARGUMENT_NAME";
        public const string NUMBER_PRECISION = "DATA_PRECISION";
        public const string NUMBER_SCALE = "DATA_SCALE";
        public const string PARAMETER_PREFIX = ":";
        public const string TABLE_NAME = "TABLE_NAME";
    }

    /// <summary>
    /// Summary for the MySqlSchemaVariable class
    /// </summary>
    public class MySqlSchemaVariable
    {
        public const string PARAMETER_PREFIX = "?";
    }

    /// <summary>
    /// Summary for the AccessSchemaVariable class
    /// </summary>
    public class AccessSchemaVariable {
        // OleDB Schema Cols
        public const string TABLE_TYPE = "TABLE_TYPE";
        public const string TABLE_NAME = "TABLE_NAME";
        public const string COLUMN_NAME = "COLUMN_NAME";
        public const string ORDINAL_POSITION = "ORDINAL_POSITION";
        public const string DATA_TYPE = "DATA_TYPE";
        public const string COLUMN_DEFAULT = "COLUMN_DEFAULT";
        public const string MAX_LENGTH = "CHARACTER_MAXIMUM_LENGTH";
        public const string IS_NULLABLE = "IS_NULLABLE";
        public const string FK_COLUMN_NAME = "FK_COLUMN_NAME";
        public const string FK_TABLE_NAME = "FK_TABLE_NAME";
        public const string PK_COLUMN_NAME = "PK_COLUMN_NAME";
        public const string PK_TABLE_NAME = "PK_TABLE_NAME";
        public const string COLUMN_FLAGS = "COLUMN_FLAGS";
        public const string PROCEDURE_NAME = "PROCEDURE_NAME";
        public const string PROCEDURE_DEF = "PROCEDURE_DEFINITION";

        public const string DEFAULT = "DEFAULT";

        // Addl Cols manufactured from Access DAO
        public const string ALLOW_EMPTY_STRING = "ALLOW_EMPTY_STRING";
        public const string AUTO_INCREMENT = "AUTO_INCREMENT";
        // Addl calculated cols
        public const string PK_TYPE = "PK_TYPE";
        // Stored Proc Columns
        public const string SP_SCHEMA = "SPSchema";
        public const string SP_NAME = "SPName";
        public const string SP_PARAM_ORDINALPOS = "OrdinalPosition";
        public const string SP_PARAM_MODE = "mode";
        public const string SP_PARAM_NAME = "Name";
        public const string SP_PARAM_DBDATATYPE = "DataType";
        public const string SP_PARAM_DATALENGTH = "DataLength";
        public const string SP_PARAM_PREFIX = "";
        // Addl cols from external sources
        public const string EXTPROP_IS_IDENTITY = "ColumnIsAutoNum";
        public const string EXTPROP_ALLOW_EMPTYSTRING = "ColumnAllowZeroLengthString";

		public const string GEN_PARAM_PREFIX = "PARM__";	
    }

	public class AccessSql {
		public const string GET_INT_IDENTITY = "SELECT @@IDENTITY;";
    }

    /// <summary>
    /// Summary for the ServerVariable class
    /// </summary>
    public class ServerVariable
    {
        public const string SERVER_NAME = "SERVER_NAME";
        public const string SERVER_PORT = "SERVER_PORT";
        public const string SERVER_PORT_SECURE = "SERVER_PORT_SECURE";
    }

    /// <summary>
    /// Summary for the Ports class
    /// </summary>
    public class Ports
    {
        public const string HTTP = "80";
        public const string HTTPS = "443";
    }

    /// <summary>
    /// Summary for the ProtocolPrefix class
    /// </summary>
    public class ProtocolPrefix
    {
        public const string HTTP = "http://";
        public const string HTTPS = "https://";
    }

    /// <summary>
    /// Summary for the CodeFragment class
    /// </summary>
    public class CodeFragment
    {
        public const string NULLABLE_VARIABLE = "?";
        public const string NULLABLE_VARIABLE_VB = "Nullable(Of {0})";
    }

    /// <summary>
    /// Summary for the ScaffoldCss class
    /// </summary>
    public class ScaffoldCss
    {
        public const string BUTTON = "scaffoldButton";
        public const string CHECK_BOX = "scaffoldCheckBox";
        public const string DROP_DOWN = "scaffoldDropDown";
        public const string EDIT_ITEM = "scaffoldEditItem";
        public const string EDIT_ITEM_CAPTION = "scaffoldEditItemCaption";
        public const string EDIT_TABLE = "scaffoldEditTable";
        public const string EDIT_TABLE_LABEL = "scaffoldEditTableLabel";
        public const string GRID = "scaffoldGrid";
        public const string TEXT_BOX = "scaffoldTextBox";
        public const string WRAPPER = "scaffoldWrapper";
    }

    /// <summary>
    /// Summary for the RegexPattern class
    /// </summary>
    public class RegexPattern
    {
        public const string ALPHA = "[^a-zA-Z]";
        public const string ALPHA_NUMERIC = "[^a-zA-Z0-9]";
        public const string ALPHA_NUMERIC_SPACE = @"[^a-zA-Z0-9\s]";
        public const string CREDIT_CARD_AMERICAN_EXPRESS = @"^(?:(?:[3][4|7])(?:\d{13}))$";
        public const string CREDIT_CARD_CARTE_BLANCHE = @"^(?:(?:[3](?:[0][0-5]|[6|8]))(?:\d{11,12}))$";
        public const string CREDIT_CARD_DINERS_CLUB = @"^(?:(?:[3](?:[0][0-5]|[6|8]))(?:\d{11,12}))$";
        public const string CREDIT_CARD_DISCOVER = @"^(?:(?:6011)(?:\d{12}))$";
        public const string CREDIT_CARD_EN_ROUTE = @"^(?:(?:[2](?:014|149))(?:\d{11}))$";
        public const string CREDIT_CARD_JCB = @"^(?:(?:(?:2131|1800)(?:\d{11}))$|^(?:(?:3)(?:\d{15})))$";
        public const string CREDIT_CARD_MASTER_CARD = @"^(?:(?:[5][1-5])(?:\d{14}))$";
        public const string CREDIT_CARD_STRIP_NON_NUMERIC = @"(\-|\s|\D)*";
        public const string CREDIT_CARD_VISA = @"^(?:(?:[4])(?:\d{12}|\d{15}))$";
        public const string EMAIL = @"^([0-9a-zA-Z]+[-._+&])*[0-9a-zA-Z]+@([-0-9a-zA-Z]+[.])+[a-zA-Z]{2,6}$";
        public const string EMBEDDED_CLASS_NAME_MATCH = "(?<=^_).*?(?=_)";
        public const string EMBEDDED_CLASS_NAME_REPLACE = "^_.*?_";
        public const string EMBEDDED_CLASS_NAME_UNDERSCORE_MATCH = "(?<=^UNDERSCORE).*?(?=UNDERSCORE)";
        public const string EMBEDDED_CLASS_NAME_UNDERSCORE_REPLACE = "^UNDERSCORE.*?UNDERSCORE";
        public const string GUID = "[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}";
        public const string IP_ADDRESS = @"^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
        public const string LOWER_CASE = @"^[a-z]+$";
        public const string NUMERIC = "[^0-9]";
        public const string SOCIAL_SECURITY = @"^\d{3}[-]?\d{2}[-]?\d{4}$";
        public const string SQL_EQUAL = @"\=";
        public const string SQL_GREATER = @"\>";
        public const string SQL_GREATER_OR_EQUAL = @"\>.*\=";
        public const string SQL_IS = @"\x20is\x20";
        public const string SQL_IS_NOT = @"\x20is\x20not\x20";
        public const string SQL_LESS = @"\<";
        public const string SQL_LESS_OR_EQUAL = @"\<.*\=";
        public const string SQL_LIKE = @"\x20like\x20";
        public const string SQL_NOT_EQUAL = @"\<.*\>";
        public const string SQL_NOT_LIKE = @"\x20not\x20like\x20";

        public const string STRONG_PASSWORD =
            @"(?=^.{8,255}$)((?=.*\d)(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[^A-Za-z0-9])(?=.*[a-z])|(?=.*[^A-Za-z0-9])(?=.*[A-Z])(?=.*[a-z])|(?=.*\d)(?=.*[A-Z])(?=.*[^A-Za-z0-9]))^.*";

        public const string UPPER_CASE = @"^[A-Z]+$";
        public const string URL = @"^^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&%\$#_=]*)?$";
        public const string US_CURRENCY = @"^\$(([1-9]\d*|([1-9]\d{0,2}(\,\d{3})*))(\.\d{1,2})?|(\.\d{1,2}))$|^\$[0](.00)?$";
        public const string US_TELEPHONE = @"^[01]?[- .]?(\([2-9]\d{2}\)|[2-9]\d{2})[- .]?\d{3}[- .]?\d{4}$";
        public const string US_ZIPCODE = @"^\d{5}$";
        public const string US_ZIPCODE_PLUS_FOUR = @"^\d{5}((-|\s)?\d{4})$";
        public const string US_ZIPCODE_PLUS_FOUR_OPTIONAL = @"^\d{5}((-|\s)?\d{4})?$";
    }

    /// <summary>
    /// Summary for the ExtendedPropertyName class
    /// </summary>
    public class ExtendedPropertyName
    {
        public const string SSX_COLUMN_BINARY_FILE_EXTENSION = "SSX_COLUMN_BINARY_FILE_EXTENSION";
        public const string SSX_COLUMN_DISPLAY_NAME = "SSX_COLUMN_DISPLAY_NAME";
        public const string SSX_COLUMN_PROPERTY_NAME = "SSX_COLUMN_PROPERTY_NAME";
        public const string SSX_TABLE_CLASS_NAME_PLURAL = "SSX_TABLE_CLASS_NAME_PLURAL";
        public const string SSX_TABLE_CLASS_NAME_SINGULAR = "SSX_TABLE_CLASS_NAME_SINGULAR";
        public const string SSX_TABLE_DISPLAY_NAME = "SSX_TABLE_DISPLAY_NAME";
    }
}