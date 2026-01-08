import { Component, OnInit } from '@angular/core';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-patient-dashboard',
  standalone: true,
  imports: [MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './patient-dashboard.html',
  styleUrl: './patient-dashboard.scss'
})
export class PatientDashboardComponent implements OnInit {
  appointments: any[] = [];
  isLoading = false;

  constructor(
    private router: Router,
    private patientService: PatientService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading = true;
    this.patientService.getPatientAppointments().subscribe({
      next: (response) => {
        this.appointments = response.data || [];
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load appointments');
        this.isLoading = false;
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