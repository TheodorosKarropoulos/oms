using OMS.Domain.Common;

namespace OMS.Application.Identity.Abstractions;

public interface IIdentityService
{
    Task<Result> UserExistsAsync(Guid id);
}