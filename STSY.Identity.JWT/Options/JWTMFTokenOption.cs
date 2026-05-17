namespace STSY.Identity.JWT.Options
{
    public class JWTMFTokenOption
    {
        public bool IsPrimary { get; set; }
        public string SecretKey { get; set; }
        public string KeyId { get; set; }
        public int LifeInMinutes { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        /// <summary>
        /// please select one from the list of supported algorithms in  <see cref="Microsoft.IdentityModel.Tokens.SecurityAlgorithms"/>
        /// </summary>
        public string SecurityAlgorithms { get; set; }
    }
}
