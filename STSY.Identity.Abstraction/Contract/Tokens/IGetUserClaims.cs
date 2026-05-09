using STSY.Identity.Abstraction.Models.Output.UserModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Contract.Tokens
{
    public interface IGetUserClaims
    {
        Task<IEnumerable<Claim>> GetUserClaimsAsync(ExtendedUser user, CancellationToken cancellationToken = default);
    }
}
