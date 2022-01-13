using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDesign.LoggingAPI.DAL.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        public void ChangeEndSession(Guid sessionId, DateTime endSession);
    }
}
