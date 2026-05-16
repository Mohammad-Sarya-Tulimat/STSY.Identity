namespace STSY.Identity.Abstraction.Models.Output.Tokens
{
    public class STSYTokenValidationResult
    {
        public bool IsValid { get; set; }
        public string ResourceId { get; set; }
        public string ResourceType { get; set; }
        public bool IsAcepted(string id, string idType)
        {
            return IsValid && string.Equals(id, id) && string.Equals(ResourceType, idType);
        }
    }
}
