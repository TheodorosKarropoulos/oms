using OMS.Domain.Constants;
using OMS.Domain.Entities;

namespace OMS.Domain.Repositories;

public interface IOrderRepository
{
    void Add(Order order);
    
    void Update(Order order);
    
    Task<Order?> GetByIdAsync(Guid orderId);
    
    Task<Order?> GetByUserIdAndByIdAsync(Guid userId, Guid orderId);
    
    Task<ICollection<Order>> GetOrdersAsync(OrderStatus? status);
    
    Task<ICollection<Order>> GetOrdersForDeliveryByCourierIdAsync(Guid courierId);
    
    Task<ICollection<Order>> GetOrdersByCustomerIdAsync(Guid customerId, OrderStatus? status = null);
}