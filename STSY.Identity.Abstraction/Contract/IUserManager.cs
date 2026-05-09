using STSY.Identity.Abstraction.Models.Input;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IUserManager
    {
        Task CreateUser(UserCreateInput input, CancellationToken cancellationToken);
        Task AddRole(string userId, string role, CancellationToken cancellationToken);
        Task<bool> IsSecurityChangesAllowed(string userId, string sessionId, CancellationToken cancellationToken);
        Task EnableSecurityChanges(string userId, string sessionId, DateTimeOffset expiration, CancellationToken cancellationToken);
        Task DisableSecurityChanges(string userId, CancellationToken cancellationToken);

    }
}
