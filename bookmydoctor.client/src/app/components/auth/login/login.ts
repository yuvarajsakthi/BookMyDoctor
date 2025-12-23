import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../core/services/auth.service';
import { LoginRequest, SendOtpRequestDto, VerifyOtpRequestDto } from '../../../core/models/auth.models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  loginForm: FormGroup;
  otpForm: FormGroup;
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
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._%+-]+@gmail\.com$/)]],
      password: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/)
      ]]
    });

    this.otpForm = this.fb.group({
      email: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._%+-]+@gmail\.com$/)]],
      otp: ['', [Validators.required, Validators.pattern(/^\d{6}$/)]]
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.spinner.show();
      const loginData: LoginRequest = this.loginForm.value;
      
      this.authService.loginWithEmail(loginData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.spinner.hide();
          if (response.success) {
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('user', JSON.stringify(response.data.user));
            this.toastr.success('Login successful!');
            
            const userRole = response.data.user?.userRole;
            if (userRole === 'Admin') {
              this.router.navigate(['/dashboard/admin']);
            } else if (userRole === 'Doctor') {
              this.router.navigate(['/dashboard/doctor']);
            } else if (userRole === 'Patient') {
              this.router.navigate(['/dashboard/patient']);
            } else {
              this.router.navigate(['/dashboard/patient']); // Default to patient
            }
          } else {
            this.toastr.error(response.message);
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
            localStorage.setItem('token', response.data.token);
            localStorage.setItem('user', JSON.stringify(response.data.user));
            this.toastr.success('Login successful!');
            
            const userRole = response.data.user?.userRole;
            if (userRole === 'Admin') {
              this.router.navigate(['/dashboard/admin']);
            } else if (userRole === 'Doctor') {
              this.router.navigate(['/dashboard/doctor']);
            } else if (userRole === 'Patient') {
              this.router.navigate(['/dashboard/patient']);
            } else {
              this.router.navigate(['/dashboard/patient']);
            }
          } else {
            this.toastr.error(response.message);
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

  switchLoginMode(mode: 'password' | 'otp') {
    this.loginMode = mode;
    this.otpSent = false;
    this.isLoading = false;
  }
}