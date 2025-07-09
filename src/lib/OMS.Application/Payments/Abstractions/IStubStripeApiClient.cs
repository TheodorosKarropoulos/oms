using OMS.Application.Payments.Options;
using OMS.Domain.Common;

namespace OMS.Application.Payments.Abstractions;

public interface IStubStripeApiClient
{
    Task<Result<string>> ProcessPaymentAsync(StripePaymentOptions options);
}