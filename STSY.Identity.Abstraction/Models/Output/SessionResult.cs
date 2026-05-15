using System.Collections.Generic;

namespace STSY.Identity.Abstraction.Models.Output
{
    public class SessionResult
    {
        public bool isSuccess { get; set; }
        public bool IsMfRequred { get; set; }
        public string Message { get; set; }
        public Dictionary<string, object> SessionData { get; set; }
    }
}
