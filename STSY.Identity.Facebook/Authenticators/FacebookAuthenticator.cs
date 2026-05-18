using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Input.account;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Identity.Facebook.Authenticators
{


    public class FacebookAuthenticator : IAuthenticator, IExternalAccountCreator
    {
        public const string FacebookCredentialToken = "FacebookToken";
        IReadUsers _readUsers;
        IUserManager _userManager;
        STSYFacebookIdentityOption _option;
        public FacebookAuthenticator(IReadUsers readUsers, IUserManager userManager, STSYFacebookIdentityOption sTSYFacebookIdentityOption)
        {
            _readUsers = readUsers;
            _userManager = userManager;
            _option = sTSYFacebookIdentityOption;
        }
        public string CredentialType => Constant.FacebookProviderType;

        public bool AllowStepUp => false;

        public string Provider => Constant.FacebookProviderType;

        public async Task<AuthenticatorResult> ValidateCredentialAsync(Dictionary<string, object> credentials)
        {
            if (credentials.TryGetValue(FacebookCredentialToken, out var clienttoken))
            {
                var facebookUserToken = clienttoken.ToString();
                var faceBookValidator = new FacebookAuthProvider(_option);
                var profile = await faceBookValidator.ValidateAsync(facebookUserToken);
                if (profile == null) throw new STSYIdentityException("Invalid credentials");
                var user = await _readUsers.GetUserByLinkedAccountsIdAsync(Constant.FacebookProviderType, profile.Id);
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
            throw new STSYIdentityException("Invalid credentials");
        }

        public async Task<UserData> CreateAccount(ExternalAccountCreateInput externalAccountCreateInput)
        {
            if (externalAccountCreateInput?.Credentials != null && externalAccountCreateInput.Credentials.TryGetValue(FacebookCredentialToken, out var clienttoken))
            {
                var facebookUserToken = clienttoken.ToString();
                var faceBookValidator = new FacebookAuthProvider(_option);
                var profile = await faceBookValidator.ValidateAsync(facebookUserToken);
                if (profile == null || string.IsNullOrEmpty(profile.Email)) throw new STSYIdentityException("Invalid credentials");
                var user = await _readUsers.GetUserByLinkedAccountsIdAsync(Constant.FacebookProviderType, profile.Id);
                if (user == null) await _readUsers.GetUserByUserNameOrEmailAsync(profile.Email);
                if (user != null) throw new STSYIdentityException("there is account linked to provider email");
                return await _userManager.CreateUser(new ExternalUserCreate
                {
                    Email = profile.Email,
                    FirstName = profile.FirstName,
                    LastName = profile.LastName,
                    UserName = profile.Email,
                    Provider = Constant.FacebookProviderType,
                    ProviderId = profile.Id,
                    EmailVerified = profile.Verified ?? false
                });

            }
            throw new STSYIdentityException("Invalid credentials");
        }
    }
}