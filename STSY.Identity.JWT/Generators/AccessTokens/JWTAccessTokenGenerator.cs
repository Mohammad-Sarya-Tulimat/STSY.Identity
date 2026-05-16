using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using STSY.Identity.Abstraction.Contract.Tokens;
using STSY.Identity.Abstraction.Models.Output.Tokens;
using STSY.Identity.JWT.Model;
using STSY.Identity.JWT.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace STSY.Identity.JWT.Generators.Access
{
    public class JWTAccessTokenGenerator : IAccessTokenGenerator
    {
        private readonly Dictionary<string, JWTAccessKeyOption> _keysById;
        private readonly JWTAccessKeyOption _activeKey;
        public JWTAccessTokenGenerator(List<JWTAccessKeyOption> jWTAccessKeyOption)
        {
            _keysById = jWTAccessKeyOption.ToDictionary(k => k.KeyId, v => v);
            _activeKey = jWTAccessKeyOption.Where(s => s.IsPrimary).FirstOrDefault();
        }
        public async Task<TokenData> GenerateAccessToken(string resourceId, string resourceType, List<Claim> inputClaims)
        {

            var claims = inputClaims.ToList();
            claims.Add(new Claim("ResourceId", resourceId));
            claims.Add(new Claim("ResourceType", resourceType));
            var key = Algorithms.GetSecurityKey(_activeKey.SecurityAlgorithms, _activeKey.SecretKey);
            key.KeyId = _activeKey.KeyId;
            var expirateion = DateTime.UtcNow.AddMinutes(_activeKey.LifeInMinutes);
            var creds = new SigningCredentials(key, _activeKey.SecurityAlgorithms);
            var token = new JwtSecurityToken(
                issuer: _activeKey.Issuer,
                audience: _activeKey.Audience,
                claims: claims,
                expires: expirateion,
                signingCredentials: creds
            );
            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenData
            {
                TokenType = "JWTAccessToken",
                Expiration = expirateion,
                Token = jwt
            };
        }
        private string GetKeyId(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);
            return token.Header.TryGetValue("kid", out var kid) ? kid?.ToString() : null;
        }
        public async Task<STSYTokenValidationResult> ValidateAccessToken(string token)
        {
            if (!_keysById.TryGetValue(GetKeyId(token), out var option))
            {
                return new STSYTokenValidationResult { IsValid = false };
            }
            var jsonWebTokenHandler = new JsonWebTokenHandler();
            var key = Algorithms.GetSecurityKey(option.SecurityAlgorithms, option.SecretKey);
            key.KeyId = option.KeyId;
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = option.Issuer,
                ClockSkew = TimeSpan.Zero,
                ValidAudience = option.Audience,
                IssuerSigningKey = key
            };
            var tokenValidationResult = await jsonWebTokenHandler.ValidateTokenAsync(token, validationParameters);
            var result = new STSYTokenValidationResult
            {
                IsValid = tokenValidationResult.IsValid
            };
            if (tokenValidationResult.IsValid)
            {
                result.ResourceType = tokenValidationResult.ClaimsIdentity.Claims.FirstOrDefault(c => c.Type == "ResourceType")?.Value;
                result.ResourceId = tokenValidationResult.ClaimsIdentity.Claims.FirstOrDefault(c => c.Type == "ResourceId")?.Value;
            }
            return result;
        }
        public Jwks GetJwk()
        {
            var jwks = new Jwks();
            foreach (var item in _keysById.Values)
            {
                var asymmetricAlgorithm = Algorithms.AsymmetricAlgorithmFromJson(item.SecurityAlgorithms, item.SecretKey);
                var jwk = JwksGenerator.Generate(asymmetricAlgorithm, item.SecurityAlgorithms, item.KeyId);
                jwks.Keys.Add(jwk);
            }
            return jwks;
        }
    }
}
