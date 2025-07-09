using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OMS.Application.Identity.Abstractions;
using OMS.Application.Identity.Services;
using OMS.Application.MenuItems.Abstractions;
using OMS.Application.MenuItems.Services;
using OMS.Application.MenuItems.Validators;
using OMS.Application.Orders.Abstractions;
using OMS.Application.Orders.Services;
using OMS.Application.Payments.Abstractions;
using OMS.Application.Payments.Gateways;
using OMS.Application.Payments.Services;

namespace OMS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.RegisterServices();
        services.RegisterPaymentGateway();
        
        services.RegisterValidators();
        
        return services;
    }
    
    private static IServiceCollection RegisterValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<MenuItemOptionsValidator>();
        
        return services;
    }

    private static IServiceCollection RegisterServices(this IServiceCollection services)
    {
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        return services;
    }

    private static IServiceCollection RegisterPaymentGateway(this IServiceCollection services)
    {
        services
            .AddTransient<StripePaymentGateway>()
            .AddTransient<PaypalPaymentGateway>()
            .AddTransient<CashPaymentGateway>();

        services.AddSingleton<IPaymentGatewayFactory, PaymentGatewayFactory>();

        return services;
    }
}