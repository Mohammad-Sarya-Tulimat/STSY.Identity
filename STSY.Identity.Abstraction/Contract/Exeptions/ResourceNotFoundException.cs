using System;

namespace STSY.Identity.Abstraction.Contract.Exeptions
{
    public class ResourceNotFoundException : STSYIdentityException
    {
        public string Referance { get; set; }
        public string ResourceName { get; set; }
        public ResourceNotFoundException(string resourceNmae, string referance, string message) : base(message)
        {
            Referance = referance;
            ResourceName = resourceNmae;
        }
        public ResourceNotFoundException(string resourceNmae, string referance, string message, Exception innerException) : base(message, innerException)
        {
            Referance = referance;
            ResourceName = resourceNmae;
        }
    }
}
