using Serilog;

namespace VeeamTechTask
{
    internal interface ISynchronizer
    {
        void Synchronize(string source, string destination);

    }

    internal class Synchronizer(ILogger logger) : ISynchronizer
    {
        public void Synchronize(string source, string destination) 
        {
            if (Path.IsPathRooted(source))
                source = Path.GetFullPath(source);

            if (Path.IsPathRooted(destination))
                destination = Path.GetFullPath(destination);

            var sourceDirInfo = new DirectoryInfo(source)!;
            if (!sourceDirInfo.Exists)
            {
                sourceDirInfo.Create();
                logger.Debug("Directory {dir} created", sourceDirInfo);

            }

            var destinationDirInfo = new DirectoryInfo(destination)!;
            if (!destinationDirInfo.Exists)
            {
                destinationDirInfo.Create();
                logger.Debug("Directory {dir} created", destinationDirInfo);

            }

            var allSourceFilesDict = GetFilesWithRelativePaths(sourceDirInfo);
            var allDestinationFilesDict = GetFilesWithRelativePaths(destinationDirInfo);

            foreach (var sourceFile in allSourceFilesDict)
            {
                var destinationFullPath = Path.Combine(destinationDirInfo.FullName, sourceFile.Key);

                if (!sourceFile.Value.IsFileReadable())
                {
                    logger.Information("File {file} is not readable", sourceFile.Value.FullName);
                    continue;
                
                }

                var destinationFileExists = allDestinationFilesDict.TryGetValue(sourceFile.Key, out var replicaFile);

                var destinationFileUpdated = !destinationFileExists 
                    || replicaFile!.LastWriteTime != sourceFile.Value.LastWriteTime 
                    || !replicaFile.EqualsTo(sourceFile.Value);

                if (destinationFileUpdated) 
                {
                    if (!destinationFileExists)
                        logger.Debug("File {file} is created", destinationFullPath);
                    else 
                        logger.Debug("File {file} is updated", destinationFullPath);

                    sourceFile.Value.Upsert(destinationFullPath);

                }

            }

            var filesToRemove = allDestinationFilesDict.Keys.Where(k => !allSourceFilesDict.ContainsKey(k));

            foreach (var filePath in filesToRemove) 
            {
                var replicaFile = allDestinationFilesDict[filePath];

                if (!replicaFile.TryToDelete())
                    logger.Debug("File {file} is not unable for deleting", replicaFile.FullName);
                else
                    logger.Debug("File {file} is deleted", replicaFile.FullName);

            }

        }

        private static Dictionary<string, FileInfo> GetFilesWithRelativePaths(DirectoryInfo directory)
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
