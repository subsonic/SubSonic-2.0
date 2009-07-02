<%@ Page Language="C#"%>
<%@ Import namespace="SubSonic.Utilities"%>
<%@ Import Namespace="SubSonic" %>

<%
	const string providerName = "#PROVIDER#";
	const string viewName = "#VIEW#";
	//The data we need
	TableSchema.Table view = DataService.GetSchema(viewName, providerName, TableType.View);
	ICodeLanguage language = new VBCodeLanguage();

	//The main vars we need
	TableSchema.TableColumnCollection cols = view.Columns;
	string className = view.ClassName;
	string nSpace = DataService.Providers[providerName].GeneratedNamespace;

    
%>
Namespace <%=nSpace %>
    ''' <summary>
    ''' Strongly-typed collection for the <%=className%> class.
    ''' </summary>
    <Serializable()> _
    Public Partial Class <%=className%>Collection 
    Inherits ReadOnlyList(Of <%= className %>, <%=className%>Collection)       
        Public Sub New()
        End Sub
    End Class

    ''' <summary>
    ''' This is  Read-only wrapper class for the <%=viewName%> view.
    ''' </summary>
    <Serializable()> _
    Public Partial Class <%=className%> 
    Inherits ReadOnlyRecord(Of <%= className %>)
    
	    #Region "Default Settings"
	    Protected Shared Sub SetSQLProps()
	        GetTableSchema()
	    End Sub
	    #End Region

        #Region "Schema Accessor"
        Public Shared ReadOnly Property Schema() As TableSchema.Table
            Get
                If (BaseSchema Is Nothing) Then
                    SetSQLProps()
                End If
                Return BaseSchema
            End Get
        End Property
	    
	    Private Shared Sub GetTableSchema()
	        If (Not IsSchemaInitialized) Then
	            'Schema declaration
				Dim schema As TableSchema.Table = New TableSchema.Table("<%=viewName%>", TableType.View, DataService.GetInstance("<%=providerName%>"))
				schema.Columns = New TableSchema.TableColumnCollection()
				schema.SchemaName = "<%=view.SchemaName %>"
                
                'Columns
                <%
                string propertyName = String.Empty;
                foreach(TableSchema.TableColumn col in cols){
                    string varName = "col" + col.ArgumentName;
                %>
                Dim <%=varName %> As New TableSchema.TableColumn(schema)
                <%=varName %>.ColumnName = "<%= col.ColumnName %>"
                <%=varName %>.DataType = DbType.<%=col.DataType %>
                <%=varName %>.MaxLength = <%=col.MaxLength %>
                <%=varName %>.AutoIncrement = False
                <%=varName %>.IsNullable = <%=col.IsNullable.ToString().ToLower()%>
                <%=varName %>.IsPrimaryKey = False
                <%=varName %>.IsForeignKey = False
                <%=varName %>.IsReadOnly = <%= col.IsReadOnly.ToString().ToLower() %>
                 <%
				if(col.IsForeignKey)
				{
                %>
				<%=varName %>.ForeignKeyTableName = "<%= col.ForeignKeyTableName %>"
                <% } %>
                schema.Columns.Add(<%=varName%>)
                <%
                }
                %>
                BaseSchema = schema
				
				'add this schema to the provider
                'so we can query it later
                DataService.Providers("<%=providerName %>").AddSchema("<%=viewName%>",schema)
	        End If
	    End Sub
	    #End Region
	    
        #Region "Query Accessor"
        Public Shared Function CreateQuery As Query
            Return New Query(Schema)
        End Function
	    #End Region
	    
	    #Region ".ctors"
	    Public Sub New()
	        SetSQLProps()
            SetDefaults()
            MarkNew()
	    End Sub
	    
		Public Sub New(ByVal useDatabaseDefaults As Boolean)
			SetSQLProps()
			If useDatabaseDefaults = True Then
				ForceDefaults()
			End If
			MarkNew()
		End Sub
		
	    Public Sub New(ByVal keyID As Object)
	        SetSQLProps()
		    LoadByKey(keyID)
	    End Sub
    	
    	Public Sub new(ByVal columnName As String, ByVal columnValue As Object)
    	    SetSQLProps()
            LoadByParam(columnName , columnValue)
    	End Sub
	    #End Region
	    
	    #Region "Props"
	    <%
        foreach(TableSchema.TableColumn col in cols){
            string propName = col.PropertyName;
            string varType = Utility.GetVariableType(col.DataType, col.IsNullable, language);
        %>  
        <XmlAttribute("<%=propName%>")> _
        Public Property <%=propName%>() As <%=varType%> 
		    Get
			    Return GetColumnValue(Of <%= varType %>)("<%= col.ColumnName %>")
			End Get
            Set(ByVal value As <%=varType%>)
			    SetColumnValue("<%=col.ColumnName%>", value)
            End Set
        End Property
	    <%
	    }
	    %>
	    #End Region
    
	    #Region "Columns Struct"
	    Public Structure Columns
			Dim x as Integer
	        <% 
		    foreach (TableSchema.TableColumn col in cols) {
                string propName = col.PropertyName;
            %>
            Public Shared <%=propName%> As String = "<%=col.ColumnName%>"
            <%
              } 
            %>
	    End Structure
	    #End Region
    End Class
End Namespace
