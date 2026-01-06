using BookMyDoctor.Server.Data;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Repositories.Implementations;
using BookMyDoctor.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace BookMyDoctor.Server.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BmdContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        public UnitOfWork(BmdContext context)
        {
            _context = context;
            Users = new BmdRepository<User>(_context);
            Appointments = new BmdRepository<Appointment>(_context);
            Availabilities = new BmdRepository<Availability>(_context);
            Clinics = new BmdRepository<Clinic>(_context);
            Payments = new BmdRepository<Payment>(_context);
            Notifications = new BmdRepository<Notification>(_context);
        }

        public IBmdRepository<User> Users { get; }
        public IBmdRepository<Appointment> Appointments { get; }
        public IBmdRepository<Availability> Availabilities { get; }
        public IBmdRepository<Clinic> Clinics { get; }
        public IBmdRepository<Payment> Payments { get; }
        public IBmdRepository<Notification> Notifications { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _transaction?.Dispose();
                    _context.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}