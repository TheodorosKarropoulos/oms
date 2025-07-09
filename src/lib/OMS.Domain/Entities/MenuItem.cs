using OMS.Domain.Common;
using OMS.Domain.Constants;
using OMS.Domain.Primitives;

namespace OMS.Domain.Entities;

public sealed class MenuItem : AuditableEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public string Name { get; private set; } = string.Empty;

    public Money Price { get; private set; } = new(0, Currencies.EUR);

    public string? PictureUrl { get; private set; }

    public bool IsActive { get; private set; } = true;

    public void Deactivate() => IsActive = false;

    public static MenuItem Create(
        string name,
        string currency,
        decimal price,
        string? pictureUrl = null)
    {
        var menuItem = new MenuItem
        {
            Name = name,
            Price = new Money(price, currency),
            PictureUrl = pictureUrl
        };

        return menuItem;
    }
    
    public void Update(
        string name,
        string currency,
        decimal price,
        string? pictureUrl = null)
    {
        Name = name;
        Price = new Money(price, currency);
        PictureUrl = pictureUrl;
    }
}