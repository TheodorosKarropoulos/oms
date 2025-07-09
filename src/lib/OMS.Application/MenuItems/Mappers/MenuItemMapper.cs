using OMS.Application.MenuItems.Dtos;
using OMS.Domain.Entities;

namespace OMS.Application.MenuItems.Mappers;

internal static class MenuItemMapper
{
    internal static MenuItemDto ToDto(this MenuItem menuItem)
    {
        return new MenuItemDto
        {
            Name = menuItem.Name,
            Currency = menuItem.Price.Currency,
            PictureUrl = menuItem.PictureUrl,
            Price = menuItem.Price.Amount
        };
    }
}