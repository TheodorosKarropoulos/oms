using Microsoft.EntityFrameworkCore;
using OMS.Domain.Entities;
using OMS.Domain.Repositories;

namespace OMS.Infrastructure.Persistence.Repositories;

internal sealed class MenuItemRepository : IMenuItemRepository
{
    private readonly OmsDbContext _dbContext;

    public MenuItemRepository(OmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Create(MenuItem menuItem)
    {
        _dbContext.MenuItems.Add(menuItem);
    }

    public void Update(MenuItem menuItem)
    {
        _dbContext.MenuItems.Update(menuItem);
    }

    public async Task<MenuItem?> GetByIdAsync(Guid menuItemId)
    {
        return await _dbContext.MenuItems
            .FirstOrDefaultAsync(x => x.Id == menuItemId);
    }

    public async Task<ICollection<MenuItem>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _dbContext.MenuItems
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
    }

    public void Delete(MenuItem menuItem)
    {
        _dbContext.MenuItems.Remove(menuItem);
    }

    public async Task<ICollection<MenuItem>> GatAllAsync()
    {
        return await _dbContext.MenuItems.ToListAsync();
    }
}