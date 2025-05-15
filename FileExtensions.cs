using System.Security.Cryptography;
using System.Collections;

namespace VeeamTechTask
{
    internal static class FileExtensions
    {
        public static bool IsFileReadable(this FileInfo file) 
        {
            try
            {
                using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                return true;

            }
            catch { return false; }

        }

        public static bool IsFileWritable(this FileInfo file)
        {
            try
            {
                using (var stream = file.Open(FileMode.Open, FileAccess.Write, FileShare.Read))
                    return true;

            }
            catch { return false; }

        }

        public static void Upsert(this FileInfo file, string destination)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            file.CopyTo(destination, overwrite: true);
        }

        public static bool TryToDelete(this FileInfo file) 
        {
            if (!file.IsFileWritable())
                return false;

            file.Delete();
            return true;

        }

        public static bool EqualsTo(this FileInfo file1, FileInfo file2)
        {
            using var md5 = MD5.Create();
            byte[] file1Hash;
            
            using (var stream = file1.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var buffer = new byte[1024 * 1024];

                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                }

                md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

                file1Hash = md5.Hash!;

            }

            byte[] file2Hash;

            using (var stream = file2.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var buffer = new byte[1024 * 1024];

                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    md5.TransformBlock(buffer, 0, bytesRead, null, 0);
                }

                md5.TransformFinalBlock(Array.Empty<byte>(), 0, 0);

                file2Hash = md5.Hash!;

            }

            return StructuralComparisons.StructuralEqualityComparer.Equals(file1Hash, file2Hash);

        }

    }

}
