using System.IO;

namespace MRDC.Extenstions {
    public static class DirectoryInfoExtenstions {
        public static FileInfo GetTempFile(this DirectoryInfo di) {
            var fileName = Path.GetRandomFileName();
            return new FileInfo(Path.Combine(di.FullName, fileName));
        }
    }
}