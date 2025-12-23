using AutoMapper;
using BookMyDoctor.Server.DTOs;
using BookMyDoctor.Server.Models;
using BookMyDoctor.Server.Models.Enums;
using BookMyDoctor.Server.Repositories.UnitOfWork;
using BookMyDoctor.Server.Services.Interfaces;

namespace BookMyDoctor.Server.Services.Implementations
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public PatientService(IUnitOfWork unitOfWork, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IEnumerable<DoctorResponseDto>> SearchDoctorsAsync(string? specialty, string? location, DateOnly? date, TimeOnly? availableFrom, TimeOnly? availableTo)
        {
            var doctors = await _unitOfWork.Doctors.FindAsync(d => d.IsApproved);
            
            if (!string.IsNullOrEmpty(specialty))
                doctors = doctors.Where(d => d.Specialty?.Contains(specialty, StringComparison.OrdinalIgnoreCase) == true);

            if (date.HasValue && (availableFrom.HasValue || availableTo.HasValue))
            {
                var appointments = await _unitOfWork.Appointments.FindAsync(a => a.AppointmentDate == date.Value);
                var busyDoctorIds = appointments.Select(a => a.DoctorId).ToHashSet();
                doctors = doctors.Where(d => !busyDoctorIds.Contains(d.DoctorId));
            }

            return doctors.Select(d => _mapper.Map<DoctorResponseDto>(d));
        }

        public async Task<IEnumerable<DoctorResponseDto>> GetDoctorsAsync()
        {
            var doctors = await _unitOfWork.Doctors.FindAsync(d => d.IsApproved);
            return doctors.Select(d => _mapper.Map<DoctorResponseDto>(d));
        }

        public async Task<DoctorResponseDto?> GetDoctorProfileAsync(int doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            return doctor != null ? _mapper.Map<DoctorResponseDto>(doctor) : null;
        }

        public async Task<object> GetDoctorAvailabilityAsync(int doctorId, int? clinicId, DateOnly? date)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            if (doctor == null) throw new ArgumentException("Doctor not found");

            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.DoctorId == doctorId);
            
            if (clinicId.HasValue)
                appointments = appointments.Where(a => a.ClinicId == clinicId.Value);
            if (date.HasValue)
                appointments = appointments.Where(a => a.AppointmentDate == date.Value);

            var availableSlots = new List<string>();
            if (date.HasValue)
            {
                var bookedTimes = appointments.Select(a => a.StartTime).ToHashSet();
                for (var hour = 9; hour < 17; hour++)
                {
                    for (var minute = 0; minute < 60; minute += 30)
                    {
                        var timeSlot = new TimeOnly(hour, minute);
                        if (!bookedTimes.Contains(timeSlot))
                            availableSlots.Add(timeSlot.ToString("HH:mm"));
                    }
                }
            }

            return new { Doctor = _mapper.Map<DoctorResponseDto>(doctor), AvailableSlots = availableSlots };
        }

        public async Task<Appointment> BookAppointmentAsync(int patientId, BookAppointmentDto request)
        {
            var appointment = new Appointment
            {
                PatientId = patientId,
                DoctorId = request.DoctorId,
                ClinicId = request.ClinicId,
                AppointmentDate = request.AppointmentDate,
                StartTime = request.StartTime,
                EndTime = request.StartTime.AddMinutes(30),
                Status = AppointmentStatus.Pending,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Appointments.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> RescheduleAppointmentAsync(int appointmentId, PatientRescheduleDto request)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null) throw new ArgumentException("Appointment not found");

            appointment.AppointmentDate = request.NewDate;
            appointment.StartTime = request.NewStartTime;
            appointment.EndTime = request.NewStartTime.AddMinutes(30);
            appointment.Reason = request.Reason;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
            return appointment;
        }

        public async Task CancelAppointmentAsync(int appointmentId, string? reason)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null) throw new ArgumentException("Appointment not found");

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.Reason = reason;

            await _unitOfWork.Appointments.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync(int patientId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _unitOfWork.Appointments.FindAsync(a => 
                a.PatientId == patientId && a.AppointmentDate >= today && a.Status == AppointmentStatus.Booked);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentHistoryAsync(int patientId)
        {
            return await _unitOfWork.Appointments.FindAsync(a => a.PatientId == patientId);
        }

        public async Task<PatientResponseDto?> GetPatientProfileAsync(int patientId)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
            return patient != null ? _mapper.Map<PatientResponseDto>(patient) : null;
        }

        public async Task UpdatePatientProfileAsync(int patientId, PatientUpdateDto request)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId);
            if (patient == null) throw new ArgumentException("Patient not found");

            // Get the associated user
            var user = await _unitOfWork.Users.GetByIdAsync(patient.UserId);
            if (user == null) throw new ArgumentException("User not found");

            // Handle profile picture upload
            if (request.ProfilePicture != null)
            {
                var uploadResult = await _cloudinaryService.UploadProfilePhotoAsync(
                    request.ProfilePicture, 
                    patient.UserId.ToString()
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
            
            // Update patient fields
            if (request.BloodGroup.HasValue) patient.BloodGroup = request.BloodGroup.Value;
            if (request.EmergencyContactNumber != null) patient.EmergencyContact = request.EmergencyContactNumber;
            if (request.DateOfBirth.HasValue) patient.DateOfBirth = request.DateOfBirth.Value;

            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.Patients.UpdateAsync(patient);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<PatientMedicalHistory>> GetMedicalRecordsAsync(int patientId)
        {
            return await _unitOfWork.PatientMedicalHistories.FindAsync(r => r.PatientId == patientId);
        }

        public async Task<Payment> InitiatePaymentAsync(PaymentCreateDto request)
        {
            var payment = _mapper.Map<Payment>(request);
            payment.PaymentStatus = PaymentStatus.Pending;
            payment.CreatedAt = DateTime.UtcNow;

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();
            return payment;
        }

        public async Task<PaymentStatus> GetPaymentStatusAsync(int paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null) throw new ArgumentException("Payment not found");
            return payment.PaymentStatus;
        }

        public async Task<IEnumerable<Payment>> GetInvoicesAsync(int patientId)
        {
            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.PatientId == patientId);
            var appointmentIds = appointments.Select(a => a.AppointmentId);
            return await _unitOfWork.Payments.FindAsync(p => p.AppointmentId.HasValue && appointmentIds.Contains(p.AppointmentId.Value));
        }

        public async Task<int> GetPatientIdByUserIdAsync(int userId)
        {
            var patient = await _unitOfWork.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            return patient?.PatientId ?? 0;
        }

        
        // Messaging
        public async Task<object> StartConversationAsync(int patientId, StartConversationDto request)
        {
            return new { ConversationId = new Random().Next(1000, 9999), Message = "Conversation started" };
        }

        public async Task<IEnumerable<ConversationDto>> GetConversationsAsync(int patientId)
        {
            return new List<ConversationDto>
            {
                new ConversationDto { ConversationId = 101, Subject = "Consultation", DoctorName = "Dr. Smith", LastMessageTime = DateTime.UtcNow }
            };
        }

        public async Task<IEnumerable<MessageResponseDto>> GetMessagesAsync(int conversationId)
        {
            var messages = await _unitOfWork.Messages.FindAsync(m => m.MessageId > 0); // Mock filter
            return messages.Select(m => _mapper.Map<MessageResponseDto>(m));
        }

        public async Task SendMessageAsync(int patientId, SendMessageDto request)
        {
            var message = new Message
            {
                SenderId = patientId,
                MessageText = request.Content,
                SentAt = DateTime.UtcNow
            };
            await _unitOfWork.Messages.AddAsync(message);
            await _unitOfWork.SaveChangesAsync();
        }

        // AI Assistant
        public async Task<AiResponseDto> ProcessAiQueryAsync(AiQueryDto request)
        {
            // Mock AI response
            return new AiResponseDto
            {
                Response = "Based on your symptoms, I recommend consulting a specialist.",
                Suggestions = new List<string> { "Orthopedics", "In-Person consultation" }
            };
        }

        // Favorites (using Notifications table as mock storage)
        public async Task AddFavoriteDoctorAsync(int patientId, int doctorId)
        {
            var notification = new Notification
            {
                UserId = patientId,
                Message = $"Favorite:Doctor:{doctorId}",
                SentAt = DateTime.UtcNow
            };
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task AddFavoriteClinicAsync(int patientId, int clinicId)
        {
            var notification = new Notification
            {
                UserId = patientId,
                Message = $"Favorite:Clinic:{clinicId}",
                SentAt = DateTime.UtcNow
            };
            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<FavoriteDto>> GetFavoritesAsync(int patientId)
        {
            var favorites = await _unitOfWork.Notifications.FindAsync(n => n.UserId == patientId && n.Message!.StartsWith("Favorite:"));
            return favorites.Select(f => new FavoriteDto
            {
                FavoriteId = f.NotificationId,
                Type = f.Message!.Split(':')[1],
                ItemId = int.Parse(f.Message.Split(':')[2]),
                Name = "Favorite Item"
            });
        }

        public async Task RemoveFavoriteAsync(int favoriteId)
        {
            await _unitOfWork.Notifications.DeleteByIdAsync(favoriteId);
            await _unitOfWork.SaveChangesAsync();
        }

        // Reminders (using PatientMedicalHistory table as mock storage)
        public async Task<ReminderResponseDto> SetReminderAsync(int patientId, ReminderDto request)
        {
            var reminder = new PatientMedicalHistory
            {
                PatientId = patientId,
                Condition = $"Reminder:{request.AppointmentId}",
                Notes = $"{request.ReminderTime}:{request.Message}",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.PatientMedicalHistories.AddAsync(reminder);
            await _unitOfWork.SaveChangesAsync();

            return new ReminderResponseDto
            {
                ReminderId = reminder.HistoryId,
                AppointmentId = request.AppointmentId,
                ReminderTime = request.ReminderTime,
                Message = request.Message,
                IsActive = true
            };
        }

        public async Task<IEnumerable<ReminderResponseDto>> GetRemindersAsync(int patientId)
        {
            var reminders = await _unitOfWork.PatientMedicalHistories.FindAsync(r => r.PatientId == patientId && r.Condition!.StartsWith("Reminder:"));
            return reminders.Select(r => new ReminderResponseDto
            {
                ReminderId = r.HistoryId,
                AppointmentId = int.Parse(r.Condition!.Replace("Reminder:", "")),
                ReminderTime = DateTime.Parse(r.Notes!.Split(':')[0]),
                Message = r.Notes.Split(':')[1],
                IsActive = true
            });
        }

        public async Task DeleteReminderAsync(int reminderId)
        {
            await _unitOfWork.PatientMedicalHistories.DeleteByIdAsync(reminderId);
            await _unitOfWork.SaveChangesAsync();
        }
        
    }
}