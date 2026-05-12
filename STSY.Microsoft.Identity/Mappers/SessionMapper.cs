using STSY.Identity.Abstraction.Models.Output.Sessions;
using STSY.Microsoft.Identity.Models;
using System.Collections.Generic;
using System.Linq;

namespace STSY.Microsoft.Identity.Mappers
{
    internal static class SessionMapper
    {
        internal static UserSession AsUserSession(this MicrosoftIdentityUserSession session)
        {
            return new UserSession
            {
                Id = session.Id,
                UserId = session.UserId,
                SessionType = session.SessionType,
                IpAddress = session.IpAddress,
                Location = session.Location,
                CreatedAt = session.CreatedAt,
                ExpiredAt = session.ExpiredAt,
            };
        }
        internal static IEnumerable<UserSession> AsUserSession(this IEnumerable<MicrosoftIdentityUserSession> sessions)
        {
            return sessions.Select(s => s.AsUserSession());
        }
        internal static IQueryable<UserSession> AsUserSession(this IQueryable<MicrosoftIdentityUserSession> sessions)
        {
            return sessions.Select(session =>
            new UserSession
            {
                Id = session.Id,
                UserId = session.UserId,
                SessionType = session.SessionType,
                IpAddress = session.IpAddress,
                Location = session.Location,
                CreatedAt = session.CreatedAt,
                ExpiredAt = session.ExpiredAt,
            });
        }
    }
}
