using STSY.Identity.Abstraction.Models.Output.UserModels;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Authentication
{
    public interface ISessionManager
    {
        Task<Dictionary<string, object>> CreateSessionAsync(ExtendedUser user, string sessionId, CancellationToken cancellationToken = default);
        Task<Dictionary<string, object>> RefreshSessionAsync(ExtendedUser user, string sessionId, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default);
        Task<bool> ValidateSessionAsync(ExtendedUser user, string sessionId, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default);

    }
}
