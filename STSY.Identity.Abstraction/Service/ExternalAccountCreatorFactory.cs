using STSY.Identity.Abstraction.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STSY.Identity.Abstraction.Service
{
    public class ExternalAccountCreatorFactory
    {
        IEnumerable<IExternalAccountCreator> _externalAccountCreator;
        public ExternalAccountCreatorFactory(IEnumerable<IExternalAccountCreator> externalAccountCreator)
        {
            _externalAccountCreator = externalAccountCreator;
        }
        public IExternalAccountCreator Get(string provider)
        {
            var creator = _externalAccountCreator.Where(s => string.Equals(provider, s.Provider)).FirstOrDefault();
            if (creator == null) throw new NotImplementedException($"No implementation for {provider} provider");
            return creator;
        }
    }
}
