using OMS.Domain.Common;

namespace OMS.Application.Identity.Abstractions;

public interface IAuthService
{
    Task<Result<string>> AuthenticateAsync(string email, string password);
}