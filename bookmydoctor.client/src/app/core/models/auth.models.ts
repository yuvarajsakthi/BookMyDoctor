export type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T;
};

export type RegisterRequest = {
  userName: string;
  email: string;
  phone?: string;
  password: string;
  userRole: string;
  gender?: string;
};

export type LoginRequest = {
  email: string;
  password: string;
};

export type SendOtpRequestDto = {
  email: string;
  purpose: number;
};

export type VerifyOtpRequestDto = {
  email: string;
  otp: string;
};

export type ForgotPasswordRequest = {
  email: string;
};

export type ResetPasswordRequest = {
  email: string;
  otp: string;
  newPassword: string;
};

export type ChangePasswordRequest = {
  currentPassword: string;
  newPassword: string;
};

export type AuthResponse = {
  token: string;
  user: any;
};