using System;

namespace STSY.Identity.Abstraction.Contract.Models.Sessions
{
    public class UserSession
    {
        public string Id { get; set; }
        public string SessionType { get; set; }
        public string UserId { get; set; }
        public string IpAddress { get; set; }
        public string Location { get; set; }
        public DateTimeOffset ExpiredAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
