using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Models.Input;
using STSY.Identity.Models;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services
{
    public class MicrosoftIdentityUserManager : IUserManager
    {
        UserManager<MicrosoftIdentityUser> _userManager;
        public MicrosoftIdentityUserManager(UserManager<MicrosoftIdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task AddRole(string userId, string role, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task ChangeUserPassword(string userId, string newpassword, string oldpassword, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            await _userManager.ChangePasswordAsync(user, oldpassword, newpassword);
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
    }
}
