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
        /// <returns cref="List{ProgSnap2Event}">Returns all events asynchronously </returns>
        public virtual async Task<List<ProgSnap2Event>> GetAll()
        {
            return await _context.Events.ToListAsync();
        }

        /// <summary>
        /// Returns event by ID or null if not found
        /// </summary>
        /// <param name="id">ID of ProgSnap2Event</param>
        /// <returns cref="ProgSnap2Event?"> Returns the requested Event in case it exists</returns>
        public virtual async Task<ProgSnap2Event?> GetById(string id)
        {
            return await _context.Events.FindAsync(id);
        }

        /// <summary>
        /// Inserts event into database asynchronously and returns the inserted event. This also generates all values that are database generated.
        /// </summary>
        /// <param name="entity" cref="ProgSnap2Event">Entity that has not been written to the database.</param>
        /// <returns cref="ProgSnap2Event">Returns the saved entity</returns>
        public virtual async Task<ProgSnap2Event> Insert(ProgSnap2Event entity)
        {
            await _context.Events.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Updates event in database asynchronously and returns the updated event.
        /// </summary>
        /// <param name="entity">Entity that does exist in database to update</param>
        /// <returns cref="ProgSnap2Event">Updated entity</returns>
        public virtual async Task<ProgSnap2Event> Update(ProgSnap2Event entity)
        {
            _context.Events.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Delete event from database asynchronously by ID and returns void. If entity is not found, throws exception.
        /// </summary>
        /// <param name="id">ID of entity to delete</param>
        /// <exception cref="Exception">Entity does not exist in database</exception>
        public virtual async Task Delete(string id)
        {
            ProgSnap2Event? entity = await _context.Events.FindAsync(id);

            if (entity == null)
            {
                throw new Exception("Entity not found"); // TODO: add custom exception
            }
            _context.Events.Remove(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Generic method to get event by column name and value. This can be used to get events by any column instead of just ID. Only caveat is that the column name must be case-sensitive and it must be a string.
        /// </summary>
        /// <param name="column">Column name (Case-Sensitive) </param>
        /// <param name="value">Column value</param>
        /// <returns cref="ProgSnap2Event?"></returns>
        public virtual async Task<ProgSnap2Event?> GetBy(string column, string value)
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
        public virtual async Task<List<ProgSnap2Event>> GetAllBy(string column, string value)
        {
            return await _context.Events.Where(e => EF.Property<string>(e, column) == value).ToListAsync();
        }


        /// <summary>
        /// To get correct event order, we need to get the last event in the session and subject and increment the order by 1.
        /// In case there are no events in the session and subject, we return 0.
        /// </summary>
        /// <param name="sessionId">SessionId of event</param>
        /// <param name="subjectId">SubjectId of event</param>
        /// <returns cref="int">Next order value</returns>
        public virtual async Task<int> GetNextOrder(Guid sessionId, Guid subjectId)
        {
            ProgSnap2Event? lastEvent = await _context.Events.Where(e => e.SessionId == sessionId && e.SubjectId == subjectId).OrderByDescending(e => e.Order).FirstOrDefaultAsync();
            if (lastEvent == null)
            {
                return 0;
            }
            return lastEvent.Order + 1;
        }
        
        /// <summary>
        /// To get correct event order, we need to get the last event in the session and subject and increment the order by 1.
        /// </summary>
        /// <param name="sessionId">SessionId of event</param>
        /// <param name="subjectId">SubjectId of event</param>
        /// <param name="projectId">ProjectId of event</param>
        /// <returns></returns>
        public virtual async Task<Guid> GetPreviousCodeState(Guid sessionId, Guid subjectId, string projectId)
        {
            ProgSnap2Event? lastEvent = await _context.Events.Where(e => e.SessionId == sessionId && e.SubjectId == subjectId && e.ProjectId == projectId).OrderByDescending(e => e.Order).FirstOrDefaultAsync();
            if (lastEvent == null)
            {
                return Guid.Empty;
            }
            return lastEvent.CodeStateId;
        }

        /// <summary>
        /// Returns list of all unique code states in the database.
        /// </summary>
        /// <returns cref="List{Guid}">List of all unique code states</returns>
        public async Task<List<Guid>> GetUniqueCodeStates(){
            List<Guid> codeStates = await _context.Events.Select(e => e.CodeStateId).Distinct().ToListAsync();
            return codeStates;
        }
    }
}
