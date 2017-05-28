using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MRDC.Extenstions;
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
            Log.Logger.Information("{ConsumeDirectory} contains {FoundFilesNum} files", directory.FullName, files.Count);
            return files;
        }

        public void CreateFile([NotNull] FileInfo fileInfo) {
            Directory.CreateDirectory(fileInfo.DirectoryName);
            var fileStream = fileInfo.Create();
            fileStream.Close();
        }

        public static (bool Result, string Message, DirectoryInfo DirInfo) IsPathAccessable(string path) {
            if (path.IsNullOrEmpty())
                return (false, "Path is empty.", null);

            DirectoryInfo directoryInfo = null;
            try {
                var fullPath = Path.GetFullPath(path);
                var directoryName = Path.GetDirectoryName(fullPath);

                directoryInfo = new DirectoryInfo(directoryName);
                if (!directoryInfo.Exists) {
                    directoryInfo.Create();
                }

                var testFile = Path.Combine(directoryInfo.FullName, Path.GetTempFileName());
                var fileInfo = new FileInfo(testFile);
                fileInfo.Create();
                fileInfo.Delete();
            }

            catch (Exception e) {
                return (false, e.Message, null);
            }

            return (true, "", directoryInfo);
        }
    }
}
