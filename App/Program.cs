using Serilog;
using VeeamTechTask.App;

AppConfig.Init(args);

const string outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] {Level:u3} {Message:lj}{NewLine}{Exception}";

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console(outputTemplate: outputTemplate)
    .WriteTo.File(path: AppConfig.LogPath, outputTemplate: outputTemplate)
    .CreateLogger();

var synchronizer = new Synchronizer(logger.ForContext<ISynchronizer>());
var synchronizerApp = new SynchronizerApp(synchronizer, logger.ForContext<SynchronizerApp>());

var cts = new CancellationTokenSource();
var ct = cts.Token;

await synchronizerApp.RunAsync(ct);

AppDomain.CurrentDomain.ProcessExit += (sender, e) => cts.Cancel();