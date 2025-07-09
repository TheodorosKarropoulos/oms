using Microsoft.Extensions.Logging;
using OMS.Application.Payments.Abstractions;
using OMS.Application.Payments.Options;
using OMS.Domain.Common;
using OMS.Domain.Constants;

namespace OMS.Infrastructure.Integrations.Payments.Clients;

internal sealed class StubPayPalApiClient : IStubPayPalApiClient
{
    private readonly ILogger<StubPayPalApiClient> _logger;
    private readonly HttpClient _httpClient;

    public StubPayPalApiClient(
        IHttpClientFactory httpClientFactory,
        ILogger<StubPayPalApiClient> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(nameof(PaymentProvider.PayPal));
    }
    
    public async Task<Result<string>> ProcessPaymentAsync(PaypalPaymentOptions options)
    {
        try
        {
            var transactionId = $"FAKE-PAYPAL-{options.Amount}-{options.Currency}-{Guid.NewGuid():N}"
                .ToUpperInvariant();
            
            return Result.Success(transactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment with PayPal");
            return Result.Fail<string>("Error processing payment with PayPal.", StatusCode.InternalServerError);
        }
    }
}