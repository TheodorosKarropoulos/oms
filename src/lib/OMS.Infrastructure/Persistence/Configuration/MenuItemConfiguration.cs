using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OMS.Domain.Entities;

namespace OMS.Infrastructure.Persistence.Configuration;

internal sealed class MenuItemConfiguration
    : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("MenuItems");

        builder
            .HasKey(x => x.Id)
            .IsClustered(false);

        builder
            .Property(x => x.Name)
            .HasMaxLength(250)
            .IsRequired();

        builder
            .OwnsOne(x => x.Price, p =>
            {
                p.Property(m => m.Amount)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                p.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsFixedLength()
                    .IsRequired();
            });
    }
}