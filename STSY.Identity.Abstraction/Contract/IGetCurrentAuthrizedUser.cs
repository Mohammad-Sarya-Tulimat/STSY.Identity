using STSY.Identity.Abstraction.Models;

namespace STSY.Identity.Abstraction.Contract
{
    public interface IGetCurrentAuthorizedUser
    {
        CurrentAuthrizedUser CurrentUser { get; }
    }
}
