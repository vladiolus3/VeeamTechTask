using System.Security.Cryptography;
using System.Collections;

namespace VeeamTechTask.App
{
    internal static class FileExtensions
    {
        public static bool IsFileReadable(this FileInfo fileInfo) => fileInfo.CheckFileAccess(FileAccess.Read);

        public static bool IsFileWritable(this FileInfo fileInfo) => fileInfo.CheckFileAccess(FileAccess.Write); 

        public static bool CheckFileAccess(this FileInfo fileInfo, FileAccess fileAccess) 
        {
            try
            {
                using var stream = fileInfo.Open(FileMode.Open, fileAccess, FileShare.Read);
                return true;

            }
            catch { return false; }

        }

        public static void Upsert(this FileInfo fileInfo, string destination)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            fileInfo.CopyTo(destination, overwrite: true);

        }

        public static bool TryToDelete(this FileSystemInfo fileSystemInfo) 
        {
            try
            {
                if (fileSystemInfo is DirectoryInfo dirInfo)
                {
                    dirInfo.Delete(true);

                }
                else if (fileSystemInfo is FileInfo fileInfo)
                {
                    fileInfo.Delete();

                }

                return true;
            }
            catch { return false; }

        }

        public static bool EqualsTo(this FileInfo fileInfo1, FileInfo fileInfo2)
        {
            using var md5 = MD5.Create();
            byte[] file1Hash = GetFileHash(md5, fileInfo1);       
            byte[] file2Hash = GetFileHash(md5, fileInfo2);

            return StructuralComparisons.StructuralEqualityComparer.Equals(file1Hash, file2Hash);

        }

        private static byte[] GetFileHash(MD5 md5, FileInfo fileInfo) 
        {
            using var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            var buffer = new byte[1024 * 1024];

            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                md5.TransformBlock(buffer, 0, bytesRead, null, 0);
            }

            md5.TransformFinalBlock([], 0, 0);

            return md5.Hash!;
        }

    }

}
