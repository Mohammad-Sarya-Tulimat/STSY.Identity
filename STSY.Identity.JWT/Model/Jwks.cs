using System.Collections.Generic;

namespace STSY.Identity.JWT.Model
{
    public class Jwks
    {
        public List<Jwk> Keys { get; set; } = new List<Jwk>();
    }

    public class Jwk
    {
        public string kty { get; set; }
        public string use { get; set; }
        public string alg { get; set; }
        public string kid { get; set; }

        // RSA
        public string n { get; set; }
        public string e { get; set; }

        // EC
        public string crv { get; set; }
        public string x { get; set; }
        public string y { get; set; }
    }
}
