using OMS.Application.Orders.Dtos;
using OMS.Domain.Entities;

namespace OMS.Application.Orders.Mappers;

internal static class OrderMapper
{
    internal static OrderDto ToDto(this Order order)
    {
        return new OrderDto
        {
            DeliveryMethod = order.DeliveryMethod.ToString(),
            Status = order.Status.ToString(),
            CreatedAt = order.CreatedAt,
            CustomerId = order.CustomerId,
            SpecialInstructions = order.SpecialInstructions,
            Items = order.Items.Select(x => x.ToDto()).ToList(),
            Address = order.DeliveryAddress?.ToDto()
        };
    }
}