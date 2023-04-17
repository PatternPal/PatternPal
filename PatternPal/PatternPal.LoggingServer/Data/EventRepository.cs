using Microsoft.EntityFrameworkCore;
using PatternPal.LoggingServer.Data.Interfaces;
using PatternPal.LoggingServer.Models;

namespace PatternPal.LoggingServer.Data
{
    public class EventRepository : IRepository<ProgSnap2Event>
    {
        private readonly ProgSnap2ContextClass _context;

        public EventRepository(ProgSnap2ContextClass context)
        {
            _context = context;
        }

        public async Task<List<ProgSnap2Event>> GetAll()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<ProgSnap2Event?> GetById(string id)
        {
            return await _context.Events.FindAsync(id);
        }

        public async Task<ProgSnap2Event> Insert(ProgSnap2Event entity)
        {
            await _context.Events.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ProgSnap2Event> Update(ProgSnap2Event entity)
        {
            _context.Events.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ProgSnap2Event> Delete(string id)
        {
            ProgSnap2Event? entity = await _context.Events.FindAsync(id);

            if (entity == null)
            {
                throw new Exception("Entity not found");
            }
            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ProgSnap2Event?> GetBy(string column, string value)
        {
            return await _context.Events.FirstOrDefaultAsync(e => EF.Property<string>(e, column) == value);
        }

        public async Task<List<ProgSnap2Event>> GetAllBy(string column, string value)
        {
            return await _context.Events.Where(e => EF.Property<string>(e, column) == value).ToListAsync();
        }

        public async Task<int> GetNextOrder(Guid sessionId, Guid subjectId)
        {
            ProgSnap2Event? lastEvent = await _context.Events.Where(e => e.SessionId == sessionId && e.SubjectId == subjectId).OrderByDescending(e => e.Order).FirstOrDefaultAsync();
            if (lastEvent == null)
            {
                return 0;
            }
            return lastEvent.Order + 1;
        }
    }
}
