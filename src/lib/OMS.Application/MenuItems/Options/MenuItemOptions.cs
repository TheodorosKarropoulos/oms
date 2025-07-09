using OMS.Domain.Constants;

namespace OMS.Application.MenuItems.Options;

public sealed record MenuItemOptions(
    string Name,
    decimal Price,
    string Currency = Currencies.EUR,
    string? PictureUrl = null);