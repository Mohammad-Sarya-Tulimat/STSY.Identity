using System.Text.Json.Serialization;

namespace STSY.Identity.Facebook.Models
{
    public class FacebookDebugTokenResponse
    {
        [JsonPropertyName("data")]
        public FacebookDebugTokenData Data { get; set; }
    }

    public class FacebookDebugTokenData
    {
        [JsonPropertyName("app_id")]
        public string AppId { get; set; }

        [JsonPropertyName("is_valid")]
        public bool IsValid { get; set; }
    }
    public class FacebookProfileResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("verified")]
        public bool? Verified { get; set; }
    }
}
