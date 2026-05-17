using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Models;
using STSY.Microsoft.Identity.Mappers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services.Authenticators
{
    public class PasswordAuthenticator : IAuthenticator
    {

        UserManager<MicrosoftIdentityUser> _userManager;
        public PasswordAuthenticator(UserManager<MicrosoftIdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public const string CredentialTypeValue = "Password";
        public string CredentialType => CredentialTypeValue;

        public bool AllowStepUp => true;

        public async Task<AuthenticatorResult> ValidateCredentialAsync(Dictionary<string, object> credentials)
        {
            if (credentials == null || !credentials.ContainsKey(CredentialKeys.PASSWORD_KEY)) throw new ArgumentNullException(nameof(credentials), "password required");
            if (credentials == null || !credentials.ContainsKey(CredentialKeys.EMAIL_OR_USERNAME_KEY)) throw new ArgumentNullException(nameof(credentials), "email required");
            var userName = credentials[CredentialKeys.EMAIL_OR_USERNAME_KEY].ToString();
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) user = await _userManager.FindByEmailAsync(userName);
            var isValid = await _userManager.CheckPasswordAsync(user, credentials[CredentialKeys.PASSWORD_KEY].ToString());
            return new AuthenticatorResult
            {
                Success = isValid,
                User = user.ToUserData(),
                NeedMfactor = true,
            };
        }
    }
}
