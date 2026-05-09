using STSY.Identity.Abstraction.Models.Output.UserModels;

namespace STSY.Identity.Abstraction.Models
{
    public class CurrentAuthrizedUser : UserData
    {
        public string SessionId { get; set; }
    }
}
