using AutoMapper;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;
using Kanini.LMP.Application.Services.Implementations;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IOtpService _otpService;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IOtpService otpService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _otpService = otpService;
            _mapper = mapper;
        }

        public async Task<UserResponseDto> RegisterPatientAsync(PatientCreateDto request)
        {
            var userDto = _mapper.Map<UserDto>(request);
            userDto.UserRole = UserRole.Patient;
            userDto.IsActive = true;
            
            var userEntity = _mapper.Map<User>(userDto);
            userEntity.PasswordHash = PasswordService.HashPassword(request.Password);
            
            var createdUser = await _unitOfWork.Users.AddAsync(userEntity);
            await _unitOfWork.SaveChangesAsync();
            
            var patientDto = _mapper.Map<PatientDto>(request);
            patientDto.UserId = createdUser.UserId;
            
            var patientEntity = _mapper.Map<Patient>(patientDto);
            await _unitOfWork.Patients.AddAsync(patientEntity);
            await _unitOfWork.SaveChangesAsync();
            
            return _mapper.Map<UserResponseDto>(createdUser);
        }

        public async Task<UserResponseDto> RegisterDoctorAsync(DoctorCreateDto request)
        {
            var userDto = _mapper.Map<UserDto>(request);
            userDto.UserRole = UserRole.Doctor;
            userDto.IsActive = true;
            
            var userEntity = _mapper.Map<User>(userDto);
            userEntity.PasswordHash = PasswordService.HashPassword(request.Password);
            
            var createdUser = await _unitOfWork.Users.AddAsync(userEntity);
            await _unitOfWork.SaveChangesAsync();
            
            var doctorDto = _mapper.Map<DoctorDto>(request);
            doctorDto.UserId = createdUser.UserId;
            doctorDto.IsApproved = false;
            
            var doctorEntity = _mapper.Map<Doctor>(doctorDto);
            await _unitOfWork.Doctors.AddAsync(doctorEntity);
            await _unitOfWork.SaveChangesAsync();
            
            return _mapper.Map<UserResponseDto>(createdUser);
        }

        public async Task<(string token, UserResponseDto user)> LoginWithEmailAsync(LoginRequestDto request)
        {
            var token = await _tokenService.AuthenticateAsync(request.Email, request.Password);
            if (token == null) throw new UnauthorizedAccessException("Invalid email or password");

            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            
            // Check if user is a doctor and if approved
            if (user!.UserRole == UserRole.Doctor)
            {
                var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == user.UserId);
                if (doctor == null || !doctor.IsApproved)
                    throw new UnauthorizedAccessException("Doctor account is pending approval");
            }
            
            var userDto = _mapper.Map<UserResponseDto>(user);
            return (token, userDto);
        }

        public async Task<(string token, UserResponseDto user)> LoginWithOtpAsync(VerifyOtpRequestDto request)
        {
            if (!_otpService.ValidateOTP(request.Email, request.Otp, true)) // Remove OTP after successful login
                throw new UnauthorizedAccessException("Invalid or expired OTP");

            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) throw new ArgumentException("User not found");

            // Check if user is a doctor and if approved
            if (user.UserRole == UserRole.Doctor)
            {
                var doctor = await _unitOfWork.Doctors.FirstOrDefaultAsync(d => d.UserId == user.UserId);
                if (doctor == null || !doctor.IsApproved)
                    throw new UnauthorizedAccessException("Doctor account is pending approval");
            }

            var userDto = _mapper.Map<UserResponseDto>(user);
            var userDtoForToken = _mapper.Map<UserDto>(user);
            var token = _tokenService.GenerateToken(userDtoForToken);
            return (token, userDto);
        }

        public async Task SendOtpAsync(SendOtpRequestDto request)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) throw new ArgumentException("User not found");

            var otpDto = new OtpEmailDto
            {
                Email = request.Email,
                UserName = user.UserName,
                Purpose = request.Purpose,
                OTP = _otpService.GenerateOTP()
            };

            _otpService.StoreOTP(request.Email, otpDto.OTP);
            var emailSent = await _otpService.SendOTPAsync(otpDto);

            if (!emailSent) throw new InvalidOperationException("Failed to send OTP");
        }

        public async Task<bool> VerifyOtpAsync(VerifyOtpRequestDto request)
        {
            return _otpService.ValidateOTP(request.Email, request.Otp, false); // Don't remove OTP, just verify
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) return; // Don't reveal if email exists

            var otpDto = new OtpEmailDto
            {
                Email = request.Email,
                UserName = user.UserName,
                Purpose = OtpPurpose.FORGET_PASSWORD,
                OTP = _otpService.GenerateOTP()
            };

            _otpService.StoreOTP(request.Email, otpDto.OTP);
            await _otpService.SendOTPAsync(otpDto);
        }

        public async Task ResetPasswordWithOtpAsync(ResetPasswordWithOtpDto request)
        {
            if (!_otpService.ValidateOTP(request.Email, request.Otp, true)) // Remove OTP after successful validation
                throw new ArgumentException("Invalid or expired OTP");

            var user = await _unitOfWork.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null) throw new ArgumentException("User not found");

            user.PasswordHash = PasswordService.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null) throw new ArgumentException("User not found");

            if (!PasswordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                throw new ArgumentException("Current password is incorrect");

            user.PasswordHash = PasswordService.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}