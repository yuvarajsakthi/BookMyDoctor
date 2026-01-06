import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AppointmentService } from '../../../core/services/appointment.service';
import { AppointmentResponseDto } from '../../../core/models/admin.models';

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './doctor-dashboard.html',
  styleUrl: './doctor-dashboard.scss'
})
export class DoctorDashboardComponent implements OnInit {
  todayAppointments: AppointmentResponseDto[] = [];
  isLoading = false;
  doctorName = 'Doctor';

  constructor(
    private router: Router,
    private appointmentService: AppointmentService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadTodayAppointments();
    this.loadDoctorName();
  }

  loadDoctorName() {
    const user = sessionStorage.getItem('user');
    if (user) {
      const userData = JSON.parse(user);
      this.doctorName = userData.userName || 'Doctor';
    }
  }

  loadTodayAppointments() {
    this.isLoading = true;
    this.appointmentService.getTodayAppointmentsForDoctor().subscribe({
      next: (appointments) => {
        this.todayAppointments = appointments;
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load appointments');
        this.isLoading = false;
      }
    });
  }

  viewAppointments() {
    this.router.navigate(['/doctor/appointments']);
  }

  refreshDashboard() {
    this.loadTodayAppointments();
  }

  getPendingCount(): number {
    return this.todayAppointments.filter(a => this.getStatusText(a.status) === 'Pending').length;
  }

  getCompletedCount(): number {
    return this.todayAppointments.filter(a => this.getStatusText(a.status) === 'Completed').length;
  }

  getTodayRevenue(): number {
    return this.todayAppointments
      .filter(a => this.getStatusText(a.status) === 'Completed')
      .reduce((total, a) => total + 500, 0); // Assuming 500 per consultation
  }

  getStatusClass(status: string | number): string {
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

  logout() {
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('user');
    this.router.navigate(['/auth/login']);
  }
}