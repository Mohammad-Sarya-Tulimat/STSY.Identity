using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Output.Auth;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Authentication
{
    public interface IChallengeAuthenticator
    {

        string CredentialType { get; }
        Task<AuthInitiateResult> InitiateAsync(
            UserData user);
    }
}
