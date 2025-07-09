using Microsoft.Extensions.DependencyInjection;
using OMS.Application.Payments.Abstractions;
using OMS.Application.Payments.Services;
using OMS.Domain.Constants;

namespace OMS.Application.Payments.Gateways;

internal sealed class PaymentGatewayFactory : IPaymentGatewayFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PaymentGatewayFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public IPaymentGateway Resolve(PaymentProvider paymentProvider)
    {
        return paymentProvider switch
        {
            PaymentProvider.Stripe => _serviceProvider.GetRequiredService<StripePaymentGateway>(),
            PaymentProvider.PayPal => _serviceProvider.GetRequiredService<PaypalPaymentGateway>(),
            PaymentProvider.Cash => _serviceProvider.GetRequiredService<CashPaymentGateway>(),
            _ => throw new NotSupportedException($"Payment provider {paymentProvider} not supported")
        };
    }
}