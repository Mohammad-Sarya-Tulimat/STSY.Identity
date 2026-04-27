using System;

namespace STSY.Identity.Abstraction.Models
{
    public class UserPassKey
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
