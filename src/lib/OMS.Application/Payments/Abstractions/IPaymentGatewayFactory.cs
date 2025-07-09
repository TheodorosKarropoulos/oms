using OMS.Domain.Constants;

namespace OMS.Application.Payments.Abstractions;

public interface IPaymentGatewayFactory
{
    IPaymentGateway Resolve(PaymentProvider paymentProvider);
}