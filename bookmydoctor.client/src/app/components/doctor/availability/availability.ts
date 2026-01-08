import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { ToastrService } from 'ngx-toastr';
import { AvailabilityService } from '../../../core/services/availability.service';
import { ClinicService } from '../../../core/services/clinic.service';

@Component({
  selector: 'app-availability',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    CommonModule, ReactiveFormsModule, MatFormFieldModule, MatSelectModule,
    MatButtonModule, MatTableModule, MatIconModule, MatInputModule,
    MatDatepickerModule, MatNativeDateModule, MatTabsModule, MatCardModule
  ],
  templateUrl: './availability.html',
  styleUrl: './availability.scss'
})
export class AvailabilityComponent implements OnInit {
  availabilityForm: FormGroup;
  breakForm: FormGroup;
  dayOffForm: FormGroup;
  availability: any[] = [];
  breaks: any[] = [];
  daysOff: any[] = [];
  clinics: any[] = [];
  weekDays = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

  constructor(
    private fb: FormBuilder,
    private toastr: ToastrService,
    private availabilityService: AvailabilityService,
    private clinicService: ClinicService,
    private cdr: ChangeDetectorRef
  ) {
    this.availabilityForm = this.fb.group({
      clinicId: ['', Validators.required],
      dayOfWeek: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });

    this.breakForm = this.fb.group({
      clinicId: ['', Validators.required],
      dayOfWeek: ['', Validators.required],
      startTime: ['', Validators.required],
      endTime: ['', Validators.required]
    });

    this.dayOffForm = this.fb.group({
      date: ['', Validators.required],
      reason: ['']
    });
  }

  ngOnInit() {
    this.loadClinics();
    this.loadAvailability();
    this.loadBreaks();
    this.loadDaysOff();
  }

  loadClinics() {
    this.clinicService.getClinics().subscribe({
      next: (response) => {
        this.clinics = response || [];
        this.cdr.markForCheck();
      },
      error: () => {
        this.toastr.error('Failed to load clinics');
      }
    });
  }

  loadAvailability() {
    this.availabilityService.getDoctorAvailability().subscribe({
      next: (data) => {
        this.availability = data;
        this.cdr.markForCheck();
      },
      error: () => {
        this.toastr.error('Failed to load availability');
      }
    });
  }

  loadBreaks() {
    this.availabilityService.getDoctorBreaks().subscribe({
      next: (data) => {
        this.breaks = data;
        this.cdr.markForCheck();
      },
      error: () => {
        this.toastr.error('Failed to load breaks');
      }
    });
  }

  loadDaysOff() {
    this.availabilityService.getDoctorDaysOff().subscribe({
      next: (data) => {
        this.daysOff = data;
        this.cdr.markForCheck();
      },
      error: () => {
        this.toastr.error('Failed to load days off');
      }
    });
  }

  addAvailability() {
    if (this.availabilityForm.valid) {
      this.availabilityService.addAvailability(this.availabilityForm.value).subscribe({
        next: () => {
          this.toastr.success('Working hours added');
          this.availabilityForm.reset();
          this.loadAvailability();
        },
        error: () => {
          this.toastr.error('Failed to add working hours');
        }
      });
    }
  }

  addBreak() {
    if (this.breakForm.valid) {
      this.availabilityService.addBreak(this.breakForm.value).subscribe({
        next: () => {
          this.toastr.success('Break time added');
          this.breakForm.reset();
          this.loadBreaks();
        },
        error: () => {
          this.toastr.error('Failed to add break');
        }
      });
    }
  }

  addDayOff() {
    if (this.dayOffForm.valid) {
      const formValue = this.dayOffForm.value;
      const dayOffData = {
        date: formValue.date.toISOString().split('T')[0],
        reason: formValue.reason
      };
      
      this.availabilityService.addDayOff(dayOffData).subscribe({
        next: () => {
          this.toastr.success('Day off scheduled');
          this.dayOffForm.reset();
          this.loadDaysOff();
        },
        error: () => {
          this.toastr.error('Failed to schedule day off');
        }
      });
    }
  }

  removeAvailability(id: number) {
    this.availabilityService.removeAvailability(id).subscribe({
      next: () => {
        this.toastr.success('Working hours removed');
        this.loadAvailability();
      },
      error: () => {
        this.toastr.error('Failed to remove working hours');
      }
    });
  }

  removeBreak(id: number) {
    this.availabilityService.removeBreak(id).subscribe({
      next: () => {
        this.toastr.success('Break removed');
        this.loadBreaks();
      },
      error: () => {
        this.toastr.error('Failed to remove break');
      }
    });
  }

  removeDayOff(id: number) {
    this.availabilityService.removeDayOff(id).subscribe({
      next: () => {
        this.toastr.success('Day off cancelled');
        this.loadDaysOff();
      },
      error: () => {
        this.toastr.error('Failed to cancel day off');
      }
    });
  }

  getDaySlots(dayIndex: number): any[] {
    return this.availability.filter(slot => slot.dayOfWeek == dayIndex);
  }

  getClinicName(clinicId: number): string {
    const clinic = this.clinics.find(c => c.clinicId === clinicId);
    return clinic ? clinic.clinicName : 'Unknown Clinic';
  }
}