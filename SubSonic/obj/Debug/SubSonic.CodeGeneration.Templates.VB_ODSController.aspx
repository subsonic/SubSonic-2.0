<%@ Page Language="C#" %>
<%@ Import namespace="SubSonic.Utilities"%>
<%@ Import Namespace="SubSonic" %>
<%
    //The data we need
	const string providerName = "#PROVIDER#";
	const string tableName = "#TABLE#";
	TableSchema.Table tbl = DataService.GetSchema(tableName, providerName, TableType.Table);
	DataProvider provider = DataService.Providers[providerName];
	ICodeLanguage lang = new VBCodeLanguage();
    
    //The main vars we need
    TableSchema.TableColumnCollection cols = tbl.Columns;
    TableSchema.TableColumn[] keys = cols.GetPrimaryKeys();

  const bool showGenerationInfo = false;
  
%>

<% if(showGenerationInfo)
   { %>
 'Generated on <%=DateTime.Now.ToString() %> by <%=Environment.UserName %>
<% }  %>
<%
    if(keys.Length > 0)
    {
%>
Namespace <%=provider.GeneratedNamespace %>

    ''' <summary>
    ''' Controller class for <%=tbl.Name %>
    ''' </summary>
		[<]System.ComponentModel.DataObject()[>] Public Partial Class <%=tbl.ClassName %>Controller
    
        ' Preload our schema..
        Dim thisSchemaLoad As <%=tbl.ClassName%> = New <%=tbl.ClassName%>()

        Private strUserName As String = String.Empty
        Protected ReadOnly Property UserName() As String
            Get
				If strUserName.Length = 0 Then
		        
    				If Not System.Web.HttpContext.Current Is Nothing Then
						strUserName = System.Web.HttpContext.Current.User.Identity.Name
					Else
		        		strUserName = System.Threading.Thread.CurrentPrincipal.Identity.Name
					End If
					Return strUserName
				End If
				Return strUserName
			End Get
        End Property

        [<]DataObjectMethod(DataObjectMethodType.Select, True)[>] Public Function FetchAll() As <%=tbl.ClassName%>Collection
        
            Dim coll As <%=tbl.ClassName%>Collection = New <%=tbl.ClassName%>Collection()
            Dim qry As Query = New Query(<%=tbl.ClassName%>.Schema)
            coll.LoadAndCloseReader(qry.ExecuteReader())
            Return coll
            
        End Function

        [<]DataObjectMethod(DataObjectMethodType.Select, True)[>] Public Function FetchByID(ByVal <%=tbl.PrimaryKey.PropertyName%> As Object) As <%=tbl.ClassName%>Collection 
        
            Dim coll As <%=tbl.ClassName%>Collection = New <%=tbl.ClassName%>Collection().Where("<%=tbl.PrimaryKey.ColumnName %>", <%=tbl.PrimaryKey.PropertyName%>).Load()
            Return coll
        
        End Function
        
        [<]DataObjectMethod(DataObjectMethodType.Select, True)[>] Public Function FetchByQuery(ByVal qry As SubSonic.Query) As <%=tbl.ClassName%>Collection 
        
            Dim coll As <%=tbl.ClassName%>Collection = New <%=tbl.ClassName%>Collection()
            coll.LoadAndCloseReader(qry.ExecuteReader())
            Return coll
        
        End Function

        [<]DataObjectMethod(DataObjectMethodType.Delete, True)[>] Public Function Delete(ByVal <%=tbl.PrimaryKey.PropertyName%> As Object) as Boolean
        
            Return (<%=tbl.ClassName%>.Delete(<%=tbl.PrimaryKey.PropertyName%>) = 1)
        
        End Function

        [<]DataObjectMethod(DataObjectMethodType.Delete, False)[>] Public Function Destroy(ByVal <%=tbl.PrimaryKey.PropertyName%> As Object) as Boolean
        
            Return (<%=tbl.ClassName%>.Destroy(<%=tbl.PrimaryKey.PropertyName%>) = 1)
        
        End Function

        <%
            string deleteArgs = String.Empty;
            string whereArgs = String.Empty;

            string deleteDelim = String.Empty;
            string whereDelim = String.Empty;
			bool useNullables = tbl.Provider.GenerateNullableProperties;
			foreach (TableSchema.TableColumn key in keys)
			{
				string propName = key.PropertyName;
				bool useNullType = useNullables ? key.IsNullable : false;
				string varType = Utility.GetVariableType(key.DataType, useNullType, lang);

                deleteArgs += deleteDelim + "ByVal " + propName + " As " + varType;
                deleteDelim = ",";

                whereArgs += whereDelim + "(\"" + propName + "\", " + propName + ")";
                whereDelim = ".AND";
                
            }
            // add this delete method if the table has multiple keys
            if (keys.Length > 1)
            {
        %>
       
        [<]DataObjectMethod(DataObjectMethodType.Delete, True)[>] Public Function Delete(<%=deleteArgs%>) as Boolean
            Dim qry As Query = new Query(<%=tbl.ClassName%>.Schema)
            qry.QueryType = QueryType.Delete
            qry.AddWhere<%=whereArgs%>
            qry.Execute()
            Return true
        End Function
       
    	<%
    	    }
			string insertArgs = String.Empty;
			string updateArgs = String.Empty;
			const string seperator = ",";

			foreach (TableSchema.TableColumn col in cols)
			{
				string propName = col.PropertyName;
				bool useNullType = useNullables ? col.IsNullable : false;
				string varType = Utility.GetVariableType(col.DataType, useNullType, lang);

				updateArgs += "ByVal " + propName + " As " + varType + seperator;
				if (!col.AutoIncrement)
				{
					insertArgs += "ByVal " + propName + " As " + varType + seperator;
				}
			}
			if (insertArgs.Length > 0)
				insertArgs = insertArgs.Remove(insertArgs.Length - seperator.Length, seperator.Length);
			if (updateArgs.Length > 0)
				updateArgs = updateArgs.Remove(updateArgs.Length - seperator.Length, seperator.Length);
    	%>
    	
	    ''' <summary>
	    ''' Inserts a record, can be used with the Object Data Source
	    ''' </summary>
        [<]DataObjectMethod(DataObjectMethodType.Insert, True)[>] Public Sub Insert(<%=insertArgs%>)
	   
		    Dim item As <%=tbl.ClassName %> = New <%= tbl.ClassName %>()
		    <% 
		    foreach (TableSchema.TableColumn col in cols) {
                if (!col.AutoIncrement) { 
            %>
            item.<%=col.PropertyName%> = <%=col.PropertyName%>
            <%
                }
              } 
            %>
	    
		    item.Save(UserName)
	   
	   End Sub

    	
	    ''' <summary>
	    ''' Updates a record, can be used with the Object Data Source
	    ''' </summary>
        [<]DataObjectMethod(DataObjectMethodType.Update, True)[>] Public Sub Update(<%=updateArgs%>)
	    
		    Dim item As <%=tbl.ClassName%> = New <%=tbl.ClassName %>()
	        item.MarkOld()
	        item.IsLoaded = True
		    <% 
		    foreach (TableSchema.TableColumn col in cols) 
			{
				%>
			item.<%=col.PropertyName%> = <%=col.PropertyName%> 
				<%
			} 
            %>
	        item.Save(UserName)
	    End Sub

    End Class

End Namespace
<%
    }
    else
    {
%>
' The class <%= tbl.ClassName %>Controller was not generated because <%= tableName %> does not have a primary key.
<% } %>