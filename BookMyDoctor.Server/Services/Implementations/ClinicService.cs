using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class ClinicService : IClinicService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClinicService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Clinic>> GetClinicsAsync()
        {
            return await _unitOfWork.Clinics.GetAllAsync();
        }

        public async Task<Clinic> GetClinicByIdAsync(int id)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(id);
            if (clinic == null) throw new ArgumentException("Clinic not found");
            return clinic;
        }

        public async Task<Clinic> CreateClinicAsync(ClinicCreateDto request)
        {
            var clinic = new Clinic
            {
                ClinicName = request.ClinicName,
                Address = request.Address,
                City = request.City,
                State = request.State,
                Country = request.Country,
                ZipCode = request.ZipCode,
                IsActive = true
            };

            await _unitOfWork.Clinics.AddAsync(clinic);
            await _unitOfWork.SaveChangesAsync();
            return clinic;
        }

        public async Task UpdateClinicAsync(int id, ClinicUpdateDto request)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(id);
            if (clinic == null) throw new ArgumentException("Clinic not found");

            if (request.ClinicName != null) clinic.ClinicName = request.ClinicName;
            if (request.Address != null) clinic.Address = request.Address;
            if (request.City != null) clinic.City = request.City;
            if (request.State != null) clinic.State = request.State;
            if (request.Country != null) clinic.Country = request.Country;
            if (request.ZipCode != null) clinic.ZipCode = request.ZipCode;
            if (request.IsActive.HasValue) clinic.IsActive = request.IsActive.Value;

            await _unitOfWork.Clinics.UpdateAsync(clinic);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteClinicAsync(int id)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(id);
            if (clinic == null) throw new ArgumentException("Clinic not found");

            clinic.IsActive = false;
            await _unitOfWork.Clinics.UpdateAsync(clinic);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}