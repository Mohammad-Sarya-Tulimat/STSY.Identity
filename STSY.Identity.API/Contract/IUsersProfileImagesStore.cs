namespace STSY.Identity.API.Contract
{
    public interface IUsersProfileImagesStore
    {
        Task<string> SaveImageAsync(string userId, UploadedFile image, CancellationToken cancellationToken = default);
        Task RemoveImageAsync(string userId, string imageRefreance, CancellationToken cancellationToken = default);
    }
}
