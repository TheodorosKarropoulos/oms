using Microsoft.EntityFrameworkCore;
using OMS.Domain.Constants;
using OMS.Domain.Entities;
using OMS.Domain.Repositories;

namespace OMS.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository : IOrderRepository
{
    private readonly OmsDbContext _dbContext;

    public OrderRepository(OmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Add(Order order)
    {
        _dbContext.Add(order);
    }

    public void Update(Order order)
    {
        _dbContext.Update(order);
    }

    public async Task<Order?> GetByIdAsync(Guid orderId)
    {
        return await _dbContext.Orders
            .Include(x => x.Items)
            .Include(x => x.DeliveryAddress)
            .FirstOrDefaultAsync(x => x.Id == orderId);
    }

    public async Task<Order?> GetByUserIdAndByIdAsync(Guid userId, Guid orderId)
    {
        return await _dbContext.Orders
            .Include(x => x.Items)
            .Include(x => x.DeliveryAddress)
            .FirstOrDefaultAsync(x => x.Id == orderId && x.CustomerId == userId);
    }

    public async Task<ICollection<Order>> GetOrdersAsync(OrderStatus? status)
    {
        var query = _dbContext.Orders.AsQueryable();

        if (status is not null)
        {
            query = query.Where(x => x.Status == status);
        }
        
        return await query.ToListAsync();
    }

    public async Task<ICollection<Order>> GetOrdersForDeliveryByCourierIdAsync(Guid courierId)
    {
        return await _dbContext.Orders
            .Where(x => x.CourierId == courierId && x.Status == OrderStatus.ReadyForDelivery)
            .ToListAsync(); 
    }

    public async Task<ICollection<Order>> GetOrdersByCustomerIdAsync(Guid customerId, OrderStatus? status)
    {
        var query = _dbContext.Orders
            .Where(x => x.CustomerId == customerId);
        
        if (status is not null)
        {
            query = query.Where(x => x.Status == status);
        }
        
        return await query.ToListAsync();
    }
}