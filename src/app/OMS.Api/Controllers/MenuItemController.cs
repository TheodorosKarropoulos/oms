using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OMS.Api.Constants;
using OMS.Api.Extensions;
using OMS.Application.MenuItems.Abstractions;
using OMS.Application.MenuItems.Options;

namespace OMS.Api.Controllers;

[Authorize(Policy = OmsPolicies.AdminOnly)]
[ApiController]
[Route("api/menuitems")]
public sealed class MenuItemController : ControllerBase
{
    private readonly IMenuItemService _menuItemService;

    public MenuItemController(IMenuItemService menuItemService)
    {
        _menuItemService = menuItemService;
    }

    [HttpGet]
    public async Task<IResult> GetMenuItems()
    {
        var result = await _menuItemService.GetAllAsync();
        return result.AsResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<IResult> GetMenuItem(Guid id)
    {
        var result = await _menuItemService.GetAsync(id);
        return result.AsResult();
    }

    [HttpPost]
    public async Task<IResult> Create([FromBody] MenuItemOptions request)
    {
        var result = await _menuItemService
            .CreateAsync(request);

        return result.AsResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IResult> Update(Guid id, [FromBody] MenuItemOptions options)
    {
        var result = await _menuItemService.UpdateAsync(id, options);
        return result.AsResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IResult> Delete(Guid id)
    {
        var result = await _menuItemService.DeleteAsync(id);
        return result.AsResult();
    }
}