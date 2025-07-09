using OMS.Domain.Entities;

namespace OMS.Domain.Repositories;

public interface IMenuItemRepository
{
    void Create(MenuItem menuItem);
    
    void Update(MenuItem menuItem);
    
    Task<MenuItem?> GetByIdAsync(Guid menuItemId);
    
    Task<ICollection<MenuItem>> GetByIdsAsync(IEnumerable<Guid> ids);
   
    void Delete(MenuItem menuItem);
    Task<ICollection<MenuItem>> GatAllAsync();
}