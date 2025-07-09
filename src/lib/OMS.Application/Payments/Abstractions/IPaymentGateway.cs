using OMS.Application.Payments.Dtos;
using OMS.Application.Payments.Options;
using OMS.Domain.Common;

namespace OMS.Application.Payments.Abstractions;

public interface IPaymentGateway
{
    Task<Result<PaymentDto>> PayAsync(PaymentOptions options);
}