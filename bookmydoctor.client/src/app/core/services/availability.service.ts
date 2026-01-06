import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

@Injectable({
  providedIn: 'root'
})
export class AvailabilityService {
  private readonly apiUrl = `${environment.apiUrl}/api/availability`;

  constructor(private http: HttpClient) {}

  getDoctorAvailability(): Observable<any[]> {
    return this.http.get<ApiResponse<any[]>>(this.apiUrl)
      .pipe(map(response => response.data || []));
  }

  addAvailability(availability: any): Observable<void> {
    return this.http.post<void>(this.apiUrl, availability);
  }

  removeAvailability(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}