import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { RouterModule } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';
import { PaymentService } from '../../../core/services/payment.service';
import { environment } from '../../../../environments/environment';

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
    private paymentService: PaymentService,
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
    // Navigate to reschedule page with appointment ID
    window.location.href = `/patient/reschedule-appointment/${appointmentId}`;
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'pending': return 'status-pending';
      case 'booked': return 'status-booked';
      case 'completed': return 'status-completed';
      case 'cancelled': return 'status-cancelled';
      case 'paymentdone': return 'status-payment-done';
      default: return 'status-default';
    }
  }

  canRescheduleAppointment(status: string): boolean {
    return status?.toLowerCase() === 'booked' || status?.toLowerCase() === 'pending';
  }

  canCancelAppointment(status: string): boolean {
    return status?.toLowerCase() === 'booked' || status?.toLowerCase() === 'pending';
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
  }

  processPayment() {
    if (!this.selectedAppointment) return;

    // Create payment order
    this.paymentService.completePaymentAfterAppointment(this.selectedAppointment.appointmentId)
      .subscribe({
        next: (response) => {
          if (response.success && response.data) {
            this.openRazorpayCheckout(response.data);
          }
        },
        error: (error) => {
          this.toastr.error('Failed to create payment order');
        }
      });
  }

  private openRazorpayCheckout(paymentData: any) {
    const options = {
      key: environment.razorpayKeyId,
      amount: paymentData.amount * 100, // Amount in paise
      currency: paymentData.currency,
      name: 'BookMyDoctor',
      description: paymentData.description,
      order_id: paymentData.razorpayOrderId,
      prefill: {
        name: 'Patient Name',
        email: 'patient@example.com'
      },
      theme: {
        color: '#3498db'
      }
    };

    this.paymentService.openRazorpay(options)
      .then((response: any) => {
        // Verify payment
        this.verifyPayment(response);
      })
      .catch((error: any) => {
        this.toastr.error('Payment cancelled or failed');
        this.closePaymentModal();
      });
  }

  private verifyPayment(razorpayResponse: any) {
    const verifyRequest = {
      razorpayOrderId: razorpayResponse.razorpay_order_id,
      razorpayPaymentId: razorpayResponse.razorpay_payment_id,
      razorpaySignature: razorpayResponse.razorpay_signature
    };

    this.paymentService.verifyPayment(verifyRequest)
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.selectedAppointment.status = 'PaymentDone';
            this.toastr.success('Payment completed successfully!');
            this.closePaymentModal();
          }
        },
        error: (error) => {
          this.toastr.error('Payment verification failed');
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