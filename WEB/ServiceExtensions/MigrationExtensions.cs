using DataAccess.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace WEB.ServiceExtensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            using BlogDbContext context = scope.ServiceProvider.GetRequiredService<BlogDbContext>();

            context.Database.Migrate();
        }
    }
}

