export type PatientResponseDto = {
  patientId: number;
  userName: string;
  email: string;
  phone?: string;
  dateOfBirth?: string;
  gender?: string;
  bloodGroup?: string;
  emergencyContact?: string;
};

export type BookAppointmentDto = {
  doctorId: number;
  clinicId: number;
  appointmentDate: string;
  startTime: string;
  reason?: string;
};

export type PatientRescheduleDto = {
  newDate: string;
  newStartTime: string;
  reason?: string;
};

export type PaymentCreateDto = {
  appointmentId: number;
  amount: number;
  paymentMethod: string;
};

export type NearbyClinicDto = {
  clinicId: number;
  name: string;
  address: string;
  distance: number;
  latitude: number;
  longitude: number;
};

export type DirectionsDto = {
  route: string;
  duration: number;
  distance: number;
};

export type StartConversationDto = {
  doctorId: number;
  subject: string;
};

export type ConversationDto = {
  conversationId: number;
  subject: string;
  doctorName: string;
  lastMessageTime: string;
};

export type SendMessageDto = {
  conversationId: number;
  content: string;
};

export type EscalateDto = {
  conversationId: number;
  reason: string;
};

export type AiQueryDto = {
  query: string;
  patientId: number;
};

export type AiResponseDto = {
  response: string;
  suggestions: string[];
};

export type FavoriteDto = {
  favoriteId: number;
  type: string;
  itemId: number;
  name: string;
};

export type RecurringAppointmentDto = {
  doctorId: number;
  clinicId: number;
  appointmentTime: string;
  frequency: string;
  startDate: string;
  endDate?: string;
};

export type RecurringAppointmentResponseDto = {
  recurringId: number;
  doctorName: string;
  clinicName: string;
  appointmentTime: string;
  frequency: string;
  startDate: string;
  endDate?: string;
};

export type ReminderDto = {
  appointmentId: number;
  reminderTime: string;
  message: string;
};

export type ReminderResponseDto = {
  reminderId: number;
  appointmentId: number;
  reminderTime: string;
  message: string;
  isActive: boolean;
};