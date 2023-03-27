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
            var entity = await _context.Events.FindAsync(id);

            if (entity == null)
            {
                throw new Exception("Entity not found");
            }
            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }


    }
}
