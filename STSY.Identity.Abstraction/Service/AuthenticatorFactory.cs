using STSY.Identity.Abstraction.Contract.Authentication;
using System.Collections.Generic;
using System.Linq;

namespace STSY.Identity.Abstraction.Service
{
    public class AuthenticatorFactory
    {
        private readonly IEnumerable<IAuthenticator> _authenticators;
        private readonly IEnumerable<IMFAuthenticator> _mfAuthenticators;
        private readonly IEnumerable<IChallengeAuthenticator> _challengeAuthenticators;
        public AuthenticatorFactory(IEnumerable<IAuthenticator> authenticators, IEnumerable<IMFAuthenticator> mfAuthenticators, IEnumerable<IChallengeAuthenticator> challengeAuthenticators)
        {
            _authenticators = authenticators;
            _challengeAuthenticators = challengeAuthenticators;
            _mfAuthenticators = mfAuthenticators;
        }
        public IAuthenticator GetAuthenticator(string credentialType)
        {
            return _authenticators.Where(s => string.Equals(s.CredentialType, credentialType, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        public IMFAuthenticator GetMFAuthenticator(string credentialType)
        {
            return _mfAuthenticators.Where(s => string.Equals(s.CredentialType, credentialType, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
        public IChallengeAuthenticator GetChallengeGenerator(string credentialType)
        {
            return _challengeAuthenticators.Where(s => string.Equals(s.CredentialType, credentialType, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
    }
}
