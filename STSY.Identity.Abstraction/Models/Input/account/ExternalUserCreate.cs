namespace STSY.Identity.Abstraction.Models.Input.account
{
    public class ExternalUserCreate : UserCreateInput
    {
        public string Provider { get; set; }
        public string ProviderId { get; set; }
        public bool EmailVerified { get; set; } = false;
        public bool PhoneVerified { get; set; } = false;
    }
}
