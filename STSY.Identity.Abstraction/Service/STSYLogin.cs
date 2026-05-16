using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Contract.Models.UserModels;
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
            var authenticator = _authenticatorFactory.GetAuthenticator(loginInput.CredentialType);
            if (authenticator == null) throw new NotImplementedException($"cannot find authenticator of type {loginInput.CredentialType}");
            var validationResult = await authenticator.ValidateCredentialAsync(loginInput.Credentials);
            if (validationResult.User != null && await _userManager.IsLocked(validationResult.User.Id, cancellationToken)) throw new AuthenticatorException("user is locked out");
            if (validationResult.Success)
            {
                if (validationResult.NeedMfactor && await _userManager.IsMFAEnabled(validationResult.User.Id, cancellationToken))//2factor
                {
                    return await _sessionCreator.CreateMFSessionAsync(validationResult.User, cancellationToken);
                }
                else
                {
                    await _userManager.ResetLock(validationResult.User.Id, cancellationToken);
                    return await _sessionCreator.CreateSessionAsync(validationResult.User, cancellationToken);
                }
            }

            if (validationResult.User != null)
                await _userManager.AccessFailedAsync(validationResult.User.Id, cancellationToken);
            throw new AuthenticatorException("invalid credentials");
        }
        public async Task<SessionResult> MFALogin(LoginInput loginInput, CancellationToken cancellationToken = default)
        {
            var validation = await _sessionCreator.ValidateMFSessionAsync(loginInput.Credentials);
            if (!validation.Success) throw new AuthenticatorException("Invalid or expired session.");
            var user = await _readUsers.GetUserByIdAsync(validation.UserId);
            var authenticator = _authenticatorFactory.GetMFAuthenticator(loginInput.CredentialType);
            if (authenticator == null) throw new NotImplementedException($"cannot find authenticator of type {loginInput.CredentialType} for 2factor");
            var validationResult = await authenticator.ValidateCredentialAsync(user.Id, loginInput.Credentials);
            if (user == null) throw new AuthenticatorException("invalid credentials");
            if (await _userManager.IsLocked(user.Id, cancellationToken)) throw new AuthenticatorException("user is locked out");
            if (validationResult.Success)
            {
                await _userManager.ResetLock(user.Id, cancellationToken);
                return await _sessionCreator.CreateSessionAsync(user, cancellationToken);
            }
            await _userManager.AccessFailedAsync(user.Id, cancellationToken);
            throw new AuthenticatorException("invalid credentials");
        }

        public async Task<AuthInitiateResult> GetChallenge(LoginInput loginInput, CancellationToken cancellationToken = default)
        {
            var authenticator = _authenticatorFactory.GetChallengeGenerator(loginInput.CredentialType);
            if (authenticator == null) throw new NotImplementedException($"cannot find ChallengeGenerator of type {loginInput.CredentialType}");
            UserData user = null;
            if (loginInput.Credentials.TryGetValue(CredentialKeys.EMAIL_OR_USERNAME_KEY, out var email))
            {
                user = await _readUsers.GetUserByUserNameOrEmailAsync(email.ToString());
            }
            else
            {
                var validateMfaToken = await _sessionCreator.ValidateMFSessionAsync(loginInput.Credentials);
                if (!validateMfaToken.Success)
                {
                    throw new AuthenticatorException("invalid credentials");
                }
                user = await _readUsers.GetUserByIdAsync(validateMfaToken.UserId, cancellationToken);
            }
            if (user == null) throw new AuthenticatorException("invalid credentials");
            return await authenticator.InitiateAsync(user);
        }
    }
}
