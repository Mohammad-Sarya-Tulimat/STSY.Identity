using STSY.Identity.Abstraction.Contract.Authentication;
using STSY.Identity.Abstraction.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace STSY.Identity.Abstraction.Service
{
    public class AuthenticatorFactory
    {
        private readonly IEnumerable<IAuthenticator> _authenticators;
        private readonly IEnumerable<IChallengeAuthenticator> _challengeAuthenticators;
        public AuthenticatorFactory(IEnumerable<IAuthenticator> authenticators, IEnumerable<IChallengeAuthenticator> challengeAuthenticators)
        {
            _authenticators = authenticators;
            _challengeAuthenticators = challengeAuthenticators;
        }
        public IAuthenticator GetAuthenticator(string credentialType, AuthenticatorUsage authenticatorUsage)
        {
            return _authenticators.Where(s => string.Equals(s.CredentialType, credentialType, System.StringComparison.OrdinalIgnoreCase) && (authenticatorUsage & s.Usage) == authenticatorUsage).FirstOrDefault();
        }
        public IChallengeAuthenticator GetChallengeGenerator(string credentialType)
        {
            return _challengeAuthenticators.Where(s => string.Equals(s.CredentialType, credentialType, System.StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }
    }
}
