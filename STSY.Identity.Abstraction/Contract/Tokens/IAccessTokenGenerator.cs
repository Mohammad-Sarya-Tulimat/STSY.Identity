using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Tokens
{
    public interface IAccessTokenGenerator
    {
        Task<TokenData> GenerateAccessToken(ExtendedUser userData, List<Claim> claim = null);
        Task<TokenData> GenerateMFAToken(ExtendedUser userData);
    }
}
