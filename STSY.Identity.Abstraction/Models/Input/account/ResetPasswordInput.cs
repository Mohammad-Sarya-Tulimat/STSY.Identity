namespace STSY.Identity.Abstraction.Models.Input.account
{
    public class ResetPasswordInput
    {
        public string UserNameOrEmail { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
