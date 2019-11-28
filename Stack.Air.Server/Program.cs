using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using com.b_velop.stack.DataContext.Abstract;
using com.b_velop.Stack.Air.Server.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;
using Prometheus;

namespace Stack.Air.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            const string file = "nlog.config";
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
                    webBuilder.UseUrls("http://*:5000");
                })
                .ConfigureServices(services =>
                {
                    var server = Environment.GetEnvironmentVariable("SERVER");
                    var password = Environment.GetEnvironmentVariable("PASSWORD");
                    var dbName = Environment.GetEnvironmentVariable("DATABASE");
                    var user = Environment.GetEnvironmentVariable("USER");

                    var connectionString = $"Server={server},1433;Database={dbName};User Id={user};Password={password}";
#if DEBUG
                    connectionString = "Server=localhost,1433;Database=Measure;User Id=sa;Password=foo123bar!";
#endif

                    services.AddDbContext<MeasureContext>(options =>
                    {
                        options.EnableDetailedErrors(true);
                        options.EnableSensitiveDataLogging(true);
                        options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Stack.Air.Server"));
                    });
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                })
              .UseNLog();
    }
}
