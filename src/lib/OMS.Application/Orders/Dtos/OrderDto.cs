namespace OMS.Application.Orders.Dtos;

public record OrderDto
{
    public Guid Id { get; init; }
    
    public Guid CustomerId { get; init; }
    
    public required string DeliveryMethod { get; init; }
    
    public required string Status { get; init; }

    public ICollection<OrderItemDto> Items { get; init; } = [];

    public string? SpecialInstructions { get; init; }
    
    public OrderAddressDto? Address { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
}