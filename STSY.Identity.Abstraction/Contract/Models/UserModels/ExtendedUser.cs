using System.Collections.Generic;

namespace STSY.Identity.Abstraction.Contract.Models.UserModels
{
    public class ExtendedUser : UserData
    {
        public bool IsEmailConfirmed { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
        public List<string> Roles { get; set; }
    }
}
