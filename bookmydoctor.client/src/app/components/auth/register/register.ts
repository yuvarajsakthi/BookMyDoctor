import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../core/services/auth.service';
import { RegisterRequest } from '../../../core/models/auth.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink
  ],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent {
  basicInfoForm: FormGroup;
  contactForm: FormGroup;
  passwordForm: FormGroup;
  currentStep = 0;
  hidePassword = true;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private spinner: NgxSpinnerService,
    private toastr: ToastrService
  ) {
    this.basicInfoForm = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.pattern(/^[a-zA-Z0-9._%+-]+@gmail\.com$/)]]
    });

    this.contactForm = this.fb.group({
      phone: ['', [Validators.pattern(/^\d{10}$/)]],
      userRole: ['', [Validators.required]],
      gender: ['']
    });

    this.passwordForm = this.fb.group({
      password: ['', [
        Validators.required, 
        Validators.minLength(8),
        Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/)
      ]]
    });
  }

  getStepDescription(): string {
    switch (this.currentStep) {
      case 0: return 'Enter your basic information';
      case 1: return 'Choose your role and contact details';
      case 2: return 'Create a secure password';
      default: return '';
    }
  }

  nextStep() {
    const currentForm = this.getCurrentForm();
    if (currentForm && currentForm.valid && this.currentStep < 2) {
      this.currentStep++;
    } else if (currentForm) {
      // Mark all fields as touched to show validation errors
      Object.keys(currentForm.controls).forEach(key => {
        currentForm.get(key)?.markAsTouched();
      });
    }
  }

  getCurrentForm(): FormGroup {
    switch (this.currentStep) {
      case 0: return this.basicInfoForm;
      case 1: return this.contactForm;
      case 2: return this.passwordForm;
      default: return this.basicInfoForm;
    }
  }

  previousStep() {
    if (this.currentStep > 0) {
      this.currentStep--;
    }
  }

  onSubmit() {
    // Mark all forms as touched to show validation errors
    this.markAllFormsTouched();
    
    if (this.basicInfoForm.valid && this.contactForm.valid && this.passwordForm.valid) {
      this.isLoading = true;
      this.spinner.show();
      
      const registerData: RegisterRequest = {
        userName: this.basicInfoForm.value.userName,
        email: this.basicInfoForm.value.email,
        password: this.passwordForm.value.password,
        userRole: this.contactForm.value.userRole
      };
      
      // Add optional fields only if they have values
      if (this.contactForm.value.phone) {
        registerData.phone = this.contactForm.value.phone;
      }
      
      if (this.contactForm.value.gender) {
        registerData.gender = this.contactForm.value.gender;
      }
      
      this.authService.register(registerData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.spinner.hide();
          if (response.success) {
            this.toastr.success('Registration successful!');
            this.router.navigate(['/auth/login']);
          } else {
            this.toastr.error(response.message);
          }
        },
        error: (error) => {
          this.isLoading = false;
          this.spinner.hide();
          this.toastr.error('Registration failed. Please try again.');
        }
      });
    }
  }

  markAllFormsTouched() {
    [this.basicInfoForm, this.contactForm, this.passwordForm].forEach(form => {
      Object.keys(form.controls).forEach(key => {
        form.get(key)?.markAsTouched();
      });
    });
  }
}