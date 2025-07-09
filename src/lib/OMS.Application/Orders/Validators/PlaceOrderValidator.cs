using FluentValidation;
using OMS.Application.Orders.Options;

namespace OMS.Application.Orders.Validators;

public sealed class PlaceOrderValidator 
    : AbstractValidator<PlaceOrderOptions>
{
    private const int MaxLines = 10;
    
    public PlaceOrderValidator()
    {
        RuleFor(x => x.DeliveryMethod)
            .IsInEnum()
            .WithMessage("Delivery method is invalid.");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("At least one line is required.")
            .Must(x => x.Count <= MaxLines)
            .WithMessage($"Maximum {MaxLines} lines are allowed.");

        RuleForEach(x => x.Lines)
            .ChildRules(x =>
            {
                x.RuleFor(y => y.MenuItemId)
                    .NotEmpty()
                    .WithMessage("Menu item id is required.");

                x.RuleFor(y => y.Quantity)
                    .GreaterThan(0)
                    .WithMessage("Quantity must be greater than 0.");
            });
    }
}