using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Abstraction.Models.Input.account;
using STSY.Identity.Abstraction.Models.Output;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Identity.Google.Authenticators
{
    public class GoogleAuthenticator : IAuthenticator
    {
        public const string GoogleCredentialType = "GoogleIdToken";
        STSYGoogleIdentityOption _options;
        IReadUsers _readUsers;
        IUserManager _userManager;
        public GoogleAuthenticator(STSYGoogleIdentityOption options, IReadUsers readUsers, IUserManager userManager)
        {
            _options = options;
            _readUsers = readUsers;
            _userManager = userManager;
        }
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
                    User = user,
                    NeedMfactor = true,
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

        public async Task<STSYIdentityResult> CreateUser(Dictionary<string, object> credentials)
        {
            var auth = new GoogleValidator();
            var result = await auth.VerifyGoogleToken(credentials[GoogleCredentialType].ToString(), _options.Audience);
            var user = await _readUsers.GetUserByLinkedAccountsIdAsync(Constant.GoogleProviderType, result.Subject);
            if (user == null) await _readUsers.GetUserByUserNameOrEmailAsync(result.Email);
            if (user != null) throw new STSYIdentityException("there is account linked");
            return await _userManager.CreateUser(new ExternalUserCreateInput
            {
                Email = result.Email,
                FirstName = result.GivenName,
                LastName = result.FamilyName,
                UserName = result.Email,
                Provider = Constant.GoogleProviderType,
                ProviderId = result.Subject,
                EmailVerified = result.EmailVerified,
            });
        }
    }
}