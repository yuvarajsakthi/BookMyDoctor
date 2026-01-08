import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private apiUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient) {}

  getProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/users/patient/profile`);
  }

  updateProfile(profileData: any): Observable<any> {
    const userId = this.getCurrentUserId();
    return this.http.put(`${this.apiUrl}/users/${userId}`, profileData);
  }

  getNearbyClinics(): Observable<any> {
    return this.http.get(`${this.apiUrl}/clinics`);
  }

  searchDoctors(specialty?: string, location?: string): Observable<any> {
    let params = '';
    if (specialty) params += `specialty=${specialty}&`;
    if (location) params += `location=${location}&`;
    return this.http.get(`${this.apiUrl}/users/doctors/search?${params}`);
  }

  getDoctorsByClinic(clinicId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/doctor/clinic/${clinicId}`);
  }

  getDoctorAvailability(doctorId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/availability/doctor/${doctorId}`);
  }

  getAvailableSlots(doctorId: number, date: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/availability/doctor/${doctorId}/slots?date=${date}`);
  }

  getPatientAppointments(): Observable<any> {
    const token = sessionStorage.getItem('token');
    if (!token) {
      throw new Error('User not authenticated');
    }
    return this.http.get(`${this.apiUrl}/appointments/patient`);
  }

  bookAppointment(appointmentData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/appointments`, appointmentData);
  }

  cancelAppointment(appointmentId: number): Observable<any> {
    return this.http.put(`${this.apiUrl}/appointments/${appointmentId}/cancel`, {});
  }

  rescheduleAppointment(rescheduleData: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/appointments/${rescheduleData.appointmentId}/reschedule`, rescheduleData);
  }

  getAppointmentById(appointmentId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/appointments/${appointmentId}`);
  }

  getPaymentHistory(): Observable<any> {
    return this.http.get(`${this.apiUrl}/payment/history`);
  }

  downloadInvoice(paymentId: number): Observable<Blob> {
    return this.http.get(`${this.apiUrl}/payment/invoice/${paymentId}`, { responseType: 'blob' });
  }

  private getCurrentUserId(): number {
    const user = sessionStorage.getItem('user');
    if (user) {
      const userData = JSON.parse(user);
      return userData.userId || 0;
    }
    return 0;
  }
}