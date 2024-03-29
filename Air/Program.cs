﻿using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
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
                file = "nlog.config";
            else
                file = "nlog.config";

            var logger = NLogBuilder.ConfigureNLog(file).GetCurrentClassLogger();
            try
            {
                logger.Debug("init main");
                if (env != "Development")
                {
                    var metricServer = new MetricPusher(
                        endpoint: "https://push.qaybe.de/metrics",
                        job: "Stack.Air");
                    metricServer.Start();
                }
                CreateHostBuilder(args)
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
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
          Host.CreateDefaultBuilder(args)
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 webBuilder.UseStartup<Startup>();
                 webBuilder.UseUrls("http://*:5063");
             })
              .ConfigureLogging((hostingContext, logging) =>
              {
                  logging.ClearProviders();
                  logging.SetMinimumLevel(LogLevel.Trace);
              })
              .UseNLog();
    }
}
