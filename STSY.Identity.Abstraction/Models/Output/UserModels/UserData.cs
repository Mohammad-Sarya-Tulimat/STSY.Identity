using System;

namespace STSY.Identity.Abstraction.Models.Output.UserModels
{
    public class UserData
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
