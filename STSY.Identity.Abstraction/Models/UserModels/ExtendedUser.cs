using System.Collections.Generic;

namespace STSY.Identity.Abstraction.Models.UserModel
{
    public class ExtendedUser : UserData
    {
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
        public List<string> Roles { get; set; }
    }
}
