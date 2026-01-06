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

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<IEnumerable<UserResponseDto>> SearchDoctorsAsync(string? specialty, string? location)
        {
            var doctors = await _unitOfWork.Users.FindAsync(u => u.UserRole == UserRole.Doctor && u.IsApproved);
            
            if (!string.IsNullOrEmpty(specialty))
                doctors = doctors.Where(d => d.Specialty?.Contains(specialty, StringComparison.OrdinalIgnoreCase) == true);

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

        public async Task UpdateUserProfileAsync(int userId, object request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}