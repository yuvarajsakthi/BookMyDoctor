import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { DoctorResponseDto } from '../models/doctor.models';

@Injectable({
  providedIn: 'root'
})
export class DoctorService {
  private readonly apiUrl = `${environment.apiUrl}/api/doctor`;

  constructor(private http: HttpClient) {}

  getProfile(): Observable<any> {
    return this.http.get<ApiResponse<any>>(`${this.apiUrl}/profile`)
      .pipe(map(response => response.data));
  }

  updateProfile(profile: any): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/profile`, profile);
  }

  searchDoctors(specialty?: string, location?: string, date?: string): Observable<DoctorResponseDto[]> {
    let params = new HttpParams();
    if (specialty) params = params.set('specialty', specialty);
    if (location) params = params.set('location', location);
    if (date) params = params.set('date', date);
    
    return this.http.get<ApiResponse<DoctorResponseDto[]>>(`${this.apiUrl}/search`, { params })
      .pipe(map(response => response.data || []));
  }

  getDoctorById(doctorId: number): Observable<DoctorResponseDto> {
    return this.http.get<ApiResponse<DoctorResponseDto>>(`${this.apiUrl}/${doctorId}`)
      .pipe(map(response => response.data!));
  }
}