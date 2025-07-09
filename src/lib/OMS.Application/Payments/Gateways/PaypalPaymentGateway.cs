using OMS.Application.Payments.Abstractions;
using OMS.Application.Payments.Dtos;
using OMS.Application.Payments.Mappers;
using OMS.Application.Payments.Options;
using OMS.Domain.Common;

namespace OMS.Application.Payments.Gateways;

internal sealed class PaypalPaymentGateway : IPaymentGateway
{
    private readonly IStubPayPalApiClient _payPalApiClient;

    public PaypalPaymentGateway(
        IStubPayPalApiClient payPalApiClient)
    {
        _payPalApiClient = payPalApiClient;
    }
    
    public async Task<Result<PaymentDto>> PayAsync(PaymentOptions options)
    {
        var result = await _payPalApiClient
            .ProcessPaymentAsync(options.ToPaypalPaymentOptions());
            
        if (!result.Successful)
        {
            return Result.Fail<PaymentDto>(result.ErrorMessage, result.StatusCode);
        }
            
        return Result.Success(new PaymentDto(result.Value));
    }
}