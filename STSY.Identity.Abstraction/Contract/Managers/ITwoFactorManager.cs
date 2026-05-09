using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Managers
{
    public interface ITwoFactorManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="enabled"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>  
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>" 
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task SetTwoFactorEnabled(string userId, bool enabled, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>  
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>" 
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<List<string>> GenerateNewRecoveryCode(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>  
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>" 
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<string> ReGenerateTOTKey(string userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>  
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>" 
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<bool> ValidateTOTKey(string userId, string code, CancellationToken cancellationToken = default);
    }
}
