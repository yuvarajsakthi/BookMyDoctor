import { Component, OnInit } from '@angular/core';

import { ActivatedRoute, Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { DoctorService } from '../../../core/services/doctor.service';
import { DoctorResponseDto } from '../../../core/models/doctor.models';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-doctor-profile',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatProgressSpinnerModule,
    MatDividerModule
],
  templateUrl: './doctor-profile.html',
  styleUrl: './doctor-profile.scss',
})
export class DoctorProfile implements OnInit {
  doctor: DoctorResponseDto | null = null;
  isLoading = true;
  doctorId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private doctorService: DoctorService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.doctorId = Number(this.route.snapshot.paramMap.get('id'));
    if (this.doctorId) {
      this.loadDoctorProfile();
    } else {
      this.toastr.error('Invalid doctor ID');
      this.router.navigate(['/patient/doctor-search']);
    }
  }

  loadDoctorProfile() {
    this.isLoading = true;
    this.doctorService.getDoctorById(this.doctorId).subscribe({
      next: (doctor) => {
        this.doctor = doctor;
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load doctor profile');
        this.isLoading = false;
        this.router.navigate(['/patient/doctor-search']);
      }
    });
  }

  bookAppointment() {
    this.router.navigate(['/patient/book-appointment'], { 
      queryParams: { doctorId: this.doctorId } 
    });
  }

  goBack() {
    this.router.navigate(['/patient/doctor-search']);
  }

  getGenderText(gender?: number): string {
    switch (gender) {
      case 0: return 'Male';
      case 1: return 'Female';
      case 2: return 'Other';
      default: return 'Not specified';
    }
  }

  getAvailabilityText(): string {
    if (this.doctor?.startTime && this.doctor?.endTime) {
      return `${this.doctor.startTime} - ${this.doctor.endTime}`;
    }
    return 'Contact for availability';
  }
}
