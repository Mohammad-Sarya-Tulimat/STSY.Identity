using STSY.Identity.Abstraction.Contract;
using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Contract.Exeptions;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Contract.Tokens;
using STSY.Identity.Abstraction.Models.Input.Login;
using STSY.Identity.Abstraction.Models.Output.Auth;
using STSY.Identity.Abstraction.Models.Output.UserModels;
using System;
using System.Collections.Generic;
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
        private readonly IMFTokenGenerator _mFTokenGenerator;
        public STSYLogin(
            AuthenticatorFactory authenticatorFactory,
            IReadUsers readUsers,
            IUserManager userManager,
            ISessionManager sessionCreator,
            IMFTokenGenerator mFTokenGenerator)
        {
            this._authenticatorFactory = authenticatorFactory;
            this._readUsers = readUsers;
            this._userManager = userManager;
            this._sessionCreator = sessionCreator;
            this._mFTokenGenerator = mFTokenGenerator;
        }
        public async Task<Dictionary<string, object>> Login(LoginInput loginInput, CancellationToken cancellationToken = default)
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
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    result["MFToke"] = await _mFTokenGenerator.GenerateMFAToken(user.Id, nameof(UserData));
                    return result;
                }
                else
                {
                    var sid = Guid.NewGuid().ToString();
                    return await _sessionCreator.CreateSessionAsync(user, sid, cancellationToken);
                }
            }
            await _userManager.AccessFailedAsync(user.Id, cancellationToken);
            throw new AuthenticatorException("invalid credentials");
        }


        public async Task<Dictionary<string, object>> MFALogin(LoginInput loginInput, CancellationToken cancellationToken = default)
        {
            var authenticator = _authenticatorFactory.GetAuthenticator(loginInput.CredentialType, Models.Enums.AuthenticatorUsage.MultiFactor);
            if (authenticator == null) throw new NotImplementedException($"cannot find authenticator of type {loginInput.CredentialType} for {Models.Enums.AuthenticatorUsage.MultiFactor} factor");
            var user = await _readUsers.GetUserByUserNameOrEmailAsync(loginInput.EmailOrUserName, cancellationToken);
            if (user == null) throw new ResourceNotFoundException(nameof(loginInput), loginInput.EmailOrUserName, "connot fiund fiund user");
            if (await _userManager.IsLocked(user.Id, cancellationToken)) throw new AuthenticatorException("user is locked out");

            var validateMfaToken = await _mFTokenGenerator.ValidateMFAToken(loginInput.MFAToken);
            if (validateMfaToken.IsAcepted(user.Id, nameof(UserData))) throw new AuthenticatorException("invalid mfa token");
            var isValid = await authenticator.ValidateCredentialAsync(user, loginInput.Credentials);
            if (isValid)
            {
                var sid = Guid.NewGuid().ToString();
                return await _sessionCreator.CreateSessionAsync(user, sid, cancellationToken);
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
