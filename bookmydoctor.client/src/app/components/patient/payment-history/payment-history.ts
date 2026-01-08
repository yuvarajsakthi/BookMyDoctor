import { Component, OnInit } from '@angular/core';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ToastrService } from 'ngx-toastr';
import { PaymentService, PaymentResponseDto } from '../../../core/services/payment.service';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-payment-history',
  imports: [
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule
],
  templateUrl: './payment-history.html',
  styleUrl: './payment-history.scss',
})
export class PaymentHistory implements OnInit {
  payments: PaymentResponseDto[] = [];
  isLoading = true;

  constructor(
    private paymentService: PaymentService,
    private patientService: PatientService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadPaymentHistory();
  }

  loadPaymentHistory() {
    this.patientService.getPaymentHistory().subscribe({
      next: (response) => {
        this.payments = response || [];
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load payment history');
        this.isLoading = false;
      }
    });
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'completed': return 'status-completed';
      case 'pending': return 'status-pending';
      case 'failed': return 'status-failed';
      default: return 'status-default';
    }
  }

  downloadInvoice(paymentId: number) {
    this.patientService.downloadInvoice(paymentId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `invoice-${paymentId}.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: () => {
        this.toastr.error('Failed to download invoice');
      }
    });
  }

  formatDate(dateString: string): string {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }
}
