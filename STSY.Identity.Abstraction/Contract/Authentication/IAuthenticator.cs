using STSY.Identity.Abstraction.Models.Enums;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Authentication
{

    public interface IAuthenticator
    {
        AuthenticatorUsage Usage { get; }
        CredentialType CredentialType { get; }
        Task<bool> ValidateCredentialAsync(UserData userData, Dictionary<string, object> credentials);
    }

}
