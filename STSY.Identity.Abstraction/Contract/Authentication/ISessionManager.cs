using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Output;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Authentication
{
    public interface ISessionManager
    {
        Task<SessionResult> CreateSessionAsync(ExtendedUser user, CancellationToken cancellationToken = default);
        Task<SessionResult> RefreshSessionAsync(ExtendedUser user, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default);
        Task<bool> ValidateSessionAsync(ExtendedUser user, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default);
        Task<SessionResult> CreateMFSessionAsync(ExtendedUser user, CancellationToken cancellationToken = default);
        Task<bool> ValidateMFSessionAsync(ExtendedUser user, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default);



    }
}
