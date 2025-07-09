using OMS.Application.Orders.Dtos;
using OMS.Application.Orders.Options;
using OMS.Domain.Entities;

namespace OMS.Application.Orders.Mappers;

internal static class OrderAddressMapper
{
    internal static OrderAddressDto ToDto(this Address address)
    {
        return new OrderAddressDto
        {
            City = address.City,
            Country = address.Country,
            Street = address.Street,
            ZipCode = address.ZipCode,
            State = address.State
        };
    }

    internal static Address ToEntity(this DeliveryAddressOptions options)
    {
        return new Address
        {
            City = options.City,
            Country = options.Country,
            State = options.State ?? string.Empty,
            Street = options.Street,
            ZipCode = options.ZipCode,
        };
    }
}