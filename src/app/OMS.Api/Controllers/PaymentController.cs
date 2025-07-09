using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMS.Api.Constants;
using OMS.Api.Extensions;
using OMS.Application.Payments.Abstractions;
using OMS.Application.Payments.Options;

namespace OMS.Api.Controllers;

[Authorize(Policy = OmsPolicies.CustomerOnly)]
[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost]
    public async Task<IResult> Pay([FromBody] PaymentOptions options)
    {
        var result = await _paymentService.PayAsync(options);
        return result.AsResult();
    }
}