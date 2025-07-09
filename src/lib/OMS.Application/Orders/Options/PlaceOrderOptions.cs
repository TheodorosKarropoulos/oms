using OMS.Domain.Constants;

namespace OMS.Application.Orders.Options;

public sealed record PlaceOrderOptions(
    DeliveryMethod DeliveryMethod,
    IReadOnlyCollection<OrderLineOptions> Lines,
    PaymentProvider PaymentProvider,
    string PaymentTransactionId,
    string Currency = Currencies.EUR,
    DeliveryAddressOptions? DeliveryAddress = null,
    string? SpecialInstructions = null);

public record OrderLineOptions(Guid MenuItemId, int Quantity, decimal Price);

public record DeliveryAddressOptions(
    string Street,
    string City,
    string ZipCode,
    string Country,
    string? State = null);