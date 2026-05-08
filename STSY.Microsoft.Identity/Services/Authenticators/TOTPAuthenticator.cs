using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Models.Enums;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using STSY.Identity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services.Authenticators
{
    public class TOTPAuthenticator : IAuthenticator
    {
        public AuthenticatorUsage Usage => AuthenticatorUsage.MultiFactor;
        public CredentialType CredentialType => CredentialType.TOtp;

        UserManager<MicrosoftIdentityUser> _userManager;
        public TOTPAuthenticator(UserManager<MicrosoftIdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<bool> ValidateCredentialAsync(UserData userData, Dictionary<string, object> credentials)
        {
            var appUser = await _userManager.FindByIdAsync(userData.Id);
            if (credentials == null || !credentials.ContainsKey(CredentialKeys.OTP_KEY)) throw new AuthenticatorException("OTP is required.");

            if (!await _userManager.GetTwoFactorEnabledAsync(appUser)) throw new AuthenticatorException("Two factor authentication is not enabled for this user.");
            return await _userManager.VerifyTwoFactorTokenAsync(appUser, TokenOptions.DefaultAuthenticatorProvider, credentials[CredentialKeys.OTP_KEY].ToString());
        }
    }
}
