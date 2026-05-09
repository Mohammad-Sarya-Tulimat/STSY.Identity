using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;

namespace STSY.Identity.API.EndPoints
{
    public static class MFASeupEndPoint
    {
        public static IEndpointRouteBuilder MapMFASeupApis(this IEndpointRouteBuilder app, string prefix)
        {
            app.MapPost($"{prefix}/mfa/change-recovery-codes", async (
    [FromServices] ITowFactorManager twoFactorManager,
    [FromServices] IUserManager userManager,
    [FromServices] IGetCurrentAuthorizedUser currentUser,
    CancellationToken token = default
    ) =>
            {
                if (!await userManager.IsSecurityChangesAllowed(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, token))
                {
                    return Results.Forbid();
                }
                var recoveryCodes = await twoFactorManager.GenerateNewRecoveryCode(currentUser.CurrentUser.Id, token);
                return Results.Ok(recoveryCodes);
            }).RequireAuthorization();

            app.MapPost($"{prefix}/mfa/new-totp-key", async (
                [FromServices] ITowFactorManager twoFactorManager,
                [FromServices] IUserManager userManager,
                [FromServices] IGetCurrentAuthorizedUser currentUser,
                CancellationToken token = default
            ) =>
            {
                if (!await userManager.IsSecurityChangesAllowed(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, token))
                {
                    return Results.Forbid();
                }
                var recoveryCodes = await twoFactorManager.ReGenerateTOTKey(currentUser.CurrentUser.Id, token);
                return Results.Ok(recoveryCodes);
            }).RequireAuthorization();

            app.MapPost($"{prefix}/mfa/validate-totp-key", async (
            [FromServices] ITowFactorManager twoFactorManager,
            [FromServices] IUserManager userManager,
            [FromServices] IGetCurrentAuthorizedUser currentUser,
            [FromBody] Dictionary<string, string> body,
            CancellationToken token = default) =>
            {
                var recoveryCodes = await twoFactorManager.ValidateTOTKey(currentUser.CurrentUser.Id, body[CredentialKeys.OTP_KEY], token);
                return Results.Ok(recoveryCodes);
            }).RequireAuthorization();

            return app;
        }
    }
}
