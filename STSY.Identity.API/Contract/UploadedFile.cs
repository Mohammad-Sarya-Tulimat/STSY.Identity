namespace STSY.Identity.API.Contract
{
    public class UploadedFile
    {
        public Stream Content { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
    }
}
