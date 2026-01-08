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

  getDoctorBreaks(): Observable<any[]> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/breaks`)
      .pipe(map(response => response.data || []));
  }

  getDoctorDaysOff(): Observable<any[]> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/days-off`)
      .pipe(map(response => response.data || []));
  }

  addAvailability(availability: any): Observable<void> {
    return this.http.post<void>(this.apiUrl, availability);
  }

  addBreak(breakData: any): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/breaks`, breakData);
  }

  addDayOff(dayOffData: any): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/days-off`, dayOffData);
  }

  removeAvailability(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  removeBreak(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/breaks/${id}`);
  }

  removeDayOff(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/days-off/${id}`);
  }
}