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

  getPatientAppointments(): Observable<any> {
    return this.http.get(`${this.apiUrl}/appointments/patient`);
  }

  bookAppointment(appointmentData: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/appointments`, appointmentData);
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