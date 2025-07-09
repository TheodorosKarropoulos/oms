using OMS.Domain.Constants;

namespace OMS.Domain.Common;

public sealed record Money
{
    public decimal Amount { get; init; }

    public string Currency { get; init; } = Currencies.EUR;
    
    public Money(decimal amount, string currency = Currencies.EUR) => (Amount, Currency) = (amount, currency);
    
    public override string ToString() => $"{Amount:0.00} {Currency}";
}