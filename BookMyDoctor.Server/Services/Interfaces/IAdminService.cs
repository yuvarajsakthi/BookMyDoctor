using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models.Enums;

namespace BookMyDoctor.Server.Services.Interfaces
{
    public interface IAdminService
    {        
        // Get Users
        Task<IEnumerable<UserResponseDto>> GetAllPatientsAsync();
        Task<IEnumerable<UserResponseDto>> GetAllDoctorsAsync();
        Task<UserResponseDto?> GetUserDetailsAsync(int userId);
        
        // User Management
        Task<UserResponseDto> UpdatePatientAsync(int userId, PatientUpdateDto request);
        Task<UserResponseDto> UpdateDoctorAsync(int userId, DoctorUpdateDto request);
        Task ApproveRejectDoctorAsync(int userId, bool isApproved);
        Task ActivateDeactivateUserAsync(int userId, bool isActive);

        // Clinic Management
        Task<ClinicResponseDto> CreateClinicAsync(ClinicCreateDto request);
        Task<IEnumerable<ClinicResponseDto>> GetAllClinicsAsync();
        Task<ClinicResponseDto?> GetClinicDetailsAsync(int clinicId);
        Task<ClinicResponseDto> UpdateClinicAsync(int clinicId, ClinicUpdateDto request);
        Task DisableClinicAsync(int clinicId);
        Task AssignDoctorToClinicAsync(int clinicId, int doctorId);
        Task RemoveDoctorFromClinicAsync(int clinicId, int doctorId);
        Task SetClinicWorkingHoursAsync(int clinicId, ClinicWorkingHoursDto request);
        Task AddClinicHolidayAsync(int clinicId, ClinicHolidayDto request);

        // Reports
        Task<DashboardSummaryDto> GetDashboardSummaryAsync();
        Task<IEnumerable<AppointmentResponseDto>> GetAppointmentReportsAsync(DateOnly from, DateOnly to);
        Task<object> GetRevenueReportAsync(int? clinicId);
        Task<byte[]> ExportReportsAsync(string type, string format);
        
        // Invoices
        Task<InvoiceDto> GenerateInvoiceAsync(InvoiceDto request);
        Task SendInvoiceAsync(int invoiceId);
        
        // Payments
        Task<IEnumerable<PaymentResponseDto>> GetPaymentsAsync(PaymentStatus? status);
        Task ReconcilePaymentAsync(int paymentId, string? notes);
    }
}