import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../core/services/auth.service';
import { ForgotPasswordRequest, ResetPasswordRequest, VerifyOtpRequestDto } from '../../../core/models/auth.models';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.scss'
})
export class ForgotPasswordComponent {
  emailForm: FormGroup;
  otpForm: FormGroup;
  resetForm: FormGroup;
  currentStep = 0;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService
  ) {
    this.emailForm = this.fb.group({
      email: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._%+-]+@gmail\.com$/)]]
    });

    this.otpForm = this.fb.group({
      otp: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });

    this.resetForm = this.fb.group({
      newPassword: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/)
      ]]
    });
  }

  getStepDescription(): string {
    switch (this.currentStep) {
      case 0: return 'Enter your email address to receive an OTP';
      case 1: return 'Enter the 6-digit OTP sent to your email';
      case 2: return 'Create a new secure password';
      default: return '';
    }
  }

  sendOtp() {
    if (this.emailForm.valid) {
      this.isLoading = true;
      this.spinner.show();
      const request: ForgotPasswordRequest = this.emailForm.value;
      
      this.authService.forgotPassword(request).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.spinner.hide();
          if (response.success) {
            this.toastr.success('OTP sent to your email!');
            this.currentStep = 1;
          } else {
            this.toastr.error(response.message);
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.spinner.hide();
          this.toastr.error('Failed to send OTP. Please try again.');
        }
      });
    }
  }

  verifyOtp() {
    if (this.otpForm.valid) {
      this.isLoading = true;
      this.spinner.show();
      const request: VerifyOtpRequestDto = {
        email: this.emailForm.value.email,
        otp: this.otpForm.value.otp
      };
      
      this.authService.verifyOtp(request).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.spinner.hide();
          if (response.success) {
            this.toastr.success('OTP verified successfully!');
            this.currentStep = 2;
          } else {
            this.toastr.error('Invalid OTP. Please try again.');
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.spinner.hide();
          this.toastr.error('OTP verification failed. Please try again.');
        }
      });
    }
  }

  resetPassword() {
    if (this.resetForm.valid) {
      this.isLoading = true;
      this.spinner.show();
      const request: ResetPasswordRequest = {
        email: this.emailForm.value.email,
        otp: this.otpForm.value.otp,
        newPassword: this.resetForm.value.newPassword
      };
      
      this.authService.resetPassword(request).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.spinner.hide();
          if (response.success) {
            this.toastr.success('Password reset successfully!');
            this.router.navigate(['/auth/login']);
          } else {
            this.toastr.error(response.message);
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.spinner.hide();
          this.toastr.error('Password reset failed. Please try again.');
        }
      });
    }
  }
}