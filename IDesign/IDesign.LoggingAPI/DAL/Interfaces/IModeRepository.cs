using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDesign.LoggingAPI.DAL.Interfaces
{
    public interface IModeRepository : IGenericRepository<Mode>
    {
        public Mode GetById(string id);
    }
}
