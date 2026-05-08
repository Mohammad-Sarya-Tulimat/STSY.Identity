using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Tokens
{
    public interface IRefreshTokenGenerator
    {
        Task<TokenData> GenerateRefreshToken(ExtendedUser userData);
    }
}
