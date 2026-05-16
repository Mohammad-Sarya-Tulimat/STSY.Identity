using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
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
                    return Results.Unauthorized();
                }
                catch (ForbidException ex)
                {
                    return Results.Forbid();
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
                    return Results.Unauthorized();
                }
                catch (ForbidException ex)
                {
                    return Results.Forbid();
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
                    return Results.Unauthorized();
                }
                catch (ForbidException ex)
                {
                    return Results.Forbid();
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
            return app;
        }
    }
}
