using STSY.Identity.Abstraction.Models.Output;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IReadUsers
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<ExtendedUser> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserPassKey>> GetUserPassKeyAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserSession>> GetUserSessionsAsync(string userId, CancellationToken cancellationToken = default);

    }
}
