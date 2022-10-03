Imports DevExpress.Utils
Imports DevExpress.Xpf.Grid
Imports System.Windows

Namespace DynamicNodeLoading

    Public Partial Class MainWindow
        Inherits Window

        Private Property Helper As FileSystemDataProvider

        Public Sub New()
            Me.InitializeComponent()
            Helper = New FileSystemHelper()
            InitDrives()
        End Sub

        Public Sub InitDrives()
            Me.grid.BeginDataUpdate()
            Try
                Dim root As String() = Helper.GetLogicalDrives()
                For Each s As String In root
                    Dim node As TreeListNode = New TreeListNode() With {.Content = New FileSystemItem(s, "Drive", "<Drive>", s)}
                    Me.view.Nodes.Add(node)
                    node.IsExpandButtonVisible = DefaultBoolean.True
                Next
            Catch
            End Try

            Me.grid.EndDataUpdate()
        End Sub

        Private Sub OnNodeExpanding(ByVal sender As Object, ByVal e As TreeList.TreeListNodeAllowEventArgs)
            Dim node As TreeListNode = e.Node
            If node.Tag Is Nothing OrElse CBool(node.Tag) = False Then
                InitFolder(node)
                node.Tag = True
            End If
        End Sub

        Private Sub InitFolder(ByVal treeListNode As TreeListNode)
            Me.grid.BeginDataUpdate()
            InitFolders(treeListNode)
            InitFiles(treeListNode)
            Me.grid.EndDataUpdate()
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

        Private Function HasFiles(ByVal path As String) As Boolean
            Dim root As String() = Helper.GetFiles(path)
            If root.Length > 0 Then Return True
            root = Helper.GetDirectories(path)
            If root.Length > 0 Then Return True
            Return False
        End Function
    End Class
End Namespace
