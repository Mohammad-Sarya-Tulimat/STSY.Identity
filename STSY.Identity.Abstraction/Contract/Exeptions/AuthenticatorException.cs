using System;

namespace STSY.Identity.Abstraction.Contract.Exeptions
{
    public class AuthenticatorException : ApplicationException
    {
        public AuthenticatorException(string message) : base(message)
        {
        }
        public AuthenticatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
