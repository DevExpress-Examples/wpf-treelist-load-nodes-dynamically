using DevExpress.Utils;
using DevExpress.Xpf.Grid;
using System.Windows;

namespace DynamicNodeLoading {
    public partial class MainWindow : Window {
        FileSystemDataProvider Helper { get; set; }
        public MainWindow() {
            InitializeComponent();
            Helper = new FileSystemHelper();
            InitDrives();
        }
        public void InitDrives() {
            grid.BeginDataUpdate();
            try {
                string[] root = Helper.GetLogicalDrives();

                foreach (string s in root) {
                    TreeListNode node = new TreeListNode() { Content = new FileSystemItem(s, "Drive", "<Drive>", s) };
                    view.Nodes.Add(node);
                    node.IsExpandButtonVisible = DefaultBoolean.True;
                }
            }
            catch { }
            grid.EndDataUpdate();
        }

        private void OnNodeExpanding(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeAllowEventArgs e) {
            TreeListNode node = e.Node;
            if (node.Tag == null || (bool)node.Tag == false) {
                InitFolder(node);
                node.Tag = true;
            }
        }

        private void InitFolder(TreeListNode treeListNode) {
            grid.BeginDataUpdate();
            InitFolders(treeListNode);
            InitFiles(treeListNode);
            grid.EndDataUpdate();
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

        private bool HasFiles(string path) {
            string[] root = Helper.GetFiles(path);
            if (root.Length > 0) return true;
            root = Helper.GetDirectories(path);
            if (root.Length > 0) return true;
            return false;
        }
    }
}
