using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using OMS.Infrastructure.Persistence;

namespace OMS.Api.Tests.Integration.Common;

public class OmsDefaultFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly TestDbContainer _dbContainer = new MsSqlDbContainer();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((content, services) =>
        {
            services.RemoveAll<IHostedService>();
        });
        
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services
                .SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<OmsDbContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<OmsDbContext>(options =>
            {
                options.UseSqlServer(_dbContainer.ConnectionString);
            });
            
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = "TestScheme";
                    options.DefaultChallengeScheme = "TestScheme";
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { });
        });
    }
    
    public IServiceScope CreateScope()
        => Services.CreateScope();

    public async ValueTask InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }

    public new async ValueTask DisposeAsync()
    {
        await _dbContainer.DisposeAsync();

        await base.DisposeAsync();
    }
}