using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Models.Input.Login;
using STSY.Identity.Abstraction.Models.Output;
using STSY.Identity.Abstraction.Models.Output.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Identity.Abstraction.Service
{
    public class STSYLogin
    {
        private readonly AuthenticatorFactory _authenticatorFactory;
        private readonly IReadUsers _readUsers;
        private readonly IUserManager _userManager;
        private readonly ISessionManager _sessionCreator;
        public STSYLogin(
            AuthenticatorFactory authenticatorFactory,
            IReadUsers readUsers,
            IUserManager userManager,
            ISessionManager sessionCreator)
        {
            this._authenticatorFactory = authenticatorFactory;
            this._readUsers = readUsers;
            this._userManager = userManager;
            this._sessionCreator = sessionCreator;
        }
        public async Task<SessionResult> Login(LoginInput loginInput, CancellationToken cancellationToken = default)
        {
            var authenticator = _authenticatorFactory.GetAuthenticator(loginInput.CredentialType, Models.Enums.AuthenticatorUsage.Primary);
            if (authenticator == null) throw new NotImplementedException($"cannot find authenticator of type {loginInput.CredentialType} for {Models.Enums.AuthenticatorUsage.Primary} factor");
            var user = await _readUsers.GetUserByUserNameOrEmailAsync(loginInput.EmailOrUserName, cancellationToken);
            if (user == null) throw new ResourceNotFoundException(nameof(loginInput), loginInput.EmailOrUserName, "connot fiund fiund user");
            if (await _userManager.IsLocked(user.Id, cancellationToken)) throw new AuthenticatorException("user is locked out");
            var isValid = await authenticator.ValidateCredentialAsync(user, loginInput.Credentials);
            if (isValid)
            {
                if (await _userManager.IsMFAEnabled(user.Id, cancellationToken))//2factor
                {
                    return await _sessionCreator.CreateMFSessionAsync(user, cancellationToken);
                }
                else
                {
                    return await _sessionCreator.CreateSessionAsync(user, cancellationToken);
                }
            }
            await _userManager.AccessFailedAsync(user.Id, cancellationToken);
            throw new AuthenticatorException("invalid credentials");
        }


        public async Task<SessionResult> MFALogin(LoginInput loginInput, CancellationToken cancellationToken = default)
        {
            var authenticator = _authenticatorFactory.GetAuthenticator(loginInput.CredentialType, Models.Enums.AuthenticatorUsage.MultiFactor);
            if (authenticator == null) throw new NotImplementedException($"cannot find authenticator of type {loginInput.CredentialType} for {Models.Enums.AuthenticatorUsage.MultiFactor} factor");
            var user = await _readUsers.GetUserByUserNameOrEmailAsync(loginInput.EmailOrUserName, cancellationToken);
            if (user == null) throw new ResourceNotFoundException(nameof(loginInput), loginInput.EmailOrUserName, "connot fiund fiund user");
            if (await _userManager.IsLocked(user.Id, cancellationToken)) throw new AuthenticatorException("user is locked out");
            var validateMfaToken = await _sessionCreator.ValidateMFSessionAsync(user, loginInput.Credentials);
            if (!validateMfaToken) throw new AuthenticatorException("invalid mfa token");
            var isValid = await authenticator.ValidateCredentialAsync(user, loginInput.Credentials);
            if (isValid)
            {
                return await _sessionCreator.CreateSessionAsync(user, cancellationToken);
            }
            await _userManager.AccessFailedAsync(user.Id, cancellationToken);
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
