using AutoMapper;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
using PaymentModel = BookMyDoctor.Server.Models.Payment;

namespace BookMyDoctor.Server.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User Creation Mappings
            CreateMap<PatientCreateDto, User>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => UserRole.Patient))
                .ForMember(dest => dest.EmergencyContact, opt => opt.MapFrom(src => src.EmergencyContactNumber));
            
            CreateMap<DoctorCreateDto, User>()
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => UserRole.Doctor));
            
            // User Response Mappings
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.HasValue ? src.DateOfBirth.Value.ToString("yyyy-MM-dd") : null));
            
            CreateMap<User, UserDto>();
            
            // Appointment mappings
            CreateMap<Appointment, AppointmentResponseDto>()
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.AppointmentDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm")))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.UserName))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.UserName));
            
            // Availability mappings
            CreateMap<Availability, AvailabilityResponseDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.AvailabilityId))
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => (int)src.DayOfWeek))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH:mm")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH:mm")))
                .ForMember(dest => dest.DoctorName, opt => opt.MapFrom(src => src.Doctor.UserName));
            
            // Notification Mappings
            CreateMap<Notification, NotificationResponseDto>();
            
            // Payment Mappings
            CreateMap<PaymentModel, PaymentResponseDto>()
                .ForMember(dest => dest.AppointmentDetails, opt => opt.MapFrom(src => 
                    src.Appointment != null ? 
                    $"{src.Appointment.Patient.UserName} - Dr. {src.Appointment.Doctor.UserName} ({src.Appointment.AppointmentDate:MMM dd, yyyy})" : 
                    "N/A"));
        }
    }
}