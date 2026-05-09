using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Managers
{
    public interface IPassKeyManager
    {
        /// <summary>
        ///  Generate passkey creation options for the user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"> when user is null </exception>"
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>"  
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<string> GeneratePassKeyCreationOptionsAsync(UserData user);
        /// <summary>
        ///  Perform passkey attestation for the given credential
        ///  this will save pass key if it valid
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns> 
        /// <exception cref="System.ArgumentNullException"> when credential is null or empty </exception>"
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<STSYIdentityResult> PasskeyAttestationAsync(string credential);
        /// <summary>
        /// remove passkey for the user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"> when user or id is null </exception>"
        /// <exception cref="Exeptions.ResourceNotFoundException"></exception>" 
        /// <exception cref="Exeptions.STSYIdentityException">general identity exception</exception>" 
        Task<STSYIdentityResult> RemovePassKey(UserData user, byte[] id);
    }
}
