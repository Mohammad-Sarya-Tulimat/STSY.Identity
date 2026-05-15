using STSY.Identity.Abstraction.Contract.Models;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IGetCurrentAuthorizedUser
    {
        CurrentAuthrizedUser CurrentUser { get; }
    }
}
