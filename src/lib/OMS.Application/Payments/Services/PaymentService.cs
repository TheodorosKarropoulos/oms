using Microsoft.Extensions.Logging;
using OMS.Application.Common.Abstractions;
using OMS.Application.Payments.Abstractions;
using OMS.Application.Payments.Dtos;
using OMS.Application.Payments.Options;
using OMS.Domain.Common;

namespace OMS.Application.Payments.Services;

internal sealed class PaymentService : IPaymentService
{
    private readonly ILogger<PaymentService> _logger;
    private readonly IPaymentGatewayFactory _factory;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(
        ILogger<PaymentService> logger,
        IPaymentGatewayFactory factory,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _factory = factory;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<PaymentDto>> PayAsync(PaymentOptions options)
    {
        var gateway = _factory.Resolve(options.Provider);
        var result = await gateway.PayAsync(options);

        if (!result.Successful)
        {
            _logger.LogWarning("Payment failed: {ResultErrorMessage}", result.ErrorMessage);
            return result;
        }
        
        await _unitOfWork.SaveChangesAsync();

        return result;
    }
}