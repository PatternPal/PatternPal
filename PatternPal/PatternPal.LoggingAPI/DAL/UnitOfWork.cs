using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PatternPal.LoggingAPI.DAL
{
    public class UnitOfWork : IDisposable
    {
        private readonly LoggingContext _context;
        private bool _disposed = false;
        private ActionRepository _actionRepository;
        private ActionTypeRepository _actionTypeRepository;
        private ExtensionErrorRepository _extensionErrorRepository;
        private ModeRepository _modeRepository;
        private SessionRepository _sessionRepository;
        public ActionRepository ActionRepository
        {
            get
            {
                if (_actionRepository == null)
                    _actionRepository = new ActionRepository(_context);
                return _actionRepository;
            }
        }
        public ActionTypeRepository ActionTypeRepository
        {
            get
            {
                if (_actionTypeRepository == null)
                    _actionTypeRepository = new ActionTypeRepository(_context);
                return _actionTypeRepository;
            }
        }
        public ExtensionErrorRepository ExtensionErrorRepository
        {
            get
            {
                if (_extensionErrorRepository == null)
                    _extensionErrorRepository = new ExtensionErrorRepository(_context);
                return _extensionErrorRepository;
            }
        }
        public ModeRepository ModeRepository
        {
            get
            {
                if (_modeRepository == null)
                    _modeRepository = new ModeRepository(_context);
                return _modeRepository;
            }
        }
        public SessionRepository SessionRepository
        {
            get
            {
                if (_sessionRepository == null)
                    _sessionRepository = new SessionRepository(_context);
                return _sessionRepository;
            }
        }
        public UnitOfWork(LoggingContext context)
        {
            _context = context;
        }
        /// <summary>
        /// save changed
        /// </summary>
        /// <exception cref="DbUpdateException"></exception>
        /// <exception cref="DbUpdateConcurrencyException"></exception>
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }
    }
}
