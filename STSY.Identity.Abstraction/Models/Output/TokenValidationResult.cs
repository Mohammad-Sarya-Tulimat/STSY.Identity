namespace STSY.Identity.Abstraction.Models.Output
{
    public class TokenValidationResult
    {
        public bool IsValid { get; set; }
        public string Id { get; set; }
        public string IdType { get; set; }
        public bool IsAcepted(string id, string idType)
        {
            return IsValid && string.Equals(id, id) && string.Equals(IdType, idType);
        }
    }
}
