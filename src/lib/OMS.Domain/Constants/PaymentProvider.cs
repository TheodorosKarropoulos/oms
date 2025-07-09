namespace OMS.Domain.Constants;

public enum PaymentProvider : byte
{
    Cash = 0,
    Stripe,
    PayPal
}