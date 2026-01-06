import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { ToastrService } from 'ngx-toastr';
import { AvailabilityService } from '../../../core/services/availability.service';

@Component({
  selector: 'app-availability',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    MatTableModule,
    MatIconModule
  ],
  template: `
    <div class="availability-container">
      <div class="page-header">
        <div class="header-left">
          <button class="back-btn" (click)="goBack()">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M20 11H7.83l5.59-5.59L12 4l-8 8 8 8 1.41-1.41L7.83 13H20v-2z"/>
            </svg>
          </button>
          <div class="header-text">
            <h1>Set Availability</h1>
            <p>Manage your working hours and schedule</p>
          </div>
        </div>
      </div>

      <div class="availability-content">
        <div class="add-availability-card">
          <h2>Add New Availability</h2>
          <form [formGroup]="availabilityForm" (ngSubmit)="addAvailability()">
            <div class="form-grid">
              <div class="form-group">
                <label>Day of Week</label>
                <select formControlName="dayOfWeek" class="form-select">
                  <option value="">Select Day</option>
                  <option value="0">Sunday</option>
                  <option value="1">Monday</option>
                  <option value="2">Tuesday</option>
                  <option value="3">Wednesday</option>
                  <option value="4">Thursday</option>
                  <option value="5">Friday</option>
                  <option value="6">Saturday</option>
                </select>
              </div>
              
              <div class="form-group">
                <label>Start Time</label>
                <input type="time" formControlName="startTime" class="form-input">
              </div>
              
              <div class="form-group">
                <label>End Time</label>
                <input type="time" formControlName="endTime" class="form-input">
              </div>
              
              <div class="form-group">
                <button type="submit" class="btn-add" [disabled]="!availabilityForm.valid">
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z"/>
                  </svg>
                  Add Slot
                </button>
              </div>
            </div>
          </form>
        </div>

        <div class="availability-schedule">
          <h2>Current Schedule</h2>
          <div class="schedule-grid">
            <div class="day-column" *ngFor="let day of weekDays; let i = index">
              <div class="day-header">
                <h3>{{ day }}</h3>
                <span class="slot-count">{{ getDaySlots(i).length }} slots</span>
              </div>
              <div class="day-slots">
                <div class="time-slot" *ngFor="let slot of getDaySlots(i)">
                  <div class="slot-time">
                    <span class="start-time">{{ slot.startTime }}</span>
                    <span class="separator">-</span>
                    <span class="end-time">{{ slot.endTime }}</span>
                  </div>
                  <div class="slot-actions">
                    <button class="btn-delete" (click)="removeAvailability(slot.id)" title="Remove">
                      <svg width="14" height="14" viewBox="0 0 24 24" fill="currentColor">
                        <path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/>
                      </svg>
                    </button>
                  </div>
                </div>
                <div class="empty-day" *ngIf="getDaySlots(i).length === 0">
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="currentColor">
                    <path d="M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm-2 15l-5-5 1.41-1.41L10 14.17l7.59-7.59L19 8l-9 9z"/>
                  </svg>
                  <p>No availability</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .availability-container {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
      background: #f8f9fa;
      min-height: 100vh;
    }

    .page-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 32px;
      background: white;
      padding: 24px;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .header-left {
      display: flex;
      align-items: center;
      gap: 16px;
    }

    .back-btn {
      background: #f8f9fa;
      border: 1px solid #dee2e6;
      border-radius: 8px;
      padding: 8px;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s;
    }

    .back-btn:hover {
      background: #e9ecef;
    }

    .header-text h1 {
      margin: 0 0 4px 0;
      color: #333;
      font-size: 1.8rem;
      font-weight: 600;
    }

    .header-text p {
      margin: 0;
      color: #666;
      font-size: 0.9rem;
    }

    .availability-content {
      display: grid;
      gap: 32px;
    }

    .add-availability-card {
      background: white;
      padding: 24px;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .add-availability-card h2 {
      margin: 0 0 24px 0;
      color: #333;
      font-size: 1.3rem;
    }

    .form-grid {
      display: grid;
      grid-template-columns: 2fr 1fr 1fr 1fr;
      gap: 16px;
      align-items: end;
    }

    .form-group {
      display: flex;
      flex-direction: column;
    }

    .form-group label {
      font-weight: 500;
      color: #333;
      margin-bottom: 8px;
      font-size: 14px;
    }

    .form-select,
    .form-input {
      padding: 12px 16px;
      border: 2px solid #e9ecef;
      border-radius: 8px;
      font-size: 14px;
      transition: border-color 0.2s;
    }

    .form-select:focus,
    .form-input:focus {
      outline: none;
      border-color: #007bff;
    }

    .btn-add {
      background: #007bff;
      color: white;
      border: none;
      padding: 12px 20px;
      border-radius: 8px;
      cursor: pointer;
      display: flex;
      align-items: center;
      gap: 8px;
      font-weight: 500;
      transition: background-color 0.2s;
    }

    .btn-add:hover:not(:disabled) {
      background: #0056b3;
    }

    .btn-add:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }

    .availability-schedule {
      background: white;
      padding: 24px;
      border-radius: 12px;
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }

    .availability-schedule h2 {
      margin: 0 0 24px 0;
      color: #333;
      font-size: 1.3rem;
    }

    .schedule-grid {
      display: grid;
      grid-template-columns: repeat(7, 1fr);
      gap: 16px;
    }

    .day-column {
      border: 1px solid #e9ecef;
      border-radius: 8px;
      overflow: hidden;
    }

    .day-header {
      background: #f8f9fa;
      padding: 12px;
      text-align: center;
      border-bottom: 1px solid #e9ecef;
    }

    .day-header h3 {
      margin: 0 0 4px 0;
      font-size: 0.9rem;
      color: #333;
      font-weight: 600;
    }

    .slot-count {
      font-size: 0.75rem;
      color: #666;
    }

    .day-slots {
      padding: 8px;
      min-height: 200px;
    }

    .time-slot {
      background: #e3f2fd;
      border: 1px solid #bbdefb;
      border-radius: 6px;
      padding: 8px;
      margin-bottom: 8px;
      display: flex;
      justify-content: space-between;
      align-items: center;
    }

    .slot-time {
      display: flex;
      align-items: center;
      gap: 4px;
      font-size: 0.8rem;
      color: #1976d2;
      font-weight: 500;
    }

    .separator {
      color: #666;
    }

    .btn-delete {
      background: #ffebee;
      border: 1px solid #ffcdd2;
      border-radius: 4px;
      padding: 4px;
      cursor: pointer;
      color: #d32f2f;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s;
    }

    .btn-delete:hover {
      background: #ffcdd2;
    }

    .empty-day {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      height: 150px;
      color: #999;
      text-align: center;
    }

    .empty-day svg {
      margin-bottom: 8px;
      opacity: 0.5;
    }

    .empty-day p {
      margin: 0;
      font-size: 0.8rem;
    }

    @media (max-width: 1024px) {
      .schedule-grid {
        grid-template-columns: repeat(4, 1fr);
      }
    }

    @media (max-width: 768px) {
      .form-grid {
        grid-template-columns: 1fr;
      }

      .schedule-grid {
        grid-template-columns: repeat(2, 1fr);
      }
    }

    @media (max-width: 480px) {
      .schedule-grid {
        grid-template-columns: 1fr;
      }
    }
  `]
})
export class AvailabilityComponent implements OnInit {
  availabilityForm: FormGroup;
  availability: any[] = [];
  weekDays = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private availabilityService: AvailabilityService
  ) {
    this.availabilityForm = this.fb.group({
      dayOfWeek: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });
  }

  ngOnInit() {
    this.loadAvailability();
  }

  loadAvailability() {
    this.availabilityService.getDoctorAvailability().subscribe({
      next: (data) => {
        this.availability = data;
      },
      error: () => {
        this.toastr.error('Failed to load availability');
      }
    });
  }

  addAvailability() {
    if (this.availabilityForm.valid) {
      this.availabilityService.addAvailability(this.availabilityForm.value).subscribe({
        next: () => {
          this.toastr.success('Availability slot added');
          this.availabilityForm.reset();
          this.loadAvailability();
        },
        error: () => {
          this.toastr.error('Failed to add availability');
        }
      });
    }
  }

  removeAvailability(id: number) {
    this.availabilityService.removeAvailability(id).subscribe({
      next: () => {
        this.toastr.success('Availability slot removed');
        this.loadAvailability();
      },
      error: () => {
        this.toastr.error('Failed to remove availability');
      }
    });
  }

  getDaySlots(dayIndex: number): any[] {
    return this.availability.filter(slot => slot.dayOfWeek == dayIndex);
  }

  goBack() {
    // Navigation logic
  }
}