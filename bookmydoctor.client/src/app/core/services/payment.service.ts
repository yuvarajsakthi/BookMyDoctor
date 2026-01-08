import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';

export interface CreatePaymentDto {
  appointmentId: number;
  amount: number;
}

export interface VerifyPaymentDto {
  razorpayOrderId: string;
  razorpayPaymentId: string;
  razorpaySignature: string;
}

export interface PaymentResponseDto {
  paymentId: number;
  appointmentId?: number;
  razorpayOrderId?: string;
  razorpayPaymentId?: string;
  amount: number;
  currency: string;
  paymentStatus: string;
  description?: string;
  createdAt: string;
  paidAt?: string;
}

declare var Razorpay: any;

@Injectable({
  providedIn: 'root'
})
export class PaymentService {
  private readonly apiUrl = `${environment.apiUrl}/api/payment`;

  constructor(private http: HttpClient) {}

  createPayment(request: CreatePaymentDto): Observable<ApiResponse<PaymentResponseDto>> {
    return this.http.post<ApiResponse<PaymentResponseDto>>(`${this.apiUrl}/create`, request);
  }

  verifyPayment(request: VerifyPaymentDto): Observable<ApiResponse<PaymentResponseDto>> {
    return this.http.post<ApiResponse<PaymentResponseDto>>(`${this.apiUrl}/verify`, request);
  }

  completePaymentAfterAppointment(appointmentId: number): Observable<ApiResponse<PaymentResponseDto>> {
    return this.http.post<ApiResponse<PaymentResponseDto>>(`${this.apiUrl}/complete/${appointmentId}`, {});
  }

  openRazorpay(options: any): Promise<any> {
    return new Promise((resolve, reject) => {
      // Load Razorpay script dynamically
      if (!(window as any).Razorpay) {
        const script = document.createElement('script');
        script.src = 'https://checkout.razorpay.com/v1/checkout.js';
        script.onload = () => {
          this.initializeRazorpay(options, resolve, reject);
        };
        document.head.appendChild(script);
      } else {
        this.initializeRazorpay(options, resolve, reject);
      }
    });
  }

  private initializeRazorpay(options: any, resolve: any, reject: any) {
    const rzp = new (window as any).Razorpay({
      ...options,
      handler: (response: any) => resolve(response),
      modal: {
        ondismiss: () => reject('Payment cancelled')
      }
    });
    rzp.open();
  }
}