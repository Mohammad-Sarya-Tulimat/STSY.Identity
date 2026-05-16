
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.API;

namespace STSY.Microsoft.Identity.EndPoints
{
    public static class PassKeyEndPoints
    {
        public static IEndpointRouteBuilder MapSTSYPassKeyApis(this IEndpointRouteBuilder app, string prefix)
        {

            app.MapPost($"{prefix}/passkey/createOption", async (
                [FromServices] IPassKeyManager passKeyManager,
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
                    var creationOption = await passKeyManager.GeneratePassKeyCreationOptionsAsync(currentUser.CurrentUser);
                    return Results.Ok(creationOption);
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

            app.MapPost($"{prefix}/passkey/assert", async (
                [FromServices] IPassKeyManager passKeyManager,
                [FromServices] IUserManager userManager,
                [FromServices] IGetCurrentAuthorizedUser currentUser,
                [FromBody] string credential,
                CancellationToken token = default
                ) =>
            {
                try
                {

                    if (!await userManager.IsStepUpEnabled(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, token))
                    {
                        return Results.Forbid();
                    }
                    var creationOption = await passKeyManager.PasskeyAttestationAsync(credential);
                    if (!creationOption.Success)
                    {
                        return Results.BadRequest(creationOption);
                    }
                    return Results.Ok(creationOption);
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
                    if (!await userManager.IsStepUpEnabled(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, token))
                    {
                        return Results.Forbid();
                    }
                    var result = await passKeyManager.RemovePassKey(currentUser.CurrentUser, id);
                    if (result.Success)
                    {
                        return Results.Ok(result);
                    }
                    else
                    {
                        return Results.BadRequest(result);
                    }
                }
                catch (ForbidException ex)
                {
                    return Results.Forbid();
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
