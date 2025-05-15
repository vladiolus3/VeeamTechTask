using Serilog;

namespace VeeamTechTask
{
    sealed internal class SynchronizerApp(ISynchronizer synchronizer, ILogger logger)
    {
        public async Task RunAsync(CancellationToken cancellationToken) 
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    synchronizer.Synchronize(AppConfig.Source!, AppConfig.Destination!);
                    logger.Information("Synchronization process has been completed.");
                    await Task.Delay(TimeSpan.FromSeconds(AppConfig.Interval!.Value), cancellationToken);

                }

            }
            catch (Exception exception)
            {
                logger.Error("An error occurred during synchronization: {error}", exception.Message);

            }
            finally
            {
                logger.Information("Synchronization process has been stopped.");

            }

        }

    }

}
