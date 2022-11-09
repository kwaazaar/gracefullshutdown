
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SlowShutdownWorker;

class Program
{
    static async Task<int> Main(string[] args)
    {
        try
        {
            TimeSpan shutdownDuration = (args.Length == 1) ? TimeSpan.FromSeconds(int.Parse(args[0])) : TimeSpan.FromSeconds(60);
            
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                    {
                        services.Configure<HostOptions>(o => o.ShutdownTimeout = shutdownDuration.Add(TimeSpan.FromSeconds(1))); // Allow one extra second of total shutdown time

                        services.AddHostedService(sp =>
                            new BackgroundWorker(sp.GetRequiredService<ILogger<BackgroundWorker>>(),
                            shutdownDuration));
                    }
                )
                .RunConsoleAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("PROGRAM EXCEPTION: {0}", ex);
            return -1;
        }
    }
}