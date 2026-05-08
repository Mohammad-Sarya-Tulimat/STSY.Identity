using System;

namespace STSY.Identity.Abstraction.Models.Enums
{
    [Flags]
    public enum AuthenticatorUsage
    {
        None = 0,
        Primary = 1,
        MultiFactor = 2
    }
}
