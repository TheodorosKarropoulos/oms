namespace OMS.Domain.Entities;

public sealed class Address
{
    public Guid Id { get; init; } = Guid.NewGuid();
    
    public required string Street { get; init; }
    
    public required string City { get; init; }
    
    public required string State { get; init; }
    
    public required string ZipCode { get; init; }
    
    public required string Country { get; init; }
}