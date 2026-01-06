import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { ToastrService } from 'ngx-toastr';
import { ClinicService, Clinic } from '../../../core/services/clinic.service';

@Component({
  selector: 'app-clinic-management',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatChipsModule
  ],
  templateUrl: './clinic-management.html',
  styleUrl: './clinic-management.scss'
})
export class ClinicManagementComponent implements OnInit {
  dataSource = new MatTableDataSource<Clinic>([]);
  displayedColumns = ['clinicName', 'address', 'city', 'isActive', 'actions'];
  isLoading = false;

  constructor(
    private clinicService: ClinicService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadClinics();
  }

  loadClinics() {
    this.isLoading = true;
    this.clinicService.getClinics().subscribe({
      next: (clinics) => {
        this.dataSource.data = clinics;
        this.isLoading = false;
      },
      error: () => {
        this.toastr.error('Failed to load clinics');
        this.isLoading = false;
      }
    });
  }

  createClinic() {
    this.router.navigate(['/admin/clinics/create']);
  }

  editClinic(clinic: Clinic) {
    this.router.navigate(['/admin/clinics/edit', clinic.clinicId]);
  }

  deleteClinic(clinic: Clinic) {
    if (confirm('Are you sure you want to delete this clinic?')) {
      this.clinicService.deleteClinic(clinic.clinicId).subscribe({
        next: () => {
          this.toastr.success('Clinic deleted successfully');
          this.loadClinics();
        },
        error: () => {
          this.toastr.error('Failed to delete clinic');
        }
      });
    }
  }

  toggleClinicStatus(clinic: Clinic) {
    const newStatus = !clinic.isActive;
    const updateData = {
      clinicName: clinic.clinicName,
      address: clinic.address,
      city: clinic.city,
      state: clinic.state,
      country: clinic.country,
      zipCode: clinic.zipCode,
      isActive: newStatus
    };
    
    this.clinicService.updateClinic(clinic.clinicId, updateData).subscribe({
      next: () => {
        clinic.isActive = newStatus;
        this.toastr.success(`Clinic ${newStatus ? 'activated' : 'deactivated'} successfully`);
      },
      error: () => {
        this.toastr.error('Failed to update clinic status');
      }
    });
  }

  goBack() {
    this.router.navigate(['/dashboard/admin']);
  }

  getClinicStats() {
    const clinics = this.dataSource.data;
    return {
      total: clinics.length,
      active: clinics.filter(c => c.isActive).length,
      inactive: clinics.filter(c => !c.isActive).length
    };
  }
}