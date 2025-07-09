using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OMS.Domain.Primitives;

namespace OMS.Infrastructure.Interceptors;

internal sealed class AuditSaveChangesInterceptor 
    : SaveChangesInterceptor
{
    private readonly TimeProvider _timeProvider;

    public AuditSaveChangesInterceptor(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }
    
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        SetAuditFields(eventData.Context);
        return base.SavingChanges(eventData, result);
    }
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        SetAuditFields(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    private void SetAuditFields(DbContext? ctx)
    {
        if (ctx is null)
        {
            return;
        }

        foreach (var entry in ctx.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified)
            {
                var now = _timeProvider.GetUtcNow();
                
                entry.Entity.LastModified = now;
                
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.Created = now;
                }
            }
        }
    }
}