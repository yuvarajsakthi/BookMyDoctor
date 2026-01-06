using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;
using AutoMapper;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AvailabilityService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AvailabilityResponseDto>> GetDoctorAvailabilityAsync(int doctorId)
        {
            var availability = await _unitOfWork.Availabilities.FindAsync(a => a.DoctorId == doctorId && a.IsActive);
            return _mapper.Map<IEnumerable<AvailabilityResponseDto>>(availability);
        }

        public async Task AddAvailabilityAsync(int doctorId, AvailabilityDto request)
        {
            var availability = new Availability
            {
                DoctorId = doctorId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsActive = true
            };

            await _unitOfWork.Availabilities.AddAsync(availability);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAvailabilityAsync(int availabilityId)
        {
            var availability = await _unitOfWork.Availabilities.GetByIdAsync(availabilityId);
            if (availability != null)
            {
                availability.IsActive = false;
                await _unitOfWork.Availabilities.UpdateAsync(availability);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}