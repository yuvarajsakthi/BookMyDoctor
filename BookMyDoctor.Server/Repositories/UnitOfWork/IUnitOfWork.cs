using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Repositories.Interfaces;

namespace BookMyDoctor.Server.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBmdRepository<User> Users { get; }
        IBmdRepository<Appointment> Appointments { get; }
        IBmdRepository<Availability> Availabilities { get; }
        IBmdRepository<Clinic> Clinics { get; }
        IBmdRepository<Payment> Payments { get; }
        IBmdRepository<Notification> Notifications { get; }
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}