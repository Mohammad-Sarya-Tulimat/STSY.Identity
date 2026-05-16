using STSY.Identity.Abstraction.Contract.Models.Sessions;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Output;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Authentication
{
    public interface ISessionManager
    {
        Task<SessionResult> CreateSessionAsync(UserData user, CancellationToken cancellationToken = default);
        Task<SessionResult> RefreshSessionAsync(Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default);
        Task<SessionResult> CreateMFSessionAsync(UserData user, CancellationToken cancellationToken = default);


        Task<SessionValidateResult> ValidateSessionAsync(Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default);
        Task<SessionValidateResult> ValidateMFSessionAsync(Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default);


    }
}
