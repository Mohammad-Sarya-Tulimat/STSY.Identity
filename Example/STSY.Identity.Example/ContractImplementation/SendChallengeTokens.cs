using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Enums;

namespace STSY.Identity.Example.ContractImplementation
{
    public class SendChallengeTokens : ISendChallengeTokens
    {
        public Task<bool> SendChallengeTokensAsync(UserData user, ChallengeTypeToSend credentialType, string tokenOrCode)
        {
            return Task.FromResult(true);
        }
    }
}
