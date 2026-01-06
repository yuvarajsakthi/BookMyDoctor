import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { ToastrService } from 'ngx-toastr';
import { DoctorService } from '../../../core/services/doctor.service';

@Component({
  selector: 'app-doctor-profile',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule
  ],
  template: `
    <div class="profile-container">
      <div class="profile-header">
        <div class="profile-avatar">
          <img [src]="profileData?.profileUrl || 'data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTIwIiBoZWlnaHQ9IjEyMCIgdmlld0JveD0iMCAwIDEyMCAxMjAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSIxMjAiIGhlaWdodD0iMTIwIiBmaWxsPSIjZjBmMGYwIi8+CjxjaXJjbGUgY3g9IjYwIiBjeT0iNDAiIHI9IjE2IiBmaWxsPSIjY2NjIi8+CjxwYXRoIGQ9Ik0zMCA4MGMwLTE2LjU2OSAxMy40MzEtMzAgMzAtMzBzMzAgMTMuNDMxIDMwIDMwdjQwSDMwVjgweiIgZmlsbD0iI2NjYyIvPgo8L3N2Zz4K'" alt="Profile Photo">
          <button class="avatar-edit-btn" title="Change Photo">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
              <path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/>
            </svg>
          </button>
        </div>
        <div class="profile-info">
          <h1>{{ profileData?.userName }}</h1>
          <p class="specialty">{{ profileData?.specialty }}</p>
          <div class="profile-stats">
            <div class="stat">
              <span class="stat-value">{{ profileData?.experienceYears }}</span>
              <span class="stat-label">Years Experience</span>
            </div>
            <div class="stat">
              <span class="stat-value">₹{{ profileData?.consultationFee }}</span>
              <span class="stat-label">Consultation Fee</span>
            </div>
          </div>
        </div>
      </div>

      <div class="profile-content">
        <div class="profile-card">
          <div class="card-header">
            <h2>Personal Information</h2>
            <button class="edit-btn" (click)="toggleEdit()" [class.active]="isEditing">
              <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
                <path d="M3 17.25V21h3.75L17.81 9.94l-3.75-3.75L3 17.25zM20.71 7.04c.39-.39.39-1.02 0-1.41l-2.34-2.34c-.39-.39-1.02-.39-1.41 0l-1.83 1.83 3.75 3.75 1.83-1.83z"/>
              </svg>
              {{ isEditing ? 'Cancel' : 'Edit' }}
            </button>
          </div>

          <form [formGroup]="profileForm" (ngSubmit)="updateProfile()" class="profile-form">
            <div class="form-grid">
              <div class="form-group">
                <label>Full Name</label>
                <input 
                  type="text" 
                  formControlName="userName">
              </div>

              <div class="form-group">
                <label>Email</label>
                <input 
                  type="email" 
                  formControlName="email">
              </div>

              <div class="form-group">
                <label>Phone</label>
                <input 
                  type="tel" 
                  formControlName="phone">
              </div>

              <div class="form-group">
                <label>Gender</label>
                <select formControlName="gender">
                  <option value="Male">Male</option>
                  <option value="Female">Female</option>
                  <option value="Other">Other</option>
                </select>
              </div>

              <div class="form-group">
                <label>Specialty</label>
                <input 
                  type="text" 
                  formControlName="specialty">
              </div>

              <div class="form-group">
                <label>Experience (Years)</label>
                <input 
                  type="number" 
                  formControlName="experienceYears">
              </div>

              <div class="form-group">
                <label>Consultation Fee (₹)</label>
                <input 
                  type="number" 
                  formControlName="consultationFee">
              </div>
            </div>

            <div class="form-group full-width">
              <label>Bio</label>
              <textarea 
                formControlName="bio" 
                rows="4" 
                placeholder="Tell patients about yourself..."></textarea>
            </div>

            <div class="form-actions" *ngIf="isEditing">
              <button type="button" class="btn-secondary" (click)="cancelEdit()">
                Cancel
              </button>
              <button type="submit" class="btn-primary" [disabled]="!profileForm.valid || isLoading">
                <span *ngIf="isLoading" class="spinner"></span>
                {{ isLoading ? 'Saving...' : 'Save Changes' }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .profile-container {
      max-width: 1000px;
      margin: 0 auto;
      padding: 24px;
    }

    .profile-header {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border-radius: 16px;
      padding: 32px;
      color: white;
      display: flex;
      align-items: center;
      gap: 24px;
      margin-bottom: 32px;
    }

    .profile-avatar {
      position: relative;
    }

    .profile-avatar img {
      width: 120px;
      height: 120px;
      border-radius: 50%;
      border: 4px solid rgba(255, 255, 255, 0.2);
      object-fit: cover;
    }

    .avatar-edit-btn {
      position: absolute;
      bottom: 0;
      right: 0;
      background: #007bff;
      border: none;
      border-radius: 50%;
      width: 36px;
      height: 36px;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      color: white;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
    }

    .profile-info h1 {
      margin: 0 0 8px 0;
      font-size: 2.5rem;
      font-weight: 600;
    }

    .specialty {
      font-size: 1.2rem;
      opacity: 0.9;
      margin: 0 0 24px 0;
    }

    .profile-stats {
      display: flex;
      gap: 32px;
    }

    .stat {
      text-align: center;
    }

    .stat-value {
      display: block;
      font-size: 1.8rem;
      font-weight: 600;
    }

    .stat-label {
      font-size: 0.9rem;
      opacity: 0.8;
    }

    .profile-card {
      background: white;
      border-radius: 16px;
      box-shadow: 0 4px 20px rgba(0, 0, 0, 0.1);
      overflow: hidden;
    }

    .card-header {
      padding: 24px 32px;
      border-bottom: 1px solid #f0f0f0;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .card-header h2 {
      margin: 0;
      color: #333;
      font-size: 1.5rem;
    }

    .edit-btn {
      display: flex;
      align-items: center;
      gap: 8px;
      background: #f8f9fa;
      border: 1px solid #dee2e6;
      padding: 8px 16px;
      border-radius: 8px;
      cursor: pointer;
      color: #495057;
      transition: all 0.2s;
    }

    .edit-btn:hover {
      background: #e9ecef;
    }

    .edit-btn.active {
      background: #dc3545;
      color: white;
      border-color: #dc3545;
    }

    .profile-form {
      padding: 32px;
    }

    .form-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
      gap: 24px;
      margin-bottom: 24px;
    }

    .form-group {
      display: flex;
      flex-direction: column;
    }

    .form-group.full-width {
      grid-column: 1 / -1;
    }

    .form-group label {
      font-weight: 500;
      color: #333;
      margin-bottom: 8px;
      font-size: 14px;
    }

    .form-group input,
    .form-group select,
    .form-group textarea {
      padding: 12px 16px;
      border: 2px solid #e9ecef;
      border-radius: 8px;
      font-size: 14px;
      transition: border-color 0.2s;
    }

    .form-group input:focus,
    .form-group select:focus,
    .form-group textarea:focus {
      outline: none;
      border-color: #007bff;
    }

    .form-group input:disabled,
    .form-group select:disabled,
    .form-group textarea:disabled {
      background: #f8f9fa;
      border-color: #e9ecef;
      cursor: default;
    }

    .form-actions {
      display: flex;
      gap: 16px;
      justify-content: flex-end;
      padding-top: 24px;
      border-top: 1px solid #f0f0f0;
    }

    .btn-primary,
    .btn-secondary {
      padding: 12px 24px;
      border-radius: 8px;
      border: none;
      cursor: pointer;
      font-weight: 500;
      display: flex;
      align-items: center;
      gap: 8px;
      transition: all 0.2s;
    }

    .btn-primary {
      background: #007bff;
      color: white;
    }

    .btn-primary:hover:not(:disabled) {
      background: #0056b3;
    }

    .btn-primary:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .btn-secondary {
      background: #6c757d;
      color: white;
    }

    .btn-secondary:hover {
      background: #545b62;
    }

    .spinner {
      width: 16px;
      height: 16px;
      border: 2px solid transparent;
      border-top: 2px solid currentColor;
      border-radius: 50%;
      animation: spin 1s linear infinite;
    }

    @keyframes spin {
      0% { transform: rotate(0deg); }
      100% { transform: rotate(360deg); }
    }

    @media (max-width: 768px) {
      .profile-header {
        flex-direction: column;
        text-align: center;
      }

      .profile-stats {
        justify-content: center;
      }

      .form-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class DoctorProfileComponent implements OnInit {
  profileForm: FormGroup;
  profileData: any = null;
  isEditing = false;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private doctorService: DoctorService
  ) {
    this.profileForm = this.fb.group({
      userName: [{value: '', disabled: true}, Validators.required],
      email: [{value: '', disabled: true}, [Validators.required, Validators.email]],
      phone: [{value: '', disabled: true}],
      gender: [{value: '', disabled: true}],
      specialty: [{value: '', disabled: true}],
      experienceYears: [{value: 0, disabled: true}],
      consultationFee: [{value: 0, disabled: true}],
      bio: [{value: '', disabled: true}]
    });
  }

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.doctorService.getProfile().subscribe({
      next: (profile) => {
        this.profileData = profile;
        this.profileForm.patchValue(profile);
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
      this.doctorService.updateProfile(this.profileForm.value).subscribe({
        next: () => {
          this.toastr.success('Profile updated successfully');
          this.profileData = { ...this.profileData, ...this.profileForm.value };
          this.isEditing = false;
          this.isLoading = false;
        },
        error: () => {
          this.toastr.error('Failed to update profile');
          this.isLoading = false;
        }
      });
    }
  }
}