using STSY.Identity.Abstraction.Models.Input;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IUserManager
    {
        Task CreateUser(UserCreateInput input, CancellationToken cancellationToken);
        Task AddRole(string userId, string role, CancellationToken cancellationToken);


    }
}
