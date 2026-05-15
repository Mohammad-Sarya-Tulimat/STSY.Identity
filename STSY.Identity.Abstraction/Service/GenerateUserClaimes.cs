using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Contract.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Service
{
    public class GenerateUserClaimes
    {
        IEnumerable<IGetUserClaims> _getUserClaims;
        public GenerateUserClaimes(IEnumerable<IGetUserClaims> getUserClaims = null)
        {
            _getUserClaims = getUserClaims;
        }
        public async Task<List<Claim>> GetClaims(ExtendedUser user, string sessionId, CancellationToken cancellationToken = default)
        {
            var cliems = new List<Claim>();
            foreach (var item in _getUserClaims ?? new List<IGetUserClaims>())
            {
                cliems.AddRange(await item.GetUserClaimsAsync(user, cancellationToken));
            }
            return cliems;
        }
    }
}
