using Sentry.Profiling;

namespace HyzenAuth.Core;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseSentry(ConfigureSentryOptions);
                webBuilder.UseStartup<Startup>();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                logging.AddConsole();
                logging.AddDebug();
                logging.AddEventSourceLogger();
            });
    }
    
    private static void ConfigureSentryOptions(SentryOptions options)
    {
        var appSettings = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<Program>()
            .Build();
        
        options.Dsn = Environment.GetEnvironmentVariable("SENTRY_DSN", EnvironmentVariableTarget.User) ?? appSettings["Sentry:DSN"];
        
        options.Debug = appSettings.GetValue<bool>("Sentry:Debug");
        options.TracesSampleRate = 1.0;
        options.ProfilesSampleRate = 1.0;
        options.AddIntegration(new ProfilingIntegration(TimeSpan.FromMilliseconds(500)));
        options.CaptureFailedRequests = true;
        options.ExperimentalMetrics = new ExperimentalMetricsOptions { EnableCodeLocations = true };
        options.AttachStacktrace = true;
    }
}