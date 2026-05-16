namespace STSY.Identity.Abstraction.Models.Input.account
{
    public class ExternalUserCreateInput : UserCreateInput
    {
        public string Provider { get; set; }
        public string ProviderId { get; set; }
        public bool EmailVerified { get; set; } = false;
        public bool PhoneVerified { get; set; } = false;
    }
}
