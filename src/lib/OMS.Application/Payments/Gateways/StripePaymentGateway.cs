using OMS.Application.Payments.Abstractions;
using OMS.Application.Payments.Dtos;
using OMS.Application.Payments.Mappers;
using OMS.Application.Payments.Options;
using OMS.Domain.Common;

namespace OMS.Application.Payments.Gateways;

internal sealed class StripePaymentGateway : IPaymentGateway
{
    private readonly IStubStripeApiClient _stripeApiClient;

    public StripePaymentGateway(
        IStubStripeApiClient stripeApiClient)
    {
        _stripeApiClient = stripeApiClient;
    }
    
    public async Task<Result<PaymentDto>> PayAsync(PaymentOptions options)
    {
        var result = await _stripeApiClient
            .ProcessPaymentAsync(options.ToStripePaymentOptions());

        if (!result.Successful)
        {
            return Result.Fail<PaymentDto>(result.ErrorMessage, result.StatusCode);
        }
        
        return Result.Success(new PaymentDto(result.Value));
    }
}