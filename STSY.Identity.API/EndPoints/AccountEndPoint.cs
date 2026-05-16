using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Models.Input.account;

namespace STSY.Identity.API.EndPoints
{
    public static class AccountEndPoint
    {
        public static IEndpointRouteBuilder MapSTSYAccountEndPoint(this IEndpointRouteBuilder app, string prefix)
        {
            app.MapPost($"{prefix}/account", async ([FromBody] UserCreateInput request,
                [FromServices] IUserManager userManager,
                [FromServices] IReadUsers readUsers,
                [FromServices] ISessionManager loginService,
                CancellationToken token = default) =>
            {
                try
                {
                    var result = await userManager.CreateUser(request, token);
                    if (result.Success)
                    {

                        var user = await readUsers.GetUserByUserNameOrEmailAsync(request.UserName, token);
                        var session = await loginService.CreateSessionAsync(user, token);
                        if (session.isSuccess)
                        {
                            return Results.Ok(session);
                        }
                        else
                        {
                            return Results.BadRequest(session);
                        }
                    }
                    else
                        return Results.BadRequest(result);
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

            app.MapPost($"{prefix}/account/reset-password", async ([FromBody] ResetPasswordInput request, [FromServices] IPasswordManager passwordManager, [FromServices] IReadUsers readUsers, CancellationToken token = default) =>
            {
                try
                {
                    var user = await readUsers.GetUserByUserNameOrEmailAsync(request.UserNameOrEmail, token);
                    if (user != null)
                    {

                        var result = await passwordManager.ResetPasswordAsync(user.Id, request.Token, request.NewPassword, token);
                        if (result.Success)
                        {
                            return Results.Ok(result);
                        }
                        else
                        {
                            return Results.BadRequest(result);
                        }
                    }
                    else
                        return Results.BadRequest();
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
                    return Results.InternalServerError(new { ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while generate challenge");
                }
            }).AllowAnonymous();

            app.MapPost($"{prefix}/account/reset-password-challenge", async ([FromBody] ResetPasswordInput request,
                [FromServices] ISendChallengeTokens sendChallengeTokens,
                [FromServices] IPasswordManager passwordManager,
                [FromServices] IReadUsers readUsers,
                CancellationToken cancellationToken = default) =>
            {
                try
                {
                    var user = await readUsers.GetUserByUserNameOrEmailAsync(request.UserNameOrEmail, cancellationToken);
                    if (user != null)
                    {
                        var token = await passwordManager.GeneratePasswordResetTokenAsync(user.Id, cancellationToken);
                        await sendChallengeTokens.SendChallengeTokensAsync(user, Abstraction.Models.Enums.ChallengeTypeToSend.PasswordReset, token);
                        return Results.Ok("token generated and sent to your email".AsResult());
                    }
                    else
                        return Results.BadRequest();
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
                    return Results.InternalServerError(new { ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.InternalServerError("error while generate challenge");
                }
            }).AllowAnonymous();
            app.MapPost($"{prefix}/account/change-password", async ([FromBody] ChangePassword request,
                [FromServices] IPasswordManager passwordManager,
                [FromServices] IGetCurrentAuthorizedUser readUsers,
                CancellationToken token = default) =>
            {
                try
                {
                    var result = await passwordManager.ChangeUserPasswordAsync(readUsers.CurrentUser.Id, request.OldPassword, request.NewPassword, token);
                    if (result.Success)
                    {
                        return Results.Ok(result);

                    }
                    return Results.BadRequest(result);
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
            }).RequireAuthorization();


            return app;

        }
    }
}
