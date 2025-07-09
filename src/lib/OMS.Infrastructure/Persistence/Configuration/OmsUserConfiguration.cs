using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OMS.Domain.Entities;

namespace OMS.Infrastructure.Persistence.Configuration;

internal sealed class OmsUserConfiguration 
    : IEntityTypeConfiguration<OmsUser>
{
    public void Configure(EntityTypeBuilder<OmsUser> builder)
    {
        builder.ToTable("Users", "identity");
    }
}