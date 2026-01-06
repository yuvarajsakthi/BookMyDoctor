using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IClinicService
    {
        Task<IEnumerable<Clinic>> GetClinicsAsync();
        Task<Clinic> GetClinicByIdAsync(int id);
        Task<Clinic> CreateClinicAsync(ClinicCreateDto request);
        Task UpdateClinicAsync(int id, ClinicUpdateDto request);
        Task DeleteClinicAsync(int id);
    }
}