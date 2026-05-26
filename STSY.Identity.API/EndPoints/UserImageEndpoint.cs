using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.API.Contract;
namespace STSY.Identity.API.EndPoints
{
    public static class UserImageEndpoint
    {
        public static async Task<IEndpointRouteBuilder> MapSTSYAccountImageEndPoint(this IEndpointRouteBuilder app, string prefix)
        {
            app.MapPost($"{prefix}/account/profile-image",
             async (IFormFile file,
               [FromServices] IUserManager userManager,
               [FromServices] IReadUsers readUsers,
               [FromServices] IUsersProfileImagesStore imagesStore,
               [FromServices] IGetCurrentAuthorizedUser currentUser,
               CancellationToken token = default) =>
             {
                 try
                 {
                     using (var stream = file.OpenReadStream())
                     {
                         var user = await readUsers.GetUserByIdAsync(currentUser.CurrentUser.Id);
                         if (!string.IsNullOrEmpty(user.ImageReference))
                         {
                             await imagesStore.RemoveImageAsync(user.Id, user.ImageReference, token);
                         }
                         string imageid = await imagesStore.SaveImageAsync(
                             currentUser.CurrentUser.Id,
                             new UploadedFile
                             {
                                 ContentType = file.ContentType,
                                 Content = stream,
                                 FileName = file.FileName
                             },
                         token);
                         await userManager.UpdateProfileImageRef(currentUser.CurrentUser, imageid, token);
                     }
                     return Results.Ok("Updated".AsResult());
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

            app.MapDelete($"{prefix}/account/profile-image",
                 async ([FromServices] IReadUsers readUsers,
                   [FromServices] IUserManager userManager,
                   [FromServices] IUsersProfileImagesStore imagesStore,
                   [FromServices] IGetCurrentAuthorizedUser currentUser,
                   CancellationToken token = default) =>
                 {
                     try
                     {
                         var user = await readUsers.GetUserByIdAsync(currentUser.CurrentUser.Id);
                         await imagesStore.RemoveImageAsync(user.Id, user.ImageReference, token);
                         await userManager.UpdateProfileImageRef(currentUser.CurrentUser, "", token);

                         return Results.Ok("Removed".AsResult());
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
