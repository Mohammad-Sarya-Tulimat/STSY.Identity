using System;

namespace STSY.Identity.Abstraction.Models.Output.Tokens
{
    public class TokenData
    {
        public string Token { get; set; }
        public string TokenType { get; set; }
        public DateTimeOffset Expiration { get; set; }
    }
}
