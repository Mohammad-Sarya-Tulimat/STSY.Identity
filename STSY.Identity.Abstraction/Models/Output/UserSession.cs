using System;

namespace STSY.Identity.Abstraction.Models.Output
{
    public class UserSession
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
    }
}
