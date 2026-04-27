using STSY.Identity.Abstraction.Models.UserModel;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Tokens
{
    public interface IRefreshTokenGenerator
    {
        Task<string> GenerateRefreshToken(ExtendedUser userData);
    }
}
