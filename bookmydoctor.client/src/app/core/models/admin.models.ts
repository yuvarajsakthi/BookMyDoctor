export type DashboardSummaryDto = {
  totalPatients: number;
  totalDoctors: number;
  totalAppointments: number;
  totalRevenue: number;
  pendingDoctors: UserResponseDto[];
};

export type UserResponseDto = {
  userId: number;
  userName: string;
  email: string;
  phone?: string;
  gender?: number;
  userRole: number;
  isActive: boolean;
  profileUrl?: string;
  specialty?: string;
  experienceYears?: number;
  consultationFee?: number;
  bio?: string;
  isApproved?: boolean;
  bloodGroup?: number;
  emergencyContact?: string;
  dateOfBirth?: string;
};

export type ClinicCreateDto = {
  clinicName: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  zipCode?: string;
};

export type ClinicUpdateDto = {
  clinicName?: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  zipCode?: string;
  isActive?: boolean;
};

export type AppointmentResponseDto = {
  appointmentId: number;
  date: string;
  startTime: string;
  endTime: string;
  patientName: string;
  doctorName: string;
  status: number;
  reason?: string;
  patientId: number;
  doctorId: number;
  clinicId?: number;
};

export type NearbyClinicDto = {
  clinicId: number;
  name: string;
  address: string;
  distance: number;
  latitude: number;
  longitude: number;
};