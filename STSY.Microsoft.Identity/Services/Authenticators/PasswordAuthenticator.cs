using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Enums;
using STSY.Identity.Models;
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
        public AuthenticatorUsage Usage => AuthenticatorUsage.Primary;

        public async Task<bool> ValidateCredentialAsync(UserData userData, Dictionary<string, object> credentials)
        {
            if (userData == null) throw new ArgumentNullException(nameof(userData), "user data cannot be null");
            if (credentials == null || !credentials.ContainsKey(CredentialKeys.PASSWORD_KEY)) throw new ArgumentNullException(nameof(credentials), "password required");
            var user = await _userManager.FindByIdAsync(userData.Id);
            if (user == null) return false;
            return await _userManager.CheckPasswordAsync(user, credentials[CredentialKeys.PASSWORD_KEY].ToString());
        }
    }
}
