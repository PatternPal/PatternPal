using PatternPal.LoggingAPI.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatternPal.LoggingAPI.DAL
{
    public class ModeRepository : GenericRepository<Mode>, IModeRepository
    {
        public ModeRepository(LoggingContext context) : base(context) { }

        public Mode GetById(string id)
        {
            return _context.Modes.Find(id);
        }
    }
}
