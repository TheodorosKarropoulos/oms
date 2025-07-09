using OMS.Application.MenuItems.Dtos;
using OMS.Application.MenuItems.Options;
using OMS.Domain.Common;

namespace OMS.Application.MenuItems.Abstractions;

public interface IMenuItemService
{
    Task<Result<Guid>> CreateAsync(MenuItemOptions options);
    
    Task<Result> UpdateAsync(Guid menuItemId, MenuItemOptions options);
    
    Task<Result> DeleteAsync(Guid menuItemId);
    
    Task<Result<MenuItemDto>> GetAsync(Guid menuItemId);
    
    Task<Result<IEnumerable<MenuItemDto>>> GetAllAsync();
}