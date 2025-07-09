using OMS.Application.Orders.Dtos;
using OMS.Domain.Entities;

namespace OMS.Application.Orders.Mappers;

internal static class OrderItemMapper
{
    internal static OrderItemDto ToDto(this OrderItem orderItem)
    {
        return new OrderItemDto
        {
            Currency = orderItem.UnitPrice.Currency,
            UnitPrice = orderItem.UnitPrice.Amount,
            MenuItemId = orderItem.MenuItemId,
            Quantity = orderItem.Quantity,
            TotalPrice = orderItem.TotalPrice.Amount
        };
    }
}