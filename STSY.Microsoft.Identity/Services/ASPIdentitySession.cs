using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Models.Sessions;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Contract.Tokens;
using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Models;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services
{
    public class ASPIdentitySession : ISessionManager
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SignInManager<MicrosoftIdentityUser> _signInManager;
        private readonly UserManager<MicrosoftIdentityUser> _userManager;
        private readonly IGetUserClaims _generateUserClaims;
        public ASPIdentitySession(
            IHttpContextAccessor httpContextAccessor,
            SignInManager<MicrosoftIdentityUser> signInManager,
            UserManager<MicrosoftIdentityUser> userManager,
            IGetUserClaims generateUserClaims)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._signInManager = signInManager;
            this._userManager = userManager;
            this._generateUserClaims = generateUserClaims;
        }

        public async Task<SessionResult> CreateMFSessionAsync(UserData user, CancellationToken cancellationToken = default)
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]{
            new Claim(ClaimTypes.NameIdentifier, user.Id)}, IdentityConstants.TwoFactorUserIdScheme));
            await _httpContextAccessor.HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, principal);
            Dictionary<string, object> result = new Dictionary<string, object>();
            return new SessionResult
            {
                IsSuccess = true,
                IsMFARequired = true,
                Message = "Require Mf",
                SessionData = result
            };
        }

        public async Task<SessionValidateResult> ValidateMFSessionAsync(Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var authResult = await _httpContextAccessor.HttpContext.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);
            var result = new SessionValidateResult
            {
                Success = authResult.Succeeded
            };
            if (authResult.Succeeded)
            {
                result.UserId = authResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            return result;
        }
        public async Task<SessionResult> CreateSessionAsync(UserData user, CancellationToken cancellationToken = default)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
            var dbuser = await _userManager.FindByIdAsync(user.Id);
            if (await _signInManager.CanSignInAsync(dbuser))
            {
                return await this.GetNewSessionResult(dbuser, System.Guid.NewGuid().ToString(), cancellationToken);
            }
            throw new AuthenticatorException("cannot Signin");
        }
        public async Task<SessionResult> RefreshSessionAsync(Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var validateResult = await ValidateSessionAsync(dataToValidate, cancellationToken);
            if (validateResult.Success)
            {
                var user = await _userManager.FindByIdAsync(validateResult.UserId);
                await _signInManager.SignOutAsync();
                return await this.GetNewSessionResult(user, validateResult.SessionId, cancellationToken);
            }
            throw new AuthenticatorException("Invalid session");
        }

        public async Task<SessionValidateResult> ValidateSessionAsync(Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var claimuser = _httpContextAccessor.HttpContext?.User;
            var isSignedIn = _signInManager.IsSignedIn(claimuser);
            var result = new SessionValidateResult
            {
                Success = isSignedIn,
            };
            if (isSignedIn)
            {
                result.UserId = claimuser.FindFirstValue(ClaimTypes.NameIdentifier);
                result.SessionId = claimuser.FindFirstValue(ClaimTypes.Sid);
            }
            return result;
        }
        private async Task<SessionResult> GetNewSessionResult(MicrosoftIdentityUser user, string sessionId, CancellationToken cancellationToken = default)
        {
            var cliems = (await this._generateUserClaims.GetUserClaimsAsync(user.Id, cancellationToken)).ToList();
            cliems.Add(new Claim(ClaimTypes.Sid, sessionId));
            cliems.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            await _signInManager.SignInWithClaimsAsync(user, true, cliems);
            return new SessionResult
            {
                IsSuccess = true,
                IsMFARequired = false,
                Message = "Singin done"
            };
        }
    }
}
