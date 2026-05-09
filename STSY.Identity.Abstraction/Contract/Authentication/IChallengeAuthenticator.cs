using STSY.Identity.Abstraction.Models.Output.Auth;
using STSY.Identity.Abstraction.Models.Output.UserModels;
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
