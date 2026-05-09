using Microsoft.AspNetCore.Identity;
using STSY.Identity.Abstraction.Models.Output;
using System.Linq;

namespace STSY.Microsoft.Identity.Mappers
{
    internal static class IdentityResultMapper
    {
        internal static STSYIdentityResult AsSTSYIdentityResult(this IdentityResult identityResult)
        {
            if (identityResult == null)
            {
                return null;
            }
            var message = string.Empty;
            if (identityResult.Errors != null && identityResult.Errors.Any())
            {
                foreach (var error in identityResult.Errors.Where(s => !string.IsNullOrEmpty(s.Description)))
                {
                    message += $"{error.Code}: {error.Description}\n";
                }
            }
            return new STSYIdentityResult
            {
                Success = identityResult.Succeeded,
                Message = message,
            };
        }
    }
}
