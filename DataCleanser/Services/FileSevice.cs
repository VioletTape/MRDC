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
            Log.Logger.Information("{ConsumeDirectory} contains {FoundFilesNum} files", directory.FullName, files.Count);
            return files;
        }

        public void CreateFile([NotNull] FileInfo fileInfo) {
            Directory.CreateDirectory(fileInfo.DirectoryName);
            var fileStream = fileInfo.Create();
            fileStream.Close();
        }
    }
}
