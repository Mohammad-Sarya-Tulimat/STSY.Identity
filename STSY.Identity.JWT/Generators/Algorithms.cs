
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;
using System.Text.Json;

namespace STSY.Identity.JWT.Generators
{
    internal static class Algorithms
    {
        public static SecurityKey GetSymmetricKey(string base64Key)
        {
            var secret = Convert.FromBase64String(
                base64Key);

            return new SymmetricSecurityKey(secret);
        }
        public static RSA RSAFromJson(string json)
        {
            var parameters = JsonSerializer.Deserialize<RSAParameters>(json);
            RSA rsa = RSA.Create();
            rsa.ImportParameters(parameters);
            return rsa;
        }

        public static ECDsa ECDsaFromJson(string json)
        {
            var parameters = JsonSerializer.Deserialize<ECParameters>(json);
            ECDsa ecdsa = ECDsa.Create();
            ecdsa.ImportParameters(parameters);
            return ecdsa;
        }

        public static AsymmetricAlgorithm AsymmetricAlgorithmFromJson(string algorithm, string key)
        {
            if (algorithm == SecurityAlgorithms.RsaSha256 || algorithm == SecurityAlgorithms.RsaSha384 || algorithm == SecurityAlgorithms.RsaSha512)
            {
                return (RSAFromJson(key));
            }

            if (algorithm == SecurityAlgorithms.EcdsaSha256 ||
                algorithm == SecurityAlgorithms.EcdsaSha384 ||
                algorithm == SecurityAlgorithms.EcdsaSha512)
            {
                return (ECDsaFromJson(key));
            }
            throw new NotSupportedException($"Algorithm '{algorithm}' is not supported.");
        }
        public static SecurityKey GetSecurityKey(string algorithm, string key)
        {
            if (algorithm == SecurityAlgorithms.HmacSha256 ||
                algorithm == SecurityAlgorithms.HmacSha384 ||
                algorithm == SecurityAlgorithms.HmacSha512)
            {
                return GetSymmetricKey(key);
            }
            if (algorithm == SecurityAlgorithms.RsaSha256 ||
                algorithm == SecurityAlgorithms.RsaSha384 ||
                algorithm == SecurityAlgorithms.RsaSha512)
            {
                return new RsaSecurityKey(RSAFromJson(key));
            }

            if (algorithm == SecurityAlgorithms.EcdsaSha256 ||
                algorithm == SecurityAlgorithms.EcdsaSha384 ||
                algorithm == SecurityAlgorithms.EcdsaSha512)
            {
                return new ECDsaSecurityKey(ECDsaFromJson(key));
            }
            throw new NotSupportedException($"Algorithm '{algorithm}' is not supported.");
        }

        public static string GetRsaPublicKeyJson(RSA rsa)
        {
            var publicParams = rsa.ExportParameters(false);
            return JsonSerializer.Serialize(publicParams);
        }
        public static string GetEcdsaPublicKeyJson(ECDsa ecdsa)
        {
            var publicParams = ecdsa.ExportParameters(false);
            return JsonSerializer.Serialize(publicParams);
        }
        public static string GetPublicKeyJson(AsymmetricAlgorithm rsa)
        {
            if (rsa is RSA rsaAlg)
            {
                return GetRsaPublicKeyJson(rsaAlg);
            }
            else if (rsa is ECDsa ecdsaAlg)
            {
                return GetEcdsaPublicKeyJson(ecdsaAlg);
            }
            return null;
        }
        public static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

    }
}
