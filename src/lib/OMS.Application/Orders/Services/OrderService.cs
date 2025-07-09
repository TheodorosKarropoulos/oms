using Microsoft.Extensions.Logging;
using OMS.Application.Common.Abstractions;
using OMS.Application.Identity.Abstractions;
using OMS.Application.Orders.Abstractions;
using OMS.Application.Orders.Dtos;
using OMS.Application.Orders.Mappers;
using OMS.Application.Orders.Options;
using OMS.Domain.Common;
using OMS.Domain.Constants;
using OMS.Domain.Entities;
using OMS.Domain.Repositories;

namespace OMS.Application.Orders.Services;

internal sealed class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMenuItemRepository _menuRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityService _identityService;

    public OrderService(
        ILogger<OrderService> logger,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IMenuItemRepository menuRepository,
        ICurrentUser currentUser,
        IIdentityService identityService)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _menuRepository = menuRepository;
        _currentUser = currentUser;
        _identityService = identityService;
    }
    
    public async Task<Result<Guid>> PlaceOrderAsync(PlaceOrderOptions options)
    {
        var menuItems = await _menuRepository
            .GetByIdsAsync(options.Lines.Select(x => x.MenuItemId));

        var linesContainsValidItems = options.Lines
            .Select(commandLine => menuItems.FirstOrDefault(x => x.Id == commandLine.MenuItemId))
            .Any(item => item is null);
        
        if (!linesContainsValidItems)
        {
            return Result.Fail<Guid>("Menu item not found.", StatusCode.BadRequest);
        }
        
        var order = Order.Create(_currentUser.UserIdGuid,
            options.DeliveryMethod,
            options.Lines.Select(x => (x.MenuItemId, x.Quantity, x.Price, options.Currency)).ToList(),
            options.PaymentProvider,
            options.PaymentTransactionId,
            options.DeliveryAddress?.ToEntity(),
            options.SpecialInstructions);

        if (!string.IsNullOrWhiteSpace(options.PaymentTransactionId))
        {
            order.MarkAsPaid();
        }

        _orderRepository.Add(order);
        
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success(order.Id, StatusCode.Created);
    }

    public async Task<Result> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            return Result.Fail("Update order status failed. Order not found.", StatusCode.BadRequest);
        }

        order.ChangeStatus(newStatus);

        _orderRepository.Update(order);
        
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<Result> AssignCourierAsync(Guid orderId, Guid courierId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            return Result.Fail("Assign courier failed. Order not found.", StatusCode.BadRequest);
        }

        var userExistsResults = await _identityService.UserExistsAsync(courierId);
        if (!userExistsResults.Successful)
        {
            return Result.Fail("Assign courier failed. Courier not found.", StatusCode.BadRequest);
        }
        
        order.AssignToDeliveryStaff(courierId);
        
        _orderRepository.Update(order);
        
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<Result<OrderDto>> GetOrderAsync(Guid orderId)
    {
        var order = _currentUser.IsCustomerRole
            ? await _orderRepository.GetByUserIdAndByIdAsync(_currentUser.UserIdGuid, orderId)
            : await _orderRepository.GetByIdAsync(orderId);
        
        if (order is null)
        {
            return Result.Fail<OrderDto>("Order not found.", StatusCode.NotFound);    
        }
        
        return Result.Success(order.ToDto());
    }

    public async Task<Result<List<OrderDto>>> GetOrdersAsync(OrderStatus? status)
    {
        var orders = await GetOrdersByUserRoleAsync(status);

        var dtos = orders
            .Select(x => x.ToDto())
            .ToList();
        
        return Result.Success(dtos);
    }

    private async Task<ICollection<Order>> GetOrdersByUserRoleAsync(OrderStatus? status)
    {
        if (_currentUser.IsAdminRole || _currentUser.IsStaffRole)
        {
            return await _orderRepository.GetOrdersAsync(status);
        }
        
        if (_currentUser.IsCustomerRole)
        {
            return await _orderRepository.GetOrdersByCustomerIdAsync(_currentUser.UserIdGuid);
        } 
        
        if (_currentUser.IsCourierRole)
        {
            return await _orderRepository.GetOrdersForDeliveryByCourierIdAsync(_currentUser.UserIdGuid);
        }

        return [];
    }
}