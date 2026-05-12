using STSY.Identity.Abstraction.Models.Output.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Tokens
{
    public interface IAccessTokenGenerator
    {
        Task<TokenData> GenerateAccessToken(string id, string idType, List<Claim> claim);

        Task<TokenValidationResult> ValidateAccessToken(string token);
    }
}
