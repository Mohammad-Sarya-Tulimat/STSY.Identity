using Microsoft.AspNetCore.Identity;

namespace STSY.Identity.Models
{
    public class MicrosoftIdentityUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
