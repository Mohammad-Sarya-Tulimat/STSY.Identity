using STSY.Identity.Abstraction.Contract.Tokens;
using STSY.Identity.Abstraction.Models.UserModel;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Service
{
    public class RandomTokenGenerator : IRefreshTokenGenerator
    {
        public async Task<string> GenerateRefreshToken(ExtendedUser userData)
        {
            using (var rng = RandomNumberGenerator.Create())
            {

                StringBuilder builder = new StringBuilder();
                var buffer = new byte[64];
                rng.GetBytes(buffer);
                builder
                    .Append(Guid.NewGuid().ToString().Replace("-", ""))
                    .Append(Convert.ToBase64String(buffer))
                    .Append(Guid.NewGuid().ToString().Replace("-", ""))
                    ;
                var token = builder.ToString();
                return token;
            }
        }
    }
}
