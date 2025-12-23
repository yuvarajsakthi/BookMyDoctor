using AutoMapper;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;

namespace BookMyDoctor.Server.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Mappings
            CreateMap<PatientCreateDto, User>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => Models.Enums.UserRole.Patient));
            
            CreateMap<PatientCreateDto, Patient>()
                .ForMember(dest => dest.EmergencyContact, opt => opt.MapFrom(src => src.EmergencyContactNumber));
            
            CreateMap<DoctorCreateDto, User>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => Models.Enums.UserRole.Doctor));
            
            CreateMap<DoctorCreateDto, Doctor>();
            
            CreateMap<PatientUpdateDto, User>();
            CreateMap<PatientUpdateDto, Patient>()
                .ForMember(dest => dest.EmergencyContact, opt => opt.MapFrom(src => src.EmergencyContactNumber));
            CreateMap<DoctorUpdateDto, User>();
            CreateMap<DoctorUpdateDto, Doctor>();
            
            CreateMap<User, UserResponseDto>();

            // Doctor mappings
            CreateMap<Doctor, DoctorResponseDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.User.Phone))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.User.Gender))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.User.IsActive));
            
            CreateMap<DoctorUpdateDto, Doctor>();
            CreateMap<DoctorUpdateDto, User>();
            
            // Appointment mappings
            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.AppointmentDate))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.User.UserName))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.UserName));
            
            // Blocked appointment mapping
            CreateMap<Appointment, BlockedAppointmentResponseDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.AppointmentDate));
            
            CreateMap<BlockTimeSlotDto, Appointment>()
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.Date))
                .ForMember(dest => dest.PatientId, opt => opt.Ignore());
            
            // Auth DTOs
            CreateMap<UserDto, User>();
            CreateMap<PatientDto, Patient>();
            CreateMap<DoctorDto, Doctor>();
            CreateMap<User, UserDto>();
            CreateMap<PatientCreateDto, UserDto>();
            CreateMap<DoctorCreateDto, UserDto>();
            CreateMap<PatientCreateDto, PatientDto>()
                .ForMember(dest => dest.EmergencyContact, opt => opt.MapFrom(src => src.EmergencyContactNumber));
            CreateMap<DoctorCreateDto, DoctorDto>();
            
            // Clinic Mappings
            CreateMap<ClinicCreateDto, Clinic>();
            CreateMap<ClinicUpdateDto, Clinic>();
            CreateMap<Clinic, ClinicResponseDto>();
            
            // Appointment Mappings
            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.User.UserName))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.User.UserName));
            
            // Payment Mappings
            CreateMap<Payment, PaymentResponseDto>();
        }
    }
}