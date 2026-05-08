using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Abstraction.Models.Output.UserModels;
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
        public async Task<ExtendedUser> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;
            var roles = await _userManager.GetRolesAsync(user);
            return user.ToExtendedUser(roles.ToList());
        }

        public async Task<IEnumerable<UserPassKey>> GetUserPassKeyAsync(string userId, CancellationToken cancellationToken = default)
        {

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;
            var userPasskeys = await _userManager.GetPasskeysAsync(user);
            return userPasskeys.Select(s => s.ToUserPassKey());
        }

        public async Task<IQueryable<UserData>> GetUsersAsync(Expression<Func<UserData, bool>> expression)
        {
            var users = _userManager.Users.AsUserData();
            if (expression != null)
                users = users.Where(expression);
            return users;
        }

        public async Task<IEnumerable<UserSession>> GetUserSessionsAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _sTSYIdentityDbContext.UserSessions.Where(s => userId.Equals(s.UserId)).Select(s => new UserSession
            {
                Id = s.Id.ToString(),
                IpAddress = s.IpAddress,
                Location = s.Location,
                DateTimeOffset = s.CreatedAt
            }).ToListAsync(cancellationToken);
        }
    }
}
