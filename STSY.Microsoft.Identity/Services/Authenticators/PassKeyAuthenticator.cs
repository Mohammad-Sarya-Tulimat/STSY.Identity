using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Models.Enums;
using STSY.Identity.Abstraction.Models.Output.Auth;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using STSY.Identity.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace STSY.Microsoft.Identity.Services.Authenticators
{
    public class PassKeyAuthenticator : IAuthenticator, IChallengeAuthenticator, IPassKeyManager
    {
        public CredentialType CredentialType => CredentialType.PassKey;
        public AuthenticatorUsage Usage => AuthenticatorUsage.Primary | AuthenticatorUsage.MultiFactor;

        UserManager<MicrosoftIdentityUser> _userManager;
        SignInManager<MicrosoftIdentityUser> _signInManager;

        public PassKeyAuthenticator(UserManager<MicrosoftIdentityUser> userManager, SignInManager<MicrosoftIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<bool> ValidateCredentialAsync(UserData userData, Dictionary<string, object> credentials)
        {
            if (userData == null) throw new ArgumentNullException(nameof(userData), "user data cannot be null");
            if (credentials == null || !credentials.ContainsKey(CredentialKeys.PASSWORD_KEY)) throw new AuthenticatorException("credentials is required.");
            var userKey = await _signInManager.PerformPasskeyAssertionAsync(credentials[CredentialKeys.PASSKEY_KEY].ToString());
            if (userKey.Succeeded)
            {
                if (userData.Id == userData.Id)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<AuthInitiateResult> InitiateAsync(UserData user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var token = await _signInManager.MakePasskeyRequestOptionsAsync(appUser);
            return new AuthInitiateResult
            {
                IsSuccess = true,
                Data = new Dictionary<string, object>
                {
                    { "options",token }
                }
            };
        }
        public async Task<string> GeneratePassKeyCreation(UserData user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var passkeyCreationObject = new PasskeyUserEntity() { DisplayName = $"{appUser.FirstName},{appUser.LastName}", Id = appUser.Id, Name = appUser.UserName };
            return await _signInManager.MakePasskeyCreationOptionsAsync(passkeyCreationObject);
        }
        public async Task<bool> ValidatePassKey(string credential)
        {
            var result = await _signInManager.PerformPasskeyAttestationAsync(credential);

            return result.Succeeded;
        }
        public async Task<bool> RemovePassKey(UserData user, byte[] id)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var result = await _userManager.RemovePasskeyAsync(appUser, id);
            return result.Succeeded;
        }
    }
}
