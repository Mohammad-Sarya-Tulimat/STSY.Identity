using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IPasswordManager
    {
        Task<string> GeneratePasswordResetTokenAsync(string userId, CancellationToken cancellationToken);
        Task ResetPassword(string userId, string resetToken, string newPassword, CancellationToken cancellationToken);
        Task ChangeUserPassword(string userId, string newpassword, string oldpassword, CancellationToken cancellationToken);
    }
}
