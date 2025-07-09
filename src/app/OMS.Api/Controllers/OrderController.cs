using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMS.Api.Constants;
using OMS.Api.Contracts.Requests;
using OMS.Api.Extensions;
using OMS.Application.Orders.Abstractions;
using OMS.Application.Orders.Options;
using OMS.Domain.Constants;

namespace OMS.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(Policy = OmsPolicies.CustomerOnly)]
    [HttpPost]
    public async Task<IResult> PlaceOrder(PlaceOrderOptions options)
    {
        var result = await _orderService.PlaceOrderAsync(options);
        return result.AsResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> GetOrder(Guid id)
    {
        var result = await _orderService.GetOrderAsync(id);
        return result.AsResult();
    }

    [Authorize(Policy = OmsPolicies.StaffOnly)]
    [Authorize(Policy = OmsPolicies.DeliveryStaff)]
    [HttpPatch("{id:guid}/status")]
    public async Task<IResult> UpdateOrderStatus(Guid id, UpdateOrderStatusRequest request)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, request.NewStatus);
        return result.AsResult();
    }

    [Authorize(Policy = OmsPolicies.StaffOnly)]
    [HttpPatch("{id:guid}/assign/{courierId:guid}")]
    public async Task<IResult> AssignOrderToCourier(Guid id, Guid courierId)
    {
        var result = await _orderService.AssignCourierAsync(id, courierId);
        return result.AsResult();
    }

    [HttpGet]
    public async Task<IResult> GetOrders([FromQuery] OrderStatus? status)
    {
        var result = await _orderService.GetOrdersAsync(status);
        return result.AsResult();
    }
}