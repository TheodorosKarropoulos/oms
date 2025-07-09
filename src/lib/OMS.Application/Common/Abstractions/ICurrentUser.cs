using OMS.Domain.Constants;

namespace OMS.Application.Common.Abstractions;

public interface ICurrentUser
{
    string UserId { get; }
    
    IReadOnlyCollection<string> Roles { get; }
    
    bool IsInRole(string role) => Roles.Contains(role);

    bool IsAdminRole => Roles.Contains(UserRoles.Admin);
    
    bool IsStaffRole => Roles.Contains(UserRoles.Staff);
    
    bool IsCustomerRole => Roles.Contains(UserRoles.Customer);
    
    bool IsCourierRole => Roles.Contains(UserRoles.DeliveryStaff);
    
    Guid UserIdGuid => Guid.TryParse(UserId, out var id) ? id : Guid.Empty;
}