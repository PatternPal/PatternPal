using IDesign.LoggingAPI.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IDesign.LoggingAPI.DAL
{
    public class ExtensionErrorRepository : GenericRepository<ExtensionError>, IExtensionErrorRepository
    {
        public ExtensionErrorRepository(LoggingContext context) : base(context) { }
    }
}
