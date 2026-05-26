using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Models.Input.Login;
using STSY.Identity.Abstraction.Service;
namespace STSY.Identity.API.EndPoints
{
    public static class LoginEndPoint
    {
        public static IEndpointRouteBuilder MapSTSYLoginEndPoint(this IEndpointRouteBuilder app, string prefix)
        {
            app.MapPost($"{prefix}/auth/challenge", async ([FromBody] LoginInput request, [FromServices] STSYLogin login, CancellationToken token = default) =>
            {
                try
                {
                    var result = await login.GetChallenge(request, token);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message.AsResult());
                }
                catch (AuthenticatorException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status401Unauthorized);
                }
                catch (ForbidException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status403Forbidden);
                }
                catch (ResourceNotFoundException ex)
                {
                    return Results.NotFound(ex.Message.AsResult());
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message.AsResult());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while generate challenge".AsResult());
                }
            }).AllowAnonymous();

            app.MapPost($"{prefix}/auth/login", async ([FromBody] LoginInput request, [FromServices] STSYLogin login, CancellationToken token = default) =>
            {
                try
                {
                    var result = await login.Login(request, token);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message.AsResult());
                }
                catch (AuthenticatorException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status401Unauthorized);
                }
                catch (ForbidException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status403Forbidden);
                }
                catch (ResourceNotFoundException ex)
                {
                    return Results.NotFound(ex.Message.AsResult());
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message.AsResult());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while processing  login".AsResult());
                }
            }).AllowAnonymous();
            app.MapPost($"{prefix}/auth/refresh", async ([FromBody] Dictionary<string, object> request, [FromServices] ISessionManager manager, CancellationToken token = default) =>
            {
                try
                {
                    var result = await manager.RefreshSessionAsync(request, token);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message.AsResult());
                }
                catch (AuthenticatorException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status401Unauthorized);
                }
                catch (ForbidException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status403Forbidden);
                }
                catch (ResourceNotFoundException ex)
                {
                    return Results.NotFound(ex.Message.AsResult());
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message.AsResult());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while processing  login".AsResult());
                }
            }).AllowAnonymous();
            app.MapPost($"{prefix}/auth/mfa_login", async ([FromBody] LoginInput request, [FromServices] STSYLogin login, CancellationToken token = default) =>
            {
                try
                {
                    var result = await login.MFALogin(request, token);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message.AsResult());
                }
                catch (AuthenticatorException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status401Unauthorized);
                }
                catch (ForbidException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status403Forbidden);
                }
                catch (ResourceNotFoundException ex)
                {
                    return Results.NotFound(ex.Message.AsResult());
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message.AsResult());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while processing MFA login".AsResult());
                }
            }).AllowAnonymous();


            app.MapPost($"{prefix}/auth/logout", async (
                [FromServices] IGetCurrentAuthorizedUser currentUser,
                [FromServices] ISessionManager sessionManager, CancellationToken token = default) =>
            {
                try
                {
                    await sessionManager.SignOutAsync(currentUser.CurrentUser.SessionId, token);
                    return Results.Ok("logout done".AsResult());
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message.AsResult());
                }
                catch (AuthenticatorException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status401Unauthorized);
                }
                catch (ForbidException ex)
                {
                    return Results.Json(ex.Message.AsResult(), statusCode: StatusCodes.Status403Forbidden);
                }
                catch (ResourceNotFoundException ex)
                {
                    return Results.NotFound(ex.Message.AsResult());
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message.AsResult());
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while processing  login".AsResult());
                }
            }).RequireAuthorization();
            return app;
        }
    }
}
