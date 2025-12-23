using AutoMapper;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public DoctorService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<DoctorResponseDto?> GetProfileAsync(int doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            return doctor != null ? _mapper.Map<DoctorResponseDto>(doctor) : null;
        }

        public async Task UpdateProfileAsync(int doctorId, DoctorUpdateDto request)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            if (doctor == null) throw new ArgumentException("Doctor not found");

            // Get the associated user
            var user = await _unitOfWork.Users.GetByIdAsync(doctor.UserId);
            if (user == null) throw new ArgumentException("User not found");

            // Handle profile picture upload
            if (request.ProfilePicture != null)
            {
                var uploadResult = await _cloudinaryService.UploadProfilePhotoAsync(
                    request.ProfilePicture, 
                    doctor.UserId.ToString()
                );
                
                if (uploadResult.Error == null)
                {
                    user.ProfileUrl = uploadResult.SecureUrl.ToString();
                    user.ProfilePhotoPublicId = uploadResult.PublicId;
                }
            }

            // Update user fields
            if (request.UserName != null) user.UserName = request.UserName;
            if (request.Phone != null) user.Phone = request.Phone;
            if (request.Gender.HasValue) user.Gender = request.Gender.Value;
            user.UpdatedAt = DateTime.UtcNow;
            
            // Update doctor fields
            if (request.Specialty != null) doctor.Specialty = request.Specialty;
            if (request.ExperienceYears.HasValue) doctor.ExperienceYears = request.ExperienceYears.Value;
            if (request.ConsultationFee.HasValue) doctor.ConsultationFee = request.ConsultationFee.Value;
            if (request.Bio != null) doctor.Bio = request.Bio;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.Doctors.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ClinicResponseDto>> GetAssignedClinicsAsync(int doctorId)
        {
            var doctorClinics = await _unitOfWork.DoctorClinics.FindAsync(dc => dc.DoctorId == doctorId);
            var clinicIds = doctorClinics.Select(dc => dc.ClinicId);
            var clinics = await _unitOfWork.Clinics.FindAsync(c => clinicIds.Contains(c.ClinicId));
            return _mapper.Map<IEnumerable<ClinicResponseDto>>(clinics);
        }

        public async Task SetAvailabilityAsync(int doctorId, SetAvailabilityDto request)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            if (doctor == null) throw new ArgumentException("Doctor not found");

            // Convert day of week string to byte (0 = Sunday, 1 = Monday, etc.)
            var dayOfWeekByte = request.DayOfWeek.ToLower() switch
            {
                "sunday" => (byte)0,
                "monday" => (byte)1,
                "tuesday" => (byte)2,
                "wednesday" => (byte)3,
                "thursday" => (byte)4,
                "friday" => (byte)5,
                "saturday" => (byte)6,
                _ => throw new ArgumentException("Invalid day of week")
            };

            doctor.ClinicId = request.ClinicId;
            doctor.DayOfWeek = dayOfWeekByte;
            doctor.StartTime = request.StartTime;
            doctor.EndTime = request.EndTime;
            
            await _unitOfWork.Doctors.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkDayOffAsync(int doctorId, DayOffDto request)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => 
                a.DoctorId == doctorId && a.AppointmentDate == request.Date);

            foreach (var appointment in appointments)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                appointment.Reason = request.Reason ?? "Doctor unavailable";
                await _unitOfWork.Appointments.UpdateAsync(appointment);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AcceptAppointmentAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null) throw new ArgumentException("Appointment not found");

            appointment.Status = AppointmentStatus.Booked;
            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeclineAppointmentAsync(int appointmentId, string? reason)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null) throw new ArgumentException("Appointment not found");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.Reason = reason;
            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task FreeBlockedSlotAsync(int blockId)
        {
            await _unitOfWork.Appointments.DeleteByIdAsync(blockId);
            await _unitOfWork.SaveChangesAsync();
        }
        
        public async Task<int> GetDoctorIdByUserIdAsync(int userId)
        {
            var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            return doctor?.DoctorId ?? 0;
        }

        public async Task<BlockedAppointmentResponseDto> BlockTimeSlotAsync(int doctorId, BlockTimeSlotDto request)
        {
            var blockedAppointment = _mapper.Map<Appointment>(request);
            blockedAppointment.DoctorId = doctorId;
            blockedAppointment.Status = AppointmentStatus.Blocked;
            blockedAppointment.Reason ??= "Time slot blocked by doctor";
            
            await _unitOfWork.Appointments.AddAsync(blockedAppointment);
            await _unitOfWork.SaveChangesAsync();
            
            return _mapper.Map<BlockedAppointmentResponseDto>(blockedAppointment);
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAvailabilityCalendarAsync(int doctorId, int? clinicId, string? month)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.DoctorId == doctorId);
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetTodayAppointmentsAsync(int doctorId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.DoctorId == doctorId && a.AppointmentDate == today);
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentsAsync(int doctorId, DateOnly? date, AppointmentStatus? status)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => 
                a.DoctorId == doctorId && 
                (!date.HasValue || a.AppointmentDate == date) &&
                (!status.HasValue || a.Status == status));
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<DoctorDashboardDto> GetDashboardSummaryAsync(int doctorId)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.DoctorId == doctorId);
            var today = DateOnly.FromDateTime(DateTime.Today);
            
            return new DoctorDashboardDto
            {
                TotalAppointments = appointments.Count(),
                TodayAppointments = appointments.Count(a => a.AppointmentDate == today),
                PendingAppointments = appointments.Count(a => a.Status == AppointmentStatus.Pending),
                CompletedAppointments = appointments.Count(a => a.Status == AppointmentStatus.Completed),
                TotalEarnings = 0,
                AverageRating = 0,
                TotalPatients = appointments.Select(a => a.PatientId).Distinct().Count()
            };
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentAnalyticsAsync(int doctorId, DateOnly from, DateOnly to)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => 
                a.DoctorId == doctorId && 
                a.AppointmentDate >= from && 
                a.AppointmentDate <= to);
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetCancellationAnalyticsAsync(int doctorId)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => 
                a.DoctorId == doctorId && 
                a.Status == AppointmentStatus.Cancelled);
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<IEnumerable<object>> GetNotificationsAsync(int doctorId)
        {
            var pendingAppointments = await _unitOfWork.Appointments.FindAsync(a => 
                a.DoctorId == doctorId && a.Status == AppointmentStatus.Pending);
            return pendingAppointments.Select(a => new { 
                Type = "appointment_request", 
                Message = $"New appointment request from patient", 
                AppointmentId = a.AppointmentId,
                Date = a.AppointmentDate 
            });
        }

        public async Task<byte[]> ExportReportsAsync(int doctorId, string type, string format)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.DoctorId == doctorId);
            var reportData = System.Text.Json.JsonSerializer.Serialize(appointments.Select(a => new {
                a.AppointmentId,
                a.AppointmentDate,
                a.StartTime,
                a.EndTime,
                a.Status,
                a.Reason
            }));
            return System.Text.Encoding.UTF8.GetBytes(reportData);
        }
    }
}