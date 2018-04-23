#Region "Using"

Imports Microsoft.VisualBasic
Imports System
Imports System.Data
Imports System.Configuration
Imports System.Collections
Imports System.Web
Imports System.Web.Security
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports System.Web.UI.HtmlControls
Imports DevExpress.Web.ASPxGridView
Imports System.Web.SessionState
Imports System.ComponentModel
#End Region
Partial Public Class MasterDetailSetup
	Inherits System.Web.UI.Page
	Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
		grid.DataSource = DataProvider.GetRootItems()
		grid.DataBind()
	End Sub
	Private dataProvider_Renamed As SetupItemProvider
	Protected ReadOnly Property DataProvider() As SetupItemProvider
		Get
			If dataProvider_Renamed Is Nothing Then
				dataProvider_Renamed = New SetupItemProvider()
			End If
			Return dataProvider_Renamed
		End Get
	End Property
	Protected Sub detailGrid_DataSelect(ByVal sender As Object, ByVal e As EventArgs)
		Dim detailGrid As ASPxGridView = TryCast(sender, ASPxGridView)
		detailGrid.DataSource = DataProvider.GetItems(CInt(Fix(detailGrid.GetMasterRowKeyValue())))
	End Sub
	Protected Sub grid_CustomCallback(ByVal sender As Object, ByVal e As ASPxGridViewCustomCallbackEventArgs)
		Dim id As Integer = -1
		If Integer.TryParse(e.Parameters, id) Then
			ModifyItems(id)
			grid.DataSource = DataProvider.GetRootItems()
			grid.DataBind()
		End If
	End Sub
	Protected Sub grid_CustomDataCallback(ByVal sender As Object, ByVal e As ASPxGridViewCustomDataCallbackEventArgs)
		Dim id As Integer = -1
		If Integer.TryParse(e.Parameters, id) Then
			ModifyItems(id)
		End If
		e.Result = Nothing
	End Sub
	Protected Function GetMasterGridCallBack(ByVal visibleIndex As Integer, ByVal id As Integer) As String
		If grid.DetailRows.IsVisible(visibleIndex) Then
			Return String.Format("grid.PerformCallback({0})", id)
		End If
		Return GetGridDataCallBack(id)
	End Function
	Protected Function GetGridDataCallBack(ByVal id As Integer) As String
		Return String.Format("grid.GetValuesOnCustomCallback({0}, null)", id)
	End Function
	Protected Sub ModifyItems(ByVal id As Integer)
		Dim item As SetupItem = DataProvider.GetItemById(id)
		If item Is Nothing Then
		Return
		End If
		item.IsChecked = Not item.IsChecked
		For Each childItem As SetupItem In DataProvider.GetItems(id)
			childItem.IsChecked = item.IsChecked
		Next childItem
	End Sub
	#Region "DataSource"
	Private Shared SetupItemIdCounter As Integer = 0

	Public Class SetupItem
		Private id_Renamed As Integer
		Private name_Renamed As String
		Private isChecked_Renamed As Boolean
		Private parent_Renamed As SetupItem

		Public Sub New()
			Me.New(String.Empty, False)
		End Sub
		Public Sub New(ByVal name As String, ByVal isChecked As Boolean)
			Me.New(name, isChecked, Nothing)
		End Sub
		Public Sub New(ByVal name As String, ByVal isChecked As Boolean, ByVal parent As SetupItem)
			SetupItemIdCounter += 1
			Me.id_Renamed = SetupItemIdCounter
			Me.name_Renamed = name
			Me.isChecked_Renamed = isChecked
			Me.parent_Renamed = parent
		End Sub
		Public ReadOnly Property Id() As Integer
			Get
				Return id_Renamed
			End Get
		End Property
		Public Property Name() As String
			Get
				Return name_Renamed
			End Get
			Set(ByVal value As String)
				name_Renamed = value
			End Set
		End Property
		Public Property IsChecked() As Boolean
			Get
				Return isChecked_Renamed
			End Get
			Set(ByVal value As Boolean)
				isChecked_Renamed = value
			End Set
		End Property
		Public ReadOnly Property Parent() As SetupItem
			Get
				Return parent_Renamed
			End Get
		End Property
		Public ReadOnly Property ParentId() As Integer
			Get
				If Not Parent Is Nothing Then
					Return Parent.Id
				Else
					Return -1
				End If
			End Get
		End Property
		Public ReadOnly Property HtmlChecked() As String
			Get
				If IsChecked Then
					Return "checked"
				Else
					Return String.Empty
				End If
			End Get
		End Property
		Protected Friend Sub Assign(ByVal source As SetupItem)
			Name = source.Name
		End Sub
	End Class

	Public Class SetupItemProvider
		Private ReadOnly Property Session() As HttpSessionState
			Get
				Return HttpContext.Current.Session
			End Get
		End Property

		Public Function GetRootItems() As BindingList(Of SetupItem)
			Return GetItems(-1)
		End Function
		Public Function GetItems(ByVal parentId As Integer) As BindingList(Of SetupItem)
			Dim res As BindingList(Of SetupItem) = New BindingList(Of SetupItem)()
			For Each item As SetupItem In GetAllItems()
				If item.ParentId = parentId Then
					res.Add(item)
				End If
			Next item
			Return res
		End Function
		Public Function GetItemById(ByVal id As Integer) As SetupItem
			For Each item As SetupItem In GetAllItems()
				If item.Id = id Then
				Return item
				End If
			Next item
			Return Nothing
		End Function
		Protected Function GetAllItems() As BindingList(Of SetupItem)
			Dim res As BindingList(Of SetupItem) = TryCast(Session("SetupItems"), BindingList(Of SetupItem))
			If res Is Nothing Then
				res = CreateData()
				Session("SetupItems") = res
			End If
			Return res
		End Function

		Private Function CreateData() As BindingList(Of SetupItem)
			Dim res As BindingList(Of SetupItem) = New BindingList(Of SetupItem)()
			For i As Integer = 0 To 9
				Dim category As SetupItem = New SetupItem(String.Format("Category {0}", i + 1), False)
				res.Add(category)
				For j As Integer = 0 To 4
					res.Add(New SetupItem(String.Format("Item {0} - {1}", i + 1, j + 1), False, category))
				Next j
			Next i
			Return res
		End Function
		Public Sub Update(ByVal data As SetupItem)
			Dim item As SetupItem = GetItemById(data.Id)
			If Not item Is Nothing Then
				item.Assign(data)
			End If
		End Sub
	End Class
	#End Region
End Class
