using OMS.Application.Payments.Options;
using OMS.Domain.Common;

namespace OMS.Application.Payments.Abstractions;

public interface IStubPayPalApiClient
{
    Task<Result<string>> ProcessPaymentAsync(PaypalPaymentOptions options);
}