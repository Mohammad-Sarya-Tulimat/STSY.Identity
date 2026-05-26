using STSY.Identity.Abstraction.Contract.Models.Sessions;
using STSY.Identity.Abstraction.Models.Output;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Managers
{
    public interface ISessionStorage
    {
        Task<IEnumerable<UserSession>> ListSession(string userId, CancellationToken cancellationToken);
        Task<UserSession> GetSession(string seassionId, CancellationToken cancellationToken);
        Task<IDictionary<string, object>> GetSessionProtectedData(string seassionId, CancellationToken cancellationToken);
        Task<STSYIdentityResult> AddSession(string userId, UserSession session, IDictionary<string, object> sessionProtectionData, CancellationToken cancellationToken);
        Task<STSYIdentityResult> UpdateSession(string userId, UserSession session, IDictionary<string, object> sessionProtectionData, CancellationToken cancellationToken);
        Task<STSYIdentityResult> RemoveSession(string sessionId, CancellationToken cancellationToken = default);
    }
}
