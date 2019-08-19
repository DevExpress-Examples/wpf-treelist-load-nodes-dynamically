Imports Microsoft.VisualBasic
Imports System
Imports System.Windows
Imports DevExpress.Xpf.Grid
Imports System.IO
Imports DevExpress.Utils

Namespace DynamicNodeLoading
	''' <summary>
	''' Interaction logic for MainWindow.xaml
	''' </summary>
	Partial Public Class MainWindow
		Inherits Window
		Public Sub New()
			InitializeComponent()
			Helper = New FileSystemHelper()
			InitDrives()
		End Sub

		Private Sub treeListView1_NodeExpanding(ByVal sender As Object, ByVal e As DevExpress.Xpf.Grid.TreeList.TreeListNodeAllowEventArgs)
			Dim node As TreeListNode = e.Node
			If node.Tag Is Nothing OrElse CBool(node.Tag) = False Then
				InitFolder(node)
				node.Tag = True
			End If
		End Sub

		Private privateHelper As FileSystemDataProvider
		Private Property Helper() As FileSystemDataProvider
			Get
				Return privateHelper
			End Get
			Set(ByVal value As FileSystemDataProvider)
				privateHelper = value
			End Set
		End Property

		Public Sub InitDrives()
			grid.BeginDataUpdate()
			Try
				Dim root() As String = Helper.GetLogicalDrives()

				For Each s As String In root
					Dim node As New TreeListNode() With {.Content = New FileSystemItem(s, "Drive", "<Drive>", s)}
					treeListView1.Nodes.Add(node)
					node.IsExpandButtonVisible = DefaultBoolean.True
				Next s
			Catch
			End Try
			grid.EndDataUpdate()
		End Sub
		Private Sub InitFolder(ByVal treeListNode As TreeListNode)
			grid.BeginDataUpdate()
			InitFolders(treeListNode)
			InitFiles(treeListNode)
			grid.EndDataUpdate()
		End Sub

		Private Sub InitFiles(ByVal treeListNode As TreeListNode)
			Dim item As FileSystemItem = TryCast(treeListNode.Content, FileSystemItem)
			If item Is Nothing Then
				Return
			End If
			Dim node As TreeListNode
			Try
				Dim root() As String = Helper.GetFiles(item.FullName)
				For Each s As String In root
					node = New TreeListNode() With {.Content = New FileSystemItem(Helper.GetFileName(s), "File", Helper.GetFileSize(s).ToString(), s)}
					node.IsExpandButtonVisible = DefaultBoolean.False
					treeListNode.Nodes.Add(node)
				Next s
			Catch
			End Try
		End Sub

		Private Sub InitFolders(ByVal treeListNode As TreeListNode)
			Dim item As FileSystemItem = TryCast(treeListNode.Content, FileSystemItem)
			If item Is Nothing Then
				Return
			End If

			Try
				Dim root() As String = Helper.GetDirectories(item.FullName)
				For Each s As String In root
					Try
						Dim node As New TreeListNode() With {.Content = New FileSystemItem(Helper.GetDirectoryName(s), "Folder", "<Folder>", s)}
						treeListNode.Nodes.Add(node)

						node.IsExpandButtonVisible = If(HasFiles(s), DefaultBoolean.True, DefaultBoolean.False)
					Catch
					End Try
				Next s
			Catch
			End Try
		End Sub

		Private Function HasFiles(ByVal path As String) As Boolean
			Dim root() As String = Helper.GetFiles(path)
			If root.Length > 0 Then
				Return True
			End If
			root = Helper.GetDirectories(path)
			If root.Length > 0 Then
				Return True
			End If
			Return False
		End Function
		Public MustInherit Class FileSystemDataProvider
			Public MustOverride Function GetLogicalDrives() As String()
			Public MustOverride Function GetDirectories(ByVal path As String) As String()
			Public MustOverride Function GetFiles(ByVal path As String) As String()
			Public MustOverride Function GetDirectoryName(ByVal path As String) As String
			Public MustOverride Function GetFileName(ByVal path As String) As String
			Public MustOverride Function GetFileSize(ByVal path As String) As Long
		End Class
		Public Class FileSystemHelper
			Inherits FileSystemDataProvider

			Public Overrides Function GetLogicalDrives() As String()
				Return Directory.GetLogicalDrives()
			End Function

			Public Overrides Function GetDirectories(ByVal path As String) As String()
				Return Directory.GetDirectories(path)
			End Function

			Public Overrides Function GetFiles(ByVal path As String) As String()
				Return Directory.GetFiles(path)
			End Function

			Public Overrides Function GetDirectoryName(ByVal path As String) As String
				Return New DirectoryInfo(path).Name
			End Function

			Public Overrides Function GetFileName(ByVal path As String) As String
				Return New FileInfo(path).Name
			End Function

			Public Overrides Function GetFileSize(ByVal path As String) As Long
				Return New FileInfo(path).Length
			End Function
		End Class
	End Class

	Public Class FileSystemItem
		Public Sub New(ByVal item_name As String, ByVal item_type As String, ByVal item_size As String, ByVal item_fullName As String)
			Name = item_name
			ItemType = item_type
			Size = item_sizesize
			FullName = item_fullName
		End Sub
		Private privateName As String
		Public Property Name() As String
			Get
				Return privateName
			End Get
			Set(ByVal value As String)
				privateName = value
			End Set
		End Property
		Private privateItemType As String
		Public Property ItemType() As String
			Get
				Return privateItemType
			End Get
			Set(ByVal value As String)
				privateItemType = value
			End Set
		End Property
		Private privateSize As String
		Public Property Size() As String
			Get
				Return privateSize
			End Get
			Set(ByVal value As String)
				privateSize = value
			End Set
		End Property
		Private privateFullName As String
		Public Property FullName() As String
			Get
				Return privateFullName
			End Get
			Set(ByVal value As String)
				privateFullName = value
			End Set
		End Property
	End Class
End Namespace
