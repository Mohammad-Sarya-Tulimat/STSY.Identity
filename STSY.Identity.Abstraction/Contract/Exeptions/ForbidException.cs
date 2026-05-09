using System;

namespace STSY.Identity.Abstraction.Contract.Exeptions
{
    public class ForbidException : STSYIdentityException
    {
        public ForbidException(string message) : base(message)
        {
        }
        public ForbidException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
