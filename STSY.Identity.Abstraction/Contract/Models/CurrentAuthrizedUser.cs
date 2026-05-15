using STSY.Identity.Abstraction.Contract.Models.UserModels;

namespace STSY.Identity.Abstraction.Contract.Models
{
    public class CurrentAuthrizedUser : UserData
    {
        public string SessionId { get; set; }
    }
}
