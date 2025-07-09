using FluentValidation.TestHelper;
using OMS.Application.MenuItems.Options;
using OMS.Application.MenuItems.Validators;
using OMS.Domain.Constants;

namespace OMS.Application.Tests.Unit.MenuItems.Validators;

public class MenuItemOptionsValidatorTests
{
    private readonly MenuItemOptionsValidator _validator = new();

    private static MenuItemOptions Valid() => 
        new ("Cheeseburger", 7.90m, Currencies.EUR);

    [Fact]
    public void Valid_options_pass_validation()
    {
        var result = _validator.TestValidate(Valid());
        result.ShouldNotHaveAnyValidationErrors();
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Name_is_required(string? badName)
    {
        var model = Valid() with { Name = badName! };

        var result = _validator.TestValidate(model);

        result.ShouldHaveValidationErrorFor(m => m.Name)
            .WithErrorMessage("Menu Item Name is required.");
    }
    
    [Fact]
    public void Name_longer_than_250_chars_fails()
    {
        var model = Valid() with { Name = new string('x', 251) };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(m => m.Name);
    }
    
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Price_must_be_positive(decimal price)
    {
        var model = Valid() with { Price = price };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(m => m.Price)
            .WithErrorMessage("Menu Item Price must be greater than 0.");
    }
    
    [Theory]
    [InlineData("EU")]
    [InlineData("EURO")]
    [InlineData("eur")]
    [InlineData("€€€")]
    public void Currency_must_be_iso4217(string badCurrency)
    {
        var model = Valid() with { Currency = badCurrency };

        _validator.TestValidate(model)
            .ShouldHaveValidationErrorFor(m => m.Currency);
    }
}