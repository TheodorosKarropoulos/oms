using OMS.Application.Payments.Abstractions;
using OMS.Application.Payments.Dtos;
using OMS.Application.Payments.Options;
using OMS.Domain.Common;

namespace OMS.Application.Payments.Gateways;

internal sealed class CashPaymentGateway : IPaymentGateway
{
    public Task<Result<PaymentDto>> PayAsync(PaymentOptions options)
    {
        throw new NotImplementedException();
    }
}