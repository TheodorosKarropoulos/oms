using OMS.Domain.Constants;

namespace OMS.Api.Contracts.Requests;

public sealed record UpdateOrderStatusRequest(OrderStatus NewStatus);