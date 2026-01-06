import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-book-appointment',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './book-appointment.html',
  styleUrl: './book-appointment.scss'
})
export class BookAppointmentComponent implements OnInit {
  appointmentForm!: FormGroup;
  clinics: any[] = [];
  doctors: any[] = [];
  isLoading = false;
  isSubmitting = false;

  constructor(
    private fb: FormBuilder,
    private patientService: PatientService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.initForm();
    this.loadData();
  }

  initForm() {
    this.appointmentForm = this.fb.group({
      clinicId: ['', Validators.required],
      doctorId: ['', Validators.required],
      date: ['', Validators.required],
      startTime: ['', Validators.required],
      reason: ['']
    });
  }

  loadData() {
    this.isLoading = true;
    Promise.all([
      this.patientService.getNearbyClinics().toPromise(),
      this.patientService.searchDoctors().toPromise()
    ]).then(([clinicsResponse, doctorsResponse]) => {
      this.clinics = clinicsResponse?.data || [];
      this.doctors = doctorsResponse?.data || [];
      this.isLoading = false;
    }).catch(() => {
      this.toastr.error('Failed to load data');
      this.isLoading = false;
    });
  }

  onSubmit() {
    if (this.appointmentForm.invalid) return;

    this.isSubmitting = true;
    const formValue = this.appointmentForm.value;
    const appointmentData = {
      clinicId: formValue.clinicId,
      doctorId: formValue.doctorId,
      date: formValue.date.toISOString().split('T')[0],
      startTime: formValue.startTime,
      reason: formValue.reason
    };

    this.patientService.bookAppointment(appointmentData).subscribe({
      next: () => {
        this.toastr.success('Appointment booked successfully');
        this.router.navigate(['/patient/appointments']);
        this.isSubmitting = false;
      },
      error: () => {
        this.toastr.error('Failed to book appointment');
        this.isSubmitting = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/patient/appointments']);
  }
}