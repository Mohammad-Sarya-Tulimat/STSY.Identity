using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Input.account;
using STSY.Identity.Abstraction.Models.Output;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Managers
{
    public interface IUserManager
    {
        /// <summary>
        ///  Create a new user
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>       
        /// /// <exception cref="System.ArgumentNullException"></exception>   
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<UserData> CreateUser(UserCreateInput input, CancellationToken cancellationToken = default);
        Task<UserData> CreateUser(ExternalUserCreate input, CancellationToken cancellationToken = default);
        Task<UserData> UpdateUser(UserData user, UserUpdateInput update, CancellationToken cancellationToken = default);
        Task UpdateProfileImageRef(UserData user, string imageref, CancellationToken cancellationToken = default);

        /// <summary>
        ///  Add a role to the user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="role"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>        
        /// <exception cref="System.ArgumentNullException"></exception>  
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>" 
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<STSYIdentityResult> AddRole(string userId, string role, CancellationToken cancellationToken);

        /// <summary>
        ///  Check if step up is enabled for the user, this will check if the session exists in cache and is not expired
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>  
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>" 
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<bool> IsStepUpEnabled(string userId, string sessionId, CancellationToken cancellationToken);


        /// <summary>
        ///  Enable step up for the user, this will add the session to cache with expiration time and user will not be required to do step up for next request until expiration time
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="sessionId"></param>
        /// <param name="expiration"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>        
        /// <exception cref="System.ArgumentNullException"></exception>  
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>" 
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<STSYIdentityResult> EnableStepUpAsync(string userId, string sessionId, DateTimeOffset expiration, CancellationToken cancellationToken);

        /// <summary>
        ///  rmove step up for the user, this will remove the session from cache and user will be required to do step up again for next request
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>  
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>" 
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<STSYIdentityResult> DisableStepUp(string userId, CancellationToken cancellationToken);

        Task<bool> IsMFAEnabled(string userId, CancellationToken cancellationToken);

        Task<bool> IsLocked(string userId, CancellationToken cancellationToken);

        Task<STSYIdentityResult> ResetLock(string userId, CancellationToken cancellationToken);
        Task<STSYIdentityResult> AccessFailedAsync(string userId, CancellationToken cancellationToken);

    }
}
