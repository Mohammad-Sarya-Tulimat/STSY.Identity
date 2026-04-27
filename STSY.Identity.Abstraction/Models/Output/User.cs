using System;

namespace STSY.Identity.Abstraction.Models.Output
{
    public class User
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
