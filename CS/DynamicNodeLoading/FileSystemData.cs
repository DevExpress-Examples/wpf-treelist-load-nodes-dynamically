using System;
using System.IO;
using System.Linq;

namespace DynamicNodeLoading {
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

    public abstract class FileSystemDataProvider {
        public abstract string[] GetLogicalDrives();
        public abstract string[] GetDirectories(string path);
        public abstract string[] GetFiles(string path);
        public abstract string GetDirectoryName(string path);
        public abstract string GetFileName(string path);
        public abstract string GetFileSize(string path);
        internal string GetFileSize(long size) {
            if (size >= 1024)
                return string.Format("{0:### ### ###} KB", size / 1024);
            return string.Format("{0} Bytes", size);
        }
    }

    public class FileSystemHelper : FileSystemDataProvider {
        public override string[] GetLogicalDrives() {
            return Directory.GetLogicalDrives();
        }

        public override string[] GetDirectories(string path) {
            try {
                return Directory.EnumerateDirectories(path).ToArray();
            }
            catch (UnauthorizedAccessException) {
                return new string[] { };
            }
        }

        public override string[] GetFiles(string path) {
            try {
                return Directory.EnumerateFiles(path).ToArray();
            }
            catch (UnauthorizedAccessException) {
                return new string[] { };
            }
        }

        public override string GetDirectoryName(string path) {
            return new DirectoryInfo(path).Name;
        }

        public override string GetFileName(string path) {
            return new FileInfo(path).Name;
        }

        public override string GetFileSize(string path) {
            long size = new FileInfo(path).Length;
            return GetFileSize(size);
        }
    }
}
