using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Models;
using System.Collections.Generic;
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
        public ASPIdentitySession(IHttpContextAccessor httpContextAccessor, SignInManager<MicrosoftIdentityUser> signInManager, UserManager<MicrosoftIdentityUser> userManager)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._signInManager = signInManager;
            this._userManager = userManager;
        }

        public async Task<SessionResult> CreateMFSessionAsync(ExtendedUser user, CancellationToken cancellationToken = default)
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]{
            new Claim(ClaimTypes.NameIdentifier, user.Id)}, IdentityConstants.TwoFactorUserIdScheme));
            await _httpContextAccessor.HttpContext.SignInAsync(IdentityConstants.TwoFactorUserIdScheme, principal);
            Dictionary<string, object> result = new Dictionary<string, object>();
            return new SessionResult
            {
                isSuccess = true,
                IsMfRequred = true,
                Message = "Require Mf",
                SessionData = result
            };
        }

        public async Task<bool> ValidateMFSessionAsync(ExtendedUser user, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var result = await _httpContextAccessor.HttpContext.AuthenticateAsync(IdentityConstants.TwoFactorUserIdScheme);
            if (result.Succeeded)
            {
                var userId = result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.Equals(userId, user.Id))
                {
                    return true;
                }
            }
            return false;
        }
        public async Task<SessionResult> CreateSessionAsync(ExtendedUser user, CancellationToken cancellationToken = default)
        {
            await _httpContextAccessor.HttpContext.SignOutAsync(IdentityConstants.TwoFactorUserIdScheme);
            var dbuser = await _userManager.FindByIdAsync(user.Id);
            if (await _signInManager.CanSignInAsync(dbuser))
            {
                await _signInManager.SignInAsync(dbuser, true);
                return new SessionResult
                {
                    isSuccess = true,
                    IsMfRequred = false,
                    Message = "Singin done"
                };
            }
            throw new AuthenticatorException("cannot Signin");
        }

        public async Task<SessionResult> RefreshSessionAsync(ExtendedUser user, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var dbuser = await _userManager.FindByIdAsync(user.Id);
            await _signInManager.RefreshSignInAsync(dbuser);
            return new SessionResult
            {
                isSuccess = true,
                Message = "Session refreshed",
                IsMfRequred = false
            };
        }

        public async Task<bool> ValidateSessionAsync(ExtendedUser user, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var claimuser = _httpContextAccessor.HttpContext?.User;
            return _signInManager.IsSignedIn(claimuser) && string.Equals(claimuser.Identity.Name, user.UserName, System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
