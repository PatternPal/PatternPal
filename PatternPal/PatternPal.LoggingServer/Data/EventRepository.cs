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
        /// <summary>
        /// Returns list of all events
        /// </summary>
        /// <returns></returns>
        public async Task<List<ProgSnap2Event>> GetAll()
        {
            return await _context.Events.ToListAsync();
        }

        /// <summary>
        /// Returns event by id or null if not found
        /// </summary>
        /// <param name="id">ID of Progsnap2Event</param>
        /// <returns> Event or null if not found </returns>
        public async Task<ProgSnap2Event?> GetById(string id)
        {
            return await _context.Events.FindAsync(id);
        }

        /// <summary>
        /// Inserts event into database asynchronously and returns the inserted event.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<ProgSnap2Event> Insert(ProgSnap2Event entity)
        {
            await _context.Events.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates event in database asynchronously and returns the updated event.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>ProgSnap2Event</returns>
        public async Task<ProgSnap2Event> Update(ProgSnap2Event entity)
        {
            _context.Events.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Delete event from database asynchronously by id and returns void. If entity is not found, throws exception.
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="Exception"></exception>
        public async Task Delete(string id)
        {
            ProgSnap2Event? entity = await _context.Events.FindAsync(id);

            if (entity == null)
            {
                throw new Exception("Entity not found");
            }
            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Generic method to get event by column name and value. This can be used to get events by any column instead of just id. Only caveat is that the column name must be case-sensitive and it must be a string.
        /// </summary>
        /// <param name="column">Column name (Case-Sensitive) </param>
        /// <param name="value">Column value</param>
        /// <returns cref="ProgSnap2Event?"></returns>
        public async Task<ProgSnap2Event?> GetBy(string column, string value)
        {
            return await _context.Events.FirstOrDefaultAsync(e => EF.Property<string>(e, column) == value);
        }

        /// <summary>
        /// Same filter as GetBy, but returns a list of all matching events.
        /// TODO: This is a temporary solution. We should be able to filter by any column, not just string columns.
        /// TODO: Limit, Offset to prevent large queries
        /// </summary>
        /// <param name="column">Column name (Case-Sensitive) </param>
        /// <param name="value">Column value</param>
        /// <returns></returns>
        public async Task<List<ProgSnap2Event>> GetAllBy(string column, string value)
        {
            return await _context.Events.Where(e => EF.Property<string>(e, column) == value).ToListAsync();
        }


        /// <summary>
        /// To get correct event order, we need to get the last event in the session and subject and increment the order by 1.
        /// In case there are no events in the session and subject, we return 0.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="subjectId"></param>
        /// <returns cref="int"></returns>
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
