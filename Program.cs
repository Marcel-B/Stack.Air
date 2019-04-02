using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Prometheus;

namespace com.b_velop.stack.Air
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var file = string.Empty;
            if (env == "Development")
                file = "nlog-dev.config";
            else
                file = "nlog.config";

            var logger = NLogBuilder.ConfigureNLog(file).GetCurrentClassLogger();
            try
            {
                if (env != "Development")
                {
                    var metricServer = new MetricPusher(
                        endpoint: "https://push.qaybe.de/metrics",
                        job: "stack_air");
                    metricServer.Start();
                }

                logger.Debug("init main");
                CreateWebHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                //NLog: catch setup errors
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
              .UseUrls("http://*:6000")
              .UseStartup<Startup>()
              .ConfigureLogging((hostingContext, logging) =>
              {
                  logging.ClearProviders();
                  logging.SetMinimumLevel(LogLevel.Trace);
              })
              .UseNLog();
    }
}
