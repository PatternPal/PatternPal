using PatternPal.LoggingAPI.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatternPal.LoggingAPI.DAL
{
    public class ActionTypeRepository : GenericRepository<ActionType>, IActionTypeRepository
    {
        public ActionTypeRepository(LoggingContext context) : base(context) { }
        public ActionType GetById(string id)
        {
            return _context.ActionTypes.Find(id);
        }
    }
}
