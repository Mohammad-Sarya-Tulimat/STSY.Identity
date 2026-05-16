using Microsoft.EntityFrameworkCore;
using STSY.Identity.Abstraction.Contract.Managers;
using STSY.Identity.Abstraction.Contract.Models.Sessions;
using STSY.Identity.Abstraction.Models.Output;
using STSY.Microsoft.Identity.DBContext;
using STSY.Microsoft.Identity.Mappers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace STSY.Microsoft.Identity.Services
{
    public class SessionStorage : ISessionStorage
    {
        STSYIdentityDbContext _dbcontext;
        public SessionStorage(STSYIdentityDbContext sTSYIdentityDbContext)
        {
            this._dbcontext = sTSYIdentityDbContext;
        }
        public async Task<STSYIdentityResult> AddSession(string userId, UserSession session, IDictionary<string, object> sessionProtectionData, CancellationToken cancellationToken)
        {
            this._dbcontext.UserSessions.Add(new Models.MicrosoftIdentityUserSession
            {
                Id = session.Id,
                UserId = session.UserId,
                SessionType = session.SessionType,
                IpAddress = session.IpAddress,
                Location = session.Location,
                CreatedAt = session.CreatedAt,
                ExpiredAt = session.ExpiredAt,
                ProtectedData = sessionProtectionData,
            });
            await this._dbcontext.SaveChangesAsync(cancellationToken);
            return STSYIdentityResult.SuccessResult;
        }

        public async Task<UserSession> GetSession(string sessionId, CancellationToken cancellationToken)
        {
            var session = await this._dbcontext.UserSessions.FindAsync(sessionId);
            if (session == null) return null;
            return session.AsUserSession();
        }

        public async Task<IDictionary<string, object>> GetSessionProtectedData(string sessionId, CancellationToken cancellationToken)
        {
            var session = await this._dbcontext.UserSessions.FindAsync(sessionId);
            if (session == null) return null;
            return session.ProtectedData;
        }

        public async Task<IEnumerable<UserSession>> ListSession(string userId, CancellationToken cancellationToken)
        {
            return await this._dbcontext.UserSessions.AsUserSession().Where(s => s.UserId.Equals(userId)).ToListAsync(cancellationToken);
        }

        public async Task<STSYIdentityResult> RemoveSession(string userId, string sessionId, CancellationToken cancellationToken)
        {
            var result = await this._dbcontext.UserSessions.Where(s => s.UserId.Equals(userId) && s.Id.Equals(sessionId)).ExecuteDeleteAsync(cancellationToken);
            if (result > 0)
                return STSYIdentityResult.SuccessResult;
            else return STSYIdentityResult.BuildFailure("cannot find session");
        }

        public async Task<STSYIdentityResult> UpdateSession(string userId, UserSession session, IDictionary<string, object> sessionProtectionData, CancellationToken cancellationToken)
        {
            var dbSession = await this._dbcontext.UserSessions.FindAsync(session.Id);
            if (dbSession == null) return STSYIdentityResult.BuildFailure("cannot find session");
            if (!string.Equals(dbSession.UserId, userId)) return STSYIdentityResult.BuildFailure("cannot find session");
            dbSession.Location = session.Location;
            dbSession.IpAddress = session.IpAddress;
            dbSession.ExpiredAt = session.ExpiredAt;
            dbSession.SessionType = session.SessionType;
            dbSession.ProtectedData = sessionProtectionData;
            await this._dbcontext.SaveChangesAsync(cancellationToken);
            return STSYIdentityResult.SuccessResult;
        }
    }
}
