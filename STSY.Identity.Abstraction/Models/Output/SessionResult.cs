using System.Collections.Generic;

namespace STSY.Identity.Abstraction.Models.Output
{
    public class SessionResult
    {
        public bool IsSuccess { get; set; }
        public bool IsMFARequired { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> SessionData { get; set; }
    }
}
