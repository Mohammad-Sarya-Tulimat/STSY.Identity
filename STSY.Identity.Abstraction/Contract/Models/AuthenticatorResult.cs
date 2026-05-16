using STSY.Identity.Abstraction.Contract.Models.UserModels;

namespace STSY.Identity.Abstraction.Contract.Models
{
    public class AuthenticatorResult
    {
        public bool Success { get; set; }
        public UserData User { get; set; }
        public bool NeedMfactor { get; set; } = false;
    }
}
