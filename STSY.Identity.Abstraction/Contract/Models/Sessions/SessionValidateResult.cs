namespace STSY.Identity.Abstraction.Contract.Models.Sessions
{
    public class SessionValidateResult
    {
        public bool Success { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
    }
}
