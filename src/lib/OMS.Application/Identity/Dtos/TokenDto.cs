namespace OMS.Application.Identity.Dtos;

public sealed record TokenDto
{
    public required string Token { get; init; }
    
    public required string RefreshToken { get; init; }
    
    public DateTimeOffset Expires { get; init; }
}