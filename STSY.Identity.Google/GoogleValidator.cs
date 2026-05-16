using Google.Apis.Auth;
using STSY.Identity.Abstraction.Contract.Exeptions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Identity.Google
{
    public class GoogleValidator
    {
        public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string idToken, List<string> Audience)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = Audience
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                idToken,
                settings);
            if (!payload.EmailVerified)
                throw new AuthenticatorException("google account you are using is not verified");
            return payload;
        }
    }
}
