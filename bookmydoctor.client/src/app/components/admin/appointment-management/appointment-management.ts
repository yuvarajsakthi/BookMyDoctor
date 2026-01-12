import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { AppointmentService } from '../../../core/services/appointment.service';
import { AppointmentResponseDto } from '../../../core/models/admin.models';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-appointment-management',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule
],
  templateUrl: './appointment-management.html',
  styleUrl: './appointment-management.scss'
})
export class AppointmentManagementComponent implements OnInit {
  dataSource = new MatTableDataSource<AppointmentResponseDto>([]);
  availability: any[] = [];
  payments: any[] = [];
  showAvailabilityModal = false;
  showPaymentsModal = false;
  isLoading = false;

  constructor(
    private appointmentService: AppointmentService,
    private router: Router,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading = true;
    this.cdr.detectChanges();
    
    this.appointmentService.getAppointments().pipe(
      finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (appointments) => {
        this.dataSource.data = appointments;
      },
      error: (error) => {
        this.toastr.error('Failed to load appointments');
      }
    });
  }

  loadAvailability() {
    this.appointmentService.getDoctorAvailability().subscribe({
      next: (data) => {
        this.availability = data;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load availability');
      }
    });
  }

  loadPayments() {
    this.appointmentService.getAllPayments().subscribe({
      next: (data) => {
       this.payments = data;
        this.cdr.detectChanges();
      },
      error: (error) => {
        this.toastr.error('Failed to load payments');
      }
    });
  }

  getAppointmentStats() {
    const appointments = this.dataSource.data;
    return {
      total: appointments.length,
      scheduled: appointments.filter(a => this.getStatusText(a.status).toLowerCase() === 'pending').length,
      completed: appointments.filter(a => this.getStatusText(a.status).toLowerCase() === 'completed').length,
      cancelled: appointments.filter(a => this.getStatusText(a.status).toLowerCase() === 'cancelled').length
    };
  }

  getStatusClass(status: string | number): string {
    const statusStr = typeof status === 'string' ? status : this.getStatusText(status);
    switch (statusStr.toLowerCase()) {
      case 'pending': return 'pending';
      case 'booked':
      case 'scheduled': return 'active';
      case 'completed': return 'active';
      case 'cancelled': return 'inactive';
      default: return 'active';
    }
  }

  getStatusText(status: string | number): string {
    if (typeof status === 'string') return status;
    switch (status) {
      case 0: return 'Pending';
      case 1: return 'Booked';
      case 2: return 'Completed';
      case 3: return 'Cancelled';
      default: return 'Unknown';
    }
  }

  formatDate(dateString: string): string {
    if (!dateString) return 'N/A';
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    });
  }

  goBack() {
    this.router.navigate(['/dashboard/admin']);
  }

  getDayName(dayOfWeek: number): string {
    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    return days[dayOfWeek] || 'Unknown';
  }

  getDoctorInitials(name: string): string {
    if (!name) return 'D';
    const names = name.split(' ');
    if (names.length >= 2) {
      return (names[0][0] + names[1][0]).toUpperCase();
    }
    return name[0].toUpperCase();
  }

  getPaymentStatusClass(status: string): string {
    switch (status.toLowerCase()) {
      case 'paid': return 'active';
      case 'pending': return 'pending';
      case 'failed': return 'inactive';
      default: return 'pending';
    }
  }

  openAvailabilityModal() {
    this.showAvailabilityModal = true;
    if (this.availability.length === 0) {
      this.loadAvailability();
    }
  }

  closeAvailabilityModal() {
    this.showAvailabilityModal = false;
  }

  openPaymentsModal() {
    this.showPaymentsModal = true;
    if (this.payments.length === 0) {
      this.loadPayments();
    }
  }

  closePaymentsModal() {
    this.showPaymentsModal = false;
  }
}