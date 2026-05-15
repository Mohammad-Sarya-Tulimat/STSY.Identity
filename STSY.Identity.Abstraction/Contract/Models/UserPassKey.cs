using System;

namespace STSY.Identity.Abstraction.Contract.Models
{
    public class UserPassKey
    {
        public byte[] Id { get; set; }
        public string Name { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
