import { Component, OnInit } from '@angular/core';

import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ToastrService } from 'ngx-toastr';
import { ClinicService, Clinic } from '../../../core/services/clinic.service';
import { ClinicCreateDto, ClinicUpdateDto } from '../../../core/models/admin.models';

@Component({
  selector: 'app-clinic-form',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
],
  templateUrl: './clinic-form.html',
  styleUrl: './clinic-form.scss'
})
export class ClinicFormComponent implements OnInit {
  clinicForm!: FormGroup;
  clinicId?: number;
  isLoading = false;
  isSubmitting = false;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private clinicService: ClinicService,
    private router: Router,
    private route: ActivatedRoute,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.initForm();
    this.route.params.subscribe(params => {
      if (params['clinicId']) {
        this.clinicId = +params['clinicId'];
        this.isEditMode = true;
        this.loadClinic();
      }
    });
  }

  initForm() {
    this.clinicForm = this.fb.group({
      clinicName: ['', Validators.required],
      address: [''],
      city: [''],
      state: [''],
      country: [''],
      zipCode: ['']
    });
  }

  loadClinic() {
    if (!this.clinicId) return;
    
    this.isLoading = true;
    this.clinicService.getClinic(this.clinicId).subscribe({
      next: (clinic) => {
        this.clinicForm.patchValue(clinic);
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load clinic details');
        this.isLoading = false;
      }
    });
  }

  onSubmit() {
    if (this.clinicForm.invalid) return;

    this.isSubmitting = true;
    const formData = this.clinicForm.value;
    
    if (this.isEditMode) {
      this.clinicService.updateClinic(this.clinicId!, formData as ClinicUpdateDto).subscribe({
        next: () => {
          this.toastr.success('Clinic updated successfully');
          this.goBack();
          this.isSubmitting = false;
        },
        error: () => {
          this.toastr.error('Failed to update clinic');
          this.isSubmitting = false;
        }
      });
    } else {
      this.clinicService.createClinic(formData as ClinicCreateDto).subscribe({
        next: () => {
          this.toastr.success('Clinic created successfully');
          this.goBack();
          this.isSubmitting = false;
        },
        error: () => {
          this.toastr.error('Failed to create clinic');
          this.isSubmitting = false;
        }
      });
    }
  }

  goBack() {
    this.router.navigate(['/admin/clinics']);
  }

  get title() {
    return this.isEditMode ? 'Edit Clinic' : 'Create Clinic';
  }
}