using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Data.Common;
using SubSonic.Utilities;
using System.Text.RegularExpressions; 

namespace SubSonic
{
    public class AccessDataProvider: DataProvider
    {
        // Provider for Access 2003.
        // Script generation is implemented although currently useless (DDL is only partially capable of DDL operations)
        // No paging.
        //-----------------------------------------------------------------------------------------------------------------------//

        public const bool ThrowExceptionOnDefaultIsExpression = true;

        #region .ctor

        public AccessDataProvider() {
            this.dbAllowsMultipleStatement = false;
            this.dbAllowsSpOutputParam = false;
            this.dbRequiresBracketedJoins = true;
            this.dbSupportsITransactionLocal = false;
        }
        
        #endregion

        private const string EXTENDED_PROPERTIES_TABLES =
            @"SELECT TableName, TableExtNameSingular, TableExtNamePlural FROM [_lib_TableMetaData]";

        private const string EXTENDED_PROPERTIES_COLUMNS =
            @"SELECT TableName, ColumnName, ColumnExtName, ColumnIsAutoNum, ColumnAllowZeroLengthString
            FROM [_lib_ColumnMetaData]";

        private static readonly object _lockPK = new object();
        private static readonly object _lockFK = new object();
        private static readonly object _lockIndex = new object();
        private static readonly object _lockColumns = new object();
        private static readonly object _lockTables = new object();
        private static readonly object _lockManyToManyCheck = new object();
        private static readonly object _lockManyToManyMap = new object();
        private static readonly object _lockExtendedProperties = new object();
        private static readonly object _lockSPs = new object();
        private static readonly object _lockViews = new object();

        private static bool extendedPropsLoaded = false;
        
        private static readonly DataSet dsExtendedProperties = new DataSet();
        private static readonly DataSet dsColumns = new DataSet();
        private static readonly DataSet dsFK = new DataSet();
        private static readonly DataSet dsPK = new DataSet();
        private static readonly DataSet dsSPParams = new DataSet();
        private static ListDictionary arSP = new ListDictionary();
        private static ListDictionary arViews = new ListDictionary();
        private static ListDictionary arTables = new ListDictionary();

        /// <summary>
        /// Gets the type of the named provider.
        /// </summary>
        /// <value>The type of the named provider.</value>
        public override string NamedProviderType {
            get { return DataProviderTypeName.MSACCESS; }
        }

        private string GetAccessDBNameAndPathFromConnectionString(string connectionString)
        {
            string lookFor = "Data Source";
            foreach (string s in connectionString.Split(new char[] { ';' }))
            {
                if (s.StartsWith(lookFor + "=", StringComparison.InvariantCultureIgnoreCase))
                    return s.Substring(lookFor.Length + 1);
            }
            return "";
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <returns></returns>
        public override DbConnection CreateConnection()
        {
            return CreateConnection(DefaultConnectionString);
        }

        /// <summary>
        /// Creates the connection.
        /// </summary>
        /// <param name="newConnectionString">The new connection string.</param>
        /// <returns></returns>
        public override DbConnection CreateConnection(string newConnectionString)
        {
            OleDbConnection retVal = new OleDbConnection(newConnectionString);
            retVal.Open();
            return retVal;
        }

        /// <summary>
        /// Adds the params.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="qry">The qry.</param>
        private void AddParams(OleDbCommand oleCmd, QueryCommand qryCmd)
        {
            // For parameters in MS Access, it is necessary for parameters to be declared at the start of the statement :
            //   eg. PARAMETERS [cat 1] Text(255), [comp 2] Text(255);
            //       SELECT CategoryName, [comp 2] as x1, [cat 1] as x2, [comp 2] as x3 From Categories WHERE CategoryName=[cat 1];
            //
            // If this declaration is not done, the command object's parameters are simply assigned in order to SQL parameters as they are 
            // encountered without regard to name.  Hence the ability to maintain the meaning of parameters through the parameter name and to 
            // repeat a parameter is lost.
            //
            // It is assumed that this declaration is done already for Stored Procedures, since we don't have the SQL at hand anyway.
            // We need to add the declaration to the SQL for Text commands.

                string paramDeclaration = String.Empty ;
                bool addParamDeclaration = (qryCmd.CommandType == CommandType.Text);
            if(qryCmd.Parameters != null)
            {

                foreach(QueryParameter param in qryCmd.Parameters)
                {
                    if (addParamDeclaration) {
                        DaoDataTypeEnum daoDataType = GetDAOTypeFromDbType(param.DataType);
                        string ddlDataType = GetDDLTypeNameFromDAOType(daoDataType);
                        paramDeclaration += (paramDeclaration == String.Empty ? "" : ", ") + FormatParameterNameForSQL(param.ParameterName) + " " + ddlDataType;
                    }

                    OleDbParameter sqlParam = new OleDbParameter(FormatParameterNameForSQL(param.ParameterName), GetOleDBType(param.DataType));
                    sqlParam.Direction = param.Mode;

                    //output parameters need to define a size
                    //our default is 50
                    if(sqlParam.Direction == ParameterDirection.Output || sqlParam.Direction == ParameterDirection.InputOutput)
                    {
                        sqlParam.Size = param.Size;
                    }

                    //Debug.WriteLine(param.ParameterName + ", " + param.Size + ", " + param.DataType + ", " + sqlParam.Size + ", " + sqlParam.DbType);

                    //fix for NULLs as parameter values
                    if(param.ParameterValue == null) // || Utility.IsMatch(param.ParameterValue.ToString(), "null"))
                    {
                        sqlParam.Value = DBNull.Value;
                    }
                    else if(param.DataType == DbType.Guid)
                    {
                        string paramValue = param.ParameterValue.ToString();
                        if(!String.IsNullOrEmpty(paramValue))
                        {
                            if(!Utility.IsMatch(paramValue, AccessSchemaVariable.DEFAULT))
                            {
                                sqlParam.Value = new Guid(param.ParameterValue.ToString());
                            }
                        }
                        else
                        {
                            sqlParam.Value = DBNull.Value;
                        }
                    }
                    else
                    {
                        sqlParam.Value = param.ParameterValue;
                    }

                    oleCmd.Parameters.Add(sqlParam);

                }

                // if needed, add the parameters declaration to the start of the command
                if (addParamDeclaration && paramDeclaration != string.Empty) {
                    oleCmd.CommandText = "PARAMETERS " + paramDeclaration + "; " + oleCmd.CommandText;
                }
            }

            // Print Parameters
            string s = "qryCmd Params: " + Environment.NewLine;

            int i = 1;
            foreach(QueryParameter prop in qryCmd.Parameters) {
                object val = prop.ParameterValue;
                if (val == null) val = DBNull.Value;
                s += i++.ToString() + ". " + prop.ParameterName + ": " + prop.DataType.ToString() + ": " + val.ToString() + Environment.NewLine;
            }

            //s += "oleCmd Params: " + Environment.NewLine;
            //i = 1;
            //foreach (OleDbParameter prop in oleCmd.Parameters) {
            //    s += i++.ToString() + ". " + prop.ParameterName + ": " + prop.OleDbType.ToString() + ": " + prop.Value.ToString() + Environment.NewLine;
            //}

            s += "cmd.CommandText: " + oleCmd.CommandText;
        }

        /// <summary>
        /// Checkouts the output params.
        /// </summary>
        /// <param name="cmd">The CMD.</param>
        /// <param name="qry">The qry.</param>
        private static void CheckoutOutputParams(OleDbCommand cmd, QueryCommand qry)
        {
            // MS Access does not support output parameters

            //if (qry.CommandType == CommandType.StoredProcedure && qry.HasOutputParams())
            //{
            //    //loop the params, getting the values and setting them for the return
            //    foreach (QueryParameter param in qry.Parameters)
            //    {
            //        if (param.Mode == ParameterDirection.InputOutput || param.Mode == ParameterDirection.Output || param.Mode == ParameterDirection.ReturnValue)
            //        {
            //            object oVal = cmd.Parameters[param.ParameterName].Value;
            //            param.ParameterValue = oVal;
            //            qry.OutputValues.Add(oVal);
            //        }
            //    }
            //}
        }

        #region Jet/OleDb/System Type conversion functions

        /// <summary>
        /// Return the OleDBType for a given DbType.
        /// </summary>
        /// <param name="dbType">The dbType</param>
        /// <returns>OleDbType</returns>
        public static OleDbType GetOleDBType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.AnsiString:
                    return OleDbType.VarChar;
                case DbType.AnsiStringFixedLength:
                    return OleDbType.Char;
                case DbType.Binary:
                    return OleDbType.VarBinary;
                case DbType.Boolean:
                    return OleDbType.Boolean;
                case DbType.Byte:
                    return OleDbType.TinyInt;
                case DbType.Currency:
                    return OleDbType.Decimal;
                case DbType.Date:
                    return OleDbType.Date;
                case DbType.DateTime:
                    return OleDbType.Date; // maps to System.DateTime
                case DbType.Decimal:
                    return OleDbType.Decimal;
                case DbType.Double:
                    return OleDbType.Double;
                case DbType.Guid:
                    return OleDbType.Guid;
                case DbType.Int16:
                    return OleDbType.SmallInt;
                case DbType.Int32:
                    return OleDbType.Integer;
                case DbType.Int64:
                    return OleDbType.BigInt;
                case DbType.Object:
                    return OleDbType.Variant;
                case DbType.SByte:
                    return OleDbType.TinyInt;
                case DbType.Single:
                    return OleDbType.Single;
                case DbType.String:
                    return OleDbType.VarWChar; // maps to System.String;
                case DbType.StringFixedLength:
                    return OleDbType.VarChar;
                case DbType.Time:
                    return OleDbType.DBTime;
                case DbType.UInt16:
                    return OleDbType.UnsignedSmallInt;
                case DbType.UInt32:
                    return OleDbType.UnsignedInt;
                case DbType.UInt64:
                    return OleDbType.UnsignedBigInt;
                case DbType.VarNumeric:
                    return OleDbType.Decimal;

                default:
                    return OleDbType.VarChar;
            }
        }


        // From : http://allenbrowne.com/ser-49.html - thanx Allen Browne !
        // see the original page for a _lot_ more detail for all Access vers
        //
        // There are several historical interfaces to access, and here they are :
        // (note: The complex data types require Access 2007 or later, and cannot be created 
        //  with DDL or ADOX.  DAO is the native API for Access.)
        //
        //   Jet		     DDL		                DAO Constant	            ADOX Constant
        //  (GUI)		     (SQL)	                (enum, dec, hex)            (enum, dec, hex)
        //  --------------   ---------------------   -------------------------   ----------------------------------
        //  Text             TEXT (size)            dbText	           10  0A   adVarWChar	              202	CA 	
        //                                          dbComplexText	  109  6D	 	 	 
        //                   CHAR (size)	        dbText 	           10  0A	adWChar	                  130	82
        //  Memo	         MEMO	                dbMemo	           12  0C	adLongVarWChar	          203	CB
        //  Number: Byte     BYTE	                dbByte	            2  02	adUnsignedTinyInt	       17	11
        //                                          dbComplexByte	  102  66	 	 	 
        //  Number: Integer	 SHORT	                dbInteger	        3  03	adSmallInt	                2	02
        //                                          dbComplexInteger  103  67	 	 	 
        //  Number: Long	 LONG	                dbLong	            4  04	adInteger	                3	03
        //                                          dbComplexLong	  104  68	 	 	 
        //  Number: Single	 SINGLE	                dbSingle	        6  06	adSingle	                4	04
        //                                          dbComplexSingle	  105  69	 	 	 
        //  Number: Double	 DOUBLE	                dbDouble	        7  07	adDouble	                5	05
        //                                          dbComplexDouble	  106  6A	 	 	 
        //  Number: Replica	 GUID	                dbGUID	           15  0F	adGUID	                   72	48
        //                                          dbComplexGUID	  107  6B	 	 	 
        //  Number: Decimal	 DECIMAL (prec, scale)  dbDecimal	       20  14	adNumeric	              131	83
        //                                          dbComplexDecimal  108  6C	 	 	 
        //  Date/Time	     DATETIME	            dbDate	            8  08	adDate	                    7	07
        //  Currency	     CURRENCY	            dbCurrency	        5  05	adCurrency	                6	06
        //  Auto Number	     COUNTER (seed, inc)	dbLong with attr    4  04	adInteger with attr	        3	03
        //  Yes/No	         YESNO	                dbBoolean	        1  01	adBoolean	               11	0B
        //  OLE Object	     LONGBINARY	            dbLongBinary	   11  0B	adLongVarBinary	          205	CD
        //  Hyperlink		                        dbMemo with attr   12  0C	adLongVarWChar with attr  203	CB
        //  Attachment	 	                        dbAttachment	  101  65	 	 	 
        //  	             BINARY (size)	        dbBinary	        9  09	adVarBinary	              204   CC

        public enum DaoDataTypeEnum
        {
            dbBoolean   =    1,
            dbByte      =    2,
            dbInteger   =    3,
            dbLong      =    4,
            dbBigInt    =   16,

            dbCurrency  =    5,
            dbDecimal   =   20,
            dbNumeric   =   19,
            dbSingle    =    6,
            dbDouble    =    7,
            dbFloat     =   21,

            dbChar      =   18,
            dbText      =   10,
            dbMemo      =   12,

            dbDate      =    8,
            dbTime      =   22,
            dbTimeStamp =   23,

            dbGUID      =   15,
            dbBinary    =    9,
            dbLongBinary =  11,
            dbVarBinary =   17
        }

        /// <summary>
        /// Gets the type of the native.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        /// <returns></returns>
        public static DaoDataTypeEnum GetDAOTypeFromDbType(DbType dbType)
        {
            switch (dbType)
            {
                case DbType.Boolean:
                    return DaoDataTypeEnum.dbBoolean;
                case DbType.SByte:
                case DbType.Byte:
                    return DaoDataTypeEnum.dbByte;
                case DbType.UInt16:
                case DbType.Int16:
                    return DaoDataTypeEnum.dbInteger;
                case DbType.UInt32:
                case DbType.Int32:
                    return DaoDataTypeEnum.dbLong;
                case DbType.UInt64:
                case DbType.Int64:
                    return DaoDataTypeEnum.dbBigInt;
                case DbType.Single:
                    return DaoDataTypeEnum.dbSingle;
                case DbType.Double:
                case DbType.VarNumeric:
                    return DaoDataTypeEnum.dbDouble;
                case DbType.Decimal:
                    return DaoDataTypeEnum.dbDecimal;
                case DbType.Currency:
                    return DaoDataTypeEnum.dbCurrency;

                case DbType.Date:
                case DbType.DateTime:
                    return DaoDataTypeEnum.dbDate;
                case DbType.Time:
                    return DaoDataTypeEnum.dbTime;
                
                case DbType.Object:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    return DaoDataTypeEnum.dbText;

                case DbType.Binary:
                    return DaoDataTypeEnum.dbLongBinary;
                case DbType.Guid:
                    return DaoDataTypeEnum.dbGUID;
                case DbType.Xml:
                    return DaoDataTypeEnum.dbMemo;

                default:
                    return DaoDataTypeEnum.dbText;
            }
        }

        /// <summary>
        /// Return the DbType for a given JET database type.
        /// </summary>
        /// <param name="adoxDataType">The JET data type</param>
        /// <returns>DbType</returns>
        public override DbType GetDbType(string adoxDataTypeNumericAsString)
        {
            /*
            Access data types (filtered thru OLEDB):

            Short	    2	5			        System.Int16	
            Long	    3	10			        System.Int32	
            Single	    4	7			        System.Single	
            Double	    5	15			        System.Double	
            Currency	6	19			        System.Decimal	
            DateTime	7	8			        System.DateTime	
            Bit	        11	2		            System.Boolean	
            Byte	    17	3			        System.Byte	            	
            GUID	    72	16			        System.Guid	            	
            BigBinary	204	4000			    System.Byte[]	    	
            LongBinary	205	1073741823			System.Byte[]		
            VarBinary	204	510		max length	System.Byte[]		
            LongText	203	536870910			System.String		
            VarChar	    202	255		max length	System.String		
            Decimal	    131	28		precision,scale	System.Decimal
            */

            //ADOX.DataTypeEnum.adBigInt;
            //ADOX.DataTypeEnum.adBinary;
            //ADOX.DataTypeEnum.adBoolean;
            //ADOX.DataTypeEnum.adBSTR;
            //ADOX.DataTypeEnum.adChapter;
            //ADOX.DataTypeEnum.adChar ;
            //ADOX.DataTypeEnum.adCurrency;
            //ADOX.DataTypeEnum.adDate;
            //ADOX.DataTypeEnum.adDBDate;
            //ADOX.DataTypeEnum.adDBTime;
            //ADOX.DataTypeEnum.adDBTimeStamp;
            //ADOX.DataTypeEnum.adDecimal;
            //ADOX.DataTypeEnum.adDouble;
            //ADOX.DataTypeEnum.adEmpty;

            switch (Int32.Parse(adoxDataTypeNumericAsString))
            {
                case (0): // "Empty" 
                    return DbType.String;
                case (2): // "SmallInt" 
                    return DbType.Int16;
                case (3): // "Integer" 
                    return DbType.Int32;
                case (4): // "Real" 
                    return DbType.Single;
                case (5): // "Double" 
                    return DbType.Double;
                case (6): // "Currency" 
                    return DbType.Currency;
                case (7): // "Date" 
                    return DbType.DateTime;
                case (8): // "BSTR" 
                    return DbType.String;
                case (9): // "IDispatch" 
                    return DbType.String;
                case (10): // "Error Code" 
                    return DbType.Int32;
                case (11): // "Boolean" 
                    return DbType.Boolean;
                case (12): // "Variant" 
                    return DbType.String;
                case (13): // "IUnknown" 
                    return DbType.String;
                case (14): // "Decimal" 
					return DbType.Double; // ** Decimal type not supported in DAO **
                case (16): // "TinyInt" 
                    return DbType.Int16;
                case (17): // "Unsigned TinyInt (BYTE)" 
                    return DbType.Byte;
                case (18): // "Unsigned Small Int (WORD)" 
                    return DbType.Int16;
                case (19): // "Unsigned Int (DWORD)" 
                    return DbType.Int32;
                case (20): // "BigInt" 
                    return DbType.Int64;
                case (21): // "Unsigned Big Int" 
                    return DbType.Int64;
                case (64): // "FileTime" 
                    return DbType.DateTime;
                case (72): // "Unique Identifier (GUID)" 
                    return DbType.Guid;
                case (128): // "Binary" 
                    return DbType.Binary;
                case (129): // "Char" 
                    return DbType.Byte;
                case (130): // "nChar" 
                    return DbType.String;
                case (131): // "Numeric" 
					return DbType.Double; // ** Decimal type not supported in DAO **
                case (132): // "User Defined (UDT)" 
                    return DbType.DateTime;
                case (133): // "DBDate" 
                    return DbType.DateTime;
                case (134): // "DBTime" 
                    return DbType.DateTime;
                case (135): // "SmallDateTime" 
                    return DbType.DateTime;
                case (136): // "Chapter" 
                    return DbType.String;
                case (138): // "Automation (PropVariant)" 
                    return DbType.Object;
                case (139): // "VarNumeric" 
                    return DbType.VarNumeric;
                case (200): // "VarChar" 
                    return DbType.String;
                case (201): // "Text" 
                    return DbType.String;
                case (202): // "nVarChar" 
                    return DbType.String;
                case (203): // "nText" 
                    return DbType.String;
                case (204): // "VarBinary" 
                    return DbType.Binary;
                case (205): // "Image" 
                    return DbType.Binary;
                default:
                    return DbType.String;

            }

        } // GetDbType

        /// <summary>
        /// Return the DbType for a given DAO DataType.
        /// </summary>
        /// <param name="DaoDataType">The DAO DataType</param>
        /// <returns>DbType</returns>
        public static DbType GetDbTypeFromDAOType(DaoDataTypeEnum daoDataType)
        {
            // Access DAO data types conversion to OleDb data types - Ben Mc
            switch (daoDataType)
            {
                case DaoDataTypeEnum.dbBoolean:    return DbType.Boolean;           // dbBoolean = 1 
                case DaoDataTypeEnum.dbByte:       return DbType.Byte ;   // dbByte = 2 
                case DaoDataTypeEnum.dbInteger:    return DbType.Int16;          // dbInteger = 3 
                case DaoDataTypeEnum.dbLong:       return DbType.Int32;           // dbLong = 4 
                case DaoDataTypeEnum.dbBigInt:     return DbType.Int64;            // dbBigInt = 16 

                case DaoDataTypeEnum.dbCurrency:   return DbType.Currency;          // dbCurrency = 5 
                case DaoDataTypeEnum.dbDecimal:    return DbType.Decimal;           // dbDecimal = 20 
                case DaoDataTypeEnum.dbNumeric:    return DbType.Double;           // dbNumeric = 19 
                case DaoDataTypeEnum.dbSingle:     return DbType.Single;            // dbSingle = 6 
                case DaoDataTypeEnum.dbDouble:     return DbType.Double;            // dbDouble = 7 
                case DaoDataTypeEnum.dbFloat:      return DbType.Double;           // dbFloat = 21 

                case DaoDataTypeEnum.dbChar:       return DbType.String;              // dbChar = 18 
                case DaoDataTypeEnum.dbText:       return DbType.String;           // dbText = 10 
                case DaoDataTypeEnum.dbMemo:       return DbType.String;          // dbMemo = 12 

                case DaoDataTypeEnum.dbDate:       return DbType.Date;              // dbDate = 8 
                case DaoDataTypeEnum.dbTime:       return DbType.Time;              // dbTime = 22 
                case DaoDataTypeEnum.dbTimeStamp:  return DbType.Date;       // dbTimeStamp = 23 

                case DaoDataTypeEnum.dbGUID:       return DbType.Guid;              // dbGUID = 15 
                case DaoDataTypeEnum.dbBinary:     return DbType.Binary;            // dbBinary = 9 
                case DaoDataTypeEnum.dbLongBinary: return DbType.Binary;            // dbLongBinary = 11 
                case DaoDataTypeEnum.dbVarBinary:  return DbType.Binary;            // dbVarBinary = 17 
                default:
                    return DbType.String;
            }
        }

        /// <summary>
        /// Return the DDL data type for a given DAO DataType.
        /// </summary>
        /// <param name="DaoDataType">The DAO DataType</param>
        /// <returns>OleDbType</returns>
        public static string GetDDLTypeNameFromDAOType(DaoDataTypeEnum daoDataType)
        {
            // Access DAO data types conversion to Access DDL data type names - Ben Mc
            switch (daoDataType)
            {
                case DaoDataTypeEnum.dbBoolean:    return "YESNO";             // dbBoolean = 1 
                case DaoDataTypeEnum.dbByte:       return "BYTE";              // dbByte = 2 
                case DaoDataTypeEnum.dbInteger:    return "SHORT";             // dbInteger = 3 
                case DaoDataTypeEnum.dbLong:       return "LONG";              // dbLong = 4 
                case DaoDataTypeEnum.dbBigInt:     return "unsupported";       // dbBigInt = 16 

                case DaoDataTypeEnum.dbCurrency:   return "CURRENCY";          // dbCurrency = 5 
                case DaoDataTypeEnum.dbDecimal:    return "DECIMAL(28, 0)";    // dbDecimal = 20 
                case DaoDataTypeEnum.dbNumeric:    return "DECIMAL(28, 0)";    // dbNumeric = 19 
                case DaoDataTypeEnum.dbSingle:     return "SINGLE";            // dbSingle = 6 
                case DaoDataTypeEnum.dbDouble:     return "DOUBLE";            // dbDouble = 7 
                case DaoDataTypeEnum.dbFloat:      return "DOUBLE";            // dbFloat = 21 

                case DaoDataTypeEnum.dbChar:       return "CHAR(255)";         // dbChar = 18 
                case DaoDataTypeEnum.dbText:       return "TEXT(255)";         // dbText = 10 
                case DaoDataTypeEnum.dbMemo:       return "MEMO";              // dbMemo = 12 

                case DaoDataTypeEnum.dbDate:       return "DATETIME";          // dbDate = 8 
                case DaoDataTypeEnum.dbTime:       return "DATETIME";          // dbTime = 22 
                case DaoDataTypeEnum.dbTimeStamp:  return "DATETIME";          // dbTimeStamp = 23 

				case DaoDataTypeEnum.dbGUID:	   return "UNIQUEIDENTIFIER";  // dbGUID = 15 
                case DaoDataTypeEnum.dbBinary:     return "BINARY(255)";       // dbBinary = 9 
                case DaoDataTypeEnum.dbLongBinary: return "LONGBINARY";        // dbLongBinary = 11 
                case DaoDataTypeEnum.dbVarBinary:  return "BINARY(255)";       // dbVarBinary = 17 
                default:
                    return "unsupported";
            }
        } 

        #endregion

        /// <summary>
        /// Qualify table name according to RDBMS format (eg. '[table]')
        /// </summary>
        public override string QualifyTableName(string schemaName, string tableName)
        {
            return FormatIdentifier(tableName);
        }

        /// <summary>
        /// Qualify column name according to RDBMS format (eg. '[table].[column]')
        /// </summary>
        public override string QualifyColumnName(string schemaName, string tableName, string columnName)
        {
            return (string.IsNullOrEmpty(tableName) ? "" : FormatIdentifier(tableName) + ".")
                + FormatIdentifier(columnName);
        }


        /// <summary>
        /// Format the Parameter for inclusion in SQL (eg. @param_name for MSSQL, [param_name] for MSAccess)
        /// Only use this in the final marshalling for SQL insertion, NOT in initial formatting of the parameter name.
        /// </summary>
        public override string FormatParameterNameForSQL(string parameterName)
        {
            string prefix = GetParameterPrefix();
			if (!parameterName.StartsWith(prefix) && !parameterName.StartsWith("[") && !parameterName.StartsWith("[" + prefix)) {
				parameterName = prefix + parameterName;
			}
            return FormatIdentifier(parameterName);
        }


        /// <summary>
        /// Format the Parameter name to ensure it is valid
        /// </summary>
		public override string PreformatParameterName(string parameterName)
        {
			return parameterName;
        }

        /// <summary>
        /// Filter out elements of the SQL for unit testing
        /// </summary>
		public override string FilterTestSQL(string sqlString)
        {
            return sqlString.Replace("[dbo].", "").Replace("@", "").Replace("(", "").Replace(")", "");
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="rdr">The RDR.</param>
        /// <param name="par">The par.</param>
        public override void SetParameter(IDataReader rdr, StoredProcedure.Parameter par)
        {
            par.DBType = (DbType) rdr[AccessSchemaVariable.SP_PARAM_DBDATATYPE];
            string sMode = rdr[AccessSchemaVariable.SP_PARAM_MODE].ToString();
            par.Name = rdr[AccessSchemaVariable.SP_PARAM_NAME].ToString();
        }

        /// <summary>
        /// Gets the parameter prefix.
        /// </summary>
        /// <returns></returns>
		public override string GetParameterPrefix()
        {
			return AccessSchemaVariable.GEN_PARAM_PREFIX;
        }

        /// <summary>
        /// Delimits the name of the db.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns></returns>
        public override string FormatIdentifier(string columnName)
        {
			if (String.IsNullOrEmpty(columnName)) { return String.Empty; }

			if (!columnName.StartsWith("[") && !columnName.EndsWith("]")) {
                return "[" + columnName + "]";
            }
			return columnName;
        }

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
		public override IDataReader GetReader(QueryCommand qry) {
			AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this);
			OleDbCommand cmd = new OleDbCommand(qry.CommandSql);
			cmd.CommandType = qry.CommandType;
			cmd.CommandTimeout = qry.CommandTimeout;
			AddParams(cmd, qry);

			cmd.Connection = (OleDbConnection)automaticConnectionScope.Connection;
			//let this bubble up
			IDataReader rdr;

			//Thanks jcoenen!
			try {
				// if it is a shared connection, we shouldn't be telling the reader to close it when it is done
				if (automaticConnectionScope.IsUsingSharedConnection)
					rdr = cmd.ExecuteReader();
				else
					rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
			}
			catch (OleDbException) {
				// AutoConnectionScope will figure out what to do with the connection
				automaticConnectionScope.Dispose();
				//rethrow retaining stack trace.
				throw;
			}
			CheckoutOutputParams(cmd, qry);

			return rdr;
		}

        /// <summary>
        /// Gets the single record reader.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDataReader GetSingleRecordReader(QueryCommand qry)
        {
            AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this);
            OleDbCommand cmd = new OleDbCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;
            AddParams(cmd, qry);

            cmd.Connection = (OleDbConnection)automaticConnectionScope.Connection;
            IDataReader rdr;

            try
            {
                // if it is a shared connection, we shouldn't be telling the reader to close it when it is done
                if (automaticConnectionScope.IsUsingSharedConnection)
                    rdr = cmd.ExecuteReader(CommandBehavior.SingleRow);
                else
                    rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleRow);
            }
            catch (OleDbException)
            {
                // AutoConnectionScope will figure out what to do with the connection
                automaticConnectionScope.Dispose();
                //rethrow retaining stack trace.
                throw;
            }
            CheckoutOutputParams(cmd, qry);

            return rdr;
        }

        /// <summary>
        /// Gets the data set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override T GetDataSet<T>(QueryCommand qry)
        {
            T ds = new T();
            if(qry.CommandType == CommandType.Text)
            {
                qry.CommandSql = qry.CommandSql;
            }
            OleDbCommand cmd = new OleDbCommand(qry.CommandSql);
            cmd.CommandType = qry.CommandType;
            cmd.CommandTimeout = qry.CommandTimeout;
            OleDbDataAdapter da = new OleDbDataAdapter(cmd);

            AddTableMappings(da, ds);
            using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
            {
                cmd.Connection = (OleDbConnection)conn.Connection;
                AddParams(cmd, qry);
                da.Fill(ds);

                CheckoutOutputParams(cmd, qry);

                return ds;
            }
        }

		private string GetSqlBeforeIdentityFetchSql(string sql, ref bool selectIdentity) {
			// if the identity select clause follows the main SQL, trim it and run a second command
			string commandSql = sql;
			selectIdentity = commandSql.EndsWith(";" + AccessSql.GET_INT_IDENTITY);
			if (selectIdentity) { commandSql = commandSql.Substring(0, commandSql.Length - AccessSql.GET_INT_IDENTITY.Length); }
			return commandSql;
		}

        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override object ExecuteScalar(QueryCommand qry) {
			bool selectIdentity = false;
			string commandSql = GetSqlBeforeIdentityFetchSql(qry.CommandSql, ref selectIdentity);

			using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
            {
				OleDbCommand cmd = new OleDbCommand(commandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout;
                AddParams(cmd, qry);
                cmd.Connection = (OleDbConnection)automaticConnectionScope.Connection;
                object result = cmd.ExecuteScalar();

                // Run an additional command to return a generated identity value
				if (selectIdentity) {
					OleDbCommand idCmd = new OleDbCommand(AccessSql.GET_INT_IDENTITY);
                    idCmd.CommandTimeout = qry.CommandTimeout;
                    idCmd.Connection = (OleDbConnection)automaticConnectionScope.Connection;
                    result = idCmd.ExecuteScalar();
                }

                CheckoutOutputParams(cmd, qry);

                return result;
            }
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override int ExecuteQuery(QueryCommand qry)
        {
			bool selectIdentity = false;
			string commandSql = GetSqlBeforeIdentityFetchSql(qry.CommandSql, ref selectIdentity);

            using(AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
            {
				OleDbCommand cmd = new OleDbCommand(commandSql);
                cmd.CommandType = qry.CommandType;
                cmd.CommandTimeout = qry.CommandTimeout;

                AddParams(cmd, qry);
                cmd.Connection = (OleDbConnection)automaticConnectionScope.Connection;
                int result = cmd.ExecuteNonQuery();
                CheckoutOutputParams(cmd, qry);
                return result;
            }
        }

        /// <summary>
        /// Loads the extended property data set.
        /// </summary>
        private void LoadExtendedPropertyDataSet()
        {
            if (dsExtendedProperties.Tables[Name] == null)
            {
                lock (_lockExtendedProperties)
                {
                    if (dsExtendedProperties.Tables[Name] == null)
                    {
                        QueryCommand cmdExtProps = new QueryCommand(EXTENDED_PROPERTIES_TABLES, Name);
                        DataTable dt = new DataTable(Name);
                        dt.Load(GetReader(cmdExtProps));
                        dsExtendedProperties.Tables.Add(dt);
                    }
                }
            }

            if (dsExtendedProperties.Tables[Name + "_Columns"] == null)
            {
                lock (_lockExtendedProperties)
                {
                    if (dsExtendedProperties.Tables[Name + "_Columns"] == null)
                    {
                        QueryCommand cmdExtProps = new QueryCommand(EXTENDED_PROPERTIES_COLUMNS, Name);
                        DataTable dt = new DataTable(Name + "_Columns");
                        dt.Load(GetReader(cmdExtProps));
                        dsExtendedProperties.Tables.Add(dt);

                        if(dt.Rows.Count >0)
                            extendedPropsLoaded = true;
                    }
                }
            }
        }

        /// <summary>
        /// Sets the extended table properties.
        /// </summary>
        /// <param name="tblSchema">The TBL schema.</param>
        private void SetExtendedTableProperties(TableSchema.AbstractTableSchema tblSchema)
        {
            if (UseExtendedProperties)
            {
                DataRow[] drTableProps = null;
                LoadExtendedPropertyDataSet();

                DataTable dtExtProps = dsExtendedProperties.Tables[Name];
                drTableProps = dtExtProps.Select("TableName ='" + tblSchema.TableName + "'");

                if (drTableProps != null) {
                    if (drTableProps.Length != 1) {
                        throw new Exception("AccessDataProvider Extended Properties must consist of exactly one row !");
                    }

                    for (int i = 0; i < dtExtProps.Columns.Count; i++)
                        tblSchema.ExtendedProperties.Add(new TableSchema.ExtendedProperty(
                            dtExtProps.Columns[i].ColumnName, drTableProps[0][i].ToString())
                        );

                    tblSchema.ApplyExtendedProperties();
                }
            }
        }

        /// <summary>
        /// Sets the extended column properties.
        /// </summary>
        /// <param name="tblSchema">The TBL schema.</param>
        /// <param name="col">The col.</param>
        private void SetExtendedColumnProperties(TableSchema.AbstractTableSchema tblSchema, TableSchema.TableColumn col)
        {
            if (UseExtendedProperties)
            {
                DataRow[] drColumnProps = null;
                LoadExtendedPropertyDataSet();

                DataTable dtExtProps = dsExtendedProperties.Tables[Name + "_Columns"];
                drColumnProps = dtExtProps.Select("TableName ='" + tblSchema.TableName + "' AND ColumnName = '" + col.ColumnName + "'");

                if (drColumnProps != null)
                {
                    if (drColumnProps.Length != 1)
                    {
                        throw new Exception("AccessDataProvider Extended Column Properties must consist of exactly one row !");
                    }

                    for (int i = 0; i < dtExtProps.Columns.Count; i++)
                        col.ExtendedProperties.Add(new TableSchema.ExtendedProperty(
                            dtExtProps.Columns[i].ColumnName, drColumnProps[0][i].ToString())
                        );

                    col.ApplyExtendedProperties();
                }
            }
        }

        //    !!!! Empty properties : [0]TABLE_CATALOG  [1]TABLE_SCHEMA  [4]COLUMN_GUID  [5]COLUMN_PROPID  [12]TYPE_GUID  
        //        [16]NUMERIC_SCALE  [18]CHARACTER_SET_CATALOG  [19]CHARACTER_SET_SCHEMA  [20]CHARACTER_SET_NAME  
        //        [21]COLLATION_CATALOG  [22]COLLATION_SCHEMA  [23]COLLATION_NAME  [24]DOMAIN_CATALOG  [25]DOMAIN_SCHEMA  
        //        [26]DOMAIN_NAME  

        //TABLE_NAME                     COLUMN_NAME               ORDINAL_POSITION  COLUMN_HASDEFAULT  COLUMN_DEFAULT  
        //[2]String                      [3]String                 [6]Int64          [7]Boolean         [8]String       

        //    COLUMN_FLAGS  IS_NULLABLE  DATA_TYPE  CHARACTER_MAXIMUM_LENGTH  CHARACTER_OCTET_LENGTH  NUMERIC_PRECISION  
        //    [9]Int64      [10]Boolean   [11]Int32   [13]Int64                  [14]Int64                [15]Int32          

        //    DATETIME_PRECISION  DESCRIPTION                                         
        //    [17]Int64            [27]String                                           



        //    !!!! Empty properties : [0]TABLE_CATALOG  [1]TABLE_SCHEMA  [3]INDEX_CATALOG  [4]INDEX_SCHEMA  [18]COLUMN_GUID  
        //    [19]COLUMN_PROPID  [23]FILTER_CONDITION  

        //TABLE_NAME         INDEX_NAME             PRIMARY_KEY  UNIQUE  CLUSTERED  TYPE  FILL_FACTOR  INITIAL_SIZE  
        //    NULLS  SORT_BOOKMARKS  AUTO_UPDATE  NULL_COLLATION  ORDINAL_POSITION  COLUMN_NAME         
        //    COLLATION  CARDINALITY  PAGES  INTEGRATED  

        /// <summary>
        /// Force-reloads a provider's schema
        /// </summary>
        public override void ReloadSchema() {
            if (dsColumns != null) { dsColumns.Tables.Clear(); }
            if (dsExtendedProperties != null) { dsExtendedProperties.Tables.Clear(); }
            if (dsFK != null) { dsFK.Tables.Clear(); }
            if (dsPK != null) { dsPK.Tables.Clear(); }
            //dsIndex.Tables.Clear();
            //dsManyToManyCheck.Tables.Clear();
            //dsManyToManyMap.Tables.Clear();
            //if (dtParamSql != null) dtParamSql.Clear();
        }


        /// <summary>
        /// Load up the PK and FK metadata datatables (or do nothing if already loaded).
        /// </summary>
        /// <returns></returns>
        public void CachePkFkData()
        {
            if (dsPK.Tables[Name] == null || dsFK.Tables[Name] == null) {
                using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
                {
                    OleDbConnection autoOleDbConn = automaticConnectionScope.Connection as OleDbConnection;

                    // lazy load raw PK metadata into dsPK.Tables[Name]
                    if (dsPK.Tables[Name] == null)
                    {
                        lock (_lockPK)
                        {
                            if (dsPK.Tables[Name] == null)
                            {
                                DataTable dt = autoOleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Primary_Keys, null);
                                dt.TableName = Name;

                                // Set PK_TYPE to SV=SingleValue or COMP=Composite depending on if the PK table occurs more
                                // more than once in the list.
                                dt.Columns.Add(new DataColumn(AccessSchemaVariable.PK_TYPE, typeof(string)));
                                
                                for (int i = 0; i < dt.Rows.Count ; i++) {
                                    bool occursMoreThanOnce = false;
                                    for (int j = 0; j < dt.Rows.Count ; j++) {
                                        if (i != j || dt.Rows[i][AccessSchemaVariable.TABLE_NAME] == dt.Rows[j][AccessSchemaVariable.TABLE_NAME])
                                            occursMoreThanOnce = true;
                                    }
                                    dt.Rows[i][AccessSchemaVariable.PK_TYPE] = occursMoreThanOnce ? "COMP" : "SV";
                                }
                                dt.AcceptChanges();

                                dsPK.Tables.Add(dt);
                            }
                        }
                    }

                    // lazy load raw FK metadata into dsFK.Tables[Name]
                    if (dsFK.Tables[Name] == null)
                    {
                        lock (_lockFK)
                        {
                            if (dsFK.Tables[Name] == null)
                            {
                                DataTable dt = autoOleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Foreign_Keys, null);
                                dt.TableName = Name;
                                dsFK.Tables.Add(dt);
                            }
                        }
                    }
                }
            }
        }

        private bool StringInArray(string[] strArray, string s)
        {
            for (int i = 0; i < strArray.Length; i++) {
                if (strArray[i] == s) return true;
            }
            return false;
        }

        /// <summary>
        /// Load up the Table metadata datatables (or do nothing if already loaded).
        /// </summary>
        /// <returns></returns>
        public void CacheColumnData()
        {
            CacheTableNameList();
            string[] tempTableList = (string[])arTables[Name];

            if (dsColumns.Tables[Name] == null)
            {
                using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
                {
                    OleDbConnection autoOleDbConn = automaticConnectionScope.Connection as OleDbConnection;

                    // Use DAO to extract parameters 
                    DAO.DBEngineClass dbc = new DAO.DBEngineClass();
                    DAO.Database db = dbc.OpenDatabase(
                        GetAccessDBNameAndPathFromConnectionString(autoOleDbConn.ConnectionString)
                        , null, false, "");

                    // lazy load raw column metadata into dsColumns.Tables[Name]
                    lock (_lockColumns)
                    {
                        if (dsColumns.Tables[Name] == null)
                        {
                            DataTable dt = autoOleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Columns, null);
                            dt.TableName = Name;

                            // Use Access DAO to fill in the missing schema info
                            // Store it here to preserve caching of the schema dataset
                            dt.Columns.Add(new DataColumn(AccessSchemaVariable.ALLOW_EMPTY_STRING, typeof(bool)));
                            dt.Columns.Add(new DataColumn(AccessSchemaVariable.AUTO_INCREMENT, typeof(bool)));
                            foreach (DataRow dr in dt.Rows)
                            {
                                string tempTableName = (string) dr[AccessSchemaVariable.TABLE_NAME];
                                bool tempAllowEmptyString = true;
                                bool tempAutoInc = false;

                                if (StringInArray(tempTableList, tempTableName))  {
                                    // if this is a table, extract addl DAO info
                                    DAO.Field dbField = db.TableDefs[tempTableName].Fields[dr[AccessSchemaVariable.COLUMN_NAME]];
                                    tempAllowEmptyString = dbField.AllowZeroLength;
                                    tempAutoInc = ((dbField.Attributes & (int)DAO.FieldAttributeEnum.dbAutoIncrField) != 0);
                                }

                                dr[AccessSchemaVariable.ALLOW_EMPTY_STRING] = tempAllowEmptyString;
                                dr[AccessSchemaVariable.AUTO_INCREMENT] = tempAutoInc;
                            }
                            dt.AcceptChanges();

                            dsColumns.Tables.Add(dt);
                        }
                    }

                    db.Close();
                }
            }
        }

        private string FixAccessColumnDefault(string s)
        {

            // deal with '=date()'

            // defaults allowed: [num], "[string]", #[date string]#, ="[string]", =#[date string]#, =[num]
            // any default starting with an equals sign other that the above formats is not allowed
            // returning an empty string indicates failure
            if (Regex.IsMatch(s, @""".*"""))  {
                return s.Substring(2, s.Length - 2);
            }
            else if (Regex.IsMatch(s, @"="".*""")) {
                return s.Substring(3, s.Length - 3);
            }
            else if (Regex.IsMatch(s, @"#.*#")) {
                return s.Substring(2, s.Length - 2);
            }
            else if (Regex.IsMatch(s, @"=#.*#")) {
                return s.Substring(3, s.Length - 3);
            }
            else if (Regex.IsMatch(s, @"=[0-9\.]+")) {
                return s.Substring(2);
            }
            else if (s == "=Date()" || s == "=Time()" || s == "=Now()") {
                return s.Substring(2);
            }
            else if (s == "Date()" || s == "Time()" || s == "Now()") {
                return s;
            }
            else if (Regex.IsMatch(s, @"=.+\(.*"))  {
                return "";
            }
            else if (Regex.IsMatch(s, @".+\(.*"))   {
                return "";
            }
            else {
                return s;
            }
        }

        /// <summary>
        /// Gets the table schema.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="tableType">Type of the table.</param>
        /// <returns></returns>
        public override TableSchema.Table GetTableSchema(string tableName, TableType tableType)
        {
            // create TableSchema.Table object, ForeignKeys collection, get extended props
            TableSchema.TableColumnCollection columns = new TableSchema.TableColumnCollection();
            TableSchema.Table tbl = new TableSchema.Table(tableName, tableType, this);

            SetExtendedTableProperties(tbl);
            tbl.ForeignKeys = new TableSchema.ForeignKeyTableCollection();

            // For Access there is actually no way to determine whether a column is an Autonumber (the
            // equivalent of a MSSQL identity).  OleDb gives the most detailed info, but not this one piece
            // of information.
            // An urban myth is circulating that COLUMN_FLAGS = 90 and DATA_TYPE = 3 (Int) means an Autonumber,
            // but these conditions are met for any non-nullable Long field (of which an existing Autonumber 
            // will certainly be one).
            //
            // In addition, Access has the 'Allow Zero Length String' property, which is unique among DBMS's AFAIK.
            // This could cause errors, so I'll attempt to have it added to the Schema class and check for it in 
            // ActiveRecord.  It will be 'true' for all DBMS's apart from Access.
            //
            // With this release, I have included an Access code module which creates and keeps current metadata 
            // tables containing this additional info. The 'CreateBasicColumnAddlInfoTable' sub in this module
            // should be run on Access startup to keep metadata up to date.  The SubSonic table schema 
            // ExtendedProperties are configured to load this data.
            //
            // Hence if bool 'extendedPropsLoaded' :
            //    false = no AutoNumber data available, use the first Non-Null Long field and hope for the best.
            //    true = Access columns/schema Extended Properties 'ColumnIsAutoNum' and 'ColumnAllowZeroLengthString'
            //            contain the missing two metadata items.
            //
            // It would be luverly to use DBConnection.GetSchema, but we simply don't get enough info
            // from it (FK's are the deal-breaker), so we'll be using GetOleDbSchemaTable here.  
            // This forces users of Access to use OleDb, which shouldn't be too much of an issue since it's the 
            // best option anyway.

            using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
            {
                OleDbConnection autoOleDbConn = automaticConnectionScope.Connection as OleDbConnection;
                
                // Chuck a tantie if our connection is not of the OleDb variety
                if (autoOleDbConn == null) {
                    throw new Exception("AccessDataProvider uses only an OleDb Connection");
                }

                // COLUMNS
                CacheColumnData();

                // Iterate columns ordered by ordinal position
                DataRow[] drColumns = dsColumns.Tables[Name].Select(
                    AccessSchemaVariable.TABLE_NAME + "=" + q(tableName), 
                    AccessSchemaVariable.ORDINAL_POSITION + " ASC"
                );

                bool haveAssignedAutoNumberColumn = false;

                for (int i = 0; i < drColumns.Length; i++)
                {
//                    Debug.WriteLine(drColumns[i]["COLUMN_NAME"] + ": flags = " + drColumns[i]["COLUMN_FLAGS"] + "   type = " + drColumns[i]["DATA_TYPE"]);

                    TableSchema.TableColumn column = new TableSchema.TableColumn(tbl);
                    string columnName = drColumns[i][AccessSchemaVariable.COLUMN_NAME].ToString();
                    column.ColumnName = columnName;
                    string adoxDataType = drColumns[i][AccessSchemaVariable.DATA_TYPE].ToString();
                    column.DataType = GetDbType(adoxDataType);

                    if (drColumns[i][AccessSchemaVariable.COLUMN_DEFAULT] != DBNull.Value)
                    {
                        string tempDefaultSetting = drColumns[i][AccessSchemaVariable.COLUMN_DEFAULT].ToString().Trim();
                        string defaultSetting = FixAccessColumnDefault(tempDefaultSetting);

                        if (ThrowExceptionOnDefaultIsExpression && defaultSetting == string.Empty)
                            throw new Exception("AccessDataProvider: Table " + q(tableName) + " Column " + q(column.ColumnName) 
                                + " has an invalid default '" + tempDefaultSetting + "'.  Expressions containing functions cannot be specified as defaults");
                        
                        if (defaultSetting != string.Empty) column.DefaultSetting = defaultSetting;
                    }

                    int maxLength;
                    int.TryParse(drColumns[i][AccessSchemaVariable.MAX_LENGTH].ToString(), out maxLength);
                    column.MaxLength = maxLength;
                    column.IsNullable = Convert.ToBoolean(drColumns[i][AccessSchemaVariable.IS_NULLABLE]);
                    column.IsReadOnly = false; // no read only columns
                    //column.AllowEmptyString = bool.Parse(drColumns[i][AccessSchemaVariable.ALLOW_EMPTY_STRING].ToString());
                    column.AutoIncrement = bool.Parse(drColumns[i][AccessSchemaVariable.AUTO_INCREMENT].ToString());

                    columns.Add(column);

                    tbl.SchemaName = string.Empty;
                    SetExtendedColumnProperties(tbl, column);

                    //// not sure if I have to do this, but it has been added to the collection so I'm being on the safe side
                    //column = columns.GetColumn(columnName);

                    //// Work out if this is an AutoNumber column using Extended props if available
                    //bool isIdentity = false;
                    //bool allowEmptyString = false;

                    //if (extendedPropsLoaded) {
                    //    string isIdentityString = SubSonic.TableSchema.ExtendedProperty.GetExtendedProperty(
                    //        column.ExtendedProperties, AccessSchemaVariable.EXTPROP_IS_IDENTITY).PropertyValue;
                    //    isIdentity = Convert.ToBoolean(isIdentityString);

                    //    string allowEmptyStringString = SubSonic.TableSchema.ExtendedProperty.GetExtendedProperty(
                    //        column.ExtendedProperties, AccessSchemaVariable.EXTPROP_ALLOW_EMPTYSTRING).PropertyValue;
                    //    allowEmptyString = Convert.ToBoolean(allowEmptyStringString);
                    //} else {   
                    //    if (!haveAssignedAutoNumberColumn
                    //     && Convert.ToInt32(drColumns[i][AccessSchemaVariable.COLUMN_FLAGS]) == 90
                    //     && Convert.ToInt32(drColumns[i][AccessSchemaVariable.DATA_TYPE]) == 3)
                    //    {
                    //        isIdentity = true;
                    //        haveAssignedAutoNumberColumn = true;
                    //    }
                    //}


                }

                // TABLE PRIMARY KEYS
                CachePkFkData();
                // Iterate PKs for this table and mark column IsPrimaryKey = true
                DataRow[] drPK = dsPK.Tables[Name].Select(AccessSchemaVariable.TABLE_NAME + "=" + q(tableName));
                for (int i = 0; i < drPK.Length; i++)
                {
                    string columnName = drPK[i][AccessSchemaVariable.COLUMN_NAME].ToString();
                    TableSchema.TableColumn column = columns.GetColumn(columnName);
                    column.IsPrimaryKey = true;
                }

                // FOREIGN KEYS
                CachePkFkData();

                // Iterate FKs for which this table is the FK table and 
                //  - mark column: IsForeignKey = true
                //  - add info to ForeignKeys collection about the related primary tables
                DataRow[] drFK = dsFK.Tables[Name].Select(AccessSchemaVariable.FK_TABLE_NAME + "=" + q(tableName));
                for (int i = 0; i < drFK.Length; i++)
                {
                    string columnName = drFK[i][AccessSchemaVariable.FK_COLUMN_NAME].ToString();
                    TableSchema.TableColumn column = columns.GetColumn(columnName);
                    column.IsForeignKey = true;
					column.ForeignKeyTableName = drFK[i][AccessSchemaVariable.PK_TABLE_NAME].ToString();

                    TableSchema.ForeignKeyTable fkTable = new TableSchema.ForeignKeyTable(this);
                    fkTable.ColumnName = columnName;
                    fkTable.TableName = drFK[i][AccessSchemaVariable.PK_TABLE_NAME].ToString();
                    fkTable.PrimaryColumnName = drFK[i][AccessSchemaVariable.PK_COLUMN_NAME].ToString();
                    fkTable.ForeignColumnName = drFK[i][AccessSchemaVariable.FK_COLUMN_NAME].ToString();
                    SetExtendedTableProperties(fkTable);
                    tbl.ForeignKeys.Add(fkTable);
                }

                // Iterate FKs for which this table is the PK table and 
                //  - add to PrimaryKeyTables collection
                //  - check for many-to-many
                DataRow[] drOtherFK = dsFK.Tables[Name].Select(AccessSchemaVariable.PK_TABLE_NAME + "=" + q(tableName));
                for (int i = 0; i < drOtherFK.Length; i++)
                {
                    string thisTablePkColumnName = drOtherFK[i][AccessSchemaVariable.PK_COLUMN_NAME].ToString();
                    string fkColumnName = drOtherFK[i][AccessSchemaVariable.FK_COLUMN_NAME].ToString();
                    string fkTableName = drOtherFK[i][AccessSchemaVariable.FK_TABLE_NAME].ToString();
                    TableSchema.PrimaryKeyTable pkTable = new TableSchema.PrimaryKeyTable(this);
                    pkTable.ColumnName = fkColumnName;
                    pkTable.TableName = fkTableName;
                    SetExtendedTableProperties(pkTable);
                    tbl.PrimaryKeyTables.Add(pkTable);

                    // Many-to-many : we are looking for a foreign table using this table as a PK
                    // and with a key covering multiple FK columns.  We want to store the other
                    // FK tables and columns for that table.
                    // We can't extract this info using SQL in Access, so work it out from the existing FK/PK data

                    // check the FK is part of the foreign table's PK
                    DataRow[] drChkPK = dsPK.Tables[Name].Select(
                        AccessSchemaVariable.TABLE_NAME + "=" + q(fkTableName)
                        + " AND " + AccessSchemaVariable.COLUMN_NAME + "=" + q(fkColumnName));
                    if (drChkPK.Length == 1)
                    {
                        //  now check the other members of the foreign table's PK
                        DataRow[] drChkPK1 = dsPK.Tables[Name].Select(
                            AccessSchemaVariable.TABLE_NAME + "=" + q(fkTableName)
                            + " AND " + AccessSchemaVariable.COLUMN_NAME + "<>" + q(fkColumnName));
                        for (int j = 0; j < drChkPK1.Length; j++)
                        {
                            // If this FK table has other PK columns, interrogate them
                            string fkTableOtherKeyColName = drChkPK1[j][AccessSchemaVariable.COLUMN_NAME].ToString();

                            DataRow[] drMap = dsFK.Tables[Name].Select(
                                AccessSchemaVariable.FK_TABLE_NAME + "=" + q(fkTableName)
                                + " AND " + AccessSchemaVariable.FK_COLUMN_NAME + "=" + q(fkTableOtherKeyColName));
                            // (should be zero or one records depending if the column is actually a FK)
                            for (int k = 0; k < drMap.Length; k++)
                            {
                                TableSchema.ManyToManyRelationship m = new TableSchema.ManyToManyRelationship(fkTableName, tbl.Provider);
                                m.ForeignTableName = drMap[k][AccessSchemaVariable.PK_TABLE_NAME].ToString();
                                m.ForeignPrimaryKey = drMap[k][AccessSchemaVariable.PK_COLUMN_NAME].ToString();
                                m.MapTableLocalTableKeyColumn = fkColumnName;
                                m.MapTableForeignTableKeyColumn = fkTableOtherKeyColName;
                                tbl.ManyToManys.Add(m);
                            }
                        }
                    }
                }

                tbl.Columns = columns;
                return tbl;

            } // using
        } // GetTableSchema

        /// <summary>
        /// Gets the SP params.
        /// </summary>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        public override IDataReader GetSPParams(string spName)
        {
            // STORED PROC PARAMS
            // Cannot access this thru OleDB so use DAO to manufacture an equivalent datatable
            // (keeps DataTable caching method consistent throughout provider)
            if (dsSPParams.Tables[Name] == null) {
                lock (_lockSPs) {
                    if (dsSPParams.Tables[Name] == null) {
                        using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
                        {
                            OleDbConnection autoOleDbConn = automaticConnectionScope.Connection as OleDbConnection;

                            // Chuck a tantie if our connection is not of the OleDb variety
                            if (autoOleDbConn == null) {
                                throw new Exception("AccessDataProvider uses only an OleDb Connection");
                            }

                            // Use DAO to extract parameters 
                            DAO.DBEngineClass dbc = new DAO.DBEngineClass();
                            DAO.Database db = dbc.OpenDatabase(
                                GetAccessDBNameAndPathFromConnectionString(autoOleDbConn.ConnectionString)
                                , null, false, "");

                            DataTable dt = new DataTable(Name);
                            dt.Columns.Add(new DataColumn(AccessSchemaVariable.SP_SCHEMA, typeof(string)));
                            dt.Columns.Add(new DataColumn(AccessSchemaVariable.SP_NAME, typeof(string)));
                            dt.Columns.Add(new DataColumn(AccessSchemaVariable.SP_PARAM_ORDINALPOS, typeof(Int32)));
                            dt.Columns.Add(new DataColumn(AccessSchemaVariable.SP_PARAM_MODE, typeof(string)));
                            dt.Columns.Add(new DataColumn(AccessSchemaVariable.SP_PARAM_NAME, typeof(string)));
                            dt.Columns.Add(new DataColumn(AccessSchemaVariable.SP_PARAM_DBDATATYPE, typeof(Int32)));
                            dt.Columns.Add(new DataColumn(AccessSchemaVariable.SP_PARAM_DATALENGTH, typeof(Int32)));

                            // Use Access DAO to fill in the missing schema info
                            // Store it here to preserve caching of the schema dataset
                            foreach (DAO.QueryDef qd in db.QueryDefs)
                            {
                                if (qd.Name.StartsWith("~")) continue;
                                if (qd.Parameters.Count == 0) continue;

                                try
                                {
                                    for (int i = 0; i <= qd.Parameters.Count; i++)
                                    {
                                        DAO.Parameter DAOparam = qd.Parameters[i];

                                        DataRow newrow = dt.NewRow();
                                        newrow[AccessSchemaVariable.SP_SCHEMA] = "";
                                        newrow[AccessSchemaVariable.SP_NAME] = qd.Name;
                                        newrow[AccessSchemaVariable.SP_PARAM_ORDINALPOS] = i;
                                        newrow[AccessSchemaVariable.SP_PARAM_MODE] = "IN";
                                        newrow[AccessSchemaVariable.SP_PARAM_NAME] = DAOparam.Name;
                                        newrow[AccessSchemaVariable.SP_PARAM_DBDATATYPE] =
                                            (Int32)GetDbTypeFromDAOType((DaoDataTypeEnum) DAOparam.Type);
                                        newrow[AccessSchemaVariable.SP_PARAM_DATALENGTH] = DBNull.Value;
                                        dt.Rows.Add(newrow);
                                    }
                                }
                                catch (Exception ex) { };
                            }
                            dt.AcceptChanges();
                            dsSPParams.Tables.Add(dt);

                            db.Close();
                        }
                    }
                }
            }

            DataView dv = new DataView(dsSPParams.Tables[Name]);
            dv.RowFilter = AccessSchemaVariable.SP_NAME + "=" + q(spName);
            dv.Sort = AccessSchemaVariable.SP_PARAM_ORDINALPOS;
            DataTable dtNew = dv.ToTable();
            return dtNew.CreateDataReader();
        }

        /// <summary>
        /// Gets the SP list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetSPList()
        {
            if (! arSP.Contains(Name)) {
                lock (_lockSPs)
                {
                    using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
                    {
                        OleDbConnection autoOleDbConn = automaticConnectionScope.Connection as OleDbConnection;

                        if (! arSP.Contains(Name)) {
                            DataTable dt = autoOleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Procedures, null);
                            
                            StringBuilder sList = new StringBuilder();
                            foreach (DataRow dr in dt.Rows) {
                                String procName = dr[AccessSchemaVariable.PROCEDURE_NAME].ToString();
                                if (! procName.StartsWith("~")) {
                                    sList.Append(procName);
                                    sList.Append("|");
                                    //DataTableReader tempDr = GetSPParams(procName);
                                    //if (tempDr.HasRows && !tempDr[AccessSchemaVariable.PROCEDURE_DEF].ToString().TrimStart().StartsWith("PARAMETERS"))
                                    if (GetSPParams(procName) != null && !dr[AccessSchemaVariable.PROCEDURE_DEF].ToString().TrimStart().StartsWith("PARAMETERS"))
                                    {
                                        // warn if the SP has parameters but no PARAMETERS declaration
                                        Utility.WriteTrace("Procedure " + procName + " has parameters but does not start with a PARAMETERS declaration");
                                    }
                                }
                            }
                            string strList = sList.ToString();
                            string[] templist = strList.Split(new char[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

                            arSP.Add (Name, templist);
                        }
                    }
                }
            }
            return (string[]) arSP[Name];
        }

        private void CacheViewNameList()
        {
            if (!arViews.Contains(Name))
            {
                lock (_lockViews)
                {
                    using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
                    {
                        OleDbConnection autoOleDbConn = automaticConnectionScope.Connection as OleDbConnection;

                        if (!arViews.Contains(Name))
                        {
                            DataTable dt = autoOleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Views, null);

                            StringBuilder sList = new StringBuilder();
                            foreach (DataRow dr in dt.Rows)
                            {
                                sList.Append(dr[AccessSchemaVariable.TABLE_NAME].ToString());
                                sList.Append("|");
                            }
                            string strList = sList.ToString();
                            string[] templist = strList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            Array.Sort(templist);
                            arViews.Add(Name, templist);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the view name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetViewNameList()
        {
            CacheViewNameList();

            if (CurrentConnectionStringIsDefault) ViewNames = (string[])arViews[Name];
            return (string[])arViews[Name];
        }

        private void CacheTableNameList()
        {
            //'Need this fresh - RC'  -- What does this mean ? -- Ben Mc

            if (!arTables.Contains(Name))
            {
                lock (_lockTables)
                {
                    using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
                    {
                        OleDbConnection autoOleDbConn = automaticConnectionScope.Connection as OleDbConnection;

                        if (!arTables.Contains(Name))
                        {
                            DataTable dt = autoOleDbConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                            StringBuilder sList = new StringBuilder();
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr[AccessSchemaVariable.TABLE_TYPE].ToString() == "TABLE")
                                {
                                    sList.Append(dr[AccessSchemaVariable.TABLE_NAME].ToString());
                                    sList.Append("|");
                                }
                            }
                            string strList = sList.ToString();
                            string[] templist = strList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                            Array.Sort(templist);
                            arTables.Add(Name, templist);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the table name list.
        /// </summary>
        /// <returns></returns>
        public override string[] GetTableNameList()
        {
            CacheTableNameList();

            if (CurrentConnectionStringIsDefault) TableNames = (string[]) arTables[Name];
            return (string[]) arTables[Name];
        }

        /// <summary>
        /// Gets the foreign key table and column names for the given primary key table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override ArrayList GetPrimaryKeyTableNames(string tableName)
        {
            CachePkFkData();

            DataRow[] drFK = dsFK.Tables[Name].Select(AccessSchemaVariable.PK_TABLE_NAME + "=" + q(tableName));
            ArrayList list = new ArrayList();
            foreach (DataRow dr in drFK) {
                list.Add(new string[] { dr[AccessSchemaVariable.FK_TABLE_NAME].ToString(), dr[AccessSchemaVariable.FK_COLUMN_NAME].ToString() });
            }
            return list;
        }

        /// <summary>
        /// Gets the foreign key table names for the given primary key table, and loads the schemas.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override TableSchema.Table[] GetPrimaryKeyTables(string tableName)
        {
            CachePkFkData();

            DataRow[] drFK = dsFK.Tables[Name].Select(AccessSchemaVariable.PK_TABLE_NAME + "=" + q(tableName));
            ArrayList list = new ArrayList();
            foreach (DataRow dr in drFK) {
                list.Add(dr[AccessSchemaVariable.FK_TABLE_NAME].ToString());
            }
            if (list.Count > 0) {
                TableSchema.Table[] tables = new TableSchema.Table[list.Count];
                for (int i = 0; i < list.Count; i++)
                    tables[i] = DataService.GetSchema((string)list[i], Name, TableType.Table);
                return tables;
            }
            return null;
        }

        /// <summary>
        /// Get PK table given FK table and column
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName, string fkTableName)
        {
            CachePkFkData();

            DataRow[] drFK = dsFK.Tables[Name].Select(
                AccessSchemaVariable.FK_TABLE_NAME + "=" + q(fkTableName)
                + " AND " + AccessSchemaVariable.FK_COLUMN_NAME + "=" + q(fkColumnName));
            if (drFK.Length != 1) return null;
            return drFK[0][AccessSchemaVariable.PK_TABLE_NAME].ToString();
        }

        /// <summary>
        /// // Get PK table(s) given PK Column (PK can be partial for referencing tables with the column as part of the PK)
        /// </summary>
        /// <param name="fkColumnName">Name of the fk column.</param>
        /// <returns></returns>
        public override string GetForeignKeyTableName(string fkColumnName)
        {
        // WTF ???
        // This should happily return multiple rows where the PK column is used as 
        // part of the multiple-column PK of a foreign table.
        // Assume we are looking at single PK's only ???

            CachePkFkData();

            DataRow[] drPK = dsPK.Tables[Name].Select(
                AccessSchemaVariable.PK_COLUMN_NAME + "=" + q(fkColumnName)
                + " AND " + AccessSchemaVariable.PK_TYPE + "='SV'");
            if (drPK.Length != 1) return null;
            return drPK[0][AccessSchemaVariable.PK_TABLE_NAME].ToString();
        }
        
        /// <summary>
        /// Get PK table(s), FK column(s) given FK table
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public override string[] GetForeignKeyTables(string tableName)
        {
            CachePkFkData();

            // Get PK table(s), FK column(s) given FK table
            DataRow[] drFK = dsFK.Tables[Name].Select(AccessSchemaVariable.FK_TABLE_NAME + "=" + q(tableName));
            StringBuilder sList = new StringBuilder();
            foreach (DataRow dr in drFK)
            {
                sList.Append(dr[AccessSchemaVariable.PK_TABLE_NAME].ToString());
                sList.Append("|");
            }
            string strList = sList.ToString();
            string[] templist = strList.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
            return templist;
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="qry">The qry.</param>
        /// <returns></returns>
        public override IDbCommand GetCommand(QueryCommand qry)
        {
            OleDbCommand cmd = new OleDbCommand(qry.CommandSql);
            AddParams(cmd, qry);
            return cmd;
        }

        /// <summary>
        /// Executes a transaction using the passed-commands
        /// </summary>
        /// <param name="commands"></param>
        public override void ExecuteTransaction(QueryCommandCollection commands)
        {
            //make sure we have at least one
            if(commands.Count > 0)
            {
                OleDbCommand cmd = null;

                //a using statement will make sure we close off the connection

                using(AutomaticConnectionScope conn = new AutomaticConnectionScope(this))
                {

                    //open up the connection and start the transaction
                    if(conn.Connection.State == ConnectionState.Closed)
                        conn.Connection.Open();

                    OleDbTransaction trans = (OleDbTransaction)conn.Connection.BeginTransaction();

                    foreach(QueryCommand qry in commands)
                    {
						bool selectIdentity = false;
						string commandSql = GetSqlBeforeIdentityFetchSql(qry.CommandSql, ref selectIdentity);

						cmd = new OleDbCommand(commandSql, (OleDbConnection)conn.Connection, trans);
                        cmd.CommandType = qry.CommandType;

                        AddParams(cmd, qry);

                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch(OleDbException x)
                        {
                            //if there's an error, roll everything back
                            trans.Rollback();

                            //clean up
                            conn.Connection.Close();
                            cmd.Dispose();

                            //throw the error retaining the stack.
                            throw new Exception(x.Message);
                        }
                    }
                    //if we get to this point, we're good to go
                    trans.Commit();

                    //close off the connection
                    conn.Connection.Close();
                    if(cmd != null)
                    {
                        cmd.Dispose();
                    }
                }
            }
            else
            {
                throw new Exception("No commands present");
            }
        }

        /// <summary>
        /// Gets the table name given the primary key column.
        /// </summary>
        /// <param name="pkName">Name of the pk.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public override string GetTableNameByPrimaryKey(string pkName, string providerName)
        {
            CachePkFkData();

            DataRow[] drPK = dsPK.Tables[Name].Select(
                AccessSchemaVariable.PK_COLUMN_NAME + "=" + q(pkName)
                + " AND " + AccessSchemaVariable.PK_TYPE + "='SV'");
            if (drPK.Length != 1) return null;
            return drPK[0][AccessSchemaVariable.PK_TABLE_NAME].ToString();
        }

        /// <summary>
        /// Gets the database version.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        protected override string GetDatabaseVersion(string providerName)
        {
            using (AutomaticConnectionScope automaticConnectionScope = new AutomaticConnectionScope(this))
            {
                DataTable dt = automaticConnectionScope.Connection.GetSchema("DataSourceInformation");
                string version = dt.Rows[0]["DataSourceProductName"] + " " + dt.Rows[0]["DataSourceProductVersion"];
                return version;
            }
        }
        
        #region SQL Builders

        //this is only used with the SQL constructors below
        //it's not used in the command builders above, which need to set the parameters
        //right at the time of the command build


        // Paging Template (sample page=3, pagelen=10) :
        //
        //  SELECT TOP 10 *
        //  FROM (
        //      SELECT TOP 30 *
        //      FROM (
        //          SELECT CategoryID, CategoryName, ProductName, ProductSales
        //          FROM [Sales by Category]
        //          ORDER BY CategoryName ASC, ProductName ASC
        //      ) AS t0
        //      ORDER BY CategoryName ASC, ProductName ASC
        //  ) AS t1
        //  ORDER BY CategoryName DESC , ProductName DESC;

        private const string PAGING_SQL_FIRST_PAGE =
            @"					
        SELECT TOP {0} *
        FROM ({1}) as t0
        ";

        private const string PAGING_SQL_OTHER_PAGES =
            @"
        SELECT *
        FROM (
            SELECT TOP {0} *
            FROM (
                SELECT TOP {1} *
                FROM ({4}) as t0
                {2}
                ) as t1
            {3}
            ) as t2
        {2}";

        //-----------------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Creates a SELECT statement based on the Query object settings
        /// </summary>
        /// <returns></returns>
        public override string GetSelectSql(Query qry)
        {
            if(qry.columns == null || qry.columns.Count == 0)
            {
                TableSchema.Table table = qry.Schema;
                string distinct = qry.IsDistinct ? SqlFragment.DISTINCT : String.Empty;

                //different rules for how to do TOP
                string select = SqlFragment.SELECT + distinct + SqlFragment.TOP + qry.Top + " ";

                StringBuilder order = new StringBuilder();
                StringBuilder reverseOrder = new StringBuilder();
                StringBuilder tempQuery = new StringBuilder();
                StringBuilder query = new StringBuilder();
                string columns;

                //append on the selectList, which is a property that can be set
                //and is "*" by default

                if(qry.SelectList != null && qry.SelectList.Trim().Length >= 2) {
                    columns = qry.SelectList;
                }
                else  {
                    columns = GetQualifiedSelect(table);
                }

                //Added by Rob to account for FK lookups
                //TODO: Remove this hack and rebuild this query tool!
                //:):):)
                if (qry.AliasForeignKeys)
                {
                    List<TableSchema.TableColumn> AddedColumns = new List<TableSchema.TableColumn>();
                    // don't add table owner for JET SQL
                    string fullQFormat = "[{0}].[{1}]";
                    foreach (TableSchema.TableColumn col in qry.Schema.Columns)
                    {
                        if (col.IsForeignKey)
                        {
                            TableSchema.Table fkTable = DataService.GetForeignKeyTable(col, col.Table);

                            //replace the ID in the columns list with a nested SELECT
                            string fkAlias = "(SELECT [" + fkTable.Descriptor.ColumnName + "] FROM [" + col.ForeignKeyTableName + "] WHERE [" + col.ForeignKeyTableName +
                                "].[" + fkTable.PrimaryKey.ColumnName + "]=[" + col.Table.Name + "].[" + col.ColumnName + "]) as " + col.ColumnName;

                            string lookFor = string.Format(fullQFormat, col.Table.Name, col.ColumnName);

                            columns = columns.Replace(lookFor, fkAlias);

                            //add columns to the core schema to account for this
                            //AddedColumns.Add(fkTable.Columns[1]);
                        }
                    }

                }
                
                string where = BuildWhere(qry);

                //Finally, do the orderby
                if(qry.OrderByCollection.Count > 0)
                {
                    for(int j = 0; j < qry.OrderByCollection.Count; j++)
                    {
                        string orderString = qry.OrderByCollection[j].OrderString;
                        string reverseOrderString = qry.OrderByCollection[j].OrderStringReversed;
                        if(!String.IsNullOrEmpty(orderString))  {
                            order.Append(orderString);
                            reverseOrder.Append(reverseOrderString);
                            if(j + 1 != qry.OrderByCollection.Count)  {
                                order.Append(", ");
                                reverseOrder.Append(", ");
                            }
                        }
                    }
                }
                else
                {
                    if(table.PrimaryKey != null) {
                        order.Append(OrderBy.Asc(table.PrimaryKey.ColumnName).OrderString);
                        reverseOrder.Append(OrderBy.Asc(table.PrimaryKey.ColumnName).OrderStringReversed);
                    }
                }
				if (order.Length > 0) {
					order.Insert(0, SqlFragment.ORDER_BY);
					reverseOrder.Insert(0, SqlFragment.ORDER_BY);
				}

                tempQuery.Append(select);
                tempQuery.Append(columns);
                tempQuery.Append(SqlFragment.FROM);
                tempQuery.Append(qry.Provider.QualifyTableName(table.SchemaName, table.Name));
                tempQuery.Append(where);
				tempQuery.Append(order); 

                if (qry.PageIndex < 0) {
                    query.Append(tempQuery);
                    query.Append(";");
                } 
                else {
                    // Paging is 1-based (NOT 0-based)
                    if (qry.PageIndex == 1)
                    {
                        query.Append(string.Format(
                            PAGING_SQL_FIRST_PAGE,
                            qry.PageSize,
                            tempQuery));
                        query.Append(";");
                    }
                    else
                    {
                        query.Append(string.Format(
                            PAGING_SQL_OTHER_PAGES,
                            qry.PageSize,
                            qry.PageSize * (qry.PageIndex),
                            order,
                            reverseOrder,
                            tempQuery));
                        query.Append(";");
                    }
                }

                return query.ToString();
            }
            else
            // No query column, generate from schema automatically
            {
                StringBuilder strJoin = new StringBuilder();
                StringBuilder strSelect = new StringBuilder(SqlFragment.SELECT);
                string strFrom = SqlFragment.FROM + qry.Provider.QualifyTableName(qry.Schema.SchemaName, qry.Schema.TableName);
                List<TableSchema.Table> uniqueTables = new List<TableSchema.Table>();
                foreach (TableSchema.TableColumn col in qry.columns)
                {
                    if (!uniqueTables.Contains(col.Table))
                        uniqueTables.Add(col.Table);
                }

                StringBuilder sourceDef = new StringBuilder(SqlFragment.FROM);
                for (int i = 0; i < uniqueTables.Count; i++)
                {
                    sourceDef.Append(qry.Provider.QualifyTableName(uniqueTables[i].SchemaName, uniqueTables[i].TableName));
                    sourceDef.Append(" j");
                    sourceDef.Append(uniqueTables[i].ClassName);

                    if (i + 1 < uniqueTables.Count)
                        sourceDef.Append(", ");
                }
                sourceDef.AppendLine();

                for (int i = 0; i < qry.columns.Count; i++)
                {
                    string joinType = SqlFragment.INNER_JOIN;
                    StringBuilder col = new StringBuilder();
                    TableSchema.TableColumn tblCol = qry.columns[i];

                    if (qry.columns[i].IsNullable)
                        joinType = SqlFragment.LEFT_JOIN;
                    if (qry.columns[i].Table == qry.Schema)
                        col.Append(qry.columns[i].ColumnName);
                    else
                    {
                        foreach (TableSchema.TableColumn colPrimaryTable in qry.Schema.Columns)
                        {
                            if (colPrimaryTable.IsForeignKey && !String.IsNullOrEmpty(colPrimaryTable.ForeignKeyTableName) &&
                               colPrimaryTable.ForeignKeyTableName == tblCol.Table.Name)
                            {
                                string strJoinPrefix = SqlFragment.JOIN_PREFIX + i;
                                //TableSchema.Table fkTable = DataService.GetForeignKeyTable(table.Columns[i], table);
                                TableSchema.Table fkTable = tblCol.Table;
                                string dataCol = tblCol.ColumnName;
                                string selectCol = qry.Schema.Provider.QualifyColumnName("", strJoinPrefix, dataCol);
                                col = new StringBuilder(selectCol);
                                strJoin.Append(joinType);
                                strJoin.Append(qry.Schema.Provider.FormatIdentifier(fkTable.Name));
                                strJoin.Append(SqlFragment.SPACE);
                                strJoin.Append(strJoinPrefix);
                                strJoin.Append(SqlFragment.ON);
                                string columnReference = qry.Schema.Provider.QualifyColumnName(qry.Schema.SchemaName, qry.Schema.Name, colPrimaryTable.ColumnName);
                                strJoin.Append(columnReference);
                                strJoin.Append(SqlFragment.EQUAL_TO);
                                string joinReference = qry.Schema.Provider.QualifyColumnName("", strJoinPrefix, fkTable.PrimaryKey.ColumnName);
                                strJoin.Append(joinReference);
                                if (qry.OrderByCollection.Count > 0)
                                {
                                    foreach (OrderBy ob in qry.OrderByCollection)
                                        ob.OrderString = ob.OrderString.Replace(columnReference, selectCol);
                                }
                                break;
                            }
                        }
                    }

                    if (i + 1 != qry.columns.Count)
                        col.Append(", ");

                    strSelect.Append(col);
                }

                StringBuilder strSQL = new StringBuilder();
                strSQL.Append(strSelect);
                strSQL.Append(strFrom);
                strSQL.Append(strJoin);

                if (qry.wheres.Count > 0)
                {
                    string strWhere = BuildWhere(qry);
                    strSQL.Append(strWhere);
                }

                if (qry.OrderByCollection.Count > 0)
                {
                    for (int j = 0; j < qry.OrderByCollection.Count; j++)
                    {
                        string orderString = qry.OrderByCollection[j].OrderString;
                        if (!String.IsNullOrEmpty(orderString))
                        {
                            strSQL.Append(orderString);
                            if (j + 1 != qry.OrderByCollection.Count)
                                strSQL.Append(", ");
                        }
                    }
                }
                return strSQL.ToString();
            }
        }

        /// <summary>
        /// Returns a qualified list of columns ([Table].[Column])
        /// </summary>
        /// <returns></returns>
        protected static string GetQualifiedSelect(TableSchema.Table table)
        {
            StringBuilder sb = new StringBuilder();
            foreach(TableSchema.TableColumn tc in table.Columns)
                sb.AppendFormat(", [{0}].[{1}]", table.Name, tc.ColumnName);

            string result = sb.ToString();
            if(result.Length > 1)
                result = sb.ToString().Substring(1);

            return result;
        }

        //-----------------------------------------------------------------------------------------------------------------------//
        /// <summary>
        /// Loops the TableColums[] array for the object, creating a SQL string
        /// for use as an INSERT statement
        /// </summary>
        /// <returns></returns>
        public override string GetInsertSql(Query qry)
        {
            TableSchema.Table table = qry.Schema;

            //split the TablNames and loop out the SQL
            string insertSQL = SqlFragment.INSERT_INTO + qry.Provider.QualifyTableName(table.SchemaName, table.Name);
            //string client = DataService.GetClientType();

            string cols = String.Empty;
            string pars = String.Empty;

			//returns Guid from MSSQL2005 only!
            bool primaryKeyIsGuid = false;
            bool primaryKeyisIdentity = false;
            string primaryKeyName = "";

            bool isFirstColumn = true;
            //if table columns are null toss an exception
            foreach(TableSchema.TableColumn col in table.Columns)
            {
                string colName = col.ColumnName;
                if(!(col.DataType == DbType.Guid && col.DefaultSetting != null && col.DefaultSetting == AccessSchemaVariable.DEFAULT))
                {
                    if(!col.AutoIncrement  && !col.IsReadOnly)
                    {
                        if(!isFirstColumn)
                        {
                            cols += ",";
                            pars += ",";
                        }

                        isFirstColumn = false;

                        cols += FormatIdentifier(colName);
                        pars += FormatParameterNameForSQL(colName);
                    }
                    if (col.IsPrimaryKey)
                    {
                        primaryKeyName = col.ColumnName;
                        if (col.DataType == DbType.Guid) primaryKeyIsGuid = true;
                        if (col.AutoIncrement ) primaryKeyisIdentity = true;
                    }
                }
            }

            insertSQL += "(" + cols + ") ";

            // Add "SELECT @@IDENTITY;" to statement. (Access can't process multiple 
            // commands but we'll tweak the ExecuteScalar method when this test is detected.

			//Non Guid's
			insertSQL = String.Concat(insertSQL, "VALUES(", pars, ");");
			insertSQL = String.Concat(insertSQL, AccessSql.GET_INT_IDENTITY);

			return insertSQL;
        }

        // for access, do not add anything to the end of the update string
        protected override string AdjustUpdateSql(Query qry, TableSchema.Table table, string updateSql)        {
            return updateSql;
        }

        #endregion

		public override ISqlGenerator GetSqlGenerator(SqlQuery sqlQuery) {
			return new MSJetGenerator(sqlQuery);
		}

        #region SQL Scripters

        /// <summary>
        /// Scripts the data.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns></returns>
        public override string ScriptData(string tableName, string providerName)
        {
            StringBuilder result = new StringBuilder();
            if (CodeService.ShouldGenerate(tableName, providerName))
            {
                StringBuilder fieldList = new StringBuilder();
                StringBuilder insertStatement = new StringBuilder();
                StringBuilder statements = new StringBuilder();

                // Cannot disable and enable constraints in SQL.  We will need to do this in Access DAO
                // at the time the data INSERTs are run.

                insertStatement.Append("INSERT INTO [" + tableName + "] ");

                //pull the schema for this table
                TableSchema.Table table = Query.BuildTableSchema(tableName, providerName);

                //build the insert list.
                string lastColumnName = table.Columns[table.Columns.Count - 1].ColumnName;
                foreach (TableSchema.TableColumn col in table.Columns)
                {
                    fieldList.Append("[");
                    fieldList.Append(col.ColumnName);
                    fieldList.Append("]");

                    if (!Utility.IsMatch(col.ColumnName, lastColumnName))
                        fieldList.Append(", ");
                }

                //complete the insert statement
                insertStatement.Append("(");
                insertStatement.Append(fieldList);
                insertStatement.AppendLine(")");

                //get the table data
                IDataReader rdr = new Query(table).ExecuteReader();
                //bool isNumeric = false;
                //TableSchema.TableColumn thisColumn=null;

                while (rdr.Read())
                {
                    StringBuilder thisStatement = new StringBuilder();
                    thisStatement.Append(insertStatement);
                    thisStatement.Append("VALUES(");
                    //loop the schema and pull out the values from the reader
                    foreach (TableSchema.TableColumn col in table.Columns)
                    {
                        if (!col.IsReadOnly)
                        {
                            object oData = rdr[col.ColumnName];
                            if (oData != null && oData != DBNull.Value)
                            {
                                if (col.DataType == DbType.Boolean)
                                {
                                    bool bData = Convert.ToBoolean(oData);
                                    thisStatement.Append(bData ? "1" : " 0");
                                }
                                else if (col.DataType == DbType.Byte)
                                    thisStatement.Append(oData);
                                else if (col.DataType == DbType.Binary)
                                {
                                    thisStatement.Append("0x");
                                    thisStatement.Append(Utility.ByteArrayToString((Byte[])oData).ToUpper());
                                }
                                else if (col.IsNumeric)
                                    thisStatement.Append(oData);
                                else if (col.IsDateTime)
                                {
                                    DateTime dt = DateTime.Parse(oData.ToString());
                                    thisStatement.Append("#");
                                    thisStatement.Append(dt.ToString("yyyy-MMM-dd HH:mm:ss"));
                                    thisStatement.Append("#");
                                }
                                else
                                {
                                    thisStatement.Append("'");
                                    thisStatement.Append(oData.ToString().Replace("'", "''"));
                                    thisStatement.Append("'");
                                }
                            }
                            else
                                thisStatement.Append("NULL");

                            if (!Utility.IsMatch(col.ColumnName, lastColumnName))
                                thisStatement.Append(", ");
                        }
                    }

                    //add in a closing paren
                    thisStatement.AppendLine(")");
                    statements.Append(thisStatement);
                }
                rdr.Close();

                // IDENTITY INSERT not necessary for Access (worryingly, we can always insert identities in Access)
                result.Append(statements);
            }
            return result.ToString();
        }

        #endregion

        //-----------------------------------------------------------------------------------------------------------------------//
        public override DbCommand GetDbCommand(QueryCommand qry)
        {
            AutomaticConnectionScope conn = new AutomaticConnectionScope(this);
            DbCommand cmd = conn.Connection.CreateCommand();
            cmd.CommandText = qry.CommandSql;
            cmd.CommandType = qry.CommandType;

            foreach(QueryParameter par in qry.Parameters)
            {
                cmd.Parameters.Add(par);
            }

            return cmd;
        }


        #region Migration Support

        // Generate these scripts anyway, even though Access can't utilise them
        // (for planned future Access 'ScriptReader' software - Ben Mc)

        private const string ADD_COLUMN = @"ALTER TABLE {0} ADD  {1}  {2}";
        private const string ALTER_COLUMN = @"ALTER TABLE {0} ALTER COLUMN {1} {2}";
        private const string CREATE_TABLE = "CREATE TABLE {0} ({1} \r\n)";
        private const string DROP_COLUMN = @"ALTER TABLE {0} DROP COLUMN {1} ";
        private const string DROP_TABLE = @"DROP TABLE {0}";


        /// <summary>
        /// Sets the column attributes.
        /// </summary>
        /// <param name="col">The col.</param>
        /// <returns></returns>
        private static string SetColumnAttributes(TableSchema.TableColumn col)
        {
            StringBuilder sb = new StringBuilder();
            //if (col.DataType == DbType.String && col.MaxLength > 8000)

            if (col.IsPrimaryKey && col.AutoIncrement)
                sb.Append(" COUNTER(1,1) ");
            else
                sb.Append(" " + GetDDLTypeNameFromDAOType(GetDAOTypeFromDbType(col.DataType)));

            if (col.IsPrimaryKey)
            {
                sb.Append(" NOT NULL PRIMARY KEY ");
            }
            else
            {
                if (col.MaxLength > 0 && col.MaxLength < 8000)
                    sb.Append("(" + col.MaxLength + ")");

                if (!col.IsNullable)
                    sb.Append(" NOT NULL ");
                else
                    sb.Append(" NULL ");

                if (!String.IsNullOrEmpty(col.DefaultSetting))
                    sb.Append(" CONSTRAINT DF_" + col.Table.Name + "_" + col.ColumnName + " DEFAULT (" + col.DefaultSetting + ")");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Adds the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="column">The column.</param>
        public void AddColumn(string tableName, TableSchema.TableColumn column)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(ADD_COLUMN, tableName, column.ColumnName, SetColumnAttributes(column));
            SetColumnAttributes(column);
            QueryCommand cmd = new QueryCommand(sql.ToString(), Name);
            DataService.ExecuteQuery(cmd);
        }

        /// <summary>
        /// Alters the column.
        /// </summary>
        /// <param name="column">The column.</param>
        public void AlterColumn(TableSchema.TableColumn column)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(ALTER_COLUMN, column.Table.Name, column.ColumnName, SetColumnAttributes(column));
            SetColumnAttributes(column);
            QueryCommand cmd = new QueryCommand(sql.ToString(), Name);
            DataService.ExecuteQuery(cmd);
        }

        /// <summary>
        /// Removes the column.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columnName">Name of the column.</param>
        public void RemoveColumn(string tableName, string columnName)
        {
            StringBuilder sql = new StringBuilder();
            //pull the schema
            TableSchema.Table tbl = DataService.GetSchema(tableName, Name);
            TableSchema.TableColumn c = tbl.GetColumn(columnName);

            //check to see if there are any constraints
            QueryCommand cmd;
            if (!string.IsNullOrEmpty(c.DefaultSetting))
            {
                string dropSql = "ALTER TABLE " + tableName + " DROP CONSTRAINT DF_" + c.Table.Name + "_" + c.ColumnName;
                cmd = new QueryCommand(dropSql, Name);
                DataService.ExecuteQuery(cmd);

                //drop FK constraints ...

                //drop CHECK constraints ...
            }
            sql.AppendFormat(DROP_COLUMN, tableName, columnName);
            cmd = new QueryCommand(sql.ToString(), Name);
            DataService.ExecuteQuery(cmd);
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="columns">The columns.</param>
        public void CreateTable(string tableName, TableSchema.TableColumnCollection columns)
        {
            StringBuilder createSql = new StringBuilder();

            foreach (TableSchema.TableColumn col in columns)
            {
                string sql = string.Format("\r\n  {0} {1},", col.ColumnName, SetColumnAttributes(col));
                createSql.Append(sql);
            }
            string columnSql = createSql.ToString();
            //scrape the comma
            columnSql = SubSonic.Sugar.Strings.Chop(columnSql, ",");
            string createCommand = string.Format(CREATE_TABLE, tableName, columnSql);

            QueryCommand cmd = new QueryCommand(createCommand, Name);
            DataService.ExecuteQuery(cmd);
        }

        /// <summary>
        /// Drops the table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        public void DropTable(string tableName)
        {
            string dropCommand = string.Format(DROP_TABLE, tableName);
            QueryCommand cmd = new QueryCommand(dropCommand, Name);
            DataService.ExecuteQuery(cmd);
        }

        #endregion

        //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // library functions I can't do without - Ben Mc
        //''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
         public string q(object i) {
            // add single quotes to string with escaping (also convert null to empty string)
             if (i == null || i == System.DBNull.Value) {
                return @"''";
            }
            else if (((((string) i).IndexOf(@"'") + 1) == 0)) {
                return @"'" + i + @"'";
            }
            else {
                return @"'" + i.ToString().Replace (@"'", @"''") + @"'";
            }
        }

        public string dq(object i)
        {
            // add double quotes to string with escaping (also convert null to empty string)
            if (i == null || i == System.DBNull.Value)
            {
                return "\"\"";
            }
            else if (((((string)i).IndexOf("\"") + 1) == 0))
            {
                return "\"" + i + "\"";
            }
            else
            {
                return "\"" + i.ToString().Replace("\"", "\"\"") + "\"";
            }
        }
    }
}
