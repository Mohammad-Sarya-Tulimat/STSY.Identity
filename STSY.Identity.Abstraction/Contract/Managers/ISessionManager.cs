using STSY.Identity.Abstraction.Models.Output;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Managers
{
    public interface ISessionManager
    {
        Task<List<UserSession>> ListSession(string userId, CancellationToken cancellationToken);
        Task<STSYIdentityResult> AddSession(string userId, UserSession session, string token, CancellationToken cancellationToken);
        Task<STSYIdentityResult> UpdateSession(string userId, UserSession session, string token, CancellationToken cancellationToken);
        Task<STSYIdentityResult> RemoveSession(string userId, string sessionId, CancellationToken cancellationToken);
        Task<STSYIdentityResult> ValidateSession(string userId, string sessionId, string token, CancellationToken cancellationToken);
    }
}
