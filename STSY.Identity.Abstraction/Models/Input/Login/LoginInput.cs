using STSY.Identity.Abstraction.Models.Enums;
using System.Collections.Generic;

namespace STSY.Identity.Abstraction.Models.Input.Login
{
    public class LoginInput
    {
        public string Identitier { get; set; }
        public ChallengeTypeToSend Method { get; set; }
        public Dictionary<string, object> Credentials { get; set; }
    }
}
