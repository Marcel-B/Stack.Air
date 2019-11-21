using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using com.b_velop.stack.Air.Services;
using com.b_velop.stack.Air.BL;
using Microsoft.Extensions.Configuration;
using GraphQL.Client;
using GraphQL.Common.Request;
using Swashbuckle.AspNetCore.Swagger;
using com.b_velop.IdentityProvider;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace com.b_velop.stack.Air
{
    public class Startup
    {
        private readonly IWebHostEnvironment _env;

        public IConfiguration Configuration { get; }


        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _env = env;
            Configuration = configuration;
        }

        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddScoped<IUploadService, UploadService>();
            services.AddHttpClient<IIdentityProviderService, IdentityProviderService>();

            var clientId = System.Environment.GetEnvironmentVariable("ClientId");
            var scope = System.Environment.GetEnvironmentVariable("Scope");
            var secret = System.Environment.GetEnvironmentVariable("Secret");
            var issuer = System.Environment.GetEnvironmentVariable("Issuer");
            var url = System.Environment.GetEnvironmentVariable("GraphQLUrl") ?? "https://data.qaybe.de/graphql";
            services.AddScoped(_ => new ApiSecret
            {
                AuthorityUrl = issuer,
                ClientId = clientId,
                ClientSecret = secret,
                GraphQLUrl = url,
                Scope = scope
            });

            services.AddSingleton(new GraphQLClient(url));
            services.AddSingleton<GraphQLRequest>();
            services.AddMemoryCache();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Air API", Version = "v1" });
            });
            services.AddControllers();
            services.AddMvc(
                options => options.EnableEndpointRouting = false
            );

            
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Air API v1");
            });
            app.UseMvc();
        }
    }
}
