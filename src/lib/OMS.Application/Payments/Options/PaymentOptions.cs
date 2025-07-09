using OMS.Domain.Constants;

namespace OMS.Application.Payments.Options;

public record PaymentOptions
{
    public PaymentProvider Provider { get; init; }

    public required string Currency { get; init; }

    public decimal Amount { get; init; }
}