using System.Collections.Generic;

namespace STSY.Identity.Abstraction.Models.Input.Login
{
    public class LoginInput
    {
        public string CredentialType { get; set; }
        public Dictionary<string, object> Credentials { get; set; }
    }
}
