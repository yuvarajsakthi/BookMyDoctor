import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterModule, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';
import { AppointmentService, AppointmentRescheduleDto } from '../../../core/services/appointment.service';
import { PaymentService } from '../../../core/services/payment.service';
import { environment } from '../../../../environments/environment';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-patient-appointments',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    RouterModule
  ],
  templateUrl: './patient-appointments.html',
  styleUrl: './patient-appointments.scss'
})
export class PatientAppointmentsComponent implements OnInit {
  appointments: any[] = [];
  isLoading = false;
  showPaymentModal = false;
  selectedAppointment: any = null;

  constructor(
    private patientService: PatientService,
    private appointmentService: AppointmentService,
    private paymentService: PaymentService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadAppointments();
  }

  loadAppointments() {
    this.isLoading = true;
    this.cdr.detectChanges();
    
    this.patientService.getPatientAppointments().pipe(
      finalize(() => {
        this.isLoading = false;
        this.cdr.detectChanges();
      })
    ).subscribe({
      next: (response) => {
        this.toastr.success('Appointments loaded successfully');
        this.appointments = response.data || response || [];
      },
      error: (error) => {
        this.toastr.error('Failed to load appointments');
        this.appointments = [];
      }
    });
  }

  cancelAppointment(appointmentId: number) {
    if (confirm('Are you sure you want to cancel this appointment?')) {
      this.patientService.cancelAppointment(appointmentId).subscribe({
        next: () => {
          this.appointments = this.appointments.filter(a => a.appointmentId !== appointmentId);
          this.toastr.success('Appointment cancelled successfully');
        },
        error: () => {
          this.toastr.error('Failed to cancel appointment');
        }
      });
    }
  }

  rescheduleAppointment(appointmentId: number) {
    const newDate = prompt('Enter new date (YYYY-MM-DD):');
    const newTime = prompt('Enter new time (HH:MM):');
    const reason = prompt('Enter reason for reschedule:');
    
    if (newDate && newTime) {
      const rescheduleData: AppointmentRescheduleDto = {
        newDate: newDate,
        newStartTime: newTime,
        reason: reason || 'Patient requested reschedule'
      };
      
      this.appointmentService.rescheduleAppointment(appointmentId, rescheduleData).subscribe({
        next: () => {
          this.toastr.success('Reschedule request submitted successfully. Waiting for doctor approval.');
          this.loadAppointments();
        },
        error: (error) => {
          this.toastr.error('Failed to submit reschedule request');
        }
      });
    }
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'pending': return 'status-pending';
      case 'approved': return 'status-approved';
      case 'rejected': return 'status-rejected';
      case 'booked': return 'status-booked';
      case 'completed': return 'status-completed';
      case 'cancelled': return 'status-cancelled';
      case 'paymentdone': return 'status-payment-done';
      default: return 'status-default';
    }
  }

  canRescheduleAppointment(status: string): boolean {
    return status?.toLowerCase() === 'booked' || status?.toLowerCase() === 'pending' || status?.toLowerCase() === 'approved';
  }

  canCancelAppointment(status: string): boolean {
    return status?.toLowerCase() === 'booked' || status?.toLowerCase() === 'pending' || status?.toLowerCase() === 'approved';
  }

  canPayAppointment(status: string): boolean {
    return status?.toLowerCase() === 'completed';
  }

  payAppointment(appointmentId: number) {
    this.selectedAppointment = this.appointments.find(a => a.appointmentId === appointmentId);
    this.showPaymentModal = true;
  }

  closePaymentModal() {
    this.showPaymentModal = false;
    this.selectedAppointment = null;
    this.cdr.detectChanges();
  }

  processPayment() {
    if (!this.selectedAppointment) return;

    // Create payment order first
    this.paymentService.createPayment({
      appointmentId: this.selectedAppointment.appointmentId,
      amount: 500
    }).subscribe({
      next: (response) => {
        if (response.success && response.data) {
          this.openRazorpayCheckout(response.data);
        } else {
          this.toastr.error('Failed to create payment order');
        }
      },
      error: (error) => {
        this.toastr.error('Failed to create payment order');
      }
    });
  }

  private openRazorpayCheckout(paymentData: any) {
    const options = {
      key: environment.razorpayKeyId || 'rzp_test_key',
      amount: paymentData.amount * 100, // Amount in paise
      currency: paymentData.currency || 'INR',
      name: 'BookMyDoctor',
      description: paymentData.description || 'Consultation Fee',
      order_id: paymentData.razorpayOrderId,
      prefill: {
        name: 'Patient Name',
        email: 'patient@example.com'
      },
      theme: {
        color: '#3498db'
      },
      handler: (response: any) => {
        this.toastr.info('Processing payment verification...');
        this.verifyPayment(response);
      },
      modal: {
        ondismiss: () => {
          this.toastr.info('Payment cancelled');
          this.closePaymentModal();
        }
      }
    };

    this.paymentService.openRazorpay(options)
      .then((response: any) => {
        this.toastr.info('Payment completed, verifying...');
        this.verifyPayment(response);
      })
      .catch(() => {
        this.toastr.info('Payment cancelled');
        this.router.navigate(['/patient/appointments']);
      });
  }

  private verifyPayment(razorpayResponse: any) {
    // Check if we have all required fields
    if (!razorpayResponse.razorpay_order_id || !razorpayResponse.razorpay_payment_id || !razorpayResponse.razorpay_signature) {
      this.toastr.error('Payment verification failed: Missing required data');
      this.closePaymentModal();
      return;
    }

    const verifyRequest = {
      razorpayOrderId: razorpayResponse.razorpay_order_id,
      razorpayPaymentId: razorpayResponse.razorpay_payment_id,
      razorpaySignature: razorpayResponse.razorpay_signature
    };

    this.toastr.info('Verifying payment...');

    this.paymentService.verifyPayment(verifyRequest)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.selectedAppointment.status = 'PaymentDone';
            this.toastr.success('Payment completed successfully!');
            this.cdr.detectChanges();
            this.closePaymentModal();
          } else {
            this.toastr.error('Payment verification failed');
            this.closePaymentModal();
          }
        },
        error: (error) => {
          this.toastr.error('Payment verification failed: ' + (error?.error?.message || 'Unknown error'));
          this.closePaymentModal();
        }
      });
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    return date.toLocaleDateString('en-US', { 
      weekday: 'long', 
      year: 'numeric', 
      month: 'long', 
      day: 'numeric' 
    });
  }

  formatTime(timeString: string): string {
    if (!timeString) return '';
    const [hours, minutes] = timeString.split(':');
    const hour = parseInt(hours);
    const ampm = hour >= 12 ? 'PM' : 'AM';
    const displayHour = hour % 12 || 12;
    return `${displayHour}:${minutes} ${ampm}`;
  }
}