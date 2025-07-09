using Microsoft.Extensions.Logging;
using OMS.Application.Payments.Abstractions;
using OMS.Application.Payments.Options;
using OMS.Domain.Common;
using OMS.Domain.Constants;

namespace OMS.Infrastructure.Integrations.Payments.Clients;

internal sealed class StubStripeClient : IStubStripeApiClient
{
    private readonly ILogger<StubStripeClient> _logger;
    private readonly HttpClient _httpClient;

    public StubStripeClient(
        IHttpClientFactory httpClientFactory,
        ILogger<StubStripeClient> logger)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(nameof(PaymentProvider.Stripe));
    }
    
    public async Task<Result<string>> ProcessPaymentAsync(StripePaymentOptions options)
    {
        try
        {
            var transactionId = $"FAKE-STRIPE-{options.Amount}-{options.Currency}-{Guid.NewGuid():N}"
                .ToUpperInvariant();
            
            return Result.Success(transactionId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing payment with Stripe");
            return Result.Fail<string>("Error processing payment with Stripe.", StatusCode.InternalServerError);
        }
    }
}