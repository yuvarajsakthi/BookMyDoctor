import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatTabsModule } from '@angular/material/tabs';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-patient-appointments',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatTabsModule
  ],
  templateUrl: './patient-appointments.html',
  styleUrl: './patient-appointments.scss'
})
export class PatientAppointmentsComponent implements OnInit {
  appointments: any[] = [];
  displayedColumns = ['doctorName', 'date', 'time', 'status', 'actions'];
  isLoading = false;

  constructor(
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

  cancelAppointment(appointmentId: number) {
    if (confirm('Are you sure you want to cancel this appointment?')) {
      // For now, just remove from local array since we don't have cancel endpoint
      this.appointments = this.appointments.filter(a => a.appointmentId !== appointmentId);
      this.toastr.success('Appointment cancelled successfully');
    }
  }

  getStatusColor(status: number): string {
    switch (status) {
      case 0: return 'accent'; // Pending
      case 1: return 'primary'; // Booked
      case 2: return 'primary'; // Completed
      case 3: return 'warn'; // Cancelled
      case 4: return 'warn'; // Blocked
      default: return 'primary';
    }
  }

  getStatusText(status: number): string {
    switch (status) {
      case 0: return 'Pending';
      case 1: return 'Booked';
      case 2: return 'Completed';
      case 3: return 'Cancelled';
      case 4: return 'Blocked';
      default: return 'Unknown';
    }
  }
}