using System.Collections.Generic;

namespace STSY.Identity.Abstraction.Models.Input.account
{
    public class ExternalAccountCreateInput
    {
        public string Provider { get; set; }
        public string Password { get; set; }
        public Dictionary<string, object> Credentials { get; set; }
    }
}
