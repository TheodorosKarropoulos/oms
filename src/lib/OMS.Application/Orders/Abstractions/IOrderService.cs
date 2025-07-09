using OMS.Application.Orders.Dtos;
using OMS.Application.Orders.Options;
using OMS.Domain.Common;
using OMS.Domain.Constants;

namespace OMS.Application.Orders.Abstractions;

public interface IOrderService
{
    Task<Result<Guid>> PlaceOrderAsync(PlaceOrderOptions options);

    Task<Result> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
    
    Task<Result> AssignCourierAsync(Guid orderId, Guid courierId);
    
    Task<Result<OrderDto>> GetOrderAsync(Guid orderId);
    
    Task<Result<List<OrderDto>>> GetOrdersAsync(OrderStatus? status);
}