import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
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
import { AppointmentService, AppointmentStatusUpdateDto, AppointmentApprovalDto, BlockSlotDto, DoctorRescheduleDto } from '../../../core/services/appointment.service';
import { AppointmentResponseDto } from '../../../core/models/admin.models';
import { finalize } from 'rxjs/operators';

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
  pendingAppointments: AppointmentResponseDto[] = [];
  displayedColumns = ['patientName', 'date', 'time', 'status', 'actions'];
  isLoading = false;
  activeTab = 'pending';

  constructor(
    private appointmentService: AppointmentService,
    private toastr: ToastrService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.loadPendingAppointments();
    this.loadTodayAppointments();
  }

  loadPendingAppointments() {
    this.appointmentService.getDoctorPendingAppointments().subscribe({
      next: (appointments) => {
        this.pendingAppointments = appointments || [];
      },
      error: (error) => {
        this.toastr.error('Failed to load pending appointments');
      }
    });
  }

  loadTodayAppointments() {
    this.isLoading = true;
    this.appointmentService.getTodayAppointmentsForDoctor()
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.cdr.detectChanges();
        })
      )
      .subscribe({
        next: (appointments) => {
          this.appointments = appointments?.filter(a => 
            this.getStatusText(a.status) === 'Approved' || this.getStatusText(a.status) === 'Booked'
          ) || [];
        },
        error: (error) => {
          this.appointments = [];
          this.toastr.error('Failed to load appointments');
        }
      });
  }

  updateStatus(appointmentId: number, status: number) {
    const statusUpdate: AppointmentStatusUpdateDto = { status };
    this.appointmentService.updateAppointmentStatus(appointmentId, statusUpdate).subscribe({
      next: () => {
        this.toastr.success('Appointment status updated');
        this.loadPendingAppointments();
        this.loadTodayAppointments();
      },
      error: () => {
        this.toastr.error('Failed to update appointment status');
      }
    });
  }

  approveAppointment(appointmentId: number) {
    const decision: AppointmentApprovalDto = { isApproved: true };
    this.appointmentService.approveOrRejectAppointment(appointmentId, decision).subscribe({
      next: () => {
        this.toastr.success('Appointment approved successfully');
        this.loadPendingAppointments();
        this.loadTodayAppointments();
      },
      error: (error) => {
        this.toastr.error('Failed to approve appointment');
      }
    });
  }

  rejectAppointment(appointmentId: number) {
    const decision: AppointmentApprovalDto = { 
      isApproved: false, 
      reason: 'Doctor unavailable',
      blockSlot: true 
    };
    this.appointmentService.approveOrRejectAppointment(appointmentId, decision).subscribe({
      next: () => {
        this.toastr.success('Appointment rejected and slot blocked');
        this.loadPendingAppointments();
      },
      error: (error) => {
        this.toastr.error('Failed to reject appointment');
      }
    });
  }

  completeAppointment(appointmentId: number) {
    this.appointmentService.completeAppointment(appointmentId).subscribe({
      next: () => {
        this.toastr.success('Appointment completed successfully');
        this.loadTodayAppointments();
      },
      error: (error) => {
        this.toastr.error('Failed to complete appointment');
      }
    });
  }

  rescheduleAppointment(appointment: any) {
    const newDate = prompt('Enter new date (YYYY-MM-DD):');
    const newTime = prompt('Enter new time (HH:MM):');
    const reason = prompt('Enter reason for reschedule:');
    
    if (newDate && newTime) {
      const rescheduleData: DoctorRescheduleDto = {
        appointmentId: appointment.appointmentId,
        newDate: newDate,
        newStartTime: newTime,
        reason: reason || 'Doctor rescheduled'
      };
      
      this.appointmentService.doctorRescheduleAppointment(appointment.appointmentId, rescheduleData).subscribe({
        next: () => {
          this.toastr.success('Appointment rescheduled successfully');
          this.loadPendingAppointments();
          this.loadTodayAppointments();
        },
        error: (error) => {
          this.toastr.error('Failed to reschedule appointment');
        }
      });
    }
  }

  blockSlot(date: string, startTime: string, endTime: string) {
    // Get user data for doctorId and clinicId
    const user = JSON.parse(sessionStorage.getItem('user') || '{}');
    const blockData: BlockSlotDto = {
      doctorId: user.userId || 0,
      clinicId: 1, // Default clinic - should be dynamic
      date: date,
      startTime: startTime,
      endTime: endTime,
      reason: 'Manually blocked by doctor'
    };
    
    this.appointmentService.blockTimeSlot(blockData).subscribe({
      next: () => {
        this.toastr.success('Time slot blocked');
        this.loadTodayAppointments();
      },
      error: () => {
        this.toastr.error('Failed to block time slot');
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
      case 1: return 'Approved';
      case 2: return 'Rejected';
      case 3: return 'Booked';
      case 4: return 'Completed';
      case 5: return 'Cancelled';
      case 6: return 'PaymentDone';
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