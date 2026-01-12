import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './patient-dashboard.html',
  styleUrl: './patient-dashboard.scss'
})
export class PatientDashboardComponent implements OnInit {
  appointments: any[] = [];
  isLoading = false;

  constructor(
    private router: Router,
    private patientService: PatientService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading = true;
    this.cdr.detectChanges();
    
    this.patientService.getPatientAppointments().pipe(
      finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (response) => {
        this.toastr.success('Appointments loaded successfully');
        this.appointments = response?.data || response || [];
      },
      error: (error) => {
        this.toastr.error('Failed to load appointments');
        this.appointments = [];
      }
    });
  }

  searchDoctors() {
    this.router.navigate(['/patient/doctor-search']);
  }

  findClinics() {
    this.router.navigate(['/patient/nearby-clinics']);
  }

  bookAppointment() {
    this.router.navigate(['/patient/book-appointment']);
  }

  viewAppointments() {
    this.router.navigate(['/patient/appointments']);
  }

  viewProfile() {
    this.router.navigate(['/patient/profile']);
  }

  getUserName(): string {
    const user = sessionStorage.getItem('user');
    if (user) {
      const userData = JSON.parse(user);
      return userData.userName || 'Patient';
    }
    return 'Patient';
  }
}