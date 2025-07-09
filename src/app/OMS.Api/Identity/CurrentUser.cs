using System.Security.Claims;
using OMS.Application.Common.Abstractions;

namespace OMS.Api.Identity;

internal sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal Principal => httpContextAccessor.HttpContext!.User;

    public string UserId => Principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
    
    public IReadOnlyCollection<string> Roles =>
        Principal.FindAll(ClaimTypes.Role).Select(x => x.Value).ToArray();
}