namespace OMS.Application.Orders.Dtos;

public sealed record OrderItemDto
{
    public Guid MenuItemId { get; init; }

    public int Quantity { get; init; }

    public required string Currency { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal TotalPrice { get; init; }
}