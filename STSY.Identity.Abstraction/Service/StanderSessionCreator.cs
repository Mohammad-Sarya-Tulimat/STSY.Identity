using STSY.Identity.Abstraction.Contract;
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
        private readonly IReadUsers _readUsers;
        public StanderSessionCreator(
            IAccessTokenGenerator accessTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IMFTokenGenerator mFTokenGenerator,
            ISessionStorage sessionManager,
            IGetUserClaims generateUserClaims,
            IReadUsers readUsers)
        {
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _mFTokenGenerator = mFTokenGenerator;
            _sessionStorage = sessionManager;
            _readUsers = readUsers;
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
        public async Task<SessionResult> CreateSessionAsync(UserData user, CancellationToken cancellationToken = default)
        {
            string sessionId = Guid.NewGuid().ToString();
            Dictionary<string, object> tokenDatas = new Dictionary<string, object>();
            var cliems = await _generateUserClaims.GetUserClaimsAsync(user.Id, cancellationToken);
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

        public async Task<SessionResult> CreateMFSessionAsync(UserData user, CancellationToken cancellationToken = default)
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


        public async Task<SessionResult> RefreshSessionAsync(Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var sessionId = dataToValidate[SessionIdKey].ToString();
            var validate = await this.ValidateSessionAsync(dataToValidate, cancellationToken);
            if (!validate.Success)
            {
                throw new UnauthorizedAccessException("Invalid session");
            }
            var user = await _readUsers.GetUserByIdAsync(validate.UserId, cancellationToken);
            Dictionary<string, object> tokenDatas = new Dictionary<string, object>();
            var cliems = await _generateUserClaims.GetUserClaimsAsync(user.Id, cancellationToken);
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
        public async Task<SessionValidateResult> ValidateSessionAsync(Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var sessionId = dataToValidate[SessionIdKey].ToString();
            var oldaDb = await _sessionStorage.GetSession(sessionId, cancellationToken);
            if (oldaDb == null) return new SessionValidateResult { Success = false };
            var dbData = await _sessionStorage.GetSessionProtectedData(sessionId, cancellationToken);
            var oldInput = dataToValidate[RefreshTokenKey]?.ToString();
            var oldDbToken = dbData[RefreshTokenKey]?.ToString();
            if (!oldDbToken.Equals(HashRefresh(oldInput))) return new SessionValidateResult { Success = false };
            return new SessionValidateResult { Success = true, SessionId = sessionId, UserId = oldaDb.UserId };
        }
        public async Task<SessionValidateResult> ValidateMFSessionAsync(Dictionary<string, object> dataToValidate, CancellationToken cancellationToken = default)
        {
            var oldInput = dataToValidate[MFATokenKey]?.ToString();
            var validateMfaToken = await _mFTokenGenerator.ValidateMFAToken(oldInput);
            return new SessionValidateResult
            {
                Success = validateMfaToken.IsValid,
                UserId = validateMfaToken.ResourceId,
                SessionId = null
            };
        }
    }
}
