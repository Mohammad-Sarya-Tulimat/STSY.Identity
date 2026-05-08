using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Models.Input;
using STSY.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services
{
    public class MicrosoftIdentityUserManager : IUserManager, IPasswordManager, ITowFactorManager
    {
        UserManager<MicrosoftIdentityUser> _userManager;
        public MicrosoftIdentityUserManager(UserManager<MicrosoftIdentityUser> userManager)
        {
            _userManager = userManager;
        }
        #region IUserManager
        public async Task AddRole(string userId, string role, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.AddToRoleAsync(user, role);
        }
        public async Task CreateUser(UserCreateInput input, CancellationToken cancellationToken)
        {
            var user = new MicrosoftIdentityUser
            {
                UserName = input.UserName,
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName,
                PhoneNumber = input.PhoneNumber
            };

            await _userManager.CreateAsync(user, input.Password);
        }
        #endregion
        #region IPasswordManager
        public async Task ChangeUserPassword(string userId, string newpassword, string oldpassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.ChangePasswordAsync(user, oldpassword, newpassword);
        }
        public async Task<string> GeneratePasswordResetTokenAsync(string userId, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task ResetPassword(string userId, string resetToken, string newPassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
        }
        #endregion 
        #region ITowFactorManager
        public async Task SetTwoFactorEnabled(string userId, bool enabled, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (!enabled)
                await _userManager.SetTwoFactorEnabledAsync(user, enabled);
            else
            {
                bool hasEmail = await _userManager.IsEmailConfirmedAsync(user);
                bool hasPhone = await _userManager.IsPhoneNumberConfirmedAsync(user);
                bool hasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null;
                bool hasCode = (await _userManager.CountRecoveryCodesAsync(user)) != 0;
                if (hasEmail || hasPhone || hasAuthenticator || hasCode)
                {
                    await _userManager.SetTwoFactorEnabledAsync(user, enabled);
                }
                else
                {
                    throw new InvalidOperationException("No 2FA method available for user.");
                }
            }
        }

        public async Task<List<string>> GenerateNewRecoveryCode(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 6);
            return recoveryCodes.ToList();
        }
        public async Task<string> ReGenerateTOTKey(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            return await _userManager.GetAuthenticatorKeyAsync(user);
        }
        public async Task<bool> ValidateTOTKey(string userId, string code, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);
        }
        #endregion
    }
}
