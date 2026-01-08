import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { NgxSpinnerService } from 'ngx-spinner';
import { ToastrService } from 'ngx-toastr';
import { AuthService } from '../../../core/services/auth.service';
import { PatientCreateDto, DoctorCreateDto } from '../../../core/models/auth.models';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    RouterLink
],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent {
  private static readonly PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]+$/;
  
  basicInfoForm!: FormGroup;
  contactForm!: FormGroup;
  passwordForm!: FormGroup;
  doctorForm!: FormGroup;
  currentStep = 0;
  hidePassword = true;
  isLoading = false;
  selectedRole: 'patient' | 'doctor' = 'patient';

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
    this.basicInfoForm = this.fb.group({
      userName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]]
    });

    this.contactForm = this.fb.group({
      phone: ['', [Validators.pattern(/^\d{10}$/)]],
      userRole: ['patient', [Validators.required]],
      gender: [0]
    });

    this.passwordForm = this.fb.group({
      password: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(RegisterComponent.PASSWORD_PATTERN)
      ]]
    });

    this.doctorForm = this.fb.group({
      specialty: ['', [Validators.required]],
      experienceYears: [0, [Validators.required, Validators.min(0)]]
    });
  }

  getStepDescription(): string {
    switch (this.currentStep) {
      case 0: return 'Enter your basic information';
      case 1: return 'Choose your role and contact details';
      case 2: return this.selectedRole === 'doctor' ? 'Enter your professional details' : 'Create a secure password';
      case 3: return 'Create a secure password';
      default: return '';
    }
  }

  onRoleChange() {
    this.selectedRole = this.contactForm.value.userRole;
  }

  nextStep() {
    const currentForm = this.getCurrentForm();
    if (currentForm && currentForm.valid) {
      const maxSteps = this.selectedRole === 'doctor' ? 3 : 2;
      if (this.currentStep < maxSteps) {
        this.currentStep++;
      }
    } else if (currentForm) {
      currentForm.markAllAsTouched();
    }
  }

  getCurrentForm(): FormGroup {
    switch (this.currentStep) {
      case 0: return this.basicInfoForm;
      case 1: return this.contactForm;
      case 2: return this.selectedRole === 'doctor' ? this.doctorForm : this.passwordForm;
      case 3: return this.passwordForm;
      default: return this.basicInfoForm;
    }
  }

  previousStep() {
    if (this.currentStep > 0) {
      this.currentStep--;
    }
  }

  onSubmit() {
    this.markAllFormsTouched();
    
    if (this.isAllFormsValid()) {
      this.isLoading = true;
      this.spinner.show();
      
      if (this.selectedRole === 'patient') {
        this.registerPatient();
      } else {
        this.registerDoctor();
      }
    }
  }

  private registerPatient() {
    const patientData: PatientCreateDto = {
      userName: this.basicInfoForm.value.userName,
      email: this.basicInfoForm.value.email,
      password: this.passwordForm.value.password,
      phone: this.contactForm.value.phone || undefined,
      gender: this.contactForm.value.gender || undefined
    };
    
    this.authService.registerPatient(patientData).subscribe({
      next: (response) => this.handleRegistrationResponse(response),
      error: (error) => this.handleRegistrationError(error)
    });
  }

  private registerDoctor() {
    const doctorData: DoctorCreateDto = {
      userName: this.basicInfoForm.value.userName,
      email: this.basicInfoForm.value.email,
      password: this.passwordForm.value.password,
      phone: this.contactForm.value.phone || undefined,
      gender: this.contactForm.value.gender || undefined,
      specialty: this.doctorForm.value.specialty,
      experienceYears: this.doctorForm.value.experienceYears
    };
    
    this.authService.registerDoctor(doctorData).subscribe({
      next: (response) => this.handleRegistrationResponse(response),
      error: (error) => this.handleRegistrationError(error)
    });
  }

  private handleRegistrationResponse(response: any) {
    this.isLoading = false;
    this.spinner.hide();
    
    if (!response) {
      this.toastr.error('No response received from server');
      return;
    }
    
    if (response.success) {
      this.toastr.success('Registration successful!');
      this.router.navigate(['/auth/login']);
    } else {
      this.toastr.error(response.message || 'Registration failed');
    }
  }

  private handleRegistrationError(error: any) {
    this.isLoading = false;
    this.spinner.hide();
    const errorMessage = error.error?.message || error.message || 'Registration failed. Please try again.';
    this.toastr.error(errorMessage);
  }

  private isAllFormsValid(): boolean {
    const requiredForms = [this.basicInfoForm, this.contactForm, this.passwordForm];
    if (this.selectedRole === 'doctor') {
      requiredForms.push(this.doctorForm);
    }
    return requiredForms.every(form => form.valid);
  }

  private markAllFormsTouched() {
    const forms = [this.basicInfoForm, this.contactForm, this.passwordForm];
    if (this.selectedRole === 'doctor') {
      forms.push(this.doctorForm);
    }
    forms.forEach(form => form.markAllAsTouched());
  }
}