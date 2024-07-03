using Microsoft.EntityFrameworkCore;
using Newsletter.Api.Database;

namespace Newsletter.Api.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
}
