using OMS.Application.Payments.Options;

namespace OMS.Application.Payments.Mappers;

internal static class PaymentOptionsMapper
{
    internal static PaypalPaymentOptions ToPaypalPaymentOptions(this PaymentOptions options)
    {
        return new PaypalPaymentOptions
        {
            Currency = options.Currency,
            Amount = options.Amount,
            Provider = options.Provider
        };
    }

    internal static StripePaymentOptions ToStripePaymentOptions(this PaymentOptions options)
    {
        return new StripePaymentOptions
        {
            Currency = options.Currency,
            Amount = options.Amount,
            Provider = options.Provider
        };
    }
}