import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AppointmentService, AppointmentStatusUpdateDto } from '../../../core/services/appointment.service';
import { AppointmentResponseDto } from '../../../core/models/admin.models';

@Component({
  selector: 'app-doctor-appointments',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatSelectModule,
    MatFormFieldModule
  ],
  templateUrl: './doctor-appointments.html',
  styleUrl: './doctor-appointments.scss'
})
export class DoctorAppointmentsComponent implements OnInit {
  appointments: AppointmentResponseDto[] = [];
  displayedColumns = ['patientName', 'date', 'time', 'status', 'actions'];
  isLoading = false;

  constructor(
    private appointmentService: AppointmentService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading = true;
    this.appointmentService.getTodayAppointmentsForDoctor().subscribe({
      next: (appointments) => {
        this.appointments = appointments;
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load appointments');
        this.isLoading = false;
      }
    });
  }

  updateStatus(appointmentId: number, status: number) {
    const statusUpdate: AppointmentStatusUpdateDto = { status };
    this.appointmentService.updateAppointmentStatus(appointmentId, statusUpdate).subscribe({
      next: () => {
        this.toastr.success('Appointment status updated');
        this.loadAppointments();
      },
      error: () => {
        this.toastr.error('Failed to update appointment status');
      }
    });
  }

  getStatusColor(status: string | number): string {
    const statusStr = typeof status === 'string' ? status.toLowerCase() : this.getStatusText(status).toLowerCase();
    switch (statusStr) {
      case 'pending': return 'primary';
      case 'booked': case 'scheduled': return 'accent';
      case 'completed': return 'accent';
      case 'cancelled': case 'rejected': return 'warn';
      default: return 'primary';
    }
  }

  getStatusText(status: string | number): string {
    if (typeof status === 'string') return status;
    switch (status) {
      case 0: return 'Pending';
      case 1: return 'Booked';
      case 2: return 'Cancelled';
      case 3: return 'Completed';
      default: return 'Unknown';
    }
  }

  goBack() {
    this.router.navigate(['/dashboard/doctor']);
  }

  getPendingCount(): number {
    return this.appointments.filter(a => this.getStatusText(a.status) === 'Pending').length;
  }

  getCompletedCount(): number {
    return this.appointments.filter(a => this.getStatusText(a.status) === 'Completed').length;
  }

  getPatientInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  getCardClass(status: string | number): string {
    const statusStr = this.getStatusText(status).toLowerCase();
    return statusStr;
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      month: 'short', 
      day: 'numeric',
      year: 'numeric'
    });
  }
}