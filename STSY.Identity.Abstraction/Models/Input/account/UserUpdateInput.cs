using System;

namespace STSY.Identity.Abstraction.Models.Input.account
{
    public class UserUpdateInput
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}
