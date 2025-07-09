using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OMS.Domain.Entities;

namespace OMS.Infrastructure.Persistence.Configuration;

internal sealed class OrderConfiguration
    : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(x => x.Id)
            .IsClustered(false);
        
        builder.OwnsMany(x => x.Items, p =>
        {
            p.WithOwner().HasForeignKey("OrderId");
            p.HasKey("Id");
            p.ToTable("OrderItems");

            p.OwnsOne(i => i.UnitPrice, m =>
            {
                m.Property(p => p.Amount)
                    .HasColumnName("UnitPrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                m.Property(p => p.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsFixedLength()
                    .IsRequired();
            });
        });
        
        builder.OwnsOne(c => c.DeliveryAddress, a =>
        {
            a.WithOwner().HasForeignKey("OrderId");
            a.ToTable("OrderAddress");
            a.HasKey("Id");

            a.Property(p => p.City).IsRequired().HasMaxLength(200);
            a.Property(p => p.Country).HasMaxLength(200);
            a.Property(p => p.State).HasMaxLength(100);
            a.Property(p => p.ZipCode).HasMaxLength(20);
            a.Property(p => p.Street).HasMaxLength(200);
        });
    }
}