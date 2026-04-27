using STSY.Identity.Abstraction.Models;
using STSY.Identity.Abstraction.Models.UserModel;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IReadUsers
    {
        Task<IEnumerable<UserData>> GetUsersAsync();
        Task<ExtendedUser> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserPassKey>> GetUserPassKeyAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserSession>> GetUserSessionsAsync(string userId, CancellationToken cancellationToken = default);

    }
}
