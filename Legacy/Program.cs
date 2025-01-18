using System.Diagnostics;
using Hyzen.SDK.SecretManager;

namespace Legacy;

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
                if (!Debugger.IsAttached) // Don't use Sentry when debugging
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
        options.Dsn = HyzenSecret.GetSecret("HYZEN-AUTH-SENTRY-DSN");

        options.Debug = Debugger.IsAttached;
        options.AutoSessionTracking = true;
        options.StackTraceMode = StackTraceMode.Enhanced;
        options.IsGlobalModeEnabled = false;
        
        options.TracesSampleRate = 1.0;
        options.ProfilesSampleRate = 1.0;
        
        options.AddIntegration(new ProfilingIntegration(TimeSpan.FromMilliseconds(500)));
        
        options.CaptureFailedRequests = true;
        options.AttachStacktrace = true;
    }
}