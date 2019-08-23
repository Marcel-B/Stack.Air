using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using com.b_velop.stack.Air.Services;
using com.b_velop.App.IdentityProvider;
using com.b_velop.stack.Air.BL;
using Microsoft.Extensions.Configuration;
using GraphQL.Client;
using GraphQL.Common.Request;
using com.b_velop.stack.Air.Middlewares;

namespace com.b_velop.stack.Air
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;

        public IConfiguration Configuration { get; }

        public Startup(
            IConfiguration configuration,
            IHostingEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddSingleton<IUploadService, UploadService>();
            services.AddHttpClient<IIdentityProviderService, IdentityProviderService>();
            var url = string.Empty;
            if (_env.IsDevelopment())
            {
                services.Configure<ApiSecret>(Configuration.GetSection("ApiSecret-dev"));
                url = Configuration.GetSection("ApiSecret-dev").GetSection("GraphQLUrl").Value;
            }
            else
            {
                services.Configure<ApiSecret>(Configuration.GetSection("ApiSecret"));
                url = Configuration.GetSection("ApiSecret").GetSection("GraphQLUrl").Value;
            }
            services.AddSingleton(new GraphQLClient(url));
            services.AddSingleton<GraphQLRequest>();
            services.AddMemoryCache();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMetircCollector();
            }
            app.UseMvc();
        }
    }
}
