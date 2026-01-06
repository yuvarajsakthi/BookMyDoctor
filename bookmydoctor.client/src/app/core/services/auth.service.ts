import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  PatientCreateDto,
  DoctorCreateDto,
  LoginRequestDto,
  ForgotPasswordRequestDto,
  ResetPasswordWithOtpDto,
  ChangePasswordRequestDto,
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

  registerPatient(request: PatientCreateDto): Observable<ApiResponse<any>> {
    const formData = new FormData();
    Object.keys(request).forEach(key => {
      const value = (request as any)[key];
      if (value !== undefined && value !== null) {
        formData.append(key, value);
      }
    });
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/register/patient`, formData);
  }

  registerDoctor(request: DoctorCreateDto): Observable<ApiResponse<any>> {
    const formData = new FormData();
    Object.keys(request).forEach(key => {
      const value = (request as any)[key];
      if (value !== undefined && value !== null) {
        formData.append(key, value);
      }
    });
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/register/doctor`, formData);
  }

  loginWithEmail(request: LoginRequestDto): Observable<ApiResponse<AuthResponse>> {
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
    formData.append('purpose', request.purpose.toString());
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

  forgotPassword(request: ForgotPasswordRequestDto): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('email', request.email);
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/password/forgot`, formData);
  }

  resetPassword(request: ResetPasswordWithOtpDto): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('email', request.email);
    formData.append('otp', request.otp);
    formData.append('newPassword', request.newPassword);
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/password/reset`, formData);
  }

  changePassword(request: ChangePasswordRequestDto): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('currentPassword', request.currentPassword);
    formData.append('newPassword', request.newPassword);
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/password/change`, formData);
  }
}