using STSY.Identity.Abstraction.Models.Output.Tokens;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Tokens
{
    public interface IMFTokenGenerator
    {
        Task<TokenData> GenerateMFAToken(string id, string idType);
        Task<STSYTokenValidationResult> ValidateMFAToken(string token);
    }
}
