export type { ApiResponse } from './api-response.model';

export type PatientCreateDto = {
  userName: string;
  email: string;
  phone?: string;
  password: string;
  gender?: number;
  bloodGroup?: number;
  emergencyContactNumber?: string;
  dateOfBirth?: string;
};

export type DoctorCreateDto = {
  userName: string;
  email: string;
  phone?: string;
  password: string;
  gender?: number;
  specialty: string;
  experienceYears: number;
};

export type LoginRequestDto = {
  email: string;
  password: string;
};

export type LoginOtpRequestDto = {
  email: string;
};

export type SendOtpRequestDto = {
  email: string;
  purpose: number;
};

export type VerifyOtpRequestDto = {
  email: string;
  otp: string;
};

export type ForgotPasswordRequestDto = {
  email: string;
};

export type ResetPasswordWithOtpDto = {
  email: string;
  otp: string;
  newPassword: string;
};

export type ChangePasswordRequestDto = {
  currentPassword: string;
  newPassword: string;
};

export type UserDto = {
  userId: number;
  userName: string;
  email: string;
  phone?: string;
  gender?: number;
  userRole: number;
  isActive: boolean;
};

export type PatientDto = {
  userId: number;
  bloodGroup?: number;
  emergencyContact?: string;
  dateOfBirth?: string;
};

export type DoctorDto = {
  userId: number;
  specialty?: string;
  experienceYears?: number;
  consultationFee?: number;
  isApproved: boolean;
};

export type AuthResponse = {
  token: string;
  user: any;
};

// Legacy types for backward compatibility
export type RegisterRequest = PatientCreateDto;
export type LoginRequest = LoginRequestDto;
export type ForgotPasswordRequest = ForgotPasswordRequestDto;
export type ResetPasswordRequest = ResetPasswordWithOtpDto;
export type ChangePasswordRequest = ChangePasswordRequestDto;