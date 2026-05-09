using System;

namespace STSY.Identity.Abstraction.Contract.Exeptions
{
    public class STSYIdentityException : ApplicationException
    {
        public STSYIdentityException(string message) : base(message)
        {
        }
        public STSYIdentityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
