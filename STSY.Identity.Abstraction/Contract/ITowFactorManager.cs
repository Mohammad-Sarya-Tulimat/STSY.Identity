using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface ITowFactorManager
    {
        Task SetTwoFactorEnabled(string userId, bool enabled, CancellationToken cancellationToken = default);
        Task<List<string>> GenerateNewRecoveryCode(string userId, CancellationToken cancellationToken = default);
        Task<string> ReGenerateTOTKey(string userId, CancellationToken cancellationToken = default);
        Task<bool> ValidateTOTKey(string userId, string code, CancellationToken cancellationToken = default);
    }
}
