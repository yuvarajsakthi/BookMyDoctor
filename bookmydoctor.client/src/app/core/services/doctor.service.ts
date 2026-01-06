import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

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
}