using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Repositories.Interfaces;

namespace BookMyDoctor.Server.Repositories.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IBmdRepository<User> Users { get; }
        IBmdRepository<Doctor> Doctors { get; }
        IBmdRepository<Patient> Patients { get; }
        IBmdRepository<Appointment> Appointments { get; }
        IBmdRepository<DoctorClinic> DoctorClinics { get; }
        IBmdRepository<Clinic> Clinics { get; }
        IBmdRepository<Payment> Payments { get; }
        IBmdRepository<Notification> Notifications { get; }
        IBmdRepository<Message> Messages { get; }
        IBmdRepository<PatientMedicalHistory> PatientMedicalHistories { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}