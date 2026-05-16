using STSY.Identity.Abstraction.Contract.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IReadUsers
    {
        Task<IQueryable<UserData>> GetUsersAsync(Expression<Func<UserData, bool>> expression);
        Task<UserData> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserData> GetUserByUserNameOrEmailAsync(string userNameOrEmail, CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetUserLinkedAccountsAsync(string userId, CancellationToken cancellationToken = default);
        Task<UserData> GetUserByLinkedAccountsIdAsync(string provider, string providerUserId, CancellationToken cancellationToken = default);

    }
}
