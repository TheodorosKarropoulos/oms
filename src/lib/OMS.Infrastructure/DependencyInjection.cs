using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OMS.Application.Common.Abstractions;
using OMS.Application.Identity.Abstractions;
using OMS.Application.Payments.Abstractions;
using OMS.Domain.Constants;
using OMS.Domain.Repositories;
using OMS.Infrastructure.Identity;
using OMS.Infrastructure.Integrations.Payments.Clients;
using OMS.Infrastructure.Interceptors;
using OMS.Infrastructure.Persistence;
using OMS.Infrastructure.Persistence.Repositories;

namespace OMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddScoped<IOrderRepository, OrderRepository>()
            .AddScoped<IMenuItemRepository, MenuItemRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>();

        services.RegisterDbContext(configuration);
        services.RegisterPaymentApiClients();

        services.AddScoped<ITokenProvider, TokenProvider>();
        
        return services;
    }

    private static IServiceCollection RegisterDbContext(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddScoped<AuditSaveChangesInterceptor>();
        
        services.AddDbContext<OmsDbContext>((sp, opt) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditSaveChangesInterceptor>();
            
            opt.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .AddInterceptors(auditInterceptor);
        });

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<OmsDbContext>());
        
        return services;
    }
    
    private static IServiceCollection RegisterPaymentApiClients(this IServiceCollection services)
    {
        services
            .AddHttpClient<IStubStripeApiClient, StubStripeClient>(nameof(PaymentProvider.Stripe))
            .ConfigureHttpClient(http =>
            {
                http.BaseAddress = new Uri("https://api.stripe.com/");
                http.DefaultRequestHeaders.Add("User-Agent", "OMS-Stub/1.0");
            });
        
        services
            .AddHttpClient<IStubPayPalApiClient, StubPayPalApiClient>(nameof(PaymentProvider.PayPal))
            .ConfigureHttpClient(http =>
            {
                http.BaseAddress = new Uri("https://api.paypal.com/");
                http.DefaultRequestHeaders.Add("User-Agent", "OMS-Stub/1.0");
            });
        
        return services;
    }
}