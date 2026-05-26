using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Models;
using System.Security.Claims;

namespace STSY.Identity.Example.ContractImplementation
{
    public class GetCurrentAuthorizedUser : IGetCurrentAuthorizedUser
    {
        IHttpContextAccessor _httpContextAccessor;
        public GetCurrentAuthorizedUser(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        private string GetValue(ClaimsPrincipal user, string type)
        {
            return user.Claims.FirstOrDefault(s => s.Type.Equals(type))?.Value;
        }
        public CurrentAuthrizedUser CurrentUser
        {
            get
            {
                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated == false) return null;
                var user = _httpContextAccessor.HttpContext.User;
                var birth = DateTime.MinValue;
                var date = GetValue(user, ClaimTypes.DateOfBirth);
                if (date != null)
                    birth = DateTime.Parse(date);
                return new CurrentAuthrizedUser
                {
                    Id = GetValue(user, ClaimTypes.NameIdentifier),
                    Email = GetValue(user, ClaimTypes.Email),
                    PhoneNumber = GetValue(user, ClaimTypes.MobilePhone),
                    UserName = GetValue(user, ClaimTypes.Name),
                    FirstName = GetValue(user, ClaimTypes.GivenName),
                    LastName = GetValue(user, ClaimTypes.Surname),
                    SessionId = GetValue(user, ClaimTypes.Sid),
                    DateOfBirth = birth,

                };
            }
        }
    }
}
