import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { PatientService } from '../../../core/services/patient.service';

@Component({
  selector: 'app-patient-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './patient-profile.html',
  styleUrls: ['./patient-profile.scss']
})
export class PatientProfileComponent implements OnInit {
  profileForm: FormGroup;
  profileData: any = null;
  isEditing = false;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private patientService: PatientService
  ) {
    this.profileForm = this.fb.group({
      userName: [{value: '', disabled: true}, Validators.required],
      email: [{value: '', disabled: true}, [Validators.required, Validators.email]],
      phone: [{value: '', disabled: true}],
      gender: [{value: '', disabled: true}],
      dateOfBirth: [{value: '', disabled: true}],
      bloodGroup: [{value: '', disabled: true}],
      emergencyContact: [{value: '', disabled: true}]
    });
  }

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.patientService.getProfile().subscribe({
      next: (response) => {
        this.profileData = response.data;
        this.profileForm.patchValue(this.profileData);
      },
      error: () => {
        this.toastr.error('Failed to load profile');
      }
    });
  }

  toggleEdit() {
    this.isEditing = !this.isEditing;
    if (this.isEditing) {
      this.profileForm.enable();
    } else {
      this.profileForm.disable();
      this.profileForm.patchValue(this.profileData);
    }
  }

  cancelEdit() {
    this.isEditing = false;
    this.profileForm.disable();
    this.profileForm.patchValue(this.profileData);
  }

  updateProfile() {
    if (this.profileForm.valid) {
      this.isLoading = true;
      this.patientService.updateProfile(this.profileForm.value).subscribe({
        next: () => {
          this.toastr.success('Profile updated successfully');
          this.profileData = { ...this.profileData, ...this.profileForm.value };
          this.isEditing = false;
          this.profileForm.disable();
          this.isLoading = false;
        },
        error: () => {
          this.toastr.error('Failed to update profile');
          this.isLoading = false;
        }
      });
    }
  }

  getAge(): number {
    if (!this.profileData?.dateOfBirth) return 0;
    const today = new Date();
    const birthDate = new Date(this.profileData.dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    return age;
  }
}