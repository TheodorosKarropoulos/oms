using OMS.Domain.Constants;
using OMS.Domain.Entities;
using OMS.Domain.Exceptions;

namespace OMS.Domain.Tests.Unit.Entities;

public class OrderTests
{
    [Fact]
    public void Pickup_Pending_To_Preparing_Succeeds()
    {
        // Arrange
        var order = CreatePickupOrder();
        
        // Act
        order.ChangeStatus(OrderStatus.Preparing);
        
        // Assert
        Assert.Equal(OrderStatus.Preparing, order.Status);
    }

    [Fact]
    public void Pickup_Preparing_To_ReadyForPickup_Succeeds()
    {
        // Arrange
        var order = CreatePickupOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        
        // Act
        order.ChangeStatus(OrderStatus.ReadyForPickup);
        
        // Assert
        Assert.Equal(OrderStatus.ReadyForPickup, order.Status);
    }

    [Fact]
    public void Delivery_Pending_To_Preparing_Succeeds()
    {
        // Arrange
        var order = CreateDeliveryOrder();
        
        // Act
        order.ChangeStatus(OrderStatus.Preparing);
        
        // Assert
        Assert.Equal(OrderStatus.Preparing, order.Status);
    }

    [Fact]
    public void Delivery_Preparing_To_ReadyForDelivery_Succeeds()
    {
        // Arrange
        var order = CreateDeliveryOrder();
        order.ChangeStatus(OrderStatus.Preparing);

        // Act
        order.ChangeStatus(OrderStatus.ReadyForDelivery);

        // Assert
        Assert.Equal(OrderStatus.ReadyForDelivery, order.Status);
    }

    [Fact]
    public void Delivery_ReadyForDelivery_To_OutForDelivery_Succeeds()
    {
        // Arrange
        var order = CreateDeliveryOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.ReadyForDelivery);
        
        // Act
        order.ChangeStatus(OrderStatus.OutForDelivery);
        
        // Assert
        Assert.Equal(OrderStatus.OutForDelivery, order.Status);
    }

    [Fact]
    public void Delivery_OutForDelivery_To_Delivered_Succeeds()
    {
        // Arrange
        var order = CreateDeliveryOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.ReadyForDelivery);
        order.ChangeStatus(OrderStatus.OutForDelivery);
        
        // Act
        order.ChangeStatus(OrderStatus.Delivered);
        
        // Assert
        Assert.Equal(OrderStatus.Delivered, order.Status);
    }

    [Fact]
    public void Delivery_OutForDelivery_To_UnableToDeliver_Succeeds()
    {
        // Arrange
        var order = CreateDeliveryOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.ReadyForDelivery);
        order.ChangeStatus(OrderStatus.OutForDelivery);
        
        // Act
        order.ChangeStatus(OrderStatus.UnableToDeliver);
        
        // Assert
        Assert.Equal(OrderStatus.UnableToDeliver, order.Status);
    }

    [Fact]
    public void Pickup_Pending_SkipPreparing_Throws()
    {
        // Arrange
        var order = CreatePickupOrder();
        
        // Act
        // Assert
        Assert.Throws<InvalidOrderStatusException>(() => 
            order.ChangeStatus(OrderStatus.ReadyForPickup));
    }

    [Fact]
    public void Pickup_Pending_To_ReadyForDelivery_Throws()
    {
        // Arrange
        var order = CreatePickupOrder();
        
        // Act
        // Assert
        Assert.Throws<InvalidOrderStatusException>(() => 
            order.ChangeStatus(OrderStatus.ReadyForDelivery));
    }

    [Fact]
    public void Pickup_Preparing_To_ReadyForDelivery_Throws()
    {
        // Arrange
        var order = CreatePickupOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        
        // Act
        // Assert
        Assert.Throws<InvalidOrderStatusException>(() => 
            order.ChangeStatus(OrderStatus.ReadyForDelivery));
    }

    [Fact]
    public void Pickup_ReadyForPickup_To_Delivered_Throws()
    {
        // Arrange
        var order = CreatePickupOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.ReadyForPickup);
        
        // Act
        // Assert
        Assert.Throws<InvalidOrderStatusException>(() => 
            order.ChangeStatus(OrderStatus.Delivered));
    }

    [Fact]
    public void Delivery_Pending_SkipPreparing_Throws()
    {
        // Arrange
        var order = CreateDeliveryOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        
        // Act
        // Assert
        Assert.Throws<InvalidOrderStatusException>(() => 
            order.ChangeStatus(OrderStatus.OutForDelivery));
    }
    
    [Fact]
    public void Delivery_ReadyForDelivery_SkipOutForDelivery_Throws()
    {
        // Arrange
        var order = CreateDeliveryOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.ReadyForDelivery);

        // Act
        // Assert
        Assert.Throws<InvalidOrderStatusException>(
            () => order.ChangeStatus(OrderStatus.Delivered));
    }
    
    [Fact]
    public void Delivery_Delivered_To_OutForDelivery_Throws()
    {
        // Arrange
        var order = CreateDeliveryOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.ReadyForDelivery);
        order.ChangeStatus(OrderStatus.OutForDelivery);
        order.ChangeStatus(OrderStatus.Delivered);

        // Act
        // Assert
        Assert.Throws<InvalidOrderStatusException>(
            () => order.ChangeStatus(OrderStatus.OutForDelivery));
    }
    
    [Fact]
    public void Delivery_UnableToDeliver_To_Delivered_Throws()
    {
        // Arrange
        var order = CreateDeliveryOrder();
        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.ReadyForDelivery);
        order.ChangeStatus(OrderStatus.OutForDelivery);
        order.ChangeStatus(OrderStatus.UnableToDeliver);

        // Act
        // Assert
        Assert.Throws<InvalidOrderStatusException>(
            () => order.ChangeStatus(OrderStatus.Delivered));
    }

    private static Order CreatePickupOrder()
    {
        return Order.Create(
            Guid.NewGuid(),
            DeliveryMethod.Pickup, 
            [(Guid.NewGuid(), 1, 10m, Currencies.EUR)],
            paymentProvider: PaymentProvider.Cash,
            "CASH-TX-2004");
    }

    private static Order CreateDeliveryOrder()
    {
        return Order.Create(
            Guid.NewGuid(),
            DeliveryMethod.Delivery, 
            [(Guid.NewGuid(), 1, 10m, Currencies.EUR)],
            PaymentProvider.Cash,
            "CASH-TX-2004",
            new Address
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "NY",
                ZipCode = "12345",
                Country = "GR"
            });
    }
}