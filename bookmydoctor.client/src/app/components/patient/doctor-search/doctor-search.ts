import { Component, OnInit } from '@angular/core';

import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { Router } from '@angular/router';
import { DoctorService } from '../../../core/services/doctor.service';
import { DoctorResponseDto } from '../../../core/models/doctor.models';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-doctor-search',
  standalone: true,
  imports: [
    FormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule
],
  templateUrl: './doctor-search.html',
  styleUrl: './doctor-search.scss'
})
export class DoctorSearchComponent implements OnInit {
  searchFilters = {
    specialty: '',
    location: '',
    date: ''
  };

  doctors: DoctorResponseDto[] = [];
  isLoading = false;
  specialties = [
    'Cardiology', 'Dermatology', 'Neurology', 'Orthopedics', 
    'Pediatrics', 'Psychiatry', 'General Medicine', 'Gynecology'
  ];

  constructor(
    private doctorService: DoctorService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.searchDoctors();
  }

  searchDoctors() {
    this.isLoading = true;
    this.doctorService.searchDoctors(
      this.searchFilters.specialty || undefined,
      this.searchFilters.location || undefined,
      this.searchFilters.date || undefined
    ).subscribe({
      next: (doctors) => {
        this.doctors = doctors;
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to search doctors');
        this.isLoading = false;
      }
    });
  }

  clearFilters() {
    this.searchFilters = { specialty: '', location: '', date: '' };
    this.searchDoctors();
  }

  viewDoctorProfile(doctorId: number) {
    this.router.navigate(['/patient/doctor-profile', doctorId]);
  }

  bookAppointment(doctorId: number) {
    this.router.navigate(['/patient/book-appointment'], { queryParams: { doctorId } });
  }
}