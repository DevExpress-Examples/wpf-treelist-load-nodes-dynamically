using System;
using System.Windows;
using DevExpress.Xpf.Grid;
using System.IO;
using DevExpress.Utils;

namespace DynamicNodeLoading {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Helper = new FileSystemHelper();
            InitDrives();
        }

        private void treeListView1_NodeExpanding(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeAllowEventArgs e) {
            TreeListNode node = e.Node;
            if (node.Tag == null || (bool)node.Tag == false) {
                InitFolder(node);
                node.Tag = true;
            }
        }

        FileSystemDataProvider Helper { get; set; }

        public void InitDrives() {
            grid.BeginDataUpdate();
            try {
                string[] root = Helper.GetLogicalDrives();

                foreach (string s in root) {
                    TreeListNode node = new TreeListNode() { Content = new FileSystemItem(s, "Drive", "<Drive>", s) };
                    treeListView1.Nodes.Add(node);
                    node.IsExpandButtonVisible = DefaultBoolean.True;
                }
            }
            catch { }
            grid.EndDataUpdate();
        }
        private void InitFolder(TreeListNode treeListNode) {
            grid.BeginDataUpdate();
            InitFolders(treeListNode);
            InitFiles(treeListNode);
            grid.EndDataUpdate();
        }

        private void InitFiles(TreeListNode treeListNode) {
            FileSystemItem item = treeListNode.Content as FileSystemItem;
            if (item == null) return;
            TreeListNode node;
            try {
                string[] root = Helper.GetFiles(item.FullName);
                foreach (string s in root) {
                    node = new TreeListNode() { Content = new FileSystemItem(Helper.GetFileName(s), "File", Helper.GetFileSize(s).ToString(), s) };
                    node.IsExpandButtonVisible = DefaultBoolean.False;
                    treeListNode.Nodes.Add(node);
                }
            }
            catch { }
        }

        private void InitFolders(TreeListNode treeListNode) {
            FileSystemItem item = treeListNode.Content as FileSystemItem;
            if (item == null) return;

            try {
                string[] root = Helper.GetDirectories(item.FullName);
                foreach (string s in root) {
                    try {
                        TreeListNode node = new TreeListNode() { Content = new FileSystemItem(Helper.GetDirectoryName(s), "Folder", "<Folder>", s) };
                        treeListNode.Nodes.Add(node);

                        node.IsExpandButtonVisible = HasFiles(s) ? DefaultBoolean.True : DefaultBoolean.False;
                    }
                    catch { }
                }
            }
            catch { }
        }

        private bool HasFiles(string path) {
            string[] root = Helper.GetFiles(path);
            if (root.Length > 0) return true;
            root = Helper.GetDirectories(path);
            if (root.Length > 0) return true;
            return false;
        }
        public abstract class FileSystemDataProvider {
            public abstract string[] GetLogicalDrives();
            public abstract string[] GetDirectories(string path);
            public abstract string[] GetFiles(string path);
            public abstract string GetDirectoryName(string path);
            public abstract string GetFileName(string path);
            public abstract long GetFileSize(string path);
        }
        public class FileSystemHelper : FileSystemDataProvider {

            public override string[] GetLogicalDrives() {
                return Directory.GetLogicalDrives();
            }

            public override string[] GetDirectories(string path) {
                return Directory.GetDirectories(path);
            }

            public override string[] GetFiles(string path) {
                return Directory.GetFiles(path);
            }

            public override string GetDirectoryName(string path) {
                return new DirectoryInfo(path).Name;
            }

            public override string GetFileName(string path) {
                return new FileInfo(path).Name;
            }

            public override long GetFileSize(string path) {
                return new FileInfo(path).Length;
            }
        }
    }

    public class FileSystemItem {
        public FileSystemItem(string name, string type, string size, string fullName) {
            Name = name;
            ItemType = type;
            Size = size;
            FullName = fullName;
        }
        public string Name { get; set; }
        public string ItemType { get; set; }
        public string Size { get; set; }
        public string FullName { get; set; }
    }
}
