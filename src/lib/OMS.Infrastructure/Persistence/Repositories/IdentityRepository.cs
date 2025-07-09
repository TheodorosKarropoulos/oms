using Microsoft.AspNetCore.Identity;
using OMS.Domain.Entities;
using OMS.Domain.Repositories;

namespace OMS.Infrastructure.Persistence.Repositories;

internal class IdentityRepository : IIdentityRepository
{
    private readonly UserManager<OmsUser> _userManager;

    public IdentityRepository(UserManager<OmsUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<OmsUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<bool> CheckPasswordAsync(OmsUser user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<ICollection<string>> GetRolesAsync(OmsUser user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> UserExistsAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        return user != null;
    }
}