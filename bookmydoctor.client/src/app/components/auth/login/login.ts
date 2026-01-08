import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../core/services/auth.service';
import { LoginRequestDto, SendOtpRequestDto, VerifyOtpRequestDto } from '../../../core/models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  loginForm!: FormGroup;
  otpForm!: FormGroup;
  hidePassword = true;
  isLoading = false;
  loginMode: 'password' | 'otp' = 'password';
  otpSent = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService
  ) {
    this.initializeForms();
  }

  private initializeForms() {
    this.loginForm = this.createLoginForm();
    this.otpForm = this.createOtpForm();
  }

  private createLoginForm(): FormGroup {
    return this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/)
      ]]
    });
  }

  private createOtpForm(): FormGroup {
    return this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      otp: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.spinner.show();
      const loginData: LoginRequestDto = this.loginForm.value;
      
      this.authService.loginWithEmail(loginData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.spinner.hide();
          if (response.success) {
            this.handleLoginSuccess(response);
          } else {
            const safeMessage = this.sanitizeMessage(response.message) || 'Login failed';
            this.toastr.error(safeMessage);
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.spinner.hide();
          this.toastr.error('Login failed. Please try again.');
        }
      });
    }
  }

  sendOtp() {
    if (this.otpForm.get('email')?.valid) {
      this.isLoading = true;
      this.spinner.show();
      const otpData: SendOtpRequestDto = {
        email: this.otpForm.value.email,
        purpose: 0
      };
      
      this.authService.sendOtp(otpData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.spinner.hide();
          if (response.success) {
            this.otpSent = true;
            this.toastr.success('OTP sent to your email!');
          } else {
            this.toastr.error(this.sanitizeMessage(response.message) || 'Failed to send OTP');
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

  loginWithOtp() {
    if (this.otpForm.valid) {
      this.isLoading = true;
      this.spinner.show();
      
      const otpLoginData: VerifyOtpRequestDto = this.otpForm.value;
      
      this.authService.loginWithOtp(otpLoginData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.spinner.hide();
          if (response.success) {
            this.handleLoginSuccess(response);
          } else {
            this.toastr.error(this.sanitizeMessage(response.message) || 'OTP login failed');
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.spinner.hide();
          this.toastr.error('OTP login failed. Please try again.');
        }
      });
    }
  }

  private handleLoginSuccess(response: any) {
    if (!response?.data?.token || !response?.data?.user) {
      this.toastr.error('Invalid login response');
      return;
    }
    
    sessionStorage.setItem('token', response.data.token);
    sessionStorage.setItem('user', JSON.stringify(response.data.user));
    this.toastr.success('Login successful!');
    this.navigateBasedOnRole(response.data.user?.userRole);
  }

  private sanitizeMessage(message?: string): string {
    if (!message) return '';
    return message.replace(/<[^>]*>/g, '').substring(0, 200);
  }

  private navigateBasedOnRole(userRole: string | number) {
    if (userRole == null) {
      this.toastr.warning('Invalid user role, redirecting to patient dashboard');
      this.router.navigate(['/dashboard/patient']).catch(err => {
        console.error('Navigation failed:', err);
        this.toastr.error('Navigation failed');
      });
      return;
    }
    
    let route: string;
    
    // Handle both string and number roles
    if (typeof userRole === 'string') {
      switch (userRole.toLowerCase()) {
        case 'admin':
          route = '/dashboard/admin';
          break;
        case 'doctor':
          route = '/dashboard/doctor';
          break;
        case 'patient':
          route = '/dashboard/patient';
          break;
        default:
          this.toastr.warning('Unknown user role, redirecting to patient dashboard');
          route = '/dashboard/patient';
      }
    } else {
      switch (userRole) {
        case 0:
          route = '/dashboard/admin';
          break;
        case 1:
          route = '/dashboard/doctor';
          break;
        case 2:
          route = '/dashboard/patient';
          break;
        default:
          this.toastr.warning('Unknown user role, redirecting to patient dashboard');
          route = '/dashboard/patient';
      }
    }
    
    this.router.navigate([route]).catch(err => {
      console.error('Navigation failed:', err);
      this.toastr.error('Navigation failed');
    });
  }

  switchLoginMode(mode: 'password' | 'otp') {
    if (!this.isLoading) {
      this.loginMode = mode;
      this.otpSent = false;
      this.loginForm.reset();
      this.otpForm.reset();
    }
  }
}