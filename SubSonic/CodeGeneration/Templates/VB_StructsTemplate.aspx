<%@ Page Language="C#"%>
<%@ Import Namespace="SubSonic" %>
<%@ Import Namespace="System.Data" %>

<%foreach(DataProvider p in DataService.Providers){

      TableSchema.Table[] tables = DataService.GetTables(p.Name);
      TableSchema.Table[] views = DataService.GetViews(p.Name);

%>
Namespace <%=p.GeneratedNamespace%>
	#Region "Tables Struct"
	Public Partial Structure Tables
		Dim x As Integer
		<%
			foreach (TableSchema.Table t in tables)
			{
			    if(CodeService.ShouldGenerate(t.Name, p.Name))
			    {
%>
		Public Shared ReadOnly <%= t.ClassName %> As String = "<%= t.TableName %>"
        <%
			    }
			}
%>
	End Structure
	#End Region

	#Region "Schemas"
	Public Class Schema
		<%
			foreach (TableSchema.Table t in tables)
			{
			    if(CodeService.ShouldGenerate(t.Name, p.Name))
			    {
%>
		Public Shared ReadOnly Property <%=t.ClassName%> As TableSchema.Table
			Get
				Return DataService.GetSchema("<%=t.Name%>","<%=p.Name%>")
			End Get
		End Property
        <%
			    }
			}
%>
	End Class
	#End Region


    #Region "View Struct"
    Public Partial Structure Views
		Dim x As Integer
		<%
			foreach (TableSchema.Table v in views)
			{
				if (CodeService.ShouldGenerate(v.Name, p.Name))
				{
%>
		Public Shared ReadOnly <%= v.ClassName %> As String = "<%= v.TableName %>"
        <%
			}
		}
%>
    End Structure
    #End Region

	#Region "Query Factories"
	Public Partial Class DB

		Private Sub New()
		End Sub

		Public Shared _provider As DataProvider = DataService.Providers("<%=p.Name %>")
		Private Shared _repository As ISubSonicRepository

		Public Shared Property Repository() As ISubSonicRepository
			Get
				If _repository Is Nothing Then
					Return New SubSonicRepository(_provider)
				End If

				Return _repository
			End Get
			Set
				_repository = Value
			End Set
		End Property


		Public Shared Function SelectAllColumnsFrom(Of T As {RecordBase(Of T), New})() As [Select]
			Return Repository.SelectAllColumnsFrom(Of T)()
		End Function

		Public Shared Function [Select]() As [Select]
			Return Repository.Select()
		End Function

		Public Shared Function [Select](ParamArray ByVal columns As String()) As [Select]
			Return Repository.Select(columns)
		End Function

		Public Shared Function [Select](ParamArray ByVal aggregates As Aggregate()) As [Select]
			Return Repository.Select(aggregates)
		End Function

		Public Shared Function Update(Of T As {RecordBase(Of T), New})() As Update
			Return Repository.Update(Of T)()
		End Function

		Public Shared Function Insert() As Insert
			Return Repository.Insert()
		End Function

		Public Shared Function Delete() As Delete
			Return Repository.Delete()
		End Function

		Public Shared Function Query() As InlineQuery
			Return Repository.Query()
		End Function

        <%if (p.TableBaseClass=="RepositoryRecord"){%>
        
        #Region "Repository Compliance"
        
		Public Shared Function [Get](Of T As {RepositoryRecord(Of T), New})(ByVal primaryKeyValue As Object) As T
			Return Repository.Get(Of T)(primaryKeyValue)
		End Function

		Public Shared Function [Get](Of T As {RepositoryRecord(Of T), New})(ByVal columnName As String, ByVal columnValue As Object) As T
			Return Repository.Get(Of T)(columnName,columnValue)
		End Function

		Public Shared Sub Delete(Of T As {RepositoryRecord(Of T), New})(ByVal columnName As String, ByVal columnValue As Object)
			Repository.Delete(Of T)(columnName, columnValue)
		End Sub

		Public Shared Sub Delete(Of T As {RepositoryRecord(Of T), New})(ByVal item As RepositoryRecord(Of T))
			Repository.Delete(Of T)(item)
		End Sub

		Public Shared Sub DeleteByKey(Of T As {RepositoryRecord(Of T), New})(ByVal itemId As Object)
			Repository.DeleteByKey(Of T)(itemId)
		End Sub

		Public Shared Sub Destroy(Of T As {RepositoryRecord(Of T), New})(ByVal item As RepositoryRecord(Of T))
			Repository.Destroy(Of T)(item)
		End Sub

		Public Shared Sub Destroy(Of T As {RepositoryRecord(Of T), New})(ByVal columnName As String, ByVal value As Object)
			Repository.Destroy(Of T)(columnName,value)
		End Sub

		Public Shared Sub DestroyByKey(Of T As {RepositoryRecord(Of T), New})(ByVal itemId As Object)
			Repository.DestroyByKey(Of T)(itemId)
		End Sub

		Public Shared Function Save(Of T As {RepositoryRecord(Of T), New})(ByVal item As RepositoryRecord(Of T)) As Integer
			Return Repository.Save(Of T)(item)
		End Function

		Public Shared Function Save(Of T As {RepositoryRecord(Of T), New})(ByVal item As RepositoryRecord(Of T), ByVal userName As String) As Integer
			Return Repository.Save(Of T)(item,userName)
		End Function

		Public Shared Function SaveAll(Of ItemType As {RepositoryRecord(Of ItemType), New}, ListType As {RepositoryList(Of ItemType, ListType), New})(ByVal itemList As RepositoryList(Of ItemType, ListType)) As Integer
			Return Repository.SaveAll(Of ItemType, ListType)(itemList)
		End Function

		Public Shared Function SaveAll(Of ItemType As {RepositoryRecord(Of ItemType), New}, ListType As {RepositoryList(Of ItemType, ListType), New})(ByVal itemList As RepositoryList(Of ItemType, ListType), ByVal userName As String) As Integer
			Return Repository.SaveAll(Of ItemType, ListType)(itemList, userName)
		End Function

		Public Shared Function Update(Of T As {RepositoryRecord(Of T), New})(ByVal item As RepositoryRecord(Of T)) As Integer
			Return Repository.Update(Of T)(item, "")
		End Function

		Public Shared Function Update(Of T As {RepositoryRecord(Of T), New})(ByVal item As RepositoryRecord(Of T), ByVal userName As String) As Integer
			Return Repository.Update(Of T)(item, userName)
		End Function

		Public Shared Function Insert(Of T As {RepositoryRecord(Of T), New})(ByVal item As RepositoryRecord(Of T)) As Integer
			Return Repository.Insert(Of T)(item)
		End Function

		Public Shared Function Insert(Of T As {RepositoryRecord(Of T), New})(ByVal item As RepositoryRecord(Of T), ByVal userName As String) As Integer
			Return Repository.Insert(Of T)(item,userName)
		End Function

        #End Region
        <%}%>
	End Class
	#End Region

End Namespace
<%} %>
 


#Region "Databases"
Public Partial Structure Databases
	Dim x As Integer
	<%foreach (DataProvider p in DataService.Providers) { %>
	Public Shared ReadOnly <%= p.Name %> As String = "<%= p.Name%>"
    <%}%>
End Structure
#End Region
