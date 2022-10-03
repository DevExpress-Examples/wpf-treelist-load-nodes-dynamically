Imports System.Windows
Imports DevExpress.Xpf.Grid
Imports System.IO
Imports DevExpress.Utils

Namespace DynamicNodeLoading

    ''' <summary>
    ''' Interaction logic for MainWindow.xaml
    ''' </summary>
    Public Partial Class MainWindow
        Inherits Window

        Public Sub New()
            Me.InitializeComponent()
            Helper = New FileSystemHelper()
            InitDrives()
        End Sub

        Private Sub treeListView1_NodeExpanding(ByVal sender As Object, ByVal e As TreeList.TreeListNodeAllowEventArgs)
            Dim node As TreeListNode = e.Node
            If node.Tag Is Nothing OrElse CBool(node.Tag) = False Then
                InitFolder(node)
                node.Tag = True
            End If
        End Sub

        Private Property Helper As FileSystemDataProvider

        Public Sub InitDrives()
            Me.grid.BeginDataUpdate()
            Try
                Dim root As String() = Helper.GetLogicalDrives()
                For Each s As String In root
                    Dim node As TreeListNode = New TreeListNode() With {.Content = New FileSystemItem(s, "Drive", "<Drive>", s)}
                    Me.treeListView1.Nodes.Add(node)
                    node.IsExpandButtonVisible = DefaultBoolean.True
                Next
            Catch
            End Try

            Me.grid.EndDataUpdate()
        End Sub

        Private Sub InitFolder(ByVal treeListNode As TreeListNode)
            Me.grid.BeginDataUpdate()
            InitFolders(treeListNode)
            InitFiles(treeListNode)
            Me.grid.EndDataUpdate()
        End Sub

        Private Sub InitFiles(ByVal treeListNode As TreeListNode)
            Dim item As FileSystemItem = TryCast(treeListNode.Content, FileSystemItem)
            If item Is Nothing Then Return
            Dim node As TreeListNode
            Try
                Dim root As String() = Helper.GetFiles(item.FullName)
                For Each s As String In root
                    node = New TreeListNode() With {.Content = New FileSystemItem(Helper.GetFileName(s), "File", Helper.GetFileSize(s).ToString(), s)}
                    node.IsExpandButtonVisible = DefaultBoolean.False
                    treeListNode.Nodes.Add(node)
                Next
            Catch
            End Try
        End Sub

        Private Sub InitFolders(ByVal treeListNode As TreeListNode)
            Dim item As FileSystemItem = TryCast(treeListNode.Content, FileSystemItem)
            If item Is Nothing Then Return
            Try
                Dim root As String() = Helper.GetDirectories(item.FullName)
                For Each s As String In root
                    Try
                        Dim node As TreeListNode = New TreeListNode() With {.Content = New FileSystemItem(Helper.GetDirectoryName(s), "Folder", "<Folder>", s)}
                        treeListNode.Nodes.Add(node)
                        node.IsExpandButtonVisible = If(HasFiles(s), DefaultBoolean.True, DefaultBoolean.False)
                    Catch
                    End Try
                Next
            Catch
            End Try
        End Sub

        Private Function HasFiles(ByVal path As String) As Boolean
            Dim root As String() = Helper.GetFiles(path)
            If root.Length > 0 Then Return True
            root = Helper.GetDirectories(path)
            If root.Length > 0 Then Return True
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

        Public Sub New(ByVal name As String, ByVal type As String, ByVal size As String, ByVal fullName As String)
            Me.Name = name
            ItemType = type
            Me.Size = size
            Me.FullName = fullName
        End Sub

        Public Property Name As String

        Public Property ItemType As String

        Public Property Size As String

        Public Property FullName As String
    End Class
End Namespace
