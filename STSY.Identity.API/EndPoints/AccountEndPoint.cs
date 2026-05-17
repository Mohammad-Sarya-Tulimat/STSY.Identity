using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Models.Input.account;
using STSY.Identity.Abstraction.Models.Input.Login;
using STSY.Identity.Abstraction.Service;

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
                    var result = await passwordManager.ChangeUserPasswordAsync(currentUser.CurrentUser.Id, request.NewPassword, request.OldPassword, token);
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

            app.MapPost($"{prefix}/account/step-up", async ([FromBody] LoginInput request,
                [FromServices] IUserManager userManager,
                [FromServices] IGetCurrentAuthorizedUser currentUser,
                [FromServices] AuthenticatorFactory factory,
                CancellationToken token = default) =>
            {
                try
                {
                    var authenticator = factory.GetAuthenticator(request.CredentialType);
                    if (!authenticator.AllowStepUp) return Results.Forbid();
                    request.Credentials[CredentialKeys.EMAIL_OR_USERNAME_KEY] = currentUser.CurrentUser.UserName;
                    var result = await authenticator.ValidateCredentialAsync(request.Credentials);
                    if (result.Success && result.User.Id.Equals(currentUser.CurrentUser.Id))
                    {
                        await userManager.EnableStepUpAsync(currentUser.CurrentUser.Id, currentUser.CurrentUser.SessionId, DateTimeOffset.UtcNow.AddMinutes(5), token);
                        return Results.Ok("valid".AsResult());
                    }
                    return Results.Forbid();
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
            app.MapPost($"{prefix}/account/step-up-challenge", async ([FromBody] LoginInput request,
    [FromServices] IUserManager userManager,
    [FromServices] IGetCurrentAuthorizedUser currentUser,
    [FromServices] AuthenticatorFactory factory,
    CancellationToken token = default) =>
            {
                try
                {
                    var authenticator = factory.GetChallengeGenerator(request.CredentialType);
                    var result = await authenticator.InitiateAsync(currentUser.CurrentUser);
                    if (result.IsSuccess)
                    {
                        return Results.Ok(result);
                    }
                    return Results.Forbid();
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
