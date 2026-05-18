using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Input.account;
using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Models;
using STSY.Microsoft.Identity.Mappers;
using STSY.Microsoft.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services
{
    public class MicrosoftIdentityUserManager : IUserManager, IPasswordManager, ITwoFactorManager
    {
        UserManager<MicrosoftIdentityUser> _userManager;
        public MicrosoftIdentityUserManager(UserManager<MicrosoftIdentityUser> userManager)
        {
            _userManager = userManager;
        }
        #region IUserManager
        public async Task<STSYIdentityResult> AddRole(string userId, string role, CancellationToken cancellationToken)
        {
            if (role == null) throw new ArgumentNullException(nameof(role));
            if (userId == null) throw new ArgumentNullException(nameof(userId));
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.AsSTSYIdentityResult();
        }
        public async Task<UserData> CreateUser(UserCreateInput input, CancellationToken cancellationToken = default)
        {
            try
            {
                if (input == null) throw new ArgumentNullException(nameof(input));
                var user = new MicrosoftIdentityUser
                {
                    UserName = input.UserName,
                    Email = input.Email,
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    DateOfBirth = input.DateOfBirth,
                    PhoneNumber = input.PhoneNumber,
                    LockoutEnabled = true,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                var result = await _userManager.CreateAsync(user, input.Password);
                if (result.Succeeded)
                    return user.ToUserData();
                else throw new STSYIdentityException("error while create user" + string.Join("\n", result.Errors.Select(s => s.Description)));
            }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }


        public async Task<UserData> CreateUser(ExternalUserCreate input, CancellationToken cancellationToken = default)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            var user = new MicrosoftIdentityUser
            {
                UserName = input.UserName,
                Email = input.Email,
                FirstName = input.FirstName,
                LastName = input.LastName,
                PhoneNumber = input.PhoneNumber,
                LockoutEnabled = true,
                CreatedAt = DateTimeOffset.UtcNow,
                DateOfBirth = input.DateOfBirth,
                UserExternalLogins = new List<MicrosoftIdentityUserExternalLogin>() {
                    new MicrosoftIdentityUserExternalLogin {
                        LinkedAt=DateTimeOffset.UtcNow,
                        Provider=input.Provider,
                        ProviderUserId=input.ProviderId}
              }
            };
            var result = await _userManager.CreateAsync(user, input.Password);
            if (result.Succeeded)
                return user.ToUserData();
            else throw new STSYIdentityException("error while create user" + string.Join("\n", result.Errors.Select(s => s.Description)));
        }
        public async Task<bool> IsStepUpEnabled(string userId, string sessionId, CancellationToken cancellationToken)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                if (sessionId == null) throw new ArgumentNullException(nameof(sessionId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                return user.IsStepUpEnabled(sessionId);
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        public async Task<STSYIdentityResult> EnableStepUpAsync(string userId, string sessionId, DateTimeOffset expiration, CancellationToken cancellationToken)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                user.UpdateStepUp(sessionId, expiration);
                await _userManager.UpdateAsync(user);
                return STSYIdentityResult.SuccessResult;
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        public async Task<STSYIdentityResult> DisableStepUp(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                user.RemoveStepUp();
                await _userManager.UpdateAsync(user);
                return STSYIdentityResult.SuccessResult;
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }


        public async Task<bool> IsMFAEnabled(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                return await _userManager.GetTwoFactorEnabledAsync(user);
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        public async Task<bool> IsLocked(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                return await _userManager.IsLockedOutAsync(user);
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        public async Task<STSYIdentityResult> ResetLock(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                var result = await _userManager.ResetAccessFailedCountAsync(user);
                return result.AsSTSYIdentityResult();
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }

        public async Task<STSYIdentityResult> AccessFailedAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                return (await _userManager.AccessFailedAsync(user)).AsSTSYIdentityResult();
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        #endregion
        #region IPasswordManager
        public async Task<STSYIdentityResult> ChangeUserPasswordAsync(string userId, string newpassword, string oldpassword, CancellationToken cancellationToken)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                if (newpassword == null) throw new ArgumentNullException(nameof(newpassword));
                if (oldpassword == null) throw new ArgumentNullException(nameof(oldpassword));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                var result = await _userManager.ChangePasswordAsync(user, oldpassword, newpassword);
                return result.AsSTSYIdentityResult();
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        public async Task<string> GeneratePasswordResetTokenAsync(string userId, CancellationToken cancellationToken)
        {
            try
            {

                if (userId == null) throw new ArgumentNullException(nameof(userId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                return await _userManager.GeneratePasswordResetTokenAsync(user);
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        public async Task<STSYIdentityResult> ResetPasswordAsync(string userId, string resetToken, string newPassword, CancellationToken cancellationToken)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                if (newPassword == null) throw new ArgumentNullException(nameof(newPassword));
                if (resetToken == null) throw new ArgumentNullException(nameof(resetToken));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);
                return result.AsSTSYIdentityResult();
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
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
                if (hasEmail || hasPhone || hasAuthenticator)
                {
                    await _userManager.SetTwoFactorEnabledAsync(user, enabled);
                }
                else
                {
                    throw new InvalidOperationException("No 2FA method available for user.");
                }
            }
        }

        private async Task DisableTowFactorIfNoOther(MicrosoftIdentityUser user)
        {
            bool hasEmail = await _userManager.IsEmailConfirmedAsync(user);
            bool hasPhone = await _userManager.IsPhoneNumberConfirmedAsync(user);
            if (!(hasEmail || hasPhone))
            {
                await _userManager.SetTwoFactorEnabledAsync(user, false);
            }
        }
        public async Task<List<string>> GenerateNewRecoveryCode(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 6);
                return recoveryCodes.ToList();
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        public async Task<string> ReGenerateTOTKey(string userId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                await _userManager.ResetAuthenticatorKeyAsync(user);
                await DisableTowFactorIfNoOther(user);
                return await _userManager.GetAuthenticatorKeyAsync(user);
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }
        public async Task<bool> ValidateTOTKey(string userId, string code, CancellationToken cancellationToken = default)
        {
            try
            {
                if (userId == null) throw new ArgumentNullException(nameof(userId));
                if (code == null) throw new ArgumentNullException(nameof(code));
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) throw new ResourceNotFoundException(nameof(MicrosoftIdentityUser), userId, "User not found.");
                var result = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultAuthenticatorProvider, code);
                if (result && !await _userManager.GetTwoFactorEnabledAsync(user))
                    await SetTwoFactorEnabled(user.Id, true, cancellationToken);
                return result;
            }
            catch (STSYIdentityException ex) { throw; }
            catch (ArgumentException ex) { throw; }
            catch (Exception ex) { throw new STSYIdentityException(ex.Message, ex); }
        }

        #endregion
    }
}
