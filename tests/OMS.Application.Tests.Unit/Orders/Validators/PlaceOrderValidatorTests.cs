using FluentValidation.TestHelper;
using OMS.Application.Orders.Options;
using OMS.Application.Orders.Validators;
using OMS.Domain.Constants;

namespace OMS.Application.Tests.Unit.Orders.Validators;

public class PlaceOrderValidatorTests
{
    private readonly PlaceOrderValidator _validator = new();

    private static PlaceOrderOptions Valid() =>
        new(DeliveryMethod.Delivery,
            new List<OrderLineOptions> { new(Guid.NewGuid(), 2, 12m) },
            PaymentProvider.PayPal, Currencies.EUR, "PAYPAL-TX-2004",new DeliveryAddressOptions(
                "Street", "City", "ZipCode", "GR", "State"));
    
    private static PlaceOrderOptions InValidMenuItemId() =>
        new(DeliveryMethod.Delivery,
            new List<OrderLineOptions> { new(Guid.Empty, 2, 12m) },
            PaymentProvider.PayPal, Currencies.EUR, "PAYPAL-TX-2004", new DeliveryAddressOptions(
                "Street", "City", "ZipCode", "GR", "State"));
    
    private static PlaceOrderOptions InValidQuantity(int quantity) =>
        new(DeliveryMethod.Delivery,
            new List<OrderLineOptions> { new(Guid.NewGuid(), quantity, 12m) },
            PaymentProvider.PayPal, Currencies.EUR, "PAYPAL-TX-2004",new DeliveryAddressOptions(
                "Street", "City", "ZipCode", "GR", "State"));
    
    [Fact]
    public void Valid_options_pass()
    {
        _validator.TestValidate(Valid())
            .ShouldNotHaveAnyValidationErrors();
    }
    
    [Fact]
    public void Invalid_delivery_method_fails()
    {
        var model = Valid() with { DeliveryMethod = (DeliveryMethod)99 };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(o => o.DeliveryMethod)
            .WithErrorMessage("Delivery method is invalid.");
    }
    
    [Fact]
    public void Empty_lines_fail()
    {
        var model = Valid() with { Lines = [] };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(o => o.Lines)
            .WithErrorMessage("At least one line is required.");
    }
    
    [Fact]
    public void More_than_max_lines_fail()
    {
        const int MaxLines = 10;
        
        var lines = new List<OrderLineOptions>();
        for (var i = 0; i < MaxLines + 1; i++)
            lines.Add( new OrderLineOptions(Guid.NewGuid(), 2, 12m));

        var model = Valid() with { Lines = lines };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(o => o.Lines)
            .WithErrorMessage($"Maximum {MaxLines} lines are allowed.");
    }
    
    [Fact]
    public void Line_with_empty_menuItemId_fails()
    {
        var model = InValidMenuItemId();

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor("Lines[0].MenuItemId")
            .WithErrorMessage("Menu item id is required.");
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Line_with_non_positive_quantity_fails(int qty)
    {
        var model = InValidQuantity(qty);

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor("Lines[0].Quantity")
            .WithErrorMessage("Quantity must be greater than 0.");
    }
}