using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatternPal.LoggingAPI.DAL.Interfaces
{
    public interface IModeRepository : IGenericRepository<Mode>
    {
        public Mode GetById(string id);
    }
}
