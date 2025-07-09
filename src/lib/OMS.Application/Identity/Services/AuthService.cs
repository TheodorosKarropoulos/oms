using OMS.Application.Identity.Abstractions;
using OMS.Domain.Common;
using OMS.Domain.Repositories;

namespace OMS.Application.Identity.Services;

internal sealed class AuthService : IAuthService
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IIdentityRepository _identityRepository;

    public AuthService(
        ITokenProvider tokenProvider,
        IIdentityRepository identityRepository)
    {
        _tokenProvider = tokenProvider;
        _identityRepository = identityRepository;
    }
    
    public async Task<Result<string>> AuthenticateAsync(string email, string password)
    {
        var user = await _identityRepository.GetByEmailAsync(email);
        if (user is null)
        {
            return Result.Fail<string>("Unable to authenticate user.", StatusCode.BadRequest);
        }

        var verified = await _identityRepository.CheckPasswordAsync(user, password);
        if (!verified)
        {
            return Result.Fail<string>("Unable to authenticate user.", StatusCode.BadRequest);
        }
        
        var userRoles = await _identityRepository.GetRolesAsync(user);
        
        return Result.Success(_tokenProvider.Create(user, userRoles));
    }
}