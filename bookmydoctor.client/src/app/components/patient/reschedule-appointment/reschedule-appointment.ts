import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-reschedule-appointment',
  imports: [
    CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule,
    MatSelectModule, MatButtonModule, MatDatepickerModule, MatNativeDateModule,
    MatProgressSpinnerModule, MatIconModule, MatCardModule
  ],
  templateUrl: './reschedule-appointment.html',
  styleUrl: './reschedule-appointment.scss',
})
export class RescheduleAppointment implements OnInit {
  rescheduleForm!: FormGroup;
  appointment: any = null;
  availableTimeSlots: any[] = [];
  isLoading = true;
  isSubmitting = false;
  appointmentId!: number;
  minDate = new Date();

  constructor(
    private fb: FormBuilder,
    private patientService: PatientService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.appointmentId = Number(this.route.snapshot.paramMap.get('id'));
    this.initForm();
    this.loadAppointment();
  }

  initForm() {
    this.rescheduleForm = this.fb.group({
      date: ['', Validators.required],
      startTime: ['', Validators.required]
    });

    this.rescheduleForm.get('date')?.valueChanges.subscribe(() => {
      this.loadAvailableSlots();
    });
  }

  loadAppointment() {
    this.isLoading = true;
    this.cdr.detectChanges();
    
    this.patientService.getAppointmentById(this.appointmentId).pipe(
      finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (response) => {
        this.toastr.success('Appointment details loaded successfully');
        this.appointment = response?.data || response;
      },
      error: (error) => {
        this.toastr.error('Failed to load appointment');
        this.router.navigate(['/patient/appointments']);
      }
    });
  }

  loadAvailableSlots() {
    const selectedDate = this.rescheduleForm.get('date')?.value;
    if (!selectedDate || !this.appointment) return;

    const dateString = selectedDate.toISOString().split('T')[0];
    this.patientService.getAvailableSlots(this.appointment.doctorId, dateString).subscribe({
      next: (response) => {
        this.availableTimeSlots = response?.data || [];
        this.rescheduleForm.patchValue({ startTime: '' });
      },
      error: () => {
        this.toastr.error('Failed to load available slots');
      }
    });
  }

  onSubmit() {
    if (this.rescheduleForm.invalid) return;

    this.isSubmitting = true;
    const formValue = this.rescheduleForm.value;
    const rescheduleData = {
      appointmentId: this.appointmentId,
      newDate: formValue.date.toISOString().split('T')[0],
      newStartTime: formValue.startTime
    };

    this.patientService.rescheduleAppointment(rescheduleData).subscribe({
      next: () => {
        this.toastr.success('Appointment rescheduled successfully!');
        this.router.navigate(['/patient/appointments']);
      },
      error: () => {
        this.toastr.error('Failed to reschedule appointment');
        this.isSubmitting = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/patient/appointments']);
  }

  dateFilter = (date: Date | null): boolean => {
    if (!date) return false;
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return date >= today;
  }
}
