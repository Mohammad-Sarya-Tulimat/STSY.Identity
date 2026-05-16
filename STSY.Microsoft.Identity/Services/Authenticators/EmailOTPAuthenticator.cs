using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Models;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Enums;
using STSY.Identity.Abstraction.Models.Output.Auth;
using STSY.Identity.Models;
using STSY.Microsoft.Identity.Mappers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services.Authenticators
{
    public class EmailOTPAuthenticator : IChallengeAuthenticator
    {

        public const string CredentialTypeValue = "EmailOtp";
        public string CredentialType => CredentialTypeValue;

        UserManager<MicrosoftIdentityUser> _userManager;
        ISendChallengeTokens _sendChallengeTokens;
        ISessionManager _sessionManager;
        public EmailOTPAuthenticator(UserManager<MicrosoftIdentityUser> userManager, ISendChallengeTokens sendChallengeTokens, ISessionManager sessionManager)
        {
            _userManager = userManager;
            _sendChallengeTokens = sendChallengeTokens;
            _sessionManager = sessionManager;
        }
        public async Task<AuthInitiateResult> InitiateAsync(UserData user)
        {
            var appUser = await _userManager.FindByIdAsync(user.Id);
            if (!appUser.EmailConfirmed) throw new AuthenticatorException("Email is not confirmed for this user.");
            var token = await _userManager.GenerateTwoFactorTokenAsync(appUser, TokenOptions.DefaultEmailProvider);
            await _sendChallengeTokens.SendChallengeTokensAsync(user, ChallengeTypeToSend.EmailOtp, token);
            return new AuthInitiateResult
            {
                IsSuccess = true,
                Data = new Dictionary<string, object>
                {
                    { "Message", "OTP has been sent to your registered email address." }
                }
            };
        }
        public async Task<AuthenticatorResult> ValidateCredentialAsync(Dictionary<string, object> credentials)
        {
            var validation = await _sessionManager.ValidateMFSessionAsync(credentials);
            if (!validation.Success) throw new AuthenticatorException("Invalid or expired session.");
            var appUser = await _userManager.FindByIdAsync(validation.UserId);
            if (!appUser.EmailConfirmed) throw new AuthenticatorException("Email is not confirmed for this user.");
            if (credentials == null || !credentials.ContainsKey(CredentialKeys.OTP_KEY)) throw new AuthenticatorException("OTP is required.");
            if (!await _userManager.GetTwoFactorEnabledAsync(appUser)) throw new AuthenticatorException("Two factor authentication is not enabled for this user.");
            var isValid = await _userManager.VerifyTwoFactorTokenAsync(appUser, TokenOptions.DefaultEmailProvider, credentials[CredentialKeys.OTP_KEY].ToString());
            return new AuthenticatorResult
            {
                Success = isValid,
                User = appUser.ToUserData()
            };
        }
    }
}
