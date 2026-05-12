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
        public static IEndpointRouteBuilder MapLoginEndPoint(this IEndpointRouteBuilder app, string prefix)
        {
            app.MapPost($"{prefix}/auth/challenge", async ([FromServices] LoginInput request, [FromServices] STSYLogin login, CancellationToken token = default) =>
            {
                try
                {
                    var result = await login.GetChallenge(request, token);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
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
                    return Results.NotFound(ex.Message);
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while generate challenge");
                }
            }).AllowAnonymous();

            app.MapPost($"{prefix}/auth/login", async ([FromServices] LoginInput request, [FromServices] STSYLogin login, CancellationToken token = default) =>
            {
                try
                {
                    var result = await login.Login(request, token);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
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
                    return Results.NotFound(ex.Message);
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while processing  login");
                }
            }).AllowAnonymous();
            app.MapPost($"{prefix}/auth/mfa_login", async ([FromServices] LoginInput request, [FromServices] STSYLogin login, CancellationToken token = default) =>
            {
                try
                {
                    var result = await login.MFALogin(request, token);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
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
                    return Results.NotFound(ex.Message);
                }
                catch (STSYIdentityException ex)
                {
                    return Results.InternalServerError(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while processing MFA login");
                }
            }).AllowAnonymous();
            return app;
        }
    }
}
