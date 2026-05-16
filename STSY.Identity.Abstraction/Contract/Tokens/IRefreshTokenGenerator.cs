using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Output.Tokens;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Tokens
{
    public interface IRefreshTokenGenerator
    {
        Task<TokenData> GenerateRefreshToken(UserData userData);
    }
}
