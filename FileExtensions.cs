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

        public static bool EqualsTo(this FileInfo file1, FileInfo file2)
        {
            return file1.Equals(file2); //todo: rework comparing

        }


    }

}
