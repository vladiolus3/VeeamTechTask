namespace VeeamTechTask
{
    internal interface ISynchronizer
    {
        void Synchronize(string source, string destination);

    }

    internal class Synchronizer : ISynchronizer
    {
        public void Synchronize(string source, string destination) 
        {
            if (Path.IsPathRooted(source))
                source = Path.GetFullPath(source);

            if (Path.IsPathRooted(destination))
                destination = Path.GetFullPath(destination);

            var sourceDirInfo = new DirectoryInfo(source)!;
            var destinationDirInfo = new DirectoryInfo(destination)!;

            var allSourceFilesDict = GetFilesWithRelativePaths(sourceDirInfo);
            var allDestinationFilesDict = GetFilesWithRelativePaths(destinationDirInfo);

            foreach (var sourceFile in allSourceFilesDict)
            {
                if (!allDestinationFilesDict.ContainsKey(sourceFile.Key)) 
                {
                    //add to dest folder
                    continue;
                }

                var replicaFile = allDestinationFilesDict[sourceFile.Key];

                if (replicaFile!.LastWriteTime != sourceFile.Value.LastWriteTime) 
                {
                    //update in dest folder

                }

                // add checking by reading files

            }

            var filesToRemove = allDestinationFilesDict.Keys.Where(k => !allSourceFilesDict.ContainsKey(k));

            foreach (var filePath in filesToRemove) 
            {
                //allDestinationFilesDict[filePath] file remove in dest folder

            }

        }

        private IReadOnlyDictionary<string, FileInfo> GetFilesWithRelativePaths(DirectoryInfo directory)
        {
            var files = directory.GetFiles("*", SearchOption.AllDirectories);
            var filesDict = new Dictionary<string, FileInfo>(); 

            foreach (var file in files)
            {
                var key = Path.GetRelativePath(directory.FullName, file.FullName);
                filesDict[key] = file;

            }

            return filesDict;
            
        }

    }

}
