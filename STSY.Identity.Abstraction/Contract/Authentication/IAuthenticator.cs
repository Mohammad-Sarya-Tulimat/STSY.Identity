using STSY.Identity.Abstraction.Contract.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Authentication
{
    public interface IAuthenticator
    {
        string CredentialType { get; }
        bool AllowStepUp { get; }
        Task<AuthenticatorResult> ValidateCredentialAsync(Dictionary<string, object> credentials);
    }
    public interface IMFAuthenticator
    {
        string CredentialType { get; }
        Task<AuthenticatorResult> ValidateCredentialAsync(string userId, Dictionary<string, object> credentials);
    }
}
