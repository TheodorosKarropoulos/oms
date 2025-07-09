using FluentValidation;
using OMS.Application.MenuItems.Options;

namespace OMS.Application.MenuItems.Validators;

public sealed class MenuItemOptionsValidator
    : AbstractValidator<MenuItemOptions>
{
    public MenuItemOptionsValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Menu Item Name is required.")
            .MaximumLength(250);
        
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Menu Item Price must be greater than 0.");
        
        RuleFor(x => x.Currency)
            .Length(3)
            .WithMessage("Menu Item Currency must be 3 characters.")
            .Matches("^[A-Z]{3}$")
            .WithMessage("Menu Item Currency must be in ISO 4217 format.");
    }
}