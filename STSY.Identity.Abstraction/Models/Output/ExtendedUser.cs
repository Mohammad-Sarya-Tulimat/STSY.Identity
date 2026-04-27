using System.Collections.Generic;

namespace STSY.Identity.Abstraction.Models.Output
{
    public class ExtendedUser : User
    {
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
        public List<string> Roles { get; set; }
    }
}
