import { Component, OnInit } from '@angular/core';

import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MaterialModule } from '../../../shared/material.module';
import { AdminService } from '../../../core/services/admin.service';
import { AuthService } from '../../../core/services/auth.service';
import { UserResponseDto } from '../../../core/models/admin.models';
import { BloodGroupUtil } from '../../../core/utils/blood-group.util';

@Component({
  selector: 'app-user-form',
  imports: [
    ReactiveFormsModule,
    MaterialModule
],
  templateUrl: './user-form.html',
  styleUrl: './user-form.scss',
})
export class UserForm implements OnInit {
  userForm!: FormGroup;
  userType: 'doctor' | 'patient' = 'doctor';
  userId?: number;
  isEditMode = false;
  isLoading = false;
  returnUrl = '/admin/doctors';

  genderOptions = [
    { value: 0, label: 'Male' },
    { value: 1, label: 'Female' },
    { value: 2, label: 'Other' }
  ];

  bloodGroupOptions = BloodGroupUtil.getBloodGroupOptions();

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private adminService: AdminService,
    private authService: AuthService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.userType = params['type'] || 'doctor';
      this.userId = params['id'] ? +params['id'] : undefined;
      this.returnUrl = params['returnUrl'] || '/admin/doctors';
      this.isEditMode = !!this.userId;
      
      this.initializeForm();
      
      if (this.isEditMode) {
        this.loadUserData();
      }
    });
  }

  initializeForm() {
    const baseFields = {
      userName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phone: ['', [Validators.pattern(/^[0-9]{10}$/)]],
      gender: [''],
      password: [this.isEditMode ? '' : '', this.isEditMode ? [] : [Validators.required, Validators.minLength(6)]]
    };

    if (this.userType === 'doctor') {
      this.userForm = this.fb.group({
        ...baseFields,
        specialty: ['', Validators.required],
        experienceYears: ['', [Validators.min(0), Validators.max(50)]],
        consultationFee: ['', [Validators.min(0)]],
        bio: ['']
      });
    } else {
      this.userForm = this.fb.group({
        ...baseFields,
        bloodGroup: [''],
        emergencyContactNumber: ['', [Validators.pattern(/^[0-9]{10}$/)]],
        dateOfBirth: ['']
      });
    }
  }

  loadUserData() {
    if (!this.userId) return;
    
    this.isLoading = true;
    this.adminService.getUserDetails(this.userId).subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success && response.data) {
          const user = response.data;
          this.userForm.patchValue({
            userName: user.userName,
            email: user.email,
            phone: user.phone,
            gender: user.gender,
            ...(this.userType === 'doctor' ? {
              specialty: user.specialty,
              experienceYears: user.experienceYears,
              consultationFee: user.consultationFee
            } : {
              bloodGroup: user.bloodGroup,
              emergencyContactNumber: user.emergencyContact,
              dateOfBirth: user.dateOfBirth ? new Date(user.dateOfBirth) : null
            })
          });
        } else {
          this.toastr.error(response.message);
        }
      },
      error: () => {
        this.isLoading = false;
        this.toastr.error('Failed to load user data');
      }
    });
  }

  onSubmit() {
    if (this.userForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isLoading = true;
    const formValue = this.userForm.value;

    if (this.isEditMode) {
      this.updateUser(formValue);
    } else {
      this.createUser(formValue);
    }
  }

  createUser(formValue: any) {
    const registerData = {
      ...formValue,
      userRole: this.userType === 'doctor' ? 1 : 2
    };

    const registerMethod = this.userType === 'doctor' ? 
      this.authService.registerDoctor(registerData) :
      this.authService.registerPatient(registerData);

    registerMethod.subscribe({
      next: (response: any) => {
        this.isLoading = false;
        if (response.success) {
          this.toastr.success(`${this.userType === 'doctor' ? 'Doctor' : 'Patient'} created successfully`);
          this.goBack();
        } else {
          this.toastr.error(response.message);
        }
      },
      error: () => {
        this.isLoading = false;
        this.toastr.error(`Failed to create ${this.userType}`);
      }
    });
  }

  updateUser(formValue: any) {
    if (!this.userId) return;

    this.toastr.success(`${this.userType === 'doctor' ? 'Doctor' : 'Patient'} updated successfully`);
    this.goBack();
  }

  prepareDoctorUpdateData(formValue: any): any {
    return {
      userName: formValue.userName,
      phone: formValue.phone,
      gender: formValue.gender,
      specialty: formValue.specialty,
      experienceYears: formValue.experienceYears,
      consultationFee: formValue.consultationFee,
      bio: formValue.bio
    };
  }

  preparePatientUpdateData(formValue: any): any {
    return {
      userName: formValue.userName,
      phone: formValue.phone,
      gender: formValue.gender,
      bloodGroup: formValue.bloodGroup,
      emergencyContactNumber: formValue.emergencyContactNumber,
      dateOfBirth: formValue.dateOfBirth ? formValue.dateOfBirth.toISOString().split('T')[0] : undefined
    };
  }

  markFormGroupTouched() {
    Object.keys(this.userForm.controls).forEach(key => {
      const control = this.userForm.get(key);
      control?.markAsTouched();
    });
  }

  getFieldError(fieldName: string): string {
    const control = this.userForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) return `${fieldName} is required`;
      if (control.errors['email']) return 'Invalid email format';
      if (control.errors['minlength']) return `${fieldName} is too short`;
      if (control.errors['pattern']) return `Invalid ${fieldName} format`;
      if (control.errors['min']) return `${fieldName} must be positive`;
      if (control.errors['max']) return `${fieldName} is too large`;
    }
    return '';
  }

  goBack() {
    this.router.navigate([this.returnUrl]);
  }
}
