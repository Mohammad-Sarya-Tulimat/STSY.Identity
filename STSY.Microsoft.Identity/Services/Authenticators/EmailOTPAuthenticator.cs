using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Models.Enums;
using STSY.Identity.Abstraction.Models.Output.Auth;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using STSY.Identity.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services.Authenticators
{
    public class EmailOTPAuthenticator : IAuthenticator, IChallengeAuthenticator
    {

        public AuthenticatorUsage Usage => AuthenticatorUsage.MultiFactor;

        public CredentialType CredentialType => CredentialType.EmailOtp;

        UserManager<MicrosoftIdentityUser> _userManager;
        ISendChallengeTokens _sendChallengeTokens;
        public EmailOTPAuthenticator(UserManager<MicrosoftIdentityUser> userManager, ISendChallengeTokens sendChallengeTokens)
        {
            _userManager = userManager;
            _sendChallengeTokens = sendChallengeTokens;
        }
        public async Task<AuthInitiateResult> InitiateAsync(UserData user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            if (!appUser.EmailConfirmed) throw new AuthenticatorException("Email is not confirmed for this user.");
            var token = await _userManager.GenerateTwoFactorTokenAsync(appUser, TokenOptions.DefaultEmailProvider);
            await _sendChallengeTokens.SendChallengeTokensAsync(user, this.CredentialType, token);
            return new AuthInitiateResult
            {
                IsSuccess = true,
                Data = new Dictionary<string, object>
                {
                    { "Message", "OTP has been sent to your registered email address." }
                }
            };
        }
        public async Task<bool> ValidateCredentialAsync(UserData userData, Dictionary<string, object> credentials)
        {
            var appUser = await _userManager.FindByIdAsync(userData.Id);
            if (!appUser.EmailConfirmed) throw new AuthenticatorException("Email is not confirmed for this user.");
            if (credentials == null || !credentials.ContainsKey(CredentialKeys.OTP_KEY)) throw new AuthenticatorException("OTP is required.");
            if (!await _userManager.GetTwoFactorEnabledAsync(appUser)) throw new AuthenticatorException("Two factor authentication is not enabled for this user.");
            return await _userManager.VerifyTwoFactorTokenAsync(appUser, TokenOptions.DefaultEmailProvider, credentials[CredentialKeys.OTP_KEY].ToString());
        }
    }
}
