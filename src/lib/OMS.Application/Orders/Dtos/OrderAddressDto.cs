namespace OMS.Application.Orders.Dtos;

public record OrderAddressDto
{
    public required string Street { get; init; }
    
    public required string City { get; init; }
    
    public required string State { get; init; }
    
    public required string ZipCode { get; init; }
    
    public required string Country { get; init; }
}