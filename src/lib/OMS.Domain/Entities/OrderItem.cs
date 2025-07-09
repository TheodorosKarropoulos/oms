using OMS.Domain.Common;

namespace OMS.Domain.Entities;

public sealed class OrderItem
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public Guid MenuItemId { get; init; }
    
    public int Quantity { get; init; }
    
    public required Money UnitPrice { get; init; }
    
    public Money TotalPrice => new Money(Quantity * UnitPrice.Amount, UnitPrice.Currency);
    
}