namespace OMS.Application.MenuItems.Dtos;

public sealed record MenuItemDto
{
    public required string Name { get; init; }
    
    public decimal Price { get; init; }
    
    public required string Currency { get; init; }
    
    public string? PictureUrl { get; init; }
}