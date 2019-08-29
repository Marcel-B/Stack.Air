using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using com.b_velop.stack.Air.Services;
using com.b_velop.App.IdentityProvider;
using com.b_velop.stack.Air.BL;
using Microsoft.Extensions.Configuration;
using GraphQL.Client;
using GraphQL.Common.Request;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc;

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

            services.AddMvc(
                options => options.EnableEndpointRouting = false
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Air API", Version = "v1" });
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Air API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
