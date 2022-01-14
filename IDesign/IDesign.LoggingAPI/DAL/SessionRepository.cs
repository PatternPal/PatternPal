using IDesign.LoggingAPI.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDesign.LoggingAPI.DAL
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(LoggingContext context) : base(context) { }
        public Session GetById(Guid id)
        {
            return _context.Set<Session>().Find(id);
        }
        public void ChangeEndSession(Guid sessionId, DateTime endSession)
        {
            Session session = GetById(sessionId);
            if (session == null || session.StartSession > endSession)
            {
                throw new Exception();
            }
            session.EndSession = endSession;
            Update(session);
        }
    }
}
