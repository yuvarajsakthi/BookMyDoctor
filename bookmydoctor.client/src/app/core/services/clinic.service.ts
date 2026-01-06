import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ClinicCreateDto, ClinicUpdateDto, NearbyClinicDto } from '../models/admin.models';
import { ApiResponse } from '../models/api-response.model';

export interface Clinic {
  clinicId: number;
  clinicName: string;
  address?: string;
  city?: string;
  state?: string;
  country?: string;
  zipCode?: string;
  isActive: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class ClinicService {
  private readonly apiUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient) {}

  getClinics(): Observable<Clinic[]> {
    return this.http.get<ApiResponse<Clinic[]>>(`${this.apiUrl}/clinics`)
      .pipe(map(response => response.data || []));
  }

  getClinic(id: number): Observable<Clinic> {
    return this.http.get<Clinic>(`${this.apiUrl}/clinics/${id}`);
  }

  createClinic(clinic: ClinicCreateDto): Observable<Clinic> {
    return this.http.post<Clinic>(`${this.apiUrl}/clinics`, clinic);
  }

  updateClinic(id: number, clinic: ClinicUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/clinics/${id}`, clinic);
  }

  deleteClinic(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/clinics/${id}`);
  }

  getNearbyClinics(latitude: number, longitude: number, radiusKm: number = 10): Observable<NearbyClinicDto[]> {
    return this.http.get<NearbyClinicDto[]>(`${this.apiUrl}/location/nearby-clinics`, {
      params: { latitude: latitude.toString(), longitude: longitude.toString(), radiusKm: radiusKm.toString() }
    });
  }
}