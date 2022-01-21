using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatternPal.LoggingAPI.DAL.Interfaces
{
    public interface IActionTypeRepository : IGenericRepository<ActionType>
    {
        public ActionType GetById(string id);
    }
}
