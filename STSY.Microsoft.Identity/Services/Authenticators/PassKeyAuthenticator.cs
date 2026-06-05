using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Input.account;
using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Abstraction.Models.Output.Auth;
using STSY.Identity.Abstraction.Service;
using STSY.Identity.Models;
using STSY.Microsoft.Identity.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace STSY.Microsoft.Identity.Services.Authenticators
{
    public class PassKeyAuthenticator : IMFAuthenticator, IAuthenticator, IChallengeAuthenticator, IPassKeyManager
    {
        public const string CredentialTypeValue = "PassKey";
        public string CredentialType => CredentialTypeValue;
        public bool AllowStepUp => true;
        UserManager<MicrosoftIdentityUser> _userManager;
        SignInManager<MicrosoftIdentityUser> _signInManager;
        public PassKeyAuthenticator(UserManager<MicrosoftIdentityUser> userManager, SignInManager<MicrosoftIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<AuthenticatorResult> ValidateCredentialAsync(Dictionary<string, object> credentials)
        {
            if (credentials == null || !credentials.ContainsKey(CredentialKeys.PASSKEY_KEY)) throw new AuthenticatorException("credentials is required.");
            var userKey = await _signInManager.PerformPasskeyAssertionAsync(credentials[CredentialKeys.PASSKEY_KEY].ToString());
            return new AuthenticatorResult
            {
                Success = userKey.Succeeded,
                User = userKey.User.ToUserData(),
            };
        }

        public async Task<AuthenticatorResult> ValidateCredentialAsync(string userId, Dictionary<string, object> credentials)
        {
            var result = await this.ValidateCredentialAsync(credentials);
            result.Success = result.Success && string.Equals(result.User.Id, userId);
            return result;
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
        public async Task<string> GeneratePassKeyCreationOptionsAsync(UserData user)
        {
            try
            {
                if (user == null) throw new ArgumentNullException(nameof(user), "user data cannot be null");
                var appUser = await _userManager.FindByIdAsync(user.Id);
                if (appUser == null) throw new ResourceNotFoundException(nameof(UserData), user.Id, "User not found.");
                var passkeyCreationObject = new PasskeyUserEntity() { DisplayName = $"{appUser.FirstName},{appUser.LastName}", Id = appUser.Id, Name = appUser.UserName };
                return await _signInManager.MakePasskeyCreationOptionsAsync(passkeyCreationObject);
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        public async Task<STSYIdentityResult> PasskeyAttestationAsync(PassKeyAttestation attestation)
        {
            try
            {
                if (attestation == null) throw new ArgumentNullException(nameof(attestation), "credential cannot be null");
                var result = await _signInManager.PerformPasskeyAttestationAsync(attestation.Credential);
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByIdAsync(result.UserEntity.Id);
                    result.Passkey.Name = result.Passkey.Name ?? attestation.Name;
                    await _userManager.AddOrUpdatePasskeyAsync(user, result.Passkey);
                }
                return new STSYIdentityResult
                {
                    Success = result.Succeeded,
                    Message = result?.Failure?.Message
                };
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex)
            {
                throw new STSYIdentityException(ex.Message, ex);
            }
        }
        public async Task<STSYIdentityResult> RemovePassKey(UserData user, string id)
        {
            try
            {
                if (user == null) throw new ArgumentNullException(nameof(user), "user data cannot be null");
                if (id == null) throw new ArgumentNullException(nameof(id), "id cannot be null");
                var credId = id.Base64UrlToByte();
                var appUser = await _userManager.FindByIdAsync(user.Id);
                if (appUser == null) throw new ResourceNotFoundException(nameof(UserData), user.Id, "User not found.");
                var result = await _userManager.RemovePasskeyAsync(appUser, credId);
                return result.AsSTSYIdentityResult();
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }

        public async Task<List<UserPassKey>> ListPassKey(UserData user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user), "user data cannot be null");
            var appUser = await _userManager.FindByIdAsync(user.Id);
            var keys = await _userManager.GetPasskeysAsync(appUser);
            return keys.Select(key => key.ToUserPassKey()).ToList();
        }
    }
}
