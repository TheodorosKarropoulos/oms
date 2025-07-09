using OMS.Domain.Entities;

namespace OMS.Application.Identity.Abstractions;

public interface ITokenProvider
{
    string Create(OmsUser user, ICollection<string> roles);
}