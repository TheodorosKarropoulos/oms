namespace OMS.Domain.Constants;

public enum OrderStatus
{
    Pending,
    Preparing,
    ReadyForPickup,
    ReadyForDelivery,
    OutForDelivery,
    Delivered,
    UnableToDeliver
}