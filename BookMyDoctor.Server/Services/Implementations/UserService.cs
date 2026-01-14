using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;
using AutoMapper;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IEnumerable<UserResponseDto>> GetDoctorsAsync()
        {
            var doctors = await _unitOfWork.Users.FindAsync(u => u.UserRole == UserRole.Doctor && u.IsApproved);
            return _mapper.Map<IEnumerable<UserResponseDto>>(doctors);
        }

        public async Task<IEnumerable<UserResponseDto>> GetPatientsAsync()
        {
            var patients = await _unitOfWork.Users.FindAsync(u => u.UserRole == UserRole.Patient);
            return _mapper.Map<IEnumerable<UserResponseDto>>(patients);
        }

        public async Task<UserResponseDto> GetDoctorByIdAsync(int id)
        {
            var doctor = await _unitOfWork.Users.GetByIdAsync(id);
            if (doctor?.UserRole != UserRole.Doctor) throw new ArgumentException("Doctor not found");
            return _mapper.Map<UserResponseDto>(doctor);
        }

        public async Task<UserResponseDto> GetPatientByIdAsync(int id)
        {
            var patient = await _unitOfWork.Users.GetByIdAsync(id);
            if (patient?.UserRole != UserRole.Patient) throw new ArgumentException("Patient not found");
            return _mapper.Map<UserResponseDto>(patient);
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) throw new ArgumentException("User not found");
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<IEnumerable<UserResponseDto>> SearchDoctorsAsync(string? specialty, string? location, DateTime? date)
        {
            var doctors = await _unitOfWork.Users.FindAsync(u => u.UserRole == UserRole.Doctor && u.IsApproved && u.IsActive);
            
            if (!string.IsNullOrEmpty(specialty))
                doctors = doctors.Where(d => d.Specialty?.Contains(specialty, StringComparison.OrdinalIgnoreCase) == true);

            // For location filtering, we'd need to join with clinic data
            // For now, we'll skip location filtering as it requires clinic relationship
            
            return _mapper.Map<IEnumerable<UserResponseDto>>(doctors);
        }

        public async Task<IEnumerable<UserResponseDto>> GetDoctorsByClinicAsync(int clinicId)
        {
            // For now, return all approved doctors since clinic-doctor relationship isn't defined
            // In a real scenario, you'd have a DoctorClinic junction table
            var doctors = await _unitOfWork.Users.FindAsync(u => u.UserRole == UserRole.Doctor && u.IsApproved);
            return _mapper.Map<IEnumerable<UserResponseDto>>(doctors);
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var totalPatients = await _unitOfWork.Users.CountAsync(u => u.UserRole == UserRole.Patient);
            var totalDoctors = await _unitOfWork.Users.CountAsync(u => u.UserRole == UserRole.Doctor);
            var totalAppointments = await _unitOfWork.Appointments.CountAsync();
            var pendingDoctors = await _unitOfWork.Users.FindAsync(u => u.UserRole == UserRole.Doctor && !u.IsApproved);
            
            var paidPayments = await _unitOfWork.Payments.FindAsync(p => p.PaymentStatus == PaymentStatus.Paid);
            var totalRevenue = paidPayments.Sum(p => p.Amount);

            return new DashboardSummaryDto
            {
                TotalPatients = totalPatients,
                TotalDoctors = totalDoctors,
                TotalAppointments = totalAppointments,
                TotalRevenue = totalRevenue,
                PendingDoctors = _mapper.Map<IEnumerable<UserResponseDto>>(pendingDoctors)
            };
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _unitOfWork.Appointments.GetAllAsync();
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<UserResponseDto> GetDoctorProfileAsync(int doctorId)
        {
            var doctor = await _unitOfWork.Users.GetByIdAsync(doctorId);
            if (doctor?.UserRole != UserRole.Doctor) throw new ArgumentException("Doctor not found");
            return _mapper.Map<UserResponseDto>(doctor);
        }

        public async Task<UserResponseDto> GetPatientProfileAsync(int patientId)
        {
            var patient = await _unitOfWork.Users.GetByIdAsync(patientId);
            if (patient?.UserRole != UserRole.Patient) throw new ArgumentException("Patient not found");
            return _mapper.Map<UserResponseDto>(patient);
        }

        public async Task<User?> GetUserProfileAsync(int userId)
        {
            return await _unitOfWork.Users.GetByIdAsync(userId);
        }

        public async Task UpdateUserProfileAsync(int userId, DoctorProfileUpdateDto request, IFormFile? profileImage)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            if (!string.IsNullOrEmpty(request.UserName)) user.UserName = request.UserName;
            if (!string.IsNullOrEmpty(request.Phone)) user.Phone = request.Phone;
            if (request.Gender.HasValue) user.Gender = request.Gender.Value;
            if (!string.IsNullOrEmpty(request.Specialty)) user.Specialty = request.Specialty;
            if (request.ExperienceYears.HasValue) user.ExperienceYears = request.ExperienceYears.Value;
            if (request.ConsultationFee.HasValue) user.ConsultationFee = request.ConsultationFee.Value;
            if (request.Bio != null) user.Bio = request.Bio;

            if (profileImage != null)
            {
                var uploadResult = await _cloudinaryService.UploadProfilePhotoAsync(profileImage, userId.ToString());
                if (uploadResult?.SecureUrl != null)
                {
                    if (!string.IsNullOrEmpty(user.ProfilePhotoPublicId))
                    {
                        await _cloudinaryService.DeleteFileAsync(user.ProfilePhotoPublicId);
                    }
                    user.ProfileUrl = uploadResult.SecureUrl.ToString();
                    user.ProfilePhotoPublicId = uploadResult.PublicId;
                }
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}