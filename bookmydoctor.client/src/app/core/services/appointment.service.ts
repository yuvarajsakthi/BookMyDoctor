import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AppointmentResponseDto } from '../models/admin.models';
import { ApiResponse } from '../models/api-response.model';

export interface BookAppointmentDto {
  doctorId: number;
  clinicId: number;
  date: string;
  startTime: string;
  reason?: string;
}

export interface AppointmentStatusUpdateDto {
  status: number;
  reason?: string;
}

export interface AppointmentRescheduleDto {
  newDate: string;
  newStartTime: string;
  reason?: string;
}

export interface AppointmentApprovalDto {
  isApproved: boolean;
  reason?: string;
  blockSlot?: boolean;
}

export interface DoctorRescheduleDto {
  appointmentId: number;
  newDate: string;
  newStartTime: string;
  reason?: string;
}

export interface BlockSlotDto {
  doctorId: number;
  clinicId: number;
  date: string;
  startTime: string;
  endTime: string;
  reason?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AppointmentService {
  private readonly apiUrl = `${environment.apiUrl}/api/appointments`;

  constructor(private http: HttpClient) {}

  getAppointments(): Observable<AppointmentResponseDto[]> {
    return this.http.get<ApiResponse<AppointmentResponseDto[]>>(this.apiUrl)
      .pipe(map(response => response.data || []));
  }

  getDoctorAvailability(): Observable<any[]> {
    return this.http.get<ApiResponse<any[]>>(`${environment.apiUrl}/api/availability/all`)
      .pipe(map(response => {
        return response.data || [];
      }));
  }

  getAllPayments(): Observable<any[]> {
    return this.http.get<ApiResponse<any[]>>(`${environment.apiUrl}/api/payments/all`)
      .pipe(map(response => response.data || []));
  }

  getTodayAppointmentsForDoctor(): Observable<AppointmentResponseDto[]> {
    return this.http.get<ApiResponse<AppointmentResponseDto[]>>(`${this.apiUrl}/today/doctor`)
      .pipe(map(response => response.data || []));
  }

  updateAppointmentStatus(id: number, status: AppointmentStatusUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/status`, status);
  }

  bookAppointment(appointment: BookAppointmentDto): Observable<any> {
    return this.http.post<any>(this.apiUrl, appointment);
  }

  getAppointmentById(id: number): Observable<AppointmentResponseDto> {
    return this.http.get<AppointmentResponseDto>(`${this.apiUrl}/${id}`);
  }

  rescheduleAppointment(id: number, reschedule: AppointmentRescheduleDto): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}/patient-reschedule`, reschedule);
  }

  doctorRescheduleAppointment(id: number, reschedule: DoctorRescheduleDto): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}/doctor-reschedule`, reschedule);
  }

  cancelAppointment(id: number, reason?: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/cancel`, reason);
  }

  getUpcomingAppointments(): Observable<AppointmentResponseDto[]> {
    return this.http.get<ApiResponse<AppointmentResponseDto[]>>(`${this.apiUrl}/patient`)
      .pipe(map(response => response.data || []));
  }

  getAppointmentHistory(): Observable<AppointmentResponseDto[]> {
    return this.http.get<ApiResponse<AppointmentResponseDto[]>>(`${this.apiUrl}/history`)
      .pipe(map(response => response.data || []));
  }

  blockTimeSlot(blockData: BlockSlotDto): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/block-slot`, blockData);
  }

  getDoctorPendingAppointments(): Observable<AppointmentResponseDto[]> {
    return this.http.get<ApiResponse<AppointmentResponseDto[]>>(`${this.apiUrl}/doctor/pending`)
      .pipe(map(response => response.data || []));
  }

  approveOrRejectAppointment(id: number, approval: AppointmentApprovalDto): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}/approve`, approval);
  }

  completeAppointment(id: number): Observable<any> {
    return this.http.put<any>(`${this.apiUrl}/${id}/complete`, {});
  }
}