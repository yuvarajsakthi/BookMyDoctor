import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import {
  ClinicCreateDto,
  ClinicUpdateDto,
  DashboardSummaryDto,
  UserResponseDto,
  AppointmentResponseDto
} from '../models/admin.models';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  private readonly apiUrl = `${environment.apiUrl}/api/admin`;

  constructor(private http: HttpClient) {}

  getAllPatients(): Observable<ApiResponse<UserResponseDto[]>> {
    return this.http.get<ApiResponse<UserResponseDto[]>>(`${environment.apiUrl}/api/users/patients`);
  }

  getAllDoctors(): Observable<ApiResponse<UserResponseDto[]>> {
    return this.http.get<ApiResponse<UserResponseDto[]>>(`${environment.apiUrl}/api/users/doctors`);
  }

  getPendingDoctors(): Observable<ApiResponse<UserResponseDto[]>> {
    return this.http.get<ApiResponse<UserResponseDto[]>>(`${this.apiUrl}/doctors/pending`);
  }

  getUserDetails(userId: number): Observable<ApiResponse<UserResponseDto>> {
    return this.http.get<ApiResponse<UserResponseDto>>(`${environment.apiUrl}/api/users/${userId}`);
  }

  approveRejectDoctor(userId: number, isApproved: boolean): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('isApproved', isApproved.toString());
    return this.http.put<ApiResponse<any>>(`${environment.apiUrl}/api/users/${userId}/approval`, formData);
  }

  updateUserStatus(userId: number, isActive: boolean): Observable<ApiResponse<any>> {
    const formData = new FormData();
    formData.append('isActive', isActive.toString());
    return this.http.put<ApiResponse<any>>(`${environment.apiUrl}/api/users/${userId}`, formData);
  }

  updateUser(userId: number, userData: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${environment.apiUrl}/api/users/${userId}`, userData);
  }

  deleteUser(userId: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${environment.apiUrl}/api/users/${userId}`);
  }

  createClinic(request: ClinicCreateDto): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(`${environment.apiUrl}/api/clinics`, request);
  }

  getAllClinics(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(`${environment.apiUrl}/api/clinics`);
  }

  getClinicDetails(clinicId: number): Observable<ApiResponse<any>> {
    return this.http.get<ApiResponse<any>>(`${environment.apiUrl}/api/clinics/${clinicId}`);
  }

  updateClinic(clinicId: number, request: ClinicUpdateDto): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${environment.apiUrl}/api/clinics/${clinicId}`, request);
  }

  deleteClinic(clinicId: number): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${environment.apiUrl}/api/clinics/${clinicId}`);
  }

  getDashboardSummary(): Observable<ApiResponse<DashboardSummaryDto>> {
    return this.http.get<ApiResponse<DashboardSummaryDto>>(`${environment.apiUrl}/api/users/admin/dashboardsummary`);
  }
}