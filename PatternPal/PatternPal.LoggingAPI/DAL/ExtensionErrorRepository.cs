using PatternPal.LoggingAPI.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PatternPal.LoggingAPI.DAL
{
    public class ExtensionErrorRepository : GenericRepository<ExtensionError>, IExtensionErrorRepository
    {
        public ExtensionErrorRepository(LoggingContext context) : base(context) { }
    }
}
