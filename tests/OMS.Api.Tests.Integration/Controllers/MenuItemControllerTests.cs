using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OMS.Api.Tests.Integration.Common;
using OMS.Application.MenuItems.Dtos;
using OMS.Domain.Constants;
using OMS.Domain.Entities;
using OMS.Infrastructure.Persistence;

namespace OMS.Api.Tests.Integration.Controllers;

public class MenuItemControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;

    private readonly OmsDbContext _dbContext;
    
    public MenuItemControllerTests(OmsDefaultFixture fixture)
    {
        _client = fixture.CreateClient();

        var scope = fixture.CreateScope();

        _dbContext = scope.ServiceProvider.GetRequiredService<OmsDbContext>();
    }

    [Fact]
    public async Task GetAll_Returns_200_With_Data()
    {
        // Arrange
        _client.DefaultRequestHeaders.Add(TestAuthHandler.TestUserRolesHeader, UserRoles.Admin);
        var menuItems = new List<MenuItem>
        {
            MenuItem.Create("Burger", Currencies.EUR, 12.9m),
            MenuItem.Create("Fries", Currencies.EUR, 2m)
        };
        
        _dbContext.MenuItems.AddRange(menuItems);
        
        await _dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
        
        // Act
        var response = await _client.GetAsync("api/menuitems", TestContext.Current.CancellationToken);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var data = await response.Content
            .ReadFromJsonAsync<MenuItemDto[]>(cancellationToken: TestContext.Current.CancellationToken);

        Assert.NotNull(data);
        var names = data.Select(i => i.Name).ToList();
        
        Assert.Contains("Fries", names);
        Assert.Contains("Burger", names);
    }
    
    public ValueTask InitializeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}