using OMS.Domain.Entities;

namespace OMS.Domain.Repositories;

public interface IIdentityRepository
{
    Task<OmsUser?> GetByEmailAsync(string email);
    
    Task<bool> CheckPasswordAsync(OmsUser user, string password);
    
    Task<ICollection<string>> GetRolesAsync(OmsUser user);
    
    Task<bool> UserExistsAsync(Guid id);
}