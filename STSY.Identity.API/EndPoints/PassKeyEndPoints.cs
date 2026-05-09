
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Exeptions;

namespace STSY.Microsoft.Identity.EndPoints
{
    public static class PassKeyEndPoints
    {
        public static IEndpointRouteBuilder MapPassKeyApis(this IEndpointRouteBuilder app, string prefix)
        {

            app.MapPost($"{prefix}/passkey/createOption", async (
                [FromServices] IPassKeyManager passKeyManager,
                [FromServices] IUserManager userManager,
                [FromServices] IGetCurrentAuthorizedUser currentUser,
                CancellationToken token = default
                ) =>
            {
                if (!await userManager.IsSecurityChangesAllowed(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, token))
                {
                    return Results.Forbid();
                }
                var creationOption = await passKeyManager.GeneratePassKeyCreation(currentUser.CurrentUser);
                return Results.Ok(creationOption);
            }).RequireAuthorization();

            app.MapPost($"{prefix}/passkey/assert", async (
                [FromServices] IPassKeyManager passKeyManager,
                [FromServices] IUserManager userManager,
                [FromServices] IGetCurrentAuthorizedUser currentUser,
                [FromBody] string credential,
                CancellationToken token = default
                ) =>
            {
                if (!await userManager.IsSecurityChangesAllowed(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, token))
                {
                    return Results.Forbid();
                }
                var creationOption = await passKeyManager.ValidatePassKey(credential);
                if (!creationOption)
                {
                    return Results.BadRequest(new { Message = "invalid credential" });
                }
                return Results.Ok(new { Message = "created" });
            }).RequireAuthorization();

            app.MapDelete($"{prefix}/passkey/{{id}}", async (
                byte[] id,
                [FromServices] IPassKeyManager passKeyManager,
                [FromServices] IUserManager userManager,
                [FromServices] IGetCurrentAuthorizedUser currentUser,
                CancellationToken token = default
                ) =>
            {
                try
                {
                    if (!await userManager.IsSecurityChangesAllowed(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, token))
                    {
                        return Results.Forbid();
                    }
                    var removed = await passKeyManager.RemovePassKey(currentUser.CurrentUser, id);
                    if (removed)
                    {
                        return Results.Ok(new { Message = "removed" });
                    }
                    else
                    {
                        return Results.BadRequest(new { Message = "cannot delete" });
                    }
                }
                catch (ForbidException ex)
                {
                    return Results.Forbid();
                }
            }).RequireAuthorization();

            return app;
        }
    }
}
