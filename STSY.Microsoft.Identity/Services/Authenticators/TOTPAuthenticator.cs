using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Abstraction.Models.Enums;
using STSY.Identity.Models;
using STSY.Microsoft.Identity.Mappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services.Authenticators
{
    public class TOTPAuthenticator : IAuthenticator
    {
        public AuthenticatorUsage Usage => AuthenticatorUsage.MultiFactor;

        public const string CredentialTypeValue = "TOtp";
        public string CredentialType => CredentialTypeValue;

        UserManager<MicrosoftIdentityUser> _userManager;
        ISessionManager _sessionManager;
        public TOTPAuthenticator(UserManager<MicrosoftIdentityUser> userManager, ISessionManager sessionManager)
        {
            _userManager = userManager;
            _sessionManager = sessionManager;
        }
        public async Task<AuthenticatorResult> ValidateCredentialAsync(Dictionary<string, object> credentials)
        {
            var validation = await _sessionManager.ValidateMFSessionAsync(credentials);
            if (!validation.Success) throw new AuthenticatorException("Invalid or expired session.");
            var appUser = await _userManager.FindByIdAsync(validation.UserId);
            if (credentials == null || !credentials.ContainsKey(CredentialKeys.OTP_KEY)) throw new AuthenticatorException("OTP is required.");
            if (!await _userManager.GetTwoFactorEnabledAsync(appUser)) throw new AuthenticatorException("Two factor authentication is not enabled for this user.");
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(appUser, TokenOptions.DefaultAuthenticatorProvider, credentials[CredentialKeys.OTP_KEY].ToString());
            return new AuthenticatorResult
            {
                Success = isValid,
                User = appUser.ToUserData()
            };
        }
    }
}
