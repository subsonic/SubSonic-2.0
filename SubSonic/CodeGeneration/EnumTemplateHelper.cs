using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Data;

namespace SubSonic {
	public class EnumTemplateHelper {

		// ----------------------------------------------------------------------------------------------------
		// ENUM GENERATION CODE
		// ----------------------------------------------------------------------------------------------------
		const string MultiMarker = "MULTI=";

		public static string GetEnumSql(string idCol, string descriptionCol, string keyCol, string tablename, string sqlWhereClause) {
			return "SELECT " + idCol + "," + descriptionCol + (keyCol == "" ? "" : "," + keyCol) + " FROM " + tablename + " " + sqlWhereClause;
		}

		public static string GetEnumHeaderFooter(string tblName, string EnumName, string IdCol, string DescriptionCol, 
		 string EnumRowScript, bool idColIsString, bool isVb) {
			if (isVb) {
				if (idColIsString) {
					return "\r\n\t\t' string enum derived from database rows: " + tblName
						+ "." + DescriptionCol + ", " + tblName + "." + IdCol
						+ "\r\n\t\tPublic Structure " + EnumName
						+ "\r\n" + EnumRowScript
						+ "\r\n\t\t\tPublic Property Value() As String"
						+ "\r\n\t\t\t\tGet"
						+ "\r\n\t\t\t\t\tReturn m_Value"
						+ "\r\n\t\t\t\tEnd Get"
						+ "\r\n\t\t\t\tSet"
						+ "\r\n\t\t\t\t\tm_Value = Value"
						+ "\r\n\t\t\t\tEnd Set"
						+ "\r\n\t\t\tPrivate m_Value As String"
						+ "\r\n\t\t\tPublic Overrides Function ToString() As String"
						+ "\r\n\t\t\t\tReturn Value"
						+ "\r\n\t\t\tEnd Function"
						+ "\r\n\t\tEnd Structure\r\n";
				} else {
					return "\r\n\t\t' enum derived from database rows: " + tblName
						+ "." + DescriptionCol + ", " + tblName + "." + IdCol
						+ "\r\n\t\tPublic Enum " + EnumName + " {"
						+ "\r\n" + EnumRowScript
						+ "\r\n\t\tEnd Enum\r\n";
				}
			} else {
				if (idColIsString) {
					return "\r\n\t\t// string enum derived from database rows: " + tblName
						+ "." + DescriptionCol + ", " + tblName + "." + IdCol
						+ "\r\n\t\tpublic struct " + EnumName + " {"
						+ "\r\n" + EnumRowScript
						+ "\r\n\t\t\tpublic string Value { get; set; }"
						+ "\r\n\t\t\tpublic override string ToString() { return Value; }"
						+ "\r\n\t\t}\r\n";
				}
				else {
					return "\r\n\t\t// enum derived from database rows: " + tblName
						+ "." + DescriptionCol + ", " + tblName + "." + IdCol
						+ "\r\n\t\tpublic enum " + EnumName + " {"
						+ "\r\n" + EnumRowScript
						+ "\r\n\t\t}\r\n";
				}
			}
		}

		public static string GetEnumName(string CleanName, bool idColIsString) {
			return CleanName + "Enum" + (idColIsString ? "Str" : "");
		}

		public static string CleanUp(string tableName) {
			string result = tableName;
			result = Regex.Replace(tableName, @"[^\w]+", "_").Trim();
			if (result == "") { result = "_"; }
			if (Char.IsDigit(result.ToCharArray(0, 1)[0])) { result = "_" + result; }
			return result;
		}

		public static string GetEnumScript(DataProvider provider, TableSchema.Table tbl, bool GenerateEnumDebugText, string[] EnumSettings,
		 string[] EnumSettingsExclude, ICodeLanguage language) {
			string rtn = "";
			bool IsVb = (language.Identifier=="VB.NET");
			string commentmarker = (IsVb ? "'" : "//");

			if (GenerateEnumDebugText) { rtn += "\r\n\r\n\t\t" + commentmarker + " tbl: " + tbl.Name; }

			for (int k = 0; k < EnumSettings.Length; k++) {
				string enumSetting = EnumSettings[k];
				string enumSettingExlude = "";
				if (EnumSettingsExclude.Length > k) { enumSettingExlude = EnumSettingsExclude[k]; }

				string[] settings = enumSetting.Split(new char[] { ':' });
				string regExFind = "";
				if (settings.Length > 0) { regExFind = settings[0].Trim(); }

				bool matched = regExFind.Length > 0
					&& Regex.IsMatch(tbl.Name, regExFind, RegexOptions.IgnoreCase);
				bool excluded = enumSettingExlude.Trim() != ""
					&& Regex.IsMatch(tbl.Name, enumSettingExlude.Trim(), RegexOptions.IgnoreCase);

				bool found = matched && !excluded;
				if (GenerateEnumDebugText) { rtn += "\r\n\t\t" + commentmarker + " " + k.ToString() + ": " + (matched ? "" : "not ") + "matched '" + regExFind + "'"; }
				if (GenerateEnumDebugText) { rtn += "\r\n\t\t" + commentmarker + " " + k.ToString() + ": " + (excluded ? "" : "not ") + "excluded '" + enumSettingExlude.Trim() + "'"; }

				if (found) {
					string SqlScript = "";
					//if (GenerateEnumDebugText) { SqlScript += "\r\n\t\t// tbl: " + tbl.Name + "\r\n\t\t// match: " + enumSetting; }

					// Get Enum Details
					string EnumName = "";
					string IdCol = "";
					string DescriptionCol = "";
					string multiKeyCol = "";
					string SqlWhereClause = "";
					bool idColIsString = false;
					bool isMulti = false;

					bool idColFound = false;
					bool descColFound = false;
					bool multiKeyColFound = false;

					if (settings.Length > 1) { EnumName = settings[1].Trim(); }
					if (EnumName.StartsWith(MultiMarker, StringComparison.InvariantCultureIgnoreCase)) {
						isMulti = true;
						multiKeyCol = EnumName.Substring(MultiMarker.Length);
					}
					if (settings.Length > 2) { IdCol = settings[2].Trim(); }
					if (settings.Length > 3) { DescriptionCol = settings[3].Trim(); }
					if (settings.Length > 4) { SqlWhereClause = settings[4].Trim(); }

					// check the cols do actually exist if specified
					foreach (var col in tbl.Columns) {
						if (IdCol == "" && col.IsPrimaryKey) { IdCol = col.ColumnName; }
						if (DescriptionCol == "" && !col.IsPrimaryKey && !col.IsForeignKey 
						&& col.DataType == System.Data.DbType.String) { DescriptionCol = col.ColumnName; }

						if (IdCol == col.ColumnName) { idColFound = true; idColIsString = (col.DataType == System.Data.DbType.String); }
						if (DescriptionCol == col.ColumnName) { descColFound = true; }

						if (isMulti && multiKeyCol == col.ColumnName) { multiKeyColFound = true; }
					}

					if (EnumName == "") { EnumName = GetEnumName(tbl.ClassName, idColIsString); }

					// generate the script or a warning message
					if (!idColFound || !descColFound || (isMulti && !multiKeyColFound)) {
						SqlScript += "\r\n\t\t" + commentmarker + " " + tbl.Name + ": enumSetting could not be matched to ID" + (isMulti ? ", Key " : "")
						+ " and Description columns. Setting=" + enumSetting + "\r\n";
					}
					else {
						//pull the tables in a reader
						int rowCount = 0;
						List<string> enumValList = new List<string>();
						string enumMemberScript = "";
						string lastKeyVal = "";
						string sql = GetEnumSql(IdCol, DescriptionCol, multiKeyCol, tbl.Name, SqlWhereClause);
						try {
							using (IDataReader rdr = provider.GetReader(new QueryCommand(sql))) {
								while (rdr.Read()) {
									string enumMemberName = CleanUp(rdr[DescriptionCol].ToString());
									if (enumValList.Contains(enumMemberName)) {
										int uniqueVal = 0;
										string tempMemberName = enumMemberName;
										while (enumValList.Contains(tempMemberName)) {
											tempMemberName = enumMemberName + (++uniqueVal).ToString();
										}
										enumMemberName = tempMemberName;
									}

									string enumMemberValue = rdr[IdCol].ToString();
									string enumKeyVal = (isMulti ? CleanUp(rdr[multiKeyCol].ToString()) : "");

									if (rowCount != 0 && lastKeyVal != enumKeyVal) {
										// we are doing a multi read, use the key val to generate an enum for each block of key values
										string tempEnumName = GetEnumName(lastKeyVal, idColIsString);
										SqlScript += GetEnumHeaderFooter(tbl.Name, tempEnumName, IdCol, DescriptionCol, enumMemberScript, idColIsString, IsVb);
										enumMemberScript = "";
									}

									if (IsVb) {
										if (idColIsString) {
											enumMemberScript += "\t\t\tPublic Const " + enumMemberName + " As String = \"" + enumMemberValue + "\";\r\n";
										}
										else {
											enumMemberScript += "\t\t\t" + enumMemberName + " = " + enumMemberValue;
										}
									} else {
										if (idColIsString) {
											enumMemberScript += "\t\t\tpublic const string " + enumMemberName + " = \"" + enumMemberValue + "\";\r\n";
										}
										else {
											enumMemberScript += (enumMemberScript == "" ? "" : ",\r\n");
											enumMemberScript += "\t\t\t" + enumMemberName + " = " + enumMemberValue;
										}
									}

									enumValList.Add(enumMemberName);
									lastKeyVal = enumKeyVal;
									rowCount++;
								}
							}
							if (rowCount == 0) {
								SqlScript += "\r\n\t\t" + commentmarker + " " + tbl.Name + ": enum generation was specfied but the database table had no records\r\n";
							}
							else {
								string tempEnumName = (isMulti ? GetEnumName(lastKeyVal, idColIsString) : EnumName);
								SqlScript += GetEnumHeaderFooter(tbl.Name, tempEnumName, IdCol, DescriptionCol, enumMemberScript, idColIsString, IsVb);
							}
						}
						catch (Exception ex) {
							SqlScript += "\r\n\t\t" + commentmarker + " SQL fetch error in SQL \"" + sql + "\" : " + ex.Message;
						}

					}
					rtn += SqlScript;
				}
			}
			return rtn;
		}

	}
}
