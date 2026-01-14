import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';

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
    MaterialModule,
    CommonModule
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
  selectedFile: File | null = null;
  selectedFileName = '';
  isDragging = false;

  genderOptions = [
    { value: 'Male', label: 'Male' },
    { value: 'Female', label: 'Female' }
  ];

  bloodGroupOptions = [
    { value: 'APositive', label: 'A+' },
    { value: 'ANegative', label: 'A-' },
    { value: 'BPositive', label: 'B+' },
    { value: 'BNegative', label: 'B-' },
    { value: 'ABPositive', label: 'AB+' },
    { value: 'ABNegative', label: 'AB-' },
    { value: 'OPositive', label: 'O+' },
    { value: 'ONegative', label: 'O-' }
  ];

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private adminService: AdminService,
    private authService: AuthService,
    private toastr: ToastrService,
    private cdr: ChangeDetectorRef
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
      gender: ['Male'],
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
        bloodGroup: ['APositive'],
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
          
          // Set userType based on actual user role - handle both string and number
          this.userType = (user.userRole === 1 || user.userRole as any === 'Doctor' || (user.userRole as any) === 'doctor') ? 'doctor' : 'patient';

          // Reinitialize form with correct type
          this.initializeForm();
          // Trigger change detection
          this.cdr.detectChanges();
          
          // Use setTimeout to ensure form is ready
          setTimeout(() => {
            this.userForm.patchValue({
              userName: user.userName,
              email: user.email,
              phone: user.phone,
              gender: user.gender || 'Male',
              ...(this.userType === 'doctor' ? {
                specialty: user.specialty,
                experienceYears: user.experienceYears,
                consultationFee: user.consultationFee,
                bio: user.bio
              } : {
                bloodGroup: user.bloodGroup,
                emergencyContactNumber: user.emergencyContact,
                dateOfBirth: user.dateOfBirth ? new Date(user.dateOfBirth) : null
              })
            });
          }, 0);
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

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
      this.selectedFileName = file.name;
    }
  }

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
    
    const files = event.dataTransfer?.files;
    if (files && files.length > 0) {
      const file = files[0];
      if (file.type.startsWith('image/')) {
        this.selectedFile = file;
        this.selectedFileName = file.name;
      }
    }
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

    const updateData = this.userType === 'doctor' ? 
      this.prepareDoctorUpdateData(formValue) : 
      this.preparePatientUpdateData(formValue);

    const formData = new FormData();
    Object.keys(updateData).forEach(key => {
      if (updateData[key] !== null && updateData[key] !== undefined) {
        formData.append(key, updateData[key]);
      }
    });
    
    if (this.selectedFile) {
      formData.append('profileImage', this.selectedFile);
    }

    this.adminService.updateUser(this.userId, formData).subscribe({
      next: (response: any) => {
        this.isLoading = false;
        if (response.success) {
          this.toastr.success(`${this.userType === 'doctor' ? 'Doctor' : 'Patient'} updated successfully`);
          this.goBack();
        } else {
          this.toastr.error(response.message);
        }
      },
      error: () => {
        this.isLoading = false;
        this.toastr.error(`Failed to update ${this.userType}`);
      }
    });
  }

  prepareDoctorUpdateData(formValue: any): any {
    return {
      userName: formValue.userName,
      phone: formValue.phone,
      gender: formValue.gender === 'Male' ? 0 : 1,
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
