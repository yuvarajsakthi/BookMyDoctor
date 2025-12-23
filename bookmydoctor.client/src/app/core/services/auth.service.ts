import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  RegisterRequest,
  LoginRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
  ChangePasswordRequest,
  AuthResponse,
  VerifyOtpRequestDto,
  SendOtpRequestDto
} from '../models/auth.models';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/api/auth`;

  constructor(private http: HttpClient) {}

  register(request: RegisterRequest): Observable<ApiResponse<AuthResponse>> {
    const formData = new FormData();
    Object.keys(request).forEach(key => {
      formData.append(key, (request as any)[key]);
    });
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/register`, formData);
  }

  loginWithEmail(request: LoginRequest): Observable<ApiResponse<AuthResponse>> {
    const formData = new FormData();
    formData.append('email', request.email);
    formData.append('password', request.password);
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/login/email`, formData);
  }

  loginWithOtp(request: VerifyOtpRequestDto): Observable<ApiResponse<AuthResponse>> {
    const formData = new FormData();
    formData.append('email', request.email);
    formData.append('otp', request.otp);
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/login/email/otp`, formData);
  }

  sendOtp(request: SendOtpRequestDto): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('email', request.email);
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/otp/send`, formData);
  }

  verifyOtp(request: VerifyOtpRequestDto): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('email', request.email);
    formData.append('otp', request.otp);
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/otp/verify`, formData);
  }

  logout(): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/logout`, {});
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('email', request.email);
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/password/forgot`, formData);
  }

  resetPassword(request: ResetPasswordRequest): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('email', request.email);
    formData.append('otp', request.otp);
    formData.append('newPassword', request.newPassword);
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/password/reset`, formData);
  }

  changePassword(request: ChangePasswordRequest): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('currentPassword', request.currentPassword);
    formData.append('newPassword', request.newPassword);
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/password/change`, formData);
  }
}