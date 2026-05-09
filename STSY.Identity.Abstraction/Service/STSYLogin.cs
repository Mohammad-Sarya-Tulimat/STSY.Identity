using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Contract.Tokens;
using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Abstraction.Models.Output.Auth;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Service
{
    public class STSYLogin
    {
        private const string IdType = "userId";
        private readonly AuthenticatorFactory _authenticatorFactory;
        private readonly IAccessTokenGenerator _accessTokenGenerator;
        private readonly IRefreshTokenGenerator _refreshTokenGenerator;
        private readonly IReadUsers _readUsers;
        private readonly ISessionManager _sessionManager;
        private readonly IUserManager _userManager;
        private readonly GenerateUserClaimes _generateUserClaimes;
        public STSYLogin(AuthenticatorFactory authenticatorFactory,
            IAccessTokenGenerator accessTokenGenerator,
            IRefreshTokenGenerator refreshTokenGenerator,
            IReadUsers readUsers,
            ISessionManager sessionManager,
            IUserManager userManager,
            GenerateUserClaimes generateUserClaimes)
        {
            _authenticatorFactory = authenticatorFactory;
            _accessTokenGenerator = accessTokenGenerator;
            _refreshTokenGenerator = refreshTokenGenerator;
            _readUsers = readUsers;
            _sessionManager = sessionManager;
            _userManager = userManager;
            _generateUserClaimes = generateUserClaimes;
        }
        public async Task<List<TokenData>> Login(LoginInput loginInput, CancellationToken cancellationToken = default)
        {
            List<TokenData> tokenDatas = new List<TokenData>();
            var authenticator = _authenticatorFactory.GetAuthenticator(loginInput.CredentialType, Models.Enums.AuthenticatorUsage.Primary);
            if (authenticator == null) throw new NotImplementedException($"cannot find authenticator of type {loginInput.CredentialType} for {Models.Enums.AuthenticatorUsage.Primary} factor");
            var user = await _readUsers.GetUserByUserNameOrEmailAsync(loginInput.EmailOrUserName, cancellationToken);
            if (user == null) throw new ResourceNotFoundException(nameof(loginInput), loginInput.EmailOrUserName, "connot fiund fiund user");
            var isValid = await authenticator.ValidateCredentialAsync(user, loginInput.Credentials);
            if (isValid)
            {

                var sid = Guid.NewGuid().ToString();
                var cliems = await _generateUserClaimes.GetClaims(user, sid, cancellationToken);
                if (await _userManager.IsMFAEnabled(user.Id, cancellationToken))//2factor
                {
                    var mfaToken = await _accessTokenGenerator.GenerateMFAToken(user.Id, IdType, cliems);
                    tokenDatas.Add(mfaToken);
                    return tokenDatas;
                }
                else
                {
                    var accessToken = await _accessTokenGenerator.GenerateAccessToken(user.Id, IdType, cliems);
                    var refreshToken = await _refreshTokenGenerator.GenerateRefreshToken(user);
                    tokenDatas.Add(accessToken);
                    tokenDatas.Add(refreshToken);
                    var userSession = new UserSession
                    {
                        Id = sid,
                        UserId = user.Id,
                        DateTimeOffset = DateTimeOffset.UtcNow,
                    };
                    await _sessionManager.AddSession(user.Id, userSession, refreshToken.Token, cancellationToken);
                    return tokenDatas;
                }
            }
            throw new AuthenticatorException("invalid credentials");
        }


        public async Task<List<TokenData>> MFALogin(LoginInput loginInput, CancellationToken cancellationToken = default)
        {
            List<TokenData> tokenDatas = new List<TokenData>();
            var authenticator = _authenticatorFactory.GetAuthenticator(loginInput.CredentialType, Models.Enums.AuthenticatorUsage.MultiFactor);
            if (authenticator == null) throw new NotImplementedException($"cannot find authenticator of type {loginInput.CredentialType} for {Models.Enums.AuthenticatorUsage.MultiFactor} factor");
            var user = await _readUsers.GetUserByUserNameOrEmailAsync(loginInput.EmailOrUserName, cancellationToken);
            if (user == null) throw new ResourceNotFoundException(nameof(loginInput), loginInput.EmailOrUserName, "connot fiund fiund user");
            var validateMfaToken = await _accessTokenGenerator.ValidateMFAToken(loginInput.MFAToken);
            if (validateMfaToken.IsAcepted(user.Id, IdType)) throw new AuthenticatorException("invalid mfa token");
            var isValid = await authenticator.ValidateCredentialAsync(user, loginInput.Credentials);
            if (isValid)
            {
                var sid = Guid.NewGuid().ToString();
                var cliems = await _generateUserClaimes.GetClaims(user, sid, cancellationToken);
                var accessToken = await _accessTokenGenerator.GenerateAccessToken(user.Id, IdType, cliems);
                var refreshToken = await _refreshTokenGenerator.GenerateRefreshToken(user);
                tokenDatas.Add(accessToken);
                tokenDatas.Add(refreshToken);
                var userSession = new UserSession
                {
                    Id = sid,
                    UserId = user.Id,
                    DateTimeOffset = DateTimeOffset.UtcNow,
                };
                await _sessionManager.AddSession(user.Id, userSession, refreshToken.Token, cancellationToken);
                return tokenDatas;

            }
            throw new AuthenticatorException("invalid credentials");
        }
        public async Task<AuthInitiateResult> GetChallenge(LoginInput loginInput, CancellationToken cancellationToken = default)
        {
            var authenticator = _authenticatorFactory.GetChallengeGenerator(loginInput.CredentialType);
            if (authenticator == null) throw new NotImplementedException($"cannot find ChallengeGenerator of type {loginInput.CredentialType}");
            var user = await _readUsers.GetUserByUserNameOrEmailAsync(loginInput.EmailOrUserName, cancellationToken);
            if (user == null) throw new ResourceNotFoundException(nameof(loginInput), loginInput.EmailOrUserName, "connot fiund fiund user");
            return await authenticator.InitiateAsync(user);
        }

    }
}
