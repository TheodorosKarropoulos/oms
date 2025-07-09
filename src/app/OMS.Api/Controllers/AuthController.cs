using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OMS.Api.Extensions;
using OMS.Application.Identity.Abstractions;

namespace OMS.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IResult> Login([FromBody] LoginRequest request)
    {
        var tokenResult = await _authService.AuthenticateAsync(request.Email, request.Password);
        return tokenResult.AsResult();
    }
}