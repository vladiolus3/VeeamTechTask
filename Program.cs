using VeeamTechTask;

var cts = new CancellationTokenSource();
var ct = cts.Token;

var synchronizer = new Synchronizer();
var synchronizerApp = new SynchronizerApp(synchronizer);

await synchronizerApp.RunAsync(args, ct);

AppDomain.CurrentDomain.ProcessExit += (sender, e) => cts.Cancel();