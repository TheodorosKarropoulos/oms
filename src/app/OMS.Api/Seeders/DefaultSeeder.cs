using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OMS.Domain.Common;
using OMS.Domain.Constants;
using OMS.Domain.Entities;
using OMS.Infrastructure.Persistence;

namespace OMS.Api.Seeders;

internal static class DefaultSeeder
{
    // private static readonly Guid PepperoniPizzaId = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87001");
    // private static readonly Guid MargheritaPizzaId = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87002");
    private static readonly Guid StaffUserId = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87005");
    private static readonly Guid DeliveryStaffUserId = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87006");
    private static readonly Guid CustomerUserId = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87007");
    private static readonly Guid AdminUserId = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87008");
    
    private static readonly Guid Address1 = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87018");
    private static readonly Guid Address2 = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87019");
    private static readonly Guid Address3 = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87020");
    private static readonly Guid Address4 = Guid.Parse("f6f1d4a4-9e18-4a27-af8d-4c43c1a87021");

    private const string SecureUserPassword = "Dev123!";

    internal static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        await SeedUsersAsync(serviceProvider);
        await SeedMenuItemsAsync(serviceProvider);
        await SeedOrdersAsync(serviceProvider);
    }

    private static async Task SeedUsersAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<OmsUser>>();

        foreach (var role in new[] { UserRoles.Customer, UserRoles.Staff, UserRoles.DeliveryStaff, UserRoles.Admin })
        {
            if (await roleManager.RoleExistsAsync(role))
            {
                continue;
            }
            
            await roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
        }

        async Task CreateUser(string email, string role, Guid id)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
            {
                user = new OmsUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    Id = id
                };

                await userManager.CreateAsync(user, SecureUserPassword);
            }

            var userRoles = await userManager.GetRolesAsync(user);
            if (!userRoles.Contains(role))
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }

        await CreateUser("staff@demo.io", UserRoles.Staff, StaffUserId);
        await CreateUser("delivery@demo.io", UserRoles.DeliveryStaff, DeliveryStaffUserId);
        await CreateUser("admin@demo.io", UserRoles.Admin, AdminUserId);
        await CreateUser("customer@demo.io", UserRoles.Customer, CustomerUserId);
    }

    private static async Task SeedMenuItemsAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<OmsDbContext>();

        var menuItems = new List<MenuItem>
        {
            MenuItem.Create("Pepperoni Pizza", Currencies.EUR, 10m),
            MenuItem.Create("Margherita Pizza", Currencies.EUR, 15m),
            MenuItem.Create("Chicken Caesar Salad", Currencies.EUR, 12m),
            MenuItem.Create("Spaghetti Carbonara", Currencies.EUR, 11.3m)
        };

        foreach (var menuItem in menuItems)
        {
            var menuItemEntity = await dbContext.MenuItems
                .FirstOrDefaultAsync(x => x.Name == menuItem.Name);

            if (menuItemEntity is null)
            {
                await dbContext.MenuItems.AddAsync(menuItem);
            }
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedOrdersAsync(IServiceProvider serviceProvider)
    {
        var dbContext = serviceProvider.GetRequiredService<OmsDbContext>();
        await SeedPendingNotPaidOrder(dbContext);
        await SeedPreparingOrder(dbContext);
        await SeedPendingPaidOrder(dbContext);
        await SeedReadyForDeliverOrder(dbContext);
        
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedPendingNotPaidOrder(OmsDbContext dbContext)
    {
        var orderExists = await dbContext.Orders
            .AnyAsync(x => x.DeliveryAddress!.Id == Address1);

        if (orderExists)
        {
            return;
        }
        
        var address = new Address
        {
            City = "Demoville",
            Country = "GR",
            Street = "Demoville Street 1",
            ZipCode = "12345",
            Id = Address1,
            State = "DV"
        };

        var pepperoniPizzaId = (await dbContext.MenuItems
            .FirstAsync(x => x.Name == "Pepperoni Pizza"))
            .Id;

        var margheritaPizzaId = (await dbContext.MenuItems
                .FirstAsync(x => x.Name == "Margherita Pizza"))
            .Id;
        
        var order = Order.Create(CustomerUserId,
            DeliveryMethod.Pickup,
            [
                (pepperoniPizzaId, 1, 10.5m, Currencies.EUR),
                (margheritaPizzaId, 1, 15.5m, Currencies.EUR)
            ],
            PaymentProvider.Cash,
            "TXN-CASH-0001",
            address);
        
        dbContext.Orders.Add(order);
    }

    private static async Task SeedPreparingOrder(OmsDbContext dbContext)
    {
        var orderExists = await dbContext.Orders
            .AnyAsync(x => x.DeliveryAddress!.Id == Address2);

        if (orderExists)
        {
            return;
        }
        
        var address = new Address
        {
            City = "Demoville",
            Country = "GR",
            Street = "Demoville Street 2",
            ZipCode = "12345",
            Id = Address2,
            State = "DV"
        };
        
        var pepperoniPizzaId = (await dbContext.MenuItems
                .FirstAsync(x => x.Name == "Pepperoni Pizza"))
            .Id;

        var margheritaPizzaId = (await dbContext.MenuItems
                .FirstAsync(x => x.Name == "Margherita Pizza"))
            .Id;
        
        var order = Order.Create(CustomerUserId,
            DeliveryMethod.Delivery,
            [
                (pepperoniPizzaId, 1, 10.5m, Currencies.EUR),
                (margheritaPizzaId, 1, 15.5m, Currencies.EUR)
            ],
            PaymentProvider.PayPal,
            "PAYPAL-TX-2002",
            address);
        
        order.ChangeStatus(OrderStatus.Preparing);
        order.MarkAsPaid();
        
        dbContext.Orders.Add(order);
    }
    
    private static async Task SeedReadyForDeliverOrder(OmsDbContext dbContext)
    {
        var orderExists = await dbContext.Orders
            .AnyAsync(x => x.DeliveryAddress!.Id == Address3);

        if (orderExists)
        {
            return;
        }
        
        var address = new Address
        {
            City = "Demoville",
            Country = "GR",
            Street = "Demoville Street 3",
            ZipCode = "12345",
            Id = Address3,
            State = "DV"
        };
        
        var pepperoniPizzaId = (await dbContext.MenuItems
                .FirstAsync(x => x.Name == "Pepperoni Pizza"))
            .Id;

        var margheritaPizzaId = (await dbContext.MenuItems
                .FirstAsync(x => x.Name == "Margherita Pizza"))
            .Id;
        
        var order = Order.Create(CustomerUserId,
            DeliveryMethod.Delivery,
            [
                (pepperoniPizzaId, 1, 10.5m, Currencies.EUR),
                (margheritaPizzaId, 1, 15.5m, Currencies.EUR)
            ],
            PaymentProvider.PayPal,
            "PAYPAL-TX-2003",
            address);
        
        order.ChangeStatus(OrderStatus.Preparing);
        order.ChangeStatus(OrderStatus.ReadyForDelivery);
        order.MarkAsPaid();
        
        dbContext.Orders.Add(order);
    }
    
    private static async Task SeedPendingPaidOrder(OmsDbContext dbContext)
    {
        var orderExists = await dbContext.Orders
            .AnyAsync(x => x.DeliveryAddress!.Id == Address4);

        if (orderExists)
        {
            return;
        }
        
        var address = new Address
        {
            City = "Demoville",
            Country = "GR",
            Street = "Demoville Street 4",
            ZipCode = "12345",
            Id = Address4,
            State = "DV"
        };

        var pepperoniPizzaId = (await dbContext.MenuItems
                .FirstAsync(x => x.Name == "Pepperoni Pizza"))
            .Id;

        var margheritaPizzaId = (await dbContext.MenuItems
                .FirstAsync(x => x.Name == "Margherita Pizza"))
            .Id;
        
        var order = Order.Create(CustomerUserId,
            DeliveryMethod.Delivery,
            [
                (pepperoniPizzaId, 1, 10.5m, Currencies.EUR),
                (margheritaPizzaId, 1, 15.5m, Currencies.EUR)
            ],
            PaymentProvider.PayPal,
            "PAYPAL-TX-2004",
            address);
        
        order.MarkAsPaid();
        
        dbContext.Orders.Add(order);
    }
}