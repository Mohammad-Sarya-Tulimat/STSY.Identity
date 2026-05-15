using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Contract.Models.Sessions;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
using STSY.Identity.Abstraction.Contract.Tokens;
using STSY.Identity.Abstraction.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Service
{
    public class StanderSessionCreator : ISessionManager
    {
        private const string RefreshTokenKey = "refreshToken";

        private const string SessionIdKey = "sessionId";
        private const string MFATokenKey = "mfaToken";
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IMFTokenGenerator _mFTokenGenerator;
        private readonly ISessionStorage _sessionStorage;
        private readonly IGetUserClaims _generateUserClaims;
        public StanderSessionCreator(
            IAccessTokenGenerator accessTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IMFTokenGenerator mFTokenGenerator,
            ISessionStorage sessionManager,
            IGetUserClaims generateUserClaims)
        {
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _mFTokenGenerator = mFTokenGenerator;
            _sessionStorage = sessionManager;
        }
        private string HashRefresh(string refreshToken)
        {
            // Implement your hashing logic here, for example using SHA256
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(refreshToken);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        private Dictionary<string, object> ToSave(string refreshToken)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result[RefreshTokenKey] = HashRefresh(refreshToken);
            return result;
        }
        public async Task<SessionResult> CreateSessionAsync(ExtendedUser user, CancellationToken cancellationToken = default)
        {
            string sessionId = Guid.NewGuid().ToString();
            Dictionary<string, object> tokenDatas = new Dictionary<string, object>();
            var cliems = await _generateUserClaims.GetUserClaimsAsync(user, cancellationToken);
            var refreshToen = await _refreshTokenGenerator.GenerateRefreshToken(user);

            tokenDatas["accessToken"] = await _accessTokenGenerator.GenerateAccessToken(user.Id, nameof(UserData), cliems.ToList());
            tokenDatas[RefreshTokenKey] = refreshToen;
            tokenDatas[SessionIdKey] = sessionId;
            var userSession = new UserSession
            {
                Id = sessionId,
                UserId = user.Id,
                CreatedAt = DateTimeOffset.UtcNow,
                ExpiredAt = refreshToen.Expiration,
                SessionType = "StanderSession",

            };
            await _sessionStorage.AddSession(user.Id, userSession, ToSave(refreshToen.Token), cancellationToken);
            return new SessionResult
            {
                isSuccess = true,
                IsMfRequred = false,
                Message = "login successfully",
                SessionData = tokenDatas
            };
        }

        public async Task<SessionResult> RefreshSessionAsync(ExtendedUser user, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var sessionId = dataToValidate[SessionIdKey].ToString();
            if (!await ValidateSessionAsync(user, dataToValidate, cancellationToken))
            {
                throw new UnauthorizedAccessException("Invalid session");
            }
            Dictionary<string, object> tokenDatas = new Dictionary<string, object>();
            var cliems = await _generateUserClaims.GetUserClaimsAsync(user, cancellationToken);
            var refreshToen = await _refreshTokenGenerator.GenerateRefreshToken(user);
            tokenDatas["accessToken"] = await _accessTokenGenerator.GenerateAccessToken(user.Id, nameof(UserData), cliems.ToList());
            tokenDatas[RefreshTokenKey] = refreshToen;
            tokenDatas[SessionIdKey] = sessionId;
            var userSession = new UserSession
            {
                Id = sessionId,
                UserId = user.Id,
                SessionType = "StanderSession",
                ExpiredAt = refreshToen.Expiration,
            };
            await _sessionStorage.UpdateSession(user.Id, userSession, ToSave(refreshToen.Token), cancellationToken);
            return new SessionResult
            {
                isSuccess = true,
                IsMfRequred = false,
                Message = "Session refreshed successfully",
                SessionData = tokenDatas
            };
        }
        public async Task<bool> ValidateSessionAsync(ExtendedUser user, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var sessionId = dataToValidate[SessionIdKey].ToString();
            var oldaDb = await _sessionStorage.GetSessionProtectedData(user.Id, sessionId, cancellationToken);
            if (oldaDb == null) return false;
            var oldInput = dataToValidate[RefreshTokenKey]?.ToString();
            var oldDbToken = oldaDb[RefreshTokenKey]?.ToString();
            if (!oldDbToken.Equals(HashRefresh(oldInput))) return false;
            return true;
        }

        public async Task<SessionResult> CreateMFSessionAsync(ExtendedUser user, CancellationToken cancellationToken = default)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result[MFATokenKey] = await _mFTokenGenerator.GenerateMFAToken(user.Id, nameof(UserData));
            return new SessionResult
            {
                isSuccess = true,
                IsMfRequred = true,
                Message = "MFA token generated successfully",
                SessionData = result
            };
        }
        public async Task<bool> ValidateMFSessionAsync(ExtendedUser user, Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var oldInput = dataToValidate[MFATokenKey]?.ToString();
            var validateMfaToken = await _mFTokenGenerator.ValidateMFAToken(oldInput);
            if (validateMfaToken.IsAcepted(user.Id, nameof(UserData))) return false;
            return true;
        }
    }
}
