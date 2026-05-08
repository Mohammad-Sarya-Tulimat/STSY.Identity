using STSY.Identity.Abstraction.Models.Enums;
using STSY.Identity.Abstraction.Models.Output.Auth;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Authentication
{
    public interface IChallengeAuthenticator
    {

        CredentialType CredentialType { get; }
        Task<AuthInitiateResult> InitiateAsync(
            UserData user);
    }
}
