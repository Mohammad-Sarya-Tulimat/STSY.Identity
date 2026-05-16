using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;

namespace STSY.Identity.API.EndPoints
{
    public static class MFASeupEndPoint
    {
        public static IEndpointRouteBuilder MapSTSYMFASeupApis(this IEndpointRouteBuilder app, string prefix)
        {
            app.MapPost($"{prefix}/mfa/change-recovery-codes", async (
            [FromServices] ITwoFactorManager twoFactorManager,
            [FromServices] IUserManager userManager,
            [FromServices] IGetCurrentAuthorizedUser currentUser,
            CancellationToken token = default) =>
            {
                try
                {
                    if (!await userManager.IsStepUpEnabled(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, token))
                    {
                        return Results.Forbid();
                    }
                    var recoveryCodes = await twoFactorManager.GenerateNewRecoveryCode(currentUser.CurrentUser.Id, token);
                    return Results.Ok(recoveryCodes);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message.AsResult());
                }
                catch (ResourceNotFoundException ex)
                {
                    return Results.NotFound(ex.Message.AsResult());
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message.AsResult());
                }
            }).RequireAuthorization();

            app.MapPost($"{prefix}/mfa/new-totp-key", async (
                [FromServices] ITwoFactorManager twoFactorManager,
                [FromServices] IUserManager userManager,
                [FromServices] IGetCurrentAuthorizedUser currentUser,
                CancellationToken token = default
            ) =>
            {
                try
                {

                    if (!await userManager.IsStepUpEnabled(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, token))
                    {
                        return Results.Forbid();
                    }
                    var recoveryCodes = await twoFactorManager.ReGenerateTOTKey(currentUser.CurrentUser.Id, token);
                    return Results.Ok(recoveryCodes);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message.AsResult());
                }
                catch (ResourceNotFoundException ex)
                {
                    return Results.NotFound(ex.Message.AsResult());
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message.AsResult());
                }
            }).RequireAuthorization();

            app.MapPost($"{prefix}/mfa/validate-totp-key", async (
            [FromServices] ITwoFactorManager twoFactorManager,
            [FromServices] IUserManager userManager,
            [FromServices] IGetCurrentAuthorizedUser currentUser,
            [FromBody] Dictionary<string, string> body,
            CancellationToken token = default) =>
            {
                try
                {
                    var recoveryCodes = await twoFactorManager.ValidateTOTKey(currentUser.CurrentUser.Id, body[CredentialKeys.OTP_KEY], token);
                    return Results.Ok(recoveryCodes);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message.AsResult());
                }
                catch (ResourceNotFoundException ex)
                {
                    return Results.NotFound(ex.Message.AsResult());
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message.AsResult());
                }
            }).RequireAuthorization();

            return app;
        }
    }
}
