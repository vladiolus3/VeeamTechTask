namespace VeeamTechTask
{
    sealed internal class SynchronizerApp
    {
        private ISynchronizer synchronizer = new Synchronizer();
        private string? source;
        private string? destination;
        private int? interval;

        public async Task RunAsync(IReadOnlyList<string> args, CancellationToken cancellationToken) 
        {
            try
            {
                ValidateArgs(args);

                while (!cancellationToken.IsCancellationRequested)
                {
                    synchronizer.Synchronize(source!, destination!);
                    await Task.Delay(interval!.Value, cancellationToken);

                }

            }
            catch { }

        }

        private void ValidateArgs(IReadOnlyList<string> args)
        {
            bool IsValidPath(string path)
            {
                try
                {
                    var fullPath = Path.GetFullPath(path);
                    return Directory.Exists(fullPath);

                }
                catch
                {
                    return false;

                }

            }

            if (args == null || args.Count != 3)
                throw new ArgumentException("Expected 3 arguments: <source> <destination> <intervalSeconds>");

            foreach (var arg in args)
            {
                var splitedArg = arg.Split("=");

                if (splitedArg.Length != 2)
                    throw new ArgumentException($"Incorrect argument: {arg}. Must be as key=value");

                var (key, value) = (splitedArg[0], splitedArg[1]);

                switch (key)
                {
                    case "source":
                        if (!IsValidPath(value))
                            throw new ArgumentException($"Source path is invalid or does not exist: {value}");

                        source = value;
                        break;

                    case "destination":
                        if (!IsValidPath(value))
                            throw new ArgumentException($"Destination path is invalid or does not exist: {value}");

                        destination = value;
                        break;

                    case "interval":
                        if (!int.TryParse(value, out var parsedValue) || parsedValue <= 0)
                            throw new ArgumentException($"Interval must be a positive integer. Provided: {value}");

                        interval = parsedValue;
                        break;

                }

            }

            if (source == null) throw new ArgumentException("Source can not be null");
            if (destination == null) throw new ArgumentException("Destination can not be null");
            if (interval == null) throw new ArgumentException("Internal can not be null");

        }


    }

}
