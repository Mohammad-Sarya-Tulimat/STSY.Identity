using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Models.Input.account;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IExternalAccountCreator
    {
        string Provider { get; }
        Task<UserData> CreateAccount(ExternalAccountCreateInput externalAccountCreateInput);
    }
}
