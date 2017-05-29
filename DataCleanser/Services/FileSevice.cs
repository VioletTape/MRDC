using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Serilog;

namespace MRDC.Services {
    internal class FileSevice {
        public List<FileInfo> GetFiles([NotNull] DirectoryInfo directory) {
            if (!directory.Exists) {
                Log.Warning("{SourceDir} does not exists.", directory.FullName);
                return new List<FileInfo>();
            }

            var files = Directory.GetFiles(directory.FullName)
                                 .Select(i => new FileInfo(i))
                                 .ToList();
            return files;
        }

        public void CreateFile([NotNull] FileInfo fileInfo) {
            Directory.CreateDirectory(fileInfo.DirectoryName);
            var fileStream = fileInfo.Create();
            fileStream.Close();
        }

        public DirectoryInfo CreateTempDir(string dirName) {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var combine = Path.Combine(baseDir, "Temp", dirName);
            var directoryInfo = new DirectoryInfo(combine);
            directoryInfo.Create();
            return directoryInfo;
        }

        public FileSevice CleanUpTempDir() {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var combine = Path.Combine(baseDir, "Temp");
            if(Directory.Exists(combine))
                Directory.Delete(combine, true);
            return this;
        }


        public DirectoryInfo GetTempDir() {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var combine = Path.Combine(baseDir, "Temp");
            var directoryInfo = new DirectoryInfo(combine);
            if (!directoryInfo.Exists) {
                directoryInfo.Create();
            }
            return directoryInfo;
        }

        public List<DirectoryInfo> GetSortedTempFolders() {
            var tempDir = GetTempDir();
            var folders = tempDir.GetDirectories().ToList();
            folders.Sort((dix, diy) => string.Compare(dix.Name, diy.Name, StringComparison.Ordinal));
            return folders;
        }
    }
}
