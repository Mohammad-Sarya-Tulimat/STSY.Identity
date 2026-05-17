using STSY.Identity.Facebook;
using STSY.Identity.Facebook.Models;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class FacebookAuthProvider
{
    private readonly STSYFacebookIdentityOption _options;

    public FacebookAuthProvider(
        STSYFacebookIdentityOption options)
    {
        _options = options;
    }

    public async Task<FacebookProfileResponse> ValidateAsync(
        string token)
    {
        using (HttpClient _httpClient = new HttpClient())
        {

            var appAccessToken =
                $"{_options.AppId}|{_options.Secrets}";

            // Validate token
            var debugUrl =
                $"https://graph.facebook.com/debug_token" +
                $"?input_token={token}" +
                $"&access_token={appAccessToken}";

            var debugResponse =
                await _httpClient.GetAsync(debugUrl);

            if (!debugResponse.IsSuccessStatusCode)
            {
                return null;
            }
            var debugResultStr = await debugResponse.Content.ReadAsStringAsync();
            var debugResult = JsonSerializer.Deserialize<FacebookDebugTokenResponse>(debugResultStr);
            if (debugResult?.Data == null)
            {
                return null;
            }

            if (!debugResult.Data.IsValid)
            {
                return null;
            }

            if (debugResult.Data.AppId != _options.AppId)
            {
                return null;
            }
            var meUrl =
                $"https://graph.facebook.com/me" +
                $"?fields=id,email,first_name,last_name,name,verified" +
                $"&access_token={token}";

            var meResponse =
                await _httpClient.GetAsync(meUrl);

            if (!meResponse.IsSuccessStatusCode)
            {
                return null;
            }

            var profileStr = await meResponse.Content.ReadAsStringAsync();
            var profile = JsonSerializer.Deserialize<FacebookProfileResponse>(profileStr);
            if (profile == null || string.IsNullOrWhiteSpace(profile.Id))
            {
                return null;
            }
            return profile;
        }
    }
}