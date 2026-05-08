using STSY.Identity.Abstraction.Models.Enums;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface ISendChallengeTokens
    {
        Task<bool> SendChallengeTokensAsync(UserData user, CredentialType credentialType, string tokenOrCode);
    }
}
