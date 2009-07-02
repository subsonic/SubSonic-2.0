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
	string className = tbl.ClassName;
    string thisPK = null;
    string varPK = null;
    string varPKType = null;
    if (tbl.PrimaryKey != null)
    {
        thisPK = tbl.PrimaryKey.PropertyName;
        varPK = tbl.PrimaryKey.ArgumentName;
        varPKType = Utility.GetVariableType(tbl.PrimaryKey.DataType, tbl.PrimaryKey.IsNullable, lang);
    }
  const bool showGenerationInfo = false;

  string baseClass = tbl.Provider.TableBaseClass;
  string collectionBaseClass = String.Format("List(Of {0})", className);

  if (baseClass == "RepositoryRecord" || baseClass == "ActiveRecord")
  {
      //string baseSuffix;

      if (tbl.Provider.TableBaseClass == "RepositoryRecord")
          collectionBaseClass = String.Format("RepositoryList(Of {0}, {0}Collection)", className);
      else
          collectionBaseClass = String.Format("ActiveList(Of {0}, {0}Collection)", className);

      baseClass = String.Format("Inherits {0}(Of {1})\r\n", baseClass, className);
  }
%>

<% if(showGenerationInfo)
   { %>
 'Generated on <%=DateTime.Now.ToString() %> by <%=Environment.UserName %>
<% }  %>
<%
    if(thisPK != null)
    {
%>
Namespace <%=provider.GeneratedNamespace %>
	''' <summary>
	''' Strongly-typed collection for the <%=className%> class.
	''' </summary>
	<Serializable> _
	Public Partial Class <%=className%>Collection 
	Inherits <%=collectionBaseClass%>
	    Public Sub New()
		End Sub
		
		Public Function Filter() As <%=className %>Collection
			For i As Integer = Me.Count - 1 To 0 Step -1
				Dim o As <%=className %> = Me(i)
				For Each w As SubSonic.Where In Me.wheres
					Dim remove As Boolean = False
					Dim pi As System.Reflection.PropertyInfo = o.GetType().GetProperty(w.ColumnName)
					If pi.CanRead Then
						Dim val As Object = pi.GetValue(o, Nothing)
						Select Case w.Comparison
							Case SubSonic.Comparison.Equals
								If (Not val.Equals(w.ParameterValue)) Then
									remove = True
								End If
						End Select
					End If

					If remove Then
						Me.Remove(o)
						Exit For
					End If
				Next w
			Next i
			Return Me
		End Function
		
		
	End Class

	''' <summary>
	''' This is an ActiveRecord class which wraps the <%=tableName%> table.
	''' </summary>
	<Serializable> _
	Public Partial Class <%=className%> 
	<%=baseClass%>
		#Region ".ctors and Default Settings"
		
		Public Sub New()
			SetSQLProps()
			InitSetDefaults()
			MarkNew()
		End Sub
		
		Public Sub New(ByVal useDatabaseDefaults As Boolean)
			SetSQLProps()
			If useDatabaseDefaults = True Then
				ForceDefaults()
			End If
			MarkNew()
		End Sub

		Private Sub InitSetDefaults()
			SetDefaults()
		End Sub

        <%
        if(tbl.Provider.TableBaseClass == "ActiveRecord")
        {%>
		Public Sub New(ByVal keyID As Object)
			SetSQLProps()
			InitSetDefaults()
			LoadByKey(keyID)
		End Sub

		Public Sub New(ByVal columnName As String, ByVal columnValue As Object)
			SetSQLProps()
			InitSetDefaults()
			LoadByParam(columnName,columnValue)
		End Sub
		<%
        }%>
        
		Protected Shared Sub SetSQLProps()
			GetTableSchema()
		End Sub

		#End Region
		
		#Region "Schema and Query Accessor"
		
		Public Shared ReadOnly Property Schema() As TableSchema.Table
			Get
				If BaseSchema Is Nothing Then
					SetSQLProps()
				End If

				Return BaseSchema
			End Get

		End Property

		Private Shared Sub GetTableSchema()
			If (Not IsSchemaInitialized) Then
				'Schema declaration
				Dim schema As TableSchema.Table = New TableSchema.Table("<%=tableName%>", TableType.Table, DataService.GetInstance("<%=providerName%>"))
				schema.Columns = New TableSchema.TableColumnCollection()
				schema.SchemaName = "<%=tbl.SchemaName %>"
				'columns
				
                <%
                foreach(TableSchema.TableColumn col in cols)
                {
                    string varName = "col" +  col.ArgumentName;
                %>
                Dim <%=varName %> As TableSchema.TableColumn = New TableSchema.TableColumn(schema)
                <%=varName %>.ColumnName = "<%=col.ColumnName%>"
                <%=varName %>.DataType = DbType.<%=col.DataType %>
                <%=varName %>.MaxLength = <%=col.MaxLength %>
                <%=varName %>.AutoIncrement = <%=col.AutoIncrement.ToString().ToLower() %>
                <%=varName %>.IsNullable = <%=col.IsNullable.ToString().ToLower()%>
                <%=varName %>.IsPrimaryKey = <%=col.IsPrimaryKey.ToString().ToLower()%>
                <%=varName %>.IsForeignKey = <%=col.IsForeignKey.ToString().ToLower()%>
                <%=varName %>.IsReadOnly = <%= col.IsReadOnly.ToString().ToLower() %>
                <% if (!String.IsNullOrEmpty(col.DefaultSetting))
				   {					   
				%>
						<%=varName%>.DefaultSetting = "<%= col.DefaultSetting%>"
				<%
					}
                %>
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
                DataService.Providers("<%=providerName %>").AddSchema("<%=tableName%>",schema)
			End If
		End Sub

		Public Shared Function CreateQuery() As Query
			Return New Query(Schema)
		End Function
		
		#End Region
		
		#Region "Props"
	
        <%
			bool useNullables = tbl.Provider.GenerateNullableProperties;
			foreach(TableSchema.TableColumn col in cols)
			{
            string propName = col.PropertyName;
			string varType = Utility.GetVariableType(col.DataType, false, lang);
            string nullableVarType = Utility.GetVariableType(col.DataType, col.IsNullable, lang);

			if (useNullables || Utility.IsMatch(varType, nullableVarType))
			{
        %>
        <Bindable(True)> _  
        <XmlAttribute("<%=propName%>")> _
        Public Property <%=propName%> As <%=nullableVarType%> 
			Get
				Return GetColumnValue(Of <%= nullableVarType %>)(Columns.<%=col.PropertyName%>)
			End Get
		    
			Set
				SetColumnValue(Columns.<%=col.PropertyName%>, Value)
			End Set
		End Property
		
		
		<%
			}
			else
			{  
	    %>
	    <Bindable(True)> _
		<XmlAttribute("<%=propName%>")> _
		Public Property <%=propName%> As <%=varType%> 
			Get
				Dim prop<%=propName%> As <%= nullableVarType%> = GetColumnValue(Of <%= nullableVarType%>)(Columns.<%=col.PropertyName%>)
				If Not prop<%=propName%>.HasValue Then
					Return <%= Utility.GetDefaultValue(col, lang)%>
				End If	
				Return prop<%=propName%>.Value
			End Get
		    
			Set
				SetColumnValue(Columns.<%=col.PropertyName%>, Value)
			End Set
		End Property

		<XmlIgnore()> _
		Public Property <%=propName%>HasValue As Boolean
			Get
				Dim prop<%=propName%> As <%= nullableVarType%> = GetColumnValue(Of <%= nullableVarType%>)(Columns.<%=col.PropertyName%>)
				If prop<%=propName%>.HasValue Then
					Return True
				End If
				Return False
			End Get
			
			Set
			  Dim prop<%=propName%> As <%= nullableVarType%> = GetColumnValue(Of <%= nullableVarType%>)(Columns.<%=col.PropertyName%>)
			  If Value = False Then
				 SetColumnValue(Columns.<%=col.PropertyName%>, Nothing)
			  Else If Value = True And prop<%=propName%>.HasValue = False Then
				 SetColumnValue(Columns.<%=col.PropertyName%>, <%= Utility.GetDefaultValue(col, lang)%>)
			  End If
		   End Set
		End Property
	    
		<% 
			}
        }
	    %>
		

		#End Region
		
		
		<%
        if(tbl.PrimaryKeyTables.Count > 0 && tbl.Provider.TableBaseClass == "ActiveRecord")
        {%>
        
	    #Region "PrimaryKey Methods"
	    <%
    TableSchema.PrimaryKeyTableCollection pkTables = tbl.PrimaryKeyTables;
    if(pkTables != null)
    {
        ArrayList usedMethodNames = new ArrayList();
        foreach(TableSchema.PrimaryKeyTable pk in pkTables)
        {
            TableSchema.Table pkTbl = DataService.GetSchema(pk.TableName, providerName, TableType.Table);
            if(pkTbl.PrimaryKey != null && CodeService.ShouldGenerate(pkTbl))
            {
                string pkClass = pk.ClassName;
				string pkClassQualified = provider.GeneratedNamespace + "." + pkClass;
                string pkMethod = pk.ClassNamePlural;
				string pkColumn = pkTbl.GetColumn(pk.ColumnName).PropertyName;

                if(Utility.IsMatch(pkClass, pkMethod))
                {
                    pkMethod += "Records";
                }

                if(pk.ClassName == className)
                {
                    pkMethod = "Child" + pkMethod;
                }

                if(usedMethodNames.Contains(pkMethod))
                {
                    pkMethod += "From" + className;
                    if(usedMethodNames.Contains(pkMethod))
                    {
                        pkMethod += pkColumn;
                    }
                }

                usedMethodNames.Add(pkMethod);
                
                if(!String.IsNullOrEmpty(provider.RelatedTableLoadPrefix))
				{
					pkMethod = provider.RelatedTableLoadPrefix + pkMethod;
				}

                bool methodsNoLazyLoad = !provider.GenerateRelatedTablesAsProperties && !provider.GenerateLazyLoads;
                bool methodsLazyLoad = !provider.GenerateRelatedTablesAsProperties && provider.GenerateLazyLoads;
                bool propertiesNoLazyLoad = provider.GenerateRelatedTablesAsProperties && !provider.GenerateLazyLoads;
                bool propertiesLazyLoad = provider.GenerateRelatedTablesAsProperties && provider.GenerateLazyLoads;
				
                if(methodsNoLazyLoad)
                {
%>
			Public Function <%=pkMethod%>() As <%=pkClassQualified%>Collection 
	
				Return New <%=pkClassQualified%>Collection().Where(<%=pkTbl.ClassName%>.Columns.<%=pkColumn%>, <%=tbl.PrimaryKey.PropertyName%>).Load()
	
			End Function
			<%
                }
                else if(methodsLazyLoad)
                {
%>
			Dim col<%=pkMethod%> As <%=pkClassQualified%>Collection 
			Public Function <%=pkMethod%>() As <%=pkClassQualified%>Collection 
	
				If(col<%=pkMethod%> Is Nothing)
					col<%=pkMethod%> = New <%=pkClassQualified%>Collection().Where(<%=pkTbl.ClassName%>.Columns.<%=pkColumn%>, <%=tbl.PrimaryKey.PropertyName%>).Load()
				End If
				
				Return col<%=pkMethod%> 
				
			End Function
<%
                }
                else if(propertiesNoLazyLoad)
                {
%>
			Public ReadOnly Property <%=pkMethod%>() As <%=pkClassQualified%>Collection 

				Get
					Return New <%=pkClassQualified%>Collection().Where(<%=pkTbl.ClassName%>.Columns.<%=pkColumn%>, <%=tbl.PrimaryKey.PropertyName%>).Load()
				End Get
				
			End Property
<%
                }
                else if(propertiesLazyLoad)
                {
%>	
			Dim col<%=pkMethod%> As <%=pkClassQualified%>Collection 
			Public Property <%=pkMethod%>() As <%=pk.ClassName%>Collection 
				
				Get
					If(col<%=pkMethod%> Is Nothing)
						col<%=pkMethod%> = New <%=pkClassQualified%>Collection().Where(<%=pkTbl.ClassName%>.Columns.<%=pkColumn%>, <%=tbl.PrimaryKey.PropertyName%>).Load()
					End If
					
					Return col<%=pkMethod%>
				End Get
				
				Set(ByVal Value As <%=pk.ClassName%>Collection)
					col<%=pkMethod%> = Value
				End Set
				
			End Property
		<%
                }
            }
        }
    }
%>
		#End Region
		
		<%
}
%>

		
		
		
		
		
		<%
        if(tbl.ForeignKeys.Count > 0 && tbl.Provider.TableBaseClass == "ActiveRecord")
        {%>
	    #Region "ForeignKey Methods"
	    <%
			TableSchema.ForeignKeyTableCollection fkTables = tbl.ForeignKeys;

			if (fkTables != null)
			{
				ArrayList usedPropertyNames = new ArrayList();
				foreach (TableSchema.ForeignKeyTable fk in tbl.ForeignKeys)
				{
					TableSchema.Table fkTbl = DataService.GetSchema(fk.TableName, providerName, TableType.Table);
					if (CodeService.ShouldGenerate(fkTbl))
					{
						string fkClass = fk.ClassName;
						string fkClassQualified = provider.GeneratedNamespace + "." + fkClass;
						string fkMethod = fk.ClassName;
						string fkID = tbl.GetColumn(fk.ColumnName).PropertyName;
						string fkColumnID = fk.ColumnName;


						//it's possible this table is "relatin to itself"
						//check to make sure the class names are not the same
						//if they are, use the fk columnName
						if (fk.ClassName == className)
						{
							fkMethod = "Parent" + fk.ClassName;
						}

						if (usedPropertyNames.Contains(fk.ClassName))
						{
							fkMethod += "To" + fkID;
						}

						if (tbl.GetColumn(fkMethod) != null)
						{
							fkMethod += "Record";
						}
	    %>

		''' <summary>
		''' Returns a <%=fkClass%> ActiveRecord object related to this <%=className%>
		''' </summary>

		Public Property <%=fkMethod%>() As <%=fkClassQualified%>
			Get
				Return <%=fkClassQualified%>.FetchByID(Me.<%=fkID%>)
			End Get

			Set
				SetColumnValue("<%=fkColumnID%>", Value.<%=fkTbl.PrimaryKey.PropertyName%>)
			End Set

		End Property

	    <%
			usedPropertyNames.Add(fk.ClassName);
		}
	}
}
	    %>
	    #End Region
	    <%} else {%>
	    'no foreign key tables defined (<%=tbl.ForeignKeys.Count.ToString() %>)
	    <%} %>
		




		<%
        if(tbl.ManyToManys.Count > 0 && tbl.Provider.TableBaseClass == "ActiveRecord")
        {%>
	    #Region "Many To Many Helpers"
	    <%
			TableSchema.ManyToManyRelationshipCollection mm = tbl.ManyToManys;
			if (mm != null)
			{
				ArrayList usedConstraints = new ArrayList();
				foreach (TableSchema.ManyToManyRelationship m in mm)
				{
					TableSchema.Table fkSchema = DataService.GetSchema(m.ForeignTableName, providerName, TableType.Table);
					if (fkSchema != null && !usedConstraints.Contains(fkSchema.ClassName) && CodeService.ShouldGenerate(fkSchema) && CodeService.ShouldGenerate(m.MapTableName, m.Provider.Name))
					{
						usedConstraints.Add(fkSchema.ClassName);
						string fkClass = fkSchema.ClassName;
						string fkClassQualified = provider.GeneratedNamespace + "." + fkClass;
                        string mapParameter = Utility.PrefixParameter(m.MapTableLocalTableKeyColumn, provider);
                        string getSql = "SELECT * FROM " + fkSchema.QualifiedName +
                                        SqlFragment.INNER_JOIN + Utility.QualifyTableName(m.SchemaName, m.MapTableName, provider) +
                                        SqlFragment.ON + Utility.QualifyColumnName(m.ForeignTableName, m.ForeignPrimaryKey, provider) + SqlFragment.EQUAL_TO +
                                        Utility.QualifyColumnName(m.MapTableName, m.MapTableForeignTableKeyColumn, provider) +
                                        SqlFragment.WHERE + Utility.QualifyColumnName(m.MapTableName, m.MapTableLocalTableKeyColumn, provider) + SqlFragment.EQUAL_TO +
                                        mapParameter;
                        string deleteSql = "DELETE FROM " + Utility.QualifyTableName(m.SchemaName, m.MapTableName, provider) +
                                        SqlFragment.WHERE + Utility.QualifyColumnName(m.MapTableName, m.MapTableLocalTableKeyColumn, provider) + SqlFragment.EQUAL_TO +
                                        mapParameter;
                        string varFKType = Utility.GetVariableType(fkSchema.PrimaryKey.DataType, fkSchema.PrimaryKey.IsNullable, lang);
                
	    %>
	     
        Public Function Get<%=fkClass%>Collection() As <%=fkClassQualified%>Collection 
	        Return <%=className%>.Get<%=fkClass%>Collection(Me.<%=thisPK%>)
		End Function
		
        Public Shared Function Get<%=fkClass%>Collection(ByVal <%= varPK%> As <%= varPKType%>) As <%=fkClassQualified%>Collection
        
            Dim cmd As SubSonic.QueryCommand = New SubSonic.QueryCommand("<%= getSql %>", <%=className%>.Schema.Provider.Name)
            
            cmd.AddParameter("<%= mapParameter %>", <%= varPK%>, DbType.<%= DataService.GetSchema(m.MapTableName, providerName).GetColumn(m.MapTableLocalTableKeyColumn).DataType.ToString() %>)
            Dim rdr As IDataReader = SubSonic.DataService.GetReader(cmd)
            Dim coll As <%=fkClass%>Collection = New <%=fkClass%>Collection()
            coll.LoadAndCloseReader(rdr)

            Return coll
            
        End Function
        
        Public Shared Sub Save<%=fkClass%>Map(ByVal <%= varPK%> As <%= varPKType%>, ByVal items As <%=fkClass%>Collection)
        
            Dim coll As QueryCommandCollection = New SubSonic.QueryCommandCollection()

            'delete out the existing
            Dim cmdDel As QueryCommand = New QueryCommand("<%= deleteSql %>", <%=className%>.Schema.Provider.Name)
            cmdDel.AddParameter("<%= mapParameter %>", <%= varPK%>, DbType.<%=DataService.GetSchema(m.MapTableName, providerName).GetColumn(m.MapTableLocalTableKeyColumn).DataType.ToString()%>)
            'add this in
            coll.Add(cmdDel)
			DataService.ExecuteTransaction(coll)

            For Each item As <%=fkClass%> In items 
				Dim var<%=m.ClassName%> As <%=m.ClassName%> = New <%= m.ClassName%>()
				var<%=m.ClassName%>.SetColumnValue("<%=m.MapTableLocalTableKeyColumn%>", <%= varPK%>)
				var<%=m.ClassName%>.SetColumnValue("<%=m.MapTableForeignTableKeyColumn%>", item.GetPrimaryKeyValue())
				var<%=m.ClassName%>.Save()
			Next
			
        End Sub
        
        
        Public Shared Sub Save<%=fkClass%>Map(ByVal <%= varPK%> As <%= varPKType%>, ByVal itemList As System.Web.UI.WebControls.ListItemCollection)
        
            Dim coll As QueryCommandCollection = New SubSonic.QueryCommandCollection()

            'delete out the existing
            Dim cmdDel As QueryCommand = New QueryCommand("<%= deleteSql %>", <%=className%>.Schema.Provider.Name)
            cmdDel.AddParameter("<%= mapParameter %>", <%= varPK%>, DbType.<%=DataService.GetSchema(m.MapTableName, providerName).GetColumn(m.MapTableLocalTableKeyColumn).DataType.ToString()%>)

            'add this in
            coll.Add(cmdDel)
            DataService.ExecuteTransaction(coll)

            For Each l As System.Web.UI.WebControls.ListItem In itemList 
            
                If l.Selected
				    
					Dim var<%=m.ClassName%> As <%=m.ClassName%> = New <%= m.ClassName%>()
					var<%=m.ClassName%>.SetColumnValue("<%=m.MapTableLocalTableKeyColumn%>", <%= varPK%>)
					var<%=m.ClassName%>.SetColumnValue("<%=m.MapTableForeignTableKeyColumn%>", l.Value)
					var<%=m.ClassName%>.Save()
				    
                End If

            Next

        End Sub
        
        Public Shared Sub Save<%=fkClass%>Map(ByVal <%= varPK%> As <%= varPKType%>, ByVal itemList() As <%= varFKType%>) 
        
            Dim coll As QueryCommandCollection = New SubSonic.QueryCommandCollection()

            'delete out the existing
            Dim cmdDel As QueryCommand = New QueryCommand("<%= deleteSql %>", <%=className%>.Schema.Provider.Name)
            cmdDel.AddParameter("<%= mapParameter %>", <%= varPK%>, DbType.<%=DataService.GetSchema(m.MapTableName, providerName).GetColumn(m.MapTableLocalTableKeyColumn).DataType.ToString()%>)

            'add this in
            coll.Add(cmdDel)
			DataService.ExecuteTransaction(coll)
            
            For Each item As <%= varFKType%> In itemList
            
                	Dim var<%=m.ClassName%> As <%=m.ClassName%> = New <%= m.ClassName%>()
					var<%=m.ClassName%>.SetColumnValue("<%=m.MapTableLocalTableKeyColumn%>", <%= varPK%>)
					var<%=m.ClassName%>.SetColumnValue("<%=m.MapTableForeignTableKeyColumn%>", item)
					var<%=m.ClassName%>.Save()

            Next
            
        End Sub
        
		Public Shared Sub Delete<%=fkClass%>Map(ByVal <%= varPK%> As <%= varPKType%>) 
		
            Dim cmdDel As QueryCommand = New QueryCommand("<%= deleteSql %>", <%=className%>.Schema.Provider.Name)
            cmdDel.AddParameter("<%= mapParameter %>", <%= varPK%>, DbType.<%=DataService.GetSchema(m.MapTableName, providerName).GetColumn(m.MapTableLocalTableKeyColumn).DataType.ToString()%>)
            DataService.ExecuteQuery(cmdDel)

        End Sub
	      
	      
	      
	    <%
			}
		}
	}
	    %>
	    #End Region
	    <%} else {%>
	    'no ManyToMany tables defined (<%=tbl.ManyToManys.Count.ToString() %>)
	    <%} %>

		
        <%
        if(tbl.Provider.TableBaseClass == "ActiveRecord")
        {%>
		#Region "ObjectDataSource support"
		<%
            string insertArgs = String.Empty;
            string updateArgs = String.Empty;
			const string seperator = ",";

			foreach (TableSchema.TableColumn col in cols)
			{
				string propName = col.ArgumentName;
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
		Public Shared Sub Insert(<%=insertArgs%>)
			Dim item As <%=className%> = New <%=className%>()
			<% 
		    foreach (TableSchema.TableColumn col in cols) {
				if (!col.AutoIncrement)
				{ 
            %>
            item.<%=col.PropertyName%> = <%=col.ArgumentName%>
            <%
                }
              } 
            %>
			If Not System.Web.HttpContext.Current Is Nothing Then
				item.Save(System.Web.HttpContext.Current.User.Identity.Name)
			Else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name)
			End If
		End Sub

		''' <summary>
		''' Updates a record, can be used with the Object Data Source
		''' </summary>
		Public Shared Sub Update(<%=updateArgs%>)
			Dim item As <%=className%> = New <%=className%>()
		    <% 
		    foreach (TableSchema.TableColumn col in cols) 
			{
				%>
                item.<%=col.PropertyName%> = <%=col.ArgumentName%>
				<%
			} 
            %>
			item.IsNew = False
			If Not System.Web.HttpContext.Current Is Nothing Then
				item.Save(System.Web.HttpContext.Current.User.Identity.Name)
			Else
				item.Save(System.Threading.Thread.CurrentPrincipal.Identity.Name)
			End If
		End Sub

		#End Region
		<%} %>
		
		#Region "Typed Columns"
        <% for (int i = 0; i < cols.Count; i++)
               {%>
        
        Public Shared ReadOnly Property <%= cols[i].PropertyName%>Column() As TableSchema.TableColumn
            Get
                Return Schema.Columns(<%= i %>)
            End Get
        End Property
        <%} %>
        
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
		
				
		#Region "Update PK Collections"
		
<%
    if (tbl.PrimaryKeyTables.Count > 0 && tbl.Provider.TableBaseClass == "ActiveRecord")
    {
        %>
        Public Sub SetPKValues()
<%
        TableSchema.PrimaryKeyTableCollection pkTables = tbl.PrimaryKeyTables;
        if (pkTables != null)
        {
            ArrayList usedMethodNames = new ArrayList();
            foreach (TableSchema.PrimaryKeyTable pk in pkTables)
            {
                TableSchema.Table pkTbl = DataService.GetSchema(pk.TableName, providerName, TableType.Table);
                if (pkTbl.PrimaryKey != null && CodeService.ShouldGenerate(pkTbl))
                {
                    string pkClass = pk.ClassName;
                    string pkClassQualified = provider.GeneratedNamespace + "." + pkClass;
                    string pkMethod = pk.ClassNamePlural;
                    TableSchema.TableColumn pkColumn = pkTbl.GetColumn(pk.ColumnName);
                    string pkColumnName = pkColumn.PropertyName;

                    if (Utility.IsMatch(pkClass, pkMethod))
                    {
                        pkMethod += "Records";
                    }

                    if (pk.ClassName == className)
                    {
                        pkMethod = "Child" + pkMethod;
                    }

                    if (usedMethodNames.Contains(pkMethod))
                    {
                        pkMethod += "From" + className;
                        if (usedMethodNames.Contains(pkMethod))
                        {
                            pkMethod += pkColumn;
                        }
                    }

                    usedMethodNames.Add(pkMethod);

                    if (!String.IsNullOrEmpty(provider.RelatedTableLoadPrefix))
                    {
                        pkMethod = provider.RelatedTableLoadPrefix + pkMethod;
                    }

                    bool methodsNoLazyLoad = !provider.GenerateRelatedTablesAsProperties && !provider.GenerateLazyLoads;
                    bool methodsLazyLoad = !provider.GenerateRelatedTablesAsProperties && provider.GenerateLazyLoads;
                    bool propertiesNoLazyLoad = provider.GenerateRelatedTablesAsProperties && !provider.GenerateLazyLoads;
                    bool propertiesLazyLoad = provider.GenerateRelatedTablesAsProperties && provider.GenerateLazyLoads;
                    string nullCheck = String.Empty;
                    
                    if (methodsLazyLoad || propertiesLazyLoad)
                    {
%>
                If Not col<%=pkMethod%> Is Nothing Then
                    For Each item As <%=pkClassQualified%> in col<%=pkMethod%>
<% if(pkColumn.IsNullable)
{
    nullCheck = String.Concat("item.", pkColumnName, " Is Nothing Or ");
}
%>
                        If <%=nullCheck%>item.<%=pkColumnName%> <> <%=thisPK%> Then
                            item.<%=pkColumnName%> = <%=thisPK%>
                        End If
                    Next
               End If
		<%
    }
}
}
}
        %>
        End Sub<%
}
%>

        #End Region
        
        #Region "Deep Save"
		
<%
    if (tbl.PrimaryKeyTables.Count > 0 && tbl.Provider.TableBaseClass == "ActiveRecord")
    {
        %>
        Public Sub DeepSave()
            Save()
            
<%
        TableSchema.PrimaryKeyTableCollection pkTables = tbl.PrimaryKeyTables;
        if (pkTables != null)
        {
            ArrayList usedMethodNames = new ArrayList();
            foreach (TableSchema.PrimaryKeyTable pk in pkTables)
            {
                TableSchema.Table pkTbl = DataService.GetSchema(pk.TableName, providerName, TableType.Table);
                if (pkTbl.PrimaryKey != null && CodeService.ShouldGenerate(pkTbl))
                {
                    string pkClass = pk.ClassName;
                    string pkClassQualified = provider.GeneratedNamespace + "." + pkClass;
                    string pkMethod = pk.ClassNamePlural;
                    string pkColumn = pkTbl.GetColumn(pk.ColumnName).PropertyName;

                    if (Utility.IsMatch(pkClass, pkMethod))
                    {
                        pkMethod += "Records";
                    }

                    if (pk.ClassName == className)
                    {
                        pkMethod = "Child" + pkMethod;
                    }

                    if (usedMethodNames.Contains(pkMethod))
                    {
                        pkMethod += "From" + className;
                        if (usedMethodNames.Contains(pkMethod))
                        {
                            pkMethod += pkColumn;
                        }
                    }

                    usedMethodNames.Add(pkMethod);

                    if (!String.IsNullOrEmpty(provider.RelatedTableLoadPrefix))
                    {
                        pkMethod = provider.RelatedTableLoadPrefix + pkMethod;
                    }

                    bool methodsNoLazyLoad = !provider.GenerateRelatedTablesAsProperties && !provider.GenerateLazyLoads;
                    bool methodsLazyLoad = !provider.GenerateRelatedTablesAsProperties && provider.GenerateLazyLoads;
                    bool propertiesNoLazyLoad = provider.GenerateRelatedTablesAsProperties && !provider.GenerateLazyLoads;
                    bool propertiesLazyLoad = provider.GenerateRelatedTablesAsProperties && provider.GenerateLazyLoads;

                    if (methodsLazyLoad || propertiesLazyLoad)
                    {
%>
                If Not col<%=pkMethod%> Is Nothing Then
                    col<%=pkMethod%>.SaveAll()
                End If
		<%
    }
}
}
}
        %>
        End Sub<%
}
%>

        #End Region
        
	End Class

End Namespace
<%
    }
    else
    {
%>
' The class <%= className %> was not generated because <%= tableName %> does not have a primary key.
<% } %>
