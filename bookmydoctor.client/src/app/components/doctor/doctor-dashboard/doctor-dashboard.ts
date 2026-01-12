import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AppointmentService } from '../../../core/services/appointment.service';
import { AppointmentResponseDto } from '../../../core/models/admin.models';
import { Observable } from 'rxjs';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-doctor-dashboard',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatProgressSpinnerModule],
  templateUrl: './doctor-dashboard.html',
  styleUrl: './doctor-dashboard.scss'
})
export class DoctorDashboardComponent implements OnInit {
  todayAppointments: AppointmentResponseDto[] = [];
  weeklyAppointments: any[] = [];
  monthlyStats: any = {};
  isLoading = false;
  doctorName = 'Doctor';

  constructor(
    private router: Router,
    private appointmentService: AppointmentService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadTodayAppointments();
    this.loadDoctorName();
    this.loadWeeklyData();
    this.loadMonthlyStats();
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
    this.cdr.detectChanges();
    
    this.appointmentService.getTodayAppointmentsForDoctor()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (appointments) => {
          this.todayAppointments = appointments || [];
        },
        error: (error) => {
          this.todayAppointments = [];
          this.toastr.error('Failed to load appointments');
        }
      });
  }

  viewAppointments() {
    this.router.navigate(['/doctor/appointments']);
  }

  refreshDashboard() {
    this.loadTodayAppointments();
    this.loadWeeklyData();
    this.loadMonthlyStats();
  }

  getPendingCount(): number {
    return this.todayAppointments.filter(a => this.getStatusText(a.status) === 'Pending').length;
  }

  getCompletedCount(): number {
    return this.todayAppointments.filter(a => this.getStatusText(a.status) === 'Completed').length;
  }

  getCancelledCount(): number {
    return this.todayAppointments.filter(a => this.getStatusText(a.status) === 'Cancelled').length;
  }

  getTotalWeeklyAppointments(): number {
    return this.weeklyAppointments.reduce((sum, day) => sum + day.appointments, 0);
  }

  getTotalWeeklyCancellations(): number {
    return this.weeklyAppointments.reduce((sum, day) => sum + day.cancellations, 0);
  }

  loadWeeklyData() {
    // Mock weekly data - replace with actual service call
    this.weeklyAppointments = [
      { day: 'Mon', appointments: 8, cancellations: 1 },
      { day: 'Tue', appointments: 12, cancellations: 2 },
      { day: 'Wed', appointments: 10, cancellations: 0 },
      { day: 'Thu', appointments: 15, cancellations: 3 },
      { day: 'Fri', appointments: 9, cancellations: 1 },
      { day: 'Sat', appointments: 6, cancellations: 0 },
      { day: 'Sun', appointments: 4, cancellations: 1 }
    ];
  }

  loadMonthlyStats() {
    // Mock monthly stats - replace with actual service call
    this.monthlyStats = {
      totalAppointments: 245,
      totalCancellations: 18,
      totalRevenue: 122500,
      averageRating: 4.7,
      patientSatisfaction: 94
    };
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
      case 1: return 'Approved';
      case 2: return 'Rejected';
      case 3: return 'Booked';
      case 4: return 'Completed';
      case 5: return 'Cancelled';
      case 6: return 'PaymentDone';
      default: return 'Unknown';
    }
  }

  logout() {
    sessionStorage.removeItem('token');
    sessionStorage.removeItem('user');
    this.router.navigate(['/auth/login']);
  }
}