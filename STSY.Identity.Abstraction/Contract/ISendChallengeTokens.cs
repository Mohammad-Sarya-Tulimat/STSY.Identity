using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Enums;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface ISendChallengeTokens
    {
        Task<bool> SendChallengeTokensAsync(UserData user, ChallengeTypeToSend credentialType, string tokenOrCode);
    }
}
