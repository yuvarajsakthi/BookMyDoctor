import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ToastrService } from 'ngx-toastr';
import { AdminService } from '../../../core/services/admin.service';

@Component({
  selector: 'app-doctor-management',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './doctor-management.html',
  styleUrl: './doctor-management.scss'
})
export class DoctorManagementComponent implements OnInit {
  doctors: any[] = [];
  displayedColumns: string[] = ['userName', 'email', 'phone', 'gender'];
  isLoading = false;

  constructor(
    private adminService: AdminService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.loadDoctors();
  }

  loadDoctors() {
    this.isLoading = true;
    this.adminService.getAllUsers().subscribe({
      next: (response) => {
        this.isLoading = false;
        if (response.success) {
          this.doctors = response.data.filter(user => user.userRole === 'Doctor');
        } else {
          this.toastr.error(response.message);
        }
      },
      error: (error) => {
        this.isLoading = false;
        this.toastr.error('Failed to load doctors');
      }
    });
  }

  goBack() {
    this.router.navigate(['/dashboard/admin']);
  }

  getGenderDisplay(gender: any): string {
    if (gender === null || gender === undefined) {
      return 'N/A';
    }
    // Handle both enum number and string values
    if (typeof gender === 'number') {
      const genderMap: { [key: number]: string } = {
        0: 'Male',
        1: 'Female', 
        2: 'Other'
      };
      return genderMap[gender] || 'N/A';
    }
    return gender.toString();
  }
}
