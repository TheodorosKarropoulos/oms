using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OMS.Application.Common.Abstractions;
using OMS.Domain.Entities;

namespace OMS.Infrastructure.Persistence;

public class OmsDbContext : IdentityDbContext<OmsUser, IdentityRole<Guid>, Guid>, IUnitOfWork
{
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    public DbSet<Order> Orders => Set<Order>();
    
    public OmsDbContext(DbContextOptions<OmsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(typeof(OmsDbContext).Assembly);
    }
}