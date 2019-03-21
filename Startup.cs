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
        public IConfiguration Configuration { get; }

        public Startup(
            IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddSingleton<IUploadService, UploadService>();
            services.AddHttpClient<IIdentityProviderService, IdentityProviderService>();
            services.Configure<ApiSecret>(Configuration.GetSection("ApiSecret"));
            var url = Configuration.GetSection("ApiSecret").GetSection("GraphQLUrl").Value;
            services.AddSingleton(new GraphQLClient(url));
            services.AddSingleton<GraphQLRequest>();
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
            app.UseMetircCollector();
            app.UseMvc();
        }
    }
}
