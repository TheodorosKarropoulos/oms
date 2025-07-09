using OMS.Domain.Common;
using OMS.Domain.Constants;
using OMS.Domain.Exceptions;
using OMS.Domain.Primitives;

namespace OMS.Domain.Entities;

public sealed class Order : AuditableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public Guid CustomerId { get; private set; }
    
    public Guid? CourierId { get; private set; }

    public DeliveryMethod DeliveryMethod { get; private set; }

    public string? SpecialInstructions { get; private set; }
    
    public string? PaymentTransactionId { get; private set; }
    
    public PaymentProvider PaymentProvider { get; private set; }
    
    public bool IsPaid { get; private set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public IEnumerable<OrderItem> Items { get; private set; } = [];
    
    public Address? DeliveryAddress { get; private set; }

    private static readonly HashSet<(OrderStatus from, OrderStatus to)> StatusTransitions =
    [
        (OrderStatus.Pending, OrderStatus.Preparing),
        (OrderStatus.Preparing, OrderStatus.ReadyForPickup),
        (OrderStatus.Preparing, OrderStatus.ReadyForDelivery),
        (OrderStatus.ReadyForDelivery, OrderStatus.OutForDelivery),
        (OrderStatus.OutForDelivery, OrderStatus.Delivered),
        (OrderStatus.OutForDelivery, OrderStatus.UnableToDeliver)
    ];

    public void ChangeStatus(OrderStatus newStatus)
    {
        if (!StatusTransitions.Contains((Status, newStatus)))
        {
            throw new InvalidOrderStatusException($"Invalid status transition from: {Status} to: {newStatus}");
        }

        if (DeliveryMethod == DeliveryMethod.Pickup &&
            newStatus is OrderStatus.ReadyForDelivery or OrderStatus.OutForDelivery)
        {
            throw new InvalidOrderStatusException("Pickup orders cannot be marked for delivery.");
        }

        Status = newStatus;
    }

    public static Order Create(
        Guid customerId,
        DeliveryMethod deliveryMethod,
        ICollection<(Guid menuItemId, int quantity, decimal price, string currency)> items,
        PaymentProvider paymentProvider,
        string? paymentTransactionId,
        Address? deliveryAddress = null,
        string? specialInstructions = null)
    {
        if (items.Count == 0)
        {
            throw new ValidationException("Order must contain at least one item.");
        }
        
        var order = new Order
        {
            CustomerId = customerId,
            DeliveryMethod = deliveryMethod,
            SpecialInstructions = specialInstructions,
            PaymentProvider = paymentProvider,
            PaymentTransactionId = paymentTransactionId,
            Items = items.Select(x => 
                new OrderItem
                {
                    UnitPrice = new Money(x.price, x.currency),
                    Quantity = x.quantity,
                    MenuItemId = x.menuItemId
                }).ToList(),
            DeliveryAddress = deliveryAddress
        };
        
        return order;
    }
    
    public void AssignToDeliveryStaff(Guid courierId)
    {
        if (Status != OrderStatus.ReadyForDelivery)
        {
            throw new InvalidOrderStatusException("Only orders in 'Ready for delivery' status can be assigned to a delivery staff.");
        }
        
        CourierId = courierId;
        ChangeStatus(OrderStatus.OutForDelivery);
    }
    
    public void MarkAsPaid()
    {
        IsPaid = true;
    }

    public decimal TotalAmount => Items.Sum(x => x.TotalPrice.Amount);
}