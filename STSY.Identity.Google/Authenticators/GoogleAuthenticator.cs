using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Abstraction.Models.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Identity.Google.Authenticators
{
    public class GoogleAuthenticator : IAuthenticator
    {
        public const string GoogleCredentialType = "GoogleIdToken";
        STSYGoogleIdentityOption _options;
        IReadUsers _readUsers;
        public GoogleAuthenticator(STSYGoogleIdentityOption options, IReadUsers readUsers)
        {
            _options = options;
            _readUsers = readUsers;
        }
        public AuthenticatorUsage Usage => AuthenticatorUsage.Primary;
        public string CredentialType => "Google";
        public async Task<AuthenticatorResult> ValidateCredentialAsync(Dictionary<string, object> credentials)
        {
            var auth = new GoogleValidator();
            var result = await auth.VerifyGoogleToken(credentials[GoogleCredentialType].ToString(), _options.Audience);
            var user = await _readUsers.GetUserByLinkedAccountsIdAsync(Constant.GoogleProviderType, result.Subject);
            if (user != null)
            {
                return new AuthenticatorResult
                {
                    Success = true,
                    User = user
                };

            }
            else
            {
                return new AuthenticatorResult
                {
                    Success = false,
                    User = null
                };
            }
        }
    }
}