import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-nearby-clinics',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './nearby-clinics.html',
  styleUrl: './nearby-clinics.scss'
})
export class NearbyClinicsComponent implements OnInit {
  clinics: any[] = [];
  isLoading = false;

  constructor(
    private patientService: PatientService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadClinics();
  }

  loadClinics() {
    this.isLoading = true;
    this.patientService.getNearbyClinics().subscribe({
      next: (response) => {
        this.clinics = response.data || [];
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load clinics');
        this.isLoading = false;
      }
    });
  }
}