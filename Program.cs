using VeeamTechTask;

var cts = new CancellationTokenSource();
var ct = cts.Token;

var synchronizerApp = new SynchronizerApp();

await synchronizerApp.RunAsync(args, ct);

AppDomain.CurrentDomain.ProcessExit += (sender, e) => cts.Cancel();