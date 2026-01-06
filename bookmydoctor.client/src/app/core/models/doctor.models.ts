export type DoctorResponseDto = {
  doctorId: number;
  userId: number;
  userName: string;
  email: string;
  phone?: string;
  gender?: number;
  specialty?: string;
  experienceYears?: number;
  consultationFee?: number;
  bio?: string;
  isApproved: boolean;
  isActive: boolean;
  startTime?: string;
  endTime?: string;
  dayOfWeek?: number;
};

export type DayOffDto = {
  date: string;
  reason?: string;
  isRecurring?: boolean;
};

export type DoctorDashboardDto = {
  totalAppointments: number;
  todayAppointments: number;
  pendingAppointments: number;
  completedAppointments: number;
  totalEarnings: number;
  averageRating: number;
  totalPatients: number;
};

export type BlockTimeSlotDto = {
  clinicId: number;
  date: string;
  startTime: string;
  endTime: string;
  reason?: string;
};

export type SetAvailabilityDto = {
  clinicId: number;
  dayOfWeek: string;
  startTime: string;
  endTime: string;
  slotDurationMinutes?: number;
};

export type BlockedAppointmentResponseDto = {
  appointmentId: number;
  doctorId: number;
  clinicId: number;
  date: string;
  startTime: string;
  endTime: string;
  reason?: string;
};