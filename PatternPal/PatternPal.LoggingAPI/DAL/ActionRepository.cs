using PatternPal.LoggingAPI.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatternPal.LoggingAPI.DAL
{
    public class ActionRepository : GenericRepository<Action>, IActionRepository
    {
        public ActionRepository(LoggingContext context) : base(context) { }
    }
}
