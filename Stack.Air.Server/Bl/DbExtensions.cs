using com.b_velop.stack.DataContext.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace com.b_velop.Stack.Air.Server.Bl
{
    public static class DbExtensions
    {
        public static void UpdateDatabase(
         this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<MeasureContext>();
            context.Database.Migrate();
        }
    }
}
