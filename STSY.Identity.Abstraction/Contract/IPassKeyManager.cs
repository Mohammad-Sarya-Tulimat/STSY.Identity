using STSY.Identity.Abstraction.Models.Output.UserModels;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IPassKeyManager
    {
        Task<string> GeneratePassKeyCreation(UserData user);
        Task<bool> ValidatePassKey(string credential);
        Task<bool> RemovePassKey(UserData user, byte[] id);
    }
}
