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
                var destinationFullPath = Path.Combine(destinationDirInfo.FullName, sourceFile.Key);

                if (!sourceFile.Value.IsFileReadable())
                    continue;

                var destinationFileExists = allDestinationFilesDict.TryGetValue(sourceFile.Key, out var replicaFile);

                var destinationFileUpdated = !destinationFileExists 
                    || replicaFile!.LastWriteTime != sourceFile.Value.LastWriteTime 
                    || !replicaFile.EqualsTo(sourceFile.Value);

                if (destinationFileUpdated) 
                {
                    sourceFile.Value.Upsert(destinationFullPath);

                }

            }

            var filesToRemove = allDestinationFilesDict.Keys.Where(k => !allSourceFilesDict.ContainsKey(k));

            foreach (var filePath in filesToRemove) 
            {
                var replicaFile = allDestinationFilesDict[filePath];

                if (!replicaFile.TryToDelete()) 
                {
                    //logging
                }

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
