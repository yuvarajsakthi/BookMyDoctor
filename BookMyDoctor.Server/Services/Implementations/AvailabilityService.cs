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

        public async Task<IEnumerable<object>> GetAvailableSlotsAsync(int doctorId, string date)
        {
            if (!DateOnly.TryParse(date, out var selectedDate))
                return new List<object>();

            var dayOfWeek = selectedDate.DayOfWeek;
            var availability = await _unitOfWork.Availabilities.FindAsync(
                a => a.DoctorId == doctorId && a.DayOfWeek == dayOfWeek && a.IsActive);

            if (!availability.Any())
                return new List<object>();

            var bookedSlots = await _unitOfWork.Appointments.FindAsync(
                a => a.DoctorId == doctorId && a.AppointmentDate == selectedDate);

            var slots = new List<object>();
            foreach (var avail in availability)
            {
                var currentTime = avail.StartTime;
                while (currentTime < avail.EndTime)
                {
                    var isBooked = bookedSlots.Any(b => b.StartTime == currentTime);
                    if (!isBooked)
                    {
                        slots.Add(new
                        {
                            value = currentTime.ToString("HH:mm"),
                            display = currentTime.ToString("hh:mm tt")
                        });
                    }
                    currentTime = currentTime.AddMinutes(30);
                }
            }

            return slots;
        }

        public async Task AddAvailabilityAsync(int doctorId, AvailabilityDto request)
        {
            var availability = new Availability
            {
                DoctorId = doctorId,
                DayOfWeek = (DayOfWeek)request.DayOfWeek,
                StartTime = TimeOnly.ParseExact(request.StartTime, "HH:mm"),
                EndTime = TimeOnly.ParseExact(request.EndTime, "HH:mm"),
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