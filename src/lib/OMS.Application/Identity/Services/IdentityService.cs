using Microsoft.Extensions.Logging;
using OMS.Application.Identity.Abstractions;
using OMS.Domain.Common;
using OMS.Domain.Repositories;

namespace OMS.Application.Identity.Services;

internal sealed class IdentityService : IIdentityService
{
    private readonly ILogger<IdentityService> _logger;
    private readonly IIdentityRepository _identityRepository;

    public IdentityService(
        ILogger<IdentityService> logger,
        IIdentityRepository identityRepository)
    {
        _logger = logger;
        _identityRepository = identityRepository;
    }
    
    public async Task<Result<string>> AuthenticateAsync(string email, string password)
    {
        var user = await _identityRepository.GetByEmailAsync(email);
        if (user is null || !await _identityRepository.CheckPasswordAsync(user, password))
        {
            return Result.Fail<string>("Unable to authenticate user.", StatusCode.BadRequest);
        }
        
        var roles = await _identityRepository.GetRolesAsync(user);

        return Result.Success(string.Empty);
    }

    public async Task<Result> UserExistsAsync(Guid id)
    {
        var userExists = await _identityRepository.UserExistsAsync(id);
        
        return userExists ?  Result.Success() : Result.Fail("User not found", StatusCode.BadRequest);
    }
}