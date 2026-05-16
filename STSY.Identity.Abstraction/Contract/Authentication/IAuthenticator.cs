using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Abstraction.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Authentication
{
    public interface IAuthenticator
    {
        AuthenticatorUsage Usage { get; }
        string CredentialType { get; }
        Task<AuthenticatorResult> ValidateCredentialAsync(Dictionary<string, object> credentials);
    }
}
