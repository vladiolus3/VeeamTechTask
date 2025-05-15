namespace VeeamTechTask
{
    internal static class AppConfig
    {
        public static string? Source { get; private set; }
        public static string? Destination { get; private set; }
        public static string LogPath { get; private set; } 
        public static int? Interval { get; private set; }

        static AppConfig()
        {
            LogPath = Path.Combine(Directory.GetCurrentDirectory(), "logs.txt");

        }

        public static void Init(IReadOnlyList<string> args) 
        {
            ValidateArgs(args);

        }

        private static void ValidateArgs(IReadOnlyList<string> args)
        {
            static bool IsValidPath(string path)
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

            if (args == null || (args.Count != 3 && args.Count != 4))
                throw new ArgumentException("Expected at least 3 arguments: <source> <destination> <intervalSeconds>. " +
                    "<logPath> is optional.");

            foreach (var arg in args)
            {
                var splitedArg = arg.Split("=");

                if (splitedArg.Length != 2)
                    throw new ArgumentException($"Incorrect argument: {arg}. Must be as key=value");

                var (key, value) = (splitedArg[0].ToLower(), splitedArg[1]);

                switch (key)
                {
                    case "source":
                    case "s":
                        if (!IsValidPath(value))
                            throw new ArgumentException($"Source path is invalid or does not exist: {value}");

                        Source = value;
                        break;

                    case "destination":
                    case "d":
                        if (!IsValidPath(value))
                            throw new ArgumentException($"Destination path is invalid or does not exist: {value}");

                        Destination = value;
                        break;

                    case "logpath":
                    case "l":
                        if (!IsValidPath(Directory.GetParent(value)!.FullName))
                            throw new ArgumentException($"Log path is invalid or does not exist: {value}");

                        LogPath = value;
                        break;

                    case "interval":
                    case "i":
                        if (!int.TryParse(value, out var parsedValue) || parsedValue <= 0)
                            throw new ArgumentException($"Interval must be a positive integer. Provided: {value}");

                        Interval = parsedValue;
                        break;

                }

            }

            if (Source == null) throw new ArgumentException("Source can not be null");
            if (Destination == null) throw new ArgumentException("Destination can not be null");
            if (Interval == null) throw new ArgumentException("Internal can not be null");

        }

    }
}
