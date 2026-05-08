using System.Collections.Generic;

namespace STSY.Identity.Abstraction.Models.Output.Auth
{
    public class AuthInitiateResult
    {
        public bool IsSuccess { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }
}
