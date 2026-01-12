using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
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
                ClinicId = request.ClinicId,
                IsActive = true
            };

            await _unitOfWork.Availabilities.AddAsync(availability);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveAvailabilityAsync(int id)
        {
            var availability = await _unitOfWork.Availabilities.GetByIdAsync(id);
            if (availability != null)
            {
                availability.IsActive = false;
                await _unitOfWork.Availabilities.UpdateAsync(availability);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<object>> GetDoctorBreaksAsync(int doctorId)
        {
            // Use existing Availability model with a flag or different approach
            // For now, return breaks as separate availability entries
            var breaks = await _unitOfWork.Availabilities.FindAsync(a => a.DoctorId == doctorId && a.IsActive);
            // Filter or mark breaks differently - for simplicity, return empty for now
            return new List<object>();
        }

        public async Task AddBreakAsync(int doctorId, DoctorBreakDto request)
        {
            // Add break as a special availability entry
            var breakAvailability = new Availability
            {
                DoctorId = doctorId,
                DayOfWeek = (DayOfWeek)request.DayOfWeek,
                StartTime = TimeOnly.ParseExact(request.StartTime, "HH:mm"),
                EndTime = TimeOnly.ParseExact(request.EndTime, "HH:mm"),
                IsActive = true
            };

            await _unitOfWork.Availabilities.AddAsync(breakAvailability);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveBreakAsync(int id)
        {
            var availability = await _unitOfWork.Availabilities.GetByIdAsync(id);
            if (availability != null)
            {
                availability.IsActive = false;
                await _unitOfWork.Availabilities.UpdateAsync(availability);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<object>> GetDoctorDaysOffAsync(int doctorId)
        {
            // Use appointments with a special status or reason to track days off
            var daysOff = await _unitOfWork.Appointments.FindAsync(a => 
                a.DoctorId == doctorId && 
                a.Reason != null && 
                a.Reason.StartsWith("DAY_OFF:"));
            
            return daysOff.Select(d => new {
                id = d.AppointmentId,
                date = d.AppointmentDate.ToString("yyyy-MM-dd"),
                reason = d.Reason?.Replace("DAY_OFF:", "")
            });
        }

        public async Task AddDayOffAsync(int doctorId, DoctorDayOffDto request)
        {
            // Get the first clinic associated with the doctor
            var doctorAvailability = await _unitOfWork.Availabilities.FindAsync(a => a.DoctorId == doctorId && a.ClinicId.HasValue);
            var clinicId = doctorAvailability.FirstOrDefault()?.ClinicId ?? 1; // Default to clinic 1 if none found
            
            // Create a special appointment entry to mark day off
            var dayOffAppointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = doctorId, // Use doctor as patient for day off entries
                ClinicId = clinicId,
                AppointmentDate = DateOnly.ParseExact(request.Date, "yyyy-MM-dd"),
                StartTime = TimeOnly.MinValue,
                EndTime = TimeOnly.MaxValue,
                Status = AppointmentStatus.Cancelled,
                Reason = $"DAY_OFF:{request.Reason ?? "Day Off"}"
            };

            await _unitOfWork.Appointments.AddAsync(dayOffAppointment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveDayOffAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment != null && appointment.Reason?.StartsWith("DAY_OFF:") == true)
            {
                await _unitOfWork.Appointments.DeleteAsync(appointment);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task<object> GetAllAvailabilityDataAsync(int doctorId)
        {
            var availability = await _unitOfWork.Availabilities.FindAsync(a => a.DoctorId == doctorId && a.IsActive);
            var daysOff = await _unitOfWork.Appointments.FindAsync(a => 
                a.DoctorId == doctorId && 
                a.Reason != null && 
                a.Reason.StartsWith("DAY_OFF:"));
            
            return new
            {
                availability = availability.Select(a => new { 
                    id = a.AvailabilityId, 
                    clinicId = a.ClinicId, 
                    dayOfWeek = (int)a.DayOfWeek, 
                    startTime = a.StartTime.ToString(@"HH\:mm"), 
                    endTime = a.EndTime.ToString(@"HH\:mm") 
                }),
                breaks = new List<object>(), // Breaks can be handled separately if needed
                daysOff = daysOff.Select(d => new {
                    id = d.AppointmentId,
                    date = d.AppointmentDate.ToString("yyyy-MM-dd"),
                    reason = d.Reason?.Replace("DAY_OFF:", "")
                })
            };
        }

        public async Task<IEnumerable<object>> GetAllDoctorsAvailabilityAsync()
        {
            var availability = await _unitOfWork.Availabilities.FindAsync(a => a.IsActive);
            var doctors = await _unitOfWork.Users.FindAsync(u => u.UserRole == UserRole.Doctor);
            
            return availability.Select(a => new {
                availabilityId = a.AvailabilityId,
                doctorId = a.DoctorId,
                doctorName = doctors.FirstOrDefault(d => d.UserId == a.DoctorId)?.UserName ?? "Unknown Doctor",
                dayOfWeek = (int)a.DayOfWeek,
                startTime = a.StartTime.ToString(@"hh\:mm"),
                endTime = a.EndTime.ToString(@"hh\:mm"),
                isActive = a.IsActive
            });
        }
    }
}