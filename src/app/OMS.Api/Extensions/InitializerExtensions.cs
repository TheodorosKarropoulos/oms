using Microsoft.EntityFrameworkCore;
using OMS.Api.Seeders;
using OMS.Infrastructure.Persistence;

namespace OMS.Api.Extensions;

internal static class InitializerExtensions
{
    internal static async Task SeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<OmsDbContext>();

        await dbContext.Database.MigrateAsync();

        await DefaultSeeder.SeedAsync(scope.ServiceProvider);
    }
}