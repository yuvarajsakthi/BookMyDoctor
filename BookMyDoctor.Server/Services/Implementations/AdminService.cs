using AutoMapper;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public AdminService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllPatientsAsync()
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.UserRole == UserRole.Patient);
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllDoctorsAsync()
        {
            var users = await _unitOfWork.Users.FindAsync(u => u.UserRole == UserRole.Doctor);
            return _mapper.Map<IEnumerable<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto?> GetUserDetailsAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) return null;

            var userResponse = _mapper.Map<UserResponseDto>(user);
            
            if (user.UserRole == UserRole.Doctor)
            {
                var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (doctor != null)
                {
                    userResponse.Specialty = doctor.Specialty;
                    userResponse.ExperienceYears = doctor.ExperienceYears;
                    userResponse.ConsultationFee = doctor.ConsultationFee;
                    userResponse.IsApproved = doctor.IsApproved;
                }
            }
            else if (user.UserRole == UserRole.Patient)
            {
                var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient != null)
                {
                    userResponse.BloodGroup = patient.BloodGroup;
                    userResponse.EmergencyContact = patient.EmergencyContact;
                    userResponse.DateOfBirth = patient.DateOfBirth;
                }
            }

            return userResponse;
        }

        public async Task<UserResponseDto> UpdatePatientAsync(int userId, PatientUpdateDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || user.UserRole != UserRole.Patient)
                throw new ArgumentException("Patient not found");

            // Handle profile picture upload
            if (request.ProfilePicture != null)
            {
                var uploadResult = await _cloudinaryService.UploadProfilePhotoAsync(
                    request.ProfilePicture, 
                    userId.ToString()
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
            
            await _unitOfWork.Users.UpdateAsync(user);

            var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                if (request.BloodGroup.HasValue) patient.BloodGroup = request.BloodGroup.Value;
                if (request.EmergencyContactNumber != null) patient.EmergencyContact = request.EmergencyContactNumber;
                if (request.DateOfBirth.HasValue) patient.DateOfBirth = request.DateOfBirth.Value;
                
                await _unitOfWork.Patients.UpdateAsync(patient);
            }

            await _unitOfWork.SaveChangesAsync();
            var result = await GetUserDetailsAsync(userId);
            return result ?? throw new InvalidOperationException("Failed to retrieve updated patient details");
        }

        public async Task<UserResponseDto> UpdateDoctorAsync(int userId, DoctorUpdateDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null || user.UserRole != UserRole.Doctor)
                throw new ArgumentException("Doctor not found");

            // Handle profile picture upload
            if (request.ProfilePicture != null)
            {
                var uploadResult = await _cloudinaryService.UploadProfilePhotoAsync(
                    request.ProfilePicture, 
                    userId.ToString()
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
            
            await _unitOfWork.Users.UpdateAsync(user);

            var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor != null)
            {
                if (request.Specialty != null) doctor.Specialty = request.Specialty;
                if (request.ExperienceYears.HasValue) doctor.ExperienceYears = request.ExperienceYears.Value;
                if (request.ConsultationFee.HasValue) doctor.ConsultationFee = request.ConsultationFee.Value;
                if (request.Bio != null) doctor.Bio = request.Bio;
                
                await _unitOfWork.Doctors.UpdateAsync(doctor);
            }

            await _unitOfWork.SaveChangesAsync();
            var result = await GetUserDetailsAsync(userId);
            return result ?? throw new InvalidOperationException("Failed to retrieve updated doctor details");
        }

        public async Task ApproveRejectDoctorAsync(int userId, bool isApproved)
        {
            var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (doctor == null)
                throw new ArgumentException("Doctor not found");

            doctor.IsApproved = isApproved;
            await _unitOfWork.Doctors.UpdateAsync(doctor);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ActivateDeactivateUserAsync(int userId, bool isActive)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            user.IsActive = isActive;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<ClinicResponseDto> CreateClinicAsync(ClinicCreateDto request)
        {
            var clinic = _mapper.Map<Clinic>(request);
            await _unitOfWork.Clinics.AddAsync(clinic);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ClinicResponseDto>(clinic);
        }

        public async Task<IEnumerable<ClinicResponseDto>> GetAllClinicsAsync()
        {
            var clinics = await _unitOfWork.Clinics.GetAllAsync();
            return _mapper.Map<IEnumerable<ClinicResponseDto>>(clinics);
        }

        public async Task<ClinicResponseDto?> GetClinicDetailsAsync(int clinicId)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(clinicId);
            return clinic == null ? null : _mapper.Map<ClinicResponseDto>(clinic);
        }

        public async Task<ClinicResponseDto> UpdateClinicAsync(int clinicId, ClinicUpdateDto request)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(clinicId);
            if (clinic == null)
                throw new ArgumentException("Clinic not found");

            _mapper.Map(request, clinic);
            await _unitOfWork.Clinics.UpdateAsync(clinic);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<ClinicResponseDto>(clinic);
        }

        public async Task DisableClinicAsync(int clinicId)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(clinicId);
            if (clinic == null)
                throw new ArgumentException("Clinic not found");

            clinic.IsActive = false;
            await _unitOfWork.Clinics.UpdateAsync(clinic);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AssignDoctorToClinicAsync(int clinicId, int doctorId)
        {
            var doctorClinic = new DoctorClinic
            {
                ClinicId = clinicId,
                DoctorId = doctorId
            };
            await _unitOfWork.DoctorClinics.AddAsync(doctorClinic);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task RemoveDoctorFromClinicAsync(int clinicId, int doctorId)
        {
            var doctorClinic = await _unitOfWork.DoctorClinics.FirstOrDefaultAsync(dc => dc.ClinicId == clinicId && dc.DoctorId == doctorId);
            if (doctorClinic != null)
            {
                await _unitOfWork.DoctorClinics.DeleteAsync(doctorClinic);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task SetClinicWorkingHoursAsync(int clinicId, ClinicWorkingHoursDto request)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(clinicId);
            if (clinic == null)
                throw new ArgumentException("Clinic not found");

            clinic.OpenTime = request.OpenTime;
            clinic.CloseTime = request.CloseTime;
            clinic.WorkingDays = request.WorkingDays;
            await _unitOfWork.Clinics.UpdateAsync(clinic);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddClinicHolidayAsync(int clinicId, ClinicHolidayDto request)
        {
            var clinic = await _unitOfWork.Clinics.GetByIdAsync(clinicId);
            if (clinic == null)
                throw new ArgumentException("Clinic not found");

            var holidayDates = clinic.HolidayDates ?? "";
            var newHoliday = $"{request.Date:yyyy-MM-dd}:{request.Reason}";
            clinic.HolidayDates = string.IsNullOrEmpty(holidayDates) ? newHoliday : $"{holidayDates};{newHoliday}";
            
            await _unitOfWork.Clinics.UpdateAsync(clinic);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
        {
            var totalPatients = await _unitOfWork.Users.CountAsync(u => u.UserRole == UserRole.Patient);
            var totalDoctors = await _unitOfWork.Users.CountAsync(u => u.UserRole == UserRole.Doctor);
            var totalAppointments = await _unitOfWork.Appointments.CountAsync();
            var totalRevenue = (await _unitOfWork.Payments.FindAsync(p => p.PaymentStatus == PaymentStatus.Paid)).Sum(p => p.Amount);

            return new DashboardSummaryDto
            {
                TotalPatients = totalPatients,
                TotalDoctors = totalDoctors,
                TotalAppointments = totalAppointments,
                TotalRevenue = totalRevenue
            };
        }

        public async Task<IEnumerable<AppointmentResponseDto>> GetAppointmentReportsAsync(DateOnly from, DateOnly to)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.AppointmentDate >= from && a.AppointmentDate <= to);
            return _mapper.Map<IEnumerable<AppointmentResponseDto>>(appointments);
        }

        public async Task<object> GetRevenueReportAsync(int? clinicId)
        {
            var payments = clinicId.HasValue 
                ? await _unitOfWork.Payments.FindAsync(p => p.Appointment!.ClinicId == clinicId && p.PaymentStatus == PaymentStatus.Paid)
                : await _unitOfWork.Payments.FindAsync(p => p.PaymentStatus == PaymentStatus.Paid);
            
            return new { TotalRevenue = payments.Sum(p => p.Amount), PaymentCount = payments.Count() };
        }

        public async Task<byte[]> ExportReportsAsync(string type, string format)
        {
            return format.ToLower() switch
            {
                "pdf" => System.Text.Encoding.UTF8.GetBytes($"PDF Report: {type}"),
                "excel" => System.Text.Encoding.UTF8.GetBytes($"Excel Report: {type}"),
                "csv" => System.Text.Encoding.UTF8.GetBytes($"CSV Report: {type}"),
                _ => throw new ArgumentException("Unsupported format")
            };
        }

        public async Task<InvoiceDto> GenerateInvoiceAsync(InvoiceDto request)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
                throw new ArgumentException("Appointment not found");

            request.Amount = appointment.Doctor.ConsultationFee ?? 0;
            return request;
        }

        public async Task SendInvoiceAsync(int invoiceId)
        {
            await Task.CompletedTask;
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetPaymentsAsync(PaymentStatus? status)
        {
            var payments = status.HasValue 
                ? await _unitOfWork.Payments.FindAsync(p => p.PaymentStatus == status)
                : await _unitOfWork.Payments.GetAllAsync();
            
            return _mapper.Map<IEnumerable<PaymentResponseDto>>(payments);
        }

        public async Task ReconcilePaymentAsync(int paymentId, string? notes)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new ArgumentException("Payment not found");

            payment.PaymentStatus = PaymentStatus.Paid;
            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}