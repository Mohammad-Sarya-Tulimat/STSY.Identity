using STSY.Identity.JWT.Model;
using System;
using System.Security.Cryptography;

namespace STSY.Identity.JWT.Generators
{
    public static class JwksGenerator
    {
        public static Jwk Generate(AsymmetricAlgorithm key, string alg, string kid)
        {

            var jwk = new Jwk
            {
                kid = kid,
                use = "sig",
                alg = alg
            };

            if (key is RSA rsa)
            {
                var p = rsa.ExportParameters(false);

                jwk.kty = "RSA";
                jwk.n = Algorithms.Base64UrlEncode(p.Modulus);
                jwk.e = Algorithms.Base64UrlEncode(p.Exponent);
            }
            else if (key is ECDsa ecdsa)
            {
                var p = ecdsa.ExportParameters(false);

                jwk.kty = "EC";
                jwk.crv = GetCurveName(p.Curve);
                jwk.x = Algorithms.Base64UrlEncode(p.Q.X);
                jwk.y = Algorithms.Base64UrlEncode(p.Q.Y);
            }
            else
            {
                throw new NotSupportedException("Only RSA and ECDSA are supported");
            }
            return jwk;
        }

        private static string GetCurveName(ECCurve curve)
        {
            if (curve.Oid.FriendlyName == "nistP256") return "P-256";
            if (curve.Oid.FriendlyName == "nistP384") return "P-384";
            if (curve.Oid.FriendlyName == "nistP521") return "P-521";
            return curve.Oid.FriendlyName;
        }
    }
}
