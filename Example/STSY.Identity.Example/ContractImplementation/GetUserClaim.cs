using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Tokens;
using System.Security.Claims;

namespace STSY.Identity.Example.ContractImplementation
{
    public class GetUserClaim : IGetUserClaims
    {
        IReadUsers _readUsers;
        public GetUserClaim(IReadUsers readUsers)
        {
            _readUsers = readUsers;
        }
        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _readUsers.GetUserByIdAsync(userId);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
            claims.Add(new Claim(ClaimTypes.GivenName, user.FirstName));
            claims.Add(new Claim(ClaimTypes.Surname, user.LastName));
            claims.Add(new Claim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString()));
            return claims;
        }
    }
}
