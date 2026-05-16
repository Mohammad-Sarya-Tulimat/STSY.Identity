using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Models;
using STSY.Microsoft.Identity.DBContext;
using STSY.Microsoft.Identity.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
namespace STSY.Microsoft.Identity.Services
{
    public class MicrosoftIdentityUserRepo : IReadUsers
    {
        UserManager<MicrosoftIdentityUser> _userManager;
        STSYIdentityDbContext _sTSYIdentityDbContext;
        public MicrosoftIdentityUserRepo(UserManager<MicrosoftIdentityUser> userManager, STSYIdentityDbContext sTSYIdentityDbContext)
        {
            _userManager = userManager;
            _sTSYIdentityDbContext = sTSYIdentityDbContext;
        }
        public async Task<UserData> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;
            return user.ToUserData();
        }

        public async Task<UserData> GetUserByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(userNameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(userNameOrEmail);
            if (user == null)
                return null;
            return user.ToUserData();
        }

        public async Task<UserData> GetUserByLinkedAccountsIdAsync(string provider, string providerUserId, CancellationToken cancellationToken = default)
        {
            var user = await _sTSYIdentityDbContext.UserExternalLogins.Where(x =>
            string.Equals(x.ProviderUserId, providerUserId)
            && string.Equals(x.Provider, provider)
            ).Select(x => x.User).FirstOrDefaultAsync(cancellationToken);
            if (user == null) return null;
            return user.ToUserData();
        }
        public async Task<IEnumerable<string>> GetUserLinkedAccountsAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _sTSYIdentityDbContext.UserExternalLogins.Where(x => x.UserId == userId).Select(x => x.Provider).ToListAsync(cancellationToken);
        }
        public async Task<IQueryable<UserData>> GetUsersAsync(Expression<Func<UserData, bool>> expression)
        {
            var users = _userManager.Users.AsUserData();
            if (expression != null)
                users = users.Where(expression);
            return users;
        }
    }
}
