using FluentValidation;
using Microsoft.Extensions.Logging;
using OMS.Application.Common.Abstractions;
using OMS.Application.MenuItems.Abstractions;
using OMS.Application.MenuItems.Dtos;
using OMS.Application.MenuItems.Mappers;
using OMS.Application.MenuItems.Options;
using OMS.Domain.Common;
using OMS.Domain.Entities;
using OMS.Domain.Repositories;

namespace OMS.Application.MenuItems.Services;

internal sealed class MenuItemService : IMenuItemService
{
    private readonly ILogger<MenuItemService> _logger;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly IValidator<MenuItemOptions> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public MenuItemService(
        ILogger<MenuItemService> logger,
        IMenuItemRepository menuItemRepository,
        IValidator<MenuItemOptions> validator,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _menuItemRepository = menuItemRepository;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid>> CreateAsync(MenuItemOptions options)
    {
        var validationResult = await _validator.ValidateAsync(options);
        if (!validationResult.IsValid)
        {
            return Result.Fail<Guid>("Create menu item failed due to validation errors", StatusCode.BadRequest);
        }
        
        var menuItem = MenuItem.Create(options.Name, options.Currency, options.Price, options.PictureUrl);
        
        _menuItemRepository.Create(menuItem);
        
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success(menuItem.Id);
    }

    public async Task<Result> UpdateAsync(Guid menuItemId, MenuItemOptions options)
    {
        var validationResult = await _validator.ValidateAsync(options);
        if (!validationResult.IsValid)
        {
            return Result.Fail("Update menu item failed due to validation errors.", StatusCode.BadRequest);
        }
        
        var menuItem = await _menuItemRepository.GetByIdAsync(menuItemId);
        if (menuItem is null)
        {
            return Result.Fail("Update menu item failed. Menu item not found.", StatusCode.BadRequest);
        }
        
        menuItem.Update(options.Name, options.Currency, options.Price, options.PictureUrl);
        
        _menuItemRepository.Update(menuItem);
        
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid menuItemId)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(menuItemId);
        if (menuItem is null)
        {
            return Result.Fail("Delete menu item failed. Menu item not found.", StatusCode.BadRequest);
        }

        _menuItemRepository.Delete(menuItem);
        
        await _unitOfWork.SaveChangesAsync();
        
        return Result.Success();
    }

    public async Task<Result<MenuItemDto>> GetAsync(Guid menuItemId)
    {
        var menuItem = await _menuItemRepository.GetByIdAsync(menuItemId);
        if (menuItem is null)
        {
            return Result.Fail<MenuItemDto>("Menu item not found.", StatusCode.BadRequest);
        }
        
        return Result.Success(menuItem.ToDto());
    }

    public async Task<Result<IEnumerable<MenuItemDto>>> GetAllAsync()
    {
        var menuItems = await _menuItemRepository.GatAllAsync();
        
        var result = menuItems
            .Select(x => x.ToDto());
            
        return Result.Success(result);
      
    }
}