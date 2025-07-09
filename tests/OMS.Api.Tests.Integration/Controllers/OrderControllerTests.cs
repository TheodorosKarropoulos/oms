using System.Net;
using System.Net.Http.Json;
using OMS.Api.Tests.Integration.Common;

namespace OMS.Api.Tests.Integration.Controllers;

public class OrderControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    
    public OrderControllerTests(OmsDefaultFixture fixture)
    {
        _client = fixture.CreateClient();
    }

    [Fact]
    public async Task PlaceOrder_without_lines_returns_400()
    {
        var badCmd = new
        {
            customerId = Guid.NewGuid(),
            deliveryMethod = "Delivery",
            lines = Array.Empty<object>()
        };

        var rsp = await _client.PostAsJsonAsync("/orders", badCmd);
        Assert.Equal(HttpStatusCode.BadRequest, rsp.StatusCode);
        
    }
    
    public ValueTask InitializeAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}