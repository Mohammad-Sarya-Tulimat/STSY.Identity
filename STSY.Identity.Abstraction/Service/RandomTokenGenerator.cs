using STSY.Identity.Abstraction.Contract.Tokens;
using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using STSY.Identity.Abstraction.Options;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Service
{
    public class RandomTokenGenerator : IRefreshTokenGenerator
    {
        private readonly RandomRefreshTokenOption _randomRefreshTokenOption;
        public RandomTokenGenerator(RandomRefreshTokenOption randomRefreshTokenOption)
        {
            _randomRefreshTokenOption = randomRefreshTokenOption;
        }
        public async Task<TokenData> GenerateRefreshToken(ExtendedUser userData)
        {
            using (var rng = RandomNumberGenerator.Create())
            {

                StringBuilder builder = new StringBuilder();
                var buffer = new byte[_randomRefreshTokenOption.RefreshTokenSize];
                rng.GetBytes(buffer);
                builder
                    .Append(Guid.NewGuid().ToString().Replace("-", ""))
                    .Append(Convert.ToBase64String(buffer))
                    .Append(Guid.NewGuid().ToString().Replace("-", ""))
                    ;
                var token = builder.ToString();
                return new TokenData
                {
                    Token = token,
                    TokenType = "Refresh",
                    Expiration = DateTimeOffset.UtcNow.AddHours(_randomRefreshTokenOption.ExpireHours)
                };
            }
        }
    }
}
