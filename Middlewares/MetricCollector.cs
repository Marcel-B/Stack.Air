using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Prometheus;

namespace com.b_velop.stack.Air.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class MetricCollector
    {
        private readonly RequestDelegate _next;
        public static Gauge RequestDuration = Metrics.CreateGauge(
            "b_velop_stack_air_requests_duration",
            "Request duration of all requests.",
            new GaugeConfiguration
            {
                LabelNames = new[] { "Path", "Method" }
            });

        public static Counter RequestCoutner = Metrics.CreateCounter(
            "b_velop_stack_air_requests_total",
            "Request counter of all requests",
            new CounterConfiguration
            {
                LabelNames = new[] { "Path", "Method" }
            });


        public MetricCollector(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            RequestCoutner.WithLabels(httpContext.Request.Path, httpContext.Request.Method).Inc();
            using (RequestDuration.WithLabels(httpContext.Request.Path, httpContext.Request.Method).NewTimer())
            {
                return _next(httpContext);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MetricCollectorExtensions
    {
        public static IApplicationBuilder UseMetircCollector(this IApplicationBuilder builder)
            => builder.UseMiddleware<MetricCollector>();
    }
}
