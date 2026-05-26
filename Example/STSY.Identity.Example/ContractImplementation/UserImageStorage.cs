using STSY.Identity.API.Contract;

namespace STSY.Identity.Example.ContractImplementation
{
    public class UserImageStorage : IUsersProfileImagesStore
    {
        const string BaseUr = "D:/test/userImage";
        private string GetImageExtensionFromContentType(string contentType, string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                return Path.GetExtension(fileName).Trim();
            }
            return contentType.ToLower() switch
            {
                "image/jpeg" => ".jpg",
                "image/jpg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/bmp" => ".bmp",
                "image/webp" => ".webp",
                "image/tiff" => ".tiff",
                "image/svg+xml" => ".svg",
                _ => ".bin"
            };
        }

        public async Task<string> SaveImageAsync(string userId, UploadedFile uploadedFile, CancellationToken cancellationToken = default)
        {
            var directory = Path.Combine(BaseUr, userId);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            var fileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + this.GetImageExtensionFromContentType(uploadedFile.ContentType, uploadedFile.FileName);
            var userImage = Path.Combine(directory, fileName);
            using var newfile = File.Create(userImage);
            uploadedFile.Content.CopyTo(newfile);
            return fileName;
        }
        public Task RemoveImageAsync(string userId, string imageRefreance, CancellationToken cancellationToken = default)
        {
            var userImage = Path.Combine(BaseUr, userId, imageRefreance);
            if (File.Exists(userImage))
                File.Delete(userImage);
            return Task.CompletedTask;

        }
    }
}
